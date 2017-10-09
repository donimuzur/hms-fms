(function () {
    'use strict';

    angular.module("app")
    .controller("detailCostEstimateCPCtrl", ctrl);
    
    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService) {

        var vm = this;
        var contractRequisitionId = Number($stateParams.contractRequisitionId);
        vm.contractRequisitionId = Number($stateParams.contractRequisitionId);
        var loadmsg = "MESSAGE.LOADING";
        vm.isTenderVerification = true;

        vm.breadcrumbs = [
            { title: "BREADCRUMB.PROSES_PENGADAAN", href: "" },
            { title: "BREADCRUMB.VERIFIKASI_TENDER", href: "#/verifikasi-tender-cp" },
            { title: "BREADCRUMB.DETAIL_CONTRACT_REQUISITION", href: "#/verifikasi-tender-cp/detail-contract-requisition/" + $stateParams.contractRequisitionId },
            { title: "BREADCRUMB.DETAIL_COST_ESTIMATE", href: "" }
        ];

        vm.count = 0;
        vm.pageNumber = 1;
        vm.pageSize = 10;
        vm.searchText = "";
        vm.ceLines = [];

        vm.isViewingXL = false;
        vm.ceLinesFromXL = [];

        vm.fileUpload;

        vm.onSearchSubmit = function (searchText) {
            vm.searchText = searchText;
            vm.pageNumber = 1;
            vm.loadData();
        };

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('data-contract-requisition');
            loadCRInfo();
            vm.loadData();
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
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        }

        vm.loadData = loadData;
        function loadData() {
            UIControlService.loadLoading(loadmsg);
            DataContractRequisitionService.SelectCELine({
                Parameter: contractRequisitionId,
                Keyword: vm.searchText,
                Limit: vm.pageSize,
                Offset: (vm.pageNumber - 1) * vm.pageSize,
                column: 1,
            }, function (reply) {
                if (reply.status === 200) {
                    vm.ceLines = reply.data.List;
                    vm.count = reply.data.Count;
                } else {
                    UIControlService.msg_growl('error', $filter('translate')('MESSAGE.ERR_LOAD_CELINE'));
                }
                UIControlService.unloadLoading();
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        };

        vm.back = back;
        function back() {
            $state.transitionTo('detail-contract-requisition-cp', { contractRequisitionId: contractRequisitionId });
        };

        vm.subCost = subCost;
        function subCost() {
            $state.transitionTo('atur-subcost-estimate-cp', { contractRequisitionId: contractRequisitionId });
        };
    }
})();