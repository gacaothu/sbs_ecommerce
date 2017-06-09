var url = window.location.href;

$('#successModal').on('click', function () {
    window.location.reload();
});

$('.itemtoggle').click(function () {
    $(this).toggleClass('glyphicon-menu-down glyphicon-menu-up');
    $(this).parent().parent().find('.descrip-widget').toggle();
});

function SaveDragMenu() {
    var lstID = [];
    $('#Colum-Layout').children().each(function () {
        lstID.push($(this).attr('id').replace('item', ''));
    });
    $.ajax({
        url: UrlContent('/Admin/SaveMenu'),
        data: { lstID: lstID },
        type: 'POST',
        success: function (rs) {
            showAlertMessageAndReload('Menu has been updated successfully.', url);
        }
    });
}

function AddChildMenu() {
    if ($('#txtChildLabel').val().trim() == '') {
        $('#txtErrorChildLabel').show();
        return;
    }
    else {
        $('#txtErrorChildLabel').hide();
    }
    if ($('#rad-child-3').prop('checked') == true) {
        if ($('#txtChildUrl').val().trim() == '') {
            $('#txtErrorChildUrl').show();
            return;
        }
        else {
            $('#txtErrorChildUrl').hide();
        }
        $.ajax({
            url: UrlContent('/Admin/SaveMenu'),
            data: { id: $('#addChildMenuModal').attr('data-id'), url: $('#txtChildUrl').val(), name: $('#txtChildLabel').val() },
            type: 'POST',
            success: function (rs) {
                $('#addChildMenuModal').modal('hide');
                showAlertMessageAndReload('Menu child has been created successfully.', url);
            }
        });
    }
    else {
        var url = UrlContent("/Pages/Index/" + $('#input-page-child').find('option:selected').val());
        $.ajax({
            url: UrlContent('/Admin/SaveMenu'),
            data: { id: $('#addChildMenuModal').attr('data-id'), url: url, name: $('#txtChildLabel').val() },
            type: 'POST',
            success: function (rs) {
                $('#addChildMenuModal').modal('hide');
                showAlertMessageAndReload('Menu child has been created successfully.', url);
            }
        });
    }
}

function AddChildMenuConfirm(id) {
    $('#addChildMenuModal').attr('data-id', id);
    $('#addChildMenuModal').modal('show');
    //$(el).parent().parent().parent().find('.descrip-widget').append('<div class="child-item">Child Item</div>');
}

function EditHTML(id, name, href) {
    $('#error-url-edit-menu').hide();
    $('#error-label-edit-menu').hide();
    $('#editMenuModal').find('#txtUrl').val(href);
    $('#editMenuModal').find('#txtLabel').val(name);
    $('#editMenuModal').attr('data-id', id);
    $('#editMenuModal').modal('show');
}

function EditChildMenu(idParent, idChild, name, href) {
    $('#editChildMenuModal').find('#txtChildUrl-Modal').val(href);
    $('#editChildMenuModal').find('#txtChildLabel-Modal').val(name);
    $('#editChildMenuModal').attr('data-parentID', idParent);
    $('#editChildMenuModal').attr('data-childID', idChild);
    $('#editChildMenuModal').modal('show');
}

function SaveEditChildMenu() {
    if ($('#txtChildUrl-Modal').val().trim() == '') {
        $('#txteditErrorChildUrl').show();
        return;
    }
    else {
        $('#txteditErrorChildUrl').hide();
    }

    if ($('#txtChildLabel-Modal').val().trim() == '') {
        $('#txteditErrorChildLabel').show();
        return;
    }
    else {
        $('#txteditErrorChildLabel').hide();
    }
    $.ajax({
        url: UrlContent('/Admin/SaveMenu'),
        data: {
            parentID: $('#editChildMenuModal').attr('data-parentID'),
            childrenID: $('#editChildMenuModal').attr('data-childID'),
            url: $('#txtChildUrl-Modal').val(),
            name: $('#txtChildLabel-Modal').val()
        },
        type: 'POST',
        success: function (rs) {
            $('#editChildMenuModal').modal('hide');
            showAlertMessageAndReload('Menu child has been updated successfully.', url);
        }
    });
}

function ConfirmDeactive(id) {
    $('#confirm-menu').attr('data-id', id);
    $('#confirm-menu').modal('show');
}

function ConfirmDeleteChild(parentID, childID) {
    $('#confirm-child-menu').attr('data-parentID', parentID);
    $('#confirm-child-menu').attr('data-childID', childID);
    $('#confirm-child-menu').modal('show');
}

function DeleteChildMenu() {
    $.ajax({
        url: UrlContent('/Admin/DeleteChildMenu'),
        data: { parentID: $('#confirm-child-menu').attr('data-parentID'), childrenID: $('#confirm-child-menu').attr('data-childID') },
        type: 'POST',
        success: function (rs) {
            $('#confirm-child-menu').modal('hide');
            showAlertMessageAndReload('Menu child has been deleted successfully.', url);
        }
    });
}

