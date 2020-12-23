// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 13.11.2020 09:47
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SIKOSI.Crypto.Interfaces;

namespace SIKOSI.SecureServices
{
    /// <summary>
    ///     <para>
    ///         This class provides an encrypted registration at a specified route.
    ///     </para>
    ///     Class EncryptedRegistrationService. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class EncryptedRegistrationService
    {
        #region Properties

        /// <summary>
        ///     A function to get the public key of the server where to registrate.
        /// </summary>
        public Func<Task<byte[]>> GetReceiverPublicKeyFunc { get; set; }

        /// <summary>
        ///     The encryption to be used for communicating with the server.
        /// </summary>
        public ISecureEncryption Encryption { get; set; }

        #endregion

        /// <summary>
        ///     Tries to register at the given route of the server and the HttpClient.
        ///     Communicates encrypted with the server.
        ///     Use this method if you expect Data in the answer of the server.
        /// </summary>
        /// <typeparam name="RegistrationModel">
        ///     The registration type to be sent to the server.
        ///     Could be any type with a parameterless constructor.
        /// </typeparam>
        /// <typeparam name="RegistrationResultModel">
        ///     The expected registration result type.
        ///     Could be any type with a parameterless constructor.
        /// </typeparam>
        /// <param name="registrationModel">The registration model to be sent to the server.</param>
        /// <param name="http">The http client to be used to send requests to the server.</param>
        /// <param name="registrationRoute">
        ///     The route on the server where to registrate. If base address is not set in http object,
        ///     provide the full route beginning with "http...".
        /// </param>
        /// <param name="encoding">The encoding to be used. A default encoding will be used if null.</param>
        /// <returns>
        ///     Contains an indicator whether registration process was successful and the expected model
        ///     in the answer of the server after successful registration or a an Exception that has been occurred while
        ///     registration.
        ///     ResultData could be any model with a parameterless constructor.
        /// </returns>
        public async Task<SecureServiceResult<RegistrationResultModel>> TryEncryptedRegistration<RegistrationModel, RegistrationResultModel>(RegistrationModel registrationModel, HttpClient http, string registrationRoute, Encoding encoding = null) where RegistrationModel : new() where RegistrationResultModel : new()
        {
            try
            {
                if (http == null) throw new ArgumentNullException(nameof(http));

                // get public key from receiver/server
                var receiverPublicKey = await GetReceiverPublicKeyFunc();

                var com = new EncryptedCommunicationService {ReceiverPublicKey = receiverPublicKey, Encryption = Encryption};

                // encrypt bytes to model
                var serviceResult = await com.TryEncryptedJsonPost<RegistrationModel, RegistrationResultModel>(registrationModel, http, registrationRoute);

                if (serviceResult.IsServiceSuccessful) return new SecureServiceResult<RegistrationResultModel>(serviceResult.ResultModel);
                return new SecureServiceResult<RegistrationResultModel>(serviceResult.Exception);
            }
            catch (Exception e)
            {
                return new SecureServiceResult<RegistrationResultModel>(e);
            }
        }

        /// <summary>
        ///     Tries to register at the given route of the server and the HttpClient.
        ///     Communicates encrypted with the server.
        ///     Use this method if you only expect an OK-Or-NotOk answer and no Data.
        /// </summary>
        /// <typeparam name="RegistrationModel">
        ///     The registration type to be sent to the server.
        ///     Could be any type with a parameterless constructor.
        /// </typeparam>
        /// <param name="registrationModel">The registration model to be sent to the server.</param>
        /// <param name="http">The http client to be used to send requests to the server.</param>
        /// <param name="registrationRoute">
        ///     The route on the server where to registrate. If base address is not set in http object,
        ///     provide the full route beginning with "http...".
        /// </param>
        /// <param name="encoding">The encoding to be used. A default encoding will be used if null.</param>
        /// <returns>
        ///     Contains an indicator whether registration process was successful
        ///     and an Exception that has been occurred while registration if not successful.
        /// </returns>
        public async Task<SecureServiceResultNoContent> TryEncryptedRegistrationWithoutResultModel<RegistrationModel>(RegistrationModel registrationModel, HttpClient http, string registrationRoute, Encoding encoding = null) where RegistrationModel : new()
        {
            try
            {
                if (http == null) throw new ArgumentNullException(nameof(http));

                // get public key from receiver/server
                var receiverPublicKey = await GetReceiverPublicKeyFunc();

                var com = new EncryptedCommunicationService {ReceiverPublicKey = receiverPublicKey, Encryption = Encryption};

                // encrypt bytes to model
                var serviceResult = await com.TryEncryptedJsonPostWithoutResultModel(registrationModel, http, registrationRoute);

                if (serviceResult.IsServiceSuccessful) return new SecureServiceResultNoContent();
                return new SecureServiceResultNoContent(serviceResult.Exception);
            }
            catch (Exception e)
            {
                return new SecureServiceResultNoContent(e);
            }
        }
    }
}