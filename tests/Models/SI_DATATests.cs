using Xunit;
using openrmf_msg_score.Models;

namespace tests.Models
{
    public class SI_DATATests
    {
        // ---- Pass Tests ----

        [Fact]
        public void Test_NewSIData_IsNotNull()
        {
            var data = new SI_DATA();
            Assert.NotNull(data);
        }

        [Fact]
        public void Test_SIData_SidName_IsSetCorrectly()
        {
            var data = new SI_DATA { SID_NAME = "stigid" };
            Assert.Equal("stigid", data.SID_NAME);
        }

        [Fact]
        public void Test_SIData_SidData_IsSetCorrectly()
        {
            var data = new SI_DATA { SID_DATA = "Google_Chrome_Current_Windows" };
            Assert.Equal("Google_Chrome_Current_Windows", data.SID_DATA);
        }

        [Fact]
        public void Test_SIData_BothFields_SetAndVerified()
        {
            var data = new SI_DATA
            {
                SID_NAME = "title",
                SID_DATA = "Google Chrome Current Windows STIG"
            };
            Assert.NotEmpty(data.SID_NAME);
            Assert.NotEmpty(data.SID_DATA);
            Assert.Equal("title", data.SID_NAME);
            Assert.Equal("Google Chrome Current Windows STIG", data.SID_DATA);
        }

        [Theory]
        [InlineData("stigid", "Google_Chrome_Current_Windows")]
        [InlineData("title", "Google Chrome Current Windows STIG")]
        [InlineData("version", "1")]
        [InlineData("releaseinfo", "Release: 30 Benchmark Date: 25 Jul 2024")]
        public void Test_SIData_CommonStigInfoFields_Pass(string name, string value)
        {
            var data = new SI_DATA { SID_NAME = name, SID_DATA = value };
            Assert.Equal(name, data.SID_NAME);
            Assert.Equal(value, data.SID_DATA);
        }

        // ---- Fail Tests ----

        [Fact]
        public void Test_SIData_SidName_NullByDefault()
        {
            var data = new SI_DATA();
            Assert.Null(data.SID_NAME);
        }

        [Fact]
        public void Test_SIData_SidData_NullByDefault()
        {
            var data = new SI_DATA();
            Assert.Null(data.SID_DATA);
        }

        [Fact]
        public void Test_SIData_SidName_NotEqualToSidData()
        {
            var data = new SI_DATA { SID_NAME = "stigid", SID_DATA = "Chrome_Windows" };
            Assert.NotEqual(data.SID_NAME, data.SID_DATA);
        }
    }
}
