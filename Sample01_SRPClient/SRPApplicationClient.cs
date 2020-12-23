// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 08.07.2020 15:03
// Entwickler      Gregor Faiman
// Projekt         SIKOSI
namespace Sample01_SRPClient
{
    using System;
    using System.Text;
    using System.Net.Http;
    using System.Numerics;
    using System.Threading.Tasks;
    using SIKOSI.SRPShared;
    using System.Security.Cryptography;
    using SRP_SDK;
    using SecurityDriven.Inferno;

    /// <summary>
    /// This class represents an end user and offers functionality from an end users point of view.
    /// </summary>
    public class SrpApplicationClient
    {
        /// <summary>
        /// Base address of client.
        /// </summary>
        private Uri clientBaseAddress;

        /// <summary>
        /// Underlying SRP client responsible for handling SRP specific procedures.
        /// </summary>
        private SrpClient srpClient;

        /// <summary>
        /// Backing field of the <see cref="Username"/> Property.
        /// </summary>
        private string username;

        /// <summary>
        /// Backing field of the <see cref="Password"/> Property.
        /// </summary>
        private string password;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The passowrd.</param>
        /// <param name="srpClient">The underlying SRP client.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if user name is null.
        /// Is thrown if password is null.
        /// Is thrown if the underlying SRP client is null.
        /// Is thrown if HTTP client is null.
        /// </exception>
        public SrpApplicationClient(string userName, string password, SrpClient srpClient, Uri clientBaseAddress)
        {
            this.SrpClient = srpClient;
            this.Username = userName;
            this.Password = password;
            this.clientBaseAddress = clientBaseAddress;
        }

        /// <summary>
        /// Gets the user password.
        /// </summary>
        /// <value>The user password.</value>
        /// <exception cref="ArgumentNullException">
        /// Thrown if value is null.
        /// </exception>
        public string Password
        {
            get
            {
                return this.password;
            }

            private set
            {
                this.password = value ?? throw new ArgumentNullException(nameof(value), "Password must not be null");
            }
        }

        /// <summary>
        /// Gets the user name.
        /// </summary>
        /// <value>The user name.</value>
        /// <exception cref="ArgumentNullException">
        /// Thrown if value is null.
        /// </exception>
        public string Username
        {
            get
            {
                return this.username;
            }

            private set
            {
                this.username = value ?? throw new ArgumentNullException(nameof(value), "User name must not be null");
            }
        }

        /// <summary>
        /// Gets the underlying SRP client.
        /// </summary>
        /// <value>The SRP client.</value>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if you attempt to set null.
        /// </exception> 

        public SrpClient SrpClient
        {
            get
            {
                return this.srpClient;
            }

            private set
            {
                this.srpClient = value ?? throw new ArgumentNullException(nameof(value), "Srp client must not be null");
            }
        }

