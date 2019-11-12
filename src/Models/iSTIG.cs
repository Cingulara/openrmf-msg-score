using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace openrmf_msg_score.Models
{

    // this matches the ISTIG XML section in the Checklist CKL file
    public class iSTIG {

        public iSTIG (){
            STIG_INFO = new STIG_INFO();
            VULN = new List<VULN>();
        }

        public STIG_INFO STIG_INFO { get; set; }

        [XmlElement("VULN")]
        public List<VULN> VULN { get; set; }
    }
}