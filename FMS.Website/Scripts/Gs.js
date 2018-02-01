$(document).ready(function () {
    $('#GsParent').removeClass('collapse');

    if ($('.title-page').html() == 'GS Report') {
        $('#GsParent').addClass('active');
    }
});