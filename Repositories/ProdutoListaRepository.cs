using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using WebAppServer.Models;

namespace WebAppServer.Repositories
{
    public class ProdutoListaRepository
    {
        private readonly ConfigDB configDB;
        private DataRepository repData;

        public ProdutoListaRepository()
        {
            configDB = new ConfigDB();
            repData = new DataRepository();

        }
        public async Task<string> GravarDados(string operacao, string dados)
        {
            string str_ret = "";
            Int64 id_ret = 0;
            ProdutoLista produtolista = new ProdutoLista();
            List<ProdutoLista> listDados = new List<ProdutoLista>();
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
                listDados = JsonConvert.DeserializeObject<List<ProdutoLista>>(str_param.IndexOf("[") == -1 ? "[" + str_param + "]" : str_param);

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
                                id_ret = Convert.ToInt64(repData.ManutencaoTabela<ProdutoLista>(operacao, listDados, "ntv_tbl_produto_lista", conn, tran).Split(";")[0]);
                                produtolista = listDados[0];
                                produtolista.id = id_ret;
                                str_ret = JsonConvert.SerializeObject(produtolista);
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
                str_ret = "Error : " + ex.Message.ToString();
            }
            return str_ret;

        }
    }
}
