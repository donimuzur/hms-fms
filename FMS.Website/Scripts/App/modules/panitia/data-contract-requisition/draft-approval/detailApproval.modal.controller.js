(function () {
    'use strict';

    angular.module("app")
    .controller("detailDraftApprovalCtrl", ctrl);

    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModalInstance', 'item', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService', 'CommonEngineService', 'GlobalConstantService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModalInstance, item, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService, CommonEngineService, GlobalConstantService) {

        var vm = this;
        var contractRequisitionId = item.contractRequisitionId;
        var loadmsg = "MESSAGE.LOADING";

        vm.crDApps = [];
        vm.employeeFullName = "";
        vm.employeeID = 0;
        vm.information = "";

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('data-contract-requisition');

            UIControlService.loadLoadingModal(loadmsg);
            CommonEngineService.GetLoggedEmployee(function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    vm.employeeFullName = reply.data.FullName + ' ' + reply.data.SurName;
                    vm.employeeID = reply.data.EmployeeID;
                    vm.loadData();
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_USER'));
                }
            }, function (error) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_USER'));
            });
        };

        vm.loadData = loadData;
        function loadData() {
            UIControlService.loadLoadingModal(loadmsg);
            DataContractRequisitionService.GetCRDApprovals({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    vm.ProjectTitle = reply.data.ProjectTitle;
                    vm.StatusName = reply.data.StatusName;
                    vm.crDApps = reply.data.ContractRequisitionDraftApprovals;
                    vm.crDApps.forEach(function (cra) {
                        cra.ApprovalDate = UIControlService.convertDate(cra.ApprovalDate);
                        if (cra.ApprovalStatus !== null) {
                            cra.ApprovalStatus = cra.ApprovalStatus === true ? 'APPROVED' : 'REJECTED';
                        }
                        if (vm.employeeID == cra.EmployeeID) {
                            vm.information = cra.Information;
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