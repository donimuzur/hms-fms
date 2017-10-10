(function () {
    'use strict';

    angular.module("app")
    .controller("paketPengadaanCECtrl", ctrl);

    ctrl.$inject = ['$state', '$http', '$filter', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'PaketPengadaanCEService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $http, $filter, $uibModal, $translate, $translatePartialLoader, $location, SocketService, PaketPengadaanCEService, UIControlService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";

        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.totalItems = 0;
        vm.keyword = "";
        vm.column = 1;

        vm.contractRequisition = [];

        vm.statusLabels = [];

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('paket-pengadaan-ce');
            vm.loadContracts();
        };

        vm.onSearchClick = onSearchClick;
        function onSearchClick(keyword) {
            vm.keyword = keyword;
            vm.currentPage = 1;
            vm.loadContracts();
        }

        vm.onFilterTypeChange = onFilterTypeChange;
        function onFilterTypeChange(column) {
            vm.column = column;
        }

        vm.loadContracts = loadContracts;
        function loadContracts() {
            UIControlService.loadLoading(loadmsg);
            PaketPengadaanCEService.SelectCR({
                Keyword: vm.keyword,
                Offset: vm.pageSize * (vm.currentPage - 1),
                Limit: vm.pageSize,
                Column: vm.column
            }, function (reply) {
                UIControlService.unloadLoading();
                vm.contractRequisition = reply.data.List;
                vm.contractRequisition.forEach(function (cr) {
                    cr.PublishedDateConverted = convertDate(cr.PublishedDate);
                });
                vm.totalItems = reply.data.Count;
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
            });
        };

        vm.detail = detail;
        function detail(dt) {
            $state.transitionTo('atur-paket-pengadaan', { contractRequisitionId: dt.ContractRequisitionId });
        };

        vm.dokumen = dokumen;
        function dokumen(dt) {
            $state.transitionTo('contract-requisition-docs-ce', { contractRequisitionId: dt.ContractRequisitionId });
        };

        function convertDate(date) {
            return UIControlService.convertDate(date);
        }
    }
})();