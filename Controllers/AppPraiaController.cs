using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebAppServer.Models;
using WebAppServer.Repositories;

namespace WebAppServer.Controllers
{
    [Route("api/[controller]")]
    public class AppPraiaController : Controller
    {
        private readonly SyncRepository _repSync;
        private readonly UsuarioRepository _repUser;

        private readonly ILogger<AppPraiaController> _logger;

        public AppPraiaController(ILogger<AppPraiaController> logger, SyncRepository repsync, UsuarioRepository repuser)
        {
            _logger = logger;
            _repSync = repsync;
            _repUser = repuser;
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
        public async Task<ActionResult<dynamic>> SynchronizeDataDown([FromBody] string filtros)
        {

            dynamic ret = await _repSync.SyncDataDownload(filtros);
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

    }
}
