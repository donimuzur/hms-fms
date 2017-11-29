function selectVehicle(urlFunction) {
    
    var vehType = $('#Detail_VehicleType').val();
    var employee = $('[name="Detail.EmployeeId"]').val();

    if ($('#Detail_VehicleType').val() == "") {
        $('#tb-body-select-veh').html("");
        $('#tb-body-select-veh').append('<tr><td style="text-align:center" colspan="8">no data<td></tr>');
    }
    else {
        $.ajax({
            type: 'POST',
            url: urlFunction,
            data: {
                vehType: vehType,
                
                employeeId: employee
            },
            success: function (data) {
                if (data.length > 0) {
                    $('#tb-body-select-veh').html("");
                    for (var i = 0; i < data.length; i++) {
                        
                        var tableData = '<tr>' +
                            '<td><input type="hidden" name="mstfleetid" id="Detail_VehicleData[' + i + ']_MstFleetId" /><input name="selectvehicleradio" id="selectvehicleradio[' + i + ']" type="radio"></td>' +
                            '<td><input type="hidden" name="policenumber" id="Detail_VehicleData[' + i + ']_PoliceNumber" value=' + data[i].PoliceNumber + '></input>' + data[i].PoliceNumber + '</td>' +
                            '<td><input type="hidden" name="manufacturer" id="Detail_VehicleData[' + i + ']_Manufacturer" value=' + data[i].Manufacturer + '></input>' + data[i].Manufacturer + '</td>' +
                            '<td><input type="hidden" name="model" id="Detail_VehicleData[' + i + ']_Models" value=' + data[i].Models + '></input>' + data[i].Models + '</td>' +
                            '<td><input type="hidden" name="series" id="Detail_VehicleData[' + i + ']_Series" value=' + data[i].Series + '></input>' + data[i].Series + '</td>' +
                            '<td><input type="hidden" name="bodytype" id="Detail_VehicleData[' + i + ']_BodyType" value=' + data[i].BodyType + '></input>' + data[i].BodyType + '</td>' +
                            '<td><input type="hidden" name="vendorname" id="Detail_VehicleData[' + i + ']_VendorName" value=' + data[i].VendorName + '></input>' + data[i].VendorName + '</td>' +
                            '<td><input type="hidden" name="color" id="Detail_VehicleData[' + i + ']_Color" value=' + data[i].Color + '></input>' + data[i].Color + '</td>' +
                            '<td><input type="hidden" name="startdate" id="Detail_VehicleData[' + i + ']_StartContract" value=' + data[i].StartContract + '></input>' + moment(data[i].StartContract).format('DD-MMM-YYYY') + '</td>' +
                            '<td><input type="hidden" name="enddate" id="Detail_VehicleData[' + i + ']_EndContract" value=' + data[i].EndContract + '></input>' + moment(data[i].EndContract).format('DD-MMM-YYYY') + '</td>' +
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

function ToggleTemporary() {
    
    var expectedDate = $("#expectedToggle").val();
    var effectiveDate = $("[name='Detail.EffectiveDate']").val();
    
    var expected = moment(expectedDate);
    var effective = moment(effectiveDate);
    if (expected > effective) {
        $("#temporaryButton").show();
        $("#tempExpected").val(expectedDate);
    } else {
        $("#temporaryButton").hide();
    }
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
            

            if (response.EmployeeVehicle != null) {
                $("#Detail_VehicleType").val(response.EmployeeVehicle.VehicleType.toUpperCase());
                $("#Detail_VehicleUsage").val(response.EmployeeVehicle.VehicleUsage.toUpperCase());
                $("[name='Detail.VehicleType']").val(response.EmployeeVehicle.VehicleType.toUpperCase());
                $("[name='Detail.VehicleUsage']").val(response.EmployeeVehicle.VehicleUsage.toUpperCase());
                $("[name='Detail.PoliceNumber']").val(response.EmployeeVehicle.PoliceNumber);
                $("[name='Detail.Manufacturer']").val(response.EmployeeVehicle.Manufacturer);
                $("[name='Detail.Model']").val(response.EmployeeVehicle.Models);
                $("[name='Detail.SERIES']").val(response.EmployeeVehicle.Series);
                $("[name='Detail.Body']").val(response.EmployeeVehicle.BodyType);
                $("[name='Detail.CostCenter']").val(response.EmployeeVehicle.CostCenter);
                $("[name='Detail.LocationCity']").val(response.EmployeeVehicle.City);
                $("[name='Detail.VendorName']").val(response.EmployeeVehicle.VendorName);
                var startContract = moment(response.EmployeeVehicle.StartContract).format('DD-MMM-YYYY');
                var endContract = moment(response.EmployeeVehicle.EndContract).format('DD-MMM-YYYY');
                $("[name='Detail.StartPeriod']").val(startContract);
                $("[name='Detail.MstFleetId']").val(response.EmployeeVehicle.MstFleetId);
                $("[name='Detail.EndPeriod']").val(endContract);
            }
        }
    });
}
function changePoliceNumberCheck() {
    
    if ($("#changePolice").is(":checked")) {
        $("#newPoliceNumber").removeAttr("disabled");
    } else {
        $("#newPoliceNumber").attr("disabled", "disabled");
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

function InitEmployee(url,urlsearch) {


    
    var options = {
        url: url,
        getValue: "EMPLOYEE_ID",

        template: {
            type: "description",
            fields: {
                description: "FORMAL_NAME"
            }
        },

        list: {
            match: {
                enabled: true
            },
            onChooseEvent: function () {
                GetEmployee(urlsearch, "#employeeSelect");
            }
        },

        //theme: "plate-dark"

        //ajaxSettings: {
        //    dataType: "json",
        //    method: "POST",
        //    data: {
        //        dataType: "json"
        //    }
        //},

        //preparePostData: function (data) {
        //    data.phrase = $("#employeeSelect").val();
        //    return data;
        //},
        //getValue: function (element) {
        //    return element.EMPLOYEE_ID;
        //},
        //requestDelay: 400

    };

    $("#employeeSelect").easyAutocomplete(options);
}

$(document).ready(function () {
    
    if ($("#changePolice").is(":checked")) {
        $("#newPoliceNumber").removeAttr("disabled");
    } else {
        $("#newPoliceNumber").attr("disabled", "disabled");
    }

    

    $("#btnSelectVehicle").click(function () {
        var tr = $("[name='selectvehicleradio']:checked").parents("tr");
        var manufacturer = $(tr).find("[name='manufacturer']").val();
        var models = $(tr).find("[name='model']").val();
        var series = $(tr).find("[name='series']").val();
        var bodytype = $(tr).find("[name='bodytype']").val();
        var vendorname = $(tr).find("[name='vendorname']").val();

        var startdate = $(tr).find("[name='startdate']").val();
        var enddate = $(tr).find("[name='enddate']").val();
        var policenumber = $(tr).find("[name='policenumber']").val();
        var mstfleetid = $(tr).find("[name='mstfleetid']").val();

        $("[name='Detail.PoliceNumber']").val(policenumber);
        $("[name='Detail.Manufacturer']").val(manufacturer);
        $("[name='Detail.Model']").val(models);
        $("[name='Detail.SERIES']").val(series);
        $("[name='Detail.Body']").val(bodytype);
        $("[name='Detail.VendorName']").val(vendorname);
        $("[name='Detail.MstFleetId']").val(mstfleetid);
        $("[name='Detail.StartPeriod']").val(moment(startdate).format('DD-MMM-YYYY'));
        $("[name='Detail.EndPeriod']").val(moment(enddate).format('DD-MMM-YYYY'));
        //var tableData = '<tr>' +
        //                    '<td><input type="hidden" name="Detail.PoliceNumber" id="Detail_PoliceNumber" value="' + policenumber + '"></input>' + policenumber + '</td>' +
        //                    '<td><input type="hidden" name="Detail.Manufacturer" id="Detail_Manufacturer" value="' + manufacturer + '"></input>' + manufacturer + '</td>' +
        //                    '<td><input type="hidden" name="Detail.Models" id="Detail_Models" value="' + model + '"></input>' + model + '</td>' +
        //                    '<td><input type="hidden" name="Detail.SERIES" id="Detail_Series" value="' + series + '"></input>' + series + '</td>' +
        //                    '<td><input type="hidden" name="Detail.BODY_TYPE" id="Detail_BodyType" value="' + bodytype + '"></input>' + bodytype + '</td>' +
        //                    '<td><input type="hidden" name="Detail.VendorName" id="Detail_VendorName" value="' + vendorname + '"></input>' + vendorname + '</td>' +

        //                    '<td><input type="hidden" name="Detail.StartDate" id="Detail_StartDate" value="' + startdate + '"></input>' + startdate + '</td>' +
        //                    '<td><input type="hidden" name="Detail.EndDate" id="Detail_EndDate" value="' + enddate + '"></input>' + enddate + '</td>' +
        //                    '</tr>';
        //$('#tb-body-select-vehicle').html(tableData);

        $('#selectvehmodal').modal('hide');
    });
});