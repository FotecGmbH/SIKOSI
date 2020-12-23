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
    /// Interface to provide secure encryption, decryption and own public key that can be used to encrypt data for this instance.
    /// </summary>
    public interface ISecureEncryption
    {
        #region Properties

        /// <summary>
        /// The public key.
        /// </summary>
        byte[] PublicKey { get; }

        #endregion

        /// <summary>
        /// Decrypts the given cipher data.
        /// </summary>
        /// <param name="cipherData">The cipher data.</param>
        /// <returns>The result of the process of decyption.</returns>
        CryptoResult DecryptData(byte[] cipherData);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherPublicKey">The public key of the recipient of the data.</param>
        /// <param name="dataToEncrypt">The data to encrypt.</param>
        /// <param name="associatedData">Optional -the associated data.</param>
        /// <param name="symmetricKey">Optional - the symmetric key.</param>
        /// <returns>The result of the process of encyption.</returns>
        CryptoResult EncryptData(byte[] otherPublicKey, byte[] dataToEncrypt, byte[] associatedData = null, byte[] symmetricKey = null);
    }
}