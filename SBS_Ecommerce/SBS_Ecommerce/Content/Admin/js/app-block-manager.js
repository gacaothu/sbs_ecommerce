var url = window.location.href;
$(function () {
    if (sessionStorage.reloadAfterLoadPage) {
        var msgCallback = localStorage.getItem("callback");
        if (msgCallback) {
            var html = getAlertHTML(msgCallback);
            $('.main-content').prepend(html);
        }
        sessionStorage.reloadAfterLoadPage = false;
        localStorage.removeItem("callback");
    }
    $("table").dataTable();
    CKEDITOR.replace('txtHTML');
});
$('#successModal').on('click', function () {
    window.location.reload();
});

function ConfirmAddBlock() {
    $('#addBlockModal').find('h3').text('Add Block');
    $('#addBlockModal').find('#txtTitleHTML').val('');
    $('#errorTitle').hide();

    CKEDITOR.instances['txtHTML'].setData('');
    $('#addBlockModal').attr('data-action', 'Add');
    $('#addBlockModal').modal('show');
}

function ConfirmEditBlock(id) {
    $('#addBlockModal').find('h3').text('Edit Block');
    $('#addBlockModal').attr('data-action', 'Edit');
    $('#addBlockModal').attr('data-id', id);
    $('#errorTitle').hide();

    $.ajax({
        //url: '@Url.Action("GetContentBlock", "Admin")',
        url: UrlContent("Admin/GetContentBlock"),
        data: { id: id },
        type: 'POST',
        success: function (rs) {
            CKEDITOR.instances['txtHTML'].setData(rs.Content);
            $('#addBlockModal').find('#txtTitleHTML').val(rs.Title);
            $('#addBlockModal').modal('show');
        }
    });
}

function SaveBlock() {
    var content = CKEDITOR.instances['txtHTML'].getData();
    if ($('#txtTitleHTML').val() == '') {
        $('#errorTitle').show();
        return;
    }
    else {
        $('#errorTitle').hide();
    }

    if ($('#addBlockModal').attr('data-action') == 'Add') {
        $.ajax({
            //url: '@Url.Action("AddBlock", "Admin")',
            url: UrlContent("Admin/AddBlock"),
            data: { content: content, title: $('#txtTitleHTML').val() },
            type: 'POST',
            success: function (rs) {
                $('#addBlockModal').modal('hide');
                sessionStorage.reloadAfterLoadPage = true;
                localStorage.setItem("callback", "Block has been created successfully.");
                window.location.href = url;
                //showMessageAlert('1', 'Block has been created successfully.');
            }
        });
    }
    else {
        $.ajax({
            //url: '@Url.Action("EditBlock", "Admin")',
            url: UrlContent("Admin/EditBlock"),
            data: { id: $('#addBlockModal').attr('data-id'), content: content, title: $('#txtTitleHTML').val() },
            type: 'POST',
            success: function (rs) {
                $('#addBlockModal').modal('hide');
                sessionStorage.reloadAfterLoadPage = true;
                localStorage.setItem("callback", "Block has been updated successfully.");
                window.location.href = url;
                //showMessageAlert('1', 'Block has been updated successfully.');
            }
        });
    }
}

function ConfirmDelete(id) {
    $('#confirm-delete').attr('data-id', id);
    $('#confirm-delete').modal('show');
}

function DeleteBlock(id) {
    $.ajax({
        //url: '@Url.Action("DeleteBlock", "Admin")',
        url: UrlContent("Admin/DeleteBlock"),
        data: { id: id },
        type: 'POST',
        success: function (rs) {
            $('#confirm-delete').modal('hide');
            sessionStorage.reloadAfterLoadPage = true;
            localStorage.setItem("callback", "Block has been deleted successfully.");
            window.location.href = url;
            //showMessageAlert('1', 'Block has been deleted successfully.');
        }
    });
}

function Preview(id) {
    $.ajax({
        //url: '@Url.Action("GetContentBlock", "Admin")',
        url: UrlContent("Admin/GetContentBlock"),
        data: { id: id },
        type: 'POST',
        success: function (rs) {
            $('.content-block').html(rs.Content);
            $('#preBlockModal').modal('show');
        }
    });
}