$(".update-profile").click(function () {
    $(".update-form").show();
    $(".general-form").hide()
})
$(".update-shipping-address").click(function () {
    $(".shipping-update").show();
    $(".shipping-form").hide();
    $("#add-shipping").hide();
})
$(".add-shipping").click(function () {
    $("#add-shipping").show();
    $(".shipping-update").hide();
    $("#shippingForm").hide();
})

function deletecustomeraddress(addressId) {
    if (confirm('Are you sure?')) {
        var postData = {
            addressId: addressId
        };
        addAntiForgeryToken(postData);

        $.ajax({
            cache: false,
            type: 'POST',
            url: '/Customer/AddressDelete',
            data: postData,
            dataType: 'json',
            success: function (data) {
                location.href = data.redirect;
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert('Failed to delete');
            }
        });
    }
}