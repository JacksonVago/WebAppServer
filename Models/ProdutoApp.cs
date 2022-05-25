using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class ProdutoApp
    {
        public Int64 id { get; set; }
        public Int64 id_grupo { get; set; }
        public string str_descricao { get; set; }
        public string str_obs { get; set; }
        public Int32? int_qtd_est { get; set; }
        public Int32? int_qtd_combo { get; set; }
        public Double dbl_val_unit { get; set; }
        public Double dbl_val_desc { get; set; }
        public Double dbl_perc_desc { get; set; }
        public Double dbl_val_combo { get; set; }
        public string str_foto { get; set; }
        public Int16 int_tipo { get; set; }
        public DateTime dtm_inclusao { get; set; }
        public DateTime? dtm_alteracao { get; set; }
        public Int16 int_situacao { get; set; }
        public Int64 id_server { get; set; }
        public Int16 int_sinc { get; set; }
    }
}
