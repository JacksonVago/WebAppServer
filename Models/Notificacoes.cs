using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class Notificacoes
    {
        public Int64 id { get; set; }
        public Int64 id_empresa { get; set; }
        public Int64 id_usu_orig { get; set; }
        public Int64 id_usu_dest { get; set; }
        public string str_tabela { get; set; }
        public Int64 id_registro { get; set; }
        public DateTime dtm_inclusao { get; set; }
        public Int16 int_situacao { get; set; }
    }
}
