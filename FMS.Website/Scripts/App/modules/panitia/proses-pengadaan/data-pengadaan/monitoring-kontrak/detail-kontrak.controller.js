(function () {
    'use strict';

    angular.module("app").controller("DetailKonCtrl", ctrl);

    ctrl.$inject = ['$state', '$stateParams', '$http', '$filter', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataKontrakService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $stateParams, $http, $filter, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataKontrakService, UIControlService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";
        vm.datakontrak;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.totalItems = 0;
        vm.keyword = "";
        vm.column = 1;
        vm.maxSize = 10;
        vm.list = [];
        vm.init = init;
        var id = Number($stateParams.id);
        vm.id = Number($stateParams.id);

        function init() {
            $translatePartialLoader.addPart('detail-tender-variasi');
            loadPaket();
        };

        vm.loadPaket = loadPaket;
        function loadPaket() {
            UIControlService.loadLoading(loadmsg);
            DataKontrakService.VariasiKontrak({
                Status: vm.id,
                Offset: vm.pageSize * (vm.currentPage - 1),
                Limit: vm.pageSize,
                Column: vm.column
            },
            function (reply) {
                console.info("data:" + JSON.stringify(reply.data));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.datakontrak = data;
                    vm.list = reply.data.List;
                    var datas = vm.list;
                    vm.totalItems = Number(data.Count);
                    console.info("List:" + JSON.stringify(vm.list));

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
        }



        vm.convertDate = convertDate;
        function convertDate(date) {
            return UIControlService.convertDate(date);
        }
    }
})();
