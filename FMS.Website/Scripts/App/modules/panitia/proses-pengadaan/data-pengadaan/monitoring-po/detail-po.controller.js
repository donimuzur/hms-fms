(function () {
    'use strict';

    angular.module("app").controller("detailPO", ctrl);

    ctrl.$inject = ['$state', '$stateParams', '$http', '$filter', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataMonitoringService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $stateParams, $http, $filter, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataMonitoringService, UIControlService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";
        vm.vendor_name = "";
        vm.list_detail = "";
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.totalItems = 0;
        vm.keyword = "";
        vm.column = 1;
        vm.vendorid = String($stateParams.id);

        vm.paket = [];

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('detail-po');
            loadPaket();
        };

        vm.loadPaket = loadPaket;
        function loadPaket() {
            UIControlService.loadLoading(loadmsg);
            DataMonitoringService.Detail({
                Keyword: vm.keyword,
                Parameter: vm.vendorid,
                Offset: vm.pageSize * (vm.currentPage - 1),
                Limit: vm.pageSize,
                Column: vm.column

            }, function (reply) {
                console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.paket = reply.data.List;

                    vm.totalItems = Number(data.Count);
                } else {
                    $.growl.error({ message: "MESSAGE.ERR_LOAD" });
                    UIControlService.unloadLoading();

                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
            });
        };

        vm.cariPaket = cariPaket;
        function cariPaket(keyword) {
            vm.keyword = keyword;
            vm.currentPage = 1;
            loadPaket();
        };

        vm.PO = PO;
        function PO(id) {
            $state.transitionTo('export-po', { id: id });
        };
        vm.Aknowsheet = Aknowsheet;
        function Aknowsheet(id) {
            $state.transitionTo('acknowledgementsheet', { id: id });
        };
        vm.convertDate = convertDate;
        function convertDate(date) {
            return UIControlService.convertDate(date);
        }
    }
})();
