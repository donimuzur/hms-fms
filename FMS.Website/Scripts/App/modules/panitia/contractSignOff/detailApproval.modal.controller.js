(function () {
    'use strict';

    angular.module("app")
    .controller("detailApprovalSignOffCtrl", ctrl);

    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModalInstance', 'item', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'UIControlService', 'GlobalConstantService', 'ContractSignOffService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModalInstance, item, $translate, $translatePartialLoader, $location, SocketService, UIControlService, GlobalConstantService, ContractSignOffService) {

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
            ContractSignOffService.GetApproval({
                ID: vm.ID
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.list = reply.data;
                    console.info("list"+JSON.stringify(vm.list));
                    for (var j = 0; j < vm.list[0].CE.length; j++) {
                        if (j == 0) vm.employeeFullNameCE = vm.list[0].CE[j].FullName + ' ' + vm.list[0].CE[j].SurName;
                        else vm.employeeFullNameCE += ', ' + vm.list[0].CE[j].FullName + ' ' + vm.list[0].CE[j].SurName;
                    }
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

                
        vm.printForm = printForm;
        function printForm(formEEC) {
            $uibModalInstance.close();

            var innerContents = document.getElementById(formEEC).innerHTML;
            var popupWindow = window.open('', '', 'width=600,height=700,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no');
            popupWindow.document.open();
            popupWindow.document.write('<html><head><title>Form EEC-'+vm.list[0].ContractSignOff1.TenderStepData.tender.TenderName+'</title><link rel="stylesheet" type="text/css" media="print" href="assets/css/print.css" /></head><body onload="window.print()">' + innerContents + '</body></html>');
            popupWindow.document.close();

            //$uibModalInstance.close();
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
            ContractSignOffService.SendApproval({
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