(function () {
    'use strict';

    angular.module("app")
    .controller("detailApprovalSignOffCtrl", ctrl);

    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModalInstance', 'item', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'UIControlService', 'GlobalConstantService', 'PenetapanPemenangVHSservice'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModalInstance, item, $translate, $translatePartialLoader, $location, SocketService, UIControlService, GlobalConstantService, PenetapanPemenangVHSservice) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";
        vm.ID = item.ID;
        vm.flag = item.flag;
        vm.Status = item.Status;
        vm.crApps = [];
        vm.employeeFullName = "";
        vm.employeeID = 0;
        vm.information = "";
        vm.flagEmp = item.flag;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('verifikasi-tender');
            loadData();
        };

        vm.loadData = loadData;
        function loadData() {
            vm.crApps = [];
            UIControlService.loadLoading(loadmsg);
            PenetapanPemenangVHSservice.GetApproval({
                ID: vm.ID
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.list = reply.data;
                    for (var i = 0; i < vm.list.length; i++) {
                        vm.crApps.push({
                            IsActive: vm.list[i].IsActive,
                            ID: vm.list[i].ID,
                            EmployeeID: vm.list[i].EmployeeID,
                            ApprovalDate: vm.list[i].ApprovalDate,
                            ApprovalStatus: vm.list[i].ApprovalStatus,
                            Remark: vm.list[i].Remark,
                            EmployeeFullName: vm.list[i].MstEmployee.FullName + ' ' + vm.list[i].MstEmployee.SurName,
                            EmployeePositionName: vm.list[i].MstEmployee.PositionName,
                            EmployeeDepartmentName: vm.list[i].MstEmployee.DepartmentName,
                            IsHighPriority: vm.list[i].IsHighPriority
                        });
                    }
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_APPROVERS'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_APPROVERS'));
            });
        }

        vm.approve = approve;
        function approve() {
            sendApproval(1);
        }

        vm.reject = reject;
        function reject() {
            sendApproval(0);
        }

        function sendApproval(approvalStatus) {
            UIControlService.loadLoadingModal(loadmsg);
            PenetapanPemenangVHSservice.SendApproval({
                ID: vm.ID,
                Status: approvalStatus,
                Remark: vm.information,
                flagEmp: vm.flagEmp
            }, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", $filter('translate')('Berhasil Approval'));
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