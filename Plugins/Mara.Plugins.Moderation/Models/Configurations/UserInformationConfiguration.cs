//
//  UserInformationConfiguration.cs
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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mara.Plugins.Moderation.Models.Configurations
{
    /// <summary>
    /// Configures the <see cref="UserInformation"/> type.
    /// </summary>
    public sealed class UserInformationConfiguration : IEntityTypeConfiguration<UserInformation>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<UserInformation> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.HasMany(x => x.Infractions).WithOne(x => x.UserInformation);
        }
    }
}
