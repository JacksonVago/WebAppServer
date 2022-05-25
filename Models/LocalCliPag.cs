using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class LocalCliPag
    {
        public Int64 id { get; set; }
        public Int64 id_empresa { get; set; }
        public Int64 id_localcli { get; set; }
        public Int64 id_formapag { get; set; }
        public DateTime dtm_pagto { get; set; }
        public DateTime? dtm_cancel { get; set; }
        public double dbl_val_pgto { get; set; }
        public double dbl_desconto { get; set; }
        public Int64 id_app { get; set; }
        public Int64 id_user_man { get; set; }
    }
}
