var url = window.location.href;
$(function () {
    checkAlertMessageDisplay('.main-content');
    CKEDITOR.replace('txtHTML');
    $("table").dataTable();
});

$('#successModal').on('click', function () {
    window.location.reload();
});

function ConfirmAddPage() {
    $('#addPageModal').find('h3').text('Add Page');
    $('#addPageModal').find('#txtTitleHTML').val('');
    $('#errorTitle').hide();
    CKEDITOR.instances['txtHTML'].setData('');
    $('#addPageModal').attr('data-action', 'Add');
    $('#addPageModal').modal('show');
}

function ConfirmEditBlock(id) {
    $('#addPageModal').find('h3').text('Edit Block');
    $('#addPageModal').attr('data-action', 'Edit');
    $('#addPageModal').attr('data-id', id);
    $('#errorTitle').hide();
    $.ajax({
        url: UrlContent("Admin/GetContentPage"),
        data: { id: id },
        type: 'POST',
        success: function (rs) {
            CKEDITOR.instances['txtHTML'].setData(rs.Content);
            $('#addPageModal').find('#txtTitleHTML').val(rs.Title);
            $('#addPageModal').modal('show');
        }
    });
}

function SavePage() {
    var content = CKEDITOR.instances['txtHTML'].getData();
    if ($('#txtTitleHTML').val().trim() == '') {
        $('#errorTitle').text("The name is required.");
        $('#errorTitle').show();
        return;
    }
    else {
        //Check not space
        if ($('#txtTitleHTML').val().trim().indexOf(" ") > -1) {
            $('#errorTitle').text("The name can not contain space.");
            $('#errorTitle').show();
            return;
        }
        else {
            //Check duplicate
            $.ajax({
                url: UrlContent("Admin/CheckDuplicateNamePage"),
                data: { name: $('#txtTitleHTML').val().trim(), id: $('#addPageModal').attr('data-id') },
                async: true,
                success: function (rs) {
                    if (rs == true) {
                        $('#errorTitle').text("The name has been duplicated.");
                        $('#errorTitle').show();
                        return;
                    }
                    else {
                        $('#errorTitle').hide();
                        if ($('#addPageModal').attr('data-action') == 'Add') {
                            $.ajax({
                                url: UrlContent("Admin/AddPage"),
                                data: { content: content, title: $('#txtTitleHTML').val()},
                                type: 'POST',
                                success: function (rs) {
                                    $('#addPageModal').modal('hide');
                                    showAlertMessageAndReload("Page has been created successfully.", url);
                                }
                            });
                        }
                        else {
                            $.ajax({
                                url: UrlContent("Admin/EditPage"),
                                data: { id: $('#addPageModal').attr('data-id'), content: content, title: $('#txtTitleHTML').val()},
                                type: 'POST',
                                success: function (rs) {
                                    $('#addPageModal').modal('hide');
                                    showAlertMessageAndReload("Page has been updated successfully.", url);
                                }
                            });
                        }
                    }
                }
            });
        }
    }
}

function ConfirmDelete(id) {
    $('#confirm-delete').attr('data-id', id);
    $('#confirm-delete').modal('show');
}

function DeletePage(id) {
    $.ajax({
        url: UrlContent("Admin/DeletePage"),
        data: { id: id },
        type: 'POST',
        success: function (rs) {
            $('#confirm-delete').modal('hide');
            showAlertMessageAndReload("Page has been deleted successfully.", url);
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

$('span[onclick]').each(function () {
    $(this).data('onclick', this.onclick);
    this.onclick = function (event) {
        if ($(this).attr('id', 'cke_122_label')) { // HERE
            return false;
        };
        $(this).data('onclick').call(this, event || window.event);
    };
});
