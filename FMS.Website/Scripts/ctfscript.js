$(document).ready(function () {
    $('#CtfMenu').removeClass('collapse');
    $('#DashboardEpaf').removeClass('active');
    $('#CtfOpenBenefit').removeClass('active');
    $('#CtfOpenWTC').removeClass('active');
    $('#CtfCompleted').removeClass('active');
    
    if ($('.title-page').html() == 'CTF Dashboard') {
        $('#CtfDashboard').addClass('active');
    }
    else if ($('.title-page').html() == 'CTF Open Document') {
        $('#CtfOpen').addClass('active');
    }
    else if ($('.title-page').html() == 'CTF Completed Document') {
        $('#CtfCompleted').addClass('active');
    }
    else if ($('.title-page').html() == 'CTF Personal Dashboard') {
        $('#CtfParent').addClass('active');
    }
});

function InitPoliceNumber(url) {
    var options = {
        url: url,
        getValue: "PoliceNumber",
        ajaxSettings: {
            dataType: "json",
            method: "POST",
            data: {
                dataType: "json"
            }
        },
        template: {
            type: "description",
            fields: {
                description: "VehicleType"
            }
        },
        list: {
            match: {
                enabled: true
            },
            onChooseEvent: function () {
                GetVehicle();
            }
        },
        requestDelay: 400
    };

    $("#PoliceNumber").easyAutocomplete(options);
}
function InitEmployee(url) {
    var options = {
        url: url,
        getValue: "EMPLOYEE_ID",
        ajaxSettings: {
            dataType: "json",
            method: "POST",
            data: {
                dataType: "json"
            }
        },
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
                GetEmployee();
            }
        }
    };

    $("#EmployeeId").easyAutocomplete(options);
}