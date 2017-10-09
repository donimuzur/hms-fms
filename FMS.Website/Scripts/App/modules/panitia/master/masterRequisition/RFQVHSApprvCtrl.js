(function () {
	'use strict';

	angular.module("app").controller("RFQVHSApprvController", ctrl);

	ctrl.$inject = ['$uibModalInstance', 'item', 'UIControlService', '$uibModal', 'RFQVHSService'];

	function ctrl($uibModalInstance, item, UIControlService, $uibModal, RFQVHSService) {
		var vm = this;
		vm.apprvs = [];

		vm.getDetailApproval = getDetailApproval;
		function getDetailApproval() {
			RFQVHSService.detailApproval({
				ID: item
			}, function (reply) {
				if (reply.status === 200) {
					UIControlService.unloadLoading();
					vm.apprvs = reply.data;
				} else {
					UIControlService.unloadLoading();
				}
			}, function (err) {
				UIControlService.unloadLoading();
			});
		}

		vm.closeDetaiApprv = closeDetaiApprv;
		vm.cancel = closeDetaiApprv;
		function closeDetaiApprv() {
		    $uibModalInstance.close();
		}


	}
})();