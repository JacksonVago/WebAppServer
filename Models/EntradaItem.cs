using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class EntradaItem
    {
        public Int64 id { get; set; }
        public Int64 id_empresa { get; set; }
        public Int64 id_entrada { get; set; }
        public Int64 id_produto { get; set; }
        public double dbl_precounit { get; set; }
        public double int_qtd_comp { get; set; }
        public double dbl_tot_item { get; set; }
        public double dbl_desconto { get; set; }
        public double dbl_tot_liq { get; set; }
        public Int16 int_situacao { get; set; } // 0 - Digitado / 1 - Conferido / 2 - Divergênte / 9 - Cancelado
        public Int64 id_app { get; set; }
        public Int64 id_user_man { get; set; }
    }
}
