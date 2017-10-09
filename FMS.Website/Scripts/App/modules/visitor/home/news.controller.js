(function () {
    'use strict';

    angular.module("app")
    .controller("NewsController", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'NewsService'];
    /* @ngInject */
    function ctrl($rootScope, $http, $translate, $translatePartialLoader, $location, SocketService, NewsService) {

        /* jshint validthis: true */
        var vm = this;

        // bindable variables
        vm.news = {};

        // functions
        vm.initialize = initialize;


        // function declarations
        function initialize() {

            // Load partial traslastion
            $translatePartialLoader.addPart('home');
            vm.panels = [1, 2, 3];
            loadNews();


        }

        function loadNews() {
            NewsService.find(
                function (response) {
                    response = response.data;
                    if (response.status == 200) {
                        vm.news = response.result.data;
                    } else {

                    }
                },
                function (error) {
                    
                });
        }

        // services and events



    }
})();