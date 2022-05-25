using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class PedidoApp
    {
        public Int64 id { get; set; }
        public Int64 id_localcli { get; set; }
        public DateTime dtm_pedido { get; set; }
        public DateTime? dtm_pagto { get; set; }
        public DateTime? dtm_cancel { get; set; }
        public Int16 int_qtd_item { get; set; }
        public double dbl_val_tot { get; set; }
        public double dbl_val_desc { get; set; }
        public double dbl_val_liq { get; set; }
        public double dbl_val_pag { get; set; }
        public string str_obervacao { get; set; }
        public Int16 int_situacao { get; set; } //0 - Aberto / 1 - Confirmado / 2 - Entregue Parcial / 3 - Entregue total / 4 - Pago parcialmente / 5 - Pago total / 9 - Cancelado
        public Int64 id_usuario { get; set; }
        public Int64 id_server { get; set; }
        public Int16 int_sinc { get; set; }
    }
}
