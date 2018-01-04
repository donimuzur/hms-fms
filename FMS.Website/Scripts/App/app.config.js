(function () {
	'use strict';

	angular.module('app').config(configuration);

	configuration.$inject = ["$stateProvider", "$urlRouterProvider", "$ocLazyLoadProvider", "$translateProvider", '$translatePartialLoaderProvider'];

	function configuration($stateProvider, $urlRouterProvider, $ocLazyLoadProvider, $translateProvider, $translatePartialLoaderProvider) {
	    var root = document.location.protocol + "://" + document.location.hostname + document.location.pathname;
		var modulePath = root + "Scripts/App/modules";
		var partialsPath = root + "Scripts/app/partials";

		// ocLazyLoad configuration
		$ocLazyLoadProvider.config({
			debug: false,
			event: true
		});

		// translation configuration
		$translatePartialLoaderProvider.addPart('main');
		$translateProvider.useLoader('$translatePartialLoader', {
			urlTemplate: 'Application/Language?part={part}'
		});
		$translateProvider.preferredLanguage(getCurrLang()); //untuk ganti bahasa
		$translateProvider.useSanitizeValueStrategy('sanitize');

		// state configuration
		$urlRouterProvider.deferIntercept();
		$urlRouterProvider.otherwise('/home');

		function getCurrLang() {
			if (localStorage.getItem("currLang") == null || localStorage.getItem("currLang") == '')
				localStorage.setItem('currLang', 'id');

			return localStorage.getItem("currLang");
		}
	}
})();