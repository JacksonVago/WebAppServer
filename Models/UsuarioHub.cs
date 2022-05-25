using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppServer.Models
{
    class UsuarioHub
    {
        public Int64 id_usu_orig { get; set; }
        public Int64 id_empresa { get; set; }
        public Int64 id_usu_dest { get; set; }
        public string str_idconnect { get; set; }
    }
}
