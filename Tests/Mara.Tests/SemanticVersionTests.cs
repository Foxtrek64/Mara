//
//  SemanticVersionTests.cs
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
using Mara.Common.Models;
using Xunit;
using Xunit.Abstractions;

// Test documentation provided as names
#pragma warning disable CS1591, SA1600

namespace Mara.Tests
{
    /// <summary>
    /// Provides a set of tests for the <see cref="SemanticVersion"/> type.
    /// </summary>
    public class SemanticVersionTests
    {
        private readonly ITestOutputHelper _output;

        public SemanticVersionTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private class SemanticVersionSingle : TheoryData<SemanticVersion>
        {
            public SemanticVersionSingle()
            {
                Add(new SemanticVersion(1, 0, 0, "alpha"));
                Add(new SemanticVersion(1, 0, 0, "alpha.1"));
                Add(new SemanticVersion(1, 0, 0, "alpha.beta"));
                Add(new SemanticVersion(1, 0, 0, "beta"));
                Add(new SemanticVersion(1, 0, 0, "beta.2"));
                Add(new SemanticVersion(1, 0, 0, "beta.11"));
                Add(new SemanticVersion(1, 0, 0, "rc.1"));
                Add(new SemanticVersion(1, 0, 0));
            }
        }

        private class SemanticVersionPairs : TheoryData<SemanticVersion, SemanticVersion>
        {
            public SemanticVersionPairs()
            {
                Add(new SemanticVersion(1, 0, 0, "alpha"), new SemanticVersion(1, 0, 0, "alpha.1"));
                Add(new SemanticVersion(1, 0, 0, "alpha.1"), new SemanticVersion(1, 0, 0, "alpha.beta"));
                Add(new SemanticVersion(1, 0, 0, "alpha.beta"), new SemanticVersion(1, 0, 0, "beta"));
                Add(new SemanticVersion(1, 0, 0, "beta"), new SemanticVersion(1, 0, 0, "beta.2"));
                Add(new SemanticVersion(1, 0, 0, "beta.2"), new SemanticVersion(1, 0, 0, "beta.11"));
                Add(new SemanticVersion(1, 0, 0, "beta.11"), new SemanticVersion(1, 0, 0, "rc.1"));
                Add(new SemanticVersion(1, 0, 0, "rc.1"), new SemanticVersion(1, 0, 0));
            }
        }

        private class SemanticVersionStringPairs : TheoryData<SemanticVersion, string>
        {
            public SemanticVersionStringPairs()
            {
                Add(new SemanticVersion(1, 0, 0), "1.0.0");
                Add(new SemanticVersion(1, 0, 0, "alpha"), "1.0.0-alpha");
                Add(new SemanticVersion(1, 0, 0, buildLabel: "123"), "1.0.0+123");
                Add(new SemanticVersion(1, 0, 0, "alpha", "123"), "1.0.0-alpha+123");
            }
        }

        [Fact]
        public void SemanticVersionFromVersionTests()
        {
            var version = new Version(1, 2, 3, 4);
            var semVer = (SemanticVersion)version;

            Assert.Equal((uint)version.Major, semVer.Major);
            Assert.Equal((uint)version.Minor, semVer.Minor);
            Assert.Equal((uint)version.Revision, semVer.Patch);
            Assert.Equal(version.Build.ToString(), semVer.BuildLabel);
        }

        [Theory]
        [ClassData(typeof(SemanticVersionPairs))]
        public void SemanticVersionComparisonTest(SemanticVersion semver1, SemanticVersion semver2)
        {
            _output.WriteLine($"Assertion: '{semver1}' < '{semver2}'");
            Assert.True(semver1 < semver2);
        }

        [Theory]
        [ClassData(typeof(SemanticVersionStringPairs))]
        public void SemanticVersionToString(SemanticVersion semver, string semverString)
        {
            Assert.Equal(semverString, semver.ToString());
        }

        [Fact]
        public void BuildNumberDoesNotFactorInComparison()
        {
            var semver1 = new SemanticVersion(1, 0, 0);
            var semver2 = new SemanticVersion(1, 0, 0, null, "100");

            Assert.Equal(semver1, semver2);
            Assert.Equal(0, semver1.CompareTo(semver2));
        }
    }
}
