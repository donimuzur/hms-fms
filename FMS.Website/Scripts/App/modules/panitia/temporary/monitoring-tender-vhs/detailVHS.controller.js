angular.module('eprocAppPanitia')
.controller('detailVHSCtrl', function ($scope, $http, $rootScope, $modal, $state) {
    $scope.detailVhsList = [
        {date:"25-07-2016", reserv:"16305853", res_i:"1",mat_doc:"4911672306",mat_doc_i:"2",mat:"15081432",qty:1,
         unit:"EA",currency:"USD",price:"132.06",total:"132.06",status:"Belum Terkirim"   
        }
    ]; 
    
});