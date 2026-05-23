using Xunit;
using openrmf_msg_score.Models;

namespace tests.Models
{
    public class ASSETTests
    {
        // ---- Pass Tests ----

        [Fact]
        public void Test_NewAsset_IsNotNull()
        {
            var asset = new ASSET();
            Assert.NotNull(asset);
        }

        [Fact]
        public void Test_Asset_Role_IsSetCorrectly()
        {
            var asset = new ASSET { ROLE = "Member Server" };
            Assert.Equal("Member Server", asset.ROLE);
        }

        [Fact]
        public void Test_Asset_AssetType_IsSetCorrectly()
        {
            var asset = new ASSET { ASSET_TYPE = "Computing" };
            Assert.Equal("Computing", asset.ASSET_TYPE);
        }

        [Fact]
        public void Test_Asset_Marking_IsSetCorrectly()
        {
            var asset = new ASSET { MARKING = "CUI" };
            Assert.Equal("CUI", asset.MARKING);
        }

        [Fact]
        public void Test_Asset_HostName_IsSetCorrectly()
        {
            var asset = new ASSET { HOST_NAME = "server01" };
            Assert.Equal("server01", asset.HOST_NAME);
        }

        [Fact]
        public void Test_Asset_HostIp_IsSetCorrectly()
        {
            var asset = new ASSET { HOST_IP = "192.168.1.100" };
            Assert.Equal("192.168.1.100", asset.HOST_IP);
        }

        [Fact]
        public void Test_Asset_HostMac_IsSetCorrectly()
        {
            var asset = new ASSET { HOST_MAC = "AA:BB:CC:DD:EE:FF" };
            Assert.Equal("AA:BB:CC:DD:EE:FF", asset.HOST_MAC);
        }

        [Fact]
        public void Test_Asset_HostFqdn_IsSetCorrectly()
        {
            var asset = new ASSET { HOST_FQDN = "server01.corp.example.com" };
            Assert.Equal("server01.corp.example.com", asset.HOST_FQDN);
        }

        [Fact]
        public void Test_Asset_TechArea_IsSetCorrectly()
        {
            var asset = new ASSET { TECH_AREA = "Application Review" };
            Assert.Equal("Application Review", asset.TECH_AREA);
        }

        [Fact]
        public void Test_Asset_TargetKey_IsSetCorrectly()
        {
            var asset = new ASSET { TARGET_KEY = "2350" };
            Assert.Equal("2350", asset.TARGET_KEY);
        }

        [Fact]
        public void Test_Asset_WebOrDatabase_IsSetCorrectly()
        {
            var asset = new ASSET { WEB_OR_DATABASE = "true" };
            Assert.Equal("true", asset.WEB_OR_DATABASE);
        }

        [Fact]
        public void Test_Asset_WebDbSite_IsSetCorrectly()
        {
            var asset = new ASSET { WEB_DB_SITE = "https://myapp.example.com" };
            Assert.Equal("https://myapp.example.com", asset.WEB_DB_SITE);
        }

        [Fact]
        public void Test_Asset_WebDbInstance_IsSetCorrectly()
        {
            var asset = new ASSET { WEB_DB_INSTANCE = "instance01" };
            Assert.Equal("instance01", asset.WEB_DB_INSTANCE);
        }

        [Fact]
        public void Test_Asset_WithAllFields_Pass()
        {
            var asset = new ASSET
            {
                ROLE = "Member Server",
                ASSET_TYPE = "Computing",
                MARKING = "CUI",
                HOST_NAME = "server01",
                HOST_IP = "10.0.0.1",
                HOST_MAC = "AA:BB:CC:DD:EE:FF",
                HOST_FQDN = "server01.corp.local",
                TECH_AREA = "Application Review",
                TARGET_KEY = "2350",
                WEB_OR_DATABASE = "false",
                WEB_DB_SITE = string.Empty,
                WEB_DB_INSTANCE = string.Empty
            };

            Assert.NotNull(asset);
            Assert.NotEmpty(asset.ROLE);
            Assert.NotEmpty(asset.ASSET_TYPE);
            Assert.NotEmpty(asset.HOST_NAME);
            Assert.NotEmpty(asset.HOST_IP);
            Assert.NotEmpty(asset.HOST_MAC);
            Assert.NotEmpty(asset.HOST_FQDN);
            Assert.NotEmpty(asset.TECH_AREA);
            Assert.NotEmpty(asset.TARGET_KEY);
            Assert.NotEmpty(asset.WEB_OR_DATABASE);
        }

        // ---- Fail Tests ----

        [Fact]
        public void Test_Asset_HostName_NullByDefault()
        {
            var asset = new ASSET();
            Assert.Null(asset.HOST_NAME);
        }

        [Fact]
        public void Test_Asset_Role_NullByDefault()
        {
            var asset = new ASSET();
            Assert.Null(asset.ROLE);
        }

        [Fact]
        public void Test_Asset_HostIp_NullByDefault()
        {
            var asset = new ASSET();
            Assert.Null(asset.HOST_IP);
        }

        [Theory]
        [InlineData("server01", "10.0.0.1", "AA:BB:CC:00:11:22")]
        [InlineData("webserver", "172.16.0.5", "FF:EE:DD:CC:BB:AA")]
        public void Test_Asset_NetworkFields_SetAndVerified(string hostname, string ip, string mac)
        {
            var asset = new ASSET { HOST_NAME = hostname, HOST_IP = ip, HOST_MAC = mac };
            Assert.Equal(hostname, asset.HOST_NAME);
            Assert.Equal(ip, asset.HOST_IP);
            Assert.Equal(mac, asset.HOST_MAC);
        }
    }
}
