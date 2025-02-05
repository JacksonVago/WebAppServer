using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System;
using WebAppServer.Models;

namespace WebAppServer.Repositories
{
    public class ContaRepository
    {
        private readonly ConfigDB configDB;
        private DataRepository repData;

        public ContaRepository()
        {
            configDB = new ConfigDB();
            repData = new DataRepository();
        }

        public async Task<string> GravarContaPostgres(string pagCliente, string locCliente)
        {
            string str_ret = "";
            string str_ret_fim = "";
            string str_ids = "";
            string str_operacao = "";
            Int64 id_pag = 0;

            Pedido ped_grv = new Pedido();

            List<LocalCliPag> pagCli = new List<LocalCliPag>();
            List<LocalCliente> localCli = new List<LocalCliente>();
            List<Local> local = new List<Local>();

            DataTable dtt_retorno = new DataTable();

            string str_param = "";

            try
            {
                //Carrega dados pedido
                if (pagCliente.GetType() != typeof(string))
                {
                    str_param = JsonConvert.SerializeObject(pagCliente);
                }
                else
                {
                    str_param = pagCliente;
                }
                pagCli = JsonConvert.DeserializeObject<List<LocalCliPag>>(str_param.IndexOf("[") == -1 ? "[" + str_param + "]" : str_param);

                if (pagCli != null)
                {
                    //Carrega local cliente
                    if (locCliente.GetType() != typeof(string))
                    {
                        str_param = JsonConvert.SerializeObject(locCliente);
                    }
                    else
                    {
                        str_param = locCliente;
                    }
                    localCli = JsonConvert.DeserializeObject<List<LocalCliente>>(str_param.IndexOf("[") == -1 ? "[" + str_param + "]" : str_param);


                    string str_conn = configDB.ConnectString;

                    using (NpgsqlConnection conn = new NpgsqlConnection(configDB.ConnectString))
                    {
                        conn.Open();

                        using (NpgsqlTransaction tran = conn.BeginTransaction())
                        {
                            try
                            {
                                //Grava pagamento
                                str_ret = JsonConvert.SerializeObject(pagCli);
                                dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);

                                //Incluir coluna da operação a ser executada
                                if (dtt_retorno.Columns.Count > 0)
                                {
                                    if (!dtt_retorno.Columns.Contains("str_operation"))
                                    {
                                        dtt_retorno.Columns.Add(new DataColumn("str_operation", System.Type.GetType("System.String")));
                                        dtt_retorno.Rows[0]["str_operation"] = "I";
                                    }
                                    str_ret = JsonConvert.SerializeObject(dtt_retorno);
                                    str_ret_fim = str_ret;

                                    string sqlStr = "select * from f_man_tbl_ntv_tbl_localclipag('{\"dados\": " + str_ret + "}') as id";
                                    str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, tran);
                                    dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);

                                    if (dtt_retorno.Rows.Count > 0)
                                    {
                                        //Guarda id do pagamento
                                        id_pag = Convert.ToInt64(dtt_retorno.Rows[0]["id"].ToString().Replace(";", ""));

                                        //Atualiza local cliente
                                        str_ret = JsonConvert.SerializeObject(localCli);
                                        dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);

                                        //Incluir coluna da operação a ser executada
                                        if (dtt_retorno.Columns.Count > 0)
                                        {
                                            if (!dtt_retorno.Columns.Contains("str_operation"))
                                            {
                                                dtt_retorno.Columns.Add(new DataColumn("str_operation", System.Type.GetType("System.String")));
                                                dtt_retorno.Rows[0]["str_operation"] = "U";
                                            }
                                            str_ret = JsonConvert.SerializeObject(dtt_retorno);
                                            str_ret_fim = str_ret;

                                            sqlStr = "select * from f_man_tbl_ntv_tbl_localcliente('{\"dados\": " + str_ret + "}') as id";
                                            str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, tran);
                                            dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);

                                            if (dtt_retorno.Rows.Count > 0)
                                            {
                                                //Libera local quando o pagamento for total
                                                if (localCli[0].int_situacao == 4)
                                                {
                                                    //Consulta local 
                                                    sqlStr = "select * from f_sel_tbl_ntv_tbl_local(" + localCli[0].id_local.ToString() + "," + localCli[0].id_empresa.ToString() + ",0)";

                                                    str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, tran);
                                                    dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);

                                                    if (dtt_retorno.Columns.Count > 0)
                                                    {
                                                        dtt_retorno.Columns.Add(new DataColumn("str_operation", System.Type.GetType("System.String")));
                                                        dtt_retorno.Rows[0]["str_operation"] = "U";
                                                        dtt_retorno.Rows[0]["int_situacao"] = 1;

                                                        str_ret = JsonConvert.SerializeObject(dtt_retorno);

                                                        //Libera local para outro cliente
                                                        sqlStr = "select * from f_man_tbl_ntv_tbl_local('{\"dados\": " + str_ret + "}') as id";
                                                        str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, tran);
                                                        dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);

                                                        if (dtt_retorno.Rows.Count > 0)
                                                        {
                                                            //Atualiza situação do pedidos somente quando paagar total
                                                            //Consulta pedidos
                                                            sqlStr = "select * from f_sel_tbl_ntv_tbl_pedido(0," + localCli[0].id_empresa + "," + localCli[0].id + ",'2001-01-01','2001-01-01',1)";

                                                            str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, tran);
                                                            dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);

                                                            if (dtt_retorno.Columns.Count > 0)
                                                            {
                                                                dtt_retorno.Columns.Add(new DataColumn("str_operation", System.Type.GetType("System.String")));

                                                                for (int i = 0; i < dtt_retorno.Rows.Count; i++)
                                                                {
                                                                    dtt_retorno.Rows[i]["int_situacao"] = 4;
                                                                    dtt_retorno.Rows[i]["str_operation"] = "U";
                                                                }
                                                                str_ret = JsonConvert.SerializeObject(dtt_retorno);

                                                                sqlStr = "select * from f_man_tbl_ntv_tbl_pedido('{\"dados\": " + str_ret + "}') as id";
                                                                str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, tran);
                                                                dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);

                                                                if (dtt_retorno.Rows.Count > 0)
                                                                {
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        str_ret += "#|#|";
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }

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
