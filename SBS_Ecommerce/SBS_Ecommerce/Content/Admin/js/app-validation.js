function validateRequired(id, field, require) {
    if (id.indexOf('#') < 0) {
        id = '#' + id;
    }

    var value = $(id).val();
    if ($(id + '_Err').get(0) != null) {
        $(id + '_Err').get(0).setAttribute("class", "field-validation-error");
        if ($(id + '_Err').get(0).childNodes.length > 0) $(id + '_Err').get(0).removeChild($(id + '_Err').get(0).childNodes[0]);

        if (require == true) {
            if (isEmpty(value)) {

                var spanErr = document.createElement("span");
                spanErr.textContent = "The " + field + " field is required.";
                $(id + '_Err').get(0).appendChild(spanErr);

                return false;
            }
            return true;
        }
    }
    return true;
}

function validateSelectRequired(id, field, require) {
    if (id.indexOf('#') < 0) {
        id = '#' + id;
    }

    var value = $(id).val();
    if ($(id + '_Err').get(0) != null) {
        $(id + '_Err').get(0).setAttribute("class", "field-validation-error");
        if ($(id + '_Err').get(0).childNodes.length > 0) $(id + '_Err').get(0).removeChild($(id + '_Err').get(0).childNodes[0]);

        if (require == true) {
            if (value.indexOf('--') >= 0) {

                var spanErr = document.createElement("span");
                spanErr.textContent = "The " + field + " field is required.";
                $(id + '_Err').get(0).appendChild(spanErr);

                return false;
            }
            return true;
        }
    }
    return true;
}

function validateEmail(id, filed_name, require) {
    if (id.indexOf('#') < 0) {
        id = '#' + id;
    }
    var isvalid = false;
    var value = $(id).val();

    if ($(id + '_Err').get(0) != null) {
        $(id + '_Err').get(0).setAttribute("class", "field-validation-valid");
        if ($(id + '_Err').get(0).childNodes.length > 0) $(id + '_Err').get(0).removeChild($(id + '_Err').get(0).childNodes[0]);

        if (require == true) {
            if (isEmpty(value)) {
                $(id + '_Err').get(0).setAttribute("class", "field-validation-error");

                var spanErr = document.createElement("span");
                spanErr.textContent = "The " + filed_name + " field is required.";
                $(id + '_Err').get(0).appendChild(spanErr);

                mandatoryColor(id);
                isvalid = false;
                return isvalid;
            }
            isvalid = true;
        } else if (require == false) {
            if (isEmpty(value)) {
                errorRemoveColor(id);
                isvalid = true;
                return isvalid;
            }
            isvalid = true;
        }

        if (isvalid) {
            var re = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i;

            if (re.test(value) == false) {
                $(id + '_Err').get(0).setAttribute("class", "field-validation-error");

                var spanErr = document.createElement("span");
                spanErr.textContent = "The value '" + value + "' is not a valid email.";
                $(id + '_Err').get(0).appendChild(spanErr);

                isvalid = false;
            } else {
                mandatoryRemoveColor(id);
                errorRemoveColor(id);
            }
        }
    }

    return isvalid;
}

function validateComparision(id, field, compareid, comparefield, require) {
    if (id.indexOf('#') < 0) {
        id = '#' + id;
    }

    var isvalid = false;
    var value = $(id).val();
    if ($(id + '_Err').get(0) != null) {
        $(id + '_Err').get(0).setAttribute("class", "field-validation-error");
        if ($(id + '_Err').get(0).childNodes.length > 0) $(id + '_Err').get(0).removeChild($(id + '_Err').get(0).childNodes[0]);

        if (require == true) {
            if (isEmpty(value)) {

                var spanErr = document.createElement("span");
                spanErr.textContent = "The " + field + " field is required.";
                $(id + '_Err').get(0).appendChild(spanErr);

                isvalid = false;
            }
            isvalid = true;
        } else {
            if (isEmpty(value)) {
                errorRemoveColor(id);
                isvalid = true;
                return isvalid;
            }
            isvalid = true;
        }

        if (isvalid) {
            if (compareid.indexOf('#') < 0) {
                compareid = '#' + compareid;
            }
            
            var compare = $(compareid).val().replace(':','');
            value = value.replace(':', '');
            var result = parseInt(value) - parseInt(compare);
            if (result <= 0) {
                $(id + '_Err').get(0).setAttribute("class", "field-validation-error");

                var spanErr = document.createElement("span");
                spanErr.textContent = "The " + field + " must be larger than The " + comparefield;
                $(id + '_Err').get(0).appendChild(spanErr);

                isvalid = false;
            } else {
                mandatoryRemoveColor(id);
                errorRemoveColor(id);
            }
        }
    }
    return true;
}

