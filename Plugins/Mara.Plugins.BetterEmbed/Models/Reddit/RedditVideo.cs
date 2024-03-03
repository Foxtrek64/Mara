//
//  RedditVideo.cs
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

using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Mara.Plugins.BetterEmbeds.Models.Reddit
{
    /// <inheritdoc />
    [PublicAPI]
    public record RedditVideo
    (
        [property: JsonPropertyName("fallback_url")] string Url,
        [property: JsonPropertyName("height")] int Height,
        [property: JsonPropertyName("width")] int Width,
        [property: JsonPropertyName("duration")] int Duration,
        [property: JsonPropertyName("is_gif")] bool IsGif
    ) : IRedditVideo;
}
