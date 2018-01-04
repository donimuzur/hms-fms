function GetChangesLogs(modulid, formid) {
    if (formid == null) return;
    $.ajax({
        type: 'POST',
        url: '/ChangesLog/Get',
        data: {
            ModulId: modulid,
            FormId : formid
        },
        success: function (response) {
            $('#changes').html("");
            $('#changes').html(response);
            
        }
    });
}