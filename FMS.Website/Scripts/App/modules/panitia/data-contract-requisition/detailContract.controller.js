(function () {
    'use strict';

    angular.module("app")
    .controller("detailContractReqCtrl", ctrl);
    
    ctrl.$inject = ['$state', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService) {

        var vm = this;
        var contractRequisitionId = Number($stateParams.contractRequisitionId);
        var loadmsg = "MESSAGE.LOADING";

        vm.breadcrumbs = [
            { title: "BREADCRUMB.MASTER_REQUISITION", href: "" },
            { title: "BREADCRUMB.DATA_CONTRACT_REQUISITION", href: "#/data-contract-requisition" },
            { title: "BREADCRUMB.DETAIL_CONTRACT_REQUISITION", href: "" }
        ];

        vm.projectTitle = "";
        vm.contractSponsor = "";
        vm.projectManager = "";
        vm.isSubmitted = false;

        vm.formList = [];

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('data-contract-requisition');

            UIControlService.loadLoading(loadmsg);
            DataContractRequisitionService.IsRequestor({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    if (reply.data === true) {
                        loadDetails();
                    } else {
                        UIControlService.msg_growl("warning", $filter('translate')('MESSAGE.ERR_NOT_REQUESTOR'));
                        $state.transitionTo('data-contract-requisition');
                    }
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_CHK_REQUESTOR'));
                    $state.transitionTo('data-contract-requisition');
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
                $state.transitionTo('data-contract-requisition');
            });
        };

        function loadDetails() {
            UIControlService.loadLoading(loadmsg);
            DataContractRequisitionService.GetRefCRDet(
            function (reply) {
                if (reply.status === 200) {
                    vm.formList = reply.data;
                    for (var i = 0; i < vm.formList.length; i++) {
                        vm.formList[i].ContractRequisitionId = contractRequisitionId;
                        vm.formList[i].Information = "";
                        vm.formList[i].notNeeded = vm.formList[i].LinkState !== 'form-contract-requisition';
                        vm.formList[i].mandatory = (vm.formList[i].LinkState === 'form-contract-requisition' || vm.formList[i].LinkState === 'detail-cost-estimate')
                        vm.formList[i].Status = vm.formList[i].mandatory;
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
                            vm.isSubmitted = contractRequisition.StatusName !== 'CR_DRAFT' && contractRequisition.StatusName.lastIndexOf('CR_REJECT_', 0) !== 0;
                            vm.isRejectVerification = contractRequisition.StatusName === 'CR_REJECT_2';
                            if (contractRequisition.DirectAward !== null) {
                                for (var i = 0; i < vm.formList.length; i++) {
                                    if (vm.formList[i].LinkState === 'scope-ofwork-assesment-dc' && contractRequisition.DirectAward === true) {
                                        vm.formList[i].notNeeded = true;
                                    } else if (vm.formList[i].LinkState === 'weighting-matrix' && contractRequisition.DirectAward === true) {
                                        vm.formList[i].notNeeded = true;
                                    } else if (vm.formList[i].LinkState === 'direct-award-form' && contractRequisition.DirectAward === false) {
                                        vm.formList[i].notNeeded = true;
                                    } else if (vm.formList[i].LinkState === 'direct-award-form' && contractRequisition.DirectAward === true) {
                                        vm.formList[i].notNeeded = false;
                                        vm.formList[i].mandatory = true;
                                        vm.formList[i].Status = true;
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
                                vm.formList[i].VerificationStatus = details[i].VerificationStatus;
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
                    ProjectManagerFullName: vm.projectManagerName,
                    IsTenderVerification: vm.isSubmitted
                };
                var modalInstance = $uibModal.open({
                    templateUrl: "app/modules/panitia/data-contract-requisition/directAwardForm.html",
                    controller: "directAwardFormCtrl",
                    controllerAs: "daFormCtrl",
                    resolve: { item: function () { return item; } }
                });
                modalInstance.result.then(function (ret) {
                    vm.projectManager = ret.ProjectManager;
                    vm.projectManagerName = ret.ProjectManagerFullName;
                });
            } else {
                if (vm.contractRequisitionVariationId > 0 && dt.LinkState === 'form-contract-requisition') {
                    $state.transitionTo('contract-variation', { contractRequisitionId: contractRequisitionId, contractRequisitionVariationId: vm.contractRequisitionVariationId });
                }
                else {
                    $state.transitionTo(dt.LinkState, { contractRequisitionId: contractRequisitionId });
                }
            }
        };

        vm.save = save;
        function save() {
            UIControlService.loadLoading(loadmsg);
            DataContractRequisitionService.SaveDetail({
                ContractRequisitionId: contractRequisitionId,
                ProjectTitle: vm.projectTitle,
                ContractSponsor: vm.contractSponsor,
                ProjectManager: vm.projectManager,
                ContractRequisitionDetails: vm.formList
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SAVE_DET'));
                    loadDetails();
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE_DET'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        };

        vm.back = back;
        function back(dt) {
            $state.transitionTo('data-contract-requisition');
        };
    };
})();