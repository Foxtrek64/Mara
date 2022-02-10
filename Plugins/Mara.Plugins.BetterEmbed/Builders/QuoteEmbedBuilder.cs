//
//  QuoteEmbedBuilder.cs
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Humanizer;
using Mara.Common.Discord.Feedback.Errors;
using Mara.Common.Results;
using Mara.Plugins.Core;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Extensions.Builder;
using Remora.Discord.Extensions.Embeds;
using Remora.Results;

using FormatUtilities = Mara.Common.Discord.FormatUtilities;

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
            var jumpHyperlink = FormatUtilities.Url($"{SourceGuild.Name}:{SourceChannel.Name.Value}", jumpUrl);

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

            embedBuilder.AddField("Quoted by", $"{FormatUtilities.Mention(Requester)} from {FormatUtilities.Bold(jumpHyperlink)}", false);

            embedBuilder.WithFooter(CoreConstants.DismissableEmbedFooter);

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
            // If the source channel is not NSFW, check to see if the guild is NSFW.
            if (SourceChannel.IsNsfw.IsDefined(out bool isSourceNSFW) && !isSourceNSFW)
            {
                if (SourceGuild is not { NSFWLevel: GuildNSFWLevel.Explicit })
                {
                    return Result.FromSuccess();
                }
            }

            // The channel did not have nsfw information, it is nsfw, or the guild is nsfw.
            if (DestinationChannel.IsNsfw.IsDefined(out bool isDestinationNSFW))
            {
                return isDestinationNSFW
                    ? Result.FromSuccess()
                    : new NotSafeForWorkError();
            }

            // We couldn't get an NSFW state from the destination channel.
            return new NotSafeForWorkError("Warning: could not determine if destination channel is NSFW. Unable to post embed. Please try again.");
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
                if (!embed.Fields.IsDefined(out var fields))
                {
                    return false;
                }

                return fields.Any(field => field.Name == "Quoted by");
            });

            return embed is { };
        }
    }
}
