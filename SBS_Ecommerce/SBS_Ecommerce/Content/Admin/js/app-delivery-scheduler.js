var url = window.location.href;
$(function () {
    checkAlertMessageDisplay('.main-content');
    $('#delivery-scheduler-table').dataTable();
    $(".datetimepicker").datetimepicker({
        autoclose: true,
        showDate: false,
        dateFormat: '',
        timeFormat: 'HH:mm tt',
        componentIcon: '.s7-date',
        navIcons: {
            rightIcon: 's7-angle-right',
            leftIcon: 's7-angle-left'
        }
    });
});

function showModal() {
    clearControls(['txtFromHour', 'txtToHour', 'txtRate']);
    clearAllErrors(['txtFromHour', 'txtToHour', 'txtRate']);
    $('#form-add-edit-scheduler').modal('show');
}

$(document).on('click', '#add-update-scheduler-btn', function () {
    var id = $(this).data('id');
    var check = true;
    check = validateControls();

    if (!check) {
        return;
    }
    var data = {
        timeSlot: $('#txtDisplayText').val(),
        fromHour: $('#txtFromHour').val(),
        toHour: $('#txtToHour').val(),
        rate: $('#txtRate').val(),
        perSlot: $('#txtPerSlot').val(),
        isWeekday: $('#chkWeekend').is(':checked'),
        isWeekend: $('#chkWeekend').is(':checked'),
        isHoliday: $('#chkHoliday').is(':checked'),
        isActive: $('#chkActive').is(':checked'),
        createdAt: $('#txtCreatedAt').val()
    }
    if (id) {
        data['id'] = id
    }
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/InsertOrUpdateDeliveryScheduler"),
        data: data,
        success: function (rs) {
            showAlertFromResponse(rs);
        }
    });
});
function showConfirm(id) {
    $('#confirm-delete').attr('data-id', id);
    $('#confirm-delete').modal('show');
}

function openEdit(id) {
    $.ajax({
        url: UrlContent("Admin/GetDeliveryScheduler"),
        data: {
            id: id
        },
        success: function (rs) {
            if (rs.Status == 0) {
                $('#form-add-edit-scheduler').empty();
                $('#form-add-edit-scheduler').append(rs.Html);
                $('#form-add-edit-scheduler').modal('show');
            } else {
                showAlertFromResponse(rs);
            }
        }
    });
}

function deleteDeliveryScheduler(id) {
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/DeleteDeliveryScheduler"),
        data: {
            id: id
        },
        success: function (rs) {
            $('#confirm-delete').modal('toggle');
            showAlertFromResponse(rs);
        }
    });
}


function validateControls() {
    var check = true;
    if (!validateRequired('txtFromHour', 'From Hour', true)) {
        check = false;
    }
    if (!validateComparision('txtToHour', 'To Hour', 'txtFromHour', 'From Hour', true)) {
        check = false;
    }
    if (!validateRequired('txtRate', 'Rate', true)) {
        check = false;
    }
    return check;
}