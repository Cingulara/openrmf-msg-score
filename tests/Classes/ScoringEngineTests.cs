using Xunit;
using openrmf_msg_score.Classes;
using openrmf_msg_score.Models;
using System.Collections.Generic;

namespace tests.Classes
{
    public class ScoringEngineTests
    {
        // Shared test helper to build a VULN with a given severity and status
        private static VULN BuildVuln(string severity, string status, string severityOverride = "")
        {
            var vuln = new VULN
            {
                STATUS = status,
                SEVERITY_OVERRIDE = severityOverride,
                SEVERITY_JUSTIFICATION = string.Empty,
                FINDING_DETAILS = string.Empty,
                COMMENTS = string.Empty
            };
            vuln.STIG_DATA.Add(new STIG_DATA
            {
                VULN_ATTRIBUTE = "Severity",
                ATTRIBUTE_DATA = severity
            });
            return vuln;
        }

        private static CHECKLIST BuildChecklist(List<VULN> vulns)
        {
            var checklist = new CHECKLIST();
            checklist.STIGS.iSTIG.VULN = vulns;
            return checklist;
        }

        // CKL XML used by ScoreChecklistString tests
        private const string SimpleCklXml = @"<CHECKLIST>
    <ASSET>
        <ROLE>Member Server</ROLE>
        <ASSET_TYPE>Computing</ASSET_TYPE>
        <MARKING></MARKING>
        <HOST_NAME>TestServer</HOST_NAME>
        <HOST_IP>10.0.0.1</HOST_IP>
        <HOST_MAC>AA:BB:CC:DD:EE:FF</HOST_MAC>
        <HOST_FQDN>testserver.local</HOST_FQDN>
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
            </STIG_INFO>
            <VULN>
                <STIG_DATA><VULN_ATTRIBUTE>Severity</VULN_ATTRIBUTE><ATTRIBUTE_DATA>high</ATTRIBUTE_DATA></STIG_DATA>
                <STATUS>Open</STATUS>
                <FINDING_DETAILS>CAT1 Open finding</FINDING_DETAILS>
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
                <FINDING_DETAILS>CAT3 Open finding</FINDING_DETAILS>
                <COMMENTS></COMMENTS>
                <SEVERITY_OVERRIDE></SEVERITY_OVERRIDE>
                <SEVERITY_JUSTIFICATION></SEVERITY_JUSTIFICATION>
            </VULN>
        </iSTIG>
    </STIGS>
</CHECKLIST>";

        // ---- Pass Tests: ScoreChecklist ----

        [Fact]
        public void Test_ScoreChecklist_ReturnsNotNullScore()
        {
            var checklist = BuildChecklist(new List<VULN>
            {
                BuildVuln("high", "Open")
            });
            var score = ScoringEngine.ScoreChecklist(checklist);
            Assert.NotNull(score);
        }

        [Fact]
        public void Test_ScoreChecklist_EmptyVulnList_ReturnsZeroScores()
        {
            var checklist = BuildChecklist(new List<VULN>());
            var score = ScoringEngine.ScoreChecklist(checklist);
            Assert.Equal(0, score.totalCat1Open);
            Assert.Equal(0, score.totalCat2Open);
            Assert.Equal(0, score.totalCat3Open);
            Assert.Equal(0, score.totalOpen);
        }

        [Fact]
        public void Test_ScoreChecklist_SingleCat1Open_CountsCorrectly()
        {
            var checklist = BuildChecklist(new List<VULN>
            {
                BuildVuln("high", "Open")
            });
            var score = ScoringEngine.ScoreChecklist(checklist);
            Assert.Equal(1, score.totalCat1Open);
            Assert.Equal(0, score.totalCat2Open);
            Assert.Equal(0, score.totalCat3Open);
            Assert.Equal(1, score.totalOpen);
        }

