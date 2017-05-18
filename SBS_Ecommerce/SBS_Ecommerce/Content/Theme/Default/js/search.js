var oldTerm = null;
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
        if (childCategories != null && childCategories.Items != null && childCategories.Items.length>0) {
            $('#categories').find('.menu').empty();
            for (var i = 0; i < childCategories.Items.length; i++) {
                $('#categories').find('.menu').append('<li class="col-sm-3"><a style="cursor:pointer;" onclick="searchProduct(this)" data-id=' + childCategories.Items[i].Category_ID + '>' + childCategories.Items[i].Category_Name + '</a></li>');
            }
        }
        //change color
        $('#categories').find('.menu').find('.col-sm-3').each(function (rs) {
            $(this).css('background', 'none');
            //alert($(this).find('a').attr('data-id'));
            if ($(this).find('a').attr('data-id') == cgId) {
                $(this).css('background', '#4bcd9f');
            }

        });

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
                var price = "";
                if (lstProducts[i].Promotion_Price>0) {
                    price = lstProducts[i].Promotion_Price;
                }
                else {
                    price = lstProducts[i].Selling_Price;
                }
                $('#product-result-search').append('<div onclick="SearchProductIntel(' + "\'" + lstProducts[i].Product_ID + "\'" + ')" style="postion:relative;float:left;width:100%" class="item-product-result"><span style="width:55px;height:55px;position:relative;float:left;margin-right:5px;"><img src="' + domain + lstProducts[i].Small_Img + '" style="width:100%;height:100%" /></span><span>' +
                    lstProducts[i].Product_Name + '</span><span style="color:#e26f47;margin-left:5px;">$' + price + '</span></div>');
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
    var brandId = '';
    $('#brand').find('.input-rule').each(function () {
        if ($(this).attr('class').indexOf('selected') >= 0) {
            //alert($(this).children('input').attr('data-id'));
            brandId = brandId + "_" + ($(this).children('input').attr('data-id'));
        }
    });
  
    if (brandId.length > 0) {
        brandId = brandId.slice(1);
        url = window.location.href;
        if (url.indexOf('&lstBrandID=') == -1) {
            url = url + "&lstBrandID=" + brandId;
            
        }
        else {
            url = replacePramater(url, 'lstBrandID', brandId);
        }

        window.history.pushState("", "", url);
        processAPI();
    }
    else {
        url = replacePramater(url, 'lstBrandID', brandId);
        window.history.pushState("", "", url);
        processAPI();
    }
    
});

$(document).on('change', '#price .input-rule', function () {
    var priceId = '';
    $('#price').find('.input-rule').each(function () {
        if ($(this).attr('class').indexOf('selected') >= 0) {
            //alert($(this).children('input').attr('data-id'));
            priceId = priceId + "_" + ($(this).children('input').attr('data-id'));
        }
    });

    if (priceId.length > 0) {
        priceId = priceId.slice(1);
        url = window.location.href;
        if (url.indexOf('&lstRangeID=') == -1) {
            url = url + "&lstRangeID=" + priceId;

        }
        else {
            url = replacePramater(url, 'lstRangeID', priceId);
        }

        window.history.pushState("", "", url);
        processAPI();
    }
    else {
        url = replacePramater(url, 'lstRangeID', brandId);
        window.history.pushState("", "", url);
        processAPI();
    }
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
    url = clearPramater(url, 'lstBrandID');
    url = clearPramater(url, 'lstRangeID');
    window.history.pushState("", "", url);

    //Clear checkbox 
    $('.input-rule').each(function () {
        if ($(this).attr('class').indexOf('selected') >= 0) {
            $(this).removeClass('selected');
            $(this).find('input').removeAttr('checked');
        }
    })


    
    processAPI();
}

function navigatePage(e, currentpage) {
    var url = window.location.href;
    url = replacePramater(url, 'currentPage', currentpage);
    window.history.pushState("", "", url);
    processAPI();
    //window.location.href = url;
}

function processAPI() {
    var data = {
        keyWord: getPramater('keyWord'),
        lstBrandID: getPramater('lstBrandID'),
        lstRangeID: getPramater('lstRangeID'),
        sort: getPramater('sort'),
        sortType: getPramater('sortType'),
        cgID: getPramater('cgID'),
        filter: true,
        CurrentPage: getPramater('currentPage')
    }
   
    $.ajax({
        type: 'POST',
        url: UrlContent("/Product/Search"),
        data: data,
        success: function (rs) {
            $('.products').empty();
            $('.pagination-bar').remove();
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
    if (url.indexOf('&'+pramater+'=')!=-1) {
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

function clearPramater(url, pramater) {
    if (url.indexOf('&' + pramater + '=')!=-1) {
       
        var lstPramater = url.split('&');
        for (var i = 0; i < lstPramater.length; i++) {
            if (lstPramater[i].indexOf(pramater + '=') != -1) {
                url = url.replace('&'+lstPramater[i], '');
                return url;
            }
        }
    }
    else {
        
        return url;
    }
}