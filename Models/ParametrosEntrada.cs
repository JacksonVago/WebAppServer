using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppServer.Models
{
    public class ParametrosEntrada
    {
        public string generico { get; set; } 
        public string dllnome { get; set; }
        public string classe { get; set; }
        public string metodo { get; set; }
        public string procedure { get; set; }
        public List<Parametros> parametros { get; set; }

        public ParametrosEntrada()
        {
            parametros = new List<Parametros>();
        }
    }
}
