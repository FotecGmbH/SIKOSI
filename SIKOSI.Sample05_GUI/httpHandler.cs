    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    namespace SIKOSI.Sample05_GUI
    {
        public class HttpHandler : IDisposable
        {
            public double LatestTemp { get; private set; }
            public double LatestHum { get; private set; }
            public double LatestDistance { get; private set; }
            public bool DoorOpen { get; set; }
            public bool DoorStateSet { get; set; }
            public bool LatestTempSet { get; private set; }
            public bool LatestDistanceSet { get; private set; }
            public bool LatestHumSet { get; private set; }
            private HttpClient client;
            private Task _workerTask;




            public HttpHandler()
            {
                client = new HttpClient();
            }

            private async Task GetLatestTempValues(CancellationToken token)
            {
                using var request = new HttpRequestMessage
                                    {
                                        Method = HttpMethod.Get,
                                        RequestUri = new Uri("https://fotecdraginolht65.data.thethingsnetwork.org/api/v2/query?last=10s"),
                                        Headers =
                                        {
                                            {"ACCEPT", "application/json"},
                                            {"Authorization", "key ttn-account-v2.qkycIcLeKiFH39L1uqR5X2vkzIJH2x0ryCxQaLXtWsk"}
                                        }
                                    };
                var response = await client.SendAsync(request, token);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = "{ 'results' :" + await response.Content.ReadAsStringAsync() + "}";
                    JObject jsonContent = JObject.Parse(content);
                    var lastResult = jsonContent["results"].Children().Last();
                    var tempData = lastResult.ToObject<LHT65Message>();
                    LatestTempSet = true;
                    LatestHumSet = true;
                    LatestHum = tempData.Hum_SHT;
                    LatestTemp = tempData.TempC_SHT;
                }
            }

            public void Start(CancellationToken token)
            {
                _workerTask = Task.Run(async () =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        await GetLatestTempValues(token);
                        await Task.Delay(1000, token);
                    }
                });
            }

            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            public void Dispose()
            {
                client.Dispose();
            }
        }

        public class LHT65Message : BaseHttpMessage
        {
            //{{"BatV": 3.115,"Ext_sensor": "Temperature Sensor","Hum_SHT": 50.8,"TempC_DS": 327.67,"TempC_SHT": 28.82,"device_id": "01822b1c","raw": "zCsLQgH8AX//f/8=","time": "2020-08-21T12:50:22.203323153Z"
            public double BatV { get; set; }
            public string Ext_sensor { get; set; }
            public double Hum_SHT { get; set; }
            public double TempC_DS { get; set; }
            public double TempC_SHT { get; set; }
        }

        public abstract class BaseHttpMessage
        {
            public string device_id { get; set; }
            public string raw { get; set; }
            public string time { get; set; }
        }
    }
