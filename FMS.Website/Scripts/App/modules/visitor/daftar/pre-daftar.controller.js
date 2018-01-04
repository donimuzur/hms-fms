(function () {
	'use strict';

	angular.module("app").controller("PreDaftar", ctrl);

	ctrl.$inject = ['$scope', '$http', '$translate', '$translatePartialLoader', '$location', 'SocketService'];
	/* @ngInject */
	function ctrl($scope, $http, $translate, $translatePartialLoader, $location, SocketService) {

		/* jshint validthis: true */
		var vm = this;

		// bindable variables
		vm.panels = [];
		vm.activeSlide = 0;
		vm.news = [];

		// functions
		vm.initialize = initialize;

		// function declarations
		function initialize() {

			// Load partial traslastion
			$translatePartialLoader.addPart('pre-daftar');
			vm.panels = [1, 2, 3];
		}

	}
})();