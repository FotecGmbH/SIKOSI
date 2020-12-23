// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created:        14.05.2020 12:36
// Developer:      Roman Jahn
// Project         SIKOSI
//
// Released under GPL-3.0-only

using System.Text;

namespace SIKOSI.Crypto
{
    /// <summary>
    /// <para>Class for extension methods.</para>
    /// Class Extensions. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Converts a byte array into a string using Unicode encoding (UTF-16).
        /// </summary>
        /// <param name="bytes">The bytes to be converted.</param>
        /// <returns>The converted string.</returns>
        public static string GetString(this byte[] bytes)
        {
            return Encoding.Unicode.GetString(bytes);
        }

        /// <summary>
        /// Converts a string into a byte array using Unicode encoding (UTF-16).
        /// </summary>
        /// <param name="text">The string to be converted.</param>
        /// <returns>The converted bytes.</returns>
        public static byte[] GetBytes(this string text)
        {
            return Encoding.Unicode.GetBytes(text);
        }
    }
}