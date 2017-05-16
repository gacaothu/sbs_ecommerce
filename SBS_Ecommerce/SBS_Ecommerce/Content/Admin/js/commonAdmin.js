function showMessageAlert(value, textMsg) {
    var url = window.location.href;
    if (url.indexOf('msg=') != -1 && url.indexOf('textMsg=') != -1)
    {
      
        var listParam = url.split('&');
        url = url.replace(listParam[1], 'msg=' + value);
        url = url.replace(listParam[2], 'textMsg=' + textMsg);
    }
    else
    {
        url = url + '?&msg=' + value +"&textMsg=" + textMsg;
    }

    window.location.href = url;
}