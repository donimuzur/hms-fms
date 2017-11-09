$(document).ready(function () {
    $('#CsfMenu').removeClass('collapse');
    $('#CsfDashboard').removeClass('active');
    $('#CsfOpen').removeClass('active');
    $('#CsfCompleted').removeClass('active');

    if ($('.title-page').html() == 'CSF Dashboard') {
        $('#CsfDashboard').addClass('active');
    }
    else if ($('.title-page').html() == 'CSF Open Document') {
        $('#CsfOpen').addClass('active');
    }
    else if ($('.title-page').html() == 'CSF Completed Document') {
        $('#CsfCompleted').addClass('active');
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