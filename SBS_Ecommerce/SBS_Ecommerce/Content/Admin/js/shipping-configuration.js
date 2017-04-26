$('#add-item-btn').click(function () {
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
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/CreateWeightBased"),
        data: {
            Min: min,
            Max: max,
            Rate: rate,
            DeliveryCompany: deliveryCompany,
            Country: country
        },
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

function checkFloatNumber(n) {
    var regex = /^[+-]?\d+(\.\d+)?$/i;
    return regex.test(n);
}