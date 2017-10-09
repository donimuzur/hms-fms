(function () {
	'use strict';

	angular.module('app').factory('CommonEngineService', serviceMethod);

	serviceMethod.$inject = ['$http', '$q', 'GlobalConstantService'];

	/* @ngInject */
	function serviceMethod($http, $q, GlobalConstantService) {

	    var endpoint = GlobalConstantService.getConstant("api_endpoint");

	    var service = {
	        GetLoggedEmployee: GetLoggedEmployee,
	        GetLoggedVendor: GetLoggedVendor
		};

		return service;

		function GetLoggedEmployee(successCallback, errorCallback) {
		    GlobalConstantService.get(endpoint + "/common/getloggedemployee").then(successCallback, errorCallback);
		}

		function GetLoggedVendor(successCallback, errorCallback) {
		    GlobalConstantService.get(endpoint + "/common/get-logged-vendor").then(successCallback, errorCallback);
		}
	}
})();