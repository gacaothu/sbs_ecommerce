$(document).on('click', '#add-update-btn', function(){
    var check = true;
    var errMsg = '';

    var min = $('#txtMin').val();
    var max = $('#txtMax').val();
    var rate = $('#txtRate').val();
    var deliveryCompany = $('#delivery-company').val();
    var country = $('#country').val();

    if (!min) {
        check = false;
        errMsg += ' - Min value is required \n';
    }
    if (min && !checkFloatNumber(min)) {
        check = false;
        errMsg += ' - Min value is not valid \n';
    }
    if (!max) {
        check = false;
        errMsg += ' - Max value is required \n';
    }
    if (max && !checkFloatNumber(max)) {
        check = false;
        errMsg += ' - Min value is not valid \n';
    }
    if (!rate) {
        check = false;
        errMsg += ' - Rate value is required \n';
    }
    if (rate && !checkFloatNumber(rate)) {
        check = false;
        errMsg += ' - Rate value is not valid \n';
    }
    if (deliveryCompany.indexOf('--') >= 0) {
        check = false;
        errMsg += ' - Delivery Company is required \n';
    }
    if (country.indexOf('--') >=0 ) {
        check = false;
        errMsg += ' - Country is required \n';
    }

    if (!check) {
        $('#errMsg').text(errMsg);
        return;
    }
    $('#errMsg').text(errMsg);
    var data = {
        Min: min,
        Max: max,
        Rate: rate,
        DeliveryCompany: deliveryCompany,
        Country: country
    }
    var type = $(this).data('type');
    var url = type == 'add' ? UrlContent("Admin/CreateWeightBased") : UrlContent("Admin/UpdateWeightBased");
    if (type != 'add') {
        data['Id'] = parseInt($(this).data('id'))
    }
    $.ajax({
        type: 'POST',
        url: url,
        data: data,
        success: function (rs) {
            if (rs.Status == 0) {
                window.location.reload();
            }
            if (rs.Status == -1) {
                $('#errMsg').text(rs.Message);
            }
        }
    });
});

$('#enable-weight-chk').click(function () {
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/UpdateWeighBasedConfiguration"),
        success: function (rs) {
            console.log(rs.Message);
        }
    });
});

$('#enable-local-chk').click(function () {
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/UpdateLocalPickupConfiguration"),
        success: function (rs) {
            console.log(rs.Message);
        }
    });
});

function duplicateItem(e) {
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/DuplicateWeightBase"),
        data: { id: $(e).data('id') },
        success: function (rs) {
            if (rs.Status == 0) {
                window.location.reload();
            }
        }
    });
}

function openEdit(e) {
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/GetWeightBased"),
        data: { id: $(e).data('id') },
        success: function (rs) {
            $('#form-add-edit').empty();
            $('#form-add-edit').append(rs.Partial);
            $('#form-add-edit').modal('show');
        }
    });
}

function checkFloatNumber(n) {
    var regex = /^[+-]?\d+(\.\d+)?$/i;
    return regex.test(n);
}