angular.module('eprocAppPanitia')
.controller( 'rekananDitolakCtrl', function( $scope, $http, $cookieStore, $state, $rootScope ){        
    $scope.totalRecords = 0;
    $scope.currentPage = 1;
    $scope.fullSize = 10;

    $scope.dataRekanan = [];
    $scope.menuhome = 0;
    $scope.txtcari = "";

    $scope.init = function(){
        $scope.menuhome = $rootScope.menuhome;
        $rootScope.getSession().then(function(result){
            $rootScope.userSession = result.data.data;
            $rootScope.userLogin = $rootScope.userSession.session_data.username;
            $rootScope.authorize($scope.jLoad2(1));
        });
    };

    $scope.jLoad2 = function(current) {
        $scope.dataRekanan = [];
        $scope.currentPage = current;
        $scope.offset = (current * 10) - 10;
        $rootScope.loadLoading("Silahkan Tunggu...");
        
        $http.post($rootScope.url_api+"rekanan/ditolak/count",{txtcari: "%%"})
        .success(function(reply){
            if(reply.status === 200){
                var data = reply.result.data;
                $scope.totalRecords = data;

            }
            else{
                $.growl.error({ message: "Gagal mendapatkan jumlah data rekanan ditolak!!" });
                $rootScope.unloadLoading();
                return;
            }
        })
        .error(function(err) {
            $.growl.error({ message: "Gagal Akses API >"+err });
            $rootScope.unloadLoading();
            return;
        });
        
        $http.post($rootScope.url_api+"rekanan/ditolak/select",{
            offset: $scope.offset,
            limit: 10,
            param: "%" + $scope.txtcari + "%"})
        .success(function(reply){
            if(reply.status === 200){
                var data = reply.result.data;
                $scope.dataRekanan = data;
                $rootScope.unloadLoading();
            }
            else{
                $.growl.error({ message: "Gagal mendapatkan jumlah data rekanan ditolak!!" });
                $rootScope.unloadLoading();
                return;
            }
        })
        .error(function(err) {
            $.growl.error({ message: "Gagal Akses API >"+err });
            $rootScope.unloadLoading();
            return;
        });

    };

    $scope.cari = function() {

        $rootScope.authorize($scope.jLoad2(1));
    };
}); // end rekananDitolakCtrl