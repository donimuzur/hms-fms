angular.module('eprocAppPanitia')
.controller('detailCostEstimateCtrl', function ($scope, $http, $rootScope, $modal, $cookieStore, $state) {
    $scope.monitoring_list = [
    {desc:"Mobilisasi dan Demobilisasi alat dan personel",qty:"1",unit:"LS",ucost:"Rp. 80.000.000.000,00",
        lcost:"Rp. 80.000.000.000,00",tax:"Rp 0,00"},
    {desc:"Infill drilling - open hole (HQ) 0-150m",qty:"1687",unit:"meter",ucost:"Rp. 475.000.000,00",
        lcost:"Rp. 801.000.000,00",tax:"Rp 0,00"}    
    ];
    
    $scope.back = function() {
        $state.transitionTo('detail-contract-requisition');
    };
    
    $scope.aturSubCost = function() {
        $state.transitionTo('atur-subcost-estimate');
    };
    
});