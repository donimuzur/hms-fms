(function () {
    'use strict';

    angular.module("app")
    .controller("formContractVarDACtrl", ctrl);
    
    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService) {

        var vm = this;
        var contractRequisitionId = Number($stateParams.contractRequisitionId);
        var contractRequisitionVariationId = Number($stateParams.contractRequisitionVariationId);
        var action = $stateParams.action;

        var loadmsg = "";

        vm.contractRecVar = {};
        vm.isCalendarOpened = [false, false, false, false];
        vm.isTenderVerification = true;
        vm.contractReqVar = {};
        vm.reasonRefs = [];

        vm.breadcrumbs = [
            { title: "BREADCRUMB.MASTER_REQUISITION", href: "" },
            { title: "BREADCRUMB.DATA_CONTRACT_REQUISITION", href: "#/data-contract-requisition" },
            { title: "BREADCRUMB.APPROVAL", href: "#/data-contract-requisition/approval" },
            { title: "BREADCRUMB.DETAIL_CONTRACT_REQUISITION", href: "#/data-contract-requisition/approval/detail-contract-requisition/" + $stateParams.contractRequisitionId },
            { title: "BREADCRUMB.DATA_CONTRACT_VARIATION", href: "" }
        ];

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('contract-variation');
            $translate.refresh().then(function () {
                loadmsg = $filter('translate')('MESSAGE.LOADING');
            });
            DataContractRequisitionService.IsApprover({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    if (reply.data === true) {
                        vm.loadData();
                    } else {
                        UIControlService.msg_growl("warning", $filter('translate')('MESSAGE.ERR_NOT_APPROVER'));
                        $state.transitionTo('contract-requisition-draft-approval');
                    }
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_CHK_APPROVER'));
                    $state.transitionTo('contract-requisition-draft-approval');
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_CHK_APPROVER'));
                $state.transitionTo('contract-requisition-draft-approval');
            });
        };

        vm.loadData = loadData;
        function loadData() {
            UIControlService.loadLoading(loadmsg);
            DataContractRequisitionService.GetCRVariationReasons(function (reply) {
                UIControlService.unloadLoading();
                vm.reasonRefs = reply.data;
                loadDataForEdit();
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_REASONS'));
            });
        };

        function loadDataForEdit() {
            UIControlService.loadLoading(loadmsg);
            DataContractRequisitionService.GetVariationByID({
                ID: contractRequisitionVariationId
            }, function (reply) {                
                vm.contractRecVar = reply.data;
                vm.contractRecVar.IsAnnualPlan = vm.contractRecVar.IsAnnualPlan ? "1" : "0";
                vm.contractRecVar.OriginalStartDate = new Date(Date.parse(vm.contractRecVar.OriginalStartDate));
                vm.contractRecVar.OriginalEndDate = new Date(Date.parse(vm.contractRecVar.OriginalEndDate));
                convertToDate();
                vm.isTenderVerification = vm.contractRecVar.StatusName !== 'CR_DRAFT' && vm.contractRecVar.StatusName.lastIndexOf('CR_REJECT_', 0) !== 0;
                if (vm.contractRecVar.ReasonRefs && vm.contractRecVar.ReasonRefs.length > 0) {
                    for (var i = 0; i < vm.reasonRefs.length; i++) {
                        vm.reasonRefs[i].isChecked = false;
                        for (var j = 0; j < vm.contractRecVar.ReasonRefs.length; j++) {
                            if (vm.reasonRefs[i].RefID === vm.contractRecVar.ReasonRefs[j].RefID) {
                                vm.reasonRefs[i].isChecked = true;
                                break;
                            }
                        }
                    }
                }
                if (vm.contractRecVar.CurrencySymbol === 'IDR') {
                    vm.contractRecVar.OriginalValueInUSD = vm.contractRecVar.OriginalValue * vm.contractRecVar.RateIDRToUSD;
                    vm.contractRecVar.ContractCommitmentTotalValueInUSD = vm.contractRecVar.ContractCommitmentTotalValue * vm.contractRecVar.RateIDRToUSD;
                }
                calculateVariationPercent();
                UIControlService.unloadLoading();
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD'));
            });
        }

        function calculateVariationPercent() {
            
            //Variasi nilai
            vm.contractRecVar.NewTotalValue =
                Number(vm.contractRecVar.ContractCommitmentTotalValue) + Number(vm.contractRecVar.FurtherCommitmentTotalValue);
            var originalValue = vm.contractRecVar.OriginalValue;
            var newValue = vm.contractRecVar.NewTotalValue;
            var percentageValue = originalValue > 0 ? (newValue * 100 / originalValue) - 100 : 0;
            vm.contractValueVariationPercent = Number(percentageValue.toFixed(2));
            if (vm.contractValueVariationPercent < 0) {
                vm.contractValueVariationPercent = 0;
            }

            if (vm.contractRecVar.CurrencySymbol === 'IDR') {
                vm.contractRecVar.FurtherCommitmentTotalValueInUSD = Number(vm.contractRecVar.FurtherCommitmentTotalValue) * vm.contractRecVar.RateIDRToUSD;
                vm.contractRecVar.NewTotalValueInUSD = vm.contractRecVar.NewTotalValue * vm.contractRecVar.RateIDRToUSD;
            }

            //variasi tanggal
            vm.contractDateVariationPercent = 0;
            if (vm.NewPlannedEndDate) {
                var originalDuration = (vm.contractRecVar.OriginalEndDate - vm.contractRecVar.OriginalStartDate);
                var newDuration = (vm.NewPlannedEndDate - vm.contractRecVar.OriginalStartDate);
                var percentageDuration = originalDuration > 0 ? (newDuration * 100 / originalDuration) - 100 : 0;
                vm.contractDateVariationPercent = Number(percentageDuration.toFixed(2));
            }
            if (vm.contractDateVariationPercent < 0) {
                vm.contractDateVariationPercent = 0;
            }
        };

        //Supaya muncul di date picker saat awal load
        function convertToDate(){
            if (vm.contractRecVar.DateRequested) {
                vm.DateRequested = new Date(Date.parse(vm.contractRecVar.DateRequested));
            }
            if (vm.contractRecVar.DateRequired) {
                vm.DateRequired = new Date(Date.parse(vm.contractRecVar.DateRequired));
            }
            if (vm.contractRecVar.CurrentEndDate) {
                vm.CurrentEndDate = new Date(Date.parse(vm.contractRecVar.CurrentEndDate));
            }
            if (vm.contractRecVar.NewPlannedEndDate) {
                vm.NewPlannedEndDate = new Date(Date.parse(vm.contractRecVar.NewPlannedEndDate));
            }
        }

        vm.back = back;
        function back() {
            $state.transitionTo('detail-contract-requisition-da', { contractRequisitionId: contractRequisitionId });
        };
    }
})();