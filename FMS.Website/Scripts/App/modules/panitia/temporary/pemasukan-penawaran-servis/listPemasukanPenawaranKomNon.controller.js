angular.module('eprocAppPanitia')
.controller('listPemasukanPenawaranKomNonCtrl', function ($scope, $http, $rootScope, $cookieStore, $state) {
    $scope.vendorList = [
        {kode:"T#2205R", nama_vendor:"PT. Malifax Maju Jaya", tgl:"02/10/2016", ket:""},
        {kode:"T#2055", nama_vendor:"PT. Bujaya Karya Makmur", tgl:"02/10/2016", ket:""},
        {kode:"T#2055R", nama_vendor:"PT. Indotruck Utama", tgl:"02/10/2016", ket:""}
    ];
    
});