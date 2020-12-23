// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 18.05.2020 11:36
// Entwickler      Manuel Fasching
// Projekt         SIKOSI

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using SIKOSI.Services.DB.Interfaces;

namespace Sample0_Basic.Entities
{
    /// <summary>
    ///     User
    /// </summary>
    [Table("User")]
    public class TblUser : ITblUser
    {
        #region Properties

        ///// <summary>
        /////     Date when the user account was created
        ///// </summary>
        //public DateTime CreatedAt { get; set; }

        ///// <summary>
        /////     Date when the user account was last updated
        ///// </summary>
        //public DateTime? UpdatedAt { get; set; }

        ///// <summary>
        /////     Date of the last login
        ///// </summary>
        //public DateTime? LastLoginAt { get; set; }

        ///// <summary>
        /////     Date of last failed login attempt
        ///// </summary>
        //public DateTime? LoginFailedAt { get; set; }

        ///// <summary>
        /////     Count of failed login attempts
        ///// </summary>
        //public int LoginFailedCount { get; set; }

        ///// <summary>
        /////     Aktiv or not
        ///// </summary>
        //public bool IsActive { get; set; }

        /// <summary>
        ///     Password Hash
        /// </summary>
        [JsonIgnore] // WICHTIG! von System.Text.Json.Serialization und NICHT Newtonsoft
        public byte[] PasswordHash { get; set; }

        /// <summary>
        ///     Password Salt
        /// </summary>
        [JsonIgnore]
        public byte[] PasswordSalt { get; set; }

        //[JsonIgnore]
        //public List<TblRefreshToken> RefreshTokens { get; set; }

        #endregion

        #region Interface Implementations

        /// <summary>
        ///     Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     Firstname
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Lastname
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// User Role -> Default Member
        /// </summary>
        public string Role { get; set; } = "Member";

        #endregion
    }
}