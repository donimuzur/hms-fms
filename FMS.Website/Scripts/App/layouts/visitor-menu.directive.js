(function () {
	'use strict';

	angular.module('app').directive('visitorMenu', visitorMenu);

	function visitorMenu() {
		var directive = {
			restrict: 'E',
			replace: true,
			templateUrl: 'app/partials/layouts/visitor-menu.html'
		};

		return directive;
	}

})();