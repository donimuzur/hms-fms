(function () {
	'use strict';

	angular.module('app').factory('AuthService', authService);

	authService.$inject = ['$http', '$q', '$rootScope', 'GlobalConstantService'];

	//@ngInject
	function authService($http, $q, $rootScope, GlobalConstantService) {
	    var getAuth = GlobalConstantService.getConstant('auth_endpoint');
		var getLoginUrl = GlobalConstantService.getConstant('login_endpoint');
		var endpoint = GlobalConstantService.getConstant("admin_endpoint");
		var logoutEndpoint = GlobalConstantService.getConstant("logout_endpoint");
		var headers = {};

		//interfaces
		var service = {
			isLoggedIn: isLoggedIn,
			login: login,
			getMenus: getMenus,
			getUserLogin: getUserLogin,
			getRoleUserLogin: getRoleUserLogin,
			logout: logout,
			auth: auth,
            getRoles: getRoles
		};

		return service;

		//public methods
		function getRoleUserLogin(param, successCallback, errorCallback) {
			GlobalConstantService.post(endpoint + "/GetRoles", param).then(successCallback, errorCallback);
		}
		function getUserLogin(successCallback, errorCallback) {
			GlobalConstantService.get(getAuth + "/GetBaseData").then(successCallback, errorCallback);
		}

		function isLoggedIn(successCallback, errorCallback) {
			getIsLoggedIn().then(successCallback, errorCallback);
		}

		function login(loginModel, successCallback, errorCallback) {
			doLogin(loginModel.username, loginModel.password).then(successCallback, errorCallback);
		}

		function getMenus(successCallback, errorCallback) {
			GlobalConstantService.get(endpoint + "/menu-roles").then(successCallback, errorCallback);
		}

		function logout(successCallback, errorCallback) {
			GlobalConstantService.get(logoutEndpoint).then(successCallback, errorCallback);
		}

		function auth(successCallback, errorCallback) {
		    var url = "Login/IsAuthorized";;
		    GlobalConstantService.get(url).then(successCallback, errorCallback);
		}

		function getRoles(successCallback, errorCallback) {
		    var url = "Login/GetRole";
		    GlobalConstantService.get(url).then(successCallback, errorCallback);
		}

		//private methods
		function getIsLoggedIn() {
			var def = $q.defer();

			headers.Authorization = 'bearer ' + GlobalConstantService.readToken();

			$http.get(getAuth + '/isloggedin', { headers: headers }).then(function (data) {
				def.resolve(data);
			}, function () {
				def.reject('Failed to get data');
			});

			return def.promise;
		}

        function doLogin(username, password) {
            console.log(getLoginUrl);
			var def = $q.defer();

			$http.post(getLoginUrl, 'grant_type=password&username=' + username + '&password=' + password, {
				headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
			}).then(function (data) {
				def.resolve(data);
			}, function (error) {
				def.reject(error);
				//def.reject('Failed to get data');
			});

			return def.promise;
		}
	}
})();