(function () {
    'use strict';

    angular.module("app")
    .controller("SuratPernyataanController", ctrl);
    
    ctrl.$inject = ['$http', '$uibModal','$translate', '$translatePartialLoader', '$location', 'SocketService', 'SuratPernyataanService'];
    /* @ngInject */
    function ctrl($http, $uibModal, $translate, $translatePartialLoader, $location, SocketService, SuratPernyataanService) {
        var vm = this;
        
        vm.bisaMengubahData;
        vm.dokumen_sp = [];
        vm.tampilkanAlert = false;
        vm.setujuPI = false;
        vm.setujuPM = false;
        vm.tanggal_setuju_converted;
        vm.todaydate;
        vm.todaydateconverted;
        
        vm.init = init;
        function init() {
                 $rootScope.getSession().then(function (result) {
                     $rootScope.userSession = result.data.data;
                     $rootScope.userLogin = $rootScope.userSession.session_data.username;
                     $rootScope.rekananid = $rootScope.userSession.session_data.rekanan_id;
                     $rootScope.nama_perusahaan = $rootScope.userSession.session_data.nama_perusahaan;
                     $rootScope.authorize(loadawal());
                 });
             };

            vm.loadawal = loadawal;
             function loadawal() {
                 $http.post($rootScope.url_api + "rekanan/cekBisaMengubahData", {
                     rekananId: [$rootScope.rekananid]
                 }).success(function (reply) {
                     if (reply.status === 200) {
                         vm.data = reply.result.data;
                         vm.bisaMengubahData = data[0].bisa_mengubah_data === "1";
                         $rootScope.unloadLoading();
                     } else {
                         $.growl.error({message: "Gagal mendapatkan Hak Bisa Mengubah Data!!"});
                         $rootScope.unloadLoading();
                         return;
                     }
                     $rootScope.unloadLoading();
                 }).error(function (err) {
                     $.growl.error({message: "Gagal Akses API >" + err});
                     $rootScope.unloadLoading();
                     return;
                 });
                 $http.post($rootScope.url_api + "SPC/getSuratPernyataan", {
                     rekanan_id: $rootScope.rekananid
                 }).success(function (reply) {
                     if (reply.status === 200) {
                         vm.data = reply.result.data;
                         vm.dokumen_sp = data.data[0];
                         vm.todaydate = data.waktu;

                         if (vm.dokumen_sp.pernyataanminat_agreement !== null) {
                             vm.tampilkanAlert = true;
                             vm.setujuPI = true;
                             vm.setujuPM = true;
                             vm.tanggal_setuju_converted = $rootScope.convertTanggal(vm.dokumen_sp.tanggal_setuju);
                             $rootScope.insertStatusIsiData($rootScope.rekananid, 'sp', 1);
                         } else {
                             vm.todaydateconverted = $rootScope.convertTanggal(vm.todaydate);
                             $rootScope.insertStatusIsiData($rootScope.rekananid, 'sp', 0);
                         }
                         $rootScope.unloadLoading();
                     } else {
                         $.growl.error({message: "Gagal mendapatkan Data Surat Pernyataan!!"});
                         $rootScope.unloadLoading();
                         return;
                     }
                     $rootScope.unloadLoading();
                 }).error(function (err) {
                     $.growl.error({message: "Gagal Akses API >" + err});
                     $rootScope.unloadLoading();
                     return;
                 });
             }

             vm.filePernyataan = filePernyataan;

             function filePernyataan(elm) {
                 vm.filePernyataan = elm.files;
             };
             
             vm.ubahPI = ubahPI;
             function ubahPI(obj) {
                 vm.ubahPI = obj;
             };

             vm.simpanpersetujuan =simpanpersetujuan;
             function simpanpersetujuan() {
                 vm.param = [];

                 if (vm.setujuPM === true) {
                     param.push(1);
                 } else {
                     param.push(null);
                 }

                 if (vm.setujuPI === true) {
                     param.push(1);
                 } else {
                     param.push(null);
                 }
                 $rootScope.loadLoading("Silahkan Tunggu...");
                 $http.post($rootScope.url_api + "SPC/suratpernyataan", {
                     rekanan_id: $rootScope.rekananid,
                     param: param,
                     username: $rootScope.userLogin
                 }).success(function (reply) {
                     if (reply.status === 200) {
                         $.growl.notice({title: "[INFO]", message: "Anda Telah Menyetujui Surat Pernyataan"});
                         $rootScope.unloadLoading();
                         $rootScope.insertStatusIsiData($rootScope.rekananid, 'sp', 1);
                         vm.init();
                     } else {
                         $.growl.error({title: "[PERINGATAN]", message: "Persetujuan Gagal Disimpan"});
                         $rootScope.insertStatusIsiData($rootScope.rekananid, 'sp', 0);
                         $rootScope.unloadLoading();
                         return;
                     }
                 }).error(function (err) {
                     $.growl.error({message: "Gagal Akses API >" + err});
                     $rootScope.unloadLoading();
                     return;
                 });
             };

             vm.hilangkanAlert = hilangkanAlert;
             function hilangkanAlert() {
                 vm.tampilkanAlert = false;
             };
        
    }
})();

