(function () {
    'use strict';

    angular.module("app")
    .controller("KepemilikanSahamController", ctrl);
    
    ctrl.$inject = ['$http', '$uibModal','$translate', '$translatePartialLoader', '$location', 'SocketService', 'KepemilikanSahamService'];
    /* @ngInject */
    function ctrl($http, $uibModal, $translate, $translatePartialLoader, $location, SocketService, KepemilikanSahamService) {
        var vm = this;
    }
})();


// angular.module('eprocApp')
//         .controller('kepemilikanSahamCtrl', function($scope, $http, $modal, $cookieStore, $state, $rootScope) {
//             $scope.bisaMengubahData;
			 
//             $scope.initialize = function() {
// 			$rootScope.loadLoading("Silahkan Menunggu...");
// 			$rootScope.getSession().then(function (result) {
// 				$rootScope.userSession = result.data.data;
// 				$rootScope.userLogin = $rootScope.userSession.session_data.username;
// 				$rootScope.rekananid = $rootScope.userSession.session_data.rekanan_id;
// 				$rootScope.nama_perusahaan = $rootScope.userSession.session_data.nama_perusahaan;
// 				$rootScope.authorize(loadawal());
// 			});
// 			function loadawal() {
//                 $http.post($rootScope.url_api + "rekanan/cekBisaMengubahData", {
//                     rekananId: [$rootScope.rekananid]
//                 }).success(function (reply) {
//                     if (reply.status === 200) {
//                         var data = reply.result.data;
//                         $scope.bisaMengubahData = data[0].bisa_mengubah_data=="1";
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
//                 $http.post($rootScope.url_api + "kepemsaham/getSaham", {
//                     rekanan_id: $rootScope.rekananid
//                 }).success(function (reply) {
//                     if (reply.status === 200) {
//                         var data = reply.result.data;
// 						$scope.dataall = data;
// 						if($scope.dataall.length > 0){
// 							$rootScope.insertStatusIsiData($rootScope.rekananid, 'ks', 1);
// 						}
// 						else{
// 							$rootScope.insertStatusIsiData($rootScope.rekananid, 'ks', 0);
// 						}
//                         $rootScope.unloadLoading();
//                     } else {
//                         $.growl.error({message: "Gagal mendapatkan Data Kepemilikan Saham!!"});
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
// 			}
// 			$scope.CU = function() {
//                 var mdCU = $modal.open({
//                     templateUrl: 'CUSahamCtrl.html',
//                     controller: 'CUSahamCtrl',
//                     resolve: {
//                         isEdit: function() {
//                             return 0;
//                         },
//                         datane: function() {
//                             return "NULL";
//                         }
//                     }
//                 });
//                 mdCU.result.then(function() {
//                     $scope.initialize();
//                 });
//             };
//             $scope.CUEdit = function(obj) {
//                 var mdCU = $modal.open({
//                     templateUrl: 'CUSahamCtrl.html',
//                     controller: 'CUSahamCtrl',
//                     resolve: {
//                         isEdit: function() {
//                             return 1;
//                         },
//                         datane: function() {
//                             return obj;
//                         }
//                     }
//                 });
//                 mdCU.result.then(function() {

//                 });
//             };
//             $scope.Hapus = function(id) {
//                 bootbox.confirm("Yakin menghapus data ?", function(res) {
//                     if (res) {
// 						$http.post($rootScope.url_api+"kepemsaham/deleteSaham", {
// 							rekanan_id: $rootScope.rekananid,
// 							username: $rootScope.userLogin,
// 							id_kpm_saham: id
// 						})
// 						.success(function (reply) {
// 							if (reply.status === 200) {
// 								$.growl.notice({title: "[INFO]", message: "Anda Telah Menghapus Data Kepemilikan Saham"});
// 								$rootScope.unloadLoadingModal();
// 								$scope.initialize();
// 								//$modalInstance.close();
// 							} else {
// 								$.growl.error({title: "[PERINGATAN]", message: "Data Gagal Dihapus"});
// 								$rootScope.unloadLoadingModal();
// 								return;
// 							}
// 						}).error(function (err) {
// 							$.growl.error({message: "Gagal Akses API >" + err});
// 							$rootScope.unloadLoadingModal();
// 							return;
// 						});
//                     }
//                 });
//             };
//             $scope.vw = function(obj) {
//                 var vW = $modal.open({
//                     templateUrl: 'VwSahamCtrl.html',
//                     controller: 'VwSahamCtrl',
//                     resolve: {
//                         datane: function() {
//                             return obj;
//                         }
//                     }
//                 });
//                 vW.result.then(function() {

//                 });
//             };

// 		});
		
