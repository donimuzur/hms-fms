(function () {
	'use strict';

	angular.module('app').directive('adminTopMenu', navigation);

	function navigation() {
		var directive = {
			restrict: 'E',
			replace: true,
			templateUrl: "Webpage/AdminHeader",
			controller: directiveController,
			controllerAs: "tmCtrl",
			bindToController: true
		};

		return directive;
	}

	directiveController.$inject = ['$scope'];

	function directiveController($scope) {
	    var vm = this;

        vm.toggle = toggle;
        function toggle() {
            console.info(":");
			var duration = 500;
			var animation = 'easeOutCirc';
			if ($(window).width() <= 992) {
				$('.row-offcanvas').toggleClass('active', duration, animation);
				$('.left-side').removeClass("collapse-left");
				$(".right-side").removeClass("strech");
				$('.row-offcanvas').toggleClass("relative", duration, animation);
			} else {
				//Else, enable content streching
				$('.left-side').toggleClass("collapse-left", duration, animation);
				$(".right-side").toggleClass("strech", duration, animation);
			}
		}


	}
})();