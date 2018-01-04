angular.module('eprocAppPanitia')
.controller('verifikasiTenderCPCtrl', function ($scope, $http, $rootScope, $modal, $cookieStore, $state) {
    $scope.verifikasi_list = [
        {not:"TE#02873654", rfq:"Pekerjaan Pemboran Infil",type:"CSMS",tgl_rev:"01/07/2015",status:"Approved",ket:""}
    ];
    
    $scope.detailContract = function(dt) {
        //console.info("detail");
        var modalInstance = $modal.open({
            templateUrl: 'application/modules/temporary/verifikasi-tender/detailContractModal.html',
            controller: 'detailContractCtrl',
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
            templateUrl: 'application/modules/temporary/verifikasi-tender/formApprovalModal.html',
            controller: 'detailApprovalCPCtrl',
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