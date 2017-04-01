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
        url: '/Product/ReviewProduct',
        data: { rate: rate, title: title, name: name, comment: comment, prID :prID},
        success: function (rs) {
            alert('ok');
        }
    });

}