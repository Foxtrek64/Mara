//
//  Infraction.cs
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

using System;
using Remora.Rest.Core;

namespace Mara.Plugins.Moderation.Models
{
    /// <summary>
    /// An EF model representing a user infraction.
    /// </summary>
    public sealed class Infraction
    {
        /// <summary>
        /// Gets or sets the unique identifier of this infraction.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique id of the user being awarded punishment.
        /// </summary>
        public Snowflake User { get; set; }

        /// <summary>
        /// Gets or sets the guild id in which this infraction took place.
        /// </summary>
        public Snowflake GuildId { get; set; }

        /// <summary>
        /// Gets or sets the date and time the infraction took place.
        /// </summary>
        public DateTimeOffset TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the kind of action was taken by the responsible moderator.
        /// </summary>
        public InfractionKind InfractionKind { get; set; }

        /// <summary>
        /// Gets or sets the date and time at which a temporary punishment expires. If the punishment is not temporary, this will be null.
        /// </summary>
        public DateTimeOffset? PunishmentExpiration { get; set; }

        /// <summary>
        /// Gets or sets the unique id of the responsible for resolving the issue.
        /// </summary>
        public Snowflake ResponsibleModerator { get; set; }

        /// <summary>
        /// Gets or sets additional information about this infraction.
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this infraction was rescinded.
        /// </summary>
        public bool Rescinded { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="UserInformation"/> this infraction belongs to, for setting up the relationship.
        /// </summary>
        public UserInformation? UserInformation { get; set; }
    }
}
