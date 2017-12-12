$(document).ready(function () {
    $('#VehicleParent').removeClass('collapse');

    if ($('.title-page').html() == 'Vehicles Report' || $('.title-page').html() =='Vehicles Report Detail') {
        $('#VehicleParent').addClass('active');
    }

});