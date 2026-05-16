using Xunit;
using openrmf_msg_score.Models;

namespace tests.Models
{
    public class STIG_DATATests
    {
        // ---- Pass Tests ----

        [Fact]
        public void Test_NewStigData_IsNotNull()
        {
            var data = new STIG_DATA();
            Assert.NotNull(data);
        }

        [Fact]
        public void Test_StigData_VulnAttribute_IsSetCorrectly()
        {
            var data = new STIG_DATA { VULN_ATTRIBUTE = "Severity" };
            Assert.Equal("Severity", data.VULN_ATTRIBUTE);
        }

        [Fact]
        public void Test_StigData_AttributeData_IsSetCorrectly()
        {
            var data = new STIG_DATA { ATTRIBUTE_DATA = "high" };
            Assert.Equal("high", data.ATTRIBUTE_DATA);
        }

        [Fact]
        public void Test_StigData_BothFields_SetAndVerified()
        {
            var data = new STIG_DATA
            {
                VULN_ATTRIBUTE = "Severity",
                ATTRIBUTE_DATA = "medium"
            };
            Assert.NotEmpty(data.VULN_ATTRIBUTE);
            Assert.NotEmpty(data.ATTRIBUTE_DATA);
        }

        [Theory]
        [InlineData("Severity", "high")]
        [InlineData("Severity", "medium")]
        [InlineData("Severity", "low")]
        [InlineData("Vuln_Num", "V-221558")]
        [InlineData("Group_Title", "SRG-APP-000001")]
        public void Test_StigData_CommonVulnAttributes_Pass(string attribute, string data)
        {
            var stigData = new STIG_DATA { VULN_ATTRIBUTE = attribute, ATTRIBUTE_DATA = data };
            Assert.Equal(attribute, stigData.VULN_ATTRIBUTE);
            Assert.Equal(data, stigData.ATTRIBUTE_DATA);
        }

        // ---- Fail Tests ----

        [Fact]
        public void Test_StigData_VulnAttribute_NullByDefault()
        {
            var data = new STIG_DATA();
            Assert.Null(data.VULN_ATTRIBUTE);
        }

        [Fact]
        public void Test_StigData_AttributeData_NullByDefault()
        {
            var data = new STIG_DATA();
            Assert.Null(data.ATTRIBUTE_DATA);
        }

        [Fact]
        public void Test_StigData_AttributeData_NotEqualToVulnAttribute()
        {
            var data = new STIG_DATA { VULN_ATTRIBUTE = "Severity", ATTRIBUTE_DATA = "high" };
            Assert.NotEqual(data.VULN_ATTRIBUTE, data.ATTRIBUTE_DATA);
        }

        [Fact]
        public void Test_StigData_AttributeData_HighNotMedium()
        {
            var data = new STIG_DATA { VULN_ATTRIBUTE = "Severity", ATTRIBUTE_DATA = "high" };
            Assert.NotEqual("medium", data.ATTRIBUTE_DATA);
        }
    }
}
