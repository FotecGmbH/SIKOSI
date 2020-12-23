// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 10.06.2020 10:12
// Entwickler      Manuel Fasching
// Projekt         SIKOSI


using System;

namespace Sample02_SignalRClient.Model
{
    /// <summary>
    /// <para>Model for Authentication</para>
    /// Klasse AuthenticateModel. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class AuthenticateModel
    {
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
    }
}