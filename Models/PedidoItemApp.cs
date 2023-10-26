using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class PedidoItemApp
    {
        public Int64 id { get; set; }
        public Int64 id_pedido { get; set; }
        public Int64 id_produto { get; set; }
        public double dbl_precounit { get; set; }
        public double int_qtd_comp { get; set; }
        public double dbl_tot_item { get; set; }
        public double dbl_desconto { get; set; }
        public double dbl_tot_liq { get; set; }
        public Int16 int_situacao { get; set; } // 0 - Não entregue / 1 - Entregue / 9 - Cancelado
        public string str_combo { get; set; }
        public string str_obs { get; set; }
        public Int64 id_usuario { get; set; }
        public Int64 id_server { get; set; }
        public Int16 int_sinc { get; set; }
    }
}
