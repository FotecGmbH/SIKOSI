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
using System.Text;
using Newtonsoft.Json;
using SIKOSI.Crypto.Interfaces;
using SIKOSI.Exchange.Model;

namespace SIKOSI.Crypto.Helper
{
    /// <summary>
    ///     <para>
    ///         This static class converts a model to an encrypted byte array and vice versa.
    ///     </para>
    ///     Klasse ModelToEncryptedBytesConverter. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class ModelToEncryptedBytesConverter
    {
        /// <summary>
        /// The default encoding.
        /// </summary>
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;

        /// <summary>
        /// Converts a model to an encrypted byte array using the specified encryption.
        /// The output is just readable for the owner of the given puplic key.
        /// </summary>
        /// <typeparam name="TModel">The type of the model that should be encrypted.</typeparam>
        /// <param name="model">The the model that should be encrypted.</param>
        /// <param name="encryption">The encryption to be used.</param>
        /// <param name="receiverPublicKey">The public key of which the owner can read/decrypt the encrypted bytes.</param>
        /// <param name="encoding">Optional - the encoding to use. If omitted, a default encoding will be used.</param>
        /// <returns>The encrypted byte array.</returns>
        public static byte[] ConvertFromModelToEncryptedByteArray<TModel>(TModel model, ISecureEncryption encryption, byte[] receiverPublicKey, Encoding encoding = null)
        {
            if (model is null) throw new ArgumentNullException(nameof(model));

            if (encryption is null) throw new ArgumentNullException(nameof(encryption));

            if (receiverPublicKey is null) throw new ArgumentNullException(nameof(receiverPublicKey));

            if (encoding is null) encoding = DefaultEncoding;

            var senderContainer = new EncryptionContainer<TModel> {Data = model, SenderPublicKey = encryption.PublicKey};

            // convert model and own public key to json
            var jsonString = JsonConvert.SerializeObject(senderContainer);

            // encrypt data
            var encryptionResult = encryption.EncryptData(receiverPublicKey, encoding.GetBytes(jsonString));

            if (!encryptionResult.Success) throw encryptionResult.CausingException;

            return encryptionResult.ResultBytes;
        }

        /// <summary>
        /// Converts an encrypted byte array to an expected model using the specified encryption.
        /// The byte array must have been created using the public key of the given encryption.
        /// </summary>
        /// <typeparam name="TModel">The expected type of model in the result.</typeparam>
        /// <param name="bytes">The encrypted bytes.</param>
        /// <param name="encryption">The encryption.</param>
        /// <param name="encoding">Optional - the encoding to use. If omitted, a default encoding will be used.</param>
        /// <returns>The resulting data and the public key of the of the sender/creator of this data.</returns>
        public static EncryptionContainer<TModel> ConvertFromEncryptedByteArrayToModel<TModel>(byte[] bytes, ISecureEncryption encryption, Encoding encoding = null)
        {
            if (bytes is null) throw new ArgumentNullException(nameof(bytes));

            if (encryption is null) throw new ArgumentNullException(nameof(encryption));

            if (encoding is null) encoding = DefaultEncoding;

            var decryptionResult = encryption.DecryptData(bytes);

            if (!decryptionResult.Success) throw decryptionResult.CausingException;

            var receiverContainer = JsonConvert.DeserializeObject<EncryptionContainer<TModel>>(encoding.GetString(decryptionResult.ResultBytes));

            return receiverContainer;
        }
    }
}