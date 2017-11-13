$(document).ready(function () {
    $('#CtfMenu').removeClass('collapse');
    $('#CtfDashboard').removeClass('active');
    $('#CtfOpen').removeClass('active');
    $('#CtfCompleted').removeClass('active');

    if ($('.title-page').html() == 'CTF Dashboard') {
        $('#CtfDashboard').addClass('active');
    }
    else if ($('.title-page').html() == 'CTF Open Document WTC' || $('.title-page').html() == 'CTF Open Document Benefit' || $('.title-page').html() == 'CTF Open Document' || $('.title-page').html() == 'Car Termination Form Benefit' || $('.title-page').html() == 'Car Termination Form WTC') {
        $('#CtfOpen').addClass('active');
    }
    else if ($('.title-page').html() == 'CTF Completed Document WTC' || $('.title-page').html() == 'CTF Completed Document Benefit' || $('.title-page').html() == 'CTF Completed Document') {
        $('#CtfCompleted').addClass('active');
    }
});

function ValidateInput() {
    var result = true;

    if ($('#Detail_EmployeeId').val() == '') {
        AddValidationClass(false, 'Detail_EmployeeId');
        result = false;
    }

    if ($('#Detail_ReasonId').val() == '') {
        AddValidationClass(false, 'Detail_ReasonId');
        result = false;
    }

    if ($('#Detail_EffectiveDate').val() == '') {
        AddValidationClass(false, 'Detail_EffectiveDate');
        result = false;
    }

    return result;
}

function AddValidationClass(isValid, objName) {
    if (isValid) {
        $('#' + objName).removeClass('input-validation-error');
        $('#' + objName).addClass('valid');
    } else {
        $('#' + objName).removeClass('valid');
        $('#' + objName).addClass('input-validation-error');
    }
}