
function GetTableData(table) {
 
    var columnLength = $(table).find("tbody tr:first td").length;
    
    var datarows = [];
    table.find('tbody tr').each(function (i, el) {
        var datacolumn = [];
        var $tds = $(this).find('td');
        for (var j = 0; j < columnLength; j++) {
            datacolumn[j] = $tds.eq(j).text();
            
        }

        datarows[i] = datacolumn;
    });
    return datarows;
}

