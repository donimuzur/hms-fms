(function () {
	'use strict';

	angular.module("app").controller("CROpenLockCtrl", ctrl);

	ctrl.$inject = ['$translatePartialLoader', 'PermintaanUbahDataService', '$state', '$stateParams', 'UIControlService', '$uibModal', ];
	function ctrl($translatePartialLoader, PUbahDataService, $state, $stateParams, UIControlService, $uibModal) {
		var vm = this;
		vm.listLockVendor = [];
		vm.FilterColumn = 0;
		vm.textSearch = '';
		vm.maxSize = 10;
		vm.currentPage = 0;
		vm.currentMainPage = Number($stateParams.currentMainPage);
		vm.isCalendarOpened = [false, false, false, false];
		vm.verifikasi = {};

		vm.init = init;
		function init() {
			//console.info("currMainPage OL:" + vm.currentMainPage);
			vm.listDropdown = [
                { Value: 0, Name: "SELECT.ALL" },
                { Value: 1, Name: "SELECT.REQUESTED" },
                { Value: 2, Name: "SELECT.APPROVED" },
                { Value: 3, Name: "SELECT.REJECTED" },
                { Value: 4, Name: "SELECT.FINAL_APPROVED" },
                { Value: 5, Name: "SELECT.FINAL_REJECTED" }];

			$translatePartialLoader.addPart("permintaan-ubah-data");

			if (vm.currentMainPage === 0 || vm.currentMainPage === undefined) {
				jLoad(1);
			} else {
				jLoad(vm.currentMainPage);
			}
		};

		vm.jLoad = jLoad;
		function jLoad(current) {
			UIControlService.loadLoading("Silahkan Tunggu...");
			vm.currentPage = current;
			var offset = (current * 10) - 10;
			PUbahDataService.getDataCR({
				//column: vm.FilterColumn,
				Keyword: vm.textSearch,
				//Offset: (current - 1) * vm.maxSize,
				Offset: offset,
				Status: vm.cmbStatus,
				Limit: 10,
				Date1: UIControlService.getStrDate(vm.verifikasi.StartDate),
				Date2: UIControlService.getStrDate(vm.verifikasi.EndDate)
			}, function (reply) {
				console.info("dataQue:" + JSON.stringify(reply));
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.data;
					vm.listVendors = data.List;
					for (var i = 0; i < vm.listVendors.length; i++) {
						if (!(vm.listVendors.ChangeRequestDate === null)) {
							vm.listVendors[i].ChangeRequestDate = UIControlService.getStrDate(vm.listVendors[i].ChangeRequestDate);
						}
						if (!(vm.listVendors[i].EndChangeDate === null)) {
							vm.listVendors[i].EndChangeDate = UIControlService.getStrDate(vm.listVendors[i].EndChangeDate);
						}
					}
					vm.totalItems = data.Count;
				} else {
					UIControlService.msg_growl("error", "Gagal mendapatkan data Master Badan Usaha");
					UIControlService.unloadLoading();
				}
			}, function (err) {
				//console.info("error:" + JSON.stringify(err));
				UIControlService.msg_growl("error", "Gagal akses API");
				UIControlService.unloadLoading();
			});
		}

		vm.openCalendar = openCalendar;
		function openCalendar(index) {
			vm.isCalendarOpened[index] = true;
		};

		vm.verifyEndDate = verifyEndDate;
		function verifyEndDate(selectedEndDate, selectedStartDate) {
			var convertedEndDate = UIControlService.getStrDate(selectedEndDate);
			var convertedStartDate = UIControlService.getStrDate(selectedStartDate);
			//console.info("selected end date" + JSON.stringify(convertedEndDate));
			//console.info("selected start date" + JSON.stringify(convertedStartDate));
			if (convertedEndDate < convertedStartDate) {
				UIControlService.msg_growl("warning", "Tanggal batas akhir tidak boleh sebelum batas awal");
				vm.verifikasi.EndDate = " ";
			}
			//else {
			//  console.info("masak");
			//}
		}

		vm.show = show;
		function show() {
			if (vm.Status === undefined) {
				vm.cmbStatus = 0;
			} else {
				vm.cmbStatus = vm.Status.Value;
			}
			jLoad(1);
		}

		vm.formCRVendor = formCRVendor;
		function formCRVendor(data) {
			console.info(JSON.stringify(data));
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/data-rekanan/buka-lock-rekanan/detail-changerequest.html',
				controller: 'DetailCRVendorCtrl',
				controllerAs: 'detCRVCtrl',
				resolve: {
					item: function () {
						return data;
					}
				}
			});
			modalInstance.result.then(function () {
				vm.init();
			});
		}

		vm.detailCR = detailCR;
		function detailCR(ID, currentPage) {
			$state.transitionTo('detail-openlock-vendor', { CRID: ID, currentMainPage: currentPage });
		}
	}
})();