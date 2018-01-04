(function () {
	'use strict';

	angular.module('app').provider('routerHelper', routerHelperProvider);

	routerHelperProvider.$inject = ['$locationProvider', '$stateProvider', '$urlRouterProvider'];
	/* @ngInject */
	function routerHelperProvider($locationProvider, $stateProvider, $urlRouterProvider) {
		/* jshint validthis:true */
		this.$get = RouterHelper;

		// $locationProvider.html5Mode(true);

		RouterHelper.$inject = ['$state', '$ocLazyLoad', '$urlRouter'];
		/* @ngInject */
		function RouterHelper($state, $ocLazyLoad, $urlRouter) {
			var hasOtherwise = false;

			var service = {
				configureStates: configureStates,
				getStates: getStates
			};

			return service;

			function configureStates(states, otherwisePath) {
				var test = [];
				states.forEach(function (state) {
					var existingState = $state.get(state.state);

					if (existingState !== null) {
						return;
					}
					var root = document.location.protocol + "://" + document.location.hostname + document.location.pathname;
					var newstate = {
						templateUrl:state.template,
						url: state.url,
						pageTitle: state.title
					};

					$stateProvider.state(state.state, newstate);
					test.push(newstate);
				});
				//console.info(JSON.stringify(test));
				//$urlRouter.sync();
				//$urlRouter.listen();
				if (otherwisePath && !hasOtherwise) {
					hasOtherwise = true;
					//console.log("otherwise", otherwisePath);
					$urlRouterProvider.otherwise(otherwisePath);
				}
			}

			function getStates() {
				return $state.get();
			}
		}
	}
})();