angular.module('eprocAppPanitia')
        .controller('pendaftaranPrakualifikasiCtrl', function($scope, $http, $rootScope, $modal, $stateParams, $cookieStore) {

            $scope.flowpaket_id = Number($stateParams.flowpaket_id);
            $scope.paket_lelang_id = Number($stateParams.paket_lelang_id);
            $scope.pendaftaran = [];
            $scope.userBisaNgatur = false;
            $scope.page_id = 103;
            $scope.nama_paket = "";
            $scope.nama_tahapan = "";
            $scope.is_created = false;
            $scope.status = -1;
            $scope.peserta = [];
            $scope.menuhome = 0;
            $scope.labelcurr;
//            eb.onopen = function() {
//               $rootScope.autorize();
//                $scope.init();
//            };

            $scope.init = function(){
                $scope.menuhome = $rootScope.menuhome;
                $rootScope.getSession().then(function(result) {
                    $rootScope.userSession = result.data.data;
                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
                    $rootScope.authorize(loadPendaftaran());
                });
            };

            function loadPendaftaran() {
                var arr = [];
                arr.push($rootScope.userLogin);
                arr.push($scope.paket_lelang_id);
                arr.push($scope.page_id);
                $rootScope.loadLoading("Silahkan Tunggu...");
                //itp.panitia.cekBisaMengatur
                $http.post($rootScope.url_api + 'panitia/cekbisamengatur', {
                    param: arr, 
                    page_id: $scope.page_id
                }).success(function(reply) {
                    if (reply.status === 200 && reply.result.data.length > 0) {
                        $scope.userBisaNgatur = reply.result.data[0].bisa_mengatur;
                    }
                }).error(function(err) {
                    $.growl.error({ message: "Gagal Akses API >"+err });
                    return;
                });
                
                //itp.paketDetail.getInfo
                $http.post($rootScope.url_api + 'paket/detail/info', {
                    paket_lelang_id: $scope.paket_lelang_id,
                    flowpaket_id: $scope.flowpaket_id
                }).success(function(reply) {
                    if (reply.status === 200) {
                        $scope.nama_paket = reply.result.data.result[0].nama_paket;
                        $scope.nama_tahapan = reply.result.data.result[0].nama_tahapan;
                        $scope.labelcurr = reply.result.data.result[0].label;
                        $scope.is_created = reply.result.data.result[0].is_created;
                        $scope.status = reply.result.data.result[0].status;
                    }
                }).error(function(err) {
                    $.growl.error({ message: "Gagal Akses API >"+err });
                    return;
                });

                var param = [];
                param.push($scope.flowpaket_id);
                //itp.pendaftaranLelang.select
                $http.post($rootScope.url_api + 'pendaftaranlelang/select', {
                    param: param, 
                    page_id: $scope.page_id,
                    paket_lelang_id: $scope.paket_lelang_id
                }).success(function(reply) {
                    $rootScope.unloadLoading();
                    if (reply.status === 200) {
                        $scope.pendaftaran = reply.result.data;
                    }
                }).error(function(err) {
                    $.growl.error({ message: "Gagal Akses API >"+err });
                    return;
                });
                
                //itp.pendaftaranLelang.cekYangSudahMendaftar
                $http.post($rootScope.url_api + 'pendaftaranlelang/cek', {
                    paket_lelang_id: $scope.paket_lelang_id
                }).success(function(reply) {
                    if (reply.status === 200) {
                        $scope.peserta = reply.result.data;
                        for(var i=0;i<$scope.peserta.length;i++){
                            $scope.peserta[i].tgl_daftar = convertTanggal2($scope.peserta[i].tgl_daftar);
                        }
                    }
                }).error(function(err) {
                    $.growl.error({ message: "Gagal Akses API >"+err });
                    return;
                });
            };
            
            function convertTanggal2(input) {
                return input ? $rootScope.convertTanggal(input) : "";
            };

            $scope.uploadDokumen = function(pen) {
                var modalInstance = $modal.open({
                    templateUrl: 'upload-dokumen.html',
                    controller: uploadDokumenCtrl2,
                    resolve: {
                        item: function() {
                            return pen;
                        }
                    }
                });
                modalInstance.result.then(function() {
                    loadPendaftaran();
                });
            };

            $scope.lihatPendaftaranLelang = function(pen) {
                var modalInstance = $modal.open({
                    templateUrl: 'lihat-pendaftaran-lelang.html',
                    controller: lihatPendaftaranLelangCtrl,
                    resolve: {
                        item: function() {
                            return pen;
                        }
                    }
                });
            };
        });

