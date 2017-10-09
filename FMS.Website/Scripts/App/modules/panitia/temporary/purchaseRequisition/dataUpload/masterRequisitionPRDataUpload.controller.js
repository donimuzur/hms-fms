angular.module('eprocAppPanitia')
.controller('masterRequisitionPRDataUploadCtrl', function ($scope, $http, $rootScope, $modal, $cookieStore, $state) {
    $scope.monitoring_list = [
        {tender:"Pengadaan Kontrak A",not:"TE#00002205R", nok:"46005652",nm_vendor:"PT.Metalindo Indonesia",
            nilai_k:"IDR 250.000.000",sisa_k:"IDR  50.000.000",tot_dist:"IDR 200.000.000",ket:""},
        {tender:"Pengadaan Kontrak B",not:"TE#00003405", nok:"46005666",nm_vendor:"PT.KSU Nevida",
            nilai_k:"IDR 500.000.000",sisa_k:"IDR 380.000.000",tot_dist:"IDR 120.000.000",ket:""}
    ];
    
//    $scope.detailDistribusi = function(dt) {
//        //console.info("detail");
//        var modalInstance = $modal.open({
//            templateUrl: 'application/modules/temporary/monitoring-kontrak-jasa/detailDistribusi.html',
//            controller: 'detailDistribusiCtrl',
//            resolve: {
//                item: function() {
//                    return dt;
//                }
//            }
//        });
//        modalInstance.result.then(function() {
//            $scope.init;
//        });
//    };    
    
    
});