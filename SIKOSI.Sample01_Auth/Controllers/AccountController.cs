// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 11.05.2020 14:45
// Developer      Manuel Fasching
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using SIKOSI.Exchange.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SIKOSI.Exceptions;
using SIKOSI.Sample01_Auth.Helpers;
using SIKOSI.Services.Auth.Interfaces;

namespace SIKOSI.Sample01_Auth.Controllers
{
    /// <summary>
    ///     Account
    /// </summary>
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        /// <summary>
        ///     Constructor
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
        ///     Authentication
        /// </summary>
        /// <param name="model">Credentials</param>
        /// <returns>Authenticated User</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("/api/account/login")]
        public IActionResult Authenticate([FromBody] AuthenticateModel model)
        {
            var user = _userService.Authenticate(model.Username, model.Password);

            if (user == null) return BadRequest(new {message = "Failed to authenticate. Please check username and password."});

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
                                  {
                                      Subject = new ClaimsIdentity(new[]
                                                                   {
                                                                       new Claim(ClaimTypes.Name, user.Id.ToString()),
                                                                       new Claim(ClaimTypes.Role, user.Role), 
                                                                   }),
                                      Expires = DateTime.UtcNow.AddMinutes(_appSettings.TokenExpirationTimeout),
                                      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                                  };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenAsString = tokenHandler.WriteToken(token);

            var res = new AuthUserModel
                      {
                          Id = user.Id,
                          Username = user.Username,
                          FirstName = user.FirstName,
                          LastName = user.LastName,
                          Token = tokenAsString
                      };

            return Ok(res);
        }


        /// <summary>
        ///     Logout
        /// </summary>
        /// <returns>true if you succeeded</returns>
        [HttpGet]
        [Route("/api/account/logout")]
        public bool Logout()
        {
            //Nothing to be done here
            //Token have to be deleted on client side
            //Token has an expiration time, so it will automatically be invalid
            //You can either keep the token expiry times short and use the refresh token to get a new token
            //Other possibility is to create a token blocklist and store the token at logout

            return true;
        }

        /// <summary>
        ///     Manage
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/account/manage")]
        [Authorize]
        public IActionResult Manage()
        {
            ClaimsIdentity identity = null;
            identity = HttpContext.User.Identity as ClaimsIdentity;

            var user = _userService.GetById(int.Parse(identity.Name));

            return Ok(user);
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="model">User</param>
        /// <returns>status of success</returns>
        [AllowAnonymous]
        [HttpPost("api/account/register")]
        public IActionResult Register([FromBody] RegisterModel model)
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
                return BadRequest(new {message = ex.Message});
            }
        }

        /// <summary>
        ///     Gets the IpAdress from the requesting Client
        /// </summary>
        /// <returns>IP Adress</returns>
        private string GetIpAdress()
        {
            if (Request.Headers.ContainsKey("X-Forwareded-For"))
                return Request.Headers["X-Forwareded-For"];
            return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}