(function () {
	'use strict';

	angular.module("app").controller("DataVendorCtrl", ctrl);

	ctrl.$inject = ['UIControlService', '$uibModalInstance', 'PengumumanPengadaanService','item'];

	function ctrl(UIControlService, $uibModalInstance, PengumumanPengadaanService, items) {
		var vm = this;

		vm.vendors = null;
		vm.currentPage = 1;
		vm.maxSize = 10;
		vm.keyword = '';

		vm.getAllVendors = getAllVendors;
		function getAllVendors() {
			UIControlService.loadLoadingModal('LOADING.GETVENDORS.MESSAGE');
			PengumumanPengadaanService.getAllVendors({
				Vendors: items,
				Offset: (vm.currentPage - 1) * vm.maxSize,
				Limit: vm.maxSize,
				Keyword: vm.keyword
			}, function (reply) {
				if (reply.status === 200) {
					vm.vendors = reply.data.List;
					vm.totalItems = reply.data.Count;
					UIControlService.unloadLoadingModal();
				} else {
					UIControlService.unloadLoadingModal();
					UIControlService.msg_growl("error", 'NOTIFICATION.GET.LOCATION.ERROR', "NOTIFICATION.GET.LOCATION.TITLE");
				}
			}, function (err) {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("error", 'NOTIFICATION.GET.LOCATION.ERROR', "NOTIFICATION.GET.LOCATION.TITLE");
			});
		}

		vm.pageChanged = pageChanged;
		function pageChanged() {
			getAllVendors();
		}

		vm.selectVendor = selectVendor;
		function selectVendor(selectedVendor) {
		    //console.info(JSON.stringify(selectedVendor));
		    var getVendor = { Code: selectedVendor.Code, VendorName: selectedVendor.Name, 
		        Email: selectedVendor.Email, VendorID: selectedVendor.VendorID }
		    $uibModalInstance.close(getVendor);
		}

		vm.batal = batal;
		function batal() {
		    $uibModalInstance.dismiss('cancel');
		};
	}
})();