var url = window.location.href;
$(function () {
    checkAlertMessageDisplay('.main-content');
    $('#delivery-company-table').dataTable();
});

$(document).on('click', '#add-edit-btn', function () {
    var id = $(this).data('id');
    var name = $('#txtName').val();
    var address = $('#txtAddress').val();
    var ward = $('#txtWard').val();
    var district = $('#txtDistrict').val();
    var city = $('#txtCity').val();
    var phone = $('#txtPhone').val();
    var email = $('#txtEmail').val();
    var country = $('#slCountry').val();
    var check = true;

    clearAllErrors();
    check = validateControls();
    if (!check) {
        return false;
    }
    var data = {
        CompanyName: name,
        Address: address,
        Ward: ward,
        District: district,
        City: city,
        Country: country,
        Phone: phone,
        Email: email
    };
    if (id) {
        data['Id'] = id;
    }
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/InsertOrUpdateDeliveryCompany"),
        data: data,
        success: function (rs) {
            if (rs.Status == 0) {
                if (id)
                    showAlertMessageAndReload("Delivery company has been updated successfully.", url);
                else
                    showAlertMessageAndReload("Delivery company has been created successfully.", url);
            } else if (rs.Status == -1) {
                //fix to standard pattern by sun 
                $('#alert-err-msg').removeAttr('hidden');
                $('#alert-err-msg').empty();
                $('#alert-err-msg').text(rs.Message);
            }
        }
    })
});

function showConfirm(id) {
    $('#confirm-delete').attr('data-id', id);
    $('#confirm-delete').modal('show');
}

function openEdit(id) {
    $.ajax({
        url: UrlContent("Admin/GetDeliveryCompany"),
        data: { id: id },
        success: function (rs) {
            if (rs.Status == 0) {
                $('#form-delivery-company').empty();
                $('#form-delivery-company').append(rs.Partial);
                $('#form-delivery-company').modal('show');
            } else if (rs.Status == -1) {
                showNot(rs.Message);
            }
        }
    });
}

function deleteDeliveryCompany(id) {
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/DeleteDeliveryCompany"),
        data: { id: id },
        success: function (rs) {
            if (rs.Status == 0) {
                showAlertMessageAndReload("Delivery company has been deleted successfully.", url);
            } else if (rs.Status == -1) {
                showNot(rs.Message);
            }
        }
    });
}

function showNot(msg) {
    $.gritter.add({
        title: 'Notification',
        text: msg,
        class_name: 'color danger'
    });
}

function showModal() {
    clearControls();
    clearAllErrors();
    $('#form-delivery-company').modal('show');
}

function validateControls() {
    var check = true;
    if (!validateRequired('txtName', 'Company Name', true)) {
        check = false;
    }
    if (!validateRequired('txtAddress', 'Address', true)) {
        check = false;
    }
    if (!validateRequired('txtAddress', 'Address', true)) {
        check = false;
    }
    if (!validateEmail('txtEmail', 'Email', false)) {
        check = false;
    }
    if (!validateSelectRequired('slCountry', 'Country', true)) {
        check = false;
    }
    return check;
}

function clearControls() {
    $('#txtName').val('');
    $('#txtAddress').val('');
    $('#txtWard').val('');
    $('#txtDistrict').val('');
    $('#txtCity').val('');
    $('#txtEmail').val('');
    $('#slCountry').val('--');
}
function clearAllErrors() {
    clearError('txtName');
    clearError('txtAddress');
    clearError('txtWard');
    clearError('txtDistrict');
    clearError('txtCity');
    clearError('txtEmail');
    clearError('slCountry');
}