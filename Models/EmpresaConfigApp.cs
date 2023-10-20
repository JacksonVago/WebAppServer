using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class EmpresaConfigApp
    {
        public Int64 id_empresa { get; set; }
        public string str_impressora { get; set; }
        public string str_imp_ped { get; set; }
        public string str_imp_conta { get; set; }
        public string str_conf_rec { get; set; }
        public string str_conf_pronto { get; set; }
        public string str_conf_ent { get; set; }
        public double dbl_perc_serv { get; set; }
        public Int64 id_server { get; set; }
        public Int16 int_sinc { get; set; }
    }
}
