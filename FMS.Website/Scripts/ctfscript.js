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
    else if ($('.title-page').html() == 'Mass Upload Ctf') {
        $('#CtfOpen').addClass('active');
    }
});