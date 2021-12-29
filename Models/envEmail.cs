using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppServer.Models
{
    public class envEmail
    {
        public Int64 id { get; set; }
        public string str_de { get; set; }
        public string str_para { get; set; }
        public string str_cc { get; set; }
        public string str_ass { get; set; }
        public string str_msg { get; set; }
        public string str_html { get; set; }
        public DateTime dtm_data_inc { get; set; }        
        public string str_modulo { get; set; }
        public string str_obs { get; set; }
        public Int16 int_situacao { get; set; }
    }
}
