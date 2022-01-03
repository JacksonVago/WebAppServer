using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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
                Id = 0,
                int_cgccpf = 0,
                str_nome = "",
                str_fantasia = "",
                str_email = str_mail,
                int_telefone = 0,
                int_id_user_adm = 0,
                dtm_inclusao = DateTime.Now,
                int_situacao = 0,
                int_sitpag = 0,
                dtm_ultpag = null,
                IdApp = 0
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
                                str_email = str_mail,
                                dtm_acesso = DateTime.Now,
                                int_cod_acesso = 0,
                                dtm_confirma = null,
                                int_situacao = 0
                            });

                            id_prim = Convert.ToInt64(repData.ManutencaoTabela<UserPrimAcess>("I", primAcesses, "ntv_tbl_prim_acess", conn, tran).Split(";")[0]);

                            str_ret = repData.ConsultaGenerica("[{ \"nome\":\"ID\", \"valor\":\"" + id_prim.ToString() + "\", \"tipo\":\"Int64\"}," +
                                                               "{ \"nome\":\"id_empresa\", \"valor\":\"0\", \"tipo\":\"Int64\"}," +
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
                                    str_de = "jackson@natividadesolucoes.com.br",
                                    str_para = str_mail,
                                    str_cc = "",
                                    str_ass = "Adesão ao APP Pè na Areia",
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
                        throw ex;
                    }
                }
                conn.Close();
            }

            return primAcesses;


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
                str_ret = repData.ConsultaGenerica("[{ \"nome\":\"ID\", \"valor\":\"0\", \"tipo\":\"Int64\"},{ \"nome\":\"id_empresa\", \"valor\":\"0\", \"tipo\":\"Int64\"},{ \"nome\":\"Email\", \"valor\":\"" + str_mail + "\", \"tipo\":\"string\"},{ \"nome\":\"CodAcesso\", \"valor\":\"" + codigo.ToString() + "\", \"tipo\":\"Int64\"},{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}]", "ntv_p_sel_tbl_prim_acess", conn);
                primAcesses = JsonConvert.DeserializeObject<List<UserPrimAcess>>(str_ret);

                conn.Close();
            }
            return primAcesses;
        }
    }
}
