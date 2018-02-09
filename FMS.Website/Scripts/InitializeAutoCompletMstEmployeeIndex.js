function initializeAutoComplete(urlToLoad, id) {
    var options = {
        url: urlToLoad,
        list: {
            match: {
                enabled: true
            }
        },
        theme: "round"
    };
    $(id).easyAutocomplete(options);
}