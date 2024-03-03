//
//  StringLocalizerExtensions.cs
//
//  Author:
//       LuzFaltex Contributors <support@luzfaltex.com>
//
//  Copyright (c) LuzFaltex, LLC.
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mara.Common.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Mara.Common.Extensions
{
    /// <summary>
    /// Provides extensions for <see cref="ResourceManagerStringLocalizer"/>.
    /// </summary>
    public static class StringLocalizerExtensions
    {
        /// <summary>
        /// Allows users to retrieve a localized string with the specified culture.
        /// </summary>
        /// <param name="stringLocalizer">The localizer to extend.</param>
        /// <param name="key">The key of the phrase to retrieve.</param>
        /// <param name="culture">The culture for the phrase to retrieve..</param>
        /// <returns>A <see cref="LocalizedString"/> with the requested value.</returns>
        /// <exception cref="ArgumentNullException">If any of the values are null.</exception>
        /// <exception cref="InvalidOperationException">If <see cref="ResourceManagerStringLocalizer.GetStringSafely(string, CultureInfo?)"/> is removed.</exception>
        public static LocalizedString GetStringWithCulture(this ResourceManagerStringLocalizer stringLocalizer, string key, CultureInfo culture)
        {
            ArgumentNullException.ThrowIfNull(stringLocalizer, nameof(stringLocalizer));
            ArgumentNullException.ThrowIfNull(key, nameof(key));
            ArgumentNullException.ThrowIfNull(culture, nameof(culture));

            MethodInfo? getStringSafely = typeof(ResourceManagerStringLocalizer)
                                                        .GetMethod
                                                        (
                                                            "GetStringSafely",
                                                            BindingFlags.NonPublic
                                                        );

            FieldInfo? resourceBaseName = typeof(ResourceManagerStringLocalizer).GetField("_resourceBaseName");

            if (getStringSafely is null || resourceBaseName is null)
            {
                throw new InvalidOperationException();
            }

#pragma warning disable SA1010
            string? value = Unsafe.As<string>(getStringSafely.Invoke(stringLocalizer, [key, culture]));
#pragma warning restore SA1010
            string? searchedLocation = Unsafe.As<string>(resourceBaseName.GetValue(stringLocalizer));
            return new LocalizedString(key, value ?? key, resourceNotFound: value == null, searchedLocation: searchedLocation);
        }
    }
}
