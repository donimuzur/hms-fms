(function () {
    'use strict';

    angular.module("app")
    .controller("detailApprovalCtrl", ctrl);

    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModalInstance', 'item', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'UIControlService', 'GlobalConstantService', 'PurchaseRequisitionService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModalInstance, item, $translate, $translatePartialLoader, $location, SocketService, UIControlService, GlobalConstantService, PurchReqService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";
        vm.ID = item.RFQID;
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
            PurchReqService.GetApproval({
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
                            EmployeeFullName: vm.list[i].employee.FullName + ' ' + vm.list[i].employee.SurName,
                            EmployeePositionName: vm.list[i].employee.PositionName,
                            EmployeeDepartmentName: vm.list[i].employee.DepartmentName,
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


        vm.cancel = cancel;
        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };

        //send approval
        vm.Approval = Approval;
        function Approval() {
            PurchReqService.Approval({ ID: vm.ID, Remark: vm.information }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "Berhasil Input Approval");
                    $uibModalInstance.close();
                } else {
                    UIControlService.msg_growl("error", "FORM.MSG_ERR_SAVE");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                UIControlService.unloadLoadingModal();
            });
        }

        //send Reject
        vm.Reject = Reject;
        function Reject() {
            PurchReqService.Reject({ ID: vm.ID, Remark: vm.information }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "Berhasil Reject");
                    $uibModalInstance.close();
                } else {
                    UIControlService.msg_growl("error", "FORM.MSG_ERR_SAVE");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                UIControlService.unloadLoadingModal();
            });
        }

    }
})();