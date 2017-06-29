
class Carrinho {
    
    clickIncremento(btn) {

        var data = this.getData(btn)
        data.Quantidade++;
        this.postQuantidade(data);

    }
    clickDecremento(btn) {

        var data = this.getData(btn)
        data.Quantidade--;
        this.postQuantidade(data);

    }

    updateQuantidade(input) {
        var data = this.getData(input);
        this.postQuantidade(data);
    }

    getData(elemento) {
        var linhaDoItem = $(elemento).parents('[item-id]');
            var itemId = linhaDoItem.attr('item-id');
            var novaQtd = linhaDoItem.find('input').val();

            return {
                Id: itemId,
                Quantidade: novaQtd
            };
    }

    postQuantidade(data) {
        var token = $('input[name=__RequestVerificationToken]').val();
        var heder = {};
        heder['RequestVerificationToken'] = token;

        $.ajax({
            url: '/pedido/PostQuantidade',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            headers: heder
        }).done(function (response) {
            this.setQuatidade(response.itemPedido);
            this.setSubTotal(response.itemPedido);
            this.setTotal(response.carrinhoViewModel);
            this.setNumeroItem(response.carrinhoViewModel);
            if (response.itemPedido.quantidade == 0) {
                this.removeItem(response.itemPedido);
            }
            }.bind(this));
    }

    setQuatidade(itemPedido) {
        this.getLinhaDoItem(itemPedido)
            .find('input').val(itemPedido.quantidade);
    }

    setSubTotal(itemPedido) {
        this.getLinhaDoItem(itemPedido)
            .find('[subtotal]').html(itemPedido.subtotal.duasCasas());
    }    

    setTotal(carrinhoViewModel) {
        $('[total]').html(carrinhoViewModel.total.duasCasas());
    }

    getLinhaDoItem(itemPedido) {
        return $('[item-id=' + itemPedido.id + ']');
    }

    removeItem(itemPedido) {
        this.getLinhaDoItem(itemPedido).remove();
    }

    setNumeroItem(carrinhoViewModel) {
        var texto =
            'Total: '
            + carrinhoViewModel.itens.length
            + ' '
            + (carrinhoViewModel.itens.length > 1
                ? 'itens' : 'item');
            $('[numero-item]').html(texto);
    }
}

var carrinho = new Carrinho();

Number.prototype.duasCasas = function () {
    return this.toFixed(2).replace('.', ',');

}