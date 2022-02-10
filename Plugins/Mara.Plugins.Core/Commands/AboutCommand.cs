//
//  AboutCommand.cs
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

using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using Mara.Common.Discord.Feedback;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Discord.Extensions.Embeds;
using Remora.Results;

namespace Mara.Plugins.Core.Commands
{
    /// <summary>
    /// A command which provides information about the bot.
    /// </summary>
    public sealed class AboutCommand : CommandGroup
    {
        private readonly FeedbackService _feedbackService;
        private readonly IdentityInformationConfiguration _identityInformation;
        private readonly IDiscordRestUserAPI _userApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutCommand"/> class.
        /// </summary>
        /// <param name="feedbackService">The Command Feedback service.</param>
        /// <param name="identityInformation">The bot's identity.</param>
        /// <param name="userApi">A <see cref="IDiscordRestUserAPI"/> for getting user information.</param>
        public AboutCommand(FeedbackService feedbackService, IdentityInformationConfiguration identityInformation, IDiscordRestUserAPI userApi)
        {
            _feedbackService = feedbackService;
            _identityInformation = identityInformation;
            _userApi = userApi;
        }

        /// <summary>
        /// Provides an informational embed describing the bot.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Command("about")]
        [Description("Provides information about the bot.")]
        public async Task<IResult> ShowBotInfoAsync()
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("Mara")
                .WithUrl("https://mara.luzfaltex.com")
                .WithColour(Color.Pink)
                .WithDescription("A custom-tailored Discord moderation bot by LuzFaltex.");

            if (_identityInformation.Application.Team is { } team)
            {
                var avatarUrlResult = CDN.GetTeamIconUrl(team, imageSize: 256);

                if (avatarUrlResult.IsSuccess)
                {
                    var avatarUrl = avatarUrlResult.Entity;
                    embedBuilder = embedBuilder.WithAuthor(team.Name, iconUrl: avatarUrl!.AbsoluteUri);
                }
                else
                {
                    var teamOwner = await _userApi.GetUserAsync(team.OwnerUserID, CancellationToken);
                    if (teamOwner.IsSuccess)
                    {
                        embedBuilder = embedBuilder.WithAuthor(teamOwner.Entity);
                    }
                }
            }
            else
            {
                var ownerId = _identityInformation.Application.Owner!.ID.Value;
                var user = await _userApi.GetUserAsync(ownerId, CancellationToken);
                if (user.IsSuccess)
                {
                    embedBuilder = embedBuilder.WithAuthor(user.Entity);
                }
            }

            embedBuilder.AddField("Version", typeof(CorePlugin).Assembly.GetName().Version?.ToString(3) ?? "1.0.0");

            var buildResult = embedBuilder.Build();
            if (buildResult.IsDefined(out var embed))
            {
                return await _feedbackService.SendContextualEmbedAsync(embed, ct: CancellationToken);
            }

            return Result.FromError(buildResult);
        }
    }
}
