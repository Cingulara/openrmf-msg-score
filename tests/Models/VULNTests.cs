using Xunit;
using openrmf_msg_score.Models;

namespace tests.Models
{
    public class VULNTests
    {
        // ---- Pass Tests ----

        [Fact]
        public void Test_NewVuln_IsNotNull()
        {
            var v = new VULN();
            Assert.NotNull(v);
        }

        [Fact]
        public void Test_NewVuln_HasDefaultEmptyStigDataList()
        {
            var v = new VULN();
            Assert.NotNull(v.STIG_DATA);
            Assert.Empty(v.STIG_DATA);
        }

        [Fact]
        public void Test_Vuln_Status_IsSetCorrectly()
        {
            var v = new VULN { STATUS = "Open" };
            Assert.Equal("Open", v.STATUS);
        }

        [Fact]
        public void Test_Vuln_FindingDetails_IsSetCorrectly()
        {
            var v = new VULN { FINDING_DETAILS = "This is an open finding." };
            Assert.Equal("This is an open finding.", v.FINDING_DETAILS);
        }

        [Fact]
        public void Test_Vuln_Comments_IsSetCorrectly()
        {
            var v = new VULN { COMMENTS = "Reviewed by admin" };
            Assert.Equal("Reviewed by admin", v.COMMENTS);
        }

        [Fact]
        public void Test_Vuln_SeverityOverride_IsSetCorrectly()
        {
            var v = new VULN { SEVERITY_OVERRIDE = "medium" };
            Assert.Equal("medium", v.SEVERITY_OVERRIDE);
        }

        [Fact]
        public void Test_Vuln_SeverityJustification_IsSetCorrectly()
        {
            var v = new VULN { SEVERITY_JUSTIFICATION = "Risk accepted by ISSO" };
            Assert.Equal("Risk accepted by ISSO", v.SEVERITY_JUSTIFICATION);
        }

        [Fact]
        public void Test_Vuln_AddStigData_CountIncreases()
        {
            var v = new VULN();
            v.STIG_DATA.Add(new STIG_DATA { VULN_ATTRIBUTE = "Severity", ATTRIBUTE_DATA = "high" });
            v.STIG_DATA.Add(new STIG_DATA { VULN_ATTRIBUTE = "Vuln_Num", ATTRIBUTE_DATA = "V-221558" });
            Assert.Equal(2, v.STIG_DATA.Count);
        }

        [Fact]
        public void Test_Vuln_WithAllFields_Pass()
        {
            var v = new VULN
            {
                STATUS = "Open",
                FINDING_DETAILS = "High severity finding.",
                COMMENTS = "No mitigations.",
                SEVERITY_OVERRIDE = string.Empty,
                SEVERITY_JUSTIFICATION = string.Empty
            };
            v.STIG_DATA.Add(new STIG_DATA { VULN_ATTRIBUTE = "Severity", ATTRIBUTE_DATA = "high" });

            Assert.NotNull(v);
            Assert.Equal("Open", v.STATUS);
            Assert.NotEmpty(v.FINDING_DETAILS);
            Assert.Single(v.STIG_DATA);
            Assert.Equal("high", v.STIG_DATA[0].ATTRIBUTE_DATA);
        }

        [Theory]
        [InlineData("Open")]
        [InlineData("NotAFinding")]
        [InlineData("Not_Reviewed")]
        [InlineData("Not_Applicable")]
        public void Test_Vuln_ValidStatusValues_Pass(string status)
        {
            var v = new VULN { STATUS = status };
            Assert.Equal(status, v.STATUS);
        }

        // ---- Fail Tests ----

        [Fact]
        public void Test_Vuln_Status_NullByDefault()
        {
            var v = new VULN();
            Assert.Null(v.STATUS);
        }

        [Fact]
        public void Test_Vuln_StigDataCount_ZeroByDefault()
        {
            var v = new VULN();
            Assert.Empty(v.STIG_DATA);
        }

        [Fact]
        public void Test_Vuln_SeverityOverride_EmptyStringIsNullOrEmpty()
        {
            var v = new VULN { SEVERITY_OVERRIDE = string.Empty };
            Assert.True(string.IsNullOrEmpty(v.SEVERITY_OVERRIDE));
        }

        [Fact]
        public void Test_Vuln_Status_OpenNotEqualToNotAFinding()
        {
            var v = new VULN { STATUS = "Open" };
            Assert.NotEqual("NotAFinding", v.STATUS);
        }
    }
}
