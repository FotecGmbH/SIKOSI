// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 22.06.2020 14:26
// Entwickler      Manuel Fasching
// Projekt         SIKOSI


using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SIKOSI.SampleDatabase01.Entities
{
    [Table("RefreshToken")]
    public class TblRefreshToken
    {
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Date of Expiration
        /// </summary>
        public DateTime Expires { get; set; }
        /// <summary>
        /// Token is expired
        /// </summary>
        public bool IsExpired => DateTime.UtcNow >= Expires;

        /// <summary>
        /// Date of creation
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// IP which has created the token
        /// </summary>
        public string CreatedByIp { get; set; }

        /// <summary>
        /// Token is revoked
        /// </summary>
        public DateTime? Revoked { get; set; }

        /// <summary>
        /// IP of the revoked request
        /// </summary>
        public string RevokedByIp { get; set; }

        /// <summary>
        /// Token gets replayed by
        /// </summary>
        public string ReplacedByToken { get; set; }

        /// <summary>
        /// Is Revoked
        /// </summary>
        public bool IsActive => Revoked == null && !IsExpired;
    }
}