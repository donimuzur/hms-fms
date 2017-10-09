(function () {
    'use strict';

    angular.module("app")
    .controller("directAwardFormCtrl", ctrl);
    
    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModalInstance', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService', 'item'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModalInstance, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService, item) {

        var vm = this;
        var contractRequisitionId = item.contractRequisitionId;
        var loadmsg = "";

        vm.directAward = {};
        vm.isTenderVerification = item.IsTenderVerification;

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
                    if (reply.data){
                        vm.directAward = reply.data;
                        vm.directAward.EstimatedCost = vm.directAward.EstimatedCost + "";
                        vm.acknowledgementChecked = true;
                    }
                    else {
                        vm.directAward = {};
                        vm.directAward.ContractRequisitionId = contractRequisitionId;
                        vm.directAward.EstimatedCost = "1";
                    }                   

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
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD'));
            });
        };

        vm.selectRequestor = selectRequestor;
        function selectRequestor() {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-contract-requisition/formContractRequisition.selectEmployeeModal.html',
                controller: 'selectEmployeeModal',
                controllerAs: 'selectEmployeeCtrl',
            });
            modalInstance.result.then(function (selectedEmployee) {
                vm.directAward.Requestor = selectedEmployee.EmployeeID;
                vm.directAward.RequestorFullName = selectedEmployee.FullName + ' ' + selectedEmployee.SurName;
                vm.directAward.RequestorDepartmentId = selectedEmployee.DepartmentID;
                vm.directAward.RequestorDepartmentName = selectedEmployee.DepartmentName;
            });
        };

        vm.selectPM = selectPM;
        function selectPM() {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-contract-requisition/formContractRequisition.selectEmployeeModal.html',
                controller: 'selectEmployeeModal',
                controllerAs: 'selectEmployeeCtrl',
            });
            modalInstance.result.then(function (selectedEmployee) {
                vm.directAward.ContractRequisition.ProjectManager = selectedEmployee.EmployeeID;
                vm.directAward.ProjectManagerFullName = selectedEmployee.FullName + ' ' + selectedEmployee.SurName;
            });
        };

        vm.selectCompany = selectCompany;
        function selectCompany() {
            var item = {
                currentData: []
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-contract-requisition/formContractRequisition.selectVendorModal.html',
                controller: 'selectVendorModal',
                controllerAs: 'selectVendorCtrl',
                resolve: { item: function () { return item; } }
            });
            modalInstance.result.then(function (selectedVendor) {
                vm.directAward.Company = selectedVendor.VendorID;
                vm.directAward.CompanyName = selectedVendor.VendorName;

                UIControlService.loadLoadingModal(loadmsg);
                DataContractRequisitionService.GetVendorContact({
                    VendorID: selectedVendor.VendorID
                }, function (reply) {
                    UIControlService.unloadLoadingModal();
                    if (reply.status === 200) {
                        var contact = reply.data;
                        vm.directAward.CompanyContactName = contact.Name;
                        vm.directAward.CompanyPhone = contact.Phone;
                        vm.directAward.CompanyAddress = contact.AddressInfo;
                        vm.directAward.CompanyCityStateZip = contact.AddressDetail;
                    } else {
                        UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_ADDRESS'));
                    }
                }, function (error) {
                    UIControlService.unloadLoadingModal();
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_ADDRESS'));
                });
            });
        };

        vm.onSimpanClick = onSimpanClick;
        function onSimpanClick() {
            var checkField = checkRequiredField();
            if (!checkField) {
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_INCOMPLETE_FIELD'));
                return;
            }
            if (!vm.acknowledgementChecked) {
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_NO_ACKNOWLEDGEMENT'));
                return;
            }

            UIControlService.loadLoadingModal(loadmsg);
            DataContractRequisitionService.SaveDirectAward(vm.directAward, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SAVE_DA'));
                    $uibModalInstance.close({
                        ProjectManager: vm.directAward.ContractRequisition.ProjectManager,
                        ProjectManagerFullName: vm.directAward.ProjectManagerFullName
                    });
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE_DA'));
                }
            }, function (error) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE_DA'));
            });
        };

        function checkRequiredField() {
            if (!vm.directAward.DescOfReqService) {
                return false;
            }
            return true;
        }

        vm.onBatalClick = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();