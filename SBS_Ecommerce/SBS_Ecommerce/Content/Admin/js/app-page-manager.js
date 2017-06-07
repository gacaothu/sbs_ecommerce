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
    $('#addPageModal').find('#checkLayout').prop('checked', false);
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
        //url: '@Url.Content("~/Admin/GetContentPage")',
        url: UrlContent("Admin/GetContentPage"),
        data: { id: id },
        type: 'POST',
        success: function (rs) {
            CKEDITOR.instances['txtHTML'].setData(rs.Content);
            $('#addPageModal').find('#txtTitleHTML').val(rs.Title);
            $('#addPageModal').find('#checkLayout').prop('checked', rs.UsingLayout);
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
                //url: '@Url.Action("CheckDuplicateNamePage","Admin")',
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
                                //url: '@Url.Content("~/Admin/AddPage")',
                                url: UrlContent("Admin/AddPage"),
                                data: { content: content, title: $('#txtTitleHTML').val(), usingLayout: $('#addPageModal').find('#checkLayout').prop('checked') },
                                type: 'POST',
                                success: function (rs) {
                                    $('#addPageModal').modal('hide');
                                    sessionStorage.reloadAfterLoadPage = true;
                                    localStorage.setItem("callback", "Page has been created successfully.");
                                    window.location.href = url;
                                    //showMessageAlert('1', 'Page has been created successfully.');
                                }

                            });
                        }
                        else {
                            $.ajax({
                                //url: '@Url.Content("~/Admin/EditPage")',
                                url: UrlContent("Admin/EditPage"),
                                data: { id: $('#addPageModal').attr('data-id'), content: content, title: $('#txtTitleHTML').val(), usingLayout: $('#addPageModal').find('#checkLayout').prop('checked') },
                                type: 'POST',
                                success: function (rs) {
                                    $('#addPageModal').modal('hide');
                                    sessionStorage.reloadAfterLoadPage = true;
                                    localStorage.setItem("callback", "Page has been updated successfully.");
                                    window.location.href = url;
                                    //showMessageAlert('1', 'Page has been updated successfully.');
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
        //url: '@Url.Content("~/Admin/DeletePage")',
        url: UrlContent("Admin/DeletePage"),
        data: { id: id },
        type: 'POST',
        success: function (rs) {
            $('#confirm-delete').modal('hide');
            sessionStorage.reloadAfterLoadPage = true;
            localStorage.setItem("callback", "Page has been deleted successfully.");
            window.location.href = url;
            //showMessageAlert('1', 'Page has been deleted successfully.');
        }

    });
}

function Preview(id) {
    $.ajax({
        //url: '@Url.Content("~/Admin/GetContentBlock")',
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
