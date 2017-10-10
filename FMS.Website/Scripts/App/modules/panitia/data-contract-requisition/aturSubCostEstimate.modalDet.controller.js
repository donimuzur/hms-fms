(function () {
    'use strict';

    angular.module("app")
    .controller("aturSubCostEstimateDetCtrl", ctrl);

    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModalInstance', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService', 'item'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModalInstance, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService, item) {

        var vm = this;
        var loadmsg = "";
        var contractRequisitionCESubID = item.contractRequisitionCESubID;

        vm.ceSub = {};

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('data-contract-requisition');
            $translate.refresh().then(function () {
                loadmsg = $filter('translate')('MESSAGE.LOADING');
            });
            vm.loadData();
        };

        vm.loadData = loadData;
        function loadData() {
            UIControlService.loadLoadingModal(loadmsg);
            DataContractRequisitionService.GetDetailedCESub({
                ContractRequisitionCESubID: contractRequisitionCESubID
            }, function (reply) {
                if (reply.status === 200) {
                    vm.ceSub = reply.data;
                    UIControlService.unloadLoadingModal();
                } else {
                    UIControlService.unloadLoadingModal();
                    UIControlService.msg_growl('error', $filter('translate')('MESSAGE.ERR_LOAD_CELINE'));
                }
            }, function (error) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        };

        vm.batal = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();