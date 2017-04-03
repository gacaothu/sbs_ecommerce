var availableTags = [];

$(function () {
    getTags();
    $("#tags").autocomplete({
        source: availableTags,
        classes: {
            "ui-autocomplete": "search"
        }
    });
});

function getTags() {
    $.ajax({
        url: '/Product/GetTags',
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
        url: '/Wishlist/InsertToWishlist',
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
                alert("Please login first.");
            }
        }
    });
}

function removeFromWishlist(id) {
    $.ajax({
        type: 'POST',
        url: '/Wishlist/RemoveFromWishlist',
        data: { id: id },
        success: function (rs) {
            if (rs.reponse == 0) {
                location.reload();
            } else if (rs.reponse == -1) {
                console.log("failed");
            } else {
                alert("Please login first.");
            }
        }
    });
}

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