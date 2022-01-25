using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            List<UsuarioApp> usuApp = new List<UsuarioApp>();

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

                                str_operacao = (empApp[0].id_server > 0 ?"U":"I");

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
                                id_emp = Convert.ToInt64(repData.ManutencaoTabela<Empresa>(str_operacao, emp, "ntv_tbl_empresa", conn, tran).Split(";")[0]);
                                dyn_ret = id_emp;
                                break;

                            case "Usuario":
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

                                    List<UsuarioApp> usuRet = new List<UsuarioApp>();
                                    usuRet = usuI.Cast<UsuarioApp>().ToList();

                                    for (int r = 0; r < str_ret.Split(";").Length; r++)
                                    {
                                        if (str_ret.Split(";")[r] != null && str_ret.Split(";")[r] != "")
                                        {
                                            usuRet[r].id_server = Convert.ToInt64(str_ret.Split(";")[r]);
                                        }
                                    }
                                    dyn_ret = usuRet;
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
