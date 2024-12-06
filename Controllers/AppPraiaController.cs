using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebAppServer.Models;
using WebAppServer.Repositories;

namespace WebAppServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppPraiaController : Controller
    {
        private readonly SyncRepository _repSync;
        private readonly UsuarioRepository _repUser;
        private readonly AppRepository _repApp;

        private readonly ILogger<AppPraiaController> _logger;

        public AppPraiaController(ILogger<AppPraiaController> logger, SyncRepository repsync, UsuarioRepository repuser, AppRepository repapp)
        {
            _logger = logger;
            _repSync = repsync;
            _repUser = repuser;
            _repApp = repapp;
        }

        [HttpPost("v1/SyncDataUpload")]
        public async Task<ActionResult<dynamic>> SynchronizeDataUp([FromQuery] Int64 company, [FromQuery] Int64 user, [FromQuery] string entity, [FromBody] DadosSync data)
        {

            dynamic ret = await _repSync.SychronizeData(company, user, entity, data);
            if (ret != null)
            {
                return ret;
            }
            return BadRequest("Problemas na sincronização");
        }

        [HttpPost("v1/SyncDataDownloadRecovery")]
        public async Task<ActionResult<dynamic>> SynchronizeDataDownRecovery([FromBody] DadosSync filtros)
        {

            dynamic ret = await _repSync.SyncDataDownloadRecovery(filtros.Dados.ToString());
            if (ret != null)
            {
                return ret;
            }
            return BadRequest("Problemas na sincronização");
        }

        [HttpPost("v1/SyncDataDownload")]
        public async Task<ActionResult<dynamic>> SynchronizeDataDown([FromBody] DadosSync filtros)
        {

            dynamic ret = await _repSync.SyncDataDownload(filtros.Dados.ToString());
            if (ret != null)
            {
                return ret;
            }
            return BadRequest("Problemas na sincronização");
        }

        [HttpPut("v1/SyncDataDownload/{filtros}")]
        public async Task<ActionResult<dynamic>> SyncDataDone(string filtros)
        {

            dynamic ret = await _repSync.UpdDataDownload(filtros);
            if (ret != null)
            {
                return ret;
            }
            return BadRequest("Problemas na sincronização");
        }

        [HttpPost("v1/SaveUser")]
        public async Task<Int64> SaveUserServer([FromQuery] Int64 company_serv, [FromQuery] Int64 company_app, [FromQuery] Int64 user_serv, [FromQuery] Int64 user_app, [FromBody] UsuarioApp data)
        {
            Int64 ret = await _repUser.GravarUsuario(company_serv, company_app, user_serv, user_app, data);
            if (ret != null)
            {
                //Envia e-mail de confirmação para o usuario
                return ret;
            }
            return 0;
        }

        [HttpPost("v1/ExecutaProcessoGen")]
        public async Task<ActionResult<dynamic>> ExecutaProcessoGen([FromBody] ParametrosEntrada paramEntrada)
        {
            dynamic dyn_retorno = null;
            try
            {
                dyn_retorno = _repApp.ExecutaProcessoGen(paramEntrada);
                return dyn_retorno;
            }
            catch (Exception ex)
            {
                return NotFound(new { mensagem = ex.Message.ToString() });
            }

        }

        [HttpPost("v1/ExecutaSql")]
        public async Task<ActionResult<dynamic>> ExecutaSql([FromBody] DadosSync sql)
        {
            dynamic dyn_retorno = null;
            try
            {
                dyn_retorno = _repApp.ExecutaSql(sql.Dados.ToString().Replace("||", "'"));
                return dyn_retorno;
            }
            catch (Exception ex)
            {
                return NotFound(new { mensagem = ex.Message.ToString() });
            }

        }

        [HttpPost("v1/Maintenence")]
        public async Task<ActionResult<dynamic>> ExecuteDynamicModel([FromBody] GenModels genModels)
        {
            dynamic dyn_retorno = null;
            try
            {
                Assembly genModel = null;

                //string assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WebAppServer.dll");
                //genModel = Assembly.LoadFrom(assemblyPath);
                genModel = Assembly.LoadFrom(@"D:\Jackson\Natividade\APP\AppServer\WebAppServer\WebAppServer\bin\Debug\netcoreapp3.1\WebAppServer.dll");


                Type ClasseImporta = genModel.GetType("WebAppServer." + genModels.classe);
                object obj = Activator.CreateInstance(ClasseImporta);
                MethodInfo Metodo = ClasseImporta.GetMethod(genModels.metodo);

                if (genModels.parametros != null)
                {
                    object[] paramMetodo = new object[genModels.parametros.Count];
                    for (int p = 0; p < genModels.parametros.Count; p++)
                    {
                        switch (genModels.parametros[p].tipo)
                        {
                            case "string":
                            case "String":
                                paramMetodo[p] = genModels.parametros[p].valor.Replace("'", "\"");
                                break;

                            case "Int16":
                            case "int16":
                                paramMetodo[p] = Convert.ToInt16(genModels.parametros[p].valor);
                                break;

                            case "int":
                            case "Int32":
                            case "int32":
                                paramMetodo[p] = Convert.ToInt32(genModels.parametros[p].valor);
                                break;

                            case "Int64":
                            case "int64":
                                paramMetodo[p] = Convert.ToInt64(genModels.parametros[p].valor);
                                break;

                            case "DateTime":
                            case "datetime":
                                paramMetodo[p] = Convert.ToDateTime(genModels.parametros[p].valor);
                                break;

                            case "Double":
                            case "double":
                                paramMetodo[p] = Convert.ToDouble(genModels.parametros[p].valor);
                                break;

                        }

                    }
                    dyn_retorno = Metodo.Invoke(obj, paramMetodo);
                }
                else
                {
                    dyn_retorno = Metodo.Invoke(obj, null);
                }
                
            }
            catch (Exception ex)
            {
                return NotFound(new { mensagem = ex.Message.ToString() });
            }

            return dyn_retorno;

        }

        [HttpPost("v1/ConsultaPostgres")]
        public async Task<ActionResult<dynamic>> ConsultaPostgres([FromBody] DadosPostgres dados)
        {
            dynamic dyn_retorno = null;
            try
            {
                string sqlStr = "select * from f_sel_tbl_" + dados.tabela + "(" + dados.Dados + ")";
                dyn_retorno = _repApp.ExecutaSqlPostgres(sqlStr);
                return dyn_retorno;
            }
            catch (Exception ex)
            {
                return NotFound(new { mensagem = ex.Message.ToString() });
            }

        }

        [HttpPost("v1/MaintenencePostgres")]
        public async Task<ActionResult<dynamic>> MaintenencePostgres([FromBody] DadosPostgres dados)
        {
            dynamic dyn_retorno = null;
            try
            {
                string sqlStr = "select * from f_man_tbl_" + dados.tabela + "('{\"dados\": " + dados.Dados + "}') as id";
                dyn_retorno = _repApp.ExecutaSqlPostgres(sqlStr);
                return dyn_retorno;
            }
            catch (Exception ex)
            {
                return NotFound(new { mensagem = ex.Message.ToString() });
            }

        }

    }
}
