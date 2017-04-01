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

function DeleteCustomerAddress(addressId) {
    if (confirm('Are you sure?')) {
        var postData = {
            addressId: addressId
        };
        $.ajax({
            cache: false,
            type: 'POST',
            url: '/Account/AddressDelete',
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
/* Start Jquery page change password */
 $("#frmChangePass").submit(function () {
    $('.text-success').css("display", "none");
})
/* End Jquery page change password */