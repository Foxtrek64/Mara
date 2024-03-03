//
//  SemanticVersion.cs
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
using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Unicode;
using JetBrains.Annotations;

namespace Mara.Common.Models
{
    /// <summary>
    /// A <see cref="Version"/> which follows the specification established at <see href="https://semver.org/spec/v2.0.0.html"/>.
    /// </summary>
    /// <param name="major">The <see cref="Major"/> component, to be incremented for breaking additions or changes.</param>
    /// <param name="minor">The <see cref="Minor"/> component, to be incremented for non-breaking additions or changes.</param>
    /// <param name="patch">The <see cref="Patch"/> component, to be incremented for non-breaking bug fixes.</param>
    /// <param name="preReleaseLabel">The <see cref="PreReleaseLabel"/> component, marking this component as being in beta and providing .</param>
    /// <param name="buildLabel">The value of the <see cref="BuildLabel"/> component of this <see cref="SemanticVersion"/>.</param>
    [PublicAPI]
    public readonly struct SemanticVersion
    (
        uint major,
        uint minor,
        uint patch,
        string? preReleaseLabel = null,
        string? buildLabel = null
    )
        : IComparable,
          IComparable<SemanticVersion>,
          IEquatable<SemanticVersion>,
#if NET8_0_OR_GREATER
          ISpanFormattable,
          IUtf8SpanFormattable
#else
          ISpanFormattable
#endif
    {
        /// <summary>
        /// Gets the Major compontent of this <see cref="SemanticVersion"/>.
        /// </summary>
        public uint Major { get; } = major;

        /// <summary>
        /// Gets the Minor component of this <see cref="SemanticVersion"/>.
        /// </summary>
        public uint Minor { get; } = minor;

        /// <summary>
        /// Gets the Patch component of this <see cref="SemanticVersion"/>.
        /// </summary>
        public uint Patch { get; } = patch;

        /// <summary>
        /// Gets the PreReleaseLabel component of this <see cref="SemanticVersion"/>.
        /// </summary>
        public string? PreReleaseLabel { get; } = preReleaseLabel;

        /// <summary>
        /// Gets the BuildLabel component of this <see cref="SemanticVersion"/>.
        /// </summary>
        public string? BuildLabel { get; } = buildLabel;

        /// <inheritdoc />
        public int CompareTo(object? obj)
        {
            return obj is SemanticVersion other
                ? CompareTo(other)
                : throw new InvalidOperationException();
        }

        /// <inheritdoc />
        /// <remarks>
        /// Precedence refers to how versions are compared to each other when ordered.
        /// <list type="number">
        ///     <item>Precedence MUST be calculated by separating the version into major, minor, patch, and pre-release
        /// identifiers in that order. Build metadata does not figure into precedence.</item>
        ///     <item>
        ///         Precedence is determined by the first difference when comparing each of these identifiers from left
        /// to right as follows: major, minor, and patch versions are always compared numerically.
        ///         <code>1.0.0 &lt; 2.0.0 &lt; 2.1.0 &lt; 2.1.1</code>
        ///     </item>
        ///     <item>
        ///         When major, minor, and patch are equal, a pre-release version has lower precedence than a normal
        /// version.
        ///         <code>1.0.0-alpha &lt; 1.0.0</code>
        ///     </item>
        ///     <item>
        ///         Precedence for two pre-release versions with the same major, minor, and patch version MUST be
        /// determined by comparing each dot-separated identifier from left to right until a difference is found, as follows:
        ///         <list type="number">
        ///             <item>Identifiers consisting only of digits are compared numerically.</item>
        ///             <item>Identifiers with letters or hyphens are compared lexically in ASCII sort order.</item>
        ///             <item>Numeric identifiers always have lower precedence than non-numeric identifiers.</item>
        ///             <item>A larger set of pre-release fields has a higher precedence than a smaller set if all of
        /// the proceeding identifiers are equal.</item>
        ///         </list>
        ///     </item>
        /// </list>
        /// </remarks>
        /// <example>1.0.0-alpha &lt; 1.0.0-alpha.1 &lt; 1.0.0-alpha.beta &lt; 1.0.0-beta &lt; 1.0.0-beta.2 &lt; 1.0.0-beta.11 &lt; 1.0.0-rc.1 &lt; 1.0.0.</example>
        public int CompareTo(SemanticVersion other)
        {
            // 1. Compare major, minor, and patch numerically.
            // 1. If one is greater than or less than the other, return.
            int majorComparison = Major.CompareTo(other.Major);
            if (majorComparison != 0)
            {
                return majorComparison;
            }

            int minorComparison = Minor.CompareTo(other.Minor);
            if (minorComparison != 0)
            {
                return minorComparison;
            }

            int patchComparison = Patch.CompareTo(other.Patch);
            if (patchComparison != 0)
            {
                return patchComparison;
            }

            // If major, minor, and patch are even, check pre-release.
            string? left = PreReleaseLabel?.ToLowerInvariant();
            string? right = other.PreReleaseLabel?.ToLowerInvariant();

            return (left, right) switch
            {
                (null, null) => 0, // No pre-release labels. Even.
                (null, not null) => 1, // Other has pre-release label. This version is newer.
                (not null, null) => -1, // This has pre-release label. Other version is newer.
                _ => LexicographicalCompare(left, right) // Both have a pre-release label. Compare lexicographically.
            };

            static int LexicographicalCompare(string left, string right)
            {
                var leftArray = left.Split('.');
                var rightArray = right.Split('.');

                for (int i = 0; ; i++)
                {
                    if (i == leftArray.Length || i == rightArray.Length)
                    {
                        break;
                    }

                    // We have remaining elements to compare
                    var leftEntry = leftArray[i];
                    var rightEntry = rightArray[i];

                    uint? leftNum = uint.TryParse(leftEntry, out uint parseResult) ? parseResult : null;
                    uint? rightNum = uint.TryParse(rightEntry, out parseResult) ? parseResult : null;

                    int comparison = (leftNum, rightNum) switch
                    {
                        (not null, not null) => leftNum.Value.CompareTo(rightNum),
                        (not null, null) => -1, // Numeric identifiers always have lower precedence
                        (null, not null) => 1,
                        (null, null) => string.Compare(leftEntry, rightEntry, StringComparison.OrdinalIgnoreCase)
                    };
                    if (comparison != 0)
                    {
                        return comparison;
                    }
                }

                // All elements were equal and we've run out of elements to compare.
                // If one is longer, that has higher precedence.
                // If they're the same length, then they are equal.
                return leftArray.Length.CompareTo(rightArray.Length);
            }
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is SemanticVersion other && Equals(other);
        }

        /// <inheritdoc />
        public bool Equals(SemanticVersion other)
        {
            return Major == other.Major &&
                Minor == other.Minor &&
                Patch == other.Patch &&
                PreReleaseLabel == other.PreReleaseLabel;
        }

        /// <summary>
        /// Compares two instances of <see cref="SemanticVersion"/> for equality.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <returns>True if the SemanticVersions are equal; otherwise, false.</returns>
        public static bool Equals(SemanticVersion left, SemanticVersion right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Major, Minor, Patch, PreReleaseLabel);
        }

