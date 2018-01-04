(function () {
    'use strict';

    angular.module("app")
    .controller("formApprovalModalCtrl", ctrl);
    
    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DashboardService'];
    /* @ngInject */
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, DashboardService) {
        
    }
})();