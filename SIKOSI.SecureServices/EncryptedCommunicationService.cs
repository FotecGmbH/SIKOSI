// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 13.11.2020 07:50
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using SIKOSI.Crypto.Helper;
using SIKOSI.Crypto.Interfaces;

namespace SIKOSI.SecureServices
{
    /// <summary>
    ///     <para>
    ///         This class provides encrypted communication methods at a specified route.
    ///     </para>
    ///     Class EncryptedCommunicationService. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class EncryptedCommunicationService
    {
        #region Properties

        /// <summary>
        ///     The public key of the server to communicate with.
        /// </summary>
        public byte[] ReceiverPublicKey { get; set; }

        /// <summary>
        ///     The encryption to be used for communicating with the server.
        /// </summary>
        public ISecureEncryption Encryption { get; set; }

        #endregion

        /// <summary>
        ///     Tries an encrypted post using an encrypted json-format using the specified route and http client.
        ///     Use this method if you DO expect any data in the answer of the server.
        /// </summary>
        /// <typeparam name="SendingModel"></typeparam>
        /// <typeparam name="ReceivingModel"></typeparam>
        /// <param name="sendingModel">The model to be sent to the server. Must have a parameterless constructor.</param>
        /// <param name="http">The http client to be used to send requests to the server.</param>
        /// <param name="apiRoute">
        ///     The route on the server where to send the requests. If base address is not set in http object,
        ///     provide the full route beginning with "http...".
        /// </param>
        /// <param name="encoding">The encoding to be used. A default encoding will be used if null.</param>
        /// <returns>
        ///     Contains an indicator whether communication process was successful and the expected model
        ///     in the answer of the server after successful communication or a an exception that has been occurred while
        ///     communication.
        ///     ResultData could be any model with a parameterless constructor.
        /// </returns>
        public async Task<SecureServiceResult<ReceivingModel>> TryEncryptedJsonPost<SendingModel, ReceivingModel>(SendingModel sendingModel, HttpClient http, string apiRoute, Encoding encoding = null) where SendingModel : new() where ReceivingModel : new()
        {
            if (http == null) throw new ArgumentNullException(nameof(http));

            try
            {
                var encryptedBytes = ModelToEncryptedBytesConverter.ConvertFromModelToEncryptedByteArray(sendingModel, Encryption, ReceiverPublicKey, encoding);

                // send data and wait for response
                HttpResponseMessage response = default;
                
                try
                {
                    response = await http.PostAsJsonAsync(apiRoute, encryptedBytes).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    //httpClient tasks get sometimes cancelled -> create a new httpClient
                    var newHttp = new HttpClient { BaseAddress = http.BaseAddress };
                    newHttp.DefaultRequestHeaders.Authorization = http.DefaultRequestHeaders.Authorization;
                    response = await newHttp.PostAsJsonAsync(apiRoute, encryptedBytes).ConfigureAwait(false);
                } 

                if (!response.IsSuccessStatusCode) return new SecureServiceResult<ReceivingModel>(new Exception(response.ReasonPhrase));

                var receivedBytes = await ReadBytesFromResponse(response);

                if (receivedBytes != null)
                {
                    // encrypt bytes to model
                    var receivedContainer = ModelToEncryptedBytesConverter.ConvertFromEncryptedByteArrayToModel<ReceivingModel>(receivedBytes, Encryption, encoding);

                    // communication successful
                    return new SecureServiceResult<ReceivingModel>(receivedContainer.Data);
                }

                return new SecureServiceResult<ReceivingModel>(new Exception("Couldn't read data in the response of the data receiver!"));
            }
            catch (Exception e)
            {
                return new SecureServiceResult<ReceivingModel>(e);
            }
        }

        /// <summary>
        ///     Tries an encrypted post using an encrypted json-format, the specified route and http client.
        ///     Use this method if you do NOT expect any data in the answer of the server.
        /// </summary>
        /// <typeparam name="SendingModel">The model type to be sent to the server. Must have a parameterless constructor.</typeparam>
        /// <param name="sendingModel">The model to be sent to the server. Must have a parameterless constructor.</param>
        /// <param name="http">The http client to be used to send requests to the server.</param>
        /// <param name="apiRoute">
        ///     The route on the server where to send the requests. If base address is not set in http object,
        ///     provide the full route beginning with "http...".
        /// </param>
        /// <param name="encoding">The encoding to be used. A default encoding will be used if null.</param>
        /// <returns>
        ///     Contains an indicator whether communication process was successful
        ///     and an exception that has been occurred while registration if not successful.
        /// </returns>
        public async Task<SecureServiceResultNoContent> TryEncryptedJsonPostWithoutResultModel<SendingModel>(SendingModel sendingModel, HttpClient http, string apiRoute, Encoding encoding = null) where SendingModel : new()
        {
            if (http == null) throw new ArgumentNullException(nameof(http));

            try
            {
                var encryptedBytes = ModelToEncryptedBytesConverter.ConvertFromModelToEncryptedByteArray(sendingModel, Encryption, ReceiverPublicKey, encoding);

                // send data and wait for response
                HttpResponseMessage response = default;
                
                try
                {
                    response = await http.PostAsJsonAsync(apiRoute, encryptedBytes).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    //httpClient tasks get sometimes cancelled -> create a new httpClient
                    var newHttp = new HttpClient { BaseAddress = http.BaseAddress };
                    newHttp.DefaultRequestHeaders.Authorization = http.DefaultRequestHeaders.Authorization;
                    response = await newHttp.PostAsJsonAsync(apiRoute, encryptedBytes).ConfigureAwait(false);
                } 

                if (!response.IsSuccessStatusCode) return new SecureServiceResultNoContent(new Exception(response.ReasonPhrase));

                // communication successful
                return new SecureServiceResultNoContent();
            }
            catch (Exception e)
            {
                return new SecureServiceResultNoContent(e);
            }
        }

        /// <summary>
        ///     Tries an encrypted get using json format in the answer of the server.
        /// </summary>
        /// <typeparam name="ReceivingModel">The expected type of the data in the answer of the server.</typeparam>
        /// <param name="http">The http client to be used to send requests to the server.</param>
        /// <param name="getRoute">
        ///     The route on the server where to send the requests. If base address is not set in http object,
        ///     provide the full route beginning with "http...".
        /// </param>
        /// <param name="encoding">The encoding to be used. A default encoding will be used if null.</param>
        /// <returns>
        ///     Contains an indicator whether communication process was successful and the expected model
        ///     in the answer of the server after successful communication or a an exception that has been occurred while
        ///     communication.
        ///     ResultData could be any model with a parameterless constructor.
        /// </returns>
        public async Task<SecureServiceResult<ReceivingModel>> TryEncryptedJsonGet<ReceivingModel>(HttpClient http, string getRoute, Encoding encoding = null)
        {
            if (http == null) throw new ArgumentNullException(nameof(http));

            try
            {
                HttpResponseMessage response = default;
                
                try
                {
                    response = await http.GetAsync(getRoute).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    //httpClient tasks get sometimes cancelled -> create a new httpClient
                    var newHttp = new HttpClient { BaseAddress = http.BaseAddress };
                    newHttp.DefaultRequestHeaders.Authorization = http.DefaultRequestHeaders.Authorization;
                    response = await newHttp.GetAsync(getRoute).ConfigureAwait(false);
                }

                if (!response.IsSuccessStatusCode) return new SecureServiceResult<ReceivingModel>(new Exception(response.ReasonPhrase));

                var receivedBytes = await ReadBytesFromResponse(response);

                if (receivedBytes != null)
                {
                    // encrypt bytes to model
                    var receivedContainer = ModelToEncryptedBytesConverter.ConvertFromEncryptedByteArrayToModel<ReceivingModel>(receivedBytes, Encryption, encoding);

                    // communication successful
                    return new SecureServiceResult<ReceivingModel>(receivedContainer.Data);
                }

                return new SecureServiceResult<ReceivingModel>(new Exception("Couldn't read data in the response of the get receiver!"));
            }
            catch (Exception e)
            {
                return new SecureServiceResult<ReceivingModel>(e);
            }
        }

        /// <summary>
        ///     Reads the bytes from the response. The content type of the response can be of "application/json" or
        ///     "application/octet-stream".
        /// </summary>
        /// <param name="response">The response to be "parsed"</param>
        /// <returns>The byte array read from the response.</returns>
        private async Task<byte[]> ReadBytesFromResponse(HttpResponseMessage response)
        {
            if (response?.Content?.Headers?.ContentType?.MediaType == null) return new byte[0];

            switch (response.Content.Headers.ContentType.MediaType)
            {
                case "application/json":
                    return await response.Content.ReadFromJsonAsync<byte[]>();
                case "application/octet-stream":
                    return await response.Content.ReadAsByteArrayAsync();
                default:
                    return new byte[0];
            }
        }
    }
}