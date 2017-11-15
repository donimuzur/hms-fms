function selectVehicle(urlFunction) {
    var vehUsage = $('#Detail_VehicleUsage').find("option:selected").text();

    if ($('#Detail_VehicleUsage').find("option:selected").val() == "") {
        $('#tb-body-select-veh').html("");
        $('#tb-body-select-veh').append('<tr><td style="text-align:center" colspan="8">no data<td></tr>');
    }
    else {
        $.ajax({
            type: 'POST',
            url: urlFunction,
            data: { vehUsage: vehUsage },
            success: function (data) {
                if (data.length > 0) {
                    $('#tb-body-select-veh').html("");
                    for (var i = 0; i < data.length; i++) {
                        var tableData = '<tr>' +
                            '<td><input name="selectvehicleradio" id="selectvehicleradio[' + i + ']" type="radio"></td>' +
                            '<td><input type="hidden" name="manufacturer" id="Detail_VehicleData[' + i + ']_PoliceNumber" value=' + data[i].PoliceNumber + '></input>' + data[i].PoliceNumber + '</td>' +
                            '<td><input type="hidden" name="manufacturer" id="Detail_VehicleData[' + i + ']_Manufacturer" value=' + data[i].Manufacturer + '></input>' + data[i].Manufacturer + '</td>' +
                            '<td><input type="hidden" name="model" id="Detail_VehicleData[' + i + ']_Models" value=' + data[i].Models + '></input>' + data[i].Models + '</td>' +
                            '<td><input type="hidden" name="series" id="Detail_VehicleData[' + i + ']_Series" value=' + data[i].Series + '></input>' + data[i].Series + '</td>' +
                            '<td><input type="hidden" name="bodytype" id="Detail_VehicleData[' + i + ']_BodyType" value=' + data[i].BodyType + '></input>' + data[i].BodyType + '</td>' +
                            '<td><input type="hidden" name="vendorname" id="Detail_VehicleData[' + i + ']_VendorName" value=' + data[i].VendorName + '></input>' + data[i].VendorName + '</td>' +
                            
                            '<td><input type="hidden" name="startdate" id="Detail_VehicleData[' + i + ']_StartDate" value=' + data[i].StartDate + '></input>' + data[i].StartDate + '</td>' +
                            '<td><input type="hidden" name="enddate" id="Detail_VehicleData[' + i + ']_EndDate" value=' + data[i].EndDate + '></input>' + data[i].EndDate + '</td>' +
                            '</tr>';
                        $('#tb-body-select-veh').append(tableData);
                    }
                } else {
                    $('#tb-body-select-veh').html("");
                    $('#tb-body-select-veh').append('<tr><td style="text-align:center" colspan="8">no data<td></tr>');
                }
            }
        });
    }
}



function fillDropdownFromAjax(url, data, dropdown) {
    $.ajax({
        type: 'POST',
        url: url,
        data: data,
        success: function (res) {
            $(dropdown).html("<option >---select---</option>");
            debugger;
            if (res.length > 0) {
                $(dropdown).html("<option >---select---</option>");
                for (var i = 0; i < res.length; i++) {
                    var tableData = "<option value='" + res[i].Value + "'>" + res[i].Text + "</option>";
                    $(dropdown).append(tableData);
                }
            } 
        }
    });
}


function GetEmployee(urlGet,obj) {

    var Id = $(obj).val();
    $(".vehicle").val("");
    $.ajax({
        url: urlGet,
        type: "POST",
        dataType: "JSON",
        data: { Id: Id },
        success: function (response) {
            $("[name='Detail.EmployeeId']").val(Id);
            $("[name='Detail.EmployeeName']").val(response.FORMAL_NAME);
            $("[name='Detail.CostCenter']").val(response.COST_CENTER);
            $("[name='Detail.GroupLevel']").val(response.GROUP_LEVEL);
            $("[name='Detail.LocationCity']").val(response.CITY);
            $("[name='Detail.LocationOffice']").val(response.BASETOWN);
            $("[name='Detail.LocationOffice']").val(response.BASETOWN);

            if (response.EmployeeVehicle != null) {
                $("#Detail_VehicleType").val(response.EmployeeVehicle.VehicleType.toUpperCase());
                $("#Detail_VehicleUsage").val(response.EmployeeVehicle.VehicleUsage.toUpperCase());
                $("[name='Detail.VehicleType']").val(response.EmployeeVehicle.VehicleType.toUpperCase());
                $("[name='Detail.VehicleUsage']").val(response.EmployeeVehicle.VehicleUsage.toUpperCase());
                $("[name='Detail.PoliceNumber']").val(response.EmployeeVehicle.PoliceNumber);
                $("[name='Detail.Manufacturer']").val(response.EmployeeVehicle.Manufacturer);
                $("[name='Detail.Model']").val(response.EmployeeVehicle.Models);
                $("[name='Detail.SERIES']").val(response.EmployeeVehicle.Series);
                $("[name='Detail.BodyType']").val(response.EmployeeVehicle.BodyType);
                $("[name='Detail.VendorName']").val(response.EmployeeVehicle.VendorName);
                $("[name='Detail.StartPeriod']").val(response.EmployeeVehicle.StartDate);
                $("[name='Detail.EndPeriod']").val(response.EmployeeVehicle.EndDate);
            }
        }
    });
}
function changePoliceNumberCheck() {
    if ($("#changePolice").is(":checked")) {
        $("#changePoliceNumber").removeAttr("readonly");
    }
}

function changeCity(obj) {
    var cityParam = $(obj).val();
    if (cityParam != null) {
        fillDropdownFromAjax('@Url.Action("GetLocationByCity","TraCrf")', { city: cityParam }, "#newOfficeLocation");
    }
}

function GetRelocation(obj) {
    var relType = $(obj).val();

    if (relType == "RELOCATE_UNIT") {
        $("#changeUnitButton").hide();
    } else if (relType == "CHANGE_UNIT") {
        $("#changeUnitButton").show();
    } else {
        $("#changeUnitButton").hide();
    }
}