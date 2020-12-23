using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SIKOSI.Crypto.Interfaces;
using SIKOSI.Exchange.Model;

namespace SIKOSI.Sample08_IotServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IotController : ControllerBase
    {
        private readonly ILogger<IotController> _logger;
        private readonly ISecureEncryption _encryption;
        private readonly Encoding encoder = Encoding.UTF8;

        public IotController(ILogger<IotController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("/publickey")]
        public IActionResult GetPublicKey()
        {
            return new FileContentResult(_encryption.PublicKey, "application/octet-stream");
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/postmeasurement")]
        public IActionResult PostMeasurement([FromBody] byte[] bytes)
        {
            if (bytes == null) return BadRequest();

            //decrypt bytes
            var decryptionResult = _encryption.DecryptData(bytes);

            if (decryptionResult.Success)
            {
                // deserialize from JSON-String
                var receivedEncContainer = JsonConvert.DeserializeObject<EncryptionContainer<DistanceMeasurementModel>>(encoder.GetString(decryptionResult.ResultBytes));

                if (receivedEncContainer != null && receivedEncContainer.Data != null)
                {
                    var measurementModel = receivedEncContainer.Data;

                    // do anything with the model

                    _logger.LogInformation($"New measurement value received! Value: {measurementModel.DistanceMM}mm");
                    return Ok();
                }
            }

            return BadRequest();
        }
    }
}
