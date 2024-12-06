using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using WebAppServer.Models;

namespace WebAppServer.Repositories
{
    public class LocalClienteRepository
    {
        private readonly ConfigDB configDB;
        private DataRepository repData;

        public LocalClienteRepository()
        {
            configDB = new ConfigDB();
            repData = new DataRepository();

        }
        public async Task<string> GravarDados(string operacao, string dados)
        {
            string str_ret = "";
            Int64 id_ret = 0;
            LocalCliente local = new LocalCliente();
            List<LocalCliente> listDados = new List<LocalCliente>();
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
                listDados = JsonConvert.DeserializeObject<List<LocalCliente>>(str_param.IndexOf("[") == -1 ? "[" + str_param + "]" : str_param);

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
                                id_ret = Convert.ToInt64(repData.ManutencaoTabela<LocalCliente>(operacao, listDados, "ntv_tbl_localcliente", conn, tran).Split(";")[0]);
                                local = listDados[0];
                                local.id = id_ret;
                                str_ret = JsonConvert.SerializeObject(local);
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
        public async Task<string> GravarDadosPagto(string operacao, string dados)
        {
            string str_ret = "";
            Int64 id_ret = 0;
            LocalCliPag local = new LocalCliPag();
            List<LocalCliPag> listDados = new List<LocalCliPag>();
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
                listDados = JsonConvert.DeserializeObject<List<LocalCliPag>>(str_param.IndexOf("[") == -1 ? "[" + str_param + "]" : str_param);

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
                                id_ret = Convert.ToInt64(repData.ManutencaoTabela<LocalCliPag>(operacao, listDados, "ntv_tbl_localcliente", conn, tran).Split(";")[0]);
                                local = listDados[0];
                                local.id = id_ret;
                                str_ret = JsonConvert.SerializeObject(local);
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
