function addToCart(id, quantity) {
    $.ajax({
        url: "/Product/AddCart",
        data: { id: id, count: quantity },
        success: function (rs) {
            window.location.href = "/Product/Checkout";
        }
    });
}

function removeFromCart(id) {
    $.ajax({
        url: "/Product/RemoveCart",
        data: { id: id },
        success: function (rs) {
            window.location.reload();
        }
    });
}

function removeCartHome(id) {
    $.ajax({
        url: "/Product/RemoveCart",
        data: { id: id },
        success: function (rs) {
            window.location.reload();
        }
    });
}