(function () {
    'use strict';

    angular.module("app")
    .controller("PajakController", ctrl);
    
    ctrl.$inject = ['$http', '$uibModal','$translate', '$translatePartialLoader', '$location', 'SocketService', 'PajakService'];
    /* @ngInject */
    function ctrl($http, $uibModal, $translate, $translatePartialLoader, $location, SocketService, PajakService) {
        var vm = this;
    }
})();

// angular.module('eprocApp')        
//         .controller('pajakCtrl', function($scope, $http, $cookieStore, $state, $rootScope, $modal) {
//             $scope.bisaMengubahData;
            
//             $scope.initialize = function() {
//                 $rootScope.loadLoading("Silahkan Menunggu...");
// 				$rootScope.getSession().then(function (result) {
//                     $rootScope.userSession = result.data.data;
//                     $rootScope.userLogin = $rootScope.userSession.session_data.username;
//                     $rootScope.rekananid = $rootScope.userSession.session_data.rekanan_id;
					
//                     $rootScope.authorize(loadawal());
//                 });
//             };
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
//                     $http.post($rootScope.url_api + "logging", {
//                         message: "Tidak berhasil akses API : " + JSON.stringify(err),
//                         source: "pajak.js - rekanan/cekBisaMengubahData"
//                     }) 
//                     .then(function(response){
//                         // do nothing
//                         // don't have to feedback
//                     }); 
//                     $rootScope.unloadLoading();
//                     return;
//                 });
//                 $http.post($rootScope.url_api + "pajak/getPajak", {
//                     rekanan_id: $rootScope.rekananid
//                 }).success(function (reply) {
//                     if (reply.status === 200) {
//                         var data = reply.result.data;
//                         $scope.datapajak = data;
//                         $scope.todaydate = data.waktu;

//                         if ($scope.datapajak.length > 0) {
//                             $scope.tampilkanAlert = true;
//                             $scope.setujuPI = true;
//                             $scope.setujuPM = true;							
//                             $rootScope.insertStatusIsiData($rootScope.rekananid, 'dp', 1);
//                         } else {
//                             $scope.todaydateconverted = $rootScope.convertTanggal($scope.todaydate);
//                             $rootScope.insertStatusIsiData($rootScope.rekananid, 'dp', 0);
//                         }
//                         $rootScope.unloadLoading();
//                     } else {
//                         $.growl.error({message: "Gagal mendapatkan Data Pajak!!"});
//                         $rootScope.unloadLoading();
//                         return;
//                     }
//                     $rootScope.unloadLoading();
//                 }).error(function (err) {
//                     $.growl.error({message: "Gagal Akses API >" + err});
//                     $.growl.error({message: "Gagal Akses API >" + err});
//                     $http.post($rootScope.url_api + "logging", {
//                         message: "Tidak berhasil akses API : " + JSON.stringify(err),
//                         source: "pajak.js - pajak/getPajak"
//                     }) 
//                     .then(function(response){
//                         // do nothing
//                         // don't have to feedback
//                     }); 

//                     $rootScope.unloadLoading();
//                     return;
//                 });
//             }

//             $scope.tambah = function() {
//                 var tambah = $modal.open({
//                     templateUrl: 'tambahPajak.html',
//                     controller: 'tambahPajakCtrl'
//                 });
//                 tambah.result.then(function() {
//                     $scope.initialize();
//                 });
//             };

//             $scope.editPajak = function(obj) {
//                 var edit = $modal.open({
//                     templateUrl: 'editPajak.html',
//                     controller: 'editPajakCtrl',
//                     resolve: {
//                         datane: function() {
//                             return obj;
//                         }
//                     }
//                 });
//                 edit.result.then(function() {
//                     $scope.initialize();
//                 });
//             };

//             $scope.hapusPajak = function(id) {
//                 bootbox.confirm("Yakin menghapus data ?", function(res) {
//                     if (res) {
// 						$http.post($rootScope.url_api+"pajak/deletePajak", {
// 							jenis_pajak: $scope.datapajak.jenis_pajak,
// 							masa_pajak: $rootScope.convertTanggalRoot($scope.datapajak.masa_pajak),
// 							nomorbukti_pajak: $scope.datapajak.nomorbukti_pajak,
// 							tanggalbukti_pajak: $rootScope.convertTanggalRoot($scope.datapajak.tanggalbukti_pajak),
// 							//dokumenurl_pajak: reply.result.data.files[0].url,
// 							rekanan_id: $rootScope.rekanan_id,
// 							username: $rootScope.userLogin,
// 							pajak_id: id
// 						})
// 						.success(function (reply) {
// 							if (reply.status === 200) {
// 								$.growl.notice({title: "[INFO]", message: "Anda Telah Menghapus Data Pajak"});
// 								$rootScope.unloadLoading();
// 								$scope.initialize();
// 								//$modalInstance.close();
// 							} else {
// 								$.growl.error({title: "[PERINGATAN]", message: "Data Gagal Dihapus"});
// 								$rootScope.unloadLoading();
// 								return;
// 							}
// 						}).error(function (err) {
// 							$.growl.error({message: "Gagal Akses API >" + err});
//                             $http.post($rootScope.url_api + "logging", {
//                                 message: "Tidak berhasil akses API : " + JSON.stringify(err),
//                                 source: "pajak.js - pajak/deletePajak"
//                             }) 
//                             .then(function(response){
//                                 // do nothing
//                                 // don't have to feedback
//                             }); 

