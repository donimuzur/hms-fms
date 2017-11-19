$(document).ready(function () {
    $('#TempMenu').removeClass('collapse');
    $('#TempOpen').removeClass('active');
    $('#TempCompleted').removeClass('active');

    if ($('.title-page').html() == 'Temporary Open Document') {
        $('#TempOpen').addClass('active');
    }
    else if ($('.title-page').html() == 'Temporary Completed Document') {
        $('#TempCompleted').addClass('active');
    }
    else if ($('.title-page').html() == 'Temporary Personal Dashboard') {
        $('#TempParent').addClass('active');
    }

});