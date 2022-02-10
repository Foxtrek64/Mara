//
//  Audit.cs
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

using System.Collections.Generic;
using Remora.Rest.Core;

namespace Mara.Plugins.Moderation.Models
{
    /// <summary>
    /// Represents an action that takes place on a server.
    /// </summary>
    public sealed class Audit
    {
        /// <summary>
        /// Gets or sets the audit's unique id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the id of the Guild where this event took place.
        /// </summary>
        public Snowflake GuildId { get; set; }

        /// <summary>
        /// Gets or sets the user or bot responsible for the action.
        /// </summary>
        public Snowflake Source { get; set; }

        /// <summary>
        /// Gets or sets the kind of event that took place.
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// Gets or sets the target of the action.
        /// </summary>
        public Snowflake Target { get; set; }

        /// <summary>
        /// Gets or sets a collection of actions taken during this change.
        /// </summary>
        public List<AuditAction> AuditActions { get; set; } = new();

        /// <summary>
        /// Gets or sets the change number.
        /// </summary>
        public int? ChangeNumber { get; set; }

        /// <summary>
        /// Gets or sets a user-provided comment regarding the audit entry.
        /// </summary>
        public string? Comment { get; set; }
    }
}
