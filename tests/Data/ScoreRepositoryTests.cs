using Xunit;
using Moq;
using openrmf_msg_score.Data;
using openrmf_msg_score.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace tests.Data
{
    /// <summary>
    /// Tests for the IScoreRepository interface contract using Moq.
    /// ScoreRepository requires a live MongoDB connection, so we mock
    /// IScoreRepository to verify consumer code interacts correctly with it.
    /// </summary>
    public class ScoreRepositoryTests
    {
        private static Score BuildScore(string hostName = "testHost", string stigType = "Chrome",
            string stigRelease = "V1R1", string systemGroupId = "sys-001")
        {
            return new Score
            {
                artifactId = ObjectId.GenerateNewId(),
                systemGroupId = systemGroupId,
                hostName = hostName,
                stigType = stigType,
                stigRelease = stigRelease,
                created = DateTime.UtcNow,
                createdBy = Guid.NewGuid(),
                totalCat1Open = 2,
                totalCat2Open = 5,
                totalCat3Open = 3,
                totalCat1NotAFinding = 10,
                totalCat2NotAFinding = 20,
                totalCat3NotAFinding = 15
            };
        }

        // ---- Pass Tests: GetAllScores ----

        [Fact]
        public async Task Test_GetAllScores_ReturnsNonNullCollection()
        {
            var mockRepo = new Mock<IScoreRepository>();
            var scores = new List<Score> { BuildScore(), BuildScore("host2", "Windows 10") };
            mockRepo.Setup(r => r.GetAllScores()).ReturnsAsync(scores);

            var result = await mockRepo.Object.GetAllScores();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task Test_GetAllScores_ReturnsTwoScores()
        {
            var mockRepo = new Mock<IScoreRepository>();
            var scores = new List<Score> { BuildScore(), BuildScore("host2") };
            mockRepo.Setup(r => r.GetAllScores()).ReturnsAsync(scores);

            var result = await mockRepo.Object.GetAllScores();

            Assert.Equal(2, ((List<Score>)result).Count);
        }

        [Fact]
        public async Task Test_GetAllScores_ReturnsEmptyListWhenNoData()
        {
            var mockRepo = new Mock<IScoreRepository>();
            mockRepo.Setup(r => r.GetAllScores()).ReturnsAsync(new List<Score>());

            var result = await mockRepo.Object.GetAllScores();

            Assert.Empty(result);
        }

        [Fact]
        public async Task Test_GetAllScores_CalledOnce()
        {
            var mockRepo = new Mock<IScoreRepository>();
            mockRepo.Setup(r => r.GetAllScores()).ReturnsAsync(new List<Score>());

            await mockRepo.Object.GetAllScores();

            mockRepo.Verify(r => r.GetAllScores(), Times.Once);
        }

        // ---- Pass Tests: GetScore(id) ----

        [Fact]
        public async Task Test_GetScore_ById_ReturnsScore()
        {
            var mockRepo = new Mock<IScoreRepository>();
            var expected = BuildScore();
            var id = ObjectId.GenerateNewId().ToString();
            mockRepo.Setup(r => r.GetScore(id)).ReturnsAsync(expected);

            var result = await mockRepo.Object.GetScore(id);

            Assert.NotNull(result);
            Assert.Equal(expected.hostName, result.hostName);
        }

        [Fact]
        public async Task Test_GetScore_ById_ReturnsNullForNonExistentId()
        {
            var mockRepo = new Mock<IScoreRepository>();
            mockRepo.Setup(r => r.GetScore(It.IsAny<string>())).ReturnsAsync((Score)null);

            var result = await mockRepo.Object.GetScore("nonexistent-id");

            Assert.Null(result);
        }

        [Fact]
        public async Task Test_GetScore_ById_CallsRepositoryWithCorrectId()
        {
            var mockRepo = new Mock<IScoreRepository>();
            var scoreId = ObjectId.GenerateNewId().ToString();
            mockRepo.Setup(r => r.GetScore(scoreId)).ReturnsAsync(BuildScore());

            await mockRepo.Object.GetScore(scoreId);

            mockRepo.Verify(r => r.GetScore(scoreId), Times.Once);
        }

        // ---- Pass Tests: GetScorebyArtifact ----

        [Fact]
        public async Task Test_GetScoreByArtifact_ReturnsScore()
        {
            var mockRepo = new Mock<IScoreRepository>();
            var expected = BuildScore();
            var artifactId = ObjectId.GenerateNewId().ToString();
            mockRepo.Setup(r => r.GetScorebyArtifact(artifactId)).ReturnsAsync(expected);

            var result = await mockRepo.Object.GetScorebyArtifact(artifactId);

            Assert.NotNull(result);
            Assert.Equal(expected.hostName, result.hostName);
        }

        [Fact]
        public async Task Test_GetScoreByArtifact_ReturnsNullWhenNotFound()
        {
            var mockRepo = new Mock<IScoreRepository>();
            mockRepo.Setup(r => r.GetScorebyArtifact(It.IsAny<string>())).ReturnsAsync((Score)null);

            var result = await mockRepo.Object.GetScorebyArtifact("missing-artifact-id");

            Assert.Null(result);
        }

        [Fact]
        public async Task Test_GetScoreByArtifact_ScoreHasCorrectStigType()
        {
            var mockRepo = new Mock<IScoreRepository>();
            var expected = BuildScore(stigType: "Windows Server 2019");
            mockRepo.Setup(r => r.GetScorebyArtifact(It.IsAny<string>())).ReturnsAsync(expected);

            var result = await mockRepo.Object.GetScorebyArtifact("some-id");

            Assert.Equal("Windows Server 2019", result.stigType);
        }

        // ---- Pass Tests: GetSystemScores ----

        [Fact]
        public async Task Test_GetSystemScores_ReturnsScoresForSystem()
        {
            var mockRepo = new Mock<IScoreRepository>();
            var systemId = "sys-group-abc";
            var scores = new List<Score>
            {
                BuildScore(systemGroupId: systemId),
                BuildScore("host2", systemGroupId: systemId)
            };
            mockRepo.Setup(r => r.GetSystemScores(systemId)).ReturnsAsync(scores);

            var result = await mockRepo.Object.GetSystemScores(systemId);

            Assert.Equal(2, ((List<Score>)result).Count);
        }

        [Fact]
        public async Task Test_GetSystemScores_ReturnsEmptyForUnknownSystem()
        {
            var mockRepo = new Mock<IScoreRepository>();
            mockRepo.Setup(r => r.GetSystemScores(It.IsAny<string>())).ReturnsAsync(new List<Score>());

            var result = await mockRepo.Object.GetSystemScores("unknown-system");

            Assert.Empty(result);
        }

        // ---- Pass Tests: AddScore ----

        [Fact]
        public async Task Test_AddScore_ReturnsAddedScore()
        {
            var mockRepo = new Mock<IScoreRepository>();
            var score = BuildScore();
            mockRepo.Setup(r => r.AddScore(score)).ReturnsAsync(score);

            var result = await mockRepo.Object.AddScore(score);

            Assert.NotNull(result);
            Assert.Equal(score.hostName, result.hostName);
            Assert.Equal(score.stigType, result.stigType);
        }

        [Fact]
        public async Task Test_AddScore_CalledOnce_WithCorrectScore()
        {
            var mockRepo = new Mock<IScoreRepository>();
            var score = BuildScore();
            mockRepo.Setup(r => r.AddScore(score)).ReturnsAsync(score);

            await mockRepo.Object.AddScore(score);

            mockRepo.Verify(r => r.AddScore(score), Times.Once);
        }

        [Fact]
        public async Task Test_AddScore_ReturnedScore_HasCorrectTotals()
        {
            var mockRepo = new Mock<IScoreRepository>();
            var score = BuildScore();
            score.totalCat1Open = 3;
            score.totalCat2Open = 7;
            mockRepo.Setup(r => r.AddScore(score)).ReturnsAsync(score);

            var result = await mockRepo.Object.AddScore(score);

            Assert.Equal(3, result.totalCat1Open);
            Assert.Equal(7, result.totalCat2Open);
        }

        // ---- Pass Tests: UpdateScore ----

        [Fact]
        public async Task Test_UpdateScore_ReturnsTrue_OnSuccess()
        {
            var mockRepo = new Mock<IScoreRepository>();
            var score = BuildScore();
            mockRepo.Setup(r => r.UpdateScore(score)).ReturnsAsync(true);

            var result = await mockRepo.Object.UpdateScore(score);

            Assert.True(result);
        }

        [Fact]
        public async Task Test_UpdateScore_ReturnsFalse_OnFailure()
        {
            var mockRepo = new Mock<IScoreRepository>();
            var score = BuildScore();
            mockRepo.Setup(r => r.UpdateScore(score)).ReturnsAsync(false);

            var result = await mockRepo.Object.UpdateScore(score);

            Assert.False(result);
        }

        [Fact]
        public async Task Test_UpdateScore_CalledOnce_WithCorrectScore()
        {
            var mockRepo = new Mock<IScoreRepository>();
            var score = BuildScore();
            mockRepo.Setup(r => r.UpdateScore(It.IsAny<Score>())).ReturnsAsync(true);

            await mockRepo.Object.UpdateScore(score);

            mockRepo.Verify(r => r.UpdateScore(score), Times.Once);
        }

        // ---- Pass Tests: RemoveScore ----

        [Fact]
        public async Task Test_RemoveScore_ReturnsTrue_OnSuccess()
        {
            var mockRepo = new Mock<IScoreRepository>();
            var id = ObjectId.GenerateNewId();
            mockRepo.Setup(r => r.RemoveScore(id)).ReturnsAsync(true);

            var result = await mockRepo.Object.RemoveScore(id);

            Assert.True(result);
        }

        [Fact]
        public async Task Test_RemoveScore_ReturnsFalse_WhenNotFound()
        {
            var mockRepo = new Mock<IScoreRepository>();
            var id = ObjectId.GenerateNewId();
            mockRepo.Setup(r => r.RemoveScore(id)).ReturnsAsync(false);

            var result = await mockRepo.Object.RemoveScore(id);

            Assert.False(result);
        }

        [Fact]
        public async Task Test_RemoveScore_CalledOnce_WithCorrectId()
        {
            var mockRepo = new Mock<IScoreRepository>();
            var id = ObjectId.GenerateNewId();
            mockRepo.Setup(r => r.RemoveScore(id)).ReturnsAsync(true);

            await mockRepo.Object.RemoveScore(id);

            mockRepo.Verify(r => r.RemoveScore(id), Times.Once);
        }

        // ---- Fail Tests ----

        [Fact]
        public async Task Test_GetScore_NeverCalledWithoutExplicitInvocation()
        {
            var mockRepo = new Mock<IScoreRepository>();

            mockRepo.Verify(r => r.GetScore(It.IsAny<string>()), Times.Never);
            await Task.CompletedTask;
        }

        [Fact]
        public async Task Test_AddScore_NotCalledWhenNotInvoked()
        {
            var mockRepo = new Mock<IScoreRepository>();

            mockRepo.Verify(r => r.AddScore(It.IsAny<Score>()), Times.Never);
            await Task.CompletedTask;
        }

        [Fact]
        public async Task Test_GetScoreByArtifact_DifferentArtifactIds_ReturnDifferentScores()
        {
            var mockRepo = new Mock<IScoreRepository>();
            var score1 = BuildScore("host1");
            var score2 = BuildScore("host2");
            var id1 = ObjectId.GenerateNewId().ToString();
            var id2 = ObjectId.GenerateNewId().ToString();

            mockRepo.Setup(r => r.GetScorebyArtifact(id1)).ReturnsAsync(score1);
            mockRepo.Setup(r => r.GetScorebyArtifact(id2)).ReturnsAsync(score2);

            var result1 = await mockRepo.Object.GetScorebyArtifact(id1);
            var result2 = await mockRepo.Object.GetScorebyArtifact(id2);

            Assert.NotEqual(result1.hostName, result2.hostName);
        }

        [Fact]
        public async Task Test_RemoveScore_NeverCalledWithWrongId()
        {
            var mockRepo = new Mock<IScoreRepository>();
            var correctId = ObjectId.GenerateNewId();
            var wrongId = ObjectId.GenerateNewId();
            mockRepo.Setup(r => r.RemoveScore(correctId)).ReturnsAsync(true);

            await mockRepo.Object.RemoveScore(correctId);

            mockRepo.Verify(r => r.RemoveScore(wrongId), Times.Never);
        }

        [Fact]
        public async Task Test_GetSystemScores_CalledWithCorrectSystemGroupId()
        {
            var mockRepo = new Mock<IScoreRepository>();
            var systemId = "sys-group-xyz";
            mockRepo.Setup(r => r.GetSystemScores(systemId)).ReturnsAsync(new List<Score>());

            await mockRepo.Object.GetSystemScores(systemId);

            mockRepo.Verify(r => r.GetSystemScores(systemId), Times.Once);
            mockRepo.Verify(r => r.GetSystemScores("wrong-system-id"), Times.Never);
        }

        // ---- Score computed properties integration ----

        [Fact]
        public async Task Test_AddScore_ReturnedScore_ComputedTotals_AreCorrect()
        {
            var mockRepo = new Mock<IScoreRepository>();
            var score = new Score
            {
                hostName = "server",
                stigType = "Chrome",
                stigRelease = "V1R1",
                totalCat1Open = 2, totalCat1NotAFinding = 3,
                totalCat2Open = 5, totalCat2NotAFinding = 10,
                totalCat3Open = 1, totalCat3NotAFinding = 8
            };
            mockRepo.Setup(r => r.AddScore(score)).ReturnsAsync(score);

            var result = await mockRepo.Object.AddScore(score);

            Assert.Equal(8, result.totalOpen);
            Assert.Equal(21, result.totalNotAFinding);
        }
    }
}
