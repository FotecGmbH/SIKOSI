// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 24.11.2020 09:55
// Entwickler      Manuel Fasching
// Projekt         SIKOSI

using System;

namespace SIKOSI.Exchange.Interfaces
{
    public interface IUser
    {
        #region Properties

        /// <summary>
        ///     User ID
        /// </summary>
        int Id { get; set; }

        /// <summary>
        ///     FirstName
        /// </summary>
        string FirstName { get; set; }

        /// <summary>
        ///     LastName
        /// </summary>
        string LastName { get; set; }

        /// <summary>
        ///     Username
        /// </summary>
        string Username { get; set; }

        /// <summary>
        /// User Role
        /// </summary>
        string Role { get; set; }

        #endregion
    }
}