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
    [Route("api/AppPraia")]
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

        [HttpPost("v1/SyncData")]
        public async Task<ActionResult<dynamic>> SynchronizeData([FromQuery] Int64 company, [FromQuery] Int64 user, [FromQuery] string entity, [FromBody] DadosSync data)
        {

            dynamic ret = _repSync.SychronizeData(company, user, entity, data);
            if (ret != null)
            {
                return ret;
            }
            return BadRequest("Problemas na sincronização");
        }

        [HttpPost("v1/SaveUserAdm")]
        public async Task<Int64> SaveAdministrator([FromQuery] Int64 company, [FromQuery] Int64 user, [FromBody] UsuarioApp data)
        {

            Int64 ret = await _repUser.GravarUsuario(company, user, data);
            if (ret != null)
            {
                return ret;
            }
            return 0;
        }


    }
}
