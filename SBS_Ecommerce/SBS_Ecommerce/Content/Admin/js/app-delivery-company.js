$(function () {
    $('#delivery-company-table').dataTable();
});

function addDeliveryCompany() {
    $.ajax({
        type: 'GET',
        url: UrlContent("Admin/AddDeliveryCompany"),
        success: function (rs) {
            appendHtml('#form-delivery-company', rs);
        }
    });
}

function openEdit(id) {
    $.ajax({
        url: UrlContent("Admin/EditDeliveryCompany"),
        data: { id: id },
        success: function (rs) {
            appendHtml('#form-delivery-company', rs);
        }
    });
}

function showConfirm(id) {
    $('#idCompany').val(id);
    $('#confirm-delete').modal('show');
}