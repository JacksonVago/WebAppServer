using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WebAppServer.Repositories;

namespace WebAppServer.Services
{
    public class webserverHS : IHostedService
    {
        private readonly LocalRepository _repData;
        private readonly string _strConnect;
        private Timer time;
        private bool tarefa_ativa = false;

        public webserverHS(LocalRepository repository)
        {
            _repData = repository;

        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            time = new Timer(IniciaDia, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                //time = new Timer(RegistraPIX, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));                
                //return StartAsync(cancellationToken);
                return Task.CompletedTask;
                //throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }

        }

        private void IniciaDia(object state)
        {
            //tarefa_ativa = true;
            if (!tarefa_ativa)
            {
                tarefa_ativa = true;
                DataTable dtt_tran = new DataTable();
                DataTable dtt_banco = new DataTable();
                string str_retorno = "";

                str_retorno = _repData.IniciaDia();
            }

        }
    }
}
