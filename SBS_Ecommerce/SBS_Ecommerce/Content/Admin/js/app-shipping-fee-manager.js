var url = window.location.href;
$(function () {
    checkAlertMessageDisplay('.main-content');
    $("#table1").dataTable();
});

function addWeightBase() {
    $.ajax({
        type: 'GET',
        url: UrlContent("Admin/AddWeightBased"),
        success: function (rs) {
            appendHtml('#form-add-edit', rs);
        }
    });
}

function openEdit(id) {
    $.ajax({
        type: 'GET',
        url: UrlContent("Admin/EditWeightBased"),
        data: { id: id },
        success: function (rs) {
            appendHtml('#form-add-edit', rs);
        }
    });
}

$(document).on('click', '#enable-weight-chk', function () { 
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/UpdateWeighBasedConfiguration"),
        success: function (rs) {
            showAlertFromResponse(rs);
        }
    });
});

$(document).on('click', '#enable-local-chk', function () { 
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/UpdateLocalPickupConfiguration"),
        success: function (rs) {
            showAlertFromResponse(rs);
        }
    });
});

$(document).on('click', '#pick-save-btn', function () {
    var id = $('#pick-txtId').val();
    var check = true;
    check = validatePickup();

    if (!check) {
        return false;
    }
    var data = {
        Phone: $('#pick-txtPhone').val(),
        Address: $('#pick-txtAddress').val(),
        Ward: $('#pick-txtWard').val(),
        District: $('#pick-txtDistrict').val(),
        City: $('#pick-txtCity').val(),
        Country: $('#pick-country').val()
    }
    if (id) {
        data['Id'] = id;
    }
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/UpdateLocalPickupInfo"),
        data: data,
        success: function (rs) {
            showAlertFromResponse(rs);
        }
    });
});

function showConfirmDuplicate(id) {
    $('#idDuplicate').val(id);
    $('#confirm-duplicate').modal('show');
}

function showConfirmDelete(id) {
    $('#idDelete').val(id);
    $('#confirm-delete').modal('show');
}

function validatePickup() {
    var check = true;
    if (!validateRequired('pick-txtPhone', 'Phone', true)) {
        check = false;
    }
    if (!validateRequired('pick-txtAddress', 'Address', true)) {
        check = false;
    }   
    if (!validateSelectRequired('pick-country', 'Country', true)) {
        check = false;
    }
    return check;
}
