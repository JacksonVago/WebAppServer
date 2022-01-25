using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class EmpresaApp
    {
        public Int64 id { get; set; }
        public Int64? int_cgccpf { get; set; }
        public string str_nome { get; set; }
        public string str_fantasia { get; set; }
        public string str_email { get; set; }
        public Int64? int_telefone { get; set; }
        public int int_local_atend { get; set; }
        public Int64 int_id_user_adm { get; set; }
        public DateTime dtm_inclusao { get; set; }
        public int int_situacao { get; set; }
        public int int_sitpag { get; set; }
        public DateTime? dtm_ultpag { get; set; }
        public Int64 id_server { get; set; }
        public Int16 int_sinc { get; set; }
    }
}