function validateNum(id, filed_name, require, allowZero, minval, maxval, digi) {
    if (id.indexOf('#') < 0)
        id = '#' + id;

    var value = $(id).val();
    if ($(id + '_Err').get(0) != null) {
        clearError(id);

        if (require == true) {
            if (allowZero == true) {
                if (parseAmt(value) == 0) {
                    $(id).val(0);
                    //return true;
                }
            }
            else if (isEmpty(value)) {
                displayError(id, filed_name, value, 1);
                return false;
            }
        }
        else {
            if (allowZero == true) {
                if (parseAmt(value) == 0) {
                    $(id).val(0);
                    //return true;
                }
            }
            else if (parseAmt(value) == 0) {
                $(id).val('');
                //return true;
            }
        }

        if (digi != null && digi >= 0)
            $(id).val(parseAmt(parseInt(value).toFixed(digi)));
        else
            $(id).val(parseAmt(value));

        if (maxval != null && maxval > 0) {
            if (parseAmt(value) > parseAmt(maxval))
                $(id).val(parseAmt(maxval));
        }

        if (minval != null && minval > 0) {
            if (parseAmt(value) < parseAmt(minval))
                $(id).val(parseAmt(minval));
        }
    }
    return true;
}

function validateNumComparision(id, field, compareid, comparefield, require) {
    if (id.indexOf('#') < 0) {
        id = '#' + id;
    }

    var isvalid = false;
    var value = $(id).val();
    if ($(id + '_Err').get(0) != null) {
        isvalid = validateNum(id, field, true);

        if (isvalid) {
            if (compareid.indexOf('#') < 0) {
                compareid = '#' + compareid;
            }
            
            var compare = $(compareid).val();
            var result = parseFloat(value) - parseFloat(compare);
            if (result <= 0) {
                $(id + '_Err').get(0).setAttribute("class", "field-validation-error");

                var spanErr = document.createElement("span");
                spanErr.textContent = "The " + field + " must be larger than The " + comparefield;
                $(id + '_Err').get(0).appendChild(spanErr);

                isvalid = false;
            } else {
                mandatoryRemoveColor(id);
                errorRemoveColor(id);
            }
        }
        return isvalid;
    }
    return true;
}

function displayError(id, filed_name, value, errtype) {
    var errid = id + '_Err';
    $(errid).get(0).setAttribute("class", "field-validation-error");

    var spanErr = document.createElement("span");
    if (errtype == 1)//requried
    {
        spanErr.textContent = "The " + filed_name + " field is required.";
        $(errid).get(0).appendChild(spanErr);
        mandatoryColor(id);
    }
    else if (errtype == 2)//invalid
    {
        spanErr.textContent = "The value '" + value + "' is not valid for " + filed_name + " .";
        errorColor(id);
    }
}
function clearError(id) {
    if (id.indexOf('#') < 0) {
        id = '#' + id;
    }
    var errid = id + '_Err';
    $(errid).get(0).setAttribute("class", "field-validation-valid");

    if ($(errid).get(0).childNodes.length > 0)
        $(errid).get(0).removeChild($(errid).get(0).childNodes[0]);
}

function errorRemoveColor(cid) {
    if (cid.indexOf('#') < 0) {
        cid = '#' + cid;
    }
    if ($(cid).get(0) != null) {
        $(cid + '_chosen').removeClass('input-validation-error');
        $(cid).removeClass('input-validation-error');
    }
}

function mandatoryColor(cid) {
    if (cid.indexOf('#') < 0) {
        cid = '#' + cid;
    }
    if ($(cid).get(0) != null) {
        if ($(cid).get(0).hasAttribute('readonly') == false & $(cid).get(0).hasAttribute('disabled') == false) {
            $(cid + '_chosen').addClass('mandatory-validation');
            $(cid).addClass('mandatory-validation');
        }

    }
}

function mandatoryRemoveColor(cid) {
    if (cid.indexOf('#') < 0) {
        cid = '#' + cid;
    }
    if ($(cid).get(0) != null) {
        $(cid + '_chosen').removeClass('mandatory-validation');
        $(cid).removeClass('mandatory-validation');
    }
}

function isEmpty(input) {
    if (input == null || input == '' || input == undefined) {
        return true;
    }
    return false;
}

function parseAmt(amt) {
    amt = parseFloat(amt);
    if (isNaN(amt))
        amt = 0;
    return parseFloat(amt.toFixed(2));
}