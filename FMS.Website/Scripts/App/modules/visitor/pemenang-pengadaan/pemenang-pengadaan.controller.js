(function () {
    'use strict';
    angular.module("app")
    .controller("PemenangPengadaanCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'PemenangPengadaanService'];
    /* @ngInject */
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, PemenangPengadaanService) {
        /* jshint validthis: true */
        //console.info("pemenang!");
        var vm = this;
        vm.panels = [];
        // functions
        vm.init = init;
        vm.loadPemenangPengadaan = loadPemenangPengadaan;
        vm.jLoad = jLoad;
        vm.cariPemenang = cariPemenang;
        // function declarations
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.maxSize = 1;
        vm.srcText = "";
        vm.pemenang = [];


        function init(){
            //loadPemenangPengadaan(); //sementara di komen krn rootScope
            $translatePartialLoader.addPart('pemenang-pengadaan');
        };

        function loadPemenangPengadaan() {
            vm.currentPage = 1;
            $rootScope.loadLoading("Silahkan Tunggu...");
            $http.post($rootScope.url_api + 'pemenang/count', {param: 1}).success(function(reply) {
                if (reply.status === 200) {
                    vm.totalItems = reply.result.data;
                }
                else
                    alert("gagal mendapatkan jumlah pemenang");
            });

            var offset = 0;
            var limit = 10;
            var param2 = [];
            param2.push(1);
            $http.post($rootScope.url_api + 'pemenang/select', {param: param2,
                action: 'view'
            }).success(function(reply) {
                $rootScope.unloadLoading();
                if (reply.status === 200) {
                    vm.pemenang = reply.result.data;
                }
                else
                    alert("gagal mendapatkan list pemenang");
            });
        }
        ;

        function jLoad (current) {
            vm.pemenang = [];
            vm.currentPage = current;
            vm.offset = (current * 10) - 10;
            var param = [];
            param.push("%" + vm.srcText + "%");
            param.push(vm.offset);
            var limit = 10;
            param.push(limit);
            $rootScope.loadLoading("Silahkan Tunggu...");
            $http.post($rootScope.url_api + 'pemenang/select', {param: param}).success(function(reply) {
                $rootScope.unloadLoading();
                if (reply.status === 200)
                    vm.pemenang = reply.result.data;
                else
                    alert("gagal mendapatkan list pemenang");
            });
        };

        function detailPemenang(idPaket) {
            $state.transitionTo('pemenang-pengadaan-detail', {idPaket: idPaket});
        };

        function cariPemenang() {
            var param = [];
            param.push(1);
            $rootScope.loadLoading("Silahkan Tunggu...");
            $http.post($rootScope.url_api + 'pemenang/count', {param: 1}).success(function(reply) {
                if (reply.status === 200) {
                    vm.totalItems = reply.result.data;
                }
                else
                    alert("gagal mendapatkan jumlah pemenang");
            });
            var param2 = [];
            param2.push("%" + vm.srcText + "%");

            $http.post($rootScope.url_api + 'pemenang/select', {param: param2, action: 'cariLelang'}).success(function(reply) {
                $rootScope.unloadLoading();
                if (reply.status === 200) {
                    vm.pemenang = reply.result.data;
                }
                else
                    alert("gagal mendapatkan list pemenang");
            });
        };
    }
})();


