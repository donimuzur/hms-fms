(function () {
    'use strict';

    angular.module("app")
    .controller("modalDetailMetodeEvaluasiCtrl", ctrl);
    
    ctrl.$inject = ['$http', '$state', '$filter', '$stateParams', '$uibModalInstance', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'MetodeEvaluasiService', 'UIControlService', 'item'];
    /* @ngInject */
    function ctrl($http, $state, $filter, $stateParams, $uibModalInstance, $translate, $translatePartialLoader, $location, SocketService, metodeEvaluasiService, UIControlService, item) {

        var vm = this;
        var metode_evaluasi_id = item.metode_evaluasi_id;
        var Administrasi = [];
        var AdministrasiLevel1 = [];
        var AdministrasiLevel2 = [];
        var AdministrasiLevel3 = [];
        var Teknis = [];
        var TeknisLevel1 = [];
        var TeknisLevel2 = [];
        var TeknisLevel3 = [];
        var Harga = [];
        var HargaLevel1 = [];
        var HargaLevel2 = [];
        var HargaLevel3 = [];
        var Barang = [];
        var BarangLevel1 = [];
        var BarangLevel2 = [];
        var BarangLevel3 = [];
        var VHS = [];
        var VHSLevel1 = [];
        var VHSLevel2 = [];
        var VHSLevel3 = [];
        var loadingMessage = "";
        vm.Administrasi;
        vm.bobotAdministrasi = 0;
        vm.Teknis;
        vm.bobotTeknis = 0;
        vm.Harga;
        vm.bobotHarga = 0;
        vm.Barang;
        vm.bobotBarang = 0;
        vm.VHS;
        vm.bobotVHS = 0;
        vm.kategori;
        vm.nama;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('metode-evaluasi');
            $translate.refresh().then(function () {
                loadingMessage = $filter('translate')('MESSAGE.LOADING');
            });
            vm.initialize()
        };

        vm.initialize = initialize;
        function initialize() {
            UIControlService.loadLoading(loadingMessage);
            metodeEvaluasiService.selectById({
                EvaluationMethodId: metode_evaluasi_id
            }, function (reply) {
                if (reply.status === 200) {
                    vm.kategori = reply.data.EvaluationMethodDetails;
                    for (var i = 0; i < vm.kategori.length; i++) {
                        if (vm.kategori[i].DetailType === 'Administrasi')
                            vm.bobotAdministrasi = vm.kategori[i].Weight;
                        else if (vm.kategori[i].DetailType === 'Teknis')
                            vm.bobotTeknis = vm.kategori[i].Weight;
                        else if (vm.kategori[i].DetailType === 'Harga')
                            vm.bobotHarga = vm.kategori[i].Weight;
                        else if (vm.kategori[i].DetailType === 'Barang')
                            vm.bobotBarang = vm.kategori[i].Weight;
                        else if (vm.kategori[i].DetailType === 'VHS')
                            vm.bobotVHS = vm.kategori[i].Weight;
                    }
                    vm.nama = reply.data.EvaluationMethodName;
                } else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_LOAD");
                }
            }, function (err) {
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
            });

            metodeEvaluasiService.selectDCByMethod({
                EvaluationMethodId: metode_evaluasi_id
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var hasil = reply.data;
                    for (var i = 0; i < hasil.length; i++) {
                        if (hasil[i].DetailType === 'Administrasi') {
                            Administrasi.push(hasil[i]);
                        }
                        else if (hasil[i].DetailType === 'Teknis') {
                            Teknis.push(hasil[i]);
                        }
                        else if (hasil[i].DetailType === 'Harga') {
                            Harga.push(hasil[i]);
                        }
                        else if (hasil[i].DetailType === 'Barang') {
                            Barang.push(hasil[i]);
                        }
                        else if (hasil[i].DetailType === 'VHS') {
                            VHS.push(hasil[i]);
                        }
                    }
                    for (var i = 0; i < Administrasi.length; i++) {
                        if (Administrasi[i].Level === 1) {
                            AdministrasiLevel1.push(Administrasi[i]);
                        }
                        else if (Administrasi[i].Level === 2) {
                            AdministrasiLevel2.push(Administrasi[i]);
                        }
                        else if (Administrasi[i].Level === 3) {
                            AdministrasiLevel3.push(Administrasi[i]);
                        }
                    }
                    for (var i = 0; i < AdministrasiLevel2.length; i++) {
                        AdministrasiLevel2[i].sub = [];
                        for (var j = 0; j < AdministrasiLevel3.length; j++) {
                            if (AdministrasiLevel3[j].Parent === AdministrasiLevel2[i].CriteriaId) {
                                AdministrasiLevel2[i].sub.push(AdministrasiLevel3[j]);
                            }
                        }
                    }
                    for (var i = 0; i < AdministrasiLevel1.length; i++) {
                        AdministrasiLevel1[i].sub = [];
                        for (var j = 0; j < AdministrasiLevel2.length; j++) {
                            if (AdministrasiLevel2[j].Parent === AdministrasiLevel1[i].CriteriaId) {
                                AdministrasiLevel1[i].sub.push(AdministrasiLevel2[j]);
                            }
                        }

                    }

                    for (var i = 0; i < Teknis.length; i++) {
                        if (Teknis[i].Level === 1) {
                            TeknisLevel1.push(Teknis[i]);
                        }
                        else if (Teknis[i].Level === 2) {
                            TeknisLevel2.push(Teknis[i]);
                        }
                        else if (Teknis[i].Level === 3) {
                            TeknisLevel3.push(Teknis[i]);
                        }
                    }
                    for (var i = 0; i < TeknisLevel2.length; i++) {
                        TeknisLevel2[i].sub = [];
                        for (var j = 0; j < TeknisLevel3.length; j++) {
                            if (TeknisLevel3[j].Parent === TeknisLevel2[i].CriteriaId) {
                                TeknisLevel2[i].sub.push(TeknisLevel3[j]);
                            }
                        }
                    }
                    for (var i = 0; i < TeknisLevel1.length; i++) {
                        TeknisLevel1[i].sub = [];
                        for (var j = 0; j < TeknisLevel2.length; j++) {
                            if (TeknisLevel2[j].Parent === TeknisLevel1[i].CriteriaId) {
                                TeknisLevel1[i].sub.push(TeknisLevel2[j]);
                            }
                        }
                    }

                    for (var i = 0; i < Harga.length; i++) {
                        if (Harga[i].Level === 1) {
                            HargaLevel1.push(Harga[i]);
                        }
                        else if (Harga[i].Level === 2) {
                            HargaLevel2.push(Harga[i]);
                        }
                        else if (Harga[i].Level === 3) {
                            HargaLevel3.push(Harga[i]);
                        }
                    }
                    for (var i = 0; i < HargaLevel2.length; i++) {
                        HargaLevel2[i].sub = [];
                        for (var j = 0; j < HargaLevel3.length; j++) {
                            if (HargaLevel3[j].Parent === HargaLevel2[i].CriteriaId) {
                                HargaLevel2[i].sub.push(HargaLevel3[j]);
                            }
                        }
                    }
                    for (var i = 0; i < HargaLevel1.length; i++) {
                        HargaLevel1[i].sub = [];
                        for (var j = 0; j < HargaLevel2.length; j++) {
                            if (HargaLevel2[j].Parent === HargaLevel1[i].CriteriaId) {
                                HargaLevel1[i].sub.push(HargaLevel2[j]);
                            }
                        }
                    }

                    for (var i = 0; i < Barang.length; i++) {
                        if (Barang[i].Level === 1) {
                            BarangLevel1.push(Barang[i]);
                        }
                        else if (Barang[i].Level === 2) {
                            BarangLevel2.push(Barang[i]);
                        }
                        else if (Barang[i].Level === 3) {
                            BarangLevel3.push(Barang[i]);
                        }
                    }
                    for (var i = 0; i < BarangLevel2.length; i++) {
                        BarangLevel2[i].sub = [];
                        for (var j = 0; j < BarangLevel3.length; j++) {
                            if (BarangLevel3[j].Parent === BarangLevel2[i].CriteriaId) {
                                BarangLevel2[i].sub.push(BarangLevel3[j]);
                            }
                        }
                    }
                    for (var i = 0; i < BarangLevel1.length; i++) {
                        BarangLevel1[i].sub = [];
                        for (var j = 0; j < BarangLevel2.length; j++) {
                            if (BarangLevel2[j].Parent === BarangLevel1[i].CriteriaId) {
                                BarangLevel1[i].sub.push(BarangLevel2[j]);
                            }
                        }
                    }

                    for (var i = 0; i < VHS.length; i++) {
                        if (VHS[i].Level === 1) {
                            VHSLevel1.push(VHS[i]);
                        }
                        else if (VHS[i].Level === 2) {
                            VHSLevel2.push(VHS[i]);
                        }
                        else if (VHS[i].Level === 3) {
                            VHSLevel3.push(VHS[i]);
                        }
                    }
                    for (var i = 0; i < VHSLevel2.length; i++) {
                        VHSLevel2[i].sub = [];
                        for (var j = 0; j < VHSLevel3.length; j++) {
                            if (VHSLevel3[j].Parent === VHSLevel2[i].CriteriaId) {
                                VHSLevel2[i].sub.push(VHSLevel3[j]);
                            }
                        }
                    }
                    for (var i = 0; i < VHSLevel1.length; i++) {
                        VHSLevel1[i].sub = [];
                        for (var j = 0; j < VHSLevel2.length; j++) {
                            if (VHSLevel2[j].Parent === VHSLevel1[i].CriteriaId) {
                                VHSLevel1[i].sub.push(VHSLevel2[j]);
                            }
                        }
                    }

                    vm.Administrasi = AdministrasiLevel1;
                    vm.Teknis = TeknisLevel1;
                    vm.Harga = HargaLevel1;
                    vm.Barang = BarangLevel1;
                    vm.VHS = VHSLevel1;
                } else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_LOAD");
                }
            }, function (err) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
            });
        };

        vm.keluar = keluar;
        function keluar() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();