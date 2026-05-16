using Xunit;
using openrmf_msg_score.Models;

namespace tests.Models
{
    public class STIGSTests
    {
        // ---- Pass Tests ----

        [Fact]
        public void Test_NewStigs_IsNotNull()
        {
            var stigs = new STIGS();
            Assert.NotNull(stigs);
        }

        [Fact]
        public void Test_NewStigs_HasDefaultISTIG()
        {
            var stigs = new STIGS();
            Assert.NotNull(stigs.iSTIG);
        }

        [Fact]
        public void Test_NewStigs_ISTIG_HasDefaultStigInfo()
        {
            var stigs = new STIGS();
            Assert.NotNull(stigs.iSTIG.STIG_INFO);
        }

        [Fact]
        public void Test_NewStigs_ISTIG_HasEmptyVulnList()
        {
            var stigs = new STIGS();
            Assert.Empty(stigs.iSTIG.VULN);
        }

        [Fact]
        public void Test_Stigs_ISTIG_CanBeAssigned()
        {
            var istig = new iSTIG();
            istig.VULN.Add(new VULN { STATUS = "Open" });
            var stigs = new STIGS { iSTIG = istig };
            Assert.NotNull(stigs.iSTIG);
            Assert.Single(stigs.iSTIG.VULN);
        }

        [Fact]
        public void Test_Stigs_VulnsAddedViaISTIG_Pass()
        {
            var stigs = new STIGS();
            stigs.iSTIG.VULN.Add(new VULN { STATUS = "Open" });
            stigs.iSTIG.VULN.Add(new VULN { STATUS = "NotAFinding" });
            stigs.iSTIG.VULN.Add(new VULN { STATUS = "Not_Reviewed" });
            Assert.Equal(3, stigs.iSTIG.VULN.Count);
        }

        // ---- Fail Tests ----

        [Fact]
        public void Test_Stigs_DefaultISTIG_VulnCountIsZero()
        {
            var stigs = new STIGS();
            Assert.Empty(stigs.iSTIG.VULN);
        }

        [Fact]
        public void Test_Stigs_DefaultStigInfoSIData_IsEmpty()
        {
            var stigs = new STIGS();
            Assert.Empty(stigs.iSTIG.STIG_INFO.SI_DATA);
        }
    }
}
