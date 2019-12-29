// Copyright (c) Cingulara LLC 2019 and Tutela LLC 2019. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.
using System;
using openrmf_msg_score.Models;
using System.Linq;

namespace openrmf_msg_score.Classes
{
    public static class ScoringEngine 
    {
        /// <summary>
        /// Return a score of all status items and categories of vulnerabilities based 
        /// on the raw checklist passed in. This routine takes the raw checklist, makes it 
        /// a real CHECKLIST object, then calls the below ScoreChecklist() function.
        /// </summary>
        /// <param name="rawChecklist">The raw XML string from the checklist CKL file</param>
        /// <returns>A Score record showing the data from the XML checklist passed in</returns>
        public static Score ScoreChecklistString(string rawChecklist) {
          return ScoreChecklist(ChecklistLoader.LoadChecklist(rawChecklist));
        }

        /// <summary>
        /// Return a score of all status items and categories of vulnerabilities based 
        /// on the CHECKLIST object. This routine takes in the C# class and ises Linq to 
        /// do a count on records to find them based on severity (high, medium, low) and 
        /// the status.
        /// </summary>
        /// <param name="xml">The XML of a checklist in C# class object structure</param>
        /// <returns>A Score record showing the data from the XML checklist object passed in</returns>
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