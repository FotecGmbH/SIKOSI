// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 08.07.2020 15:04
// Entwickler      Gregor Faiman
// Projekt         SIKOSI
namespace SIKOSI.Sample02_SRP.Controllers
{
    using SRP_SDK;
    using SRPShared;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Numerics;
    using System.Threading.Tasks;
    using SIKOSI.Sample02_SRP.Services;

    /// <summary>
    /// This class represents the controller handling SRP requests.
    /// </summary>
    [ApiController]
    public class SrpProtocolController : ControllerBase
    {
        /// <summary>
        /// Service for SRP related actions.
        /// </summary>
        private ISRPProtocolService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="SrpProtocolController"/> class.
        /// </summary>
        /// <param name="service">The SRP protocol service.</param>
        public SrpProtocolController(ISRPProtocolService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Action that gets invoked when the user requests the salt for a given user name.
        /// </summary>
        /// <param name="userName">The specified user name.</param>
        /// <returns>The salt for this particular user if successful. Otherwise a bad request result.</returns>
        [HttpGet]
        [Route("/api/user/getsalt/{userName}")]
        public async Task<IActionResult> GetSaltAsync(string userName)
        {
            byte[] salt;

            if (userName == null)
            {
                return BadRequest("Username was null");
            }

            salt = await this.service.GetSalt(userName);

            return Content(Convert.ToBase64String(salt));
        }

        /// <summary>
        /// This method posts the client generated proof to the server in order to validate that
        /// both client and server have got the correct key.
        /// </summary>
        /// <param name="model">The SRP proof model.</param>
        /// <returns>The server proof, however only if the client proof was able to be verified.</returns>
        [HttpPost]
        [Route("/api/user/proof/postproof")]
        public async Task<IActionResult> GenerateProofAsync(SrpProofModel model)
        {
            if (model.ClientProof == null)
                return BadRequest(model);

            byte[] sessionKey;
            byte[] serverProof;

            bool isValidProof;

            BigInteger verifier;

            var user = await this.service.GetSpecificUser(model.Username);

            verifier = user.Verifier.ToBigInteger();
            sessionKey = this.service.GenerateSessionKey(verifier);
            isValidProof = this.service.ValidateClientProof(verifier, sessionKey, model.ClientProof);

            if (!isValidProof)
                return BadRequest();

            serverProof = this.service.CalculateServerProof(verifier, sessionKey, model.ClientProof);

            return Content(Convert.ToBase64String(serverProof));
        }

        /// <summary>
        /// Completes registration for a given user, persisting a user record in the database.
        /// </summary>
        /// <param name="model">The model containing registration information.</param>
        /// <returns>An okay result.</returns>
        [HttpPost]
        [Route("/api/user/registration/")]
        public async Task<IActionResult> CompleteRegistrationAsync(SrpRegistrationModel model)
        {
            var exists = await this.service.Exists(model.Username);

            if (exists)
                return BadRequest("A user with the specified user name already exists.");

            await this.service.CompleteRegistration(model.Username, model.VerifierBytes, model.SaltBytes);

            return Ok();
        }

        /// <summary>
        /// Posts the client login information containing username and generated client public value to the server.
        /// On receiving the values, the server calculates its own pair of ephemeral values, and proceeds to return the public value back to the client.
        /// </summary>
        /// <param name="model">Login model containing client generated value A, as well as username.</param>
        /// <returns>The public server value.</returns>
        [HttpPost]
        [Route("/api/user/login/postvalue")]
        public async Task<IActionResult> PostClientGeneratedValue(SrpAuthenticationModel model)
        {
            var exists = await this.service.Exists(model.Username);

            if (!exists)
                return BadRequest($"User with specified username {model.Username} does not exist.");

            var user = await this.service.GetSpecificUser(model.Username);

            byte[] publicServerValue;

            // The SRP SDK implementation itself only implements the calculation logic required to calculate the session keys, verifier and
            // other required values for the protocol to work.
            // It does nothing to persist values that have already been calculated.
            // Doing so is the responsibility of the progammer integrating the SDK into their project.
            this.service.CreateUserRecordInMemory(user.Verifier.ToBigInteger(), user.Username, model.ClientValue);
            publicServerValue = this.service.GenerateServerProtocolValues(user.Verifier.ToBigInteger());

            return Content(Convert.ToBase64String(publicServerValue));
        }

        /// <summary>
        /// Posts an encrypted message from the client to the server.
        /// This method is used for testing and makes the server accept input in the format xxx:xxxx
        /// and sends back anything that comes after the colon.
        /// </summary>
        /// <param name="messageModel">The message model.</param>
        /// <returns>The server response.</returns>
        [HttpPost]
        [Route("api/user/postmessage")]
        public async Task<IActionResult> GetAnswer(SrpMessageModel messageModel)
        {
            if (messageModel == null)
                throw new ArgumentNullException(nameof(messageModel), "Message model must not be null.");

            try
            {
                var reply = await this.service.GetMessageReply(messageModel.Username, messageModel.EncryptedMessage);
                return Content(Convert.ToBase64String(reply));
            }
            catch (FormatException)
            {
                return BadRequest("Invalid input format. Expecting message string to contain a colon (:) so the server can send back whatever comes after the colon.");
            }
        }
    }
}