// 							$rootScope.unloadLoading();
// 							return;
// 						});						
//                     }
//                 });
//             };
//         });

//         var tambahPajakCtrl = function($scope, $modalInstance, $http, $cookieStore, $state, $rootScope) {
//             $scope.datapajak = {};
//             $scope.file;
//             var id_page_config = 5;
//             $scope.fileDok = function(elm) {
//                 $scope.file = elm.files;
//                 $scope.$apply();
//             };
            
//             $scope.inittambah = function (){
//                 $rootScope.fileuploadconfig(id_page_config);
// 				$rootScope.rekanan_id = $rootScope.userSession.session_data.rekanan_id;
// 				$rootScope.username = $rootScope.userSession.session_data.username;
// 				//$scope.datapajak.masa_pajak = "24-05-2016";
// 				//$scope.datapajak.tanggalbukti_pajak = "24-05-2018";
//             };
            
//             $scope.simpan = function() {
//                 if ($scope.datapajak.jenis_pajak === '' || $scope.datapajak.jenis_pajak === undefined) {
//                     $.growl.error({title: "[WARNING]", message: "Jenis pajak belum diisi"});
//                     return;
//                 }
//                 if ($scope.datapajak.masa_pajak === '' || $scope.datapajak.masa_pajak === undefined) {
//                     $.growl.error({title: "[WARNING]", message: "Masa pajak belum diisi"});
//                     return;
//                 }
//                 if ($scope.datapajak.nomorbukti_pajak === '' || $scope.datapajak.nomorbukti_pajak === undefined) {
//                     $.growl.error({title: "[WARNING]", message: "Nomor bukti belum diisi"});
//                     return;
//                 }
//                 if ($scope.datapajak.tanggalbukti_pajak === '' || $scope.datapajak.tanggalbukti_pajak === undefined) {
//                     $.growl.error({title: "[WARNING]", message: "Tanggal bukti belum diisi"});
//                     return;
//                 }
//                 if ($scope.file === undefined) {
//                     $.growl.error({title: "[WARNING]", message: "File dokumen belum dipilih"});
//                     return;
//                 } else {
//                     var fileInput = $('.upload-file');
//                     var extFile = $('.upload-file').val().split('.').pop().toLowerCase();
//                     var maxSize = fileInput.data('max-size');
//                     if (fileInput.get(0).files.length) {
//                         var fileSize = fileInput.get(0).files[0].size/1024;
//                         if (fileSize > maxSize) {
//                             $.growl.error({title: "[WARNING]", message: "Ukuran file terlalu besar"});
//                             return;
//                         } else {
//                             var restrictedExt = $rootScope.limitfiletype;
//                             if ($.inArray(extFile, restrictedExt) === -1) {
//                                 $.growl.error({title: "[WARNING]", message: "Format file tidak valid"});
//                                 return;
//                             } else {
//                                 uploadDanInsert($scope.file);
//                             }
//                         }
//                     }
//                 }
//             };
            
//             function uploadDanInsert(filetoUpload) {
//   $rootScope.loadLoadingModal("Upload Data...");
//                 var fd = new FormData();
//                 angular.forEach(filetoUpload, function(file) {
//                     fd.append("uploads", file);
//                 });
//                 $http.post($rootScope.url_api + "/upload/" + $rootScope.rekanan_id + "/", fd,
//                         {
//                             withCredentials: true,
//                             transformRequest: angular.identity(),
//                             headers: {'Content-Type': undefined}
//                         })
//                         .success(function(reply) {
// 							$http.post($rootScope.url_api+"pajak/insertPajak", {
// 								jenis_pajak: $scope.datapajak.jenis_pajak,
// 								masa_pajak: $rootScope.convertTanggalRoot($scope.datapajak.masa_pajak),
// 								nomorbukti_pajak: $scope.datapajak.nomorbukti_pajak,
// 								tanggalbukti_pajak: $rootScope.convertTanggalRoot($scope.datapajak.tanggalbukti_pajak),
// 								dokumenurl_pajak: reply.result.data.files[0].url,
// 								rekanan_id: $rootScope.rekanan_id,
// 								username: $rootScope.username
// 							})
// 							.success(function (reply) {
// 								if (reply.status === 200) {
// 									$.growl.notice({title: "[INFO]", message: "Anda Telah Menambah Data Pajak"});
// 									$rootScope.unloadLoadingModal();
// 									$modalInstance.close();
// 								} else {
// 									$.growl.error({title: "[PERINGATAN]", message: "Data Gagal Disimpan"});
// 									$rootScope.unloadLoadingModal();
// 									return;
// 								}
// 							}).error(function (err) {
// 								$.growl.error({message: "Gagal Akses API >" + err});
//                                 $http.post($rootScope.url_api + "logging", {
//                                     message: "Tidak berhasil akses API : " + JSON.stringify(err),
//                                     source: "pajak.js - pajak/insertPajak"
//                                 }) 
//                                 .then(function(response){
//                                     // do nothing
//                                     // don't have to feedback
//                                 }); 


