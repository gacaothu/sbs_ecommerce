var url = window.location.href;
$(function () {
    checkAlertMessageDisplay('.main-content');
    $('#delivery-company-table').dataTable();
});

$(document).on('click', '#add-edit-btn', function () {
    var id = $(this).data('id');
    var check = true;

    check = validateControls();
    if (!check) {
        return false;
    }
    var data = {
        CompanyName: $('#txtName').val(),
        Address: $('#txtAddress').val(),
        Ward: $('#txtWard').val(),
        District: $('#txtDistrict').val(),
        City: $('#txtCity').val(),
        Country: $('#slCountry').val(),
        Phone: $('#txtPhone').val(),
        Email: $('#txtEmail').val()
    };
    if (id) {
        data['Id'] = id;
    }
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/InsertOrUpdateDeliveryCompany"),
        data: data,
        success: function (rs) {
            showAlertFromResponse(rs);
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
                $('#form-delivery-company').append(rs.Html);
                $('#form-delivery-company').modal('show');
            } else if (rs.Status == -1) {
                showAlertFromResponse(rs);
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
            showAlertFromResponse(rs);
        }
    });
}

function showModal() {
    clearControls(['txtName', 'txtAddress', 'txtWard', 'txtDistrict', 'txtCity', 'txtEmail', 'slCountry']);
    clearAllErrors(['txtName', 'txtAddress', 'slCountry']);
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
    if (!validateRequired('slCountry', 'Country', true)) {
        check = false;
    }
    return check;
}