using Xunit;
using openrmf_msg_score.Classes;
using openrmf_msg_score.Models;
using System;
using System.Collections.Generic;

namespace tests.Classes
{
    public class ChecklistLoaderTests
    {
        // Minimal valid CKL XML for testing
        private const string ValidCklXml = @"<CHECKLIST>
    <ASSET>
        <ROLE>Member Server</ROLE>
        <ASSET_TYPE>Computing</ASSET_TYPE>
        <MARKING>CUI</MARKING>
        <HOST_NAME>TestHost</HOST_NAME>
        <HOST_IP>192.168.1.100</HOST_IP>
        <HOST_MAC>AA:BB:CC:DD:EE:FF</HOST_MAC>
        <HOST_FQDN>testhost.corp.local</HOST_FQDN>
        <TECH_AREA></TECH_AREA>
        <TARGET_KEY>2350</TARGET_KEY>
        <WEB_OR_DATABASE>false</WEB_OR_DATABASE>
        <WEB_DB_SITE></WEB_DB_SITE>
        <WEB_DB_INSTANCE></WEB_DB_INSTANCE>
    </ASSET>
    <STIGS>
        <iSTIG>
            <STIG_INFO>
                <SI_DATA>
                    <SID_NAME>stigid</SID_NAME>
                    <SID_DATA>Google_Chrome_Current_Windows</SID_DATA>
                </SI_DATA>
                <SI_DATA>
                    <SID_NAME>title</SID_NAME>
                    <SID_DATA>Google Chrome Current Windows STIG</SID_DATA>
                </SI_DATA>
            </STIG_INFO>
            <VULN>
                <STIG_DATA>
                    <VULN_ATTRIBUTE>Severity</VULN_ATTRIBUTE>
                    <ATTRIBUTE_DATA>high</ATTRIBUTE_DATA>
                </STIG_DATA>
                <STATUS>Open</STATUS>
                <FINDING_DETAILS>High severity open finding</FINDING_DETAILS>
                <COMMENTS></COMMENTS>
                <SEVERITY_OVERRIDE></SEVERITY_OVERRIDE>
                <SEVERITY_JUSTIFICATION></SEVERITY_JUSTIFICATION>
            </VULN>
            <VULN>
                <STIG_DATA>
                    <VULN_ATTRIBUTE>Severity</VULN_ATTRIBUTE>
                    <ATTRIBUTE_DATA>medium</ATTRIBUTE_DATA>
                </STIG_DATA>
                <STATUS>NotAFinding</STATUS>
                <FINDING_DETAILS></FINDING_DETAILS>
                <COMMENTS></COMMENTS>
                <SEVERITY_OVERRIDE></SEVERITY_OVERRIDE>
                <SEVERITY_JUSTIFICATION></SEVERITY_JUSTIFICATION>
            </VULN>
            <VULN>
                <STIG_DATA>
                    <VULN_ATTRIBUTE>Severity</VULN_ATTRIBUTE>
                    <ATTRIBUTE_DATA>low</ATTRIBUTE_DATA>
                </STIG_DATA>
                <STATUS>Not_Reviewed</STATUS>
                <FINDING_DETAILS></FINDING_DETAILS>
                <COMMENTS></COMMENTS>
                <SEVERITY_OVERRIDE></SEVERITY_OVERRIDE>
                <SEVERITY_JUSTIFICATION></SEVERITY_JUSTIFICATION>
            </VULN>
        </iSTIG>
    </STIGS>
</CHECKLIST>";

