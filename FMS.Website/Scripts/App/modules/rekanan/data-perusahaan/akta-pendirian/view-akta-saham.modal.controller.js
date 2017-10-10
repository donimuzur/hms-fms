
(function () {
    'use strict';

    angular.module("app").controller("viewAktaSahamCtrl", ctrl);

    ctrl.$inject = ['$http', '$uibModalInstance', 'item', '$filter', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'UIControlService', 'GlobalConstantService'];
    /* @ngInject */
    function ctrl($http, $uibModalInstance, item, $filter, $translate, $translatePartialLoader, $location, SocketService, UIControlService, GlobalConstantService) {

        var loadmsg = 'MESSAGE.LOADING';
        var vm = this;

        vm.folderFile = GlobalConstantService.getConstant('api') + "/";

        vm.stocks = item.stocks;
        vm.documentNo = item.documentNo;

        vm.close = close;
        function close() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();