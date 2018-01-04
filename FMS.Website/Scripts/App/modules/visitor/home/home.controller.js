(function () {
	'use strict';

	angular.module("app").controller("HomeController", ctrl);

	ctrl.$inject = ['$scope', '$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'NewsService'];
	/* @ngInject */
	function ctrl($scope, $http, $translate, $translatePartialLoader, $location, SocketService, NewsService) {

		/* jshint validthis: true */
		var vm = $scope;

		// bindable variables
		vm.panels = [];
		vm.activeSlide = 0;
		vm.news = [];

		// functions
		vm.initialize = initialize;
		vm.navigate = goToSlide;
		vm.test = function () {
		    alert("Test");
		}

		// function declarations
		function initialize() {

			// Load partial traslastion
			$translatePartialLoader.addPart('home');
			vm.panels = [1, 2, 3];
			loadNews();
		}

		function loadNews() {
			NewsService.getFrontNews(function (response) {
				vm.news = response.data;
				//response = response.data;
				//if (response.status == 200) {
				//    vm.news = response.result.data;
				//} else {

				//}
			},
			function (error) {

			});
		}

		function goToSlide(index) {
			vm.activeSlide = index;
		}

		// services and events



	}
})();