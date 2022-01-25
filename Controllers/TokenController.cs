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

namespace WebAppServer.Controllers
{
    [Route("api/AppPraia/[controller]")]
    [ApiController]
    public class TokenController : Controller
    {
        private readonly UsuarioRepository _repository;
        private readonly UsuarioAcessoRepository _repAcesso;
        private readonly IHttpContextAccessor _httpContext;

        public TokenController(UsuarioRepository repository, UsuarioAcessoRepository repAcess, IHttpContextAccessor contextAccessor)
        {
            _repository = repository;
            _repAcesso = repAcess;
            _httpContext = contextAccessor;

        }

        
        [HttpPost]
        public async Task<IActionResult> CreateJwtTokenAsync([FromBody] Usuariotoken user)
        {
            UsuarioAcesso user_ret = new UsuarioAcesso();
            user_ret = await _repository.ValidaUsuario(user.username, user.password);
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

                            if (await _repository.GravarAcesso(user_ret, token.access_token, ip, metodo, JsonConvert.SerializeObject(user_ret)))
                            {
                                return Ok(new
                                {
                                    token = token

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
        

        [HttpGet("v1/PrimeiroAcesso/{email}")]
        public async Task<dynamic> PrimeiroAcesso(string email)
        {
            try
            {
                dynamic ret = await _repAcesso.PrimeiroAcessoEmp(email);
                //return ret;
                return Ok(new
                {
                    UserPrimAcess = ret

                });
            }
            catch(Exception ex)
            {
                return NotFound(new { message = ex.Message.ToString() });
            }
        }

        
        [HttpGet("v1/ValidaCodAcess")]
        public async Task<dynamic> ValPrimAcess([FromQuery] string email, [FromQuery] Int64 codigo)
        {
            try
            {

                dynamic ret = await _repAcesso.ValPrimeiroAcesso(email, codigo);
                return Ok(new
                {
                    UserPrimAcess = ret

                });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message.ToString() });
            }
        }

        [HttpGet("v1/ok")]
        public async Task<string> ok()
        {
            return "Acessei";
        }
    }
}
