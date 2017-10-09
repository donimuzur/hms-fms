(function () {
	'use strict';

	angular.module("app").factory("ConverterService", service);

	service.$inject = [];
	/* @ngInject */
	function service() {
		// interfaces
		var service = {
			all: all
		};

		return service;

		// implementation

		// yyyy-mm-dd or yyyy/mm/dd to dd Mon yyyy
		function toLongDate(input) {
			if (!input) {
				return 'N/A';
			}

			var date = input.substring(8, 10);
			var mon = input.substring(5, 7);
			var year = input.substring(0, 4);

			if (isNaN(year)) { // wrong format
				return input; // return as it is
			}

			return date + "-" + mon + "-" + year;
		}
	}
})();