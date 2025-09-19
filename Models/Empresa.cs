using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppServer.Models
{
    public class Empresa
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
        public DateTime? dtm_ultpag{ get; set; }
        public Int64 id_app { get; set; }
        public Int64 id_user_man { get; set; }
        public string str_chave_pix { get; set; }
        public string str_logo { get; set; }
    }
}
