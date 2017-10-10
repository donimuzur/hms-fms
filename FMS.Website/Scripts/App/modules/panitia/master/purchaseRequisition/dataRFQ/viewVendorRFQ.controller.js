(function () {
	'use strict';

	angular.module('app').controller('viewVendorRFQCtrl', ctrl);

	ctrl.$inject = ['model', 'PurchaseRequisitionService', 'UIControlService', '$uibModalInstance'];

	function ctrl(model, PurchReqService, UIControlService, $uibModalInstance) {
		var vm = this;
		//console.info("rfq:" + JSON.stringify(model));
		vm.vendors = null;
		vm.currentPage = 1;
		vm.maxSize = 10;
		vm.keyword = '';

		vm.init = init;
		function init() {
			UIControlService.loadLoadingModal('LOADING.VIEW_VENDOR');
			PurchReqService.viewVendor({
				CommodityID: model.CommodityID,
				IsLocal: model.IsLocal,
				IsNational: model.IsNational,
				IsInternational: model.IsInternational,
				CompScale: model.CompScale
			}, function (reply) {
			    console.info("ven:" + JSON.stringify(reply));
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

		vm.batal = batal;
		function batal() {
		    $uibModalInstance.dismiss('cancel');
		};
	}
})();