//angular.module('eprocApp')
//        .controller('pemenangPengadaanCtrl', function(vm, $http, $cookies, $state, $rootScope, $modal) {
//            vm.totalItems = 0;
//            vm.currentPage = 1;
//            vm.maxSize = 1;
//            vm.srcText = "";
//            vm.pemenang = [];
//
//
//            vm.init = function() {
//                loadPemenangPengadaan();
//            };
//
//            function loadPemenangPengadaan() {
//                vm.currentPage = 1;
////                var param = [];
////                param.push(1);
//                $rootScope.loadLoading("Silahkan Tunggu...");
//                $http.post($rootScope.url_api + 'pemenang/count', {param: 1}).success(function(reply) {
//                    if (reply.status === 200) {
//                        vm.totalItems = reply.result.data;
//                    }
//                    else
//                        alert("gagal mendapatkan jumlah pemenang");
//                });
//
//                var offset = 0;
//                var limit = 10;
//                var param2 = [];
//                param2.push(1);
//                $http.post($rootScope.url_api + 'pemenang/select', {param: param2,
//                    action: 'view'
//                }).success(function(reply) {
//                    $rootScope.unloadLoading();
//                    if (reply.status === 200) {
//                        vm.pemenang = reply.result.data;
//                    }
//                    else
//                        alert("gagal mendapatkan list pemenang");
//                });
//            }
//            ;
//
//            vm.jLoad = function(current) {
//                vm.pemenang = [];
//                vm.currentPage = current;
//                vm.offset = (current * 10) - 10;
//                var param = [];
//                param.push("%" + vm.srcText + "%");
//                param.push(vm.offset);
//                var limit = 10;
//                param.push(limit);
//                $rootScope.loadLoading("Silahkan Tunggu...");
//                $http.post($rootScope.url_api + 'pemenang/select', {param: param}).success(function(reply) {
//                    $rootScope.unloadLoading();
//                    if (reply.status === 200)
//                        vm.pemenang = reply.result.data;
//                    else
//                        alert("gagal mendapatkan list pemenang");
//                });
//            };
//
//            vm.detailPemenang = function(idPaket) {
//                $state.transitionTo('pemenang-pengadaan-detail', {idPaket: idPaket});
//            };
//
//            vm.cariPemenang = function() {
//                var param = [];
//                param.push(1);
//                $rootScope.loadLoading("Silahkan Tunggu...");
//                $http.post($rootScope.url_api + 'pemenang/count', {param: 1}).success(function(reply) {
//                    if (reply.status === 200) {
//                        vm.totalItems = reply.result.data;
//                    }
//                    else
//                        alert("gagal mendapatkan jumlah pemenang");
//                });
//                var param2 = [];
//                param2.push("%" + vm.srcText + "%");
////                var offset = 0;
////                param2.push(offset);
////                var limit = 10;
////                param2.push(limit);
//                $http.post($rootScope.url_api + 'pemenang/select', {param: param2, action: 'cariLelang'}).success(function(reply) {
//                    $rootScope.unloadLoading();
//                    if (reply.status === 200) {
//                        vm.pemenang = reply.result.data;
//                    }
//                    else
//                        alert("gagal mendapatkan list pemenang");
//                });
//            };
//        })
//        .controller('pemenangPengadaanDetailCtrl', function($stateParams, vm, $http, $cookies, $state, $rootScope, $modal) {
//            vm.idPaket = Number($stateParams.idPaket);
//            vm.pemenang = [];
//
//            vm.init = function() {
//                loadAwalDetail();
//            };
//
//            function loadAwalDetail() {
//                var param = [];
//                param.push(vm.idPaket);
//                $http.post($rootScope.url_api + 'pemenang/select', {param: param, action: 'detail'}).success(function(reply) {
//                    if (reply.status === 200) {
//                        vm.pemenang = reply.result.data;
//                    }
//                    else
//                        alert("gagal mendapatkan list pemenang");
//                });
//            }
//            ;
//            vm.kembali = function() {
//                $state.transitionTo('pemenang-pengadaan');
//            };
//            vm.cariPemenang = function() {
//                var param = [];
//                param.push(1);
//                $http.post($rootScope.url_api + 'pemenang/count', {param: 1}).success(function(reply) {
//                    if (reply.status === 200) {
//                        vm.totalItems = reply.result.data;
//                    }
//                    else
//                        alert("gagal mendapatkan jumlah pemenang");
//                });
//                var param2 = [];
//                param2.push(vm.idPaket);
//                param2.push("%" + vm.srcText + "%");
////                var offset = 0;
////                param2.push(offset);
////                var limit = 10;
////                param2.push(limit);
//                $http.post($rootScope.url_api + 'pemenang/select', {param: param2, action: 'cariRekanan'}).success(function(reply) {
//                    if (reply.status === 200) {
//                        vm.pemenang = reply.result.data;
//                    }
//                    else
//                        alert("gagal mendapatkan list pemenang");
//                });
//            };
//        });
//;
//
