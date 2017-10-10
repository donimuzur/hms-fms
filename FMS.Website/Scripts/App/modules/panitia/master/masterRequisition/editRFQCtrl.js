/*
(function () {
	'use strict';

	angular.module("app").controller("editRFQCtrl", ctrl);

	ctrl.$inject = ['item', 'RFQVHSService', 'UIControlService'];

	function ctrl(item, RFQVHSService, UIControlService) {
		var vm = this;

		vm.rfqvhs = null;

		vm.loadRFQVHS = loadRFQVHS;
		function loadRFQVHS() {
			UIControlService.loadLoadingModal('LOADING.LOAD.RFQ');
			RFQVHSService.loadRFQVHS({ ID: item }, function (reply) {
				if (reply.status === 200) {
					UIControlService.unloadLoadingModal();
					vm.rfqvhs = reply.data;
				} else {
				}
			}, function (err) {
			});
		}
	}
})();
*/