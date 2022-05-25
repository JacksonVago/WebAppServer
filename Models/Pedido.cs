using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppServer.Models
{
    public class Pedido
    {
        public Int64 id { get; set; }
        public Int64 id_empresa { get; set; }
        public Int64 id_localcli { get; set; }
        public DateTime dtm_pedido { get; set; }
        public DateTime? dtm_pagto { get; set; }
        public DateTime? dtm_cancel { get; set; }
        public Int16 int_qtd_item { get; set; } 
        public double dbl_val_tot { get; set; }
        public double dbl_val_desc { get; set; }
        public double dbl_val_liq { get; set; }
        public double dbl_val_pag { get; set; }
        public string str_obs { get; set; }
        public Int16 int_situacao { get; set; } //0 - Aberto / 1 - Confirmado / 2 - Entregue Parcial / 3 - Entregue total / 4 - Pago parcialmente / 5 - Pago total / 9 - Cancelado
        public Int64 id_usuario { get; set; }
        public Int64 id_app { get; set; }
        public Int64 id_user_man { get; set; }
    }
}
