(function () {
	'use strict';

	angular.module("app").controller("DetailSureActive", ctrl);
	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'VerifikasiDataService', 'UIControlService', 'item', '$uibModalInstance', '$uibModal', 'GlobalConstantService'];
	/* @ngInject */
	function ctrl($http, $translate, $translatePartialLoader, $location, SocketService,
        VerifikasiDataService, UIControlService, item, $uibModalInstance, $uibModal, GlobalConstantService) {
		var vm = this;
		vm.VendorID = item.VendorID;
		vm.Description = item.Description;
		vm.act = item.act;
		vm.init = init;
		vm.aktifasi = "";
		vm.pageSize = 10;
		vm.Keyword = "";

		function init() {
		};

		vm.cancel = cancel;
		function cancel() {
			$uibModalInstance.dismiss('cancel');
		}

		vm.save = save;
		function save() {
			VerifikasiDataService.editActive({
				IsActive: vm.act,
				VendorID: vm.VendorID,
				Description: vm.Description
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var msg = "";
					if (vm.act === false) msg = " NonAktifkan ";
					if (vm.act === true) msg = "Aktifkan ";
					UIControlService.msg_growl("success", "Data Berhasil di " + msg);
					$uibModalInstance.close();
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
})();
