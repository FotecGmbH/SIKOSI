// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created:        11.05.2020 14:18
// Developer:      Roman Jahn
// Project         SIKOSI
//
// Released under GPL-3.0-only

using System;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using SIKOSI.Crypto.Interfaces;

namespace SIKOSI.Crypto.DiffieHellmanKeyExchange
{
    /// <summary>
    /// <para>
    /// This class provides functionalities for the Elliptic Curve Diffie-Hellman key exchange using the Bouncy Castle library.
    /// The underlying used elliptic curve is Curve25519.
    /// </para>
    /// Class EcdhBouncyCastle. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class EcdhBouncyCastle : IDiffieHellmanKeyExchange
    {
        /// <summary>
        /// The private key for the Diffie-Hellman key exchange.
        /// </summary>
        private byte[] privateKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="EcdhBouncyCastle"/> class.
        /// A new key pair gets instantiated as well, i.e. <see cref="InitNewKeys"/> gets called.
        /// </summary>
        public EcdhBouncyCastle()
        {
            InitNewKeys();
        }

        /// <summary>
        /// The public key for the Diffie-Hellman key exchange.
        /// Use this key as parameter in the <see cref="GetSharedSecretKey(byte[])"/> method 
        /// of another instance of the <see cref="EcdhBouncyCastle"/> class to compute the shared secret key.
        /// </summary>
        public byte[] PublicKey { get; private set; }

        /// <summary>
        /// Initializes a new key pair (private key, public key) for the Diffie-Hellman key exchange.
        /// The public key can then be retrieved from the <see cref="PublicKey"/> property.
        /// </summary>
        public void InitNewKeys()
        {
            var parameters = new X25519PrivateKeyParameters(new SecureRandom());

            privateKey = parameters.GetEncoded();
            PublicKey = parameters.GeneratePublicKey().GetEncoded();
        }

        /// <summary>
        /// Gets the computed shared key for the Diffie-Hellman key exchange using the public key of another instance of the <see cref="EcdhBouncyCastle"/> class.
        /// The returned shared key is of length 256 bits, i.e. 32 bytes.
        /// </summary>
        /// <param name="otherPublicKey">The public key of another instance of the <see cref="EcdhBouncyCastle"/> class.</param>
        /// <returns>The shared key.</returns>
        public byte[] GetSharedSecretKey(byte[] otherPublicKey)
        {
            if (otherPublicKey is null) throw new ArgumentNullException(nameof(otherPublicKey));

            if (otherPublicKey.Length != 32) throw new ArgumentException("Given public key has not the correct length.", nameof(otherPublicKey));

            var privPar = new X25519PrivateKeyParameters(privateKey, 0);

            var pubPar = new X25519PublicKeyParameters(otherPublicKey, 0);
        
            var result = new byte[32];

            privPar.GenerateSecret(pubPar, result, 0);

            return result;
        }
    }
}