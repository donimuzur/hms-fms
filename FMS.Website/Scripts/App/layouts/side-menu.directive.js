(function () {
	'use strict';

	angular.module('app').directive('sideMenu', navigation);

	function navigation() {
		var directive = {
			restrict: 'E',
			replace: true,
			scope: { menuItems: "=" },
			templateUrl: "app/partials/layouts/admin-menu.html",
			controller: directiveController
		};

		return directive;
	}

	directiveController.$inject = ['$scope'];

	function directiveController($scope) {

		$(".sidebar").slimscroll({
			height: ($(window).height() - $(".header").height()) + "px",
			color: "rgba(#333)"
		});

		//var item, _i, _len, _ref, _ref1;
		//_ref = $scope.menuItems;
		//for (_i = 0, _len = _ref.length; _i < _len; _i++) {
		//    item = _ref[_i];
		//    item.hasSubmenu = (item.submenu != null) && ((_ref1 = item.submenu) != null ? _ref1.length : void 0) > 0;
		//    item.expanded = false;
		//    item.hasBadge = !(item.badge == null);
		//}
		return console.log($scope.menuItems);
	}
})();