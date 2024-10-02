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
    [Authorize]
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

                //genModel = Assembly.LoadFrom("/home/ubuntu/WebAppServer/WebAppServer.dll");
                //genModel = Assembly.LoadFrom("./app/publish/WebAppServer.dll");

                string assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WebAppServer.dll");
                genModel = Assembly.LoadFrom(assemblyPath);
                /*
                if (System.IO.File.Exists("/app/publish/WebAppServer.dll"))
                {
                    genModel = Assembly.LoadFrom("/app/publish/WebAppServer.dll");
                }
                else
                {
                    genModel = Assembly.LoadFrom(@"D:\Jackson\Natividade\APP\AppServer\WebAppServer\WebAppServer\bin\Debug\netcoreapp3.1\WebAppServer.dll");
                }*/


                Type ClasseImporta = genModel.GetType("WebAppServer." + genModels.classe);
                object obj = Activator.CreateInstance(ClasseImporta);
                MethodInfo Metodo = ClasseImporta.GetMethod(genModels.metodo);
                object[] paramMetodo = new object[2];
                paramMetodo[0] = genModels.Operacao;
                paramMetodo[1] = genModels.Dados.Replace("'", "\"");
                dyn_retorno = Metodo.Invoke(obj, paramMetodo);
                
            }
            catch (Exception ex)
            {
                return NotFound(new { mensagem = ex.Message.ToString() });
            }

            return dyn_retorno;

        }
    }
}
