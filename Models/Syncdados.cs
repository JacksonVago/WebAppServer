using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class Syncdados
    {
		public Int64 id { get; set; }
		public Int64 id_empresa { get; set; }
		public Int64 id_usuario { get; set; }
		public Int64 id_usuario_dest{ get; set; }
		public string str_tabela { get; set; }
		public Int64 id_registro { get; set; }
		public DateTime dtm_inclusao { get; set; }
		public DateTime? dtm_sync { get; set; }
		public Int16 int_situacao { get; set; }
	}
}
