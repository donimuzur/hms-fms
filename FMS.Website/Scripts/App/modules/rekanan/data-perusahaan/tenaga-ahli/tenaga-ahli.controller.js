(function () {
	'use strict';

	angular.module("app").controller("TenagaAhliController", ctrl);

	ctrl.$inject = ['$timeout','$http', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'TenagaAhliService', 'UIControlService', 'GlobalConstantService'];
	/* @ngInject */
	function ctrl($timeout,$http, $uibModal, $translate, $translatePartialLoader, $location, SocketService, TenagaAhliService, UIControlService, GlobalConstantService) {
		var vm = this;
		vm.fullSize = 10;
		var offset = 0;
		vm.flag = 0;
		vm.folderFile = GlobalConstantService.getConstant('api') + "/";
		vm.vendorexpertsCertificate = [];
		vm.isApprovedCR = false;

		vm.initialize = initialize;
		function initialize() {
			$translatePartialLoader.addPart('tenaga-ahli');
			loadVendor();
			jLoad(1);
			checkCR();
		}
		vm.loadVendor = loadVendor;
		function loadVendor() {
			TenagaAhliService.selectVendor(function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
				    vm.Vendor = reply.data;
				    console.info("vendor>>" + JSON.stringify(vm.Vendor));
				    if (vm.Vendor.VerifiedSendDate === null && vm.Vendor.VerifiedDate === null) {
				        vm.IsApprovedCR = true;
				    }
				} else {
					$.growl.error({ message: "Gagal mendapatkan data vendor" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				console.info("error:" + JSON.stringify(err));
				//$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}

		vm.jLoad = jLoad;
		function jLoad(current) {
			//console.info("curr "+current)
			vm.vendorexperts = [];
			vm.currentPage = current;
			var offset = (current * 10) - 10;
			TenagaAhliService.all({
				Offset: offset,
				Limit: vm.fullSize,
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
				    var data = reply.data.List;
				    vm.vendorexperts = [];
				    console.info("vendorExprt:" + JSON.stringify(vm.vendorexperts));
				    for (var i = 0; i < data.length; i++) {
				        if (data[i].IsActive === true) {
				            vm.vendorexperts.push(data[i]);
				        }
				    }
					//loadCertificate(vm.vendorexperts);
				} else {
					$.growl.error({ message: "Gagal mendapatkan data Tenaga Ahli Perusahaan" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				console.info("error:" + JSON.stringify(err));
				//$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}

		function checkCR() {
			UIControlService.loadLoading("Silahkan Tunggu");
			TenagaAhliService.getCRbyVendor({ CRName: 'OC_VENDOREXPERTS' }, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					//console.info("CR:" + JSON.stringify(reply.data.length));
					if (reply.data === true) {
						vm.isApprovedCR = true;
					} else {
						vm.isApprovedCR = false;
					}
					console.info(JSON.stringify(vm.isApprovedCR));
				}

			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}


		vm.loadCertificate = loadCertificate;
		function loadCertificate(data) {
		    vm.listJob = [];
		    vm.listEducation = [];
		    vm.listCertificate = [];
			//console.info("curr "+current)
			TenagaAhliService.selectCertificate({
				Offset: offset,
				Limit: vm.fullSize,
				Status: data.ID
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
				    vm.vendorexpertsCertificate = reply.data;
				    for (var i = 0; vm.vendorexpertsCertificate.length; i++) {
				        vm.vendorexpertsCertificate[i].StartDate = new Date(Date.parse(vm.vendorexpertsCertificate[i].StartDate));
				        vm.vendorexpertsCertificate[i].EndDate = new Date(Date.parse(vm.vendorexpertsCertificate[i].EndDate));
				        if (vm.vendorexpertsCertificate[i].SysReference.Name == "JOB_EXPERIENCE" && vm.vendorexpertsCertificate[i].IsActive == true)
				            vm.listJob.push(vm.vendorexpertsCertificate[i]);
				        else if (vm.vendorexpertsCertificate[i].SysReference.Name == "DIPLOMA" && vm.vendorexpertsCertificate[i].IsActive == true)
				            vm.listEducation.push(vm.vendorexpertsCertificate[i]);
				        else if (vm.vendorexpertsCertificate[i].SysReference.Name == "CERTIFICATE" && vm.vendorexpertsCertificate[i].IsActive == true)
				            vm.listCertificate.push(vm.vendorexpertsCertificate[i]);

				    }
				} else {
					$.growl.error({ message: "Gagal mendapatkan data Tenaga Ahli Perusahaan" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				console.info("error:" + JSON.stringify(err));
				//$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}

		vm.UbahTenagaAhli = UbahTenagaAhli;
		function UbahTenagaAhli(data) {
			vm.flag = 0;
			console.info("masuk form add/edit");
			var data = {
				act: false,
				item: data
			}
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/rekanan/data-perusahaan/tenaga-ahli/FormTenagaAhli.html',
				controller: 'formTenagaAhliCtrl',
				controllerAs: 'formTenagaAhliCtrl',
				resolve: {
					item: function () {
						return data;
					}
				}
			});
			modalInstance.result.then(function () {
				initialize();
				viewdetail(vm.dataExpert);
			});
		}

		vm.lihatDetail = lihatDetail;
		function lihatDetail(data) {
			vm.flag = 0;
			console.info("masuk form add/edit");
			var data = {
				item: data
			}
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/rekanan/data-perusahaan/tenaga-ahli/DetailTenagaAhli.html',
				controller: 'DetailTenagaAhliCtrl',
				controllerAs: 'DetailTenagaAhliCtrl',
				resolve: {
					item: function () {
						return data;
					}
				}
			});

			modalInstance.result.then(function () {
				initialize();
				viewdetail(vm.dataExpert);
			});
		}

		vm.HapusData = HapusData;
		function HapusData(obj, act) {
			vm.flag = 0;
			TenagaAhliService.editActive({
				ID: obj.ID,
				IsActive: act
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					if (act === false)
						UIControlService.msg_growl("success", "Data Berhasil di Non Aktifkan");
					if (act === true)
						UIControlService.msg_growl("success", "Data Berhasil di Aktifkan");
					$timeout(function () {
					    window.location.reload();
					}, 1000); 
				} else {
					UIControlService.msg_growl("error", "Gagal menonaktifkan data ");
					return;
				}
			}, function (err) {
				UIControlService.msg_growl("error", "Gagal Akses API ");
				UIControlService.unloadLoading();
			});
		}

		vm.TambahTenagaAhli = TambahTenagaAhli;
		function TambahTenagaAhli(data) {
			console.info("masuk form add/edit");
			var data = {
				act: true,
				item: data
			}
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/rekanan/data-perusahaan/tenaga-ahli/FormTenagaAhli.html',
				controller: 'formTenagaAhliCtrl',
				controllerAs: 'formTenagaAhliCtrl',
				resolve: {
					item: function () {
						return data;
					}
				}
			});
			modalInstance.result.then(function () {

				initialize();
				//viewdetail(vm.dataExpert);
				window.location.reload();
			});
		}

		vm.hapusdetail = hapusdetail;
		function hapusdetail(data, act) {
			TenagaAhliService.editCertificateActive({
				ID: data.ID,
				IsActive: act
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					if (act === false)
						UIControlService.msg_growl("success", "Data Berhasil di Hapus");

					if (act === true)
					    UIControlService.msg_growl("success", "Data Berhasil di Aktifkan");
					loadCertificate(vm.dataExpert);
				} else {
					UIControlService.msg_growl("error", "Gagal menonaktifkan data ");
					return;
				}
			}, function (err) {
				UIControlService.msg_growl("error", "Gagal Akses API ");
				UIControlService.unloadLoading();
			});
		}

		vm.DetailCertificate = DetailCertificate;
		function DetailCertificate(data1, act, type) {
			console.info("masuk form add/edit");
			if (data1 == undefined) {
				vm.cc = {
					VendorExpertsID: vm.dataExpert.ID
				}
			} else {
				vm.cc = data1
			}

			var data = {
				type: type,
				act: act,
				item: vm.cc
			}

			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/rekanan/data-perusahaan/tenaga-ahli/FormDetailTenagaAhli.html',
				controller: 'formDetailTenagaAhliCtrl',
				controllerAs: 'formDetailTenagaAhliCtrl',
				resolve: {
					item: function () {
						return data;
					}
				}
			});
			modalInstance.result.then(function () {
				initialize();
				viewdetail(vm.dataExpert);
			});
		}

		vm.viewdetail = viewdetail;
		function viewdetail(data) {
			vm.namavendor = data.Name;
			vm.flag = 1;
			loadCertificate(data);
			vm.dataExpert = data;
		}
	}
})();
