angular.module('eprocAppPanitia').controller('verifikasiKelengkapanDokumenCtrl', function ($scope, $http, $rootScope, $modal, $cookieStore) {
    $scope.totalItems = 0;
    $scope.currentPage = 0;
    $scope.maxSize = 10;
    $scope.Pegawais = [];
    $scope.page_id = 35;
    $scope.userBisaMengatur;
    $scope.kata = new Kata("");
    $scope.menuhome = 0;
    $scope.userId = 0;
    $scope.rfqList = [
        {rfqName: 'PT. Malifak Maju Jaya', rfqDate: '2016-01-01', status: 'Lulus', remark: ''},
        {rfqName: 'Vendor B', rfqDate: '2016-01-02', status: 'Tidak Lulus', remark: ''},
        {rfqName: 'Vendor C', rfqDate: '2016-01-03', status: 'Lulus', remark: ''},
        {rfqName: 'Vendor D', rfqDate: '2016-01-04', status: 'Lulus', remark: ''},
        {rfqName: 'Vendor E', rfqDate: '2016-01-05', status: 'Lulus', remark: ''},
        {rfqName: 'Vendor F', rfqDate: '2016-01-06', status: 'Tidak Lulus', remark: ''},
        {rfqName: 'Vendor G', rfqDate: '2016-01-07', status: 'Tidak Lulus', remark: ''},
        {rfqName: 'Vendor H', rfqDate: '2016-01-08', status: 'Tidak Lulus', remark: ''},
        {rfqName: 'Vendor I', rfqDate: '2016-01-09', status: 'Lulus', remark: ''},
        {rfqName: 'Vendor J', rfqDate: '2016-01-10', status: 'Lulus', remark: ''}
    ];

    $scope.loadPegawai = function () {
        $rootScope.getSession().then(function (result) {
            $rootScope.userSession = result.data.data;
            //console.info($rootScope.userSession);
            $rootScope.userLogin = $rootScope.userSession.session_data.username;
            $scope.userId = Number($rootScope.userSession.session_data.pegawai_id);
            $rootScope.authorize(loadData());
        });
    };

    /* awal pencarian  edit by ani */
    $scope.cariPegawai = function () {
        $http.post($rootScope.url_api + "pegawai/pegawaicountsearch", {
            nama_pegawai: "%" + $scope.kata.srcText + "%", "offset": $scope.currentPage, "limit": $scope.maxSize
        }).success(function (reply) {
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.totalItems = data;
            } else {
                $.growl.error({message: "Gagal mendapatkan data jumlah pegawai!"});
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
        });

        $http.post($rootScope.url_api + "pegawai/pegawaisearch", {
            nama_pegawai: "%" + $scope.kata.srcText + "%", "offset": $scope.currentPage, "limit": $scope.maxSize
        }).success(function (reply) {
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.Pegawais = data;
            } else {
                $.growl.error({message: "Gagal mendapatkan data pegawai!"});
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
        });
    };

    function loadData() {
        var param = [];
        param.push($rootScope.userlogged);
        param.push($scope.page_id);

        $http.post($rootScope.url_api + "roles/check_authority", {
            username: $rootScope.userLogin, page_id: $scope.page_id, jenis_mengatur: 1
        }).success(function (reply) {
            if (reply.status === 200) {
                var data = reply.result.data[0];
                $scope.userBisaMengatur = $rootScope.strtobool(data.bisa_mengatur);
            } else {
                $.growl.error({message: "Gagal mendapatkan data jumlah pegawai!"});
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
        });

        $http.post($rootScope.url_api + "pegawai/pegawaicountsearch", {
            nama_pegawai: "%" + $scope.kata.srcText + "%", "offset": $scope.currentPage, "limit": $scope.maxSize
        }).success(function (reply) {
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.totalItems = data;
            } else {
                $.growl.error({message: "Gagal mendapatkan data jumlah pegawai!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
            return;
        });

        $http.post($rootScope.url_api + "pegawai/pegawaisearch", {
            nama_pegawai: "%" + $scope.kata.srcText + "%", "offset": 0, "limit": $scope.maxSize
        }).success(function (reply) {
            //console.info("reply: "+JSON.stringify(reply));
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.Pegawais = data;
            } else {
                $.growl.error({message: "Gagal mendapatkan data pegawai!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
            return;
        });
    }
    $scope.jLoad2 = function (current) {
        $scope.Pegawais = [];
        $scope.currentPage = current;
        $scope.offset = (current * 10) - 10;
        $rootScope.authorize($http.post($rootScope.url_api + "pegawai/pegawaisearch", {
            nama_pegawai: "%" + $scope.kata.srcText + "%", "offset": $scope.offset, "limit": $scope.maxSize
        }).success(function (reply) {
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.Pegawais = data;
            } else {
                $.growl.error({message: "Gagal mendapatkan data pegawai!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
            return;
        }));
    };

   /*tamplikan modal detail evaluasi*/
    $scope.detailVerifikasiView = function (size) {
        var item = {userId: $scope.userId};
        var modalInstance = $modal.open({
            templateUrl: 'detailVerifikasi.html',
            controller: detailVerifikasiModal,
            size: size,
            resolve: {item: function () {
                    return item;
                }}
        });
        modalInstance.result.then(function (newPeg) {
            $rootScope.authorize(loadData());
        });
    };

    var detailVerifikasiModal = function ($scope, $modalInstance, $http, item, $rootScope, $modal) {
        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
        //$scope.
        //alert();
    };
    });



    function resetPass(Id, newPass, confirmPass) {
        var self = this;
        self.Id = Id;
        self.newPass = newPass;
        self.confirmPass = confirmPass;
    }
    function Kata(srcText) {
        var self = this;
        self.srcText = srcText;
    }