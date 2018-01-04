(function () {
    'use strict';

    angular.module("app")
    .controller("uploadSOWDocsCPCtrl", ctrl);

    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService', 'GlobalConstantService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService, GlobalConstantService) {

        var vm = this;
        var contractRequisitionId = Number($stateParams.contractRequisitionId);
        vm.contractRequisitionId = Number($stateParams.contractRequisitionId);
        var loadmsg = "MESSAGE.LOADING";
        vm.isTenderVerification = true;

        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.sowDocs = [];

        vm.breadcrumbs = [
            { title: "BREADCRUMB.PROSES_PENGADAAN", href: "" },
            { title: "BREADCRUMB.VERIFIKASI_TENDER", href: "#/verifikasi-tender-cp" },
            { title: "BREADCRUMB.DETAIL_CONTRACT_REQUISITION", href: "#/verifikasi-tender-cp/detail-contract-requisition/" + $stateParams.contractRequisitionId },
            { title: "BREADCRUMB.SOW_DOCS", href: "" }
        ];

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
            DataContractRequisitionService.SelectSOWDocs({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status == 200) {
                    vm.sowDocs = reply.data;
                } else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_LOAD_SOW_DOC");
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                UIControlService.unloadLoading();
            });
        };        

        vm.back = back;
        function back() {
            $state.transitionTo('detail-contract-requisition-cp', { contractRequisitionId: contractRequisitionId });
        };
    }
})();