(function () {
	'use strict';

	angular.module("app").factory("NotificationService", svc);

	svc.$inject = ['GlobalConstantService'];

	function svc(GlobalConstantService) {
		var endpoint = GlobalConstantService.getConstant("api_endpoint");
		var adminEndpoint = GlobalConstantService.getConstant("admin_endpoint");
		var vendorEndpoint = GlobalConstantService.getConstant("vendor_endpoint");
		// interfaces
		var service = {
			activationReqCount: activationReqCount,
			dataChgReqCount: dataChgReqCount,
			dataCntrctReqApprv: dataCntrctReqApprv
		};

		return service;

		function activationReqCount(successCallback, errorCallback) {
			GlobalConstantService.get(endpoint + "/verifiedvendor/activationReqCount").then(successCallback, errorCallback);
		}

		function dataChgReqCount(successCallback, errorCallback) {
			GlobalConstantService.get(vendorEndpoint + "/changerequest/dataChgReqCount").then(successCallback, errorCallback);
		}

		function dataCntrctReqApprv(successCallback, errorCallback) {
			GlobalConstantService.get(vendorEndpoint + "/changerequest/dataCntrctReqApprvCount").then(successCallback, errorCallback);
		}
	}
})();