        /// <summary>
        /// Returns a string representation of the value contained in this <see cref="SemanticVersion"/>.
        /// </summary>
        /// <returns><see cref="major"/>.<see cref="minor"/>.<see cref="patch"/>-<see cref="preReleaseLabel"/>+<see cref="buildLabel"/>.</returns>
        public override string ToString()
        {
            return ToString(GetFieldCount());
        }

        /// <summary>
        /// Returns a string representation of the value contained in this <see cref="SemanticVersion"/>.
        /// </summary>
        /// <param name="fieldCount">The number of fields to include in the result.</param>
        /// <returns><see cref="major"/>.<see cref="minor"/>.<see cref="patch"/>-<see cref="preReleaseLabel"/>+<see cref="buildLabel"/>.</returns>
        public string ToString(int fieldCount)
        {
            const int Int32NumberBufferLength = 10 + 1;
            Span<char> dest = stackalloc char[(5 * Int32NumberBufferLength) + 5]; // 3 numbers, 2 strings, 5 separators.
            bool success = TryFormat(dest, fieldCount, out int charsWritten);
            Debug.Assert(success, "Format successful!");
            return dest[..charsWritten].ToString();
        }

        /// <inheritdoc/>
        string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString();

        /// <summary>
        /// Tries to format the value of the current instance into the provided span of characters.
        /// </summary>
        /// <param name="destination">The span in which to write this instance's value formatted as a span of characters.</param>
        /// <param name="charsWritten">When this method returns, contains the number of characters that were written in destination.</param>
        /// <returns><see langword="true"/> if the formatting was successful; otherwise, <see langword="false"/>.</returns>
        public bool TryFormat(Span<char> destination, out int charsWritten)
            => TryFormatCore(destination, GetFieldCount(), out charsWritten);