// 								$rootScope.unloadLoadingModal();
// 								return;
// 							});
//                         })
//                         .error(function(urlerror) {
//                             $.growl.error({title: "[PERINGATAN]", message: "Dokumen gagal diunggah"});
//                             $http.post($rootScope.url_api + "logging", {
//                                 message: "Tidak berhasil akses API : " + JSON.stringify(err),
//                                 source: "pajak.js - upload"
//                             }) 
//                             .then(function(response){
//                                 // do nothing
//                                 // don't have to feedback
//                             }); 

//                              $rootScope.unloadLoadingModal();
//                         }
//                         );
//             }

//             $scope.batal = function() {
//                 $modalInstance.dismiss('cancel');
//             };

//         };
//        var editPajakCtrl = function($scope, $modalInstance, $http, $cookieStore, datane, $state, $rootScope) {
//             $scope.datapajak = datane;
//             $scope.file;
//             var id_page_config = 5;
//             $scope.fileDok = function(elm) {
//                 $scope.file = elm.files;
//                 $scope.$apply();
//             };
            
//             $scope.initedit = function (){
//                 $rootScope.fileuploadconfig(id_page_config);
// 				$rootScope.rekanan_id = $rootScope.userSession.session_data.rekanan_id;
// 				$rootScope.username = $rootScope.userSession.session_data.username;
//                 $scope.datapajak.masa_pajak = $rootScope.convertTanggal($scope.datapajak.masa_pajak);
//                 $scope.datapajak.tanggalbukti_pajak = $rootScope.convertTanggal($scope.datapajak.tanggalbukti_pajak);
								
//             };
            
//             $scope.simpan = function() {
//                 if ($scope.datapajak.jenis_pajak === '' || $scope.datapajak.jenis_pajak === undefined) {
//                     $.growl.error({title: "[WARNING]", message: "Jenis pajak belum diisi"});
//                     return;
//                 }
//                 if ($scope.datapajak.masa_pajak === '' || $scope.datapajak.masa_pajak === undefined) {
//                     $.growl.error({title: "[WARNING]", message: "Masa pajak belum diisi"});
//                     return;
//                 }
//                 if ($scope.datapajak.nomorbukti_pajak === '' || $scope.datapajak.nomorbukti_pajak === undefined) {
//                     $.growl.error({title: "[WARNING]", message: "Nomor bukti belum diisi"});
//                     return;
//                 }
//                 if ($scope.datapajak.tanggalbukti_pajak === '' || $scope.datapajak.tanggalbukti_pajak === undefined) {
//                     $.growl.error({title: "[WARNING]", message: "Tanggal bukti belum diisi"});
//                     return;
//                 }
//                 if ($scope.file === undefined) {
//                     update($scope.datapajak.dokumenurl_pajak);
//                 }
//                 else {
//                     var fileInput = $('.upload-file');
//                     var extFile = $('.upload-file').val().split('.').pop().toLowerCase();
//                     var maxSize = fileInput.data('max-size');
//                     if (fileInput.get(0).files.length) {
//                         var fileSize = fileInput.get(0).files[0].size;
//                         if (fileSize > maxSize) {
//                             $.growl.error({title: "[WARNING]", message: "Ukuran file terlalu besar"});
//                             return;
//                         } else {
//                             var restrictedExt = $rootScope.limitfiletype;
//                             if ($.inArray(extFile, restrictedExt) === -1) {
//                                 $.growl.error({title: "[WARNING]", message: "Format file tidak valid"});
//                                 return;
//                             } else {
//                                 uploadDanUpdate($scope.file);
//                             }
//                         }
//                     }
//                 }
//             };

