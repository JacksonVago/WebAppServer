using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class PrdEstoqueApp
    {
        public Int64 id { get; set; }
        public Int64 id_produto { get; set; }
        public Double? int_qtd_est { get; set; }
        public DateTime dtm_estoque { get; set; }
        public Int64 id_server { get; set; }
        public Int16 int_sinc { get; set; }
    }
}
