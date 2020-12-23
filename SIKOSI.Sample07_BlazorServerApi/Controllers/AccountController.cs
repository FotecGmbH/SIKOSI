// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 03.12.2020 13:47
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using SIKOSI.Crypto.Helper;
using SIKOSI.Crypto.Interfaces;
using SIKOSI.Exceptions;
using SIKOSI.Exchange.Interfaces;
using SIKOSI.Exchange.Model;
using SIKOSI.Sample07_EncryptedChat.Hubs;
using SIKOSI.SampleDatabase02.Entities;
using SIKOSI.Services.Auth.Interfaces;

namespace SIKOSI.Sample07_EncryptedChat.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _chatHubContext;

        private readonly Encoding _encoder = Encoding.UTF8;
        private readonly ISecureEncryption _encryption;

        private readonly IUserFileService _userService;

        public AccountController(ISecureEncryption encryption, IUserFileService userService, IHubContext<ChatHub> chatHubContext)
        {
            _encryption = encryption ?? throw new ArgumentNullException(nameof(encryption));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _chatHubContext = chatHubContext ?? throw new ArgumentNullException(nameof(chatHubContext));
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/api/account/encryptedregister")]
        public async Task<IActionResult> EncryptedRegistration([FromBody] byte[] bytes)
        {
            if (bytes == null) return BadRequest();

            try
            {
                //decrypt bytes
                var receivedEncContainer = ModelToEncryptedBytesConverter.ConvertFromEncryptedByteArrayToModel<RegisterModel>(bytes, _encryption, _encoder);

                if (!CheckRegistrationData(receivedEncContainer.Data)) return BadRequest();

                var tempUserData = new User
                                   {
                                       Username = receivedEncContainer.Data.Username,
                                       FirstName = receivedEncContainer.Data.FirstName,
                                       LastName = receivedEncContainer.Data.LastName,
                                       PublicKey = receivedEncContainer.SenderPublicKey
                                   };

                _userService.Create(tempUserData, receivedEncContainer.Data.Password);
            }
            catch (AppException e)
            {
                return Conflict(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest();
            }

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/api/account/encryptedlogin")]
        public async Task<IActionResult> EncryptedAuthenticate([FromBody] byte[] bytes)
        {
            if (bytes is null) return BadRequest();

            try
            {
                //decrypt bytes
                var receivedEncContainer = ModelToEncryptedBytesConverter.ConvertFromEncryptedByteArrayToModel<AuthenticateModel>(bytes, _encryption, _encoder);

                var authModel = receivedEncContainer.Data;

                var user = _userService.Authenticate(authModel.Username, authModel.Password);

                if (user is null) return Unauthorized();

                //update public key of user
                var tblUser = user as TblUser;
                tblUser.PublicKey = receivedEncContainer.SenderPublicKey;
                _userService.Update(tblUser);


                var token = GetToken(user);

                var authUserModel = new AuthUserModel
                                    {
                                        Id = user.Id,
                                        Username = user.Username,
                                        FirstName = user.FirstName,
                                        LastName = user.LastName,
                                        Token = token
                                    };

                // send updated data (especially user's public key) to all via signalR
                var userDataForOthers = new {authUserModel.Id, authUserModel.Username, authUserModel.FirstName, authUserModel.LastName, tblUser.PublicKey};
                await _chatHubContext.Clients.All.SendAsync("NewUserAvailable", userDataForOthers);

                // encrypt response data for user
                var response = ModelToEncryptedBytesConverter.ConvertFromModelToEncryptedByteArray(authUserModel, _encryption, receivedEncContainer.SenderPublicKey, _encoder);

                // send response as byte array
                return new FileContentResult(response, "application/octet-stream");

                // ...or send response as json
                //return Ok(response);
            }
            catch
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("/api/publickey")]
        public IActionResult GetPublicKey()
        {
            // send response as byte array
            return new FileContentResult(_encryption.PublicKey, "application/octet-stream");

            // ...or send response as json
            //return Ok(encryption.PublicKey);
        }

        [Authorize]
        [HttpGet]
        [Route("/api/groupkey")]
        public IActionResult DistributeGroupKey()
        {
            var distributor = _userService.GetAll().FirstOrDefault();

            if (!(distributor is null)) _chatHubContext.Clients.All.SendAsync("DistributeGroupKey", distributor.Id);

            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("/api/allusers/{id}")]
        public IActionResult GetAllUsersEncrypted(int id)
        {
            try
            {
                var user = _userService.GetById(id) as TblUser;

                if (user is null) return NotFound("Id not found");

                var users = _userService.GetAll();

                var data = users.Select<IUser, object>(x =>
                {
                    var user = x as TblUser;

                    if (user is null) return new {x.Id, x.FirstName, x.LastName, x.Username};

                    return new {user.Id, user.FirstName, user.LastName, user.Username, user.PublicKey};
                }).ToArray();

                var response = ModelToEncryptedBytesConverter.ConvertFromModelToEncryptedByteArray(data, _encryption, user.PublicKey, _encoder);

                // send response as byte array
                return new FileContentResult(response, "application/octet-stream");

                // ...or send response as json
                //return Ok(response);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost]
        [Route("/api/postfile/{id}")]
        public IActionResult PostFile(int id, [FromBody] byte[] bytes)
        {
            if (bytes is null) return BadRequest();

            try
            {
                //decrypt bytes
                var receivedEncContainer = ModelToEncryptedBytesConverter.ConvertFromEncryptedByteArrayToModel<ExFile>(bytes, _encryption, _encoder);

                var file = receivedEncContainer.Data;

                //save file for user with specified parameter id (content is encrypted only readable/decryptable with used password!)
                _userService.SaveUserFile(id, file);

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpGet]
        [Route("/api/getfilesmetadata/{userId}")]
        public IActionResult GetFilesMetaData(int userId)
        {
            try
            {
                //get user to send even the metadata of the file encrypted
                var user = _userService.GetById(userId) as TblUser;

                if (user is null) return Unauthorized();

                //get all files of this user
                var files = _userService.GetAllUserFiles(userId);

                if (files is null) return BadRequest();

                //just send metadata of files (not the encrypted content)
                foreach (var file in files) file.Content = null;

                var response = ModelToEncryptedBytesConverter.ConvertFromModelToEncryptedByteArray(files, _encryption, user.PublicKey, _encoder);

                // send encrypted response as byte array
                return new FileContentResult(response, "application/octet-stream");
            }
            catch
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpGet]
        [Route("/api/getfilecontent/{userId}/{fileId}")]
        public IActionResult GetFileContent(int userId, int fileId)
        {
            try
            {
                //get user to send the metadata of the file encrypted
                var user = _userService.GetById(userId) as TblUser;

                if (user is null) return Unauthorized();

                //get specified file
                var file = _userService.GetUserFile(userId, fileId);

                if (file is null) return BadRequest();

                var response = ModelToEncryptedBytesConverter.ConvertFromModelToEncryptedByteArray(file.Content, _encryption, user.PublicKey, _encoder);

                // send encrypted response as byte array
                return new FileContentResult(response, "application/octet-stream");
            }
            catch
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/account/update/{id}")]
        public IActionResult UpdateUserData(int id, [FromBody] byte[] bytes)
        {
            if (bytes is null) return BadRequest();

            //decrypt bytes
            var receivedEncContainer = ModelToEncryptedBytesConverter.ConvertFromEncryptedByteArrayToModel<UserUpdateModel>(bytes, _encryption, _encoder);

            if (receivedEncContainer is null || receivedEncContainer.Data is null) return BadRequest();

            var receivedUser = receivedEncContainer.Data;

            if (receivedUser.Id != id) return BadRequest();

            IUser user = default;

            // if new password should be changed
            if (!string.IsNullOrWhiteSpace(receivedUser.Password))
            {
                // check confirmation of new password
                if (receivedUser.Password != receivedUser.ConfirmPassword) return BadRequest();

                // check old password
                user = _userService.Authenticate(receivedUser.Username, receivedUser.OldPassword);

                if (user is null) return BadRequest();
            }

            // get user if password should not be changed
            if (user is null) user = _userService.GetById(receivedUser.Id);

            // if user is still null - no user with that id registered
            if (user is null) return NotFound("User not found");

            _userService.Update(user, receivedUser.Password);

            return Ok();
        }

        private bool CheckRegistrationData(RegisterModel registrationModel)
        {
            return
                !string.IsNullOrWhiteSpace(registrationModel.Username) &&
                !string.IsNullOrWhiteSpace(registrationModel.FirstName) &&
                !string.IsNullOrWhiteSpace(registrationModel.LastName) &&
                !string.IsNullOrWhiteSpace(registrationModel.Password) &&
                registrationModel.Password == registrationModel.ConfirmPassword;
        }


        /// <summary>
        ///     Tries to retrieve a byte array out of the body.
        ///     Returns null if it fails to retrieve a byte array.
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

        private string GetToken(IUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("TheS3cr3Tt0kenItIs!REPLACE!!:-)");
            var tokenDescriptor = new SecurityTokenDescriptor
                                  {
                                      Subject = new ClaimsIdentity(new[]
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

            if (!StringValues.IsNullOrEmpty(stringValue)) return stringValue.ToString();

            return null;
        }
    }
}