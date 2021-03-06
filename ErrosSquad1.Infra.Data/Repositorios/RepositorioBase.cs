﻿using ErrosSquad1.Dominio.Entidades;
using ErrosSquad1.Dominio.Interfaces.Repositorios;
using ErrosSquad1.Infra.Data.Contextos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ErrosSquad1.Infra.Data.Repositorios
{
    public class RepositorioBase<TEntidade> : IRepositorioBase<TEntidade>
        where TEntidade : EntidadeBase
    {
        protected readonly AppDbContext contexto;

        public RepositorioBase(AppDbContext contexto)
            : base()
        {
            this.contexto = contexto;
        }

        /*public void Excluir(int id)
        {
            var entidade = SelecionarPorId(id);
            if (entidade != null)
            {
                contexto.InitTransacao();
                contexto.Set<TEntidade>().Remove(entidade);
                contexto.SendChanges();
            }
        }*/

        public void Excluir(TEntidade entidade)
        {
            contexto.InitTransacao();
            contexto.Set<TEntidade>().Remove(entidade);
            contexto.SendChanges();
        }

        public TEntidade SelecionarPorId(int id)
        {
            return contexto.Set<TEntidade>().Find(id);
        }

        public IEnumerable<TEntidade> SelecionarTodos()
        {
            return contexto.Set<TEntidade>().ToList();
        }
    }
}
