(function () {
    'use strict';

    angular.module("app")
    .controller("PengingatAktivitasCtrl", ctrl);
    
    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'PendaftaranRekService'];
    /* @ngInject */
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, PendaftaranRekService) {
        var vm = this;
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.maxSize = 10;
        vm.paket = [];
        vm.statusTercentang = false;
        vm.namaLelangTercentang = false;
        vm.namaTahapanTercentang = false;
        vm.selectedStatus = "5";
        vm.srcNamaLelang = "";
        vm.srcNamaTahapan = "";
        
        vm.is_verified = false;
        vm.SIUP_expired = false;
        vm.blacklist;
        vm.ubahDataApproved;
        vm.tanggalBolehUbahData;

        vm.totalItemsPekerjaan = 0;
        vm.currentPagePekerjaan = 1;
        vm.maxSizePekerjaan = 10;
        vm.pekerjaan = [];
        vm.namaPekerjaanTercentang = false;
        vm.waktuMulaiTercentang = false;
        vm.waktuSelesaiTercentang = false;
        vm.srcTextPekerjaan = "";
        vm.waktuMulai1 = "";
        vm.waktuMulai2 = "";
        vm.waktuSelesai1 = "";
        vm.waktuSelesai2 = "";
        vm.tolak = "";
        
        vm.init = init;
        
        function init(){
            
        }
    }
})();
    
