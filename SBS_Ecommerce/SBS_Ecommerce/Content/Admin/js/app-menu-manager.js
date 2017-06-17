var url = window.location.href;

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
            showAlertFromResponse(rs);
        }
    });
}

function AddChildMenu() {
    var data = null;
    if ($('#txtChildLabel').val().trim() == '') {
        $('#error-add-child-label').show();
        return;
    }
    else {
        $('#error-add-child-label').hide();
    }
    if ($('#rad-child-3').prop('checked') == true) {
        if ($('#txtChildUrl').val().trim() == '') {
            $('#error-add-child-url').show();
            return;
        }
        else {
            $('#error-add-child-url').hide();
        }
        data = {
            id: $('#addChildMenuModal').attr('data-id'),
            url: $('#txtChildUrl').val(),
            name: $('#txtChildLabel').val()
        }
    }
    else {
        data = {
            id: $('#addChildMenuModal').attr('data-id'),
            url: "~/Pages/Index/" + $('#input-page-child').find('option:selected').val(),
            name: $('#txtChildLabel').val()
        };
    }
    $.ajax({
        url: UrlContent('/Admin/AddChildMenu'),
        data: data,
        type: 'POST',
        success: function (rs) {
            $('#addChildMenuModal').modal('hide');
            showAlertFromResponse(rs);
        }
    });
}

function AddChildMenuConfirm(id) {
    $('#error-add-child-url').hide();
    $('#error-add-child-label').hide();
    $('#addChildMenuModal').attr('data-id', id);
    $('#addChildMenuModal').modal('show');
    //$(el).parent().parent().parent().find('.descrip-widget').append('<div class="child-item">Child Item</div>');
}

function EditHTML(id, name, href) {
    var nameOfPage = getNameOfPage(href);
    if (checkNameValid('#edit-page', nameOfPage)) {
        $('#editMenuModal').find('#edit-page').val(nameOfPage);
    }
    $('#error-edit-url-menu').hide();
    $('#error-edit-label-menu').hide();

    $('#editMenuModal').find('#txtUrl').val(href);
    $('#editMenuModal').find('#txtLabel').val(name);
    $('#editMenuModal').attr('data-id', id);
    $('#editMenuModal').modal('show');
}

function EditChildMenu(idParent, idChild, name, href) {
    var nameOfPage = getNameOfPage(href);
    if (checkNameValid('#input-page-child-edit', nameOfPage)) {
        $('#editChildMenuModal').find('#input-page-child-edit').val(nameOfPage);
    }
    $('#error-edit-child-url').hide();
    $('#error-edit-child-label').hide();

    $('#editChildMenuModal').find('#txtChildUrl-Modal').val(href);    
    $('#editChildMenuModal').find('#txtChildLabel-Modal').val(name);
    $('#editChildMenuModal').attr('data-parentID', idParent);
    $('#editChildMenuModal').attr('data-childID', idChild);
    $('#editChildMenuModal').modal('show');
}

function SaveEditChildMenu() {
    if ($('#txtChildUrl-Modal').val().trim() == '') {
        $('#error-edit-child-url').show();
        return;
    }
    else {
        $('#error-edit-child-url').hide();
    }

    if ($('#txtChildLabel-Modal').val().trim() == '') {
        $('#error-edit-child-label').show();
        return;
    }
    else {
        $('#error-edit-child-label').hide();
    }
    var data = null;
    if ($('#rad-child-4').prop('checked') == true) {
        data = {
            parentID: $('#editChildMenuModal').attr('data-parentID'),
            childrenID: $('#editChildMenuModal').attr('data-childID'),
            url: "~/Pages/Index/" + $('#input-page-child-edit').find('option:selected').val(),
            name: $('#txtChildLabel-Modal').val()
        }
    } else {
        data = {
            parentID: $('#editChildMenuModal').attr('data-parentID'),
            childrenID: $('#editChildMenuModal').attr('data-childID'),
            url: $('#txtChildUrl-Modal').val(),
            name: $('#txtChildLabel-Modal').val()
        }
    }

    $.ajax({
        url: UrlContent('/Admin/EditChildMenu'),
        data: data,
        type: 'POST',
        success: function (rs) {
            $('#editChildMenuModal').modal('hide');
            showAlertFromResponse(rs);
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
            showAlertFromResponse(rs);
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
            showAlertFromResponse(rs);
        }
    });
}

function SaveEditMenu() {
    if ($('#txtLabel').val() == "") {
        $('#error-edit-label-menu').show();
        return;
    }
    else {
        $('#error-edit-label-menu').hide();
    }
    var newurl = '';
    if ($('#rad5').prop('checked') == true) {
        if ($('#txtUrl').val() == "") {
            $('#error-edit-url-menu').show();
            return;
        }
        else {
            $('#error-edit-url-menu').hide();
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
            showAlertFromResponse(rs);
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
                    showAlertFromResponse(rs);
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
                showAlertFromResponse(rs);
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

$(document).on('change', '.rd-option-child', function (e) {
    if ($(this).attr('id') == "rad-child-4") {
        $('.select-customlinks-child-edit').hide();
        $('.slect-page-child-edit').show();
    }
    else {
        $('.select-customlinks-child-edit').show();
        $('.slect-page-child-edit').hide();
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

function getNameOfPage(name) {
    var index = name.lastIndexOf('/');
    return name.substr(index + 1, name.length);
}

function checkNameValid(select, name) {
    var check = false;
    var id = select + ' option';
    $(id).each(function () {
        if ($(this).val() == name) {
            check = true;
        }
    });
    return check;
}