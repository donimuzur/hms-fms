(function () {
    'use strict';

    angular.module('app').factory('ExcelReaderService', excelReaderService);

    excelReaderService.$inject = ['$upload', 'GlobalConstantService'];

    /* @ngInject */
    function excelReaderService($upload, GlobalConstantService) {
        
        var endpoint = GlobalConstantService.getConstant("api_endpoint");

        var service = {
            readExcel: readExcel,
            upload:upload
        };

        function readExcel(file, successCallback, errorCallback) {
            console.info(file);
            var param = {
                url: endpoint + '/readexcel/',
                file: file,
                fields: {}
            };
            $upload.upload(param).then(successCallback, errorCallback);
        }
        function upload(param, successCallback, errorCallback) {

            var param = {
                url: endpoint + '/uploadImage/',
                file: param,
                fields: {
                    folder: "UPLOAD_DIRECTORIES_BANKDETAIL",
                    prefix: param[0].name,
                    size: Number(param[0].size),
                    types: ".pdf"
                }
            };
            $upload.upload(param).then(successCallback, errorCallback);
        }
        
        return service;
    }
})();