// 		var CUSahamCtrl = function($scope, $modalInstance, isEdit, datane, $http, $cookieStore, $state, $rootScope) {
//             $scope.isEdit = isEdit;
//             $scope.saham = {};
//             $scope.initialize = function() {
//                 if ($scope.isEdit === 1) {
//                     $scope.saham = datane;
//                 }
// 				$rootScope.username = $rootScope.userSession.session_data.username;
//             };
//             $scope.simpan = function() {
//                 if ($scope.saham.nama_kpm_saham === undefined || $scope.saham.nama_kpm_saham === null || $scope.saham.nama_kpm_saham === '') {
//                     $.growl.error({title: "[WARNING]", message: "Nama pemilik saham belum diisi"});
//                     return;
//                 }
//                 if ($scope.saham.noktp_kpm_saham === undefined || $scope.saham.noktp_kpm_saham === null || $scope.saham.noktp_kpm_saham === '') {
//                     $.growl.error({title: "[WARNING]", message: "Nomor KTP belum diisi"});
//                     return;
//                 }
//                 if ($scope.saham.alamat_kpm_saham === undefined || $scope.saham.alamat_kpm_saham === null || $scope.saham.alamat_kpm_saham === '') {
//                     $.growl.error({title: "[WARNING]", message: "Alamat belum diisi"});
//                     return;
//                 }
//                 if ($scope.saham.jml_kpm_saham === undefined || $scope.saham.jml_kpm_saham === null || $scope.saham.jml_kpm_saham === '') {
//                     $.growl.error({title: "[WARNING]", message: "Jumlah saham belum diisi"});
//                     return;
//                 }
//                 if ($scope.saham.satuan_kpm_saham === undefined || $scope.saham.satuan_kpm_saham === null || $scope.saham.satuan_kpm_saham === '') {
//                     $.growl.error({title: "[WARNING]", message: "Satuan belum diisi"});
//                     return;
//                 }
//                 if ($scope.isEdit === 0) {
//                     insert();
//                 }
//                 else if ($scope.isEdit === 1)
//                     update();
//             };
//             $scope.batal = function() {
//                 $modalInstance.dismiss('cancel');
//             };
//             function insert() {

//                 $rootScope.loadLoadingModal("Menyimpan Data...");
				
// 				$http.post($rootScope.url_api+"kepemsaham/insertSaham", {
// 					nama_kpm_saham: $scope.saham.nama_kpm_saham,
// 					noktp_kpm_saham: $scope.saham.noktp_kpm_saham,
// 					alamat_kpm_saham: $scope.saham.alamat_kpm_saham,
// 					jml_kpm_saham: $scope.saham.jml_kpm_saham,
// 					satuan_kpm_saham: $scope.saham.satuan_kpm_saham,
// 					rekanan_id: $rootScope.rekananid,
// 					username: $rootScope.username
// 				})
// 				.success(function (reply) {
// 					if (reply.status === 200) {
// 						$.growl.notice({title: "[INFO]", message: "Anda Telah Menambah Data Kepemilikan Saham"});
// 						$rootScope.unloadLoadingModal();
// 						$modalInstance.close();
// 					} else {
// 						$.growl.error({title: "[PERINGATAN]", message: "Data Gagal Disimpan"});
// 						$rootScope.unloadLoadingModal();
// 						return;
// 					}
// 				}).error(function (err) {
// 					$.growl.error({message: "Gagal Akses API >" + err});
// 					$rootScope.unloadLoadingModal();
// 					return;
// 				});
//             }
// 			function update() {
// 				$http.post($rootScope.url_api+"kepemsaham/updateSaham", {
// 					nama_kpm_saham: $scope.saham.nama_kpm_saham,
// 					noktp_kpm_saham: $scope.saham.noktp_kpm_saham,
// 					alamat_kpm_saham: $scope.saham.alamat_kpm_saham,
// 					jml_kpm_saham: $scope.saham.jml_kpm_saham,
// 					satuan_kpm_saham: $scope.saham.satuan_kpm_saham,
// 					rekanan_id: $rootScope.rekananid,
// 					username: $rootScope.username,
// 					id_kpm_saham: $scope.saham.id_kpm_saham
// 				})
// 				.success(function (reply) {
// 					if (reply.status === 200) {
// 						$.growl.notice({title: "[INFO]", message: "Anda Telah Merubah Data Saham"});
// 						$rootScope.unloadLoadingModal();
// 						$modalInstance.close();
// 					} else {
// 						$.growl.error({title: "[PERINGATAN]", message: "Data Gagal Dirubah"});
// 						$rootScope.unloadLoadingModal();
// 						return;
// 					}
// 				}).error(function (err) {
// 					$.growl.error({message: "Gagal Akses API >" + err});
// 					$rootScope.unloadLoadingModal();
// 					return;
// 				});
//             }
// 		};
		
// 		var VwSahamCtrl = function($scope, $modalInstance, datane, $http) {
//             $scope.data = datane;
//             $scope.batal = function() {
//                 $modalInstance.dismiss('cancel');
//             };
//         };
// 		