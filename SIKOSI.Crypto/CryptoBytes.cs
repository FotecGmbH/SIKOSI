// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created:        14.05.2020 14:22
// Developer:      Roman Jahn
// Project         SIKOSI
//
// Released under GPL-3.0-only

using System;
using System.Collections.Generic;
using System.Linq;

namespace SIKOSI.Crypto
{
    /// <summary>
    /// <para>An internal class used for splitting and concatenating the bytes that gets transmitted between sender and receiver.</para>
    /// Class CryptoBytes. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    internal class CryptoBytes
    {
        /// <summary>
        /// The size of the nonce in the encrypted byte array.
        /// The nonce (IV) will be used in the AES-GCM algorithm.
        /// </summary>
        private const int DefaultNonceSize = 12;

        /// <summary>
        /// The size of the tag in the ecrypted byte array.
        /// The tag will be used for authentication of the data in the AES-GCM algorithm.
        /// </summary>
        private const int DefaultTagSize = 16;

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptoBytes"/> class.
        /// </summary>
        public CryptoBytes(int? nonceSize = null, int? tagSize = null)
        {
            Nonce = new byte[nonceSize ?? DefaultNonceSize];
            Tag = new byte[tagSize ?? DefaultTagSize];
        }

        /// <summary>
        /// The nonce/IV used in AES-GCM encryption.
        /// </summary>
        public byte[] Nonce { get; set; }

        /// <summary>
        /// The tag used in AES-GCM encryption.
        /// </summary>
        public byte[] Tag { get; set; }

        /// <summary>
        /// The key that gets transmitted.
        /// Either the sender's public key (unencrypted) when using the Diffie-Hellman key exchange or
        /// the symmetric key (encrypted with the public key of the receiver) when using RSA encryption.
        /// Leave null in symmetric encryption -> then no key gets transmitted.
        /// </summary>
        public byte[] Key { get; set; }

        /// <summary>
        /// The salt that gets transmitted.
        /// Leave null if no salt is needed -> then no salt gets transmitted.
        /// </summary>
        public byte[] Salt { get; set; }

        /// <summary>
        /// The associated data that gets transmitted.
        /// Can be an empty byte array.
        /// </summary>
        public byte[] AssociatedData { get; set; }

        /// <summary>
        /// The encrypted data.
        /// </summary>
        public byte[] Cipher { get; set; }

        /// <summary>
        /// Gets the bytes to be transmitted to the other communication partner of a RSA encryption.
        /// Concatenates the necessary bytes in following order: Tag-Nonce-EncryptedSymmetricKeyLength-EncryptedSymmetricKey-Cipher
        /// </summary>
        /// <returns>The bytes to be transmitted to the other communication partner of a RSA encryption.</returns>
        public byte[] GetConcatenatedBytes() 
        {
            // ensure key is not null
            Key ??= new byte[0];

            Salt ??= new byte[0];

            // ensure associated data is not null
            AssociatedData ??= new byte[0];

            /*Compute the length of the key and the associated data.
             * Used to save the length of the key and the length of the associated data in the transmitted bytes.
             * These arrays are always 4 bytes long.
             */
            var tagLength = BitConverter.GetBytes(Tag.Length);

            var nonceLength = BitConverter.GetBytes(Nonce.Length);

            var keyLength = BitConverter.GetBytes(Key.Length);

            var saltLength = BitConverter.GetBytes(Salt.Length);

            //get length of associated data
            var associatedDataLength = BitConverter.GetBytes(AssociatedData.Length);

            return tagLength.Concat(Tag.Concat(nonceLength.Concat(Nonce.Concat(keyLength.Concat(Key.Concat(saltLength.Concat(Salt.Concat(associatedDataLength.Concat(AssociatedData.Concat(Cipher)))))))))).ToArray();
        }

        /// <summary>
        /// Splits the received byte array (created from the <see cref="GetConcatenatedBytes"/> method) into the individual parts.
        /// </summary>
        /// <param name="encryptedBytes">The received bytes.</param>
        /// <returns>A <see cref="CryptoBytes"/> object with the bytes splitted in the individual parts.</returns>
        public static CryptoBytes SplitBytes(IEnumerable<byte> encryptedBytes)
        {
            int seeker = 0;

            // Get length of tag
            var tagLength = BitConverter.ToInt32(encryptedBytes.Take(4).ToArray());
            seeker += 4;
            var tag = encryptedBytes.Skip(seeker).Take(tagLength).ToArray();
            seeker += tagLength;
            
            // Get length of the nonce
            var nonceLength = BitConverter.ToInt32(encryptedBytes.Skip(seeker).Take(4).ToArray());
            seeker += 4;
            var nonce = encryptedBytes.Skip(seeker).Take(nonceLength).ToArray();
            seeker += nonceLength;

            // Get length of the key.
            var keyLength = BitConverter.ToInt32(encryptedBytes.Skip(seeker).Take(4).ToArray());
            seeker += 4;
            var key = encryptedBytes.Skip(seeker).Take(keyLength).ToArray();
            seeker += keyLength;

            // Get length of the salt.
            var saltLength = BitConverter.ToInt32(encryptedBytes.Skip(seeker).Take(4).ToArray());
            seeker += 4;
            var salt = encryptedBytes.Skip(seeker).Take(saltLength).ToArray();
            seeker += saltLength;

            // Get length of the associated data.
            var associatedDataLength = BitConverter.ToInt32(encryptedBytes.Skip(seeker).Take(4).ToArray());
            seeker += 4;
            var associatedData = encryptedBytes.Skip(seeker).Take(associatedDataLength).ToArray();
            seeker += associatedDataLength;

            var cipher = encryptedBytes.Skip(seeker).ToArray();

            return new CryptoBytes
                   {
                       Tag = tag,
                       Nonce = nonce,
                       Key = key,
                       Salt = salt,
                       AssociatedData = associatedData,
                       Cipher = cipher
                   };
        }
    }
}