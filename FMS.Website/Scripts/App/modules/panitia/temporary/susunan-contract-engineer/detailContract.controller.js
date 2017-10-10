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


        vm.projectTitle = "";
        vm.contractSponsor = "";
        vm.projectManager = "";
        vm.isSubmitted = false;

        vm.formList = [];

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('data-contract-requisition');

            UIControlService.loadLoading(loadmsg);
            loadDetails();
        };

        function loadDetails() {
            UIControlService.loadLoading(loadmsg);
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
            console.info(JSON.stringify(dt.LinkState));
            if (dt.LinkState === 'direct-award-form') {
                var item = {
                    contractRequisitionId: contractRequisitionId,
                    ProjectManager: vm.projectManager,
                    ProjectManagerFullName: vm.projectManagerName,
                    IsTenderVerification: vm.isSubmitted
                };
                var modalInstance = $uibModal.open({
                    templateUrl: "app/modules/panitia/susunan-contract-engineer/directAwardForm.html",
                    controller: "directAwardFormCtrl",
                    controllerAs: "daFormCtrl",
                    resolve: { item: function () { return item; } }
                });
                modalInstance.result.then(function (ret) {
                    vm.projectManager = ret.ProjectManager;
                    vm.projectManagerName = ret.ProjectManagerFullName;
                });
            }
            else if (dt.LinkState === 'form-contract-requisition') {
                $state.transitionTo('form-contract-requisition-contract', { contractRequisitionId: contractRequisitionId });
            }
            else if (dt.LinkState === 'scope-ofwork-assesment-dc') {
                $state.transitionTo('scope-ofwork-assesment-dc-contract', { contractRequisitionId: contractRequisitionId });
            }
            else if (dt.LinkState === 'csms-decision') {
                $state.transitionTo('csms-decision-contract', { contractRequisitionId: contractRequisitionId });
            }
            else if (dt.LinkState === 'upload-sow-docs') {
                $state.transitionTo('upload-sow-docs-contract', { contractRequisitionId: contractRequisitionId });
            }
            else if (dt.LinkState === 'responsibility-matrix-dc') {
                $state.transitionTo('responsibility-matrix-dc-contract', { contractRequisitionId: contractRequisitionId });
            }
            else if (dt.LinkState === 'detail-cost-estimate') {
                $state.transitionTo('detail-cost-estimate-contract', { contractRequisitionId: contractRequisitionId });
            }
            else if (dt.LinkState === 'weighting-matrix') {
                $state.transitionTo('weighting-matrix-contract', { contractRequisitionId: contractRequisitionId });
            }
        };

        vm.back = back;
        function back(dt) {
            $state.transitionTo('contract-engineer');
        };
    };
})();