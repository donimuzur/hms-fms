$(document).ready(function () {
    $('#CfmIdleParent').removeClass('collapse');

    if ($('.title-page').html() == 'CFM Idle Report') {
        $('#CfmIdleParent').addClass('active');
    }

});