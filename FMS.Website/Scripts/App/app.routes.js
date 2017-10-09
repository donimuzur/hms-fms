angular.module('app').run(appRun);

appRun.$inject = ['routerHelper', '$http', '$q'];
/* @ngInject */
function appRun(routerHelper, $http, $q) {
	var states = getStates($http, $q);

	routerHelper.configureStates(states, '/home');
}

function getStates($http, $q) {
	var defer = $q.defer();

	$http.get('app/setting/page.routes.json').then(function (data) {
		defer.resolve(data);
	},
	function (err) {
		console.log(err);
		defer.reject(err);
	});

	return defer.promise;
}