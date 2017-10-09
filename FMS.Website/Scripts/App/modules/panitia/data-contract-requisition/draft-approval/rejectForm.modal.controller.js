(function () {
    'use strict';

    angular.module("app")
    .controller("rejectFormCtrl", ctrl);

    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModalInstance', 'item', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService', 'CommonEngineService', 'GlobalConstantService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModalInstance, item, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService, CommonEngineService, GlobalConstantService) {

        var vm = this;
        var contractRequisitionId = item.contractRequisitionId;
        var projectTitle = item.projectTitle;

        vm.remark = "";

        vm.init = init;
        function init() {
            
        };

        vm.reject = reject;
        function reject() {
            $uibModalInstance.close(vm.remark);
        }        

        vm.cancel = cancel;
        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();