(function () {
	'use strict';

	angular.module('app').factory('GlobalConstantService', serviceMethod);

	serviceMethod.$inject = ['$location', '$window', '$http', '$q'];

	/* @ngInject */
	function serviceMethod($location, $window, $http, $q) {
		var token;
		var refreshToken;
		var headers = {};

		headers.Authorization = 'bearer ' + readToken();
		var port = "52738";
		var dir = "";
		//		var host = "http://localhost:" + port + "/PTVI.EPROC.WEBAPI";
		var host = "http://localhost:" + port;
		host = host + dir;
		//var host = "http://142.40.33.118:" + port + "/PTVI.EPROC.WEBAPI";

		var storage = {
			'api': host,
			'api_endpoint': host + '/apps/vale/api/public',
			'admin_endpoint': host + '/apps/vale/api/admin',
			'vendor_endpoint': host + '/apps/vale/api/vendor',
			'auth_endpoint': host + '/api/Auth',
			'login_endpoint': host + '/login',
			'logout_endpoint': host + '/api/logout'
		};

		var service = {
			get: get,
			post: post,
			getConstant: getConstant,
			readToken: readToken,
			readRefreshToken: readRefreshToken,
			getLoginState: getLoginState,
			getModuleLayer: getModuleLayer
		};

		return service;

		// API http executor
		function get(url) {
			//callRefreshToken();

			var def = $q.defer();

		//	loadHeader();

			$http.get(url, { withCredentials: true }).then(function (data) {
				def.resolve(data);
			}, function (response) {
				if (response.status === 401)
					window.location.href = '#/login';

				def.reject(response.data);
			});

			return def.promise;
		}

		function post(url, data) {
			// callRefreshToken();

			var def = $q.defer();

			// loadHeader();

			$http.post(url, data, { withCredentials: true }).then(function (data) {
				def.resolve(data);
			}, function (response) {
				if (response.status === 401) {
					//window.location.href = '/#/login';
					$location.path('/login');
				}
				else if (response.status === 403) {
					//$window.location.reload();
					//window.location.href = '/#/';
					$location.path('/');
				}

				def.reject(response.data);
			});

			return def.promise;
		}

		function getConstant(name) {
			return storage[name];
		}

		function readToken() {
			return localStorage.getItem('eProcValeToken');
		}

		function readRefreshToken() {
			return localStorage.getItem('eProcValeRefreshToken');
		}

		function loadHeader() {
			headers.Authorization = 'bearer ' + readToken();
		}

		function callRefreshToken() {
			doRefreshToken().then(function (reply) {
				if (reply.status === 200) {
					localStorage.removeItem('eProcValeToken');
					localStorage.removeItem('eProcValeRefreshToken');
					localStorage.setItem('eProcValeToken', reply.data.access_token);
					localStorage.setItem('eProcValeRefreshToken', reply.data.refresh_token);
					localStorage.setItem('sessEnd', new Date().setSeconds(new Date().getSeconds() + reply.data.expires_in));
					localStorage.setItem('roles', JSON.parse(reply.data.roles));
					localStorage.setItem('username', JSON.parse(reply.data.username));
				} else {
				}
			}, function (err) {
			});
		}

		function doRefreshToken() {
			var def = $q.defer();

			$http.post(getConstant('login_endpoint'), 'grant_type=refresh_token&refresh_token=' + readRefreshToken(), {
				headers: {
					'Content-Type': 'application/x-www-form-urlencoded',
					'Authorization': 'bearer ' + readToken()
				}
			}).then(function (data) {
				def.resolve(data);
			}, function (response) {
				def.reject(response.data);
			});

			return def.promise;
		}

		function getLoginState() {
			//if (localStorage.getItem("sessEnd") == null || localStorage.getItem("sessEnd") == '') {
			//	localStorage.removeItem('eProcValeToken');
			//	localStorage.removeItem('eProcValeRefreshToken');
			//	localStorage.removeItem('roles');
			//	localStorage.removeItem('sessEnd');
			//	localStorage.removeItem('username');
			//	localStorage.removeItem('login');
			//	localStorage.removeItem('moduleLayer');
			//	return 'false';
			//}

			var sessEnd = new Date(parseInt(localStorage.getItem("sessEnd")));
			if (true) {
				return 'true';
			} else {
				localStorage.removeItem('eProcValeToken');
				localStorage.removeItem('eProcValeRefreshToken');
				localStorage.removeItem('roles');
				localStorage.removeItem('sessEnd');
				localStorage.removeItem('username');
				localStorage.removeItem('login');
				localStorage.removeItem('moduleLayer');
				return 'false';
			}

			//return localStorage.getItem("login");
		}

		function getModuleLayer() {
			return localStorage.getItem("moduleLayer");
		}
	}
})();