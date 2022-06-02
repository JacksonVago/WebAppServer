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

                                        case "ntv_tbl_produto":
                                            str_tabela = "Produto";
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
            dataDownRec = JsonConvert.DeserializeObject<List<DadosDownloadRecovery>>(filtros);

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

                            case "Produto":
                                str_proc = "ntv_p_sel_tbl_produto";
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

                            default:
                                str_proc = "";
                                break;

                        }

                        //Executconsulta
                        if (str_proc.Length > 0)
                        {
                            str_ret = repData.ConsultaGenerica(dataDownRec[i].filtro, str_proc, conn);
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
            dynamic dyn_ret = null;

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
                                var usuI = from n in usuApp where n.id_server == 0 select n;
                                var usuU = from n in usuApp where n.id_server > 0 select n;

                                List<Usuario> usu = new List<Usuario>();

                                //Inclusões
                                if (usuI != null && usuI.Count() > 0)
                                {
                                    foreach (UsuarioApp e in usuI)
                                    {
                                        usu.Add(new Usuario
                                        {
                                            id = e.id_server,
                                            id_empresa = company,
                                            str_nome = e.str_nome,
                                            str_login = e.str_login,
                                            str_senha = e.str_senha,
                                            str_email = e.str_email,
                                            int_telefone = e.int_telefone,
                                            int_tipo = e.int_tipo,
                                            dtm_inclusao = e.dtm_inclusao,
                                            dtm_saida = e.dtm_saida,
                                            int_situacao = e.int_situacao,
                                            str_foto = e.str_foto,
                                            id_app = e.id,
                                            id_user_man = user
                                        });
                                    }

                                    str_ret = repData.ManutencaoTabela<Usuario>("I", usu, "ntv_tbl_usuario", conn, tran);
                                    dyn_ret = str_ret;
                                }

                                //Alterações
                                if (usuU != null && usuU.Count() > 0)
                                {
                                    usu.Clear();

                                    foreach (UsuarioApp e in usuU)
                                    {
                                        usu.Add(new Usuario
                                        {
                                            id = e.id_server,
                                            id_empresa = company,
                                            str_nome = e.str_nome,
                                            str_login = e.str_login,
                                            str_senha = e.str_senha,
                                            str_email = e.str_email,
                                            int_telefone = e.int_telefone,
                                            int_tipo = e.int_tipo,
                                            dtm_inclusao = e.dtm_inclusao,
                                            dtm_saida = e.dtm_saida,
                                            int_situacao = e.int_situacao,
                                            str_foto = e.str_foto,
                                            id_app = e.id,
                                            id_user_man = user
                                        });
                                    }
                                    str_ret = repData.ManutencaoTabela<Usuario>("U", usu, "ntv_tbl_usuario", conn, tran);
                                }
                                dyn_ret = str_ret;
                                break;

                            case "Produto":
                                List<ProdutoApp> prdApp = new List<ProdutoApp>();
                                prdApp = JsonConvert.DeserializeObject<List<ProdutoApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));
                                var regI = from n in prdApp where n.id_server == 0 select n;
                                var regU = from n in prdApp where n.id_server > 0 select n;

                                List<Produto> reg = new List<Produto>();

                                //Inclusões
                                if (regI != null && regI.Count() > 0)
                                {
                                    foreach (ProdutoApp e in regI)
                                    {
                                        reg.Add(new Produto
                                        {
                                            id = e.id_server,
                                            id_empresa = company,
                                            id_grupo = e.id_grupo,
                                            str_descricao = e.str_descricao,
                                            str_obs = e.str_obs,
                                            int_qtd_est = e.int_qtd_est,
                                            int_qtd_combo = e.int_qtd_combo,
                                            dbl_val_unit = e.dbl_val_unit,
                                            dbl_val_desc = e.dbl_val_desc,
                                            dbl_perc_desc = e.dbl_perc_desc,
                                            dbl_val_combo = e.dbl_val_combo,
                                            str_foto = e.str_foto,
                                            int_tipo = e.int_tipo,
                                            dtm_inclusao = e.dtm_inclusao,
                                            dtm_alteracao = e.dtm_alteracao,
                                            int_situacao = e.int_situacao,
                                            id_app = e.id,
                                            id_user_man = user
                                        });
                                    }

                                    str_ret = repData.ManutencaoTabela<Produto>("I", reg, "ntv_tbl_produto", conn, tran);
                                    dyn_ret = str_ret;
                                }

                                //Alterações
                                if (regU != null && regU.Count() > 0)
                                {
                                    reg.Clear();

                                    foreach (ProdutoApp e in regU)
                                    {
                                        reg.Add(new Produto
                                        {
                                            id = e.id_server,
                                            id_empresa = company,
                                            id_grupo = e.id_grupo,
                                            str_descricao = e.str_descricao,
                                            str_obs = e.str_obs,
                                            int_qtd_est = e.int_qtd_est,
                                            int_qtd_combo = e.int_qtd_combo,
                                            dbl_val_unit = e.dbl_val_unit,
                                            dbl_val_desc = e.dbl_val_desc,
                                            dbl_perc_desc = e.dbl_perc_desc,
                                            dbl_val_combo = e.dbl_val_combo,
                                            str_foto = e.str_foto,
                                            int_tipo = e.int_tipo,
                                            dtm_inclusao = e.dtm_inclusao,
                                            dtm_alteracao = e.dtm_alteracao,
                                            int_situacao = e.int_situacao,
                                            id_app = e.id,
                                            id_user_man = user
                                        });
                                    }
                                    str_ret = repData.ManutencaoTabela<Produto>("U", reg, "ntv_tbl_produto", conn, tran);
                                }
                                dyn_ret = str_ret;
                                break;

                            case "ItemCombo":
                                List<ItemComboApp> icomboApp = new List<ItemComboApp>();
                                icomboApp = JsonConvert.DeserializeObject<List<ItemComboApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));
                                var icomboI = from n in icomboApp where n.id_server == 0 select n;
                                var icomboU = from n in icomboApp where n.id_server > 0 select n;

                                List<ItemCombo> icombo = new List<ItemCombo>();

                                //Inclusões
                                if (icomboI != null && icomboI.Count() > 0)
                                {
                                    foreach (ItemComboApp e in icomboI)
                                    {
                                        icombo.Add(new ItemCombo
                                        {
                                            id = e.id_server,
                                            id_empresa = company,
                                            id_produto = e.id_produto,
                                            int_qtd_item = e.int_qtd_item,
                                            id_app = e.id,
                                            id_user_man = user
                                        });
                                    }

                                    str_ret = repData.ManutencaoTabela<ItemCombo>("I", icombo, "ntv_tbl_itemcombo", conn, tran);
                                    dyn_ret = str_ret;
                                }

                                //Alterações
                                if (icomboU != null && icomboU.Count() > 0)
                                {
                                    icombo.Clear();

                                    foreach (ItemComboApp e in icomboU)
                                    {
                                        icombo.Add(new ItemCombo
                                        {
                                            id = e.id_server,
                                            id_empresa = company,
                                            id_produto = e.id_produto,
                                            int_qtd_item = e.int_qtd_item,
                                            id_app = e.id,
                                            id_user_man = user
                                        });
                                    }
                                    str_ret = repData.ManutencaoTabela<ItemCombo>("U", icombo, "ntv_tbl_itemcombo", conn, tran);
                                }
                                dyn_ret = str_ret;
                                break;

                            case "Grupo":
                                List<GrupoApp> grpApp = new List<GrupoApp>();
                                grpApp = JsonConvert.DeserializeObject<List<GrupoApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));
                                var grpI = from n in grpApp where n.id_server == 0 select n;
                                var grpU = from n in grpApp where n.id_server > 0 select n;

                                List<Grupo> grp = new List<Grupo>();

                                //Inclusões
                                if (grpI != null && grpI.Count() > 0)
                                {
                                    foreach (GrupoApp e in grpI)
                                    {
                                        grp.Add(new Grupo
                                        {
                                            id = e.id_server,
                                            id_empresa = company,
                                            str_descricao = e.str_descricao,
                                            str_foto = e.str_foto,
                                            dtm_inclusao = e.dtm_inclusao,
                                            dtm_alteracao = e.dtm_alteracao,
                                            int_situacao = e.int_situacao,
                                            id_app = e.id,
                                            id_user_man = user
                                        });
                                    }

                                    str_ret = repData.ManutencaoTabela<Grupo>("I", grp, "ntv_tbl_grupo", conn, tran);
                                    dyn_ret = str_ret;
                                }

                                //Alterações
                                if (grpU != null && grpU.Count() > 0)
                                {
                                    grp.Clear();

                                    foreach (GrupoApp e in grpU)
                                    {
                                        grp.Add(new Grupo
                                        {
                                            id = e.id_server,
                                            id_empresa = company,
                                            str_descricao = e.str_descricao,
                                            str_foto = e.str_foto,
                                            dtm_inclusao = e.dtm_inclusao,
                                            dtm_alteracao = e.dtm_alteracao,
                                            int_situacao = e.int_situacao,
                                            id_app = e.id,
                                            id_user_man = user
                                        });
                                    }

                                    str_ret = repData.ManutencaoTabela<Grupo>("U", grp, "ntv_tbl_grupo", conn, tran);
                                }
                                dyn_ret = str_ret;
                                break;

                            case "FormaPag":
                                List<FormaPagApp> formaApp = new List<FormaPagApp>();
                                formaApp = JsonConvert.DeserializeObject<List<FormaPagApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));
                                var formaI = from n in formaApp where n.id_server == 0 select n;
                                var formaU = from n in formaApp where n.id_server > 0 select n;

                                List<FormaPag> forma = new List<FormaPag>();

                                //Inclusões
                                if (formaI != null && formaI.Count() > 0)
                                {
                                    foreach (FormaPagApp e in formaI)
                                    {
                                        forma.Add(new FormaPag
                                        {
                                            id = e.id_server,
                                            id_empresa = company,
                                            str_descricao = e.str_descricao,
                                            dtm_inclusao = e.dtm_inclusao,
                                            dtm_alteracao = e.dtm_alteracao,
                                            int_situacao = e.int_situacao,
                                            id_app = e.id,
                                            id_user_man = user
                                        });
                                    }

                                    str_ret = repData.ManutencaoTabela<FormaPag>("I", forma, "ntv_tbl_formapag", conn, tran);
                                    dyn_ret = str_ret;
                                }

                                //Alterações
                                if (formaU != null && formaU.Count() > 0)
                                {
                                    forma.Clear();

                                    foreach (FormaPagApp e in formaU)
                                    {
                                        forma.Add(new FormaPag
                                        {
                                            id = e.id_server,
                                            id_empresa = company,
                                            str_descricao = e.str_descricao,
                                            dtm_inclusao = e.dtm_inclusao,
                                            dtm_alteracao = e.dtm_alteracao,
                                            int_situacao = e.int_situacao,
                                            id_app = e.id,
                                            id_user_man = user
                                        });
                                    }

                                    str_ret = repData.ManutencaoTabela<FormaPag>("U", forma, "ntv_tbl_formapag", conn, tran);
                                }
                                dyn_ret = str_ret;
                                break;

                            case "Local":
                                List<LocalApp> locApp = new List<LocalApp>();
                                locApp = JsonConvert.DeserializeObject<List<LocalApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));
                                var locI = from n in locApp where n.id_server == 0 select n;
                                var locU = from n in locApp where n.id_server > 0 select n;

                                List<Local> loc = new List<Local>();

                                //Inclusões
                                if (locI != null && locI.Count() > 0)
                                {
                                    foreach (LocalApp e in locI)
                                    {
                                        loc.Add(new Local
                                        {
                                            id = e.id_server,
                                            id_empresa = company,
                                            int_numero = e.int_numero,
                                            int_tipo = e.int_tipo,
                                            int_qtd_pess = e.int_qtd_pess,
                                            str_foto = e.str_foto,
                                            dtm_inclusao = e.dtm_inclusao,
                                            dtm_alteracao = DateTime.Now,
                                            int_situacao = e.int_situacao,
                                            id_app = e.id,
                                            id_user_man = user
                                        });
                                    }

                                    str_ret = repData.ManutencaoTabela<Local>("I", loc, "ntv_tbl_local", conn, tran);
                                    dyn_ret = str_ret;
                                }

                                //Alterações
                                if (locU != null && locU.Count() > 0)
                                {
                                    loc.Clear();

                                    foreach (LocalApp e in locU)
                                    {
                                        loc.Add(new Local
                                        {
                                            id = e.id_server,
                                            id_empresa = company,
                                            int_numero = e.int_numero,
                                            int_tipo = e.int_tipo,
                                            int_qtd_pess = e.int_qtd_pess,
                                            str_foto = e.str_foto,
                                            dtm_inclusao = e.dtm_inclusao,
                                            dtm_alteracao = DateTime.Now,
                                            int_situacao = e.int_situacao,
                                            id_app = e.id,
                                            id_user_man = user
                                        });
                                    }
                                    str_ret = repData.ManutencaoTabela<Local>("U", loc, "ntv_tbl_local", conn, tran);
                                }
                                dyn_ret = str_ret;
                                break;

                            case "LocalCliente":
                                List<LocalClienteApp> locCliApp = new List<LocalClienteApp>();
                                locCliApp = JsonConvert.DeserializeObject<List<LocalClienteApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));
                                var locCliI = from n in locCliApp where n.id_server == 0 select n;
                                var locCliU = from n in locCliApp where n.id_server > 0 select n;

                                List<LocalCliente> locCli = new List<LocalCliente>();

                                //Inclusões
                                if (locCliI != null && locCliI.Count() > 0)
                                {
                                    foreach (LocalClienteApp e in locCliI)
                                    {
                                        locCli.Add(new LocalCliente
                                        {
                                            id = e.id_server,
                                            id_empresa = company,
                                            id_local = e.id_local,
                                            str_cliente = e.str_cliente,
                                            str_senha = e.str_senha,
                                            int_qtdped = e.int_qtdped,
                                            int_qtditem = e.int_qtditem,
                                            dbl_val_tot = e.dbl_val_tot,
                                            dbl_val_desc = e.dbl_val_desc,
                                            dbl_val_liq = e.dbl_val_liq,
                                            dbl_val_pag = e.dbl_val_pag,
                                            dtm_inclusao = e.dtm_inclusao,
                                            dtm_pagto = e.dtm_pagto,
                                            dtm_cancel = e.dtm_cancel,
                                            int_situacao = e.int_situacao,
                                            id_app = e.id,
                                            id_user_man = user
                                        });
                                    }

                                    str_ret = repData.ManutencaoTabela<LocalCliente>("I", locCli, "ntv_tbl_localcliente", conn, tran);
                                    dyn_ret = str_ret;
                                }

                                //Alterações
                                if (locCliU != null && locCliU.Count() > 0)
                                {
                                    locCli.Clear();

                                    foreach (LocalClienteApp e in locCliI)
                                    {
                                        locCli.Add(new LocalCliente
                                        {
                                            id = e.id_server,
                                            id_empresa = company,
                                            id_local = e.id_local,
                                            str_cliente = e.str_cliente,
                                            str_senha = e.str_senha,
                                            int_qtdped = e.int_qtdped,
                                            int_qtditem = e.int_qtditem,
                                            dbl_val_tot = e.dbl_val_tot,
                                            dbl_val_desc = e.dbl_val_desc,
                                            dbl_val_liq = e.dbl_val_liq,
                                            dbl_val_pag = e.dbl_val_pag,
                                            dtm_inclusao = e.dtm_inclusao,
                                            dtm_pagto = e.dtm_pagto,
                                            dtm_cancel = e.dtm_cancel,
                                            int_situacao = e.int_situacao,
                                            id_app = e.id,
                                            id_user_man = user
                                        });
                                    }
                                    str_ret = repData.ManutencaoTabela<LocalCliente>("U", locCli, "ntv_tbl_localcliente", conn, tran);
                                }
                                dyn_ret = str_ret;
                                break;

                            case "Pedido":
                                List<PedidoApp> pedApp = new List<PedidoApp>();
                                pedApp = JsonConvert.DeserializeObject<List<PedidoApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));
                                var pedI = from n in pedApp where n.id_server == 0 select n;
                                var pedU = from n in pedApp where n.id_server > 0 select n;

                                List<Pedido> ped = new List<Pedido>();

                                //Inclusões
                                if (pedI != null && pedI.Count() > 0)
                                {
                                    foreach (PedidoApp e in pedI)
                                    {
                                        ped.Add(new Pedido
                                        {
                                            id = e.id_server,
                                            id_empresa = company,
                                            id_localcli = e.id_localcli,
                                            dtm_pedido = e.dtm_pedido,
                                            dtm_pagto = e.dtm_pagto,
                                            dtm_cancel = e.dtm_cancel,
                                            int_qtd_item = e.int_qtd_item,
                                            dbl_val_tot = e.dbl_val_tot,
                                            dbl_val_desc = e.dbl_val_desc,
                                            dbl_val_liq = e.dbl_val_liq,
                                            dbl_val_pag = e.dbl_val_pag,
                                            str_obs = e.str_obervacao,
                                            int_situacao = e.int_situacao,
                                            id_usuario = e.id_usuario,
                                            id_app = e.id,
                                            id_user_man = user
                                        });
                                    }

                                    str_ret = repData.ManutencaoTabela<Pedido>("I", ped, "ntv_tbl_pedido", conn, tran);
                                    dyn_ret = str_ret;
                                }

                                //Alterações
                                if (pedU != null && pedU.Count() > 0)
                                {
                                    ped.Clear();

                                    foreach (PedidoApp e in pedI)
                                    {
                                        ped.Add(new Pedido
                                        {
                                            id = e.id_server,
                                            id_empresa = company,
                                            id_localcli = e.id_localcli,
                                            dtm_pedido = e.dtm_pedido,
                                            dtm_pagto = e.dtm_pagto,
                                            dtm_cancel = e.dtm_cancel,
                                            int_qtd_item = e.int_qtd_item,
                                            dbl_val_tot = e.dbl_val_tot,
                                            dbl_val_desc = e.dbl_val_desc,
                                            dbl_val_liq = e.dbl_val_liq,
                                            dbl_val_pag = e.dbl_val_pag,
                                            str_obs = e.str_obervacao,
                                            int_situacao = e.int_situacao,
                                            id_usuario = e.id_usuario,
                                            id_app = e.id,
                                            id_user_man = user
                                        });
                                    }
                                    str_ret = repData.ManutencaoTabela<Pedido>("U", ped, "ntv_tbl_pedido", conn, tran);
                                }
                                dyn_ret = str_ret;
                                break;

                            case "PedidoItem":
                                List<PedidoItemApp> pedIApp = new List<PedidoItemApp>();
                                pedIApp = JsonConvert.DeserializeObject<List<PedidoItemApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));
                                var pedII = from n in pedIApp where n.id_server == 0 select n;
                                var pedIU = from n in pedIApp where n.id_server > 0 select n;

                                List<PedidoItem> pedIt = new List<PedidoItem>();

                                //Inclusões
                                if (pedII != null && pedII.Count() > 0)
                                {
                                    foreach (PedidoItemApp e in pedII)
                                    {
                                        pedIt.Add(new PedidoItem
                                        {
                                            id = e.id_server,
                                            id_empresa = company,
                                            id_pedido = e.id_pedido,
                                            id_produto = e.id_produto,
                                            dbl_precounit = e.dbl_precounit,
                                            int_qtd_comp = e.int_qtd_comp,
                                            dbl_tot_item = e.dbl_tot_item,
                                            dbl_desconto = e.dbl_desconto,
                                            dbl_tot_liq = e.dbl_tot_liq,
                                            int_situacao = e.int_situacao,
                                            id_app = e.id,
                                            id_user_man = user
                                        });
                                    }

                                    str_ret = repData.ManutencaoTabela<PedidoItem>("I", pedIt, "ntv_tbl_pedidoitem", conn, tran);
                                    dyn_ret = str_ret;
                                }

                                //Alterações
                                if (pedIU != null && pedIU.Count() > 0)
                                {
                                    pedIt.Clear();

                                    foreach (PedidoItemApp e in pedII)
                                    {
                                        pedIt.Add(new PedidoItem
                                        {
                                            id = e.id_server,
                                            id_empresa = company,
                                            id_pedido = e.id_pedido,
                                            id_produto = e.id_produto,
                                            dbl_precounit = e.dbl_precounit,
                                            int_qtd_comp = e.int_qtd_comp,
                                            dbl_tot_item = e.dbl_tot_item,
                                            dbl_desconto = e.dbl_desconto,
                                            dbl_tot_liq = e.dbl_tot_liq,
                                            int_situacao = e.int_situacao,
                                            id_app = e.id,
                                            id_user_man = user
                                        });
                                    }
                                    str_ret = repData.ManutencaoTabela<PedidoItem>("U", pedIt, "ntv_tbl_pedidoitem", conn, tran);
                                }
                                dyn_ret = str_ret;
                                break;

                            case "LocalCliPag":
                                List<LocalCliPagApp> cliPApp = new List<LocalCliPagApp>();
                                cliPApp = JsonConvert.DeserializeObject<List<LocalCliPagApp>>(data.Dados.ToString().Replace("','", "\",\"").Replace("':'", "\":\"").Replace("':", "\":").Replace(",'", ",\"").Replace("{'", "{\"").Replace("'}", "\"}"));
                                var cliPI = from n in cliPApp where n.id_server == 0 select n;
                                var cliPU = from n in cliPApp where n.id_server > 0 select n;

                                List<LocalCliPag> cliP = new List<LocalCliPag>();

                                //Inclusões
                                if (cliPI != null && cliPI.Count() > 0)
                                {
                                    foreach (LocalCliPagApp e in cliPI)
                                    {
                                        cliP.Add(new LocalCliPag
                                        {
                                            id = e.id_server,
                                            id_empresa = company,
                                            id_localcli = e.id_localcli,
                                            id_formapag = e.id_formapag,
                                            dtm_pagto = e.dtm_pagto,
                                            dtm_cancel = e.dtm_cancel,
                                            dbl_val_pgto = e.dbl_val_pgto,
                                            dbl_desconto = e.dbl_desconto,
                                            id_app = e.id,
                                            id_user_man = user
                                        });
                                    }

                                    str_ret = repData.ManutencaoTabela<LocalCliPag>("I", cliP, "ntv_tbl_localclipag", conn, tran);
                                    dyn_ret = str_ret;
                                }

                                //Alterações
                                if (cliPU != null && cliPU.Count() > 0)
                                {
                                    cliP.Clear();

                                    foreach (LocalCliPagApp e in cliPI)
                                    {
                                        cliP.Add(new LocalCliPag
                                        {
                                            id = e.id_server,
                                            id_empresa = company,
                                            id_localcli = e.id_localcli,
                                            id_formapag = e.id_formapag,
                                            dtm_pagto = e.dtm_pagto,
                                            dtm_cancel = e.dtm_cancel,
                                            dbl_val_pgto = e.dbl_val_pgto,
                                            dbl_desconto = e.dbl_desconto,
                                            id_app = e.id,
                                            id_user_man = user
                                        });
                                    }
                                    str_ret = repData.ManutencaoTabela<LocalCliPag>("U", cliP, "ntv_tbl_localclipag", conn, tran);
                                }
                                dyn_ret = str_ret;
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
