$(function () {
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
    clearControls();
    clearAllErrors();
    $('#form-add-edit-scheduler').modal('show');
}

$(document).on('click', '#add-update-scheduler-btn', function () {
    var id = $(this).data('id');
    var DisplayText = $('#txtDisplayText').val();
    var fromHour = $('#txtFromHour').val();
    var toHour = $('#txtToHour').val();
    var rate = $('#txtRate').val();
    var PerSlot = $('#txtPerSlot').val();
    var IsWeekday = $('#chkWeekend').is(':checked');
    var IsWeekend = $('#chkWeekend').is(':checked');
    var IsHoliday = $('#chkHoliday').is(':checked');
    var IsActive = $('#chkActive').is(':checked');
    var check = true;

    clearAllErrors();
    check = validateControls();

    if (!check) {
        return;
    }
    var data = {
        timeSlot: DisplayText,
        fromHour: fromHour,
        toHour: toHour,
        rate: rate,
        perSlot: PerSlot,
        isWeekday: IsWeekday,
        isWeekend: IsWeekend,
        isHoliday: IsHoliday,
        isActive: IsActive,
    }
    if (id) {
        data['id'] = id
    }
    $.ajax({
        type: 'POST',
        url: UrlContent("Admin/InsertOrUpdateDeliveryScheduler"),
        data: data,
        success: function (rs) {
            if (rs.Status == 0) {
                window.location.reload();
            } else if (rs.Status == -1) {
                //fix to standard pattern by sun 
                $('#alert-err-msg').removeAttr('hidden');
                $('#alert-err-msg').empty();
                $('#alert-err-msg').append(rs.Message);
            }
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
                $('#form-add-edit-scheduler').append(rs.Partial);
                $('#form-add-edit-scheduler').modal('show');
            } else if (rs.Status == -1) {
                $.gritter.add({
                    title: 'Notification',
                    text: rs.Message,
                    class_name: 'color danger'
                });
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
            if (rs.Status == 0) {
                window.location.reload();
            } else if (rs.Status == -1) {
                $.gritter.add({
                    title: 'Notification',
                    text: rs.Message,
                    class_name: 'color danger'
                });
            }
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

function clearControls() {
    $('#txtFromHour').val('');
    $('#txtToHour').val('');
    $('#txtRate').val('');
}

function clearAllErrors() {
    clearError('#txtFromHour');
    clearError('#txtToHour');
    clearError('#txtRate');
}