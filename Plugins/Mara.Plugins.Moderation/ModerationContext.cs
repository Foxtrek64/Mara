//
//  ModerationContext.cs
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

using System.Reflection;
using JetBrains.Annotations;
using Mara.Common.ValueConverters;
using Mara.Plugins.Moderation.Models;
using Microsoft.EntityFrameworkCore;
using Remora.Rest.Core;

namespace Mara.Plugins.Moderation
{
    /// <summary>
    /// Provides a database context for the moderation plugin.
    /// </summary>
    [PublicAPI]
    public sealed class ModerationContext : DbContext
    {
        /// <summary>
        /// Gets a collection user information.
        /// </summary>
        public DbSet<UserInformation> UserInfo => Set<UserInformation>();

        /// <summary>
        /// Gets a collection of infractions.
        /// </summary>
        public DbSet<Infraction> Infractions => Set<Infraction>();

        /// <summary>
        /// Gets a collection of guild configuration information.
        /// </summary>
        public DbSet<GuildConfiguration> GuildConfigurations => Set<GuildConfiguration>();

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
            configurationBuilder.Properties<Snowflake>()
                                .HaveConversion<SnowflakeToNumberConverter>();
        }
    }
}
