(function () {
	'use strict';

	angular.module("app").controller("jabatanModalCtrl", ctrl);

	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'DepartemenService', 'RoleService', 'UIControlService', 'item', '$uibModalInstance'];

	function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, DepartemenService,
		RoleService, UIControlService, item, $uibModalInstance) {
		var vm = this;

	}
})();