var url = window.location.href;
$(function () {
    if (sessionStorage.reloadAfterLoadPage) {
        var tab = localStorage.getItem('tab');
        var msgCallback = localStorage.getItem("callback");
        if (tab && tab == 'local-pickup') {
            if (msgCallback) {
                var html = '<div role="alert" class="alert alert-success alert-dismissible">'
                    + '<button type="button" data-dismiss="alert" aria-label="Close" class="close"><span aria-hidden="true" class="s7-close"></span></button>'
                    + '<span id="alert-msg-success">' + msgCallback + '</span></div>';
                $('#local-pickup').prepend(html);
                $('#local-pickup').addClass('active');
                $('#weight-based').removeClass('active');
                $('#main-tab li').first().removeClass('active');
                $('#main-tab li').last().addClass('active');
            }
        } else{
            if (msgCallback) {
                var html = '<div role="alert" class="alert alert-success alert-dismissible">'
                    + '<button type="button" data-dismiss="alert" aria-label="Close" class="close"><span aria-hidden="true" class="s7-close"></span></button>'
                    + '<span id="alert-msg-success">' + msgCallback + '</span></div>';
                $('#weight-based').prepend(html);
                //$('#weight-based').addClass('active');
                //$('#local-pickup').removeClass('active');
                //$('#main-tab li').first().addClass('active');
                //$('#main-tab li').last().removeClass('active');
            }
        }
        
        sessionStorage.reloadAfterLoadPage = false;
        localStorage.removeItem("callback");
        localStorage.removeItem('tab');
    }
    $("#table1").dataTable();
});
$(document).on('click', '#add-update-btn', function () {
    var check = true;
    var errMsg = '';

    var min = $('#txtMin').val();
    var max = $('#txtMax').val();
    var rate = $('#txtRate').val();
    var unit = $('#slUnit').val();
    var deliveryCompany = $('#slDelivery').val();
    var country = $('#slCountry').val();

    clearAllErrors();
    check = validateControls();

    if (!check) {
        return false;
    }
    var data = {
        Min: min,
        Max: max,
        Rate: rate,
        UnitOfMass: unit,
        DeliveryCompany: deliveryCompany,
        Country: country
    }
    var type = $(this).data('type');
    var controller = type == 'add' ? UrlContent("Admin/CreateWeightBased") : UrlContent("Admin/UpdateWeightBased");
    if (type != 'add') {
        data['Id'] = parseInt($(this).data('id'))
    }
    $.ajax({
        type: 'POST',
        url: controller,
        data: data,
        success: function (rs) {
            if (rs.Status == 0) {
                sessionStorage.reloadAfterLoadPage = true;
                //localStorage.setItem('tab', 'weight-based');
                if(type == 'add')
                    localStorage.setItem("callback", "Weight Base Fee has been created successfully.");
                else
                    localStorage.setItem("callback", "Weight Base Fee has been updated successfully.");
                window.location.href = url;
            }
            if (rs.Status == -1) {
                $('#errMsg').text(rs.Message);
            }
        }
    });
});

$(document).on('click', '#enable-weight-chk', function () { 
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/UpdateWeighBasedConfiguration"),
        success: function (rs) {
            if (rs.Message == "Success") {
                sessionStorage.reloadAfterLoadPage = true;
                localStorage.setItem("callback", "Shipping Fee setting has been updated successfully.");
                window.location.href = url;
            }
        }
    });
});

$(document).on('click', '#enable-local-chk', function () { 
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/UpdateLocalPickupConfiguration"),
        success: function (rs) {
            sessionStorage.reloadAfterLoadPage = true;
            localStorage.setItem('tab', 'local-pickup');
            localStorage.setItem("callback", "Local pickup setting has been updated successfully.");
            window.location.href = url;
        }
    });
});

$(document).on('click', '#pick-save-btn', function () {
    var id = $('#pick-txtId').val();
    var phone = $('#pick-txtPhone').val();
    var address = $('#pick-txtAddress').val();
    var ward = $('#pick-txtWard').val();
    var district = $('#pick-txtDistrict').val();
    var city = $('#pick-txtCity').val();
    var country = $('#pick-country').val();

    var check = true;

    clearPickupErrors();
    check = validatePickup();

    if (!check) {
        return false;
    }

    var data = {
        Phone: phone,
        Address: address,
        Ward: ward,
        District: district,
        City: city,
        Country: country
    }
    if (id) {
        data['Id'] = id;
    }
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/UpdateLocalPickupInfo"),
        data: data,
        success: function (rs) {
            if (rs.Status == 0) {
                sessionStorage.reloadAfterLoadPage = true;
                localStorage.setItem('tab', 'local-pickup');
                localStorage.setItem("callback", "Local pickup fee has been saved successfully.");
                window.location.href = url;
                //showNotification('Update local pickup information successful.', 0);
            }
            if (rs.Status == -1) {
                sessionStorage.reloadAfterLoadPage = true;
                localStorage.setItem('tab', 'local-pickup');
                localStorage.setItem("callback", "Local pickup fee has been saved successfully.");
                window.location.href = url;
                showNotification(rs.Message, -1);
            }
        }
    });
});

function showNotification(s, t) {
    var classname = 'color success';
    if (t != 0) {
        classname = 'color danger';
    }
    $.gritter.add({
        title: 'Notification',
        text: s,
        class_name: classname
    });
    return false;
}

function duplicateItem(id) {
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/DuplicateWeightBase"),
        data: { id: id },
        success: function (rs) {
            if (rs.Status == 0) {
                sessionStorage.reloadAfterLoadPage = true;
                //localStorage.setItem('tab', 'weight-based');
                localStorage.setItem("callback", "Weight Base Fee has been duplicated successfully.");
                window.location.href = url;
            } else {
                showNotification(rs.Message, -1);
            }
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
            if (rs.Status == 0) {
                sessionStorage.reloadAfterLoadPage = true;
                //localStorage.setItem('tab', 'weight-based');
                localStorage.setItem("callback", "Weight Base Fee has been deleted successfully.");
                window.location.href = url;
            } else {
                showNotification(rs.Message, -1);
            }
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
    clearControls();
    clearAllErrors();
    $('#form-add-edit').modal('show');
}

function clearPickupErrors() {
    clearError('pick-txtPhone');
    clearError('pick-txtAddress');
    clearError('pick-txtWard');
    clearError('pick-txtDistrict');
    clearError('pick-txtPhone');
    clearError('pick-country');
}

function validatePickup() {
    var check = true;
    if (!validateRequired('pick-txtPhone', 'Phone', true)) {
        check = false;
    }
    if (!validateRequired('pick-txtAddress', 'Phone', true)) {
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
    if (!validateSelectRequired('slUnit', 'Unit', true)) {
        check = false;
    }
    if (!validateSelectRequired('slDelivery', 'Delivery Company', true)) {
        check = false;
    }
    if (!validateSelectRequired('slCountry', 'Country', true)) {
        check = false;
    }
    return check;
}

function clearControls() {
    $('#txtMin').val('');
    $('#txtMax').val('');
    $('#txtRate').val('');
    $('#slUnit').val('--');
    $('#slDelivery').val('--');
    $('#slCountry').val('--');
}

function clearAllErrors() {
    clearError('txtMin');
    clearError('txtMax');
    clearError('txtRate');
    clearError('slUnit');
    clearError('slDelivery');
    clearError('slCountry');
}