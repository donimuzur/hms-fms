(function () {
	'use strict';

	angular.module('app').directive('carouselControls', controls);

	function controls() {
		var directive = {
			restrict: 'A',
			link: function (scope, element, attrs) {
				scope.goNext = function () {
					element.isolateScope().next();
				};
				scope.goPrev = function () {
					element.isolateScope().prev();
				};
			}
		};

		return directive;
	}

	directiveController.$inject = ['$scope'];

	function directiveController($scope) {
		var vm = this;
	}

})();