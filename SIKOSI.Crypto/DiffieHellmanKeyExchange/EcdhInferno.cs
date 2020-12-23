// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created:        11.05.2020 14:44
// Developer:      Roman Jahn
// Project         SIKOSI
//
// Released under GPL-3.0-only

using SIKOSI.Crypto.Interfaces;
using SecurityDriven.Inferno.Extensions;
using System;
using System.Security.Cryptography;

namespace SIKOSI.Crypto.DiffieHellmanKeyExchange
{
    /// <summary>
    /// <para>
    /// This class provides functionalities for the Elliptic Curve Diffie Hellman key exchange using the SecurityDriven.Inferno and System.Security.Cryptography libraries.
    /// The underlying used elliptic curve is P-384 as documented in https://securitydriven.net/inferno/.
    /// </para>
    /// Class EcdhInferno. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class EcdhInferno : IDiffieHellmanKeyExchange
    {
        /// <summary>
        /// The key pair (private/public key) for the Diffie-Hellman key exchange.
        /// </summary>
        private CngKey keyPair;

        /// <summary>
        /// Initializes a new instance of the <see cref="EcdhInferno"/> class.
        /// A new key pair gets instantiated as well, i.e. <see cref="InitNewKeys"/> gets called.
        /// </summary>
        public EcdhInferno()
        {
            InitNewKeys();
        }

        /// <summary>
        /// The public key for the Diffie-Hellman key exchange.
        /// Use this key as parameter in the <see cref="GetSharedSecretKey(byte[])"/> method 
        /// of another instance of the <see cref="EcdhInferno"/> class to compute the shared secret key.
        /// </summary>
        public byte[] PublicKey { get; private set; }

        /// <summary>
        /// Initializes a new key pair (private key, public key) for the Diffie-Hellman key exchange.
        /// The public key can then be retrieved from the <see cref="PublicKey"/> property.
        /// </summary>
        public void InitNewKeys()
        {
            keyPair = CngKeyExtensions.CreateNewDhmKey();

            PublicKey = keyPair.GetPublicBlob();
        }

        /// <summary>
        /// Gets the computed shared key for the Diffie-Hellman key exchange using the public key of another instance of the <see cref="EcdhInferno"/> class.
        /// The returned shared key is of length 384 bits, i.e. 48 bytes.
        /// </summary>
        /// <param name="otherPublicKey">The public key of another instance of the <see cref="EcdhInferno"/> class.</param>
        /// <returns>The shared key.</returns>
        public byte[] GetSharedSecretKey(byte[] otherPublicKey)
        {
            if (otherPublicKey is null) throw new ArgumentNullException(nameof(otherPublicKey));

            if (otherPublicKey.Length != 104) throw new ArgumentException("Given public key has not the correct length.", nameof(otherPublicKey));

            return keyPair.GetSharedDhmSecret(CngKey.Import(otherPublicKey, CngKeyBlobFormat.EccPublicBlob));
        }
    }
}