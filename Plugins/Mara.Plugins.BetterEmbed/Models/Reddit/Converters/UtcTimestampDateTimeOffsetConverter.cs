//
//  UtcTimestampDateTimeOffsetConverter.cs
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
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Mara.Plugins.BetterEmbeds.Models.Reddit.Converters
{
    /// <summary>
    /// A <see cref="JsonConverter{T}"/> responsible for converting a number of seconds since the Unix epoch to a <see cref="DateTimeOffset"/>.
    /// </summary>
    [PublicAPI]
    public sealed class UtcTimestampDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(DateTimeOffset) ||
                    typeToConvert == typeof(DateTimeOffset?);
        }

        /// <inheritdoc />
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TryGetDouble(out double value))
            {
                return DateTimeOffset.FromUnixTimeSeconds((long)value).UtcDateTime;
            }

            return DateTimeOffset.UnixEpoch.UtcDateTime;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.ToUnixTimeSeconds());
        }
    }
}
