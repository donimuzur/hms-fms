(function () {
	'use strict';

	angular.module('app').directive('pageFooter', navigation);

	function navigation() {
		var directive = {
			restrict: "E",
			replace: true,
			templateUrl: "app/partials/layouts/footer.html",
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