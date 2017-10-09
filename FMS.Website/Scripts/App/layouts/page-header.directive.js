(function () {
	'use strict';

	angular.module('app').directive('pageHeader', navigation);

	function navigation() {
		var directive = {
			restrict: "E",
			replace: true,
			templateUrl: "app/partials/layouts/top-navigation.html",
			controller: directiveController,
			controllerAs: "vm",
			bindToController: true
		};

		return directive;
	}

	directiveController.$inject = ['$scope'];

	function directiveController($scope) {
		var vm = this;
	}
})();