using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class LocalCliPagApp
    {
        public Int64 id { get; set; }
        public Int64 id_empresa { get; set; }
        public Int64 id_localcli { get; set; }
        public Int64 id_formapag { get; set; }
        public DateTime dtm_pagto { get; set; }
        public DateTime? dtm_cancel { get; set; }
        public double dbl_val_pgto { get; set; }
        public double dbl_desconto { get; set; }
        public double dbl_gorjeta { get; set; }
        public double dbl_acrescimo { get; set; }
        public Int64 id_usuario { get; set; }
        public Int64 id_server { get; set; }
        public Int16 int_sinc { get; set; }
    }
}
