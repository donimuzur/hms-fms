angular.module('eprocAppPanitia')
        .directive('numberMasking', function () {
            return {
                require: 'ngModel',
                link: function (scope, element, attrs, modelCtrl) {
                    modelCtrl.$parsers.push(function (inputValue) {
                        if (inputValue === undefined)
                            return '';
                        var transformedInput = inputValue.replace(/[^0-9]/g, '');
                        if (transformedInput !== inputValue) {
                            modelCtrl.$setViewValue(transformedInput);
                            modelCtrl.$render();
                        }
                        return transformedInput;
                    });
                }
            };
        })
        .controller('equipmentStandardCtrl', function ($scope, $modal, $http, $cookieStore, $state, $rootScope) {
            $scope.bisaMengubahData;
            $scope.dataalat = [];
            $scope.dataNSE = [];
            $scope.dataSE = [];

            $scope.loadfirst = function () {
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
                $scope.dataalat = [];
                $scope.dataNSE = [];
                $scope.dataSE = [];
                $http.post($rootScope.url_api + "DPC/getall", {
                    rekanan_id: $rootScope.rekananid
                }).success(function (reply) {
                    if (reply.status === 200) {
                        var data = reply.result.data;
                        $scope.dataalat = data;
                        for (i = 0; i < $scope.dataalat.length; i++) {
                            if ($scope.dataalat[i].jenis_alat === 'NSE') {
                                $scope.dataNSE.push($scope.dataalat[i]);
                            }
                            if ($scope.dataalat[i].jenis_alat === 'SE') {
                                $scope.dataSE.push($scope.dataalat[i]);
                            }
                        }
                        if ($scope.dataalat.length > 0) {
                            $rootScope.insertStatusIsiData($rootScope.rekananid, 'dpl', 1);
                        } else {
                            $rootScope.insertStatusIsiData($rootScope.rekananid, 'dpl', 0);
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
                        source: "tenaga-ahli.js - DPC/getall"
                    }).then(function (response) {
                        // do nothing
                        // don't have to feedback
                    });
                    $rootScope.unloadLoading();
                    return;
                });
            }

            /* awal fungsi detail perlengkapan by ani */
            $scope.detailAlat = function (idAlat) {
                var modalInstance = $modal.open({
                    templateUrl: 'detailAlatModal.html',
                    controller: detailAlatModal,
                    resolve: {
                        item: function () {
                            return idAlat;
                        }
                    }
                });
                modalInstance.result.then(function () {
                    $scope.loadfirst();
                });
            };
            /* akhir fungsi detail perlengkapan by ani */
            $scope.action = -1;
            $scope.resolvedata = {};
            $scope.openmodal = function (act, data, kategori) {
                $scope.action = act === "add" ? 0 : 1;
                $scope.resolvedata = act === "add" ? {} : data;
                var opnmdl;
                var kategori_name;
                if(kategori === 1){
                    kategori_name = "Data Bangunan dan Infrastruktur";
                    opnmdl = $modal.open({
                        templateUrl: "mdlBangunan.html",
                        controller: "mdlBangunan",
                        resolve: {
                            action: function () {
                                return $scope.action;
                            },
                            datane: function () {
                                return $scope.resolvedata;
                            }
                        }
                    });
                }
                else{
                    if(kategori === 2){
                        kategori_name = "Kendaraan";
                    }
                    else{
                        kategori_name = "Peralatan";
                    }
                    opnmdl = $modal.open({
                        templateUrl: "mdlAUalat.html",
                        controller: "mdlAUalat",
                        resolve: {
                            action: function () {
                                return $scope.action;
                            },
                            datane: function () {
                                return $scope.resolvedata;
                            }
                        }
                    });
                }
                opnmdl.result.then(function () {
                    $scope.loadfirst();
                });
            };
            $scope.hapus = function (idAlat, url) {
                bootbox.confirm('<h3 class="afta-font">Yakin menghapus data ?</h3>', function (res) {
                    if (res) {
                        $http.post($rootScope.url_api + "DPC/delete", {
                            id_alat: idAlat,
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
                                $rootScope.unloadLoading();
                                $scope.loadfirst();
                            } else {
                                $.growl.error({title: "[PERINGATAN]", message: "Data gagal dihapus"});
                                $rootScope.unloadLoading();
                                return;
                            }
                        }).error(function (err) {
                            $.growl.error({message: "Gagal Akses API >" + err});
                            $http.post($rootScope.url_api + "logging", {
                                message: "Tidak berhasil akses API : " + JSON.stringify(err),
                                source: "tenaga-ahli.js - DPC/insert"
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
        .controller('mdlBangunan', function ($scope, $http, $modalInstance, action, datane, $cookieStore, $state, $stateParams, $rootScope) {
            $scope.batal = function () {
                angular.copy($scope.data_lama, $scope.data);
                $modalInstance.dismiss('cancel');
            };
        })
        .controller('mdlAUalat', function ($scope, $http, $modalInstance, action, datane, $cookieStore, $state, $stateParams, $rootScope) {
            var id_page_config = 7;
            $scope.data = {};
            $scope.data_lama = {};
            $scope.file;
            $scope.action = action;
            $scope.filesChange = function (elm) {
                $scope.file = elm.files;
            };
            $scope.initial = function () {
                $rootScope.getSession().then(function (result) {
                    $rootScope.userSession = result.data.data;
                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
                    $rootScope.rekananid = $rootScope.userSession.session_data.rekanan_id;
                });
                $rootScope.fileuploadconfig(id_page_config);
                $scope.data = action === 0 ? {} : datane;
                if (action > 0) {
                    angular.copy(datane, $scope.data_lama);
                }
            };

            $scope.simpan = function () {
                if ($scope.data.nama_alat === undefined || $scope.data.nama_alat === '') {
                    $.growl.error({title: "[WARNING]", message: "Nama alat belum diisi"});
                    return;
                } else if ($scope.data.jumlah_alat === undefined || $scope.data.jumlah_alat === '') {
                    $.growl.error({title: "[WARNING]", message: "Jumlah belum diisi"});
                    return;
                } else if ($scope.data.merk_alat === undefined || $scope.data.merk_alat === '') {
                    $.growl.error({title: "[WARNING]", message: "Merk alat belum diisi"});
                    return;
                } else if (action === 0 && $scope.file === undefined) {
                    $.growl.error({title: "[WARNING]", message: "File belum dipilih"});
                    return;
                } else if ($scope.data.lokasi_alat === undefined || $scope.data.lokasi_alat === "") {
                    $.growl.error({title: "[WARNING]", message: "Lokasi belum diisi"});
                    return;
                } else if ($scope.data.status_alat === undefined || $scope.data.status_alat === '') {
                    $.growl.error({title: "[WARNING]", message: "Status alat belum diisi"});
                    return;
                } else if ($scope.data.kondisi_alat === undefined || $scope.data.kondisi_alat === '') {
                    $.growl.error({title: "[WARNING]", message: "Kondisi alat belum diisi"});
                    return;
                } else if (Number($scope.data.kondisi_alat) > 100) {
                    $.growl.error({title: "[WARNING]", message: "Masukkan Kondisi Persentase yang valid (0-100)"});
                    return;
                } else if ($scope.data.jenis_alat === undefined || $scope.data.jenis_alat === '') {
                    $.growl.error({title: "[WARNING]", message: "Jenis perlengkapan belum diisi"});
                    return;
                } else {
                    if ($scope.data.buktikpm_alat === undefined || $scope.data.buktikpm_alat === '') {
                        $scope.data.buktikpm_alat = "";
                    }
                    if ($scope.data.kapasitas_alat === undefined || $scope.data.kapasitas_alat === '') {
                        $scope.data.kapasitas_alat = "";
                    }
                    if ($scope.data.thn_alat === undefined || $scope.data.thn_alat === "") {
                        $scope.data.thn_alat = "";
                    }
                    if (action === 0) {
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
                    } else if (action === 1 && $scope.file === undefined) {
                        update();
                    } else if (action === 1 && $scope.file !== undefined) {
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
            $scope.batal = function () {
                angular.copy($scope.data_lama, $scope.data);
                $modalInstance.dismiss('cancel');
            };

            function update() {
                $rootScope.loadLoadingModal("Menyimpan Data...");
                $http.post($rootScope.url_api + "DPC/update", {
                    id_alat: $scope.data.id_alat,
                    nama_alat: $scope.data.nama_alat,
                    jumlah_alat: parseInt($scope.data.jumlah_alat),
                    buktikpm_alat: $scope.data.buktikpm_alat,
                    kapasitas_alat: $scope.data.kapasitas_alat,
                    merk_alat: $scope.data.merk_alat,
                    thn_alat: $scope.data.thn_alat,
                    lokasi_alat: $scope.data.lokasi_alat,
                    status_alat: $scope.data.status_alat,
                    kondisi_alat: parseInt($scope.data.kondisi_alat),
                    jenis_alat: $scope.data.jenis_alat,
                    rekanan_id: $rootScope.rekananid,
                    dokumenurl_alat: $scope.data.dokumenurl_alat,
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
                        source: "tenaga-ahli.js - DPC/update"
                    }).then(function (response) {
                        // do nothing
                        // don't have to feedback
                    });
                    $rootScope.unloadLoadingModal();
                    return;
                });
            }

            function upload(insert) {
                $rootScope.loadLoadingModal("Upload File...");
                var fd = new FormData();
                angular.forEach($scope.file, function (file) {
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
                                $http.post($rootScope.url_api + "DPC/insert", {
                                    nama_alat: $scope.data.nama_alat,
                                    jumlah_alat: parseInt($scope.data.jumlah_alat),
                                    buktikpm_alat: $scope.data.buktikpm_alat,
                                    kapasitas_alat: $scope.data.kapasitas_alat,
                                    merk_alat: $scope.data.merk_alat,
                                    thn_alat: $scope.data.thn_alat,
                                    lokasi_alat: $scope.data.lokasi_alat,
                                    status_alat: $scope.data.status_alat,
                                    kondisi_alat: parseInt($scope.data.kondisi_alat),
                                    jenis_alat: $scope.data.jenis_alat,
                                    rekanan_id: $rootScope.rekananid,
                                    dokumenurl_alat: urldok.result.data.files[0].url,
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
                                        source: "tenaga-ahli.js - DPC/insert"
                                    }).then(function (response) {
                                        // do nothing
                                        // don't have to feedback
                                    });
                                    $rootScope.unloadLoadingModal();
                                    return;
                                });
                            } else {
                                $http.post($rootScope.url_api + "DPC/update", {
                                    id_alat: $scope.data.id_alat,
                                    nama_alat: $scope.data.nama_alat,
                                    jumlah_alat: parseInt($scope.data.jumlah_alat),
                                    buktikpm_alat: $scope.data.buktikpm_alat,
                                    kapasitas_alat: $scope.data.kapasitas_alat,
                                    merk_alat: $scope.data.merk_alat,
                                    thn_alat: $scope.data.thn_alat,
                                    lokasi_alat: $scope.data.lokasi_alat,
                                    status_alat: $scope.data.status_alat,
                                    kondisi_alat: parseInt($scope.data.kondisi_alat),
                                    jenis_alat: $scope.data.jenis_alat,
                                    rekanan_id: $rootScope.rekananid,
                                    dokumenurl_alat: urldok.result.data.files[0].url,
                                    username: $rootScope.userLogin
                                }).success(function (reply) {
                                    if (reply.status === 200) {
                                        $http.post($rootScope.url_api + "deleteFile", {
                                            url: $scope.data.dokumenurl_alat
                                        }).then(function (res) {
//                                    $.growl.notice({title: "[INFO]", message: "Anda Telah Menghapus Data Dokumen Lain-lain"});
//                                    $rootScope.unloadLoading();
//                                    $scope.initial();
                                        });
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
                                        source: "tenaga-ahli.js - DPC/update"
                                    }).then(function (response) {
                                        // do nothing
                                        // don't have to feedback
                                    });
                                    $rootScope.unloadLoadingModal();
                                    return;
                                });
                            }
                        })
                        .error(function (error) {
                            $.growl.error({title: "[PERINGATAN]", message: "File gagal diupload"});
                        });
            }
        });
/* detail rincian perlengkapan atau alat */
var detailAlatModal = function ($scope, $modalInstance, item, $cookieStore, $rootScope) {
    $scope.page_id = 129;
    $scope.selectedAlat = item;

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};
        