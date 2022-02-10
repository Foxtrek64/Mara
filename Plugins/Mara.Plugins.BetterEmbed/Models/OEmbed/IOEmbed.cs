//
//  IOEmbed.cs
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
using Remora.Rest.Core;

namespace Mara.Plugins.BetterEmbeds.Models.OEmbed
{
    /// <summary>
    /// An OEmbed object.
    /// </summary>
    public interface IOEmbed
    {
        /// <summary>
        /// Gets the resource type.
        /// </summary>
        string Type { get; init; }

        /// <summary>
        /// Gets the OEmbed version number.
        /// </summary>
        Version Version { get; init; }

        /// <summary>
        /// Gets the title of the resource.
        /// </summary>
        Optional<string> Title { get; init; }

        /// <summary>
        /// Gets the name of the author or owner of the resource.
        /// </summary>
        Optional<string> AuthorName { get; init; }

        /// <summary>
        /// Gets the url of the author or owner of the resource.
        /// </summary>
        Optional<string> AuthorUrl { get; init; }

        /// <summary>
        /// Gets the name of the provider of the resource.
        /// </summary>
        Optional<string> ProviderName { get; init; }

        /// <summary>
        /// Gets the url of the provider of the resource.
        /// </summary>
        Optional<string> ProviderUrl { get; init; }

        /// <summary>
        /// Gets the suggested cache lifetime for this resource, in seconds.
        /// </summary>
        Optional<int> CacheAge { get; init; }

        /// <summary>
        /// Gets the url to a thumbnail image representing the resource.
        /// </summary>
        Optional<string> ThumbnailUrl { get; init; }

        /// <summary>
        /// Gets the width of the thumbnail. Must be present if the <see cref="ThumbnailUrl"/> is present.
        /// </summary>
        Optional<int> ThumbnailWidth { get; init; }

        /// <summary>
        /// Gets the width of the thumbnail. Must be present if the <see cref="ThumbnailUrl"/> is present.
        /// </summary>
        Optional<int> ThumbnailHeight { get; init; }

        /// <summary>
        /// Gets the photo contained by this resource, if any.
        /// </summary>
        Optional<IPhoto> Photo { get; init; }

        /// <summary>
        /// Gets the video contained by this resource, if any.
        /// </summary>
        Optional<IVideo> Video { get; init; }
    }
}
