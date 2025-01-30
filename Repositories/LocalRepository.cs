using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using WebAppServer.Models;
using System.Data;
using Npgsql;

namespace WebAppServer.Repositories
{
    public class LocalRepository
    {
        private readonly ConfigDB configDB;
        private DataRepository repData;

        public LocalRepository()
        {
            configDB = new ConfigDB();
            repData = new DataRepository();

        }
        public async Task<string> GravarDados(string operacao, string dados)
        {
            string str_ret = "";
            Int64 id_ret = 0;
            Local local = new Local();
            List<Local> listDados = new List<Local>();
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
                listDados = JsonConvert.DeserializeObject<List<Local>>(str_param.IndexOf("[") == -1 ? "[" + str_param + "]" : str_param);

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
                                id_ret = Convert.ToInt64(repData.ManutencaoTabela<Local>(operacao, listDados, "ntv_tbl_local", conn, tran).Split(";")[0]);
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
        public async Task<string> GravarDadosPostgres(string operacao, string dados)
        {
            string str_ret = "";
            Int64 id_ret = 0;
            Usuario usuario = new Usuario();
            List<Local> listDados = new List<Local>();
            List<Usuario> listUser = new List<Usuario>();
            string str_param = "";

            DataTable dtt_retorno = new DataTable();

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
                listDados = JsonConvert.DeserializeObject<List<Local>>(str_param.IndexOf("[") == -1 ? "[" + str_param + "]" : str_param);

                if (listDados != null)
                {
                    string str_conn = configDB.ConnectString;

                    using (NpgsqlConnection conn = new NpgsqlConnection(str_conn))
                    {
                        conn.Open();

                        using (NpgsqlTransaction tran = conn.BeginTransaction())
                        {
                            try
                            {

                                str_ret = JsonConvert.SerializeObject(listDados);
                                dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);

                                //Incluir coluna da operação a ser executada
                                if (dtt_retorno.Columns.Count > 0)
                                {
                                    if (!dtt_retorno.Columns.Contains("str_operation"))
                                    {
                                        dtt_retorno.Columns.Add(new DataColumn("str_operation", System.Type.GetType("System.String")));
                                        dtt_retorno.Rows[0]["str_operation"] = operacao;
                                    }
                                    str_ret = JsonConvert.SerializeObject(dtt_retorno);

                                    string sqlStr = "select * from f_man_tbl_ntv_tbl_local('{\"dados\": " + str_ret + "}') as id";
                                    str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, tran);
                                    //log.EscreverTextoNoArquivo("WebAppServer.Repositories.ProdutoRepository.GravarDadosPostgres", "Produtos.log", DateTime.Now.ToString("G") + " : Salvou." + str_ret);

                                    dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);

                                    if (dtt_retorno.Rows.Count > 0)
                                    {
                                        id_ret = Convert.ToInt64(dtt_retorno.Rows[0]["id"].ToString().Replace(";", ""));
                                        listDados[0].id = id_ret;

                                        //Gravar usuário do local para autoatendimento somento na inclusão do local
                                        if (operacao == "I")
                                        {
                                            usuario.id = 0;
                                            usuario.id_empresa = listDados[0].id_empresa;
                                            usuario.str_nome = "Usuário autoatendimento Local " + listDados[0].int_numero.ToString();
                                            usuario.str_login = listDados[0].id_empresa.ToString() + listDados[0].id.ToString();
                                            usuario.str_senha = listDados[0].id_empresa.ToString() + listDados[0].id.ToString();
                                            usuario.str_email = "";
                                            usuario.int_telefone = 0;
                                            usuario.int_tipo = 9;
                                            usuario.dtm_inclusao = DateTime.Now;
                                            usuario.dtm_saida = null;
                                            usuario.int_situacao = 1;
                                            usuario.str_foto = "";
                                            usuario.id_app = 0;
                                            usuario.id_user_man = listDados[0].id_user_man;

                                            listUser.Add(usuario);

                                            str_ret = JsonConvert.SerializeObject(listUser);
                                            dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);

                                            //Incluir coluna da operação a ser executada
                                            if (dtt_retorno.Columns.Count > 0)
                                            {
                                                if (!dtt_retorno.Columns.Contains("str_operation"))
                                                {
                                                    dtt_retorno.Columns.Add(new DataColumn("str_operation", System.Type.GetType("System.String")));
                                                    dtt_retorno.Rows[0]["str_operation"] = operacao;
                                                }
                                                str_ret = JsonConvert.SerializeObject(dtt_retorno);

                                                sqlStr = "select * from f_man_tbl_ntv_tbl_usuario('{\"dados\": " + str_ret + "}') as id";
                                                str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, tran);

                                                dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);

                                                if (dtt_retorno.Rows.Count > 0)
                                                {
                                                }
                                            }

                                        }
                                    }

                                }


                                str_ret = JsonConvert.SerializeObject(listDados[0]);
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
