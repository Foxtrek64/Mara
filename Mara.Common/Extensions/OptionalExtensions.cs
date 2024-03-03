//
//  OptionalExtensions.cs
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
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Mara.Common.Extensions
{
    /// <summary>
    /// Provides a set of extensions and helpers for Optional{T} and related types.
    /// </summary>
    [PublicAPI]
    public static class OptionalExtensions
    {
        /// <summary>
        /// Enumerates handling for string to Optional{string}.
        /// </summary>
        public enum StringHandlingOptions
        {
            /// <summary>
            /// If the string is null or empty, treat the value of the optional as null.
            /// </summary>
            NullOrEmptyIsNull,

            /// <summary>
            /// If the string is null or empty, treat the value of the optional as empty.
            /// </summary>
            NullOrEmptyIsEmpty,

            /// <summary>
            /// If the string is null or white space, treat the value of the optional as null.
            /// </summary>
            NullOrWhiteSpaceIsNull,

            /// <summary>
            /// If the string is null or white space, treat the value of the optional as empty.
            /// </summary>
            NullOrWhiteSpaceIsEmpty
        }

        /// <summary>
        /// Provides an operator for converting a <see cref="string"/> to an <see cref="Optional{TValue}"/>.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="options">Options controlling how to convert the string.</param>
        /// <returns>An <see cref="Optional{TValue}"/> representing the original value.</returns>
        public static Optional<string> AsOptional(this string value, StringHandlingOptions options = StringHandlingOptions.NullOrEmptyIsEmpty)
        {
            return options switch
            {
                StringHandlingOptions.NullOrEmptyIsNull when string.IsNullOrEmpty(value) => new Optional<string>(value),
                StringHandlingOptions.NullOrEmptyIsEmpty when string.IsNullOrEmpty(value) => default,
                StringHandlingOptions.NullOrWhiteSpaceIsNull when string.IsNullOrWhiteSpace(value) => new Optional<string>(value),
                StringHandlingOptions.NullOrWhiteSpaceIsEmpty when string.IsNullOrWhiteSpace(value) => default,
                _ when !string.IsNullOrWhiteSpace(value) => value.AsOptional(),
                _ => throw new ArgumentOutOfRangeException(nameof(options), options, "StringHandlingOptions value must be enumerated")
            };
        }
    }
}
