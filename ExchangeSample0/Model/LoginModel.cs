// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 24.11.2020 11:22
// Entwickler      Manuel Fasching
// Projekt         SIKOSI

using System;

namespace SIKOSI.Exchange.Model
{
    /// <summary>
    /// <para>DESCRIPTION</para>
    /// Klasse LoginModel. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// User
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
    }
}