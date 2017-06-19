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

    $('#IsWeekday').bind('change', function () {
        customCheckboxValue(this);
    });
    $('#IsWeekend').bind('change', function () {
        customCheckboxValue(this);
    });
    $('#IsWeekday').bind('change', function () {
        customCheckboxValue(this);
    });
    $('#IsHoliday').bind('change', function () {
        customCheckboxValue(this);
    });
    $('#IsActive').bind('change', function () {
        customCheckboxValue(this);
    });
});

function customCheckboxValue(e) {
    if (e.checked) {
        $(e).attr('value', 'true');
    }
    else {
        $(e).attr('value', 'false');
    }
}

function addDeliveryScheduler() {
    $.ajax({
        type: 'GET',
        url: UrlContent("Admin/AddDeliveryScheduler"),
        success: function (rs) {
            appendHtml(rs);
        }
    });
}

function getDeliveryScheduler(id) {
    $.ajax({
        url: UrlContent("Admin/GetDeliveryScheduler"),
        data: {
            id: id
        },
        success: function (rs) {
            appendHtml(rs);
        }
    });
}

function appendHtml(rs) {
    $('#form-add-edit-scheduler').empty();
    $('#form-add-edit-scheduler').append(rs);
    $('#form-add-edit-scheduler').modal('show');
}

//function showModal() {
//    clearControls(['txtFromHour', 'txtToHour', 'txtRate']);
//    clearAllErrors(['txtFromHour', 'txtToHour', 'txtRate']);
//    $('#form-add-edit-scheduler').modal('show');
//}

//$(document).on('click', '#add-update-scheduler-btn', function () {
//    var id = $(this).data('id');
//    var check = true;
//    check = validateControls();

//    if (!check) {
//        return;
//    }
//    var data = {
//        timeSlot: $('#txtDisplayText').val(),
//        fromHour: $('#txtFromHour').val(),o
//        toHour: $('#txtToHour').val(),
//        rate: $('#txtRate').val(),
//        perSlot: $('#txtPerSlot').val(),
//        isWeekday: $('#chkWeekend').is(':checked'),
//        isWeekend: $('#chkWeekend').is(':checked'),
//        isHoliday: $('#chkHoliday').is(':checked'),
//        isActive: $('#chkActive').is(':checked'),
//        createdAt: $('#txtCreatedAt').val()
//    }
//    if (id) {
//        data['id'] = id
//    }
//    $.ajax({
//        type: 'POST',
//        url: UrlContent("Admin/InsertOrUpdateDeliveryScheduler"),
//        data: data,
//        success: function (rs) {
//            showAlertFromResponse(rs);
//        }
//    });
//});

function showConfirm(id) {
    $('#idDeliveryScheduler').val(id);
    $('#confirm-delete').modal('show');
}

function openEdit(id) {
    $.ajax({
        url: UrlContent("Admin/EditDeliveryScheduler"),
        data: {
            id: id
        },
        success: function (rs) {
            $('#form-add-edit-scheduler').empty();
            $('#form-add-edit-scheduler').append(rs);
            $('#form-add-edit-scheduler').modal('show');
        }
    });
}