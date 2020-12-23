using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SIKOSI.Sample05_IOT.Services.Implementations;
using SIKOSI.Sample05_IOT.Services.Interfaces;
using System.Net.Http;
using SIKOSI.Crypto.Interfaces;
using SIKOSI.Crypto;
using SIKOSI.SecureServices;
using SIKOSI.Crypto.DiffieHellmanKeyExchange;

namespace SIKOSI.Sample05_IOT
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (OperationCanceledException) { }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var endpoint = hostContext.Configuration.GetSection("Connectivity")["serverEndpoint"];
                    services.AddSingleton<ISensorHelper, SensorHelper>();
                    services.AddSingleton<IHttpHandler, HttpHandler>();
                    services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(endpoint) });
                    services.AddScoped<ISecureEncryption, SecureEncryptionDh>(sp => new SecureEncryptionDh(new EcdhNaCl()));
                    services.AddScoped<EncryptedCommunicationService>();
                    services.AddHostedService<Worker>();
                });
    }
}