        /// <summary>
        /// Tries to format the value of the current instance into the provided span of characters.
        /// </summary>
        /// <param name="destination">The span in which to write this instance's value formatted as a span of characters.</param>
        /// <param name="fieldCount">The number of fields to return in the formatted result.</param>
        /// <param name="charsWritten">When this method returns, contains the number of characters that were written in destination.</param>
        /// <returns><see langword="true"/> if the formatting was successful; otherwise, <see langword="false"/>.</returns>
        public bool TryFormat(Span<char> destination, int fieldCount, out int charsWritten)
            => TryFormatCore(destination, fieldCount, out charsWritten);

        /// <inheritdoc/>
        bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
            => TryFormatCore(destination, GetFieldCount(), out charsWritten);

        /// <summary>
        /// Tries to format the value of the current instance into the provided span of characters.
        /// </summary>
        /// <param name="utf8Destination">The span in which to write this instance's value formatted as a span of bytes.</param>
        /// <param name="bytesWritten">When this method returns, contains the number of bytes that were written in destination.</param>
        /// <returns><see langword="true"/> if the formatting was successful; otherwise, <see langword="false"/>.</returns>
        public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten)
            => TryFormatCore(utf8Destination, GetFieldCount(), out bytesWritten);

        /// <summary>
        /// Tries to format the value of the current instance into the provided span of characters.
        /// </summary>
        /// <param name="utf8Destination">The span in which to write this instance's value formatted as a span of bytes.</param>
        /// <param name="fieldCount">The number of fields to return in the formatted result.</param>
        /// <param name="bytesWritten">When this method returns, contains the number of bytes that were written in destination.</param>
        /// <returns><see langword="true"/> if the formatting was successful; otherwise, <see langword="false"/>.</returns>
        public bool TryFormat(Span<byte> utf8Destination, int fieldCount, out int bytesWritten)
            => TryFormatCore(utf8Destination, fieldCount, out bytesWritten);

        /// <inheritdoc/>
        bool IUtf8SpanFormattable.TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
            => TryFormatCore(utf8Destination, GetFieldCount(), out bytesWritten);

        private int GetFieldCount()
        {
            int formatFieldCount = 3;
            if (PreReleaseLabel is not null)
            {
                formatFieldCount++;
            }

            if (BuildLabel is not null)
            {
                formatFieldCount++;
            }

            return formatFieldCount;
        }

        private bool TryFormatCore<TChar>(Span<TChar> destination, int fieldCount, out int charsWritten)
            where TChar : unmanaged, IBinaryInteger<TChar>
        {
            const char DecimalSeparator = '.';
            const char PreReleaseLabelSeparator = '-';
            const char BuildLabelSeparator = '+';

            switch ((uint)fieldCount)
            {
                case > 5:
                    ThrowArgumentException(5);
                    break;
                case > 4 when BuildLabel is null:
                    ThrowArgumentException(4);
                    break;
                case > 4 when PreReleaseLabel is null:
                    ThrowArgumentException(4);
                    break;
                case > 3 when BuildLabel is null && PreReleaseLabel is null:
                    ThrowArgumentException(3);
                    break;
            }

            int totalCharactersWritten = 0;

            for (int i = 0; i < fieldCount; i++)
            {
                if (i != 0)
                {
                    if (destination.IsEmpty)
                    {
                        charsWritten = 0;
                        return false;
                    }

                    destination[0] = i switch
                    {
                        1 => TChar.CreateTruncating(DecimalSeparator),
                        2 => TChar.CreateTruncating(DecimalSeparator),
                        3 when PreReleaseLabel is not null => TChar.CreateTruncating(PreReleaseLabelSeparator),
                        3 when PreReleaseLabel is null => TChar.CreateTruncating(BuildLabelSeparator),
                        4 => TChar.CreateTruncating(BuildLabelSeparator),
                        _ => throw new InvalidOperationException() // not reachable but required.
                    };
                    destination = destination[1..];
                    totalCharactersWritten++;
                }

                int valueCharsWritten;
                bool formatted = i switch
                {
                    0 => FormatUInt(Major, destination, out valueCharsWritten),
                    1 => FormatUInt(Minor, destination, out valueCharsWritten),
                    2 => FormatUInt(Patch, destination, out valueCharsWritten),
                    3 when PreReleaseLabel is not null => FormatString(PreReleaseLabel, destination, out valueCharsWritten),
                    3 when PreReleaseLabel is null => FormatString(BuildLabel!, destination, out valueCharsWritten),
                    _ => FormatString(BuildLabel!, destination, out valueCharsWritten)
                };

                if (!formatted)
                {
                    charsWritten = 0;
                    return false;
                }

                totalCharactersWritten += valueCharsWritten;
                destination = destination[valueCharsWritten..];
            }

            charsWritten = totalCharactersWritten;
            return true;

            static bool FormatUInt(uint value, Span<TChar> destination, out int totalCharsWritten)
                => typeof(TChar) == typeof(char)
                    ? value.TryFormat(MemoryMarshal.Cast<TChar, char>(destination), out totalCharsWritten)
                    : value.TryFormat
                    (
                        MemoryMarshal.Cast<TChar, byte>(destination),
                        out totalCharsWritten,
                        default,
                        CultureInfo.InvariantCulture
                    );

            static bool FormatString(string value, Span<TChar> destination, out int totalCharsWritten)
            {
                if (typeof(TChar) == typeof(char))
                {
                    totalCharsWritten = value.Length;
                    return value.AsSpan().TryCopyTo(BitCast<char>(ref destination, value.Length));
                }

                return Utf8.FromUtf16
                    (
                        value,
                        BitCast<byte>(ref destination, value.Length),
                        out _,
                        out totalCharsWritten
                    ) == OperationStatus.Done;
            }

            static void ThrowArgumentException(int failureUpperBound)
                => throw new ArgumentException
                    ($"Value must be between 0 and {failureUpperBound}", nameof(fieldCount));

            static Span<TValue> BitCast<TValue>(ref Span<TChar> destination, int length)
                => MemoryMarshal.CreateSpan
                    (ref Unsafe.As<TChar, TValue>(ref MemoryMarshal.GetReference(destination)), length);
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
        /// Explicitly converts a <see cref="Version"/> into an instance of <see cref="SemanticVersion"/>.
        /// </summary>
        /// <param name="version">The version to convert.</param>
        public static explicit operator SemanticVersion(Version version)
            => new
            (
                major: (uint)version.Major,
                minor: (uint)version.Minor,
                patch: (uint)version.Revision,
                preReleaseLabel: null,
                buildLabel: version.Build.ToString()
            );
    }
}
