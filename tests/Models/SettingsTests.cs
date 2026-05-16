using Xunit;
using openrmf_msg_score.Models;

namespace tests.Models
{
    public class SettingsTests
    {
        // ---- Pass Tests ----

        [Fact]
        public void Test_NewSettings_IsNotNull()
        {
            var settings = new Settings();
            Assert.NotNull(settings);
        }

        [Fact]
        public void Test_Settings_ConnectionString_IsSetCorrectly()
        {
            var settings = new Settings { ConnectionString = "mongodb://localhost:27017" };
            Assert.Equal("mongodb://localhost:27017", settings.ConnectionString);
        }

        [Fact]
        public void Test_Settings_Database_IsSetCorrectly()
        {
            var settings = new Settings { Database = "openrmf" };
            Assert.Equal("openrmf", settings.Database);
        }

        [Fact]
        public void Test_Settings_BothFields_SetAndVerified()
        {
            var settings = new Settings
            {
                ConnectionString = "mongodb://mongo:27017",
                Database = "openrmfScore"
            };
            Assert.NotEmpty(settings.ConnectionString);
            Assert.NotEmpty(settings.Database);
            Assert.Equal("mongodb://mongo:27017", settings.ConnectionString);
            Assert.Equal("openrmfScore", settings.Database);
        }

        [Theory]
        [InlineData("mongodb://localhost:27017", "openrmf")]
        [InlineData("mongodb://mongo-server:27017", "openrmfScore")]
        [InlineData("mongodb://user:pass@host:27017", "prodDB")]
        public void Test_Settings_VariousConnectionStrings_Pass(string connStr, string db)
        {
            var settings = new Settings { ConnectionString = connStr, Database = db };
            Assert.Equal(connStr, settings.ConnectionString);
            Assert.Equal(db, settings.Database);
        }

        // ---- Fail Tests ----

        [Fact]
        public void Test_Settings_ConnectionString_NullByDefault()
        {
            var settings = new Settings();
            Assert.Null(settings.ConnectionString);
        }

        [Fact]
        public void Test_Settings_Database_NullByDefault()
        {
            var settings = new Settings();
            Assert.Null(settings.Database);
        }

        [Fact]
        public void Test_Settings_ConnectionString_NotEqualToDatabase()
        {
            var settings = new Settings
            {
                ConnectionString = "mongodb://localhost:27017",
                Database = "openrmf"
            };
            Assert.NotEqual(settings.ConnectionString, settings.Database);
        }
    }
}
