(function () {
    'use strict';

    angular.module("app")
    .controller("listMetodeEvaluasiCtrl", ctrl);
    
    ctrl.$inject = ['$state', '$http', '$filter','$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'MetodeEvaluasiService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $http, $filter, $uibModal, $translate, $translatePartialLoader, $location, SocketService, metodeEvaluasiService, UIControlService) {
        var vm = this;
	    vm.totalItems = 0;
        vm.currentPage = 1;
        vm.maxSize = 10;
        vm.srcText = "";
        var srcText = "";
        var lang;
        var loadingMessage = "";
        //TODO
        vm.userBisaMengatur = true;
        vm.userBisaMenambah = true;
        vm.userBisaMengubah = true;
        vm.userBisaMenghapus = true;
        vm.searchBy = 0;
        vm.metodeEvaluasi = [];
        vm.menuhome = 0;
        vm.page_id = 140;
        
        //functions
        vm.init = init;
        vm.loadAwal = loadAwal;
        vm.cari = cari;
        vm.cek_authorize = cek_authorize;
        vm.loadMetodeEvaluasi = loadMetodeEvaluasi;
        vm.ubah_aktif = ubah_aktif;
        vm.jLoad = jLoad;
        vm.ubahDetail = ubahDetail;
        vm.lihatDetail = lihatDetail;
        vm.addMetodeEvaluasi = addMetodeEvaluasi;
        
        function init() {
//            vm.menuhome = $rootScope.menuhome;
//            $rootScope.getSession().then(function(result) {
//                $rootScope.userSession = result.data.data;
//                $rootScope.userLogin = $rootScope.userSession.session_data.username;
//                $rootScope.authorize(new function() {
//                    vm.loadAwal();
//                });
//            });
            $translatePartialLoader.addPart('metode-evaluasi');
            $translate.refresh().then(function () {
                loadingMessage = $filter('translate')('MESSAGE.LOADING');
            });
            vm.loadAwal();
            lang = $translate.use();
        };
        function cek_authorize(action) {
            $rootScope.authorize(action);
        };
            
        function loadAwal() {
            vm.loadMetodeEvaluasi();
        } // end loadAwal
        
        function cari() {
            srcText = vm.srcText;
            vm.currentPage = 1;
            vm.loadMetodeEvaluasi();
        };

        function loadMetodeEvaluasi() {
            UIControlService.loadLoading(loadingMessage);
            metodeEvaluasiService.count({
                keyword: srcText
            }, function(reply) {
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.totalItems = data;
                    jLoad();
                }
                else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD'));
                    UIControlService.unloadLoading();
                }
            }, function(err) {
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD'));
                UIControlService.unloadLoading();
            });
        };
        
        function jLoad() {
            var offset = (vm.currentPage - 1) * vm.maxSize;
            metodeEvaluasiService.select({
                offset: offset,
                limit: vm.maxSize,
                keyword: srcText
            }, function(reply) {
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.metodeEvaluasi = data;
                }
                else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD'));
                }
                UIControlService.unloadLoading();
            }, function(err) {
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD'));
                UIControlService.unloadLoading();
            });
        };
        
        function addMetodeEvaluasi(){
            $state.transitionTo('tambah-metode-evaluasi', {id:0});
        }
        
        function ubah_aktif(metode) {
            UIControlService.loadLoading(loadingMessage);
            metodeEvaluasiService.isUsed({
                EvaluationMethodId: metode.EvaluationMethodId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.data === true) {
                    UIControlService.msg_growl('warning', $filter('translate')('MESSAGE.ERR_ISUSED'));
                } else {
                    var pesan = "";

                    switch (lang) {
                        case 'id': pesan = 'Anda yakin untuk ' + (metode.IsActive ? 'menonaktifkan' : 'mengaktifkan') + ' Metode Evaluasi "' + metode.EvaluationMethodName + '"?'; break;
                        default: pesan = 'Are you sure want to ' + (metode.IsActive ? 'aktivate' : 'deactivate') + ' this Evaluation Method : "' + metode.EvaluationMethodName + '" ?'; break;
                    }

                    bootbox.confirm(pesan, function (yes) {
                        if (yes) {
                            UIControlService.loadLoading(loadingMessage);
                            metodeEvaluasiService.switchActive({
                                EvaluationMethodId: metode.EvaluationMethodId,
                            }, function (reply) {
                                UIControlService.unloadLoading();
                                if (reply.status === 200) {
                                    UIControlService.msg_growl("notice", "MESSAGE.SUCC_TOGGLE_ACTIVATION");
                                    loadMetodeEvaluasi();
                                } else {
                                    UIControlService.msg_growl("error", "MESSAGE.ERR_TOGGLE_ACTIVATION");
                                }
                            }, function (err) {
                                UIControlService.unloadLoading();
                                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_TOGGLE_ACTIVATION'));
                            });
                        }
                    })
                }
            });
        };
        
        function ubahDetail(metode_id) {
            //cek apakah metode evaluasi ini sudah digunakan untuk Pengadaan
            UIControlService.loadLoading(loadingMessage);
            metodeEvaluasiService.isUsed({
                EvaluationMethodId: metode_id
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status == 200) {
                    if (reply.data === false) {
                        $state.transitionTo('tambah-metode-evaluasi', { id: metode_id });
                    } else {
                        UIControlService.msg_growl('warning', $filter('translate')('MESSAGE.ERR_ISUSED'));
                        /*
                        var modalInstance = $uibModal.open({
                            templateUrl: 'warningUbahMetodeEvaluasi.html',
                            controller: warningUbahMetodeEvaluasiCtrl
                        });
                        modalInstance.result.then(function() {
                            $state.transitionTo('detail-metode-evaluasi', {metode_id: metode_id});
                        });
                        */
                    }
                }
            }, function (err) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl('error', $filter('translate')('MESSAGE.ERR_ISUSED'));
            });
        };
        
        function lihatDetail(metode_id) {
            var kirim = {
                metode_evaluasi_id: metode_id
            };
            $uibModal.open({
                templateUrl: 'app/modules/panitia/metode-evaluasi/metodeEvaluasi.modal.html',
                controller: 'modalDetailMetodeEvaluasiCtrl',
                controllerAs: 'modalDMECtrl',
                resolve: {
                    item: function() {
                        return kirim;
                    }
                }
            });
        };
    }
    
    function Kata(srcText) {
        var self = this;
        self.srcText = srcText;
    }
})();