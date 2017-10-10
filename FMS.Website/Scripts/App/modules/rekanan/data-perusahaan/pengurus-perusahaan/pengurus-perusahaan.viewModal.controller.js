
(function () {
    'use strict';

    angular.module("app").controller("viewPengurusPerusahaanCtrl", ctrl);

    ctrl.$inject = ['$http', '$uibModalInstance', 'item', '$filter', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'PengurusPerusahaanService', 'UploaderService', 'UIControlService', 'GlobalConstantService', 'UploadFileConfigService'];
    /* @ngInject */
    function ctrl($http, $uibModalInstance, item, $filter, $translate, $translatePartialLoader, $location, SocketService, PengurusPerusahaanService, UploaderService, UIControlService, GlobalConstantService, UploadFileConfigService) {

        var vm = this;

        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.compPerson = item.compPerson;
        vm.init = init;
        function init() {
            console.info("compers" + JSON.stringify(vm.compPerson));
        }
        
        vm.close = close;
        function close() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();