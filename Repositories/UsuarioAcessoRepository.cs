using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using WebAppServer.Models;

namespace WebAppServer.Repositories
{
    public class UsuarioAcessoRepository
    {
        private readonly ConfigDB configDB;
        private readonly EmailRepository _repEmail;

        public UsuarioAcessoRepository(EmailRepository repMail)
        {
            configDB = new ConfigDB();
            _repEmail = repMail;
        }

        public async Task<IEnumerable<UserPrimAcess>> PrimeiroAcessoEmp(string str_mail)
        {
            DataRepository repData = new DataRepository();
            bool bol_ret = true;
            Int64 id_emp = 0;
            Int64 id_prim = 0;
            string str_ret = "";
            string str_corpo = "";

            List<Empresa> empresa = new List<Empresa>();
            List<UserPrimAcess> primAcesses = new List<UserPrimAcess>();

            empresa.Add(new Empresa
            {
                id = 0,
                int_cgccpf = 0,
                str_nome = "",
                str_fantasia = "",
                str_email = str_mail,
                int_telefone = 0,
                int_local_atend = 0,
                int_id_user_adm = 0,
                dtm_inclusao = DateTime.Now,
                int_situacao = 0,
                int_sitpag = 0,
                dtm_ultpag = null,
                id_app = 0
            });

            using (SqlConnection conn = new SqlConnection(configDB.ConnectString))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        id_emp = Convert.ToInt64(repData.ManutencaoTabela<Empresa>("I", empresa, "ntv_tbl_empresa", conn, tran).Split(";")[0]);
                        if (id_emp > 0)
                        {
                            primAcesses.Add(new UserPrimAcess
                            {
                                id = 0,
                                id_empresa = id_emp,
                                id_emp_serv = id_emp,
                                id_user_app = 0,
                                str_email = str_mail,
                                dtm_acesso = DateTime.Now,
                                int_cod_acesso = 0,
                                dtm_confirma = null,
                                int_situacao = 0
                            });

                            id_prim = Convert.ToInt64(repData.ManutencaoTabela<UserPrimAcess>("I", primAcesses, "ntv_tbl_prim_acess", conn, tran).Split(";")[0]);

                            str_ret = repData.ConsultaGenerica("[{ \"nome\":\"id\", \"valor\":\"" + id_prim.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                               "{ \"nome\":\"id_empresa\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                               "{ \"nome\":\"id_emp_serv\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                               "{ \"nome\":\"Email\", \"valor\":\"\", \"tipo\":\"string\"}," +
                                                               "{ \"nome\":\"CodAcesso\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                               "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}]", "ntv_p_sel_tbl_prim_acess", conn, tran);
                            primAcesses = JsonConvert.DeserializeObject<List<UserPrimAcess>>(str_ret);

                            if (primAcesses.Count > 0)
                            {
                                str_corpo += "<p style = 'font-family:Arial; font-size:120%; font-weight:bold;' > TERMO DE ADESÃO ON-LINE</p>";
                                str_corpo += "<br/>";
                                str_corpo += "<p style = 'font-family:Arial; font-size:120%; font-weight:bold;' > Prezado Cliente,</p>";
                                str_corpo += "<br/>";
                                str_corpo += "<p style = 'font-family:Arial; font-size:100%; ' > Segue número de aceite para confirmação do acesso:</p>";
                                str_corpo += "<p style = 'font-family:Arial; font-size:100%; font-weight:bold;' > " + String.Format("{0:0000}", primAcesses[0].int_cod_acesso) + "</p>";
                                str_corpo += "<p style = 'font-family:Arial; font-size:100%; ' > Atenciosamente,</p>";
                                str_corpo += "<p style = 'font-family:Arial; font-size:100%; ' > Natividade Soluções em TI</p>";

                                List<envEmail> emails = new List<envEmail>();
                                emails.Add(new envEmail
                                {
                                    id = 0,
                                    str_de = "suporte@natividadesolucoes.com.br",
                                    str_para = str_mail,
                                    str_cc = "",
                                    str_ass = "Adesão ao APP na Areia",
                                    str_msg = str_corpo,
                                    str_html = "S",
                                    dtm_data_inc = DateTime.Now,
                                    str_modulo = "Primeiro Acesso WebAppServer.Data.Usuario.Repositories",
                                    str_obs = "",
                                    int_situacao = 0

                                });
                                _repEmail.EnviarEmails(emails);
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

            return primAcesses;


        }

        public async Task<IEnumerable<dynamic>> VerificaUsuario(string str_mail)
        {
            DataRepository repData = new DataRepository();
            bool bol_ret = true;
            Int64 id_emp = 0;
            Int64 id_prim = 0;
            string str_ret = "";
            string str_corpo = "";
            dynamic dyn_ret = null;

            using (SqlConnection conn = new SqlConnection(configDB.ConnectString))
            {                
                try
                {
                    conn.Open();

                    //Verifica se o é primeiro acesso
                    str_ret = repData.ConsultaGenerica("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                        "{ \"nome\":\"id_empresa\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                        "{ \"nome\":\"id_emp_serv\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                        "{ \"nome\":\"Email\", \"valor\":\"" + str_mail + "\", \"tipo\":\"string\"}," +
                                                        "{ \"nome\":\"CodAcesso\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                        "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}]", "ntv_p_sel_tbl_prim_acess", conn);
                    dyn_ret = JsonConvert.DeserializeObject<List<UserPrimAcess>>(str_ret);

                    if (dyn_ret.Count == 0)
                    {
                        //Verifica se o usuário esta ativo 
                        str_ret = repData.ConsultaGenerica("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                            "{ \"nome\":\"id_empresa\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                            "{ \"nome\":\"login\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                            "{ \"nome\":\"email\", \"valor\":\"" + str_mail + "\", \"tipo\":\"string\"}," +
                                                            "{ \"nome\":\"situacao\", \"valor\":\"1\", \"tipo\":\"Int16\"}," +
                                                            "{ \"nome\":\"download\", \"valor\":\"0\", \"tipo\":\"Int16\"}, " +
                                                            "{ \"nome\":\"id_app\", \"valor\":\"0\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_usuario", conn);
                        dyn_ret = JsonConvert.DeserializeObject<List<Usuario>>(str_ret);
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();
                    throw ex;
                }
                conn.Close();
            }

            return dyn_ret;


        }

        public async Task<IEnumerable<UserPrimAcess>> ValPrimeiroAcesso(string str_mail, Int64 codigo)
        {

            DataRepository repData = new DataRepository();

            bool bol_ret = false;

            string str_ret = "";
            List<UserPrimAcess> primAcesses = new List<UserPrimAcess>();                        

            using (SqlConnection conn = new SqlConnection(configDB.ConnectString))
            {                
                conn.Open();
                str_ret = repData.ConsultaGenerica("[{ \"nome\":\"ID\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                    "{ \"nome\":\"id_empresa\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                    "{ \"nome\":\"id_emp_serv\", \"valor\":\"0\", \"tipo\":\"Int64\"}," + 
                                                    "{ \"nome\":\"Email\", \"valor\":\"" + str_mail + "\", \"tipo\":\"string\"}," +
                                                    "{ \"nome\":\"CodAcesso\", \"valor\":\"" + codigo.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                    "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}]", "ntv_p_sel_tbl_prim_acess", conn);
                if (str_ret != "[]")
                {
                    primAcesses = JsonConvert.DeserializeObject<List<UserPrimAcess>>(str_ret);
                    if (primAcesses.Count > 0)
                    {
                        dynamic user = await AtuPrimeiroAcesso(primAcesses[0]);
                    }
                }

                conn.Close();
            }
            return primAcesses;
        }

        public async Task<IEnumerable<dynamic>> AtuPrimeiroAcesso(UserPrimAcess primAcess)
        {

            DataRepository repData = new DataRepository();
            List<UserPrimAcess> prim = new List<UserPrimAcess>();
            List<Usuario> usu = new List<Usuario>();
            dynamic dyn_ret = null;

            Int64 int6_ret = 0;

            if (primAcess != null)
            {
                using (SqlConnection conn = new SqlConnection(configDB.ConnectString))
                {
                    conn.Open();
                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            primAcess.dtm_confirma = DateTime.Now;
                            primAcess.int_situacao = 1;

                            List<UserPrimAcess> primAcesses = new List<UserPrimAcess>();

                            primAcesses.Add(primAcess);

                            int6_ret = Convert.ToInt64(repData.ManutencaoTabela<UserPrimAcess>("U", primAcesses, "ntv_tbl_prim_acess", conn, tran).Split(";")[0]);
                            if (int6_ret > 0)
                            {
                                if (primAcesses[0].id_user_app > 0)
                                {
                                    string str_ret = repData.ConsultaGenerica("[{ \"nome\":\"id\", \"valor\":\"" + primAcesses[0].id_user_app + "\", \"tipo\":\"Int64\"}," +
                                                                       "{ \"nome\":\"id_empresa\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                                       "{ \"nome\":\"login\", \"valor\":\"\", \"tipo\":\"Int64\"}," +
                                                                       "{ \"nome\":\"Email\", \"valor\":\"\", \"tipo\":\"string\"}," +
                                                                       "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}, " +
                                                                       "{ \"nome\":\"download\", \"valor\":\"0\", \"tipo\":\"Int16\"}, " +
                                                                       "{ \"nome\":\"id_app\", \"valor\":\"0\", \"tipo\":\"Int64\"}]", "ntv_p_sel_tbl_usuario", conn, tran);
                                    if (str_ret.Length > 0 && str_ret != "[]")
                                    {
                                        usu = JsonConvert.DeserializeObject<List<Usuario>>(str_ret);

                                        if (usu[0].int_situacao == 0) {
                                            usu[0].int_situacao = 1;
                                            int6_ret = Convert.ToInt64(repData.ManutencaoTabela<Usuario>("U", usu, "ntv_tbl_usuario", conn, tran).Split(";")[0]);
                                        }

                                        dyn_ret = usu;
                                    }

                                }
                                else
                                {
                                    string str_ret = repData.ConsultaGenerica("[{ \"nome\":\"id\", \"valor\":\"" + int6_ret.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                                       "{ \"nome\":\"id_empresa\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                                       "{ \"nome\":\"id_emp_serv\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                                       "{ \"nome\":\"Email\", \"valor\":\"\", \"tipo\":\"string\"}," +
                                                                       "{ \"nome\":\"CodAcesso\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                                                       "{ \"nome\":\"situacao\", \"valor\":\"1\", \"tipo\":\"Int16\"}]", "ntv_p_sel_tbl_prim_acess", conn, tran);
                                    if (str_ret.Length > 0 && str_ret != "[]")
                                    {
                                        prim = JsonConvert.DeserializeObject<List<UserPrimAcess>>(str_ret);
                                        dyn_ret = prim;
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
            }

            return dyn_ret;
        }
        public async Task<IEnumerable<UserPrimAcess>> PrimeiroAcessoEmpPostgres(string str_mail)
        {
            DataRepository repData = new DataRepository();
            DataTable dtt_retorno = new DataTable();
            bool bol_ret = true;
            Int64 id_emp = 0;
            Int64 id_prim = 0;
            string str_ret = "";
            string str_corpo = "";

            List<Empresa> empresa = new List<Empresa>();
            List<UserPrimAcess> primAcesses = new List<UserPrimAcess>();

            empresa.Add(new Empresa
            {
                id = 0,
                int_cgccpf = 0,
                str_nome = "",
                str_fantasia = "",
                str_email = str_mail,
                int_telefone = 0,
                int_local_atend = 0,
                int_id_user_adm = 0,
                dtm_inclusao = DateTime.Now,
                int_situacao = 0,
                int_sitpag = 0,
                dtm_ultpag = null,
                id_app = 0
            });

            using (NpgsqlConnection conn = new NpgsqlConnection(configDB.ConnectString))
            {
                conn.Open();
                using (NpgsqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        str_ret = JsonConvert.SerializeObject(empresa);
                        dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);

                        //Incluir coluna da operação a ser executada
                        if (dtt_retorno.Columns.Count > 0)
                        {
                            if (!dtt_retorno.Columns.Contains("str_operation")){
                                dtt_retorno.Columns.Add(new DataColumn("str_operation", System.Type.GetType("System.String")));
                                dtt_retorno.Rows[0]["str_operation"] = "I";
                            }
                            str_ret = JsonConvert.SerializeObject(dtt_retorno);

                            string sqlStr = "select * from f_man_tbl_ntv_tbl_empresa('{\"dados\": " + str_ret + "}') as id";
                            str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, tran);
                            dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);
                            if (dtt_retorno.Columns.Count > 0)
                            {
                                //Grava configurações
                                List<EmpresaConfig> empresaConfig = new List<EmpresaConfig>();

                                empresaConfig.Add(new EmpresaConfig
                                {
                                    id_empresa = Convert.ToInt64(dtt_retorno.Rows[0]["id"].ToString().Replace(";", "")),
                                    str_impressora = "",
                                    str_imp_ped = "",
                                    str_imp_conta = "",
                                    str_conf_rec = "",
                                    str_conf_pronto = "",
                                    str_conf_ent = "N",
                                    dbl_perc_serv = 0,
                                    id_app = 0,
                                    id_user_man = 0
                                });

                                //Gera primeiro acesso
                                primAcesses.Add(new UserPrimAcess
                                {
                                    id = 0,
                                    id_empresa = Convert.ToInt64(dtt_retorno.Rows[0]["id"].ToString().Replace(";", "")),
                                    id_emp_serv = Convert.ToInt64(dtt_retorno.Rows[0]["id"].ToString().Replace(";", "")),
                                    id_user_app = 0,
                                    str_email = str_mail,
                                    dtm_acesso = DateTime.Now,
                                    int_cod_acesso = 0,
                                    dtm_confirma = null,
                                    int_situacao = 0
                                });

                                str_ret = JsonConvert.SerializeObject(empresaConfig);
                                dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);

                                if (dtt_retorno.Columns.Count > 0)
                                {
                                    if (!dtt_retorno.Columns.Contains("str_operation"))
                                    {
                                        dtt_retorno.Columns.Add(new DataColumn("str_operation", System.Type.GetType("System.String")));
                                        dtt_retorno.Rows[0]["str_operation"] = "I";
                                    }
                                    str_ret = JsonConvert.SerializeObject(dtt_retorno);
                                }

                                sqlStr = "select * from f_man_tbl_ntv_tbl_empresa_config('{\"dados\": " + str_ret + "}') as id";
                                str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, tran);

                                str_ret = JsonConvert.SerializeObject(primAcesses);
                                dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);

                                if (dtt_retorno.Columns.Count > 0)
                                {
                                    if (!dtt_retorno.Columns.Contains("str_operation"))
                                    {
                                        dtt_retorno.Columns.Add(new DataColumn("str_operation", System.Type.GetType("System.String")));
                                        dtt_retorno.Rows[0]["str_operation"] = "I";
                                    }
                                    str_ret = JsonConvert.SerializeObject(dtt_retorno);
                                }

                                sqlStr = "select * from f_man_tbl_ntv_tbl_prim_acess('{\"dados\": " + str_ret + "}') as id";
                                str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, tran);
                                dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);

                                if (dtt_retorno.Rows.Count > 0)
                                {
                                    sqlStr = "select * from f_sel_tbl_ntv_tbl_prim_acess(" + dtt_retorno.Rows[0]["id"].ToString().Replace(";","") + ",0,'',0)";

                                    str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, tran);
                                    dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);
                                    if (dtt_retorno.Rows.Count > 0)
                                    {
                                        str_corpo += "<p style = 'font-family:Arial; font-size:120%; font-weight:bold;' > TERMO DE ADESÃO ON-LINE</p>";
                                        str_corpo += "<br/>";
                                        str_corpo += "<p style = 'font-family:Arial; font-size:120%; font-weight:bold;' > Prezado Cliente,</p>";
                                        str_corpo += "<br/>";
                                        str_corpo += "<p style = 'font-family:Arial; font-size:100%; ' > Segue número de aceite para confirmação do acesso:</p>";
                                        str_corpo += "<p style = 'font-family:Arial; font-size:100%; font-weight:bold;' > " + String.Format("{0:0000}", dtt_retorno.Rows[0]["int_cod_acesso"].ToString()) + "</p>";
                                        str_corpo += "<p style = 'font-family:Arial; font-size:100%; ' > Atenciosamente,</p>";
                                        str_corpo += "<p style = 'font-family:Arial; font-size:100%; ' > Natividade Soluções em TI</p>";

                                        primAcesses[0].int_cod_acesso = Convert.ToInt64(dtt_retorno.Rows[0]["int_cod_acesso"]);
                                        primAcesses[0].id = Convert.ToInt64(dtt_retorno.Rows[0]["id"]);

                                        List<envEmail> emails = new List<envEmail>();
                                        emails.Add(new envEmail
                                        {
                                            id = 0,
                                            str_de = "suporte@natividadesolucoes.com.br",
                                            str_para = str_mail,
                                            str_cc = "",
                                            str_ass = "Adesão ao APP na Areia",
                                            str_msg = str_corpo,
                                            str_html = "S",
                                            dtm_data_inc = DateTime.Now,
                                            str_modulo = "Primeiro Acesso WebAppServer.Data.Usuario.Repositories",
                                            str_obs = "",
                                            int_situacao = 0

                                        });
                                        _repEmail.EnviarEmailsPostgres(emails);
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

            return primAcesses;


        }

        public async Task<IEnumerable<dynamic>> VerificaUsuarioPostgres(string str_mail)
        {
            DataRepository repData = new DataRepository();
            DataTable dtt_retorno = new DataTable();
            bool bol_ret = true;
            Int64 id_emp = 0;
            Int64 id_prim = 0;
            string str_ret = "";
            string str_corpo = "";
            dynamic dyn_ret = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(configDB.ConnectString))
            {
                try
                {
                    conn.Open();

                    //Verifica se o é primeiro acesso
                    string sqlStr = "select * from f_sel_tbl_ntv_tbl_prim_acess(0,0," + str_mail + ",0)";
                    str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, null);
                    dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);

                    if (dtt_retorno.Columns.Count > 0 && dtt_retorno.Rows.Count > 0)
                    {
                        //Verifica se o usuário esta ativo 
                        sqlStr = "select * from f_sel_tbl_ntv_tbl_usuario(0,0," + str_mail + ",'',1)";
                        str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, null);                        
                    }

                    dyn_ret = JsonConvert.DeserializeObject<List<Usuario>>(str_ret);
                }
                catch (Exception ex)
                {
                    conn.Close();
                    throw ex;
                }
                conn.Close();
            }

            return dyn_ret;


        }

        public async Task<IEnumerable<UserPrimAcess>> ValPrimeiroAcessoPostgres(string str_mail, Int64 codigo)
        {

            DataRepository repData = new DataRepository();            

            bool bol_ret = false;

            string str_ret = "";
            List<UserPrimAcess> primAcesses = new List<UserPrimAcess>();

            using (NpgsqlConnection conn = new NpgsqlConnection(configDB.ConnectString))
            {
                conn.Open();
                string sqlStr = "select * from f_sel_tbl_ntv_tbl_prim_acess(0,0," + str_mail + "," + codigo.ToString() + ",-1)";
                str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, null);
                if (str_ret != "[]")
                {
                    primAcesses = JsonConvert.DeserializeObject<List<UserPrimAcess>>(str_ret);
                    if (primAcesses.Count > 0)
                    {
                        dynamic user = await AtuPrimeiroAcessoPostgres(primAcesses[0]);
                    }
                }

                conn.Close();
            }
            return primAcesses;
        }

        public async Task<IEnumerable<dynamic>> AtuPrimeiroAcessoPostgres(UserPrimAcess primAcess)
        {

            DataRepository repData = new DataRepository();
            DataTable dtt_retorno = new DataTable();

            List<UserPrimAcess> prim = new List<UserPrimAcess>();
            List<Usuario> usu = new List<Usuario>();
            dynamic dyn_ret = null;

            Int64 int6_ret = 0;

            if (primAcess != null)
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(configDB.ConnectString))
                {
                    conn.Open();
                    using (NpgsqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            primAcess.dtm_confirma = DateTime.Now;
                            primAcess.int_situacao = 1;

                            List<UserPrimAcess> primAcesses = new List<UserPrimAcess>();

                            primAcesses.Add(primAcess);
                            string str_ret = JsonConvert.SerializeObject(primAcesses);

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

                            }

                            string sqlStr = "select * from f_man_tbl_ntv_tbl_prim_acess('{\"dados\": " + str_ret + "}') as id";
                            str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, tran);
                            dtt_retorno = JsonConvert.DeserializeObject<DataTable>(str_ret);

                            if (dtt_retorno.Columns.Count > 0 && dtt_retorno.Rows.Count > 0) {
                                /*if (Convert.ToInt64(dtt_retorno.Rows[0]["id_user_app"]) > 0)
                                {
                                    //Atualiza usuário
                                    sqlStr = "select * from f_sel_tbl_ntv_tbl_usuario(" + dtt_retorno.Rows[0]["id_user_app"] + ",0,'','',1)";
                                    str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, null);

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
                                        sqlStr = "select * from f_man_tbl_ntv_tbl_usuario('{\"dados\": " + str_ret + "}') as id";
                                        str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, null);
                                    }
                                }
                                else
                                {
                                    sqlStr = "select * from f_sel_tbl_ntv_tbl_prim_acess(" + dtt_retorno.Rows[0]["id"]+ ",0,'',0,-1)";
                                    str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, null);
                                }*/
                                dyn_ret = primAcesses;
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
            }

            return dyn_ret;
        }
    }
}
