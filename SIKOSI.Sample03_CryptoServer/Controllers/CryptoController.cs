// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created:        19.05.2020 15:22
// Developer:      Roman Jahn
// Project         SIKOSI
//
// Released under GPL-3.0-only

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SIKOSI.Crypto;
using SIKOSI.Crypto.Interfaces;

namespace SIKOSI.Sample03_CryptoServer.Controllers
{
    /// <summary>
    /// A controller to handle encrypted data.
    /// The data can be encrypted using RSA or Diffie-Hellman key exchange as asymmetric encryption and AES-GCM for symmetric encryption.
    /// </summary>
    [ApiController]
    [AutoValidateAntiforgeryToken]
    public class CryptoController : ControllerBase
    {
        /// <summary>
        /// An object for secure encryption using Diffie-Hellman key exchange and AES-GCM.
        /// </summary>
        private static readonly ISecureEncryption SecureEncryptionDh = new SecureEncryptionDh();

        /// <summary>
        /// An object for secure encryption using RSA and AES-GCM.
        /// </summary>
        private static readonly ISecureEncryption SecureEncryptionRsa = new SecureEncryptionRsa();
        
        /// <summary>
        /// A handler for data that was sent encrypted using Diffie-Hellman key exchange and AES-GCM.
        /// </summary>
        /// <returns>An IActionResult object that indicates whether the server was able to extract, decrypt and authenticate the data.</returns>
        [HttpPost]
        [Route("api/crypto/dh/postmessage")]
        public async Task<IActionResult> PostMessageDh()
        {
            var content = await GetBytesFromBody();

            if (content == null || content.Length == 0) return BadRequest();

            var decrypted = SecureEncryptionDh.DecryptData(content);

            if (decrypted.Success)
            {
                // Do something with the decrypted message

                return NoContent();
            }

            return BadRequest();
        }

        /// <summary>
        /// A handler for data that was sent encrypted using RSA and AES-GCM.
        /// </summary>
        /// <returns>An IActionResult object that indicates whether the server was able to extract, decrypt and authenticate the data.</returns>
        [HttpPost]
        [Route("api/crypto/rsa/postmessage")]
        public async Task<IActionResult> PostMessageRsa()
        {
            var content = await GetBytesFromBody();

            if (content == null || content.Length == 0) return BadRequest();

            var decrypted = SecureEncryptionRsa.DecryptData(content);

            if (decrypted.Success)
            {
                // Do something with the decrypted message

                return NoContent();
            }

            return BadRequest();
        }

        /// <summary>
        /// A handler to get the public key for the Diffie-Hellman key exchange.
        /// </summary>
        /// <returns>The public key for the Diffie-Hellman key exchange.</returns>
        [HttpGet]
        [Route("api/crypto/dh/publickey")]
        public async Task<IActionResult> GetPublicKeyDh()
        {
            return new FileContentResult(SecureEncryptionDh.PublicKey, "application/octet-stream");
        }

        /// <summary>
        /// A handler to get the public key for the RSA encryption.
        /// </summary>
        /// <returns>The public key for the RSA encryption.</returns>
        [HttpGet]
        [Route("api/crypto/rsa/publickey")]
        public async Task<IActionResult> GetPublicKeyRsa()
        {
            return new FileContentResult(SecureEncryptionRsa.PublicKey, "application/octet-stream");
        }

        /// <summary>
        /// Tries to retrieve a byte array out of the body.
        /// Returns null if it fails to retrieve a byte array.
        /// </summary>
        /// <returns>The retrieve byte array sent in the body, null if no byte array can be retrieved.</returns>
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
    }
}