//             function update(url) {
// 				$http.post($rootScope.url_api+"pajak/updatePajak", {
// 					jenis_pajak: $scope.datapajak.jenis_pajak,
// 					masa_pajak: $rootScope.convertTanggalRoot($scope.datapajak.masa_pajak),
// 					nomorbukti_pajak: $scope.datapajak.nomorbukti_pajak,
// 					tanggalbukti_pajak: $rootScope.convertTanggalRoot($scope.datapajak.tanggalbukti_pajak),
// 					dokumenurl_pajak: url,
// 					rekanan_id: $rootScope.rekanan_id,
// 					username: $rootScope.username,
// 					pajak_id: $scope.datapajak.pajak_id
// 				})
// 				.success(function (reply) {
// 					if (reply.status === 200) {
// 						$.growl.notice({title: "[INFO]", message: "Anda Telah Merubah Data Pajak"});
// 						$rootScope.unloadLoadingModal();
// 						$modalInstance.close();
// 					} else {
// 						$.growl.error({title: "[PERINGATAN]", message: "Data Gagal Dirubah"});
// 						$rootScope.unloadLoadingModal();
// 						return;
// 					}
// 				}).error(function (err) {
// 					$.growl.error({message: "Gagal Akses API >" + err});
//                     $http.post($rootScope.url_api + "logging", {
//                         message: "Tidak berhasil akses API : " + JSON.stringify(err),
//                         source: "pajak.js - pajak/updatePajak"
//                     }) 
//                     .then(function(response){
//                         // do nothing
//                         // don't have to feedback
//                     }); 

// 					$rootScope.unloadLoadingModal();
// 					return;
// 				});

//             }

//             function uploadDanUpdate(filetoUpload) {
//                  $rootScope.loadLoadingModal("Upload Data...");
//                 var fd = new FormData();
//                 angular.forEach(filetoUpload, function(file) {
//                     fd.append("uploads", file);
//                 });
// 				$http.post($rootScope.url_api + "/upload/" + $rootScope.rekanan_id + "/", fd,
//                 //$http.post("/upload/" + $rootScope.rekanan_id, fd,
//                         {
//                             withCredentials: true,
//                             transformRequest: angular.identity(),
//                             headers: {'Content-Type': undefined}
//                         })
//                         .success(function(urldata) {
// 							$http.post($rootScope.url_api+"pajak/updatePajak", {
// 								jenis_pajak: $scope.datapajak.jenis_pajak,
// 								masa_pajak: $rootScope.convertTanggalRoot($scope.datapajak.masa_pajak),
// 								nomorbukti_pajak: $scope.datapajak.nomorbukti_pajak,
// 								tanggalbukti_pajak: $rootScope.convertTanggalRoot($scope.datapajak.tanggalbukti_pajak),
// 								dokumenurl_pajak: urldata.result.data.files[0].url,
// 								rekanan_id: $rootScope.rekanan_id,
// 								username: $rootScope.username,
// 								pajak_id: $scope.datapajak.pajak_id
// 							})
// 							.success(function (reply) {
// 								if (reply.status === 200) {
// 									$.growl.notice({title: "[INFO]", message: "Anda Telah Merubah Data Pajak"});
// 									$rootScope.unloadLoadingModal();
// 									$modalInstance.close();
// 								} else {
// 									$.growl.error({title: "[PERINGATAN]", message: "Data Gagal Dirubah"});
// 									$rootScope.unloadLoadingModal();
// 									return;
// 								}
// 							}).error(function (err) {
// 								$.growl.error({message: "Gagal Akses API >" + err});
//                                 $http.post($rootScope.url_api + "logging", {
//                                     message: "Tidak berhasil akses API : " + JSON.stringify(err),
//                                     source: "pajak.js - pajak/updatePajak"
//                                 }) 
//                                 .then(function(response){
//                                     // do nothing
//                                     // don't have to feedback
//                                 }); 

// 								$rootScope.unloadLoadingModal();
// 								return;
// 							});

//                         })
//                         .error(function(urlerror) {
//                             $.growl.error({title: "[PERINGATAN]", message: "Dokumen gagal diunggah"});
//                             $http.post($rootScope.url_api + "logging", {
//                                 message: "Tidak berhasil akses API : " + JSON.stringify(err),
//                                 source: "pajak.js - upload edit"
//                             }) 
//                             .then(function(response){
//                                 // do nothing
//                                 // don't have to feedback
//                             }); 

//                             $rootScope.unloadLoadingModal();
//                         }
//                         );
//             }

//             $scope.batal = function() {
//                  $scope.datapajak.masa_pajak = $rootScope.convertTanggalRoot($scope.datapajak.masa_pajak);
//                 $scope.datapajak.tanggalbukti_pajak = $rootScope.convertTanggalRoot($scope.datapajak.tanggalbukti_pajak);
//                 $modalInstance.dismiss('cancel');
//             };
//         };