        [Fact]
        public void Test_ScoreChecklist_SingleCat1NotAFinding_CountsCorrectly()
        {
            var checklist = BuildChecklist(new List<VULN>
            {
                BuildVuln("high", "NotAFinding")
            });
            var score = ScoringEngine.ScoreChecklist(checklist);
            Assert.Equal(1, score.totalCat1NotAFinding);
            Assert.Equal(0, score.totalCat1Open);
        }

        [Fact]
        public void Test_ScoreChecklist_SingleCat1NotApplicable_CountsCorrectly()
        {
            var checklist = BuildChecklist(new List<VULN>
            {
                BuildVuln("high", "Not_Applicable")
            });
            var score = ScoringEngine.ScoreChecklist(checklist);
            Assert.Equal(1, score.totalCat1NotApplicable);
        }

        [Fact]
        public void Test_ScoreChecklist_SingleCat1NotReviewed_CountsCorrectly()
        {
            var checklist = BuildChecklist(new List<VULN>
            {
                BuildVuln("high", "Not_Reviewed")
            });
            var score = ScoringEngine.ScoreChecklist(checklist);
            Assert.Equal(1, score.totalCat1NotReviewed);
        }

        [Fact]
        public void Test_ScoreChecklist_SingleCat2Open_CountsCorrectly()
        {
            var checklist = BuildChecklist(new List<VULN>
            {
                BuildVuln("medium", "Open")
            });
            var score = ScoringEngine.ScoreChecklist(checklist);
            Assert.Equal(1, score.totalCat2Open);
            Assert.Equal(0, score.totalCat1Open);
            Assert.Equal(0, score.totalCat3Open);
        }

        [Fact]
        public void Test_ScoreChecklist_SingleCat3Open_CountsCorrectly()
        {
            var checklist = BuildChecklist(new List<VULN>
            {
                BuildVuln("low", "Open")
            });
            var score = ScoringEngine.ScoreChecklist(checklist);
            Assert.Equal(1, score.totalCat3Open);
            Assert.Equal(0, score.totalCat1Open);
            Assert.Equal(0, score.totalCat2Open);
        }

        [Fact]
        public void Test_ScoreChecklist_MixedVulns_AllCategoriesAndStatuses()
        {
            var checklist = BuildChecklist(new List<VULN>
            {
                BuildVuln("high", "Open"),
                BuildVuln("high", "NotAFinding"),
                BuildVuln("high", "Not_Applicable"),
                BuildVuln("high", "Not_Reviewed"),
                BuildVuln("medium", "Open"),
                BuildVuln("medium", "NotAFinding"),
                BuildVuln("medium", "Not_Applicable"),
                BuildVuln("medium", "Not_Reviewed"),
                BuildVuln("low", "Open"),
                BuildVuln("low", "NotAFinding"),
                BuildVuln("low", "Not_Applicable"),
                BuildVuln("low", "Not_Reviewed")
            });
            var score = ScoringEngine.ScoreChecklist(checklist);

            Assert.Equal(1, score.totalCat1Open);
            Assert.Equal(1, score.totalCat1NotAFinding);
            Assert.Equal(1, score.totalCat1NotApplicable);
            Assert.Equal(1, score.totalCat1NotReviewed);
            Assert.Equal(1, score.totalCat2Open);
            Assert.Equal(1, score.totalCat2NotAFinding);
            Assert.Equal(1, score.totalCat2NotApplicable);
            Assert.Equal(1, score.totalCat2NotReviewed);
            Assert.Equal(1, score.totalCat3Open);
            Assert.Equal(1, score.totalCat3NotAFinding);
            Assert.Equal(1, score.totalCat3NotApplicable);
            Assert.Equal(1, score.totalCat3NotReviewed);
            Assert.Equal(3, score.totalOpen);
            Assert.Equal(3, score.totalNotAFinding);
            Assert.Equal(3, score.totalNotApplicable);
            Assert.Equal(3, score.totalNotReviewed);
        }

