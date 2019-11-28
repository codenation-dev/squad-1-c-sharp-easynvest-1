﻿using AutoMapper;
using ErrosSquad1.Aplicacao;
using ErrosSquad1.Dominio.Entidades;
using ErrosSquad1.Infra.Data.Contextos;
using ErrosSquad1.Infra.IoC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ErrosSquad1.Servicos.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            //todo : está duplicada a string de conexão rever
            services.AddDbContext<AppDbContext>(o => o.UseSqlServer(Configuration.GetConnectionString(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=projeto_final;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False")));
            InjetorDependencias.Registrar(services);
            //services.AddAutoMapper(x => x.AddProfile(new MappingEntidade()));
            services.AddAutoMapper(typeof(MappingEntidade).Assembly);
            //inicio token
            var key = System.Text.Encoding.ASCII.GetBytes(Settings.Secret);
            services.AddCors();
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            //final token
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
           
            //inicio token
            app.UseAuthentication();
            //final token
            app.UseCors(a => a.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseMvc();
        }
    }
}
