(function () {
    'use strict';

    angular.module("app")
    .controller("crDocsCtrl", ctrl);

    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService', 'GlobalConstantService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService, GlobalConstantService) {

        var vm = this;
        var contractRequisitionId = Number($stateParams.contractRequisitionId);
        vm.contractRequisitionId = Number($stateParams.contractRequisitionId);
        var loadmsg = "MESSAGE.LOADING";

        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.docs = [];
        vm.isTenderVerification = false;

        vm.breadcrumbs = [
            { title: "BREADCRUMB.MASTER_REQUISITION", href: "" },
            { title: "BREADCRUMB.DATA_CONTRACT_REQUISITION", href: "#/data-contract-requisition" },
            { title: "BREADCRUMB.CONTRACT_REQUISITION_DOCS", href: "" }
        ];

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
                        loadCRInfo();
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
        };

        function loadCRInfo() {
            DataContractRequisitionService.SelectById({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.ProjectTitle = reply.data.ProjectTitle;
                    vm.isTenderVerification = reply.data.StatusName !== 'CR_DRAFT' && reply.data.StatusName.lastIndexOf('CR_REJECT_', 0) !== 0;
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_DET'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        }

        function generateFilterStrings(allowedTypes) {
            var filetypes = "";
            for (var i = 0; i < allowedTypes.length; i++) {
                filetypes += "." + allowedTypes[i].Name + ",";
            }
            return filetypes.substring(0, filetypes.length - 1);
        }

        vm.loadData = loadData;
        function loadData() {
            DataContractRequisitionService.SelectDocs({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status == 200) {
                    vm.docs = reply.data;
                } else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_LOAD_DOC");
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                UIControlService.unloadLoading();
            });
        };

        vm.add = add;
        function add() {
            var lempar = {
                doc: {
                    ContractRequisitionId: contractRequisitionId,
                    DocName: "",
                    DocUrl: "",
                    Size: 0,
                }
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-contract-requisition/crDocuments.modal.html',
                controller: 'crDocsModalCtrl',
                controllerAs: 'crDocsModalCtrl',
                resolve: {
                    item: function () {
                        return lempar;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.loadData();
            });
        };

        vm.edit = edit;
        function edit(doc) {
            var lempar = {
                doc: {
                    ContractRequisitionDocumentId: doc.ContractRequisitionDocumentId,
                    ContractRequisitionId: doc.ContractRequisitionId,
                    DocName: doc.DocName,
                    DocUrl: doc.DocUrl,
                    Size: doc.Size
                }
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-contract-requisition/crDocuments.modal.html',
                controller: 'crDocsModalCtrl',
                controllerAs: 'crDocsModalCtrl',
                resolve: {
                    item: function () {
                        return lempar;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.loadData();
            });
        };

        vm.del = del;
        function del(doc) {
            bootbox.confirm($filter('translate')('MESSAGE.CONFIRM_DEL_DOC'), function (yes) {
                if (yes) {
                    UIControlService.loadLoading(loadmsg);
                    DataContractRequisitionService.DeleteDoc(doc, function (reply) {
                        UIControlService.unloadLoading();
                        if (reply.status === 200) {
                            UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_DEL_DOC'));
                            vm.loadData();
                        } else {
                            UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_DEL_DOC'));
                        }
                    }, function (err) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
                    });
                }
            });
        }

        vm.back = back;
        function back() {
            $state.transitionTo('data-contract-requisition');
        };
    }
})();