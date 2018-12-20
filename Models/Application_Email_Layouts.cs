using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AIM_Interface.Models
{
    public class Application_Email_Layouts
    {
        public int PK_EMAIL { get; set; }
        public string EMAIL_TP_CONTENT { get; set; }
        public string EMAIL_SUBJECT { get; set; }
        public string EMAIL_RECIPIENTS { get; set; }
        public int EMAIL_COPY_RQSTR { get; set; }
        public int EMAIL_COPY_OWNR { get; set; }
        public string EMAIL_ADTL_CC { get; set; }
        public string EMAIL_BLIND_CC { get; set; }
        public int EMAIL_SHOW_DNR { get; set; }
        public int EMAIL_ATTACH_FLG { get; set; }
        public int APP_OPT_ID { get; set; }
        public int EMAIL_CUSTOM { get; set; }
        public string EMAIL_LAYOUT_NM { get; set; }
        public int APP_ID { get; set; }
        public string EMAIL_MESSAGE { get; set; }
        public string EMAIL_DSC_LAYOUT { get; set; }



    }

}