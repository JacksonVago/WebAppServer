
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using WebAppServer.Models;
using WebAppServer.Repositories;

namespace WebAppServer.Hubs
{
    public class AppHub : Hub
    {
        private readonly SalaRepository _repSala;
        private readonly HubRepository _repHub;
        private readonly NotificacoesRepository _repNotif;
        private readonly ILogger<AppHub> _logger;        

        public AppHub(ILogger<AppHub> logger, SalaRepository repsala, HubRepository rephub, NotificacoesRepository repnotif)
        {
            _logger = logger;
            _repSala = repsala;
            _repHub = rephub;
            _repNotif = repnotif;
        }

        /*public override async Task OnConnectedAsync()
        {
            await AtualizaGrupo(true);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await AtualizaGrupo(false);
            await base.OnDisconnectedAsync(exception);
        }*/

        public async Task OnConnect(string empresa, string user, string userAdm)
        {
            string str_conect_adm = "";

            //Verifica se é o administrador que esta conectando
            if (user == userAdm)
            {
                str_conect_adm = Context.ConnectionId;

                //Envia para todos os usuários exceto ele mesmo
                await Clients.AllExcept(str_conect_adm).SendAsync("AdmConnectionID", str_conect_adm, Context.ConnectionId);

            }
            else
            {
                //Identifica se o administrador esta conectado
                List<SalaUser> lst_salaU = await _repSala.ConsultarSalaUser(0, Convert.ToInt64(empresa), Convert.ToInt64(userAdm));
                if (lst_salaU != null && lst_salaU.Count > 0)
                {
                    if (lst_salaU[0].str_conect == "S") {
                        str_conect_adm = lst_salaU[0].str_idconnect;
                    }
                }

                //Envia de volta o id do adm conectado
                await Clients.Caller.SendAsync("AdmConnectionID", str_conect_adm, Context.ConnectionId);

                //Verica se tem notificações e envia para o usuário que esta se conectando
                string str_filtros = "";
                str_filtros += "[";
                str_filtros += "{ \"nome\":\"id\", \"valor\":\"0\", \"tipo\":\"Int64\"},";
                str_filtros += "{ \"nome\":\"id_empresa\", \"valor\":\"" + empresa + "\", \"tipo\":\"Int64\"},";
                str_filtros += "{ \"nome\":\"id_usu_orig\", \"valor\":\"0\", \"tipo\":\"Int64\"},";
                str_filtros += "{ \"nome\":\"id_usu_dest\", \"valor\":\"" + user + "\", \"tipo\":\"Int64\"},";
                str_filtros += "{ \"nome\":\"situacao\", \"valor\":\"0\", \"tipo\":\"Int16\"}";
                str_filtros += "]";

                String str_notif = await _repNotif.ConsultarNotificacoes(str_filtros);
                if (str_notif != null && str_notif != "[]")
                {
                    //Envia notficações
                    await Clients.Caller.SendAsync("ReceiveNotificacoes", str_notif);
                }

            }

            if (str_conect_adm != "")
            {
                await AtualizaGrupo(empresa, user, true);
            }
            await base.OnConnectedAsync();

        }

        public async Task OnDisconnect(string empresa, string user)
        {
            await AtualizaGrupo(empresa, user, false);
            await base.OnDisconnectedAsync(null);
        }

        public async Task SendPedido(string empresa, string userID, string destinatario, string str_pedido, string str_itens)
        {
            UsuarioHub dest = JsonConvert.DeserializeObject<UsuarioHub>(destinatario);

            //Grava o pedido no servidor e retorna com o ID
            str_pedido = await _repHub.GravarPedido(Convert.ToInt64(empresa), str_pedido);
            str_itens = await _repHub.GravarPedidoItem(Convert.ToInt64(empresa), str_itens);
            
            //Envia para o administrador
            await Clients.Client(dest.str_idconnect).SendAsync("ReceivePedido", destinatario, str_pedido, str_itens);

            //Devolve para o usuario que enviou
            await Clients.Caller.SendAsync("ReceivePedido", destinatario, str_pedido, str_itens);
        }

        public async Task SendLocalCli(string empresa, string userID, string str_localcli)
        {
            //Grava o pedido no servidor e retorna com o ID
            str_localcli = await _repHub.GravarLocalCli(Convert.ToInt64(empresa), str_localcli);

            //Envia para todos ataulizar id_serve
            await Clients.All.SendAsync("ReceiveLocalCli", str_localcli);

        }

        public async Task SendLocal(string empresa, string userID, string str_local)
        {
            //Grava o pedido no servidor e retorna com o ID
            str_local = await _repHub.GravarLocal(Convert.ToInt64(empresa), str_local);

            //Envia para todos ataulizar id_serve
            await Clients.All.SendAsync("ReceiveLocal", str_local);

        }

