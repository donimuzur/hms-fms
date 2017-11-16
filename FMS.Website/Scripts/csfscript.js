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
    else if ($('.title-page').html() == 'CSF Personal Dashboard') {
        $('#CsfParent').addClass('active');
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

function selectVehicle(urlFunction) {
    var vehUsage = $('#Detail_VehicleUsage').find("option:selected").text();
    var vehType = $('#Detail_VehicleType').val();
    var vehCat = $('#Detail_VehicleCat').find("option:selected").text();
    var groupLevel = $('#Detail_GroupLevel').val();
    var createdDate = $('#Detail_CreateDate').val();

    if ($('#Detail_VehicleUsage').find("option:selected").val() == "") {
        $('#tb-body-select-veh').html("");
        $('#tb-body-select-veh').append('<tr><td style="text-align:center" colspan="9">no data<td></tr>');
    }
    else {
        $.ajax({
            type: 'POST',
            url: urlFunction,
            data: { vehUsage: vehUsage, vehType: vehType, vehCat: vehCat, groupLevel: groupLevel, createdDate: createdDate },
            success: function (data) {
                if (data.length > 0) {
                    $('#tb-body-select-veh').html("");
                    for (var i = 0; i < data.length; i++) {
                        var tableData = '<tr>' +
                            '<td><input name="selectvehicleradio" id="selectvehicleradio_' + i + '" type="radio" value="'+ i +'"></td>' +
                            '<td><input type="hidden" name="manufacturer" id="Detail_Manufacturer_' + i + '" value=' + data[i].Manufacturer + '></input>' + data[i].Manufacturer + '</td>' +
                            '<td><input type="hidden" name="model" id="Detail_Models_' + i + '" value=' + data[i].Models + '></input>' + data[i].Models + '</td>' +
                            '<td><input type="hidden" name="series" id="Detail_Series_' + i + '" value=' + data[i].Series + '></input>' + data[i].Series + '</td>' +
                            '<td><input type="hidden" name="bodytype" id="Detail_BodyType_' + i + '" value=' + data[i].BodyType + '></input>' + data[i].BodyType + '</td>' +
                            '<td><input type="hidden" name="vendorname" id="Detail_VendorName_' + i + '" value=' + data[i].VendorName + '></input>' + data[i].VendorName + '</td>' +
                            '<td><input type="hidden" name="color" id="Detail_Color_' + i + '" value=' + data[i].Color + '></input>' + data[i].Color + '</td>' +
                            '</tr>';
                        $('#tb-body-select-veh').append(tableData);
                    }
                } else {
                    $('#tb-body-select-veh').html("");
                    $('#tb-body-select-veh').append('<tr><td style="text-align:center" colspan="9">no data<td></tr>');
                }
            }
        });
    }
}