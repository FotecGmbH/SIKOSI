// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 08.07.2020 15:04
// Entwickler      Gregor Faiman
// Projekt         SIKOSI
namespace SIKOSI.SRPShared
{
    using System;

    /// <summary>
    /// Represents the model storing data that is required when sending a message.
    /// </summary>
    public class SrpMessageModel
    {
        /// <summary>
        /// Backing field of the <see cref="Username"/> Property.
        /// </summary>
        private string username;

        /// <summary>
        /// Backing field of the <see cref="EncryptedMessage"/> Property.
        /// </summary>
        private byte[] encryptedMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="SrpMessageModel"/> class.
        /// </summary>
        public SrpMessageModel()
        {
            this.Username = "null";
            this.EncryptedMessage = new byte[1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SrpMessageModel"/> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="encryptedMessage">The encrypted message.</param>
        public SrpMessageModel(string username, byte[] encryptedMessage)
        {
            this.Username = username;
            this.EncryptedMessage = encryptedMessage;
        }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        /// <value>The user name.</value>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if value is null.
        /// </exception>
        public string Username
        {
            get
            {
                return this.username;
            }

            set
            {
                this.username = value ?? throw new ArgumentNullException(nameof(value), "Value must not be null.");
            }
        }

        /// <summary>
        /// Gets or sets the encrypted message.
        /// </summary>
        /// <value>The encrypted message.</value>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if you attempt to set null.
        /// </exception>
        public byte[] EncryptedMessage
        {
            get
            {
                return this.encryptedMessage;
            }

            set
            {
                this.encryptedMessage = value ?? throw new ArgumentNullException(nameof(value), "Value must not be null.");
            }
        }
    }
}
