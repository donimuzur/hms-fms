angular.module('eprocAppPanitia')
        .directive('maskdate', function () {
            return {
                restrict: 'AE',
                link: function (scope, elt) {
                    $(elt).inputmask("yyyy-mm-dd", {
                        placeholder: "yyyy-mm-dd"
                    });
                }
            };
        })
        .controller('keypersonalCtrl', function ($scope, $modal, $cookieStore, $state, $rootScope, $http) {
            $scope.bisaMengubahData;
            $scope.datata = [];
            /* DETAIL ARRAY */
            $scope.arrPengKerja = [];
            $scope.arrPend = [];
            $scope.arrSert = [];
            $scope.arrBhs = [];

            $scope.initialize = function () {
                $rootScope.getSession().then(function (result) {
                    $rootScope.userSession = result.data.data;
                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
                    $rootScope.rekananid = $rootScope.userSession.session_data.rekanan_id;
                    $rootScope.loadLoading("Silahkan Tunggu...");
                    $rootScope.authorize(loadawal());
                });
            };

            function loadawal() {
                $http.post($rootScope.url_api + "rekanan/cekBisaMengubahData", {
                    rekananId: [$rootScope.rekananid]
                }).success(function (reply) {
                    if (reply.status === 200) {
                        var data = reply.result.data;
                        $scope.bisaMengubahData = data[0].bisa_mengubah_data == "1";
                        $rootScope.unloadLoading();
                    } else {
                        $.growl.error({message: "Gagal mendapatkan Hak Bisa Mengubah Data!!"});
                        $rootScope.unloadLoading();
                        return;
                    }
                }).error(function (err) {
                    $.growl.error({message: "Gagal Akses API >" + err});
                    $http.post($rootScope.url_api + "logging", {
                        message: "Tidak berhasil akses API : " + JSON.stringify(err),
                        source: "tenaga-ahli.js - rekanan/cekBisaMengubahData"
                    }).then(function (response) {
                        // do nothing
                        // don't have to feedback
                    });
                    $rootScope.unloadLoading();
                    return;
                });

                $http.post($rootScope.url_api + "TAC/getall", {
                    rekanan_id: $rootScope.rekananid
                }).success(function (reply) {
                    if (reply.status === 200) {
                        var data = reply.result.data;
                        $scope.datata = data;
//                        for (var i = 0; i < $scope.datata.length; i++) {
//                            if ($scope.datata[i].ta_tgl_lahir !== null) {
//                                $scope.datata[i].ta_tgl_lahir = $rootScope.convertTanggalRootAwal($scope.datata[i].ta_tgl_lahir);
//                            }
//                        }
                        if ($scope.datata.length > 0) {
                            $rootScope.insertStatusIsiData($rootScope.rekananid, 'ta', 1);
                        } else {
                            $rootScope.insertStatusIsiData($rootScope.rekananid, 'ta', 0);
                        }
                        $rootScope.unloadLoading();
                    } else {
                        $.growl.error({message: "Gagal mendapatkan Data Tenaga Ahli!!"});
                        $rootScope.unloadLoading();
                        return;
                    }
                }).error(function (err) {
                    $.growl.error({message: "Gagal Akses API >" + err});
                    $http.post($rootScope.url_api + "logging", {
                        message: "Tidak berhasil akses API : " + JSON.stringify(err),
                        source: "tenaga-ahli.js - TAC/getall"
                    }).then(function (response) {
                        // do nothing
                        // don't have to feedback
                    });
                    $rootScope.unloadLoading();
                    return;
                });
            }

            function reloaddetail() {
                $rootScope.loadLoading("Silahkan Tunggu...");
                $scope.arrPengKerja = [];
                $scope.arrPend = [];
                $scope.arrSert = [];
                $scope.arrBhs = [];
                function convertTanggal(input) {
                    var tahun = input.substring(0, 4);
                    var bulan = input.substring(5, 7);
                    var z;
                    if (bulan === '01')
                        z = "Januari";
                    else if (bulan === '02')
                        z = "Februari";
                    else if (bulan === '03')
                        z = "Maret";
                    else if (bulan === '04')
                        z = "April";
                    else if (bulan === '05')
                        z = "Mei";
                    else if (bulan === '06')
                        z = "Juni";
                    else if (bulan === '07')
                        z = "Juli";
                    else if (bulan === '08')
                        z = "Agustus";
                    else if (bulan === '09')
                        z = "September";
                    else if (bulan === '10')
                        z = "Oktober";
                    else if (bulan === '11')
                        z = "November";
                    else if (bulan === '12')
                        z = "Desember";
                    var tanggal = input.substring(8, 10);
                    //var waktu = input.substring(11, 16);
                    return tanggal + " " + z + " " + tahun;
                }
                $http.post($rootScope.url_api + "TAC/getdetail", {
                    rekanan_ta_id: $scope.choosenTaId
                }).success(function (reply) {
                    if (reply.status === 200) {
                        var hsl = reply.result.data;
                        for (var i = 0; i < hsl.length; i++) {
                            switch (hsl[i].jenis) {
                                case 1:
                                case "1" :
                                    $scope.arrPengKerja.push(hsl[i]);
                                    break;
                                case 2:
                                case "2" :
                                    $scope.arrPend.push(hsl[i]);
                                    break;
                                case 3:
                                case "3" :
                                    $scope.arrSert.push(hsl[i]);
                                    break;
                                case 4:
                                case "4" :
                                    $scope.arrBhs.push(hsl[i]);
                                    break;
                            }
                        }

                        for (var i = 0; i < $scope.arrSert.length; i++) {
                            if ($scope.arrSert[i].tahun_awal !== null && $scope.arrSert[i].tahun_awal !== '' && $scope.arrSert[i].tahun_awal !== undefined) {
                                $scope.arrSert[i].tahun_awal_converted = convertTanggal($scope.arrSert[i].tahun_awal);
                                $scope.arrSert[i].tahun_awal = $rootScope.convertTanggalRootAwal($scope.arrSert[i].tahun_awal);
                            }
                            if ($scope.arrSert[i].tahun_akhir !== null && $scope.arrSert[i].tahun_akhir !== '' && $scope.arrSert[i].tahun_akhir !== undefined) {
                                $scope.arrSert[i].tahun_akhir_converted = convertTanggal($scope.arrSert[i].tahun_akhir);
                                $scope.arrSert[i].tahun_akhir = $rootScope.convertTanggalRootAwal($scope.arrSert[i].tahun_akhir);
                            }
                        }
                        $rootScope.unloadLoading();
                    } else {
                        $.growl.error({message: "Gagal mendapatkan Data Detail Tenaga Ahli!!"});
                        $rootScope.unloadLoading();
                        return;
                    }
                }).error(function (err) {
                    $.growl.error({message: "Gagal Akses API >" + err});
                    $http.post($rootScope.url_api + "logging", {
                        message: "Tidak berhasil akses API : " + JSON.stringify(err),
                        source: "tenaga-ahli.js - TAC/getdetail"
                    }).then(function (response) {
                        // do nothing
                        // don't have to feedback
                    });
                    $rootScope.unloadLoading();
                    return;
                });
            }

            $scope.action = -1;
            $scope.resolvedata = {};
            $scope.viewdetail = function (ta_id, nama) {
                $scope.choosenTaId = ta_id;
                $scope.namaTenaga = nama;
                $('#ta_detail_cv').slideToggle("slow");
                reloaddetail();
            };

            $scope.hapusdetail = function (id, url) {
                bootbox.confirm("Yakin menghapus data ?", function (res) {
                    if (res) {
                        $http.post($rootScope.url_api + "TAC/deletedetail", {
                            rekanan_ta_cv_id: id,
                            username: $rootScope.userLogin
                        }).success(function (reply) {
                            if (reply.status === 200) {
                                $http.post($rootScope.url_api + "deleteFile", {
                                    url: url
                                }).then(function (res) {
//                                    $.growl.notice({title: "[INFO]", message: "Anda Telah Menghapus Data Dokumen Lain-lain"});
//                                    $rootScope.unloadLoading();
//                                    $scope.initial();
                                });
                                $.growl.notice({title: "[INFO]", message: "Data berhasil dihapus"});
                                reloaddetail();
                            } else {
                                $.growl.error({title: "[PERINGATAN]", message: "Data gagal dihapus"});
                                return;
                            }
                        }).error(function (err) {
                            $.growl.error({message: "Gagal Akses API >" + err});
                            $http.post($rootScope.url_api + "logging", {
                                message: "Tidak berhasil akses API : " + JSON.stringify(err),
                                source: "tenaga-ahli.js - TAC/deletedetail"
                            }).then(function (response) {
                                // do nothing
                                // don't have to feedback
                            });
                            $rootScope.unloadLoading();
                            return;
                        });
                    }
                });
            };

            $scope.opendetailmodal = function (jenis, action, obj) {
                $scope.objdata = action === 'add' ? {} : obj;
                var mdlnya = $modal.open({
                    templateUrl: "detail1AU.html",
                    controller: "detail1AU",
                    resolve: {
                        jenis: function () {
                            return jenis;
                        },
                        action: function () {
                            return action;
                        },
                        datane: function () {
                            return $scope.objdata;
                        },
                        tenagaahli_id: function () {
                            return $scope.choosenTaId;
                        }
                    }
                });
                mdlnya.result.then(function () {
                    reloaddetail();
                });
            };

            $scope.viewmodal = function (data) {
                $modal.open({// old: var opnmdl = $modal.open({
                    templateUrl: "mdlVWta.html",
                    controller: "mdlVWta",
                    resolve: {
                        datane: function () {
                            return data;
                        }
                    }
                });
            };
            $scope.openmodal = function (act, data) {
                $scope.action = act === "add" ? 0 : 1;
                $scope.resolvedata = act === "add" ? {} : data;
                var opnmdl = $modal.open({
                    templateUrl: "mdlAUta.html",
                    controller: "mdlAUta",
                    resolve: {
                        action: function () {
                            return $scope.action;
                        },
                        datane: function () {
                            return $scope.resolvedata;
                        }
                    }
                });
                opnmdl.result.then(function () {
                    $scope.initialize();
                });
            };

            $scope.hapus = function (idta) {
                bootbox.confirm("Yakin menghapus data ?", function (res) {
                    if (res) {
                        $http.post($rootScope.url_api + "TAC/getdetail", {
                            rekanan_ta_id: idta
                        }).success(function (reply) {
                            if (reply.status === 200) {
                                var data = reply.result.data;
                                var urls = [];
                                for (var i = 0; i < data.length; i++) {
                                    urls.push(data[i].scanbukti);
                                }
                                $http.post($rootScope.url_api + "TAC/delete", {
                                    tenaga_ahli_id: idta,
                                    username: $rootScope.userLogin
                                }).success(function (reply) {
                                    if (reply.status === 200) {
                                        $http.post($rootScope.url_api + "deleteFile", {
                                            urls: urls
                                        }).then(function (res) {
//                                    $.growl.notice({title: "[INFO]", message: "Anda Telah Menghapus Data Dokumen Lain-lain"});
//                                    $rootScope.unloadLoading();
//                                    $scope.initial();
                                        });
                                        $.growl.notice({title: "[INFO]", message: "Data berhasil dihapus"});
                                        $scope.initialize();
                                    } else {
                                        $.growl.error({title: "[PERINGATAN]", message: "Data gagal dihapus"});
                                        return;
                                    }
                                }).error(function (err) {
                                    $.growl.error({message: "Gagal Akses API >" + err});
                                    $http.post($rootScope.url_api + "logging", {
                                        message: "Tidak berhasil akses API : " + JSON.stringify(err),
                                        source: "tenaga-ahli.js - TAC/delete"
                                    }).then(function (response) {
                                        // do nothing
                                        // don't have to feedback
                                    });
                                    $rootScope.unloadLoading();
                                    return;
                                });
                            } else {
                                $.growl.error({title: "[PERINGATAN]", message: "Data gagal dihapus"});
                                return;
                            }
                        }).error(function (err) {
                            $.growl.error({message: "Gagal Akses API >" + err});
                            $http.post($rootScope.url_api + "logging", {
                                message: "Tidak berhasil akses API : " + JSON.stringify(err),
                                source: "tenaga-ahli.js - TAC/getdetail"
                            }).then(function (response) {
                                // do nothing
                                // don't have to feedback
                            });
                            $rootScope.unloadLoading();
                            return;
                        });
                    }
                });
            };
        })

        .controller('mdlAUta', function ($scope, $http, $modalInstance, action, datane, $cookieStore, $state, $rootScope) {
            $scope.data = {};
            $scope.data_lama = {};
            $scope.initial = function () {
                $rootScope.getSession().then(function (result) {
                    $rootScope.userSession = result.data.data;
                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
                    $rootScope.rekananid = $rootScope.userSession.session_data.rekanan_id;
                });
                $scope.data = action === 0 ? {} : datane;
                if (action > 0) {
                    angular.copy(datane, $scope.data_lama);
                    $scope.data.ta_tgl_lahir = $rootScope.convertTanggalRootAwal($scope.data.ta_tgl_lahir);
                }
            };
            $scope.simpan = function () {
                if ($scope.data.ta_nama === undefined) {
                    $.growl.error({title: "[WARNING]", message: "Nama belum diisi"});
                    return;
                } else if ($scope.data.ta_tgl_lahir === undefined) {
                    $.growl.error({title: "[WARNING]", message: "Tanggal lahir belum diisi"});
                    return;
                } else if ($scope.data.ta_jk === undefined) {
                    $scope.data.ta_jk = "";
                } else if ($scope.data.ta_alamat === undefined) {
                    $scope.data.ta_alamat = "";
                } else if ($scope.data.ta_pendidikanakhir === undefined) {
                    $.growl.error({title: "[WARNING]", message: "Pendidikan terakhir belum diisi"});
                    return;
                } else if ($scope.data.ta_kewarganegaraan === undefined) {
                    $scope.data.ta_kewarganegaraan = "";
                } else if ($scope.data.ta_jabatan === undefined) {
                    $.growl.error({title: "[WARNING]", message: "Jabatan belum diisi"});
                    return;
                } else if ($scope.data.ta_pengalamankerja === undefined) {
                    $.growl.error({title: "[WARNING]", message: "Pengalaman kerja belum diisi"});
                    return;
                } else if ($scope.data.ta_email === undefined) {
                    $.growl.error({title: "[WARNING]", message: "Alamat email belum diisi"});
                    return;
                } else if ($scope.data.ta_status === undefined) {
                    $scope.data.ta_status = "";
                } else if ($scope.data.ta_keahlian === undefined) {
                    $.growl.error({title: "[WARNING]", message: "Keahlian belum diisi"});
                    return;
                } else if (action === 0) {
                    insert();
                } else if (action === 1) {
                    update();
                }
            };

            $scope.batal = function () {
                angular.copy($scope.data_lama, $scope.data);
                $modalInstance.dismiss('cancel');
            };
            function insert() {
                $rootScope.loadLoadingModal("Menyimpan Data...");
                $http.post($rootScope.url_api + "TAC/insert", {
                    ta_nama: $scope.data.ta_nama,
                    ta_tgl_lahir: $rootScope.convertTanggalRoot($scope.data.ta_tgl_lahir),
                    ta_jk: $scope.data.ta_jk,
                    ta_alamat: $scope.data.ta_alamat,
                    ta_kewarganegaraan: $scope.data.ta_kewarganegaraan,
                    ta_pendidikanakhir: $scope.data.ta_pendidikanakhir,
                    ta_jabatan: $scope.data.ta_jabatan,
                    ta_pengalamankerja: $scope.data.ta_pengalamankerja,
                    ta_email: $scope.data.ta_email,
                    ta_status: $scope.data.ta_status,
                    ta_keahlian: $scope.data.ta_keahlian,
                    rekanan_id: $rootScope.rekananid,
                    username: $rootScope.userLogin
                }).success(function (reply) {
                    if (reply.status === 200) {
                        $.growl.notice({title: "[INFO]", message: "Data berhasil disimpan"});
                        $rootScope.unloadLoadingModal();
                        $modalInstance.close();
                    } else {
                        $.growl.error({title: "[PERINGATAN]", message: "Data gagal disimpan"});
                        $rootScope.unloadLoadingModal();
                        return;
                    }
                }).error(function (err) {
                    $.growl.error({message: "Gagal Akses API >" + err});
                    $http.post($rootScope.url_api + "logging", {
                        message: "Tidak berhasil akses API : " + JSON.stringify(err),
                        source: "tenaga-ahli.js - TAC/insert"
                    }).then(function (response) {
                        // do nothing
                        // don't have to feedback
                    });
                    $rootScope.unloadLoading();
                    return;
                });
            }
            function update() {
                $rootScope.loadLoadingModal("Silahkan Tunggu...");
                $http.post($rootScope.url_api + "TAC/update", {
                    ta_nama: $scope.data.ta_nama,
                    ta_tgl_lahir: $rootScope.convertTanggalRoot($scope.data.ta_tgl_lahir),
                    ta_jk: $scope.data.ta_jk,
                    ta_alamat: $scope.data.ta_alamat,
                    ta_kewarganegaraan: $scope.data.ta_kewarganegaraan,
                    ta_pendidikanakhir: $scope.data.ta_pendidikanakhir,
                    ta_jabatan: $scope.data.ta_jabatan,
                    ta_pengalamankerja: $scope.data.ta_pengalamankerja,
                    ta_email: $scope.data.ta_email,
                    ta_status: $scope.data.ta_status,
                    ta_keahlian: $scope.data.ta_keahlian,
                    tenaga_ahli_id: $scope.data.tenaga_ahli_id,
                    rekanan_id: $rootScope.rekananid,
                    username: $rootScope.userLogin
                }).success(function (reply) {
                    if (reply.status === 200) {
                        $.growl.notice({title: "[INFO]", message: "Data berhasil diubah"});
                        $rootScope.unloadLoadingModal();
                        $modalInstance.close();
                    } else {
                        $.growl.error({title: "[PERINGATAN]", message: "Data gagal diubah"});
                        $rootScope.unloadLoadingModal();
                        return;
                    }
                }).error(function (err) {
                    $.growl.error({message: "Gagal Akses API >" + err});
                    $http.post($rootScope.url_api + "logging", {
                        message: "Tidak berhasil akses API : " + JSON.stringify(err),
                        source: "tenaga-ahli.js - TAC/update"
                    }).then(function (response) {
                        // do nothing
                        // don't have to feedback
                    });
                    $rootScope.unloadLoading();
                    return;
                });
            }
        })

        .controller('mdlVWta', function ($scope, $modalInstance, datane, $http, $state, $rootScope) {
            $scope.datata = datane;
            var tgl_lahir = datane.ta_tgl_lahir;
//            if (tgl_lahir !== null || tgl_lahir !== '') {
//                $scope.datata.ta_tgl_lahir_converted = $rootScope.convertTanggalRootIndonesia(tgl_lahir);
//            }
            $scope.batal = function () {
                $modalInstance.dismiss('cancel');
            };
        })

        .controller('detail1AU', function ($scope, $http, $modalInstance, jenis, action, datane, tenagaahli_id, $cookieStore, $state, $stateParams, $rootScope) {
            var id_page_config = 8;
            var tahun_awal;
            var tahun_akhir;
            $scope.data = datane;
            $scope.jenis = jenis;
            $scope.action = action;
            $scope.data_lama = {};
            $scope.fileScan;
            $scope.fileTChange = function (elm) {
                $scope.fileScan = elm.files;
            };
            $scope.initial = function () {
                $rootScope.getSession().then(function (result) {
                    $rootScope.userSession = result.data.data;
                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
                    $rootScope.rekananid = $rootScope.userSession.session_data.rekanan_id;
                });
                angular.copy(datane, $scope.data_lama);
                $rootScope.fileuploadconfig(id_page_config);
                if ($scope.action == "edit") {
                    $scope.data.tahun_awal = $rootScope.convertTanggalRootAwal($scope.data.tahun_awal);
                    $scope.data.tahun_akhir = $rootScope.convertTanggalRootAwal($scope.data.tahun_akhir);
                }
            };
            $scope.batal = function () {
                angular.copy($scope.data_lama, $scope.data);
                $modalInstance.dismiss('cancel');
//                $state.transitionTo($state.current, $stateParams, {
//                    reload: true,
//                    inherit: false,
//                    notify: true
//                });
            };

            $scope.simpan = function () {
                if ($scope.jenis !== 4 && ($scope.data.tahun_awal === undefined || $scope.data.tahun_awal === "")) {
                    $.growl.error({title: "[WARNING]", message: "Tahun awal belum diisi"});
                    return;
                } else if ($scope.jenis !== 4 && ($scope.data.tahun_akhir === undefined || $scope.data.tahun_akhir === "")) {
                    $.growl.error({title: "[WARNING]", message: "Tahun akhir belum diisi"});
                    return;
                } else if ($scope.fileScan === undefined && action === 'add') {
                    $.growl.error({title: "[WARNING]", message: "File belum dipilih"});
                    return;
                } else if ($scope.data.deskripsi === undefined || $scope.data.deskripsi === "") {
                    $.growl.error({title: "[WARNING]", message: "Deskripsi belum diisi"});
                    return;
                } else {
                    if ($scope.jenis === 4) {
                        tahun_awal = "";
                        tahun_akhir = "";
                    } else {
                        tahun_awal = $rootScope.convertTanggalRoot($scope.data.tahun_awal);
                        tahun_akhir = $rootScope.convertTanggalRoot($scope.data.tahun_akhir);
                    }
                    if (action === 'add') {
                        var fileInput = $('.upload-file');
                        var extFile = $('.upload-file').val().split('.').pop().toLowerCase();
                        var maxSize = fileInput.data('max-size');
                        if (fileInput.get(0).files.length) {
                            var fileSize = fileInput.get(0).files[0].size;
                            if (fileSize > maxSize) {
                                $.growl.error({title: "[WARNING]", message: "Ukuran file terlalu besar"});
                                return;
                            } else {
                                var restrictedExt = $rootScope.limitfiletype;
                                if ($.inArray(extFile, restrictedExt) === -1) {
                                    $.growl.error({title: "[WARNING]", message: "Format file tidak valid"});
                                    return;
                                } else {
                                    upload(true);
                                }
                            }
                        }
                    } else if (action === 'edit' && $scope.fileScan === undefined) {
                        update($scope.data.scanbukti);
                    } else if (action === 'edit' && $scope.fileScan !== undefined) {
                        var fileInput = $('.upload-file');
                        var extFile = $('.upload-file').val().split('.').pop().toLowerCase();
                        var maxSize = fileInput.data('max-size');
                        if (fileInput.get(0).files.length) {
                            var fileSize = fileInput.get(0).files[0].size;
                            if (fileSize > maxSize) {
                                $.growl.error({title: "[WARNING]", message: "Ukuran file terlalu besar"});
                                return;
                            } else {
                                var restrictedExt = $rootScope.limitfiletype;
                                if ($.inArray(extFile, restrictedExt) === -1) {
                                    $.growl.error({title: "[WARNING]", message: "Format file tidak valid"});
                                    return;
                                } else {
                                    upload(false);
                                }
                            }
                        }
                    }
                }
            };

            function upload(insert) {

                $rootScope.loadLoadingModal("Menyimpan Data...");
                var fd = new FormData();
                angular.forEach($scope.fileScan, function (file) {
                    fd.append("uploads", file);
                });
                $http.post($rootScope.url_api + "upload/" + $rootScope.rekananid + "/", fd,
                        {
                            withCredentials: true,
                            transformRequest: angular.identity(),
                            headers: {'Content-Type': undefined}
                        })
                        .success(function (urldok) {
                            if (insert) {
                                $http.post($rootScope.url_api + "TAC/insertdetail", {
                                    tahun_awal: tahun_awal,
                                    tahun_akhir: tahun_akhir,
                                    deskripsi: $scope.data.deskripsi,
                                    jenis: $scope.jenis,
                                    rekanan_ta_id: tenagaahli_id,
                                    scanbukti: urldok.result.data.files[0].url,
                                    username: $rootScope.userLogin
                                }).success(function (reply) {
                                    if (reply.status === 200) {
                                        $http.post($rootScope.url_api + "deleteFile", {
                                            url: $scope.data.scanbukti
                                        }).then(function (res) {});
                                        $.growl.notice({title: "[INFO]", message: "Data berhasil disimpan"});
                                        $rootScope.unloadLoadingModal();
                                        $modalInstance.close();
                                    } else {
                                        $.growl.error({title: "[PERINGATAN]", message: "Data gagal disimpan"});
                                        $rootScope.unloadLoadingModal();
                                        return;
                                    }
                                }).error(function (err) {
                                    $.growl.error({message: "Gagal Akses API >" + err});
                                    $http.post($rootScope.url_api + "logging", {
                                        message: "Tidak berhasil akses API : " + JSON.stringify(err),
                                        source: "tenaga-ahli.js - TAC/insertdetail"
                                    }).then(function (response) {
                                        // do nothing
                                        // don't have to feedback
                                    });
                                    $rootScope.unloadLoading();
                                    return;
                                });
                            } else {
                                $http.post($rootScope.url_api + "TAC/updatedetail", {
                                    tahun_awal: tahun_awal,
                                    tahun_akhir: tahun_akhir,
                                    deskripsi: $scope.data.deskripsi,
                                    jenis: $scope.jenis,
                                    rekanan_ta_cv_id: $scope.data.rekanan_ta_cv_id,
                                    scanbukti: urldok.result.data.files[0].url,
                                    username: $rootScope.userLogin
                                }).success(function (reply) {
                                    if (reply.status === 200) {
                                        $.growl.notice({title: "[INFO]", message: "Data berhasil diubah"});
                                        $rootScope.unloadLoadingModal();
                                        $modalInstance.close();
                                    } else {
                                        $.growl.error({title: "[PERINGATAN]", message: "Data gagal diubah"});
                                        $rootScope.unloadLoadingModal();
                                        return;
                                    }
                                }).error(function (err) {
                                    $.growl.error({message: "Gagal Akses API >" + err});
                                    $http.post($rootScope.url_api + "logging", {
                                        message: "Tidak berhasil akses API : " + JSON.stringify(err),
                                        source: "tenaga-ahli.js - TAC/insertdetail"
                                    }).then(function (response) {
                                        // do nothing
                                        // don't have to feedback
                                    });
                                    $rootScope.unloadLoading();
                                    return;
                                });
                            }
                        })
                        .error(function (error) {
                            $.growl.error({title: "[PERINGATAN]", message: "Upload file scan gagal"});

                        });
            }


            function update(url) {
                $rootScope.loadLoadingModal("Menyimpan Data...");
                $http.post($rootScope.url_api + "TAC/updatedetail", {
                    tahun_awal: tahun_awal,
                    tahun_akhir: tahun_akhir,
                    deskripsi: $scope.data.deskripsi,
                    jenis: $scope.jenis,
                    rekanan_ta_cv_id: $scope.data.rekanan_ta_cv_id,
                    scanbukti: url,
                    username: $rootScope.userLogin
                }).success(function (reply) {
                    if (reply.status === 200) {
                        $.growl.notice({title: "[INFO]", message: "Data berhasil diubah"});
                        $rootScope.unloadLoadingModal();
                        $modalInstance.close();
                    } else {
                        $.growl.error({title: "[PERINGATAN]", message: "Data gagal diubah"});
                        $rootScope.unloadLoadingModal();
                        return;
                    }
                }).error(function (err) {
                    $.growl.error({message: "Gagal Akses API >" + err});
                    $http.post($rootScope.url_api + "logging", {
                        message: "Tidak berhasil akses API : " + JSON.stringify(err),
                        source: "tenaga-ahli.js - TAC/insertdetail"
                    }).then(function (response) {
                        // do nothing
                        // don't have to feedback
                    });
                    $rootScope.unloadLoading();
                    return;
                });
            }
        });
        