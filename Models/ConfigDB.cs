using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WebAppServer.Models
{
    public class ConfigDB
    {
        private readonly string _strConnect;
        private readonly string _path;
        

        public ConfigDB()
        {
            //_path = Path.Combine("..\\WebAppServer.Data\\Config\\" + "configDB.json");
            //_path = Path.Combine(@".\WebAppServer.Data\Config\" + "configDB.json");
            //_path = Path.Combine(@".\" + "configDB.json");
            var JSON = System.IO.File.ReadAllText("configDB.json");
            var obj_config = JObject.Parse(JSON);

            //_strConnect = obj_config["ConnectionStrings"]["DeafultConnectionStrings"].ToString() + "JjmlS2023@";
            _strConnect = obj_config["ConnectionStrings1"]["DeafultConnectionStrings"].ToString() + "JjmlS2024$";
        }

        public string ConnectString {
            get
            {
                return _strConnect;
            }
        }
    }
}
