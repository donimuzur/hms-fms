(function () {
    'use strict';

    angular.module("app")
    .controller("detailContractReqDACtrl", ctrl);
    
    ctrl.$inject = ['$state', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'UIControlService', 'DataContractRequisitionService'];
    /* @ngInject */
    function ctrl($state, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, UIControlService, DataContractRequisitionService) {

        var vm = this;
        var contractRequisitionId = Number($stateParams.contractRequisitionId);
        var loadmsg = "MESSAGE.LOADING";

        vm.breadcrumbs = [
            { title: "BREADCRUMB.MASTER_REQUISITION", href: "" },
            { title: "BREADCRUMB.DATA_CONTRACT_REQUISITION", href: "#/data-contract-requisition" },
            { title: "BREADCRUMB.APPROVAL", href: "#/data-contract-requisition/approval" },
            { title: "BREADCRUMB.DETAIL_CONTRACT_REQUISITION", href: "" }
        ];

        vm.projectTitle = "";
        vm.contractSponsor = "";
        vm.projectManager = "";
        vm.isTenderVerification = true;
        vm.isOnProcess = false;

        vm.formList = [];

        vm.statusLabels = [];
        vm.statusLabels["CR_PROCESS_1"] = 'STATUS.ON_PROCESS';
        vm.statusLabels["CR_PROCESS_2"] = 'STATUS.APPROVED';
        vm.statusLabels["CR_REJECT_1"] = 'STATUS.REJECTED';

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('data-contract-requisition');
            UIControlService.unloadLoading(loadmsg);
            DataContractRequisitionService.IsApprover({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    if (reply.data === true) {
                        loadDetails();
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

        function loadDetails() {
            DataContractRequisitionService.GetRefCRDet(
            function (reply) {
                if (reply.status === 200) {
                    vm.formList = reply.data;
                    for (var i = 0; i < vm.formList.length; i++) {
                        vm.formList[i].ContractRequisitionId = contractRequisitionId;
                        vm.formList[i].Status = false;
                        vm.formList[i].Information = "";
                        vm.formList[i].notNeeded = vm.formList[i].LinkState !== 'form-contract-requisition';
                    }
                    DataContractRequisitionService.SelectDetail({
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
                            vm.isSubmitted = contractRequisition.StatusName !== 'CR_DRAFT' && contractRequisition.StatusName !== 'CR_REJECT_2';
                            if (contractRequisition.DirectAward !== null) {
                                for (var i = 0; i < vm.formList.length; i++) {
                                    if (vm.formList[i].LinkState === 'scope-ofwork-assesment-dc' && contractRequisition.DirectAward === true) {
                                        vm.formList[i].notNeeded = true;
                                    } else if (vm.formList[i].LinkState === 'weighting-matrix' && contractRequisition.DirectAward === true) {
                                        vm.formList[i].notNeeded = true;
                                    } else if (vm.formList[i].LinkState === 'direct-award-form' && contractRequisition.DirectAward === false) {
                                        vm.formList[i].notNeeded = true;
                                    } else {
                                        vm.formList[i].notNeeded = false;
                                    }
                                }
                            }
                            var details = contractRequisition.ContractRequisitionDetails;
                            for (var i = 0; i < details.length; i++) {
                                vm.formList[i].ContractRequisitionDetailId = details[i].contractRequisitionId;
                                vm.formList[i].Status = details[i].Status;
                                vm.formList[i].Information = details[i].Information;
                            }
                        } else {
                            UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_DET'));
                        }
                    }, function (error) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
                    });
                } else {
                    UIControlService.unloadLoading();
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
                    $state.transitionTo('contract-variation-da', { contractRequisitionId: contractRequisitionId, contractRequisitionVariationId: vm.contractRequisitionVariationId });
                }
                else {
                    $state.transitionTo(dt.LinkState + '-da', { contractRequisitionId: contractRequisitionId });
                }
            }
        };

        vm.back = back;
        function back(dt) {
            $state.transitionTo('contract-requisition-draft-approval');
        };
    };
})();