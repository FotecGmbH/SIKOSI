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
    /// Model for the SRP registration containing values used for registration.
    /// </summary>
    public class SrpRegistrationModel
    {
        /// <summary>
        /// Backing field of the <see cref="VerifierBytes"/> Property.
        /// </summary>
        private byte[] verifierBytes;

        /// <summary>
        /// Backing field of the <see cref="Username"/> Property.
        /// </summary>
        private string username;

        /// <summary>
        /// Backing field of the <see cref="SaltBytes"/> Property.
        /// </summary>
        private byte[] salt;

        /// <summary>
        /// Initializes a new instance of the <see cref="SrpRegistrationModel"/> class.
        /// </summary>
        public SrpRegistrationModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SrpRegistrationModel"/> class.
        /// </summary>
        /// <param name="username">The user name.</param>
        /// <param name="salt">The user salt.</param>
        /// <param name="verifier">The user verifier.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if either of the parameters are null.
        /// </exception>
        public SrpRegistrationModel(string username, byte[] salt, byte[] verifier)
        {
            this.Username = username;
            this.VerifierBytes = verifier;
            this.SaltBytes = salt;
        }

        /// <summary>
        /// Gets or sets the user verifier.
        /// </summary>
        /// <value>The user verifier.</value>
        /// <exception cref="ArgumentNullException">
        /// Thrown if value is null.
        /// </exception>
        public byte[] VerifierBytes
        {
            get
            {
                return this.verifierBytes;
            }

            set
            {
                this.verifierBytes = value ?? throw new ArgumentNullException(nameof(value), "Verifier must not be null");
            }
        }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        /// <value>The user name.</value>
        /// <exception cref="ArgumentNullException">
        /// Thrown if value is null.
        /// </exception>
        public string Username
        {
            get
            {
                return this.username;
            }

            set
            {
                this.username = value ?? throw new ArgumentNullException(nameof(value), "User name must not be null");
            }
        }

        /// <summary>
        /// Gets or sets the user salt.
        /// </summary>
        /// <value>The user salt.</value>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if salt is null.
        /// </exception>
        public byte[] SaltBytes
        {
            get
            {
                return this.salt;
            }

            set
            {
                this.salt = value ?? throw new ArgumentNullException(nameof(value), "Salt must not be null");
            }
        }
    }
}
