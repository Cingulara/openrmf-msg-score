using Xunit;
using openrmf_msg_score.Models;

namespace tests.Models
{
    public class STIG_INFOTests
    {
        // ---- Pass Tests ----

        [Fact]
        public void Test_NewStigInfo_IsNotNull()
        {
            var info = new STIG_INFO();
            Assert.NotNull(info);
        }

        [Fact]
        public void Test_NewStigInfo_HasDefaultEmptySIDataList()
        {
            var info = new STIG_INFO();
            Assert.NotNull(info.SI_DATA);
            Assert.Empty(info.SI_DATA);
        }

        [Fact]
        public void Test_StigInfo_AddSIData_CountIncreases()
        {
            var info = new STIG_INFO();
            info.SI_DATA.Add(new SI_DATA { SID_NAME = "stigid", SID_DATA = "Chrome" });
            info.SI_DATA.Add(new SI_DATA { SID_NAME = "title", SID_DATA = "Chrome STIG" });
            Assert.Equal(2, info.SI_DATA.Count);
        }

        [Fact]
        public void Test_StigInfo_SIDataEntry_CanBeRetrievedByIndex()
        {
            var info = new STIG_INFO();
            info.SI_DATA.Add(new SI_DATA { SID_NAME = "version", SID_DATA = "1" });
            Assert.Equal("version", info.SI_DATA[0].SID_NAME);
            Assert.Equal("1", info.SI_DATA[0].SID_DATA);
        }

        [Fact]
        public void Test_StigInfo_MultipleSIData_Pass()
        {
            var info = new STIG_INFO();
            info.SI_DATA.Add(new SI_DATA { SID_NAME = "stigid", SID_DATA = "Google_Chrome_Current_Windows" });
            info.SI_DATA.Add(new SI_DATA { SID_NAME = "title", SID_DATA = "Google Chrome Current Windows STIG" });
            info.SI_DATA.Add(new SI_DATA { SID_NAME = "version", SID_DATA = "1" });
            info.SI_DATA.Add(new SI_DATA { SID_NAME = "releaseinfo", SID_DATA = "Release: 30 Benchmark Date: 25 Jul 2024" });

            Assert.Equal(4, info.SI_DATA.Count);
            Assert.Equal("stigid", info.SI_DATA[0].SID_NAME);
            Assert.Equal("releaseinfo", info.SI_DATA[3].SID_NAME);
        }

        // ---- Fail Tests ----

        [Fact]
        public void Test_StigInfo_SIDataCount_ZeroByDefault()
        {
            var info = new STIG_INFO();
            Assert.Empty(info.SI_DATA);
        }

        [Fact]
        public void Test_StigInfo_SIData_AfterAddOneEntryCountIsNotZero()
        {
            var info = new STIG_INFO();
            info.SI_DATA.Add(new SI_DATA { SID_NAME = "stigid", SID_DATA = "Chrome" });
            Assert.NotEmpty(info.SI_DATA);
        }
    }
}
