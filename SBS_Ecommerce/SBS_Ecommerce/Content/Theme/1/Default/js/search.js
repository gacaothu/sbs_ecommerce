﻿var oldTerm = null;
var sort = null;
var sorttype = null;
var cgId = null;
var brandId = [];
var rangeId = [];
var lstCategories = [];
var lstProducts = []
var lstBrand = [];
var lstPriceRange = [];
var maxItem = 12;
var domain = "http://qa.bluecube.com.sg/pos3v2-wserv";
function init(term, categories, brands, pricerange) {
    oldTerm = term;
    lstCategories = categories;
    lstBrand = brands;
    lstPriceRange = pricerange;
}

window.addEventListener('popstate', function (event) {
    window.location.reload();
});



function initIndex(categories) {
    lstCategories = categories;
}

function searchProduct(e) {
    if ($(e).is('a')) {
        cgId = parseInt($(e).attr('data-id'));
        var childCategories = findCategory(lstCategories, cgId);
        if (!$.isEmptyObject(childCategories.Items)) {
            $('#categories').find('.menu').empty();
            for (var i = 0; i < childCategories.Items.length; i++) {
                $('#categories').find('.menu').append('<li class="col-sm-3"><a style="cursor:pointer;" onclick="searchProduct(this)" data-id=' + childCategories.Items[i].Category_ID + '>' + childCategories.Items[i].Category_Name + '</a></li>');
            }
        }
        SearchCategory(cgId);
    }
}

$(document).on('input', '#tags', function () {

    $('#category-result-search').empty();
    $('#product-result-search').empty();

    if ($(this).val().trim() == '') {
        $('#category-result-search').hide();
        $('#product-result-search').hide();
        $('#see-all').hide();
        return;
    }
    else {
        $('#category-result-search').show();
        $('#product-result-search').show();
        $('#resultSearch').show();
        $('#see-all').show();
    }

    var nonCategories = true;
    var countCategories = 0;
    for (var i = 0; i < lstCategories.length; i++) {
        if (countCategories < 10) {
            var indexof = lstCategories[i].Category_Name.trim().toUpperCase().search($(this).val().trim().toUpperCase());
            if (indexof >= 0) {
                $('#category-result-search').append('<div class="item-result-search" onclick="SearchCategoryIntel(' + "\'" + lstCategories[i].Category_ID + "\'" + ')">' + lstCategories[i].Category_Name + '</div>');
                nonCategories = false;
                countCategories++;
            }
        }
        else {
            break;
        }
    }

    if (nonCategories) {
        $('#category-result-search').hide();
    }
    else {
        $('#category-result-search').show();
    }

    var nonProduct = true;

    //Get lstProducts from api by search input text
    $.ajax({
        url: UrlContent("/Home/SearchProduct"),
        type: "POST",
        data: { text: $(this).val().trim() },
        success: function (lstProducts) {
            for (var i = 0; i < lstProducts.length; i++) {
                $('#product-result-search').append('<div onclick="SearchProductIntel(' + "\'" + lstProducts[i].Product_ID + "\'" + ')" style="postion:relative;float:left;width:100%" class="item-product-result"><span style="width:55px;height:55px;position:relative;float:left;margin-right:5px;"><img src="' + domain + lstProducts[i].Small_Img + '" style="width:100%;height:100%" /></span><span>' +
                    lstProducts[i].Product_Name + '</span><span style="color:#e26f47;margin-left:5px;">$' + lstProducts[i].Promotion_Price + '</span></div>');
                nonProduct = false;
            }
            if (nonProduct) {
                $('#product-result-search').hide();
            }
            else {
                $('#product-result-search').show();
            }
        }
    });

});

$(document).on('click', function () {
    if ($('#tags').is(":focus") == false) {
        $('#resultSearch').hide();
    }

});

function SearchProductIntel(id) {
    window.location.href = UrlContent('/Product/Details/' + id);
}

function SearchCategoryIntel(id) {
    $('#CategoryId').val(id);
   window.location.href= UrlContent('/Product/Search?&cgID=' + id + "&filter=false&currentPage=1");
}

$(document).on('change', '#brand .input-rule', function () {
    brandId = [];
    $('#brand').find('.input-rule').each(function () {
        if ($(this).attr('class').indexOf('selected') >= 0) {
            brandId.push(parseInt($(this).children('input').attr('data-id')));
        }
    });
    processAPI();
});

$(document).on('change', '#price .input-rule', function () {
    rangeId = [];
    $('#price').find('.input-rule').each(function () {
        if ($(this).attr('class').indexOf('selected') >= 0) {
            rangeId.push(parseInt($(this).children('input').attr('data-id')));
        }
    });
    processAPI();
});

