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
            url: UrlContent('/Account/AddressDelete'),
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

// Function select address customer page checkout 
function SelectCustomerAddress(shippingAddressId, billingAddressId) {
    var billingAddress = $("#ckbillingaddress").is(':checked');

    if (billingAddress == true && billingAddressId == 0) {
        $("#AddBillingAddressCheckOut").submit();
    } else {
        var postData = {
            shippingAddressId: shippingAddressId,
            billingAddressId: billingAddressId,
            isBillingAddress: billingAddress
        };
        $.ajax({
            cache: false,
            type: 'GET',
            url: UrlContent('/Account/ChooseAddressShipping'),
            data: postData,
            dataType: 'json',
            success: function (data) {
                location.href = data.redirect;
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert('Failed to choose');
            }
        });
    }
}

//Function remove avatar page profile 
function RemoveAvartar() {
    if (confirm('Are you sure?')) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: UrlContent('/Account/RemoveAvatar'),
            data: null,
            dataType: 'json',
            success: function (data) {
                location.href = data.redirect;
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert('Failed to remove avatar');
            }
        });
    }
}

function AddNewShippingAddrress() {
    $('.add-more-shipping').css("display", "block");
}
function closeAddNewShippingAddrress() {
    $('.add-more-shipping').css("display", "none");
}

function closeAddNewBillingAddrress() {
    $('.address-billing').css("display", "none");
    $("#ckbillingaddress").prop('checked', false);
}

function ChooseShippingPayment() {
    $("#frmChooseShippingPayment").submit();
}
function showMyImage(fileInput) {
    var files = fileInput.files;
    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        var imageType = /image.*/;
        if (!file.type.match(imageType)) {
            continue;
        }
        var img = document.getElementById("thumbnil");
        img.file = file;
        var reader = new FileReader();
        reader.onload = (function (aImg) {
            return function (e) {
                aImg.src = e.target.result;
            };
        })(img);
        reader.readAsDataURL(file);
    }
}

/* Start Jquery page change password */
$("#frmChangePass").submit(function () {
    $('.text-success').css("display", "none");
})
/* Start Jquery page check out */
function confirmPayment() {
    $("#frmPaymentSubmit").submit();
}

/*Start Jquery page payment*/
function checkBillingAddress() {
    if ($("#ckbillingaddress").is(':checked')) {
        $(".address-billing").show();
    }
    else {
        $(".address-billing").hide();
    }
}
/*Start choose shipping address*/
$(function () {
    $('div.product-chooser').not('.disabled').find('div.product-chooser-item').on('click', function () {
        $(this).parent().parent().find('div.product-chooser-item').removeClass('selected');
        $(this).addClass('selected');
        $(this).find('input[type="radio"]').prop("checked", true);
    });
});