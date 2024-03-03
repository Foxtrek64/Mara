//
//  ConsentCommands.cs
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
using System.Threading.Tasks;
using Mara.Plugins.Consent.Services;
using Mara.Plugins.Core;
using Microsoft.Extensions.Caching.Memory;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Feedback.Messages;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Rest.Core;
using Remora.Results;

namespace Mara.Plugins.Consent.Commands
{
    /// <summary>
    /// A set of commands for granting or denying consent.
    /// </summary>
    [Group("consent")]
    public sealed class ConsentCommands
    (
        IFeedbackService feedbackService,
        IOperationContext context,
        IConsentService consentService,
        IMemoryCache memoryCache
    )
        : CommandGroup
    {
        private readonly FeedbackMessageOptions _ephemeralMessage = new(MessageFlags: MessageFlags.Ephemeral);

        private readonly IReadOnlyList<IMessageComponent> _grantConsentComponents =
        [
            new ButtonComponent(ButtonComponentStyle.Success, "Grant Consent", CustomID: ConsentConstants.GrantConsentButtonId),
            new ButtonComponent(ButtonComponentStyle.Primary, "Cancel")
        ];

        private readonly IReadOnlyList<IMessageComponent> _revokeConsentComponents =
        [
            new ButtonComponent(ButtonComponentStyle.Danger, "Revoke Consent", CustomID: ConsentConstants.DenyConsentButtonId),
            new ButtonComponent(ButtonComponentStyle.Primary, "Cancel")
        ];

        /// <summary>
        /// Allows the user to consent to GDPR user collection.
        /// </summary>
        /// <param name="accept">If true, the user will not be shown a prompt with buttons and the command
        /// will execute silently.</param>
        /// <returns>The result of the command.</returns>
        [Command("grant")]
        [CommandType(ApplicationCommandType.Message)]
        public async Task<IResult> GrantConsent([Switch("accept")] bool accept)
        {
            if (!context.TryGetUserID(out Snowflake userId))
            {
                return Result.FromError(new NotFoundError("Failed to retrieve user id from context."));
            }

            if (!accept)
            {
                IPartialApplication? application = memoryCache.Get<IPartialApplication>(CoreConstants.CacheKeys.CurrentApplication);

                if (application is null || !application.PrivacyPolicyURL.IsDefined(out string? privacyPolicyUrl))
                {
                    privacyPolicyUrl = string.Empty;
                }

                return await feedbackService.SendContextualInfoAsync
                (
                    contents: "By clicking 'Grant Consent' below, you give us consent" +
                    " to collect, process, and use your data in accordance" +
                    $" with our [privacy policy]({privacyPolicyUrl}).\n\nClick Cancel to abort this operation.",
                    target: userId,
                    options: _ephemeralMessage with
                    {
                        MessageComponents = new Optional<IReadOnlyList<IMessageComponent>>(_grantConsentComponents)
                    },
                    ct: CancellationToken
                );
            }
            else
            {
                return await consentService.GrantConsent(userId, CancellationToken);
            }
        }
    }
}
