using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class Sala
    {
        public Int64 id { get; set; }
        public Int64 id_empresa { get; set; }
        public DateTime dtm_inclusao { get; set; }
        public Int64 id_app { get; set; }
    }
}
