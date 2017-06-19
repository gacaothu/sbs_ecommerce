var url = window.location.href;
$(function () {
    if (sessionStorage.reloadAfterLoadPage) {
        var tab = localStorage.getItem('tab');
        var msgCallback = localStorage.getItem("callback");
        var status = localStorage.getItem("status");
        if (tab && tab == 'local-pickup') {
            if (msgCallback) {
                var html = getAlertHTML(msgCallback, status);
                //fix to standard pattern by sun 
                $('.main-content').prepend(html);

                $("#li-weight-based").removeClass("active");
                $("#weight-based").removeClass("active");

                $("#li-local-pickup").addClass("active");
                $("#local-pickup").addClass("active in");
            }
        } else{
            if (msgCallback) {
                var html = getAlertHTML(msgCallback, status);
                $('.main-content').prepend(html);
            }
        }        
        localStorage.removeItem("callback");
        localStorage.removeItem('tab');
    }
    $("#table1").dataTable();
});
$(document).on('click', '#add-update-btn', function () {
    var check = true;
    check = validateControls();

    if (!check) {
        return false;
    }
    var data = {
        Min: $('#txtMin').val(),
        Max: $('#txtMax').val(),
        Rate: $('#txtRate').val(),
        UnitOfMass: $('#slUnit').val(),
        DeliveryCompany: $('#slDelivery').val(),
        Country: $('#slCountry').val()
    }
    var type = $(this).data('type');
    if (type != 'add') {
        data['Id'] = parseInt($(this).data('id'))
    }
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/InsertOrUpdateWeightBased"),
        data: data,
        success: function (rs) {
            showAlertFromResponse(rs);
        }
    });
});

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
            localStorage.setItem('tab', 'local-pickup');
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
            localStorage.setItem('tab', 'local-pickup');
            showAlertFromResponse(rs);
        }
    });
});

function duplicateItem(id) {
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/DuplicateWeightBase"),
        data: { id: id },
        success: function (rs) {
            showAlertFromResponse(rs);
        }
    });
}

function showConfirmDuplicate(id) {
    $('#confirm-duplicate').attr('data-id', id);
    $('#confirm-duplicate').modal('show');
}

function showConfirmDelete(id) {
    $('#confirm-delete').attr('data-id', id);
    $('#confirm-delete').modal('show');
}

function deleteItem(id) {
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/DeleteWeightBased"),
        data: { id: id },
        success: function (rs) {
            showAlertFromResponse(rs);
        }
    });
}

function openEdit(id) {
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/GetWeightBased"),
        data: { id: id },
        success: function (rs) {
            $('#form-add-edit').empty();
            $('#form-add-edit').append(rs.Partial);
            $('#form-add-edit').modal('show');
        }
    });
}

function showModal() {
    clearControls(['txtMin', 'txtMax', 'txtRate', 'slUnit', 'slDelivery', 'slCountry']);
    clearAllErrors(['txtMin', 'txtMax', 'txtRate', 'slUnit', 'slDelivery', 'slCountry']);
    $('#form-add-edit').modal('show');
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

function validateControls() {
    var check = true;
    if (!validateNum('txtMin', 'Min', true)) {
        check = false;
    }
    if (!validateNumComparision('txtMax', 'Max', 'txtMin', 'Min', true)) {
        check = false;
    }
    if (!validateNum('txtRate', 'Rate', true)) {
        check = false;
    }
    if (!validateRequired('slUnit', 'Unit', true)) {
        check = false;
    }
    if (!validateRequired('slDelivery', 'Delivery Company', true)) {
        check = false;
    }
    if (!validateRequired('slCountry', 'Country', true)) {
        check = false;
    }
    return check;
}