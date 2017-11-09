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