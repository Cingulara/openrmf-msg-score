using System;
using System.Collections.Generic;


namespace openrmf_msg_score.Models
{

    // this matches the SI_DATA XML sections in the Checklist CKL file
    public class SI_DATA {

        public SI_DATA (){

        }

        public string SID_NAME { get; set;}
        public string SID_DATA { get; set; }
    }
}