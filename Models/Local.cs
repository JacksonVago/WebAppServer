﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class Local
    {
        public Int64 id { get; set; }
        public Int64 id_empresa { get; set; }
        public Int16? int_numero { get; set; } //Número de referência do local
        public Int16 int_tipo { get; set; } // 1 = "Guarda Sol" // 2 = "Mesa" // 3 = "Residência"
        public Int16? int_qtd_pess { get; set; } // Quandtidade pessoas suportada no local
        public string str_foto { get; set; } // Foto do local
        public DateTime dtm_inclusao { get; set; } // Data de inclusão do local
        public DateTime dtm_alteracao { get; set; } // Data de inclusão do local
        public Int16 int_situacao { get; set; } // 1 - Ativo/Desocupado / 2 - Ocupado / 9 - Desativado
        public Int64 id_app { get; set; } //Identificado no servidor
        public Int64 id_user_man { get; set; }
    }
}