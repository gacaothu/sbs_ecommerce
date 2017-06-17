var url = window.location.href;
$(function () {
    checkAlertMessageDisplay('.main-content');
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
            url: UrlContent("Admin/AddBlock"),
            data: { content: content, title: $('#txtTitleHTML').val() },
            type: 'POST',
            success: function (rs) {
                $('#addBlockModal').modal('hide');
                showAlertFromResponse(rs);
            }
        });
    }
    else {
        $.ajax({
            url: UrlContent("Admin/EditBlock"),
            data: { id: $('#addBlockModal').attr('data-id'), content: content, title: $('#txtTitleHTML').val() },
            type: 'POST',
            success: function (rs) {
                $('#addBlockModal').modal('hide');
                showAlertFromResponse(rs);
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
        url: UrlContent("Admin/DeleteBlock"),
        data: { id: id },
        type: 'POST',
        success: function (rs) {
            $('#confirm-delete').modal('hide');
            showAlertFromResponse(rs);
        }
    });
}

function Preview(id) {
    $.ajax({
        url: UrlContent("Admin/GetContentBlock"),
        data: { id: id },
        type: 'POST',
        success: function (rs) {
            $('.content-block').html(rs.Content);
            $('#preBlockModal').modal('show');
        }
    });
}