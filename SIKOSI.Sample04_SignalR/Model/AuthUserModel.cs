// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 10.06.2020 10:23
// Entwickler      Manuel Fasching
// Projekt         SIKOSI


using System;

namespace SIKOSI.Sample04_SignalR.Model
{
    /// <summary>
    /// <para>Authenticated User</para>
    /// Klasse AuthUserModel. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class AuthUserModel
    {
        /// <summary>
        /// User ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User-Name
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// First-Name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last-Name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Refresh Token
        /// </summary>
        public string RefreshToken { get; set; }
    }
}