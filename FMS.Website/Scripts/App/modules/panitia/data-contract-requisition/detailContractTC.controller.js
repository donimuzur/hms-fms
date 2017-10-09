(function () {
    'use strict';

    angular.module("app")
    .controller("detailContractReqTCCtrl", ctrl);
    
    ctrl.$inject = ['$state', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'UIControlService', 'TenderVerificationService', 'RequisitionListService'];
    /* @ngInject */
    function ctrl($state, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, UIControlService, TenderVerificationService, RequisitionListService) {

        var vm = this;
        var contractRequisitionId = Number($stateParams.contractRequisitionId);
        var loadmsg = "MESSAGE.LOADING";

        vm.breadcrumbs = [
            { title: "BREADCRUMB.PROSES_PENGADAAN", href: "" },
            { title: "BREADCRUMB.REQUISITION_LIST", href: "#/requisition-list" },
            { title: "BREADCRUMB.DETAIL_CONTRACT_REQUISITION", href: "" }
        ];

        vm.projectTitle = "";
        vm.contractSponsor = "";
        vm.projectManager = "";
        vm.isTenderVerification = true;
        vm.isOnProcess = false;

        vm.formList = [];

        vm.statusLabels = [];
        vm.statusLabels["CR_PROCESS_2"] = 'STATUS.ON_PROCESS';
        vm.statusLabels["CR_PROCESS_3"] = 'STATUS.REVIEWED';
        vm.statusLabels["CR_REJECT_2"] = 'STATUS.UNCOMPLETE';
        vm.statusLabels["CR_REJECT_3"] = 'STATUS.PENDING';

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('data-contract-requisition');
            UIControlService.unloadLoading(loadmsg);
            RequisitionListService.IsApprover({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    if (reply.data === true) {
                        loadDetails();
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
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
                $state.transitionTo('requisition-list');
            });
        };

        function loadDetails() {
            vm.formList = [];
            TenderVerificationService.SelectDetail({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var contractRequisition = reply.data;
                    vm.projectTitle = contractRequisition.ProjectTitle;
                    vm.contractSponsor = contractRequisition.ContractSponsor;
                    vm.projectManager = contractRequisition.ProjectManager;
                    vm.projectManagerName = contractRequisition.ProjectManagerName;
                    vm.contractRequisitionVariationId = contractRequisition.ContractRequisitionVariationId;
                    vm.statusName = $filter('translate')(vm.statusLabels[contractRequisition.StatusName]);

                    var details = contractRequisition.ContractRequisitionDetails;
                    if (details) {
                        for (var i = 0; i < details.length; i++) {
                            var formList = {
                                ContractRequisitionId: details[i].ContractRequisitionId,
                                RefContractRequisitionDetailId: details[i].RefContractRequisitionDetailId,
                                DetailName: details[i].DetailName,
                                LinkState: details[i].LinkState,
                                VerificationStatus: details[i].VerificationStatus,
                                Information: details[i].Information
                            }
                            vm.formList.push(formList);
                        }
                    }
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_DET'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        };

        vm.gotoDetail = gotoDetail;
        function gotoDetail(dt) {
            if (dt.LinkState === 'direct-award-form') {
                var item = {
                    contractRequisitionId: contractRequisitionId,
                    ProjectManager: vm.projectManager,
                    ProjectManagerFullName: vm.projectManagerName
                };
                var modalInstance = $uibModal.open({
                    templateUrl: "app/modules/panitia/data-contract-requisition/directAwardForm.html",
                    controller: "directAwardFormCPCtrl",
                    controllerAs: "daFormCtrl",
                    resolve: { item: function () { return item; } }
                });
            } else {
                if (vm.contractRequisitionVariationId > 0 && dt.LinkState === 'form-contract-requisition') {
                    $state.transitionTo('contract-variation-tc', { contractRequisitionId: contractRequisitionId, contractRequisitionVariationId: vm.contractRequisitionVariationId });
                }
                else {
                    $state.transitionTo(dt.LinkState + '-tc', { contractRequisitionId: contractRequisitionId });
                }
            }
        };

        vm.back = back;
        function back(dt) {
            $state.transitionTo('requisition-list');
        };
    };
})();