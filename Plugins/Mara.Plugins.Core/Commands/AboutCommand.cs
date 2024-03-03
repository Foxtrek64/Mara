//
//  AboutCommand.cs
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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using ByteSizeLib;
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
using Remora.Discord.API.Gateway.Commands;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Discord.Extensions.Embeds;
using Remora.Discord.Extensions.Formatting;
using Remora.Results;

using CacheKeys = Mara.Plugins.Core.CoreConstants.CacheKeys;

namespace Mara.Plugins.Core.Commands
{
    /// <summary>
    /// A command which provides information about the bot.
    /// </summary>
    /// <param name="feedbackService">The Command Feedback service.</param>
    /// <param name="userApi">A <see cref="IDiscordRestUserAPI"/> for getting user information.</param>
    /// <param name="memoryCache">A memory cache.</param>
    /// <param name="config">The bot config.</param>
    public sealed class AboutCommand
    (
        FeedbackService feedbackService,
        IDiscordRestUserAPI userApi,
        IMemoryCache memoryCache,
        IOptions<RuntimeConfig> config
    )
        : CommandGroup
    {
        /// <summary>
        /// Provides an informational embed describing the bot.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Command("about")]
        [Description("Provides information about the bot.")]
        public async Task<Result<IMessage>> ShowBotInfoAsync()
        {
            var application = memoryCache.Get<IApplication>(CacheKeys.CurrentApplication);

            if (application is null)
            {
                return Result<IMessage>.FromError(new NotFoundError("Could not retrieve owner information."));
            }

            EmbedBuilder embedBuilder = new EmbedBuilder()
                                        .WithTitle(application.Name)
                                        .WithUrl(config.Value.BotWebsiteUrl)
                                        .WithColour(Color.Pink)
                                        .WithDescription(application.Description);

            if (application.Team is { } team)
            {
                var avatarUrlResult = CDN.GetTeamIconUrl(team, imageSize: 256);

                if (avatarUrlResult.IsSuccess)
                {
                    var avatarUrl = avatarUrlResult.Entity;
                    embedBuilder = embedBuilder.WithAuthor(team.Name, iconUrl: avatarUrl.AbsoluteUri);
                }
                else
                {
                    var teamOwner = await userApi.GetUserAsync(team.OwnerUserID, CancellationToken);
                    if (teamOwner.IsSuccess)
                    {
                        embedBuilder = embedBuilder.WithAuthor(teamOwner.Entity);
                    }
                }
            }
            else
            {
                if (!application.Owner.IsDefined(out var owner))
                {
                    return Result<IMessage>.FromError(new NotFoundError("Could not retrieve owner information."));
                }

                var user = await userApi.GetUserAsync(owner.ID.Value, CancellationToken);
                if (user.IsSuccess)
                {
                    embedBuilder = embedBuilder.WithAuthor(user.Entity);
                }
            }

            embedBuilder.AddField("Version", typeof(CorePlugin).Assembly.GetName().Version?.ToString(3) ?? "1.0.0");

            string shardText = memoryCache.TryGetValue(CacheKeys.ShardNumber, out IShardIdentification? shardIdentification)
                ? $"{shardIdentification!.ShardID}/{shardIdentification.ShardCount}"
                : "Unsharded.";

            embedBuilder.AddField("Shard", shardText);

            var uptime = memoryCache.Get<DateTimeOffset>(CacheKeys.StartupTime);
            embedBuilder.AddField("Uptime", Markdown.Timestamp(uptime, TimestampStyle.LongTime));

            var committedBytes = ByteSize.FromBytes(GC.GetGCMemoryInfo().TotalCommittedBytes);
            embedBuilder.AddField("Memory Utilization", committedBytes.ToString());

            var buildResult = embedBuilder.Build();
            return buildResult.IsDefined(out Embed? embed)
                ? await feedbackService.SendContextualEmbedAsync(embed, ct: CancellationToken)
                : Result<IMessage>.FromError(buildResult);
        }
    }
}
