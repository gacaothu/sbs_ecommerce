var PENDING = 10;
var PROCESSING = 20;
var COMPLETED = 40;
var CANCELED = 50;

$(function () {
    $('.specific-day').css('display', 'none');
});

$(document).on('click', '.clickable-row', function (e) {
    var id = $(this).attr('data-id');
    $.ajax({
        url: UrlContent('/AdminOrderMgmt/OrderDetail'),
        data: {
            id: $(this).attr('data-id')
        },
        success: function (rs) {
            $('#orderDetailModal').find('.content-block').html(rs.Partial);
            var status = parseInt(getUrlParam('kind'));
            switch (status) {
                // Pending
                case PENDING:
                    $('#processBtn').remove();
                    $('.modal-body').append('<button id="processBtn" class="btn-success btn" data-id="' + id + '">Move to Process</button>');
                    break;
                    // Processing
                case PROCESSING:
                    $('#processBtn').remove();
                    $('.modal-body').append('<button id="processBtn" class="btn-success btn" data-id="' + id + '">Move to Complete</button>');
                    break;
                    // Completed
                case COMPLETED:
                    $('#processBtn').remove();
                    break;
                    // Canceled
                case CANCELED:
                    $('#processBtn').remove();
                    break;
                default:
                    break;
            }
            $('#orderDetailModal').modal('show');
        },
        error: function (rs) {
            console.log(rs);
        }
    });
});

$(document).on('click', '#processBtn', function () {
    var id = $(this).attr('data-id');
    $.ajax({
        type: 'POST',
        url: UrlContent('/AdminOrderMgmt/UpdateStatus'),
        data: {
            id: id
        },
        success: function (rs) {
            location.reload();
        },
        error: function (rs) {
            console.log(rs);
        }
    });
});

$(document).on('change', '#filter-date', function () {
    if ($('#filter-date').val().indexOf("spec") >= 0) {
        $('.specific-day').css('display', '');
    } else {
        $('.specific-day').css('display', 'none');
    }
});

$(document).on('click', '#filter-btn', function () {
    var sortDate = $('#filter-date').val();
    var data = {
        kind: parseInt(getUrlParam('kind')),
        sortByDate: sortDate,
    };
    if (sortDate == 'spec') {
        var dateFrom = $('#dateFrom').val();
        var dateTo = $('#dateTo').val();
        if (dateFrom) {
            data['dateFrom'] = dateFrom;
        }
        if (dateTo) {
            data['dateTo'] = dateTo;
        }
    }
    if ($('#filter-status').val())
        data['status'] = $('#filter-status').val();

    console.log(data);
    $.ajax({
        url: UrlContent('/AdminOrderMgmt/FilterOrder'),
        data: data,
        success: function (rs) {
            $('.tab-content').empty();
            $('.tab-content').append(rs.Partial);
        },
        error: function (rs) {
            console.log(rs)
        }
    });
});

function getUrlParam(param) {
    var result;
    var sPageURL = window.location.search.substring(1);
    var sURLVariables = sPageURL.split('&');
    for (var i = 0; i < sURLVariables.length; i++) {
        var sParameterName = sURLVariables[i].split('=');
        if (sParameterName[0] == param) {
            result = sParameterName[1];
        }
    }
    return result;
}