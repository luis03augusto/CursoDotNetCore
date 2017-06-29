using CasaDoCodigo.Aula7;
using CasaDoCodigo.Aula7.Models;
using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CasaDoCodigo
{
    public class DataService : IDataService
    {
        private readonly Contexto _contexto;
        private readonly IHttpContextAccessor _contextAccesor;
        public DataService(Contexto contexto, IHttpContextAccessor contextAcessor)
        {
            this._contexto = contexto;
            this._contextAccesor = contextAcessor;
        }

        public void AddItemPedido(int produtoId)
        {
            var produto = _contexto.Produtos.FirstOrDefault(p => p.Id == produtoId);

            if (produto != null)
            {
                int? pedidoId = GetSessionPedidoId();

                Pedido pedido = null;

                if (pedidoId.HasValue)
                {
                    pedido = _contexto.Pedidos
                   .Where(p => p.Id == pedidoId.Value)
                   .SingleOrDefault();
                }

                if (pedido == null)
                {
                    pedido = new Pedido();
                }

                if (!_contexto.ItensPedido.Where(i => i.Produto.Id == produtoId && i.Pedido.Id == pedido.Id).Any())
                {
                    _contexto.ItensPedido.Add(new ItemPedido(pedido, produto, 1));
                    _contexto.SaveChanges();
                    SetSessionPedidoId(pedido);
                }
            }
        }

        private void SetSessionPedidoId(Pedido pedido)
        {
            _contextAccesor.HttpContext
                                    .Session.SetInt32("pedidoId", pedido.Id);
        }

        private int? GetSessionPedidoId()
        {
            return _contextAccesor.HttpContext.Session.GetInt32("pedidoId");
        }

        public List<ItemPedido> GetItensPedido()
        {
            var pedidoId = GetSessionPedidoId();
            var pedido = _contexto.Pedidos
                .Where(p => p.Id == pedidoId)
                .Single();
            return this._contexto.ItensPedido.Where(i => i.Pedido.Id == pedido.Id).ToList();
        }

        public List<Produto> GetProdutos()
        {
            return this._contexto.Produtos.ToList();
        }

        public void InicializaDB()
        {
            this._contexto.Database.EnsureCreated();
            if (this._contexto.Produtos.Count() == 0)
            {
                List<Produto> produtos = new List<Produto>
                {
                    new Produto("Sleep not found", 59.90m),
                    new Produto("May the code be with you", 59.90m),
                    new Produto("Rollback", 59.90m),
                    new Produto("REST", 69.90m),
                    new Produto("Design Patterns com Java", 69.90m),
                    new Produto("Vire o jogo com Spring Framework", 69.90m),
                    new Produto("Test-Driven Development", 69.90m),
                    new Produto("iOS: Programe para iPhone e iPad", 69.90m),
                    new Produto("Desenvolvimento de Jogos para Android", 69.90m)
                };

                foreach (var produto in produtos)
                {
                    this._contexto.Produtos
                        .Add(produto);

                    //this._contexto.ItensPedido
                    //    .Add(new ItemPedido(produto, 1));
                }

                this._contexto.SaveChanges();
            }
        }

        public UpdateItemPedidoResponse UpdateItemPedido(ItemPedido itempedido)
        {
            var itempedidoDB = _contexto.ItensPedido.FirstOrDefault(i => i.Id == itempedido.Id);
            if (itempedidoDB != null)
            {
                itempedidoDB.AtualizaQuantidade(itempedido.Quantidade);

                if (itempedidoDB.Quantidade == 0)
                    _contexto.ItensPedido.Remove(itempedidoDB);
              
                _contexto.SaveChanges();
            }
            var itensPedido = _contexto.ItensPedido.ToList();
            var carriViewModel = new CarrinhoViewModel(itensPedido);
            return new UpdateItemPedidoResponse(itempedidoDB, carriViewModel);
        }

        public Pedido GetPedido()
        {
            int? pedidoId = GetSessionPedidoId();

            return _contexto.Pedidos
                .Include(p => p.Itens)
                .ThenInclude(p => p.Produto)
                .Where(p => p.Id == pedidoId)
                .SingleOrDefault();
        }

        public Pedido UpdateCadastro(Pedido cadastro)
        {
            var pedido = GetPedido();
            pedido.UpdDateCadastro(cadastro);
            _contexto.SaveChanges();
            return pedido;
        }
    }
}
