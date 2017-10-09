angular.module('eprocAppPanitia')
.controller('detailKelengkapanTenderCECtrl', function ($scope, $http, $rootScope, $modalInstance, $cookieStore, $state) {
    $scope.formList = [
        {kode:"T#2205R", nama_form:"Sect 1 General Information & Draft of Agreement", det_form:"", ket:""},
        {kode:"T#2055", nama_form:"Sect 2 General Term & Condition", det_form:"", ket:""},
        {kode:"T#2055R", nama_form:"Sect 3 Special Term & Condition", det_form:"", ket:""}
    ];  
    
    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };  
});