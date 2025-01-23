using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using WebAppServer.Models;
using System.Linq;
using Npgsql;
using System.Data;

namespace WebAppServer.Repositories
{
    public class ProdutoRepository
    {
        private readonly ConfigDB configDB;
        private DataRepository repData;
        private readonly DynamicLog log = new DynamicLog();

        public ProdutoRepository()
        {
            configDB = new ConfigDB();
            repData = new DataRepository();

        }
        public async Task<string> GravarDados(string operacao, string produto, string itemCombo, string itemAdic)
        {
            string str_ret = "";
            string str_ids = "";
            Int64 id_ret = 0;

            Produto dados_prod = new Produto();
            ItemCombo dados_cmb = new ItemCombo();
            ItemAdicional dados_aidc = new ItemAdicional();

            List<Produto> listDados_prod = new List<Produto>();
            List<ItemCombo> listDados_cmb = new List<ItemCombo>();
            List<ItemAdicional> listDados_adic = new List<ItemAdicional>();

            string str_param = "";

            try
            {
                //Carrega dados produtos
                if (produto.GetType() != typeof(string))
                {
                    str_param = JsonConvert.SerializeObject(produto);
                }
                else
                {
                    str_param = produto;
                }
                listDados_prod = JsonConvert.DeserializeObject<List<Produto>>(str_param.IndexOf("[") == -1 ? "[" + str_param + "]" : str_param);

                if (listDados_prod != null)
                {
                    //Carrega dados produtos do combo
                    if (itemCombo.GetType() != typeof(string))
                    {
                        str_param = JsonConvert.SerializeObject(itemCombo);
                    }
                    else
                    {
                        str_param = itemCombo;
                    }
                    listDados_cmb = JsonConvert.DeserializeObject<List<ItemCombo>>(str_param.IndexOf("[") == -1 ? "[" + str_param + "]" : str_param);

                    //Carrega dados produtos adicionais
                    if (itemAdic.GetType() != typeof(string))
                    {
                        str_param = JsonConvert.SerializeObject(itemAdic);
                    }
                    else
                    {
                        str_param = itemAdic;
                    }
                    listDados_adic = JsonConvert.DeserializeObject<List<ItemAdicional>>(str_param.IndexOf("[") == -1 ? "[" + str_param + "]" : str_param);

                    string str_conn = configDB.ConnectString;

                    using (SqlConnection conn = new SqlConnection(str_conn))
                    {
                        conn.Open();

                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            try
                            {
                                //Grava produto
                                id_ret = Convert.ToInt64(repData.ManutencaoTabela<Produto>(operacao, listDados_prod, "ntv_tbl_produto", conn, tran).Split(";")[0]);
                                dados_prod = listDados_prod[0];
                                dados_prod.id = id_ret;
                                str_ret = JsonConvert.SerializeObject(dados_prod);

                                //Grava itens combo
                                if (listDados_cmb != null && listDados_cmb.Count > 0)
                                {
                                    //carrega id do produto gravado
                                    for (int i = 0; i < listDados_cmb.Count; i++)
                                    {
                                        if (operacao == "U")
                                        {
                                            listDados_cmb[i].id = -1;
                                        }
                                        listDados_cmb[i].id_produto = id_ret;
                                    }

                                    str_ids = repData.ManutencaoTabela<ItemCombo>("I", listDados_cmb, "ntv_tbl_itemcombo", conn, tran);
                                    if (str_ids.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ids.Split(";").Count(); id++)
                                        {
                                            if (str_ids.Split(";")[id] != "")
                                            {
                                                listDados_cmb[id].id = Convert.ToInt64(str_ids.Split(";")[id]);
                                            }
                                        }
                                    }

                                    str_ret += "#|#|" + JsonConvert.SerializeObject(listDados_cmb);
                                }
                                else
                                {
                                    str_ret += "#|#|";
                                }

                                //Grava itens adicionais
                                if (listDados_adic != null && listDados_adic.Count > 0)
                                {
                                    //carrega id do produto gravado
                                    for (int i = 0; i < listDados_adic.Count; i++)
                                    {
                                        if (operacao == "U")
                                        {
                                            listDados_adic[i].id = -1;
                                        }
                                        listDados_adic[i].id_produto = id_ret;
                                    }

                                    str_ids = repData.ManutencaoTabela<ItemAdicional>("I", listDados_adic, "ntv_tbl_itemadicional", conn, tran);
                                    if (str_ids.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ids.Split(";").Count(); id++)
                                        {
                                            if (str_ids.Split(";")[id] != "")
                                            {
                                                listDados_adic[id].id = Convert.ToInt64(str_ids.Split(";")[id]);
                                            }
                                        }
                                    }

                                    str_ret += "#|#|" + JsonConvert.SerializeObject(listDados_adic);
                                }
                                else
                                {
                                    str_ret += "#|#|";
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
        public async Task<string> GravarDadosPostgres(string operacao, string produto, string itemCombo, string itemAdic)
        {

            string str_ret = "";
            string str_ret_fim = "";
            string str_ids = "";
            Int64 id_ret = 0;

            Produto dados_prod = new Produto();
            ItemCombo dados_cmb = new ItemCombo();
            ItemAdicional dados_aidc = new ItemAdicional();

            List<Produto> listDados_prod = new List<Produto>();
            List<ItemCombo> listDados_cmb = new List<ItemCombo>();
            List<ItemAdicional> listDados_adic = new List<ItemAdicional>();

            DataTable dtt_retorno = new DataTable();

            string str_param = "";

            //log.EscreverTextoNoArquivo("WebAppServer.Repositories.ProdutoRepository.GravarDadosPostgres", "Produtos.log", DateTime.Now.ToString("G") + " : Inicio da gravação. ");

            try
            {
                //Carrega dados produtos
                if (produto.GetType() != typeof(string))
                {
                    str_param = JsonConvert.SerializeObject(produto);
                }
                else
                {
                    str_param = produto;
                }
                listDados_prod = JsonConvert.DeserializeObject<List<Produto>>(str_param.IndexOf("[") == -1 ? "[" + str_param + "]" : str_param);

                //log.EscreverTextoNoArquivo("WebAppServer.Repositories.ProdutoRepository.GravarDadosPostgres", "Produtos.log", DateTime.Now.ToString("G") + " : Carregou produto. ");
                if (listDados_prod != null)
                {
                    //Carrega dados produtos do combo
                    //log.EscreverTextoNoArquivo("WebAppServer.Repositories.ProdutoRepository.GravarDadosPostgres", "Produtos.log", DateTime.Now.ToString("G") + " : Carrega dados produtos do combo. ");
                    if (itemCombo.GetType() != typeof(string))
                    {
                        str_param = JsonConvert.SerializeObject(itemCombo);
                    }
                    else
                    {
                        str_param = itemCombo;
                    }
                    listDados_cmb = JsonConvert.DeserializeObject<List<ItemCombo>>(str_param.IndexOf("[") == -1 ? "[" + str_param + "]" : str_param);

                    //Carrega dados produtos adicionais
                    //log.EscreverTextoNoArquivo("WebAppServer.Repositories.ProdutoRepository.GravarDadosPostgres", "Produtos.log", DateTime.Now.ToString("G") + " : Carrega dados produtos adicionais. ");
                    if (itemAdic.GetType() != typeof(string))
                    {
                        str_param = JsonConvert.SerializeObject(itemAdic);
                    }
                    else
                    {
                        str_param = itemAdic;
                    }
                    listDados_adic = JsonConvert.DeserializeObject<List<ItemAdicional>>(str_param.IndexOf("[") == -1 ? "[" + str_param + "]" : str_param);

                    string str_conn = configDB.ConnectString;

                    using (NpgsqlConnection conn = new NpgsqlConnection(configDB.ConnectString))
                    {
                        conn.Open();

                        using (NpgsqlTransaction tran = conn.BeginTransaction())
                        {
                            try
                            {
                                //Grava produto
                                //log.EscreverTextoNoArquivo("WebAppServer.Repositories.ProdutoRepository.GravarDadosPostgres", "Produtos.log", DateTime.Now.ToString("G") + " : Grava produto.");
                                str_ret = JsonConvert.SerializeObject(listDados_prod);
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

                                    //log.EscreverTextoNoArquivo("WebAppServer.Repositories.ProdutoRepository.GravarDadosPostgres", "Produtos.log", DateTime.Now.ToString("G") + " : Chama função para gravar.");
                                    string sqlStr = "select * from f_man_tbl_ntv_tbl_produto('{\"dados\": " + str_ret + "}') as id";
                                    str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, tran);
                                    //log.EscreverTextoNoArquivo("WebAppServer.Repositories.ProdutoRepository.GravarDadosPostgres", "Produtos.log", DateTime.Now.ToString("G") + " : Salvou." + str_ret);

                                    dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);

                                    if (dtt_retorno.Rows.Count > 0)
                                    {
                                        //Guarda id do produto principal
                                        //log.EscreverTextoNoArquivo("WebAppServer.Repositories.ProdutoRepository.GravarDadosPostgres", "Produtos.log", DateTime.Now.ToString("G") + " : Guarda id do produto principal." + dtt_retorno.Rows[0]["id"].ToString().Replace(";", ""));
                                        id_ret = Convert.ToInt64(dtt_retorno.Rows[0]["id"].ToString().Replace(";", ""));

                                        listDados_prod[0].id = id_ret;
                                        str_ret_fim = JsonConvert.SerializeObject(listDados_prod);

                                        //Grava itens combo
                                        if (listDados_cmb != null && listDados_cmb.Count > 0)
                                        {

                                            //carrega id do produto gravado para os itens do combo
                                            for (int i = 0; i < listDados_cmb.Count; i++)
                                            {
                                                if (operacao == "U")
                                                {
                                                    listDados_cmb[i].id = -1;
                                                }
                                                listDados_cmb[i].id_produto = id_ret;
                                            }

                                            str_ret = JsonConvert.SerializeObject(listDados_cmb);
                                            dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);
                                            //Incluir coluna da operação a ser executada
                                            if (dtt_retorno.Columns.Count > 0)
                                            {
                                                if (!dtt_retorno.Columns.Contains("str_operation"))
                                                {
                                                    dtt_retorno.Columns.Add(new DataColumn("str_operation", System.Type.GetType("System.String")));
                                                    for (int i = 0; i < dtt_retorno.Rows.Count; i++)
                                                    {
                                                        dtt_retorno.Rows[i]["str_operation"] = "I";
                                                    }
                                                }
                                                str_ret = JsonConvert.SerializeObject(dtt_retorno);

                                                sqlStr = "select * from f_man_tbl_ntv_tbl_itemcombo('{\"dados\": " + str_ret + "}') as id";
                                                str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, tran);
                                                dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);

                                                if (dtt_retorno.Rows.Count > 0)
                                                {
                                                    for (int i = 0; i < dtt_retorno.Rows[0]["id"].ToString().Split(";").Length; i++)
                                                    {

                                                        if (dtt_retorno.Rows[0]["id"].ToString().Split(";")[i] != "")
                                                        {
                                                            listDados_cmb[i].id = Convert.ToInt64(dtt_retorno.Rows[0]["id"].ToString().Split(";")[i]);
                                                        }
                                                    }
                                                }
                                            }

                                            str_ret_fim += "#|#|" + JsonConvert.SerializeObject(listDados_cmb);
                                        }
                                        else
                                        {
                                            str_ret_fim += "#|#|";
                                        }

                                        //Grava itens adicionais
                                        if (listDados_adic != null && listDados_adic.Count > 0)
                                        {
                                            //carrega id do produto gravado
                                            for (int i = 0; i < listDados_adic.Count; i++)
                                            {
                                                if (operacao == "U")
                                                {
                                                    listDados_adic[i].id = -1;
                                                }
                                                listDados_adic[i].id_produto = id_ret;
                                            }

                                            str_ret = JsonConvert.SerializeObject(listDados_adic);
                                            dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);

                                            //Incluir coluna da operação a ser executada
                                            if (dtt_retorno.Columns.Count > 0)
                                            {
                                                if (!dtt_retorno.Columns.Contains("str_operation"))
                                                {
                                                    dtt_retorno.Columns.Add(new DataColumn("str_operation", System.Type.GetType("System.String")));
                                                    for (int i = 0; i < dtt_retorno.Rows.Count; i++)
                                                    {
                                                        dtt_retorno.Rows[i]["str_operation"] = "I";
                                                    }
                                                }
                                                str_ret = JsonConvert.SerializeObject(dtt_retorno);

                                                sqlStr = "select * from f_man_tbl_ntv_tbl_itemadicional('{\"dados\": " + str_ret + "}') as id";
                                                str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, tran);
                                                dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);

                                                if (dtt_retorno.Rows.Count > 0)
                                                {
                                                    for (int i = 0; i < dtt_retorno.Rows[0]["id"].ToString().Split(";").Length; i++)
                                                    {
                                                        listDados_adic[i].id = Convert.ToInt64(dtt_retorno.Rows[0]["id"].ToString().Split(";")[i]);
                                                    }
                                                }
                                            }
                                            str_ret_fim += "#|#|" + JsonConvert.SerializeObject(listDados_adic);
                                        }
                                        else
                                        {
                                            str_ret_fim += "#|#|";
                                        }
                                    }

                                }

                                /*
                                id_ret = Convert.ToInt64(repData.ManutencaoTabela<Produto>(operacao, listDados_prod, "ntv_tbl_produto", conn, tran).Split(";")[0]);
                                dados_prod = listDados_prod[0];
                                dados_prod.id = id_ret;
                                str_ret = JsonConvert.SerializeObject(dados_prod);

                                //Grava itens combo
                                if (listDados_cmb != null && listDados_cmb.Count > 0)
                                {
                                    //carrega id do produto gravado
                                    for (int i = 0; i < listDados_cmb.Count; i++)
                                    {
                                        if (operacao == "U")
                                        {
                                            listDados_cmb[i].id = -1;
                                        }
                                        listDados_cmb[i].id_produto = id_ret;
                                    }

                                    str_ids = repData.ManutencaoTabela<ItemCombo>("I", listDados_cmb, "ntv_tbl_itemcombo", conn, tran);
                                    if (str_ids.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ids.Split(";").Count(); id++)
                                        {
                                            if (str_ids.Split(";")[id] != "")
                                            {
                                                listDados_cmb[id].id = Convert.ToInt64(str_ids.Split(";")[id]);
                                            }
                                        }
                                    }

                                    str_ret += "#|#|" + JsonConvert.SerializeObject(listDados_cmb);
                                }
                                else
                                {
                                    str_ret += "#|#|";
                                }

                                //Grava itens adicionais
                                if (listDados_adic != null && listDados_adic.Count > 0)
                                {
                                    //carrega id do produto gravado
                                    for (int i = 0; i < listDados_adic.Count; i++)
                                    {
                                        if (operacao == "U")
                                        {
                                            listDados_adic[i].id = -1;
                                        }
                                        listDados_adic[i].id_produto = id_ret;
                                    }

                                    str_ids = repData.ManutencaoTabela<ItemAdicional>("I", listDados_adic, "ntv_tbl_itemadicional", conn, tran);
                                    if (str_ids.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ids.Split(";").Count(); id++)
                                        {
                                            if (str_ids.Split(";")[id] != "")
                                            {
                                                listDados_adic[id].id = Convert.ToInt64(str_ids.Split(";")[id]);
                                            }
                                        }
                                    }

                                    str_ret += "#|#|" + JsonConvert.SerializeObject(listDados_adic);
                                }
                                else
                                {
                                    str_ret += "#|#|";
                                }*/

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
                str_ret_fim = "Error : " + ex.Message.ToString();
            }
            return str_ret_fim;

        }
    }
}
