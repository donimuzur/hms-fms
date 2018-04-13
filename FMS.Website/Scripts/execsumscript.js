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
    $('#SummAll').removeClass('active');
    $('#SummRegion').removeClass('active');

    if ($('.title-page').html() == 'Number Of Vehicle - All') {
        $('#NumbVehicle').addClass('active');
    } else if ($('.title-page').html() == 'Number Of Vehicle - Working Tool Car') {
        $('#NumbWtc').addClass('active');
    } else if ($('.title-page').html() == 'Number Of Vehicle Make - Type') {
        $('#NumbMakeType').addClass('active');
    } else if ($('.title-page').html() == 'Odometer') {
        $('#Odometer').addClass('active');
    } else if ($('.title-page').html() == 'Fuel Purchased (liters)') {
        $('#LiterByFunction').addClass('active');
    } else if ($('.title-page').html() == 'Fuel Cost') {
        $('#FuelCostByFunction').addClass('active');
    } else if ($('.title-page').html() == 'Lease Cost') {
        $('#LeaseCostByFunction').addClass('active');
    } else if ($('.title-page').html() == 'Operational Cost') {
        $('#SalesByRegion').addClass('active');
    } else if ($('.title-page').html() == 'Accident') {
        $('#Accident').addClass('active');
    } else if ($('.title-page').html() == 'Actual Cost Vs Budget') {
        $('#AcVsOb').addClass('active');
    } else if ($('.title-page').html() == 'Sum PTD By Function') {
        $('#SumPtdByFunction').addClass('active');
    } else if ($('.title-page').html() == 'Executive Summary All') {
        $('#SummAll').addClass('active');
    } else if ($('.title-page').html() == 'Executive Summary Working Tool Car') {
        $('#SummRegion').addClass('active');
    }

    $(".chosen").chosen();
});

$('#select-function-id.chosen').on('change', function (evt, params) {
    var theList = "";

    $("li.search-choice span").each(function (e) {
        theList += $(this).text() + ",";
    });

    theList = theList.slice(0, -1);

    $('#SearchView_FunctionId').val(theList);
});

$('#SearchView_Zone.chosen').on('change', function (evt, params) {
    var theList = "";

    $("li.search-choice span").each(function (e) {
        theList += $(this).text() + ",";
    });

    theList = theList.slice(0, -1);

    $('#SearchView_ZoneId').val(theList);
});