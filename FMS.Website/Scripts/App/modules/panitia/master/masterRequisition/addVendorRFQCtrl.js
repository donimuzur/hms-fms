(function () {
	'use strict';

	angular.module("app").controller("addVendorRFQCtrl", ctrl);

	ctrl.$inject = ['UIControlService', 'items', '$uibModalInstance', 'RFQVHSService'];

	function ctrl(UIControlService, items, $uibModalInstance, RFQVHSService) {
		var vm = this;

		vm.vendors = null;
		vm.currentPage = 1;
		vm.maxSize = 10;
		vm.keyword = '';

		vm.getAllVendors = getAllVendors;
		function getAllVendors() {
			UIControlService.loadLoadingModal('LOADING.GETVENDORS.MESSAGE');
			RFQVHSService.getAllVendors({
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
			$uibModalInstance.close(selectedVendor);
		}
	}
})();