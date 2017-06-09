function msg_control(msgcode, msg) {
    var strhtml = "";
    if (msgcode > 0) {
        strhtml = '<div role="alert" class="alert alert-success alert-dismissible">'
                + '<button type="button" data-dismiss="alert" aria-label="Close" class="close"><span aria-hidden="true" class="s7-close"></span></button>'
                + '<span id="alert-msg-success">' + msg + '</span></div>';
    } else {
        strhtml = '<div role="alert" class="alert alert-primary alert-dismissible">'
               + '<button type="button" data-dismiss="alert" aria-label="Close" class="close"><span aria-hidden="true" class="s7-close"></span></button>'
               + '<span id="alert-msg-error">' + msg + '</span></div>';
    }
    $('#alert-msg').html(strhtml);
}

function msg_clear() {
    $('#alert-msg').html('');
}