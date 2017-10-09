(function () {
    'use strict';

    angular.module("app")
    .controller("detailApprovalCtrl", ctrl);

    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModalInstance', 'item', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'TenderVerificationService', 'UIControlService', 'GlobalConstantService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModalInstance, item, $translate, $translatePartialLoader, $location, SocketService, TenderVerificationService, UIControlService, GlobalConstantService) {

        var vm = this;
        var contractRequisitionId = item.contractRequisitionId;
        var loadmsg = "MESSAGE.LOADING";

        vm.crApps = [];

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('verifikasi-tender');
            vm.loadData();
        };

        vm.loadData = loadData;
        function loadData() {
            UIControlService.loadLoadingModal(loadmsg);
            TenderVerificationService.GetCRApprovals({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    vm.ProjectTitle = reply.data.ProjectTitle;
                    vm.crApps = reply.data.ContractRequisitionApprovals;
                    vm.crApps.forEach(function (cra) {
                        cra.ApprovalDate = UIControlService.convertDate(cra.ApprovalDate);
                        if (cra.ApprovalStatus !== null) {
                            cra.ApprovalStatus = cra.ApprovalStatus === true ? 'APPROVED' : 'REJECTED';
                        }
                    });
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_APPROVERS'));
                }
            }, function (error) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_APPROVERS'));
            });
        }

        vm.cancel = cancel;
        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();