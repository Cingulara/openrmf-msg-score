using System;
using System.Collections.Generic;


namespace openrmf_msg_score.Models
{

    // this matches the STIG_DATA XML sections in the Checklist CKL file
    public class STIG_DATA {

        public STIG_DATA (){

        }

		public string VULN_ATTRIBUTE { get; set; }
	    public string ATTRIBUTE_DATA { get; set;}
    }
}