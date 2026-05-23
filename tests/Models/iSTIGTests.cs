using Xunit;
using openrmf_msg_score.Models;
using System.Collections.Generic;

namespace tests.Models
{
    public class iSTIGTests
    {
        // ---- Pass Tests ----

        [Fact]
        public void Test_NewISTIG_IsNotNull()
        {
            var istig = new iSTIG();
            Assert.NotNull(istig);
        }

        [Fact]
        public void Test_NewISTIG_HasDefaultStigInfo()
        {
            var istig = new iSTIG();
            Assert.NotNull(istig.STIG_INFO);
        }

        [Fact]
        public void Test_NewISTIG_HasDefaultEmptyVulnList()
        {
            var istig = new iSTIG();
            Assert.NotNull(istig.VULN);
            Assert.Empty(istig.VULN);
        }

        [Fact]
        public void Test_ISTIG_AddVuln_CountIncreases()
        {
            var istig = new iSTIG();
            istig.VULN.Add(new VULN { STATUS = "Open" });
            istig.VULN.Add(new VULN { STATUS = "NotAFinding" });
            Assert.Equal(2, istig.VULN.Count);
        }

        [Fact]
        public void Test_ISTIG_StigInfo_SIDataCanBeAdded()
        {
            var istig = new iSTIG();
            istig.STIG_INFO.SI_DATA.Add(new SI_DATA { SID_NAME = "stigid", SID_DATA = "Chrome_Windows" });
            Assert.Single(istig.STIG_INFO.SI_DATA);
            Assert.Equal("stigid", istig.STIG_INFO.SI_DATA[0].SID_NAME);
        }

        [Fact]
        public void Test_ISTIG_VulnList_CanBeReplaced()
        {
            var istig = new iSTIG();
            istig.VULN = new List<VULN>
            {
                new VULN { STATUS = "Open" },
                new VULN { STATUS = "Not_Reviewed" },
                new VULN { STATUS = "Not_Applicable" }
            };
            Assert.Equal(3, istig.VULN.Count);
        }

        // ---- Fail Tests ----

        [Fact]
        public void Test_ISTIG_DefaultStigInfoSIData_IsEmpty()
        {
            var istig = new iSTIG();
            Assert.Empty(istig.STIG_INFO.SI_DATA);
        }

        [Fact]
        public void Test_ISTIG_VulnStatus_NotMatchingExpected_Fail()
        {
            var istig = new iSTIG();
            istig.VULN.Add(new VULN { STATUS = "Open" });
            Assert.NotEqual("NotAFinding", istig.VULN[0].STATUS);
        }
    }
}
