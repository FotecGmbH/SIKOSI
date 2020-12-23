// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created:        11.05.2020 15:11
// Developer:      Roman Jahn
// Project         SIKOSI
//
// Released under GPL-3.0-only

using NSec.Cryptography;
using SIKOSI.Crypto.Interfaces;

namespace SIKOSI.Crypto.DiffieHellmanKeyExchange
{
    /// <summary>
    /// <para>
    /// This class provides functionalities for the Elliptic Curve Diffie-Hellman key exchange using the Nsec.Cryptography library.
    /// The underlying used elliptic curve is Curve25519.
    /// </para>
    /// Class EcdhNsec. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class EcdhNsec : IDiffieHellmanKeyExchange
    {
        /// <summary>
        /// The private key for the Diffie-Hellman key exchange.
        /// </summary>
        private byte[] privateKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="EcdhNsec"/> class.
        /// A new key pair gets instantiated as well, i.e. <see cref="InitNewKeys"/> gets called.
        /// </summary>
        public EcdhNsec()
        {
            InitNewKeys();
        }

        /// <summary>
        /// The public key for the Diffie-Hellman key exchange.
        /// Use this key as parameter in the <see cref="GetSharedSecretKey(byte[])"/> method 
        /// of another instance of the <see cref="EcdhNsec"/> class to compute the shared secret key.
        /// </summary>
        public byte[] PublicKey { get; private set; }

        /// <summary>
        /// Initializes a new key pair (private key, public key) for the Diffie-Hellman key exchange.
        /// The public key can then be retrieved from the <see cref="PublicKey"/> property.
        /// </summary>
        public void InitNewKeys()
        {
            var random = RandomGenerator.Default;

            privateKey = random.GenerateBytes(KeyAgreementAlgorithm.X25519.PrivateKeySize);

            var key = Key.Import(
                KeyAgreementAlgorithm.X25519, 
                privateKey, 
                KeyBlobFormat.RawPrivateKey);

            PublicKey = key.PublicKey.Export(KeyBlobFormat.RawPublicKey);
        }

        /// <summary>
        /// Gets the computed shared key for the Diffie-Hellman key exchange using the public key of another instance of the <see cref="EcdhNsec"/> class.
        /// The returned shared key is of length 256 bits, i.e. 32 bytes.
        /// </summary>
        /// <param name="otherPublicKey">The public key of another instance of the <see cref="EcdhNsec"/> class.</param>
        /// <returns>The shared key.</returns>
        public byte[] GetSharedSecretKey(byte[] otherPublicKey)
        {
            var privKey = Key.Import(
                KeyAgreementAlgorithm.X25519,
                privateKey,
                KeyBlobFormat.RawPrivateKey);

            var publicKey = NSec.Cryptography.PublicKey.Import(
                KeyAgreementAlgorithm.X25519,
                otherPublicKey,
                KeyBlobFormat.RawPublicKey);

            var sharedSecret = KeyAgreementAlgorithm.X25519.Agree(privKey, publicKey);

            return KeyDerivationAlgorithm.HkdfSha256.DeriveBytes(sharedSecret, null, null, 32);
        }
    }
}