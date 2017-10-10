angular.module('eprocAppPanitia')
.controller('kuesionerK3LPraKualCtrl', function($state, $scope, $http, $rootScope, $cookieStore, $stateParams) {
    $scope.flowpaket_id = Number($stateParams.flowpaket_id);
    $scope.paket_lelang_id = Number($stateParams.paket_lelang_id);
    
    $scope.kuesioner_om = function() {
        $state.transitionTo('kuesionerOM-prakualifikasi', {
            flowpaket_id: $scope.flowpaket_id,
            paket_lelang_id: $scope.paket_lelang_id
        });
    };
    
    $scope.list_questions = [
        {no:1, question:"Apakah perusahaan memiliki kebijakn K3L dalam menjalankan usahanya?", 
            answer :[{id:1, detail:"Ya "},{id:2, detail:"Tidak"}]},
        {no:2, question:"Apakah perusahaan memiliki prosedur tanggap darurat dan sampai sejauh mana perusahaan menerapkan\n\
            Prosedur tanggap darurat tersebut?", 
            answer :[{id:1, detail:"Ya "},{id:2, detail:"Tidak"}]}
    ];
})

.controller('kuesionerOMPraKualCtrl', function($state, $scope, $http, $rootScope, $cookieStore, $stateParams) {
    $scope.flowpaket_id = Number($stateParams.flowpaket_id);
    $scope.paket_lelang_id = Number($stateParams.paket_lelang_id);
    
    $scope.kuesioner_sd = function() {
        $state.transitionTo('kuesionerSD-prakualifikasi', {
            flowpaket_id: $scope.flowpaket_id,
            paket_lelang_id: $scope.paket_lelang_id
        });
    };
    
    $scope.list_questions = [
        {no:1, question:"Apakah pengurus perusahaan telah menerapkan struktur oranisasi perusahaan?", 
            answer :[{id:1, detail:"Ya "},{id:2, detail:"Tidak"}]},
        {no:2, question:"Apakah pengurus perusahaan mengkomunikasikan struktur organisasi tersebut di dalam organisasi?", 
            answer :[{id:1, detail:"Ya "},{id:2, detail:"Tidak"}]}
    ];
})

.controller('kuesionerSDPraKualCtrl', function($state, $scope, $http, $rootScope, $cookieStore, $stateParams) {
    $scope.flowpaket_id = Number($stateParams.flowpaket_id);
    $scope.paket_lelang_id = Number($stateParams.paket_lelang_id);
    
    $scope.kuesioner_mpp = function() {
        $state.transitionTo('kuesionerMPP-prakualifikasi', {
            flowpaket_id: $scope.flowpaket_id,
            paket_lelang_id: $scope.paket_lelang_id
        });
    };
    
    $scope.list_questions = [
        {no:1, question:"Apakah perusahaan menyediakan dan mengelola prasarana dan peralatan kerja yang sesuai dengan\n\
            kegiatan usahanya?", 
            answer :[{id:2, detail:"Tidak Memiliki struktur/peralatan"},{id:1, detail:"Ya "}]},
        {no:2, question:"Apakah perusahaan memelihara prasarana dan peralatannya secara baik?", 
            answer :[{id:1, detail:"Prasarana dan peralatan tidak dipelihara secara baik"},{id:2, detail:"Tidak"}]}
    ];
})

;