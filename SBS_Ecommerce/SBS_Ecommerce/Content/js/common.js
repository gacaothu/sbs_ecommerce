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
                showNotModal('Add product to Wishlist successful.');
            } else if (rs.reponse == 1) {
                showNotModal('Product already existed in Wishlist.');
            } else if (rs.reponse == -1) {
                showNotModal('Error occurred while adding Wishlist.');
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
                showNotModal('Error occurred while removing Wishlist.');
            } else {
                window.location.href = UrlContent("/Account/Login");
            }
        }
    });
}

function addToCart(id, quantity) {
    addCart(id, quantity);
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
    removeCart(id);
}

function addCartFromDetail(id) {
    var count = $('#quantity_wanted_content').val();
    if (!count) {
        $('#quantity_wanted_content-error').text('Quantity cannot be empty.', false);
        $('#quantity_wanted_content-error').attr('hidden', false);
        return false;
    }
    if (parseInt(count) < 0) {
        $('#quantity_wanted_content-error').text('Quantity cannot smaller than zero.', false);
        $('#quantity_wanted_content-error').attr('hidden', false);
        return false;
    }
    if (parseInt(count) == 0) {
        $('#quantity_wanted_content-error').text('Quantity cannot be equal zero.', false);
        $('#quantity_wanted_content-error').attr('hidden', false);
        return false;
    }
    else {
        addCart(id, count);
    }
}

function showNotModal(msg) {
    $('#not-modal #not-content').text(msg);
    $('#not-modal').modal('show');
    setTimeout(function () {
        $('#not-modal').modal('hide');
    }, 1800);
}

function removeCart(id) {
    $.ajax({
        url: UrlContent("/Product/RemoveCart"),
        data: { id: id },
        success: function (rs) {
            $('.block-minicart').empty();
            $('.block-minicart').append(rs.Partial);
        }
    });
}

function addCart(id, number) {
   
    $.ajax({
        url: UrlContent("/Product/AddCart"),
        data: { id: id, count: number},
        success: function (rs) {
            if (rs != "") {
                $('.block-minicart').empty();
                $('.block-minicart').append(rs);
                $('#successModal #contentAlert').text('Add product to cart successful');
                $('#successModal').modal('show');
          
            } else {
                $('#successModal #contentAlert').text('Cannot add this product to cart');
                $('#successModal').modal('show');
            }            
            setTimeout(function () {
                $('#successModal').modal('hide');
            }, 1800);
        }
    });
}

function validateEmail(email) {
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}