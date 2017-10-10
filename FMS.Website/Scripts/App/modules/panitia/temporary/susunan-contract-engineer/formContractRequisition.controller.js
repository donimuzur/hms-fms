(function () {
    'use strict';

    angular.module("app")
    .controller("formContractReqCtrl", ctrl);
    
    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService) {

        var vm = this;
        var contractRequisitionId = Number($stateParams.contractRequisitionId);
        vm.contractRequisitionId = Number($stateParams.contractRequisitionId);
        var loadmsg = "";

        vm.contractRequisition = {};
        vm.isCalendarOpened = [false, false, false, false];
        vm.budgetDistValue = null;
        vm.budgetDistYear = null;
        vm.duration = 0;
        vm.isTenderVerification = false;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('data-contract-requisition');
            $translate.refresh().then(function () {
                loadmsg = $filter('translate')('MESSAGE.LOADING');
            });

            UIControlService.loadLoading(loadmsg);
            vm.loadData();
        };

        vm.loadData = loadData;
        function loadData() {
            UIControlService.loadLoading(loadmsg);
            DataContractRequisitionService.SelectById2({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.contractRequisition = reply.data;
                    vm.contractRequisition.DirectAward = vm.contractRequisition.DirectAward ? "1" : "0";
                    vm.contractRequisition.BudgetStatus = vm.contractRequisition.BudgetStatus ? "1" : "0";
                    vm.contractRequisition.OperatingOrCapital = vm.contractRequisition.OperatingOrCapital ? "1" : "0";
                    vm.isTenderVerification = vm.contractRequisition.StatusName !== 'CR_DRAFT' && vm.contractRequisition.StatusName !== 'CR_REJECT_2';

                    if (!vm.contractRequisition.ContractRequisitionBudgetDists) {
                        vm.contractRequisition.ContractRequisitionBudgetDists = [];
                    }
                    if (!vm.contractRequisition.ContractRequisitionVendorSuggestions) {
                        vm.contractRequisition.ContractRequisitionVendorSuggestions = [];
                    }
                    convertToDate();
                    vm.getDuration();
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        };

        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        };

        vm.getDuration = getDuration;
        function getDuration() {
            vm.duration = (vm.contractRequisition.FinishDate - vm.contractRequisition.StartDate) / 1000 / 60 / 60 / 24;
            if (vm.duration < 0) {
                vm.duration = 0;
            }
        }

        vm.selectOwner = selectOwner;
        function selectOwner() {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-contract-requisition/formContractRequisition.selectEmployeeModal.html',
                controller: 'selectEmployeeModal',
                controllerAs: 'selectEmployeeCtrl',
            });
            modalInstance.result.then(function (selectedOwner) {
                vm.contractRequisition.ProjectOwner = selectedOwner.EmployeeID;
                vm.contractRequisition.ProjectOwnerName = selectedOwner.FullName;
            });
        };

        vm.addVendorSugg = addVendorSugg;
        function addVendorSugg() {
            var item = {
                currentData : vm.contractRequisition.ContractRequisitionVendorSuggestions
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-contract-requisition/formContractRequisition.selectVendorModal.html',
                controller: 'selectVendorModal',
                controllerAs: 'selectVendorCtrl',
                resolve: { item: function () { return item; } }
            });
            modalInstance.result.then(function (selectedVendor) {
                vm.contractRequisition.ContractRequisitionVendorSuggestions.push({
                    VendorID: selectedVendor.VendorID,
                    VendorName: selectedVendor.VendorName,
                    ContractRequisitionId : contractRequisitionId,
                });
            });
        };

        vm.removeVendorSugg = removeVendorSugg;
        function removeVendorSugg(index) {
            vm.contractRequisition.ContractRequisitionVendorSuggestions.splice(index, 1);
        }

        vm.directAward = directAward;
        function directAward() {
            var item = {
                contractRequisitionId: contractRequisitionId,
                ProjectManager: vm.contractRequisition.ProjectManager,
                ProjectManagerFullName: vm.contractRequisition.ProjectManagerName,
                IsTenderVerification: vm.isTenderVerification
            };
            var modalInstance = $uibModal.open({
                templateUrl: "app/modules/panitia/data-contract-requisition/directAwardForm.html",
                controller: "directAwardFormCtrl",
                controllerAs: "daFormCtrl",
                resolve: { item: function () { return item; } }
            });
        };

        vm.addBudgetDist = addBudgetDist;
        function addBudgetDist() {
            for (var i = 0; i < vm.contractRequisition.ContractRequisitionBudgetDists.length; i++){
                if (vm.budgetDistYear === vm.contractRequisition.ContractRequisitionBudgetDists[i].Year) {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_DUPLICATE_YEAR'));
                    return;
                }
            }
            vm.contractRequisition.ContractRequisitionBudgetDists.push({
                ContractRequisitionId : contractRequisitionId,
                Value : vm.budgetDistValue,
                Year : vm.budgetDistYear
            });
            vm.budgetDistValue = null;
            vm.budgetDistYear = null;
            vm.contractRequisition.ContractRequisitionBudgetDists.sort(function (a, b) {
                return a.Year - b.Year;
            });
        };

        vm.removeBudgetDist = removeBudgetDist;
        function removeBudgetDist(index) {
            vm.contractRequisition.ContractRequisitionBudgetDists.splice(index, 1);
        };

        vm.save = save;
        function save() {
            var checkField = checkRequiredField();
            if (!checkField) {
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_INCOMPLETE_FIELD'));
                return;
            }
            if (Number(vm.contractRequisition.TotalValue) > Number(vm.contractRequisition.ApprovedBudget)) {
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_TOTAL_VALUE'));
                return;
            }

            UIControlService.loadLoading(loadmsg);
            vm.contractRequisition.DirectAward = vm.contractRequisition.DirectAward == "1";
            vm.contractRequisition.BudgetStatus = vm.contractRequisition.BudgetStatus == "1";
            vm.contractRequisition.OperatingOrCapital = vm.contractRequisition.OperatingOrCapital == "1";
            convertAllDateToString();
            DataContractRequisitionService.Update2(vm.contractRequisition, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SAVE'));
                    loadData();
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        };

        function checkRequiredField() {
            return vm.contractRequisition.ContractDetailsInfo && vm.contractRequisition.RequestedDate && vm.contractRequisition.RequiredDate
                && vm.contractRequisition.StartDate && vm.contractRequisition.FinishDate && vm.contractRequisition.ProjectOwner
                && vm.contractRequisition.OutsourcingRequest && vm.contractRequisition.Project
                && vm.contractRequisition.WBS && vm.contractRequisition.TotalValue
                && vm.contractRequisition.JustForRequest && vm.contractRequisition.CostAnalysis;
        }

        function convertAllDateToString() {
            if (vm.contractRequisition.RequestedDate) {
                vm.contractRequisition.RequestedDate = UIControlService.getStrDate(vm.contractRequisition.RequestedDate);
            }
            if (vm.contractRequisition.RequiredDate) {
                vm.contractRequisition.RequiredDate = UIControlService.getStrDate(vm.contractRequisition.RequiredDate);
            }
            if (vm.contractRequisition.StartDate) {
                vm.contractRequisition.StartDate = UIControlService.getStrDate(vm.contractRequisition.StartDate);
            }
            if (vm.contractRequisition.FinishDate) {
                vm.contractRequisition.FinishDate = UIControlService.getStrDate(vm.contractRequisition.FinishDate);
            }
        };

        //Supaya muncul di date picker saat awal load
        function convertToDate(){
            if (vm.contractRequisition.RequestedDate) {
                vm.contractRequisition.RequestedDate = new Date(Date.parse(vm.contractRequisition.RequestedDate));
            }
            if (vm.contractRequisition.RequiredDate) {
                vm.contractRequisition.RequiredDate = new Date(Date.parse(vm.contractRequisition.RequiredDate));
            }
            if (vm.contractRequisition.StartDate) {
                vm.contractRequisition.StartDate = new Date(Date.parse(vm.contractRequisition.StartDate));
            }
            if (vm.contractRequisition.FinishDate) {
                vm.contractRequisition.FinishDate = new Date(Date.parse(vm.contractRequisition.FinishDate));
            }
        }

        vm.back = back;
        function back() {
            $state.transitionTo('detail-contract-requisition-contract', { contractRequisitionId: contractRequisitionId });
        };
    }
})();