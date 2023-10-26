using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class ItemAdicionalApp
    {
        public Int64 id { get; set; }
        public Int64 id_produto { get; set; } //Id do produto
        public Int64 id_prd_adicional { get; set; } //id dos produto que compõe o combo quando negativo é uma lista de opções
        public double dbl_precounit { get; set; } //Preço do item no momento da venda
        public Int64 id_server { get; set; }
        public Int16 int_sinc { get; set; }
    }
}
