(function () {
    'use strict';

    angular.module("app").controller("viewUploadDocCtrl", ctrl);

    ctrl.$inject = ['$http', '$uibModalInstance', 'item', '$filter', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'UploadDokumenLainlainService', 'UploaderService', 'UIControlService', 'GlobalConstantService', 'UploadFileConfigService'];
    /* @ngInject */
    function ctrl($http, $uibModalInstance, item, $filter, $translate, $translatePartialLoader, $location, SocketService, UploadDokumenLainlainService, UploaderService, UIControlService, GlobalConstantService, UploadFileConfigService) {

        var vm = this;

        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.item = item.item;
        vm.ValidDateConverted = convertDate(item.item.ValidDate);

        function convertDate(date) {
            return UIControlService.convertDate(date);
        }

        vm.close = close;
        function close() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();