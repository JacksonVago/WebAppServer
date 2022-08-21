using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class Entrada
    {
        public Int64 id { get; set; }
        public Int64 id_empresa { get; set; }
        public DateTime dtm_entrada { get; set; }
        public Int64 int_nota { get; set; }
        public Int64 int_fornecedor { get; set; }
        public string str_serie { get; set; }
        public DateTime dtm_nota { get; set; }
        public double dbl_tot_nf { get; set; }
        public double int_qtd_item { get; set; }
        public double dbl_tot_item { get; set; }
        public double dbl_tot_desc { get; set; }
        public double dbl_tot_liq { get; set; }
        public string str_obs { get; set; }
        public Int16 int_situacao { get; set; } // 0 - Digitada / 1 - Conferida / 9 - Cancelado
        public Int64 id_usuario { get; set; }
        public Int64 id_app { get; set; }
        public Int64 id_user_man { get; set; }
    }
}
