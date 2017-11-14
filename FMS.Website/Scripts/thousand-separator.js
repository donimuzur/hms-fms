
$('.decimal').attr('onkeyup', 'this.value=ThausandSeperator($(this).val(), 2)');

$('.decimalMaxFour').attr('onkeyup', 'this.value=ThausandSeparatorMaxFour($(this).val(), 5)');
    
    
function removeCharacter(v, ch) {
    var tempValue = v + "";
    var becontinue = true;
    while (becontinue == true) {
        var point = tempValue.indexOf(ch);
        if (point >= 0) {
            var myLen = tempValue.length;
            tempValue = tempValue.substr(0, point) + tempValue.substr(point + 1, myLen);
            becontinue = true;
        } else {
            becontinue = false;
        }
    }
    return tempValue;
}

/*
*  using by function ThausandSeperator!!
*/
function characterControl(value) {
    var tempValue = "";
    var len = value.length;
    for (i = 0; i < len; i++) {
        var chr = value.substr(i, 1);
        if ((chr < '0' || chr > '9') && chr != '.' && chr != ',' && chr != '-') {
            chr = '';
        }
        else if (chr == '-' && i > 0)
            chr = '';
        tempValue = tempValue + chr;
    }
    return tempValue;
}


function ThausandSeperator(value, digit) {
    var thausandSepCh = ",";
    var decimalSepCh = ".";

    var tempValue = "";
    var realValue = value + "";
    var devValue = "";
    realValue = characterControl(realValue);
    var comma = realValue.indexOf(decimalSepCh);
    if (comma != -1) {
        tempValue = realValue.substr(0, comma);
        devValue = realValue.substr(comma);
        devValue = removeCharacter(devValue, thausandSepCh);
        devValue = removeCharacter(devValue, decimalSepCh);
        devValue = decimalSepCh + devValue;
        if (devValue.length > 3) {
            devValue = devValue.substr(0, 3);

        }
    } else {
        tempValue = realValue;
    }
    tempValue = removeCharacter(tempValue, thausandSepCh);

    var result = "";
    var len = tempValue.length;
    var isMinus = false;
    
    if (len > 3) {
        if (tempValue.substr(0, 1) == '-') {
            len = len - 1;
            tempValue = tempValue.substr(1, len);
            isMinus = true;
        }
    }
    while (len > 3) {
        result = thausandSepCh + tempValue.substr(len - 3, 3) + result;
        len -= 3;
    }
    result = tempValue.substr(0, len) + result;
    if (isMinus)
        result = "-" + result;
    
    return result + devValue;
}


function allCharsToUpper(value) {
    var tempValue = "";
    var len = value.length;
    for (i = 0; i < len; i++) {
        var chr = value.substr(i, 1);
        chr = chr.toUpperCase();
        if (chr == 'İ') { chr = 'I'; }
        else if (chr == 'Ğ') { chr = 'G'; }
        else if (chr == 'Ö') { chr = 'O'; }
        else if (chr == 'Ü') { chr = 'U'; }
        else if (chr == 'Ş') { chr = 'S'; }
        else if (chr == 'Ç') { chr = 'C'; }

        tempValue = tempValue + chr;
    }
    return tempValue;
}

function ThausandSeparatorMaxFour(value, digit) {
    var thausandSepCh = ",";
    var decimalSepCh = ".";

    var tempValue = "";
    var realValue = value + "";
    var devValue = "";
    realValue = characterControl(realValue);
    var comma = realValue.indexOf(decimalSepCh);
    if (comma != -1) {
        tempValue = realValue.substr(0, comma);
        devValue = realValue.substr(comma);
        devValue = removeCharacter(devValue, thausandSepCh);
        devValue = removeCharacter(devValue, decimalSepCh);
        devValue = decimalSepCh + devValue;
        if (devValue.length > digit) {
            devValue = devValue.substr(0, digit);
        }
    } else {
        tempValue = realValue;
    }
    tempValue = removeCharacter(tempValue, thausandSepCh);

    var result = "";
    var len = tempValue.length;
    while (len > 3) {
        result = thausandSepCh + tempValue.substr(len - 3, 3) + result;
        len -= 3;
    }
    result = tempValue.substr(0, len) + result;
    return result + devValue;
}
