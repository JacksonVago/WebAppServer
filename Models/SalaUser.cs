using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class SalaUser
    {
        public Int64 id { get; set; }
        public Int64 id_sala { get; set; }
        public Int64 id_usuario { get; set; }
        public string str_idconnect { get; set; }
        public string str_conect { get; set; } //0 - desconectado / 1 - Conectado
        public DateTime dtm_inclusao { get; set; }
        public Int64 id_app { get; set; }
    }
}
