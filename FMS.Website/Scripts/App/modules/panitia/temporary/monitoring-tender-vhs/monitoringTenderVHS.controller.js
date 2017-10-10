angular.module('eprocAppPanitia')
.controller('monitoringTenderVhsCtrl', function ($scope, $http, $rootScope, $modal, $state) {
    $scope.monitoring_list = [
        {sloc:"VH03",sp_cn:"4600021562",title:"VHS Plywoood", name_ven:"CV. Towuti Raya",
            v_code:"40006152", start_c:"23-Feb-15", finish_c:"01-Mar-18",target_v:"IDR 2.995.948.250.00",
            remarks:""},
        {sloc:"VH10",sp_cn:"460004712",title:"VHS Safety Consumable", name_ven:"CV. Elm Putri",
            v_code:"40006152", start_c:"01-Sep-15", finish_c:"31-Agst-16",target_v:"IDR 19.115.524.575.00",
            remarks:""}
    ];
    
    $scope.detailVHS = function(dt) {
        $state.transitionTo('detail-vhs-mt');
    };    
    
    $scope.detailAdendum = function(dt) {
        $state.transitionTo('detail-adendum-mt');
    };
    
});