﻿using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppServer.Models;

namespace WebAppServer.Repositories
{
    public class HubRepository
    {
        private readonly ConfigDB configDB;
        private DataRepository repData;

        public HubRepository()
        {
            configDB = new ConfigDB();
            repData = new DataRepository();
        }

        public async Task<string> GravarPedido(Int64 empresa, string dados)
        {
            string str_ret = "";
            string str_operacao = "";
            Int64 id_pedido = 0;
            PedidoApp pedido = new PedidoApp();
            List<Pedido> pedidos = new List<Pedido>();

            pedido = JsonConvert.DeserializeObject<PedidoApp>(dados);

            if (pedido != null)
            {
                if (pedido.id_server == 0)
                {
                    str_operacao = "I";
                }
                else
                {
                    str_operacao = "U";
                }

                pedidos.Add(new Pedido
                {
                    id = pedido.id_server,
                    id_empresa = empresa,
                    id_localcli = pedido.id_server,
                    dtm_pedido = pedido.dtm_pedido,
                    dtm_pagto = pedido.dtm_pagto,
                    dtm_cancel = pedido.dtm_cancel,
                    int_qtd_item = pedido.int_qtd_item,
                    dbl_val_tot = pedido.dbl_val_tot,
                    dbl_val_desc = pedido.dbl_val_desc,
                    dbl_val_liq = pedido.dbl_val_liq,
                    dbl_val_pag = pedido.dbl_val_pag,
                    str_obs = pedido.str_obervacao,
                    int_situacao = pedido.int_situacao, //0 - Aberto / 1 - Confirmado / 2 - Entregue Parcial / 3 - Entregue total / 4 - Pago parcialmente / 5 - Pago total / 9 - Cancelado
                    id_usuario = pedido.id_usuario,
                    id_app = pedido.id,
                    id_user_man = pedido.id_usuario
                });

                string str_conn = configDB.ConnectString;

                using (SqlConnection conn = new SqlConnection(str_conn))
                {
                    conn.Open();

                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            id_pedido = Convert.ToInt64(repData.ManutencaoTabela<Pedido>(str_operacao, pedidos, "ntv_tbl_pedido", conn, tran).Split(";")[0]);
                            pedido.id_server = id_pedido;
                            str_ret = JsonConvert.SerializeObject(pedido);
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

            return str_ret;
        }

        public async Task<string> GravarPedidoItem(Int64 empresa, string dados)
        {
            string str_ret = "";
            string str_id = "";
            string str_operacao = "";

            List<PedidoItemApp> itemApp = new List<PedidoItemApp>();
            List<PedidoItem> itens = new List<PedidoItem>();

            itemApp = JsonConvert.DeserializeObject<List<PedidoItemApp>>((dados.Contains("[") ? dados : "[" + dados + "]"));

            if (itemApp != null && itemApp.Count > 0)
            {
                if (itemApp[0].id_server == 0)
                {
                    str_operacao = "I";
                }
                else
                {
                    str_operacao = "U";
                }

                for (int i = 0; i < itemApp.Count; i++)
                {
                    itens.Add(new PedidoItem
                    {
                        id = itemApp[i].id_server,
                        id_empresa = empresa,
                        id_pedido = itemApp[i].id_pedido,
                        id_produto = itemApp[i].id_produto,
                        dbl_precounit = itemApp[i].dbl_precounit,
                        int_qtd_comp = itemApp[i].int_qtd_comp,
                        dbl_tot_item = itemApp[i].dbl_tot_item,
                        dbl_desconto = itemApp[i].dbl_desconto,
                        dbl_tot_liq = itemApp[i].dbl_tot_liq,
                        int_situacao = itemApp[i].int_situacao, // 0 - Não entregue / 1 - Entregue / 9 - Cancelado
                        id_app = itemApp[i].id,
                        id_user_man = 0
                    });

                }

                if (itens.Count > 0)
                {
                    string str_conn = configDB.ConnectString;

                    using (SqlConnection conn = new SqlConnection(str_conn))
                    {
                        conn.Open();

                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            try
                            {
                                str_id = repData.ManutencaoTabela<PedidoItem>(str_operacao, itens, "ntv_tbl_pedidoitem", conn, tran);

                                for (int i = 0; i < itemApp.Count; i++)
                                {
                                    itemApp[i].id_server = Convert.ToInt64(str_id.Split(';')[i]);
                                }
                                str_ret = JsonConvert.SerializeObject(itemApp);
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

            return str_ret;
        }

        public async Task<string> GravarLocal(Int64 empresa, string dados)
        {
            string str_ret = "";
            string str_id = "";
            string str_operacao = "";

            List<LocalApp> itemApp = new List<LocalApp>();
            List<Local> itens = new List<Local>();

            itemApp = JsonConvert.DeserializeObject<List<LocalApp>>((dados.Contains("[") ? dados : "[" + dados + "]"));

            if (itemApp != null && itemApp.Count > 0)
            {
                if (itemApp[0].id_server == 0)
                {
                    str_operacao = "I";
                }
                else
                {
                    str_operacao = "U";
                }

                for (int i = 0; i < itemApp.Count; i++)
                {
                    itens.Add(new Local
                    {
                        id = itemApp[i].id_server,
                        id_empresa = empresa,
                        int_numero = itemApp[i].int_numero,
                        int_tipo = itemApp[i].int_tipo,
                        int_qtd_pess = itemApp[i].int_qtd_pess,
                        str_foto = itemApp[i].str_foto,
                        dtm_inclusao = itemApp[i].dtm_inclusao,
                        dtm_alteracao = DateTime.Now,
                        int_situacao = itemApp[i].int_situacao,
                        id_app = itemApp[i].id,
                        id_user_man = 0
                    });

                }

                if (itens.Count > 0)
                {
                    string str_conn = configDB.ConnectString;

                    using (SqlConnection conn = new SqlConnection(str_conn))
                    {
                        conn.Open();

                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            try
                            {
                                str_id = repData.ManutencaoTabela<Local>(str_operacao, itens, "ntv_tbl_local", conn, tran);

                                for (int i = 0; i < itemApp.Count; i++)
                                {
                                    itemApp[i].id_server = Convert.ToInt64(str_id.Split(';')[i]);
                                }
                                str_ret = JsonConvert.SerializeObject(itemApp);
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

            return str_ret;
        }

        public async Task<string> GravarLocalCli(Int64 empresa, string dados)
        {
            string str_ret = "";
            string str_id = "";
            string str_operacao = "";

            List<LocalClienteApp> itemApp = new List<LocalClienteApp>();
            List<LocalCliente> itens = new List<LocalCliente>();

            itemApp = JsonConvert.DeserializeObject<List<LocalClienteApp>>((dados.Contains("[") ? dados : "[" + dados + "]"));

            if (itemApp != null && itemApp.Count > 0)
            {
                if (itemApp[0].id_server == 0)
                {
                    str_operacao = "I";
                }
                else
                {
                    str_operacao = "U";
                }

                for (int i = 0; i < itemApp.Count; i++)
                {
                    itens.Add(new LocalCliente
                    {
                        id = itemApp[i].id_server,
                        id_empresa = empresa,
                        id_local = itemApp[i].id_local,
                        str_cliente = itemApp[i].str_cliente,
                        str_senha = itemApp[i].str_senha,
                        int_qtdped = itemApp[i].int_qtdped,
                        int_qtditem = itemApp[i].int_qtditem,
                        dbl_val_tot = itemApp[i].dbl_val_tot,
                        dbl_val_desc = itemApp[i].dbl_val_desc,
                        dbl_val_liq = itemApp[i].dbl_val_liq,
                        dbl_val_pag = itemApp[i].dbl_val_pag,
                        dtm_inclusao = itemApp[i].dtm_inclusao,
                        dtm_pagto = itemApp[i].dtm_pagto,
                        dtm_cancel = itemApp[i].dtm_cancel,
                        int_situacao = itemApp[i].int_situacao,
                        id_app = itemApp[i].id,
                        id_user_man = 0
                    });

                }

                if (itens.Count > 0)
                {
                    string str_conn = configDB.ConnectString;

                    using (SqlConnection conn = new SqlConnection(str_conn))
                    {
                        conn.Open();

                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            try
                            {
                                str_id = repData.ManutencaoTabela<LocalCliente>(str_operacao, itens, "ntv_tbl_localcliente", conn, tran);

                                for (int i = 0; i < itemApp.Count; i++)
                                {
                                    itemApp[i].id_server = Convert.ToInt64(str_id.Split(';')[i]);
                                }
                                str_ret = JsonConvert.SerializeObject(itemApp);
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

            return str_ret;
        }
    }
}