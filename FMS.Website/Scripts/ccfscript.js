$(document).ready(function () {
    $('#CCFMenu').removeClass('collapse');
    $('#DashboardEpaf').removeClass('active');
    $('#CCFOpenBenefit').removeClass('active');
    $('#CCFOpenWTC').removeClass('active');
    $('#CCFCompleted').removeClass('active');
    
    if ($('.title-page').html() == 'Dashboard') {
        $('#DashboardEpaf').addClass('active');
    }
    else if ($('.title-page').html() == 'CCF Open Document Benefit') {
        $('#CCFOpenBenefit').addClass('active');
    }
    //else if ($('.title-page').html() == 'Car Termination Form Benefit') {
    //    $('#CCFOpenBenefit').addClass('active');
    //}
    else if ($('.title-page').html() == 'CCF Open Document WTC') {
        $('#CCFOpenWTC').addClass('active');
    }
    //else if ($('.title-page').html() == 'Car Termination Form WTC')
    //{
    //    $('#CCFOpenWTC').addClass('active');
    //}
    else if ($('.title-page').html() == 'CCF Completed Document') {
        $('#CCFCompleted').addClass('active');
    }
    else if ($('.title-page').html() == 'CCF Personal Dashboard') {
        $('#CCFParent').addClass('active');
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