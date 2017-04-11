var oldTerm = null;
var sort = null;
var sorttype = null;
var brandId = null;
var rangeId = null;
var cgId = null;
var lstCategories = null;
var maxItem = 12;

function init(term, categories) {
    oldTerm = term;
    lstCategories = categories;
}

function searchProduct(e) {
    switch ($('#orderby').val()) {
        case "1":
            sort = 'price';
            sorttype = 'asc';
            break;
        case "2":
            sort = 'price';
            sorttype = 'desc';
            break;
        case "3":
            sort = 'name';
            sorttype = 'asc';
            break;
        case "4":
            sort = 'name';
            sorttype = 'desc';
            break;
        default:
            break;
    }
    if ($(e).is('a')) {
        var name = $(e).text();
        var childCategories = findCategory(lstCategories, name);
        cgId = childCategories.Category_ID;
        if (!$.isEmptyObject(childCategories.Items)) {
            $('#categories').find('.menu').empty();
            for (var i = 0; i < childCategories.Items.length; i++) {
                $('#categories').find('.menu').append('<li class="col-sm-3"><a style="cursor:pointer;" onclick="searchProduct(this)">' + childCategories.Items[i].Category_Name + '</a></li>');
            }
        }
        processAPI(oldTerm, sort, sorttype, cgId, brandId, rangeId);
    } else {
        processAPI(oldTerm, sort, sorttype, cgId, brandId, rangeId);
    }
}

function navigatePage(e, maxPage) {
    var currentPage = parseInt($('#currentPage').val());
    var page = parseInt($(e).text());
    if (page == currentPage) {
        return;
    } else {
        var item = $(e).attr('data-type');
        if (item == 'next') {
            if (currentPage < maxPage) {
                page = currentPage + 1;
            } else
                return;
        } else if (item == 'prev') {
            if (currentPage > 1) {
                page = currentPage - 1;
            } else
                return;
        }
    }
    $.ajax({
        url: UrlContent("/Product/NavigatePage"),
        data: {
            currentPage: !isNaN(page) ? page : currentPage
        },
        success: function (rs) {
            $('.products').empty();
            $('.products').append(rs);
            if (isNaN(page)) {
                return;
            }
            $('.paginations').children().each(function (e) {
                $('#currentPage').val(page);
                $('.product-count').text('Showing ' + page + '/' + maxPage + ' of ' + maxPage + ' pages');
                $(this).removeClass('current');
                var liPage = parseInt($(this).text());
                if (liPage == page) {
                    $(this).addClass('current');
                    if (page == 1) {
                        $('.prev').addClass('disabled');
                        $('.next').removeClass('disabled');
                    } else if (liPage == maxPage) {
                        $('.prev').removeClass('disabled');
                        $('.next').addClass('disabled');
                    } else {
                        $('.prev').removeClass('disabled');
                        $('.next').removeClass('disabled');
                    }
                }
            });
        }
    });
}

function processAPI(term, sort, sorttype, cgId, brndId, rangeId) {
    $('.pagination-bar').remove();
    $.ajax({
        url: UrlContent("/Product/Search"),
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        data: {
            term: term,
            brandId: brndId,
            rangeId: rangeId,
            sort: sort,
            sortType: sorttype,
            cgId: cgId
        },
        success: function (rs) {
            $('.products').empty();
            $('.products').append(rs.Partial);
        },
        error: function (rs) {
            console.log(rs);
        }
    });
}

function findCategory(array, name) {
    var result;
    for (var i = 0; i < array.length; i++) {
        if (array[i].Category_Name == name) {
            result = array[i];
            break;
        }
    }
    return result;
}