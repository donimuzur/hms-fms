
(function () {
    'use strict';

    angular.module("app")
    .controller("DataUserController", ctrl);
    
    ctrl.$inject = ['$http', '$uibModal','$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataUserService'];
    /* @ngInject */
    function ctrl($http, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataUserService) {
        var vm = this;
    }
})();


// angular.module('eprocApp')
//         .controller('dataUserCtrl', function($scope, $http, $modal, $cookieStore, $state, $rootScope) {
//             $scope.infouser = {};
//             $scope.bisaMengubahData;

//             $scope.initialize = function() {
//                 $rootScope.getSession().then(function(result) {
//                     $rootScope.userSession = result.data.data;
//                     $rootScope.userLogin = $rootScope.userSession.session_data.username;
//                     $rootScope.loadLoading("Silahkan Tunggu...");
//                     $http.post($rootScope.url_api + 'rekanan/cekBisaMengubahData', {
//                         rekananId: [$rootScope.userSession.session_data.rekanan_id]
//                     }).success(function(reply) {
//                         if (reply.status === 200) {
//                             $scope.bisaMengubahData = reply.result.data[0].bisa_mengubah_data == "1";
//                         }

//                     });
//                     $http.post($rootScope.url_api + 'rekanan/datauser/getInfoUser', {
//                         rekananId: $rootScope.userSession.session_data.rekanan_id
//                     }).success(function(reply) {
//                         $rootScope.unloadLoading();
//                         $scope.infouser = reply.result.data[0];
//                     });
//                 });
//             };
            
//             /* BEGIN OPEN DIALOG UBAH PASSWORD */
//             $scope.gantipassword = function() {
//                 var dGPU = $modal.open({
//                     templateUrl: 'loginRekananDataUserPassword.html',
//                     controller: 'loginRekananDataUserPassword'
//                 });
//                 dGPU.result.then(function() {
//                     $scope.initialize();
//                 });
//             };
//             /* END OPEN DIALOG UBAH PASSWORD */

//             /* BEGIN OPEN DIALOG UBAH DATA USER */
//             $scope.ubahdata = function() {
//                 var dUDU = $modal.open({
//                     templateUrl: 'loginRekananDataUserUbah.html',
//                     controller: 'loginRekananDataUserUbah',
//                     resolve: {
//                         datauser: function() {
//                             return $scope.infouser;
//                         }
//                     }
//                 });
//                 dUDU.result.then(function() {
//                     $scope.initialize();
//                 });
//             };
//             /* END OPEN DIALOG UBAH DATA USER */
//         })

//         .controller('loginRekananDataUserUbah', function($scope, $modalInstance, datauser, $cookieStore, $http, $rootScope) {
//             $scope.infouser = datauser;
//             $scope.batal = function() {
//                 $modalInstance.dismiss('cancel');
//             };
//             $scope.ubahuser = function() {
//                 $rootScope.loadLoadingModal("Menyimpan Data...");
//                 $http.post($rootScope.url_api + 'rekanan/datauser/updateDataUser', {
//                     username: $rootScope.userLogin,
//                     rekananId: $rootScope.userSession.session_data.rekanan_id,
//                     namaPJ: $scope.infouser.nama_pj,
//                     jabatanPJ: $scope.infouser.jabatan_pj,
//                     emailPJ: $scope.infouser.email_pribadi
//                 }).success(function(reply) {
//                     if (reply.status === 200) {
//                         $.growl.notice({title: "[INFO]", message: "Data User berhasil diubah"});
//                     }
//                     else {
//                         $.growl.error({title: "[PERINGATAN]", message: "Data User gagal diubah"});
//                     }
// //                    $('#divubahdatauser').unblock();
//                     $rootScope.unloadLoadingModal();
//                     $modalInstance.close();
//                 });
//             };
//         })
//         .controller('loginRekananDataUserPassword', function($scope, $modalInstance, $cookieStore, $http, $rootScope) {
//             $scope.oldpswd = "";
//             $scope.newpswd = "";
//             $scope.rnewpswd = "";
//             $scope.setoldpswd = function(pswd) {
//                 $scope.oldpswd = pswd;
//             };
//             $scope.setnewpswd = function(pswd) {
//                 $scope.newpswd = pswd;
//             };
//             $scope.setrnewpswd = function(pswd) {
//                 $scope.rnewpswd = pswd;
//             };
//             $scope.ubahpswd = function() {
//                 if ($scope.newpswd === '' && $scope.rnewpswd === '') {
//                     $.growl.warning({title: "[PERINGATAN]", message: "PASSWORD masih kosong"});
//                     return;
//                 }
//                 if ($scope.newpswd !== $scope.rnewpswd) {
//                     $.growl.warning({title: "[PERINGATAN]", message: "PASSWORD tidak sama"});
//                     return;
//                 }
//                 else {
//                     $http.post($rootScope.url_api + 'rekanan/datauser/cekOldPassword', {
//                         rekananId: $rootScope.userSession.session_data.rekanan_id
//                     }).success(function(reply0) {
//                         //console.info(JSON.stringify(reply0));
//                         if (reply0.status === 200) {
//                             $scope.$oldpswddata = reply0.result.data[0].pswd;
//                             //console.info($scope.$oldpswddata);
//                             if ($scope.$oldpswddata == $.md5($scope.oldpswd)) {
//                                 $rootScope.loadLoadingModal("Menyimpan Data...");
//                                 $http.post($rootScope.url_api + 'rekanan/datauser/ubahPassword', {
//                                     username: $rootScope.userLogin,
//                                     rekananId: $rootScope.userSession.session_data.rekanan_id,
//                                     oldpassword: $.md5($scope.oldpswd),
//                                     newpassword: $.md5($scope.newpswd)
//                                 }).success(function(reply) {
//                                     if (reply.status !== 200) {
//                                         $.growl.error({title: "[PERINGATAN]", message: "error"});
//                                         //console.info(JSON.stringify(reply));
//                                     }
//                                     else {
//                                         $.growl.notice({title: "[INFO]", message: "berhasil"});
//                                     }
// //                                      $('#divubahpassword').unblock();
//                                     $rootScope.unloadLoadingModal();
//                                     $modalInstance.close();
//                                 });
//                             } else {
//                                 $.growl.error({title: "[PERINGATAN]", message: "Password Lama Tidak Sesuai"});
//                             }
//                         }
//                     });
//                 }
//             };
//             $scope.batal = function() {
//                 $modalInstance.dismiss('cancel');
//             };
//         });