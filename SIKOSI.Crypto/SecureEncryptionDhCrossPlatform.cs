// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 15.12.2020 12:43
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using SIKOSI.Crypto.DiffieHellmanKeyExchange;
using SIKOSI.Crypto.Interfaces;

namespace SIKOSI.Crypto
{
    /// <summary>
    ///     <para>
    ///         This class provides secure encryption using AES-CBC for symmetric encryption and HMACSHA512 for authentication.
    ///         The symmetric key is computed using the Diffie Hellman key exchange protocol.
    ///         Use this class for applications not running on Windows or Linux platform.
    ///     </para>
    /// Klasse SecureEncryptionDhCrossPlatform. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class SecureEncryptionDhCrossPlatform : ISecureEncryption
    {
        /// <summary>
        ///     The Diffie-Hellman key exchange provider.
        /// </summary>
        private readonly IDiffieHellmanKeyExchange dhKeyExchange;

        /// <summary>
        ///     The random number generator.
        /// </summary>
        private readonly RandomNumberGenerator random;

        /// <summary>
        ///     Initializes new class of the <see cref="SecureEncryptionDh" /> class with the default Diffie Hellman key exchange
        ///     provider.
        /// </summary>
        public SecureEncryptionDhCrossPlatform()
        {
            dhKeyExchange = new EcdhNaCl();
            random = RandomNumberGenerator.Create();

            //initialize key pair if necessary
            if (dhKeyExchange.PublicKey is null) dhKeyExchange.InitNewKeys();
        }

        /// <summary>
        ///     Initializes new class of the <see cref="SecureEncryptionDh" /> class.
        /// </summary>
        /// <param name="dhKeyExchange">The Diffie Hellman key exchange provider.</param>
        public SecureEncryptionDhCrossPlatform(IDiffieHellmanKeyExchange dhKeyExchange)
        {
            this.dhKeyExchange = dhKeyExchange ?? throw new ArgumentNullException(nameof(dhKeyExchange));
            random = RandomNumberGenerator.Create();

            //initialize key pair if necessary
            if (this.dhKeyExchange.PublicKey is null) dhKeyExchange.InitNewKeys();
        }

        /// <summary>
        ///     Encrypts the data only readable for the owner of the specified public key.
        /// </summary>
        /// <param name="otherPublicKey">The public key of the recipient of the data.</param>
        /// <param name="dataToEncrypt">The data to encrypt.</param>
        /// <param name="associatedData">The optional associated data to be used during the symmetric encryption via AES.</param>
        /// <param name="symmetricKey">
        ///     The symmetric key gets computed using the public key of the recipient.
        ///     It cannot be determined in the Diffie Hellman key exchange. If set, it gets ignored, so just omit this parameter.
        ///     It's here to implement the <see cref="ISecureEncryption" /> interface.
        /// </param>
        /// <returns>The encrypted data containing the nonce and authentication tag as well.</returns>
        public CryptoResult EncryptData(byte[] otherPublicKey, byte[] dataToEncrypt, byte[] associatedData = null, byte[] symmetricKey = null)
        {
            try
            {
                if (dataToEncrypt is null) throw new ArgumentNullException(nameof(dataToEncrypt));

                if (otherPublicKey is null) throw new ArgumentNullException(nameof(otherPublicKey));

                var bytes = new CryptoBytes(16)
                            {
                                Key = dhKeyExchange.PublicKey,
                                AssociatedData = associatedData
                            };

                random.GetBytes(bytes.Nonce);

                symmetricKey = GetSymmetricKey(otherPublicKey);

                using var aes = Aes.Create();
                aes.Key = symmetricKey;
                aes.IV = bytes.Nonce;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;

                using MemoryStream memoryStream = new MemoryStream();
                using ICryptoTransform encryptor = aes.CreateEncryptor();
                using CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

                // encrypt and write all complete blocks (final block is usually not complete -> padding mode) 
                cryptoStream.Write(dataToEncrypt, 0 , dataToEncrypt.Length);

                // write final block with the specified padding mode
                cryptoStream.FlushFinalBlock();

                // get bytes of the stream
                bytes.Cipher = memoryStream.ToArray();

                //HMAC as tag (concat Nonce/IV and cipher to ensure a unique hash every time)
                using HMACSHA256 hmac = new HMACSHA256(symmetricKey);
                bytes.Tag = hmac.ComputeHash(bytes.Nonce.Concat(bytes.Cipher).ToArray());

                var result = bytes.GetConcatenatedBytes();

                return new CryptoResult
                       {
                           Success = true,
                           ResultBytes = result,
                           AssociatedData = bytes.AssociatedData
                       };
            }
            catch (Exception e)
            {
                return new CryptoResult
                       {
                           Success = false,
                           CausingException = e
                       };
            }
        }

        /// <summary>
        ///     Computes the symmetric key using the provided Diffie Hellman key exchange provider.
        ///     The gets hashed with SHA-256 before being returned to ensure a size of 256 bits.
        /// </summary>
        /// <param name="otherPublicKey">The public key of the other communication partner.</param>
        /// <returns>A SHA-256 hashed symmetric key. That means it's size is always 32 bytes (256 bits).</returns>
        private byte[] GetSymmetricKey(byte[] otherPublicKey)
        {
            //initialize key pair if necessary
            if (dhKeyExchange.PublicKey is null) dhKeyExchange.InitNewKeys();

            // calculate the symmetric key using the Diffie Hellman key exchange
            var symmetricKey = dhKeyExchange.GetSharedSecretKey(otherPublicKey);

            // hash the symmetric key to ensure 256 bit size
            return SHA256.Create().ComputeHash(symmetricKey);
        }

        #region Interface Implementations

        /// <summary>
        ///     The public key for the Diffie-Hellman key exchange.
        ///     Use this key in the <see cref="EncryptData(byte[], byte[], byte[], byte[])" />
        ///     method of another instance of this class to encrypt data for this instance.
        /// </summary>
        public byte[] PublicKey => dhKeyExchange.PublicKey ?? throw new NullReferenceException("No public key available!");

        /// <summary>
        ///     Decrypts the data that was encrypted by the <see cref="EncryptData(byte[],byte[],byte[], byte[])" /> method.
        ///     The public key of this instance must have been used for encryption.
        /// </summary>
        /// <param name="cipherData">
        ///     The encrpyted data containing the nonce, authentication tag, the other public key and optional
        ///     associated data as well.
        /// </param>
        /// <returns>The decrypted data.</returns>
        public CryptoResult DecryptData(byte[] cipherData)
        {
            try
            {
                if (cipherData is null) throw new ArgumentNullException(nameof(cipherData));

                var bytes = CryptoBytes.SplitBytes(cipherData);

                byte[] decryptedBytes = new byte[bytes.Cipher.Length];

                byte[] symmetricKey = GetSymmetricKey(bytes.Key);

                //using AesGcm aes = new AesGcm(symmetricKey);
                //aes.Decrypt(bytes.Nonce, bytes.Cipher, bytes.Tag, decryptedData, bytes.AssociatedData);

                // check authentication tag
                using HMACSHA256 hmac = new HMACSHA256(symmetricKey);
                var computedTag = hmac.ComputeHash(bytes.Nonce.Concat(bytes.Cipher).ToArray());

                if (!computedTag.SequenceEqual(bytes.Tag)) 
                    return new CryptoResult
                           {
                               Success = false,
                               CausingException = new Exception("Authentication tag didn't match. Data might be tampered!")
                           };

                using var aes = Aes.Create();
                aes.Key = symmetricKey;
                aes.IV = bytes.Nonce;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;

                using MemoryStream memoryStream = new MemoryStream(bytes.Cipher);
                using ICryptoTransform decryptor = aes.CreateDecryptor();
                using CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

                // read and decrypt the data - buffer array gets filled with decrpyted data
                var readDataSize = cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);

                // truncate the created array to it's actual size (zeros at the end get removed)
                Array.Resize(ref decryptedBytes, readDataSize);

                return new CryptoResult
                       {
                           Success = true,
                           ResultBytes = decryptedBytes,
                           AssociatedData = bytes.AssociatedData
                       };
            }
            catch (Exception e)
            {
                return new CryptoResult
                       {
                           Success = false,
                           CausingException = e
                       };
            }
        }
        #endregion
    }
}