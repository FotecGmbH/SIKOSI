// (C) 2019 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created:        03.12.2020 16:25
// Developer:      Gregor Faiman
// Project         SIKOSI
//
// Released under GPL-3.0-only

using System.ComponentModel.DataAnnotations;

namespace SIKOSI.SampleDatabase03.Entities
{
    public class User
    {
        /// <summary>
        /// Gets or sets the users ID.
        /// </summary>
        [Key]
        public int ID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the users username.
        /// </summary>
        public string Username
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the users verifier.
        /// </summary>
        public byte[] Verifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the users salt.
        /// </summary>
        public byte[] Salt
        {
            get;
            set;
        }
    }
}
