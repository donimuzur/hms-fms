(function () {
    'use strict';

    angular.module("app").controller("monitoringPO", ctrl);

    ctrl.$inject = ['$state', '$http', '$filter', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataMonitoringService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $http, $filter, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataMonitoringService, UIControlService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";

        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.totalItems = 0;
        vm.keyword = "";
        vm.column = 1;
        vm.maxSize = 10;
        vm.paket = [];
    
        vm.searchBy = 0;
        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('monitoring-po');
            loadPaket(1);
        };

        vm.loadPaket = loadPaket;
        function loadPaket() {
            UIControlService.loadLoading(loadmsg);
            DataMonitoringService.Select({
                Keyword: vm.keyword,
                Offset: vm.pageSize * (vm.currentPage - 1),
                Limit: vm.pageSize,
                Column: vm.searchBy
            },
            function (reply) {
                console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.paket = reply.data.List;

                    vm.totalItems = Number(data.Count);

                }


                else {
                    $.growl.error({ message: "MESSAGE.ERR_LOAD" });
                    UIControlService.unloadLoading();
                }
            },
                        function (error) {
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

        vm.viewPO = viewPO;
        function viewPO(id) {
            $state.transitionTo('detail-po', { id : id });
        };


        vm.upload = upload;
        function upload() {
            $state.transitionTo('data-upload-po');
        };


        vm.convertDate = convertDate;
        function convertDate(date) {
            return UIControlService.convertDate(date);
        }
    }
})();
