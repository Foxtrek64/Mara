//
//  IOEmbed.cs
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
