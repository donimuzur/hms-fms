(function () {
	'use strict';

	angular.module("app").controller("rejectRFQFormCtrl", ctrl);

	ctrl.$inject = ['$uibModalInstance', 'item', 'GlobalConstantService'];
	/* @ngInject */
	function ctrl($uibModalInstance, item, GlobalConstantService) {
		var vm = this;
		var RFQVHSId = item.RFQVHSId;
		var RFQName = item.RFQName;
		var RFQCode = item.RFQCode;
		var RFQType = item.RFQType;
		var DeliveryTerms = item.DeliveryTerms;
		var EvalMethod = item.EvalMethod;
		var ProcMethod = item.ProcMethod;

		vm.remark = "";

		vm.init = init;
		function init() {
		};

		vm.reject = reject;
		function reject() {
			$uibModalInstance.close(vm.remark);
		}

		vm.cancel = cancel;
		function cancel() {
			$uibModalInstance.dismiss('cancel');
		};
	}
})();