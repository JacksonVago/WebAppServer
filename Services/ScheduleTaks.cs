using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System.Threading.Tasks;

namespace WebAppServer.Services
{
    public class ScheduleTaks : IInvocable
    {
        private readonly ILogger<ScheduleTaks> _logger;

        public ScheduleTaks(ILogger<ScheduleTaks> logger)
        {
            _logger = logger;
        }

        public async Task Invoke()
        {
            _logger.LogInformation("Hello");
            throw new System.NotImplementedException();
        }
    }
}
