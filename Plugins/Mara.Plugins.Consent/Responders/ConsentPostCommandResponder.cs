//
//  ConsentPostCommandResponder.cs
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mara.Common.Extensions;
using Mara.Plugins.Consent.Errors;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Feedback.Messages;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Discord.Commands.Services;
using Remora.Discord.Extensions.Embeds;
using Remora.Discord.Interactivity;
using Remora.Rest.Core;
using Remora.Results;
using static Mara.Plugins.Core.CoreConstants;

namespace Mara.Plugins.Consent.Responders
{
    /// <summary>
    /// Handles command results where the command failed due to a <see cref="LackOfConsentError"/>.
    /// </summary>
    [UsedImplicitly]
    public sealed class ConsentPostCommandResponder : IPostExecutionEvent
    {
        private readonly FeedbackService _feedbackService;
        private readonly MemoryCache _memoryCache;
        private readonly IStringLocalizer _stringLocalizerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsentPostCommandResponder"/> class.
        /// </summary>
        /// <param name="memoryCache">A memory cache for this bot process.</param>
        /// <param name="feedbackService">The feedback service.</param>
        /// <param name="stringLocalizerFactory">The stringLocalizerFactory for the instance.</param>
        public ConsentPostCommandResponder(MemoryCache memoryCache, FeedbackService feedbackService, ResourceManagerStringLocalizerFactory stringLocalizerFactory)
        {
            _memoryCache = memoryCache;
            _feedbackService = feedbackService;
            _stringLocalizerFactory = stringLocalizerFactory;
        }

        /// <inheritdoc/>
        public async Task<Result> AfterExecutionAsync(ICommandContext context, IResult commandResult, CancellationToken ct = default)
        {
            if (commandResult.IsSuccess || commandResult.Error is not LackOfConsentError)
            {
                return Result.FromSuccess();
            }

            var application = _memoryCache.Get<IPartialApplication>(CacheKeys.CurrentApplication);

            if (application is null || !application.PrivacyPolicyURL.IsDefined(out var privacyPolicyUrl))
            {
                privacyPolicyUrl = string.Empty;
            }

            var embedBuilder = new EmbedBuilder();
            if
            (
                context is InteractionContext interactionContext &&
                interactionContext.Interaction.Locale.IsDefined(out var locale) &&
                locale is { Length: > 0 }
            )
            {
                // TODO: Use interactionContext.Locale to translate consent prompt message. Fall back to English.
                embedBuilder
                    .WithTitle(_localizer["GDPR.Prompt.Title"])
                    .WithDescription(_localizer["GDPR.Prompt.NoConsent", privacyPolicyUrl])
                    .WithFooter(DismissibleEmbedFooter);
            }
            else
            {
                // English Fallback
                embedBuilder
                    .WithTitle("GDRP Data Protection")
                    .WithDescription
                    (
                        $"You have not granted consent, or have revoked your consent, for us to collect, process, and" +
                        $" use your data in accordance with our [privacy policy]({privacyPolicyUrl}). Please grant" +
                        $" consent and then run this command again."
                    );
            }

            var messageComponents = new List<IMessageComponent>();
            var grantConsentButton = new ButtonComponent
            (
                Style: ButtonComponentStyle.Success,
                Label: "Grant Consent",
                CustomID: CustomIDHelpers.CreateButtonID(ConsentConstants.GrantConsentButtonId)
            );
            var denyConsentButton = new ButtonComponent
            (
                Style: ButtonComponentStyle.Danger,
                Label: "Deny Consent",
                CustomID: CustomIDHelpers.CreateButtonID(ConsentConstants.DenyConsentButtonId)
            );
            messageComponents.Add(grantConsentButton);
            messageComponents.Add(denyConsentButton);

            var feedbackOptions = new FeedbackMessageOptions(MessageComponents: messageComponents.AsReadOnly());

            var buildResult = embedBuilder.Build();
            if (!buildResult.IsDefined(out Embed? embed))
            {
                return Result.FromError(buildResult);
            }

            Result<IMessage> sendMessageResult;
            if (context.TryGetGuildID(out _))
            {
                sendMessageResult = await _feedbackService.SendContextualEmbedAsync(embed, feedbackOptions, ct);
            }
            else
            {
                sendMessageResult = await _feedbackService.SendPrivateEmbedAsync(context.GetUserId(), embed, feedbackOptions, ct);
            }

            return sendMessageResult.IsSuccess
                ? Result.FromSuccess()
                : Result.FromError(sendMessageResult);
        }
    }
}
