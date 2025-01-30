using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAppServer.Services;
using WebAppServer.Repositories;
using WebAppServer.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;

namespace WebAppServer.Controllers
{
    [Route("api/AppPraia/[controller]")]
    [ApiController]
    public class TokenController : Controller
    {
        private readonly UsuarioRepository _repository;
        private readonly UsuarioAcessoRepository _repAcesso;
        private readonly IHttpContextAccessor _httpContext;
        private readonly AppRepository _repApp;

        public TokenController(UsuarioRepository repository, UsuarioAcessoRepository repAcess, IHttpContextAccessor contextAccessor, AppRepository repapp)
        {
            _repository = repository;
            _repAcesso = repAcess;
            _httpContext = contextAccessor;
            _repApp = repapp;
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateJwtTokenAsync([FromBody] Usuariotoken user)
        {
            UsuarioAcesso user_ret = new UsuarioAcesso();
            user_ret = await _repository.ValidaUsuarioPostgres(user.username, user.password);
            //user_ret = await _repository.ValidaUsuario(user.username, user.password);
            if (user_ret != null)
            {
                if (user_ret.id > 0)
                {
                    try
                    {
                        var token = TokenService.GeraToken(user_ret);
                        if (token != null)
                        {
                            var ip = _httpContext.HttpContext.Connection.LocalIpAddress.ToString();
                            var metodo = _httpContext.HttpContext.Request.Path.ToString();

                            if (await _repository.GravarAcessoPostgres(user_ret, token.access_token, ip, metodo, JsonConvert.SerializeObject(user_ret)))
                            {
                                return Ok(new
                                {
                                    token = token,
                                    user = user_ret

                                });
                            }
                            else
                            {
                                return NotFound("Problemas na gravação do acesso.");
                            }
                        }
                        else
                        {
                            return NotFound("Usuário não cadastrado em nosso sistema");
                        }
                    }
                    catch (Exception ex)
                    {
                        return NotFound(new { message = ex.Message.ToString() });
                    }
                }
                else
                {
                    return BadRequest(user_ret.username);
                }
            }

            return BadRequest("Credenciais de usuário inválida.");
        }


        [HttpPost("newUserCreaterToken")]
        public async Task<IActionResult> newUserCreaterTokenAsync([FromBody] Usuariotoken user)
        {
            UsuarioAcesso user_ret = new UsuarioAcesso();
            user_ret.id = DateTime.Now.Year * -1;
            user_ret.id_empresa = 0;
            user_ret.int_tipo = 1;
            user_ret.username = user.username;
            user_ret.password = "123";
            user_ret.validade = DateTime.Now.AddDays(1);

            if (user_ret != null && user_ret.id == DateTime.Now.Year * -1)
            {
                try
                {
                    var token = TokenService.GeraToken(user_ret);
                    if (token != null)
                    {
                        var ip = _httpContext.HttpContext.Connection.LocalIpAddress.ToString();
                        var metodo = _httpContext.HttpContext.Request.Path.ToString();

                        if (await _repository.GravarAcessoPostgres(user_ret, token.access_token, ip, metodo, JsonConvert.SerializeObject(user_ret)))
                        {
                            return Ok(new
                            {
                                token = token,
                                user = user_ret

                            });
                        }
                        else
                        {
                            return NotFound("Problemas na gravação do acesso.");
                        }
                    }
                    else
                    {
                        return NotFound("Usuário não cadastrado em nosso sistema");
                    }
                }
                catch (Exception ex)
                {
                    return NotFound(new { message = ex.Message.ToString() });
                }
            }

            return BadRequest("Credenciais de usuário inválida.");
        }

        [HttpGet("v1/FirstAcess/{email}")]
        public async Task<dynamic> PrimeiroAcesso(string email)
        {
            try
            {
                dynamic ret = await _repAcesso.PrimeiroAcessoEmpPostgres(email);
                return ret;
                /*return Ok(new
                {
                    UserPrimAcess = ret

                })*/
            }
            catch(Exception ex)
            {
                return NotFound(new { message = ex.Message.ToString() });
            }
        }

        [HttpGet("v1/ValidUser/{email}")]
        public async Task<dynamic> VerificaUsuario(string email)
        {
            try
            {
                dynamic ret = await _repAcesso.VerificaUsuarioPostgres(email);
                return ret;
                /*return Ok(new
                {
                    UserAcess = ret

                });*/
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message.ToString() });
            }
        }

        [HttpGet("v1/ValidCodAcess")]
        public async Task<dynamic> ValPrimAcess([FromQuery] string email, [FromQuery] Int64 codigo)
        {
            try
            {

                dynamic ret = await _repAcesso.ValPrimeiroAcesso(email, codigo);
                return ret;
                /*return Ok(new
                {
                    UserPrimAcess = ret

                });*/
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message.ToString() });
            }
        }

        [HttpPost("v1/LoginCardapio")]
        public async Task<ActionResult<dynamic>> LoginCardapio([FromBody] ParametrosEntrada paramEntrada)
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

        [HttpPost("v1/LocalCardapioPostgres")]
        public async Task<ActionResult<dynamic>> LocalCardapioPostgres([FromBody] DadosPostgres dados)
        {
            dynamic dyn_retorno = null;
            try
            {
                string sqlStr = "select * from f_sel_localCliente_cardapio(" + dados.Dados + ")";
                dyn_retorno = _repApp.ExecutaSqlPostgres(sqlStr);
                return dyn_retorno;
            }
            catch (Exception ex)
            {
                return NotFound(new { mensagem = ex.Message.ToString() });
            }

        }

        [HttpGet("v1/ValidaUserPostgres/{email}")]
        public async Task<dynamic> ValidaUserPostgres(string email)
        {
            try
            {
                dynamic ret = await _repAcesso.ValidaUsuarioPostgres(email);
                return ret;
                /*return Ok(new
                {
                    UserAcess = ret

                });*/
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message.ToString() });
            }
        }

        [HttpPost("v1/UpdPrimAcess")]
        public async Task<dynamic> AtuPrimAcess([FromBody] UserPrimAcess primAcess)
        {

            dynamic ret = await _repAcesso.AtuPrimeiroAcessoPostgres(primAcess);
            return ret;
        }

        [HttpGet("v1/ok")]
        public async Task<string> ok()
        {
            string _strHrInicioVnd;
            _strHrInicioVnd = "00:00:01";
            if (Convert.ToDateTime(_strHrInicioVnd) < DateTime.Now)
            {
                    dynamic ret = await _repAcesso.VerificaUsuarioPostgres("jackson@natividadesolucoes.com.br");
                return "Verdadeiro";
            }
            else
            {
                return "False";
            }
        }
    }
}
