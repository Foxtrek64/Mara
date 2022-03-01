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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using Mara.Common.Discord;
using Mara.Common.Extensions;
using Mara.Common.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Discord.Extensions.Embeds;
using Remora.Results;

using CacheKeys = Mara.Plugins.Core.CoreConstants.CacheKeys;

namespace Mara.Plugins.Core.Commands
{
    /// <summary>
    /// A command which provides information about the bot.
    /// </summary>
    public sealed class AboutCommand : CommandGroup
    {
        private readonly FeedbackService _feedbackService;
        private readonly IDiscordRestUserAPI _userApi;
        private readonly IMemoryCache _memoryCache;
        private readonly MaraConfig _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutCommand"/> class.
        /// </summary>
        /// <param name="feedbackService">The Command Feedback service.</param>
        /// <param name="userApi">A <see cref="IDiscordRestUserAPI"/> for getting user information.</param>
        /// <param name="memoryCache">A memory cache.</param>
        /// <param name="config">The bot config.</param>
        public AboutCommand(FeedbackService feedbackService, IDiscordRestUserAPI userApi, IMemoryCache memoryCache, IOptions<MaraConfig> config)
        {
            _feedbackService = feedbackService;
            _userApi = userApi;
            _memoryCache = memoryCache;
            _config = config.Value;
        }

        /// <summary>
        /// Provides an informational embed describing the bot.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Command("about")]
        [Description("Provides information about the bot.")]
        public async Task<Result<IMessage>> ShowBotInfoAsync()
        {
            var application = _memoryCache.Get<IApplication>(CacheKeys.CurrentApplication);

            var embedBuilder = new EmbedBuilder()
                .WithTitle(application.Name)
                .WithUrl(_config.BotWebsiteUrl)
                .WithColour(Color.Pink)
                .WithDescription(application.Description);

            if (application is null)
            {
                return Result<IMessage>.FromError(new NotFoundError("Could not retrieve owner information."));
            }

            if (application.Team is { } team)
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
                if (application.Owner is not IPartialUser owner)
                {
                    return Result<IMessage>.FromError(new NotFoundError("Could not retrieve owner information."));
                }

                var user = await _userApi.GetUserAsync(owner.ID.Value, CancellationToken);
                if (user.IsSuccess)
                {
                    embedBuilder = embedBuilder.WithAuthor(user.Entity);
                }
            }

            embedBuilder.AddField("Version", typeof(CorePlugin).Assembly.GetName().Version?.ToString(3) ?? "1.0.0");

            if (_memoryCache.TryGetValue<IShardIdentification>(CacheKeys.ShardNumber, out var shardNumber))
            {
                embedBuilder.AddField("Shard", $"{shardNumber.ShardID}/{shardNumber.ShardCount}");
            }
            else
            {
                embedBuilder.AddField("Shard", "Unsharded.");
            }

            var uptime = _memoryCache.Get<DateTimeOffset>(CacheKeys.StartupTime);
            embedBuilder.AddField("Uptime", FormatUtilities.DynamicTimeStamp(uptime, FormatUtilities.TimeStampStyle.LongTime));

            embedBuilder.AddField("Memory Utilization", GC.GetGCMemoryInfo().TotalCommittedBytes.ToFileSize());

            var buildResult = embedBuilder.Build();
            if (buildResult.IsDefined(out var embed))
            {
                return await _feedbackService.SendContextualEmbedAsync(embed, ct: CancellationToken);
            }

            return Result<IMessage>.FromError(buildResult);
        }
    }
}