        [Fact]
        public void Test_ScoreChecklist_SeverityOverride_High_CountsAsCat1()
        {
            // A medium severity vuln overridden to high should count as Cat1
            var vuln = new VULN
            {
                STATUS = "Open",
                SEVERITY_OVERRIDE = "high",
                SEVERITY_JUSTIFICATION = string.Empty,
                FINDING_DETAILS = string.Empty,
                COMMENTS = string.Empty
            };
            vuln.STIG_DATA.Add(new STIG_DATA { VULN_ATTRIBUTE = "Severity", ATTRIBUTE_DATA = "medium" });

            var checklist = BuildChecklist(new List<VULN> { vuln });
            var score = ScoringEngine.ScoreChecklist(checklist);

            // The override is high, so should count as Cat1 Open
            Assert.Equal(1, score.totalCat1Open);
            // No Cat2 open since override applies
            Assert.Equal(0, score.totalCat2Open);
        }

        [Fact]
        public void Test_ScoreChecklist_SeverityOverride_Medium_CountsAsCat2()
        {
            var vuln = new VULN
            {
                STATUS = "NotAFinding",
                SEVERITY_OVERRIDE = "medium",
                SEVERITY_JUSTIFICATION = string.Empty,
                FINDING_DETAILS = string.Empty,
                COMMENTS = string.Empty
            };
            vuln.STIG_DATA.Add(new STIG_DATA { VULN_ATTRIBUTE = "Severity", ATTRIBUTE_DATA = "high" });

            var checklist = BuildChecklist(new List<VULN> { vuln });
            var score = ScoringEngine.ScoreChecklist(checklist);

            Assert.Equal(1, score.totalCat2NotAFinding);
            Assert.Equal(0, score.totalCat1NotAFinding);
        }

        [Fact]
        public void Test_ScoreChecklist_SeverityOverride_Low_CountsAsCat3()
        {
            var vuln = new VULN
            {
                STATUS = "Not_Reviewed",
                SEVERITY_OVERRIDE = "low",
                SEVERITY_JUSTIFICATION = string.Empty,
                FINDING_DETAILS = string.Empty,
                COMMENTS = string.Empty
            };
            vuln.STIG_DATA.Add(new STIG_DATA { VULN_ATTRIBUTE = "Severity", ATTRIBUTE_DATA = "high" });

            var checklist = BuildChecklist(new List<VULN> { vuln });
            var score = ScoringEngine.ScoreChecklist(checklist);

            Assert.Equal(1, score.totalCat3NotReviewed);
            Assert.Equal(0, score.totalCat1NotReviewed);
        }

        [Fact]
        public void Test_ScoreChecklist_MultipleHighOpens_CountsCorrectly()
        {
            var checklist = BuildChecklist(new List<VULN>
            {
                BuildVuln("high", "Open"),
                BuildVuln("high", "Open"),
                BuildVuln("high", "Open")
            });
            var score = ScoringEngine.ScoreChecklist(checklist);
            Assert.Equal(3, score.totalCat1Open);
            Assert.Equal(3, score.totalOpen);
        }

        // ---- Pass Tests: ScoreChecklistString ----

        [Fact]
        public void Test_ScoreChecklistString_ValidXml_ReturnsNotNull()
        {
            var score = ScoringEngine.ScoreChecklistString(SimpleCklXml);
            Assert.NotNull(score);
        }

        [Fact]
        public void Test_ScoreChecklistString_ValidXml_Cat1Open_IsOne()
        {
            var score = ScoringEngine.ScoreChecklistString(SimpleCklXml);
            Assert.Equal(1, score.totalCat1Open);
        }

        [Fact]
        public void Test_ScoreChecklistString_ValidXml_Cat1NotAFinding_IsOne()
        {
            var score = ScoringEngine.ScoreChecklistString(SimpleCklXml);
            Assert.Equal(1, score.totalCat1NotAFinding);
        }