var uploadDokumenCtrl2 = function($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
    var flowpaket_id = Number(item.flowpaket_id);
    var paket_id = Number(item.paket_lelang_id);
    $scope.nama_paket = item.nama_paket;
    $scope.page_id = 103;
    $scope.file;
    //Set max n type file
    $rootScope.fileuploadconfig(15);

    $scope.filesTChanged = function(elm) {
        $scope.file = elm.files;
        $scope.$apply();
    };

    $scope.simpan = function() {
        if ($scope.file === undefined) {
            $.growl.error({title: "[WARNING]", message: "Pilih File akan di upload terlebih dahulu"});
            return;
        }
        else {
            var fileInput = $('.upload-file');
            var extFile = $('.upload-file').val().split('.').pop().toLowerCase();
            var maxSize = fileInput.data('max-size');
            if (fileInput.get(0).files.length) {
                var fileSize = fileInput.get(0).files[0].size;
                if (fileSize > maxSize) {
                    //console.info("isine if maxSize --> " + maxSize);
                    $.growl.error({title: "[WARNING]", message: "Ukuran file terlalu besar"});
                    return;
                } else {
                    var restrictedExt = $rootScope.limitfiletype;
                    if ($.inArray(extFile, restrictedExt) == -1) {
                        $.growl.error({title: "[WARNING]", message: "Format file tidak valid"});
                        return;
                    } else {
                        $rootScope.authorize(upload());
                    }
                }
            }
        }
    };

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };

    function upload() {
        //upload file ke server
        //panggil fileUploader.js dengan parameter file dan id paket
        var fd = new FormData();
        angular.forEach($scope.file, function(file) {
            fd.append("uploads", file);
        });
        $http.post($rootScope.url_api + "/upload/" + paket_id + "/", fd, {
            withCredentials: true,
            transformRequest: angular.identity(),
            headers: {'Content-Type': undefined}
        })
        .success(function(reply) {
            if(reply.status === 200){
                var result = reply.result.data;
                update(result.files[0].url);
            }
            else{
                $.growl.error({ message: "Gagal melakukan upload" });
                return;
            }
        })
        .error(function(err) {
            $.growl.error({ message: "Gagal Akses API >"+err });
            return;
        });
    };

    function update(url) {
        var param = [];
        //butuh isi_pengumuman_pengadaan, url_dokumen, flowpaket_id
        param.push(url);
        param.push(Number(flowpaket_id));
        
        //itp.pendaftaranLelang.update
        $http.post($rootScope.url_api + 'pendaftaranlelang/update', {
            username : $rootScope.userLogin,
            page_id: $scope.page_id, 
            param: param, 
            nama_paket: $scope.nama_paket, 
            paket_lelang_id: paket_id
        }).success(function(reply) {
            if (reply.status === 200){
                $.growl.notice({title: "[INFO]", message: "Berhasil mengupload dokumen dan merecord user activity"});
                $modalInstance.close();
            }
            else{
                $.growl.error({title: "[PERINGATAN]", message: "Gagal mengupload dokumen "});
            }
        }).error(function(err) {
            $.growl.error({ message: "Gagal Akses API >"+err });
            return;
        });
    };

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };
};

var lihatPendaftaranLelangCtrl = function($scope, $modalInstance, item) {
    $scope.pendaftaran = item;
    ////console.info("hahaaa = "+item.dokumen_file);

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };
};