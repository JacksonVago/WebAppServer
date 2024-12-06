using System.Collections.Generic;

namespace WebAppServer.Models
{
    public class GenModels
    {
        public string classe { get; set; }
        public string metodo { get; set; }
        public string Operacao { get; set; }
        public List<Parametros> parametros { get; set; }

        public GenModels()
        {
            parametros = new List<Parametros>();
        }


    }
}
