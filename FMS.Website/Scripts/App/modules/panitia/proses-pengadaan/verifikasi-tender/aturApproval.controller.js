(function () {
    'use strict';

    angular.module("app")
    .controller("aturApprovalCtrl", ctrl);

    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'TenderVerificationService', 'UIControlService', 'GlobalConstantService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, TenderVerificationService, UIControlService, GlobalConstantService) {

        var vm = this;
        var contractRequisitionId = Number($stateParams.contractRequisitionId);
        vm.contractRequisitionId = Number($stateParams.contractRequisitionId);
        var loadmsg = "MESSAGE.LOADING";

        vm.crApps = [];
        vm.ProjectTitle = "";

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('verifikasi-tender');            
            vm.loadData();
        };

        vm.loadData = loadData;
        function loadData() {
            UIControlService.loadLoading(loadmsg);
            TenderVerificationService.GetCRApprovals({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    if (reply.data.StatusName === 'CR_PROCESS_2') {
                        vm.ProjectTitle = reply.data.ProjectTitle;
                        vm.crApps = reply.data.ContractRequisitionApprovals;
                    } else {
                        UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_NOT_PROCESS_2'));
                        $state.transitionTo('verifikasi-tender-cp');
                    }
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_APPROVERS'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_APPROVERS'));
            });
        }

        vm.add = add;
        function add() {
            var item = {
                currentData: vm.crApps
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/proses-pengadaan/verifikasi-tender/aturApproval.modal.html',
                controller: 'selectApproverModalCtrl',
                controllerAs: 'selAppModalCtrl',
                resolve: { item: function () { return item; } }
            });
            modalInstance.result.then(function (selected) {
                vm.crApps.push({
                    EmployeeID: selected.EmployeeID,
                    EmployeeFullName: selected.FullName + ' ' + selected.SurName,
                    EmployeePositionName: selected.PositionName,
                    EmployeeDepartmentName: selected.DepartmentName,
                    IsHighPriority: false
                });
            });
        }

        vm.delete = delet;
        function delet(index) {
            vm.crApps.splice(index, 1);
        }

        vm.save = save;
        function save() {

            var chairmanExist = false;
            for (var i = 0; i < vm.crApps.length; i++) {
                if (vm.crApps[i].IsHighPriority) {
                    chairmanExist = true;
                    break;
                }
            }
            if (!chairmanExist) {
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_NO_CHAIRMAN'));
                return;
            }

            UIControlService.loadLoading(loadmsg);
            TenderVerificationService.SaveCRApprovals({
                ContractRequisitionId: contractRequisitionId,
                ContractRequisitionApprovals: vm.crApps
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SAVE_APPROVERS'));
                    vm.back();
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE_APPROVERS'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE_APPROVERS'));
            });
        };

        vm.back = back;
        function back() {
            $state.transitionTo('verifikasi-tender-cp');
        };
    }
})();