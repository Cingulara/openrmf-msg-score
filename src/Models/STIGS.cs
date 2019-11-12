using System;
using System.Collections.Generic;

namespace openrmf_msg_score.Models
{

    // this matches the STIGS XML section in the Checklist CKL file
    public class STIGS {

        public STIGS (){
            iSTIG = new iSTIG();
        }

        public iSTIG iSTIG { get; set; }
    }
}