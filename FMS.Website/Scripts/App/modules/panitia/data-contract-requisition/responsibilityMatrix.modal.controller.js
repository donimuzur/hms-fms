(function () {
	'use strict';

	angular.module("app")
    .controller("responsibilityMatrixModalCtrl", ctrl);

	ctrl.$inject = ['$state', '$http', '$filter', '$uibModalInstance', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'UIControlService', 'item'];
	/* @ngInject */
	function ctrl($state, $http, $filter, $uibModalInstance, $translate, $translatePartialLoader, $location, SocketService, UIControlService, item) {

		var vm = this;
		var loadmsg = "";

		vm.title = item.title;
		vm.respMatrix = item.respMatrix;

		vm.init = init;
		function init() {
			$translatePartialLoader.addPart('data-contract-requisition');
		};

		vm.save = save;
		function save() {
			$uibModalInstance.close();
		}

		vm.cancel = cancel;
		function cancel() {
			$uibModalInstance.dismiss('cancel');
		};
	}

})();