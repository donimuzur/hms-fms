(function () {
	'use strict';

	angular.module("app").controller("DataPengalamanController", ctrl);

	ctrl.$inject = ['$timeout','$uibModal', '$translatePartialLoader', 'UIControlService', 'VendorExperienceService'];
	/* @ngInject */
	function ctrl($timeout,$uibModal, $translatePartialLoader, UIControlService, VendorExperienceService) {
		var vm = this;
		vm.totalItems = 0;
		vm.currentPage = 1;
		vm.maxSize = 10;
		vm.keyword = '';
		vm.vendorID = [];
		vm.comodity = [];
		vm.isApprovedCR = false;

		vm.initialize = initialize;
		function initialize() {
			$translatePartialLoader.addPart('vhs-requisition');
			loadVendor();
			loadCheckCR();
		};

		vm.loadVendor = loadVendor;
		function loadVendor() {
			VendorExperienceService.selectVendor(function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.Vendor = reply.data;
					loadVendorCommodity(vm.Vendor.VendorID);
				} else {
					$.growl.error({ message: "Gagal mendapatkan data pengalaman perusahaan" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				console.info("error:" + JSON.stringify(err));
				//$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}

		vm.loadVendorCommodity = loadVendorCommodity;
		function loadVendorCommodity(data) {
			VendorExperienceService.SelectVendorCommodity({
				VendorID: data
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					console.info(reply.status);
					vm.comodity = reply.data;
					loadawal(vm.comodity.BusinessFieldID);
					console.info("comodity:" + JSON.stringify(vm.comodity));
				} else {
					$.growl.error({ message: "Gagal mendapatkan data pengalaman perusahaan" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				console.info("error:" + JSON.stringify(err));
				//$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}

		function loadCheckCR() {
			UIControlService.loadLoading("Silahkan Tunggu");
			console.info("CR:");
			VendorExperienceService.getCRbyVendor({ CRName: 'OC_VENDOREXPERIENCE' }, function (reply) {
				UIControlService.unloadLoading();
				console.info("CR:" + JSON.stringify(reply.status));
				if (reply.status === 200) {
					console.info("CR:" + JSON.stringify(reply.data));
					if (reply.data === true) {
						vm.isApprovedCR = true;
					} else {
						vm.isApprovedCR = false;
					}
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}

		vm.addExp = addExp;
		function addExp(data, isAdd, isCR) {
			console.info(isCR);
			var sendData = {
				item: data,
				isAdd: isAdd,
				isCR: isCR
			}
			//console.info(JSON.stringify(sendData));
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/rekanan/data-perusahaan/data-pengalaman/form-pengalaman.html',
				controller: 'formExpCtrl',
				controllerAs: 'formExpCtrl',
				resolve: {
					item: function () {
						return sendData;
					}
				}
			});

			modalInstance.result.then(function () {
				window.location.reload();
			});
		}

		vm.detailExp = detailExp;
		function detailExp(data) {
			var sendData = {
				item: data,
				isAdd: false
			}
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/rekanan/data-perusahaan/data-pengalaman/detail-pengalaman.html',
				controller: 'formExpCtrl',
				controllerAs: 'formExpCtrl',
				resolve: {
					item: function () {
						return sendData;
					}
				}
			});

			modalInstance.result.then(function () {
				loadawal();
			});
		}

		vm.loadAwal = loadawal;
		function loadawal(data) {
			UIControlService.loadLoading('LOADING.VENDOREXPERIENCE.MESSAGE');
			VendorExperienceService.select({
				Offset: (vm.currentPage - 1) * vm.maxSize,
				Limit: vm.maxSize,
				Keyword: vm.keyword,
				FilterType: data,
				column: 1
			}, function (reply) {
				console.info("lrjlbij" + vm.comodity.BusinessFieldID);
				if (reply.status === 200) {
					vm.listFinishExp = reply.data.List;
					console.info("listFinishExp:" + JSON.stringify(vm.listFinishExp));
					for (var i = 0; i < vm.listFinishExp.length; i++) {
						vm.listFinishExp[i].StartDate = UIControlService.getStrDate(vm.listFinishExp[i].StartDate);
						vm.listFinishExp[i].AddressInfo = vm.listFinishExp[i].Address + ", " + vm.listFinishExp[i].CityLocation.Name + ", " + vm.listFinishExp[i].CityLocation.State.Country.Name;
					}
					vm.totalItems = reply.data.Count;
					console.info("finish:" + JSON.stringify(vm.listFinishExp));
					UIControlService.unloadLoading();
				} else {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("error", 'NOTIFICATION.VENDOREXPERIENCE.ERROR', "NOTIFICATION.VENDOREXPERIENCE.TITLE");
				}
			}, function (err) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", 'NOTIFICATION.VENDOREXPERIENCE.ERROR', "NOTIFICATION.VENDOREXPERIENCE.TITLE");
			});

			UIControlService.loadLoading('LOADING.VENDOREXPERIENCE.MESSAGE');
			VendorExperienceService.select({
				Offset: (vm.currentPage - 1) * vm.maxSize,
				Limit: vm.maxSize,
				Keyword: vm.keyword,
				column: 2
			}, function (reply) {
				//console.info("current?:"+JSON.stringify(reply));
				if (reply.status === 200) {
					vm.listCurrentExp = reply.data.List;
					for (var i = 0; i < vm.listCurrentExp.length; i++) {
						vm.listCurrentExp[i].StartDate = UIControlService.getStrDate(vm.listCurrentExp[i].StartDate);
						vm.listCurrentExp[i].AddressInfo = vm.listCurrentExp[i].Address + ", " + vm.listCurrentExp[i].CityLocation.Name + ", " + vm.listCurrentExp[i].CityLocation.State.Country.Name;
					}
					console.info("current exp:" + JSON.stringify(vm.listCurrentExp));
					vm.totalItems = reply.data.Count;
					UIControlService.unloadLoading();
				} else {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("error", 'NOTIFICATION.VENDOREXPERIENCE.ERROR', "NOTIFICATION.VENDOREXPERIENCE.TITLE");
				}
			}, function (err) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", 'NOTIFICATION.VENDOREXPERIENCE.ERROR', "NOTIFICATION.VENDOREXPERIENCE.TITLE");
			});

		}

		vm.editActive = editActive;
		function editActive(data, active) {
			UIControlService.loadLoading("Silahkan Tunggu");
			VendorExperienceService.Delete({ ID: data.ID, IsActive: active }, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var msg = "";
					if (active === false) msg = " NonAktifkan ";
					if (active === true) msg = "Aktifkan ";
					UIControlService.msg_growl("success", "Data Berhasil di " + msg);
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
	}


})();// baru controller pertama
