using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class PedidoItemAdicApp
    {
        public Int64 id { get; set; }
        public Int64 id_pedido { get; set; } //Id do pedido 
        public Int64 id_ped_item { get; set; } //Id do item do pedido
        public Int64 id_produto { get; set; } //Id do produto do pedido
        public Int64 id_prod_adic { get; set; } //Id do produto adicionado
        public double int_qtd_comp { get; set; } //qtde adicionada do item
        public double dbl_val_unit { get; set; } //Preço do produto no momento da venda
        public Int64 id_usuario { get; set; }
        public Int64 id_server { get; set; }
        public Int16 int_sinc { get; set; }
    }
}
