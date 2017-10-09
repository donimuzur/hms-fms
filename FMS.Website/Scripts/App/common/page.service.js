(function () {
	'use strict';

	angular.module('app').factory('PageComponentService', serviceMethod);

	serviceMethod.$inject = ['$http', '$q'];

	/* @ngInject */
	function serviceMethod($http, $q) {
		var service = {
			config: get
		};

		return service;

		function get(url) {
			var def = $q.defer();

			$http.get(url).then(function (data) {
				def.resolve(data);
			},
			function () {
				def.reject("Failed to get config");
			});

			return def.promise;
		}
	}
})();