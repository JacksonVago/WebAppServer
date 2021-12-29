using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using WebAppServer.Models;

namespace WebAppServer.Repositories
{
    public class EmailRepository
    {
        private readonly string _path;
        private readonly Int32 _porta;
        private readonly string _serv;
        private readonly string _user;
        private readonly ConfigDB configDB;

        public EmailRepository()
        {
            //_path = Path.Combine(@".\WebAppServer.Service.Email\Config\" + "configEmail.json");
            //_path = Path.Combine("..\\WebAppServer.Service.Email\\Config\\" + "configEmail.json");
            //_path = Path.Combine(@".\" + "configEmail.json");
            var JSON = System.IO.File.ReadAllText("configEmail.json");
            var obj_config = JObject.Parse(JSON);

            _serv = obj_config["configEmail"]["servidorSMTP"].ToString();
            _porta = Convert.ToInt32(obj_config["configEmail"]["porta"].ToString());
            _user = obj_config["configEmail"]["usuario"].ToString();
            configDB = new ConfigDB();
        }

        public async void EnviarEmails(List<envEmail> mails)
        {
            DataRepository repData = new DataRepository();
            bool bol_ret = true;
            Int64 id_prim = 0;
            string str_ret = "";
            string str_operacao = "U";

            if (mails.Count > 0)
            {


                using (SqlConnection conn = new SqlConnection(configDB.ConnectString))
                {
                    conn.Open();
                    try
                    {
                        foreach (envEmail email in mails)
                        {
                            str_ret = EnviaEmail(email);
                            if (str_ret == "ok")
                            {
                                email.int_situacao = 1;
                            }
                            else
                            {
                                email.int_situacao = 2;
                            }
                            if (email.id > 0)
                            {
                                str_operacao = "U";
                            }
                            else
                            {
                                str_operacao = "I";
                            }

                            List<envEmail> email_grv = new List<envEmail>();
                            email_grv.Add(email);

                            str_ret = repData.ManutencaoTabela<envEmail>(str_operacao, email_grv, "ntv_tbl_env_emails", conn);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    conn.Close();
                }
            }

        }

        private string EnviaEmail(envEmail mail)
        {
            bool bol_ret = false;

            string str_corpo = "";
            //string[] ArrMail = emailBoleto.strEmail.Split(';');
            MailAddress remetente = null;
            MailAddress destinatario = null;
            MailAddress CC = null;

            try
            {
                remetente = new MailAddress(mail.str_de, mail.str_de);

                destinatario = new MailAddress(mail.str_para, mail.str_para);

                MailMessage email = new MailMessage(remetente, destinatario);
                //Acrescentas os copiados
                if (mail.str_cc.Length > 0)
                {
                    foreach (string strCC in mail.str_cc.Split(';'))
                    {
                        if (strCC != "")
                        {
                            CC = new MailAddress(strCC.Trim());
                            email.CC.Add(CC);
                        }
                    }
                }

                SmtpClient c_email = new SmtpClient(_serv, _porta);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                NetworkCredential credencial = null;

                credencial = new NetworkCredential(_user, "Suporte@2020");


                c_email.Credentials = credencial;

                email.Subject = mail.str_ass;
                str_corpo = mail.str_msg;
                email.IsBodyHtml = (mail.str_html.Contains("S") ? true : false);
                email.Body = str_corpo;

                c_email.Send(email);

                email.Dispose();
                return "ok";

            }
            catch (Exception ex)
            {
                //throw ex;
                //return "[" + str_mensagem + "]" + ex.Message.ToString().Replace("'", "").Replace("\n", "").Replace("\r", "");
                return ex.Message.ToString().Replace("'", "").Replace("\n", "").Replace("\r", "");
            }

        }

    }
}