        /// <summary>
        /// Registers the client with the server.
        /// </summary>
        /// <param name="userName">The chosen user name.</param>
        /// <param name="password">The chosen password.</param>
        public async Task RegisterAsync(string saltRequestUri, string registrationUri)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password), "Password must not be null");

            if (registrationUri == null)
                throw new ArgumentNullException(nameof(registrationUri), "Registration uri must not be null.");

            if (saltRequestUri == null)
                throw new ArgumentNullException(nameof(saltRequestUri), "Salt request uri must not be null.");

            BigInteger verifier;
            byte[] salt;

            using (var client = new HttpClient())
            {
                client.BaseAddress = this.clientBaseAddress;

                salt = Convert.FromBase64String(await this.RequestSaltAsync(saltRequestUri));
                verifier = this.SrpClient.GenerateVerifier(this.Username, this.Password, salt);

                var model = new SrpRegistrationModel(this.Username, salt, verifier.ToByteArray());
                var result = await client.PostAsJsonAsync<SrpRegistrationModel>(registrationUri, model);

                if (!result.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Registration could not be completed.");
                }
            }
        }

        /// <summary>
        /// Logs in asynchronously with the given credentials, returning a session key.
        /// This method however does nothing to validate whether they key is the correct one.
        /// To achieve this, use the <see cref="ComputeProof(byte[], byte[], string)"/> method.
        /// </summary>
        /// <param name="userName">The specified user name.</param>
        /// <param name="password">The specified password.</param>
        /// <param name="saltRequestUri">The identifier of the location at which the salt is stored</param>
        /// <param name="serverValueExchangeUri">The identifier of the location to which to post the value A to.</param>
        /// <returns>A task object containing the session key in its result.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if user name, request uri or password are null.
        /// </exception>
        public async Task<byte[]> LoginAsync(string saltRequestUri, string serverValueExchangeUri)
        {
            if (saltRequestUri == null)
                throw new ArgumentNullException(nameof(saltRequestUri), "Salt request uri must not be null");

            if (serverValueExchangeUri == null)
                throw new ArgumentNullException(nameof(serverValueExchangeUri), "Value post uri must not be null");

            byte[] salt;
            byte[] sessionKey;

            string valueBStringRepresentation;
            string saltBase64StringRepresentation;

            BigInteger valueA;
            BigInteger valueB;

            HttpResponseMessage authenticationResponse;

            Task<string> saltRequestTask;

            using (var client = new HttpClient())
            {
                SrpAuthenticationModel model;

                client.BaseAddress = this.clientBaseAddress;

                saltRequestTask = this.RequestSaltAsync(saltRequestUri);

                valueA = this.SrpClient.GenerateClientValues();
                model = new SrpAuthenticationModel(valueA.ToByteArray(), this.Username);

                authenticationResponse = await client.PostAsJsonAsync<SrpAuthenticationModel>(serverValueExchangeUri, model);
            }

            if (!authenticationResponse.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Client could not be authenticated.");
            }

            valueBStringRepresentation = await authenticationResponse.Content.ReadAsStringAsync();
            valueB = Convert.FromBase64String(valueBStringRepresentation).ToBigInteger();

            saltBase64StringRepresentation = await saltRequestTask;
            salt = Convert.FromBase64String(saltBase64StringRepresentation);

            // It is important to set the ServerPublicValue Property of the SRP client before attempting to generate
            // the client proof.
            this.SrpClient.ServerPublicValue = valueB;

            // At this point the client has its key, however it does not know as of yet if the server has calculated the same key.
            sessionKey = this.SrpClient.ComputeSessionKey(this.Username, password, salt, this.SrpClient.ServerPublicValue).ToByteArray();

            return sessionKey;
        }

        /// <summary>
        /// Validates whether client and server calculated the same session key.
        /// </summary>
        /// <param name="serverPublicValue">The server generated public value B.</param>
        /// <param name="sessionKey">The calculated session key by the client.</param>
        /// <param name="validateProofUri">The uri to which to post the client proof.</param>
        /// <param name="username">The client username.</param>
        /// <returns>Whether the session keys match, meaning the proof was successful.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if either of the arguments are null.
        /// </exception>
        public async Task<bool> ComputeProof(byte[] sessionKey, string validateProofUri)
        {
            if (sessionKey == null)
                throw new ArgumentNullException(nameof(sessionKey), "session key must not be null.");

            if (validateProofUri == null)
                throw new ArgumentNullException(nameof(validateProofUri), "Proof post uri must not be null.");

            byte[] clientProof;
            byte[] serverProof;
            byte[] expectedServerProof;
            string serverProofStringRepresentation;

            Task<HttpResponseMessage> postClientProofTask;
            HttpResponseMessage clientProofTaskResult;
            var padLength = this.SrpClient.SrpGroup.N.ToByteArray().Length;
            SrpProofModel model;

            clientProof = this.SrpClient.ComputeProof(sessionKey);

            using (var client = new HttpClient())
            {
                client.BaseAddress = this.clientBaseAddress;

                model = new SrpProofModel(clientProof, username);
                postClientProofTask = client.PostAsJsonAsync<SrpProofModel>(validateProofUri, model);
                expectedServerProof = this.SrpClient.GenerateExpectedServerProof(clientProof, sessionKey, padLength);
                clientProofTaskResult = await postClientProofTask;

                if (!clientProofTaskResult.IsSuccessStatusCode)
                    throw new CryptographicException("Client proof could not be validated on the server.");

                serverProofStringRepresentation = await clientProofTaskResult.Content.ReadAsStringAsync();
                serverProof = Convert.FromBase64String(serverProofStringRepresentation);

                if (serverProof.ToBigInteger() == expectedServerProof.ToBigInteger())
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Requests a salt associated with the specified user name from the server via a HTTP request.
        /// </summary>
        /// <param name="saltRequestUri">The ressource location on the API.</param>
        /// <returns>The salt associated with the user.</returns>
        public async Task<string> RequestSaltAsync(string saltRequestUri)
        {
            if (saltRequestUri == null)
                throw new ArgumentNullException(nameof(saltRequestUri), "Salt request uri must not be null");

            HttpResponseMessage salt;

            using (var client = new HttpClient())
            {
                client.BaseAddress = this.clientBaseAddress;
                salt = await client.GetAsync(saltRequestUri);
            }

            return await salt.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Sends an encrypted message to the server using the generated session key.
        /// Used for testing purposes.
        /// </summary>
        /// <param name="sessionKey">The session key.</param>
        /// <param name="message">The message.</param>
        /// <returns>The servers response.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if session key is null.
        /// Thrown if message is null.
        /// </exception>
        public async Task<string> SendMessage(byte[] sessionKey, string message)
        {
            var encryptedClientMessage = SuiteB.Encrypt(sessionKey, Encoding.UTF8.GetBytes(message).AsArraySegment());
            var model = new SrpMessageModel(this.Username, encryptedClientMessage);

            Console.WriteLine($"Encrypted client message: {Encoding.UTF8.GetString(encryptedClientMessage)}");

            using (var client = new HttpClient())
            {
                client.BaseAddress = this.clientBaseAddress;

                try
                {
                    var response = await client.PostAsJsonAsync("api/user/postmessage", model);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var encryptedServerMessage = Convert.FromBase64String(responseContent);

                    Console.WriteLine($"Encrypted server message: {Encoding.UTF8.GetString(encryptedServerMessage)}");

                    var decryptedServerMessage = SuiteB.Decrypt(sessionKey, encryptedServerMessage.AsArraySegment());
                    return Encoding.UTF8.GetString(decryptedServerMessage);
                }
                catch (Exception)
                {
                    throw new Exception("Input Format der Nachricht war ungültig. Gültiges Format erwartet sich Doppelpunkt in Nachricht.");
                }
            }
        }
    }
}
