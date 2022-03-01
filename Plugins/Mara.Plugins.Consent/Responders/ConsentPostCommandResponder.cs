//
//  ConsentPostCommandResponder.cs
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
using System.Threading;
using System.Threading.Tasks;
using Mara.Plugins.Consent.Errors;
using Microsoft.Extensions.Caching.Memory;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Feedback.Messages;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Discord.Commands.Services;
using Remora.Discord.Extensions.Embeds;
using Remora.Results;
using static Mara.Plugins.Core.CoreConstants;

namespace Mara.Plugins.Consent.Responders
{
    /// <summary>
    /// Handles command results where the command failed due to a <see cref="LackOfConsentError"/>.
    /// </summary>
    public sealed class ConsentPostCommandResponder : IPostExecutionEvent
    {
        private readonly FeedbackService _feedbackService;
        private readonly MemoryCache _memoryCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsentPostCommandResponder"/> class.
        /// </summary>
        /// <param name="memoryCache">A memory cache for this bot process.</param>
        /// <param name="feedbackService">The feedback service.</param>
        public ConsentPostCommandResponder(MemoryCache memoryCache, FeedbackService feedbackService)
        {
            _memoryCache = memoryCache;
            _feedbackService = feedbackService;
        }

        /// <inheritdoc/>
        public async Task<Result> AfterExecutionAsync(ICommandContext context, IResult commandResult, CancellationToken ct = default)
        {
            if (commandResult.IsSuccess || commandResult.Error is not LackOfConsentError error)
            {
                return Result.FromSuccess();
            }

            var application = _memoryCache.Get<IPartialApplication>(CacheKeys.CurrentApplication);

            if (application is null || !application.PrivacyPolicyURL.IsDefined(out var privacyPolicyUrl))
            {
                privacyPolicyUrl = string.Empty;
            }

            var embedBuilder = new EmbedBuilder();
            if (context is InteractionContext interactionContext)
            {
                // TODO: Use interactionContext.Locale to translate consent prompt message. Fall back to English.
                embedBuilder
                    .WithTitle("GDRP Data Protection")
                    .WithDescription($"You have not granted consent, or have revoked your consent, for us to collect, process, and use your data in accordance with our [privacy policy]({privacyPolicyUrl}). Please grant consent and then run this command again.")
                    .WithFooter(DismissableEmbedFooter);
            }
            else
            {
                // English Fallback
                embedBuilder
                    .WithTitle("GDRP Data Protection")
                    .WithDescription($"You have not granted consent, or have revoked your consent, for us to collect, process, and use your data in accordance with our [privacy policy]({privacyPolicyUrl}). Please grant consent and then run this command again.")
                    .WithFooter(DismissableEmbedFooter);
            }

            var messageComponents = new List<IMessageComponent>();
            var grantConsentButton = new ButtonComponent
                (
                    Style: ButtonComponentStyle.Success,
                    Label: "Grant Consent"
                );
            messageComponents.Add(grantConsentButton);

            var feedbackOptions = new FeedbackMessageOptions(MessageComponents: messageComponents.AsReadOnly());

            var buildResult = embedBuilder.Build();
            if (!buildResult.IsDefined(out var embed))
            {
                return Result.FromError(buildResult);
            }

            Result<IMessage> sendMessageResult;
            if (context.GuildID.IsDefined(out var guildId))
            {
                sendMessageResult = await _feedbackService.SendContextualEmbedAsync(embed, feedbackOptions, ct);
            }
            else
            {
                sendMessageResult = await _feedbackService.SendPrivateEmbedAsync(context.User.ID, embed, feedbackOptions, ct);
            }

            return sendMessageResult.IsSuccess
                ? Result.FromSuccess()
                : Result.FromError(sendMessageResult);
        }
    }
}
