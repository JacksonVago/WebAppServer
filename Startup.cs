using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
/*using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;*/
using System.Text;
using System.Threading.Tasks;
using WebAppServer.Models;
using WebAppServer.Repositories;
using WebAppServer.Hubs;
using Microsoft.IdentityModel.Tokens;

namespace WebAppServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var chave = Encoding.ASCII.GetBytes(Tokenchavehash.chave);

            services.AddScoped<AppRepository>();
            services.AddScoped<UsuarioRepository>();
            services.AddScoped<UsuarioAcessoRepository>();
            services.AddScoped<SalaRepository>();
            services.AddScoped<PedidoRepository>();
            services.AddScoped<HubRepository>();
            services.AddScoped<SyncRepository>();
            services.AddScoped<EmailRepository>();
            services.AddScoped<NotificacoesRepository>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = "natividadesolucoes.com.br",
                    ValidAudience = "natividadesolucoes.com.br",
                    IssuerSigningKey = new SymmetricSecurityKey(chave)
                };
            }
            );
            services.AddCors(options => {
                options.AddPolicy(name: "AllowSpecificOrigins",
                                      policy =>
                                      {
                                          /*policy.WithOrigins("http://localhost:3000",
                                                            "http://192.168.1.136:3000",
                                                             "http://107.22.1.181",
                                                             "http://192.168.1.226:3000",
                                                             "http://35.174.17.110");*/
                                          policy.AllowAnyOrigin();
                                          policy.WithOrigins("*");
                                          policy.AllowAnyHeader();
                                      });
            });
            services.AddControllersWithViews();
            services.AddSignalR();            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("AllowSpecificOrigins");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();            
            app.UseEndpoints(endpoints =>
            {                
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<AppHub>("AppHub");
            });

            /*app.UseSignalR(routes =>
            {
                routes.MapHub<AppHub>("/Hubs/AppHub");
            });*/

        }
    }
}
