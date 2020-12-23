﻿// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 07.07.2020 11:58
// Entwickler      Manuel Fasching
// Projekt         SIKOSI

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using SIKOSI.Exchange.Model;

namespace SIKOSI.Sample02_SRP.Helpers
{
    /// <summary>
    /// Data-Cache
    /// </summary>
    public class DataCache
    {
        /// <summary>
        ///     Dictionary mapping values generated for a client session to that clients verifier.
        /// </summary>
        private readonly Dictionary<BigInteger, ClientSessionValues> clientSessionValues;

        public DataCache()
        {
            clientSessionValues = new Dictionary<BigInteger, ClientSessionValues>();
        }

        /// <summary>
        ///     Retrieves the client generated value A as part of the login process.
        /// </summary>
        /// <param name="clientKey">The client verifier used as a key.</param>
        /// <returns>The retrieved value.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the specified key is invalid.
        /// </exception>
        public byte[] ExtractClientValueA(BigInteger clientKey)
        {
            if (!IsKeyValid(clientKey))
                throw new ArgumentException(nameof(clientKey), "Client key was invalid.");

            return clientSessionValues[clientKey].PublicClientValueA;
        }

        /// <summary>
        ///     Stores the public value A that was sent to the server by the client.
        /// </summary>
        /// <param name="clientKey">The dictionary key. This is the client verifier.</param>
        /// <param name="clientValue">The generated value.</param>
        /// <exception cref="ArgumentNullException">
        ///     Is thrown if either client key or client value are null.
        /// </exception>
        public void StoreClientGeneratedValue(BigInteger clientKey, byte[] clientValue)
        {
            if (clientKey == null)
                throw new ArgumentNullException(nameof(clientKey), "Client key must not be null.");

            if (clientValue == null)
                throw new ArgumentNullException(nameof(clientValue), "Client value must not be null.");

            clientSessionValues[clientKey].PublicClientValueA = clientValue;
        }

        /// <summary>
        ///     Creates a user record locally on the server in memory.
        /// </summary>
        /// <param name="clientKey">The client key for the dictionary.</param>
        /// <param name="username">The user name.</param>
        /// <param name="clientGeneratedValueA">The value generated by the client.</param>
        /// <returns>An empty task object.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if either of the parameters are null.
        /// </exception>
        public void CreateUserRecord(BigInteger clientKey, string username, byte[] clientGeneratedValueA)
        {
            if (username == null)
                throw new ArgumentNullException(nameof(username), "User name must not be null.");

            if (clientGeneratedValueA == null)
                throw new ArgumentNullException(nameof(clientGeneratedValueA), "Client value must not be null.");

            ClientSessionValues record;

            record = new ClientSessionValues(username);
            record.PublicClientValueA = clientGeneratedValueA;

            if (this.clientSessionValues.ContainsKey(clientKey))
                this.clientSessionValues[clientKey] = record;
            else
                this.clientSessionValues.Add(clientKey, record);
        }

        /// <summary>
        ///     Retrieves the generated public server value for a client.
        /// </summary>
        /// <param name="clientKey">The client verifier.</param>
        /// <returns>The values for this clients session.</returns>
        /// <exception cref="ArgumentException">
        ///     Is thrown if client key is invalid.
        /// </exception>
        public byte[] RetrieveServerPublicValue(BigInteger clientKey)
        {
            if (!IsKeyValid(clientKey))
                throw new ArgumentException(nameof(clientKey), "Client key was invalid.");

            return clientSessionValues[clientKey].PublicServerValue;
        }

        /// <summary>
        ///     Retrieves the generated private server value for a given client..
        /// </summary>
        /// <param name="clientKey">The client verifier.</param>
        /// <returns>The values for this clients session.</returns>
        /// <exception cref="ArgumentException">
        ///     Is thrown if client key is invalid.
        /// </exception>
        public byte[] RetrieveServerPrivateValue(BigInteger clientKey)
        {
            if (!IsKeyValid(clientKey))
                throw new ArgumentException(nameof(clientKey), "Client key was invalid.");

            return clientSessionValues[clientKey].PrivateServerValue;
        }

        /// <summary>
        ///     Stores the public and prviate server values.
        /// </summary>
        /// <param name="clientVerifier">The clients key.</param>
        /// <param name="privateServerValue">Thep rivate server value.</param>
        /// <param name="publicServerValue">The public server value.</param>
        /// <exception cref="ArgumentException">
        /// Is thrown if the client verifier is an invalid key.
        /// </exception>
        public void StoreServerValues(BigInteger clientVerifier, byte[] privateServerValue, byte[] publicServerValue)
        {
            if (!IsKeyValid(clientVerifier))
                throw new ArgumentException(nameof(clientVerifier), "Client key was invalid.");

            clientSessionValues[clientVerifier].PrivateServerValue = privateServerValue;
            clientSessionValues[clientVerifier].PublicServerValue = publicServerValue;
        }

        /// <summary>
        /// Stores the session key in memory.
        /// </summary>
        /// <param name="clientVerifier">The client verifier used as a dictionary key.</param>
        /// <param name="sessionKey">The session key to be stored.</param>
        /// <exception cref="ArgumentException">
        /// Is thrown if the client key is invalid.
        /// </exception>
        public void StoreSessionKey(BigInteger clientVerifier, byte[] sessionKey)
        {
            if (!IsKeyValid(clientVerifier))
                throw new ArgumentException("Invalid client key.");

            clientSessionValues[clientVerifier].SessionKey = sessionKey;
        }

        /// <summary>
        /// Extracts the stored session key.
        /// </summary>
        /// <param name="clientVerifier">The client key used for storing the session key in the dictionary.</param>
        /// <returns>The session key.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the specified dictionary key is invalid.
        /// </exception>
        public byte[] ExtractSessionKey(BigInteger clientVerifier)
        {
            if (!IsKeyValid(clientVerifier))
                throw new ArgumentException("Invalid client key.");

            return clientSessionValues[clientVerifier].SessionKey;
        }

        /// <summary>
        ///     Checks whether the specified dictionary key is valid.
        /// </summary>
        /// <param name="clientKey">The client verifier used as a dictionary key.</param>
        /// <returns>Whether the key is valid.</returns>
        private bool IsKeyValid(BigInteger clientKey)
        {
            if (clientKey != null && clientSessionValues.ContainsKey(clientKey))
                return true;

            return false;
        }
    }
}