////angular.module('eprocApp')
//.controller( 'pengingatAktivitasCtrl', function( $scope, $http, $cookieStore, $state, $rootScope, $modal){
//
//    $scope.totalItems = 0;
//    $scope.currentPage = 1;
//    $scope.maxSize = 10;
//    $scope.paket = [];
//    $scope.statusTercentang = false;
//    $scope.namaLelangTercentang = false;
//    $scope.namaTahapanTercentang = false;
//    $scope.selectedStatus = "5";
//    $scope.srcNamaLelang = "";
//    $scope.srcNamaTahapan = "";
//
//    $scope.is_verified = false;
//    $scope.SIUP_expired = false;
//    $scope.blacklist;
//    $scope.ubahDataApproved;
//    $scope.tanggalBolehUbahData;
//
//    $scope.totalItemsPekerjaan = 0;
//    $scope.currentPagePekerjaan = 1;
//    $scope.maxSizePekerjaan = 10;
//    $scope.pekerjaan = [];
//    $scope.namaPekerjaanTercentang = false;
//    $scope.waktuMulaiTercentang = false;
//    $scope.waktuSelesaiTercentang = false;
//    $scope.srcTextPekerjaan = "";
//    $scope.waktuMulai1 = "";
//    $scope.waktuMulai2 = "";
//    $scope.waktuSelesai1 = "";
//    $scope.waktuSelesai2 = "";
//    $scope.tolak = "";
//
//    /* TIDAK DIPAKAI DI VERSI PHP
//    eb.onopen = function(){ 
//        eb.send( auth, {sessionID: sess}, function( authReply ){ // Auth-Step2: authorizing (long auth)
//            if( authReply.status === 'ok' ){
//                
//                // Auth-Step3: set user data
//                $rootScope.isLogged = true;                
//                $rootScope.userlogged = authReply.username; 
//                $rootScope.$apply(); 
//                $scope.$apply(); 
//                                
//                // Auth-Step4: load data RekananID
//                eb.send( 'rekanan.rekanan.getRekananID', {sessionID: sess, username: $rootScope.userlogged}, function( dataReply ){ 
//                    if( dataReply.status === 'ok' ){
//                        $rootScope.rekanan_id = dataReply.result[0].rekanan_id;
//                        $rootScope.$apply(); 
//                        $scope.$apply(); 
//                        
//                        // Auth-Step5: calling MAIN action
//                        $scope.init(); 
//                        
//                        // Auth-Step6: load data RekananNama
//                        eb.send( 'rekanan.rekanan.getNama', {sessionID: sess, rekanan_id: $rootScope.rekanan_id}, function( dataReply2 ){ 
//                            if( dataReply2.status === 'ok' ){
//                                $rootScope.nama_perusahaan = dataReply2.result[0].nama_perusahaan;
//                                $rootScope.$apply(); 
//                                $scope.$apply(); 
//                            }
//                            else $.growl.error({ title: $rootScope.ttlWarning, message: $rootScope.msgCantGetCompanyName }); 
//                        });                        
//                        
//                    } else $.growl.error({ title: $rootScope.ttlWarning, message: $rootScope.msgCantGetCompanyID }); 
//                });
//            } else {
//                // Auth-Step7: found no auth!
//                $.growl.error({ title: $rootScope.ttlWarning, message: $rootScope.msgNoAuth });
//                $rootScope.isLogged = false;
//                $rootScope.userLogged = "";
//                $state.transitionTo('login-rekanan');
//                $rootScope.$apply(); 
//                $scope.$apply(); 
//            }
//        });
//    };
//    */
//
//    $scope.init = function(){
//        $rootScope.getSession().then(function(result){
//            $rootScope.userSession = result.data.data;
//            $rootScope.userLogin = $rootScope.userSession.session_data.username;
//            $rootScope.rekanan_id = $rootScope.userSession.session_data.rekanan_id;
//            $rootScope.nama_perusahaan = $rootScope.userSession.session_data.nama_perusahaan;
//            $rootScope.authorize(new function(){
//                loadAwal_NoAuth(); 
//                loadPaket_NoAuth();
//                loadPekerjaan_NoAuth();
//            });
//        });
//    };
//
//    function loadAwal_NoAuth(){
//        $rootScope.loadLoading('Silahkan tunggu...');
//        var rekanan_id = [];
//        rekanan_id.push( $rootScope.rekanan_id );
//        
//        //rekanan.rekanan.cekVerified
//        $http.post($rootScope.url_api + 'rekanan/cekVerified', {
//            param: rekanan_id
//        }).success(function(reply){
//            if(reply.status === 200){
//                $scope.is_verified = reply.result.data[0].is_verified;
//                $scope.tolak = reply.result.data[0].keterangan;
//            }
//        }).error(function(err) {
//            $.growl.error({ message: "Gagal Akses API >"+err });
//            return;
//        }); 
//
//        //rekanan.rekanan.cekSIUP
//        $http.post($rootScope.url_api + 'rekanan/cekSIUP', {
//            param: rekanan_id
//        }).success(function( reply ){
//            if( reply.status === 200 ){
//                $scope.SIUP_expired = reply.result.data.expired;
//            }
//        }).error(function(err) {
//            $.growl.error({ message: "Gagal Akses API >"+err });
//            return;
//        });
//
//        /*
//        eb.send( 'rekanan.rekanan.cekBlacklist', {
//            rekanan_id: $rootScope.rekanan_id
//        }, function( reply ){
//            if( reply.status === 200 ){
//                $scope.blacklist = reply.result.data[0]; 
//                
//                if( !($scope.blacklist.masaakhir_blacklist === null) && !($scope.blacklist.masaakhir_blacklist === '') )
//                    $scope.blacklist.masaakhir_blacklist = $rootScope.convertDate( $scope.blacklist.masaakhir_blacklist );
//                
//                $scope.$apply();
//            }
//        });
//        */
//        $scope.blacklist = {
//            flag_blacklist : $rootScope.userSession.session_data.flag_blacklist,
//            masa_blacklist : $rootScope.userSession.session_data.masa_blacklist,
//            masaakhir_blacklist : $rootScope.userSession.session_data.masaakhir_blacklist,
//            alasan_blacklist : $rootScope.userSession.session_data.alasan_blacklist
//        };
//        if(!($scope.blacklist.masaakhir_blacklist === null) && !($scope.blacklist.masaakhir_blacklist === '')){
//            $scope.blacklist.masaakhir_blacklist = $rootScope.convertTanggal( $scope.blacklist.masaakhir_blacklist );
//        }
//
//        //rekanan.rekanan.hapusRequestYangKadaluarsa
//        $http.post($rootScope.url_api + 'rekanan/hapusrequestkadaluarsa', {
//            rekanan_id: $rootScope.rekanan_id
//        }).success(function( reply ){
//            if( reply.status === 200 ){
//                //rekanan.rekanan.cekRequestUbahDataApproved
//                $http.post($rootScope.url_api + 'rekanan/cekrequestubahdata', {
//                    rekanan_id: $rootScope.rekanan_id
//                }).success(function( reply2 ){
//                    if( reply2.status === 200 ){
//                        if( reply2.result.data.bisa === true ){
//                            $scope.ubahDataApproved = true;
//                            $scope.tanggalBolehUbahData = reply2.result.data.balasan;
//                            $scope.tanggalBolehUbahData.akhir_ubah_data = $rootScope.convertTanggal( $scope.tanggalBolehUbahData.akhir_ubah_data );
//                        } else {
//                            $scope.ubahDataApproved = false;
//                        }
//                    }
//                });
//            }
//            $rootScope.unloadLoading();
//        }).error(function(err) {
//            $.growl.error({ message: "Gagal Akses API >"+err });
//            return;
//        });
//    } // end loadAwal_NoAuth
//
//    function loadPaket(){ 
////        eb.send( auth, {sessionID: sess}, function( authReply ){ // Auth-Step2: authorizing (short auth)
////            if( authReply.status === 'ok' ){
//                $rootScope.authorize(loadPaket_NoAuth()); // Auth-Step3: calling MAIN action                
////            } else {
////                // Auth-Step4: found no auth!
////                $.growl.error({ title: $rootScope.ttlWarning, message: $rootScope.msgNoAuth });
////                $rootScope.isLogged = false;
////                $rootScope.userLogged = "";
////                $state.transitionTo('login-rekanan');
////                $rootScope.$apply(); 
////                $scope.$apply(); 
////            }
////        });        
//    }
//
//    function loadPaket_NoAuth(){ 
//        $rootScope.loadLoading('Silahkan tunggu...');
//        $scope.currentPage = 1;
//        
//        //rekanan.paket.count
//        $http.post($rootScope.url_api + 'rekanan/paket/count', {
//            rekanan_id: $rootScope.rekanan_id,
//            statusTercentang: $scope.statusTercentang,
//            namaLelangTercentang: $scope.namaLelangTercentang,
//            namaTahapanTercentang: $scope.namaTahapanTercentang,
//            status: Number($scope.selectedStatus),
//            namaLelang: "%" + $scope.srcNamaLelang + "%",
//            namaTahapan: "%" + $scope.srcNamaTahapan + "%"
//        }).success(function( reply ){
//            if( reply.status === 200 ){
//                $scope.totalItems = reply.result.data;
//            }
//        }).error(function(err) {
//            $.growl.error({ message: "Gagal Akses API >"+err });
//            return;
//        });
//
//        $scope.paket = [];
//        $scope.paket.tahapanLama = [];
//        $scope.paket.tahapanNext = [];
//        var offset = 0;
//        var limit = 10;
//
//        //rekanan.paket.select
//        $http.post($rootScope.url_api + 'rekanan/paket/select', {
//            rekanan_id: $rootScope.rekanan_id,
//            statusTercentang: $scope.statusTercentang,
//            namaLelangTercentang: $scope.namaLelangTercentang,
//            namaTahapanTercentang: $scope.namaTahapanTercentang,
//            status: Number($scope.selectedStatus),
//            namaLelang: "%" + $scope.srcNamaLelang + "%",
//            namaTahapan: "%" + $scope.srcNamaTahapan + "%",
//            offset: offset,
//            limit: limit
//        }).success(function( reply ){                    
//            if( reply.result === undefined ){                        
//                $rootScope.isLogged = false;
//                $rootScope.userLogged = "";
//                $state.transitionTo('login-rekanan');
//            } else {
//                //var param = [];
//                //param.push( $rootScope.rekanan_id );
//                //param.push( reply.result.paket_lelang_id );
//                
//                if( reply.status === 200 ){
//                    $scope.paket = reply.result.data;
//
//                    if( $scope.paket.length > 0 ){
//                        for( var b = 0; b < $scope.paket.length; b++ ){
//                            $scope.paket[b].tahapanTersingkir = [];
//                            $scope.paket[b].tahapanSekarang = [];
//                            $scope.paket[b].tahapanLama = [];
//                            $scope.paket[b].tahapanNext = [];
//                            $scope.paket[b].pemenang = 0;
//                        }
//                        
//                        var arr1;
//                        var arr2 = [];
//                        
//                        for( var i = 0; i < $scope.paket.length; i++ ){
//                            arr1 = [];
//                            arr1.push( $scope.paket[i].paket_lelang_id );
//                            arr2.push( arr1 );
//                        }
//                        
//                        //rekanan.paket.cekTersingkir
//                        $http.post($rootScope.url_api + 'rekanan/paket/cektersingkir', {
//                            rekanan_id: $rootScope.rekanan_id, 
//                            param: arr2
//                        }).success(function( reply22 ){ // , replier22
//                            if( reply22.status === 200 ){
//                                for( var a = 0; a < reply22.result.data.length; a++ ){
//                                    if (reply22.result.data[a].tahapan_terakhir != null) {
//                                        var arra = [];
//                                        arra.push(reply22.result.data[a].paket_lelang_id);
//                                        arra.push(reply22.result.data[a].tahapan_terakhir);
//                                    }          
//                                    //rekanan.paket.cekTahapanTersingkir
//                                    $http.post($rootScope.url_api + 'rekanan/paket/cektahapantersingkir', {
//                                        param: arra
//                                    }).success(function( reply2 ){
//                                        if( reply2.status === 200 ){
//                                            $scope.tahapanSekarang = reply2.result.data[0];
//                                            for( var k = 0; k < $scope.paket.length; k++ ){
//                                                if( $scope.tahapanSekarang.paket_lelang_id === $scope.paket[k].paket_lelang_id )
//                                                    $scope.paket[k].tahapanTersingkir.push( $scope.tahapanSekarang );
//                                            }
//                                        }
//                                    }).error(function(err) {
//                                        $.growl.error({ message: "Gagal Akses API >"+err });
//                                        return;
//                                    });
//                                }
//                            }
//                        }).error(function(err) {
//                            $.growl.error({ message: "Gagal Akses API >"+err });
//                            return;
//                        });
//
//                        //rekanan.paket.cekPemenang
//                        $http.post($rootScope.url_api + 'rekanan/paket/cekpemenang', {
//                            rekanan_id: $rootScope.rekanan_id
//                        }).success(function( reply23 ){ // , replier23
//                            if( reply23.status === 200 ){
//                                for( var k = 0; k < reply23.result.data.length; k++ ){
//                                    for( var l = 0; l < $scope.paket.length; l++ ){
//                                        if( $scope.paket[l].paket_lelang_id == reply23.result.data[k].paket_lelang_id ) 
//                                            $scope.paket[l].pemenang = reply23.result.data[k].pemenang;
//                                    }
//                                }
//                            }
//                        }).error(function(err) {
//                            $.growl.error({ message: "Gagal Akses API >"+err });
//                            return;
//                        });
//
//                        //rekanan.paket.cekBertahan
//                        $http.post($rootScope.url_api + 'rekanan/paket/cekbertahan', {
//                            rekanan_id: $rootScope.rekanan_id, 
//                            param: arr2
//                        }).success(function( reply21 ){ // , replier21
//                            //rekanan.paket.cekTahapanSekarang
//                            $http.post($rootScope.url_api + 'rekanan/paket/cektahapan', {
//                                param: arr2
//                            }).success(function( reply2 ){
//                                if( reply2.status === 200 ){
//                                    for( var l = 0; l < $scope.paket.length; l++ ){
//                                        $scope.tahapanSekarang = reply2.result.data;
//                                        for( var j = 0; j < $scope.tahapanSekarang.length; j++ ){
//                                            if( $scope.paket[l].paket_lelang_id == $scope.tahapanSekarang[j].paket_lelang_id ){
//                                                if( $scope.tahapanSekarang[j].statusDay == 'Curr' ){
//                                                    $scope.paket[l].tahapanSekarang.push( $scope.tahapanSekarang[j] );
//                                                }
//                                                else if( $scope.tahapanSekarang[j].statusDay == 'Old' ){
//                                                    $scope.paket[l].tahapanLama.push( $scope.tahapanSekarang[j] );                                                    
//                                                }
//                                                else if( $scope.tahapanSekarang[j].statusDay == 'Next' ){
//                                                    $scope.paket[l].tahapanNext.push( $scope.tahapanSekarang[j] );
//                                                }
//                                            }
//                                        }
//                                        $rootScope.unloadLoading();
//                                    }
//                                    //ngProgress.complete();
//                                }
//                            }).error(function(err) {
//                                $.growl.error({ message: "Gagal Akses API >"+err });
//                                return;
//                            });
//                        }).error(function(err) {
//                            $.growl.error({ message: "Gagal Akses API >"+err });
//                            return;
//                        });
//                    } else {
//                        $rootScope.unloadLoading();
//                        //ngProgress.complete();
//                    }
//                }
//            } // end if not undefined
//        }).error(function(err) {
//            $.growl.error({ message: "Gagal Akses API >"+err });
//            return;
//        });
//    } // end loadPaket_NoAuth
//
//
//    function loadPekerjaan(){ 
////        eb.send( auth, {sessionID: sess}, function( authReply ){ // Auth-Step2: authorizing (short auth)
////            if( authReply.status === 'ok' ){
//                $rootScope.authorize(loadPekerjaan_NoAuth()); // Auth-Step3: calling MAIN action                
////            } else {
////                // Auth-Step4: found no auth!
////                $.growl.error({ title: $rootScope.ttlWarning, message: $rootScope.msgNoAuth });
////                $rootScope.isLogged = false;
////                $rootScope.userLogged = "";
////                $state.transitionTo('login-rekanan');
////                $rootScope.$apply(); 
////                $scope.$apply(); 
////            }
////        });        
//    }
//    
//    function loadPekerjaan_NoAuth(){
//        $rootScope.loadLoading('Silahkan tunggu...');
//        $scope.currentPagePekerjaan = 1;
//        
//        //rekanan.pekerjaan.count
//        $http.post($rootScope.url_api + 'rekanan/pekerjaan/count', {
//            rekanan_id: $rootScope.rekanan_id,
//            namaPekerjaanTercentang: $scope.namaPekerjaanTercentang,
//            waktuMulaiTercentang: $scope.waktuMulaiTercentang,
//            waktuSelesaiTercentang: $scope.waktuSelesaiTercentang,
//            srcTextPekerjaan: "%" + $scope.srcTextPekerjaan + "%",
//            waktuMulai1: $scope.waktuMulai1,
//            waktuMulai2: $scope.waktuMulai2,
//            waktuSelesai1: $scope.waktuSelesai1,
//            waktuSelesai2: $scope.waktuSelesai2
//        }).success(function( reply ){
//            if( reply.status === 200 ){
//                $scope.totalItemsPekerjaan = reply.result.data;
//            }
//        }).error(function(err) {
//            $.growl.error({ message: "Gagal Akses API >"+err });
//            return;
//        });
//
//        $scope.pekerjaan = [];
//        //rekanan.pekerjaan.select
//        $http.post($rootScope.url_api + 'rekanan/pekerjaan/select', {
//            rekanan_id: $rootScope.rekanan_id,
//            namaPekerjaanTercentang: $scope.namaPekerjaanTercentang,
//            waktuMulaiTercentang: $scope.waktuMulaiTercentang,
//            waktuSelesaiTercentang: $scope.waktuSelesaiTercentang,
//            srcTextPekerjaan: "%" + $scope.srcTextPekerjaan + "%",
//            waktuMulai1: $scope.waktuMulai1,
//            waktuMulai2: $scope.waktuMulai2,
//            waktuSelesai1: $scope.waktuSelesai1,
//            waktuSelesai2: $scope.waktuSelesai2,
//            offset: 0,
//            limit: 10
//        }).success(function( reply ){
//            if( reply.status === 200 ){
//                $scope.pekerjaan = reply.result.data;
//                for( var i = 0; i < $scope.pekerjaan.length; i++ ){
//                    $scope.pekerjaan[i].tgl_mulai_kontrak_formatted = $rootScope.convertTanggal( $scope.pekerjaan[i].tgl_mulai_kontrak );
//                    $scope.pekerjaan[i].tgl_selesai_kontrak_formatted = $rootScope.convertTanggal( $scope.pekerjaan[i].tgl_selesai_kontrak );
//                    //$scope.pekerjaan[i].sisa_waktu = hitungSelisihHari($scope.pekerjaan[i].tgl_selesai_kontrak);
//                }
//            }
//            $rootScope.unloadLoading();
//        }).error(function(err) {
//            $.growl.error({ message: "Gagal Akses API >"+err });
//            return;
//        });
//    } // end loadPekerjaan_NoAuth
//
//    $scope.jLoad = function( current ){
////        eb.send( auth, {sessionID: sess}, function( authReply ){ // Auth-Step2: authorizing (short auth)
////            if( authReply.status === 'ok' ){
//                $rootScope.authorize(jLoad_NoAuth(current)); // Auth-Step3: calling MAIN action                
////            } else {
////                // Auth-Step4: found no auth!
////                $.growl.error({ title: $rootScope.ttlWarning, message: $rootScope.msgNoAuth });
////                $rootScope.isLogged = false;
////                $rootScope.userLogged = "";
////                $state.transitionTo('login-rekanan');
////                $rootScope.$apply(); 
////                $scope.$apply(); 
////            }
////        });                
//    };
//    
//    function jLoad_NoAuth( current ){
//        $rootScope.loadLoading('Silahkan tunggu...');
//        $scope.paket = [];
//        $scope.currentPage = current;
//        $scope.offset = (current * 10) - 10;
//        var limit = 10;        
//        
//        //rekanan.paket.select
//        $http.post($rootScope.url_api + 'rekanan/paket/select', {
//            rekanan_id: $rootScope.rekanan_id,
//            statusTercentang: $scope.statusTercentang,
//            namaLelangTercentang: $scope.namaLelangTercentang,
//            namaTahapanTercentang: $scope.namaTahapanTercentang,
//            status: Number($scope.selectedStatus),
//            namaLelang: "%" + $scope.srcNamaLelang + "%",
//            namaTahapan: "%" + $scope.srcNamaTahapan + "%",
//            offset: $scope.offset,
//            limit: limit
//        }).success(function(reply) {
//            //var param = [];
//            //param.push($rootScope.rekanan_id);
//            //param.push(reply.result.paket_lelang_id);            
//            if (reply.status === 200) {
//
//                $scope.paket = reply.result.data;
//
//                if ($scope.paket.length > 0) {
//                    for (var b = 0; b < $scope.paket.length; b++){
//                        $scope.paket[b].tahapanTersingkir = [];
//                        $scope.paket[b].tahapanSekarang = [];
//                        $scope.paket[b].tahapanLama = [];
//                        $scope.paket[b].tahapanNext = [];
//                        $scope.paket[b].pemenang = 0;
//                    }
//                    
//                    var arr1;
//                    var arr2 = [];
//                    
//                    for (var i = 0; i < $scope.paket.length; i++){
//                        arr1 = [];
//                        arr1.push($scope.paket[i].paket_lelang_id);
//                        arr2.push(arr1);
//                    }
//                    //rekanan.paket.cekTersingkir
//                    $http.post($rootScope.url_api + 'rekanan/paket/cektersingkir', {
//                        rekanan_id: $rootScope.rekanan_id,
//                        param: arr2
//                    }).success(function(reply22, replier22) {
//                        if (reply22.status === 200) {
//                            var arra;
//                            for (var a = 0; a < reply22.result.data.length; a++) {
//                                if (reply22.result.data[a].tahapan_terakhir !== null) {
//                                    arra = [];
//                                    arra.push(reply22.result.data[a].paket_lelang_id);
//                                    arra.push(reply22.result.data[a].tahapan_terakhir);
//                                }
//                                //rekanan.paket.cekTahapanTersingkir
//                                $http.post($rootScope.url_api + 'rekanan/paket/cektahapantersingkir', {
//                                    param: arra
//                                }).success(function(reply2) {
//                                    if (reply2.status === 200) {
//                                        $scope.tahapanSekarang = reply2.result.data[0];
//                                        for (var k = 0; k < $scope.paket.length; k++) {
//                                            if ($scope.tahapanSekarang.paket_lelang_id === $scope.paket[k].paket_lelang_id) {
//                                                $scope.paket[k].tahapanTersingkir.push($scope.tahapanSekarang);
//                                            }
//                                        }
//                                    }
//                                }).error(function(err) {
//                                    $.growl.error({ message: "Gagal Akses API >"+err });
//                                    return;
//                                });
//                            }
//                        }
//                    }).error(function(err) {
//                        $.growl.error({ message: "Gagal Akses API >"+err });
//                        return;
//                    });
//                    
//                    //rekanan.paket.cekPemenang
//                    $http.post($rootScope.url_api + 'rekanan/paket/cekpemenang', {
//                        rekanan_id: $rootScope.rekanan_id,
//                        paket_lelang_id: reply.result.paket_lelang_id
//                    }).success(function(reply23, replier23) {
//                        if (reply23.status === 200) {
//                            for (var k = 0; k < reply23.result.data.length; k++) {
//                                for (var l = 0; l < $scope.paket.length; l++) {
//                                    if ($scope.paket[l].paket_lelang_id === reply23.result.data[k].paket_lelang_id) {
//                                        $scope.paket[l].pemenang = reply23.result.data[k].pemenang;
//                                    }
//                                }
//                            }
//                        }
//                    }).error(function(err) {
//                        $.growl.error({ message: "Gagal Akses API >"+err });
//                        return;
//                    });
//
//                    //rekanan.paket.cekBertahan
//                    $http.post($rootScope.url_api + 'rekanan/paket/cekbertahan', {
//                        rekanan_id: $rootScope.rekanan_id,
//                        param: arr2
//                    }).success(function(reply21, replier21) {
//                        //rekanan.paket.cekTahapanSekarang
//                        $http.post($rootScope.url_api + 'rekanan/paket/cektahapan', {
//                            param: arr2
//                        }).success(function(reply2) {
//                            if (reply2.status === 200) {
//                                for (var l = 0; l < $scope.paket.length; l++) {
//                                    $scope.tahapanSekarang = reply2.result.data;
//                                    for (var j = 0; j < $scope.tahapanSekarang.length; j++) {
//                                        if ($scope.paket[l].paket_lelang_id === $scope.tahapanSekarang[j].paket_lelang_id) {
//                                            if ($scope.tahapanSekarang[j].statusDay === 'Curr') {
//                                                $scope.paket[l].tahapanSekarang.push($scope.tahapanSekarang[j]);
//                                            }
//                                            else if ($scope.tahapanSekarang[j].statusDay === 'Old') {
//                                                $scope.paket[l].tahapanLama.push($scope.tahapanSekarang[j]);
//                                            }
//                                            else if ($scope.tahapanSekarang[j].statusDay === 'Next') {
//                                                $scope.paket[l].tahapanNext.push($scope.tahapanSekarang[j]);
//                                            }
//                                        }
//                                    }
//                                    $rootScope.unloadLoading();
//                                }
//                            }
//                        }).error(function(err) {
//                            $.growl.error({ message: "Gagal Akses API >"+err });
//                            return;
//                        });
//                    }).error(function(err) {
//                        $.growl.error({ message: "Gagal Akses API >"+err });
//                        return;
//                    });
//                }
//                else {
//                    $rootScope.unloadLoading();
//                }
//            }
//        }).error(function(err) {
//            $.growl.error({ message: "Gagal Akses API >"+err });
//            return;
//        });
//    }; // end jLoad_NoAuth
//
//    $scope.jLoadPekerjaan = function( current ){
//
//                $rootScope.authorize(jLoadPekerjaan_NoAuth(current)); // Auth-Step3: calling MAIN action                
//
//    };
//
//    function jLoadPekerjaan_NoAuth( current ){
//        $scope.pekerjaan = [];
//        $scope.currentPagePekerjaan = current;
//        
//        //rekanan.pekerjaan.select
//        $http.post($rootScope.url_api + 'rekanan/pekerjaan/select', {
//            rekanan_id: $rootScope.rekanan_id,
//            namaPekerjaanTercentang: $scope.namaPekerjaanTercentang,
//            waktuMulaiTercentang: $scope.waktuMulaiTercentang,
//            waktuSelesaiTercentang: $scope.waktuSelesaiTercentang,
//            srcTextPekerjaan: "%" + $scope.srcTextPekerjaan + "%",
//            waktuMulai1: $scope.waktuMulai1,
//            waktuMulai2: $scope.waktuMulai2,
//            waktuSelesai1: $scope.waktuSelesai1,
//            waktuSelesai2: $scope.waktuSelesai2,
//            offset: (current * 10) - 10,
//            limit: 10
//        }).success(function(reply) {
//            if (reply.status === 200) {
//                $scope.pekerjaan = reply.result.data;
//                for (var i = 0; i < $scope.pekerjaan.length; i++) {
//                    $scope.pekerjaan[i].tgl_mulai_kontrak_formatted = $rootScope.convertTanggal( $scope.pekerjaan[i].tgl_mulai_kontrak );
//                    $scope.pekerjaan[i].tgl_selesai_kontrak_formatted = $rootScope.convertTanggal( $scope.pekerjaan[i].tgl_selesai_kontrak );
//                }
//            }
//        }).error(function(err) {
//            $.growl.error({ message: "Gagal Akses API >"+err });
//            return;
//        });
//    }; // end jLoadPekerjaan_NoAuth
//
//    $scope.cariPaket = function(){
//        loadPaket();
//    };
//
//    $scope.centangStatus = function( obj ){
//        $scope.statusTercentang = obj;
//    };
//
//    $scope.centangNamaLelang = function( obj ){
//        $scope.namaLelangTercentang = obj;
//    };
//
//    $scope.centangNamaTahapan = function( obj ){
//        $scope.namaTahapanTercentang = obj;
//    };
//
//    $scope.cariPekerjaan = function(){
//        if( $scope.waktuMulaiTercentang === true && ($scope.waktuMulai1 === "" || $scope.waktuMulai2 === "" || $scope.waktuMulai1 === undefined || $scope.waktuMulai2 === undefined) ){
//            $.growl.error({ title: $rootScope.ttlWarning, message: 'Waktu mulai belum diisi!' });
//            return;
//        }
//        if( $scope.waktuSelesaiTercentang === true && ($scope.waktuSelesai1 === "" || $scope.waktuSelesai2 === "" || $scope.waktuSelesai1 === undefined || $scope.waktuSelesai2 === undefined) ){
//            $.growl.error({ title: $rootScope.ttlWarning, message: 'Waktu selesai belum diisi!' });
//            return;
//        }
//        
//        loadPekerjaan();
//    };
//
//    $scope.viewTahapan = function( idPaket ){
//                $state.transitionTo( 'rekanan-daftarTahapanLelang', {idPaket: idPaket} ); // Auth-Step3: calling MAIN action                
//       
//    };
//
//    $scope.centangNamaPekerjaan = function( obj ){
//        $scope.namaPekerjaanTercentang = obj;
//    };
//
//    $scope.centangWaktuMulai = function( obj ){
//        $scope.waktuMulaiTercentang = obj;
//    };
//
//    $scope.centangWaktuSelesai = function( obj ){
//        $scope.waktuSelesaiTercentang = obj;
//    };
//    
//    $scope.lihatCatatanPekerjaan = function(data) {
//        var modalInstance = $modal.open({
//            templateUrl: 'lihatCatatanPekerjaan.html',
//            controller: lihatCatatanPekerjaanCtrl,
//            resolve: {
//                item: function() {
//                    return data;
//                }
//            }
//        });
//        modalInstance.result.then(function() {
//            $rootScope.authorize(new function() {
//                $scope.loadMonitorKerja();
//            });
//        });
//    };
//}); // end pengingatAktivitasCtrl
//
//var lihatCatatanPekerjaanCtrl = function($scope, $modalInstance, $http, $cookieStore, item, $rootScope, $modal) {
//    var ttdKontrakId = item.ttd_kontrak_id;
//    $scope.namaPerusahaan = item.nama_perusahaan;
//    $scope.namaPaket = item.nama_paket;
//    $scope.keterangan1 = "";
//    $scope.keterangan2 = "";
//    $scope.sanksi1 = "";
//    $scope.sanksi2 = "";
//
//    $scope.catatan1 = [];
//    $scope.catatan2 = [];
//
//    $scope.init = function() {
//        $rootScope.authorize(new function() {
//            $scope.loadCatatan();
//        });
//    };
//
//    $scope.loadCatatan = function() {
//        $scope.catatan1 = [];
//        $scope.catatan2 = [];
//        $rootScope.loadLoadingModal("Silahkan Tunggu...");
//
//        $http.post($rootScope.url_api + "monitoring/selectCatatan", {
//            "ttd_kontrak_id": ttdKontrakId
//        }).success(function(reply) {
//            $rootScope.unloadLoadingModal();
//            if (reply.status === 200) {
//                var catatan = reply.result.data;
//                for (var i = 0; i < catatan.length; i++) {
//                    if (catatan[i].type === "1") {
//                        $scope.catatan1.push(catatan[i]);
//                    } else {
//                        $scope.catatan2.push(catatan[i]);
//                    }
//                }
//            } else {
//                $.growl.error({message: "gagal mendapatkan data catatan pekerjaan"});
//            }
//        }).error(function(err) {
//            $.growl.error({message: "Gagal Akses API > " + err});
//            return;
//        });
//    };
//    
//    $scope.batal = function() {
//        $modalInstance.dismiss('cancel');
//    };
//};