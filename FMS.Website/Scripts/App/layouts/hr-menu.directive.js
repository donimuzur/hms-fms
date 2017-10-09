(function () {

    angular.module('app').directive('hrMenu', navigation);

    function navigation(AuthService, UIControlService) {
        var template;
        var roles = localStorage.getItem('roles');
        if (roles == "SYSTEM.ROLE_HR") {
            template="Webpage/HrMenu";
        }
        else if (roles == "SYSTEM.ROLE_FLEET") {
            template = "Webpage/FleetMenu";
        }
        else{
            template = "Webpage/UserMenu";
        }
        var directive = {
            restrict: 'E',
            replace: true,
            templateUrl: template,
            link: function ($scope) {
               
                $scope.roles = localStorage.getItem('roles');
                //console.info($scope.roles);
            }
        };

        return directive;
    }

    directiveController.$inject = ['$scope', 'AuthService'];
    
    function directiveController($scope, AuthService) {
        var vm = this;
        vm.roles = localStorage.getItem('roles');
    }
})();