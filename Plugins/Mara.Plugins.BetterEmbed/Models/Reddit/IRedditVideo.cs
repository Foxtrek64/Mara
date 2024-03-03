//
//  IRedditVideo.cs
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

namespace Mara.Plugins.BetterEmbeds.Models.Reddit
{
    /// <summary>
    /// Represents a video hosted on Reddit.
    /// </summary>
    public interface IRedditVideo
    {
        /// <summary>
        /// Gets the url to the video on the redditmedia cdn.
        /// </summary>
        string Url { get; init; }

        /// <summary>
        /// Gets the height of the video.
        /// </summary>
        int Height { get; init; }

        /// <summary>
        /// Gets the width of the video.
        /// </summary>
        int Width { get; init; }

        /// <summary>
        /// Gets the duration of the video.
        /// </summary>
        int Duration { get; init; }

        /// <summary>
        /// Gets a value indicating whether the video is actually an animated image.
        /// </summary>
        bool IsGif { get; init; }
    }
}
