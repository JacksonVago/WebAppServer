using Microsoft.Data.SqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebAppServer.Models;

namespace WebAppServer.Repositories
{
    public class AppRepository
    {
        private readonly ConfigDB configDB;

        public AppRepository()
        {
            configDB = new ConfigDB();
        }

        public dynamic ExecutaProcessoGen(ParametrosEntrada paramEntrada)
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

                    dyn_ret = repData.ConsultaGenerica(paramEntrada.parametros, paramEntrada.procedure, conn);
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

        public dynamic ExecutaSql(string sql)
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

                    //Executa sql
                    dyn_ret = repData.ConsultaSQL(sql, conn, null);
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

        public dynamic ExecutaSqlPostgres(string sql)
        {
            DataRepository repData = new DataRepository();
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

                    //Executa sql
                    dyn_ret = repData.ConsultaGenericaPostgres(sql, conn, null);
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

        public dynamic ExecuteDynamicModel(GenModels genModels)
        {
            Assembly genModel = null;
            genModel = Assembly.LoadFrom(@".\WebAppSever.dll");
            Type ClasseImporta = genModel.GetType("WebAppSever." + genModels.classe);
            object obj = Activator.CreateInstance(ClasseImporta);
            Type Tmodel = obj.GetType();


            DataRepository repData = new DataRepository();
            dynamic dyn_ret = null;
            using (SqlConnection conn = new SqlConnection(configDB.ConnectString))
            {
                try
                {
                    conn.Open();

                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
//                            dyn_ret = repData.ManutencaoTabela<obj>("I", )
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
                catch (Exception ex)
                {
                    conn.Close();
                    throw ex;
                }
                conn.Close();
            }

            return dyn_ret;
        }
    }
}
