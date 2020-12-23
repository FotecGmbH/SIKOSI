// (C) 2019 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created:        03.12.2020 16:25
// Developer:      Gregor Faiman
// Project         SIKOSI
//
// Released under GPL-3.0-only

using System;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SecurityDriven.Inferno;
using SIKOSI.Sample02_SRP.Helpers;
using SIKOSI.SampleDatabase03.Context;
using SIKOSI.SampleDatabase03.Entities;
using SRP_SDK;

namespace SIKOSI.Sample02_SRP.Services
{
    /// <summary>
    /// This is a service used by the SRP protocol controller.
    /// </summary>
    public class SRPProtocolService : ISRPProtocolService
    {
        private SRPDataContext context;
        private SrpServer srpServer;

        /// <summary>
        /// Class for storing values that were calculated by the <see cref="srpServer"/> class.
        /// </summary>
        private DataCache dataCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="SRPProtocolService"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="dataCache">The in memory data cache.</param>
        public SRPProtocolService(SRPDataContext context, DataCache dataCache)
        {
            this.context = context;
            this.context.Database.EnsureCreated();
            this.srpServer = new SrpServer(new SRPGroup());
            this.dataCache = dataCache;
        }

        /// <summary>
        /// Calculates the server proof.
        /// </summary>
        /// <param name="verifier">Client verifier.</param>
        /// <param name="sessionKey">The session key as calculated by the server.</param>
        /// <param name="clientProof">The proof as calculated by the client and sent to the server.</param>
        /// <returns>The server proof.</returns>
        /// <exception cref="ArgumentException">
        /// Might be thrown in the <see cref="DataCache.ExtractClientValueA(BigInteger)"/> method.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if session key or client proof are null.
        /// </exception>
        public byte[] CalculateServerProof(BigInteger verifier, byte[] sessionKey, byte[] clientProof)
        {
            var clientPublicValue = this.dataCache.ExtractClientValueA(verifier);

            return this.srpServer.CalculateServerProof(sessionKey, clientProof, clientPublicValue);
        }

