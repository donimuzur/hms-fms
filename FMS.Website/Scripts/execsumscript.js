$(document).ready(function () {
    $('#ExecSummMenu').removeClass('collapse');
    $('#NumbVehicle').removeClass('active');
    $('#NumbWtc').removeClass('active');
    $('#NumbMakeType').removeClass('active');
    $('#Odometer').removeClass('active');
    $('#LiterByFunction').removeClass('active');
    $('#FuelCostByFunction').removeClass('active');
    $('#LeaseCostByFunction').removeClass('active');
    $('#SalesByRegion').removeClass('active');
    $('#Accident').removeClass('active');
    $('#AcVsOb').removeClass('active');
    $('#SumPtdByFunction').removeClass('active');

    if ($('.title-page').html() == 'Number Of Vehicle') {
        $('#NumbVehicle').addClass('active');
    } else if ($('.title-page').html() == 'Number Of Vehicle WTC') {
        $('#NumbWtc').addClass('active');
    } else if ($('.title-page').html() == 'Number Of Vehicle Make') {
        $('#NumbMakeType').addClass('active');
    } else if ($('.title-page').html() == 'Odometer') {
        $('#Odometer').addClass('active');
    } else if ($('.title-page').html() == 'Liter By Function') {
        $('#LiterByFunction').addClass('active');
    } else if ($('.title-page').html() == 'Fuel Cost By Function') {
        $('#FuelCostByFunction').addClass('active');
    } else if ($('.title-page').html() == 'Lease Cost By Function') {
        $('#LeaseCostByFunction').addClass('active');
    } else if ($('.title-page').html() == 'Sales By Region') {
        $('#SalesByRegion').addClass('active');
    } else if ($('.title-page').html() == 'Accident') {
        $('#Accident').addClass('active');
    } else if ($('.title-page').html() == 'AC Vs OB') {
        $('#AcVsOb').addClass('active');
    } else if ($('.title-page').html() == 'Sum PTD By Function') {
        $('#SumPtdByFunction').addClass('active');
    }

    $(".chosen").chosen();

});

$('.chosen').on('change', function (evt, params) {
    var theList = "";

    $("li.search-choice span").each(function (e) {
        theList += $(this).text() + ",";
    });

    theList = theList.slice(0, -1);

    $('#SearchView_FunctionId').val(theList);
});