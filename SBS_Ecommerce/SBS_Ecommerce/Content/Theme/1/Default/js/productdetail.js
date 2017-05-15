function AddProductReview(el) {
    var parent = $(el).parent().parent();
    var title = $(parent).find('.titleReview').val();
    var comment = $(parent).find('.commentReview').val();
    var name = $(parent).find('.nameReview').val();
    var rate = $('.star-rating').find('.rate-in').length;
    var prID = $(el).attr('data-id');
    if (title == '') {
        $('.validateTileReview').show();
        $('.titleReview').css('border', 'solid 1px red');
        return;
    }
    else {
        $('.validateTileReview').hide();
        $('.titleReview').css('border', '1px solid #dedede');
    }

    if (comment == '') {
        $('.validateCommentReview').show();
        $('.commentReview').css('border', 'solid 1px red');
        return;
    }
    else {
        $('.validateCommentReview').hide();
        $('.commentReview').css('border', '1px solid #dedede');
    }

    if (name == '') {
        $('.validateNameReview').show();
        $('.nameReview').css('border', 'solid 1px red');
        return;
    }
    else {
        $('.validateNameReview').hide();
        $('.nameReview').css('border', '1px solid #dedede');
    }

    $.ajax({
        url: UrlContent('/Product/ReviewProduct'),
        data: { rate: rate, title: title, name: name, comment: comment, prID: prID },
        success: function (rs) {
            $('#reviewProductModal').modal('hide');
            $('#successModal').find('#contentAlert').text('Product review successful added');
            $('#successModal').modal('show');
        }
    });

}


function ConfirmDeleteProductReview(id) {
    $('#confirm-delete').attr('data-id', id);
    $('#confirm-delete').modal('show');
}

function DeleteProductReview(id) {
    $.ajax({
        url: UrlContent('/Product/DeleteReview'),
        data: { id: id },
        success: function (rs) {
            $('#confirm-delete').modal('hide');
            $('#successModal').find('#contentAlert').text('Product review successful deleted');
            $('#successModal').modal('show');
        }
    });
}

function ShowEditReview(reviewID, productID) {
    $.ajax({
        url: UrlContent('/Product/GetReview'),
        data: { reviewID: reviewID, productID: productID },
        success: function (rs) {
            if (rs != false) {
                $('#editReviewProductModal').attr('data-id', reviewID);
                $('#editReviewProductModal').attr('data-productID', productID);
                $('#editReviewProductModal').find('.titleReview').val(rs.Title);
                $('#editReviewProductModal').find('.commentReview').val(rs.Content);
                $('#editReviewProductModal').find('.nameReview').val(rs.Name);
                var rating = $('#editReviewProductModal').find('.star-rating');

                $(rating).find('.rating').each(function () {
                    $(this).removeClass('rate-in').css('color', '#dedede');
                });

                for (var i = 0; i < rs.Rate; i++) {
                    $(rating).find('.rating').eq(i).addClass('rate-in').css('color', 'rgb(235, 88, 88)');
                }
                $('#editReviewProductModal').modal('show');
            }
        }
    });
}

function EditReview(el) {
    var parent = $(el).parent().parent();
    var title = $(parent).find('.titleReview').val();
    var comment = $(parent).find('.commentReview').val();
    var name = $(parent).find('.nameReview').val();
    var rate = $('.star-rating').find('.rate-in').length;
    var id = $('#editReviewProductModal').attr('data-id');
    var productID = $('#editReviewProductModal').attr('data-productID');
    if (title == '') {
        $('.validateTileReview').show();
        $('.titleReview').css('border', 'solid 1px red');
        return;
    }
    else {
        $('.validateTileReview').hide();
        $('.titleReview').css('border', '1px solid #dedede');
    }

    if (comment == '') {
        $('.validateCommentReview').show();
        $('.commentReview').css('border', 'solid 1px red');
        return;
    }
    else {
        $('.validateCommentReview').hide();
        $('.commentReview').css('border', '1px solid #dedede');
    }

    if (name == '') {
        $('.validateNameReview').show();
        $('.nameReview').css('border', 'solid 1px red');
        return;
    }
    else {
        $('.validateNameReview').hide();
        $('.nameReview').css('border', '1px solid #dedede');
    }

    $.ajax({
        url: UrlContent('/Product/EditReview'),
        data: { rate: rate, title: title, name: name, comment: comment, id: id, productID: productID },
        success: function (rs) {
            $('#editReviewProductModal').modal('hide');
            $('#successModal').find('#contentAlert').text('Product review successful edited');
            $('#successModal').modal('show');
        }
    });
}

function Writereview() {
    //Clear data

    $('#reviewProductModal').modal('show');
}

$(document).on('click', '#successModal', function () {
    window.location.reload();
});