        private const string MultiVulnCklXml = @"<CHECKLIST>
    <ASSET>
        <ROLE>Member Server</ROLE>
        <ASSET_TYPE>Computing</ASSET_TYPE>
        <MARKING></MARKING>
        <HOST_NAME>ScoreServer</HOST_NAME>
        <HOST_IP>10.0.0.5</HOST_IP>
        <HOST_MAC>11:22:33:44:55:66</HOST_MAC>
        <HOST_FQDN>scoreserver.corp.local</HOST_FQDN>
        <TECH_AREA></TECH_AREA>
        <TARGET_KEY>1234</TARGET_KEY>
        <WEB_OR_DATABASE>false</WEB_OR_DATABASE>
        <WEB_DB_SITE></WEB_DB_SITE>
        <WEB_DB_INSTANCE></WEB_DB_INSTANCE>
    </ASSET>
    <STIGS>
        <iSTIG>
            <STIG_INFO>
                <SI_DATA>
                    <SID_NAME>stigid</SID_NAME>
                    <SID_DATA>Google_Chrome_Current_Windows</SID_DATA>
                </SI_DATA>
            </STIG_INFO>
            <VULN>
                <STIG_DATA><VULN_ATTRIBUTE>Severity</VULN_ATTRIBUTE><ATTRIBUTE_DATA>high</ATTRIBUTE_DATA></STIG_DATA>
                <STATUS>Open</STATUS>
                <FINDING_DETAILS>CAT1 Open</FINDING_DETAILS>
                <COMMENTS></COMMENTS>
                <SEVERITY_OVERRIDE></SEVERITY_OVERRIDE>
                <SEVERITY_JUSTIFICATION></SEVERITY_JUSTIFICATION>
            </VULN>
            <VULN>
                <STIG_DATA><VULN_ATTRIBUTE>Severity</VULN_ATTRIBUTE><ATTRIBUTE_DATA>high</ATTRIBUTE_DATA></STIG_DATA>
                <STATUS>NotAFinding</STATUS>
                <FINDING_DETAILS></FINDING_DETAILS>
                <COMMENTS></COMMENTS>
                <SEVERITY_OVERRIDE></SEVERITY_OVERRIDE>
                <SEVERITY_JUSTIFICATION></SEVERITY_JUSTIFICATION>
            </VULN>
            <VULN>
                <STIG_DATA><VULN_ATTRIBUTE>Severity</VULN_ATTRIBUTE><ATTRIBUTE_DATA>medium</ATTRIBUTE_DATA></STIG_DATA>
                <STATUS>Not_Reviewed</STATUS>
                <FINDING_DETAILS></FINDING_DETAILS>
                <COMMENTS></COMMENTS>
                <SEVERITY_OVERRIDE></SEVERITY_OVERRIDE>
                <SEVERITY_JUSTIFICATION></SEVERITY_JUSTIFICATION>
            </VULN>
            <VULN>
                <STIG_DATA><VULN_ATTRIBUTE>Severity</VULN_ATTRIBUTE><ATTRIBUTE_DATA>medium</ATTRIBUTE_DATA></STIG_DATA>
                <STATUS>Not_Applicable</STATUS>
                <FINDING_DETAILS></FINDING_DETAILS>
                <COMMENTS></COMMENTS>
                <SEVERITY_OVERRIDE></SEVERITY_OVERRIDE>
                <SEVERITY_JUSTIFICATION></SEVERITY_JUSTIFICATION>
            </VULN>
            <VULN>
                <STIG_DATA><VULN_ATTRIBUTE>Severity</VULN_ATTRIBUTE><ATTRIBUTE_DATA>low</ATTRIBUTE_DATA></STIG_DATA>
                <STATUS>Open</STATUS>
                <FINDING_DETAILS>CAT3 Open</FINDING_DETAILS>
                <COMMENTS></COMMENTS>
                <SEVERITY_OVERRIDE></SEVERITY_OVERRIDE>
                <SEVERITY_JUSTIFICATION></SEVERITY_JUSTIFICATION>
            </VULN>
        </iSTIG>
    </STIGS>
</CHECKLIST>";

        // ---- Pass Tests ----

        [Fact]
        public void Test_LoadChecklist_ReturnsNotNull()
        {
            var checklist = ChecklistLoader.LoadChecklist(ValidCklXml);
            Assert.NotNull(checklist);
        }

        [Fact]
        public void Test_LoadChecklist_PopulatesAsset()
        {
            var checklist = ChecklistLoader.LoadChecklist(ValidCklXml);
            Assert.NotNull(checklist.ASSET);
        }

        [Fact]
        public void Test_LoadChecklist_Asset_HostNameIsCorrect()
        {
            var checklist = ChecklistLoader.LoadChecklist(ValidCklXml);
            Assert.Equal("TestHost", checklist.ASSET.HOST_NAME);
        }

        [Fact]
        public void Test_LoadChecklist_Asset_HostIpIsCorrect()
        {
            var checklist = ChecklistLoader.LoadChecklist(ValidCklXml);
            Assert.Equal("192.168.1.100", checklist.ASSET.HOST_IP);
        }

        [Fact]
        public void Test_LoadChecklist_Asset_HostMacIsCorrect()
        {
            var checklist = ChecklistLoader.LoadChecklist(ValidCklXml);
            Assert.Equal("AA:BB:CC:DD:EE:FF", checklist.ASSET.HOST_MAC);
        }

        [Fact]
        public void Test_LoadChecklist_Asset_HostFqdnIsCorrect()
        {
            var checklist = ChecklistLoader.LoadChecklist(ValidCklXml);
            Assert.Equal("testhost.corp.local", checklist.ASSET.HOST_FQDN);
        }

        [Fact]
        public void Test_LoadChecklist_Asset_RoleIsCorrect()
        {
            var checklist = ChecklistLoader.LoadChecklist(ValidCklXml);
            Assert.Equal("Member Server", checklist.ASSET.ROLE);
        }

        [Fact]
        public void Test_LoadChecklist_Asset_AssetTypeIsCorrect()
        {
            var checklist = ChecklistLoader.LoadChecklist(ValidCklXml);
            Assert.Equal("Computing", checklist.ASSET.ASSET_TYPE);
        }

