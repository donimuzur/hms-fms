(function () {
    'use strict';

    angular.module("app")
    .controller("summaryPenawaranController", ctrl);

    ctrl.$inject = ['$state', 'item', '$http', '$filter', '$uibModalInstance', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'GlobalConstantService'];
    /* @ngInject */
    function ctrl($state, item, $http, $filter, $uibModalInstance, $translate, $translatePartialLoader, $location, SocketService, GlobalConstantService) {

        var vm = this;

        vm.documentDate = item.documentDate;
        vm.documentUrl = item.documentUrl;
        vm.summary = item.summary;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";

        vm.tutup = tutup;
        function tutup() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();