//
//  AuditConfiguration.cs
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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Mara.Plugins.Moderation.Models.Configurations
{
    /// <summary>
    /// Configures the <see cref="Audit"/> type.
    /// </summary>
    public sealed class AuditConfiguration : IEntityTypeConfiguration<Audit>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Audit> builder)
        {
            builder.Property(a => a.EventType).HasConversion<EnumToNumberConverter<EventType, int>>();

            builder.HasMany(a => a.AuditActions).WithOne(aa => aa.Audit);
        }
    }
}
