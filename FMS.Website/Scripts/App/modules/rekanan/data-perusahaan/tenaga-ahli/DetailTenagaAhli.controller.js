(function () {
    'use strict';

    angular.module("app")
            .controller("DetailTenagaAhliCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'TenagaAhliService',
        'UIControlService', 'item', '$uibModalInstance', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, TenagaAhliService,
        UIControlService, item, $uibModalInstance, GlobalConstantService) {

        var vm = this;
        vm.item = item.item;

        vm.init = init;
        function init() {
            console.info(JSON.stringify(item.item));
        }

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };


    }
})();