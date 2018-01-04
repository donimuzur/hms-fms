(function () {
    'use strict';

    angular.module('app').directive('eprocMenu', navigation);

    function navigation() {
        var directive = {
            restrict: 'E',
            link: function ($scope, elt, attrs) {
                alert('Clicked!');
                $(elt).tree();
            }
        };

        return directive;
    }
})();