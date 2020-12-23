// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 03.12.2020 12:38
// Developer      Benjamin Moser
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System.Collections.Generic;
using SIKOSI.Exchange.Model;
using SIKOSI.Services.DB.Interfaces;

namespace SIKOSI.SampleDatabase02.Entities
{
    public class TblUser : ITblUserFiles
    {
        #region Properties

        /// <summary>
        ///     Emailadresse des Users
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        ///     Public Key of the User
        /// </summary>
        public byte[] PublicKey { get; set; }

        #endregion

        #region Interface Implementations

        /// <summary>
        ///     ID des Users
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Vorname des Users
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Lastname des Users
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     Benutzername des Users
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     Password Hash
        /// </summary>
        public byte[] PasswordHash { get; set; }

        /// <summary>
        ///     Password Salt
        /// </summary>
        public byte[] PasswordSalt { get; set; }

        /// <summary>
        ///     Rolle
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        ///     Dateien des Users
        /// </summary>
        public virtual ICollection<ExFile> Files { get; set; } = new List<ExFile>();

        #endregion
    }
}