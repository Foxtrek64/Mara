//
//  SemanticVersion.cs
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

namespace Mara.Common.Models
{
    /// <summary>
    /// A <see cref="Version"/> which follows the specification established at <see href="https://semver.org/spec/v2.0.0.html"/>.
    /// </summary>
    /// <param name="Major">The <see cref="Major"/> component, to be incremented for breaking additions or changes.</param>
    /// <param name="Minor">The <see cref="Minor"/> component, to be incremented for non-breaking additions or changes.</param>
    /// <param name="Patch">The <see cref="Patch"/> component, to be incremented for non-breaking bug fixes.</param>
    /// <param name="PreReleaseLabel">The <see cref="PreReleaseLabel"/> component, marking this component as being in beta and providing .</param>
    /// <param name="BuildLabel">The value of the <see cref="BuildLabel"/> component of this <see cref="SemanticVersion"/>.</param>
    public readonly record struct SemanticVersion(int Major, int Minor, int Patch, string? PreReleaseLabel = null, string? BuildLabel = null) : IComparable<SemanticVersion>
    {
        /// <inheritdoc/>
        /// <remarks>
        /// Precedence refers to how versions are compared to each other when ordered.
        /// <list type="number">
        ///     <item>Precedence MUST be calculated by separating the version into major, minor, patch, and pre-release identifiers in that order. Build metadata does not figure into precedence.</item>
        ///     <item>
        ///         Precedence is determined by the first difference when comparing each of these identifiers from left to right as follows: major, minor, and patch versions are always compared numerically.
        ///         <code>1.0.0 &lt; 2.0.0 &lt; 2.1.0 &lt; 2.1.1</code>
        ///     </item>
        ///     <item>
        ///         When major, minor, and patch are equal, a pre-release version has lower precedence than a normal version.
        ///         <code>1.0.0-alpha &lt; 1.0.0</code>
        ///     </item>
        ///     <item>
        ///         Precedence for two pre-release versions with the same major, minor, and patch version MUST be determined by comparing each dot-separated identifier from left to right until a difference is found, as follows:
        ///         <list type="number">
        ///             <item>Identifiers consisting only of digits are compared numerically.</item>
        ///             <item>Identifiers with letters or hyphens are compared lexically in ASCII sort order.</item>
        ///             <item>Numeric identifiers always have lower precedence than non-numeric identifiers.</item>
        ///             <item>A larger set of pre-release fields has a higher precedence than a smaller set if all of the proceeding identifiers are equal.</item>
        ///         </list>
        ///         <code> 1.0.0-alpha &lt; 1.0.0-alpha.1 &lt; 1.0.0-alpha.beta &lt; 1.0.0-beta &lt; 1.0.0-beta.2 &lt; 1.0.0-beta.11 &lt; 1.0.0-rc.1 &lt; 1.0.0</code>
        ///     </item>
        /// </list>
        /// </remarks>
        public int CompareTo(SemanticVersion other)
        {
            // First compare major, minor, and patch values.
            var result =
                Major != other.Major ? (Major > other.Major ? 1 : -1) :
                Minor != other.Minor ? (Minor > other.Minor ? 1 : -1) :
                Patch != other.Patch ? (Patch > other.Patch ? 1 : -1) :
                0;

            // If the major, minor, and patch values do not match, return that value.
            if (result != 0)
            {
                return result;
            }

            // Major, minor, and patch match. Let's look at the pre-release label.
            string? left = PreReleaseLabel?.ToLowerInvariant();
            string? right = other.PreReleaseLabel?.ToLowerInvariant();
            return (left, right) switch
            {
                (null, null) => 0,
                (null, { }) => 1,
                ({ }, null) => -1,
                _ => string.Compare(left, right)
            };
        }

        /// <inheritdoc/>
        /// <remarks>As per the specifications, <see cref="SemanticVersion.BuildLabel"/> is ignored in this operation.</remarks>
        public bool Equals(SemanticVersion other)
        {
            return Major == other.Major &&
                    Minor == other.Minor &&
                    Patch == other.Patch &&
                    PreReleaseLabel == other.PreReleaseLabel;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Major, Minor, Patch, PreReleaseLabel, BuildLabel);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var value = $"{Major}.{Minor}.{Patch}";

            if (PreReleaseLabel is not null)
            {
                value += $"-{PreReleaseLabel}";
            }

            if (BuildLabel is not null)
            {
                value += $"+{BuildLabel}";
            }

            return value;
        }

        // Do not comment on operators.
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static bool operator <(SemanticVersion left, SemanticVersion right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(SemanticVersion left, SemanticVersion right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(SemanticVersion left, SemanticVersion right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(SemanticVersion left, SemanticVersion right)
        {
            return left.CompareTo(right) >= 0;
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Creates a new instance of <see cref="SemanticVersion"/> using the provided <paramref name="version"/>.
        /// </summary>
        /// <param name="version">The version to use as a base for this <see cref="SemanticVersion"/>.</param>
        /// <returns>A new instance of <see cref="SemanticVersion"/>.</returns>
        public static SemanticVersion FromVersion(Version version)
        {
            return new SemanticVersion(version.Major, version.Minor, version.Revision, PreReleaseLabel: null, BuildLabel: version.Build.ToString());
        }

        /// <summary>
        /// Implicitly converts a <see cref="Version"/> into an instance of <see cref="SemanticVersion"/>.
        /// </summary>
        /// <param name="version">The version to convert.</param>
        public static implicit operator SemanticVersion(Version version)
            => FromVersion(version);
    }
}
