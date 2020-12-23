using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SIKOSI.Sample05_IOT.Services.Interfaces;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using SIKOSI.SecureServices;
using SIKOSI.Crypto.Interfaces;
using SIKOSI.Exchange.Model;

namespace SIKOSI.Sample05_IOT
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ISensorHelper _sensorHelper;
        private readonly HttpClient _http;
        private UdpClient _udpClient;
        private readonly ISecureEncryption _encryption;
        private readonly EncryptedCommunicationService _encryptedCommunication;
        private readonly IHttpHandler _httpHandler;

        public Worker(ILogger<Worker> logger, ISensorHelper sensorHelper, HttpClient http, ISecureEncryption encryption, EncryptedCommunicationService encryptedCommunication, IHttpHandler httpHandler)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sensorHelper = sensorHelper ?? throw new ArgumentNullException(nameof(sensorHelper));
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _encryption = encryption ?? throw new ArgumentNullException(nameof(encryption));
            _encryptedCommunication = encryptedCommunication ?? throw new ArgumentNullException(nameof(encryptedCommunication));
            _httpHandler = httpHandler ?? throw new ArgumentNullException(nameof(httpHandler));
            _encryptedCommunication.Encryption = encryption;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var serverPublicKey = await _http.GetByteArrayAsync("/publickey");
            var apiRoute = "/postmeasurement";
            _httpHandler.Start(stoppingToken);
            _udpClient = new UdpClient(new IPEndPoint(IPAddress.Loopback, 5555));

            _encryptedCommunication.ReceiverPublicKey = serverPublicKey;

            while (!stoppingToken.IsCancellationRequested)
            {
                var distance = _sensorHelper.GetDistance();
                byte[] buffer = Encoding.ASCII.GetBytes(distance.ToString());
                await _udpClient.SendAsync(buffer, buffer.Length, new IPEndPoint(IPAddress.Loopback, 6666));
                _logger.LogInformation($"Measured {distance}mm");
                var measurementModel = new IotMeasurementModel { DistanceMM = distance, HumPercent = _httpHandler.LatestHum, TempC = _httpHandler.LatestTemp};
                

                // don't care about result - just send
                _logger.LogDebug("Sending...");
                var response = await _encryptedCommunication.TryEncryptedJsonPostWithoutResultModel(measurementModel, _http, apiRoute);

                _logger.LogInformation($"Sent measurement to {apiRoute}");

                await Task.Delay(1000, stoppingToken);
            }

            _udpClient.Dispose();
        }
    }
}
