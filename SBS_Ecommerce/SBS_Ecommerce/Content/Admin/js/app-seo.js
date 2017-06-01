var url = window.location.href;
$(function () {
    if (sessionStorage.reloadAfterLoadPage) {
        var msgCallback = localStorage.getItem("seocallback");
        if (msgCallback) {
            var html = '<div role="alert" class="alert alert-success alert-dismissible">'
                + '<button type="button" data-dismiss="alert" aria-label="Close" class="close"><span aria-hidden="true" class="s7-close"></span></button>'
                + '<span id="alert-msg-success">' + msgCallback + '</span></div>';
            $('.panel-body').prepend(html);
            sessionStorage.reloadAfterLoadPage = false;
            localStorage.removeItem("seocallback");
        }
    }

    $('#seo-table').dataTable();

    $(document).on('click', '#add-edit-seo-btn', function () {
        var check = true;
        check = validateControls();

        if (!check) {
            return;
        }
        $.ajax({
            url: UrlContent("Admin/AddOrUpdateSEO"),
            type: 'POST',
            processData: false,
            contentType: false,
            data: new FormData($('form').get(0)),
            success: function (rs) {
                sessionStorage.reloadAfterLoadPage = true;
                if (rs.Status == 0) {
                    if ($('#txtId').val()) {
                        localStorage.setItem("seocallback", "SEO configuration has been update successfully.");
                    } else
                        localStorage.setItem("seocallback", "SEO configuration has been added successfully.");
                    window.location.href = url;
                } else {
                    console.log(rs.Message);
                }                
            }
        });
    });

    $(document).on('click', '#delete-btn', function () {
        $.ajax({
            type:'POST',
            url: UrlContent("Admin/DeleteSEO"),
            data: {id: $(this).data('id')},
            success: function (rs) {
                sessionStorage.reloadAfterLoadPage = true;
                if (rs.Status == 0) {
                    localStorage.setItem("seocallback", "SEO configuration has been deleted successfully.");
                    window.location.href = url;
                } else {
                    console.log(rs.Message);
                }
            }
        });
    });
})

function openEdit(id) {
    $.ajax({
        url: UrlContent("Admin/GetSEO"),
        data: {id: id},
        success: function (rs) {
            if (!rs.Status) {
                $('#form-seo').empty();
                $('#form-seo').append(rs);
                showSEOModal();
            }
        }
    });
}

function showConfirm(id) {
    $('#confirm-delete #delete-btn').attr('data-id', id);
    $('#confirm-delete').modal('show');
}

function showSEOModal() {
    clearErrorControls();
    $('#form-seo').modal('show');
}

function clearErrorControls() {
    clearError('txtTitle');
    clearError('txtUrl');
    clearError('txtKeywords');
    clearError('txtDescription');
}

function validateControls() {
    var check = true;
    if (!validateRequired('txtTitle', 'Title', true)) {
        check = false;
    }
    if (!validateRequired('txtUrl', 'Url', true)) {
        check = false;
    }
    if (!validateRequired('txtKeywords', 'Keywords', true)) {
        check = false;
    }
    if (!validateRequired('txtDescription', 'Description', true)) {
        check = false;
    }
    return check;
}