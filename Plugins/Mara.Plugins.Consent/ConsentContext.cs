//
//  ConsentContext.cs
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
using Microsoft.EntityFrameworkCore;
using Remora.Rest.Core;

using ConsentModel = Mara.Plugins.Consent.Models.Consent;

namespace Mara.Plugins.Consent
{
    /// <summary>
    /// Provides a database context for the Consent plugin.
    /// </summary>
    [PublicAPI]
    public sealed class ConsentContext(DbContextOptions<ConsentContext> options) : DbContext(options)
    {
        /// <summary>
        /// Gets a collection of ConsentClaims.
        /// </summary>
        public DbSet<ConsentModel> ConsentClaims => Set<ConsentModel>();

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConsentContext).Assembly);
        }

        /// <inheritdoc />
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
            configurationBuilder.Properties<Snowflake>()
                                .HaveConversion<SnowflakeToNumberConverter>();
        }
    }
}
