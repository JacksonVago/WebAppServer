using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppServer.Models;

namespace WebAppServer.Repositories
{
    public class SalaRepository
    {
        private readonly ConfigDB configDB;
        private readonly DataRepository repData;

        public SalaRepository()
        {
            configDB = new ConfigDB();
            repData = new DataRepository();
        }

        public async Task<string> ConsultarSala(Int64 id, Int64 id_empresa)
        {
            string str_ret = "";

            using (SqlConnection conn = new SqlConnection(configDB.ConnectString))
            {
                conn.Open();
                try
                {
                    List<Filtros> filtros = new List<Filtros>();
                    if (filtros.Count > 0)
                    {
                        filtros.RemoveRange(0, filtros.Count);
                    }
                    filtros.Add(new Filtros { nome = "id", valor = id.ToString(), tipo = typeof(Int64).Name });
                    filtros.Add(new Filtros { nome = "id_empresa", valor = id_empresa.ToString(), tipo = typeof(string).Name });

                    str_ret = repData.ConsultaGenerica(filtros, "ntv_p_sel_tbl_sala", conn);                    
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

        public async Task<Sala> GravarSala(Sala sala, string Operacao)
        {
            Sala ret_sala = new Sala();

            if (sala != null)
            {
                List<Sala> sala_grv = new List<Sala>();
                sala_grv.Add(sala);
                ret_sala = sala;

                Int64 int6_ret = 0;

                using (SqlConnection conn = new SqlConnection(configDB.ConnectString))
                {
                    conn.Open();
                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            int6_ret = Convert.ToInt64(repData.ManutencaoTabela<Sala>(Operacao, sala_grv, "ntv_tbl_sala", conn, tran).Split(";")[0]);
                            if (int6_ret > 0)
                            {
                                ret_sala.id = int6_ret;
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
            return ret_sala;
        }

        public async Task<List<SalaUser>> ConsultarSalaUser(Int64 id, Int64 id_empresa, Int64 id_usuario)
        {
            List<SalaUser> sala_ret = new List<SalaUser>();

            Sala ret_sala = new Sala();
            string str_ret = "";

            using (SqlConnection conn = new SqlConnection(configDB.ConnectString))
            {
                conn.Open();
                try
                {
                    List<Filtros> filtros = new List<Filtros>();
                    if (filtros.Count > 0)
                    {
                        filtros.RemoveRange(0, filtros.Count);
                    }
                    filtros.Add(new Filtros { nome = "id", valor = id.ToString(), tipo = typeof(Int64).Name });
                    filtros.Add(new Filtros { nome = "id_empresa", valor = id_empresa.ToString(), tipo = typeof(string).Name });
                    filtros.Add(new Filtros { nome = "id_usuario", valor = id_usuario.ToString(), tipo = typeof(string).Name });

                    str_ret = repData.ConsultaGenerica(filtros, "ntv_p_sel_tbl_sala_user", conn);
                    sala_ret = JsonConvert.DeserializeObject<List<SalaUser>>(str_ret);
                }
                catch (Exception ex)
                {
                    conn.Close();
                    throw ex;
                }
                conn.Close();
            }

            return sala_ret;

        }

        public async Task<SalaUser> GravarSalaUser(SalaUser salaU, string Operacao)
        {
            SalaUser ret_salaU = new SalaUser();

            if (salaU != null)
            {
                List<SalaUser> sala_grv = new List<SalaUser>();
                sala_grv.Add(salaU);
                ret_salaU = salaU;
                Int64 int6_ret = 0;

                using (SqlConnection conn = new SqlConnection(configDB.ConnectString))
                {
                    conn.Open();
                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            int6_ret = Convert.ToInt64(repData.ManutencaoTabela<SalaUser>(Operacao, sala_grv, "ntv_tbl_sala_user", conn, tran).Split(";")[0]);
                            if (int6_ret > 0)
                            {
                                ret_salaU.id = int6_ret;
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

            return ret_salaU;
        }

        public async Task<IEnumerable<SalaUserMsg>> ConsultarSalaUserMsg(Int64 id, Int64 id_empresa, Int64 id_usuario, DateTime DataIni, DateTime DataFim)
        {
            List<SalaUserMsg> sala_ret = new List<SalaUserMsg>();

            SalaUserMsg ret_salaUmsg = new SalaUserMsg();
            string str_ret = "";

            using (SqlConnection conn = new SqlConnection(configDB.ConnectString))
            {
                conn.Open();
                try
                {
                    List<Filtros> filtros = new List<Filtros>();
                    if (filtros.Count > 0)
                    {
                        filtros.RemoveRange(0, filtros.Count);
                    }
                    filtros.Add(new Filtros { nome = "id", valor = id.ToString(), tipo = typeof(Int64).Name });
                    filtros.Add(new Filtros { nome = "id_empresa", valor = id_empresa.ToString(), tipo = typeof(string).Name });
                    filtros.Add(new Filtros { nome = "id_usuario", valor = id_usuario.ToString(), tipo = typeof(string).Name });

                    str_ret = repData.ConsultaGenerica(filtros, "ntv_p_sel_tbl_sala_user_msg", conn);
                    sala_ret = JsonConvert.DeserializeObject<List<SalaUserMsg>>(str_ret);
                }
                catch (Exception ex)
                {
                    conn.Close();
                    throw ex;
                }
                conn.Close();
            }

            return sala_ret;

        }

        public async Task<SalaUserMsg> GravarSalaUserMsg(SalaUserMsg salaUmsg, string Operacao)
        {
            SalaUserMsg ret_salaUmsg = new SalaUserMsg();

            if (salaUmsg != null)
            {
                List<SalaUserMsg> salaUmsg_grv = new List<SalaUserMsg>();

                salaUmsg_grv.Add(salaUmsg);
                ret_salaUmsg = salaUmsg;
                Int64 int6_ret = 0;

                using (SqlConnection conn = new SqlConnection(configDB.ConnectString))
                {
                    conn.Open();
                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            int6_ret = Convert.ToInt64(repData.ManutencaoTabela<SalaUserMsg>(Operacao, salaUmsg_grv, "ntv_tbl_sala_user_msg", conn, tran).Split(";")[0]);
                            if (int6_ret > 0)
                            {
                                ret_salaUmsg.id = int6_ret;
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

            return ret_salaUmsg;
        }

        public async Task<List<Usuario>> ConsultaUsuario(string usuario, string connectID)
        {
            List<Usuario> lst_ret = new List<Usuario>();
            List<Filtros> filtros = new List<Filtros>();
            string str_ret = "";

            using (SqlConnection conn = new SqlConnection(configDB.ConnectString))
            {
                conn.Open();
                try
                {
                    
                    if (filtros.Count > 0)
                    {
                        filtros.RemoveRange(0, filtros.Count);
                    }
                    filtros.Add(new Filtros { nome = "id", valor = usuario, tipo = typeof(Int64).Name });
                    filtros.Add(new Filtros { nome = "id_empresa", valor = "0", tipo = typeof(Int64).Name });
                    filtros.Add(new Filtros { nome = "login", valor = "", tipo = typeof(string).Name });
                    filtros.Add(new Filtros { nome = "email", valor = "", tipo = typeof(string).Name });
                    filtros.Add(new Filtros { nome = "situacao", valor = "0", tipo = typeof(string).Name });

                    str_ret = repData.ConsultaGenerica(filtros, "ntv_p_sel_tbl_usuario", conn);
                    lst_ret = JsonConvert.DeserializeObject<List<Usuario>>(str_ret);
                }
                catch (Exception ex)
                {
                    conn.Close();
                    throw ex;
                }
                conn.Close();
            }
            return lst_ret;
        }
    }
}
