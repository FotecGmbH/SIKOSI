// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 18.12.2020 14:41
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace SIKOSI.Sample02.Helpers
{
    /// <summary>
    /// <para>Converts an IEnumerable to a bool wheter it should be visible.</para>
    /// Class ItemsToVisibilityConverter. (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class ItemsToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts an IEnumerable to a bool wheter it should be visible
        /// </summary>
        /// <param name="value">The items.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter</param>
        /// <param name="culture">Culture</param>
        /// <returns>A bool for visibilty of the items</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<object> items) return items.Any();

            return false;
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