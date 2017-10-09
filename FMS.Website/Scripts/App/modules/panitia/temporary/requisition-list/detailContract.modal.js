angular.module('eprocAppPanitia')
.controller('detailContractTCCtrl', function ($scope, $http, $rootScope, $modalInstance, $cookieStore, $state) {
    $scope.formList = [
        {nama_form:"Contract Requisition", act:1, ket:"Valid"},
        {nama_form:"Scope of Work Assessment (SOWA)", act:1, ket:"Valid"}
    ];  
    
    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };  
});