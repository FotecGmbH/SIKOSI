// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 18.12.2020 10:00
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using System.Globalization;
using System.IO;
using Xamarin.Forms;

namespace SIKOSI.Sample02.Helpers
{
    /// <summary>
    /// <para>Converts a byte array to an ImageSource.</para>
    /// Class ByteArrayToImageSourceConverter. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ByteArrayToImageSourceConverter : IValueConverter
    {
        /// <summary>
        /// Converts a byte array to an ImageSource.
        /// </summary>
        /// <param name="value">The byte array to convert.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter</param>
        /// <param name="culture">Culture</param>
        /// <returns>The ImageSource.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || !(value is byte[] imageAsBytes)) return null;
            
            return ImageSource.FromStream(() => new MemoryStream(imageAsBytes));
        }

        /// <summary>
        /// Not implemented! Always returns null!
        /// </summary>
        /// <param name="value">To be added.</param>
        /// <param name="targetType">To be added.</param>
        /// <param name="parameter">To be added.</param>
        /// <param name="culture">To be added.</param>
        /// <returns>To be added.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}