using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using WebAppServer.Models;
using System.Linq;

namespace WebAppServer.Repositories
{
    public class ProdutoRepository
    {
        private readonly ConfigDB configDB;
        private DataRepository repData;

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
    }
}
