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
            appendHtml('#form-add-edit-scheduler', rs);
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
            appendHtml('#form-add-edit-scheduler', rs);
        }
    });
}

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