using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class LocalCliente
    {
        public Int64 id { get; set; }
        public Int64 id_empresa { get; set; }
        public Int64 id_local { get; set; }
        public string str_cliente { get; set; }
        public string str_senha { get; set; }
        public Int16 int_qtdped { get; set; }
        public Int16 int_qtditem { get; set; }
        public double dbl_val_tot { get; set; }
        public double dbl_val_desc { get; set; }
        public double dbl_val_acres { get; set; }
        public double dbl_val_taxa { get; set; }
        public double dbl_val_liq { get; set; }
        public double dbl_val_pag { get; set; }
        public DateTime dtm_inclusao { get; set; }
        public DateTime? dtm_pagto { get; set; }
        public DateTime? dtm_cancel { get; set; }        
        public Int16 int_situacao { get; set; } //0 - Nunca utilizada / 1 - Em uso / 2 - Finalizada / 3 - Pagto parcial / 4 - Paga Total
        public Int64 id_usuario { get; set; }
        public Int64 id_app { get; set; }
        public Int64 id_user_man { get; set; }
    }
}
