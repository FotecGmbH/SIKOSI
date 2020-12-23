﻿// (C) 2019 FOTEC Forschungs- und Technologietransfer GmbH
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
    /// Represents the class containing information used for logging in with the SRP protocol.
    /// </summary>
    public class SrpAuthenticationModel
    {
        /// <summary>
        /// Backing field of the <see cref="Username"/> Property.
        /// </summary>
        private string username;

        /// <summary>
        /// Backing field of the <see cref="ClientValue"/> Property.
        /// </summary>
        private byte[] clientValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SrpAuthenticationModel"/> class, initializing the client value to an empty byte array.
        /// </summary>
        public SrpAuthenticationModel()
        {
            this.ClientValue = new byte[1];
            this.Username = "null";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SrpAuthenticationModel"/> class.
        /// </summary>
        /// <param name="clientValue">The value generated by the client.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if client value is null.
        /// Is thrown if user name is null.
        /// </exception>
        public SrpAuthenticationModel(byte[] clientValue, string username)
        {
            this.ClientValue = clientValue;
            this.Username = username;
        }

        /// <summary>
        /// Gets or sets the value generated by the client.
        /// In the SRP documentation this value is refered to as value A.
        /// </summary>
        /// <value>The value generated by the client.</value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Is thrown if you attempt to set a negative value.
        /// </exception>
        public byte[] ClientValue
        {
            get
            {
                return this.clientValue;
            }

            set
            {
                this.clientValue = value ?? throw new ArgumentNullException(nameof(value), "Value must not be null.");
            }
        }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        /// <value>The user login name.</value>
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
                this.username = value ?? throw new ArgumentNullException(nameof(value), "Value must not be null.");
            }
        }
    }
}
