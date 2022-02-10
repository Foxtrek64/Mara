//
//  ModerationContext.cs
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

using System.Reflection;
using Mara.Common.ValueConverters;
using Mara.Plugins.Moderation.Models;
using Microsoft.EntityFrameworkCore;
using Remora.Rest.Core;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Mara.Plugins.Moderation
{
    /// <summary>
    /// Provides a database context for the moderation plugin.
    /// </summary>
    public sealed class ModerationContext : DbContext
    {
        /// <summary>
        /// Gets or sets a collection user information.
        /// </summary>
        public DbSet<UserInformation> UserInfo { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModerationContext"/> class.
        /// </summary>
        /// <param name="options">The database options.</param>
        public ModerationContext(DbContextOptions<ModerationContext> options)
            : base(options)
        {
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        /// <inheritdoc/>
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
            configurationBuilder.Properties<Snowflake>().HaveConversion<SnowflakeToNumberConverter>();
        }
    }
}
