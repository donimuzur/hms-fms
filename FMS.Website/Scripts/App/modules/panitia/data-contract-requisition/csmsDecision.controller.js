(function () {
    'use strict';

    angular.module("app")
    .controller("csmsDecisionCtrl", ctrl);
    
    ctrl.$inject = ['$state', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService) {

        var vm = this;
        var contractRequisitionId = Number($stateParams.contractRequisitionId);
        vm.contractRequisitionId = Number($stateParams.contractRequisitionId);
        var loadmsg = "";

        vm.projectTitle = "";
        vm.contractSponsor = "";
        vm.projectManager = 0;
        vm.projectManagerName = "";
        vm.operatingOrCapital = "0";
        vm.departmentId = 0;
        vm.departmentName = "";
        vm.departments = [];

        vm.csms = {
            Area: "",
            IsSupervised: "0",
            IsAssessed: "0",
            IsHighRisk: "0"
        };
        vm.isTenderVerification = false;
        vm.breadcrumbs = [];

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('data-contract-requisition');
            $translate.refresh().then(function () {
                loadmsg = $filter('translate')('MESSAGE.LOADING');
            });
            setBreadCrumbs();

            //load daftar departemen
            DataContractRequisitionService.GetDepartments(function (reply) {
                vm.departments = reply.data;
            }, function (error) {
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_DEPARTMENTS'));
            });

            if (contractRequisitionId > 0) {
                UIControlService.loadLoading(loadmsg);
                DataContractRequisitionService.IsRequestor({
                    ContractRequisitionId: contractRequisitionId
                }, function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        if (reply.data === true) {
                            vm.loadData();
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
            } else {
                vm.loadData();
            }
        };

        function setBreadCrumbs() {
            vm.breadcrumbs.push({ title: "BREADCRUMB.MASTER_REQUISITION", href: "" });
            vm.breadcrumbs.push({ title: "BREADCRUMB.DATA_CONTRACT_REQUISITION", href: "#/data-contract-requisition" });
            if (vm.contractRequisitionId > 0) {
                vm.breadcrumbs.push({ title: "BREADCRUMB.DETAIL_CONTRACT_REQUISITION", href: "#/detail-contract-requisition/" + $stateParams.contractRequisitionId });
            }
            vm.breadcrumbs.push({ title: "BREADCRUMB.CSMS_DECISION", href: "" });
        }

        vm.loadData = loadData;
        function loadData() {
            if (contractRequisitionId > 0) {
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
                        vm.departmentId = contractRequisition.DepartmentID;
                        vm.departmentName = contractRequisition.DepartmentName;
                        vm.isTenderVerification = contractRequisition.StatusName !== 'CR_DRAFT' && contractRequisition.StatusName.lastIndexOf('CR_REJECT_', 0) !== 0;

                        if (contractRequisition.OperatingOrCapital !== null) {
                            vm.operatingOrCapital = contractRequisition.OperatingOrCapital === true ? '1' : '0';
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
            }
        };

        vm.selectManager = selectManager;
        function selectManager() {

            if (vm.isTenderVerification === true) {
                return;
            }

            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-contract-requisition/formContractRequisition.selectEmployeeModal.html',
                controller: 'selectEmployeeModal',
                controllerAs: 'selectEmployeeCtrl',
            });
            modalInstance.result.then(function (selectedManager) {
                vm.projectManager = selectedManager.EmployeeID;
                vm.projectManagerName = selectedManager.FullName + ' ' + selectedManager.SurName;
            });
        };

        vm.save = save;
        function save() {

            if (!vm.projectTitle || !vm.contractSponsor || !vm.projectManager || !vm.departmentId) {
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_FILL_ALL_FIELD'));
                return;
            }

            UIControlService.loadLoading(loadmsg);
            DataContractRequisitionService.SaveCSMS({
                ContractRequisitionId: contractRequisitionId,
                IsSupervised: vm.csms.IsSupervised === "1",
                IsAssessed: vm.csms.IsAssessed === "1",
                IsHighRisk: vm.csms.IsHighRisk === "1",
                ContractRequisition: {
                    ProjectTitle: vm.projectTitle,
                    ContractSponsor: vm.contractSponsor,
                    ProjectManager: vm.projectManager,
                    OperatingOrCapital: vm.operatingOrCapital === "1" ? true : false,
                    DepartmentID: vm.departmentId
                }
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SAVE_CSMS'));
                    if (contractRequisitionId > 0) {
                        $state.transitionTo('detail-contract-requisition', { contractRequisitionId: contractRequisitionId });
                    } else {
                        $state.transitionTo('data-contract-requisition');
                    }
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE_CSMS'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE_CSMS'));
            });
        };

        vm.cancel = cancel;
        function cancel() {
            if (contractRequisitionId > 0) {
                $state.transitionTo('detail-contract-requisition', { contractRequisitionId: contractRequisitionId });
            } else {
                $state.transitionTo('data-contract-requisition');
            }
        };
    }
})();