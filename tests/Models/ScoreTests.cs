using Xunit;
using openrmf_msg_score.Models;
using System;
using MongoDB.Bson;

namespace tests.Models
{
    public class ScoreTests
    {
        // ---- Pass Tests ----

        [Fact]
        public void Test_NewScore_IsNotNull()
        {
            var score = new Score();
            Assert.NotNull(score);
        }

        [Fact]
        public void Test_NewScore_AllCounters_DefaultToZero()
        {
            var score = new Score();
            Assert.Equal(0, score.totalCat1Open);
            Assert.Equal(0, score.totalCat1NotApplicable);
            Assert.Equal(0, score.totalCat1NotAFinding);
            Assert.Equal(0, score.totalCat1NotReviewed);
            Assert.Equal(0, score.totalCat2Open);
            Assert.Equal(0, score.totalCat2NotApplicable);
            Assert.Equal(0, score.totalCat2NotAFinding);
            Assert.Equal(0, score.totalCat2NotReviewed);
            Assert.Equal(0, score.totalCat3Open);
            Assert.Equal(0, score.totalCat3NotApplicable);
            Assert.Equal(0, score.totalCat3NotAFinding);
            Assert.Equal(0, score.totalCat3NotReviewed);
        }

        [Fact]
        public void Test_NewScore_ComputedTotals_DefaultToZero()
        {
            var score = new Score();
            Assert.Equal(0, score.totalOpen);
            Assert.Equal(0, score.totalNotApplicable);
            Assert.Equal(0, score.totalNotAFinding);
            Assert.Equal(0, score.totalNotReviewed);
            Assert.Equal(0, score.totalCat1);
            Assert.Equal(0, score.totalCat2);
            Assert.Equal(0, score.totalCat3);
        }

        [Fact]
        public void Test_Score_WithIdentityFields_Pass()
        {
            var score = new Score
            {
                systemGroupId = "sys-group-001",
                hostName = "prodServer",
                stigType = "Google Chrome",
                stigRelease = "V1R30",
                created = DateTime.UtcNow,
                updatedOn = DateTime.UtcNow,
                createdBy = Guid.NewGuid()
            };

            Assert.NotEmpty(score.systemGroupId);
            Assert.NotEmpty(score.hostName);
            Assert.NotEmpty(score.stigType);
            Assert.NotEmpty(score.stigRelease);
            Assert.NotEmpty(score.title);
            Assert.True(score.updatedOn.HasValue);
            Assert.NotEqual(Guid.Empty, score.createdBy);
        }

        [Fact]
        public void Test_Score_Title_IncludesHostAndStigInfo()
        {
            var score = new Score
            {
                hostName = "myServer",
                stigType = "Chrome",
                stigRelease = "V1R1"
            };
            Assert.Contains("myServer", score.title);
            Assert.Contains("Chrome", score.title);
            Assert.Contains("V1R1", score.title);
        }

        [Fact]
        public void Test_Score_Title_UsesUnknownHostWhenNull()
        {
            var score = new Score { hostName = null, stigType = "Chrome", stigRelease = "V1R1" };
            Assert.Contains("UnknownHost", score.title);
        }

        [Fact]
        public void Test_Score_CalculatedTotals_ComputeCorrectly()
        {
            var score = new Score
            {
                totalCat1Open = 1, totalCat1NotApplicable = 1,
                totalCat1NotAFinding = 1, totalCat1NotReviewed = 1,
                totalCat2Open = 3, totalCat2NotApplicable = 5,
                totalCat2NotAFinding = 10, totalCat2NotReviewed = 20,
                totalCat3Open = 8, totalCat3NotApplicable = 7,
                totalCat3NotAFinding = 10, totalCat3NotReviewed = 10
            };

            Assert.Equal(12, score.totalOpen);
            Assert.Equal(13, score.totalNotApplicable);
            Assert.Equal(21, score.totalNotAFinding);
            Assert.Equal(31, score.totalNotReviewed);
            Assert.Equal(4, score.totalCat1);
            Assert.Equal(38, score.totalCat2);
            Assert.Equal(35, score.totalCat3);
        }

        [Fact]
        public void Test_Score_TotalOpen_SumsCat1Cat2Cat3Open()
        {
            var score = new Score { totalCat1Open = 2, totalCat2Open = 5, totalCat3Open = 3 };
            Assert.Equal(10, score.totalOpen);
        }

        [Fact]
        public void Test_Score_TotalCat1_SumsAllCat1Statuses()
        {
            var score = new Score
            {
                totalCat1Open = 1, totalCat1NotApplicable = 2,
                totalCat1NotAFinding = 3, totalCat1NotReviewed = 4
            };
            Assert.Equal(10, score.totalCat1);
        }

        [Fact]
        public void Test_Score_ArtifactId_CanBeSet()
        {
            var score = new Score();
            var oid = ObjectId.GenerateNewId();
            score.artifactId = oid;
            Assert.Equal(oid, score.artifactId);
        }

        [Fact]
        public void Test_Score_UpdatedBy_CanBeSet()
        {
            var score = new Score();
            var guid = Guid.NewGuid();
            score.updatedBy = guid;
            Assert.True(score.updatedBy.HasValue);
            Assert.Equal(guid, score.updatedBy.Value);
        }

        [Theory]
        [InlineData(1, 0, 0, 0, 1)]
        [InlineData(2, 3, 4, 5, 14)]
        [InlineData(0, 0, 0, 0, 0)]
        public void Test_Score_TotalCat1_Calculation(int open, int na, int naf, int nr, int expected)
        {
            var score = new Score
            {
                totalCat1Open = open,
                totalCat1NotApplicable = na,
                totalCat1NotAFinding = naf,
                totalCat1NotReviewed = nr
            };
            Assert.Equal(expected, score.totalCat1);
        }

        [Theory]
        [InlineData(1, 2, 3, 6)]
        [InlineData(0, 0, 0, 0)]
        [InlineData(5, 10, 15, 30)]
        public void Test_Score_TotalOpen_Calculation(int cat1, int cat2, int cat3, int expected)
        {
            var score = new Score
            {
                totalCat1Open = cat1,
                totalCat2Open = cat2,
                totalCat3Open = cat3
            };
            Assert.Equal(expected, score.totalOpen);
        }

        // ---- Fail Tests ----

        [Fact]
        public void Test_Score_EmptyHostName_TitleDoesNotContainServer()
        {
            var score = new Score { hostName = string.Empty, stigType = "Chrome", stigRelease = "V1R1" };
            Assert.DoesNotContain("prodServer", score.title);
        }

        [Fact]
        public void Test_Score_UpdatedOn_NullByDefault()
        {
            var score = new Score();
            Assert.False(score.updatedOn.HasValue);
        }

        [Fact]
        public void Test_Score_CreatedBy_DefaultIsEmptyGuid()
        {
            var score = new Score();
            Assert.Equal(Guid.Empty, score.createdBy);
        }

        [Fact]
        public void Test_Score_TotalOpen_NotCountingNAOrNAF()
        {
            var score = new Score
            {
                totalCat1NotApplicable = 5,
                totalCat1NotAFinding = 10,
                totalCat2Open = 3
            };
            Assert.NotEqual(15, score.totalOpen);
            Assert.Equal(3, score.totalOpen);
        }

        [Fact]
        public void Test_Score_SystemGroupId_NullByDefault()
        {
            var score = new Score();
            Assert.Null(score.systemGroupId);
        }
    }
}
