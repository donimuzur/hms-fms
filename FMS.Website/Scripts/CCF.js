$(document).ready(function () {
    $('#RptCcfParent').removeClass('collapse');

    if ($('.title-page').html() == 'CCF Report') {
        $('#RptCcfParent').addClass('active');
    }
});