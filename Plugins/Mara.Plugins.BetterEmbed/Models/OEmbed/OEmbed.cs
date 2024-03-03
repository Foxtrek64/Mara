//
//  OEmbed.cs
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
    /// <inheritdoc />
    public record OEmbed
    (
        string Type,
        Version Version,
        Optional<string> Title,
        Optional<string> AuthorName,
        Optional<string> AuthorUrl,
        Optional<string> ProviderName,
        Optional<string> ProviderUrl,
        Optional<int> CacheAge,
        Optional<string> ThumbnailUrl,
        Optional<int> ThumbnailWidth,
        Optional<int> ThumbnailHeight,
        Optional<IPhoto> Photo,
        Optional<IVideo> Video
    ) : IOEmbed;
}
