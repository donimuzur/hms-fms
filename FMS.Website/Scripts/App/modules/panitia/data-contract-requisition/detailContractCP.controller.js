(function () {
    'use strict';

    angular.module("app")
    .controller("detailContractReqCPCtrl", ctrl);
    
    ctrl.$inject = ['$state', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'UIControlService', 'TenderVerificationService'];
    /* @ngInject */
    function ctrl($state, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, UIControlService, TenderVerificationService) {

        var vm = this;
        var contractRequisitionId = Number($stateParams.contractRequisitionId);
        var loadmsg = "MESSAGE.LOADING";

        vm.breadcrumbs = [
            { title: "BREADCRUMB.PROSES_PENGADAAN", href: "" },
            { title: "BREADCRUMB.VERIFIKASI_TENDER", href: "#/verifikasi-tender-cp" },
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
            loadDetails();
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
                    vm.isOnProcess = contractRequisition.StatusName == 'CR_PROCESS_2';
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
                    $state.transitionTo('contract-variation-cp', { contractRequisitionId: contractRequisitionId, contractRequisitionVariationId: vm.contractRequisitionVariationId });
                }
                else {
                    $state.transitionTo(dt.LinkState + '-cp', { contractRequisitionId: contractRequisitionId });
                }
            }
        };

        vm.accept = accept;
        function accept() {

            for (var i = 0; i < vm.formList.length; i++) {
                if (!vm.formList[i].VerificationStatus) {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_NOT_ALL_VERIFIED'));
                    return;
                }
            }

            UIControlService.loadLoadingModal(loadmsg);
            TenderVerificationService.GetCRApprovals({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200 && reply.data.ContractRequisitionApprovals.length > 0) {
                    save('CR_PROCESS_3');
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_NO_APPROVERS'));
                }
            }, function (error) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        };

        vm.reject = reject;
        function reject() {
            save('CR_REJECT_2');
        };

        function save(statusName) {
            UIControlService.loadLoading(loadmsg);
            TenderVerificationService.SaveReview({
                ContractRequisitionId: contractRequisitionId,
                ContractRequisitionDetails: vm.formList,
                StatusName: statusName
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SAVE_REV'));
                    loadDetails();
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE_REV'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        };

        vm.back = back;
        function back(dt) {
            $state.transitionTo('verifikasi-tender-cp');
        };
    };
})();