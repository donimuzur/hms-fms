function initializeAutoComplete(urlToLoad, id) {
    var options = {
        url: urlToLoad,
        list: {
            match: {
                enabled: true
            },
            maxNumberOfElements: 8
        },
        theme: "round"
    };
    $(id).easyAutocomplete(options);
}