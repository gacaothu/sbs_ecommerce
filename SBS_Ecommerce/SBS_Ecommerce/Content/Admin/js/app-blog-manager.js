var url = window.location.href;
$(function () {
    checkAlertMessageDisplay('.main-content');
    $("table").dataTable();
    CKEDITOR.replace('txtHTML');
});

function CommentBlog(id) {
    window.location.href = UrlContent("/Admin/BlogComment/" + id);
}

function ConfirmEditBlock(id) {
    $('#addBlogModal').find('h3').text('Edit Blog');
    $('#addBlogModal').attr('data-action', 'Edit');
    $('#addBlogModal').attr('data-id', id);
    $('#errorTitle').hide();
    $.ajax({
        url: UrlContent('/Admin/GetContentBlog'),
        data: { id: id },
        type: 'POST',
        success: function (rs) {
            CKEDITOR.instances['txtHTML'].setData(rs.Content);
            if (rs.Thumb != "" && rs.Thumb != null) {
                if ($('#imgThumbnail').parent().parent().find('img').attr('src') != undefined) {
                    $('#imgThumbnail').parent().parent().find('img').attr('src', rs.Thumb);
                }
                else {
                    $('#imgThumbnail').parent().parent().find('.imgSlider').empty().append('<img src="' + rs.Thumb + '" style="width:150px;height:100px;">');
                }
            }
            else {
                $('#imgThumbnail').val('');
                $('#imgThumbnail').parent().parent().find('.imgSlider').empty();
                $('#imgThumbnail').parent().parent().find('.imgSlider').append('<i class="glyphicon glyphicon-picture"></i><div style="padding-left:10px;padding-bottom: 13px;">No image</div>');
            }
            $('#addBlogModal').find('#txtTitleHTML').val(rs.Title);
            $('#addBlogModal').modal('show');
        }
    });
}

$(document).on('click', '#successModal', function () {
    window.location.reload();
});

function ConfirmAddBlog() {
    $('#addBlogModal').find('h3').text('Add Blog');
    $('#addBlogModal').find('#txtTitleHTML').val('');
     $('#errorTitle').hide();
    CKEDITOR.instances['txtHTML'].setData('');
    $('#addBlogModal').attr('data-action', 'Add');
    $('#imgThumbnail').val('');
    //Empty image
    $('#imgThumbnail').parent().parent().find('.imgSlider').empty().append('<i class="glyphicon glyphicon-picture"></i><div style="padding-left:10px;padding-bottom: 13px;">No image</div>');
    $('#addBlogModal').modal('show');
}

function SaveBlog() {
    if ($('#txtTitleHTML').val().trim() == '') {
        $('#errorTitle').show();
        return;
    }
    else {
        $('#errorTitle').hide();
    }
    var content = CKEDITOR.instances['txtHTML'].getData();
    if ($('#addBlogModal').attr('data-action') == 'Add') {
        if ($('#imgThumbnail')[0].files[0] != undefined) {
            UploadThumbnail('add');
        }
        else {
            $.ajax({
                url: UrlContent('/Admin/AddBlog'),
                data: { content: content, title: $('#txtTitleHTML').val(), path: "" },
                type: 'POST',
                success: function (rs) {
                    $('#addBlogModal').modal('hide');
                    showAlertFromResponse(rs);
                }
            });
        }
    }
    else {
        if ($('#imgThumbnail')[0].files[0] != undefined) {
            UploadThumbnail('edit');
        }
        else {
            var thumb = "";
            if ($('.imgSlider').find('img').attr('src') != undefined) {
                thumb = "nochange";
            }
            $.ajax({
                url: UrlContent('/Admin/EditBlog'),
                data: { id: $('#addBlogModal').attr('data-id'), content: content, title: $('#txtTitleHTML').val(), thumb: thumb },
                type: 'POST',
                success: function (rs) {
                    $('#addBlogModal').modal('hide');
                    showAlertFromResponse(rs);
                }
            });
        }
    }
}

function UploadThumbnail(type) {
    var content = CKEDITOR.instances['txtHTML'].getData();
    var formdata = new FormData(); //FormData object
    for (var i = 0; i < $('#imgThumbnail').length; i++) {
        if ($('#imgThumbnail')[i].files[0] != undefined) {
            formdata.append("", $('#imgThumbnail')[i].files[0]);
        }
    }
    var xhr = new XMLHttpRequest();
    xhr.open('POST', UrlContent("/Admin/UploadImageThumbnail"));
    xhr.send(formdata);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var rs = xhr.responseText.replace(/"/g, '');
            if (type == 'add') {
                $.ajax({
                    url: UrlContent('/Admin/AddBlog'),
                    data: { content: content, title: $('#txtTitleHTML').val(), path: rs },
                    type: 'POST',
                    success: function (rs) {
                        $('#addBlogModal').modal('hide');
                        showAlertFromResponse(rs);
                    }
                });
            }
            else {
                $.ajax({
                    url: UrlContent('/Admin/EditBlog'),
                    data: { id: $('#addBlogModal').attr('data-id'), content: content, title: $('#txtTitleHTML').val(), thumb: rs },
                    type: 'POST',
                    success: function (rs) {
                        $('#addBlogModal').modal('hide');
                        showAlertFromResponse(rs);
                    }
                });
            }
        }
    }
}

function ConfirmDelete(id) {
    $('#confirm-delete').attr('data-id', id);
    $('#confirm-delete').modal('show');
}

function DeleteBlog(id) {
    $.ajax({
        url: UrlContent('/Admin/DeleteBlog'),
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
        url: UrlContent('/Admin/GetContentBlock'),
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

$(document).on('change', "#imgThumbnail", function () {
    var input = this;
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            if ($(input).parent().parent().find('img').attr('src') != undefined) {
                $(input).parent().parent().find('img').attr('src', e.target.result);
            }
            else {
                $(input).parent().parent().find('.imgSlider').empty().append('<img src="' + e.target.result + '" style="width:150px;height:100px;">');
            }
        };
        reader.readAsDataURL(input.files[0]);
    }
});

function RemoveImage(element) {
    if ($(element).parent().parent().find('img').attr('src') != undefined) {
        $(element).parent().find('#imgThumbnail').val('');
        $(element).parent().parent().find('img').remove();
        $(element).parent().parent().find('.imgSlider').append('<i class="glyphicon glyphicon-picture"></i><div style="padding-left:10px;padding-bottom: 13px;">No image</div>');
    }
}