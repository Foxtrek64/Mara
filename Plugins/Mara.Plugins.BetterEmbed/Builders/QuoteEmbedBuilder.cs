//
//  QuoteEmbedBuilder.cs
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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Humanizer;
using Mara.Common.Discord.Feedback.Errors;
using Mara.Common.Errors;
using Mara.Plugins.Core;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Extensions.Builder;
using Remora.Discord.Extensions.Embeds;
using Remora.Results;

using FormatUtilities = Remora.Discord.Extensions.Formatting;

namespace Mara.Plugins.BetterEmbeds.Builders
{
    /// <summary>
    /// A service which builds embeds quoting users.
    /// </summary>
    internal sealed class QuoteEmbedBuilder : BuilderBase<IEmbed>
    {
        /// <summary>
        /// Represents a url which links directly to a Discord message.
        /// </summary>
        internal const string JumpUrl = "https://discord.com/channels/{0}/{1}/{2}";

        /// <summary>
        /// Gets the message this builder will be quoting.
        /// </summary>
        public IMessage Message { get; init; }

        /// <summary>
        /// Gets the user who requested the quote.
        /// </summary>
        public IUser Requester { get; init; }

        /// <summary>
        /// Gets the channel the quoted message belongs to.
        /// </summary>
        public IChannel SourceChannel { get; init; }

        /// <summary>
        /// Gets the channel the quoted message will be sent into.
        /// </summary>
        public IChannel DestinationChannel { get; init; }

        /// <summary>
        /// Gets the Guild which contains <see cref="SourceChannel"/>.
        /// </summary>
        public IGuild SourceGuild { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteEmbedBuilder"/> class.
        /// </summary>
        /// <param name="message">The message to quote.</param>
        /// <param name="requester">The user who requested the message be quoted.</param>
        /// <param name="sourceChannel">The channel the quoted message belongs to.</param>
        /// <param name="destinationChannel">The channel the quoted message will be sent to.</param>
        /// <param name="guild">The guild which contains <see cref="SourceChannel"/>.</param>
        public QuoteEmbedBuilder(IMessage message, IUser requester, IChannel sourceChannel, IChannel destinationChannel, IGuild guild)
        {
            Message = message;
            Requester = requester;
            SourceChannel = sourceChannel;
            DestinationChannel = destinationChannel;
            SourceGuild = guild;
        }

        /// <inheritdoc />
        public override Result<IEmbed> Build()
        {
            // If the message already contains a quote, just return that.
            if (MessageHasQuoteEmbed(Message, out var quoteEmbed))
            {
                return Result<IEmbed>.FromSuccess(quoteEmbed);
            }

            // Perform the relevant checks to ensure we can build an embed.
            var validateResult = Validate();
            if (!validateResult.IsSuccess)
            {
                return Result<IEmbed>.FromError(validateResult);
            }

            // Get a jump url we can use to traverse back to the message.
            var jumpUrl = string.Format(JumpUrl, SourceGuild.ID, SourceChannel.ID, Message.ID);
            var jumpHyperlink = FormatUtilities.Markdown.Hyperlink
                ($"{SourceGuild.Name}:{SourceChannel.Name.Value}", jumpUrl);

            // Create our embed builder
            var embedBuilder = new EmbedBuilder()
                .WithDescription(Message.Content)
                .WithTimestamp(Message.Timestamp)
                .WithAuthor(Message.Author);

            // Get top-most embed from message, if any
            // TODO: Should we re-embed better or is this sufficient?
            if (Message.Embeds.Any())
            {
                var firstEmbed = Message.Embeds[0];

                if (firstEmbed.Type.IsDefined(out var type) && type == EmbedType.Rich)
                {
                    if (firstEmbed.Fields.IsDefined(out var fields))
                    {
                        embedBuilder.SetFields((ICollection<IEmbedField>)fields);
                    }
                }
            }

            // Add activity fields
            if (Message.Activity.HasValue)
            {
                embedBuilder.AddField("Invite Type", Message.Activity.Value.Type.ToString().Humanize());
                embedBuilder.AddField("Party Id", Message.Activity.Value.PartyID.IsDefined(out var partyId) ? partyId : "No party id.");
            }

            embedBuilder.AddField("Quoted by", $"{FormatUtilities.Mention.User(Requester)} from {FormatUtilities.Markdown.Bold(jumpHyperlink)}", false);

            embedBuilder.WithFooter(CoreConstants.DismissibleEmbedFooter);

            var embedResult = embedBuilder.Build();
            return embedResult.IsDefined(out var embed)
                ? embed
                : Result<IEmbed>.FromError(embedResult);
        }

        /// <inheritdoc />
        public override Result Validate()
        {
            // We only allow quoting from a guild.
            if (!SourceChannel.GuildID.IsDefined())
            {
                return new GuildRequiredError("Quoted message must be from a guild text channel.");
            }

            // We only allow quoting into a guild.
            if (!DestinationChannel.GuildID.IsDefined())
            {
                return new GuildRequiredError("Destination channel must be a guild text channel.");
            }

            // Ensure messages from NSFW guilds or channels go into NSFW channels.
            bool sourceNsfw = IsNsfw(SourceChannel, SourceGuild);
            bool destinationNsfw = IsNsfw(DestinationChannel, null);

            // If the source channel is not NSFW, the destination does not matter.
            if (!sourceNsfw)
            {
                return Result.FromSuccess();
            }

            // If the source channel is NSFW, the destination channel must be too.
            return destinationNsfw
                ? Result.FromSuccess()
                : new NotSafeForWorkError();

            // We couldn't get an NSFW state from the destination channel.
            return new NotSafeForWorkError("Warning: could not determine if destination channel is NSFW. Unable to post embed. Please try again.");

            static bool IsNsfw(IChannel channel, IGuild? guild)
            {
                return guild is { NSFWLevel: GuildNSFWLevel.Explicit } ||
                    (channel.IsNsfw.IsDefined(out bool isChannelNsfw) && isChannelNsfw);
            }
        }

        /// <summary>
        /// Inspects a message's attributes to see if one or more is a quote.
        /// </summary>
        /// <param name="message">The message to inspect.</param>
        /// <param name="embed">The first instance of a quote embed found, if any.</param>
        /// <returns>True if a quote embed was found; otherwise, false.</returns>
        internal static bool MessageHasQuoteEmbed(IMessage message, [NotNullWhen(true)] out IEmbed? embed)
        {
            embed = message.Embeds.FirstOrDefault(embed =>
            {
                return embed.Fields.IsDefined(out var fields)
                    && fields.Any(field => field.Name == "Quoted by");
            });

            return embed is not null;
        }
    }
}
