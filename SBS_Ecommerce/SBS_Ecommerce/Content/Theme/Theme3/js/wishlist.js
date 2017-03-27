var host = window.location.href;

function addToWishlist(id) {
    console.log(host);
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