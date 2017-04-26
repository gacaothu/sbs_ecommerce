$('#add-btn').click(function () {
    var name = $('#txtName').val();
    var address = $('#txtAddress').val();
    var ward = $('#txtWard').val();
    var district = $('#txtDistrict').val();
    var city = $('#txtCity').val();
    var phone = $('#txtPhone').val();
    var email = $('#txtEmail').val();
    var country = $('#slCountry').val();
    var check = true;
    var errMsg = "";
    if (!name) {
        errMsg = ' - Company Name is required \n';
        check = false;
    }
    if (!address) {
        errMsg += ' - Address is required \n';
        check = false;
    }
    if (!ward) {
        errMsg += ' - Ward is required \n';
        check = false;
    }
    if (!district) {
        errMsg += ' - District is required \n';
        check = false;
    }
    if (!city) {
        errMsg += ' - City is required \n';
        check = false;
    }
    if (!phone) {
        errMsg += ' - Phone is required \n';
        check = false;
    }
    if (!email) {
        errMsg += ' - Email is required \n';
        check = false;
    }
    if (!validateEmail(email)) {
        errMsg += ' - Email is not valid \n';
        check = false;
    }
    if (country.indexOf('--') >= 0) {
        errMsg += ' - Country is required \n';
        check = false;
    }

    if (!check) {
        $('#errMsg').text(errMsg);
        return;
    }

    $('#errMsg').text('');
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/InsertDeliveryCompany"),
        data: {
            Name:name,
            Address: address,
            Ward: ward,
            District: district,
            City: city,
            Country: country,
            Phone: phone,
            Email: email
        }, 
        success: function (rs) {
            if (rs.Status == 0) {
                window.location.reload();
            } else if (rs.Status == -1) {
                $('#errMsg').text(rs.Message);
            }
        }
    })
});

$('#delete-btn').click(function () {
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/DeleteDeliveryCompany"),
        data: { id: $('#delivery-company-id').val() },
        success: function (rs) {
            if (rs.Status == 0) {
                window.location.reload();
            }            
        }
    })
});

function showConfirm(e) {
    $('.modal-body #delivery-company-id').val($(e).data('id'));
}

function validateEmail(s) {
    var regex = /^\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b$/i
    return regex.test(s);
}