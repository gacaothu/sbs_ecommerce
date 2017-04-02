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


function RemoveAvartar() {
    if (confirm('Are you sure?')) {
       
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/Account/RemoveAvatar',
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
/* End Jquery page change password */