(function () {
    'use strict';

    angular.module("app")
    .controller("detailApprovalCtrl", ctrl);

    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModalInstance', 'item', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'TenderVerificationService', 'UIControlService', 'RequisitionListService', 'CommonEngineService', 'GlobalConstantService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModalInstance, item, $translate, $translatePartialLoader, $location, SocketService, TenderVerificationService, UIControlService, RequisitionListService, CommonEngineService, GlobalConstantService) {

        var vm = this;
        var contractRequisitionId = item.contractRequisitionId;
        var loadmsg = "MESSAGE.LOADING";

        vm.crApps = [];
        vm.employeeFullName = "";
        vm.employeeID = 0;
        vm.information = "";
        vm.hasApproved = false;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('verifikasi-tender');

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
            TenderVerificationService.GetCRApprovals({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    vm.ProjectTitle = reply.data.ProjectTitle;
                    vm.StatusName = reply.data.StatusName;
                    vm.crApps = reply.data.ContractRequisitionApprovals;
                    vm.crApps.forEach(function (cra) {
                        cra.ApprovalDate = UIControlService.convertDate(cra.ApprovalDate);
                        if (vm.employeeID === cra.EmployeeID && cra.ApprovalStatus !== null) {
                            vm.hasApproved = true;
                        }
                        if (cra.ApprovalStatus !== null) {
                            cra.ApprovalStatus = cra.ApprovalStatus === true ? 'APPROVED' : 'REJECTED';
                        }
                        if (vm.employeeID === cra.EmployeeID) {
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

        vm.approve = approve;
        function approve() {
            sendApproval(true);
        }

        vm.reject = reject;
        function reject() {
            sendApproval(false);
        }

        function sendApproval(approvalStatus) {
            UIControlService.loadLoadingModal(loadmsg);
            RequisitionListService.SetApprovalStatus({
                ContractRequisitionId: contractRequisitionId,
                ApprovalStatus: approvalStatus,
                Information: vm.information
            }, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    $uibModalInstance.close();
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SET_APPROVAL'));
                }
            }, function (error) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SET_APPROVAL'));
            });
        }

        vm.cancel = cancel;
        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();