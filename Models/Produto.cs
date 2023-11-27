using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class Produto
    {
        public Int64 id { get; set; }
        public Int64 id_empresa { get; set; }
        public Int64 id_grupo { get; set; }
        public string str_descricao { get; set; }
        public string str_obs { get; set; }
        public Double? int_qtd_estmin { get; set; }
        public Int32? int_qtd_combo { get; set; }
        public Double dbl_val_comp { get; set; }
        public Double dbl_val_unit { get; set; }
        public Double dbl_val_desc { get; set; }
        public Double dbl_perc_desc { get; set; }
        public Double dbl_val_combo { get; set; }
        public string str_foto { get; set; }
        public Int16 int_tipo { get; set; } // 1 - Normal / 2 - Combo / 3 - Transformado / 4 - Produtos de uso e consumo
        public Int16 int_unid_med { get; set; } // Unidade de medida 1 - Unidade / 2 - Kilograma / 3 - Litro 
        public string str_venda { get; set; }
        public string str_estoque { get; set; } //Controla estoque
        public string str_nec_prep { get; set; } //Necessita preparo sim ou não (para direcionar o pedido para a preparação
        public Int16 int_qtd_adic { get; set; } // Quantidade de propdutos que podem ser adicionados
        public DateTime dtm_inclusao { get; set; }
        public DateTime? dtm_alteracao { get; set; }
        public Int16 int_situacao { get; set; }
        public Int64 id_app { get; set; }
        public Int64 id_user_man { get; set; }
    }
}
