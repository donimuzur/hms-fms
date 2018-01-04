(function () {
    'use strict';

    angular.module("app")
    .controller("csmsDecisionCPCtrl", ctrl);
    
    ctrl.$inject = ['$state', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService) {

        var vm = this;
        var contractRequisitionId = Number($stateParams.contractRequisitionId);
        vm.contractRequisitionId = Number($stateParams.contractRequisitionId);
        var loadmsg = "";
        vm.isTenderVerification = true;

        vm.projectTitle = "";
        vm.contractSponsor = "";
        vm.projectManager = 0;
        vm.projectManagerName = "";
        vm.operatingOrCapital = "0";
        vm.departmentName = "";
        vm.csms = {}

        vm.breadcrumbs = [
            { title: "BREADCRUMB.PROSES_PENGADAAN", href: "" },
            { title: "BREADCRUMB.VERIFIKASI_TENDER", href: "#/verifikasi-tender-cp" },
            { title: "BREADCRUMB.DETAIL_CONTRACT_REQUISITION", href: "#/verifikasi-tender-cp/detail-contract-requisition/" + $stateParams.contractRequisitionId },
            { title: "BREADCRUMB.CSMS_DECISION", href: "" }
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
            DataContractRequisitionService.SelectCSMS({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                if (reply.status === 200) {
                    vm.csms = reply.data;
                    vm.csms.IsSupervised = vm.csms.IsSupervised ? "1" : "0";
                    vm.csms.IsAssessed = vm.csms.IsAssessed ? "1" : "0";
                    vm.csms.IsHighRisk = vm.csms.IsHighRisk ? "1" : "0";

                    var contractRequisition = vm.csms.ContractRequisition;
                    vm.projectTitle = contractRequisition.ProjectTitle;
                    vm.contractSponsor = contractRequisition.ContractSponsor;
                    vm.projectManager = contractRequisition.ProjectManager;
                    vm.projectManagerName = contractRequisition.ProjectManagerName;
                    vm.departmentName = contractRequisition.DepartmentName;
                    if (contractRequisition.OperatingOrCapital !== null) {
                        vm.OperatingOrCapital = contractRequisition.OperatingOrCapital === true ? '1' : '0';
                        vm.operatingOrCapitalText = contractRequisition.OperatingOrCapital === true ? 'CAPITAL' : 'OPERATING';
                        vm.operatingOrCapitalText = $filter('translate')(vm.operatingOrCapitalText);
                    }
                    UIControlService.unloadLoading();
                } else {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_CSMS'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        };

        vm.cancel = cancel;
        function cancel() {
            $state.transitionTo('detail-contract-requisition-cp', { contractRequisitionId: contractRequisitionId });
        };
    }
})();