(function () {
    'use strict';

    angular.module("app")
            .controller("FormSureQuestionnaireCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'VendorRegistrationService', 'UploadFileConfigService',
        'UIControlService', 'UploaderService', '$uibModalInstance', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, VendorRegistrationService, UploadFileConfigService,
        UIControlService, UploaderService, $uibModalInstance, GlobalConstantService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.action = "";
        vm.pathFile;
        vm.Description;
        vm.fileUpload;
        vm.size;
        vm.name;
        vm.type;
        vm.flag;
        vm.selectedForm;


        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('daftar');
        }

        vm.oke = oke;
        function oke() {
            console.info("sdsd");
            $uibModalInstance.close();
        }
        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };

    }
})();