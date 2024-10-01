using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class UsuarioAcesso
    {
        public Int64 id { get; set; }
        public Int64 id_empresa { get; set; }
        public Int16 int_tipo { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public DateTime validade { get; set; }
    }
}

