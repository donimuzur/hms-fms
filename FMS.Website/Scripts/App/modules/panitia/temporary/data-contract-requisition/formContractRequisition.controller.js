angular.module('eprocAppPanitia')
.controller('formContractReqCtrl', function ($scope, $http, $rootScope, $cookieStore, $state) {
    $scope.formList = [
        {kode:"T#2205R", nama_form:"Contract Requisition", det_form:"", ket:""},
        {kode:"T#2055", nama_form:"Scope of Work Assesment (SOWA)", det_form:"", ket:""},
        {kode:"T#2055R", nama_form:"CSMS Decision Sheet", det_form:"", ket:""}
    ];  
    
    $scope.budgetList=[
        {value:"200.000.000", years:"2016"}, {value:"300.000.000", years:"2017"}
    ];
    
    $scope.vendorList = [{nama:"vendor 1"},{nama:"vendor 2"},{nama:"vendor 3"}];
    
    $scope.back = function() {
        $state.transitionTo('detail-contract-requisition');
    };
});