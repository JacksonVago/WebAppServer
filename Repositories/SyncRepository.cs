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
    public class SyncRepository
    {
        private readonly ConfigDB configDB;

        public SyncRepository()
        {            
            configDB = new ConfigDB();
        }

        public async Task<List<DadosDownload>> SyncDataDownload(string filtros)
        {
            string str_filtros_aux = filtros.Replace("||", "'");
            DataTable dtt_entity = new DataTable();

            List<DadosDownload> dataDown = new List<DadosDownload>();

            DataRepository repData = new DataRepository();

            string str_tabela = "";
            string str_tabela_aux = "";
            string str_filtros = "";
            string str_ret = "";

            using (SqlConnection conn = new SqlConnection(configDB.ConnectString))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        dtt_entity = repData.ConsultaGenericaDtt(str_filtros_aux.Replace("##", "\""), "ntv_p_sel_tbl_sync_dados", conn, tran);

                        if (dtt_entity.Rows.Count > 0)
                        {
                            
                            for (int i = 0; i < dtt_entity.Rows.Count; i++)
                            {
                                if (str_tabela_aux != dtt_entity.Rows[i]["str_tabela"].ToString())
                                {
                                    str_filtros = "[";
                                    str_filtros += "{ \"nome\":\"id_empresa\", \"valor\":\"" + dtt_entity.Rows[i]["id_empresa"].ToString() + "\", \"tipo\":\"Int64\"},";
                                    str_filtros += "{ \"nome\":\"id_user\", \"valor\":\"" + dtt_entity.Rows[i]["id_usuario_dest"].ToString() + "\", \"tipo\":\"Int64\"},";
                                    str_filtros += "{ \"nome\":\"tabela\", \"valor\":\"" + dtt_entity.Rows[i]["str_tabela"].ToString() + "\", \"tipo\":\"String\"}]";

                                    switch (dtt_entity.Rows[i]["str_tabela"].ToString())
                                    {
                                        case "ntv_tbl_empresa":
                                            str_tabela = "Empresa";
                                            break;

                                        case "ntv_tbl_formapag":
                                            str_tabela = "FormaPag";
                                            break;

                                        case "ntv_tbl_grupo":
                                            str_tabela = "Grupo";
                                            break;

                                        case "ntv_tbl_itemcombo":
                                            str_tabela = "ItemCombo";
                                            break;

                                        case "ntv_tbl_local":
                                            str_tabela = "Local";
                                            break;

                                        case "ntv_tbl_localcliente":
                                            str_tabela = "LocalCliente";
                                            break;

                                        case "ntv_tbl_localclipag":
                                            str_tabela = "LocalCliPag";
                                            break;

                                        case "ntv_tbl_pedido":
                                            str_tabela = "Pedido";
                                            break;

                                        case "ntv_tbl_pedidoitem":
                                            str_tabela = "PedidoItem";
                                            break;

                                        case "ntv_tbl_pedidoitemcombo":
                                            str_tabela = "PedidoItemCombo";
                                            break;

                                        case "ntv_tbl_produto":
                                            str_tabela = "Produto";
                                            break;

                                        case "ntv_tbl_produto_lista":
                                            str_tabela = "ProdutoLista";
                                            break;

                                        case "ntv_tbl_sala":
                                            str_tabela = "Sala";
                                            break;

                                        case "ntv_tbl_sala_user":
                                            str_tabela = "SalaUser";
                                            break;

                                        case "ntv_tbl_sala_user_msg":
                                            str_tabela = "SlaUserMsg";
                                            break;

                                        case "ntv_tbl_usuario":
                                            str_tabela = "Usuario";
                                            break;

                                        case "UsuarioHub":
                                            str_tabela = "";
                                            break;

                                        case "ntv_tbl_entrada":
                                            str_tabela = "Entrada";
                                            break;

                                        case "ntv_tbl_entrada_item":
                                            str_tabela = "EntradaItem";
                                            break;

                                        default:
                                            str_tabela = "";
                                            break;

                                    }
                                    str_tabela_aux = dtt_entity.Rows[i]["str_tabela"].ToString();

                                    //Executconsulta
                                    if (str_tabela.Length > 0)
                                    {
                                        str_ret = repData.ConsultaGenerica(str_filtros, "ntv_p_sel_dados_download", conn, tran);
                                        dataDown.Add(new DadosDownload
                                        {
                                            entity = str_tabela,
                                            data = str_ret
                                        });
                                    }
                                }

                            }
                        }
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        conn.Close();
                        throw ex;
                    }
                }
                conn.Close();
            }

            return dataDown;
        }


        public async Task<List<DadosDownload>> SyncDataDownloadRecovery(string filtros)
        {
            DataTable dtt_entity = new DataTable();
            List<DadosDownload> dataDown = new List<DadosDownload>();

            List<DadosDownloadRecovery> dataDownRec = new List<DadosDownloadRecovery>();

            try
            {
                dataDownRec = JsonConvert.DeserializeObject<List<DadosDownloadRecovery>>(filtros.Replace("||", "'"));
            }
            catch(Exception e)
            {
                string str_err = e.Message.ToString();
                throw e;
            }
            
            DataRepository repData = new DataRepository();

            string str_proc = "";
            string str_tabela_aux = "";
            string str_filtros = "";
            string str_ret = "";

            using (SqlConnection conn = new SqlConnection(configDB.ConnectString))
            {
                conn.Open();
                try
                {
                    for (int i = 0; i < dataDownRec.Count; i++)
                    {
                        switch (dataDownRec[i].entity)
                        {
                            case "Empresa":
                                str_proc = "ntv_p_sel_tbl_empresa";
                                break;

                            case "FormaPag":
                                str_proc = "ntv_p_sel_tbl_formapag";
                                break;

                            case "Grupo":
                                str_proc = "ntv_p_sel_tbl_grupo";
                                break;

                            case "ItemCombo":
                                str_proc = "ntv_p_sel_tbl_itemcombo";
                                break;

                            case "Local":
                                str_proc = "ntv_p_sel_tbl_local";
                                break;

                            case "LocalCliente":
                                str_proc = "ntv_p_sel_tbl_localcliente";
                                break;

                            case "LocalCliPag":
                                str_proc = "ntv_p_sel_tbl_localclipag";
                                break;

                            case "Pedido":
                                str_proc = "ntv_p_sel_tbl_pedido";
                                break;

                            case "PedidoItem":
                                str_proc = "ntv_p_sel_tbl_pedidoitem";
                                break;

                            case "PedidoItemCombo":
                                str_proc = "ntv_p_sel_tbl_pedidoitemcombo";
                                break;

                            case "Produto":
                                str_proc = "ntv_p_sel_tbl_produto";
                                break;

                            case "ProdutoLista":
                                str_proc = "ntv_p_sel_tbl_produto_lista";
                                break;

                            case "Sala":
                                str_proc = "ntv_p_sel_tbl_sala";
                                break;

                            case "SalaUser":
                                str_proc = "ntv_p_sel_tbl_sala_user";
                                break;

                            case "SalaUserMsg":
                                str_proc = "ntv_p_sel_tbl_sala_user_msg";
                                break;

                            case "Usuario":
                                str_proc = "ntv_p_sel_tbl_usuario";
                                break;

                            case "UsuarioHub":
                                str_proc = "";
                                break;

                            case "Entrada":
                                str_proc = "ntv_p_sel_tbl_entrada";
                                break;

                            case "EntradaItem":
                                str_proc = "ntv_p_sel_tbl_entrada_item";
                                break;

                            default:
                                str_proc = "";
                                break;

                        }

                        //Executconsulta
                        if (str_proc.Length > 0)
                        {
                            str_ret = repData.ConsultaGenerica(dataDownRec[i].filtro.Replace("##","\""), str_proc, conn);
                            dataDown.Add(new DadosDownload
                            {
                                entity = dataDownRec[i].entity,
                                data = str_ret
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();
                    throw ex;
                }
                conn.Close();
            }

            return dataDown;
        }


        public async Task<string> UpdDataDownload(string filtros)
        {
            DataTable dtt_entity = new DataTable();

            List<DadosDownload> dataDown = new List<DadosDownload>();

            DataRepository repData = new DataRepository();

            string str_tabela = "";
            string str_tabela_aux = "";
            string str_filtros = "";
            string str_ret = "";

            using (SqlConnection conn = new SqlConnection(configDB.ConnectString))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        dtt_entity = repData.ConsultaGenericaDtt(filtros, "ntv_p_sel_tbl_sync_dados", conn, tran);

                        if (dtt_entity.Rows.Count > 0)
                        {
                            List<Syncdados> dados = new List<Syncdados>();

                            for (int i = 0; i < dtt_entity.Rows.Count; i++)
                            {                                
                                dados.Add(new Syncdados
                                {

                                    id = Convert.ToInt64(dtt_entity.Rows[i]["id"]),
                                    id_empresa = Convert.ToInt64(dtt_entity.Rows[i]["id_empresa"]),
                                    id_usuario = Convert.ToInt64(dtt_entity.Rows[i]["id_usuario"]),
                                    id_usuario_dest = Convert.ToInt64(dtt_entity.Rows[i]["id_usuario_dest"]),
                                    str_tabela = dtt_entity.Rows[i]["str_tabela"].ToString(),
                                    id_registro = Convert.ToInt64(dtt_entity.Rows[i]["id_registro"]),
                                    dtm_inclusao = Convert.ToDateTime(dtt_entity.Rows[i]["dtm_inclusao"]),
                                    dtm_sync = DateTime.Now,
                                    int_situacao = 1

                                });                                
                            }

                            str_ret = repData.ManutencaoTabela<Syncdados>("U", dados, "ntv_tbl_sync_dados", conn, tran);
                        }
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        conn.Close();
                        throw ex;
                    }
                }
                conn.Close();
            }

            return str_ret;
        }

        public async Task<dynamic> SychronizeData(Int64 company, Int64 user, string entity, DadosSync data)
        {
            DataRepository repData = new DataRepository();

            Int64 id_emp = 0;
            string str_operacao = "";
            string str_ret = "";
            string str_ret_fim = "";
            string str_ret_aux = "";
            dynamic dyn_ret = null;

            DataTable dtt_reg = new DataTable();

            List<EmpresaApp> empApp = new List<EmpresaApp>();
            //List<Empresa> empU = new List<Empresa>();
            //List<Empresa> empI = new List<Empresa>();            

            using (SqlConnection conn = new SqlConnection(configDB.ConnectString))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        switch (entity)
                        {
                            case "Empresa":
                                empApp = JsonConvert.DeserializeObject<List<EmpresaApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));

                                str_operacao = (empApp[0].id_server > 0 ? "U":"I");

                                List<Empresa> emp = new List<Empresa>();

                                foreach (EmpresaApp e in empApp)
                                {
                                    emp.Add(new Empresa {
                                        id = e.id_server,
                                        int_cgccpf = e.int_cgccpf,
                                        str_nome = e.str_nome,
                                        str_fantasia = e.str_fantasia,
                                        str_email = e.str_email,
                                        int_telefone = e.int_telefone,
                                        int_local_atend = e.int_local_atend,
                                        int_id_user_adm = e.int_id_user_adm,
                                        dtm_inclusao = e.dtm_inclusao,
                                        int_situacao = e.int_situacao,
                                        int_sitpag = e.int_sitpag,
                                        dtm_ultpag = e.dtm_ultpag,
                                        id_app = e.id,
                                        id_user_man = user
                                    });
                                }
                                str_ret = repData.ManutencaoTabela<Empresa>(str_operacao, emp, "ntv_tbl_empresa", conn, tran);
                                dyn_ret = str_ret;
                                break;

                            case "Usuario":
                                List<UsuarioApp> usuApp = new List<UsuarioApp>();
                                usuApp = JsonConvert.DeserializeObject<List<UsuarioApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));
                                //var usuI = from n in usuApp where n.id_server == 0 select n;
                                //var usuU = from n in usuApp where n.id_server > 0 select n;

                                List<Usuario> usuInc = new List<Usuario>();
                                List<Usuario> usuUpd = new List<Usuario>();

                                for (int i = 0; i < usuApp.Count; i++) {

                                    
                                    dtt_reg.Clear();

                                    if (usuApp[i].id_server == 0)
                                    {
                                        //Verifica se o registro já existe
                                        dtt_reg = repData.ConsultaGenericaDtt("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"id_empresa\", \"valor\":\"" + company.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"login\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"email\", \"valor\":\"\", \"tipo\":\"string\"}," +
                                                                    "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"download\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"id_app\", \"valor\":\"" + usuApp[i].id.ToString() + "\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_usuario", conn, tran);
                                        if (dtt_reg == null || dtt_reg.Rows.Count == 0)
                                        {
                                            usuInc.Add(new Usuario
                                            {
                                                id = 0,
                                                id_empresa = company,
                                                str_nome = usuApp[i].str_nome,
                                                str_login = usuApp[i].str_login,
                                                str_senha = usuApp[i].str_senha,
                                                str_email = usuApp[i].str_email,
                                                int_telefone = usuApp[i].int_telefone,
                                                int_tipo = usuApp[i].int_tipo,
                                                dtm_inclusao = usuApp[i].dtm_inclusao,
                                                dtm_saida = usuApp[i].dtm_saida,
                                                int_situacao = usuApp[i].int_situacao,
                                                str_foto = usuApp[i].str_foto,
                                                id_app = usuApp[i].id,
                                                id_user_man = user
                                            });
                                        }
                                        else
                                        {
                                            usuUpd.Add(new Usuario
                                            {
                                                id = Convert.ToInt64(dtt_reg.Rows[0]["id"]),
                                                id_empresa = company,
                                                str_nome = usuApp[i].str_nome,
                                                str_login = usuApp[i].str_login,
                                                str_senha = usuApp[i].str_senha,
                                                str_email = usuApp[i].str_email,
                                                int_telefone = usuApp[i].int_telefone,
                                                int_tipo = usuApp[i].int_tipo,
                                                dtm_inclusao = usuApp[i].dtm_inclusao,
                                                dtm_saida = usuApp[i].dtm_saida,
                                                int_situacao = usuApp[i].int_situacao,
                                                str_foto = usuApp[i].str_foto,
                                                id_app = usuApp[i].id,
                                                id_user_man = user
                                            });

                                        }
                                    }
                                    else
                                    {
                                        usuUpd.Add(new Usuario
                                        {
                                            id = usuApp[i].id_server,
                                            id_empresa = company,
                                            str_nome = usuApp[i].str_nome,
                                            str_login = usuApp[i].str_login,
                                            str_senha = usuApp[i].str_senha,
                                            str_email = usuApp[i].str_email,
                                            int_telefone = usuApp[i].int_telefone,
                                            int_tipo = usuApp[i].int_tipo,
                                            dtm_inclusao = usuApp[i].dtm_inclusao,
                                            dtm_saida = usuApp[i].dtm_saida,
                                            int_situacao = usuApp[i].int_situacao,
                                            str_foto = usuApp[i].str_foto,
                                            id_app = usuApp[i].id,
                                            id_user_man = user
                                        });

                                    }
                                }

                                //Inclusões
                                if (usuInc.Count > 0)
                                {
                                    str_ret = repData.ManutencaoTabela<Usuario>("I", usuInc, "ntv_tbl_usuario", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += usuInc[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }

                                //Alterações
                                if (usuUpd.Count() > 0)
                                {
                                    str_ret_aux = "";
                                    str_ret = repData.ManutencaoTabela<Usuario>("U", usuUpd, "ntv_tbl_usuario", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += usuUpd[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }
                                dyn_ret = str_ret_fim;

                                break;

                            case "Produto":
                                List<ProdutoApp> prdApp = new List<ProdutoApp>();
                                prdApp = JsonConvert.DeserializeObject<List<ProdutoApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));
                                //var regI = from n in prdApp where n.id_server == 0 select n;
                                //var regU = from n in prdApp where n.id_server > 0 select n;

                                List<Produto> prdInc = new List<Produto>();
                                List<Produto> prdUpd = new List<Produto>();

                                for (int i = 0; i < prdApp.Count; i++)
                                {

                                    dtt_reg.Clear();

                                    if (prdApp[i].id_server == 0)
                                    {
                                        //Verifica se o registro já existe
                                        dtt_reg = repData.ConsultaGenericaDtt("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                                "{ \"nome\":\"id_empresa\", \"valor\":\"" + company.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                "{ \"nome\":\"download\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                "{ \"nome\":\"id_app\", \"valor\":\"" + prdApp[i].id.ToString() + "\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_produto", conn, tran);
                                        if (dtt_reg == null || dtt_reg.Rows.Count == 0)
                                        {
                                            prdInc.Add(new Produto
                                            {
                                                id = 0,
                                                id_empresa = company,
                                                id_grupo = prdApp[i].id_grupo,
                                                str_descricao = prdApp[i].str_descricao,
                                                str_obs = prdApp[i].str_obs,
                                                int_qtd_est = prdApp[i].int_qtd_est,
                                                int_qtd_estmin = prdApp[i].int_qtd_estmin,
                                                int_qtd_combo = prdApp[i].int_qtd_combo,
                                                dbl_val_unit = prdApp[i].dbl_val_unit,
                                                dbl_val_desc = prdApp[i].dbl_val_desc,
                                                dbl_perc_desc = prdApp[i].dbl_perc_desc,
                                                dbl_val_combo = prdApp[i].dbl_val_combo,
                                                str_foto = prdApp[i].str_foto,
                                                int_tipo = prdApp[i].int_tipo,
                                                int_unid_med = prdApp[i].int_unid_med,
                                                str_venda = prdApp[i].str_venda,
                                                str_estoque = prdApp[i].str_estoque,
                                                dtm_inclusao = prdApp[i].dtm_inclusao,
                                                dtm_alteracao = prdApp[i].dtm_alteracao,
                                                int_situacao = prdApp[i].int_situacao,
                                                id_app = prdApp[i].id,
                                                id_user_man = user
                                            });
                                        }
                                        else
                                        {
                                            prdUpd.Add(new Produto
                                            {
                                                id = Convert.ToInt64(dtt_reg.Rows[0]["id"]),
                                                id_empresa = company,
                                                id_grupo = prdApp[i].id_grupo,
                                                str_descricao = prdApp[i].str_descricao,
                                                str_obs = prdApp[i].str_obs,
                                                int_qtd_est = prdApp[i].int_qtd_est,
                                                int_qtd_estmin = prdApp[i].int_qtd_estmin,
                                                int_qtd_combo = prdApp[i].int_qtd_combo,
                                                dbl_val_unit = prdApp[i].dbl_val_unit,
                                                dbl_val_desc = prdApp[i].dbl_val_desc,
                                                dbl_perc_desc = prdApp[i].dbl_perc_desc,
                                                dbl_val_combo = prdApp[i].dbl_val_combo,
                                                str_foto = prdApp[i].str_foto,
                                                int_tipo = prdApp[i].int_tipo,
                                                int_unid_med = prdApp[i].int_unid_med,
                                                str_venda = prdApp[i].str_venda,
                                                str_estoque = prdApp[i].str_estoque,
                                                dtm_inclusao = prdApp[i].dtm_inclusao,
                                                dtm_alteracao = prdApp[i].dtm_alteracao,
                                                int_situacao = prdApp[i].int_situacao,
                                                id_app = prdApp[i].id,
                                                id_user_man = user
                                            });

                                        }
                                    }
                                    else
                                    {
                                        prdUpd.Add(new Produto
                                        {
                                            id = prdApp[i].id_server,
                                            id_empresa = company,
                                            id_grupo = prdApp[i].id_grupo,
                                            str_descricao = prdApp[i].str_descricao,
                                            str_obs = prdApp[i].str_obs,
                                            int_qtd_est = prdApp[i].int_qtd_est,
                                            int_qtd_estmin = prdApp[i].int_qtd_estmin,
                                            int_qtd_combo = prdApp[i].int_qtd_combo,
                                            dbl_val_unit = prdApp[i].dbl_val_unit,
                                            dbl_val_desc = prdApp[i].dbl_val_desc,
                                            dbl_perc_desc = prdApp[i].dbl_perc_desc,
                                            dbl_val_combo = prdApp[i].dbl_val_combo,
                                            str_foto = prdApp[i].str_foto,
                                            int_tipo = prdApp[i].int_tipo,
                                            int_unid_med = prdApp[i].int_unid_med,
                                            str_venda = prdApp[i].str_venda,
                                            str_estoque = prdApp[i].str_estoque,
                                            dtm_inclusao = prdApp[i].dtm_inclusao,
                                            dtm_alteracao = prdApp[i].dtm_alteracao,
                                            int_situacao = prdApp[i].int_situacao,
                                            id_app = prdApp[i].id,
                                            id_user_man = user
                                        });

                                    }
                                }

                                //Inclusões
                                if (prdInc.Count > 0)
                                {
                                    str_ret = repData.ManutencaoTabela<Produto>("I", prdInc, "ntv_tbl_produto", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += prdInc[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }

                                //Alterações
                                if (prdUpd.Count() > 0)
                                {
                                    str_ret_aux = "";
                                    str_ret = repData.ManutencaoTabela<Produto>("U", prdUpd, "ntv_tbl_produto", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += prdUpd[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }
                                dyn_ret = str_ret_fim;

                                break;

                            case "ItemCombo":
                                List<ItemComboApp> icomboApp = new List<ItemComboApp>();
                                icomboApp = JsonConvert.DeserializeObject<List<ItemComboApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));

                                List<ItemCombo> cmbInc = new List<ItemCombo>();
                                List<ItemCombo> cmbUpd = new List<ItemCombo>();

                                for (int i = 0; i < icomboApp.Count; i++)
                                {

                                    dtt_reg.Clear();

                                    if (icomboApp[i].id_server == 0)
                                    {
                                        //Verifica se o registro já existe
                                        dtt_reg = repData.ConsultaGenericaDtt("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"id_empresa\", \"valor\":\"" + company.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"download\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"id_app\", \"valor\":\"" + icomboApp[i].id.ToString() + "\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_itemcombo", conn, tran);
                                        if (dtt_reg == null || dtt_reg.Rows.Count == 0)
                                        {
                                            cmbInc.Add(new ItemCombo
                                            {
                                                id = 0,
                                                id_empresa = company,
                                                id_produto = icomboApp[i].id_produto,
                                                id_prd_combo = icomboApp[i].id_prd_combo,
                                                int_qtd_item = icomboApp[i].int_qtd_item,
                                                dbl_val_unit = icomboApp[i].dbl_val_unit,
                                                id_app = icomboApp[i].id,
                                                id_user_man = user
                                            });
                                        }
                                        else
                                        {
                                            cmbUpd.Add(new ItemCombo
                                            {
                                                id = Convert.ToInt64(dtt_reg.Rows[0]["id"]),
                                                id_empresa = company,
                                                id_produto = icomboApp[i].id_produto,
                                                id_prd_combo = icomboApp[i].id_prd_combo,
                                                int_qtd_item = icomboApp[i].int_qtd_item,
                                                dbl_val_unit = icomboApp[i].dbl_val_unit,
                                                id_app = icomboApp[i].id,
                                                id_user_man = user
                                            });

                                        }
                                    }
                                    else
                                    {
                                        cmbUpd.Add(new ItemCombo
                                        {
                                            id = icomboApp[i].id_server,
                                            id_empresa = company,
                                            id_produto = icomboApp[i].id_produto,
                                            id_prd_combo = icomboApp[i].id_prd_combo,
                                            int_qtd_item = icomboApp[i].int_qtd_item,
                                            dbl_val_unit = icomboApp[i].dbl_val_unit,
                                            id_app = icomboApp[i].id,
                                            id_user_man = user
                                        });

                                    }
                                }

                                //Inclusões
                                if (cmbInc.Count > 0)
                                {
                                    str_ret = repData.ManutencaoTabela<ItemCombo>("I", cmbInc, "ntv_tbl_itemcombo", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += cmbInc[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }

                                //Alterações
                                if (cmbUpd.Count() > 0)
                                {
                                    str_ret_aux = "";
                                    str_ret = repData.ManutencaoTabela<ItemCombo>("U", cmbUpd, "ntv_tbl_itemcombo", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += cmbUpd[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }
                                dyn_ret = str_ret_fim;

                                break;

                            case "ProdutoLista":
                                List<ProdutoListaApp> prdLApp = new List<ProdutoListaApp>();
                                prdLApp = JsonConvert.DeserializeObject<List<ProdutoListaApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));

                                List<ProdutoLista> prdLInc = new List<ProdutoLista>();
                                List<ProdutoLista> prdLUpd = new List<ProdutoLista>();

                                for (int i = 0; i < prdLApp.Count; i++)
                                {

                                    dtt_reg.Clear();

                                    if (prdLApp[i].id_server == 0)
                                    {
                                        //Verifica se o registro já existe
                                        dtt_reg = repData.ConsultaGenericaDtt("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"id_empresa\", \"valor\":\"" + company.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"download\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"id_app\", \"valor\":\"" + prdLApp[i].id.ToString() + "\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_produto_lista", conn, tran);
                                        if (dtt_reg == null || dtt_reg.Rows.Count == 0)
                                        {
                                            prdLInc.Add(new ProdutoLista
                                            {
                                                id = 0,
                                                id_empresa = company,
                                                str_nomelista = prdLApp[i].str_nomelista,
                                                str_idprodutos = prdLApp[i].str_idprodutos,
                                                id_app = prdLApp[i].id,
                                                id_user_man = user
                                            });
                                        }
                                        else
                                        {
                                            prdLUpd.Add(new ProdutoLista
                                            {
                                                id = Convert.ToInt64(dtt_reg.Rows[0]["id"]),
                                                id_empresa = company,
                                                str_nomelista = prdLApp[i].str_nomelista,
                                                str_idprodutos = prdLApp[i].str_idprodutos,
                                                id_app = prdLApp[i].id,
                                                id_user_man = user
                                            });

                                        }
                                    }
                                    else
                                    {
                                        prdLUpd.Add(new ProdutoLista
                                        {
                                            id = prdLApp[i].id_server,
                                            id_empresa = company,
                                            str_nomelista = prdLApp[i].str_nomelista,
                                            str_idprodutos = prdLApp[i].str_idprodutos,
                                            id_app = prdLApp[i].id,
                                            id_user_man = user
                                        });

                                    }
                                }

                                //Inclusões
                                if (prdLInc.Count > 0)
                                {
                                    str_ret = repData.ManutencaoTabela<ProdutoLista>("I", prdLInc, "ntv_tbl_produto_lista", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += prdLInc[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }

                                //Alterações
                                if (prdLUpd.Count() > 0)
                                {
                                    str_ret_aux = "";
                                    str_ret = repData.ManutencaoTabela<ProdutoLista>("U", prdLUpd, "ntv_tbl_produto_lista", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += prdLUpd[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }
                                dyn_ret = str_ret_fim;

                                break;

                            case "Grupo":
                                List<GrupoApp> grpApp = new List<GrupoApp>();
                                grpApp = JsonConvert.DeserializeObject<List<GrupoApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));

                                List<Grupo> grpInc = new List<Grupo>();
                                List<Grupo> grpUpd = new List<Grupo>();

                                for (int i = 0; i < grpApp.Count; i++)
                                {

                                    dtt_reg.Clear();

                                    if (grpApp[i].id_server == 0)
                                    {
                                        //Verifica se o registro já existe
                                        dtt_reg = repData.ConsultaGenericaDtt("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"id_empresa\", \"valor\":\"" + company.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"download\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"id_app\", \"valor\":\"" + grpApp[i].id.ToString() + "\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_grupo", conn, tran);
                                        if (dtt_reg == null || dtt_reg.Rows.Count == 0)
                                        {
                                            grpInc.Add(new Grupo
                                            {
                                                id = 0,
                                                id_empresa = company,
                                                str_descricao = grpApp[i].str_descricao,
                                                str_foto = grpApp[i].str_foto,
                                                dtm_inclusao = grpApp[i].dtm_inclusao,
                                                dtm_alteracao = grpApp[i].dtm_alteracao,
                                                int_situacao = grpApp[i].int_situacao,
                                                id_app = grpApp[i].id,
                                                id_user_man = user
                                            });
                                        }
                                        else
                                        {
                                            grpUpd.Add(new Grupo
                                            {
                                                id = Convert.ToInt64(dtt_reg.Rows[0]["id"]),
                                                id_empresa = company,
                                                str_descricao = grpApp[i].str_descricao,
                                                str_foto = grpApp[i].str_foto,
                                                dtm_inclusao = grpApp[i].dtm_inclusao,
                                                dtm_alteracao = grpApp[i].dtm_alteracao,
                                                int_situacao = grpApp[i].int_situacao,
                                                id_app = grpApp[i].id,
                                                id_user_man = user
                                            });

                                        }
                                    }
                                    else
                                    {
                                        grpUpd.Add(new Grupo
                                        {
                                            id = grpApp[i].id_server,
                                            id_empresa = company,
                                            str_descricao = grpApp[i].str_descricao,
                                            str_foto = grpApp[i].str_foto,
                                            dtm_inclusao = grpApp[i].dtm_inclusao,
                                            dtm_alteracao = grpApp[i].dtm_alteracao,
                                            int_situacao = grpApp[i].int_situacao,
                                            id_app = grpApp[i].id,
                                            id_user_man = user
                                        });

                                    }
                                }

                                //Inclusões
                                if (grpInc.Count > 0)
                                {
                                    str_ret = repData.ManutencaoTabela<Grupo>("I", grpInc, "ntv_tbl_grupo", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += grpInc[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }

                                //Alterações
                                if (grpUpd.Count() > 0)
                                {
                                    str_ret_aux = "";
                                    str_ret = repData.ManutencaoTabela<Grupo>("U", grpUpd, "ntv_tbl_grupo", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += grpUpd[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }
                                dyn_ret = str_ret_fim;

                                break;

                            case "FormaPag":
                                List<FormaPagApp> formaApp = new List<FormaPagApp>();
                                formaApp = JsonConvert.DeserializeObject<List<FormaPagApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));

                                List<FormaPag> formaInc = new List<FormaPag>();
                                List<FormaPag> formaUpd = new List<FormaPag>();

                                for (int i = 0; i < formaApp.Count; i++)
                                {

                                    dtt_reg.Clear();

                                    if (formaApp[i].id_server == 0)
                                    {
                                        //Verifica se o registro já existe
                                        dtt_reg = repData.ConsultaGenericaDtt("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"id_empresa\", \"valor\":\"" + company.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"download\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"id_app\", \"valor\":\"" + formaApp[i].id.ToString() + "\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_formapag", conn, tran);
                                        if (dtt_reg == null || dtt_reg.Rows.Count == 0)
                                        {
                                            formaInc.Add(new FormaPag
                                            {
                                                id = 0,
                                                id_empresa = company,
                                                str_descricao = formaApp[i].str_descricao,
                                                dtm_inclusao = formaApp[i].dtm_inclusao,
                                                dtm_alteracao = formaApp[i].dtm_alteracao,
                                                int_situacao = formaApp[i].int_situacao,
                                                id_app = formaApp[i].id,
                                                id_user_man = user
                                            });
                                        }
                                        else
                                        {
                                            formaUpd.Add(new FormaPag
                                            {
                                                id = Convert.ToInt64(dtt_reg.Rows[0]["id"]),
                                                id_empresa = company,
                                                str_descricao = formaApp[i].str_descricao,
                                                dtm_inclusao = formaApp[i].dtm_inclusao,
                                                dtm_alteracao = formaApp[i].dtm_alteracao,
                                                int_situacao = formaApp[i].int_situacao,
                                                id_app = formaApp[i].id,
                                                id_user_man = user
                                            });

                                        }
                                    }
                                    else
                                    {
                                        formaUpd.Add(new FormaPag
                                        {
                                            id = formaApp[i].id_server,
                                            id_empresa = company,
                                            str_descricao = formaApp[i].str_descricao,
                                            dtm_inclusao = formaApp[i].dtm_inclusao,
                                            dtm_alteracao = formaApp[i].dtm_alteracao,
                                            int_situacao = formaApp[i].int_situacao,
                                            id_app = formaApp[i].id,
                                            id_user_man = user
                                        });

                                    }
                                }

                                //Inclusões
                                if (formaInc.Count > 0)
                                {
                                    str_ret = repData.ManutencaoTabela<FormaPag>("I", formaInc, "ntv_tbl_formapag", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += formaInc[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }

                                //Alterações
                                if (formaUpd.Count() > 0)
                                {
                                    str_ret_aux = "";
                                    str_ret = repData.ManutencaoTabela<FormaPag>("U", formaUpd, "ntv_tbl_formapag", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += formaUpd[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }
                                dyn_ret = str_ret_fim;


                                break;

                            case "Local":
                                List<LocalApp> locApp = new List<LocalApp>();
                                locApp = JsonConvert.DeserializeObject<List<LocalApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));
                                List<Local> locInc = new List<Local>();
                                List<Local> locUpd = new List<Local>();

                                for (int i = 0; i < locApp.Count; i++)
                                {

                                    dtt_reg.Clear();

                                    if (locApp[i].id_server == 0)
                                    {
                                        //Verifica se o registro já existe
                                        dtt_reg = repData.ConsultaGenericaDtt("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"id_empresa\", \"valor\":\"" + company.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"download\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"id_app\", \"valor\":\"" + locApp[i].id.ToString() + "\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_local", conn, tran);
                                        if (dtt_reg == null || dtt_reg.Rows.Count == 0)
                                        {
                                            locInc.Add(new Local
                                            {
                                                id = 0,
                                                id_empresa = company,
                                                int_numero = locApp[i].int_numero,
                                                int_tipo = locApp[i].int_tipo,
                                                int_qtd_pess = locApp[i].int_qtd_pess,
                                                str_foto = locApp[i].str_foto,
                                                dtm_inclusao = locApp[i].dtm_inclusao,
                                                dtm_alteracao = DateTime.Now,
                                                int_situacao = locApp[i].int_situacao,
                                                id_app = locApp[i].id,
                                                id_user_man = user
                                            });
                                        }
                                        else
                                        {
                                            locUpd.Add(new Local
                                            {
                                                id = Convert.ToInt64(dtt_reg.Rows[0]["id"]),
                                                id_empresa = company,
                                                int_numero = locApp[i].int_numero,
                                                int_tipo = locApp[i].int_tipo,
                                                int_qtd_pess = locApp[i].int_qtd_pess,
                                                str_foto = locApp[i].str_foto,
                                                dtm_inclusao = locApp[i].dtm_inclusao,
                                                dtm_alteracao = DateTime.Now,
                                                int_situacao = locApp[i].int_situacao,
                                                id_app = locApp[i].id,
                                                id_user_man = user
                                            });

                                        }
                                    }
                                    else
                                    {
                                        locUpd.Add(new Local
                                        {
                                            id = locApp[i].id_server,
                                            id_empresa = company,
                                            int_numero = locApp[i].int_numero,
                                            int_tipo = locApp[i].int_tipo,
                                            int_qtd_pess = locApp[i].int_qtd_pess,
                                            str_foto = locApp[i].str_foto,
                                            dtm_inclusao = locApp[i].dtm_inclusao,
                                            dtm_alteracao = DateTime.Now,
                                            int_situacao = locApp[i].int_situacao,
                                            id_app = locApp[i].id,
                                            id_user_man = user
                                        });

                                    }
                                }

                                //Inclusões
                                if (locInc.Count > 0)
                                {
                                    str_ret = repData.ManutencaoTabela<Local>("I", locInc, "ntv_tbl_local", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += locInc[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }

                                //Alterações
                                if (locUpd.Count() > 0)
                                {
                                    str_ret_aux = "";
                                    str_ret = repData.ManutencaoTabela<Local>("U", locUpd, "ntv_tbl_local", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += locUpd[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }
                                dyn_ret = str_ret_fim;

                                break;

                            case "LocalCliente":
                                List<LocalClienteApp> locCliApp = new List<LocalClienteApp>();
                                locCliApp = JsonConvert.DeserializeObject<List<LocalClienteApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));
                                List<LocalCliente> locCliInc = new List<LocalCliente>();
                                List<LocalCliente> locCliUpd = new List<LocalCliente>();

                                for (int i = 0; i < locCliApp.Count; i++)
                                {

                                    dtt_reg.Clear();

                                    if (locCliApp[i].id_server == 0)
                                    {
                                        //Verifica se o registro já existe
                                        dtt_reg = repData.ConsultaGenericaDtt("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"id_empresa\", \"valor\":\"" + company.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"download\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"id_usuario\", \"valor\":\"" + locCliApp[i].id_usuario.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"id_app\", \"valor\":\"" + locCliApp[i].id.ToString() + "\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_localcliente", conn, tran);
                                        if (dtt_reg == null || dtt_reg.Rows.Count == 0)
                                        {
                                            locCliInc.Add(new LocalCliente
                                            {
                                                id = 0,
                                                id_empresa = company,
                                                id_local = locCliApp[i].id_local,
                                                str_cliente = locCliApp[i].str_cliente,
                                                str_senha = locCliApp[i].str_senha,
                                                int_qtdped = locCliApp[i].int_qtdped,
                                                int_qtditem = locCliApp[i].int_qtditem,
                                                dbl_val_tot = locCliApp[i].dbl_val_tot,
                                                dbl_val_desc = locCliApp[i].dbl_val_desc,
                                                dbl_val_liq = locCliApp[i].dbl_val_liq,
                                                dbl_val_pag = locCliApp[i].dbl_val_pag,
                                                dtm_inclusao = locCliApp[i].dtm_inclusao,
                                                dtm_pagto = locCliApp[i].dtm_pagto,
                                                dtm_cancel = locCliApp[i].dtm_cancel,
                                                int_situacao = locCliApp[i].int_situacao,
                                                id_usuario = locCliApp[i].id_usuario,
                                                id_app = locCliApp[i].id,
                                                id_user_man = user

                                            });
                                        }
                                        else
                                        {
                                            locCliUpd.Add(new LocalCliente
                                            {
                                                id = Convert.ToInt64(dtt_reg.Rows[0]["id"]),
                                                id_empresa = company,
                                                id_local = locCliApp[i].id_local,
                                                str_cliente = locCliApp[i].str_cliente,
                                                str_senha = locCliApp[i].str_senha,
                                                int_qtdped = locCliApp[i].int_qtdped,
                                                int_qtditem = locCliApp[i].int_qtditem,
                                                dbl_val_tot = locCliApp[i].dbl_val_tot,
                                                dbl_val_desc = locCliApp[i].dbl_val_desc,
                                                dbl_val_liq = locCliApp[i].dbl_val_liq,
                                                dbl_val_pag = locCliApp[i].dbl_val_pag,
                                                dtm_inclusao = locCliApp[i].dtm_inclusao,
                                                dtm_pagto = locCliApp[i].dtm_pagto,
                                                dtm_cancel = locCliApp[i].dtm_cancel,
                                                int_situacao = locCliApp[i].int_situacao,
                                                id_usuario = locCliApp[i].id_usuario,
                                                id_app = locCliApp[i].id,
                                                id_user_man = user
                                            });

                                        }
                                    }
                                    else
                                    {
                                        locCliUpd.Add(new LocalCliente
                                        {
                                            id = locCliApp[i].id_server,
                                            id_empresa = company,
                                            id_local = locCliApp[i].id_local,
                                            str_cliente = locCliApp[i].str_cliente,
                                            str_senha = locCliApp[i].str_senha,
                                            int_qtdped = locCliApp[i].int_qtdped,
                                            int_qtditem = locCliApp[i].int_qtditem,
                                            dbl_val_tot = locCliApp[i].dbl_val_tot,
                                            dbl_val_desc = locCliApp[i].dbl_val_desc,
                                            dbl_val_liq = locCliApp[i].dbl_val_liq,
                                            dbl_val_pag = locCliApp[i].dbl_val_pag,
                                            dtm_inclusao = locCliApp[i].dtm_inclusao,
                                            dtm_pagto = locCliApp[i].dtm_pagto,
                                            dtm_cancel = locCliApp[i].dtm_cancel,
                                            int_situacao = locCliApp[i].int_situacao,
                                            id_usuario = locCliApp[i].id_usuario,
                                            id_app = locCliApp[i].id,
                                            id_user_man = user
                                        });

                                    }
                                }

                                //Inclusões
                                if (locCliInc.Count > 0)
                                {
                                    str_ret = repData.ManutencaoTabela<LocalCliente>("I", locCliInc, "ntv_tbl_localcliente", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += locCliInc[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }

                                //Alterações
                                if (locCliUpd.Count() > 0)
                                {
                                    str_ret_aux = "";
                                    str_ret = repData.ManutencaoTabela<LocalCliente>("U", locCliUpd, "ntv_tbl_localcliente", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += locCliUpd[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }
                                dyn_ret = str_ret_fim;

                                break;

                            case "Pedido":
                                List<PedidoApp> pedApp = new List<PedidoApp>();
                                pedApp = JsonConvert.DeserializeObject<List<PedidoApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));
                                List<Pedido> pedInc = new List<Pedido>();
                                List<Pedido> pedUpd = new List<Pedido>();

                                for (int i = 0; i < pedApp.Count; i++)
                                {

                                    dtt_reg.Clear();

                                    if (pedApp[i].id_server == 0)
                                    {

                                        //Verifica se o registro já existe
                                        dtt_reg = repData.ConsultaGenericaDtt("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"id_empresa\", \"valor\":\"" + company.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"dtm_ini\", \"valor\":\"2001-01-01\", \"tipo\":\"DateTime\"}," +
                                                                    "{ \"nome\":\"dtm_fim\", \"valor\":\"2001-01-01\", \"tipo\":\"DateTime\"}," +
                                                                    "{ \"nome\":\"id_usuario\", \"valor\":\"" + pedApp[i].id_usuario.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"download\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"id_app\", \"valor\":\"" + pedApp[i].id.ToString() + "\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_pedido", conn, tran);
                                        if (dtt_reg == null || dtt_reg.Rows.Count == 0)
                                        {
                                            pedInc.Add(new Pedido
                                            {
                                                id = 0,
                                                id_empresa = company,
                                                id_localcli = pedApp[i].id_localcli,
                                                dtm_pedido = pedApp[i].dtm_pedido,
                                                dtm_pagto = pedApp[i].dtm_pagto,
                                                dtm_cancel = pedApp[i].dtm_cancel,
                                                int_qtd_item = pedApp[i].int_qtd_item,
                                                dbl_val_tot = pedApp[i].dbl_val_tot,
                                                dbl_val_desc = pedApp[i].dbl_val_desc,
                                                dbl_val_liq = pedApp[i].dbl_val_liq,
                                                dbl_val_pag = pedApp[i].dbl_val_pag,
                                                str_obs = pedApp[i].str_obervacao,
                                                int_situacao = pedApp[i].int_situacao,
                                                id_usuario = pedApp[i].id_usuario,
                                                id_app = pedApp[i].id,
                                                id_user_man = user
                                            });
                                        }
                                        else
                                        {
                                            pedUpd.Add(new Pedido
                                            {
                                                id = Convert.ToInt64(dtt_reg.Rows[0]["id"]),
                                                id_empresa = company,
                                                id_localcli = pedApp[i].id_localcli,
                                                dtm_pedido = pedApp[i].dtm_pedido,
                                                dtm_pagto = pedApp[i].dtm_pagto,
                                                dtm_cancel = pedApp[i].dtm_cancel,
                                                int_qtd_item = pedApp[i].int_qtd_item,
                                                dbl_val_tot = pedApp[i].dbl_val_tot,
                                                dbl_val_desc = pedApp[i].dbl_val_desc,
                                                dbl_val_liq = pedApp[i].dbl_val_liq,
                                                dbl_val_pag = pedApp[i].dbl_val_pag,
                                                str_obs = pedApp[i].str_obervacao,
                                                int_situacao = pedApp[i].int_situacao,
                                                id_usuario = pedApp[i].id_usuario,
                                                id_app = pedApp[i].id,
                                                id_user_man = user
                                            });

                                        }
                                    }
                                    else
                                    {
                                        pedUpd.Add(new Pedido
                                        {
                                            id = pedApp[i].id_server,
                                            id_empresa = company,
                                            id_localcli = pedApp[i].id_localcli,
                                            dtm_pedido = pedApp[i].dtm_pedido,
                                            dtm_pagto = pedApp[i].dtm_pagto,
                                            dtm_cancel = pedApp[i].dtm_cancel,
                                            int_qtd_item = pedApp[i].int_qtd_item,
                                            dbl_val_tot = pedApp[i].dbl_val_tot,
                                            dbl_val_desc = pedApp[i].dbl_val_desc,
                                            dbl_val_liq = pedApp[i].dbl_val_liq,
                                            dbl_val_pag = pedApp[i].dbl_val_pag,
                                            str_obs = pedApp[i].str_obervacao,
                                            int_situacao = pedApp[i].int_situacao,
                                            id_usuario = pedApp[i].id_usuario,
                                            id_app = pedApp[i].id,
                                            id_user_man = user
                                        });

                                    }
                                }

                                //Inclusões
                                if (pedInc.Count > 0)
                                {
                                    str_ret = repData.ManutencaoTabela<Pedido>("I", pedInc, "ntv_tbl_pedido", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += pedInc[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }

                                //Alterações
                                if (pedUpd.Count() > 0)
                                {
                                    str_ret_aux = "";
                                    str_ret = repData.ManutencaoTabela<Pedido>("U", pedUpd, "ntv_tbl_pedido", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += pedUpd[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }
                                dyn_ret = str_ret_fim;

                                break;

                            case "PedidoItem":
                                List<PedidoItemApp> pedIApp = new List<PedidoItemApp>();
                                pedIApp = JsonConvert.DeserializeObject<List<PedidoItemApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));
                                List<PedidoItem> pediInc = new List<PedidoItem>();
                                List<PedidoItem> pediUpd = new List<PedidoItem>();

                                for (int i = 0; i < pedIApp.Count; i++)
                                {

                                    dtt_reg.Clear();

                                    if (pedIApp[i].id_server == 0)
                                    {

                                        //Verifica se o registro já existe
                                        dtt_reg = repData.ConsultaGenericaDtt("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"id_empresa\", \"valor\":\"" + company.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"dtm_ini\", \"valor\":\"2001-01-01\", \"tipo\":\"DateTime\"}," +
                                                                    "{ \"nome\":\"dtm_fim\", \"valor\":\"2001-01-01\", \"tipo\":\"DateTime\"}," +
                                                                    "{ \"nome\":\"id_usuario\", \"valor\":\"" + pedIApp[i].id_usuario.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"download\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"id_app\", \"valor\":\"" + pedIApp[i].id.ToString() + "\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_pedidoitem", conn, tran);
                                        if (dtt_reg == null || dtt_reg.Rows.Count == 0)
                                        {
                                            pediInc.Add(new PedidoItem
                                            {
                                                id = 0,
                                                id_empresa = company,
                                                id_pedido = pedIApp[i].id_pedido,
                                                id_produto = pedIApp[i].id_produto,
                                                dbl_precounit = pedIApp[i].dbl_precounit,
                                                int_qtd_comp = pedIApp[i].int_qtd_comp,
                                                dbl_tot_item = pedIApp[i].dbl_tot_item,
                                                dbl_desconto = pedIApp[i].dbl_desconto,
                                                dbl_tot_liq = pedIApp[i].dbl_tot_liq,
                                                int_situacao = pedIApp[i].int_situacao,
                                                id_usuario = pedIApp[i].id_usuario,
                                                id_app = pedIApp[i].id,
                                                id_user_man = user
                                            });
                                        }
                                        else
                                        {
                                            pediUpd.Add(new PedidoItem
                                            {
                                                id = Convert.ToInt64(dtt_reg.Rows[0]["id"]),
                                                id_empresa = company,
                                                id_pedido = pedIApp[i].id_pedido,
                                                id_produto = pedIApp[i].id_produto,
                                                dbl_precounit = pedIApp[i].dbl_precounit,
                                                int_qtd_comp = pedIApp[i].int_qtd_comp,
                                                dbl_tot_item = pedIApp[i].dbl_tot_item,
                                                dbl_desconto = pedIApp[i].dbl_desconto,
                                                dbl_tot_liq = pedIApp[i].dbl_tot_liq,
                                                int_situacao = pedIApp[i].int_situacao,
                                                id_usuario = pedIApp[i].id_usuario,
                                                id_app = pedIApp[i].id,
                                                id_user_man = user
                                            });

                                        }
                                    }
                                    else
                                    {
                                        pediUpd.Add(new PedidoItem
                                        {
                                            id = pedIApp[i].id_server,
                                            id_empresa = company,
                                            id_pedido = pedIApp[i].id_pedido,
                                            id_produto = pedIApp[i].id_produto,
                                            dbl_precounit = pedIApp[i].dbl_precounit,
                                            int_qtd_comp = pedIApp[i].int_qtd_comp,
                                            dbl_tot_item = pedIApp[i].dbl_tot_item,
                                            dbl_desconto = pedIApp[i].dbl_desconto,
                                            dbl_tot_liq = pedIApp[i].dbl_tot_liq,
                                            int_situacao = pedIApp[i].int_situacao,
                                            id_usuario = pedIApp[i].id_usuario,
                                            id_app = pedIApp[i].id,
                                            id_user_man = user
                                        });

                                    }
                                }

                                //Inclusões
                                if (pediInc.Count > 0)
                                {
                                    str_ret = repData.ManutencaoTabela<PedidoItem>("I", pediInc, "ntv_tbl_pedidoitem", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += pediInc[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }

                                //Alterações
                                if (pediUpd.Count() > 0)
                                {
                                    str_ret_aux = "";
                                    str_ret = repData.ManutencaoTabela<PedidoItem>("U", pediUpd, "ntv_tbl_pedidoitem", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += pediUpd[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }
                                dyn_ret = str_ret_fim;

                                break;

                            case "PedidoItemCombo":
                                List<PedidoItemComboApp> pedICmbApp = new List<PedidoItemComboApp>();
                                pedICmbApp = JsonConvert.DeserializeObject<List<PedidoItemComboApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));
                                List<PedidoItemCombo> pediCmbInc = new List<PedidoItemCombo>();
                                List<PedidoItemCombo> pediCmbUpd = new List<PedidoItemCombo>();

                                for (int i = 0; i < pedICmbApp.Count; i++)
                                {

                                    dtt_reg.Clear();

                                    if (pedICmbApp[i].id_server == 0)
                                    {

                                        //Verifica se o registro já existe
                                        dtt_reg = repData.ConsultaGenericaDtt("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"id_empresa\", \"valor\":\"" + company.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"dtm_ini\", \"valor\":\"2001-01-01\", \"tipo\":\"DateTime\"}," +
                                                                    "{ \"nome\":\"dtm_fim\", \"valor\":\"2001-01-01\", \"tipo\":\"DateTime\"}," +
                                                                    "{ \"nome\":\"id_usuario\", \"valor\":\"" + pedICmbApp[i].id_usuario.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"download\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"id_app\", \"valor\":\"" + pedICmbApp[i].id.ToString() + "\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_pedidoitemcombo", conn, tran);
                                        if (dtt_reg == null || dtt_reg.Rows.Count == 0)
                                        {
                                            pediCmbInc.Add(new PedidoItemCombo
                                            {
                                                id = 0,
                                                id_empresa = company,
                                                id_pedido = pedICmbApp[i].id_pedido,
                                                id_ped_item = pedICmbApp[i].id_ped_item,
                                                id_produto = pedICmbApp[i].id_produto,
                                                id_prod_sel = pedICmbApp[i].id_prod_sel,
                                                int_qtd_comp = pedICmbApp[i].int_qtd_comp,
                                                id_usuario = pedICmbApp[i].id_usuario,
                                                int_situacao = pedICmbApp[i].int_situacao,
                                                id_app = pedICmbApp[i].id,
                                                id_user_man = user
                                            });
                                        }
                                        else
                                        {
                                            pediCmbUpd.Add(new PedidoItemCombo
                                            {
                                                id = Convert.ToInt64(dtt_reg.Rows[0]["id"]),
                                                id_empresa = company,
                                                id_pedido = pedICmbApp[i].id_pedido,
                                                id_ped_item = pedICmbApp[i].id_ped_item,
                                                id_produto = pedICmbApp[i].id_produto,
                                                id_prod_sel = pedICmbApp[i].id_prod_sel,
                                                int_qtd_comp = pedICmbApp[i].int_qtd_comp,
                                                int_situacao = pedICmbApp[i].int_situacao,
                                                id_usuario = pedICmbApp[i].id_usuario,
                                                id_app = pedICmbApp[i].id,
                                                id_user_man = user
                                            });

                                        }
                                    }
                                    else
                                    {
                                        pediCmbUpd.Add(new PedidoItemCombo
                                        {
                                            id = pedICmbApp[i].id_server,
                                            id_empresa = company,
                                            id_pedido = pedICmbApp[i].id_pedido,
                                            id_ped_item = pedICmbApp[i].id_ped_item,
                                            id_produto = pedICmbApp[i].id_produto,
                                            id_prod_sel = pedICmbApp[i].id_prod_sel,
                                            int_qtd_comp = pedICmbApp[i].int_qtd_comp,
                                            id_usuario = pedICmbApp[i].id_usuario,
                                            int_situacao = pedICmbApp[i].int_situacao,
                                            id_app = pedICmbApp[i].id,
                                            id_user_man = user
                                        });

                                    }
                                }

                                //Inclusões
                                if (pediCmbInc.Count > 0)
                                {
                                    str_ret = repData.ManutencaoTabela<PedidoItemCombo>("I", pediCmbInc, "ntv_tbl_pedidoitemcombo", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += pediCmbInc[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }

                                //Alterações
                                if (pediCmbUpd.Count() > 0)
                                {
                                    str_ret_aux = "";
                                    str_ret = repData.ManutencaoTabela<PedidoItemCombo>("U", pediCmbUpd, "ntv_tbl_pedidoitemcombo", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += pediCmbUpd[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }
                                dyn_ret = str_ret_fim;

                                break;

                            case "LocalCliPag":
                                List<LocalCliPagApp> cliPApp = new List<LocalCliPagApp>();
                                cliPApp = JsonConvert.DeserializeObject<List<LocalCliPagApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));
                                List<LocalCliPag> clipInc = new List<LocalCliPag>();
                                List<LocalCliPag> clipUpd = new List<LocalCliPag>();

                                for (int i = 0; i < cliPApp.Count; i++)
                                {

                                    dtt_reg.Clear();

                                    if (cliPApp[i].id_server == 0)
                                    {
                                        //Verifica se o registro já existe
                                        dtt_reg = repData.ConsultaGenericaDtt("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"id_empresa\", \"valor\":\"" + company.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"download\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"id_usuario\", \"valor\":\"" + cliPApp[i].id_usuario.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"id_app\", \"valor\":\"" + cliPApp[i].id.ToString() + "\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_localclipag", conn, tran);
                                        if (dtt_reg == null || dtt_reg.Rows.Count == 0)
                                        {
                                            clipInc.Add(new LocalCliPag
                                            {
                                                id = 0,
                                                id_empresa = company,
                                                id_localcli = cliPApp[i].id_localcli,
                                                id_formapag = cliPApp[i].id_formapag,
                                                dtm_pagto = cliPApp[i].dtm_pagto,
                                                dtm_cancel = cliPApp[i].dtm_cancel,
                                                dbl_val_pgto = cliPApp[i].dbl_val_pgto,
                                                dbl_desconto = cliPApp[i].dbl_desconto,
                                                dbl_gorjeta = cliPApp[i].dbl_gorjeta,
                                                dbl_acrescimo = cliPApp[i].dbl_acrescimo,
                                                id_usuario = cliPApp[i].id_usuario,
                                                id_app = cliPApp[i].id,
                                                id_user_man = user
                                            });
                                        }
                                        else
                                        {
                                            clipUpd.Add(new LocalCliPag
                                            {
                                                id = Convert.ToInt64(dtt_reg.Rows[0]["id"]),
                                                id_empresa = company,
                                                id_localcli = cliPApp[i].id_localcli,
                                                id_formapag = cliPApp[i].id_formapag,
                                                dtm_pagto = cliPApp[i].dtm_pagto,
                                                dtm_cancel = cliPApp[i].dtm_cancel,
                                                dbl_val_pgto = cliPApp[i].dbl_val_pgto,
                                                dbl_desconto = cliPApp[i].dbl_desconto,
                                                dbl_gorjeta = cliPApp[i].dbl_gorjeta,
                                                dbl_acrescimo = cliPApp[i].dbl_acrescimo,
                                                id_usuario = cliPApp[i].id_usuario,
                                                id_app = cliPApp[i].id,
                                                id_user_man = user
                                            });

                                        }
                                    }
                                    else
                                    {
                                        clipUpd.Add(new LocalCliPag
                                        {
                                            id = cliPApp[i].id_server,
                                            id_empresa = company,
                                            id_localcli = cliPApp[i].id_localcli,
                                            id_formapag = cliPApp[i].id_formapag,
                                            dtm_pagto = cliPApp[i].dtm_pagto,
                                            dtm_cancel = cliPApp[i].dtm_cancel,
                                            dbl_val_pgto = cliPApp[i].dbl_val_pgto,
                                            dbl_desconto = cliPApp[i].dbl_desconto,
                                            dbl_gorjeta = cliPApp[i].dbl_gorjeta,
                                            dbl_acrescimo = cliPApp[i].dbl_acrescimo,
                                            id_usuario = cliPApp[i].id_usuario,
                                            id_app = cliPApp[i].id,
                                            id_user_man = user
                                        });

                                    }
                                }

                                //Inclusões
                                if (clipInc.Count > 0)
                                {
                                    str_ret = repData.ManutencaoTabela<LocalCliPag>("I", clipInc, "ntv_tbl_localclipag", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += clipInc[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }

                                //Alterações
                                if (clipUpd.Count() > 0)
                                {
                                    str_ret_aux = "";
                                    str_ret = repData.ManutencaoTabela<LocalCliPag>("U", clipUpd, "ntv_tbl_localclipag", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += clipUpd[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }
                                dyn_ret = str_ret_fim;
                                break;

                            case "Entrada":
                                List<EntradaApp> entApp = new List<EntradaApp>();
                                entApp = JsonConvert.DeserializeObject<List<EntradaApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));
                                List<Entrada> entInc = new List<Entrada>();
                                List<Entrada> entUpd = new List<Entrada>();

                                for (int i = 0; i < entApp.Count; i++)
                                {

                                    dtt_reg.Clear();

                                    if (entApp[i].id_server == 0)
                                    {

                                        //Verifica se o registro já existe
                                        dtt_reg = repData.ConsultaGenericaDtt("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"id_empresa\", \"valor\":\"" + company.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"dtm_ini\", \"valor\":\"2001-01-01\", \"tipo\":\"DateTime\"}," +
                                                                    "{ \"nome\":\"dtm_fim\", \"valor\":\"2001-01-01\", \"tipo\":\"DateTime\"}," +
                                                                    "{ \"nome\":\"id_usuario\", \"valor\":\"" + entApp[i].id_usuario.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"download\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"id_app\", \"valor\":\"" + entApp[i].id.ToString() + "\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_entrada", conn, tran);
                                        if (dtt_reg == null || dtt_reg.Rows.Count == 0)
                                        {
                                            entInc.Add(new Entrada
                                            {
                                                id = 0,
                                                id_empresa = company,
                                                dtm_entrada = entApp[i].dtm_entrada,
                                                int_nota = entApp[i].int_nota,
                                                int_fornecedor = entApp[i].int_fornecedor,
                                                str_serie = entApp[i].str_serie,
                                                dtm_nota = entApp[i].dtm_nota,
                                                dbl_tot_nf = entApp[i].dbl_tot_nf,
                                                int_qtd_item = entApp[i].int_qtd_item,
                                                dbl_tot_item = entApp[i].dbl_tot_item,
                                                dbl_tot_desc = entApp[i].dbl_tot_desc,
                                                dbl_tot_liq = entApp[i].dbl_tot_liq,
                                                str_obs = entApp[i].str_obs,
                                                int_situacao = entApp[i].int_situacao,
                                                id_usuario = entApp[i].id_usuario,
                                                id_app = entApp[i].id,
                                                id_user_man = user
                                            });
                                        }
                                        else
                                        {
                                            entUpd.Add(new Entrada
                                            {
                                                id = Convert.ToInt64(dtt_reg.Rows[0]["id"]),
                                                id_empresa = company,
                                                dtm_entrada = entApp[i].dtm_entrada,
                                                int_nota = entApp[i].int_nota,
                                                int_fornecedor = entApp[i].int_fornecedor,
                                                str_serie = entApp[i].str_serie,
                                                dtm_nota = entApp[i].dtm_nota,
                                                dbl_tot_nf = entApp[i].dbl_tot_nf,
                                                int_qtd_item = entApp[i].int_qtd_item,
                                                dbl_tot_item = entApp[i].dbl_tot_item,
                                                dbl_tot_desc = entApp[i].dbl_tot_desc,
                                                dbl_tot_liq = entApp[i].dbl_tot_liq,
                                                str_obs = entApp[i].str_obs,
                                                int_situacao = entApp[i].int_situacao,
                                                id_usuario = entApp[i].id_usuario,
                                                id_app = entApp[i].id,
                                                id_user_man = user
                                            });

                                        }
                                    }
                                    else
                                    {
                                        entUpd.Add(new Entrada
                                        {
                                            id = entApp[i].id_server,
                                            id_empresa = company,
                                            dtm_entrada = entApp[i].dtm_entrada,
                                            int_nota = entApp[i].int_nota,
                                            int_fornecedor = entApp[i].int_fornecedor,
                                            str_serie = entApp[i].str_serie,
                                            dtm_nota = entApp[i].dtm_nota,
                                            dbl_tot_nf = entApp[i].dbl_tot_nf,
                                            int_qtd_item = entApp[i].int_qtd_item,
                                            dbl_tot_item = entApp[i].dbl_tot_item,
                                            dbl_tot_desc = entApp[i].dbl_tot_desc,
                                            dbl_tot_liq = entApp[i].dbl_tot_liq,
                                            str_obs = entApp[i].str_obs,
                                            int_situacao = entApp[i].int_situacao,
                                            id_usuario = entApp[i].id_usuario,
                                            id_app = entApp[i].id,
                                            id_user_man = user
                                        });

                                    }
                                }

                                //Inclusões
                                if (entInc.Count > 0)
                                {
                                    str_ret = repData.ManutencaoTabela<Entrada>("I", entInc, "ntv_tbl_entrada", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += entInc[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }

                                //Alterações
                                if (entUpd.Count() > 0)
                                {
                                    str_ret_aux = "";
                                    str_ret = repData.ManutencaoTabela<Entrada>("U", entUpd, "ntv_tbl_entrada", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += entUpd[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }
                                dyn_ret = str_ret_fim;

                                break;

                            case "EntradaItem":
                                List<EntradaItemApp> entIApp = new List<EntradaItemApp>();
                                entIApp = JsonConvert.DeserializeObject<List<EntradaItemApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));
                                List<EntradaItem> entiInc = new List<EntradaItem>();
                                List<EntradaItem> entiUpd = new List<EntradaItem>();

                                for (int i = 0; i < entIApp.Count; i++)
                                {

                                    dtt_reg.Clear();

                                    if (entIApp[i].id_server == 0)
                                    {

                                        //Verifica se o registro já existe
                                        dtt_reg = repData.ConsultaGenericaDtt("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"id_empresa\", \"valor\":\"" + company.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"dtm_ini\", \"valor\":\"2001-01-01\", \"tipo\":\"DateTime\"}," +
                                                                    "{ \"nome\":\"dtm_fim\", \"valor\":\"2001-01-01\", \"tipo\":\"DateTime\"}," +
                                                                    "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"download\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"id_app\", \"valor\":\"" + entIApp[i].id.ToString() + "\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_entrada_item", conn, tran);
                                        if (dtt_reg == null || dtt_reg.Rows.Count == 0)
                                        {
                                            entiInc.Add(new EntradaItem
                                            {
                                                id = 0,
                                                id_empresa = company,
                                                id_entrada = entIApp[i].id_entrada,
                                                id_produto = entIApp[i].id_produto,
                                                dbl_precounit = entIApp[i].dbl_precounit,
                                                int_qtd_comp = entIApp[i].int_qtd_comp,
                                                dbl_tot_item = entIApp[i].dbl_tot_item,
                                                dbl_desconto = entIApp[i].dbl_desconto,
                                                dbl_tot_liq = entIApp[i].dbl_tot_liq,
                                                int_situacao = entIApp[i].int_situacao,
                                                id_app = entIApp[i].id,
                                                id_user_man = user
                                            });
                                        }
                                        else
                                        {
                                            entiUpd.Add(new EntradaItem
                                            {
                                                id = Convert.ToInt64(dtt_reg.Rows[0]["id"]),
                                                id_empresa = company,
                                                id_entrada = entIApp[i].id_entrada,
                                                id_produto = entIApp[i].id_produto,
                                                dbl_precounit = entIApp[i].dbl_precounit,
                                                int_qtd_comp = entIApp[i].int_qtd_comp,
                                                dbl_tot_item = entIApp[i].dbl_tot_item,
                                                dbl_desconto = entIApp[i].dbl_desconto,
                                                dbl_tot_liq = entIApp[i].dbl_tot_liq,
                                                int_situacao = entIApp[i].int_situacao,
                                                id_app = entIApp[i].id,
                                                id_user_man = user
                                            });

                                        }
                                    }
                                    else
                                    {
                                        entiUpd.Add(new EntradaItem
                                        {
                                            id = entIApp[i].id_server,
                                            id_empresa = company,
                                            id_entrada = entIApp[i].id_entrada,
                                            id_produto = entIApp[i].id_produto,
                                            dbl_precounit = entIApp[i].dbl_precounit,
                                            int_qtd_comp = entIApp[i].int_qtd_comp,
                                            dbl_tot_item = entIApp[i].dbl_tot_item,
                                            dbl_desconto = entIApp[i].dbl_desconto,
                                            dbl_tot_liq = entIApp[i].dbl_tot_liq,
                                            int_situacao = entIApp[i].int_situacao,
                                            id_app = entIApp[i].id,
                                            id_user_man = user
                                        });

                                    }
                                }

                                //Inclusões
                                if (entiInc.Count > 0)
                                {
                                    str_ret = repData.ManutencaoTabela<EntradaItem>("I", entiInc, "ntv_tbl_entrada_item", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += entiInc[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }

                                //Alterações
                                if (entiUpd.Count() > 0)
                                {
                                    str_ret_aux = "";
                                    str_ret = repData.ManutencaoTabela<EntradaItem>("U", entiUpd, "ntv_tbl_entrada_item", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += entiUpd[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }
                                dyn_ret = str_ret_fim;

                                break;

                            case "PrdEstoque":
                                List<PrdEstoqueApp> prdEApp = new List<PrdEstoqueApp>();
                                prdEApp = JsonConvert.DeserializeObject<List<PrdEstoqueApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));
                                List<PrdEstoque> prdEInc = new List<PrdEstoque>();
                                List<PrdEstoque> prdEUpd = new List<PrdEstoque>();

                                for (int i = 0; i < prdEApp.Count; i++)
                                {

                                    dtt_reg.Clear();

                                    if (prdEApp[i].id_server == 0)
                                    {

                                        //Verifica se o registro já existe
                                        dtt_reg = repData.ConsultaGenericaDtt("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"id_empresa\", \"valor\":\"" + company.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                    "{ \"nome\":\"download\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                                                                    "{ \"nome\":\"id_app\", \"valor\":\"" + prdEApp[i].id.ToString() + "\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_produto_estoque", conn, tran);
                                        if (dtt_reg == null || dtt_reg.Rows.Count == 0)
                                        {
                                            prdEInc.Add(new PrdEstoque
                                            {
                                                id = 0,
                                                id_empresa = company,
                                                id_produto = prdEApp[i].id_produto,
                                                int_qtd_est = prdEApp[i].int_qtd_est,
                                                dtm_estoque = prdEApp[i].dtm_estoque,
                                                id_app = prdEApp[i].id,
                                                id_user_man = user
                                            });
                                        }
                                        else
                                        {
                                            prdEUpd.Add(new PrdEstoque
                                            {
                                                id = Convert.ToInt64(dtt_reg.Rows[0]["id"]),
                                                id_empresa = company,
                                                id_produto = prdEApp[i].id_produto,
                                                int_qtd_est = prdEApp[i].int_qtd_est,
                                                dtm_estoque = prdEApp[i].dtm_estoque,
                                                id_app = prdEApp[i].id,
                                                id_user_man = user
                                            });

                                        }
                                    }
                                    else
                                    {
                                        prdEUpd.Add(new PrdEstoque
                                        {
                                            id = prdEApp[i].id_server,
                                            id_empresa = company,
                                            id_produto = prdEApp[i].id_produto,
                                            int_qtd_est = prdEApp[i].int_qtd_est,
                                            dtm_estoque = prdEApp[i].dtm_estoque,
                                            id_app = prdEApp[i].id,
                                            id_user_man = user
                                        });

                                    }
                                }

                                //Inclusões
                                if (prdEInc.Count > 0)
                                {
                                    str_ret = repData.ManutencaoTabela<PrdEstoque>("I", prdEInc, "ntv_tbl_produto_estoque", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += prdEInc[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }

                                //Alterações
                                if (prdEUpd.Count() > 0)
                                {
                                    str_ret_aux = "";
                                    str_ret = repData.ManutencaoTabela<PrdEstoque>("U", prdEUpd, "ntv_tbl_produto_estoque", conn, tran);
                                    if (str_ret.Split(";").Count() > 0)
                                    {
                                        for (int id = 0; id < str_ret.Split(";").Count(); id++)
                                        {
                                            if (str_ret.Split(";")[id] != "")
                                            {
                                                str_ret_aux += prdEUpd[id].id_app + ":" + str_ret.Split(";")[id] + ";";
                                            }
                                        }
                                        str_ret_fim += str_ret_aux;
                                    }
                                }
                                dyn_ret = str_ret_fim;

                                break;

                        }
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        conn.Close();
                        throw ex;
                    }
                }
                conn.Close();
            }

            return dyn_ret;
        }
    }
}
