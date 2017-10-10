(function () {
	'use strict';

	angular.module("app").controller("UploadDokumenCtrl", ctrl);

	ctrl.$inject = ['$http', '$uibModal', '$translate', '$translatePartialLoader', '$location', '$filter', 'VerifiedSendService', 'SocketService', 'UploadDokumenLainlainService', 'UIControlService', 'GlobalConstantService'];
	/* @ngInject */
	function ctrl($http, $uibModal, $translate, $translatePartialLoader, $location, $filter, VerifiedSendService, SocketService, UploadDokumenLainlainService, UIControlService, GlobalConstantService) {
		var vm = this;

		vm.totalItems = 0;
		vm.currentPage = 0;
		vm.maxSize = 10;
		vm.page_id = 35;
		vm.menuhome = 0;
		vm.userId = 0;
		vm.jLoad = jLoad;
		vm.document = [];
		vm.Kata = "";
		vm.VendorID;
		vm.IsApprovedCR = false;

		vm.init = init;
		function init() {
			loadVerifiedVendor();
		}

		vm.loadVerifiedVendor = loadVerifiedVendor;
		function loadVerifiedVendor() {
			VerifiedSendService.selectVerifikasi(function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.verified = reply.data;
					//if (vm.verified.VerifiedSendDate === null && vm.verified.VerifiedDate === null) {
					//    vm.IsApprovedCR = true;
					//}
					vm.cekTemporary = vm.verified.IsTemporary;
					vm.VendorID = vm.verified.VendorID;
					jLoad(1);
					//console.info(JSON.stringify(vm.verified.VendorID));
				} else {
					$.growl.error({ message: "Gagal mendapatkan data Perusahaan" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				//console.info("error:" + JSON.stringify(err));
				//$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}

		vm.jLoad = jLoad;
		function jLoad(current) {
			//console.info("curr "+current)
			UIControlService.loadLoading("Silahkan Tunggu...");
			//vm.currentPage = current;
			//var offset = (current * 10) - 10;
			UploadDokumenLainlainService.SelectVend({
				VendorID: vm.VendorID
			}, function (reply) {
				//console.info("data:" + JSON.stringify(reply));
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.data;
					vm.document = data;
					vm.document.forEach(function (cr) {
						cr.ValidDateConverted = convertDate(cr.ValidDate);
					});
				} else {
					$.growl.error({ message: "Gagal mendapatkan dokumen" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				//console.info("error:" + JSON.stringify(err));
				//$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}

		function convertDate(date) {
			return UIControlService.convertDate(date);
		}

		vm.tambah = tambah;
		function tambah(data) {
			//console.info("console upload dokumen");
			var data = {
				act: 1,
				item: data
			}
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/rekanan/data-perusahaan/upload-dokumen-lainlain/upload-dokumen-lainlain.modal.html',
				controller: "UploadDokModalCtrl",
				controllerAs: "uploadModalCtrl",
				resolve: {
					item: function () {
						return data;
					}
				}
			});
			modalInstance.result.then(function () {
				jLoad(1);
			});
		}

		vm.edit = edit;
		function edit(data) {
			//console.info("console edit dokumen");
			var data = {
				act: 0,
				item: {
					ID: data.ID,
					DocumentName: data.DocumentName,
					DocumentNo: data.DocumentNo,
					ValidDate: new Date(Date.parse(data.ValidDate)),
					DocumentUrl: data.DocumentUrl,
					VendorID: vm.vendorID
				}
			}
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/rekanan/data-perusahaan/upload-dokumen-lainlain/upload-dokumen-lainlain.modal.html',
				controller: "UploadDokModalCtrl",
				controllerAs: "uploadModalCtrl",
				resolve: {
					item: function () {
						return data;
					}
				}
			});
			modalInstance.result.then(function () {
				jLoad(1);
			});
		}

		vm.view = view;
		function view(doc) {
			var data = {
				item: {
					DocumentName: doc.DocumentName,
					DocumentNo: doc.DocumentNo,
					ValidDate: doc.ValidDate,
					DocumentUrl: doc.DocumentUrl,
					VendorID: vm.vendorID
				}
			};
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/rekanan/data-perusahaan/upload-dokumen-lainlain/upload-dokumen-lainlain.viewModal.html',
				controller: 'viewUploadDocCtrl',
				controllerAs: 'viewUploadCtrl',
				resolve: {
					item: function () {
						return data;
					}
				}
			});
		};

		vm.remove = remove;
		function remove(doc) {
			bootbox.confirm('<h3 class="afta-font center-block">' + "Yakin ingin menghapus?" + '<h3>', function (reply) {
				if (reply) {
					//UIControlService.loadLoading(loadmsg);
					UploadDokumenLainlainService.remove({
						ID: doc.ID
					}, function (reply2) {
						UIControlService.unloadLoading();
						if (reply2.status === 200) {
							UIControlService.msg_growl('notice', 'Berhasil dihapus');
							jLoad(1);
						} else
							UIControlService.msg_growl('error', 'Error');
					}, function (error) {
						UIControlService.unloadLoading();
						UIControlService.msg_growl('error', 'Error');
					});
				}
			});
		};
	}
})();
// angular.module('eprocApp')
//         .directive('maskdate', function() {
//             return {
//                 restrict: 'AE',
//                 link: function(scope, elt) {
//                     $(elt).inputmask("yyyy-mm-dd", {
//                         placeholder: "yyyy-mm-dd"
//                     });
//                 }
//             };
//         })
//         .controller('dataLainLainCtrl', function($scope, $modal, $http, $cookieStore, $state, $rootScope) {
//             $scope.bisaMengubahData;
//             $scope.document = [];

//             $scope.cekBisaMengubahData = function() {
//                 $http.post($rootScope.url_api + "rekanan/cekBisaMengubahData", {
//                     rekananId: [$rootScope.rekananid]
//                 }).success(function (reply) {
//                     if (reply.status === 200) {
//                         var data = reply.result.data;
//                         $scope.bisaMengubahData = data[0].bisa_mengubah_data=="1";
//                         //console.log("Bisa mengubah data: " + $scope.bisaMengubahData);
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
//                         source: "upload-dokumen-lainlain.js - rekanan/cekBisaMengubahData"
//                     }) 
//                     .then(function(response){
//                         // do nothing
//                         // don't have to feedback
//                     }); 
//                     $rootScope.unloadLoading();
//                     return;
//                 });
//             };

//             $scope.hasCompanyProfile = false;
//             $scope.getData = function() {
//                 $rootScope.loadLoading("Loading document data  . . . ");
//                 $http.get($rootScope.url_api + "rekanan/dokumenlain/list/" + $rootScope.rekananid)
//                 .success(function (reply) {
//                     if (reply.status === 200) {
//                         var data = reply.result.data;
//                          $scope.document = data;

//                          $http.get($rootScope.url_api + "rekanan/dokumenlain/companyprofile/" + $rootScope.rekananid)
//                          .success(function(response){
//                             if(response.status === 200) {
//                                 $scope.hasCompanyProfile = Number(response.result.size) > 0;
//                                 if($scope.document.length > 0 && $scope.hasCompanyProfile){
//                                      $rootScope.insertStatusIsiData($rootScope.rekananid, 'dll', 1);
//                                  }
//                                  else{
//                                      $rootScope.insertStatusIsiData($rootScope.rekananid, 'dll', 0);
//                                  }
//                             } else {
//                                 $.growl.error({message: "Gagal mendapatkan data profil perusahaan!!"});        
//                             }

//                          })
//                          .error(function(err){

//                          });

//                         $rootScope.unloadLoading();
//                     } else {
//                         $.growl.error({message: "Gagal mendapatkan Data Dokumen Lain-lain!!"});
//                         $rootScope.unloadLoading();
//                         return;
//                     }
//                     $rootScope.unloadLoading();
//                 }).error(function (err) {
//                     $.growl.error({message: "Gagal Akses API >" + err});
//                     $http.post($rootScope.url_api + "logging", {
//                         message: "Tidak berhasil akses API : " + JSON.stringify(err),
//                         source: "upload-dokumen-lainlain.js - rekanan/dokumenlain/list"
//                     }) 
//                     .then(function(response){
//                         // do nothing
//                         // don't have to feedback
//                     }); 

//                     $rootScope.unloadLoading();
//                     return;
//                 });
//             };
//             function load() {
//                 $scope.cekBisaMengubahData();
//                 $scope.getData();
//             };
//             $scope.initial = function() {
//                 $rootScope.loadLoading("Waiting authorization. . .");
//                 $rootScope.getSession().then(function (result) {
//                     $rootScope.userSession = result.data.data;
//                     $rootScope.userLogin = $rootScope.userSession.session_data.username;
//                     $rootScope.rekananid = $rootScope.userSession.session_data.rekanan_id;

//                     $rootScope.authorize(load());

//                 });
//             };

//             $scope.add = function() {
//                 var mdl = $modal.open({
//                     templateUrl: 'mdlUploadDocCtrl.html',
//                     controller: 'mdlUploadDocCtrl'
//                 });
//                 mdl.result.then(function() {
//                     $scope.initial();
//                 });
//             };

//             $scope.delete = function(id, url) {
//                 bootbox.confirm("Yakin menghapus data ?", function(res) {
//                     if (res) {
//                         $http.post($rootScope.url_api+"rekanan/dokumenlain/delete/" + id, {
//                             username: $rootScope.userLogin
//                         })
//                         .success(function (reply) {
//                             if (reply.status === 200) {

//                                 $http.post($rootScope.url_api + "deleteFile", {
//                                     url: url
//                                 }).then(function(res){
//                                     $.growl.notice({title: "[INFO]", message: "Anda Telah Menghapus Data Dokumen Lain-lain"});
//                                     $rootScope.unloadLoading();
//                                     $scope.initial();
//                                 });

//                             } else {
//                                 $.growl.error({title: "[PERINGATAN]", message: "Data Gagal Dihapus"});
//                                 $rootScope.unloadLoading();
//                                 return;
//                             }
//                         }).error(function (err) {
//                             $.growl.error({message: "Gagal Akses API >" + err});
//                             $http.post($rootScope.url_api + "logging", {
//                                 message: "Tidak berhasil akses API : " + JSON.stringify(err),
//                                 source: "upload-dokumen-lainlain.js - rekanan/dokumenlain/delete/:id"
//                             }) 
//                             .then(function(response){
//                                 // do nothing
//                                 // don't have to feedback
//                             }); 

//                             $rootScope.unloadLoading();
//                             return;
//                         }); 

//                     }
//                 });
//             };
//         })
//         .controller('mdlUploadDocCtrl', function($scope, $http, $modalInstance, $cookieStore, $state, $stateParams, $rootScope) {
//             var id_page_config = 9;
//             $scope.data = {};
//             $scope.data.is_companyprofile = false;
//             $scope.filey;
//             $scope.fileyChange = function(elm) {
//                 $scope.filey = elm.files;
//                 $scope.$apply();
//             };
//             $scope.batal = function() {
//                 $modalInstance.dismiss('cancel');
//                 $state.transitionTo($state.current, $stateParams, {
//                     reload: true,
//                     inherit: false,
//                     notify: true
//                 });
//             };
//             $scope.initialize = function() {
//                 $rootScope.fileuploadconfig(id_page_config);
//             };
//             $scope.simpan = function() {
//                 if ($scope.data.nama_dokumen === undefined || $scope.data.nama_dokumen === '') {
//                     $.growl.error({title: "[WARNING]", message: "Nama dokumen belum diisi"});
//                     return;
//                 }
//                 if ($scope.data.nomor_dokumen === undefined) {
//                     $scope.data.nomor_dokumen = "";
//                 }
//                 if ($scope.data.masa_berlaku === undefined) {
//                     $scope.data.masa_berlaku = "";
//                 }
//                 if ($scope.filey === undefined) {
//                     $.growl.error({title: "[WARNING]", message: "File belum dipilih"});
//                     return;
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
//                                 insertWithFile();
//                             }
//                         }
//                     }
//                 }
//             };
//             function insertWithFile() {
//                 $rootScope.loadLoadingModal("Menyimpan Data...");
//                 var fd = new FormData();
//                 angular.forEach($scope.filey, function(file) {
//                     fd.append("uploads", file);
//                 });
//                 $http.post($rootScope.url_api + "upload/" + $rootScope.rekananid + "/", fd,
//                         {
//                             withCredentials: true,
//                             transformRequest: angular.identity(),
//                             headers: {'Content-Type': undefined}
//                         })
//                         .success(function(urldok) {

//                             $http.post($rootScope.url_api + "rekanan/dokumenlain/insert", {
//                                 nama_dokumen: $scope.data.nama_dokumen,
//                                 nomor_dokumen: $scope.data.nomor_dokumen,
//                                 masa_berlaku: $scope.data.masa_berlaku ? $rootScope.convertTanggalRoot($scope.data.masa_berlaku) : '',
//                                 dokumen_file: urldok.result.data.files[0].url,
//                                 rekanan_id: $rootScope.rekananid,
//                                 is_companyprofile: $scope.data.is_companyprofile,
//                                 username: $rootScope.userLogin
//                             })
//                             .success(function(response){
//                                 if(response.status === 200) {
//                                     $.growl.notice({title: "[INFO]", message: "Anda Telah Menambah Data Dokumen Lain-lain"});
//                                     $rootScope.unloadLoadingModal();
//                                     $modalInstance.close();
//                                 } else {
//                                     $.growl.error({title: "[PERINGATAN]", message: "Data Gagal Disimpan"});
//                                     $rootScope.unloadLoadingModal();
//                                     return;
//                                 }
//                             })
//                             .error(function(err){
//                                 $.growl.error({message: "Gagal Akses API >" + err});
//                                 $http.post($rootScope.url_api + "logging", {
//                                     message: "Tidak berhasil akses API : " + JSON.stringify(err),
//                                     source: "upload-dokumen-lainlain.js - rekanan/dokumenlain/insert"
//                                 }) 
//                                 .then(function(response){
//                                     // do nothing
//                                     // don't have to feedback
//                                 }); 

//                             });
//                          })
//                         .error(function(error) {
//                             $.growl.error({title: "[PERINGATAN]", message: "Upload file scan gagal"});
//                             $rootScope.unloadLoadingModal();
//                         });
//             }
//         });