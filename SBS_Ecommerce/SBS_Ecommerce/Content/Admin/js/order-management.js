var currentTab = 10;

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
                case 10:
                    $('#processBtn').remove();
                    $('.modal-body').append('<button id="processBtn" class="btn-success btn">Move to Process</button>');
                    $('#processBtn').attr('data-id', id);
                    break;
                // Processing
                case 20:
                    $('#processBtn').remove();
                    $('.modal-body').append('<button id="processBtn" class="btn-success btn">Move to Complete</button>');
                    $('#processBtn').attr('data-id', id);
                    break;
                // Completed
                case 30:
                    $('#processBtn').remove();
                    break;
                // Canceled
                case 40:
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
}

function searchOrder() {

}