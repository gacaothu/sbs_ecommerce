var url = window.location.href;

$(function () {
    checkAlertMessageDisplay('.main-content');
    $("table").dataTable();
});

function addPage() {
    $.ajax({
        type: 'GET',
        url: UrlContent("Admin/AddPage"),
        success: function (rs) {
            appendHtml('#addPageModal', rs);
        },
        error: function (rs) {
            console.dir(rs);
        }
    });
}

function editPage(id) {
    $.ajax({
        type: 'GET',
        url: UrlContent("Admin/EditPage"),
        data:{
            id: id
        },
        success: function (rs) {
            appendHtml('#addPageModal', rs);
        },
        error: function (rs) {
            console.dir(rs);
        }
    });
}

//function ConfirmAddPage() {
//    $('#addPageModal').find('h3').text('Add Page');
//    $('#addPageModal').find('#txtTitleHTML').val('');
//    $('#errorTitle').hide();
//    CKEDITOR.instances['txtHTML'].setData('');
//    $('#addPageModal').attr('data-action', 'Add');
//    $('#addPageModal').modal('show');
//}

//function ConfirmEditBlock(id) {
//    $('#addPageModal').find('h3').text('Edit Block');
//    $('#addPageModal').attr('data-action', 'Edit');
//    $('#addPageModal').attr('data-id', id);
//    $('#errorTitle').hide();
//    $.ajax({
//        url: UrlContent("Admin/GetContentPage"),
//        data: { id: id },
//        type: 'POST',
//        success: function (rs) {
//            CKEDITOR.instances['txtHTML'].setData(rs.Content);
//            $('#addPageModal').find('#txtTitleHTML').val(rs.Title);
//            $('#addPageModal').modal('show');
//        }
//    });
//}

//function SavePage() {
//    var content = CKEDITOR.instances['txtHTML'].getData();
//    if ($('#txtTitleHTML').val().trim() == '') {
//        $('#errorTitle').text("The Page name is required.");
//        $('#errorTitle').show();
//        return;
//    }
//    else {
//        //Check not space
//        if ($('#txtTitleHTML').val().trim().indexOf(" ") > -1) {
//            $('#errorTitle').text("The Page name can not contain space.");
//            $('#errorTitle').show();
//            return;
//        }
//        else {
//            if ($('#addPageModal').attr('data-id')) {
//                $.ajax({
//                    url: UrlContent("Admin/EditPage"),
//                    data: { id: $('#addPageModal').attr('data-id'), content: content, title: $('#txtTitleHTML').val() },
//                    type: 'POST',
//                    success: function (rs) {
//                        $('#addPageModal').modal('hide');
//                        showAlertFromResponse(rs);
//                    }
//                });
//            } else {
//                //Check duplicate
//                $.ajax({
//                    url: UrlContent("Admin/CheckDuplicateNamePage"),
//                    data: { name: $('#txtTitleHTML').val().trim(), id: $('#addPageModal').attr('data-id') },
//                    async: true,
//                    success: function (rs) {
//                        if (rs.Status == -1) {
//                            $('#errorTitle').text(rs.Message);
//                            $('#errorTitle').show();
//                            return false;
//                        }
//                        else {
//                            $('#errorTitle').hide();
//                            if ($('#addPageModal').attr('data-action') == 'Add') {
//                                $.ajax({
//                                    url: UrlContent("Admin/AddPage"),
//                                    data: { content: content, title: $('#txtTitleHTML').val() },
//                                    type: 'POST',
//                                    success: function (rs) {
//                                        $('#addPageModal').modal('hide');
//                                        showAlertFromResponse(rs);
//                                    }
//                                });
//                            }
//                        }
//                    }
//                });
//            }
//        }
//    }
//}

function ConfirmDelete(id) {
    $('#idPage').val(id);
    $('#confirm-delete').modal('show');
}

//function DeletePage(id) {
//    $.ajax({
//        url: UrlContent("Admin/DeletePage"),
//        data: { id: id },
//        type: 'POST',
//        success: function (rs) {
//            $('#confirm-delete').modal('hide');
//            showAlertFromResponse(rs);
//        }
//    });
//}

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
