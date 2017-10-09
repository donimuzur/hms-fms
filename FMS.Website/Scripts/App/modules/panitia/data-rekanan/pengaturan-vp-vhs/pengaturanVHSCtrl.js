angular.module('eprocAppPanitia')
.controller( 'pengaturanVHSCtrl', function( $scope, $http, $rootScope, $modal, $state, $cookieStore ){
    $scope.init = function(){
        $scope.kpiList = [
            {kpi_name: "Pengelolaan Proyek", kpi_sub:[{sub_name:"Kuantitas", 
                        detail_nilai:[{skor:1, det_skor:"Below Plan"},{skor:2, det_skor:"On Plan"},
                        {skor:3, det_skor:"Above Plan"}]},
                {sub_name:"Mutu", 
                        detail_nilai:[{skor:1, det_skor:"Not Suitable"},{skor:2, det_skor:"Suitable"}]}        
            ]},
            {kpi_name: "Pengelolaan Sumber Daya", kpi_sub:[{sub_name:"Tenaga Kerja", 
                        detail_nilai:[{skor:1, det_skor:"Below Plan"},{skor:2, det_skor:"On Plan"},
                        {skor:3, det_skor:"Above Plan"}]}]},
            {kpi_name: "Keselamatan dan Kesehatan Kerja"},
            {kpi_name: "Pengelolaan Proyek"},
            {kpi_name: "Organisasi dan Perhatian Terhadap Persyaratan"}
        ];
    };
    
    $scope.tambah_kpi = function(id, data){
        var kirim = {id_data: id, data: data};
        var modalInstance = $modal.open({
            templateUrl: 'formTambahKPI.html',
            controller: formKPICtrl,
            resolve: {
                item: function() {
                    return kirim;
                }
            }
        });
        modalInstance.result.then(function() {
            $scope.init();
        });
    };
    
    $scope.ubahkriteria = function(data){
        var modalInstance = $modal.open({
            templateUrl: 'formUbahkriteria.html',
            controller: formKriteriaCtrl,
            resolve: {
                item: function() {
                    return data;
                }
            }
        });
        modalInstance.result.then(function() {
            $scope.init();
        });
    };
    
    $scope.formnilai = function(data){
        var modalInstance = $modal.open({
            templateUrl: 'formNilaiSub.html',
            controller: formNilaiCtrl,
            resolve: {
                item: function() {
                    return data;
                }
            }
        });
        modalInstance.result.then(function() {
            $scope.init();
        });
    };
});

var formNilaiCtrl = function($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
    
};

var formKriteriaCtrl = function($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
    
};

var formKPICtrl = function($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
    console.info(JSON.stringify(item));
        
    $scope.batal = function() {
        $modalInstance.dismiss('cancel');
    };    
};
;