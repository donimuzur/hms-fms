(function () {
	'use strict';

	angular.module('app').controller('viewVendorRFQCtrl', ctrl);

	ctrl.$inject = ['model', 'RFQVHSService', 'UIControlService', '$translatePartialLoader', '$uibModalInstance'];

	function ctrl(model, RFQVHSService, UIControlService, $translatePartialLoader, $uibModalInstance) {
		var vm = this;

		vm.vendors = null;
		vm.currentPage = 1;
		vm.maxSize = 10;
		vm.keyword = '';

		vm.init = init;
		function init() {
		    UIControlService.loadLoadingModal('LOADING.VIEW.VENDOR');
		    $translatePartialLoader.addPart('vhs-requisition');
			RFQVHSService.viewVendor({
				CommodityID: model.CommodityID,
				IsLocal: model.IsLocal,
				IsNational: model.IsNational,
				IsInternational: model.IsInternational,
				CompScale: model.CompScale
			}, function (reply) {
				if (reply.status === 200) {
					vm.vendors = reply.data;
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

		vm.cancel = cancel;
		function cancel() {
		    $uibModalInstance.dismiss('cancel');
		};
	}
})();