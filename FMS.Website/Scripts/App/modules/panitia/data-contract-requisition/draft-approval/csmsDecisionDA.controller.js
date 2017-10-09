(function () {
    'use strict';

    angular.module("app")
    .controller("csmsDecisionDACtrl", ctrl);
    
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
            { title: "BREADCRUMB.MASTER_REQUISITION", href: "" },
            { title: "BREADCRUMB.DATA_CONTRACT_REQUISITION", href: "#/data-contract-requisition" },
            { title: "BREADCRUMB.APPROVAL", href: "#/data-contract-requisition/approval" },
            { title: "BREADCRUMB.DETAIL_CONTRACT_REQUISITION", href: "#/data-contract-requisition/approval/detail-contract-requisition/" + $stateParams.contractRequisitionId },
            { title: "BREADCRUMB.CSMS_DECISION", href: "" }
        ];

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('data-contract-requisition');
            $translate.refresh().then(function () {
                loadmsg = $filter('translate')('MESSAGE.LOADING');
            });
            UIControlService.unloadLoading(loadmsg);
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
                $sta$state.transitionTo('contract-requisition-draft-approval');
            });
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
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_CSMS'));
            });
        };

        vm.cancel = cancel;
        function cancel() {
            $state.transitionTo('detail-contract-requisition-da', { contractRequisitionId: contractRequisitionId });
        };
    }
})();