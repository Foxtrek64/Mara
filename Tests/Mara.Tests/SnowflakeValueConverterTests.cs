//
//  SnowflakeValueConverterTests.cs
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

using Mara.Common.ValueConverters;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Remora.Rest.Core;
using Xunit;

using DiscordConstants = Remora.Discord.API.Constants;

namespace Mara.Tests
{
    /// <summary>
    /// Provides test for <see cref="SnowflakeToNumberConverter"/>.
    /// </summary>
    public class SnowflakeValueConverterTests
    {
        private static readonly ValueConverter<Snowflake, ulong> SnowflakeToNumber = new SnowflakeToNumberConverter();

        private static readonly ulong FoxtrekId = 197291773133979648;
        private static readonly ulong MaraId = 801312393069199370;

        /// <summary>
        /// Tests whether a snowflake can be converted to its ulong form.
        /// </summary>
        [Fact]
        public void CanConvertSnowflakesToNumbers()
        {
            var converter = SnowflakeToNumber.ConvertToProviderExpression.Compile();

            var foxtrek = new Snowflake(FoxtrekId, DiscordConstants.DiscordEpoch);
            var mara = new Snowflake(MaraId, DiscordConstants.DiscordEpoch);

            Assert.Equal(FoxtrekId, converter(foxtrek));
            Assert.Equal(MaraId, converter(mara));
        }

        /// <summary>
        /// Tests whether a snowflake can be converted to its ulong form.
        /// </summary>
        [Fact]
        public void CanConvertSnowflakesToNumbersObject()
        {
            var converter = SnowflakeToNumber.ConvertToProvider;

            var foxtrek = new Snowflake(FoxtrekId, DiscordConstants.DiscordEpoch);
            var mara = new Snowflake(MaraId, DiscordConstants.DiscordEpoch);

            Assert.Equal(FoxtrekId, converter(foxtrek));
            Assert.Equal(MaraId, converter(mara));
            Assert.Null(converter(null));
        }

        /// <summary>
        /// Tests whether a ulong can be converted to a Snowflake.
        /// </summary>
        [Fact]
        public void CanConvertNumbersToSnowflakes()
        {
            var converter = SnowflakeToNumber.ConvertFromProviderExpression.Compile();

            var foxtrek = new Snowflake(FoxtrekId, DiscordConstants.DiscordEpoch);
            var mara = new Snowflake(MaraId, DiscordConstants.DiscordEpoch);

            Assert.Equal(foxtrek, converter(FoxtrekId));
            Assert.Equal(mara, converter(MaraId));
        }

        /// <summary>
        /// Tests whether a ulong can be converted to a Snowflake.
        /// </summary>
        [Fact]
        public void CanConvertNumbersToSnowflakesObject()
        {
            var converter = SnowflakeToNumber.ConvertFromProvider;

            var foxtrek = new Snowflake(FoxtrekId, DiscordConstants.DiscordEpoch);
            var mara = new Snowflake(MaraId, DiscordConstants.DiscordEpoch);

            Assert.Equal(foxtrek, converter(FoxtrekId));
            Assert.Equal(mara, converter(MaraId));
            Assert.Null(converter(null));
        }
    }
}
