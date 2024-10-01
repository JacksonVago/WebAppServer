using System.Threading.Tasks;
using System;
using WebAppServer.Models;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace WebAppServer.Repositories
{
    public class FormaPagRepository
    {
        private readonly ConfigDB configDB;
        private DataRepository repData;

        public FormaPagRepository()
        {
            configDB = new ConfigDB();
            repData = new DataRepository();

        }
        public async Task<string> GravarDados(string operacao, string dados)
        {
            string str_ret = "";
            Int64 id_forma = 0;
            Usuario formapag = new Usuario();
            List<Usuario> listDados= new List<Usuario>();
            string str_param = "";
            if (dados.GetType() != typeof(string))
            {
                str_param = JsonConvert.SerializeObject(dados);
            }
            else
            {
                str_param = dados;
            }
            try
            {
                listDados = JsonConvert.DeserializeObject<List<Usuario>>(str_param.IndexOf("[") == -1 ? "[" + str_param + "]" : str_param);

                if (listDados != null)
                {
                    string str_conn = configDB.ConnectString;

                    using (SqlConnection conn = new SqlConnection(str_conn))
                    {
                        conn.Open();

                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            try
                            {
                                id_forma = Convert.ToInt64(repData.ManutencaoTabela<Usuario>(operacao, listDados, "tbl_usuario", conn, tran).Split(";")[0]);
                                formapag.id = id_forma;
                                str_ret = JsonConvert.SerializeObject(formapag);
                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                conn.Close();
                                throw ex;
                            }

                            tran.Commit();
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                str_ret = ex.Message.ToString();
            }
            return str_ret;

        }
    }
}
