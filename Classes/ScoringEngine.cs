using System;
using System.Collections.Generic;
using openstig_msg_score.Models;
using System.Linq;
using System.Xml.Linq;

namespace openstig_msg_score.Classes
{
    public static class ScoringEngine 
    {
        public static void ScoreChecklist (string rawChecklist)
        {
            try {
                CHECKLIST xml = ChecklistLoader.LoadChecklist(rawChecklist);
                Score score = new Score();
                score.totalCat1NotReviewed = xml.STIGS.iSTIG.VULN.Where(x => x.STATUS.ToLower() == "not_reviewed").Count();
                score.totalCat1NotApplicable = xml.STIGS.iSTIG.VULN.Where(x => x.STATUS.ToLower() == "not_applicable").Count();
                score.totalCat1Open = xml.STIGS.iSTIG.VULN.Where(x => x.STATUS.ToLower() == "open").Count();
                score.totalCat1NotAFinding = xml.STIGS.iSTIG.VULN.Where(x => x.STATUS.ToLower() == "notafinding" && 
                        x.STIG_DATA.Where(y => y.VULN_ATTRIBUTE == "Severity" && 
                                               y.ATTRIBUTE_DATA == "high").FirstOrDefault() != null).Count();
                // CHECKLISTSTIGSISTIGVULN[] asdVulnerabilities = asdSTIG.VULN;
                // cklScore.NotReviewed = asdVulnerabilities.Where(x => x.STATUS.ToLower() == "not_reviewed").Count();
                // cklScore.NotApplicable = asdVulnerabilities.Where(x => x.STATUS.ToLower() == "not_applicable").Count();
                // cklScore.Open = asdVulnerabilities.Where(x => x.STATUS.ToLower() == "open").Count();
                // cklScore.NotAFinding = asdVulnerabilities.Where(x => x.STATUS.ToLower() == "notafinding").Count();
            }
            catch (Exception ex) {
                Console.WriteLine("oops! " + ex.Message);
            }
        }
    }
}