$(document).ready(function () {
    var string = $('.title-page').html();
    if (string.indexOf('User Guideline') !== -1)
    {
        $('#UserGuideMenu').removeClass('collapse');
        $('#UserGuideBENEFIT').removeClass('active');
        $('#UserGuideWTC').removeClass('active');

        if ($('.title-page').html() == 'User Guideline Benefit') {
            $('#UserGuideBENEFIT').addClass('active');
        }
        else if ($('.title-page').html() == 'User Guideline WTC') {
            $('#UserGuideWTC').addClass('active');
        }
    }
    else if (string.indexOf('Vendor SLA') !== -1) {
        $('#VendorSLAMenu').removeClass('collapse');
        $('#VendorSLATRAC').removeClass('active');
        $('#VendorSLAASSA').removeClass('active');

        if ($('.title-page').html() == 'Vendor SLA TRAC') {
            $('#VendorSLATRAC').addClass('active');
        }
        else if ($('.title-page').html() == 'Vendor SLA ASSA') {
            $('#VendorSLAASSA').addClass('active');
        }
    }
    else if (string.indexOf('Vendor Information') !== -1) {
        $('#VendorInformationMenu').removeClass('collapse');
        $('#VendorInformationTRAC').removeClass('active');
        $('#VendorInformationASSA').removeClass('active');

        if ($('.title-page').html() == 'Vendor Information TRAC') {
            $('#VendorInformationTRAC').addClass('active');
        }
        else if ($('.title-page').html() == 'Vendor Information ASSA') {
            $('#VendorInformationASSA').addClass('active');
        }
    }
    else if (string.indexOf('Vehicle Information') !== -1) {
        $('#VehicleInformationMenu').removeClass('collapse');
        $('#VehicleInformationBENEFIT').removeClass('active');
        $('#VehicleInformationWTC').removeClass('active');

        if ($('.title-page').html() == 'Vehicle Information Benefit') {
            $('#VehicleInformationBENEFIT').addClass('active');
        }
        else if ($('.title-page').html() == 'Vehicle Information WTC') {
            $('#VehicleInformationWTC').addClass('active');
        }
    }
});