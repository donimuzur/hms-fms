﻿$(document).ready(function () {
    $('#ExecSummMenu').removeClass('collapse');
    $('#NumbVehicle').removeClass('active');
    $('#NumbWtc').removeClass('active');
    $('#NumbMakeType').removeClass('active');
    $('#Odometer').removeClass('active');

    if ($('.title-page').html() == 'Number Of Vehicle') {
        $('#NumbVehicle').addClass('active');
    } else if ($('.title-page').html() == 'Number Of Vehicle WTC') {
        $('#NumbWtc').addClass('active');
    } else if ($('.title-page').html() == 'Number Of Vehicle Make') {
        $('#NumbMakeType').addClass('active');
    } else if ($('.title-page').html() == 'Odometer') {
        $('#Odometer').addClass('active');
    }

});