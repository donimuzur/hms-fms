(function () {
    'use strict';
    angular.module("app")
    .controller("PengumumanPengadaanCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'PengumumanPengadaanService',
        'UIControlService', '$uibModal'];
    /* @ngInject */
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, PengumumanPengadaanService,
        UIControlService, $uibModal) {
        /* jshint validthis: true */
        var vm = this;
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.maxSize = 10;
        vm.init = init;
        vm.txtSearch = '';
        vm.listPengumuman = [];

        // function declarations
        function init() {
            // Load partial traslastion
            $translatePartialLoader.addPart('pengumuman-pengadaan-client');
            vm.loadData(0);
        }

        vm.loadData = loadData;
        function loadData(current) {
            UIControlService.loadLoading("Silahkan Tunggu...");
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            PengumumanPengadaanService.getByOpenTender({
                Offset: offset,
                Limit: vm.maxSize,
                Keyword: vm.txtSearch
            }, function (reply) {
                //console.info("datane:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.endDateHours = []; vm.endDateMinutes = [];
                    var data = reply.data;
                    vm.listPengumuman = data.List;
                    var tglhariini = new Date();
                    tglhariini.setHours(23);
                    tglhariini.setMinutes(59);
                    tglhariini.setSeconds(0);
                    for (var i = 0; i < vm.listPengumuman.length; i++) {
                        vm.listPengumuman[i]['expired'] = false;
                        vm.listPengumuman[i].TenderStepData.ConvertedStartDate = UIControlService.getStrDate(vm.listPengumuman[i].TenderStepData.StartDate);
                        vm.listPengumuman[i].TenderStepData.ConvertedEndDate = UIControlService.getStrDate(vm.listPengumuman[i].TenderStepData.EndDate);
                        vm.listPengumuman[i]['masa_berlaku'] = Math.floor(hitungSelisihHari(tglhariini, vm.listPengumuman[i].TenderStepData.ConvertedEndDate));
                        
                        if (vm.listPengumuman[i].masa_berlaku < 0) {
                            vm.endDateHours[i] = new Date(vm.listPengumuman[i].TenderStepData.EndDate).getHours();
                            vm.endDateMinutes[i] = new Date(vm.listPengumuman[i].TenderStepData.EndDate).getMinutes();
                            if (vm.endDateHours[i] === 23 && vm.endDateMinutes[i] === 59) {
                                vm.listPengumuman[i].expired = true;
                            }
                        }
                        console.info("exp" + vm.listPengumuman[i].expired);
                        console.info("exp" + vm.listPengumuman[i].masa_berlaku);
                    }
                    vm.totalItems = Number(data.Count);
                } else {
                    UIControlService.msg_growl("error", "Gagal mendapatkan data Master Badan Usaha");
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                //console.info("error:" + JSON.stringify(err));
                UIControlService.msg_growl("error", "Gagal akses API");
                UIControlService.unloadLoading();
            });
        }

        function hitungSelisihHari(tgl1, tgl2) {
            // varibel miliday sebagai pembagi untuk menghasilkan hari
            var miliday = 24 * 60 * 60 * 1000;
            //buat object Date
            var tanggal1 = new Date(tgl1);
            var tanggal2 = new Date(tgl2);
            // Date.parse akan menghasilkan nilai bernilai integer dalam bentuk milisecond
            var tglPertama = Date.parse(tanggal1);
            var tglKedua = Date.parse(tanggal2);
            var selisih = (tglKedua - tglPertama) / miliday;
            return selisih;
        }

        vm.detailData = detailData;
        function detailData(data) {
            var modalInstance = $uibModal.open({
                templateUrl: 'detail-pengumuman-pengadaan.html',
                controller: 'detailPPController',
                controllerAs: 'detailPPController',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.loadData();
            });
        }

    }
})();