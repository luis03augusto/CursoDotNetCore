using CasaDoCodigo.Aula7;
using CasaDoCodigo.Aula7.Models;
using CasaDoCodigo.Models;
using System.Collections.Generic;

namespace CasaDoCodigo
{
    public interface IDataService
    {
        void InicializaDB();
        List<Produto> GetProdutos();
        List<ItemPedido> GetItensPedido();
        UpdateItemPedidoResponse UpdateItemPedido(ItemPedido itempedido);
        void AddItemPedido(int produtoId);
        Pedido GetPedido();
        Pedido UpdateCadastro(Pedido cadastro);
    }
}