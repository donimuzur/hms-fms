angular.module('eprocAppPanitia')
        .controller('dataPrakualifikasiCtrl', function($scope, $http, $rootScope, $state, $cookieStore, $modal, ngProgress) {

            $scope.jumlahRequestAktivasi = 0;
            $scope.jumlahRequestVerifikasi = 0;
            $scope.jumlahRequestUbahData = 0;

            $scope.totalItems = 0;
            $scope.currentPage = 1;
            $scope.maxSize = 10;
            $scope.paket = [];
            $scope.statusTercentang = false;
            $scope.namaLelangTercentang = false;
            $scope.namaTahapanTercentang = false;
            $scope.selectedStatus = "5";
            $scope.srcNamaLelang = "";
            $scope.srcNamaTahapan = "";
            $scope.page_id = 154;

            $scope.totalItemsPekerjaan = 0;
            $scope.currentPagePekerjaan = 1;
            $scope.maxSizePekerjaan = 10;
            $scope.pekerjaan = [];
            $scope.srcTextPekerjaan = "";

            $scope.namaPekerjaanTercentang = false;
            $scope.waktuMulaiTercentang = false;
            $scope.waktuSelesaiTercentang = false;
            $scope.waktuMulai1 = "";
            $scope.waktuMulai2 = "";
            $scope.waktuSelesai1 = "";
            $scope.waktuSelesai2 = "";

            $scope.init = function() {
                $rootScope.getSession().then(function(result) {
                    $rootScope.userSession = result.data.data;
                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
                    $rootScope.authorize(new function() {
                        loadPaket();
                    });
                });
            };

            function loadPaket() {

                $rootScope.readNotif(); // AWN
                $scope.currentPage = 1;
                $rootScope.loadLoading("Silahkan Tunggu...");
                
                //itp.paket.countForAdmin
                $http.post($rootScope.url_api + 'paket/admin/count', {
                    page_id: $scope.page_id,
                    statusTercentang: $scope.statusTercentang,
                    namaLelangTercentang: $scope.namaLelangTercentang,
                    namaTahapanTercentang: $scope.namaTahapanTercentang,
                    status: Number($scope.selectedStatus),
                    namaLelang: "%" + $scope.srcNamaLelang + "%",
                    namaTahapan: "%" + $scope.srcNamaTahapan + "%"
                }).success(function(reply) {
                    if (reply.status === 200) {
                        $scope.totalItems = reply.result.data;
                    }
                });

                $scope.paket = [];
                var offset = 0;
                var limit = 10;
                
                //itp.paket.selectForAdmin
                $http.post($rootScope.url_api + 'paket/admin/select', {
                    page_id: $scope.page_id,
                    statusTercentang: $scope.statusTercentang,
                    namaLelangTercentang: $scope.namaLelangTercentang,
                    namaTahapanTercentang: $scope.namaTahapanTercentang,
                    status: Number($scope.selectedStatus),
                    namaLelang: "%" + $scope.srcNamaLelang + "%",
                    namaTahapan: "%" + $scope.srcNamaTahapan + "%",
                    offset: offset,
                    limit: limit
                }).success(function(reply) {
                    if (reply.status === 200) {
                        $scope.paket = reply.result.data;
                        if ($scope.paket.length > 0) {
                            for (var i = 0; i < $scope.paket.length; i++) {
                                $scope.paket[i].tahapanSekarang = [];
                                $scope.paket[i].tahapanNext = [];
                                $scope.paket[i].tahapanLama = [];
                                $scope.paket[i].isKetua = 0;
                            }

                            var arr1;
                            var arr2 = [];

                            for (var i = 0; i < $scope.paket.length; i++) {
                                arr1 = [];
                                arr1.push($scope.paket[i].paket_id);
                                arr2.push(arr1);
                            }

                            //itp.paket.cekKetuaPanitia
                            $http.post($rootScope.url_api + 'paket/cekketua', {
                                username: $rootScope.userLogin
                            }).success(function(reply2) {
                                if (reply2.status === 200) {
                                    $scope.ketuaPanitia = reply2.result.data;
                                    for (var i = 0; i < $scope.paket.length; i++) {
                                        for (var j = 0; j < $scope.ketuaPanitia.length; j++) {
                                            if ($scope.paket[i].paket_id === $scope.ketuaPanitia[j].paket_lelang_id) {
                                                $scope.paket[i].isKetua = $scope.ketuaPanitia[j].ketua_panitia;
                                            }
                                        }
                                    }
                                }
                            });

                            //itp.paket.cekTahapanSekarang
                            $http.post($rootScope.url_api + 'paket/cektahapan', {
                                page_id: $scope.page_id,
                                param: arr2
                            }).success(function(reply2) {
                                if (reply2.status === 200) {
                                    $scope.tahapanSekarang = reply2.result.data;
                                    for (var i = 0; i < $scope.paket.length; i++) {
                                        for (var j = 0; j < $scope.tahapanSekarang.length; j++) {
                                            if ($scope.paket[i].paket_id === $scope.tahapanSekarang[j].paket_lelang_id) {
                                                if ($scope.tahapanSekarang[j].stateNext === 'Curr') {
                                                    $scope.paket[i].tahapanSekarang.push($scope.tahapanSekarang[j]);
                                                } else if ($scope.tahapanSekarang[j].stateNext === 'Next') {
                                                    $scope.paket[i].tahapanNext.push($scope.tahapanSekarang[j]);
                                                } else if ($scope.tahapanSekarang[j].stateNext === 'Old') {
                                                    $scope.paket[i].tahapanLama.push($scope.tahapanSekarang[j]);
                                                }
                                            }
                                        }
                                    }
                                    $rootScope.unloadLoading();
                                    ngProgress.complete();
                                }
                            });
                        } // end: $scope.paket.length > 0 
                        else {
                            $rootScope.unloadLoading();
                            ngProgress.complete();
                        }
                    }
                });
            } // end loadPaket

            $scope.jLoad = function(current) {
                $scope.paket = [];
                $scope.currentPage = current;
                $scope.offset = (current * 10) - 10;
                var limit = 10;
                $rootScope.loadLoading("Silahkan Tunggu...");
                //itp.paket.selectForAdmin
                $rootScope.authorize(
                    $http.post($rootScope.url_api + 'paket/admin/select', {
                        page_id: $scope.page_id,
                        statusTercentang: $scope.statusTercentang,
                        namaLelangTercentang: $scope.namaLelangTercentang,
                        namaTahapanTercentang: $scope.namaTahapanTercentang,
                        status: Number($scope.selectedStatus),
                        namaLelang: "%" + $scope.srcNamaLelang + "%",
                        namaTahapan: "%" + $scope.srcNamaTahapan + "%",
                        offset: $scope.offset,
                        limit: limit
                    }).success(function(reply) {
                        if (reply.status === 200) {
                            $scope.paket = reply.result.data;
                            if ($scope.paket.length > 0) {
                                for (var i = 0; i < $scope.paket.length; i++)
                                {
                                    $scope.paket[i].tahapanSekarang = [];
                                    $scope.paket[i].tahapanNext = [];
                                    $scope.paket[i].tahapanLama = [];
                                    $scope.paket[i].isKetua = 0;
                                }
                                var arr1;
                                var arr2 = [];
                                for (var i = 0; i < $scope.paket.length; i++)
                                {
                                    arr1 = [];
                                    arr1.push($scope.paket[i].paket_id);
                                    arr2.push(arr1);
                                }
                                //itp.paket.cekKetuaPanitia
                                $http.post($rootScope.url_api + 'paket/cekketua', {
                                    username: $rootScope.userLogin
                                }).success(function(reply2) {
                                    if (reply2.status === 200) {
                                        $scope.ketuaPanitia = reply2.result.data;
                                        for (var i = 0; i < $scope.paket.length; i++) {
                                            for (var j = 0; j < $scope.ketuaPanitia.length; j++) {
                                                if ($scope.paket[i].paket_id === $scope.ketuaPanitia[j].paket_lelang_id) {
                                                    $scope.paket[i].isKetua = $scope.ketuaPanitia[j].ketua_panitia;
                                                }
                                            }
                                        }
                                    }
                                });

                                //itp.paket.cekTahapanSekarang
                                $http.post($rootScope.url_api + 'paket/cektahapan', {
                                    page_id: $scope.page_id,
                                    param: arr2
                                }).success(function(reply2) {
                                    if (reply2.status === 200) {
                                        $scope.tahapanSekarang = reply2.result.data;
                                        for (var i = 0; i < $scope.paket.length; i++) {
                                            for (var j = 0; j < $scope.tahapanSekarang.length; j++) {
                                                if ($scope.paket[i].paket_id === $scope.tahapanSekarang[j].paket_lelang_id) {
                                                    if ($scope.tahapanSekarang[j].stateNext === 'Curr') {
                                                        $scope.paket[i].tahapanSekarang.push($scope.tahapanSekarang[j]);
                                                    } else if ($scope.tahapanSekarang[j].stateNext === 'Next') {
                                                        $scope.paket[i].tahapanNext.push($scope.tahapanSekarang[j]);
                                                    } else if ($scope.tahapanSekarang[j].stateNext === 'Old') {
                                                        $scope.paket[i].tahapanLama.push($scope.tahapanSekarang[j]);
                                                    }
                                                }
                                            }
                                        }
                                        $rootScope.unloadLoading();
                                    }
                                });
                            }
                            else {
                                $rootScope.unloadLoading();
                            }
                        }
                        else
                            alert("Tekan Tombol Refresh (F5)");
                    })
                );
            };

            $scope.cariPaket = function() {
                loadPaket();
            };

            $scope.cariPekerjaan = function() {
                if ($scope.waktuMulaiTercentang === true && ($scope.waktuMulai1 === "" || $scope.waktuMulai2 === "" || $scope.waktuMulai1 === undefined || $scope.waktuMulai2 === undefined)) {
                    alert("waktu mulai belum diisi");
                    return;
                }
                if ($scope.waktuSelesaiTercentang === true && ($scope.waktuSelesai1 === "" || $scope.waktuSelesai2 === "" || $scope.waktuSelesai1 === undefined || $scope.waktuSelesai2 === undefined)) {
                    alert("waktu selesai belum diisi");
                    return;
                }
                loadPekerjaan();
            };

            /*
             $scope.ubahKeteranganPekerjaan = function(pekerjaan) {
             var modalInstance = $modal.open({
             templateUrl: 'ubahKeteranganPekerjaan.html',
             controller: ubahKeteranganPekerjaanCtrl,
             resolve: {
             item: function() {
             return pekerjaan;
             }
             }
             });
             modalInstance.result.then(function() {
             loadPekerjaan();
             });
             };
             */

            //detail kontrak paket per subpekerjaan
            $scope.detailKeteranganPekerjaan = function(pekerjaan) {
                var modalInstance = $modal.open({
                    templateUrl: 'detailKeteranganPekerjaan.html',
                    controller: detailKeteranganPekerjaanCtrl,
                    resolve: {
                        item: function() {
                            return pekerjaan;
                        }
                    }
                });
                modalInstance.result.then(function() {
                    loadPekerjaan();
                });
            };

            $scope.viewTahapan = function(idPaket) {
                $state.transitionTo('tahapan-prakualifikasi', {idPrakual: idPaket});
            };

            $scope.menujuAktivasi = function() {
                $state.transitionTo('verifikasi-data', {aktivasiAtauVerifikasi: 1});
            };

            $scope.menujuVerifikasi = function() {
                $state.transitionTo('verifikasi-data', {aktivasiAtauVerifikasi: 3});
            };

            $scope.menujuPermintaanUbahData = function() {
                $state.transitionTo('buka-lock-rekanan', {status: 2});
            };

            $scope.menujuExpired = function() {
                $state.transitionTo('list-expired');
            };
        }) // end homeAdminCtrl


        .controller('homePanitiaCtrl', function($scope, $http, $rootScope, $state, $cookieStore, $modal, ngProgress) {
            $scope.totalItems = 0;
            $scope.currentPage = 1;
            $scope.maxSize = 10;
            $scope.paket = [];
            $scope.statusTercentang = false;
            $scope.namaLelangTercentang = false;
            $scope.namaTahapanTercentang = false;
            $scope.selectedStatus = "5";
            $scope.srcNamaLelang = "";
            $scope.srcNamaTahapan = "";
            $scope.page_id = 2;

            $scope.totalItemsPekerjaan = 0;
            $scope.currentPagePekerjaan = 1;
            $scope.maxSizePekerjaan = 10;
            $scope.pekerjaan = [];
            $scope.srcTextPekerjaan = "";

            $scope.namaPekerjaanTercentang = false;
            $scope.waktuMulaiTercentang = false;
            $scope.waktuSelesaiTercentang = false;
            $scope.waktuMulai1 = "";
            $scope.waktuMulai2 = "";
            $scope.waktuSelesai1 = "";
            $scope.waktuSelesai2 = "";

            $scope.init = function() {
                //$rootScope.refreshWaktu();
                $rootScope.getSession().then(function(result) {
                    $rootScope.userSession = result.data.data;
                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
                    $rootScope.authorize(new function() {
                        $rootScope.readNotif();
                        loadPaket();
                        loadPekerjaan();
                    });
                });
            };

            function loadPaket() {
                $scope.currentPage = 1;
                $rootScope.loadLoading("Silahkan Tunggu...");
                //itp.paket.count
                $http.post($rootScope.url_api + "paket/count", {
                    username: $rootScope.userLogin,
                    statusTercentang: $scope.statusTercentang,
                    namaLelangTercentang: $scope.namaLelangTercentang,
                    namaTahapanTercentang: $scope.namaTahapanTercentang,
                    status: Number($scope.selectedStatus),
                    namaLelang: "%" + $scope.srcNamaLelang + "%",
                    namaTahapan: "%" + $scope.srcNamaTahapan + "%"
                }).success(function(reply) {
                    if (reply.status === 200) {
                        $scope.totalItems = reply.result.data;
                    }
                }).error(function(err) {
                    $.growl.error({message: "Gagal Akses API > " + err});
                    return;
                });


                $scope.paket = [];
                var offset = 0;
                var limit = 10;
                $rootScope.loadLoading("Silahkan Tunggu...");
                //itp.paket.select
                $http.post($rootScope.url_api + "paket/select", {
                    username: $rootScope.userLogin,
                    statusTercentang: $scope.statusTercentang,
                    namaLelangTercentang: $scope.namaLelangTercentang,
                    namaTahapanTercentang: $scope.namaTahapanTercentang,
                    status: Number($scope.selectedStatus),
                    namaLelang: "%" + $scope.srcNamaLelang + "%",
                    namaTahapan: "%" + $scope.srcNamaTahapan + "%",
                    offset: offset,
                    limit: limit
                }).success(function(reply) {
                    if (reply.status === 200) {
                        $scope.paket = reply.result.data;
                        if ($scope.paket.length > 0) {
                            for (var i = 0; i < $scope.paket.length; i++)
                            {
                                $scope.paket[i].tahapanSekarang = [];
                                $scope.paket[i].tahapanLama = [];
                                $scope.paket[i].tahapanNext = [];
                                $scope.paket[i].isKetua = 0;
                            }
                            var arr1;
                            var arr2 = [];
                            for (var i = 0; i < $scope.paket.length; i++)
                            {
                                arr1 = [];
                                arr1.push($scope.paket[i].paket_id);
                                arr2.push(arr1);
                            }
                            //itp.paket.cekKetuaPanitia'
                            $http.post($rootScope.url_api + "paket/cekketua", {
                                username: $rootScope.userLogin
                            }).success(function(reply2) {
                                if (reply2.status === 200) {
                                    $scope.ketuaPanitia = reply2.result.data;
                                    for (var i = 0; i < $scope.paket.length; i++) {
                                        for (var j = 0; j < $scope.ketuaPanitia.length; j++) {
                                            if ($scope.paket[i].paket_id === $scope.ketuaPanitia[j].paket_lelang_id) {
                                                $scope.paket[i].isKetua = $scope.ketuaPanitia[j].ketua_panitia;
                                            }
                                        }
                                    }
                                }
                            });
                            //itp.paket.cekTahapanSekarang
                            $http.post($rootScope.url_api + "paket/cektahapan", {
                                page_id: $scope.page_id,
                                param: arr2
                            }).success(function(reply2) {
                                if (reply2.status === 200) {
                                    $scope.tahapanSekarang = reply2.result.data;
                                    for (var i = 0; i < $scope.paket.length; i++) {
                                        for (var j = 0; j < $scope.tahapanSekarang.length; j++) {
                                            if ($scope.paket[i].paket_id === $scope.tahapanSekarang[j].paket_lelang_id) {
                                                if ($scope.tahapanSekarang[j].stateNext === 'Curr') {
                                                    $scope.paket[i].tahapanSekarang.push($scope.tahapanSekarang[j]);
                                                } else if ($scope.tahapanSekarang[j].stateNext === 'Next') {
                                                    $scope.paket[i].tahapanNext.push($scope.tahapanSekarang[j]);
                                                } else if ($scope.tahapanSekarang[j].stateNext === 'Old') {
                                                    $scope.paket[i].tahapanLama.push($scope.tahapanSekarang[j]);
                                                }
                                            }
                                        }
                                    }
                                    $rootScope.unloadLoading();
                                    ngProgress.complete();
                                }
                            }).error(function(err) {
                                $.growl.error({message: "Gagal Akses API > " + err});
                                return;
                            });
                        }
                        else {
                            $rootScope.unloadLoading();
                            ngProgress.complete();
                        }
                    } else {
                        $rootScope.unloadLoading();
                        ngProgress.complete();
                    }
                }).error(function(err) {
                    $.growl.error({message: "Gagal Akses API > " + err});
                    return;
                });
            }

            function loadPekerjaan() {
                $scope.currentPagePekerjaan = 1;
                $rootScope.loadLoading("Silahkan Tunggu...");
                //itp.pekerjaan.count
                $http.post($rootScope.url_api + "pekerjaan/count", {
                    username: $rootScope.userLogin,
                    namaPekerjaanTercentang: $scope.namaPekerjaanTercentang,
                    waktuMulaiTercentang: $scope.waktuMulaiTercentang,
                    waktuSelesaiTercentang: $scope.waktuSelesaiTercentang,
                    srcTextPekerjaan: "%" + $scope.srcTextPekerjaan + "%",
                    waktuMulai1: $scope.waktuMulai1,
                    waktuMulai2: $scope.waktuMulai2,
                    waktuSelesai1: $scope.waktuSelesai1,
                    waktuSelesai2: $scope.waktuSelesai2
                }).success(function(reply) {
                    if (reply.status === 200) {
                        $scope.totalItemsPekerjaan = reply.result.data;
                    }
                }).error(function(err) {
                    $.growl.error({message: "Gagal Akses API > " + err});
                    return;
                });

                $scope.pekerjaan = [];
                $scope.currentPagePekerjaan = 1;
                $rootScope.loadLoading("Silahkan Tunggu...");
                //itp.pekerjaan.selectByPaket
                $http.post($rootScope.url_api + "pekerjaan/select", {
                    username: $rootScope.userLogin,
                    namaPekerjaanTercentang: $scope.namaPekerjaanTercentang,
                    waktuMulaiTercentang: $scope.waktuMulaiTercentang,
                    waktuSelesaiTercentang: $scope.waktuSelesaiTercentang,
                    srcTextPekerjaan: "%" + $scope.srcTextPekerjaan + "%",
                    waktuMulai1: $scope.waktuMulai1,
                    waktuMulai2: $scope.waktuMulai2,
                    waktuSelesai1: $scope.waktuSelesai1,
                    waktuSelesai2: $scope.waktuSelesai2,
                    offset: 0,
                    limit: 10,
                    page_id: $scope.page_id
                }).success(function(reply) {
                    if (reply.status === 200) {
                        $scope.pekerjaan = reply.result.data;
                    }
                    $rootScope.unloadLoading();
                }).error(function(err) {
                    $.growl.error({message: "Gagal Akses API > " + err});
                    return;
                });
            }

            //detail kontrak paket per subpekerjaan
            $scope.detailKeteranganPekerjaan = function(pekerjaan) {
                var modalInstance = $modal.open({
                    templateUrl: 'detailKeteranganPekerjaan.html',
                    controller: detailKeteranganPekerjaanCtrl,
                    resolve: {
                        item: function() {
                            return pekerjaan;
                        }
                    }
                });
                modalInstance.result.then(function() {
                    loadPekerjaan();
                });
            };

            function convertTanggal(input) {
                if (input){
                    return $rootScope.convertTanggalWaktu(input);
                } else {
                    return '';
                }
                return input;
            };

            function convertTanggalVersi2(input) {
                var tahun = input.substring(0, 4);
                var bulan = input.substring(5, 7);
                var karakterBulan = bulan.split('');
                if (karakterBulan[0] === '0')
                    bulan = bulan.substring(1, 2);
                var tanggal = input.substring(8, 10);
                var karakterTanggal = tanggal.split('');
                if (karakterTanggal[0] === '0')
                    tanggal = tanggal.substring(1, 2);
                return bulan + "/" + tanggal + "/" + tahun;
            };

            $scope.jLoad = function(current) {
                $scope.paket = [];
                $scope.currentPage = current;
                $scope.offset = (current * 10) - 10;
                var limit = 10;
                //itp.paket.select
                $rootScope.authorize(
                        $http.post($rootScope.url_api + "paket/select", {
                            username: $rootScope.userLogin,
                            page_id: $scope.page_id,
                            statusTercentang: $scope.statusTercentang,
                            namaLelangTercentang: $scope.namaLelangTercentang,
                            namaTahapanTercentang: $scope.namaTahapanTercentang,
                            status: Number($scope.selectedStatus),
                            namaLelang: "%" + $scope.srcNamaLelang + "%",
                            namaTahapan: "%" + $scope.srcNamaTahapan + "%",
                            offset: $scope.offset,
                            limit: limit
                        }).success(function(reply) {
                    if (reply.status === 200) {
                        $scope.paket = reply.result.data;
                        if ($scope.paket.length > 0) {
                            for (var i = 0; i < $scope.paket.length; i++)
                            {
                                $scope.paket[i].tahapanSekarang = [];
                                $scope.paket[i].tahapanLama = [];
                                $scope.paket[i].tahapanNext = [];
                                $scope.paket[i].isKetua = 0;
                            }
                            var arr1;
                            var arr2 = [];
                            for (var i = 0; i < $scope.paket.length; i++)
                            {
                                arr1 = [];
                                arr1.push($scope.paket[i].paket_id);
                                arr2.push(arr1);
                            }
                            //itp.paket.cekKetuaPanitia
                            $http.post($rootScope.url_api + "paket/cekketua", {
                                username: $rootScope.userLogin
                            }).success(function(reply2) {
                                if (reply2.status === 200) {
                                    $scope.ketuaPanitia = reply2.result.data;
                                    for (var i = 0; i < $scope.paket.length; i++) {
                                        for (var j = 0; j < $scope.ketuaPanitia.length; j++) {
                                            if ($scope.paket[i].paket_id === $scope.ketuaPanitia[j].paket_lelang_id) {
                                                $scope.paket[i].isKetua = $scope.ketuaPanitia[j].ketua_panitia;
                                            }
                                        }
                                    }
                                }
                            }).error(function(err) {
                                $.growl.error({message: "Gagal Akses API > " + err});
                                return;
                            });
                            //itp.paket.cekTahapanSekarang
                            $http.post($rootScope.url_api + "paket/cektahapan", {
                                page_id: $scope.page_id,
                                param: arr2
                            }).success(function(reply2) {
                                if (reply2.status === 200) {
                                    $scope.tahapanSekarang = reply2.result.data;
                                    for (var i = 0; i < $scope.paket.length; i++) {
                                        for (var j = 0; j < $scope.tahapanSekarang.length; j++) {
                                            if ($scope.paket[i].paket_id === $scope.tahapanSekarang[j].paket_lelang_id) {
                                                if ($scope.tahapanSekarang[j].stateNext === 'Curr') {
                                                    $scope.paket[i].tahapanSekarang.push($scope.tahapanSekarang[j]);
                                                } else if ($scope.tahapanSekarang[j].stateNext === 'Next') {
                                                    $scope.paket[i].tahapanNext.push($scope.tahapanSekarang[j]);
                                                } else if ($scope.tahapanSekarang[j].stateNext === 'Old') {
                                                    $scope.paket[i].tahapanLama.push($scope.tahapanSekarang[j]);
                                                }
                                            }
                                        }
                                    }
                                }
                            }).error(function(err) {
                                $.growl.error({message: "Gagal Akses API > " + err});
                                return;
                            });
                        }
                        else {
                            $rootScope.unloadLoading();
                        }
                    }
                    else
                        alert("Tekan Tombol Refresh (F5)");
                }).error(function(err) {
                    $.growl.error({message: "Gagal Akses API > " + err});
                    return;
                })
                        );
            };

            $scope.jLoadPekerjaan = function(current) {
                $scope.pekerjaan = [];
                $scope.currentPagePekerjaan = current;
                $rootScope.authorize(
                        //itp.pekerjaan.selectByPaket
                        $http.post($rootScope.url_api + "pekerjaan/select", {
                            username: $rootScope.userLogin,
                            namaPekerjaanTercentang: $scope.namaPekerjaanTercentang,
                            waktuMulaiTercentang: $scope.waktuMulaiTercentang,
                            waktuSelesaiTercentang: $scope.waktuSelesaiTercentang,
                            srcTextPekerjaan: "%" + $scope.srcTextPekerjaan + "%",
                            waktuMulai1: $scope.waktuMulai1,
                            waktuMulai2: $scope.waktuMulai2,
                            waktuSelesai1: $scope.waktuSelesai1,
                            waktuSelesai2: $scope.waktuSelesai2,
                            offset: (current * 10) - 10,
                            limit: 10,
                            page_id: $scope.page_id
                        }).success(function(reply) {
                    if (reply.status === 200) {
                        $scope.pekerjaan = reply.result.data;
                    }
                    else
                        alert("Tekan Tombol Refresh (F5)");
                }).error(function(err) {
                    $.growl.error({message: "Gagal Akses API > " + err});
                    return;
                })
                        );
            };

            $scope.cariPaket = function() {
                loadPaket();
            };

            $scope.cariPekerjaan = function() {
                if ($scope.waktuMulaiTercentang === true && ($scope.waktuMulai1 === "" || $scope.waktuMulai2 === "" || $scope.waktuMulai1 === undefined || $scope.waktuMulai2 === undefined)) {
                    alert("waktu mulai belum diisi");
                    return;
                }
                if ($scope.waktuSelesaiTercentang === true && ($scope.waktuSelesai1 === "" || $scope.waktuSelesai2 === "" || $scope.waktuSelesai1 === undefined || $scope.waktuSelesai2 === undefined)) {
                    alert("waktu selesai belum diisi");
                    return;
                }
                loadPekerjaan();
            };

            function hitungSelisihHari(tanggal_selesai_kontrak) {
                var date1 = new Date(convertTanggalVersi2(tanggal_selesai_kontrak));
                var date2 = new Date();
                var timeDiff = date1.getTime() - date2.getTime();
                var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));
                return diffDays;
            }

            /*
             $scope.ubahKeteranganPekerjaan = function(pekerjaan) {
             var modalInstance = $modal.open({
             templateUrl: 'ubahKeteranganPekerjaan.html',
             controller: ubahKeteranganPekerjaanCtrl,
             resolve: {
             item: function() {
             return pekerjaan;
             }
             }
             });
             modalInstance.result.then(function() {
             loadPekerjaan();
             });
             };
             */

            $scope.viewTahapan = function(idPaket) {
                $state.transitionTo('paketlelang-viewtahapan', {idPaket: idPaket});
            };
        })

        


