using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppServer.Models;

namespace WebAppServer.Repositories
{
    public class PedidoRepository
    {
        private readonly ConfigDB configDB;
        private DataRepository repData;

        public PedidoRepository()
        {
            configDB = new ConfigDB();
            repData = new DataRepository();
        }

        public async Task<string> GravarPedido(string operacao, string pedido, string item, string itemCombo, string itemAdic)
        {
            string str_ret = "";
            string str_ids = "";
            string str_operacao = "";
            Int64 id_pedido = 0;

            Pedido ped_grv = new Pedido();

            List<Pedido> pedidos = new List<Pedido>();
            List<PedidoItem> pedidoitem = new List<PedidoItem>();
            List<PedidoItemCombo> pedidoitemCmb = new List<PedidoItemCombo>();
            List<PedidoItemAdic> pedidoitemAdic = new List<PedidoItemAdic>();

            string str_param = "";

            try
            {
                //Carrega dados produtos
                if (pedido.GetType() != typeof(string))
                {
                    str_param = JsonConvert.SerializeObject(pedido);
                }
                else
                {
                    str_param = pedido;
                }
                pedidos = JsonConvert.DeserializeObject<List<Pedido>>(str_param.IndexOf("[") == -1 ? "[" + str_param + "]" : str_param);

                if (pedidos != null)
                {
                    //Carrega os itens do pedido
                    if (item.GetType() != typeof(string))
                    {
                        str_param = JsonConvert.SerializeObject(item);
                    }
                    else
                    {
                        str_param = item;
                    }
                    pedidoitem = JsonConvert.DeserializeObject<List<PedidoItem>>(str_param.IndexOf("[") == -1 ? "[" + str_param + "]" : str_param);

                    //Carrega dados produtos do combo
                    if (itemCombo.GetType() != typeof(string))
                    {
                        str_param = JsonConvert.SerializeObject(itemCombo);
                    }
                    else
                    {
                        str_param = itemCombo;
                    }
                    pedidoitemCmb = JsonConvert.DeserializeObject<List<PedidoItemCombo>>(str_param.IndexOf("[") == -1 ? "[" + str_param + "]" : str_param);

                    //Carrega dados produtos adicionais
                    if (itemAdic.GetType() != typeof(string))
                    {
                        str_param = JsonConvert.SerializeObject(itemAdic);
                    }
                    else
                    {
                        str_param = itemAdic;
                    }
                    pedidoitemAdic = JsonConvert.DeserializeObject<List<PedidoItemAdic>>(str_param.IndexOf("[") == -1 ? "[" + str_param + "]" : str_param);

                    string str_conn = configDB.ConnectString;

                    using (SqlConnection conn = new SqlConnection(str_conn))
                    {
                        conn.Open();

                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            try
                            {
                                //Grava pedido
                                id_pedido = Convert.ToInt64(repData.ManutencaoTabela<Pedido>(operacao, pedidos, "ntv_tbl_pedido", conn, tran).Split(";")[0]);
                                ped_grv = pedidos[0];
                                ped_grv.id = id_pedido;
                                str_ret = JsonConvert.SerializeObject(ped_grv);

                                //Grava itens do pedido
                                if (pedidoitem != null && pedidoitem.Count > 0)
                                {
                                    //carrega id do produto gravado
                                    for (int i = 0; i < pedidoitem.Count; i++)
                                    {
                                        if (operacao == "U")
                                        {
                                            pedidoitem[i].id = -1;
                                        }
                                        pedidoitem[i].id_pedido = id_pedido;
                                    }

                                    str_ids = repData.ManutencaoTabela<PedidoItem>("I", pedidoitem, "ntv_tbl_pedidoitem", conn, tran);
                                    if (str_ids.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ids.Split(";").Count(); id++)
                                        {
                                            if (str_ids.Split(";")[id] != "")
                                            {
                                                pedidoitem[id].id = Convert.ToInt64(str_ids.Split(";")[id]);
                                            }
                                        }
                                    }

                                    str_ret += "#|#|" + JsonConvert.SerializeObject(pedidoitem);
                                }
                                else
                                {
                                    str_ret += "#|#|";
                                }

                                //Grava itens combo
                                if (pedidoitemCmb != null && pedidoitemCmb.Count > 0)
                                {
                                    //carrega id do produto gravado
                                    for (int i = 0; i < pedidoitemCmb.Count; i++)
                                    {
                                        if (operacao == "U")
                                        {
                                            pedidoitemCmb[i].id = -1;
                                        }
                                        pedidoitemCmb[i].id_pedido = id_pedido;
                                    }

                                    str_ids = repData.ManutencaoTabela<PedidoItemCombo>("I", pedidoitemCmb, "ntv_tbl_pedidoitemcombo", conn, tran);
                                    if (str_ids.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ids.Split(";").Count(); id++)
                                        {
                                            if (str_ids.Split(";")[id] != "")
                                            {
                                                pedidoitemCmb[id].id = Convert.ToInt64(str_ids.Split(";")[id]);
                                            }
                                        }
                                    }

                                    str_ret += "#|#|" + JsonConvert.SerializeObject(pedidoitemCmb);
                                }
                                else
                                {
                                    str_ret += "#|#|";
                                }

                                //Grava itens adicionais
                                if (pedidoitemAdic != null && pedidoitemAdic.Count > 0)
                                {
                                    //carrega id do produto gravado
                                    for (int i = 0; i < pedidoitemAdic.Count; i++)
                                    {
                                        if (operacao == "U")
                                        {
                                            pedidoitemAdic[i].id = -1;
                                        }
                                        pedidoitemAdic[i].id_pedido = id_pedido;
                                    }

                                    str_ids = repData.ManutencaoTabela<PedidoItemAdic>("I", pedidoitemAdic, "ntv_tbl_pedidoitemadic", conn, tran);
                                    if (str_ids.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ids.Split(";").Count(); id++)
                                        {
                                            if (str_ids.Split(";")[id] != "")
                                            {
                                                pedidoitemAdic[id].id = Convert.ToInt64(str_ids.Split(";")[id]);
                                            }
                                        }
                                    }

                                    str_ret += "#|#|" + JsonConvert.SerializeObject(pedidoitemAdic);
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
