﻿using Microsoft.EntityFrameworkCore;
using ErrosSquad1.Dominio.Entidades;
using ErrosSquad1.Infra.Data.Mapeamentos;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using Microsoft.Extensions.Configuration;

namespace ErrosSquad1.Infra.Data.Contextos
{
    public class AppDbContext : DbContext
    {
        private IConfiguration Configuration { get; }

        public AppDbContext(
                    DbContextOptions<AppDbContext> options,
                    IConfiguration configuration) : base(options)
        {
            Configuration = configuration;
        }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Nivel> Niveis { get; set; }

        public DbSet<Ambiente> Ambientes { get; set; }

        public DbSet<Erro> Erros { get; set; }


        public IDbContextTransaction Transaction { get; private set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ErroMap());
            modelBuilder.ApplyConfiguration(new NivelMap());
            modelBuilder.ApplyConfiguration(new UsuarioMap());
            modelBuilder.ApplyConfiguration(new AmbienteMap());

        }

        public IDbContextTransaction InitTransacao()
        {
            if (Transaction == null) Transaction = this.Database.BeginTransaction();
            return Transaction;
        }

        private void RollBack()
        {
            if (Transaction != null)
            {
                Transaction.Rollback();
            }
        }

        private void Salvar()
        {
            try
            {
                ChangeTracker.DetectChanges();
                SaveChanges();
            }
            catch (Exception ex)
            {
                RollBack();
                throw new Exception(ex.Message);
            }
        }

        private void Commit()
        {
            if (Transaction != null)
            {
                Transaction.Commit();
                Transaction.Dispose();
                Transaction = null;
            }
        }

        public void SendChanges()
        {
            Salvar();
            Commit();
        }

    }
}
