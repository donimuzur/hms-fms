(function () {
    'use strict';

    angular.module("app")
    .controller("responsibilityMatrixCtrl", ctrl);
    
    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService) {

        var vm = this;
        var contractRequisitionId = Number($stateParams.contractRequisitionId);
        vm.contractRequisitionId = Number($stateParams.contractRequisitionId);
        var loadmsg = "";

        vm.respMatrix = [];
        vm.isTenderVerification = false;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('data-contract-requisition');
            $translate.refresh().then(function () {
                loadmsg = $filter('translate')('MESSAGE.LOADING');
            });

            UIControlService.loadLoading(loadmsg);
            vm.loadData();
        };

        function loadCRInfo() {
            DataContractRequisitionService.SelectById({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.ProjectTitle = reply.data.ProjectTitle;
                    vm.isTenderVerification = reply.data.StatusName !== 'CR_DRAFT' && reply.data.StatusName !== 'CR_REJECT_2';
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_DET'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        }

        vm.loadData = loadData;
        function loadData() {
            UIControlService.loadLoading(loadmsg);
            DataContractRequisitionService.SelectRespMxDetail({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.respMatrix = reply.data;
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD_RESP_MX'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        }

        vm.addItem = addItem;
        function addItem() {
            var respMatrix = {
                Description: '',
                ContractRequisitionId: contractRequisitionId,
                Parent: 0,
                IsContractorS: false,
                IsContractorI: false,
                IsContractorM: false,
                IsContractorO: false,
                IsCompanyS: false,
                IsCompanyI: false,
                IsCompanyM: false,
                IsCompanyO: false,
                Comment: '',
                Children: []
            };
            var item = {
                respMatrix: respMatrix,
                title: $filter('translate')('TAMBAH') + ' item'
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-contract-requisition/responsibilityMatrix.modal.html',
                controller: 'responsibilityMatrixModalCtrl',
                controllerAs: 'respMxModCtrl',
                resolve: {
                    item: function () {
                        return item;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.respMatrix.push(respMatrix);
            });
        };

        vm.addSubItem = addSubItem;
        function addSubItem(rm) {
            var respMatrix = {
                Description: '',
                ContractRequisitionId: rm.ContractRequisitionId,
                Parent: rm.ContractRequisitionRMId > 0 ? rm.ContractRequisitionRMId : -1,
                IsContractorS: false,
                IsContractorI: false,
                IsContractorM: false,
                IsContractorO: false,
                IsCompanyS: false,
                IsCompanyI: false,
                IsCompanyM: false,
                IsCompanyO: false,
                Comment: ''
            };
            var item = {
                respMatrix: respMatrix,
                title: $filter('translate')('TAMBAH') + ' sub-item'
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-contract-requisition/responsibilityMatrix.modal.html',
                controller: 'responsibilityMatrixModalCtrl',
                controllerAs: 'respMxModCtrl',
                resolve: {
                    item: function () {
                        return item;
                    }
                }
            });
            modalInstance.result.then(function () {
                rm.Children.push(respMatrix);
            });
        };

        vm.editItem = editItem;
        function editItem(rm) {
            var respMatrix = {
                Description : rm.Description
            };
            var item = {
                respMatrix: respMatrix,
                title: $filter('translate')('UBAH') + ' ' + (rm.Parent > 0 ? 'sub-item' : 'item')
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-contract-requisition/responsibilityMatrix.modal.html',
                controller: 'responsibilityMatrixModalCtrl',
                controllerAs: 'respMxModCtrl',
                resolve: {
                    item: function () {
                        return item;
                    }
                }
            });
            modalInstance.result.then(function () {
                rm.Description = respMatrix.Description;
            });
        };

        vm.deleteItem = deleteItem;
        function deleteItem(index) {
            bootbox.confirm($filter('translate')('MESSAGE.DEL_RESP_MX_ITEM'), function (yes) {
                if (yes) {
                    vm.respMatrix.splice(index, 1);
                    $scope.$apply();
                }
            });
        };

        vm.deleteSubItem = deleteSubItem;
        function deleteSubItem(rmParent, index) {
            bootbox.confirm($filter('translate')('MESSAGE.DEL_RESP_MX_SUB_ITEM'), function (yes) {
                if (yes) {
                    rmParent.Children.splice(index, 1);
                    $scope.$apply();
                }
            });
        };

        vm.save = save;
        function save() {
            UIControlService.loadLoading(loadmsg);
            DataContractRequisitionService.SaveRespMxDetail(vm.respMatrix, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SAVE_RESP_MX'));
                    loadData();
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE_RESP_MX'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        };

        vm.back = back;
        function back() {
            $state.transitionTo('detail-contract-requisition-contract', { contractRequisitionId: contractRequisitionId });
        };
    }
    
})();