var availableTags = [
    "ActionScript",
    "AppleScript",
    "Asp",
    "BASIC",
    "C",
    "C++",
    "Clojure",
    "COBOL",
    "ColdFusion",
    "Erlang",
    "Fortran",
    "Groovy",
    "Haskell",
    "Java",
    "JavaScript",
    "Lisp",
    "Perl",
    "PHP",
    "Python",
    "Ruby",
    "Scala",
    "Scheme"
];

$(function () {
    $("#tags").autocomplete({
        source: availableTags,
        classes: {
            "ui-autocomplete": "search"
        }
    });
});

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