/*
 var ubahKeteranganPekerjaanCtrl = function($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
 $scope.ubah = new Pekerjaan(item.tgl_mulai_kontrak, item.tgl_selesai_kontrak, item.keterangan_pekerjaan);
 var page_id = 2;
 
 $scope.simpan = function() {
 $rootScope.authorize(
 //itp.pekerjaan.edit
 $http.post($rootScope.url_api+"pekerjaan/edit", {
 tgl_mulai_kontrak: $scope.ubah.tgl_mulai_pekerjaan,
 tgl_selesai_kontrak: $scope.ubah.tgl_selesai_pekerjaan,
 keterangan_pekerjaan: $scope.ubah.keterangan_pekerjaan,
 ttd_kontrak_id: item.ttd_kontrak_id,
 nama_paket: item.nama_paket,
 page_id: page_id,
 username: $rootScope.userLogin
 }).success(function(reply) {
 if (reply.status === 200) {
 alert("Berhasil mengedit pekerjaan");
 $modalInstance.close();
 }
 else
 alert("gagal mengupdate step");
 }).error(function(err){
 $.growl.error({ message: "Gagal Akses API > " + err });
 return;
 })
 );
 };
 $scope.cancel = function() {
 $modalInstance.dismiss('cancel');
 };
 };
 */

function Pekerjaan(tgl_mulai_pekerjaan, tgl_selesai_pekerjaan, keterangan_pekerjaan) {
    var self = this;
    self.tgl_mulai_pekerjaan = tgl_mulai_pekerjaan;
    self.tgl_selesai_pekerjaan = tgl_selesai_pekerjaan;
    self.keterangan_pekerjaan = keterangan_pekerjaan;
}