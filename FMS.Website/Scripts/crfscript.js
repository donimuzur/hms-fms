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
                            '<td><input type="hidden" name="manufacturer" id="Detail_VehicleData[' + i + ']_Manufacturer" value=' + data[i].Manufacturer + '></input>' + data[i].Manufacturer + '</td>' +
                            '<td><input type="hidden" name="model" id="Detail_VehicleData[' + i + ']_Models" value=' + data[i].Models + '></input>' + data[i].Models + '</td>' +
                            '<td><input type="hidden" name="series" id="Detail_VehicleData[' + i + ']_Series" value=' + data[i].Series + '></input>' + data[i].Series + '</td>' +
                            '<td><input type="hidden" name="bodytype" id="Detail_VehicleData[' + i + ']_BodyType" value=' + data[i].BodyType + '></input>' + data[i].BodyType + '</td>' +
                            '<td><input type="hidden" name="vendorname" id="Detail_VehicleData[' + i + ']_VendorName" value=' + data[i].VendorName + '></input>' + data[i].VendorName + '</td>' +
                            '<td><input type="hidden" name="color" id="Detail_VehicleData[' + i + ']_Color" value=' + data[i].Color + '></input>' + data[i].Color + '</td>' +
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