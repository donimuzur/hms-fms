(function () {
	'use strict';

	angular.module("app").controller("ChangedDataDetailCtrl", ctrl);

	ctrl.$inject = ['item', '$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'PermintaanUbahDataService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService', '$uibModalInstance'];
	function ctrl(item, $http, $translate, $translatePartialLoader, $location, SocketService, PUbahDataService,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService, $uibModalInstance) {

		var vm = this;
		vm.init = init;
		vm.item = item;
		vm.param = item.param;
		vm.VendorID = item.vendorID;
		vm.action = vm.item.item.Action;
		vm.folderFile = GlobalConstantService.getConstant('api') + "/";
		vm.totalData = item.totalData;

		function init() {
			// console.info("totalData:" + JSON.stringify(vm.totalData));
			$translatePartialLoader.addPart('permintaan-ubah-data');
			console.info("item:" + JSON.stringify(vm.item));
			//console.info("vendorID:" + JSON.stringify(vm.VendorID));
			//UIControlService.loadLoading("Silahkan Tunggu...");
			if (vm.param === 6) {
				$translatePartialLoader.addPart('pengurus-perusahaan');
				loadCompanyPerson(1);
			} else if (vm.param === 2) {
				loadBusinessField(1);
			} else if (vm.param === 9) {
				vm.countryID = vm.item.item.StateLocationRef.CountryID;
				vm.expType = vm.item.item.ExperienceType;
				vm.datepelaksanaan = UIControlService.getStrDate(vm.item.item.StartDate);
				vm.dateselesai = UIControlService.getStrDate(vm.item.item.EndDate);
				if (vm.countryID === 360) {
					vm.lokasidetail = vm.item.item.CityLocation.Name + "," + vm.item.item.StateLocationRef.Name + "," + vm.item.item.StateLocationRef.Country.Name + "," + vm.item.item.StateLocationRef.Country.Continent.Name;
				} else {
					vm.lokasidetail = vm.item.item.StateLocationRef.Name + "," + vm.item.item.StateLocationRef.Country.Name + "," + vm.item.item.StateLocationRef.Country.Continent.Name;
				}
				loadVendorExperience(1);
			}
		}

		//CompanyPerson
		vm.loadCompanyPerson = loadCompanyPerson;
		function loadCompanyPerson(current) {
			vm.currentPage = current;
			var offset = 0;
			PUbahDataService.selectCompanyPerson({
				Parameter: vm.VendorID,
				Offset: offset,
				Limit: vm.totalData
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.verified = reply.data.List;
					vm.totalData = reply.data.Count;
					vm.changedData = [];
					for (var i = 0; i < vm.totalData; i++) {
						if (vm.verified[i].ID === vm.item.item.RefVendorId) {
							vm.changedData.push(vm.verified[i]);
						}
					}
					console.info("changedata compers:" + JSON.stringify(vm.changedData));
				} else {
					$.growl.error({ message: "Gagal mendapatkan data" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				console.info("error:" + JSON.stringify(err));
				UIControlService.unloadLoading();
			});
		}


		//BusinessField
		vm.loadBusinessField = loadBusinessField;
		function loadBusinessField(current) {
			vm.currentPage = current;
			var offset = 0;
			PUbahDataService.selectBusinessField({
				Parameter: vm.VendorID,
				Offset: offset,
				Limit: vm.totalData
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.verified = reply.data.List;
					vm.changedData = [];
					for (var i = 0; i < vm.verified.length; i++) {
						if (vm.verified[i].ID === vm.item.item.RefVendorId) {
							vm.changedData.push(vm.verified[i]);
						}
					}
					console.info("changedata bField:" + JSON.stringify(vm.changedData));
				} else {
					$.growl.error({ message: "Gagal mendapatkan data" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				console.info("error:" + JSON.stringify(err));
				UIControlService.unloadLoading();
			});
		}


		//Experience
		vm.loadVendorExperience = loadVendorExperience;
		function loadVendorExperience(current) {
			console.info("expType:" + vm.expType);
			var offset = 0;
			if (vm.expType == 3154) {
				UIControlService.loadLoading('LOADING.VENDOREXPERIENCE.MESSAGE');
				PUbahDataService.selectVendorExperience({
					Parameter: vm.VendorID,
					Offset: offset,
					Limit: vm.totalData,
					column: 1
				}, function (reply) {
					console.info("expe: " + JSON.stringify(reply));
					if (reply.status === 200) {
						vm.listCurrExp = reply.data.List;
						vm.changedData = [];
						for (var i = 0; i < vm.listCurrExp.length; i++) {
							if (vm.listCurrExp[i].ID === vm.item.item.RefVendorId) {
								vm.changedData.push(vm.listCurrExp[i]);
							}
						}
						console.info("changedata currExp:" + JSON.stringify(vm.changedData));
						if (vm.changedData[0].StateLocationRef.CountryID === 360) {
							vm.lokasidetail = vm.changedData[0].CityLocation.Name + "," + vm.changedData[0].StateLocationRef.Name + "," + vm.changedData[0].StateLocationRef.Country.Name + "," + vm.changedData[0].StateLocationRef.Country.Continent.Name;
						} else {
							vm.lokasidetail = vm.changedData[0].StateLocationRef.Name + "," + vm.changedData[0].StateLocationRef.Country.Name + "," + vm.changedData[0].StateLocationRef.Country.Continent.Name;
						}
						vm.changedData[0].StartDate = UIControlService.getStrDate(vm.changedData[0].StartDate);
						UIControlService.unloadLoading();
					} else {
						UIControlService.unloadLoading();
						UIControlService.msg_growl("error", 'NOTIFICATION.VENDOREXPERIENCE.ERROR', "NOTIFICATION.VENDOREXPERIENCE.TITLE");
					}
				}, function (err) {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("error", 'NOTIFICATION.VENDOREXPERIENCE.ERROR', "NOTIFICATION.VENDOREXPERIENCE.TITLE");
				});
			} else if (vm.expType == 3153) {
				UIControlService.loadLoading('LOADING.VENDOREXPERIENCE.MESSAGE');
				PUbahDataService.selectVendorExperience({
					Parameter: vm.VendorID,
					Offset: offset,
					Limit: 10,
					column: 2
				}, function (reply) {
					//console.info("current?:"+JSON.stringify(reply));
					if (reply.status === 200) {
						UIControlService.unloadLoading();
						vm.listFinExp = reply.data.List;
						vm.changedData = [];
						for (var i = 0; i < vm.listFinExp.length; i++) {
							if (vm.listFinExp[i].ID === vm.item.item.RefVendorId) {
								vm.changedData.push(vm.listFinExp[i]);
							}
						}
						console.info("changedata finExp:" + JSON.stringify(vm.changedData));
						if (vm.changedData[0].StateLocationRef.CountryID === 360) {
							vm.lokasidetail = vm.changedData[0].CityLocation.Name + "," + vm.changedData[0].StateLocationRef.Name + "," + vm.changedData[0].StateLocationRef.Country.Name + "," + vm.changedData[0].StateLocationRef.Country.Continent.Name;
						} else {
							vm.lokasidetail = vm.changedData[0].StateLocationRef.Name + "," + vm.changedData[0].StateLocationRef.Country.Name + "," + vm.changedData[0].StateLocationRef.Country.Continent.Name;
						}
						vm.changedData[0].StartDate = UIControlService.getStrDate(vm.changedData[0].StartDate);
					} else {
						UIControlService.unloadLoading();
						UIControlService.msg_growl("error", 'NOTIFICATION.VENDOREXPERIENCE.ERROR', "NOTIFICATION.VENDOREXPERIENCE.TITLE");
					}
				}, function (err) {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("error", 'NOTIFICATION.VENDOREXPERIENCE.ERROR', "NOTIFICATION.VENDOREXPERIENCE.TITLE");
				});
			}
		}


		//VendorBalance
		vm.batal = batal;
		function batal() {
			$uibModalInstance.close();
		}
	}
})();
//TODO


