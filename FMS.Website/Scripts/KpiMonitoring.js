$(document).ready(function () {
    $('#KpiMonitoringParent').removeClass('collapse');

    if ($('.title-page').html() == 'KPI Monitoring Report') {
        $('#KpiMonitoringParent').addClass('active');
    }

});