        public async Task ConfirmaPedido(string empresa, string userID, string destinatario, string str_pedido, string str_itens)
        {
            UsuarioHub dest = JsonConvert.DeserializeObject<UsuarioHub>(destinatario);

            //Grava o pedido no servidor e retorna com o ID
            str_pedido = await _repHub.GravarPedido(Convert.ToInt64(empresa), str_pedido);
            str_itens = await _repHub.GravarPedidoItem(Convert.ToInt64(empresa), str_itens);

            //Identifica o connectID do usuário do pedido
            //destinatário deve ser o id_usuario do servidor
            //Identifica se o administrador esta conectado
            string str_conect_id = await VerificaAdm(Convert.ToInt64(empresa), dest.id_usu_dest);

            if (str_conect_id == null || str_conect_id == "")
            {
                //Precisa gravar as notificações para poder carregar se o usuário de destino não estive conectado
                GravarNotificacoes(Convert.ToInt64(empresa), Convert.ToInt64(userID), dest.id_usu_dest, "ntv_tbl_pedido", str_pedido);
            }
            else
            {
                //Envia para o destinatário a confirmação do pedido)
                await Clients.Client(str_conect_id).SendAsync("ReceivePedido", destinatario, str_pedido, str_pedido);

            }
        }

        public async Task SendMessage(string empresa, string userID, string msg)
        {
            //Ao usar o método Client(_connections.GetUserId(chat.destination)) eu estou enviando a mensagem apenas para o usuário destino, não realizando broadcast

            await Clients.Group(empresa).SendAsync("ReceivePedido", msg, msg);
            await Clients.Client(userID).SendAsync("Receive", msg, msg);
        }

        private async Task AtualizaGrupo(string empresa, string usuario, bool connect)
        {
            string str_msg = "";
            SalaUser salaU = new SalaUser();

            //Busca sala
            string lst_ret = await _repSala.ConsultarSala(0, Convert.ToInt64(empresa));
            List<Sala> sala_ret = JsonConvert.DeserializeObject<List<Sala>>(lst_ret);

            if (sala_ret.Count == 0)
            {
                //Grava sala
                Sala sala = new Sala();
                sala.id = 0;
                sala.id_empresa = Convert.ToInt64(empresa);                    
                sala.dtm_inclusao = DateTime.Now;
                sala.id_app = 0;

                sala = await _repSala.GravarSala(sala, "I");


                //Grava usuário e adiciona na sala
                salaU.id = 0;
                salaU.id_sala = sala.id;
                salaU.id_usuario = Convert.ToInt64(usuario);
                salaU.str_idconnect = Context.ConnectionId;
                salaU.str_conect = "S";
                salaU.dtm_inclusao = DateTime.Now;
                salaU.id_app = 0;

                await _repSala.GravarSalaUser(salaU, "I");
                await Groups.AddToGroupAsync(Context.ConnectionId, empresa);
            }
            else
            {
                //Atualiza usuário na sala
                //Busca usuário
                List<SalaUser> lst_salaU = await _repSala.ConsultarSalaUser(0, Convert.ToInt64(empresa), Convert.ToInt64(usuario));
                if (lst_salaU.Count == 0 && connect)
                {
                    salaU.id = 0;
                    salaU.id_sala = sala_ret[0].id;
                    salaU.id_usuario = Convert.ToInt64(usuario);
                    salaU.str_idconnect = Context.ConnectionId;
                    salaU.str_conect = "S";
                    salaU.dtm_inclusao = DateTime.Now;
                    salaU.id_app = 0;

                    await _repSala.GravarSalaUser(salaU, "I");
                    await Groups.AddToGroupAsync(Context.ConnectionId, empresa);
                }
                else
                {
                    if (connect)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, empresa);
                    }
                    else
                    {
                        await Groups.RemoveFromGroupAsync(Context.ConnectionId, empresa);
                    }

                    //Atualiza situação do usuário na sala
                    salaU.id = lst_salaU[0].id;
                    salaU.id_sala = sala_ret[0].id;
                    salaU.id_usuario = lst_salaU[0].id_usuario;
                    salaU.str_idconnect = Context.ConnectionId;
                    salaU.str_conect = (connect ? "S" : "N");
                    salaU.dtm_inclusao = DateTime.Now;
                    salaU.id_app = lst_salaU[0].id_app;

                    await _repSala.GravarSalaUser(salaU, "U");
                }
            }

        }

        private async Task<string> VerificaAdm(Int64 empresa, Int64 userAdm)
        {
            string str_conect_adm = "";

            List<SalaUser> lst_salaU = await _repSala.ConsultarSalaUser(0, Convert.ToInt64(empresa), Convert.ToInt64(userAdm));
            if (lst_salaU != null && lst_salaU.Count > 0)
            {
                if (lst_salaU[0].str_conect == "S")
                {
                    str_conect_adm = lst_salaU[0].str_idconnect;
                }
            }
            return str_conect_adm;
        }

        private async Task GravarNotificacoes(Int64 empresa, Int64 usu_orig, Int64 usu_dest, string tabela, string dados)
        {
            string str_conect_adm = "";            
            DataTable dtt_entity = JsonConvert.DeserializeObject<DataTable>(dados);
            Notificacoes notific = new Notificacoes();

            if (dtt_entity != null && dtt_entity.Rows.Count > 0)
            {
                for (int i = 0; i < dtt_entity.Rows.Count; i++)
                {
                    notific.id = 0;
                    notific.id_empresa = empresa;
                    notific.id_usu_orig = usu_orig;
                    notific.id_usu_dest = usu_dest;
                    notific.str_tabela = tabela;
                    notific.id_registro = Convert.ToInt64(dtt_entity.Rows[i]["id"]);
                    notific.dtm_inclusao = DateTime.Now;
                    notific.int_situacao = 0; //0 - Não Recebida / 1 - Recebida

                    await _repNotif.GravarNotificacoes(JsonConvert.SerializeObject(notific));
                }
            }
        }
    }
}
