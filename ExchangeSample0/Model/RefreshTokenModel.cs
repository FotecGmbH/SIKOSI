// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 24.11.2020 11:23
// Entwickler      Manuel Fasching
// Projekt         SIKOSI

using System;

namespace SIKOSI.Exchange.Model
{
    /// <summary>
    /// <para>Refresh Token</para>
    /// Klasse RefreshTokenModel. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class RefreshTokenModel
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