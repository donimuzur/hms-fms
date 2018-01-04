(function () {
    'use strict';

    angular.module("app")
    .controller("tambahMetodeEvaluasi", ctrl);
    
    ctrl.$inject = ['$http', '$state', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'MetodeEvaluasiService', 'UIControlService'];
    /* @ngInject */
    function ctrl($http, $state, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, metodeEvaluasiService, UIControlService) {

        var evaluationMethodId = Number($stateParams.id);
        var vm = this;
        var evaluationMethodModel;
        var loadingMessage = "";

        vm.isEdit = evaluationMethodId > 0;
        vm.namaMetode = "";
        vm.barangOrJasa = "1";
        vm.batasPenawaran = null;
        vm.kriteria = [];
        vm.menuhome = 0;
        vm.page_id = 140;
        
        //functions
        vm.init = init;
        vm.loadDataAdd = loadDataAdd;

        //Bobot Default untuk Jasa
        var defaultTeknis = 25;
        var defaultHarga = 75;

        function init() {
            $translatePartialLoader.addPart('metode-evaluasi');
            $translate.refresh().then(function () {
                loadingMessage = $filter('translate')('MESSAGE.LOADING');
            });
            vm.loadDataAdd();
        };

        function loadDataAdd() {
            loadKriteriaList(vm.barangOrJasa);
            if (vm.isEdit) {
                UIControlService.loadLoading(loadingMessage);
                metodeEvaluasiService.selectById({
                    EvaluationMethodId : evaluationMethodId
                }, function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        evaluationMethodModel = reply.data;
                        vm.namaMetode = evaluationMethodModel.EvaluationMethodName;
                        vm.barangOrJasa = String(evaluationMethodModel.GoodsOrService);
                        vm.batasPenawaran = evaluationMethodModel.OfferedPriceLimit;
                        loadKriteriaList(vm.barangOrJasa);
                        if (evaluationMethodModel.EvaluationMethodDetails.length > 0) {
                            vm.kriteriaList.forEach(function (kl) {
                                evaluationMethodModel.EvaluationMethodDetails.forEach(function (emm) {
                                    if (kl.kriteria_nama === emm.DetailType) {
                                        kl.id = emm.EMDId;
                                        kl.checked = emm.IsActive;
                                        kl.bobot = emm.Weight;
                                    }
                                });
                            });
                        }
                    } else {
                        UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD'));
                    }
                }, function (err) {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD'));
                });
            }
        }

        function loadKriteriaList(barangOrJasa) {
            if (barangOrJasa === '1') { //Barang
                vm.kriteriaList = [
                    { kriteria_nama: "Barang", checked: true, bobot: 100 },
                ];
            } else if (barangOrJasa === '2') { //Jasa
                vm.kriteriaList = [
                    //{ kriteria_nama: "Administrasi", checked: true, bobot: 0 },
                    { kriteria_nama: "Teknis", checked: true, bobot: defaultTeknis },
                    { kriteria_nama: "Harga", checked: true, bobot: defaultHarga }
                ];
            } else if (barangOrJasa === '3' || barangOrJasa === '4') { //VHS/FPA
                vm.kriteriaList = [
                    { kriteria_nama: "VHS", checked: true, bobot: 100 },
                ];
            }
        }

        vm.barangOrJasaChange = barangOrJasaChange;
        function barangOrJasaChange() {
            loadKriteriaList(vm.barangOrJasa);
        }

        vm.tambah = tambah;
        function tambah() {
            if (vm.namaMetode === "") {
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_NONAME'));
                return;
            }

            var metodeEvaluasi = {
                EvaluationMethodName: vm.namaMetode,
                GoodsOrService: vm.barangOrJasa,
                OfferedPriceLimit: Number(vm.barangOrJasa) === 2 ? vm.batasPenawaran : null,
                EvaluationMethodDetails: []
            }

            if (Number(vm.barangOrJasa) === 2 && (vm.batasPenawaran === null || vm.batasPenawaran === undefined || vm.batasPenawaran > 100 || vm.batasPenawaran < 0)) {
                UIControlService.msg_growl("error", $filter('translate')("MESSAGE.ERR_INVALID_LIMIT"));
                return;
            }

            if (vm.isEdit) {
                metodeEvaluasi.EvaluationMethodId = evaluationMethodModel.EvaluationMethodId;
            }

            var totalPersen = 0;
            var details = [];
            vm.kriteriaList.forEach(function (k) {
                var detail = {
                    DetailType: k.kriteria_nama,
                    Weight: k.checked ? k.bobot : 0,
                    IsActive: k.checked
                }
                if (k.id > 0) {
                    detail.EMDId = k.id;
                }
                totalPersen += detail.Weight;
                details.push(detail);
            });

            if (details.length === 0) {
                UIControlService.msg_growl("error", $filter('translate')("MESSAGE.ERR_NOCRIT"));
                return;
            }
            if (totalPersen !== 100) {
                UIControlService.msg_growl("error", $filter('translate')("MESSAGE.ERR_NOT100"));
                return;
            }

            metodeEvaluasi.EvaluationMethodDetails = details;            

            if (vm.isEdit) {
                UIControlService.loadLoading(loadingMessage);
                metodeEvaluasiService.update(metodeEvaluasi,
                    function (reply) {
                        UIControlService.unloadLoading();
                        if (reply.status === 200) {
                            UIControlService.msg_growl("notice", "MESSAGE.SUCC_UPDATE");
                            $state.transitionTo('metode-evaluasi');
                        }
                        else {
                            UIControlService.msg_growl("error", $filter('translate')("MESSAGE.ERR_UPDATE"));
                        }
                    }, function (err) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_UPDATE'));
                    }
                );
            } else {
                UIControlService.loadLoading(loadingMessage);
                metodeEvaluasiService.insert(metodeEvaluasi,
                    function (reply) {
                        UIControlService.unloadLoading();
                        if (reply.status === 200) {
                            UIControlService.msg_growl("notice", "MESSAGE.SUCC_INSERT");
                            $state.transitionTo('metode-evaluasi');
                        }
                        else {
                            UIControlService.msg_growl("error", $filter('translate')("MESSAGE.ERR_INSERT"));
                        }
                    }, function (err) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_INSERT'));
                    }
                );
            }
        };

        vm.detail = detail;
        function detail(id) {
            $state.transitionTo('detail-metode-evaluasi', {id: id});
        }

        vm.back = back;
        function back() {
            $state.transitionTo('metode-evaluasi');
        };
    }
})();