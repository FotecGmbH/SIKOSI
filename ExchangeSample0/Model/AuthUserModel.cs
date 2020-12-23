using System;
using System.Collections.Generic;
using System.Text;
using SIKOSI.Exchange.Interfaces;

namespace SIKOSI.Exchange.Model
{
    /// <summary>
    /// <para>Authenticated User</para>
    /// Klasse AuthUserModel. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class AuthUserModel : IToken
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
