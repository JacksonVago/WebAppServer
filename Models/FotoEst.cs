using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class FotoEst
    {
        public Int64 id { get; set; }
        public Int64 id_empresa { get; set; }
        public Int64 id_produto { get; set; }
        public Double? int_qtd_fis { get; set; }
        public Double? int_qtd_res { get; set; }
        public DateTime dtm_estoque { get; set; }
        public Int64 id_app { get; set; }
        public Int64 id_user_man { get; set; }
    }
}
