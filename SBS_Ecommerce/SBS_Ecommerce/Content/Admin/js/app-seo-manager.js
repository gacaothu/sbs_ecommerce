var url = window.location.href;
$(function () {
    checkAlertMessageDisplay('.main-content');
    $('#seo-table').dataTable();

    $(document).on('click', '#add-edit-seo-btn', function () {
        var check = true;
        check = validateControls();
        if (!check) {
            return false;
        }
        $.ajax({
            url: UrlContent("Admin/AddOrUpdateSEO"),
            type: 'POST',
            processData: false,
            contentType: false,
            data: new FormData($('form').get(0)),
            success: function (rs) {
                showAlertFromResponse(rs);
            }
        });
    });

    $(document).on('click', '#delete-btn', function () {
        $.ajax({
            type:'POST',
            url: UrlContent("Admin/DeleteSEO"),
            data: {id: $(this).data('id')},
            success: function (rs) {
                showAlertFromResponse(rs);
            }
        });
    });
})

function openEdit(id) {
    $.ajax({
        url: UrlContent("Admin/GetSEO"),
        data: {id: id},
        success: function (rs) {
            if (rs.Status == 0) {
                $('#form-seo').empty();
                $('#form-seo').append(rs.Html);
                showSEOModal();
            } else {
                showAlertFromResponse(rs);
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