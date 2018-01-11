$(document).ready(function () {
    $('#TempMenu').removeClass('collapse');
    $('#TempOpen').removeClass('active');
    $('#TempCompleted').removeClass('active');

    if ($('.title-page').html() == 'Temporary Open Document' || $('.title-page').html() == 'Mass upload from Vendor') {
        $('#TempOpen').addClass('active');
    }
    else if ($('.title-page').html() == 'Temporary Completed Document') {
        $('#TempCompleted').addClass('active');
    }
    else if ($('.title-page').html() == 'Temporary Personal Dashboard' || $('.title-page').html() == 'Mass upload from Vendor - Personal Dashboard') {
        $('#TempParent').addClass('active');
    }

});

function selectVehicle(urlFunction) {
    var vehType = $('#Detail_VehicleType').val();
    var groupLevel = $('#Detail_GroupLevel').val();
    var createdDate = $('#Detail_CreateDate').val();
    var includeCfm = false;
    if ($('#Detail_IsIncludeCfmIdle').is(":checked")) {
        includeCfm = true;
    }

    if ($('#Detail_EmployeeId').find("option:selected").val() == "") {
        $('#tb-body-select-veh').html("");
        $('#tb-body-select-veh').append('<tr><td style="text-align:center" colspan="9">no data<td></tr>');
    }
    else {
        $.ajax({
            type: 'POST',
            url: urlFunction,
            data: { vehType: vehType, groupLevel: groupLevel, createdDate: createdDate, includeCfm: includeCfm },
            success: function (data) {
                if (data.length > 0) {
                    $('#tb-body-select-veh').html("");
                    for (var i = 0; i < data.length; i++) {
                        var tableData = '<tr>' +
                            '<td><input name="selectvehicleradio" id="selectvehicleradio_' + i + '" type="radio" value="' + i + '"></td>' +
                            '<td><input type="hidden" name="manufacturer" id="Detail_Manufacturer_' + i + '" value="' + data[i].Manufacturer + '"></input>' + data[i].Manufacturer + '</td>' +
                            '<td><input type="hidden" name="model" id="Detail_Models_' + i + '" value="' + data[i].Models + '"></input>' + data[i].Models + '</td>' +
                            '<td><input type="hidden" name="series" id="Detail_Series_' + i + '" value="' + data[i].Series + '"></input>' + data[i].Series + '</td>' +
                            '<td><input type="hidden" name="bodytype" id="Detail_BodyType_' + i + '" value="' + data[i].BodyType + '"></input>' + data[i].BodyType + '</td>' +
                            '<td><input type="hidden" name="color" id="Detail_Color_' + i + '" value="' + data[i].Color + '"></input>' + data[i].Color + '</td>' +
                            '<input type="hidden" name="cfmidleid" id="Detail_CfmIdleId_' + i + '" value="' + data[i].MstFleetId + '"></input>' +
                            '<input type="hidden" name="cargrouplevelcfm" id="Detail_CarGroupLevel_' + i + '" value="' + data[i].CarGroupLevel + '"></input>' +
                            '<input type="hidden" name="cargrouplevel" id="Detail_CarGroupLevel2_' + i + '" value="' + data[i].GroupLevel + '"></input>' +
                            '</tr>';
                        $('#tb-body-select-veh').append(tableData);
                    }
                } else {
                    $('#tb-body-select-veh').html("");
                    $('#tb-body-select-veh').append('<tr><td style="text-align:center" colspan="9">no data<td></tr>');
                }

                var table = $('#tbCsfVehiclePopup').DataTable({
                    "dom": 'lf<"table-overflow"t>pi',
                    "searching": true,
                    "iDisplayLength": 3,
                    orderCellsTop: true
                });

                // Apply the search
                table.columns().eq(0).each(function (colIdx) {
                    $('input', $('.filters th')[colIdx]).on('keyup change', function () {
                        table
                            .column(colIdx)
                            .search(this.value)
                            .draw();
                    });
                });
            }
        });
    }
}

function InitEmployee(url, urlsearch) {
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

    };

    $("#employeeSelect").easyAutocomplete(options);
}

function GetEmployee(urlGet, obj) {

    var Id = $(obj).val();
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
        }
    });
}