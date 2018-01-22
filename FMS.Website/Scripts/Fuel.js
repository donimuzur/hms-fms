$(document).ready(function () {
    $('#FuelParent').removeClass('collapse');

    if ($('.title-page').html() == 'Fuel Report') {
        $('#FuelParent').addClass('active');
    }
});