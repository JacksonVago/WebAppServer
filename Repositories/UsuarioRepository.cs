using WebAppServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;
using Npgsql;
using System.ComponentModel.DataAnnotations;

namespace WebAppServer.Repositories
{
    
    public class UsuarioRepository
    {
        private readonly ConfigDB configDB;
        private readonly EmailRepository _repEmail;
        private DataRepository repData;

        public UsuarioRepository()
        {
            //_strConnect = config.GetConnectionString("DeafultConnectionStrings") + "@DTILGCF06FW";
            configDB = new ConfigDB();
            _repEmail = new EmailRepository();
            repData = new DataRepository();
        }

        public async Task<UsuarioAcesso> ValidaUsuario(string usuario, string senha)
        {
            UsuarioAcesso user_ret = new UsuarioAcesso();
            List<dynamic> luser_ret = new List<dynamic>();
            DataTable dtt_usuario = new DataTable();
            DataTable dtt_permissao = new DataTable();

            DataRepository repData = new DataRepository();

            string str_ret = "";

            string str_conn = configDB.ConnectString;
            try
            {
                using (SqlConnection conn = new SqlConnection(str_conn))
                {
                    conn.Open();
                    
                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            dtt_usuario = repData.ConsultaGenericaDtt("[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                            "{ \"nome\":\"id_empresa\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                            "{ \"nome\":\"login\", \"valor\":\"" + usuario + "\", \"tipo\":\"Int64\"}," +
                            "{ \"nome\":\"email\", \"valor\":\"\", \"tipo\":\"string\"}," +
                            "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                            "{ \"nome\":\"pagina\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                            "{ \"nome\":\"qtdregs\", \"valor\":\"0\", \"tipo\":\"Int16\"}," +
                            "{ \"nome\":\"totpags\", \"valor\":\"0\", \"tipo\":\"Int64\"}]", "p_sel_usuario", conn, tran);


                            //dtt_usuario = repData.ConsultaGenericaDtt("[{ \"nome\":\"UserID\", \"valor\":\"" + usuario + "\", \"tipo\":\"string\"},{ \"nome\":\"Senha\", \"valor\":\"1\", \"tipo\":\"string\"}]", "ntv_p_sel_tbl_usuarios", conn, tran);
                            //str_ret = repData.ConsultaGenerica("[{ \"UserID\":\"" + usuario + "\", \"Senha\":\"1\"}]", "ntv_p_sel_tbl_usuarios", conn, tran);
                            //dtt_usuario = repData.ConsultaGenericaDtt("[{ \"nome\":\"UserID\", \"valor\":\"" + usuario + "\", \"tipo\":\"string\"},{ \"nome\":\"Senha\", \"valor\":\"1\", \"tipo\":\"string\"}]", "ntv_p_sel_tbl_usuarios", conn, tran);

                            if (dtt_usuario.Rows.Count > 0)
                            {
                                //if (senha.ToUpper() == Descriptografar(dtt_usuario.Rows[0]["str_senha"].ToString()).ToUpper())
                                if (senha.ToUpper() == dtt_usuario.Rows[0]["str_senha"].ToString().ToUpper())
                                {

                                    user_ret.id = Convert.ToInt64(dtt_usuario.Rows[0]["id"]);
                                    user_ret.id_empresa = Convert.ToInt64(dtt_usuario.Rows[0]["id_empresa"]);
                                    user_ret.int_tipo = Convert.ToInt16(dtt_usuario.Rows[0]["int_tipo"]);
                                    user_ret.username = dtt_usuario.Rows[0]["str_login"].ToString();
                                    user_ret.password = dtt_usuario.Rows[0]["str_senha"].ToString();
                                    user_ret.validade = DateTime.Now.AddDays(1);
                                    user_ret.int_local = Convert.ToInt16(dtt_usuario.Rows[0]["int_local_atend"]);

                                    /*Carrega permissão do usuário*/
                                    /*
                                    dtt_permissao = ConsultaUsuarioPermissao("APINatividade", 1, Convert.ToInt64(dtt_usuario.Rows[0]["id"]), conn, tran);

                                    if (dtt_permissao.Rows.Count > 0)
                                    {
                                        user_ret.id = Convert.ToInt64(dtt_usuario.Rows[0]["id"]);
                                        user_ret.username = dtt_usuario.Rows[0]["str_usuario"].ToString();
                                        user_ret.password = dtt_usuario.Rows[0]["str_senha"].ToString();
                                        user_ret.validade = DateTime.Now.AddMinutes(40);
                                    }
                                    else
                                    {
                                        user_ret.id = 0;
                                        user_ret.username = "Usuário sem permissão de acesso";
                                        user_ret.password = "";
                                        user_ret.validade = DateTime.Now.AddMinutes(30);

                                    }*/

                                    //luser_ret.Add(user_ret);
                                    //str_ret = repData.ManutencaoTabela<dynamic>("I", luser_ret, "ntv_tbl_usuario", conn, tran);
                                }
                                else
                                {
                                    user_ret.id = 0;
                                    user_ret.id_empresa = 0;
                                    user_ret.int_tipo = 0;
                                    user_ret.username = "Senha inválida";
                                    user_ret.password = "";
                                    user_ret.validade = DateTime.Now.AddDays(1);
                                    user_ret.int_local = 1;
                                }
                            }
                            else
                            {
                                user_ret.id = 0;
                                user_ret.id_empresa = 0;
                                user_ret.int_tipo = 0;
                                user_ret.username = "Usuário não cadastrado.";
                                user_ret.password = "";
                                user_ret.validade = DateTime.Now.AddDays(1);
                                user_ret.int_local = 1;
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
            catch (Exception ex)
            {
                throw ex;
            }

            return user_ret;
        }

        public async Task<UsuarioAcesso> ValidaUsuarioPostgres(string usuario, string senha)
        {
            UsuarioAcesso user_ret = new UsuarioAcesso();
            List<Usuario> luser_ret = new List<Usuario>();
            DataTable dtt_usuario = new DataTable();
            DataTable dtt_permissao = new DataTable();

            DataRepository repData = new DataRepository();

            string str_ret = "";

            string str_conn = configDB.ConnectString;
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(configDB.ConnectString))
                {
                    conn.Open();

                    using (NpgsqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            string sqlStr = "select * from f_sel_tbl_ntv_tbl_usuario(0,0,'" + usuario + "','',0)";

                            str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, tran);
                            dtt_usuario = JsonConvert.DeserializeObject<DataTable>(str_ret);
                            luser_ret = JsonConvert.DeserializeObject<List<Usuario>>(str_ret);

                            //dtt_usuario = repData.ConsultaGenericaDtt("[{ \"nome\":\"UserID\", \"valor\":\"" + usuario + "\", \"tipo\":\"string\"},{ \"nome\":\"Senha\", \"valor\":\"1\", \"tipo\":\"string\"}]", "ntv_p_sel_tbl_usuarios", conn, tran);
                            //str_ret = repData.ConsultaGenerica("[{ \"UserID\":\"" + usuario + "\", \"Senha\":\"1\"}]", "ntv_p_sel_tbl_usuarios", conn, tran);
                            //dtt_usuario = repData.ConsultaGenericaDtt("[{ \"nome\":\"UserID\", \"valor\":\"" + usuario + "\", \"tipo\":\"string\"},{ \"nome\":\"Senha\", \"valor\":\"1\", \"tipo\":\"string\"}]", "ntv_p_sel_tbl_usuarios", conn, tran);

                            if (dtt_usuario.Rows.Count > 0)
                            {
                                //if (senha.ToUpper() == Descriptografar(dtt_usuario.Rows[0]["str_senha"].ToString()).ToUpper())
                                if (senha.ToUpper() == dtt_usuario.Rows[0]["str_senha"].ToString().ToUpper())
                                {

                                    user_ret.id = Convert.ToInt64(dtt_usuario.Rows[0]["id"]);
                                    user_ret.id_empresa = Convert.ToInt64(dtt_usuario.Rows[0]["id_empresa"]);
                                    user_ret.int_tipo = Convert.ToInt16(dtt_usuario.Rows[0]["int_tipo"]);
                                    user_ret.username = dtt_usuario.Rows[0]["str_login"].ToString();
                                    user_ret.password = dtt_usuario.Rows[0]["str_senha"].ToString();
                                    user_ret.validade = DateTime.Now.AddDays(1);
                                    user_ret.int_local = Convert.ToInt16(dtt_usuario.Rows[0]["int_local_atend"]);

                                    /*Carrega permissão do usuário*/
                                    /*
                                    dtt_permissao = ConsultaUsuarioPermissao("APINatividade", 1, Convert.ToInt64(dtt_usuario.Rows[0]["id"]), conn, tran);

                                    if (dtt_permissao.Rows.Count > 0)
                                    {
                                        user_ret.id = Convert.ToInt64(dtt_usuario.Rows[0]["id"]);
                                        user_ret.username = dtt_usuario.Rows[0]["str_usuario"].ToString();
                                        user_ret.password = dtt_usuario.Rows[0]["str_senha"].ToString();
                                        user_ret.validade = DateTime.Now.AddMinutes(40);
                                    }
                                    else
                                    {
                                        user_ret.id = 0;
                                        user_ret.username = "Usuário sem permissão de acesso";
                                        user_ret.password = "";
                                        user_ret.validade = DateTime.Now.AddMinutes(30);

                                    }*/

                                    //luser_ret.Add(user_ret);
                                    //str_ret = repData.ManutencaoTabela<dynamic>("I", luser_ret, "ntv_tbl_usuario", conn, tran);
                                }
                                else
                                {
                                    user_ret.id = 0;
                                    user_ret.id_empresa = 0;
                                    user_ret.int_tipo = 0;
                                    user_ret.username = "Senha inválida";
                                    user_ret.password = "";
                                    user_ret.validade = DateTime.Now.AddDays(1);
                                    user_ret.int_local = 1;
                                }
                            }
                            else
                            {
                                user_ret.id = 0;
                                user_ret.id_empresa = 0;
                                user_ret.int_tipo = 0;
                                user_ret.username = "Usuário não cadastrado.";
                                user_ret.password = "";
                                user_ret.validade = DateTime.Now.AddDays(1);
                                user_ret.int_local = 1;
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
            catch (Exception ex)
            {
                throw ex;
            }

            return user_ret;
        }

        public async Task<bool> GravarAcesso(UsuarioAcesso usuario, string Token, string ip, string metodo, string parametros)
        {
            DataTable dtt_usuario_grv = new DataTable();
            DataRow row_usuario = null;
            StringBuilder stb_usuario = new StringBuilder();

            bool bol_ret = false;

            try
            {
                string str_conn = configDB.ConnectString;
                using (SqlConnection conn = new SqlConnection(str_conn))
                {
                    conn.Open();

                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            dtt_usuario_grv = ConsultaAcesso(-1, conn, tran);
                            row_usuario = dtt_usuario_grv.NewRow();

                            row_usuario["id"] = 0;
                            row_usuario["id_usuario"] = usuario.id;
                            row_usuario["dtm_acesso"] = DateTime.Now;
                            row_usuario["dtm_validade"] = usuario.validade;
                            row_usuario["str_token"] = Token;
                            row_usuario["str_ip"] = ip;
                            row_usuario["str_metodo"] = metodo;
                            row_usuario["str_parametros"] = parametros;
                            row_usuario["int_nivel"] = 0;

                            dtt_usuario_grv.Rows.Add(row_usuario);
                            stb_usuario = SaveThroughXML(dtt_usuario_grv, "ntv_tbl_token_acesso");
                            bol_ret = InsAcesso(stb_usuario.ToString(), conn, tran);

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
            catch (Exception ex)
            {
                throw ex;
            }

            return bol_ret;
        }

        public async Task<bool> GravarAcessoPostgres(UsuarioAcesso usuario, string Token, string ip, string metodo, string parametros)
        {
            DataTable dtt_usuario_grv = new DataTable();
            DataRow row_usuario = null;
            StringBuilder stb_usuario = new StringBuilder();

            bool bol_ret = false;

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(configDB.ConnectString))
                {
                    conn.Open();

                    using (NpgsqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            string sqlStr = "select * from f_sel_tbl_ntv_tbl_token_acesso(-1,0,'','')";

                            string str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, tran);
                            dtt_usuario_grv = JsonConvert.DeserializeObject<DataTable>(str_ret);
                            if (dtt_usuario_grv.Columns.Count == 0)
                            {
                                dtt_usuario_grv.Columns.Add(new DataColumn("str_operation", System.Type.GetType("System.String")));
                                dtt_usuario_grv.Columns.Add(new DataColumn("id", System.Type.GetType("System.Int64")));
                                dtt_usuario_grv.Columns.Add(new DataColumn("id_usuario", System.Type.GetType("System.Int64")));
                                dtt_usuario_grv.Columns.Add(new DataColumn("dtm_acesso", System.Type.GetType("System.DateTime")));
                                dtt_usuario_grv.Columns.Add(new DataColumn("dtm_validade", System.Type.GetType("System.DateTime")));
                                dtt_usuario_grv.Columns.Add(new DataColumn("str_token", System.Type.GetType("System.String")));
                                dtt_usuario_grv.Columns.Add(new DataColumn("str_ip", System.Type.GetType("System.String")));
                                dtt_usuario_grv.Columns.Add(new DataColumn("str_metodo", System.Type.GetType("System.String")));
                                dtt_usuario_grv.Columns.Add(new DataColumn("str_parametros", System.Type.GetType("System.String")));
                                dtt_usuario_grv.Columns.Add(new DataColumn("int_nivel", System.Type.GetType("System.Int32")));
                            }
                            else
                            {
                                dtt_usuario_grv.Columns.Add(new DataColumn("str_operation", System.Type.GetType("System.String")));
                            }

                            row_usuario = dtt_usuario_grv.NewRow();

                            row_usuario["str_operation"] = "I";
                            row_usuario["id"] = 0;
                            row_usuario["id_usuario"] = usuario.id;
                            row_usuario["dtm_acesso"] = DateTime.Now;
                            row_usuario["dtm_validade"] = usuario.validade;
                            row_usuario["str_token"] = Token;
                            row_usuario["str_ip"] = ip;
                            row_usuario["str_metodo"] = metodo;
                            row_usuario["str_parametros"] = parametros;
                            row_usuario["int_nivel"] = 0;                            

                            dtt_usuario_grv.Rows.Add(row_usuario);
                            str_ret = JsonConvert.SerializeObject(dtt_usuario_grv);
                            sqlStr = "select * from f_man_tbl_ntv_tbl_token_acesso('{\"dados\": " + str_ret + "}') as id";
                            str_ret = repData.ConsultaGenericaPostgres(sqlStr, conn, tran);

                            bol_ret = true;
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
            catch (Exception ex)
            {
                throw ex;
            }

            return bol_ret;
        }

        public DataTable ConsultaUsuarioPermissao(string formulario, Int64 id_operacao, Int64 id_usuario, SqlConnection conn, SqlTransaction tran)
        {
            SqlCommand command = new SqlCommand("ntv_p_sel_usuario_formulario_operacao", conn);
            command.Transaction = tran;
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@dc_formulario", formulario));
            command.Parameters.Add(new SqlParameter("@cd_usuario", id_usuario));
            command.Parameters.Add(new SqlParameter("@cd_operacao", id_operacao));

            DataTable dtt_retorno = new DataTable();

            using (var reader = command.ExecuteReader())
            {
                dtt_retorno.Load(reader);
            }

            return dtt_retorno;

        }

        public DataTable ConsultaAcesso(Int64 id, SqlConnection conn, SqlTransaction tran)
        {
            SqlCommand command = new SqlCommand("p_sel_tbl_token_acesso", conn);
            command.Transaction = tran;
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@id_usuario", id));
            //command.Parameters.Add(new SqlParameter("@str_usuario", ""));
            command.Parameters.Add(new SqlParameter("@str_login", ""));
            command.Parameters.Add(new SqlParameter("@str_nome", ""));


            DataTable dtt_retorno = new DataTable();

            using (var reader = command.ExecuteReader())
            {
                dtt_retorno.Load(reader);
            }

            return dtt_retorno;

        }
        private bool InsAcesso(string Dados, SqlConnection conn, SqlTransaction tran)
        {
            bool bol_retorno = false;

            try
            {
                SqlCommand command = conn.CreateCommand();
                SqlParameter param_id = new SqlParameter();
                command.Transaction = tran;
                command.CommandText = "ntv_p_man_tbl_token_acesso";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@Tp_processo", "I"));
                command.Parameters.Add(new SqlParameter("@Dados", Dados));
                param_id.ParameterName = "@Novos_id";
                param_id.Size = 1000;
                param_id.Direction = ParameterDirection.Output;
                command.Parameters.Add(param_id);

                command.ExecuteNonQuery();

                bol_retorno = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return bol_retorno;
        }

        private string Criptografar(string Texto)
        {
            try
            {
                return ExecutarCodificador(Texto, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string Descriptografar(string Texto)
        {
            try
            {
                return ExecutarCodificador(Texto, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string ExecutarCodificador(string Texto, bool Criptografar)
        {
            string sRetorno = "";

            try
            {
                string str_chave = "Natividade Soluções em TI";
                int iChaveIndex = 0;
                int iChaveTamanho = 25;
                int iFator1 = 95;
                int iFator2 = 5;

                int[] iChaveCaractere = new int[iChaveTamanho];

                for (int i = 0; i < iChaveTamanho; i++)
                {
                    iChaveCaractere[i] = Convert.ToInt32(Convert.ToChar(str_chave.Substring(i, 1)));
                }

                int iCount = 1;

                for (int i = 0; i < Texto.Length; i++)
                {
                    iChaveIndex++;

                    int iCaracter = Convert.ToInt32(Convert.ToChar(Texto.Substring(i, 1)));
                    int iCaracterNovo = iCaracter ^ iChaveCaractere[iChaveIndex - 1] ^ iFator1 ^ (((iCount - 1) / iFator2) % 255);
                    sRetorno += (char)iCaracterNovo;
                    iCount++;

                    if (Criptografar)
                    {
                        iFator1 = iCaracterNovo;
                    }
                    else
                    {
                        iFator1 = iCaracter;
                    }

                    if (iChaveIndex >= iChaveTamanho)
                    {
                        iChaveIndex = 0;
                    }
                }

                if (Criptografar)
                {
                    if (sRetorno.Length != Texto.Length)
                    {
                        sRetorno = "";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return sRetorno;
        }
        private StringBuilder SaveThroughXML(DataTable DtTabela, string NomeTabela)
        {
            StringBuilder sb = new System.Text.StringBuilder(1000);
            StringWriter sw = new System.IO.StringWriter(sb);

            DtTabela.TableName = NomeTabela;
            foreach (DataColumn col in DtTabela.Columns)
            {
                col.ColumnMapping = System.Data.MappingType.Attribute;
                if (col.DataType == typeof(DateTime))
                {
                    col.DateTimeMode = DataSetDateTime.Unspecified;
                }
            }

            DtTabela.WriteXml(sw, System.Data.XmlWriteMode.WriteSchema);
            return sb;
        }

        public async Task<Int64> GravarUsuario(Int64 company_serv, Int64 company_app, Int64 user_serv, Int64 user_app, dynamic dados)
        {
            DataRepository repData = new DataRepository();
            bool bol_ret = true;
            Int64 id_usu = 0;

            string str_ret = "";
            string str_corpo = "";

            UsuarioApp usuApp = new UsuarioApp();
            List<Usuario> usu = new List<Usuario>();

            string str_param = "";
            /*if (dados.GetType() != typeof(string))
            {
                str_param = JsonConvert.SerializeObject(dados);
            }
            else
            {
                str_param = dados;
            }*/

            usuApp = dados;

            usu.Add(new Usuario
            {
                id = usuApp.id_server,
                id_empresa = company_serv,
                str_nome = usuApp.str_nome,
                str_login = usuApp.str_login,
                str_senha = usuApp.str_senha,
                str_email = usuApp.str_email,
                int_telefone = usuApp.int_telefone,
                int_tipo = usuApp.int_tipo,
                dtm_inclusao = usuApp.dtm_inclusao,
                dtm_saida = usuApp.dtm_saida,
                int_situacao = usuApp.int_situacao,
                str_foto = usuApp.str_foto,
                id_app = usuApp.id,
                id_user_man = user_app
            });

            using (SqlConnection conn = new SqlConnection(configDB.ConnectString))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        id_usu = Convert.ToInt64(repData.ManutencaoTabela<Usuario>("I", usu, "ntv_tbl_usuario", conn, tran).Split(";")[0]);
                        if (id_usu > 0)
                        {                            
                            
                            if (user_serv > 0)
                            {
                                //Se for cadastro de um novo usuário envia e-mail de código de confirmação
                                Int64 id_prim = 0;

                                List<UserPrimAcess> primAcesses = new List<UserPrimAcess>();
                                primAcesses.Add(new UserPrimAcess
                                {
                                    id = 0,
                                    id_empresa = company_app,
                                    id_emp_serv = company_serv,
                                    id_user_app = id_usu,
                                    str_email = usuApp.str_email,
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
                                        str_para = usuApp.str_email,
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

                                //Gerar dados para sincronização
                                String[] arr_str = new string[] 
                                    { 
                                        "ntv_tbl_empresa", 
                                        "ntv_tbl_formapag", 
                                        "ntv_tbl_grupo", 
                                        "ntv_tbl_itemcombo", 
                                        "ntv_tbl_local", 
                                        "ntv_tbl_localcliente", 
                                        "ntv_tbl_localclipag", 
                                        "ntv_tbl_produto", 
                                        "ntv_tbl_usuario"
                                    };

                                string filtros = "";
                                
                                for (int i = 0; i < arr_str.Length; i++)
                                {
                                    filtros = "[{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
                                              "{ \"nome\":\"id_empresa\", \"valor\":\"" + company_serv.ToString() + "\", \"tipo\":\"Int64\"}," +
                                              "{ \"nome\":\"tabela\", \"valor\":\"" + arr_str[i] + "\", \"tipo\":\"string\"}," +
                                              "{ \"nome\":\"id_user\", \"valor\":\"" + user_app.ToString() + "\", \"tipo\":\"Int64\"}," +
                                              "{ \"nome\":\"id_user_dest\", \"valor\":\"" + id_usu.ToString() + "\", \"tipo\":\"Int64\"}]";
                                    bol_ret = repData.ProcedureUpdate(filtros, "ntv_p_ins_tbl_sync_dados", conn, tran);
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

            return id_usu;

        }

        public async Task<string> GravarDados(string operacao, string dados)
        {
            string str_ret = "";
            Int64 id_ret = 0;
            Usuario usuario = new Usuario();
            List<Usuario> listDados = new List<Usuario>();
            string str_param = "";
            if (dados.GetType() != typeof(string))
            {
                str_param = JsonConvert.SerializeObject(dados);
            }
            else
            {
                str_param = dados;
            }
            try
            {
                listDados = JsonConvert.DeserializeObject<List<Usuario>>(str_param.IndexOf("[") == -1 ? "[" + str_param + "]" : str_param);

                if (listDados != null)
                {
                    string str_conn = configDB.ConnectString;

                    using (SqlConnection conn = new SqlConnection(str_conn))
                    {
                        conn.Open();

                        using (SqlTransaction tran = conn.BeginTransaction())
                        {
                            try
                            {
                                id_ret = Convert.ToInt64(repData.ManutencaoTabela<Usuario>(operacao, listDados, "ntv_tbl_usuario", conn, tran).Split(";")[0]);
                                usuario = listDados[0];
                                usuario.id = id_ret;
                                str_ret = JsonConvert.SerializeObject(usuario);
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
