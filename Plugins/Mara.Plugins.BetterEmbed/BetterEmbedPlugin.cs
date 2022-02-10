//
//  BetterEmbedPlugin.cs
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
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Mara.Plugins.BetterEmbeds;
using Mara.Plugins.BetterEmbeds.API;
using Mara.Plugins.BetterEmbeds.MessageHandlers;
using Mara.Plugins.BetterEmbeds.Models.OEmbed;
using Mara.Plugins.BetterEmbeds.Models.Reddit;
using Mara.Plugins.BetterEmbeds.Models.Reddit.Converters;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Remora.Discord.Gateway.Extensions;
using Remora.Plugins.Abstractions;
using Remora.Plugins.Abstractions.Attributes;
using Remora.Rest.Extensions;
using Remora.Rest.Results;
using Remora.Results;

[assembly: RemoraPlugin(typeof(BetterEmbedPlugin))]

namespace Mara.Plugins.BetterEmbeds
{
    /// <summary>
    /// Fixes Discord's automatic embeds of sites like Reddit.
    /// </summary>
    public class BetterEmbedPlugin : PluginDescriptor
    {
        /// <inheritdoc />
        public override string Name => nameof(BetterEmbeds);

        /// <inheritdoc />
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0);

        /// <inheritdoc />
        public override string Description => "Provides improved embed functionality for links Discord handles poorly.";

        /// <inheritdoc />
        public override Result ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IRedditRestAPI, RedditRestAPI>();

            var retryDelay = Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5);

            var clientBuilder = serviceCollection
                .AddRestHttpClient<RestResultError<HttpResultError>>("Reddit")
                .ConfigureHttpClient((services, client) =>
                {
                    var assemblyName = Assembly.GetExecutingAssembly().GetName();
                    var name = assemblyName.Name ??= "LuzFaltex.Mara";
                    var version = assemblyName.Version ?? new Version(1, 0, 0);

                    client.BaseAddress = new("https://www.reddit.com/");
                    client.DefaultRequestHeaders.UserAgent.Add
                    (
                        new System.Net.Http.Headers.ProductInfoHeaderValue(name, version.ToString())
                    );
                })
                .AddTransientHttpErrorPolicy
                (
                    b => b.WaitAndRetryAsync(retryDelay)
                )
                .AddPolicyHandler
                (
                    Policy
                    .HandleResult<HttpResponseMessage>(r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    .WaitAndRetryAsync
                    (
                        1,
                        (iteration, response, context) =>
                        {
                            // If no response, try again in 1 second.
                            if (response.Result == default)
                            {
                                return TimeSpan.FromSeconds(1);
                            }

                            // Use the time specified by the endpoint; otherwise, try again in 1 second.
                            return (TimeSpan)(response.Result.Headers.RetryAfter is null or { Delta: null }
                                ? TimeSpan.FromSeconds(1)
                                : response.Result.Headers.RetryAfter.Delta);
                        },
                        (_, _, _, _) => Task.CompletedTask
                    )
                );

            serviceCollection.AddResponder<QuoteEmbedHandler>();
            serviceCollection.AddResponder<RedditEmbedHandler>();

            serviceCollection.Configure<JsonSerializerOptions>(options =>
            {
                options.AddDataObjectConverter<IMedia, Media>()
                    .WithPropertyName(x => x.RedditVideo, "reddit_video")
                    .WithPropertyName(x => x.ExternalVideo, "oembed");

                options.AddDataObjectConverter<IRedditPost, RedditPost>()
                    .WithPropertyName(x => x.Id, "id")
                    .WithPropertyName(x => x.Title, "title")
                    .WithPropertyName(x => x.Subreddit, "subreddit_name_prefixed")
                    .WithPropertyName(x => x.Author, "author")
                    .WithPropertyName(x => x.Url, "url")
                    .WithPropertyName(x => x.Permalink, "permalink")
                    .WithPropertyName(x => x.Text, "selftext")
                    .WithPropertyName(x => x.Score, "score")
                    .WithPropertyName(x => x.UpvoteRatio, "upvote_ratio")
                    .WithPropertyName(x => x.PostDate, "created_utc")
                    .WithPropertyName(x => x.PostFlair, "link_flair_text")
                    .WithPropertyName(x => x.Media, "media")
                    .WithPropertyName(x => x.IsVideo, "is_video")
                    .WithPropertyName(x => x.PostHint, "post_hint")
                    .WithPropertyName(x => x.WhitelistStatus, "whitelist_status")
                    .WithPropertyName(x => x.Thumbnail, "thumbnail")
                    .WithPropertyName(x => x.ThumbnailWidth, "thumbnail_width")
                    .WithPropertyName(x => x.ThumbnailHeight, "thumbnail_height")
                    .WithPropertyConverter(x => x.PostDate, new UtcTimestampDateTimeConverter());

                options.AddDataObjectConverter<IRedditUser, RedditUser>()
                    .WithPropertyName(x => x.DisplayNamePrefixed, "display_name_prefixed")
                    .WithPropertyName(x => x.IconImage, "icon_img");

                options.AddDataObjectConverter<IRedditVideo, RedditVideo>()
                    .WithPropertyName(x => x.Url, "fallback_url")
                    .WithPropertyName(x => x.Height, "height")
                    .WithPropertyName(x => x.Width, "width")
                    .WithPropertyName(x => x.Duration, "duration")
                    .WithPropertyName(x => x.IsGif, "is_gif");

                options.AddDataObjectConverter<IOEmbed, OEmbed>();
                options.AddDataObjectConverter<IPhoto, Photo>();
                options.AddDataObjectConverter<IVideo, Video>();
            });

            return Result.FromSuccess();
        }
    }
}
