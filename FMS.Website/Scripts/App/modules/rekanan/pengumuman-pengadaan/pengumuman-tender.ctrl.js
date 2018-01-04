(function () {
    'use strict';

    angular.module("app")
    .controller("PengumumanTenderVendorCtrl", ctrl);

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
        vm.TenderID = Number($stateParams.TenderID);
        vm.StepID = Number($stateParams.StepID);

        vm.init = init;
        function init() {
            console.info("tenderID" + vm.TenderID);
            loadSteps();
            loadDataTender();
            //loadDataTender();
            //$translatePartialLoader.addPart("pengumuman-pengadaaan-client");
            //loadDataOfferEntry();
        }


        function loadSteps() {
            PengumumanPengadaanService.GetSteps({
                ID: vm.TenderID
            }, function (reply) {
                console.info("step" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.dataSteps = reply.data;
                    vm.GoodsOrService = vm.dataSteps[0].step.GoodsOrService;
                    vm.TenderRefID = vm.dataSteps[0].tender.TenderRefID;
                    if (vm.GoodsOrService === 1) {
                        loadVHSOfferEntry()
                    }
                    else if (vm.GoodsOrService === 2) {
                        for (var i = 1; i < vm.dataSteps.length; i++) {
                            if (vm.dataSteps[i].step.FormTypeName === 'Pemasukan Penawaran Jasa') {
                                console.info("masuk");
                                vm.StepIDJasa = vm.dataSteps[i].ID;
                                console.info("stepIDjasa" + vm.StepIDJasa);
                                i = vm.dataSteps.length;
                                loadServiceOfferEntry();
                            }
                        }
                        //loadServiceOfferEntry();
                    }
                   // loadDataTender();
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        function loadDataTender() {
            PengumumanPengadaanService.getTenderReg({
                ID:vm.TenderID
            }, function (reply) {
                console.info("data tend" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.dataTender = reply.data;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }


        function loadServiceOfferEntry() {
            UIControlService.loadLoading("Silahkan Tunggu");
            PengumumanPengadaanService.getKelengkapanDocVendor({
                TenderStepID: vm.StepIDJasa
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    console.info("penawaran:" + JSON.stringify(data));
                    vm.listKelengkapan = data.VendorDocuments;
                    for (var i = 0; i < vm.listKelengkapan.length; i++) {
                        if (!(vm.listKelengkapan[i].ApproveDate === null)) {
                            vm.listKelengkapan[i].ApproveDate = UIControlService.getStrDate(vm.listKelengkapan[i].ApproveDate);
                        }
                    }

                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        function loadVHSOfferEntry() {
            PengumumanPengadaanService.loadTemplateOfferEntry({
                Status: vm.TenderRefID,
                column: 191
            }, function (reply) {
                //console.info("offEntry" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.dataOfferEntry = reply.data;
                    /*
                    for (var i = 0; i < vm.data.length; i++) {
                        if (vm.data[i].DocName === "Surat Penawaran") {
                            vm.data[i].DocName += ' ' + vm.RFQId.Limit + '%';
                        }
                    }*/
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


        vm.detailPersyaratan = detailPersyaratan;
        function detailPersyaratan() {
            var data = {
                TenderRefID:vm.TenderRefID
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/rekanan/pengumuman-pengadaan/detailPersyaratanVendor.html',
                controller: "detailPersyaratanVendorCtrl",
                controllerAs: "detailPersyaratanVendorCtrl",
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

