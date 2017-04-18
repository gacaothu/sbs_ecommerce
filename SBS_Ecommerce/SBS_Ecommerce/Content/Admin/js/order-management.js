var PENDING = 10;
var PROCESSING = 20;
var COMPLETED = 40;
var CANCELED = 50;
var currentTab = 10;

$(function () {
    $('.specific-day').css('display', 'none');
    $('.filter-status').css('display', 'none');
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
            
            switch (currentTab) {
                // Pending
                case PENDING:
                    $('#processBtn').remove();
                    $('.modal-body').append('<button id="processBtn" class="btn-success btn">Move to Process</button>');
                    $('#processBtn').attr('data-id', id);
                    break;
                // Processing
                case PROCESSING:
                    $('#processBtn').remove();
                    $('.modal-body').append('<button id="processBtn" class="btn-success btn">Move to Complete</button>');
                    $('#processBtn').attr('data-id', id);
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
            refreshTab();
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
    var data = { sortByDate: sortDate };
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
    if (currentTab == PROCESSING) {
        var status = $('#filter-status').val();
        data['status'] = status;
    }

    console.log(data);
    $.ajax({
        url: UrlContent('/AdminOrderMgmt/FilterOrder'),
        data: data,
        success: function (rs) {
            $('#pending').empty();
            $('#pending').append(rs.Pending);

            $('#processing').empty();
            $('#processing').append(rs.Processing);

            $('#completed').empty();
            $('#completed').append(rs.Completed);

            $('#canceled').empty();
            $('#canceled').append(rs.Completed);
        },
        error: function (rs) {
            console.log(rs)
        }
    });
});

function refreshTab() {
    $.ajax({
        url: UrlContent('/AdminOrderMgmt/RefreshTab'),
        data: {},
        success: function (rs) {
            $('#pending').empty();
            $('#pending').append(rs.Pending);

            $('#processing').empty();
            $('#processing').append(rs.Processing);

            $('#completed').empty();
            $('#completed').append(rs.Completed);
            
            $('#orderDetailModal').modal('toggle');
        },
        error: function (rs) {
            console.log(rs);
        }
    });
}

function onTabChange(e) {
    currentTab = parseInt($(e).attr('data-id'));
    console.log();
    if ($(e).text() == "Processing") {
        $('.filter-status').css('display', '');
    } else {
        $('.filter-status').css('display', 'none');
    }
}

function searchOrder() {

}