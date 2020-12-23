// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created:        14.05.2020 12:12
// Developer:      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;

namespace SIKOSI.Crypto
{
    /// <summary>
    ///     <para>
    ///         A container for the result of an encryption or decryption process.
    ///         Unicode encoding is being used to transform between bytes and string.
    ///     </para>
    ///     Class CryptoResult. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class CryptoResult
    {
        /// <summary>
        ///     The resulting encrypted or decrypted bytes.
        /// </summary>
        private byte[] resultBytes;

        /// <summary>
        ///     The resulting encrypted or decrypted string.
        /// </summary>
        private string resultString;

        #region Properties

        /// <summary>
        ///     Indicates whether the encryption or decryption was successfull.
        ///     If true <see cref="ResultBytes" /> and <see cref="ResultString" /> should not be null.
        /// </summary>
        public bool Success { get; internal set; }

        /// <summary>
        ///     The resulting encrypted or decrypted bytes.
        /// </summary>
        public byte[] ResultBytes
        {
            get => resultBytes;
            internal set
            {
                resultBytes = value;
                resultString = resultBytes.GetString();
            }
        }

        /// <summary>
        ///     The resulting encrypted or decrypted string.
        /// </summary>
        public string ResultString
        {
            get => resultString;
            internal set
            {
                resultString = value;
                resultBytes = resultString.GetBytes();
            }
        }

        /// <summary>
        /// The salt that was used if any.
        /// </summary>
        public byte[] Salt { get; internal set; }

        /// <summary>
        /// The associated data that was used if any.
        /// </summary>
        public byte[] AssociatedData { get; internal set; }

        /// <summary>
        ///     The error causing exception that was thrown during the encryption or decryption method.
        /// </summary>
        public Exception CausingException { get; internal set; }

        #endregion
    }
}