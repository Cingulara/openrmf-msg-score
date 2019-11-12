using System;
using System.Collections.Generic;

namespace openrmf_msg_score.Models
{

    // this matches the root CHECKLIST XML section in the Checklist CKL file
    public class CHECKLIST {

        public CHECKLIST (){
            ASSET = new ASSET();
            STIGS = new STIGS();
        }

        public ASSET ASSET { get; set; }
        public STIGS STIGS { get; set; }
    }
}