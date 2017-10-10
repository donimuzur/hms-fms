angular.module('eprocAppPanitia')
.controller('dataContractReqCtrl', function ($scope, $http, $rootScope, $modal, $cookieStore, $state) {
    $scope.paketpengadaan_list = [
        {not:"TE#02762563", rfq:"Tender Jasa Konsultan A",type:"CSMS",tgl_rev:"01/07/2015", 
            jns_tender:"Direct Award",status:"Approved",ket:""},
        {not:"TE#02762563", rfq:"Tender Jasa Konsultan B",type:"CSMS",tgl_rev:"01/07/2015", 
            jns_tender:"Direct Award",status:"OnProcess",ket:""},
        {not:"TE#67534763", rfq:"Tender Jasa Konsultan C",type:"CSMS",tgl_rev:"01/07/2015", 
            jns_tender:"Open Tender",status:"Approved",ket:""}
    ];
    
    $scope.formCSMSDec = function(dt) {
         $state.transitionTo('csms-decision');
    };
    
    $scope.detailContract = function(dt) {
        $state.transitionTo('detail-contract-requisition');
    };
    
});