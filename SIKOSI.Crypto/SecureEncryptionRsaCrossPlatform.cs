// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 15.12.2020 13:51
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SIKOSI.Crypto.Interfaces;

namespace SIKOSI.Crypto
{
    /// <summary>
    ///     <para>
    ///         This class provides secure encryption using AES-GCM for symmetric encryption.
    ///         The used symmetric key gets transmitted being encrypted by RSA.
    ///     </para>
    ///     Klasse SecureEncryptionRsaCrossPlatform. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class SecureEncryptionRsaCrossPlatform : ISecureEncryption
    {
        /// <summary>
        ///     The size of the RSA key in bits.
        /// </summary>
        private readonly int keySizeInBits;

        /// <summary>
        ///     The random number generator.
        /// </summary>
        private readonly RandomNumberGenerator random;

        /// <summary>
        ///     The RSA provider.
        /// </summary>
        private readonly RSACryptoServiceProvider rsaProvider;

        /// <summary>
        ///     Initalizes a new instance of the <see cref="SecureEncryptionRsa" /> class.
        /// </summary>
        /// <param name="keySizeInBits">
        ///     The key size in bits. Must be between 1024 and 16384 bits in increments of 8 bits.
        /// </param>
        public SecureEncryptionRsaCrossPlatform(int keySizeInBits = 2048)
        {
            if (keySizeInBits < 1024) throw new ArgumentException("Key size must be at least 1024 bits!", nameof(keySizeInBits));

            rsaProvider = new RSACryptoServiceProvider(keySizeInBits);
            var xmlString = rsaProvider.ToXmlString(false);
            PublicKey = Encoding.UTF8.GetBytes(xmlString);

            this.keySizeInBits = keySizeInBits;
            random = RandomNumberGenerator.Create();
        }

        /// <summary>
        ///     Encrypts data only decryptable for the owner of the specified public key.
        /// </summary>
        /// <param name="otherPublicKey">The RSA public key of the other communication partner.</param>
        /// <param name="dataToEncrypt">The data to encrypt.</param>
        /// <param name="associatedData">The optional associated data (for AES encryption).</param>
        /// <param name="symmetricKey">
        ///     The optional symmetric key (for AES encryption).
        ///     If specified, make sure it's 32 bytes long and truly random!
        ///     If omitted or null a random key gets created.
        /// </param>
        /// <returns>The encrypted data.</returns>
        /// <returns></returns>
        public CryptoResult EncryptData(byte[] otherPublicKey, string dataToEncrypt, byte[] associatedData = null, byte[] symmetricKey = null)
        {
            return EncryptData(otherPublicKey, dataToEncrypt.GetBytes(), associatedData, symmetricKey);
        }

        /// <summary>
        ///     Encrypts the symmetric key (or any byte array) using the specified public key of the recipient.
        /// </summary>
        /// <param name="otherPublicKey">The public key of the recipient.</param>
        /// <param name="symmetricKey">The symmetric key to encrypt. Could be any kind of data as byte array.</param>
        /// <returns>The encrypted byte array.</returns>
        private byte[] GetEncryptedSymmetricKey(byte[] otherPublicKey, byte[] symmetricKey)
        {
            using var encRsa = new RSACryptoServiceProvider(keySizeInBits);

            var xmlString = Encoding.UTF8.GetString(otherPublicKey);
            encRsa.FromXmlString(xmlString);

            return encRsa.Encrypt(symmetricKey, true);
        }

        /// <summary>
        ///     Decrypts the given data (here: the encrypted symmetric key).
        /// </summary>
        /// <param name="encryptedSymmetricKey">The encrypted data (here: the encrypted symmetric key).</param>
        /// <returns>The decrypted byte array.</returns>
        private byte[] GetDecryptedSymmetricKey(byte[] encryptedSymmetricKey)
        {
            return rsaProvider.Decrypt(encryptedSymmetricKey, true);
        }

        #region Interface Implementations

        /// <summary>
        ///     The public key of the RSA encryption.
        /// </summary>
        public byte[] PublicKey { get; }

        /// <summary>
        ///     Encrypts data only decryptable for the owner of the specified public key.
        /// </summary>
        /// <param name="otherPublicKey">The RSA public key of the other communication partner.</param>
        /// <param name="dataToEncrypt">The data to encrypt.</param>
        /// <param name="associatedData">The optional associated data (for AES encryption).</param>
        /// <param name="symmetricKey">
        ///     The optional symmetric key (for AES encryption).
        ///     If specified, make sure it's 16, 24 or 32 bytes long and truly random!
        ///     If omitted or null a random key with 32 bytes gets created.
        /// </param>
        /// <returns>The encrypted data.</returns>
        public CryptoResult EncryptData(byte[] otherPublicKey, byte[] dataToEncrypt, byte[] associatedData = null, byte[] symmetricKey = null)
        {
            try
            {
                if (dataToEncrypt is null) throw new ArgumentNullException(nameof(dataToEncrypt));

                if (otherPublicKey is null) throw new ArgumentNullException(nameof(otherPublicKey));

                if (symmetricKey is null)
                {
                    symmetricKey = new byte[32];
                    random.GetBytes(symmetricKey);
                }
                else
                {
                    if (symmetricKey.Length != 16 && symmetricKey.Length != 24 && symmetricKey.Length != 32)
                        throw new ArgumentException("If symmetric key is specified it must be of length 16, 24 or 32 bytes.",
                            nameof(symmetricKey));
                }

                var bytes = new CryptoBytes(16)
                            {
                                Cipher = new byte[dataToEncrypt.Length],
                                AssociatedData = associatedData
                            };

                random.GetBytes(bytes.Nonce);

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

                bytes.Key = GetEncryptedSymmetricKey(otherPublicKey, symmetricKey);

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
        ///     Decrypts data that was encrpyted with the public key of this instance.
        /// </summary>
        /// <param name="cipherData">The encrpted data.</param>
        /// <returns>The decrypted data.</returns>
        public CryptoResult DecryptData(byte[] cipherData)
        {
            try
            {
                if (cipherData is null) throw new ArgumentNullException(nameof(cipherData));

                var bytes = CryptoBytes.SplitBytes(cipherData);

                var symmetricKey = GetDecryptedSymmetricKey(bytes.Key);

                var decryptedBytes = new byte[bytes.Cipher.Length];

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