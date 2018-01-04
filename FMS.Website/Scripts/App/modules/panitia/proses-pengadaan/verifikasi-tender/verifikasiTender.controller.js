(function () {
    'use strict';

    angular.module("app")
    .controller("tenderVerificationCtrl", ctrl);

    ctrl.$inject = ['$state', '$http', '$filter', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'TenderVerificationService', 'DataContractRequisitionService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $http, $filter, $uibModal, $translate, $translatePartialLoader, $location, SocketService, TenderVerificationService, DataContractRequisitionService, UIControlService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";

        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.totalItems = 0;
        vm.keyword = "";
        vm.column = 1;

        vm.contractRequisition = [];

        vm.statusLabels = [];
        vm.statusLabels["CR_PROCESS_2"] = 'STATUS.ON_PROCESS';
        vm.statusLabels["CR_REJECT_2"] = 'STATUS.UNCOMPLETE';
        vm.statusLabels["CR_PROCESS_3"] = 'STATUS.REVIEWED';
        vm.statusLabels["CR_REJECT_3"] = 'STATUS.PENDING';
        vm.statusLabels["CR_APPROVED"] = 'STATUS.APPROVED';

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('verifikasi-tender');
            DataContractRequisitionService.EvaluateApprovalStatuses(function (reply) {
                vm.loadContracts();
            }, function (error) {
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
                vm.loadContracts();
            });
        };

        vm.onSearchClick = onSearchClick;
        function onSearchClick(keyword) {
            vm.keyword = keyword;
            vm.loadContracts();
            vm.currentPage = 1;
        }

        vm.onFilterTypeChange = onFilterTypeChange;
        function onFilterTypeChange(column) {
            vm.column = column;
        }

        vm.loadContracts = loadContracts;
        function loadContracts() {
            UIControlService.loadLoading(loadmsg);
            TenderVerificationService.SelectCR({
                Keyword: vm.keyword,
                Offset: vm.pageSize * (vm.currentPage - 1),
                Limit: vm.pageSize,
                Column: vm.column
            }, function (reply) {
                if (reply.status === 200) {
                    UIControlService.unloadLoading();
                    vm.contractRequisition = reply.data.List;
                    vm.totalItems = reply.data.Count;
                } else {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD'));
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD'));
            });
        };

        vm.detailApproval = detailApproval;
        function detailApproval(dt) {
            var item = {
                contractRequisitionId: dt.ContractRequisitionId
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/proses-pengadaan/verifikasi-tender/detailApproval.modal.html',
                controller: 'detailApprovalCtrl',
                controllerAs: 'detAppCtrl',
                resolve: { item: function () { return item; } }
            });
        };

        vm.menujuDokumen = menujuDokumen;
        function menujuDokumen(dt) {
            $state.transitionTo('contract-requisition-docs-cp', { contractRequisitionId: dt.ContractRequisitionId });
        };

        vm.detailContract = detailContract;
        function detailContract(dt) {
            $state.transitionTo('detail-contract-requisition-cp', { contractRequisitionId: dt.ContractRequisitionId });
        };

        vm.aturApproval = aturApproval;
        function aturApproval(dt){
            $state.transitionTo('verifikasi-tender-atur-app', { contractRequisitionId: dt.ContractRequisitionId });
        };

        vm.convertDate = convertDate;
        function convertDate(date) {
            return UIControlService.convertDate(date);
        }
    }
})();