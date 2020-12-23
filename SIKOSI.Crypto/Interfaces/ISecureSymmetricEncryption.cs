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

namespace SIKOSI.Crypto.Interfaces
{
    /// <summary>
    /// Interface to provide secure symmetric encryption and decryption with a given symmetric key or password.
    /// </summary>
    public interface ISecureSymmetricEncryption
    {
        /// <summary>
        /// Decrypts the given cipher data with the given symmetric key.
        /// </summary>
        /// <param name="cipherData">The cipher data to decrypt.</param>
        /// <param name="symmetricKey">The symmetric key that was also used while encryption.</param>
        /// <returns>The result of the process of decryption.</returns>
        CryptoResult DecryptData(byte[] cipherData, byte[] symmetricKey);

        /// <summary>
        /// Encrypts the given data with the given symmetric key.
        /// </summary>
        /// <param name="dataToEncrypt">The data to encrypt.</param>
        /// <param name="symmetricKey">The symmetric key that must also be used while decryption.</param>
        /// <param name="associatedData">The associated data.</param>
        /// <returns>The result of the process of encryption.</returns>
        CryptoResult EncryptData(byte[] dataToEncrypt, byte[] symmetricKey, byte[] associatedData = null);

        /// <summary>
        /// Encrypts the given data with the given password.
        /// </summary>
        /// <param name="dataToEncrypt">The data to encrypt.</param>
        /// <param name="password">The password for encryption.</param>
        /// <param name="passwordSalt">Optional - the salt.</param>
        /// <param name="associatedData">Optional - the associated data.</param>
        /// <returns>The result of the process of encryption.</returns>
        CryptoResult EncryptDataWithPassword(byte[] dataToEncrypt, string password, byte[] passwordSalt = null, byte[] associatedData = null);

        /// <summary>
        /// Encrypts the given cipher data using the given password.
        /// </summary>
        /// <param name="cipherData">The cipher data to be decrypted.</param>
        /// <param name="password">The password used while encryption.</param>
        /// <returns>The result of the process of decryption.</returns>
        CryptoResult DecryptDataWithPassword(byte[] cipherData, string password);
    }
}