using System;
using System.Collections.Generic;
using System.Text;

namespace SIKOSI.Exchange.Model
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