        [Fact]
        public void Test_LoadChecklist_Asset_MarkingIsCorrect()
        {
            var checklist = ChecklistLoader.LoadChecklist(ValidCklXml);
            Assert.Equal("CUI", checklist.ASSET.MARKING);
        }

        [Fact]
        public void Test_LoadChecklist_StigInfo_HasSIDataEntries()
        {
            var checklist = ChecklistLoader.LoadChecklist(ValidCklXml);
            Assert.NotNull(checklist.STIGS.iSTIG.STIG_INFO);
            Assert.NotEmpty(checklist.STIGS.iSTIG.STIG_INFO.SI_DATA);
        }

        [Fact]
        public void Test_LoadChecklist_StigInfo_StigIdIsCorrect()
        {
            var checklist = ChecklistLoader.LoadChecklist(ValidCklXml);
            var stigId = checklist.STIGS.iSTIG.STIG_INFO.SI_DATA
                .Find(x => x.SID_NAME == "stigid");
            Assert.NotNull(stigId);
            Assert.Equal("Google_Chrome_Current_Windows", stigId.SID_DATA);
        }

        [Fact]
        public void Test_LoadChecklist_HasThreeVulnerabilities()
        {
            var checklist = ChecklistLoader.LoadChecklist(ValidCklXml);
            Assert.Equal(3, checklist.STIGS.iSTIG.VULN.Count);
        }

        [Fact]
        public void Test_LoadChecklist_FirstVuln_IsHighSeverityOpen()
        {
            var checklist = ChecklistLoader.LoadChecklist(ValidCklXml);
            var firstVuln = checklist.STIGS.iSTIG.VULN[0];
            Assert.Equal("Open", firstVuln.STATUS);
            var severity = firstVuln.STIG_DATA.Find(x => x.VULN_ATTRIBUTE == "Severity");
            Assert.NotNull(severity);
            Assert.Equal("high", severity.ATTRIBUTE_DATA);
        }

        [Fact]
        public void Test_LoadChecklist_SecondVuln_IsMediumNotAFinding()
        {
            var checklist = ChecklistLoader.LoadChecklist(ValidCklXml);
            var secondVuln = checklist.STIGS.iSTIG.VULN[1];
            Assert.Equal("NotAFinding", secondVuln.STATUS);
            var severity = secondVuln.STIG_DATA.Find(x => x.VULN_ATTRIBUTE == "Severity");
            Assert.NotNull(severity);
            Assert.Equal("medium", severity.ATTRIBUTE_DATA);
        }

        [Fact]
        public void Test_LoadChecklist_ThirdVuln_IsLowNotReviewed()
        {
            var checklist = ChecklistLoader.LoadChecklist(ValidCklXml);
            var thirdVuln = checklist.STIGS.iSTIG.VULN[2];
            Assert.Equal("Not_Reviewed", thirdVuln.STATUS);
            var severity = thirdVuln.STIG_DATA.Find(x => x.VULN_ATTRIBUTE == "Severity");
            Assert.NotNull(severity);
            Assert.Equal("low", severity.ATTRIBUTE_DATA);
        }

        [Fact]
        public void Test_LoadChecklist_MultiVuln_HasFiveVulns()
        {
            var checklist = ChecklistLoader.LoadChecklist(MultiVulnCklXml);
            Assert.Equal(5, checklist.STIGS.iSTIG.VULN.Count);
        }

        [Fact]
        public void Test_LoadChecklist_HandlesTabCharactersInXml()
        {
            var xmlWithTabs = ValidCklXml.Replace("    ", "\t");
            var checklist = ChecklistLoader.LoadChecklist(xmlWithTabs);
            Assert.NotNull(checklist);
            Assert.Equal("TestHost", checklist.ASSET.HOST_NAME);
        }

        // ---- Fail Tests ----

        [Fact]
        public void Test_LoadChecklist_VulnStigData_EmptySeverityOverride_IsNullOrEmpty()
        {
            var checklist = ChecklistLoader.LoadChecklist(ValidCklXml);
            var firstVuln = checklist.STIGS.iSTIG.VULN[0];
            Assert.True(string.IsNullOrEmpty(firstVuln.SEVERITY_OVERRIDE));
        }

        [Fact]
        public void Test_LoadChecklist_Asset_TargetKeyIsNotEmpty()
        {
            var checklist = ChecklistLoader.LoadChecklist(ValidCklXml);
            Assert.Equal("2350", checklist.ASSET.TARGET_KEY);
        }

        [Fact]
        public void Test_LoadChecklist_VulnCount_NotZero()
        {
            var checklist = ChecklistLoader.LoadChecklist(ValidCklXml);
            Assert.NotEmpty(checklist.STIGS.iSTIG.VULN);
        }

        [Fact]
        public void Test_LoadChecklist_FirstVuln_NotNotReviewed()
        {
            var checklist = ChecklistLoader.LoadChecklist(ValidCklXml);
            Assert.NotEqual("Not_Reviewed", checklist.STIGS.iSTIG.VULN[0].STATUS);
        }
    }
}
