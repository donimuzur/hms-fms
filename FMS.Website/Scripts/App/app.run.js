(function () {
    'use strict';

    angular.module('app').run(runner);

    runner.$inject = ['$rootScope', '$translate', '$state', '$stateParams', '$http', '$q', 'routerHelper', '$urlRouter', 'UIControlService', 'AuthService'];

    function runner($rootScope, $translate, $state, $stateParams, $http, $q, routerHelper, $urlRouter, UIControlService, AuthService) {

        $rootScope.$state = $state;
        $rootScope.$stateParams = $stateParams;


        // Register events and broadcasts
        $rootScope.$on('$translatePartialLoaderStructureChanged', function () {
            //console.log("change translation structure");
            $translate.refresh();
        });

        $rootScope.$on("$stateChangeSuccess", function (event, currentRoute) {
            //console.log("change state success!");
            AuthService.getRoles(
                    function (response) {
                        UIControlService.unloadLoading();
                        console.log(response.data);

                        if (response.status == 200) {
                            localStorage.setItem('login', true);
                            localStorage.setItem('moduleLayer', 1);
                            localStorage.setItem("nt-account", response.data.account);
                            localStorage.setItem("username", response.data.username);
                            $rootScope.username = response.data.username;
                            var role = response.data.role;
                            localStorage.setItem("roles", role);
                            //document.location.reload();

                            if ($state.current.name == "home") {
                                UIControlService.msg_growl("success", "Welcome, " + $rootScope.username);
                            //    if (role == 'SYSTEM.ROLE_HR') {
                            //        $state.go('csf-dashboard');
                            //    } else if (role == 'SYSTEM.ROLE_FLEET') {
                            //        $state.go('CSF-WTC');
                            //    } else {
                            //        $state.go('CAF-Dashboard');
                            //    }
                            }
                        } else {
                            UIControlService.msg_growl("danger", err.data);
                        }

                    },
                    function (err) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("danger", err.data);
                    });

            //console.log("logged in: " + (!localStorage.getItem("login")) ? false : true);
            $rootScope.pageTitle = currentRoute.pageTitle;
            $rootScope.namePage = currentRoute.name;
            //filterState(currentRoute.name);
        });

        $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
            //console.log("change state start!");
        });

        getStates($http, $q).then(function (data) {
            routerHelper.configureStates(data.states, data.otherwise);
            $urlRouter.sync();
            $urlRouter.listen();
            //console.log($state.get());
        });

        function filterState(stateName) {
            var whitelist = [
                'login',
                'home'
            ];

            if (whitelist.indexOf(stateName) < 0) {
                UIControlService.handleUnauthorizedAccess('home');
            }
        }
    }

    function getStates($http, $q) {
        var defer = $q.defer();
        var root = document.location.protocol + "://" + document.location.hostname + document.location.pathname;
        $http.get('Application/AngularRoutes').then(function (data) {
            defer.resolve(data.data);
        },
		function (err) {
		    defer.reject(err);
		});
        return defer.promise;
    }


})();