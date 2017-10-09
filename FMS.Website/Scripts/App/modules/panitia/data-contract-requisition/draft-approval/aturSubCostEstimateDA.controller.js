(function () {
    'use strict';

    angular.module("app")
    .controller("aturSubCostEstimateDACtrl", ctrl);
    
    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService) {

        var vm = this;
        var contractRequisitionId = Number($stateParams.contractRequisitionId);
        vm.contractRequisitionId = Number($stateParams.contractRequisitionId);
        var loadmsg = "";
        vm.isTenderVerification = true;

        vm.breadcrumbs = [
            { title: "BREADCRUMB.MASTER_REQUISITION", href: "" },
            { title: "BREADCRUMB.DATA_CONTRACT_REQUISITION", href: "#/data-contract-requisition" },
            { title: "BREADCRUMB.APPROVAL", href: "#/data-contract-requisition/approval" },
            { title: "BREADCRUMB.DETAIL_CONTRACT_REQUISITION", href: "#/data-contract-requisition/approval/detail-contract-requisition/" + $stateParams.contractRequisitionId },
            { title: "BREADCRUMB.DETAIL_COST_ESTIMATE", href: "#/data-contract-requisition/approval/detail-contract-requisition/detail-cost-estimate/" + $stateParams.contractRequisitionId },
            { title: "BREADCRUMB.SUB_COST_ESTIMATE", href: "" }
        ];

        vm.userBisaMengatur = false;
        vm.ceSub = [];
        vm.totalHPS = 0;
        vm.searchText = '';

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('data-contract-requisition');
            $translate.refresh().then(function () {
                loadmsg = $filter('translate')('MESSAGE.LOADING');
            });
            DataContractRequisitionService.IsApprover({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    if (reply.data === true) {
                        loadCRInfo();
                        vm.loadData();
                    } else {
                        UIControlService.msg_growl("warning", $filter('translate')('MESSAGE.ERR_NOT_APPROVER'));
                        $state.transitionTo('requisition-list');
                    }
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_CHK_APPROVER'));
                    $state.transitionTo('requisition-list');
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_CHK_APPROVER'));
                $sta$state.transitionTo('requisition-list');
            });
        };

        function loadCRInfo() {
            DataContractRequisitionService.SelectById({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.currencySymbol = reply.data.CurrencySymbol;
                    vm.ProjectTitle = reply.data.ProjectTitle;
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_DET'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_DET'));
            });
        }

        vm.onSearchSubmit = function (searchText) {
            vm.searchText = searchText;
            vm.loadData();
        };

        vm.loadData = loadData;
        function loadData() {
            UIControlService.loadLoading(loadmsg);
            DataContractRequisitionService.SelectCESub({
                ContractRequisitionId: contractRequisitionId,
                Name: vm.searchText
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.ceSub = reply.data;
                    vm.totalHPS = 0;
                    vm.ceSub.forEach(function (sub) {
                        vm.totalHPS += sub.TotalLineCost;
                    })
                }
                else {
                    UIControlService.msg_growl('error', $filter('translate')('MESSAGE.ERR_LOAD_CELINE'));
                }
            }, function (err){
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_CELINE'));
            });
        }

        vm.romanize = romanize;
        function romanize(num) {
            if (!+num)
                return false;
            var digits = String(+num).split(""),
                    key = ["", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM",
                        "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC",
                        "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX"],
                    roman = "",
                    i = 3;
            while (i--)
                roman = (key[+digits.pop() + (i * 10)] || "") + roman;
            return Array(+digits.join("") + 1).join("M") + roman;
        }
                
        vm.detail = detail;
        function detail(ContractRequisitionCESubID) {
            var lempar = {
                contractRequisitionCESubID: ContractRequisitionCESubID
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-contract-requisition/aturSubCostEstimate.modalDet.html',
                controller: 'aturSubCostEstimateDetCtrl',
                controllerAs: 'aturSubCEDetCtrl',
                resolve: {
                    item: function () {
                        return lempar;
                    }
                }
            });
        };

        vm.back = back;
        function back() {
            $state.transitionTo('detail-cost-estimate-da', { contractRequisitionId: contractRequisitionId });
        };
    }
})();