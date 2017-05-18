var url = window.location.href;
$(function () {
    if (sessionStorage.reloadAfterLoadPage) {
        var msgCallback = localStorage.getItem("callback");
        if (msgCallback) {
            var html = '<div role="alert" class="alert alert-success alert-dismissible">'
                + '<button type="button" data-dismiss="alert" aria-label="Close" class="close"><span aria-hidden="true" class="s7-close"></span></button>'
                + '<span id="alert-msg-success">' + msgCallback + '</span></div>';
            $('.panel-body').prepend(html);
            sessionStorage.reloadAfterLoadPage = false;
            localStorage.removeItem("callback");
        }
    }    
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
                sessionStorage.reloadAfterLoadPage = true;
                if (id) 
                    localStorage.setItem("callback", "Delivery company has been updated successfully.");
                else
                    localStorage.setItem("callback", "Delivery company has been created successfully.");
                window.location.href = url;
            } else if (rs.Status == -1) {
                $('#errMsg').removeAttr('hidden');
                $('#errMsg').empty();
                $('#errMsg').text(rs.Message);
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
                sessionStorage.reloadAfterLoadPage = true;
                localStorage.setItem("callback", "Delivery company has been deleted successfully.");
                window.location.href = url;
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