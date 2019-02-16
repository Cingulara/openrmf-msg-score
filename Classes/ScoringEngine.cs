using System;
using System.Collections.Generic;
using openstig_msg_score.Models;
using System.Linq;
using System.Xml.Linq;

namespace openstig_msg_score.Classes
{
    public static class ScoringEngine 
    {
        public static Score  ScoreChecklistString(string rawChecklist) {
          return ScoreChecklist(ChecklistLoader.LoadChecklist(rawChecklist));
        }
        public static Score ScoreChecklist (CHECKLIST xml)
        {
            try {
                Score score = new Score();
                // CAT 1
                score.totalCat1NotReviewed = xml.STIGS.iSTIG.VULN.Where(x => x.STATUS.ToLower() == "not_reviewed" && 
                        x.STIG_DATA.Where(y => y.VULN_ATTRIBUTE == "Severity" && 
                                               y.ATTRIBUTE_DATA == "high").FirstOrDefault() != null).Count();
                score.totalCat1NotApplicable = xml.STIGS.iSTIG.VULN.Where(x => x.STATUS.ToLower() == "not_applicable" && 
                        x.STIG_DATA.Where(y => y.VULN_ATTRIBUTE == "Severity" && 
                                               y.ATTRIBUTE_DATA == "high").FirstOrDefault() != null).Count();
                score.totalCat1Open = xml.STIGS.iSTIG.VULN.Where(x => x.STATUS.ToLower() == "open" && 
                        x.STIG_DATA.Where(y => y.VULN_ATTRIBUTE == "Severity" && 
                                               y.ATTRIBUTE_DATA == "high").FirstOrDefault() != null).Count();
                score.totalCat1NotAFinding = xml.STIGS.iSTIG.VULN.Where(x => x.STATUS.ToLower() == "notafinding" && 
                        x.STIG_DATA.Where(y => y.VULN_ATTRIBUTE == "Severity" && 
                                               y.ATTRIBUTE_DATA == "high").FirstOrDefault() != null).Count();
                // CAT 2
                score.totalCat2NotReviewed = xml.STIGS.iSTIG.VULN.Where(x => x.STATUS.ToLower() == "not_reviewed" && 
                        x.STIG_DATA.Where(y => y.VULN_ATTRIBUTE == "Severity" && 
                                               y.ATTRIBUTE_DATA == "medium").FirstOrDefault() != null).Count();
                score.totalCat2NotApplicable = xml.STIGS.iSTIG.VULN.Where(x => x.STATUS.ToLower() == "not_applicable" && 
                        x.STIG_DATA.Where(y => y.VULN_ATTRIBUTE == "Severity" && 
                                               y.ATTRIBUTE_DATA == "medium").FirstOrDefault() != null).Count();
                score.totalCat2Open = xml.STIGS.iSTIG.VULN.Where(x => x.STATUS.ToLower() == "open" && 
                        x.STIG_DATA.Where(y => y.VULN_ATTRIBUTE == "Severity" && 
                                               y.ATTRIBUTE_DATA == "medium").FirstOrDefault() != null).Count();
                score.totalCat2NotAFinding = xml.STIGS.iSTIG.VULN.Where(x => x.STATUS.ToLower() == "notafinding" && 
                        x.STIG_DATA.Where(y => y.VULN_ATTRIBUTE == "Severity" && 
                                               y.ATTRIBUTE_DATA == "medium").FirstOrDefault() != null).Count();
                // CAT 3
                score.totalCat3NotReviewed = xml.STIGS.iSTIG.VULN.Where(x => x.STATUS.ToLower() == "not_reviewed" && 
                        x.STIG_DATA.Where(y => y.VULN_ATTRIBUTE == "Severity" && 
                                               y.ATTRIBUTE_DATA == "low").FirstOrDefault() != null).Count();
                score.totalCat3NotApplicable = xml.STIGS.iSTIG.VULN.Where(x => x.STATUS.ToLower() == "not_applicable" && 
                        x.STIG_DATA.Where(y => y.VULN_ATTRIBUTE == "Severity" && 
                                               y.ATTRIBUTE_DATA == "low").FirstOrDefault() != null).Count();
                score.totalCat3Open = xml.STIGS.iSTIG.VULN.Where(x => x.STATUS.ToLower() == "open" && 
                        x.STIG_DATA.Where(y => y.VULN_ATTRIBUTE == "Severity" && 
                                               y.ATTRIBUTE_DATA == "low").FirstOrDefault() != null).Count();
                score.totalCat3NotAFinding = xml.STIGS.iSTIG.VULN.Where(x => x.STATUS.ToLower() == "notafinding" && 
                        x.STIG_DATA.Where(y => y.VULN_ATTRIBUTE == "Severity" && 
                                               y.ATTRIBUTE_DATA == "low").FirstOrDefault() != null).Count();
                return score;
            }
            catch (Exception ex) {
                Console.WriteLine("oops! " + ex.Message);
                return new Score();
            }
        }
    }
}