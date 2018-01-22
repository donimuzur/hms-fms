$(document).ready(function () {
    $('#PoParent').removeClass('collapse');

    if ($('.title-page').html() == 'PO Report') {
        $('#PoParent').addClass('active');
    }
});