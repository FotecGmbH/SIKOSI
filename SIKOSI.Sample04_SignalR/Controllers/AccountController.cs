using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using Microsoft.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Extensions.Primitives;
using SIKOSI.Exchange.Model;
using System.Security.Cryptography;
using Newtonsoft.Json.Serialization;
using SIKOSI.Sample04_SignalR.Services;
using SIKOSI.Sample04_SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using SIKOSI.Crypto.Interfaces;

namespace SIKOSI.Sample04_SignalR.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private ISecureEncryption encryption;

        private Encoding encoder = Encoding.UTF8;

        private UserService userService;

        private IHubContext<ChatHub> chatHubContext;

        public AccountController(ISecureEncryption encryption, UserService userService, IHubContext<ChatHub> chatHubContext)
        {
            this.encryption = encryption;
            this.userService = userService;
            this.chatHubContext = chatHubContext;

            Initialize();
        }

        private void Initialize()
        {
            this.userService.UserAdded += async (sender, userArg) =>
            {
                // inform users about new user registrated
                await chatHubContext.Clients.All.SendAsync("NewUserAvailable", userArg.User);
            };

            //this.userService.UserDeleted += async (sender, userArg) =>
            //{
            //    // inform users
            //    await chatHubContext.Clients.All.SendAsync("UserDeleted", userArg.User);
            //};
        }

        /// <summary>
        /// Authentication
        /// </summary>
        /// <param name="model">Credentials</param>
        /// <returns>Authenticated User</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("/api/account/login")]
        public IActionResult Authenticate([FromBody] AuthenticateModel model)
        {
            AuthUserModel user = null;

            //var user = _userService.Authenticate(model.Username, model.Password);
            if (model.Username == "test" && model.Password == "tset")
            {
                user = new AuthUserModel()
                       {
                           Username = "fosch",
                           FirstName = "Manuel",
                           LastName = "Fasching",
                           Id = 1
                       };
            }

            if (user == null)
            {
                return BadRequest(new { message = "Failed to authenticate. Please check username and password." });
            }

            var tokenAsString = GetToken(new User()
                                         {
                                             Id = user.Id, 
                                             FirstName = user.FirstName, 
                                             LastName = user.LastName, 
                                             Username = user.Username
                                         });

            user.Token = tokenAsString;

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/api/account/encryptedregister")]
        public async Task<IActionResult> EncryptedRegistration([FromBody] byte[] bytes)
        {
            if (bytes == null || bytes == null) return BadRequest();

            //decrypt bytes
            var decryptionResult = encryption.DecryptData(bytes);

            if (decryptionResult.Success)
            {
                // deserialize from JSON-String
                var receivedEncContainer = JsonConvert.DeserializeObject<EncryptionContainer<RegisterModel>>(encoder.GetString(decryptionResult.ResultBytes));

                if (receivedEncContainer != null && receivedEncContainer.Data != null)
                {
                    var regModel = receivedEncContainer.Data;

                    if (userService.SameUserNameExists(regModel.Username))
                    {
                        return Conflict("Username already exists!");
                    }

                    //hash password
                    using SHA256 sha256 = SHA256.Create();

                    var pwHash = sha256.ComputeHash(encoder.GetBytes(regModel.Password));
                    
                    // if any user already exists -> new id of max-id + 1, or 1 if no user exists (first user)
                    int newID = userService.GetNewId();

                    User newUser = new User
                    {
                        Id = newID,
                        FirstName = regModel.FirstName,
                        LastName = regModel.LastName,
                        Username = regModel.Username,
                        PasswordHash = pwHash,
                        PublicKey = receivedEncContainer.SenderPublicKey
                    };

                    // add user and its public key to "database"
                    userService.AddUser(newUser);

                    return Ok();
                }
            }

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/api/account/encryptedlogin")]
        public async Task<IActionResult> EncryptedAuthenticate([FromBody] byte[] bytes)
        {
            if (bytes == null) return BadRequest();

            //decrypt bytes
            var decryptionResult = encryption.DecryptData(bytes);

            if (decryptionResult.Success)
            {
                // deserialize from JSON-String
                var receivedEncContainer = JsonConvert.DeserializeObject<EncryptionContainer<AuthenticateModel>>(encoder.GetString(decryptionResult.ResultBytes));
                    
                if (receivedEncContainer != null && receivedEncContainer.Data != null)
                {
                    var authModel = receivedEncContainer.Data;
                    var user = userService.GetUser(authModel);

                    if (user == null) return Unauthorized();

                    var token = GetToken(user);

                    //if (!usersPublicKey.Keys.Any(x => x.Id == user.Id))
                    //{
                    //    usersPublicKey.Add(user, new byte[0]);
                    //}

                    var authUserModel = new AuthUserModel
                    {
                        Id = user.Id,
                        Username = user.Username,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Token = token
                    };

                    var sendEncContainer = new EncryptionContainer<AuthUserModel> 
                    { 
                        Data = authUserModel, 
                        SenderPublicKey = encryption.PublicKey 
                    };

                    var jsonString = JsonConvert.SerializeObject(sendEncContainer);

                    // encrypt
                    var encResult = encryption.EncryptData(receivedEncContainer.SenderPublicKey, encoder.GetBytes(jsonString));

                    // send response as byte array
                    return new FileContentResult(encResult.ResultBytes, "application/octet-stream");
                    
                    // ...or send response as json
                    //return Ok(encResult.ResultBytes);
                }
            }

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("/api/publickey")]
        public IActionResult GetPublicKey()
        {
            // send response as byte array
            return new FileContentResult(encryption.PublicKey, "application/octet-stream");
                    
            // ...or send response as json
            //return Ok(encryption.PublicKey);
        }

        [Authorize]
        [HttpGet]
        [Route("/api/groupkey")]
        public IActionResult DistributeGroupKey()
        {
            var distributor = userService.GetAllUsers().FirstOrDefault();

            if (distributor != null)
            {
                chatHubContext.Clients.All.SendAsync("DistributeGroupKey", distributor.Id);
            }
            
            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("/api/allusers/{id}")]
        public IActionResult GetAllUsersEncrypted(int id)
        {
            var users = userService.GetAllUsers().ToArray();

            var sendEncContainer = new EncryptionContainer<object> 
            { 
                // don't send passwordhash and salt
                Data = users.Select(x => new { x.Id, x.FirstName, x.LastName, x.Username, x.PublicKey }).ToArray(),
                SenderPublicKey = encryption.PublicKey 
            };

            var jsonString = JsonConvert.SerializeObject(sendEncContainer);

            var user = users.FirstOrDefault(x => x.Id == id);

            if (user == null) return NotFound("Id not found");

            // encrypt
            var encResult = encryption.EncryptData(user.PublicKey, encoder.GetBytes(jsonString));

            // send response as byte array
            return new FileContentResult(encResult.ResultBytes, "application/octet-stream");
                    
            // ...or send response as json
            //return Ok(encResult.ResultBytes);
        }

        [Authorize]
        [HttpPost]
        [Route("/api/postfile/{id}")]
        public IActionResult PostFile(int id, [FromBody] byte[] bytes)
        {
            if (bytes == null) return BadRequest();

            //decrypt bytes
            var decryptionResult = encryption.DecryptData(bytes);

            if (decryptionResult.Success)
            {
                // deserialize from JSON-String
                var receivedEncContainer = JsonConvert.DeserializeObject<EncryptionContainer<ExFile>>(encoder.GetString(decryptionResult.ResultBytes));

                if (receivedEncContainer != null && receivedEncContainer.Data != null)
                {
                    var file = receivedEncContainer.Data;

                    //Todo: save file for user with specified parameter id (content is encrypted only readable/decryptable with used password!)


                    return Ok();
                }
            }

            return BadRequest();
        }

        [Authorize]
        [HttpPost]
        [Route("api/account/update/{id}")]
        public IActionResult UpdateUserData(int id, [FromBody] byte[] bytes)
        {
            if (bytes == null) return BadRequest();

            //decrypt bytes
            var decryptionResult = encryption.DecryptData(bytes);

            if (!decryptionResult.Success) return BadRequest();
            
            // deserialize from JSON-String
            var receivedEncContainer = JsonConvert.DeserializeObject<EncryptionContainer<AuthUserModel>>(encoder.GetString(decryptionResult.ResultBytes));

            if (receivedEncContainer == null || receivedEncContainer.Data == null) return BadRequest();
            
            var updatedUser = receivedEncContainer.Data;
                    
            if (updatedUser.Id != id) return BadRequest();

            if (!userService.TryUpdateUserData(updatedUser)) return BadRequest();

            return Ok();
        }

        /// <summary>
        /// Tries to retrieve a byte array out of the body.
        /// Returns null if it fails to retrieve a byte array.
        /// </summary>
        /// <returns>The retrieved byte array sent in the body, null if no byte array can be retrieved.</returns>
        private async Task<byte[]> GetBytesFromBody()
        {
            try
            {
                await using var ms = new MemoryStream();

                await Request.Body.CopyToAsync(ms);

                return ms.ToArray();
            }
            catch
            {
                return null;
            }
        }

        private string GetToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("TheS3cr3Tt0kenItIs!REPLACE!!:-)");
            var tokenDescriptor = new SecurityTokenDescriptor
                                  {
                                      Subject = new ClaimsIdentity(new Claim[]
                                                                   {
                                                                       new Claim(ClaimTypes.Name, user.Id.ToString()),
                                                                       new Claim(ClaimTypes.Email, "fasching@fotec.at"), 
                                                                   }),
                                      Expires = DateTime.UtcNow.AddDays(1), //TODO in Settings auslagern
                                      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                                  };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenAsString = tokenHandler.WriteToken(token);

            return tokenAsString;
        }

        private string GetAuthorizationHeader()
        {
            Request.Headers.TryGetValue("Authorization", out StringValues stringValue);

            if (!StringValues.IsNullOrEmpty(stringValue))
            {
                return stringValue.ToString();
            }

            return null;            
        }
    }
}
