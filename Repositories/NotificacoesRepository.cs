using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppServer.Models;

namespace WebAppServer.Repositories
{
    public class NotificacoesRepository
    {
        private readonly ConfigDB configDB;
        private DataRepository repData;

        public NotificacoesRepository()
        {
            configDB = new ConfigDB();
            repData = new DataRepository();
        }

        public string GravarNotificacoes(string dados)
        {
            string str_ret = "";
            string str_operacao = "";
            Int64 id_reg = 0;
            Notificacoes notific = new Notificacoes();
            List<Notificacoes> lst_notific = new List<Notificacoes>();

            notific = JsonConvert.DeserializeObject<Notificacoes>(dados);

            if (notific != null)
            {
                if (notific.id == 0)
                {
                    str_operacao = "I";
                }
                else
                {
                    str_operacao = "U";
                }

                lst_notific.Add(new Notificacoes
                {
                    id = notific.id,
                    id_empresa = notific.id_empresa,
                    id_usu_orig = notific.id_usu_orig,
                    id_usu_dest = notific.id_usu_dest,
                    str_tabela = notific.str_tabela,
                    id_registro = notific.id_registro,
                    dtm_inclusao = notific.dtm_inclusao,
                    int_situacao = notific.int_situacao //0 - Não Recebida / 1 - Recebida
                });

                string str_conn = configDB.ConnectString;

                using (SqlConnection conn = new SqlConnection(str_conn))
                {
                    conn.Open();

                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            id_reg = Convert.ToInt64(repData.ManutencaoTabela<Notificacoes>(str_operacao, lst_notific, "ntv_tbl_notificacoes", conn, tran).Split(";")[0]);
                            notific.id = id_reg;
                            str_ret = JsonConvert.SerializeObject(notific);
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            conn.Close();
                            str_ret = ex.Message.ToString();
                        }

                        tran.Commit();
                    }
                    conn.Close();                    
                }
            }
            return str_ret;
        }

        public async Task<string> ConsultarNotificacoes(string dados)
        {
            string str_ret = "";

            using (SqlConnection conn = new SqlConnection(configDB.ConnectString))
            {
                conn.Open();
                try
                {
                    List<Filtros> filtros = new List<Filtros>();
                    filtros = JsonConvert.DeserializeObject<List<Filtros>>(dados);
                    str_ret = repData.ConsultaGenerica(filtros, "ntv_p_sel_tbl_notificacoes", conn);
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

    }
}
