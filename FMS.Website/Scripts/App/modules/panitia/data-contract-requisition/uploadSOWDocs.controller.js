(function () {
    'use strict';

    angular.module("app")
    .controller("uploadSOWDocsCtrl", ctrl);

    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService', 'GlobalConstantService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService, GlobalConstantService) {

        var vm = this;
        var contractRequisitionId = Number($stateParams.contractRequisitionId);
        vm.contractRequisitionId = Number($stateParams.contractRequisitionId);
        var loadmsg = "MESSAGE.LOADING";

        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.sowDocs = [];
        vm.isTenderVerification = false;

        vm.breadcrumbs = [
            { title: "BREADCRUMB.MASTER_REQUISITION", href: "" },
            { title: "BREADCRUMB.DATA_CONTRACT_REQUISITION", href: "#/data-contract-requisition" },
            { title: "BREADCRUMB.DETAIL_CONTRACT_REQUISITION", href: "#/detail-contract-requisition/" + $stateParams.contractRequisitionId },
            { title: "BREADCRUMB.UPLOAD_SOW_DOCS", href: "" }
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

        vm.add = add;
        function add() {
            var lempar = {
                sowDoc: {
                    ContractRequisitionId: contractRequisitionId,
                    DocName: "",
                    DocUrl: "",
                    Size: 0,
                }
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-contract-requisition/uploadSOWDocs.modal.html',
                controller: 'uploadSOWDModalCtrl',
                controllerAs: 'uploadSOWDModalCtrl',
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
        function edit(sowDoc) {
            var lempar = {
                sowDoc: {
                    ContractRequisitionSOADocId: sowDoc.ContractRequisitionSOADocId,
                    ContractRequisitionId: sowDoc.ContractRequisitionId,
                    DocName: sowDoc.DocName,
                    DocUrl: sowDoc.DocUrl,
                    Size: sowDoc.Size
                }
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-contract-requisition/uploadSOWDocs.modal.html',
                controller: 'uploadSOWDModalCtrl',
                controllerAs: 'uploadSOWDModalCtrl',
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
        function del(sowDoc) {
            bootbox.confirm($filter('translate')('SOW.CONFIRM_DEL_SOW'), function (yes) {
                if (yes) {
                    UIControlService.loadLoading(loadmsg);
                    DataContractRequisitionService.DeleteSOWDoc(sowDoc, function (reply) {
                        UIControlService.unloadLoading();
                        if (reply.status === 200) {
                            UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_DEL_SOW_DOC'));
                            vm.loadData();
                        } else {
                            UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_DEL_SOW_DOC'));
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
            $state.transitionTo('detail-contract-requisition', { contractRequisitionId: contractRequisitionId });
        };
    }
})();