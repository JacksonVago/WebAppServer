﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class LocalClienteApp
    {
        public Int64 id { get; set; }
        public Int64 id_local { get; set; }
        public string str_cliente { get; set; }
        public string str_senha { get; set; }
        public Int16 int_qtdped { get; set; }
        public Int16 int_qtditem { get; set; }
        public double dbl_val_tot { get; set; }
        public double dbl_val_desc { get; set; }
        public double dbl_val_liq { get; set; }
        public double dbl_val_pag { get; set; }
        public DateTime dtm_inclusao { get; set; }
        public DateTime dtm_pagto { get; set; }
        public DateTime dtm_cancel { get; set; }
        public Int16 int_situacao { get; set; } //0 - Nunca utilizada / 1 - Em uso / 2 - Finalizada / 3 - Pagto parcial / 4 - Paga Total
        public Int64 id_server { get; set; }
        public Int16 int_sinc { get; set; }
    }
}