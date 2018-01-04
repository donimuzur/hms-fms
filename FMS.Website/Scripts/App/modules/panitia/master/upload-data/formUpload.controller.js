(function () {
    'use strict';

    angular.module("app")
            .controller("UploadDataCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'UploadDataService', 'UploadFileConfigService',
        'UIControlService', 'UploaderService', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, UploadDataService, UploadFileConfigService,
        UIControlService, UploaderService, GlobalConstantService) {

        var vm = this;
        vm.listForm = [];
        vm.idUploadConfigs = [];
        vm.selectUpload = selectUpload;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('form-upload-data');
            loadForm();
        }

        vm.loadForm = loadForm;
        function loadForm() {
            vm.listForm = [
                { dataName: "FORM_UPLOAD.DOA", action: "FORM_UPLOAD.BTN_DOA" },
                { dataName: "FORM_UPLOAD.BUDGET_APPROVAL", action: "FORM_UPLOAD.BTN_BUDGET_APPROVAL" },
                { dataName: "FORM_UPLOAD.BUDGET_STRUCT", action: "FORM_UPLOAD.BTN_BUDGET_STRUCT" }
            ];
        }

        function selectUpload() {

        }


    }
})();