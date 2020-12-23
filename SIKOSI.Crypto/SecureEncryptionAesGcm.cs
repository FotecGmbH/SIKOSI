// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 27.11.2020 10:10
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using SIKOSI.Crypto.Interfaces;

namespace SIKOSI.Crypto
{
    /// <summary>
    /// <para>
    /// This class provides secure encryption with AES-GCM symmetric encryption.
    /// </para>
    /// Klasse SecureEncryptionAesGcm. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class SecureEncryptionAesGcm : ISecureSymmetricEncryption
    {
        /// <summary>
        ///     The random number generator.
        /// </summary>
        private readonly RandomNumberGenerator random;

        /// <summary>
        /// The used encoding.
        /// </summary>
        private readonly Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// Initalizes a new instance of the <see cref="SecureEncryptionAesGcm" /> class.
        /// </summary>
        public SecureEncryptionAesGcm()
        {
            random = RandomNumberGenerator.Create();
        }

        /// <summary>
        /// Creates a key from a given password and salt.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="salt">The salt.</param>
        /// <returns>The key.</returns>
        private byte[] CreateKeyFromPassword(string password, byte[] salt)
        {
            // get bytes from password
            byte[] passwordBytes = encoding.GetBytes(password);

            var rfc = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);

            return rfc.GetBytes(32);
        }

        /// <summary>
        /// Ensures that a salt is not null.
        /// If null, it salt is then a random 32 byte array.
        /// </summary>
        /// <param name="salt">The salt.</param>
        /// <returns>Returns the given salt if not null, a random 32 byte array otherwise.</returns>
        private byte[] EnsureSalt(byte[] salt)
        {
            if (salt is null)
            {
                salt = new byte[32];
                random.GetBytes(salt);
            }

            return salt;
        }

        #region Interface Implementations

        /// <summary>
        /// Decrypts the given cipher data using AES-GCM with the given symmetric key. 
        /// </summary>
        /// <param name="cipherData">The data to decrypt.</param>
        /// <param name="symmetricKey">The symmetric key used to encrypt the data.</param>
        /// <returns>The result of the process of decryption.</returns>
        public CryptoResult DecryptData(byte[] cipherData, byte[] symmetricKey)
        {
            try
            {
                if (cipherData is null) throw new ArgumentNullException(nameof(cipherData));

                if (symmetricKey is null) throw new ArgumentNullException(nameof(symmetricKey));

                if (symmetricKey.Length != 16 && symmetricKey.Length != 24 && symmetricKey.Length != 32)
                    throw new ArgumentException("Symmetric key must be of length 16, 24 or 32 bytes.",
                        nameof(symmetricKey));

                var bytes = CryptoBytes.SplitBytes(cipherData);

                byte[] decryptedData = new byte[bytes.Cipher.Length];

                using AesGcm aes = new AesGcm(symmetricKey);
                aes.Decrypt(bytes.Nonce, bytes.Cipher, bytes.Tag, decryptedData, bytes.AssociatedData);

                return new CryptoResult
                       {
                           Success = true,
                           ResultBytes = decryptedData,
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
        /// Decrypts the given cipher data using AES-GCM with the given password. 
        /// </summary>
        /// <param name="cipherData">The data to decrypt.</param>
        /// <param name="password">The password to use for encryption.</param>
        /// <returns>The result of the process of decryption.</returns>
        public CryptoResult DecryptDataWithPassword(byte[] cipherData, string password)
        {
            try
            {
                if (cipherData is null) throw new ArgumentNullException(nameof(cipherData));

                if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException($"'{nameof(password)}' cannot be null or whitespace", nameof(password));

                var bytes = CryptoBytes.SplitBytes(cipherData);

                var usedSalt = bytes.Salt;

                var key = CreateKeyFromPassword(password, usedSalt);

                var result = DecryptData(cipherData, key);
                result.Salt = usedSalt;
                return result;
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
        /// Encrypts the given data using AES-GCM with the given symmetric key.
        /// </summary>
        /// <param name="dataToEncrypt">The data to encrypt.</param>
        /// <param name="symmetricKey">The symmetric key to be used for encryption.</param>
        /// <param name="associatedData">The associated data - optional parameter.</param>
        /// <returns>The result of the process of encryption.</returns>
        public CryptoResult EncryptData(byte[] dataToEncrypt, byte[] symmetricKey, byte[] associatedData = null)
        {
            try
            {
                if (dataToEncrypt is null) throw new ArgumentNullException(nameof(dataToEncrypt));

                if (symmetricKey is null) throw new ArgumentNullException(nameof(symmetricKey));

                if (symmetricKey.Length != 16 && symmetricKey.Length != 24 && symmetricKey.Length != 32)
                    throw new ArgumentException("Symmetric key must be of length 16, 24 or 32 bytes.",
                        nameof(symmetricKey));

                var bytes = new CryptoBytes
                            {
                                Cipher = new byte[dataToEncrypt.Length],
                                AssociatedData = associatedData,
                            };

                random.GetBytes(bytes.Nonce);

                try
                {
                    using var aes = new AesGcm(symmetricKey);

                    aes.Encrypt(bytes.Nonce, dataToEncrypt, bytes.Cipher, bytes.Tag, bytes.AssociatedData);

                    var result = bytes.GetConcatenatedBytes();

                    return new CryptoResult
                           {
                               Success = true,
                               ResultBytes = result,
                               AssociatedData = bytes.AssociatedData
                           };
                }
                catch (PlatformNotSupportedException)
                {
                    using var aes = Aes.Create();

                    bytes = new CryptoBytes(16)
                            {
                                AssociatedData = associatedData
                            };

                    random.GetBytes(bytes.Nonce);

                    aes.Key = symmetricKey;
                    aes.IV = bytes.Nonce;

                    using MemoryStream memoryStream = new MemoryStream();
                    using (ICryptoTransform encryptor = aes.CreateEncryptor())
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(encoding.GetString(dataToEncrypt));
                    }

                    bytes.Cipher = memoryStream.ToArray();

                    var result = bytes.GetConcatenatedBytes();

                    return  new CryptoResult
                           {
                               Success = true,
                               ResultBytes = result,
                               AssociatedData = bytes.AssociatedData
                           };
                }
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
        /// Encrypts the given data using AES-GCM with the given password. 
        /// </summary>
        /// <param name="dataToEncrypt">The data to encrypt.</param>
        /// <param name="password">The password used for encryption.</param>
        /// <param name="passwordSalt">Optional - Salt can be provided. If omitted, a random salt will be used.</param>
        /// <param name="associatedData">Optional - The associated data.</param>
        /// <returns>The result of the process of encryption.</returns>
        public CryptoResult EncryptDataWithPassword(byte[] dataToEncrypt, string password, byte[] passwordSalt = null, byte[] associatedData = null)
        {
            try
            {
                if (dataToEncrypt is null) throw new ArgumentNullException(nameof(dataToEncrypt));

                if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException($"'{nameof(password)}' cannot be null or whitespace", nameof(password));

                var usedSalt = EnsureSalt(passwordSalt);

                var key = CreateKeyFromPassword(password, usedSalt);

                var result = EncryptData(dataToEncrypt, key, associatedData);

                if (!result.Success) return result;

                // add used salt into resultBytes
                var bytes = CryptoBytes.SplitBytes(result.ResultBytes);
                bytes.Salt = usedSalt;
                result.ResultBytes = bytes.GetConcatenatedBytes();

                result.Salt = usedSalt;

                return result;
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