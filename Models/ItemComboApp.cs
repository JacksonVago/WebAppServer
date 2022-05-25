using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class ItemComboApp
    {
        public Int64 id { get; set; }
        public Int64 id_produto { get; set; }
        public Int32 int_qtd_item { get; set; }
        public Double dbl_val_unit { get; set; }
        public Int64 id_server { get; set; }
        public Int16 int_sinc { get; set; }
    }
}
