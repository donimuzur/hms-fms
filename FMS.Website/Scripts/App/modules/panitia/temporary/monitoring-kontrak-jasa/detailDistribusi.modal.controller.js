angular.module('eprocAppPanitia')
.controller('detailDistribusiCtrl', function ($scope, $http, $rootScope, $modalInstance, $modal, $state) {
    $scope.formList = [
        {th_dist:"Year 1", budget:"IDR 80.000.000", no_po:"4502651654", tgl_po:"20-Mar-2016",status_po:"Sent",ket:""},
        {th_dist:"Year 2", budget:"IDR 80.000.000", no_po:"4502651376", tgl_po:"20-Jun-2016",status_po:"Not Sent",ket:""},
        {th_dist:"Year 3", budget:"IDR 40.000.000", no_po:"4502651298", tgl_po:"20-Sep-2016",status_po:"Sent",ket:""}
    ]; 
    
    $scope.openFormDistribusi = function(dt) {
        console.info("okeee");
        var modalInstance = $modal.open({
            templateUrl: 'application/modules/temporary/monitoring-kontrak-jasa/formDistribusiBudget.html',
            controller: 'formDistribusiBudgetCtrl',
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
    
    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };  
});