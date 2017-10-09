(function () {
    'use strict';

    angular.module("app")
    .controller("dataPengadaanController", ctrl);

    ctrl.$inject = ['$state', '$http', '$filter', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataPengadaanService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $http, $filter, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataPengadaanService, UIControlService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";

        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.totalItems = 0;
        vm.keyword = "";
        vm.column = 1;

        vm.paket = [];

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('data-pengadaan');
            loadPaket();
        };

        vm.loadPaket = loadPaket;
        function loadPaket() {
            UIControlService.loadLoading(loadmsg);
            DataPengadaanService.Select({
                Keyword: vm.keyword,
                Offset: vm.pageSize * (vm.currentPage - 1),
                Limit: vm.pageSize,
                Column: vm.column
            }, function (reply) {
                vm.paket = reply.data.List;
                vm.paket.forEach(function (p) {
                    //cr.PublishedDateConverted = convertDate(cr.PublishedDate);
                    p.tahapanSekarang = [];
                    p.tahapanLama = [];
                    p.tahapanNext = [];
                    p.isKetua = false;

                    p.TenderStepDatas.forEach(function (ts) {
                        if (ts.StateNext === 'Curr') {
                            p.tahapanSekarang.push(ts);
                        } else if (ts.StateNext === 'Next') {
                            p.tahapanNext.push(ts);
                        } else if (ts.StateNext === 'Old') {
                            p.tahapanLama.push(ts);
                        }
                    });
                });
                UIControlService.unloadLoading();
                vm.totalItems = reply.data.Count;
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

        vm.viewTahapan = viewTahapan;
        function viewTahapan(paket) {
            $state.transitionTo('data-pengadaan-tahapan', { TenderRefID: paket.TenderRefID, ProcPackType: paket.ProcPackageType });
        };

        vm.convertDate = convertDate;
        function convertDate(date) {
            return UIControlService.convertDate(date);
        }
    }
})();
