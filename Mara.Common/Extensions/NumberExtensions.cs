//
//  NumberExtensions.cs
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
using System.Numerics;
using JetBrains.Annotations;

namespace Mara.Common.Extensions
{
    /// <summary>
    /// Provides a set of extensions for <see cref="INumber{TSelf}"/>.
    /// </summary>
    [PublicAPI]
    public static class NumberExtensions
    {
        /// <summary>
        /// Safely divides two numbers of the same type.
        /// </summary>
        /// <param name="numerator">The numerator.</param>
        /// <param name="denominator">The denominator.</param>
        /// <param name="fallback">If the denominator is <see cref="INumberBase{TSelf}.Zero"/>, return this number instead.</param>
        /// <typeparam name="TNumber">The type of number to divide.</typeparam>
        /// <returns>The dividend.</returns>
        public static TNumber? SafeDivide<TNumber>(TNumber numerator, TNumber denominator, TNumber? fallback = default)
            where TNumber : INumber<TNumber>
        {
            return denominator == TNumber.Zero
                ? fallback
                : numerator / denominator;
        }

        /// <summary>
        /// Safely divides two numbers of the same type, returning a percentage as a string.
        /// </summary>
        /// <param name="numerator">The numerator.</param>
        /// <param name="denominator">The denominator.</param>
        /// <param name="fallback">If the denominator is <see cref="INumberBase{TSelf}.Zero"/>, return this value instead.</param>
        /// <param name="format">The format string provided to <see cref="object.ToString()"/>.</param>
        /// <param name="formatProvider">The format provider provided to <see cref="object.ToString()"/>.</param>
        /// <typeparam name="TNumber">The type of number to divide.</typeparam>
        /// <returns>A string representation of the percentage value.</returns>
        public static string SafeDividePercentage<TNumber>
        (
            TNumber numerator,
            TNumber denominator,
            string fallback,
            string? format = "P",
            IFormatProvider? formatProvider = null
        )
            where TNumber : INumber<TNumber>
        {
            return denominator == TNumber.Zero
                ? fallback
                : (numerator / denominator).ToString(format, formatProvider);
        }
    }
}