function DeleteMenu(id) {
    $.ajax({
        url: UrlContent('/Admin/DeleteMenu'),
        data: { id: id },
        type: 'POST',
        success: function (rs) {
            $('#confirm-menu').modal('hide');
            $('#contentAlert').text('Front end website has been deleted menu.');

            showAlertMessageAndReload('Menu has been deleted successfully.', url);
        }
    });
}

function SaveEditMenu() {
    if ($('#txtLabel').val() == "") {
        $('#error-label-edit-menu').show();
        return;
    }
    else {
        $('#error-label-edit-menu').hide();
    }
    var newurl = '';
    if ($('#rad5').prop('checked') == true) {
        if ($('#txtUrl').val() == "") {
            $('#error-url-edit-menu').show();
            return;
        }
        else {
            $('#error-url-edit-menu').hide();
        }
        newurl = $('#editMenuModal').find('#txtUrl').val();
    }
    else {
        newurl = "~/Pages/Index/" + $('#edit-page').find('option:selected').val();
    }
    $.ajax({
        url: UrlContent('/Admin/EditMenu'),
        data: {
            id: $('#editMenuModal').attr('data-id'),
            url: newurl,
            name: $('#editMenuModal').find('#txtLabel').val()
        },
        type: 'POST',
        success: function (rs) {
            $('#editMenuModal').modal('hide');
            showAlertMessageAndReload('Menu has been updated successfully.', url);
        }
    });
}

function AddMenu() {
    if ($('#input-label').val() == "") {
        $('#error-menu-label').show();
        return;
    }
    else {
        $('#error-menu-label').hide();
    }
    if ($('#rad3').prop('checked') == true) {
        if ($('#input-url').val() == "") {
            $('#error-menu-url').show();
            return;
        }
        else {
            $('#error-menu-url').hide();
        }
        if ($('#input-label').val() != "" && $('#input-url').val() != "") {
            $.ajax({
                url: UrlContent('/Admin/AddMenu'),
                data: { name: $('#input-label').val(), url: $('#input-url').val() },
                type: 'POST',
                success: function (rs) {
                    showAlertMessageAndReload('Menu has been created successfully.', url);
                }
            });
        }
    }
    else {
        var path = "~/Pages/Index/" + $('#input-page').find('option:selected').val();
        $.ajax({
            url: UrlContent('/Admin/AddMenu'),
            data: { name: $('#input-label').val(), url: path },
            type: 'POST',
            success: function (rs) {
                showAlertMessageAndReload('Menu has been created successfully.', url);
            }
        });
    }
}

$(document).on('change', '.rd-option', function (e) {
    if ($(this).attr('id') == "rad2") {
        $('.select-customlinks').hide();
        $('.slect-page').show();
    }
    else {
        $('.select-customlinks').show();
        $('.slect-page').hide();
    }
});

$(document).on('change', '.rd-option', function (e) {
    if ($(this).attr('id') == "rad4") {
        $('.edit-select-customlinks').hide();
        $('.edit-slect-page').show();
    }
    else {
        $('.edit-select-customlinks').show();
        $('.edit-slect-page').hide();
    }
});

$(document).on('change', '.rd-option-child', function (e) {
    if ($(this).attr('id') == "rad-child-2") {
        $('.select-customlinks-child').hide();
        $('.slect-page-child').show();
    }
    else {
        $('.select-customlinks-child').show();
        $('.slect-page-child').hide();
    }
})

$(function () {
    checkAlertMessageDisplay('.main-content');

    $('.dragbox')
    .each(function () {
        $(this).hover(function () {
            //  $(this).find('h2').addClass('collapse');
        }, function () {
            //  $(this).find('h2').removeClass('collapse');
        })
        .find('h2').hover(function () {
            // $(this).find('.configure').css('visibility', 'visible');
        }, function () {
            //  $(this).find('.configure').css('visibility', 'hidden');
        })
        .click(function () {
            $(this).siblings('.dragbox-content').toggle();
        })
        .end()
    });
    $('.column').sortable({
        connectWith: '.column',
        handle: 'h2',
        cursor: 'move',
        placeholder: 'placeholder',
        forcePlaceholderSize: true,
        opacity: 0.4,
        stop: function (event, ui) {
            $(ui.item).find('h2').click();

            var sortorder = '';
            $('.column').each(function () {
                var itemorder = $(this).sortable('toArray');
                var columnId = $(this).attr('id');
                sortorder += columnId + '=' + itemorder.toString() + '&';

                var lstID = '';
                $('#Colum-Layout').children('.dragbox').each(function () {
                    $(this).attr('id');
                    //$('#TheForm').append('<input type="hidden" name="id" value=" ' + $(this).attr('id').replace('item', '') + ' " />');
                    lstID = lstID + $(this).attr('id').replace('item', '') + '_';
                });
                lstID = lstID.slice(0, -1);
                $('#preview-Iframe').attr('src', UrlContent("/Admin/PreviewMenu?id=" + lstID));
            });
            /*Pass sortorder variable to server using ajax to save state*/
        }
    })
    .disableSelection();
});