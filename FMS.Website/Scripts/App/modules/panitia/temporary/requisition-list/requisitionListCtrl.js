angular.module('eprocAppPanitia')
.controller('requisitionListTCCtrl', function ($scope, $http, $rootScope, $modal, $cookieStore, $state) {
    $scope.requi_list = [
        {not:"TE#02873654", rfq:"Tender Jasa Konsultan A",type:"CSMS",tgl_rev:"01/07/2015",status:"Approved",ket:""},
        {not:"TE#02873653", rfq:"Tender Jasa Konsultan B",type:"CSMS",tgl_rev:"03/07/2015",status:"Pending",ket:""}
    ];
    
    $scope.detailContract = function(dt) {
        //console.info("detail");
        var modalInstance = $modal.open({
            templateUrl: 'application/modules/temporary/requisition-list/detailContractModal.html',
            controller: 'detailContractTCCtrl',
            resolve: {
                item: function() {
                    return dt;
                }
            }
        });
        modalInstance.result.then(function() {
            $scope.init;
        });
    };
    
    $scope.detailApproval = function(dt) {
        var modalInstance = $modal.open({
            templateUrl: 'application/modules/temporary/requisition-list/formApprovalModal.html',
            controller: 'detailApprovalTCCtrl',
            resolve: {
                item: function() {
                    return dt;
                }
            }
        });
        modalInstance.result.then(function() {
            $scope.init;
        });
    };
    
});