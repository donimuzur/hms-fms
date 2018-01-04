(function () {
	'use strict';

	angular.module("app").controller("DetailCRVendorCtrl", ctrl);

	ctrl.$inject = ['$translatePartialLoader', 'SocketService', 'PermintaanUbahDataService', 'UIControlService', 'item', '$uibModalInstance', ];
	function ctrl($translatePartialLoader, SocketService, PUbahDataService, UIControlService, item, $uibModalInstance) {
		var vm = this;
		vm.CRID = item.ChangeRequestID;
		vm.CREndDate = item.EndChangeDate;
		vm.listCR = [];
		vm.VendorName = item.VendorName;
		vm.CRDate = item.ChangeRequestDate;
		vm.isCalendarOpened = [false];

		vm.init = init;
		function init() {
			$translatePartialLoader.addPart("permintaan-ubah-data");
			loadData();
			if (!(vm.CREndDate === null)) {
				vm.CREndDate = new Date(Date.parse(vm.CREndDate));
			}
		};

		function loadData() {
			UIControlService.loadLoading("Silahkan Tunggu");
			PUbahDataService.getDetailDataCR({ ChangeRequestID: vm.CRID }, function (reply) {
				//console.info("data:" + JSON.stringify(reply));
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.data;
					vm.listCR = data;
				} else {
					UIControlService.msg_growl("error", "Gagal Mendapatkan Detai Data");
					UIControlService.unloadLoading();
				}
			}, function (err) {
				//console.info("error:" + JSON.stringify(err));
				UIControlService.msg_growl("error", "Gagal akses API");
				UIControlService.unloadLoading();
			});
		}

		vm.approvedCR = approvedCR;
		function approvedCR(IsApprove) {
			if (vm.CREndDate === null) {
				UIControlService.msg_growl("warning", "Please insert end date.");
				return false;
			}

			var newdetail = [];
			//console.info(JSON.stringify(vm.listCR));
			for (var i = 0; i < vm.listCR.length; i++) {
				var approve = 0;
				if (vm.listCR[i].IsApproved === true) {
					approve = 1;
				}
				var dt = {
					DetailCRID: vm.listCR[i].DetailCRID,
					IsApproved: approve
				}
				newdetail.push(dt);
			}

			UIControlService.loadLoading("Silahkan Tunggu");
			PUbahDataService.openLockCR({
				ChangeRequestID: vm.CRID,
				IsActive: IsApprove,
				ChangeRequestDataDetails: newdetail,
				EndChangeDate: UIControlService.getStrDate(vm.CREndDate)
			}, function (reply) {
				//console.info("data:" + JSON.stringify(reply));
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					UIControlService.msg_growl("success", "Berhasil simpan data Approve");
					SocketService.emit("PermintaanUbahData");
					$uibModalInstance.close();
				} else {
					UIControlService.msg_growl("error", "Gagal Mendapatkan Detail Data");
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
		}

		vm.batal = batal;
		function batal() {
			$uibModalInstance.dismiss('cancel');
		};
	}
})();