$(document).ready(function () {
    $('#AutoGrParent').removeClass('collapse');
    if ($('.title-page').html() == 'Auto GR Report') {
        $('#AutoGrParent').addClass('active');
    }
});