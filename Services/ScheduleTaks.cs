using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;
using WebAppServer.Models;
using WebAppServer.Repositories;

namespace WebAppServer.Services
{
    public class ScheduleTaks : IInvocable
    {
        private readonly ILogger<ScheduleTaks> _logger;
        private readonly LocalRepository _local;

        public ScheduleTaks(ILogger<ScheduleTaks> logger, LocalRepository local)
        {
            _logger = logger;
            _local = local;
        }

        public async Task Invoke()
        {
            //_logger.LogInformation("Hello");
            //throw new System.NotImplementedException();
            try
            {
                _local.IniciaDia();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