        /// <summary>
        /// Completes the registration and persists user object in database.
        /// </summary>
        /// <param name="username">The users username.</param>
        /// <param name="verifierBytes">The users generated verifier.</param>
        /// <param name="saltBytes">The users salt.</param>
        /// <returns>An empty task.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if username, verifier or salt bytes are null.
        /// </exception>
        public async Task CompleteRegistration(string username, byte[] verifierBytes, byte[] saltBytes)
        {
            if (username == null)
                throw new ArgumentNullException(nameof(username), "User name for registration must not be null.");

            if (verifierBytes == null)
                throw new ArgumentNullException(nameof(verifierBytes), "Verifier must not be null.");

            if (saltBytes == null)
                throw new ArgumentNullException(nameof(saltBytes), "Salt must not be null.");

            var user = new User();
            user.Username = username;
            user.Verifier = verifierBytes;
            user.Salt = saltBytes;

            await this.context.Users.AddAsync(user);
            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Creates a user record in memory for authentication.
        /// </summary>
        /// <param name="verifier">The user verifier.</param>
        /// <param name="username">The user name.</param>
        /// <param name="clientValue">Ephemeral client value generated as part of the authentication process.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if user name or client value are nnull.
        /// </exception>
        public void CreateUserRecordInMemory(BigInteger verifier, string username, byte[] clientValue)
        {
            this.dataCache.CreateUserRecord(verifier, username, clientValue);
        }

        /// <summary>
        /// Checks whether a specific user exists.
        /// </summary>
        /// <param name="username">The user name to check for.</param>
        /// <returns>Whether a user with the specified user name exists.</returns>
        public async Task<bool> Exists(string username)
        {
            var user = await this.context.Users.FirstOrDefaultAsync(u => u.Username == username);

            return user != null;
        }

        /// <summary>
        /// Generates two ephemeral server values.
        /// </summary>
        /// <param name="verifier">The clients verifier.</param>
        /// <returns>The generated public server value.</returns>
        public byte[] GenerateServerProtocolValues(BigInteger verifier)
        {
            var publicServerValue = this.srpServer.GenerateBValues(verifier, out BigInteger privateServerValue);
            this.dataCache.StoreServerValues(verifier, privateServerValue.ToByteArray(), publicServerValue.ToByteArray());

            return publicServerValue.ToByteArray();
        }

        /// <summary>
        /// Generates a session key.
        /// </summary>
        /// <param name="verifier">The verifier to use in key generation.</param>
        /// <returns>The generated session key.</returns>
        /// <exception cref="ArgumentException">
        /// Is thrown if the needed values for session key generation have not yet been generated and stored with the specified verifier.
        /// Needed values for session key generation are: Client public value, Server private value, server public value.
        /// </exception>
        public byte[] GenerateSessionKey(BigInteger verifier)
        {
            BigInteger publicClientValue = dataCache.ExtractClientValueA(verifier).ToBigInteger();
            BigInteger privateServerValue = dataCache.RetrieveServerPrivateValue(verifier).ToBigInteger();
            BigInteger publicServerValue = dataCache.RetrieveServerPublicValue(verifier).ToBigInteger();

            var sessionKey = this.srpServer.ComputeSessionKey(verifier, publicClientValue, privateServerValue, publicServerValue);
            this.dataCache.StoreSessionKey(verifier, sessionKey.ToByteArray());

            return sessionKey.ToByteArray();
        }

        /// <summary>
        /// Gets a message reply.
        /// </summary>
        /// <param name="username">The users username.</param>
        /// <param name="encryptedMessage">The users encrypted message.</param>
        /// <returns>A reply to the message.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if username or encrypted message are null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Is thrown if a user with the specifie user name does not exist.
        /// </exception>
        /// <exception cref="FormatException">
        /// Is thrown if the decrypted message does not contain a colon.
        /// </exception>
        public async Task<byte[]> GetMessageReply(string username, byte[] encryptedMessage)
        {
            if (username == null)
                throw new ArgumentNullException(nameof(username), "User name must not be null.");

            if (encryptedMessage == null)
                throw new ArgumentNullException(nameof(encryptedMessage), "message must not be null.");

            var user = await this.GetSpecificUser(username);
            var sessionKey = this.dataCache.ExtractSessionKey(user.Verifier.ToBigInteger());

            var decryptedMessage = SuiteB.Decrypt(sessionKey, encryptedMessage.AsArraySegment());

            var splitMessage = Encoding.UTF8.GetString(decryptedMessage).Split(':');

            if (splitMessage.Length < 2)
                throw new FormatException();

            var reply = SuiteB.Encrypt(sessionKey, Encoding.UTF8.GetBytes(string.Concat($"{username} said: {splitMessage[1]}")).AsArraySegment());

            return reply;
        }

        /// <summary>
        /// Gets the salt for a specific username.
        /// </summary>
        /// <param name="username">The username to get a salt for.</param>
        /// <returns>The salt associated with this username.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if username is null.
        /// </exception>
        public async Task<byte[]> GetSalt(string username)
        {
            if (username == null)
                throw new ArgumentNullException(nameof(username), "Username to get salt for must not be null.");

            var user = await this.context.Users.Where(u => u.Username == username).FirstOrDefaultAsync();

            return (user == null || user.Salt == null) ? srpServer.GenerateSalt(username) : user.Salt;
        }

        /// <summary>
        /// Gets a specific user from the database.
        /// </summary>
        /// <param name="username">The username of the user to get.</param>
        /// <returns>The user object.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if username is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if the user does not exist.
        /// </exception>
        public async Task<User> GetSpecificUser(string username)
        {
            if (username == null)
                throw new ArgumentNullException(nameof(username), "User name must not be null.");

            return await this.context.Users.Where(u => u.Username == username).FirstOrDefaultAsync() ?? throw new ArgumentNullException(nameof(username), "User with specified username could not be found.");
        }

        /// <summary>
        /// Validates the client client proof, ensuring that the clients session key matches the servers session key.
        /// </summary>
        /// <param name="verifier">The clients verifier. Generated during registration and stored by the server.</param>
        /// <param name="sessionKey">The session key as generated by the server.</param>
        /// <param name="clientProof">The client proof as generated and sent over by the client.</param>
        /// <returns>Whether the client proof calculated by the server matches the client proof calculated by the client.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if verifier, session key or client proof are null.
        /// </exception>
        public bool ValidateClientProof(BigInteger verifier, byte[] sessionKey, byte[] clientProof)
        {
            if (verifier == null)
                throw new ArgumentNullException(nameof(verifier), "Verifier must not be null.");

            if (sessionKey == null)
                throw new ArgumentNullException(nameof(sessionKey), "Session key must not be null.");

            if (clientProof == null)
                throw new ArgumentNullException(nameof(clientProof), "Client proof must not be null.");

            var expected = this.srpServer.CalculateExpectedClientProof(sessionKey, this.dataCache.ExtractClientValueA(verifier), this.dataCache.RetrieveServerPublicValue(verifier));

            return expected == clientProof.ToBigInteger();
        }
    }
}
