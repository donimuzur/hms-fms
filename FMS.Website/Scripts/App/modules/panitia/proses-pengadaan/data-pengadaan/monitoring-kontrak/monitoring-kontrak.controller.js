(function () {
    'use strict';

    angular.module("app").controller("MonKontrak", ctrl);

    ctrl.$inject = ['$state', '$stateParams', '$http', '$filter', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataKontrakService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $stateParams, $http, $filter, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataKontrakService, UIControlService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";

        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.totalItems = 0;
        vm.keyword = "";
        vm.column = 1;
        vm.maxSize = 10;
        vm.list = [];
        vm.searchBy = 0;
        //   vm.id = Number($stateParams.id);
        vm.tenderstepid = Number($stateParams.tenderstepid);
        vm.vendorid = Number($stateParams.vendorid);
        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('monitoring-kontrak');
            loadPaket();
        };

        vm.loadPaket = loadPaket;
        function loadPaket() {
            UIControlService.loadLoading(loadmsg);
            DataKontrakService.Select({
                Keyword: vm.keyword,
                Offset: vm.pageSize * (vm.currentPage - 1),
                Limit: vm.pageSize,
                Column: vm.searchBy,
                tenderstepid: vm.tenderstepid,
                vendorid: vm.vendorid

            },
            function (reply) {
                //  console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.list = reply.data.List;

                    for (var i = 0; i < vm.list.length; i++) {
                        if (vm.list[i].SpendingValue === null) { vm.list[i].SpendingValue = "-"; }

                        if (vm.list[i].TotalValueVar === null) { vm.list[i].TotalValueVar = "-"; }
                    }
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

        vm.viewDistribusi = viewDistribusi;
        function viewDistribusi(id) {
            $state.transitionTo('detail-distribusi', { id: id});
        };

        vm.viewDetail = viewDetail;
        function viewDetail(id) {
            $state.transitionTo('detail-kontrak', { id: id });
        };
        vm.viewDok = viewDok;
        function viewDok(id) {
            $state.transitionTo('detail-dokumen', { id: id });
        };

        vm.convertDate = convertDate;
        function convertDate(date) {
            return UIControlService.convertDate(date);
        }



    }
})();
