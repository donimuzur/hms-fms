angular.module('eprocAppPanitia')
.controller('detailContractReqCtrl', function ($scope, $http, $rootScope, $cookieStore, $state) {
    $scope.formList = [
        {kode:"T#2205R", nama_form:"Contract Requisition", det_form:"form-contract-requisition", ket:""},
        {kode:"T#2055", nama_form:"Scope of Work Assesment (SOWA)", det_form:"scope-ofwork-assesment-dc", ket:""},
        {kode:"T#2055R", nama_form:"CSMS Decision Sheet", det_form:"detail-contract-requisition", ket:""},
        {kode:"T#2055R", nama_form:"SOW", det_form:"", ket:""},
        {kode:"T#2055R", nama_form:"Responsibility Matrix", det_form:"responsibility-matrix-dc", ket:""},
        {kode:"T#2055R", nama_form:"Detail Owner Estimate", det_form:"detail-cost-estimate", ket:""},
        {kode:"T#2055R", nama_form:"Weighting Matrix", det_form:"", ket:""},
        {kode:"T#2055R", nama_form:"Direct Award", det_form:"direct-award-form", ket:""}
    ]; 
    
    $scope.back = function(dt) {
        $state.transitionTo('data-contract-requisition');
    };
});