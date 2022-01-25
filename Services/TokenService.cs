//using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAppServer.Models;
/*using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;*/

namespace WebAppServer.Services
{
    public class TokenService
    {
        public static Token GeraToken(UsuarioAcesso user)
        {
            var str_retorno = "ok";
            
            /*try
            {
                var chave = Encoding.ASCII.GetBytes(Tokenchavehash.chave);
                var claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.username.ToString()),
                    new Claim("id", user.id.ToString()),
                    new Claim(ClaimTypes.Expired, user.validade.ToString())
                    //new Claim(ClaimTypes.Role, "usuario")
                };
                var key = new SymmetricSecurityKey(chave);
                var credenc = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "natividadesolucoes.com.br",
                    audience: "natividadesolucoes.com.br",
                    claims: claims,
                    expires: user.validade,
                    signingCredentials: credenc);
                var tokenHandler = new JwtSecurityTokenHandler();
                str_retorno = tokenHandler.WriteToken(token);
                TimeSpan tempo = DateTime.Now.Subtract(user.validade);
                return new Token { access_token = str_retorno, token_type = "Bearer", expires_in = Convert.ToInt64(tempo.TotalSeconds) };
            }
            catch (Exception ex)
            {
                str_retorno = "mensagem:" + ex.Message.ToString();
                throw ex;
            }*/
            return new Token { access_token = str_retorno, token_type = "Bearer", expires_in = 36000 };
        }
    }
}
