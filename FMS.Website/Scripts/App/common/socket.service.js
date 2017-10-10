(function () {
	'use strict';

	angular.module('app').factory('SocketService', socketService);

	socketService.$inject = ['$rootScope'];

	/* @ngInject */
	function socketService($rootScope) {
		//var socket = io.connect('http://142.40.33.118:1067', {
		var socket = io.connect('http://localhost:1067', {
			'reconnection': true,
			'reconnectionDelay': 1000,
			'reconnectionDelayMax': 5000,
			'reconnectionAttempts': 5
		});

		var service = {
			on: onEventHandler,
			emit: emitEventHandler
		};

		function onEventHandler(eventName, callback) {
			socket.on(eventName, eventHandler);

			function eventHandler() {
				var args = arguments;

				$rootScope.$apply(function () {
					callback.apply(socket, args);
				});
			}
		}

		function emitEventHandler(eventName, data, callback) {
			socket.emit(eventName, data, eventHandler);

			function eventHandler() {
				var args = arguments;
				$rootScope.$apply(function () {
					if (callback) {
						callback.apply(socket, args);
					}
				});
			}
		}

		return service;
	}
})();