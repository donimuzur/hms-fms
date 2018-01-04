(function () {
	'use strict';

	angular.module("app").controller("LogoutController", ctrl);

	ctrl.$inject = ['$location', '$window', '$state'];

	function ctrl($location, $window, $state) {
		var vm = this;

		vm.logout = logout;
		function logout() {
			setTimeout(function () {
				localStorage.removeItem('eProcValeToken');
				localStorage.removeItem('eProcValeRefreshToken');
				localStorage.removeItem('roles');
				localStorage.removeItem('sessEnd');
				localStorage.removeItem('username');
				localStorage.removeItem('login');
				localStorage.removeItem('moduleLayer');
				//scope.$apply(function () { $location.path("/home"); });
				redirect();
			}, 2500);
		}

		function redirect() {
			//$window.location.href = '/';
			$state.transitionTo('home');
		}
	}
})();