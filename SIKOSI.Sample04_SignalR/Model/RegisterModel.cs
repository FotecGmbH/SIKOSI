// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 10.06.2020 10:41
// Entwickler      Manuel Fasching
// Projekt         SIKOSI


using System;

namespace SIKOSI.Sample04_SignalR.Model
{
    /// <summary>
    /// <para>Model for Registration</para>
    /// Klasse RegisterModel. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class RegisterModel
    {
        /// <summary>
        /// Firstname
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Lastname
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string EMail { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// ? Do we need this?? Confirmation Password
        /// </summary>
        public string ConfirmPassword { get; set; }

    }
}