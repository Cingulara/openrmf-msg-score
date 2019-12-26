// Copyright (c) Cingulara 2019. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.

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