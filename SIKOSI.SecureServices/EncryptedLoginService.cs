// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 12.11.2020 08:37
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SIKOSI.Crypto.Interfaces;
using SIKOSI.Exchange.Interfaces;

namespace SIKOSI.SecureServices
{
    /// <summary>
    ///     <para>
    ///         This class provides an encrypted login at a specified route.
    ///     </para>
    ///     Class EncryptedLoginService. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class EncryptedLoginService
    {
        #region Properties

        /// <summary>
        ///     A function to get the public key of the server where to log in.
        /// </summary>
        public Func<Task<byte[]>> GetReceiverPublicKeyFunc { get; set; }

        /// <summary>
        ///     The encryption to be used for communicating with the server.
        /// </summary>
        public ISecureEncryption Encryption { get; set; }

        #endregion

        /// <summary>
        ///     Tries to login at the given route of the server and the HttpClient.
        ///     Communicates encrypted with the server.
        /// </summary>
        /// <typeparam name="LoginModel">
        ///     The login type to be sent to server.
        ///     Usually a type with username and password property, but could be any type with a parameterless constructor.
        /// </typeparam>
        /// <typeparam name="LoginResultModel">
        ///     The expected type in the answer of the server after successful login.
        ///     Could be any type with a parameterless constructor and containing a Token property as string (IToken).
        ///     This token gets then set in the given http object.
        /// </typeparam>
        /// <param name="loginModel">
        ///     The Login model to be sent to server.
        ///     Usually a model with username and password, but could be any model with a parameterless constructor.
        /// </param>
        /// <param name="http">The http client to be used to send requests to the server.</param>
        /// <param name="loginRoute">
        ///     The route on the server where to login. If base address is not set in http object,
        ///     provide the full route beginning with "http...".
        /// </param>
        /// <param name="encoding">The encoding to be used. A default encoding will be used if null.</param>
        /// <returns>
        ///     Contains an indicator whether login process was successful and the expected model
        ///     in the answer of the server after successful login or a an Exception that has been occurred while login.
        ///     ResultData could be any model with a parameterless constructor and containing a Token property as string (IToken).
        ///     This token gets then set in the given http object.
        /// </returns>
        public async Task<SecureServiceResult<LoginResultModel>> TryEncryptedLogin<LoginModel, LoginResultModel>(LoginModel loginModel, HttpClient http, string loginRoute, Encoding encoding = null) where LoginModel : new() where LoginResultModel : IToken, new()
        {
            try
            {
                if (http == null) throw new ArgumentNullException(nameof(http));

                // get public key from receiver/server
                var receiverPublicKey = await GetReceiverPublicKeyFunc();

                var com = new EncryptedCommunicationService {ReceiverPublicKey = receiverPublicKey, Encryption = Encryption};

                // encrypt bytes to model
                var result = await com.TryEncryptedJsonPost<LoginModel, LoginResultModel>(loginModel, http, loginRoute);

                if (result.ResultModel is IToken tokenContainer)
                {
                    // set token in http header for upcoming requests
                    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenContainer.Token);

                    // login successful
                    return new SecureServiceResult<LoginResultModel>(result.ResultModel);
                }

                return new SecureServiceResult<LoginResultModel>(new Exception($"{nameof(EncryptedLoginService)}: Couldn't get token!"));
            }
            catch (Exception e)
            {
                return new SecureServiceResult<LoginResultModel>(e);
            }
        }
    }
}