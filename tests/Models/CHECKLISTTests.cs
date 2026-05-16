using Xunit;
using openrmf_msg_score.Models;

namespace tests.Models
{
    public class CHECKLISTTests
    {
        // ---- Pass Tests ----

        [Fact]
        public void Test_NewChecklist_IsNotNull()
        {
            var chk = new CHECKLIST();
            Assert.NotNull(chk);
        }

        [Fact]
        public void Test_NewChecklist_HasDefaultAsset()
        {
            var chk = new CHECKLIST();
            Assert.NotNull(chk.ASSET);
        }

        [Fact]
        public void Test_NewChecklist_HasDefaultStigs()
        {
            var chk = new CHECKLIST();
            Assert.NotNull(chk.STIGS);
        }

        [Fact]
        public void Test_NewChecklist_DefaultStigs_HasISTIG()
        {
            var chk = new CHECKLIST();
            Assert.NotNull(chk.STIGS.iSTIG);
        }

        [Fact]
        public void Test_Checklist_AssetCanBeAssigned()
        {
            var asset = new ASSET { HOST_NAME = "myServer", HOST_IP = "10.0.0.5" };
            var chk = new CHECKLIST { ASSET = asset };
            Assert.Equal("myServer", chk.ASSET.HOST_NAME);
            Assert.Equal("10.0.0.5", chk.ASSET.HOST_IP);
        }

        [Fact]
        public void Test_Checklist_StigsCanBeAssigned()
        {
            var stigs = new STIGS();
            var chk = new CHECKLIST { STIGS = stigs };
            Assert.NotNull(chk.STIGS);
        }

        [Fact]
        public void Test_Checklist_FullyPopulated_Pass()
        {
            var chk = new CHECKLIST();
            chk.ASSET.HOST_NAME = "prodServer";
            chk.ASSET.HOST_IP = "192.168.10.1";
            chk.ASSET.ROLE = "Member Server";
            chk.ASSET.ASSET_TYPE = "Computing";
            chk.STIGS.iSTIG.VULN.Add(new VULN { STATUS = "Open" });

            Assert.NotNull(chk.ASSET);
            Assert.Equal("prodServer", chk.ASSET.HOST_NAME);
            Assert.NotNull(chk.STIGS);
            Assert.NotNull(chk.STIGS.iSTIG);
            Assert.Single(chk.STIGS.iSTIG.VULN);
        }

        // ---- Fail Tests ----

        [Fact]
        public void Test_Checklist_AssetHostName_NullByDefault()
        {
            var chk = new CHECKLIST();
            Assert.Null(chk.ASSET.HOST_NAME);
        }

        [Fact]
        public void Test_Checklist_DefaultVulnList_IsEmpty()
        {
            var chk = new CHECKLIST();
            Assert.Empty(chk.STIGS.iSTIG.VULN);
        }

        [Fact]
        public void Test_Checklist_DefaultStigInfoSIData_IsEmpty()
        {
            var chk = new CHECKLIST();
            Assert.Empty(chk.STIGS.iSTIG.STIG_INFO.SI_DATA);
        }
    }
}
