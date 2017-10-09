angular.module('eprocAppPanitia')
.controller('detailAdendumCtrl', function ($scope, $http, $rootScope, $modal) {
    $scope.detailAdendumList = [
        {adendum:"Add#01", start_c:"01-01-2011", finish_c:"31-03-2011",value:"IDR 1.104.818.090.00", remark:""},
        {adendum:"Add#02", start_c:"01-06-2011", finish_c:"31-03-2011",value:"IDR 1.181.063.842.00", remark:""},
        {adendum:"Add#03", start_c:"01-04-2013", finish_c:"31-03-2014",value:"IDR 956.787.131.00", remark:""}
    ];
    
    $scope.formAdendum = function(dt) {
        //console.info("detail");
        var modalInstance = $modal.open({
            templateUrl: 'application/modules/temporary/monitoring-tender-vhs/formAdendum.html',
            controller: 'formAdendumCtrl',
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