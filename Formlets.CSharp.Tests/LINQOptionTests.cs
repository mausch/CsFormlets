using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Microsoft.FSharp.Core;

namespace Formlets.CSharp.Tests
{
    public class LINQOptionTests
    {
        class VersionSpec
        {
            public readonly Version MinVersion;
            public readonly bool IsMinInclusive;
            public readonly Version MaxVersion;
            public readonly bool IsMaxInclusive;

            public VersionSpec(Version minVersion, bool isMinInclusive, Version maxVersion, bool isMaxInclusive)
            {
                this.MinVersion = minVersion;
                this.IsMinInclusive = isMinInclusive;
                this.MaxVersion = maxVersion;
                this.IsMaxInclusive = isMaxInclusive;
            }
        }

        FSharpOption<Version> ParseVersion(string value)
        {
            Version v;
            var ok = Version.TryParse(value, out v);
            if (ok)
                return v.ToOption();
            return FSharpOption<Version>.None;
        }

        FSharpOption<VersionSpec> ParseVersionSpec(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            value = value.Trim();
            var v = ParseVersion(value);
            if (v.HasValue())
                return new VersionSpec(v.Value, true, null, false).ToOption();
            var checkLength = L.F((string val) => val.Length < 3 ? FSharpOption<Unit>.None : FSharpOption.SomeUnit);
            var minInclusive = L.F((string val) => {
                var c = val.First();
                if (c == '[')
                    return true.ToOption();
                if (c == '(')
                    return false.ToOption();
                return FSharpOption<bool>.None;
            });
            var maxInclusive = L.F((string val) => {
                var c = val.Last();
                if (c == ']')
                    return true.ToOption();
                if (c == ')')
                    return false.ToOption();
                return FSharpOption<bool>.None;
            });
            var checkParts = L.F((string[] parts) => parts.Length > 2 ? FSharpOption<Unit>.None : FSharpOption.SomeUnit);
            var minVersion = L.F((string[] parts) => {
                if (string.IsNullOrWhiteSpace(parts[0]))
                    return FSharpOption<Version>.Some(null);
                return ParseVersion(parts[0]);
            });
            var maxVersion = L.F((string[] parts) => {
                var p = parts.Length == 2 ? parts[1] : parts[0];
                if (string.IsNullOrWhiteSpace(p))
                    return FSharpOption<Version>.Some(null);
                return ParseVersion(p);
            });
            return from x in checkLength(value)
                   from isMin in minInclusive(value)
                   from isMax in maxInclusive(value)
                   let val = value.Substring(1, value.Length-2)
                   let parts = val.Split(',')
                   from y in checkParts(parts)
                   from min in minVersion(parts)
                   from max in maxVersion(parts)
                   select new VersionSpec(min, isMin, max, isMax);
        }

        [Fact]
        public void ParseVersionSpecSimpleVersionNoBrackets() {
            var v = ParseVersionSpec("2.1");
            Assert.True(v.HasValue());
            Assert.Equal("2.1", v.Value.MinVersion.ToString());
            Assert.Null(v.Value.MaxVersion);
            Assert.False(v.Value.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecSimpleVersionNoBracketsExtraSpaces() {
            var v = ParseVersionSpec("  1  .  2  ");
            Assert.True(v.HasValue());
            Assert.Equal("1.2", v.Value.MinVersion.ToString());
            Assert.Null(v.Value.MaxVersion);
            Assert.False(v.Value.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecMaxOnlyInclusive() {
            var v = ParseVersionSpec("(,1.2]");
            Assert.True(v.HasValue());
            Assert.Null(v.Value.MinVersion);
            Assert.False(v.Value.IsMinInclusive);
            Assert.Equal("1.2", v.Value.MaxVersion.ToString());
            Assert.True(v.Value.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecMaxOnlyExclusive() {
            var v = ParseVersionSpec("(,1.2)");
            Assert.True(v.HasValue());
            Assert.Null(v.Value.MinVersion);
            Assert.False(v.Value.IsMinInclusive);
            Assert.Equal("1.2", v.Value.MaxVersion.ToString());
            Assert.False(v.Value.IsMaxInclusive);
        }


        [Fact]
        public void ParseVersionSpecExactVersion()
        {
            // Act
            var versionInfo = ParseVersionSpec("[1.2]");

            // Assert
            Assert.Equal("1.2", versionInfo.Value.MinVersion.ToString());
            Assert.True(versionInfo.Value.IsMinInclusive);
            Assert.Equal("1.2", versionInfo.Value.MaxVersion.ToString());
            Assert.True(versionInfo.Value.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecMinOnlyExclusive()
        {
            // Act
            var versionInfo = ParseVersionSpec("(1.2,)");

            // Assert
            Assert.Equal("1.2", versionInfo.Value.MinVersion.ToString());
            Assert.False(versionInfo.Value.IsMinInclusive);
            Assert.Equal(null, versionInfo.Value.MaxVersion);
            Assert.False(versionInfo.Value.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecRangeExclusiveExclusive()
        {
            // Act
            var versionInfo = ParseVersionSpec("(1.2,2.3)");

            // Assert
            Assert.Equal("1.2", versionInfo.Value.MinVersion.ToString());
            Assert.False(versionInfo.Value.IsMinInclusive);
            Assert.Equal("2.3", versionInfo.Value.MaxVersion.ToString());
            Assert.False(versionInfo.Value.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecRangeExclusiveInclusive()
        {
            // Act
            var versionInfo = ParseVersionSpec("(1.2,2.3]");

            // Assert
            Assert.Equal("1.2", versionInfo.Value.MinVersion.ToString());
            Assert.False(versionInfo.Value.IsMinInclusive);
            Assert.Equal("2.3", versionInfo.Value.MaxVersion.ToString());
            Assert.True(versionInfo.Value.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecRangeInclusiveExclusive()
        {
            // Act
            var versionInfo = ParseVersionSpec("[1.2,2.3)");
            Assert.Equal("1.2", versionInfo.Value.MinVersion.ToString());
            Assert.True(versionInfo.Value.IsMinInclusive);
            Assert.Equal("2.3", versionInfo.Value.MaxVersion.ToString());
            Assert.False(versionInfo.Value.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecRangeInclusiveInclusive()
        {
            // Act
            var versionInfo = ParseVersionSpec("[1.2,2.3]");

            // Assert
            Assert.Equal("1.2", versionInfo.Value.MinVersion.ToString());
            Assert.True(versionInfo.Value.IsMinInclusive);
            Assert.Equal("2.3", versionInfo.Value.MaxVersion.ToString());
            Assert.True(versionInfo.Value.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecRangeInclusiveInclusiveExtraSpaces()
        {
            // Act
            var versionInfo = ParseVersionSpec("   [  1 .2   , 2  .3   ]  ");

            Assert.True(versionInfo.HasValue());

            // Assert
            Assert.Equal("1.2", versionInfo.Value.MinVersion.ToString());
            Assert.True(versionInfo.Value.IsMinInclusive);
            Assert.Equal("2.3", versionInfo.Value.MaxVersion.ToString());
            Assert.True(versionInfo.Value.IsMaxInclusive);
        }

        [Fact]
        public void ParseVersionSpecShort()
        {
            var v = ParseVersionSpec("2");
            Assert.False(v.HasValue());
        }

    }
}
