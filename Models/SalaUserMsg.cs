using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class SalaUserMsg
    {
        public Int64 id { get; set; }
        public Int64 id_salauser { get; set; }
        public string str_msg { get; set; }
        public DateTime dtm_inclusao { get; set; }
        public Int64 id_app { get; set; }
    }
}
