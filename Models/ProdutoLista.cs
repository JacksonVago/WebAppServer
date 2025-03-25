using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class ProdutoLista
    {
		public Int64 id { get; set; }
		public Int64 id_empresa { get; set; } //id_empresa no servidor
		public string str_nomelista { get; set; } //Nome da lista de produtos
		public string str_idprodutos { get; set; } //id´s dos produtos que faz parte da lista
		public Int64 id_app { get; set; } //Id do registro do APP
		public Int64 id_user_man { get; set; }

        public string str_foto { get; set; } //foto da lista
    }

}
