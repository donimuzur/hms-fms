(function () {
    'use strict';

    angular.module("app")
    .controller("detailApprovalBlacklistCtrl", ctrl);

    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModalInstance', 'item', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'UIControlService', 'GlobalConstantService', 'BlacklistService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModalInstance, item, $translate, $translatePartialLoader, $location, SocketService, UIControlService, GlobalConstantService,BlacklistService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";
        vm.Id = item.TenderId;
        vm.ID = item.ID;
        vm.Status = item.Status;
        vm.crApps = [];
        vm.employeeFullName = "";
        vm.employeeID = 0;
        vm.information = "";
        vm.BlacklistID = item.data.BlacklistID;
        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('verifikasi-tender');
           loadData();
        };

        vm.loadData = loadData;
        function loadData() {
            vm.crApps = [];
            UIControlService.loadLoading("Silahkan tunggu");
           BlacklistService.GetCRApprovals({
               BlacklistID: vm.BlacklistID
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.list = reply.data;
                    console.info(vm.list);
                      //vm.crApps.push({
                        //    IsActive: vm.list[i].IsActive,
                        //    ID: vm.list[i].ID,
                        //    EmployeeID: vm.list[i].EmployeeID,
                        //    ApprovalDate: vm.list[i].ApprovalDate,
                        //    ApprovalStatus: vm.list[i].ApprovalStatus,
                        //    Remark: vm.list[i].Remark,
                        //    EmployeeFullName: vm.list[i].MstEmployee.FullName + ' ' + vm.list[i].MstEmployee.SurName,
                        //    EmployeePositionName: vm.list[i].MstEmployee.PositionName,
                        //    EmployeeDepartmentName: vm.list[i].MstEmployee.DepartmentName
                        //});
                    //for (var i = 0; i < vm.list.length; i++) {
                    //    vm.crApps.push({
                    //        IsActive: vm.list[i].IsActive,
                    //        ID: vm.list[i].ID,
                    //        EmployeeID: vm.list[i].EmployeeID,
                    //        ApprovalDate: vm.list[i].ApprovalDate,
                    //        ApprovalStatus: vm.list[i].ApprovalStatus,
                    //        Remark: vm.list[i].Remark,
                    //        EmployeeFullName: vm.list[i].MstEmployee.FullName + ' ' + vm.list[i].MstEmployee.SurName,
                    //        EmployeePositionName: vm.list[i].MstEmployee.PositionName,
                    //        EmployeeDepartmentName: vm.list[i].MstEmployee.DepartmentName
                    //    });
                    //}
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
           BlacklistService.SetApprovalStatus({
                ID: vm.ID,
                TenderReffId: vm.Id,
                Status: approvalStatus,
                Remark: vm.information
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