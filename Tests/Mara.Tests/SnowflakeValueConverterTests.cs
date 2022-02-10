//
//  SnowflakeValueConverterTests.cs
//
//  Author:
//       LuzFaltex Contributors
//
//  ISC License
//
//  Copyright (c) 2021 LuzFaltex
//
//  Permission to use, copy, modify, and/or distribute this software for any
//  purpose with or without fee is hereby granted, provided that the above
//  copyright notice and this permission notice appear in all copies.
//
//  THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
//  WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
//  MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
//  ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
//  WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
//  ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
//  OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
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
        private static readonly ValueConverter<Snowflake, ulong> _snowflakeToNumber = new SnowflakeToNumberConverter();

        private static readonly ulong FoxtrekId = 197291773133979648;
        private static readonly ulong MaraId = 801312393069199370;

        /// <summary>
        /// Tests whether a snowflake can be converted to its ulong form.
        /// </summary>
        [Fact]
        public void CanConvertSnowflakesToNumbers()
        {
            var converter = _snowflakeToNumber.ConvertToProviderExpression.Compile();

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
            var converter = _snowflakeToNumber.ConvertToProvider;

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
            var converter = _snowflakeToNumber.ConvertFromProviderExpression.Compile();

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
            var converter = _snowflakeToNumber.ConvertFromProvider;

            var foxtrek = new Snowflake(FoxtrekId, DiscordConstants.DiscordEpoch);
            var mara = new Snowflake(MaraId, DiscordConstants.DiscordEpoch);

            Assert.Equal(foxtrek, converter(FoxtrekId));
            Assert.Equal(mara, converter(MaraId));
            Assert.Null(converter(null));
        }
    }
}
