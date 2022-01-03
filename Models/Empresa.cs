using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppServer.Models
{
    public class Empresa
    {
        public Int64 Id { get; set; }
        public Int64? int_cgccpf { get; set; }
        public string str_nome { get; set; }
        public string str_fantasia { get; set; }
        public string str_email { get; set; }
        public int? int_telefone { get; set; }
        public Int64 int_id_user_adm { get; set; }
        public DateTime dtm_inclusao { get; set; }
        public int int_situacao { get; set; }
        public int int_sitpag { get; set; }
        public DateTime? dtm_ultpag{ get; set; }
        public Int64 IdApp { get; set; }
    }
}
