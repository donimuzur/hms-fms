angular.module('eprocAppPanitia')
.controller('aturSubCostEstimateCtrl', function ($scope, $http, $filter, $rootScope, $modal, $state, $cookieStore, $stateParams) {
    var page_id = 144;
    var pr_id = 7;

    $scope.userBisaMengatur = false;
    $scope.subPekerjaan = [];
    $scope.namaPR = "";
    $scope.confirmed;
    $scope.srcText = "";

    $scope.init = function () {
        $rootScope.getSession().then(function (result) {
            $rootScope.userSession = result.data.data;
            $rootScope.userLogin = $rootScope.userSession.session_data.username;
            $rootScope.authorize(loadawal());
        });
    };
    function loadawal() {
        $http.post($rootScope.url_api + "roles/check_authority",
                {username: $rootScope.userLogin, page_id: page_id, jenis_mengatur: 1}
        ).success(function (reply) {
            if (reply.status === 200) {
                var data = reply.result.data[0];
                $scope.userBisaMengatur = data;
            } else {
                $.growl.error({message: "Gagal mendapatkan data hak akses!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
            return;
        });
        $http.post($rootScope.url_api + "PR/namePR", {pr_id: pr_id})
                .success(function (reply) {
                    //console.info("reply: "+JSON.stringify(reply));
                    if (reply.status === 200) {
                        var data = reply.result.data;
                        $scope.namaPR = data[0].pr_nama;
                        $scope.confirmed = Number(data[0].confirmed);
                        console.info($scope.confirmed);
                    } else {
                        $.growl.error({message: "Gagal Mendapatkan Data Nama PR!!"});
                        return;
                    }
                }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
            return;
        });
        $scope.loadSubPekerjaan();
    }

    $scope.loadSubPekerjaan = function () {
        $http.post($rootScope.url_api + "PR/aturPRline", {
            pr_id: pr_id,
            keyword: $scope.srcText
        }).success(function (reply) {
            $rootScope.unloadLoading();
            console.info(pr_id+"__"+$scope.srcText+" reply PR: "+JSON.stringify(reply) );
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.i = 1;
                var sub = [];
                for (var i = 0; i < data.length; i++) {
                    if (data[i].parent === null) {
                        data[i].urutan_sub = romanize($scope.i);
                        $scope.i++;
                    } else {
                        data[i].urutan_sub = String(data[i].urutan_sub);
                    }
                    sub.push(data[i]);
                }
                $scope.subPekerjaan = sub;
                var totallines = 0;

                for (var i = 0; i < $scope.subPekerjaan.length; i++) {
                    $scope.subPekerjaan[i].lines = totallines;
                    $scope.subPekerjaan[i].newAttrib = 'newAttrib';
                }
                for (var i = 0; i < $scope.subPekerjaan.length; i++) {
                    for (var j = 0; j < $scope.subPekerjaan[i].pr_line.length; j++) {
                        if ($scope.subPekerjaan[i].pr_subpekerjaan_id === $scope.subPekerjaan[i].pr_line[j].pr_subpekerjaan_id) {
                            totallines = $scope.subPekerjaan[i].lines + Number($scope.subPekerjaan[i].pr_line[j].prl_line_cost);
                            $scope.subPekerjaan[i].lines = totallines;
                        }
                    }
                }
            } else {
                $.growl.error({message: "Gagal Mendapatkan Data PR!!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
            return;
        });
    };

    $scope.tambahSubPekerjaan = function () {
        var lempar = {
            pr_id: pr_id
        };
        var modalInstance = $modal.open({
            templateUrl: 'BuatSubPekerjaan.html',
            controller: BuatSubPekerjaanCtrlnya,
            resolve: {
                item: function () {
                    return lempar;
                }
            }
        });
        modalInstance.result.then(function () {
            $scope.init();
        });
    };

    $scope.confirm = function () {
        if ($scope.subPekerjaan.length === 0) {
            $.growl.error({title: "[ERROR]", message: "Belum ada sub pekerjaan yang ditambahkan"});
            return;
        } else {
            var lempar = {
                pr_id: pr_id,
                namaPR: $scope.namaPR,
                username: $rootScope.userLogin,
                subpekerjaan: $scope.subPekerjaan
            };
            var modalInstance = $modal.open({
                templateUrl: 'confirmAturPR.html',
                controller: confirmAturPRCtrl,
                resolve: {
                    item: function () {
                        return lempar;
                    }
                }
            });
            modalInstance.result.then(function () {
                $scope.init();
            });
        }
    };

    $scope.detailSubPekerjaan = function (pr_subpekerjaan_id, nama) {
        var lempar = {
            id: pr_subpekerjaan_id,
            nama: nama
        };
        $modal.open({
            templateUrl: 'detailSubPekerjaan.html',
            controller: detailSubPekerjaanCtrl,
            resolve: {
                item: function () {
                    return lempar;
                }
            }
        });
    };

    $scope.ubah = function (pr_subpekerjaan_id, nama, urutan, parent) {
        var lempar = {
            pr_id: pr_id,
            idSubPekerjaan: pr_subpekerjaan_id,
            nama: nama,
            urutanshow: false,
            urutan: 0,
            parent: 0
        };
        var modalInstance = $modal.open({
            templateUrl: 'UbahSubPekerjaan.html',
            controller: UbahSubPekerjaanCtrl,
            resolve: {
                item: function () {
                    return lempar;
                }
            }
        });
        modalInstance.result.then(function () {
            $scope.init();
        });
    };

    $scope.ubahsub = function (pr_subpekerjaan_id, nama, urutan, parent) {
        var lempar = {
            pr_id: pr_id,
            idSubPekerjaan: pr_subpekerjaan_id,
            nama: nama,
            urutanshow: true,
            urutan: urutan,
            parent: parent
        };
        var modalInstance = $modal.open({
            templateUrl: 'UbahSubPekerjaan.html',
            controller: UbahSubPekerjaanCtrl,
            resolve: {
                item: function () {
                    return lempar;
                }
            }
        });
        modalInstance.result.then(function () {
            $scope.init();
        });
    };

    $scope.tambahSubUntukSub = function (pr_subpekerjaan_id) {
        var lempar = {
            pr_id: pr_id,
            pr_subpekerjaan_id: pr_subpekerjaan_id
        };
        var modalInstance = $modal.open({
            templateUrl: 'BuatSubUntukSub.html',
            controller: BuatSubUntukSubCtrl,
            resolve: {
                item: function () {
                    return lempar;
                }
            }
        });
        modalInstance.result.then(function () {
            $scope.init();
        });
    };

    $scope.hapus = function (pr_subpekerjaan_id) {
        var lempar = {pr_subpekerjaan_id: pr_subpekerjaan_id, username: $rootScope.userLogin};
        var modalInstance = $modal.open({
            templateUrl: 'konfirmasiHapusSubPekerjaan.html',
            controller: konfirmasiHapusSubPekerjaanCtrl,
            resolve: {
                item: function () {
                    return lempar;
                }
            }
        });
        modalInstance.result.then(function () {
            $scope.init();
        });
    };

    function romanize(num) {
        if (!+num)
            return false;
        var digits = String(+num).split(""),
                key = ["", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM",
                    "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC",
                    "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX"],
                roman = "",
                i = 3;
        while (i--)
            roman = (key[+digits.pop() + (i * 10)] || "") + roman;
        return Array(+digits.join("") + 1).join("M") + roman;
    }
})