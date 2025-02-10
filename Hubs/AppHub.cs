
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

        public async Task OnConnect(string empresa, string user, string userAdm)
        {
            string str_conect_adm = "";

            /*
            //Verifica se é o administrador que esta conectando
            _logger.LogInformation(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " Hub OnConnect - " + empresa + "/" + user + "/" + userAdm);
            if (user == userAdm)
            {
                str_conect_adm = Context.ConnectionId;

                //Envia para todos os usuários exceto ele mesmo
                await Clients.OthersInGroup(empresa).SendAsync("AdmConnectionID", str_conect_adm, Context.ConnectionId);
                _logger.LogInformation(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " Hub OnConnect - Envia para todos os usuários exceto ele mesmo ");

            }
            else
            {
                //Identifica se o administrador esta conectado
                _logger.LogInformation(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " Hub OnConnect - Identifica se o administrador esta conectado ");
                List<SalaUser> lst_salaU = await _repSala.ConsultarSalaUser(0, Convert.ToInt64(empresa), Convert.ToInt64(userAdm));
                if (lst_salaU != null && lst_salaU.Count > 0)
                {
                    if (lst_salaU[0].str_conect == "S") {
                        str_conect_adm = lst_salaU[0].str_idconnect;
                    }
                }

                //Envia de volta o id do adm conectado
                _logger.LogInformation(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " Hub OnConnect - Envia de volta o id do adm conectado (" + str_conect_adm + ")");
                await Clients.Caller.SendAsync("AdmConnectionID", str_conect_adm, Context.ConnectionId);

            }

            //Verica se tem notificações e envia para o usuário que esta se conectando
            _logger.LogInformation(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " Hub OnConnect - Verica se tem notificações e envia para o usuário que esta se conectando");
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
                _logger.LogInformation(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " Hub OnConnect - Envia notficações");
                await Clients.Caller.SendAsync("ReceiveNotificacoes", str_notif);
            }

            await AtualizaGrupo(empresa, user, true);
            await base.OnConnectedAsync();
            */

        }

        public async Task OnDisconnect(string empresa, string user)
        {
            await AtualizaGrupo(empresa, user, false);
            await base.OnDisconnectedAsync(null);
        }

        public async Task AtuNotificacao(string empresa, string user, string str_notifica)
        {
            string str_notific = _repHub.AtualizarNotificacoes(str_notifica);
        }

        public async Task SendEntrada(string empresa, string userID, string userIDapp, string str_entrada, string str_itens)
        {
            string str_notEnt = "";
            string str_notItens = "";

            //Grava a entrada no servidor e retorna com o ID
            str_entrada = await _repHub.GravarEntrada(Convert.ToInt64(empresa), str_entrada);
            str_itens = await _repHub.GravarEntradaItem(Convert.ToInt64(empresa), str_entrada, str_itens);


            //Precisa gravar as notificações para quem chamou para saber se recebeu a confirmação da gravação do retorno.
            str_notEnt = _repHub.GravarNotificacoes(Convert.ToInt64(empresa), Convert.ToInt64(userID), Convert.ToInt64(userID), "ntv_tbl_entrada", str_entrada);
            str_notItens = _repHub.GravarNotificacoes(Convert.ToInt64(empresa), Convert.ToInt64(userID), Convert.ToInt64(userID), "ntv_tbl_entrada_item", str_itens);

            //Envia para o usuário que chamou
            await Clients.Caller.SendAsync("ReceiveEntrada", userIDapp, str_entrada, str_itens, str_notEnt, str_notItens);


            //Notificar os usuário off-line
        }

        //Guarda pedido no servidor e envia para o administrador (cozinha) e grava notificações caso o usuario não esteja on-line
        public async Task SendPedido(string empresa, string userID, string destinatario, string str_pedido, string str_itens, string str_itensCmb, string str_itensAdic)
        {
            string str_notPed = "";
            string str_notItens = "";
            string str_notItensCmb = "";
            string str_notItensAdic = "";
            UsuarioHub dest = JsonConvert.DeserializeObject<UsuarioHub>(destinatario);

            //Grava o pedido no servidor e retorna com o ID
            str_pedido = await _repHub.GravarPedido(Convert.ToInt64(empresa), str_pedido);
            str_itens = await _repHub.GravarPedidoItem(Convert.ToInt64(empresa), str_itens);
            str_itensCmb = await _repHub.GravarPedidoItemCombo(Convert.ToInt64(empresa), str_itensCmb);
            str_itensAdic = await _repHub.GravarPedidoItemAdic(Convert.ToInt64(empresa), str_itensAdic);

            //Precisa gravar as notificações para poder aguardar a confirmação de recebimento pelo administrador/destinatário
            if (Convert.ToInt64(userID) == dest.id_usu_dest) //Administrador confirmando o pedido pois o USERID é o adm
            {
                str_notPed = _repHub.GravarNotificacoes(Convert.ToInt64(empresa), Convert.ToInt64(userID), dest.id_usu_orig, "ntv_tbl_pedido", str_pedido);
                str_notItens = _repHub.GravarNotificacoes(Convert.ToInt64(empresa), Convert.ToInt64(userID), dest.id_usu_orig, "ntv_tbl_pedidoitem", str_itens);
                str_notItensCmb = _repHub.GravarNotificacoes(Convert.ToInt64(empresa), Convert.ToInt64(userID), dest.id_usu_orig, "ntv_tbl_pedidoitemcombo", str_itensCmb);
                str_notItensAdic = _repHub.GravarNotificacoes(Convert.ToInt64(empresa), Convert.ToInt64(userID), dest.id_usu_orig, "ntv_tbl_pedidoitemadic", str_itensAdic);
            }
            else
            {
                //Verificar se o connect ID do adm ainda é o mesmo
                string str_conect_id = await VerificaAdm(Convert.ToInt64(empresa), dest.id_usu_dest);

                if (str_conect_id != "")
                {
                    if (dest.str_idconnect != str_conect_id)
                    {
                        dest.str_idconnect = str_conect_id;
                        await Clients.Caller.SendAsync("AdmConnectionID", str_conect_id, Context.ConnectionId);
                    }
                }

                str_notPed = _repHub.GravarNotificacoes(Convert.ToInt64(empresa), Convert.ToInt64(userID), dest.id_usu_dest, "ntv_tbl_pedido", str_pedido);
                str_notItens = _repHub.GravarNotificacoes(Convert.ToInt64(empresa), Convert.ToInt64(userID), dest.id_usu_dest, "ntv_tbl_pedidoitem", str_itens);
                str_notItensCmb = _repHub.GravarNotificacoes(Convert.ToInt64(empresa), Convert.ToInt64(userID), dest.id_usu_dest, "ntv_tbl_pedidoitemcombo", str_itensCmb);
                str_notItensAdic = _repHub.GravarNotificacoes(Convert.ToInt64(empresa), Convert.ToInt64(userID), dest.id_usu_dest, "ntv_tbl_pedidoitemadic", str_itensAdic);
            }

            //Envia para o administrador
            //await Clients.Client(dest.str_idconnect).SendAsync("ReceivePedido", destinatario, str_pedido, str_itens, str_itensCmb, str_notPed, str_notItens, str_notItensCmb);
            //Envia para todos os outros usuário
            await Clients.OthersInGroup(empresa).SendAsync("ReceivePedido", destinatario, str_pedido + "|||" + str_itens + "|||" + str_itensCmb + "|||" + str_itensAdic, str_notPed + "|||" + str_notItens + "|||" + str_notItensCmb + "|||" + str_notItensAdic);

            //Precisa gravar as notificações para quem chamou para saber se recebeu a confirmação da gravação do retorno.
            str_notPed = _repHub.GravarNotificacoes(Convert.ToInt64(empresa), Convert.ToInt64(userID), Convert.ToInt64(userID), "ntv_tbl_pedido", str_pedido);
            str_notItens = _repHub.GravarNotificacoes(Convert.ToInt64(empresa), Convert.ToInt64(userID), Convert.ToInt64(userID), "ntv_tbl_pedidoitem", str_itens);
            str_notItensCmb = _repHub.GravarNotificacoes(Convert.ToInt64(empresa), Convert.ToInt64(userID), Convert.ToInt64(userID), "ntv_tbl_pedidoitemcombo", str_itensCmb);
            str_notItensAdic = _repHub.GravarNotificacoes(Convert.ToInt64(empresa), Convert.ToInt64(userID), Convert.ToInt64(userID), "ntv_tbl_pedidoitemadic", str_itensAdic);

            //Envia para o usuário que chamou
            await Clients.Caller.SendAsync("ReceivePedido", destinatario, str_pedido + "|||" + str_itens + "|||" + str_itensCmb + "|||" + str_itensAdic, str_notPed + "|||" + str_notItens + "|||" + str_notItensCmb + "|||" + str_notItensAdic);


            //Notificar os usuário off-line
        }

        //COnfirma recebimento do pedido
        public async Task ConfRecPedido(string empresa, string userID, string destinatario, string str_pedido, string str_itens, string str_itensCmb)
        {
            string str_notPed = "";
            string str_notItens = "";
            string str_notItensCmb = "";
            UsuarioHub dest = JsonConvert.DeserializeObject<UsuarioHub>(destinatario);

        }


        //Atualiza Tabela de estoque do produto
        public async Task SendProdutoEstoque(string empresa, string userOrig, string str_produto)
        {
            string str_ret = "";            

            if (str_produto.Length > 0)
            {
                //Grava o pedido no servidor e retorna com o ID
                str_ret = await _repHub.GravarProdutoEstoque(Convert.ToInt64(empresa), Convert.ToInt64(userOrig), str_produto);
            }
            else
            {
                //Caso não tenha enviado produto será um download do estoque
                str_produto = await _repHub.CarregaEstoque(Convert.ToInt64(empresa));
            }


            //Envia para todos os usuário
            await Clients.OthersInGroup(empresa).SendAsync("ReceiveEstoque", str_produto);

        }

        public async Task SendProduto(string empresa, string userOrig, string userAdm, string str_produto)
        {
            string str_notProd = "";
            

            //Grava o pedido no servidor e retorna com o ID
            str_produto = await _repHub.GravarProduto(Convert.ToInt64(empresa), Convert.ToInt64(userOrig), str_produto);

            //Grava notificações no caso de desconexão do usuário
            str_notProd = _repHub.GravarNotificacoes(Convert.ToInt64(empresa), Convert.ToInt64(userOrig), Convert.ToInt64(userAdm), "ntv_tbl_produto", str_produto);

            //Envia para todos os usuário exceto quem enviou
            await Clients.OthersInGroup(empresa).SendAsync("ReceiveProduto", str_produto);
            //await Clients.Group(empresa).SendAsync("ReceiveProduto", str_produto);

        }

        public async Task SendLocal(string empresa, string userID, string str_local)
        {
            string str_notific = "";

            //Grava o pedido no servidor e retorna com o ID
            str_local = await _repHub.GravarLocal(Convert.ToInt64(empresa), str_local);

            //Precisa gravar as notificações para poder aguardar a confirmação de recebimento
            str_notific = _repHub.GravarNotificacoes(Convert.ToInt64(empresa), Convert.ToInt64(userID), 0, "ntv_tbl_local", str_local);

            //Envia para todos ataulizar id_serve e status
            //await Clients.All.SendAsync("ReceiveLocal", str_local);
            await Clients.OthersInGroup(empresa).SendAsync("ReceiveLocal", str_local, str_notific);

        }

        public async Task SendLocalCli(string empresa, string userID, string userAdm, string str_localcli)
        {
            string str_notific = "";
            //Grava o pedido no servidor e retorna com o ID
            str_localcli = await _repHub.GravarLocalCli(Convert.ToInt64(empresa), str_localcli);

            //Precisa gravar as notificações para poder aguardar a confirmação de recebimento de quem chamou
            str_notific = _repHub.GravarNotificacoes(Convert.ToInt64(empresa), Convert.ToInt64(userID), 0, "ntv_tbl_localcliente", str_localcli);
            await Clients.Caller.SendAsync("ReceiveLocalCli", str_localcli, str_notific);

            //Precisa gravar as notificações para poder aguardar a confirmação de recebimento do admnistrador ed evia para todos
            str_notific = _repHub.GravarNotificacoes(Convert.ToInt64(empresa), Convert.ToInt64(userAdm), 0, "ntv_tbl_localcliente", str_localcli);
            await Clients.OthersInGroup(empresa).SendAsync("ReceiveLocalCli", str_localcli, str_notific);
        }

        public async Task SendLocalCliPag(string empresa, string userID, string userAdm, string str_localclipag)
        {
            string str_notific = "";
            //Grava o pedido no servidor e retorna com o ID
            str_localclipag = await _repHub.GravarLocalCli(Convert.ToInt64(empresa), str_localclipag);

            //Precisa gravar as notificações para poder aguardar a confirmação de recebimento de quem chamou
            str_notific = _repHub.GravarNotificacoes(Convert.ToInt64(empresa), Convert.ToInt64(userID), 0, "ntv_tbl_localclipag", str_localclipag);
            await Clients.Caller.SendAsync("ReceiveLocalCliPag", str_localclipag, str_notific);

            //Precisa gravar as notificações para poder aguardar a confirmação de recebimento do admnistrador ed evia para todos
            str_notific = _repHub.GravarNotificacoes(Convert.ToInt64(empresa), Convert.ToInt64(userAdm), 0, "ntv_tbl_localclipag", str_localclipag);
            await Clients.OthersInGroup(empresa).SendAsync("ReceiveLocalCliPag", str_localclipag, str_notific);
        }

        public async Task ConfirmaPedido(string empresa, string userID, string destinatario, string str_pedido, string str_itens)
        {
            string str_notific = "";

            UsuarioHub dest = JsonConvert.DeserializeObject<UsuarioHub>(destinatario);

            //Grava o pedido no servidor e retorna com o ID
            str_pedido = await _repHub.GravarPedido(Convert.ToInt64(empresa), str_pedido);
            str_itens = await _repHub.GravarPedidoItem(Convert.ToInt64(empresa), str_itens);

            //Identifica o connectID do usuário do pedido
            //destinatário deve ser o id_usuario do servidor
            //Identifica se o administrador esta conectado
            string str_conect_id = await VerificaAdm(Convert.ToInt64(empresa), dest.id_usu_dest);

            //Precisa gravar as notificações para poder carregar se o usuário de destino não estive conectado
            str_notific = _repHub.GravarNotificacoes(Convert.ToInt64(empresa), Convert.ToInt64(userID), 0, "ntv_tbl_pedido", str_pedido);

            if (str_conect_id != null || str_conect_id != "")
            {
                //Envia para o destinatário a confirmação do pedido)
                //await Clients.Group(Convert.ToInt64(empresa).ToString()).Client(str_conect_id).SendAsync("ReceivePedido", destinatario, str_pedido, str_pedido);
                await Clients.Client(str_conect_id).SendAsync("ReceivePedido", destinatario, str_pedido, str_pedido, str_notific);
                //await Clients.Caller.SendAsync("ReceivePedido", destinatario, str_pedido, str_itens, str_itensCmb, str_notPed, str_notItens, str_notItensCmb);

            }
        }

        public async Task SendMessage(string empresa, string userID, string msg)
        {
            //Ao usar o método Client(_connections.GetUserId(chat.destination)) eu estou enviando a mensagem apenas para o usuário destino, não realizando broadcast


            //Comentado para testes
            //await Clients.Group(empresa).SendAsync("ReceivePedido", msg, msg);
            //await Clients.Client(userID).SendAsync("Receive", msg, msg);
            await Clients.All.SendAsync("ReceiveMessage", msg);
        }

        public async Task ImprimePedido(string empresa, string userID, string msg)
        {
            //Ao usar o método Client(_connections.GetUserId(chat.destination)) eu estou enviando a mensagem apenas para o usuário destino, não realizando broadcast


            //Comentado para testes
            //await Clients.Group(empresa).SendAsync("ReceivePedido", msg, msg);
            //await Clients.Client(userID).SendAsync("Receive", msg, msg);
            await Clients.All.SendAsync("ImprimePedido", msg);
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

    }
}
