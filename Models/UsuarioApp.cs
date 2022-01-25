using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class UsuarioApp
    {
        public Int64 id { get; set; }
        public string str_nome { get; set; }
        public string str_login { get; set; }
        public string str_senha { get; set; }
        public string str_email { get; set; }
        public Int16 int_tipo { get; set; }
        public Int64? int_telefone { get; set; }
        public DateTime dtm_inclusao { get; set; }
        public DateTime? dtm_saida { get; set; }
        public Int16 int_situacao { get; set; }
        public string str_foto { get; set; }
        public Int64 id_server { get; set; }
        public Int16 int_sinc { get; set; }
    }
}
