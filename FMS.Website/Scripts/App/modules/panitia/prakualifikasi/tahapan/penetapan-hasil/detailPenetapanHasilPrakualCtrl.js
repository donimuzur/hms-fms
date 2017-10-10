angular.module('eprocAppPanitia')
.controller('detailPenetapanHasilPraKualCtrl', function($state, $scope, $http, $rootScope, $cookieStore, $stateParams) {
    $scope.flowpaket_id = Number($stateParams.flowpaket_id);
    $scope.paket_lelang_id = Number($stateParams.paket_lelang_id);
    $scope.lihatAktaPerusahaan = function() {
        $state.transitionTo('detail-akta-prakualifikasi', {
            flowpaket_id: $scope.flowpaket_id,
            paket_lelang_id: $scope.paket_lelang_id
        });
    };
})

.controller('detailAktaPraKualCtrl', function($state, $scope, $http, $rootScope, $cookieStore, $stateParams) {
    $scope.flowpaket_id = Number($stateParams.flowpaket_id);
    $scope.paket_lelang_id = Number($stateParams.paket_lelang_id);
    $scope.lihatIzinUsaha = function(rekanan_id) {
        $state.transitionTo('detail-ijinusaha-prakualifikasi', {
            flowpaket_id: $scope.flowpaket_id,
            paket_lelang_id: $scope.paket_lelang_id
        });
    };
})

.controller('detailIjinUsahaPraKualCtrl', function($state, $scope, $http, $rootScope, $cookieStore, $stateParams, $modal) {
    $scope.flowpaket_id = Number($stateParams.flowpaket_id);
    $scope.paket_lelang_id = Number($stateParams.paket_lelang_id);
    $scope.lihatKeuangan = function() {
        $state.transitionTo('detail-keuangan-prakualifikasi', {
            flowpaket_id: $scope.flowpaket_id,
            paket_lelang_id: $scope.paket_lelang_id
        });
    };
})

.controller('detailKeuanganPraKualCtrl', function($state, $scope, $http, $rootScope, $cookieStore, $stateParams, $modal) {
    $scope.flowpaket_id = Number($stateParams.flowpaket_id);
    $scope.paket_lelang_id = Number($stateParams.paket_lelang_id);
    $scope.lihatAktaPerusahaan = function(rekanan_id) {
        $state.transitionTo('detail-ijinusaha-prakualifikasi', {
            flowpaket_id: $scope.flowpaket_id,
            paket_lelang_id: $scope.paket_lelang_id
        });
    };
    
    $scope.list_kekayaan = [
        {no:1,jenis_harta:"Uang Tunai/Tabungan",jumlah:"10 Gram",nilai:"980000000",ket:"rek. PT.Aneka"},
        {no:2,jenis_harta:"Logam Mulia",jumlah:"10 Gram",nilai:"26478100",ket:"-"}
    ];
    
    $scope.list_hutang = [
        {no:1,jenis_harta:"Gaji Karyawan Terhutang",nilai:"980.000.000",ket:"rek. PT.Aneka"},
        {no:2,jenis_harta:"Sewa Peralatan",nilai:"26.478.100",ket:"-"},
        {no:3,jenis_harta:"Asuransi (diluar Jamsostek dan BPJS Kesehatan)",nilai:"",ket:"-"},
        {no:4,jenis_harta:"Cicilan Kendaraan",nilai:"890.324.000",ket:"-"},
        {no:5,jenis_harta:"Hutang Pinjaman Kendaraan",nilai:"10.324.000",ket:"-"}
    ];
    
    $scope.list_kendaraan = [
        {no:1,jenis:"Nissan CWB4554DLN Dump Truck 1909",tahun:"2009",nilai:"1.090.909.000"},
        {no:2,jenis:"Nissan CWB4554DLN Dump Truck 2909",tahun:"2011",nilai:"890.909.000"},
        {no:3,jenis:"Nissan CWB4554DLN Dump Truck 1278",tahun:"2010",nilai:"2.095.909.000"}
    ];
    
    $scope.list_peralatan = [
        {no:1,jenis:"Komatsu Hydralic 1909",tahun:"2009",nilai:"1.090.909.000",masa:3,nil_dep:"201.190.000",dep:50},
        {no:2,jenis:"Komatsu Hydralic 2909",tahun:"2011",nilai:"890.909.000",masa:2,nil_dep:"790.000",dep:30},
        {no:3,jenis:"Komatsu Hydralic  1278",tahun:"2010",nilai:"2.095.909.000",masa:4,nil_dep:"8.190.000",dep:20}
    ];
    
    $scope.aturIndeksKendaraan = function(dt) {
        var modalInstance = $modal.open({
            templateUrl: 'aturIndeksKendaraan.html',
            controller: aturIndeksKendaraanCtrl,
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
    
    $scope.aturIndeksPeralatan = function(dt) {
        var modalInstance = $modal.open({
            templateUrl: 'aturIndeksPeralatan.html',
            controller: aturIndeksPeralatanCtrl,
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
    
    $scope.kuesioner_k3l = function() {
        $state.transitionTo('kuesionerK3L-prakualifikasi', {
            flowpaket_id: $scope.flowpaket_id,
            paket_lelang_id: $scope.paket_lelang_id
        });
    };
});

var aturIndeksKendaraanCtrl = function($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
       
    $scope.batal = function() {
        $modalInstance.dismiss('cancel');
    };    
};

var aturIndeksPeralatanCtrl = function($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
       
    $scope.batal = function() {
        $modalInstance.dismiss('cancel');
    };    
};

;