$(document).on('change', '#orderby', function () {
    switch (parseInt(this.value)) {
        case 1:
            sort = 'price';
            sorttype = 'asc';
            break;
        case 2:
            sort = 'price';
            sorttype = 'desc';
            break;
        case 3:
            sort = 'name';
            sorttype = 'asc';
            break;
        case 4:
            sort = 'name';
            sorttype = 'desc';
            break;
        default:
            break;
    }
    SortApi();
});



function SortApi() {
    var data = {
        Keyword: getPramater('keyWord'),
        BrandID: brandId,
        RangeID: rangeId,
        Sort: sort,
        SortType: sorttype,
        CgID: getPramater('cgID'),
        Filter: true,
        CurrentPage: parseInt($('#currentPage').val())
    }
    
    //Process Url;
    var url = window.location.href;

    if (url.indexOf('&sort=') != -1) {
        url = replacePramater(url,'sort', sort);
    }
    else {
        url = url + '&sort=' + sort;
    }

    if (url.indexOf('&sortType=') != -1) {
        url = replacePramater(url,'sortType', sorttype);
    }
    else {
        url = url + '&sortType=' + sorttype;
    }


    window.history.pushState("", "", url);

    $('.pagination-bar').remove();
    $.ajax({
        type: 'POST',
        url: UrlContent("/Product/Search"),
        data: data,
        success: function (rs) {
            $('.products').empty();
            $('.products').append(rs.Partial);
            $('.page-description').empty();
            if (rs.Keyword) {
                $('.page-description').append('Showing ' + rs.Count + ' products for <strong>' + rs.Keyword + '</strong>');
            } else
                $('.page-description').append('Showing ' + rs.Count + ' products for all');
        },
        error: function (rs) {
            console.log(rs);
        }
    });
}

function SearchCategory(id) {
    //replace category
    var url = window.location.href;
    url = replacePramater(url, 'cgID', id);
    url = replacePramater(url, 'currentPage', 1);
    var data = {
        Keyword: getPramater('keyWord'),
        BrandID: brandId,
        RangeID: rangeId,
        Sort: getPramater('sort'),
        SortType: getPramater('sortType'),
        CgID: id,
        Filter: true,
        CurrentPage: 1
    }

    window.history.pushState("", "", url);

    $('.pagination-bar').remove();
    $.ajax({
        type: 'POST',
        url: UrlContent("/Product/Search"),
        data: data,
        success: function (rs) {
            $('.products').empty();
            $('.products').append(rs.Partial);
            $('.page-description').empty();
            if (rs.Keyword) {
                $('.page-description').append('Showing ' + rs.Count + ' products for <strong>' + rs.Keyword + '</strong>');
            } else
                $('.page-description').append('Showing ' + rs.Count + ' products for all');
        },
        error: function (rs) {
            console.log(rs);
        }
    });
}

function navigatePage(e, maxPage) {
    var orderby = $('#orderby').val();
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
            currentPage: !isNaN(page) ? page : currentPage,
            orderby: orderby
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

function processAPI() {
    var data = {
        Keyword: oldTerm,
        BrandID: brandId,
        RangeID: rangeId,
        Sort: sort,
        SortType: sorttype,
        CgID: cgId,
        Filter: true,
        CurrentPage: parseInt($('#currentPage').val())
    }
    $('.pagination-bar').remove();
    $.ajax({
        type: 'POST',
        url: UrlContent("/Product/Search"),
        data: data,
        success: function (rs) {
            $('.products').empty();
            $('.products').append(rs.Partial);
            $('.page-description').empty();
            if (rs.Keyword) {
                $('.page-description').append('Showing ' + rs.Count + ' products for <strong>' + rs.Keyword + '</strong>');
            } else
                $('.page-description').append('Showing ' + rs.Count + ' products for all');
        },
        error: function (rs) {
            console.log(rs);
        }
    });
}

function findCategory(array, id) {
    var result;
    for (var i = 0; i < array.length; i++) {
        if (array[i].Category_ID == id) {
            result = array[i];
            break;
        }
    }
    return result;
}

function getPramater(pramater) {
    var url = window.location.href;
    if (url.indexOf('&'+pramater+'=') != -1) {

        var lstPramate = url.split('&');
        for (var i = 0; i < lstPramate.length; i++) {
            if (lstPramate[i].indexOf(pramater+'=') != -1) {
                return lstPramate[i].replace(pramater+'=', '');
            }
        }
    }
    else {
        return '';
    }

}

function replacePramater(url,pramater,value) {
    //replace
    if (url.indexOf('&'+pramater+'=')) {
        var lstPramater = url.split('&');
        for (var i = 0; i < lstPramater.length; i++) {
            if (lstPramater[i].indexOf(pramater+'=') != -1) {
                url = url.replace(lstPramater[i], pramater + '=' + value);
                return url;
            }
        }
    }
    else {
        return '';
    }
}