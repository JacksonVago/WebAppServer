using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class Grupo
    {
        public Int64 id { get; set; }
        public Int64 id_empresa { get; set; }
        public string str_descricao { get; set; }
        public string str_foto { get; set; }
        public DateTime dtm_inclusao { get; set; }
        public DateTime? dtm_alteracao { get; set; }
        public Int16 int_situacao { get; set; }
        public Int64 id_app { get; set; }
        public Int64 id_user_man { get; set; }
    }
}
