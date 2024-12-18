﻿using System.Threading.Tasks;
using System;
using WebAppServer.Models;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace WebAppServer.Repositories
{
    public class EmpresaRepository
    {
        private readonly ConfigDB configDB;
        private DataRepository repData;

        public EmpresaRepository()
        {
            configDB = new ConfigDB();
            repData = new DataRepository();

        }
        public async Task<string> GravarDados(string operacao, string dados)
        {
            string str_ret = "";
            Int64 id_ret = 0;
            Empresa empresa = new Empresa();
            List<Empresa> listDados= new List<Empresa>();
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
                listDados = JsonConvert.DeserializeObject<List<Empresa>>(str_param.IndexOf("[") == -1 ? "[" + str_param + "]" : str_param);

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
                                id_ret = Convert.ToInt64(repData.ManutencaoTabela<Empresa>(operacao, listDados, "ntv_tbl_empresa", conn, tran).Split(";")[0]);
                                empresa = listDados[0];
                                empresa.id = id_ret;
                                str_ret = JsonConvert.SerializeObject(empresa);
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
        public async Task<string> GravarDadosConfig(string operacao, string dados)
        {
            string str_ret = "";
            Int64 id_ret = 0;
            EmpresaConfig empresa = new EmpresaConfig();
            List<EmpresaConfig> listDados = new List<EmpresaConfig>();
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
                listDados = JsonConvert.DeserializeObject<List<EmpresaConfig>>(str_param.IndexOf("[") == -1 ? "[" + str_param + "]" : str_param);

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
                                id_ret = Convert.ToInt64(repData.ManutencaoTabela<EmpresaConfig>(operacao, listDados, "ntv_tbl_empresa_config", conn, tran).Split(";")[0]);
                                empresa = listDados[0];
                                empresa.id_empresa = id_ret;
                                str_ret = JsonConvert.SerializeObject(empresa);
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