// angular.module('eprocApp')
//         .controller('suratPernyataanCtrl', function ($scope, $http, $cookieStore, $state, $rootScope) {
//             $scope.bisaMengubahData;
//             $scope.dokumen_sp = [];
//             $scope.tampilkanAlert = false;
//             $scope.setujuPI = false;
//             $scope.setujuPM = false;
//             $scope.tanggal_setuju_converted;
//             $scope.todaydate;
//             $scope.todaydateconverted;

//             $scope.init = function () {
//                 $rootScope.getSession().then(function (result) {
//                     $rootScope.userSession = result.data.data;
//                     $rootScope.userLogin = $rootScope.userSession.session_data.username;
//                     $rootScope.rekananid = $rootScope.userSession.session_data.rekanan_id;
//                     $rootScope.nama_perusahaan = $rootScope.userSession.session_data.nama_perusahaan;
//                     $rootScope.authorize(loadawal());
//                 });
//             };

//             function loadawal() {
//                 $http.post($rootScope.url_api + "rekanan/cekBisaMengubahData", {
//                     rekananId: [$rootScope.rekananid]
//                 }).success(function (reply) {
//                     if (reply.status === 200) {
//                         var data = reply.result.data;
//                         $scope.bisaMengubahData = data[0].bisa_mengubah_data === "1";
//                         $rootScope.unloadLoading();
//                     } else {
//                         $.growl.error({message: "Gagal mendapatkan Hak Bisa Mengubah Data!!"});
//                         $rootScope.unloadLoading();
//                         return;
//                     }
//                     $rootScope.unloadLoading();
//                 }).error(function (err) {
//                     $.growl.error({message: "Gagal Akses API >" + err});
//                     $rootScope.unloadLoading();
//                     return;
//                 });
//                 $http.post($rootScope.url_api + "SPC/getSuratPernyataan", {
//                     rekanan_id: $rootScope.rekananid
//                 }).success(function (reply) {
//                     if (reply.status === 200) {
//                         var data = reply.result.data;
//                         $scope.dokumen_sp = data.data[0];
//                         $scope.todaydate = data.waktu;

//                         if ($scope.dokumen_sp.pernyataanminat_agreement !== null) {
//                             $scope.tampilkanAlert = true;
//                             $scope.setujuPI = true;
//                             $scope.setujuPM = true;
//                             $scope.tanggal_setuju_converted = $rootScope.convertTanggal($scope.dokumen_sp.tanggal_setuju);
//                             $rootScope.insertStatusIsiData($rootScope.rekananid, 'sp', 1);
//                         } else {
//                             $scope.todaydateconverted = $rootScope.convertTanggal($scope.todaydate);
//                             $rootScope.insertStatusIsiData($rootScope.rekananid, 'sp', 0);
//                         }
//                         $rootScope.unloadLoading();
//                     } else {
//                         $.growl.error({message: "Gagal mendapatkan Data Surat Pernyataan!!"});
//                         $rootScope.unloadLoading();
//                         return;
//                     }
//                     $rootScope.unloadLoading();
//                 }).error(function (err) {
//                     $.growl.error({message: "Gagal Akses API >" + err});
//                     $rootScope.unloadLoading();
//                     return;
//                 });
//             }

//             $scope.filesurat;

//             $scope.filePernyataan = function (elm) {
//                 $scope.filesurat = elm.files;
//             };

//             $scope.ubahPI = function (obj) {
//                 $scope.setujuPI = obj;
//             };

//             $scope.simpanpersetujuan = function () {
//                 var param = [];

//                 if ($scope.setujuPM === true) {
//                     param.push(1);
//                 } else {
//                     param.push(null);
//                 }

//                 if ($scope.setujuPI === true) {
//                     param.push(1);
//                 } else {
//                     param.push(null);
//                 }
//                 $rootScope.loadLoading("Silahkan Tunggu...");
//                 $http.post($rootScope.url_api + "SPC/suratpernyataan", {
//                     rekanan_id: $rootScope.rekananid,
//                     param: param,
//                     username: $rootScope.userLogin
//                 }).success(function (reply) {
//                     if (reply.status === 200) {
//                         $.growl.notice({title: "[INFO]", message: "Anda Telah Menyetujui Surat Pernyataan"});
//                         $rootScope.unloadLoading();
//                         $rootScope.insertStatusIsiData($rootScope.rekananid, 'sp', 1);
//                         $scope.init();
//                     } else {
//                         $.growl.error({title: "[PERINGATAN]", message: "Persetujuan Gagal Disimpan"});
//                         $rootScope.insertStatusIsiData($rootScope.rekananid, 'sp', 0);
//                         $rootScope.unloadLoading();
//                         return;
//                     }
//                 }).error(function (err) {
//                     $.growl.error({message: "Gagal Akses API >" + err});
//                     $rootScope.unloadLoading();
//                     return;
//                 });
//             };

//             $scope.hilangkanAlert = function () {
//                 $scope.tampilkanAlert = false;
//             };
//         });
