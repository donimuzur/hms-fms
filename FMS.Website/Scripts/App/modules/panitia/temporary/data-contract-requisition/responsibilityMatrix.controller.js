angular.module('eprocAppPanitia')
.controller('responsibilityMatrixCtrl', function ($scope, $http, $rootScope, $modal, $cookieStore, $state) {
    $scope.site = [
        {item:"1.1",desc:"Offices",comment:""},
        {item:"1.2",desc:"Workshops",comment:""},
        {item:"1.3",desc:"Computers",comment:""},
        {item:"1.4",desc:"Phone",comment:""},
        {item:"1.5",desc:"Internet",comment:""},
        {item:"1.6",desc:"Consumables",comment:""}
    ];
    
    $scope.serv = [
        {item:"2.1",desc:"Power - High Voltage",comment:" "},
        {item:"2.2",desc:"Power - Low Voltage",comment:""},
        {item:"2.3",desc:"Water – Potable",comment:""},
        {item:"2.4",desc:"Phone",comment:""},
        {item:"2.5",desc:"Internet",comment:""}
    ];
    
    $scope.back = function(dt) {
        $state.transitionTo('data-contract-requisition');
    };
    
});