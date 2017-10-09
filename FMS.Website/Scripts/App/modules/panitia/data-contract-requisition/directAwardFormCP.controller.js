(function () {
    'use strict';

    angular.module("app")
    .controller("directAwardFormCPCtrl", ctrl);
    
    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModalInstance', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService', 'item'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModalInstance, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService, item) {

        var vm = this;
        var contractRequisitionId = item.contractRequisitionId;
        var loadmsg = "";

        vm.directAward = {};
        vm.isTenderVerification = true;
        vm.estimatedCostLabel = ['US$', 'IDR', 'OTHERS'];

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
            DataContractRequisitionService.SelectDirectAward({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    vm.directAward = reply.data;
                    if (!vm.directAward.ContractRequisitonDACheckLists) {
                        vm.directAward.ContractRequisitonDACheckLists = [];
                        for (var i = 0; i < 13; i++){
                            vm.directAward.ContractRequisitonDACheckLists.push({
                                Index : i,
                                IsChecked : false
                            });
                        }
                    }

                    vm.directAward.ContractRequisition = {};
                    vm.directAward.ContractRequisition.ProjectManager = item.ProjectManager;
                    vm.directAward.ProjectManagerFullName = item.ProjectManagerFullName;
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD'));
                }
            }, function (error) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        };

        vm.onBatalClick = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();