        [Fact]
        public void Test_ScoreChecklistString_ValidXml_Cat2NotReviewed_IsOne()
        {
            var score = ScoringEngine.ScoreChecklistString(SimpleCklXml);
            Assert.Equal(1, score.totalCat2NotReviewed);
        }

        [Fact]
        public void Test_ScoreChecklistString_ValidXml_Cat2NotApplicable_IsOne()
        {
            var score = ScoringEngine.ScoreChecklistString(SimpleCklXml);
            Assert.Equal(1, score.totalCat2NotApplicable);
        }

        [Fact]
        public void Test_ScoreChecklistString_ValidXml_Cat3Open_IsOne()
        {
            var score = ScoringEngine.ScoreChecklistString(SimpleCklXml);
            Assert.Equal(1, score.totalCat3Open);
        }

        [Fact]
        public void Test_ScoreChecklistString_ValidXml_TotalOpen_IsTwo()
        {
            var score = ScoringEngine.ScoreChecklistString(SimpleCklXml);
            // 1 Cat1 Open + 1 Cat3 Open = 2
            Assert.Equal(2, score.totalOpen);
        }

        // ---- Fail Tests ----

        [Fact]
        public void Test_ScoreChecklist_NoCat1Vulns_Cat1TotalsAreZero()
        {
            var checklist = BuildChecklist(new List<VULN>
            {
                BuildVuln("medium", "Open"),
                BuildVuln("low", "Open")
            });
            var score = ScoringEngine.ScoreChecklist(checklist);
            Assert.Equal(0, score.totalCat1Open);
            Assert.Equal(0, score.totalCat1);
        }

        [Fact]
        public void Test_ScoreChecklist_OnlyCat1Vulns_Cat2Cat3AreZero()
        {
            var checklist = BuildChecklist(new List<VULN>
            {
                BuildVuln("high", "Open"),
                BuildVuln("high", "NotAFinding")
            });
            var score = ScoringEngine.ScoreChecklist(checklist);
            Assert.Equal(0, score.totalCat2);
            Assert.Equal(0, score.totalCat3);
        }

        [Fact]
        public void Test_ScoreChecklist_TotalOpenNotIncludingNotApplicable()
        {
            var checklist = BuildChecklist(new List<VULN>
            {
                BuildVuln("high", "Not_Applicable"),
                BuildVuln("medium", "Not_Applicable")
            });
            var score = ScoringEngine.ScoreChecklist(checklist);
            Assert.Equal(0, score.totalOpen);
            Assert.Equal(2, score.totalNotApplicable);
        }

        [Fact]
        public void Test_ScoreChecklist_TotalNotAFindingNotCountedAsOpen()
        {
            var checklist = BuildChecklist(new List<VULN>
            {
                BuildVuln("high", "NotAFinding"),
                BuildVuln("medium", "NotAFinding"),
                BuildVuln("low", "NotAFinding")
            });
            var score = ScoringEngine.ScoreChecklist(checklist);
            Assert.Equal(0, score.totalOpen);
            Assert.Equal(3, score.totalNotAFinding);
        }

        [Theory]
        [InlineData("high", "Open", 1, 0, 0)]
        [InlineData("medium", "Open", 0, 1, 0)]
        [InlineData("low", "Open", 0, 0, 1)]
        public void Test_ScoreChecklist_SingleVuln_CorrectCategory(
            string severity, string status,
            int expectCat1, int expectCat2, int expectCat3)
        {
            var checklist = BuildChecklist(new List<VULN> { BuildVuln(severity, status) });
            var score = ScoringEngine.ScoreChecklist(checklist);
            Assert.Equal(expectCat1, score.totalCat1Open);
            Assert.Equal(expectCat2, score.totalCat2Open);
            Assert.Equal(expectCat3, score.totalCat3Open);
        }
    }
}
