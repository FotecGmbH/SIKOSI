// (C) 2019 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created:        16.09.2020 14:18
// Developer:      Gregor Faiman
// Project         SIKOSI
//
// Released under GPL-3.0-only

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using MFA_QR_CODE.Data;
using MFA_QR_CODE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MFA_QR_CODE.Controllers
{
    /// <summary>
    /// Controller der für Apples Authentifizierung verwendet wird.
    /// </summary>
    public class AuthenticationController : Controller
    {
        private SignInManager<IdentityUser> signInManager;
        private MFA_QR_CODEContext context;

        public AuthenticationController(MFA_QR_CODEContext context, SignInManager<IdentityUser> signInManager)
        {
            this.context = context;
            this.signInManager = signInManager;
        }
        
        /// <summary>
        /// Diese Methode stellt ein Callback dar, an welches der Apple Server das Ergebnis einer Authentifizierungsanfrage schickt.
        /// </summary>
        /// <param name="response">Die Antwort des Apple Servers.</param>
        /// <returns>Ein Result das angibt ob die Authentifizierung erfolgreich war.</returns>
        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        [Route("Authentication/AppleReturn")]
        public async Task<IActionResult> AppleReturn([FromForm]AppleRequestModel response)
        {
            IActionResult result;

            if (response == null)
                return BadRequest();

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(response.id_token);

            if (!await this.ValidateTokenOrigin(token.RawData, token.Header.Kid))
                return BadRequest("Invalid Json Web Token");

            if (response.user == null && response.error == null)
                result = await this.HandleAuthentication(token);
            else if (response.user != null && response.error == null)
                result = await this.HandleRegistration(token);
            else
                result = BadRequest();

            return result;
        }

        // WIrd vermutlich nicht benötigt.
        //[Route("Authentication/AppleSignIn")]
        //public IActionResult AppleSignIn(string state, string nonce)
        //{
        //    var appleSignInModel = new AppleSignInModel(state, nonce);
        //    return Redirect(string.Format("https://appleid.apple.com/auth/authorize?client_id=at.fotec.sikosi&redirect_uri=https%3A%2F%2Fa1cacea24ab0.ngrok.io%2FSignIn%2FAppleReturn&response_type=code%20id_token&state={0}&scope=name%20email&nonce={1}&response_mode=form_post&frame_id=d2d007e7-25d6-4604-803e-1fd4bcbcdb06&m=22&v=1.5.3", state, nonce));
        //}

        [Route("Authentication/AccessDenied")]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        /// <summary>
        /// Behandelt den Fall in dem ein User den Apple Login benutzt, um sich mit einem neuen Konto bei der Applikation zu registrieren.
        /// </summary>
        /// <param name="token">Der von Apple gesendete Web Token.</param>
        /// <returns>Ein redirect Objekt zur Startseite der Applikation.</returns>
        private async Task<IActionResult> HandleRegistration(JwtSecurityToken token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token), "Json Web Token must not be null..");

            IdentityUser user = new IdentityUser() { Email = token.Payload["email"].ToString(), EmailConfirmed = true, Id = token.Subject, UserName = token.Payload["email"].ToString() };
            this.context.Users.Add(user);
            await this.context.SaveChangesAsync();

            return await this.HandleAuthentication(token);
        }

        /// <summary>
        /// Behandelt den Fall in dem ein User den Apple Login benutzt, um sich mit einem bereits bestehenden Konto zu authentifizieren.
        /// </summary>
        /// <param name="token">Der von Apple gesendete Web Token.</param>
        /// <returns>Ein redirect Objekt zur Startseite der Applikation.</returns>
        private async Task<IActionResult> HandleAuthentication(JwtSecurityToken token)
        {
            IdentityUser user = this.context.Users.Where(p => p.Id == token.Subject).FirstOrDefault();

            if (user == null)
                throw new ArgumentException(nameof(token), "User associated with this token did not exist");

            await this.signInManager.SignInAsync(user, false);
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Validates that a received web token was indeed received by apple.
        /// </summary>
        /// <param name="token">The received token string.</param>
        /// <param name="keyIdentifier">The key identifier.</param>
        /// <returns>Whether the token was signed by apple.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if token or key id are null.
        /// </exception>
        private async Task<bool> ValidateTokenOrigin(string token, string keyIdentifier)
        {
            if (token == null || keyIdentifier == null)
                throw new ArgumentNullException("Either token or key id were null.");

            string result;
            JsonWebKeySet jwks;
            SecurityToken validatedToken;

            using (var httpClient = new HttpClient())
            {
                result = await httpClient.GetStringAsync("https://appleid.apple.com/auth/keys");
                jwks = new JsonWebKeySet(result);
            }

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            foreach (var item in jwks.Keys)
            {
                if (item.KeyId != keyIdentifier)
                    continue;

                rsa.ImportParameters(new RSAParameters()
                {
                    Modulus = FromBase64Url(item.N),
                    Exponent = FromBase64Url(item.E)
                });

                break;
            }

            var validationParameters = this.GetTokenValidationParameters();
            var handler = new JwtSecurityTokenHandler();
            validationParameters.IssuerSigningKey = new RsaSecurityKey(rsa);

            try
            {
                handler.ValidateToken(token, validationParameters, out validatedToken);
                rsa.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Hilfsmethode um eine base64 enkodierte URL in ein byte array umzuwandeln.
        /// </summary>
        /// <param name="base64Url">The Url die umgewandelt werden muss.</param>
        /// <returns>Das konvertierte byte array.</returns>
        private byte[] FromBase64Url(string base64Url)
        {
            string padded = base64Url.Length % 4 == 0
                ? base64Url : base64Url + "====".Substring(base64Url.Length % 4);
            string base64 = padded.Replace("_", "/")
                                  .Replace("-", "+");
            return Convert.FromBase64String(base64);
        }

        /// <summary>
        /// Generates token validation parameters.
        /// </summary>
        /// <returns>The token validation parameters.</returns>
        private TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidIssuer = "https://appleid.apple.com",
                ValidAudience = "at.fotec.sikosi",
            };
        }
    }
}
