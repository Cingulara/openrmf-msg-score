using Xunit;
using openrmf_msg_score.Models;
using System;
using System.Collections.Generic;

namespace tests.Models
{
    public class ArtifactTests
    {
        // ---- Pass Tests ----

        [Fact]
        public void Test_NewArtifact_IsNotNull()
        {
            var art = new Artifact();
            Assert.NotNull(art);
        }

        [Fact]
        public void Test_NewArtifact_HasDefaultChecklist()
        {
            var art = new Artifact();
            Assert.NotNull(art.CHECKLIST);
        }

        [Fact]
        public void Test_NewArtifact_DefaultWebDatabaseIsFalse()
        {
            var art = new Artifact();
            Assert.False(art.isWebDatabase);
        }

        [Fact]
        public void Test_NewArtifact_DefaultWebDatabaseSiteIsEmpty()
        {
            var art = new Artifact();
            Assert.Equal(string.Empty, art.webDatabaseSite);
        }

        [Fact]
        public void Test_NewArtifact_DefaultWebDatabaseInstanceIsEmpty()
        {
            var art = new Artifact();
            Assert.Equal(string.Empty, art.webDatabaseInstance);
        }

        [Fact]
        public void Test_Artifact_WithAllFields_Pass()
        {
            var art = new Artifact();
            art.created = DateTime.Now;
            art.systemGroupId = "hgt786575647rgkjghg";
            art.hostName = "myHost";
            art.stigType = "Google Chrome";
            art.stigRelease = "Version 1";
            art.version = "1";
            art.updatedOn = DateTime.Now;
            art.createdBy = Guid.NewGuid();
            art.updatedBy = Guid.NewGuid();
            art.rawChecklist = "<CHECKLIST/>"
;
            art.InternalId = "abc123";
            art.tags = new List<string> { "tag1", "tag2" };
            art.isWebDatabase = true;
            art.webDatabaseSite = "https://example.com";
            art.webDatabaseInstance = "prod";

            Assert.NotNull(art);
            Assert.NotEmpty(art.systemGroupId);
            Assert.NotEmpty(art.hostName);
            Assert.NotEmpty(art.stigType);
            Assert.NotEmpty(art.stigRelease);
            Assert.NotEmpty(art.version);
            Assert.NotEmpty(art.title);
            Assert.True(art.updatedOn.HasValue);
            Assert.NotEqual(Guid.Empty, art.createdBy);
            Assert.True(art.updatedBy.HasValue);
            Assert.NotNull(art.tags);
            Assert.Equal(2, art.tags.Count);
            Assert.True(art.isWebDatabase);
            Assert.NotEmpty(art.webDatabaseSite);
            Assert.NotEmpty(art.webDatabaseInstance);
        }

        [Fact]
        public void Test_Artifact_Title_IncludesHostNameAndStigInfo()
        {
            var art = new Artifact();
            art.hostName = "MyHost";
            art.stigType = "Chrome";
            art.stigRelease = "V1R1";
            art.version = "1";

            Assert.Contains("MyHost", art.title);
            Assert.Contains("Chrome", art.title);
            Assert.Contains("V1R1", art.title);
        }

        [Fact]
        public void Test_Artifact_Title_UsesUnknownWhenHostNameIsEmpty()
        {
            var art = new Artifact();
            art.hostName = string.Empty;
            art.stigType = "Chrome";
            art.stigRelease = "V1R1";
            art.version = "1";

            Assert.Contains("Unknown", art.title);
        }

        [Fact]
        public void Test_Artifact_Created_StoresDateCorrectly()
        {
            var expected = new DateTime(2025, 1, 15, 10, 0, 0);
            var art = new Artifact { created = expected };
            Assert.Equal(expected, art.created);
        }

        [Fact]
        public void Test_Artifact_SystemGroupId_IsSet()
        {
            var art = new Artifact { systemGroupId = "sys-001" };
            Assert.Equal("sys-001", art.systemGroupId);
        }

        [Fact]
        public void Test_Artifact_Tags_CanBeAssigned()
        {
            var art = new Artifact();
            art.tags = new List<string> { "alpha", "beta" };
            Assert.Equal(2, art.tags.Count);
            Assert.Contains("alpha", art.tags);
        }

        // ---- Fail Tests ----

        [Fact]
        public void Test_Artifact_HostNameNull_TitleUsesUnknown()
        {
            var art = new Artifact();
            art.hostName = null;
            art.stigType = "Chrome";
            art.stigRelease = "V1R1";
            art.version = "1";

            // title falls back to "Unknown" when hostName is null
            Assert.Contains("Unknown", art.title);
        }

        [Fact]
        public void Test_Artifact_UpdatedOn_NullByDefault()
        {
            var art = new Artifact();
            Assert.False(art.updatedOn.HasValue);
        }

        [Fact]
        public void Test_Artifact_CreatedBy_DefaultIsEmptyGuid()
        {
            var art = new Artifact();
            Assert.Equal(Guid.Empty, art.createdBy);
        }

        [Fact]
        public void Test_Artifact_EmptySystemGroupId_IsNullOrEmpty()
        {
            var art = new Artifact();
            Assert.True(string.IsNullOrEmpty(art.systemGroupId));
        }

        [Theory]
        [InlineData("host1", "Chrome", "V1R1", "1", "host1-Chrome-V1-V1R1")]
        [InlineData("server", "Windows 10", "V2R3", "2", "server-Windows 10-V2-V2R3")]
        public void Test_Artifact_Title_ComposedCorrectly(string host, string stigType, string stigRelease, string version, string expected)
        {
            var art = new Artifact
            {
                hostName = host,
                stigType = stigType,
                stigRelease = stigRelease,
                version = version
            };
            Assert.Equal(expected, art.title);
        }
    }
}
