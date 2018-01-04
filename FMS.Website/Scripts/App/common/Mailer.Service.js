(function () {
	'use strict';

	angular.module('app').factory('MailerService', mailerService);

	mailerService.$inject = ['GlobalConstantService'];

	function mailerService(GlobalConstantService) {
		var endpoint = GlobalConstantService.getConstant("api_endpoint");
		// interfaces
		var service = {
			getMailContent: getMailContent,
			getActMail: getActivationMail,
			sendMail: sendEmail,
		};

		return service;

		function getMailContent(model, successCallback, errorCallback) {
			GlobalConstantService.post(endpoint + "/vendor/registration/getMailContent", model).then(successCallback, errorCallback);
		}

		function getActivationMail(model, successCallback, errorCallback) {
			GlobalConstantService.post(endpoint + "/verifiedvendor/getActivationMail", model).then(successCallback, errorCallback);
		}

		function sendEmail(mail, successCallback, errorCallback) {
			GlobalConstantService.post(endpoint + "/vendor/registration/send-email", mail).then(successCallback, errorCallback);
		}
	}
})();