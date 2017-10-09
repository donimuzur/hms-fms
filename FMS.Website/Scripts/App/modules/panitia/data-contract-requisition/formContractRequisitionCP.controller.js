(function () {
    'use strict';

    angular.module("app")
    .controller("formContractReqCPCtrl", ctrl);
    
    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService) {

        var vm = this;
        var contractRequisitionId = Number($stateParams.contractRequisitionId);
        vm.contractRequisitionId = Number($stateParams.contractRequisitionId);
        var loadmsg = "";
        vm.isTenderVerification = true;

        vm.contractRequisition = {};
        vm.isCalendarOpened = [false, false, false, false];
        vm.budgetDistValue = null;
        vm.budgetDistYear = null;
        vm.duration = 0;

        vm.breadcrumbs = [
            { title: "BREADCRUMB.PROSES_PENGADAAN", href: "" },
            { title: "BREADCRUMB.VERIFIKASI_TENDER", href: "#/verifikasi-tender-cp" },
            { title: "BREADCRUMB.DETAIL_CONTRACT_REQUISITION", href: "#/verifikasi-tender-cp/detail-contract-requisition/" + $stateParams.contractRequisitionId },
            { title: "BREADCRUMB.CREATE_CONTRACT_REQUISITION", href: "" }
        ];

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
            UIControlService.loadLoading(loadmsg);
            DataContractRequisitionService.SelectById2({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.contractRequisition = reply.data;
                    vm.contractRequisition.DirectAward = vm.contractRequisition.DirectAward ? "1" : "0";
                    vm.contractRequisition.BudgetStatus = vm.contractRequisition.BudgetStatus ? "1" : "0";
                    vm.contractRequisition.OperatingOrCapitalText = vm.contractRequisition.OperatingOrCapital ? 'CAPITAL' : 'OPERATING';
                    vm.contractRequisition.OperatingOrCapitalText = $filter('translate')(vm.contractRequisition.OperatingOrCapitalText);
                    vm.contractRequisition.MstCurrency = {
                        Symbol: vm.contractRequisition.CurrencySymbol ? vm.contractRequisition.CurrencySymbol : "USD"
                    };
                    convertAllToUSD();
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

        vm.convertAllToUSD = convertAllToUSD;
        function convertAllToUSD() {
            convertAppBudgetToUSD();
            convertOutBudgetToUSD();
            convertTotalValueToUSD();
        }

        vm.convertAppBudgetToUSD = convertAppBudgetToUSD;
        function convertAppBudgetToUSD() {
            if (vm.contractRequisition.MstCurrency.Symbol !== "USD") {
                vm.contractRequisition.ApprovedBudgetInUSD = Number(vm.contractRequisition.ApprovedBudget) * vm.contractRequisition.RateIDRToUSD;
            }
        }

        vm.convertOutBudgetToUSD = convertOutBudgetToUSD;
        function convertOutBudgetToUSD() {
            if (vm.contractRequisition.MstCurrency.Symbol !== "USD") {
                vm.contractRequisition.OutstandingBudgetInUSD = Number(vm.contractRequisition.OutstandingBudget) * vm.contractRequisition.RateIDRToUSD;
            }
        }

        vm.convertTotalValueToUSD = convertTotalValueToUSD;
        function convertTotalValueToUSD() {
            if (vm.contractRequisition.MstCurrency.Symbol !== "USD") {
                vm.contractRequisition.TotalValueInUSD = Number(vm.contractRequisition.TotalValue) * vm.contractRequisition.RateIDRToUSD;
            }
        }

        vm.getDuration = getDuration;
        function getDuration() {
            vm.duration = (vm.contractRequisition.FinishDate - vm.contractRequisition.StartDate) / 1000 / 60 / 60 / 24;
            if (vm.duration < 0) {
                vm.duration = 0;
            }
        }

        vm.directAward = directAward;
        function directAward() {
            var item = {
                contractRequisitionId: contractRequisitionId,
                ProjectManager: vm.contractRequisition.ProjectManager,
                ProjectManagerFullName: vm.contractRequisition.ProjectManagerName
            };
            var modalInstance = $uibModal.open({
                templateUrl: "app/modules/panitia/data-contract-requisition/directAwardForm.html",
                controller: "directAwardFormCPCtrl",
                controllerAs: "daFormCtrl",
                resolve: { item: function () { return item; } }
            });
        };        

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
            $state.transitionTo('detail-contract-requisition-cp', { contractRequisitionId: contractRequisitionId });
        };
    }
})();