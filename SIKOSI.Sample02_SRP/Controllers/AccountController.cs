using AutoMapper;
using SIKOSI.Exchange.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SIKOSI.Services.Auth.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SIKOSI.Exceptions;
using SIKOSI.Sample02_SRP.Helpers;

namespace SIKOSI.Sample02_SRP.Controllers
{
    /// <summary>
    /// Account
    /// </summary>
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IUserService _userService;
        private readonly AppSettings _appSettings;
        private IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService">User Service</param>
        /// <param name="appSettings">Application Settings</param>
        /// <param name="mapper">Mapper</param>
        public AccountController(IUserService userService, IOptions<AppSettings> appSettings, IMapper mapper)
        {
            _userService = userService;
            _appSettings = appSettings.Value;
            _mapper = mapper;
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
            var user = _userService.Authenticate(model.Username, model.Password);

            if (user == null)
            {
                return BadRequest(new {message = "Failed to authenticate. Please check username and password."});
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
                                  {
                                      Subject = new ClaimsIdentity(new Claim[]
                                                                   {
                                                                       new Claim(ClaimTypes.Name, user.Id.ToString())
                                                                   }),
                                      Expires = DateTime.UtcNow.AddDays(1), //TODO in Settings auslagern
                                      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                                  };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenAsString = tokenHandler.WriteToken(token);

            var res = new AuthUserModel()
            {
                      Id        = user.Id,
                      Username  = user.Username,
                      FirstName = user.FirstName,
                      LastName  = user.LastName,
                      Token     = tokenAsString
            };

            return Ok(res);
        }


        /// <summary>
        /// Logout
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/account/logout")]
        public async Task<bool> Logout()
        {
            //Logout??

            return true;
        }

        /// <summary>
        /// Manage
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/account/manage")]
        public async Task<bool> Manage()
        {
            return true;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]RegisterModel model)
        {
            // map model to entity
            var user = _mapper.Map<User>(model);

            try
            {
                // create user
                _userService.Create(user, model.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Gets the IpAdress from the requesting Client
        /// </summary>
        /// <returns>IP Adress</returns>
        private string GetIpAdress()
        {
            if (Request.Headers.ContainsKey("X-Forwareded-For"))
                return Request.Headers["X-Forwareded-For"];
            else
            {
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
        }

    }
}
