$(document).on('click', '#successModal', function () {
    window.location.reload();
});

$(function () {
    CKEDITOR.replace('txtHTML');
});

function ConfirmAddBlog() {
    $('#addBlogModal').find('h3').text('Add Blog');
    $('#addBlogModal').find('#txtTitleHTML').val('');
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
                    $('#successModal').find('#contentAlert').text('Blog has been created successfully. Front end website has been displayed new blog.');
                    $('#successModal').modal('show');
                    setTimeout(function () { window.location.reload(); }, 1500);
                }

            });
        }
    }
    else {
        if ($('#imgThumbnail')[0].files[0] != undefined) {
            UploadThumbnail('edit');
        }
        else {
            $.ajax({
                url: UrlContent('/Admin/EditBlog'),
                data: { id: $('#addBlogModal').attr('data-id'), content: content, title: $('#txtTitleHTML').val(), thumb: $('.imgSlider').find('img').attr('src') },
                type: 'POST',
                success: function (rs) {
                    $('#addBlogModal').modal('hide');
                    $('#successModal').find('#contentAlert').text('Blog has been updated successfully.');
                    $('#successModal').modal('show');
                    setTimeout(function () { window.location.reload(); }, 1500);
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
            //alert(xhr.responseText);
            var rs = xhr.responseText.replace(/"/g, '');
            // window.location.reload();
            if (type == 'add') {
                $.ajax({
                    url: UrlContent('/Admin/AddBlog'),
                    data: { content: content, title: $('#txtTitleHTML').val(), path: rs },
                    type: 'POST',
                    success: function (rs) {
                        $('#addBlogModal').modal('hide');
                        $('#successModal').find('#contentAlert').text('Blog successful added');
                        $('#successModal').modal('show');
                        setTimeout(function () { window.location.reload(); }, 1500);
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
                        $('#successModal').find('#contentAlert').text('Blog successful added');
                        $('#successModal').modal('show');
                        setTimeout(function () { window.location.reload(); }, 1500);
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
            $('#successModal').find('#contentAlert').text('Blog has been deleted successfully.');
            $('#successModal').modal('show');
            setTimeout(function () { window.location.reload(); }, 1500);
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