// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 11.12.2020 15:24
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using SIKOSI.Crypto.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SIKOSI.Crypto
{
    /// <summary>
    /// <para>
    ///     This class provides secure encryption using AES-CBC for symmetric encryption and HMACSHA512 for authentication.
    ///     Use this class across different platforms.</para>
    ///     Class SecureEncryptionAesCrossPlattform. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class SecureEncryptionAesCrossPlatform : ISecureSymmetricEncryption
    {
        /// <summary>
        ///     The random number generator.
        /// </summary>
        private readonly RandomNumberGenerator random;

        /// <summary>
        /// The used encoding.
        /// </summary>
        private readonly Encoding encoding = Encoding.UTF8;

        public SecureEncryptionAesCrossPlatform()
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

                // create an array to write in the decrypted data - might be too large due to padding (gets truncated after decryption)
                var decryptedBytes = new byte[bytes.Cipher.Length];

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

        public CryptoResult EncryptData(byte[] dataToEncrypt, byte[] symmetricKey, byte[] associatedData = null)
        {
            try
            {
                if (dataToEncrypt is null) throw new ArgumentNullException(nameof(dataToEncrypt));

                if (symmetricKey is null) throw new ArgumentNullException(nameof(symmetricKey));

                if (symmetricKey.Length != 16 && symmetricKey.Length != 24 && symmetricKey.Length != 32)
                    throw new ArgumentException("Symmetric key must be of length 16, 24 or 32 bytes.",
                        nameof(symmetricKey));

                using var aes = Aes.Create();

                var bytes = new CryptoBytes(16)
                            {
                                AssociatedData = associatedData
                            };
                        
                random.GetBytes(bytes.Nonce);

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

                return  new CryptoResult
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
    }
}