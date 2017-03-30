$('#successModal').on('click', function () {
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
    $('#addBlogModal').modal('show');
}

function ConfirmEditBlock(id) {
    $('#addBlogModal').find('h3').text('Edit Blog');
    $('#addBlogModal').attr('data-action', 'Edit');
    $('#addBlogModal').attr('data-id', id);
    $.ajax({
        url: '/Admin/GetContentBlog',
        data: { id: id },
        type: 'POST',
        success: function (rs) {
            CKEDITOR.instances['txtHTML'].setData(rs.Content);
            if (rs.Thumb != null) {
                if ($('#imgThumbnail').parent().parent().find('img').attr('src') != undefined) {
                    $('#imgThumbnail').parent().parent().find('img').attr('src', e.target.result);
                }
                else {
                    $('#imgThumbnail').parent().parent().find('.imgSlider').empty().append('<img src="' + e.target.result + '" style="width:150px;height:100px;">');
                }
            }
            else {
                $('#imgThumbnail').val('');
                $('#imgThumbnail').parent().parent().find('img').remove();
                $('#imgThumbnail').parent().parent().find('.imgSlider').append('<i class="glyphicon glyphicon-picture"></i><div style="padding-left:10px;padding-bottom: 13px;">No image</div>');
            }

            $('#addBlogModal').find('#txtTitleHTML').val(rs.Title);
            $('#addBlogModal').modal('show');
        }
    });
}

function SaveBlog() {
    if ($('#addBlogModal').attr('data-action') == 'Add') {
        UploadThumbnail();
    }
    else {
        $.ajax({
            url: '/Admin/EditPage',
            data: { id: $('#addPageModal').attr('data-id'), content: content, title: $('#txtTitleHTML').val() },
            type: 'POST',
            success: function (rs) {
                $('#addPageModal').modal('hide');
                $('#successModal').find('#contentAlert').text('Page successful edited');
                $('#successModal').modal('show');
            }
        });
    }
}

function UploadThumbnail() {
    var content = CKEDITOR.instances['txtHTML'].getData();
    var formdata = new FormData(); //FormData object
    for (var i = 0; i < $('#imgThumbnail').length; i++) {
        if ($('#imgThumbnail')[i].files[0] != undefined) {
            formdata.append("", $('#imgThumbnail')[i].files[0]);
        }
    }
    var xhr = new XMLHttpRequest();
    xhr.open('POST', '/Admin/UploadImageThumbnail');
    xhr.send(formdata);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            //alert(xhr.responseText);
            var rs = xhr.responseText.replace(/"/g, '');
            // window.location.reload();
            $.ajax({
                url: '/Admin/AddBlog',
                data: { content: content, title: $('#txtTitleHTML').val(), path: rs },
                type: 'POST',
                success: function (rs) {
                    $('#addBlogModal').modal('hide');
                    $('#successModal').find('#contentAlert').text('Blog successful added');
                    $('#successModal').modal('show');
                }

            });

        }
    }
}

function ConfirmDelete(id) {
    $('#confirm-delete').attr('data-id', id);
    $('#confirm-delete').modal('show');
}

function DeleteBlog(id) {
    $.ajax({
        url: '/Admin/DeleteBlog',
        data: { id: id },
        type: 'POST',
        success: function (rs) {
            $('#confirm-delete').modal('hide');
            $('#successModal').find('#contentAlert').text('Blog successful deleted');
            $('#successModal').modal('show');
        }

    });
}

function Preview(id) {
    $.ajax({
        url: '/Admin/GetContentBlock',
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

$("#imgThumbnail").change(function () {
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