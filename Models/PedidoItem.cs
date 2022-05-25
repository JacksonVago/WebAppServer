using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppServer.Models
{
    public class PedidoItem
    {
        public Int64 id { get; set; }
        public Int64 id_empresa { get; set; }
        public Int64 id_pedido { get; set; }
        public Int64 id_produto { get; set; }
        public double dbl_precounit { get; set; }
        public double int_qtd_comp { get; set; }
        public double dbl_tot_item { get; set; }
        public double dbl_desconto { get; set; }
        public double dbl_tot_liq { get; set; }
        public Int16 int_situacao { get; set; } // 0 - Não entregue / 1 - Entregue / 9 - Cancelado
        public Int64 id_app { get; set; }
        public Int64 id_user_man { get; set; }
    }
}
