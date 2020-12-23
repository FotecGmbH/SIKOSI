// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 09.06.2020 13:41
// Developer      Manuel Fasching
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

namespace SIKOSI.Sample01_Auth.Helpers
{
    /// <summary>
    ///     <para>Application Settings</para>
    ///     Klasse AppSettings. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class AppSettings
    {
        #region Properties

        /// <summary>
        ///     Schlüssel
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        ///     Gültigkeit des Tokens in Minuten
        /// </summary>
        public int TokenExpirationTimeout { get; set; }

        #endregion
    }
}