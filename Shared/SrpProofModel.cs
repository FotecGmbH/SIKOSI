// (C) 2019 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created:        16.09.2020 14:18
// Developer:      Gregor Faiman
// Project         SIKOSI
//
// Released under GPL-3.0-only

using System;

namespace SIKOSI.SRPShared
{
    /// <summary>
    /// Represents the model storing data that is needed for validating the generated session key.
    /// </summary>
    public class SrpProofModel
    {
        /// <summary>
        /// Backing field of the <see cref="ClientProof"/> Property.
        /// </summary>
        private byte[] clientProof;

        /// <summary>
        /// Backing field of the <see cref="Username"/> Property.
        /// </summary>
        private string username;

        /// <summary>
        /// Initializes a new instance of the <see cref="SrpProofModel"/> class, defaulting the
        /// <see cref="ClientProof"/> Property to a byte array containing 1 zero byte.
        /// </summary>
        public SrpProofModel()
        {
            this.ClientProof = new byte[1];
            this.Username = "null";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SrpProofModel"/> class.
        /// </summary>
        /// <param name="clientProof">The proof as calculated by the client.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if ciphertext is null.
        /// </exception>
        public SrpProofModel(byte[] clientProof, string username)
        {
            this.ClientProof = clientProof;
            this.Username = username;
        }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        /// <value>The user name.</value>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if you attempt to set null.
        /// </exception>
        public string Username
        {
            get
            {
                return this.username;
            }

            set
            {
                this.username = value ?? throw new ArgumentNullException(nameof(value), "Username must not be null.");
            }
        }

        /// <summary>
        /// Gets or sets proof calculated by the client.
        /// </summary>
        /// <value>The client proof.</value>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if you attempt to set null.
        /// </exception>
        public byte[] ClientProof
        {
            get
            {
                return this.clientProof;
            }

            set
            {
                this.clientProof = value ?? throw new ArgumentNullException(nameof(value), "Cipher text must not be null.");
            }
        }
    }
}
