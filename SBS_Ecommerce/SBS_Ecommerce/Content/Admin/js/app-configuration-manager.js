var url = window.location.href;
$(function () {
    checkAlertMessageDisplay('.main-content');
});
function SaveConfig() {
    msg_clear();

    if ($("#txtPageID").val().trim() == "") {
        $("#error-fbID").show();
        return;
    }
    else {
        $("#error-fbID").hide();
    }
    $.ajax({
        url: '@Url.Action("SaveConfigChatting","Admin")',
        data: { pageID: $('#txtPageID').val() },
        success: function (rs) {
            showAlertFromResponse(rs);
        }
    });
}

function SaveConfigMailChimp() {
    if ($("#txtApiKey").val().trim() == "") {
        $("#error-txtApiKey").show();
        return;
    }
    else {
        $("#error-txtApiKey").hide();
    }

    $.ajax({
        url: '@Url.Action("SaveConfigMailChimp", "Admin")',
        data: { apiKey: $('#txtApiKey').val() },
        success: function (rs) {
            showAlertFromResponse(rs);
        }
    });
}


function ConfigPaypal() {
    msg_clear();

    var mod = '';
    if ($('#Mode-sanbox').prop('checked')) {
        mod = 'sanbox';
    }
    else {
        mod = 'live';
    }
    var conectionTimeout = $('#ConnectionTimeout').val();
    var clientID = $('#ClientID').val();
    var clientSecret = $('#ClientSecret').val();
    var id = $('#ConfigPaypalId').val();

    if (conectionTimeout.trim() == '') {
        $('#error-timeout').show();
        return;
    }
    else {
        $('#error-timeout').hide();
    }

    if (clientID.trim() == '') {
        $('#error-clientID').show();
        return;
    }
    else {
        $('#error-clientID').hide();
    }

    if (clientSecret == '') {
        $('#error-ClientSecret').show();
        return;
    }
    else {
        $('error-ClientSecret').hide();
    }

    $.ajax({
        url: UrlContent("/Admin/ConfigPaypal"),
        type: 'POST',
        data: { Id: id, Mode: mod, ConnectionTimeout: conectionTimeout, ClientId: clientID, ClientSecret: clientSecret },
        success: function (rs) {
            showAlertFromResponse(rs);
        }
    });
}

function ConfigEmail() {
    msg_clear();

    var id = $('#configEmailID').val();
    var email = $('#ConfigEmail').val();
    var displayName = $('#configDisplayName').val();
    var host = $('#configHost').val();
    var port = $('#configPort').val();
    var userName = $('#configUserName').val();
    var password = $('#configPassword').val();
    var ssl = $('#ckSSL').prop('checked');
    var credentials = $('#ckCredentials').prop('checked');

    if (email.trim() == '') {
        $('#error-ConfigEmail').show();
        return;
    }
    else {
        $('#error-ConfigEmail').hide();
    }

    if (displayName.trim() == '') {
        $('#error-DisplayName').show();
        return;
    }
    else {
        $('#error-DisplayName').hide();
    }

    if (host.trim() == '') {
        $('#error-configHost').show();
        return;
    }
    else {
        $('#error-configHost').hide();
    }

    if (port.trim() == '') {
        $('#error-configPort').show();
        return;
    }
    else {
        $('#error-configPort').hide();
    }

    if (userName.trim() == '') {
        $('#error-configUserName').show();
        return;
    }
    else {
        $('#error-configUserName').hide();
    }

    if (password.trim() == '') {
        $('#error-configPassword').show();
        return;
    }
    else {
        $('#error-configPassword').hide();
    }

    $.ajax({
        url: UrlContent("/Admin/ConfigEmail"),
        type: 'POST',
        data: { Id: id, Email: email, DisplayName: displayName, Host: host, Port: port, Username: userName, Password: password, EnableSsl: ssl, UseDefaultCredentials: credentials },
        success: function (rs) {
            showAlertFromResponse(rs);
        }
    });
}