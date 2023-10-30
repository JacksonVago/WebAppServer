using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebAppServer.Models;

namespace WebAppServer.Repositories
{
    public class HubRepository
    {
        private readonly ConfigDB configDB;
        private DataRepository repData;
        private readonly NotificacoesRepository _repNotif;

        public HubRepository()
        {
            configDB = new ConfigDB();
            repData = new DataRepository();
            _repNotif = new NotificacoesRepository();
        }

        public async Task<string> PagarConta(Int64 empresa, string pedidos, string formas)
        {
            string str_ret = "";
            string str_operacao = "";
            Int64 id_pedido = 0;
            PedidoApp pedido = new PedidoApp();
            List<PedidoApp> lst_pedApp = new List<PedidoApp>();
            List<Pedido> pedidosUpd = new List<Pedido>();
            List<Pedido> pedidosIns = new List<Pedido>();

            if (pedidos.IndexOf("[") > -1)
            {
                lst_pedApp = JsonConvert.DeserializeObject<List<PedidoApp>>(pedidos);
            }
            else
            {
                pedido = JsonConvert.DeserializeObject<PedidoApp>(pedidos);
            }


            if (pedido != null)
            {
                if (pedido.id_server == 0)
                {
                    pedidosIns.Add(new Pedido
                    {
                        id = pedido.id_server,
                        id_empresa = empresa,
                        id_localcli = pedido.id_localcli,
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
                }
                else
                {
                    pedidosUpd.Add(new Pedido
                    {
                        id = pedido.id_server,
                        id_empresa = empresa,
                        id_localcli = pedido.id_localcli,
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
                }
            }
            else
            {
                if (lst_pedApp.Count > 0)
                {
                    foreach (var item in lst_pedApp)
                    {
                        if (item.id_server == 0)
                        {
                            pedidosIns.Add(new Pedido
                            {
                                id = item.id_server,
                                id_empresa = empresa,
                                id_localcli = item.id_localcli,
                                dtm_pedido = item.dtm_pedido,
                                dtm_pagto = item.dtm_pagto,
                                dtm_cancel = item.dtm_cancel,
                                int_qtd_item = item.int_qtd_item,
                                dbl_val_tot = item.dbl_val_tot,
                                dbl_val_desc = item.dbl_val_desc,
                                dbl_val_liq = item.dbl_val_liq,
                                dbl_val_pag = item.dbl_val_pag,
                                str_obs = item.str_obervacao,
                                int_situacao = item.int_situacao, //0 - Aberto / 1 - Confirmado / 2 - Iniciado / 3 - Finalizado / 4 - Entregue Parcial / 5 - Entregue total / 6 - Pago parcialmente / 7 - Pago total / 9 - Cancelado
                                id_usuario = item.id_usuario,
                                id_app = item.id,
                                id_user_man = item.id_usuario
                            });
                        }
                        else
                        {
                            pedidosUpd.Add(new Pedido
                            {
                                id = item.id_server,
                                id_empresa = empresa,
                                id_localcli = item.id_localcli,
                                dtm_pedido = item.dtm_pedido,
                                dtm_pagto = item.dtm_pagto,
                                dtm_cancel = item.dtm_cancel,
                                int_qtd_item = item.int_qtd_item,
                                dbl_val_tot = item.dbl_val_tot,
                                dbl_val_desc = item.dbl_val_desc,
                                dbl_val_liq = item.dbl_val_liq,
                                dbl_val_pag = item.dbl_val_pag,
                                str_obs = item.str_obervacao,
                                int_situacao = item.int_situacao, //0 - Aberto / 1 - Confirmado / 2 - Iniciado / 3 - Finalizado / 4 - Entregue Parcial / 5 - Entregue total / 6 - Pago parcialmente / 7 - Pago total / 9 - Cancelado
                                id_usuario = item.id_usuario,
                                id_app = item.id,
                                id_user_man = item.id_usuario
                            });
                        }

                    }
                }
            }

            if (pedidosIns.Count > 0 || pedidosUpd.Count > 0)
            {
                string str_conn = configDB.ConnectString;

                using (SqlConnection conn = new SqlConnection(str_conn))
                {
                    conn.Open();

                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            //Inclusões
                            if (pedidosIns.Count > 0)
                            {
                                id_pedido = Convert.ToInt64(repData.ManutencaoTabela<Pedido>("I", pedidosIns, "ntv_tbl_pedido", conn, tran).Split(";")[0]);
                                pedido.id_server = id_pedido;
                                str_ret = JsonConvert.SerializeObject(pedido);
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


            return str_ret;
        }

        public async Task<string> GravarProduto(Int64 empresa, string dados)
        {
            string str_ret = "";
            string str_operacao = "";
            Int64 id_produto = 0;
            ProdutoApp produto = new ProdutoApp();
            List<Produto> produtos = new List<Produto>();

            produto = JsonConvert.DeserializeObject<ProdutoApp>(dados);

            if (produto != null)
            {
                if (produto.id_server == 0)
                {
                    str_operacao = "I";
                }
                else
                {
                    str_operacao = "U";
                }

                produtos.Add(new Produto
                {
                    id = produto.id_server,
                    id_empresa = empresa,
                    id_grupo = produto.id_grupo,
                    str_descricao = produto.str_descricao,
                    str_obs = produto.str_obs,
                    int_qtd_est = produto.int_qtd_est,
                    int_qtd_combo = produto.int_qtd_combo,
                    dbl_val_comp = produto.dbl_val_comp,
                    dbl_val_unit = produto.dbl_val_unit,
                    dbl_val_desc = produto.dbl_val_desc,
                    dbl_perc_desc = produto.dbl_perc_desc,
                    dbl_val_combo = produto.dbl_val_combo,
                    str_foto = produto.str_foto,
                    int_tipo = produto.int_tipo,
                    int_unid_med = produto.int_unid_med,
                    str_venda = produto.str_venda,
                    str_estoque = produto.str_estoque,
                    str_nec_prep = produto.str_nec_prep,
                    dtm_inclusao = produto.dtm_inclusao,
                    dtm_alteracao = produto.dtm_alteracao,
                    int_situacao = produto.int_situacao,
                    id_app = produto.id,
                    id_user_man = 1

                });

                string str_conn = configDB.ConnectString;

                using (SqlConnection conn = new SqlConnection(str_conn))
                {
                    conn.Open();

                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            id_produto = Convert.ToInt64(repData.ManutencaoTabela<Produto>(str_operacao, produtos, "ntv_tbl_produto", conn, tran).Split(";")[0]);
                            produto.id_server = id_produto;
                            str_ret = JsonConvert.SerializeObject(produto);
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

        public async Task<string> AtualizaEstoqueProduto(Int64 empresa, string dados)
        {
            string str_ret = "";

            ProdutoApp produto = new ProdutoApp();
            List<EstoqueProduto> produtos = new List<EstoqueProduto>();

            produtos = JsonConvert.DeserializeObject<List<EstoqueProduto>>((dados.Contains("[") ? dados : "[" + dados + "]"));

            if (produtos != null)
            {
                if (produtos.Count() > 0)
                {
                    string str_conn = configDB.ConnectString;

                    using (SqlConnection conn = new SqlConnection(str_conn))
                    {
                        conn.Open();

                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            try
                            {
                                foreach (var item in produtos)
                                {

                                    //Atualiza estoque do produto
                                    bool bol_estoque = repData.SqlUpdate("[{ \"nome\":\"id\", \"valor\":\"" + item.id_produto.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                         "{ \"nome\":\"id_empresa\", \"valor\":\"" + empresa.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                         "{ \"nome\":\"int_qtd_est\", \"valor\":\"" + item.int_qtd_est.ToString() + "\", \"tipo\":\"Int64\"}]",
                                                                         "ntv_p_atu_estoque_produto", conn, tran);
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

            return str_ret;
        }

        public async Task<string> CarregaEstoque(Int64 empresa)
        {
            string str_ret = "";

            List<Produto> produtos = new List<Produto>();
            List<EstoqueProduto> estoque = new List<EstoqueProduto>();

            string str_conn = configDB.ConnectString;

            using (SqlConnection conn = new SqlConnection(str_conn))
            {
                conn.Open();

                try
                {
                    str_ret = repData.ConsultaGenerica("[{ \"nome\":\"id\", \"valor\":\0\", \"tipo\":\"Int64\"}," +
                                                                "{ \"nome\":\"id_empresa\", \"valor\":\"" + empresa.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                                "{ \"nome\":\"download\", \"valor\":\"1\", \"tipo\":\"Int64\"}," +
                                                                "{ \"nome\":\"id_app\", \"valor\":\"0\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_produto", conn);
                    produtos = JsonConvert.DeserializeObject<List<Produto>>(str_ret);

                    //Pega os produtos que atualizam estoque
                    foreach (Produto item in produtos.Where(x=> x.str_estoque.Equals("S")))
                    {
                        estoque.Add(new EstoqueProduto
                        {
                            id_produto = item.id,
                            int_qtd_est = (double)item.int_qtd_est
                        });
                    }

                    if (estoque.Count() > 0)
                    {
                        str_ret = JsonConvert.SerializeObject(estoque);
                    }
                    else
                    {
                        str_ret = "";
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();
                    throw ex;
                }
                conn.Close();
            }

            return str_ret;
        }

        public async Task<string> GravarPrdEstoque(Int64 empresa, string dados)
        {
            string str_ret = "";
            string str_operacao = "";
            Int64 id_produto = 0;
            PrdEstoqueApp produto = new PrdEstoqueApp();
            List<PrdEstoque> produtos = new List<PrdEstoque>();

            produto = JsonConvert.DeserializeObject<PrdEstoqueApp>(dados);

            if (produto != null)
            {
                if (produto.id_server == 0)
                {
                    str_operacao = "I";
                }
                else
                {
                    str_operacao = "U";
                }

                produtos.Add(new PrdEstoque
                {
                    id = produto.id_server,
                    id_empresa = empresa,
                    id_produto = produto.id_produto,
                    int_qtd_est = produto.int_qtd_est,
                    dtm_estoque = produto.dtm_estoque,
                    id_app = produto.id,
                    id_user_man = 1

                });

                string str_conn = configDB.ConnectString;

                using (SqlConnection conn = new SqlConnection(str_conn))
                {
                    conn.Open();

                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            id_produto = Convert.ToInt64(repData.ManutencaoTabela<PrdEstoque>(str_operacao, produtos, "ntv_tbl_produto_estoque", conn, tran).Split(";")[0]);
                            produto.id_server = id_produto;
                            str_ret = JsonConvert.SerializeObject(produto);
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
                    id_localcli = pedido.id_localcli,
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
            DataTable dtt_reg = new DataTable();
            string str_ret = "";
            string str_id = "";
            string str_operacao = "";

            List<PedidoItemApp> itemApp = new List<PedidoItemApp>();
            List<PedidoItemApp> itemApp_ret = new List<PedidoItemApp>();

            List<PedidoItem> itens = new List<PedidoItem>();
            List<PedidoItem> itensUpd = new List<PedidoItem>();
            List<PedidoItem> itensIns = new List<PedidoItem>();

            itemApp = JsonConvert.DeserializeObject<List<PedidoItemApp>>((dados.Contains("[") ? dados : "[" + dados + "]"));


            if (itemApp != null && itemApp.Count > 0)
            {
                string str_conn = configDB.ConnectString;

                using (SqlConnection conn = new SqlConnection(str_conn))
                {
                    conn.Open();
                    for (int i = 0; i < itemApp.Count; i++)
                    {
                        if (itemApp[i].id_server == 0)
                        {
                            //Verifica se o registro já existe
                            dtt_reg = repData.ConsultaGenericaDtt("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                        "{ \"nome\":\"id_empresa\", \"valor\":\"" + empresa.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                        "{ \"nome\":\"dtm_ini\", \"valor\":\"2001-01-01\", \"tipo\":\"DateTime\"}," +
                                                        "{ \"nome\":\"dtm_fim\", \"valor\":\"2001-01-01\", \"tipo\":\"DateTime\"}," +
                                                        "{ \"nome\":\"id_usuario\", \"valor\":\"" + itemApp[i].id_usuario.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                        "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                        "{ \"nome\":\"download\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                        "{ \"nome\":\"id_app\", \"valor\":\"" + itemApp[i].id.ToString() + "\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_pedidoitem", conn);
                            if (dtt_reg == null || dtt_reg.Rows.Count == 0)
                            {
                                itensIns.Add(new PedidoItem
                                {
                                    id = 0,
                                    id_empresa = empresa,
                                    id_pedido = itemApp[i].id_pedido,
                                    id_produto = itemApp[i].id_produto,
                                    dbl_precounit = itemApp[i].dbl_precounit,
                                    int_qtd_comp = itemApp[i].int_qtd_comp,
                                    dbl_tot_item = itemApp[i].dbl_tot_item,
                                    dbl_desconto = itemApp[i].dbl_desconto,
                                    dbl_tot_liq = itemApp[i].dbl_tot_liq,
                                    int_situacao = itemApp[i].int_situacao,
                                    str_combo = itemApp[i].str_combo,
                                    id_usuario = itemApp[i].id_usuario,
                                    id_app = itemApp[i].id,
                                    id_user_man = 0
                                });
                            }
                            else
                            {
                                itensUpd.Add(new PedidoItem
                                {
                                    id = Convert.ToInt64(dtt_reg.Rows[0]["id"]),
                                    id_empresa = empresa,
                                    id_pedido = itemApp[i].id_pedido,
                                    id_produto = itemApp[i].id_produto,
                                    dbl_precounit = itemApp[i].dbl_precounit,
                                    int_qtd_comp = itemApp[i].int_qtd_comp,
                                    dbl_tot_item = itemApp[i].dbl_tot_item,
                                    dbl_desconto = itemApp[i].dbl_desconto,
                                    dbl_tot_liq = itemApp[i].dbl_tot_liq,
                                    int_situacao = itemApp[i].int_situacao, // 0 - Não entregue / 1 - Entregue / 9 - Cancelado
                                    str_combo = itemApp[i].str_combo,
                                    id_usuario = itemApp[i].id_usuario,
                                    id_app = itemApp[i].id,
                                    id_user_man = 0
                                });

                            }
                        }
                        else
                        {
                            itensUpd.Add(new PedidoItem
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
                                str_combo = itemApp[i].str_combo,
                                id_usuario = itemApp[i].id_usuario,
                                id_app = itemApp[i].id,
                                id_user_man = 0
                            });
                        }
                    }

                    if (itensIns.Count > 0 || itensUpd.Count > 0)
                    {
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            try
                            {
                                //Inclusões
                                if (itensIns.Count > 0)
                                {
                                    str_ret = repData.ManutencaoTabela<PedidoItem>("I", itensIns, "ntv_tbl_pedidoitem", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                PedidoItemApp item = (from reg in itemApp where reg.id == itensIns[id].id_app select reg).FirstOrDefault();
                                                item.int_sinc = 1;
                                                item.id_server = Convert.ToInt64(str_ret.Split(";")[id]);
                                                itemApp_ret.Add(item);

                                            }
                                        }
                                    }
                                }

                                //Alterações
                                if (itensUpd.Count() > 0)
                                {
                                    str_ret += repData.ManutencaoTabela<PedidoItem>("U", itensUpd, "ntv_tbl_pedidoitem", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                PedidoItemApp item = (from reg in itemApp where reg.id == itensUpd[id].id_app select reg).FirstOrDefault();
                                                item.int_sinc = 1;
                                                item.id_server = Convert.ToInt64(str_ret.Split(";")[id]);
                                                itemApp_ret.Add(item);
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                throw ex;
                            }
                            tran.Commit();
                        }
                    }
                    conn.Close();

                }


                str_ret = JsonConvert.SerializeObject(itemApp_ret);
            }

            return str_ret;
        }

        public async Task<string> GravarPedidoItemCombo(Int64 empresa, string dados)
        {
            DataTable dtt_reg = new DataTable();
            string str_ret = "";
            string str_id = "";
            string str_operacao = "";

            List<PedidoItemComboApp> itemApp = new List<PedidoItemComboApp>();
            List<PedidoItemComboApp> itemApp_ret = new List<PedidoItemComboApp>();

            List<PedidoItemCombo> itens = new List<PedidoItemCombo>();
            List<PedidoItemCombo> itensUpd = new List<PedidoItemCombo>();
            List<PedidoItemCombo> itensIns = new List<PedidoItemCombo>();

            itemApp = JsonConvert.DeserializeObject<List<PedidoItemComboApp>>((dados.Contains("[") ? dados : "[" + dados + "]"));


            if (itemApp != null && itemApp.Count > 0)
            {
                string str_conn = configDB.ConnectString;

                using (SqlConnection conn = new SqlConnection(str_conn))
                {
                    conn.Open();
                    for (int i = 0; i < itemApp.Count; i++)
                    {
                        if (itemApp[i].id_server == 0)
                        {
                            //Verifica se o registro já existe
                            dtt_reg = repData.ConsultaGenericaDtt("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                        "{ \"nome\":\"id_empresa\", \"valor\":\"" + empresa.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                        "{ \"nome\":\"dtm_ini\", \"valor\":\"2001-01-01\", \"tipo\":\"DateTime\"}," +
                                                        "{ \"nome\":\"dtm_fim\", \"valor\":\"2001-01-01\", \"tipo\":\"DateTime\"}," +
                                                        "{ \"nome\":\"id_usuario\", \"valor\":\"" + itemApp[i].id_usuario.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                        "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                        "{ \"nome\":\"download\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                        "{ \"nome\":\"id_app\", \"valor\":\"" + itemApp[i].id.ToString() + "\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_pedidoitemcombo", conn);
                            if (dtt_reg == null || dtt_reg.Rows.Count == 0)
                            {
                                itensIns.Add(new PedidoItemCombo
                                {
                                    id = 0,
                                    id_empresa = empresa,
                                    id_pedido = itemApp[i].id_pedido,
                                    id_ped_item = itemApp[i].id_ped_item,
                                    id_produto = itemApp[i].id_produto,
                                    id_prod_sel = itemApp[i].id_prod_sel,
                                    int_qtd_comp = itemApp[i].int_qtd_comp,
                                    id_usuario = itemApp[i].id_usuario,
                                    id_app = itemApp[i].id,
                                    id_user_man = 0
                                });
                            }
                            else
                            {
                                itensUpd.Add(new PedidoItemCombo
                                {
                                    id = Convert.ToInt64(dtt_reg.Rows[0]["id"]),
                                    id_empresa = empresa,
                                    id_pedido = itemApp[i].id_pedido,
                                    id_ped_item = itemApp[i].id_ped_item,
                                    id_produto = itemApp[i].id_produto,
                                    id_prod_sel = itemApp[i].id_prod_sel,
                                    int_qtd_comp = itemApp[i].int_qtd_comp,
                                    id_usuario = itemApp[i].id_usuario,
                                    id_app = itemApp[i].id,
                                    id_user_man = 0
                                });

                            }
                        }
                        else
                        {
                            itensUpd.Add(new PedidoItemCombo
                            {
                                id = itemApp[i].id_server,
                                id_empresa = empresa,
                                id_pedido = itemApp[i].id_pedido,
                                id_ped_item = itemApp[i].id_ped_item,
                                id_produto = itemApp[i].id_produto,
                                id_prod_sel = itemApp[i].id_prod_sel,
                                int_qtd_comp = itemApp[i].int_qtd_comp,
                                id_usuario = itemApp[i].id_usuario,
                                id_app = itemApp[i].id,
                                id_user_man = 0
                            });
                        }
                    }

                    if (itensIns.Count > 0 || itensUpd.Count > 0)
                    {
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            try
                            {
                                //Inclusões
                                if (itensIns.Count > 0)
                                {
                                    str_ret = repData.ManutencaoTabela<PedidoItemCombo>("I", itensIns, "ntv_tbl_pedidoitemcombo", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                PedidoItemComboApp item = (from reg in itemApp where reg.id == itensIns[id].id_app select reg).FirstOrDefault();
                                                item.int_sinc = 1;
                                                item.id_server = Convert.ToInt64(str_ret.Split(";")[id]);
                                                itemApp_ret.Add(item);

                                            }
                                        }
                                    }
                                }

                                //Alterações
                                if (itensUpd.Count() > 0)
                                {
                                    str_ret += repData.ManutencaoTabela<PedidoItemCombo>("U", itensUpd, "ntv_tbl_pedidoitemcombo", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                PedidoItemComboApp item = (from reg in itemApp where reg.id == itensUpd[id].id_app select reg).FirstOrDefault();
                                                item.int_sinc = 1;
                                                item.id_server = Convert.ToInt64(str_ret.Split(";")[id]);
                                                itemApp_ret.Add(item);
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                throw ex;
                            }
                            tran.Commit();
                        }
                    }
                    conn.Close();

                }


                str_ret = JsonConvert.SerializeObject(itemApp_ret);
            }

            return str_ret;
        }

        public async Task<string> GravarPedidoItemAdic(Int64 empresa, string dados)
        {
            DataTable dtt_reg = new DataTable();
            string str_ret = "";
            string str_id = "";
            string str_operacao = "";

            List<PedidoItemAdicApp> itemApp = new List<PedidoItemAdicApp>();
            List<PedidoItemAdicApp> itemApp_ret = new List<PedidoItemAdicApp>();

            List<PedidoItemAdic> itens = new List<PedidoItemAdic>();
            List<PedidoItemAdic> itensUpd = new List<PedidoItemAdic>();
            List<PedidoItemAdic> itensIns = new List<PedidoItemAdic>();

            itemApp = JsonConvert.DeserializeObject<List<PedidoItemAdicApp>>((dados.Contains("[") ? dados : "[" + dados + "]"));


            if (itemApp != null && itemApp.Count > 0)
            {
                string str_conn = configDB.ConnectString;

                using (SqlConnection conn = new SqlConnection(str_conn))
                {
                    conn.Open();
                    for (int i = 0; i < itemApp.Count; i++)
                    {
                        if (itemApp[i].id_server == 0)
                        {
                            //Verifica se o registro já existe
                            dtt_reg = repData.ConsultaGenericaDtt("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                        "{ \"nome\":\"id_empresa\", \"valor\":\"" + empresa.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                        "{ \"nome\":\"dtm_ini\", \"valor\":\"2001-01-01\", \"tipo\":\"DateTime\"}," +
                                                        "{ \"nome\":\"dtm_fim\", \"valor\":\"2001-01-01\", \"tipo\":\"DateTime\"}," +
                                                        "{ \"nome\":\"id_usuario\", \"valor\":\"" + itemApp[i].id_usuario.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                        "{ \"nome\":\"download\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                        "{ \"nome\":\"id_app\", \"valor\":\"" + itemApp[i].id.ToString() + "\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_pedidoitemadic", conn);
                            if (dtt_reg == null || dtt_reg.Rows.Count == 0)
                            {
                                itensIns.Add(new PedidoItemAdic
                                {
                                    id = 0,
                                    id_empresa = empresa,
                                    id_pedido = itemApp[i].id_pedido,
                                    id_ped_item = itemApp[i].id_ped_item,
                                    id_produto = itemApp[i].id_produto,
                                    id_prod_adic = itemApp[i].id_prod_adic,
                                    int_qtd_comp = itemApp[i].int_qtd_comp,
                                    dbl_val_unit = itemApp[i].dbl_val_unit,
                                    id_usuario = itemApp[i].id_usuario,
                                    id_app = itemApp[i].id,
                                    id_user_man = 0
                                });
                            }
                            else
                            {
                                itensUpd.Add(new PedidoItemAdic
                                {
                                    id = Convert.ToInt64(dtt_reg.Rows[0]["id"]),
                                    id_empresa = empresa,
                                    id_pedido = itemApp[i].id_pedido,
                                    id_ped_item = itemApp[i].id_ped_item,
                                    id_produto = itemApp[i].id_produto,
                                    id_prod_adic = itemApp[i].id_prod_adic,
                                    int_qtd_comp = itemApp[i].int_qtd_comp,
                                    dbl_val_unit = itemApp[i].dbl_val_unit,
                                    id_usuario = itemApp[i].id_usuario,
                                    id_app = itemApp[i].id,
                                    id_user_man = 0
                                });

                            }
                        }
                        else
                        {
                            itensUpd.Add(new PedidoItemAdic
                            {
                                id = itemApp[i].id_server,
                                id_empresa = empresa,
                                id_pedido = itemApp[i].id_pedido,
                                id_ped_item = itemApp[i].id_ped_item,
                                id_produto = itemApp[i].id_produto,
                                id_prod_adic = itemApp[i].id_prod_adic,
                                int_qtd_comp = itemApp[i].int_qtd_comp,
                                dbl_val_unit = itemApp[i].dbl_val_unit,
                                id_usuario = itemApp[i].id_usuario,
                                id_app = itemApp[i].id,
                                id_user_man = 0
                            });
                        }
                    }

                    if (itensIns.Count > 0 || itensUpd.Count > 0)
                    {
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            try
                            {
                                //Inclusões
                                if (itensIns.Count > 0)
                                {
                                    str_ret = repData.ManutencaoTabela<PedidoItemAdic>("I", itensIns, "ntv_tbl_pedidoitemadic", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                PedidoItemAdicApp item = (from reg in itemApp where reg.id == itensIns[id].id_app select reg).FirstOrDefault();
                                                item.int_sinc = 1;
                                                item.id_server = Convert.ToInt64(str_ret.Split(";")[id]);
                                                itemApp_ret.Add(item);

                                            }
                                        }
                                    }
                                }

                                //Alterações
                                if (itensUpd.Count() > 0)
                                {
                                    str_ret += repData.ManutencaoTabela<PedidoItemAdic>("U", itensUpd, "ntv_tbl_pedidoitemadic", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                PedidoItemAdicApp item = (from reg in itemApp where reg.id == itensUpd[id].id_app select reg).FirstOrDefault();
                                                item.int_sinc = 1;
                                                item.id_server = Convert.ToInt64(str_ret.Split(";")[id]);
                                                itemApp_ret.Add(item);
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                throw ex;
                            }
                            tran.Commit();
                        }
                    }
                    conn.Close();

                }


                str_ret = JsonConvert.SerializeObject(itemApp_ret);
            }

            return str_ret;
        }

        public async Task<string> GravarEntrada(Int64 empresa, string dados)
        {
            string str_ret = "";
            string str_operacao = "";
            Int64 id_entrada = 0;
            EntradaApp entrada = new EntradaApp();
            List<Entrada> entradas = new List<Entrada>();

            entrada = JsonConvert.DeserializeObject<EntradaApp>(dados);

            if (entrada != null)
            {
                if (entrada.id_server == 0)
                {
                    str_operacao = "I";
                }
                else
                {
                    str_operacao = "U";
                }

                entradas.Add(new Entrada
                {
                    id = entrada.id_server,
                    id_empresa = empresa,
                    dtm_entrada = entrada.dtm_entrada,
                    int_nota = entrada.int_nota,
                    int_fornecedor = entrada.int_fornecedor,
                    str_serie = entrada.str_serie,
                    id_tipo = entrada.id_tipo,
                    dtm_nota = entrada.dtm_nota,
                    dbl_tot_nf = entrada.dbl_tot_nf,
                    int_qtd_item = entrada.int_qtd_item,
                    dbl_tot_item = entrada.dbl_tot_item,
                    dbl_tot_desc = entrada.dbl_tot_desc,
                    dbl_tot_liq = entrada.dbl_tot_liq,
                    str_obs = entrada.str_obs,
                    int_situacao = entrada.int_situacao, // 0 - Digitada / 1 - Conferida / 9 - Cancelado
                    id_usuario = entrada.id_usuario,
                    id_app = entrada.id,
                    id_user_man = entrada.id_usuario
                });

                string str_conn = configDB.ConnectString;

                using (SqlConnection conn = new SqlConnection(str_conn))
                {
                    conn.Open();

                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            id_entrada = Convert.ToInt64(repData.ManutencaoTabela<Entrada>(str_operacao, entradas, "ntv_tbl_entrada", conn, tran).Split(";")[0]);
                            entrada.id_server = id_entrada;
                            str_ret = JsonConvert.SerializeObject(entrada);
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

        public async Task<string> GravarEntradaItem(Int64 empresa, string dados_ent, string dados)
        {
            DataTable dtt_reg = new DataTable();
            string str_ret = "";
            string str_id = "";
            string str_operacao = "";

            List<EntradaItemApp> itemApp = new List<EntradaItemApp>();
            List<EntradaItemApp> itemApp_ret = new List<EntradaItemApp>();
            EntradaApp entrada = new EntradaApp();

            entrada = JsonConvert.DeserializeObject<EntradaApp>(dados_ent);

            List<EntradaItem> itens = new List<EntradaItem>();
            List<EntradaItem> itensUpd = new List<EntradaItem>();
            List<EntradaItem> itensIns = new List<EntradaItem>();

            itemApp = JsonConvert.DeserializeObject<List<EntradaItemApp>>((dados.Contains("[") ? dados : "[" + dados + "]"));

            double dbl_qtd_estoque = 0;

            if (itemApp != null && itemApp.Count > 0)
            {
                string str_conn = configDB.ConnectString;

                using (SqlConnection conn = new SqlConnection(str_conn))
                {
                    conn.Open();
                    for (int i = 0; i < itemApp.Count; i++)
                    {
                        if (itemApp[i].id_server == 0)
                        {
                            //Verifica se o registro já existe
                            dtt_reg = repData.ConsultaGenericaDtt("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                        "{ \"nome\":\"id_empresa\", \"valor\":\"" + empresa.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                        "{ \"nome\":\"dtm_ini\", \"valor\":\"2001-01-01\", \"tipo\":\"DateTime\"}," +
                                                        "{ \"nome\":\"dtm_fim\", \"valor\":\"2001-01-01\", \"tipo\":\"DateTime\"}," +
                                                        "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                        "{ \"nome\":\"download\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                        "{ \"nome\":\"id_app\", \"valor\":\"" + itemApp[i].id.ToString() + "\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_entrada_item", conn);
                            if (dtt_reg == null || dtt_reg.Rows.Count == 0)
                            {
                                itensIns.Add(new EntradaItem
                                {
                                    id = 0,
                                    id_empresa = empresa,
                                    id_entrada = itemApp[i].id_entrada,
                                    id_produto = itemApp[i].id_produto,
                                    dbl_precounit = itemApp[i].dbl_precounit,
                                    int_qtd_comp = itemApp[i].int_qtd_comp,
                                    dbl_tot_item = itemApp[i].dbl_tot_item,
                                    dbl_desconto = itemApp[i].dbl_desconto,
                                    dbl_tot_liq = itemApp[i].dbl_tot_liq,
                                    int_situacao = itemApp[i].int_situacao,
                                    id_app = itemApp[i].id,
                                    id_user_man = 0
                                });
                            }
                            else
                            {
                                itensUpd.Add(new EntradaItem
                                {
                                    id = Convert.ToInt64(dtt_reg.Rows[0]["id"]),
                                    id_empresa = empresa,
                                    id_entrada = itemApp[i].id_entrada,
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
                        }
                        else
                        {
                            itensUpd.Add(new EntradaItem
                            {
                                id = itemApp[i].id_server,
                                id_empresa = empresa,
                                id_entrada = itemApp[i].id_entrada,
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
                    }

                    if (itensIns.Count > 0 || itensUpd.Count > 0)
                    {
                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            try
                            {
                                //Inclusões
                                if (itensIns.Count > 0)
                                {
                                    str_ret = repData.ManutencaoTabela<EntradaItem>("I", itensIns, "ntv_tbl_entrada_item", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                EntradaItemApp item = (from reg in itemApp where reg.id == itensIns[id].id_app select reg).FirstOrDefault();
                                                item.int_sinc = 1;
                                                item.id_server = Convert.ToInt64(str_ret.Split(";")[id]);
                                                itemApp_ret.Add(item);

                                                //Atualiza estoque do produto
                                                /*
                                                dbl_qtd_estoque = (entrada.id_tipo == 1 ? item.int_qtd_comp : item.int_qtd_comp * -1);
                                                bool bol_estoque = repData.SqlUpdate("[{ \"nome\":\"id\", \"valor\":\"" + item.id_produto.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                                     "{ \"nome\":\"id_empresa\", \"valor\":\"" + empresa.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                                     "{ \"nome\":\"int_qtd_est\", \"valor\":\"" + dbl_qtd_estoque.ToString() + "\", \"tipo\":\"Int64\"}]",
                                                                                     "ntv_p_atu_estoque_produto", conn, tran);
                                                */
                                            }
                                        }
                                    }
                                }

                                //Alterações
                                if (itensUpd.Count() > 0)
                                {
                                    str_ret += repData.ManutencaoTabela<EntradaItem>("U", itensUpd, "ntv_tbl_entrada_item", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                EntradaItemApp item = (from reg in itemApp where reg.id == itensUpd[id].id_app select reg).FirstOrDefault();
                                                item.int_sinc = 1;
                                                item.id_server = Convert.ToInt64(str_ret.Split(";")[id]);
                                                itemApp_ret.Add(item);
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                throw ex;
                            }
                            tran.Commit();
                        }
                    }
                    conn.Close();

                }


                str_ret = JsonConvert.SerializeObject(itemApp_ret);
            }

            return str_ret;
        }

        public async Task<string> GravarLocal(Int64 empresa, string dados)
        {
            string str_ret = "";
            string str_id = "";
            string str_operacao = "";
            Int64 id_local = 0;

            List<LocalApp> itemApp = new List<LocalApp>();
            List<Local> itens = new List<Local>();

            List<Local> locInc = new List<Local>();
            List<Local> locUpd = new List<Local>();

            DataTable dtt_reg = new DataTable();

            itemApp = JsonConvert.DeserializeObject<List<LocalApp>>((dados.Contains("[") ? dados : "[" + dados + "]"));

            if (itemApp != null && itemApp.Count > 0)
            {
                string str_conn = configDB.ConnectString;

                using (SqlConnection conn = new SqlConnection(str_conn))
                {
                    conn.Open();

                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            for (int i = 0; i < itemApp.Count; i++)
                            {

                                if (itemApp[i].id_server == 0)
                                {
                                    dtt_reg = repData.ConsultaGenericaDtt("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                        "{ \"nome\":\"id_empresa\", \"valor\":\"" + empresa.ToString() + "\", \"tipo\":\"Int64\"}," +
                                        "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                        "{ \"nome\":\"download\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                        "{ \"nome\":\"id_app\", \"valor\":\"" + itemApp[i].id.ToString() + "\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_local", conn, tran);
                                    if (dtt_reg == null || dtt_reg.Rows.Count == 0)
                                    {
                                        locInc.Add(new Local
                                        {
                                            id = 0,
                                            id_empresa = empresa,
                                            int_numero = itemApp[i].int_numero,
                                            int_tipo = itemApp[i].int_tipo,
                                            int_qtd_pess = itemApp[i].int_qtd_pess,
                                            str_foto = itemApp[i].str_foto,
                                            dtm_inclusao = itemApp[i].dtm_inclusao,
                                            dtm_alteracao = DateTime.Now,
                                            int_situacao = itemApp[i].int_situacao,
                                            id_app = itemApp[i].id,
                                            id_user_man = itemApp[i].id_usuario
                                        });
                                    }
                                    else
                                    {
                                        locUpd.Add(new Local
                                        {
                                            id = Convert.ToInt64(dtt_reg.Rows[0]["id"]),
                                            id_empresa = empresa,
                                            int_numero = itemApp[i].int_numero,
                                            int_tipo = itemApp[i].int_tipo,
                                            int_qtd_pess = itemApp[i].int_qtd_pess,
                                            str_foto = itemApp[i].str_foto,
                                            dtm_inclusao = itemApp[i].dtm_inclusao,
                                            dtm_alteracao = DateTime.Now,
                                            int_situacao = itemApp[i].int_situacao,
                                            id_app = itemApp[i].id,
                                            id_user_man = itemApp[i].id_usuario
                                        });

                                    }
                                }
                                else
                                {
                                    locUpd.Add(new Local
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
                                        id_user_man = itemApp[i].id_usuario
                                    });

                                }

                            }
                            //Inclusões
                            if (locInc.Count > 0)
                            {
                                id_local = Convert.ToInt64(repData.ManutencaoTabela<Local>("I", locInc, "ntv_tbl_local", conn, tran).Split(";")[0]);

                                itemApp[0].id_server = id_local;
                            }

                            //Alterações
                            if (locUpd.Count() > 0)
                            {
                                str_ret += repData.ManutencaoTabela<Local>("U", locUpd, "ntv_tbl_local", conn, tran);
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

            return str_ret;
        }

        public async Task<string> GravarLocalCli(Int64 empresa, string dados)
        {
            string str_ret = "";
            string str_id = "";
            string str_operacao = "";
            Int64 id_localcli = 0;

            DataTable dtt_reg = new DataTable();

            List<LocalClienteApp> itemApp = new List<LocalClienteApp>();
            List<LocalCliente> itens = new List<LocalCliente>();

            List<LocalCliente> locCliInc = new List<LocalCliente>();
            List<LocalCliente> locCliUpd = new List<LocalCliente>();

            itemApp = JsonConvert.DeserializeObject<List<LocalClienteApp>>((dados.Contains("[") ? dados : "[" + dados + "]"));

            if (itemApp != null && itemApp.Count > 0)
            {


                string str_conn = configDB.ConnectString;

                using (SqlConnection conn = new SqlConnection(str_conn))
                {
                    conn.Open();

                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {

                            for (int i = 0; i < itemApp.Count; i++)
                            {
                                dtt_reg.Clear();

                                if (itemApp[i].id_server == 0)
                                {
                                    //Verifica se o registro já existe
                                    dtt_reg = repData.ConsultaGenericaDtt("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                                "{ \"nome\":\"id_empresa\", \"valor\":\"" + empresa.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                "{ \"nome\":\"download\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                "{ \"nome\":\"id_usuario\", \"valor\":\"" + itemApp[i].id_usuario.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                "{ \"nome\":\"id_app\", \"valor\":\"" + itemApp[i].id.ToString() + "\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_localcliente", conn, tran);
                                    if (dtt_reg == null || dtt_reg.Rows.Count == 0)
                                    {
                                        locCliInc.Add(new LocalCliente
                                        {
                                            id = 0,
                                            id_empresa = empresa,
                                            id_local = itemApp[i].id_local,
                                            str_cliente = itemApp[i].str_cliente,
                                            str_senha = itemApp[i].str_senha,
                                            int_qtdped = itemApp[i].int_qtdped,
                                            int_qtditem = itemApp[i].int_qtditem,
                                            dbl_val_tot = itemApp[i].dbl_val_tot,
                                            dbl_val_desc = itemApp[i].dbl_val_desc,
                                            dbl_val_acres = itemApp[i].dbl_val_acres,
                                            dbl_val_taxa = itemApp[i].dbl_val_taxa,
                                            dbl_val_liq = itemApp[i].dbl_val_liq,
                                            dbl_val_pag = itemApp[i].dbl_val_pag,
                                            dtm_inclusao = itemApp[i].dtm_inclusao,
                                            dtm_pagto = itemApp[i].dtm_pagto,
                                            dtm_cancel = itemApp[i].dtm_cancel,
                                            int_situacao = itemApp[i].int_situacao,
                                            id_usuario = itemApp[i].id_usuario,
                                            id_app = itemApp[i].id,
                                            id_user_man = 0

                                        });
                                    }
                                    else
                                    {
                                        locCliUpd.Add(new LocalCliente
                                        {
                                            id = Convert.ToInt64(dtt_reg.Rows[0]["id"]),
                                            id_empresa = empresa,
                                            id_local = itemApp[i].id_local,
                                            str_cliente = itemApp[i].str_cliente,
                                            str_senha = itemApp[i].str_senha,
                                            int_qtdped = itemApp[i].int_qtdped,
                                            int_qtditem = itemApp[i].int_qtditem,
                                            dbl_val_tot = itemApp[i].dbl_val_tot,
                                            dbl_val_desc = itemApp[i].dbl_val_desc,
                                            dbl_val_acres = itemApp[i].dbl_val_acres,
                                            dbl_val_taxa = itemApp[i].dbl_val_taxa,
                                            dbl_val_liq = itemApp[i].dbl_val_liq,
                                            dbl_val_pag = itemApp[i].dbl_val_pag,
                                            dtm_inclusao = itemApp[i].dtm_inclusao,
                                            dtm_pagto = itemApp[i].dtm_pagto,
                                            dtm_cancel = itemApp[i].dtm_cancel,
                                            int_situacao = itemApp[i].int_situacao,
                                            id_usuario = itemApp[i].id_usuario,
                                            id_app = itemApp[i].id,
                                            id_user_man = 0
                                        });

                                    }
                                }
                                else
                                {
                                    locCliUpd.Add(new LocalCliente
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
                                        dbl_val_acres = itemApp[i].dbl_val_acres,
                                        dbl_val_taxa = itemApp[i].dbl_val_taxa,
                                        dbl_val_liq = itemApp[i].dbl_val_liq,
                                        dbl_val_pag = itemApp[i].dbl_val_pag,
                                        dtm_inclusao = itemApp[i].dtm_inclusao,
                                        dtm_pagto = itemApp[i].dtm_pagto,
                                        dtm_cancel = itemApp[i].dtm_cancel,
                                        int_situacao = itemApp[i].int_situacao,
                                        id_usuario = itemApp[i].id_usuario,
                                        id_app = itemApp[i].id,
                                        id_user_man = 0
                                    });

                                }

                            }

                            //Inclusões
                            if (locCliInc.Count > 0)
                            {
                                id_localcli = Convert.ToInt64(repData.ManutencaoTabela<LocalCliente>("I", locCliInc, "ntv_tbl_localcliente", conn, tran).Split(";")[0]);
                                itemApp[0].id_server = id_localcli;
                            }

                            //Alterações
                            if (locCliUpd.Count() > 0)
                            {
                                str_ret += repData.ManutencaoTabela<LocalCliente>("U", locCliUpd, "ntv_tbl_localcliente", conn, tran);
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
            return str_ret;
        }
        public string GravarNotificacoes(Int64 empresa, Int64 usu_orig, Int64 usu_dest, string tabela, string dados)
        {
            string str_notific = "";
            DataTable dtt_entity = JsonConvert.DeserializeObject<DataTable>((dados.IndexOf("[") > -1 ? dados : "[" + dados + "]"));
            Notificacoes notific = new Notificacoes();

            if (dtt_entity != null && dtt_entity.Rows.Count > 0)
            {
                for (int i = 0; i < dtt_entity.Rows.Count; i++)
                {
                    notific.id = 0;
                    notific.id_empresa = empresa;
                    notific.id_usu_orig = usu_orig;
                    notific.id_usu_dest = usu_dest;
                    notific.str_tabela = tabela;
                    notific.id_registro = Convert.ToInt64(dtt_entity.Rows[i]["id_server"]);
                    notific.dtm_inclusao = DateTime.Now;
                    notific.int_situacao = 0; //0 - Não notificado / 1 - Notificado

                    str_notific = _repNotif.GravarNotificacoes(JsonConvert.SerializeObject(notific));
                }
            }

            return str_notific;
        }
        public string GravarNotificacoes(Int64 empresa, Int64 usu_orig, string dados)
        {
            string str_notific = "";
            List<Notificacoes> lst_notific = new List<Notificacoes>();
            lst_notific = JsonConvert.DeserializeObject<List<Notificacoes>>(dados);
            Notificacoes notific = new Notificacoes();

            if (lst_notific != null || lst_notific.Count > 0)
            {
                for (int i = 0; i < lst_notific.Count; i++)
                {
                    notific.id = lst_notific[i].id;
                    notific.id_empresa = lst_notific[i].id_empresa;
                    notific.id_usu_orig = lst_notific[i].id_usu_orig;
                    notific.id_usu_dest = lst_notific[i].id_usu_dest;
                    notific.str_tabela = lst_notific[i].str_tabela;
                    notific.id_registro = lst_notific[i].id_registro;
                    notific.dtm_inclusao = lst_notific[i].dtm_inclusao;
                    notific.int_situacao = lst_notific[i].int_situacao; //0 - Não notificado / 1 - Notificado

                    str_notific = _repNotif.GravarNotificacoes(JsonConvert.SerializeObject(notific));
                }
            }

            return str_notific;
        }
    }
}
