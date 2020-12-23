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
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRP_SDK;
using SRPShared;
using SRPServerAPI.Model;
using SecurityDriven.Inferno;

namespace SRPServerAPI.Controllers
{
    [ApiController]
    public class SRPProtocolController : ControllerBase
    {
        private static SrpServer srpServer;
        private static DataCache dataCache;
        private UserContext context;

        public SRPProtocolController(UserContext context)
        {
            this.context = context;
        }

        static SRPProtocolController()
        {
            srpServer = new SrpServer(new SRPGroup());
            dataCache = new DataCache();
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
            ApplicationUser user;

            if (userName == null)
            {
                return BadRequest("Username was null");
            }

            user = await this.context.Users.Where(u => u.Username == userName).FirstOrDefaultAsync();

            if (user != null)
                salt = user.Salt;
            else
                salt = srpServer.GenerateSalt(userName);

            return Content(Convert.ToBase64String(salt));
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
            if (model == null || !ModelState.IsValid)
                return BadRequest("Invalid Model");

            if (await this.context.Users.Where(u => u.Username == model.Username).FirstOrDefaultAsync() != null)
                return BadRequest("User already exists");

            this.context.Users.Add(new ApplicationUser(model.Username, model.VerifierBytes, model.SaltBytes));
            this.context.SaveChanges();

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
            var user = this.context.Users.Where(u => u.Username == model.Username).FirstOrDefault();

            if (user == null)
                return BadRequest(model.Username);

            byte[] publicServerValue;
            BigInteger privateServerValue;

            // The SRP SDK implementation itself only implements the calculation logic required to calculate the session keys, verifier and
            // other required values for the protocol to work.
            // It does nothing to persist values that have already been calculated.
            // Doing so is the responsibility of the progammer integrating the SDK into their project.
            dataCache.TryCreateUserRecord(user.Verifier.ToBigInteger(), user.Username, model.ClientValue);

            publicServerValue = srpServer.GenerateBValues(user.Verifier.ToBigInteger(), out privateServerValue).ToByteArray();

            dataCache.StoreServerValues(user.Verifier.ToBigInteger(), privateServerValue.ToByteArray(), publicServerValue);

            return Content(Convert.ToBase64String(publicServerValue));
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

            var user = await this.context.Users.Where(u => u.Username == model.Username).FirstOrDefaultAsync();

            byte[] sessionKey;
            byte[] serverProof;

            BigInteger verifier;
            BigInteger expectedClientProof;
            BigInteger clientPublicValue;
            BigInteger serverPrivateValue;
            BigInteger serverPublicValue;

            if (user == null)
                return BadRequest(model.Username);

            verifier = user.Verifier.ToBigInteger();

            try
            {
                clientPublicValue = dataCache.ExtractClientValueA(verifier).ToBigInteger();
                serverPrivateValue = dataCache.RetrieveServerPrivateValue(verifier).ToBigInteger();
                serverPublicValue = dataCache.RetrieveServerPublicValue(verifier).ToBigInteger();
            }
            catch (Exception)
            {
                return BadRequest("User record has not yet been created.");
            }

            sessionKey = srpServer.ComputeSessionKey(verifier, clientPublicValue, serverPrivateValue, serverPublicValue).ToByteArray();

            expectedClientProof = srpServer.CalculateExpectedClientProof(sessionKey, dataCache.ExtractClientValueA(verifier), dataCache.RetrieveServerPublicValue(verifier));

            if (expectedClientProof != model.ClientProof.ToBigInteger())
                return BadRequest();

            serverProof = srpServer.CalculateServerProof(sessionKey, model.ClientProof, dataCache.ExtractClientValueA(verifier));
            dataCache.StoreSessionKey(verifier, sessionKey);

            return Content(Convert.ToBase64String(serverProof));
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
            var user = await this.context.Users.Where(u => u.Username == messageModel.Username).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("User could not be found, therefore an answer could not be created");

            var sessionKey = dataCache.ExtractSessionKey(user.Verifier.ToBigInteger());
            var decryptedMessage = SuiteB.Decrypt(sessionKey, messageModel.EncryptedMessage.AsArraySegment());

            var splitMessageArray = Encoding.UTF8.GetString(decryptedMessage).Split(':');

            if (splitMessageArray.Length > 1)
            {
                var serverMessage = Encoding.UTF8.GetBytes(splitMessageArray[1]);
                var serverAnswer = SuiteB.Encrypt(sessionKey, serverMessage.AsArraySegment());

                return Content(Convert.ToBase64String(serverAnswer));
            }

            return BadRequest();
        }
    }
}
