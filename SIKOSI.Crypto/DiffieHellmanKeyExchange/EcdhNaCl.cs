// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created:        11.05.2020 15:04
// Developer:      Roman Jahn
// Project         SIKOSI
//
// Released under GPL-3.0-only

using Chaos.NaCl;
using SIKOSI.Crypto.Interfaces;
using System.Security.Cryptography;

namespace SIKOSI.Crypto.DiffieHellmanKeyExchange
{
    /// <summary>
    /// <para>
    /// This class provides functionalities for the Elliptic Curve Diffie-Hellman key exchange using the NaCl library.
    /// The underlying used elliptic curve is Curve25519.
    /// </para>
    /// Class EcdhNaCl. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class EcdhNaCl : IDiffieHellmanKeyExchange
    {
        /// <summary>
        ///The private key for the Diffie-Hellman key exchange.
        /// </summary>
        private readonly byte[] privateKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="EcdhNaCl"/> class.
        /// A new key pair gets instantiated as well, i.e. <see cref="InitNewKeys"/> gets called.
        /// </summary>
        public EcdhNaCl()
        {
            privateKey = new byte[32];
            InitNewKeys();
        }

        /// <summary>
        /// The public key for the Diffie-Hellman key exchange.
        /// Use this key as parameter in the <see cref="GetSharedSecretKey(byte[])"/> method 
        /// of another instance of the <see cref="EcdhNaCl"/> class to compute the shared secret key.
        /// </summary>
        public byte[] PublicKey { get; private set; }

        /// <summary>
        /// Initializes a new key pair (private key, public key) for the Diffie-Hellman key exchange.
        /// The public key can then be retrieved from the <see cref="PublicKey"/> property.
        /// </summary>
        public void InitNewKeys()
        {
            RandomNumberGenerator.Create().GetBytes(privateKey);

            PublicKey = MontgomeryCurve25519.GetPublicKey(privateKey);
        }

        /// <summary>
        /// Gets the computed shared key for the Diffie-Hellman key exchange using the public key of another instance of the <see cref="EcdhNaCl"/> class.
        /// The returned shared key is of length 256 bits, i.e. 32 bytes.
        /// </summary>
        /// <param name="otherPublicKey">The public key of another instance of the <see cref="EcdhNaCl"/> class.</param>
        /// <returns>The shared key.</returns>
        public byte[] GetSharedSecretKey(byte[] otherPublicKey)
        {
            return MontgomeryCurve25519.KeyExchange(otherPublicKey, privateKey);
        }
    }
}