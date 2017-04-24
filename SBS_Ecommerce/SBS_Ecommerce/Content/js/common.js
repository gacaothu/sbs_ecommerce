var availableTags = [];

$(function () {
    getTags();
    //$("#tags").autocomplete({
    //    source: availableTags,
    //    classes: {
    //        "ui-autocomplete": "search"
    //    }
    //});
});

function getTags() {
    $.ajax({
        url: UrlContent('/Product/GetTags'),
        success: function (rs) {
            availableTags = rs;
        },
        async: false,
        error: function (rs) {
            console.log(rs);
        }
    });
}

function addToWishlist(id) {
    $.ajax({
        type: 'POST',
        url: UrlContent('/Wishlist/InsertToWishlist'),
        data: {
            id: id
        },
        success: function (rs) {
            if (rs.reponse == 0) {
                console.log("success");
            } else if (rs.reponse == 1) {
                console.log("existed");
            } else if (rs.reponse == -1) {
                console.log("failed");
            } else {
                window.location.href = UrlContent("/Account/Login");
            }
        }
    });
}

function removeFromWishlist(id) {
    $.ajax({
        type: 'POST',
        url: UrlContent('/Wishlist/RemoveFromWishlist'),
        data: { id: id },
        success: function (rs) {
            if (rs.reponse == 0) {
                location.reload();
            } else if (rs.reponse == -1) {
                console.log("failed");
            } else {
                window.location.href = UrlContent("/Account/Login");
            }
        }
    });
}

function addToCart(id, quantity) {
    $.ajax({
        url: UrlContent("/Product/AddCart"),
        data: { id: id, count: quantity },
        success: function (rs) {
            // window.location.href = UrlContent("/Product/Checkout");
            $('#successModal').modal('show');
        }
    });
}

function removeFromCart(id) {
    $.ajax({
        url: UrlContent("/Product/RemoveCart"),
        data: { id: id },
        success: function (rs) {
            window.location.reload();
        }
    });
}

function removeCartHome(id) {
    $.ajax({
        url: UrlContent("/Product/RemoveCart"),
        data: { id: id },
        success: function (rs) {
            window.location.reload();
        }
    });
}

$(document).on('click', '#successModal', function () {
    window.location.reload();
});

function validateEmail(email) {
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}