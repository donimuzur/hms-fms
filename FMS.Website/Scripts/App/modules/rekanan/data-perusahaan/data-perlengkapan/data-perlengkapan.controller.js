(function () {
	'use strict';
	angular.module("app")
    .controller("DataPerlengkapanController", ctrl);

	ctrl.$inject = ['$http', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'DataPerlengkapanService', 'UIControlService'];
	/* @ngInject */
	function ctrl($http, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataPerlengkapanService,
        UIControlService) {
		var vm = this;
		vm.maxSize = 10;
		vm.msgLoading = "MSG.LOADING";
		vm.currentPage = 1;
		vm.isApprovedCR = false;

		vm.init = init;
		function init() {
			$translatePartialLoader.addPart("data-perlengkapan");
			loadVendor();
			loadBuilding(1);
			loadEquipmentVehicle(1);
			loadEquipmentTools(1);
			loadCheckCR();
		}

		function loadCheckCR() {
			UIControlService.loadLoading("Silahkan Tunggu");
			DataPerlengkapanService.getCRbyVendor({ CRName: 'OC_VENDOREQUIPMENT' }, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					//console.info("CR:" + JSON.stringify(reply.data));
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

		vm.loadVendor = loadVendor;
		function loadVendor() {
			DataPerlengkapanService.selectVendor(function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.Vendor = reply.data;
				} else {
					$.growl.error({ message: "Gagal mendapatkan data Neraca Perusahaan" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				console.info("error:" + JSON.stringify(err));
				//$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}
		vm.listBuilding = [];
		function loadBuilding(current) {
			var offset = (current * vm.maxSize) - vm.maxSize;
			UIControlService.loadLoading(vm.msgLoading);
			DataPerlengkapanService.selectBuilding({ Ofsset: offset, Limit: vm.maxSize },
            function (reply) {
            	// console.info(JSON.stringify(reply));
            	UIControlService.unloadLoading();
            	vm.listBuilding = reply.data.List;
            }, function (err) {
            	UIControlService.msg_growl("error", "MESSAGE.API");
            	UIControlService.unloadLoading();
            });
		}

		vm.listVehicle = [];
		function loadEquipmentVehicle(current) {
			var offset = (current * vm.maxSize) - vm.maxSize;
			UIControlService.loadLoading(vm.msgLoading);
			DataPerlengkapanService.selectVehicle({ Ofsset: offset, Limit: vm.maxSize },
            function (reply) {
            	// console.info("vehi>>"+JSON.stringify(reply));
            	UIControlService.unloadLoading();
            	vm.listVehicle = reply.data.List;
            	for (var i = 0; i < vm.listVehicle.length; i++) {
            		vm.listVehicle[i].MfgDate = UIControlService.getStrDate(vm.listVehicle[i].MfgDate);
            	}
            }, function (err) {
            	UIControlService.msg_growl("error", "MESSAGE.API");
            	UIControlService.unloadLoading();
            });
		}

		vm.listEquipmentTools = [];
		function loadEquipmentTools(current) {
			//console.info("mlebu");
			var offset = (current * vm.maxSize) - vm.maxSize;
			UIControlService.loadLoading(vm.msgLoading);
			DataPerlengkapanService.selectEquipment({ Ofsset: offset, Limit: vm.maxSize },
            function (reply) {
            	// console.info("Equip>>>"+JSON.stringify(reply));
            	UIControlService.unloadLoading();
            	vm.listEquipmentTools = reply.data.List;
            	for (var i = 0; i < vm.listEquipmentTools.length; i++) {
            		vm.listEquipmentTools[i].MfgDate = UIControlService.getStrDate(vm.listEquipmentTools[i].MfgDate);
            	}
            }, function (err) {
            	UIControlService.msg_growl("error", "MESSAGE.API");
            	UIControlService.unloadLoading();
            });
		}

		vm.openForm = openForm;
		function openForm(type, data, isAdd, IsCR) {
			// console.info("cr:" + IsCR);
			var data = {
				type: type,
				data: data,
				isForm: isAdd,
				//IsCR: IsCR
			}
			var temp;
			var ctrl;
			var ctrlAs;
			if (type === "building") {
				temp = "app/modules/rekanan/data-perusahaan/data-perlengkapan/formBuilding.html";
				ctrl = "FormBuildingController";
				ctrlAs = "FormBuildingCtrl";
			} else {
				temp = "app/modules/rekanan/data-perusahaan/data-perlengkapan/formNonBuilding.html";
				ctrl = "FormNonBuildingController";
				ctrlAs = "FormNonBuildingCtrl";
			}
			var modalInstance = $uibModal.open({
				templateUrl: temp,
				controller: ctrl,
				controllerAs: ctrlAs,
				resolve: {
					item: function () {
						return data;
					}
				}
			});
			modalInstance.result.then(function () {
				window.location.reload();
			});
		}

		vm.detailForm = detailForm;
		function detailForm(type, data, isAdd) {
			var data = {
				type: type,
				data: data,
				isForm: isAdd
			}
			var ctrl;
			var ctrlAs;
			if (type === "building") {
				ctrl = "FormBuildingController";
				ctrlAs = "FormBuildingCtrl";
			} else {
				ctrl = "FormNonBuildingController";
				ctrlAs = "FormNonBuildingCtrl";
			}

			var modalInstance = $uibModal.open({
				templateUrl: "app/modules/rekanan/data-perusahaan/data-perlengkapan/detailData.html",
				controller: ctrl,
				controllerAs: ctrlAs,
				resolve: {
					item: function () {
						return data;
					}
				}
			});
			modalInstance.result.then(function () {
				window.location.reload();
			});
		}

		vm.editActiveBuilding = editActiveBuilding;
		function editActiveBuilding(data, active) {
			var act = null; var isTemp = null;
			if (vm.isApprovedCR == true) {
				act = 2; isTemp = true;
			}
			UIControlService.loadLoading("Silahkan Tunggu");
			DataPerlengkapanService.editActiveBulding({
				ID: data.ID,
				IsActive: active,
				//Action: act,
				//IsTemporary: isTemp
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var msg = "";
					if (active === false) msg = " NonAktifkan ";
					if (active === true) msg = "Aktifkan ";
					UIControlService.msg_growl("success", "Data Berhasil di " + msg);
					vm.init();
				}
				else {
					UIControlService.msg_growl("error", "Gagal menonaktifkan data ");
					return;
				}
			}, function (err) {
				UIControlService.msg_growl("error", "Gagal Akses API ");
				UIControlService.unloadLoading();
			});

		}

		vm.editActiveNonBuilding = editActiveNonBuilding;
		function editActiveNonBuilding(data, active) {
			var act = null; var isTemp = null;
			//if (vm.isApprovedCR == true) {
			//	act = 2;
			//	isTemp = true;
			//}
			UIControlService.loadLoading("Silahkan Tunggu");
			DataPerlengkapanService.editActiveNonBulding({
				ID: data.ID,
				IsActive: active,
				//Action: act,
				//IsTemporary: isTemp
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var msg = "";
					if (active === false) msg = " NonAktifkan ";
					if (active === true) msg = "Aktifkan ";
					UIControlService.msg_growl("success", "Data Berhasil di " + msg);
					vm.init();
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