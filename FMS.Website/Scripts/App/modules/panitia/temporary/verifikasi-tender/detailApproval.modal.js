angular.module('eprocAppPanitia')
.controller('detailApprovalCPCtrl', function ($scope, $http, $rootScope, $modalInstance, $cookieStore, $state) {
    $scope.formList = [
        {approv_by:"Radite .C", jabatan:"GM Procurement", tgl_approve:"27-08-2016", status:"Approved", ket:""},
        {approv_by:"Abdul Mutohir", jabatan:"GM Finance", tgl_approve:"30-08-2016", status:"", ket:""}
    ];  
    
    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };  
});