angular.module('eprocAppPanitia')
.controller('paketPengadaanCECtrl', function ($scope, $http, $rootScope, $modal, $cookieStore, $state) {
    $scope.paketpengadaan_list = [
        {not:"TE#02762563", rfq:"Tender Jasa Konsultan A",type:"CSMS",tgl_rev:"01/07/2015", 
            jns_tender:"Direct Award",status:"Publish",ket:""},
        {not:"TE#02762563", rfq:"Tender Jasa Konsultan B",type:"CSMS",tgl_rev:"01/07/2015", 
            jns_tender:"Direct Award",status:"Publish",ket:""},
        {not:"TE#67534763", rfq:"Tender Jasa Konsultan C",type:"CSMS",tgl_rev:"01/07/2015", 
            jns_tender:"Open Tender",status:"Publish",ket:""}
    ];
    
    $scope.aturPaket = function(dt) {
        //console.info("detail");
        var modalInstance = $modal.open({
            templateUrl: 'application/modules/temporary/paket-pengadaan-ce/aturPaketPengadaan.html',
            controller: 'aturPaketPengadaanCECtrl',
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
    
    $scope.detailKelengkapan = function(dt) {
        var modalInstance = $modal.open({
            templateUrl: 'application/modules/temporary/paket-pengadaan-ce/detailKelengkapanTender.html',
            controller: 'detailKelengkapanTenderCECtrl',
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