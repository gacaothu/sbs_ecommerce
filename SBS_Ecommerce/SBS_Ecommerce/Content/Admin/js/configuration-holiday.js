$(function () {
    $(".datetimepickerholiday").datetimepicker({
        autoclose: true,
        componentIcon: '.s7-date',
        navIcons: {
            rightIcon: 's7-angle-right',
            leftIcon: 's7-angle-left'
        }
    });
    $('#tblconfiguration-holiday').dataTable();
});

function addHoliday() {
    $.ajax({
        type: 'GET',
        url: UrlContent("Admin/AddHoliday"),
        success: function (rs) {
            $('.modal-content-holiday').html(rs);
            showModal();
        }
    });
}
function editHoliday(id) {
    $.ajax({
        type: 'GET',
        url: UrlContent("Admin/EditHoliday/"+id),
        success: function (rs) {
            $('.modal-content-holiday').html(rs);
            showModal();
        }
    });
}

function showModal() {
    $('#form-add-edit-holiday').modal('show');
}
