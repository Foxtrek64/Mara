//
//  EventType.cs
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

namespace Mara.Plugins.Moderation.Models
{
    /// <summary>
    /// The kind of event which caused the audit action.
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// Placeholder. Used for when there is no valid event type.
        /// </summary>
        None = 0,

        /// <summary>
        /// A channel was created.
        /// </summary>
        ChannelCreate = 1,

        /// <summary>
        /// A channel was deleted.
        /// </summary>
        ChannelDelete = 2,

        /// <summary>
        /// A channel was updated.
        /// </summary>
        ChannelUpdate = 3,

        /// <summary>
        /// A message was pinned or unpinned.
        /// </summary>
        ChannelPinsUpdate = 4,

        /// <summary>
        /// A stage was created.
        /// </summary>
        StageInstanceCreate = 5,

        /// <summary>
        /// A stage was deleted.
        /// </summary>
        StageInstanceDelete = 6,

        /// <summary>
        /// A stage was updated.
        /// </summary>
        StageInstanceUpdate = 7,

        /// <summary>
        /// A thread was created.
        /// </summary>
        ThreadCreate = 8,

        /// <summary>
        /// A thread was deleted.
        /// </summary>
        ThreadDelete = 9,

        /// <summary>
        /// A thread was updated.
        /// </summary>
        ThreadUpdate = 10,

        /// <summary>
        /// A user was banned.
        /// </summary>
        GuildBanAdd = 11,

        /// <summary>
        /// A user was unbanned.
        /// </summary>
        GuildBanRemove = 12,

        /// <summary>
        /// A guild was created.
        /// </summary>
        GuildCreate = 13,

        /// <summary>
        /// A guild was deleted.
        /// </summary>
        GuildDelete = 14,

        /// <summary>
        /// A guild's emojis were updated.
        /// </summary>
        GuildEmojisUpdate = 15,

        /// <summary>
        /// A guild's integrations were updated.
        /// </summary>
        GuildIntegrationsUpdate = 16,

        /// <summary>
        /// A user or bot joined the guild.
        /// </summary>
        GuildMemberAdd = 17,

        /// <summary>
        /// A user or bot left the guild.
        /// </summary>
        GuildMemberRemove = 18,

        /// <summary>
        /// A user or bot updated their appearance (profile picture, nickname, about me).
        /// </summary>
        GuildMemberUpdate = 19,

        /// <summary>
        /// A role was created.
        /// </summary>
        GuildRoleCreate = 20,

        /// <summary>
        ///  A role was deleted.
        /// </summary>
        GuildRoleDelete = 21,

        /// <summary>
        /// A role was updated.
        /// </summary>
        GuildRoleUpdate = 22,

        /// <summary>
        /// A sticker was added, removed, or renamed.
        /// </summary>
        GuildStickersUpdate = 23,

        /// <summary>
        /// Guild settings were updated.
        /// </summary>
        GuildUpdate = 24,

        /// <summary>
        /// An invite was created.
        /// </summary>
        GuildInviteCreate = 25,

        /// <summary>
        /// An invite was deleted.
        /// </summary>
        GuildInviteDelete = 26,

        /// <summary>
        /// A message was posted.
        /// </summary>
        MessageCreate = 27,

        /// <summary>
        /// A message was deleted.
        /// </summary>
        MessageDelete = 28,

        /// <summary>
        /// A message was updated.
        /// </summary>
        MessageUpdate = 29,

        /// <summary>
        /// A reaction was added to a message.
        /// </summary>
        MessageReactionAdd = 30,

        /// <summary>
        /// A reaction was removed from a message.
        /// </summary>
        MessageReactionRemove = 31,

        /// <summary>
        /// A webhook was created, deleted, or modified.
        /// </summary>
        WebhookUpdate = 32
    }
}
