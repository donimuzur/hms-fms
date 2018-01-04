(function () {
    'use strict';

    angular.module("app")
    .controller("PengumumanPengadaanVendorCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'PengumumanPengadaanService', '$state', 'UIControlService', '$uibModal', '$stateParams'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, PengumumanPengadaanService,
        $state, UIControlService, $uibModal, $stateParams) {
        var vm = this;
        vm.TanggalHariIni = new Date();
        vm.srcText = '';
        vm.listPengumuman = [];
        vm.currentPage = 1;
        vm.maxSize = 10;

        vm.init = init;
        function init() {
            //$translatePartialLoader.addPart("pengumuman-pengadaaan-client");
            loadDataPengumuman(1);
        }

        vm.jLoad = jLoad;
        function jLoad(current) {
            loadDataPengumuman(current);
        }

        function loadDataPengumuman(current) {
            var offset = (current * 10) - 10;
            PengumumanPengadaanService.getAllDataAnnouncementByVendor({
                Keyword: vm.srcText, Offset: offset, Limit: vm.maxSize
            }, function (reply) {
                console.info("announc::"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listPengumuman = data.List;
                    vm.total = data.Count;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.detailPengumuman = detailPengumuman;
        function detailPengumuman(data) {
            //console.info("detail: " + JSON.stringify(data));
            
            var modalInstance = $uibModal.open({
                templateUrl: 'detail-pengumuman-pengadaan-vendor.html',
                controller: "detailPPVendorController",
                controllerAs: "detailPPVendorController",
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            //console.info("okeee");
            modalInstance.result.then(function () {
                vm.jLoad(1);
            });
        }

        vm.lihatPendaftaran = lihatPendaftaran;
        function lihatPendaftaran(data) {
            //console.info("modaala");
            var data = {
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/rekanan/pengumuman-pengadaan/pendaftaranPengadaan.html',
                controller: "PendaftaranPengadaanCtrl",
                controllerAs: "PendaftaranPengadaanCtrl",
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            //console.info("okeee");
            modalInstance.result.then(function () {
                vm.jLoad(1);
            });
        }

    }
})();

