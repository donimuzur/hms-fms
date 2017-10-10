angular.module('eprocAppPanitia')
.controller('csmsDecisionCtrl', function ($scope, $http, $rootScope, $cookieStore, $state) {
    $scope.formList = [
        {nama_form:"Pengumuman Tender", act:1, ket:"Valid"},
        {nama_form:"Pendaftaran Tender", act:1, ket:"Valid"},
        {nama_form:"Pre-Bid Meeting", act:1, ket:"Valid"}
    ];  
    
    $scope.cancel = function() {
        $state.transitionTo('data-contract-requisition');
    };  
});