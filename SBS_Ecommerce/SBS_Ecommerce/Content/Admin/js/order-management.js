﻿$(function () {
    $("#order-table").dataTable();
    $(".datetimepicker").datetimepicker({
        autoclose: true,
        componentIcon: '.s7-date',
        navIcons: {
            rightIcon: 's7-angle-right',
            leftIcon: 's7-angle-left'
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
