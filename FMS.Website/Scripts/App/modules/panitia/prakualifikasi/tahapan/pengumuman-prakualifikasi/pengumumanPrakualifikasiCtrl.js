angular.module('eprocAppPanitia')
        .controller('pengumumanPrakualifikasiCtrl', function($scope, $http, $rootScope, $modal, $stateParams, $cookieStore) {
            $scope.flowpaket_id = Number($stateParams.flowpaket_id);
            $scope.paket_lelang_id = Number($stateParams.paket_lelang_id);
            $scope.pengumuman = [];
            $scope.userBisaNgatur = false;
            $scope.page_id = 102;
            $scope.page_file = 14;
            $scope.nama_paket;
            $scope.nama_tahapan;
            $scope.is_created;
            $scope.status = -1;
            $scope.menuhome = 0;
            $scope.labelcurr;

            $scope.loadAwal = function(){
                $scope.menuhome = $rootScope.menuhome;
                $rootScope.fileuploadconfig($scope.page_file);
                $rootScope.getSession().then(function(result){
                    $rootScope.userSession = result.data.data;
                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
                    $rootScope.authorize(loadPengumuman());
                });
            };

            function loadPengumuman() {
                var arr = [];
                arr.push($rootScope.userLogin);
                arr.push($scope.paket_lelang_id);
                arr.push($scope.page_id);
                //itp.panitia.cekBisaMengatur
                $http.post($rootScope.url_api + 'panitia/cekbisamengatur', {
                    param: arr,
                    page_id: $scope.page_id
                }).success(function(reply) {
                  //  console.info("atur: "+JSON.stringify(reply));
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
                     //console.info("detail: "+JSON.stringify(reply));
                    if (reply.status === 200) {
                        $scope.nama_paket = reply.result.data.result[0].nama_paket;
                        $scope.nama_tahapan = reply.result.data.result[0].nama_tahapan;
                        $scope.is_created = reply.result.data.result[0].is_created;
                        $scope.status = reply.result.data.result[0].status;
                        $scope.labelcurr = reply.result.data.result[0].label;
                    }
                }).error(function(err) {
                    $.growl.error({ message: "Gagal Akses API >"+err });
                    return;
                }); 

                var param = [];
                param.push($scope.flowpaket_id);
                //itp.pengumumanPengadaan.select
                $http.post($rootScope.url_api + 'pengumumanpengadaan/select', {
                    param: param,
                    page_id: $scope.page_id,
                    paket_lelang_id: $scope.paket_lelang_id
                }).success(function(reply) {
                     console.info("umum: "+JSON.stringify(reply));
                    if (reply.status === 200) {
                        $scope.pengumuman = reply.result.data;
                    }
                }).error(function(err) {
                    $.growl.error({ message: "Gagal Akses API >"+err });
                    return;
                }); 
            };

            $scope.buatpengumuman = function() {
                var lempar = {
                    flowpaket_id: $scope.flowpaket_id,
                    paket_lelang_id: $scope.paket_lelang_id,
                    nama_paket: $scope.nama_paket,
                    nama_tahapan: $scope.nama_tahapan
                };
                var modalInstance = $modal.open({
                    templateUrl: 'pengumuman-pengadaan.html',
                    controller: pengumumanPengadaanCreateCtrl,
                    resolve: {
                        item: function() {
                            return lempar;
                        }
                    }
                });
                modalInstance.result.then(function() {
                    loadPengumuman();
                });
            };

            $scope.lihatPengumuman = function(pgm) {
                var modalInstance = $modal.open({
                    templateUrl: 'lihat-pengumuman-pengadaan.html',
                    controller: pengumumanPengadaanViewCtrl,
                    resolve: {
                        item: function() {
                            return pgm;
                        }
                    }
                });
            };

            $scope.editPengumuman = function(pgm) {
                var lempar = {
                    flowpaket_id: $scope.flowpaket_id,
                    paket_lelang_id: $scope.paket_lelang_id,
                    nama_paket: $scope.nama_paket,
                    nama_tahapan: $scope.nama_tahapan,
                    pengumuman: pgm
                };
                var modalInstance = $modal.open({
                    templateUrl: 'ubah-pengumuman-pengadaan.html',
                    controller: pengumumanPengadaanEditCtrl,
                    resolve: {
                        item: function() {
                            return lempar;
                        }
                    }
                });
                modalInstance.result.then(function() {
                    loadPengumuman();
                });
            };

        });

var pengumumanPengadaanCreateCtrl = function($scope, $rootScope, $modalInstance, $http, item, $cookieStore, $modal) {
    var flowpaket_id = item.flowpaket_id;
    var paket_id = item.paket_lelang_id;
    var nama_address = "";
    var email_sender = "";
    $scope.nama_paket = item.nama_paket;
    $scope.nama_tahapan = item.nama_tahapan;
    $scope.page_id = 102;
    $scope.newPengumuman = new Pengumuman("", "", "");
    $scope.file;
    $scope.bidangChecked = false;
    $scope.bidangUsaha = [];
    $scope.subBidang = [];
    $scope.selectedBidang;
    $scope.selectedSub;
    $scope.emailChecked = false;
    $scope.alamatTujuan = "";
    $scope.tempEmail = [];
    $scope.selectedBidangUsaha = [];
    $scope.selectedRekanan = [];
    $scope.selectedKualifikasi;
    $scope.terbuka = false;
    $scope.subjectEmail = "";
    $scope.labelcurr;
    $scope.customTinymce = {
        theme: "modern",
        plugins: [
            "advlist autolink lists link image charmap print preview hr anchor pagebreak",
            "searchreplace wordcount visualblocks visualchars fullscreen",
            "insertdatetime media nonbreaking save table contextmenu directionality",
            "emoticons template paste textcolor"
        ],
        toolbar1: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
        toolbar2: "print preview media | forecolor backcolor emoticons",
        image_advtab: true,
        height: "200px",
        width: "auto"
    };

    $scope.loadAwal = function() {
        $rootScope.fileuploadconfig(14);
        
        $scope.kualifikasi = [];
        $scope.kualifikasi.push({kualifikasi:'Semua'});
        $scope.kualifikasi.push({kualifikasi:'Besar'});
        $scope.kualifikasi.push({kualifikasi:'Menengah'});
        $scope.kualifikasi.push({kualifikasi:'Kecil'});
        $scope.kualifikasi.push({kualifikasi:'Mikro'});
        $scope.selectedKualifikasi = $scope.kualifikasi[0];
        $scope.subjectEmail = 'eProcurement PT. Asuransi Jiwasraya (Persero) - ' + $scope.nama_paket;
    };

    $scope.insertPekerjaan = function() {
        $scope.subPekerjaan = [];
        var isi = '';
        $rootScope.loadLoadingModal("Silahkan Tunggu...");
        //itp.paketDetail.getInfo
        $http.post($rootScope.url_api + 'paket/detail/info', {
            paket_lelang_id: paket_id,
            flowpaket_id: flowpaket_id
        }).success(function(reply) {
            if (reply.status === 200) {
                $scope.labelcurr = reply.result.data.result[0].label;
                //itp.prSubPekerjaaan.selectBesertaPRLine
                $http.post($rootScope.url_api + 'prsubp/selectprline', {
                    pr_id: reply.result.data.result[0].pr_id,
                    keyword: "%%"
                }).success(function(reply) {
                    if (reply.status === 200) {
                        $scope.subPekerjaan = reply.result.data;
                        isi += '<ol type="I">';
                        $scope.subPekerjaan.forEach(function(sub) {
                            isi += '<li>';
                            isi += sub.nama;
                            if (sub.anak && sub.anak.length > 0) {
                                isi += '<ol type="A">';
                                sub.anak.forEach(function(an) {
                                    isi += '<li>';
                                    isi += '&nbsp;' + an.nama;
                                    isi += '<ol type="1">';
                                    an.pr_line.forEach(function(pr) {
                                        isi += '<li>';
                                        isi += '&nbsp; &nbsp;' + pr.prl_nama + '<br/>';
                                        isi += '</li>';
                                    });
                                    isi += '</ol>';
                                    isi += '</li>';
                                });
                                isi += '</ol>';
                            } else {
                                isi += '<ol type="1">';
                                sub.pr_line.forEach(function(pr) {
                                    isi += '<li>';
                                    isi += '&nbsp;' + pr.prl_nama + '<br/>';
                                    isi += '</li>';
                                });
                                isi += '</ol>';
                            }
                            isi += '</li>';
                        });
                        isi += '</ol>';
                        //alert(isi);
                        tinymce.get('isi_pengumuman_create').setContent($scope.newPengumuman.isi_pengumuman_pengadaan + '<br/>' + isi);
                        //$scope.newPengumuman.isi_pengumuman_pengadaan = $scope.newPengumuman.isi_pengumuman_pengadaan + '<br/>' + isi;
                        //alert($scope.newPengumuman.isi_pengumuman_pengadaan);
                        $rootScope.unloadLoadingModal();
                    }
                });
            }
        });
    };

    $scope.ubahCentangBidang = function(obj) {
        $scope.bidangChecked = obj;
        if ($scope.bidangChecked === false) {
            $scope.selectedBidangUsaha = [];
            $scope.selectedKualifikasi = $scope.kualifikasi[0];
            $scope.newPengumuman.emailFromCombo = "";
        }
    };

    $scope.ubahCentangEmail = function(obj) {
        $scope.emailChecked = obj;
        if ($scope.emailChecked === false){
            $scope.newPengumuman.emailInput = "";
        }
    };

    $scope.ubahTerbuka = function(obj) {
        $scope.terbuka = obj;
    };
    
    $scope.ubahKualifikasi = function(obj) {
        $scope.selectedKualifikasi = obj;
        $scope.getEmail();
    };
    
    $scope.onTambahBidangUsahaClick = function(){
        var modal = $modal.open({
            templateUrl: 'pilihBidangUsaha.html',
            controller: modalPilihBidangUsahaCtrl,
            resolve: {
                data: function() {
                    return $scope.selectedBidangUsaha;
                }
            }
        });
        modal.result.then(function(bu) {
            $scope.selectedBidangUsaha.push({
                id_bidang_usaha : bu.id_bidang_usaha,
                nama_bidang_usaha : bu.nama_bidang_usaha,
                string_bidang_usaha : bu.string_bidang_usaha
            });
            $scope.getEmail();
        });
    };

    $scope.onTambahRekananClick = function(){
        var modal = $modal.open({
            templateUrl: 'pilihRekanan.html',
            controller: modalPilihRekananCtrl,
            resolve: {
                data: function() {
                    return $scope.selectedRekanan;
                }
            }
        });
        modal.result.then(function(r) {
            $scope.selectedRekanan.push({
                rekanan_id : r.rekanan_id,
                nama_perusahaan : r.nama_perusahaan,
                email : r.email
            });
            $scope.$apply();
        });
    };

    $scope.onHapusBidangUsahaClick = function(bu){
        for(var i = 0; i < $scope.selectedBidangUsaha.length; i++) {
            if($scope.selectedBidangUsaha[i].id_bidang_usaha === bu.id_bidang_usaha) {
               $scope.selectedBidangUsaha.splice(i, 1);
               break;
            }
        }
        $scope.getEmail();
    };
    
    $scope.onHapusRekananClick = function(r){
        for(var i = 0; i < $scope.selectedRekanan.length; i++) {
            if($scope.selectedRekanan[i].rekanan_id === r.rekanan_id) {
               $scope.selectedRekanan.splice(i, 1);
               break;
            }
        }
        $scope.$apply();
    };
    
    $scope.filesTChanged = function(elm) {
        $scope.file = elm.files;
        $scope.$apply();
    };

    $scope.getEmail = function() {
        $scope.newPengumuman.emailFromCombo = "";
        if ($scope.selectedBidangUsaha.length > 0) {
            var param = [];
            $scope.selectedBidangUsaha.forEach(function(bu){
                param.push(bu.id_bidang_usaha);
            });
            $rootScope.loadLoadingModal("Silahkan Tunggu...");
            //itp.rekanan.getEmailByBidang
            $http.post($rootScope.url_api + 'pengumumanpengadaan/mailbybidang', {
                page_id: $scope.page_id,
                paket_lelang_id: paket_id,
                kualifikasi : $scope.selectedKualifikasi.kualifikasi,
                param: param
            }).success(function(reply) {
                if (reply.status === 200) {
                    $scope.tempEmail = reply.result.data;
                    for (var i = 0; i < $scope.tempEmail.length; i++)
                        $scope.newPengumuman.emailFromCombo = $scope.newPengumuman.emailFromCombo + $scope.tempEmail[i].email + ", ";
                } else {
                    $.growl.error({title: "[WARNING]", message: "Gagal mendapat daftar email"});
                }
                $rootScope.unloadLoadingModal();
            });
        }
    };

    $scope.simpan = function() {
        if ($scope.newPengumuman.isi_pengumuman_pengadaan === "") {
            $.growl.error({title: "[PERINGATAN]", message: "Isi pengumuman belum dimasukkan"});
            return;
        }
        if (($scope.bidangChecked && $scope.newPengumuman.emailFromCombo === "") || ($scope.emailChecked && $scope.newPengumuman.emailInput === "")) {
            $.growl.error({title: "[PERINGATAN]", message: "Alamat email masih kosong"});
            return;
        }
        if ($scope.terbuka == false && $scope.selectedBidangUsaha.length === 0 && $scope.selectedRekanan.length === 0 ) {
            $.growl.error({title: "[PERINGATAN]", message: "Anda memilih pengumuman tertutup, Anda harus memilih bidang usaha atau memilih rekanan"});
            return;
        }
        console.info(JSON.stringify($scope.selectedBidangUsaha));
        console.info(JSON.stringify($scope.selectedRekanan));
        console.info("terbuka: "+$scope.terbuka);
        if ($scope.terbuka == true && ($scope.selectedBidangUsaha.length > 0 || $scope.selectedRekanan.length > 0) ) {
            $.growl.error({title: "[PERINGATAN]", message: "Anda memilih pengumuman terbuka, Anda tidak perlu memilih bidang usaha atau memilih rekanan"});
            return;
        }
        /* cek file yang akan di upload */
        if ($scope.file === undefined) {
            $rootScope.authorize(insert(""));
        }
        else {
            var fileInput = $('.upload-file');
            var extFile = $('.upload-file').val().split('.').pop().toLowerCase();
            var maxSize = fileInput.data('max-size');
            if (fileInput.get(0).files.length) {
                var fileSize = fileInput.get(0).files[0].size;
                if (fileSize > maxSize) {
                    $.growl.error({title: "[WARNING]", message: "Ukuran file terlalu besar"});
                    $rootScope.unloadLoadingModal();
                    return;
                } else {
                    var restrictedExt = $rootScope.limitfiletype;
                    if ($.inArray(extFile, restrictedExt) == -1) {
                        $.growl.error({title: "[WARNING]", message: "Format file tidak valid"});
                        $rootScope.unloadLoadingModal();
                        return;
                    } else {
                        $rootScope.authorize(upload());
                    }
                }
            }
        }
        /* end cek file */
    };

    function upload() {
        $rootScope.loadLoadingModal("Silahkan Tunggu...");
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
            $rootScope.unloadLoadingModal();
            if(reply.status === 200){
                var result = reply.result.data;
                insert(result.files[0].url);
            }
            else{
                $.growl.error({ message: "Gagal melakukan upload" });
                return;
            }
        })
        .error(function(err) {
            $.growl.error({ message: "Gagal Akses API >"+err });
            $rootScope.unloadLoadingModal();
            return;
        });
    };

    function insert(url) {
        var id_bidang;
        var id_sub_bidang;
        if ($scope.selectedBidang === undefined || $scope.selectedBidang === null) {
            id_bidang = -1;
        }
        else {
            id_bidang = $scope.selectedBidang.id_bidang_usaha;
        }
        if ($scope.selectedSub === undefined || $scope.selectedSub === null) {
            id_sub_bidang = -1;
        }
        else {
            id_sub_bidang = $scope.selectedSub.id_sub_bidang;
        }
        var param = [];
        param.push($scope.newPengumuman.isi_pengumuman_pengadaan);
        param.push(url);
        param.push(flowpaket_id);
        param.push($scope.terbuka);
        param.push($scope.newPengumuman.emailFromCombo);
        param.push($scope.newPengumuman.emailInput);
        param.push($scope.selectedKualifikasi.kualifikasi);
        $rootScope.loadLoadingModal("Silahkan Tunggu...");
        //itp.pengumumanPengadaan.insert
        $http.post($rootScope.url_api + 'pengumumanpengadaan/insert', {
            username: $rootScope.userLogin,
            page_id: $scope.page_id,
            param: param,
            paket_lelang_id: paket_id,
            selectedBidangUsaha: $scope.selectedBidangUsaha,
            selectedRekanan: $scope.selectedRekanan,
            flow_paket_id: flowpaket_id,
            nama_tahapan: $scope.nama_tahapan,
            nama_paket: $scope.nama_paket
        }).success(function(reply) {
            if (reply.status === 200) {
                if ($scope.newPengumuman.emailFromCombo !== "" && $scope.newPengumuman.emailInput !== "")
                    $scope.alamatTujuan = $scope.newPengumuman.emailFromCombo + $scope.newPengumuman.emailInput;
                else if ($scope.newPengumuman.emailFromCombo === "")
                    $scope.alamatTujuan = $scope.newPengumuman.emailInput;
                else if ($scope.newPengumuman.emailInput === "")
                    $scope.alamatTujuan = $scope.newPengumuman.emailFromCombo;
                $scope.selectedRekanan.forEach(function(r){
                    $scope.alamatTujuan += (',' + r.email);
                });
                if ($scope.alamatTujuan !== "") {
                    var alamat = $scope.alamatTujuan.split(',');
                    for (var i = 0; i < alamat.length; i++) {
                        $http.post($rootScope.url_api + 'email/send', {
                            recipients: [
                                {
                                    email : alamat[i],
                                    name : ''
                                }
                            ],
                            subject: $scope.subjectEmail,
                            content: $scope.newPengumuman.isi_pengumuman_pengadaan,
                            attachments: []
                        }).success(function(reply) {
                        });
                    }
                    $.growl.notice({title: "[INFO]", message: "Berhasil membuat pengumuman"});
                    $rootScope.unloadLoadingModal();
                    $modalInstance.close();
                }
                else {
                    $.growl.notice({title: "[INFO]", message: "Membuat pengumuman pengadaan tanpa mengirim email"});
                    $rootScope.unloadLoadingModal();
                    $modalInstance.close();
                }
            } else{
                $.growl.error({title: "[PERINGATAN]", message: "Gagal membuat pengumuman pengadaan"});
                $rootScope.unloadLoadingModal();
            }
        });
    };

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };
};

var pengumumanPengadaanViewCtrl = function($scope, $modalInstance, $http, item) {
    $scope.pengumuman = item;

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };
};

var pengumumanPengadaanEditCtrl = function($scope, $rootScope, $modalInstance, $http, item, $cookieStore, $modal) {
    var flowpaket_id = item.flowpaket_id;
    var paket_id = item.paket_lelang_id;
    $scope.nama_paket = item.nama_paket;
    $scope.nama_tahapan = item.nama_tahapan;
    $scope.page_id = 102;
    $scope.currentURL = item.pengumuman.url_dokumen;
    $scope.pengumumanToEdit = new Pengumuman(item.pengumuman.isi_pengumuman_pengadaan, item.pengumuman.email_bidang_usaha, item.pengumuman.email_tambahan);
    $scope.terbuka = item.pengumuman.terbuka == "1" || item.pengumuman.terbuka == true;
    $scope.file;
    $scope.bidangChecked = false;
    $scope.emailChecked = false;
    $scope.subjectEmail = "";
    $scope.alamatTujuan = "";
    $scope.tempEmail = [];
    $scope.selectedBidangUsaha = [];
    $scope.selectedRekanan = [];
    $scope.selectedKualifikasi;
    var nama_address = "";
    var email_sender = "";
    $scope.labelcurr;
    $scope.customTinymce = {
        theme: "modern",
        plugins: [
            "advlist autolink lists link image charmap print preview hr anchor pagebreak",
            "searchreplace wordcount visualblocks visualchars fullscreen",
            "insertdatetime media nonbreaking save table contextmenu directionality",
            "emoticons template paste textcolor"
        ],
        toolbar1: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
        toolbar2: "print preview media | forecolor backcolor emoticons",
        image_advtab: true,
        height: "200px",
        width: "auto",
        init_instance_callback: function(editor) {
            var textContentTrigger = function() {
                $scope.textContent = editor.getBody().textContent;
            };

            editor.on('KeyUp', textContentTrigger);
            editor.on('ExecCommand', textContentTrigger);
            editor.on('SetContent', function(e) {
                if (!e.initial)
                    textContentTrigger();
            });
        }
    };

    $scope.loadAwal = function() {
        $rootScope.fileuploadconfig(14);

        if ($scope.pengumumanToEdit.emailFromCombo !== '') {
            $scope.bidangChecked = true;
        }
        if ($scope.pengumumanToEdit.emailInput !== '') {
            $scope.emailChecked = true;
        }
        //load bidangUsaha
        $rootScope.loadLoadingModal('Silahkan Tunggu...');
        
        //itp.bidangUsaha.getBUByPengumumanPengadaan
        $http.post($rootScope.url_api + 'pengumumanpengadaan/getbu', {
            id_pengumuman_pengadaan: item.pengumuman.id_pengumuman_pengadaan
        }).success(function(reply) {
            if (reply.status === 200){
                reply.result.data.forEach(function(r){
                    $scope.selectedBidangUsaha.push({
                        id_bidang_usaha : r.id_bidang_usaha,
                        nama_bidang_usaha : r.nama_bidang_usaha,
                        string_bidang_usaha : r.string_bidang_usaha
                    });
                }); 
            } else {
                $.growl.error({title: "[WARNING]", message: "Gagal mendapat daftar BU"});
            }
            $rootScope.unloadLoadingModal();
        });
        
        //itp.pengumumanPengadaan.getRekananByPengumumanPengadaan
        $http.post($rootScope.url_api + 'pengumumanpengadaan/getrekanan', {
            id_pengumuman_pengadaan: item.pengumuman.id_pengumuman_pengadaan
        }).success(function(reply) {
            if (reply.status === 200){
                reply.result.data.forEach(function(r){
                    $scope.selectedRekanan.push({
                        rekanan_id : r.rekanan_id,
                        nama_perusahaan : r.nama_perusahaan,
                        email : r.email
                    });
                }); 
            } else {
                $.growl.error({title: "[WARNING]", message: "Gagal mendapat daftar rekanan"});
            }
            $rootScope.unloadLoadingModal();
        });
        
        $scope.kualifikasi = [];
        $scope.kualifikasi.push({kualifikasi:'Semua'});
        $scope.kualifikasi.push({kualifikasi:'Besar'});
        $scope.kualifikasi.push({kualifikasi:'Menengah'});
        $scope.kualifikasi.push({kualifikasi:'Kecil'});
        $scope.kualifikasi.push({kualifikasi:'Mikro'});
        $scope.selectedKualifikasi = $scope.kualifikasi[0];
        for (var i=1;i<$scope.kualifikasi.length;i++){
            if (item.pengumuman.kualifikasi === $scope.kualifikasi[i].kualifikasi){
                $scope.selectedKualifikasi = $scope.kualifikasi[i];
            }
        }        
        $scope.subjectEmail = 'eProcurement PT. Asuransi Jiwasraya (Persero) - ' + $scope.nama_paket;
    };

    $scope.insertPekerjaan = function() {
        $scope.subPekerjaan = [];
        var isi = '';
        $rootScope.loadLoadingModal("Silahkan Tunggu...");
        //itp.paketDetail.getInfo
        $rootScope.authorize(
            $http.post($rootScope.url_api + 'paket/detail/info', {
                paket_lelang_id: paket_id,
                flowpaket_id: flowpaket_id
            }).success(function(reply) {
                if (reply.status === 200) {
                    $scope.labelcurr = reply.result.data.result[0].label;
                    if (!reply.result.data.result[0].pr_id){
                        $.growl.error({title: "[WARNING]", message: "Pengadaan ini tidak memiliki data Purchase Requisition"});
                        $rootScope.unloadLoadingModal();
                        return;
                    }
                    //itp.prSubPekerjaaan.selectBesertaPRLine
                    $http.post($rootScope.url_api + 'prsubp/selectprline', {
                        pr_id: reply.result.data.result[0].pr_id,
                        keyword: "%%"
                    }).success(function(reply) {
                        if (reply.status === 200) {
                            $scope.subPekerjaan = reply.result.data;
                            isi += '<ol type="I">';
                            $scope.subPekerjaan.forEach(function(sub) {
                                isi += '<li>';
                                isi += sub.nama;
                                if (sub.anak && sub.anak.length > 0) {
                                    isi += '<ol type="A">';
                                    sub.anak.forEach(function(an) {
                                        isi += '<li>';
                                        isi += '&nbsp;' + an.nama;
                                        isi += '<ol type="1">';
                                        an.pr_line.forEach(function(pr) {
                                            isi += '<li>';
                                            isi += '&nbsp; &nbsp;' + pr.prl_nama + '<br/>';
                                            isi += '</li>';
                                        });
                                        isi += '</ol>';
                                        isi += '</li>';
                                    });
                                    isi += '</ol>';
                                } else {
                                    isi += '<ol type="1">';
                                    sub.pr_line.forEach(function(pr) {
                                        isi += '<li>';
                                        isi += '&nbsp;' + pr.prl_nama + '<br/>';
                                        isi += '</li>';
                                    });
                                    isi += '</ol>';
                                }
                                isi += '</li>';
                            });
                            isi += '</ol>';
                            tinymce.get('isi_pengumuman_update').setContent($scope.pengumumanToEdit.isi_pengumuman_pengadaan + '<br/>' + isi);
                            //$scope.pengumumanToEdit.isi_pengumuman_pengadaan = $scope.pengumumanToEdit.isi_pengumuman_pengadaan + '<br/>' + isi;
                            $rootScope.unloadLoadingModal();
                        }
                    });
                }
            })
        );
    };

    $scope.ubahCentangBidang = function(obj) {
        $scope.bidangChecked = obj;
        if ($scope.bidangChecked === false) {
            $scope.selectedBidangUsaha = [];
            $scope.selectedKualifikasi = $scope.kualifikasi[0];
            $scope.pengumumanToEdit.emailFromCombo = "";
        }
    };

    $scope.ubahCentangEmail = function(obj) {
        $scope.emailChecked = obj;
        if ($scope.emailChecked === false)
            $scope.pengumumanToEdit.emailInput = "";
    };

    $scope.ubahTerbuka = function(obj) {
        $scope.terbuka = obj;
    };
    
    $scope.ubahKualifikasi = function(obj) {
        $scope.selectedKualifikasi = obj;
        $scope.getEmail();
    };
    
    $scope.onTambahBidangUsahaClick = function(){
        var modal = $modal.open({
            templateUrl: 'pilihBidangUsaha.html',
            controller: modalPilihBidangUsahaCtrl,
            resolve: {
                data: function() {
                    return $scope.selectedBidangUsaha;
                }
            }
        });
        modal.result.then(function(bu) {
            $scope.selectedBidangUsaha.push({
                id_bidang_usaha : bu.id_bidang_usaha,
                nama_bidang_usaha : bu.nama_bidang_usaha,
                string_bidang_usaha : bu.string_bidang_usaha
            });
            $scope.getEmail();
        });
    };
    
    $scope.onTambahRekananClick = function(){
        var modal = $modal.open({
            templateUrl: 'pilihRekanan.html',
            controller: modalPilihRekananCtrl,
            resolve: {
                data: function() {
                    return $scope.selectedRekanan;
                }
            }
        });
        modal.result.then(function(r) {
            $scope.selectedRekanan.push({
                rekanan_id : r.rekanan_id,
                nama_perusahaan : r.nama_perusahaan,
                email : r.email
            });
            $scope.$apply();
        });
    };
    
    $scope.onHapusBidangUsahaClick = function(bu){
        for(var i = 0; i < $scope.selectedBidangUsaha.length; i++) {
            if($scope.selectedBidangUsaha[i].id_bidang_usaha === bu.id_bidang_usaha) {
               $scope.selectedBidangUsaha.splice(i, 1);
               break;
            }
        }
        $scope.getEmail();
    };
    
    $scope.onHapusRekananClick = function(r){
        for(var i = 0; i < $scope.selectedRekanan.length; i++) {
            if($scope.selectedRekanan[i].rekanan_id === r.rekanan_id) {
               $scope.selectedRekanan.splice(i, 1);
               break;
            }
        }
        $scope.$apply();
    };

    $scope.filesTChanged = function(elm) {
        $scope.file = elm.files;
        $scope.$apply();
    };

    $scope.getEmail = function() {
        $scope.pengumumanToEdit.emailFromCombo = "";
        if ($scope.selectedBidangUsaha.length > 0) {
            var param = [];
            $scope.selectedBidangUsaha.forEach(function(bu){
                param.push(bu.id_bidang_usaha);
            });
            $rootScope.loadLoadingModal("Silahkan Tunggu...");
            //itp.rekanan.getEmailByBidang
            $rootScope.authorize(
                $http.post($rootScope.url_api + 'pengumumanpengadaan/mailbybidang', {
                    page_id: $scope.page_id,
                    paket_lelang_id: paket_id,
                    kualifikasi : $scope.selectedKualifikasi.kualifikasi,
                    param: param
                }).success(function(reply) {
                    if (reply.status === 200) {
                        $scope.tempEmail = reply.result.data;
                        for (var i = 0; i < $scope.tempEmail.length; i++)
                            $scope.pengumumanToEdit.emailFromCombo = $scope.pengumumanToEdit.emailFromCombo + $scope.tempEmail[i].email + ", ";
                    } else {
                        $.growl.error({title: "[WARNING]", message: "Gagal mendapat daftar email"});
                    }
                    $rootScope.unloadLoadingModal();
                })
            );
        }
    };

    $scope.simpan = function() {                
        if ($scope.pengumumanToEdit.isi_pengumuman_pengadaan === "") {
            $.growl.error({title: "[PERINGATAN]", message: "Isi pengumuman masih kosong"});
            return;
        }
        if (($scope.bidangChecked && $scope.pengumumanToEdit.emailFromCombo === "") || ($scope.emailChecked && $scope.pengumumanToEdit.emailInput === "")) {
            $.growl.error({title: "[PERINGATAN]", message: "Daftar Alamat email masih kosong"});
            return;
        }
        if ($scope.terbuka == false && $scope.selectedBidangUsaha.length === 0 && $scope.selectedRekanan.length === 0 ) {
            $.growl.error({title: "[PERINGATAN]", message: "Anda memilih pengumuman tertutup, Anda harus memilih bidang usaha atau memilih rekanan"});
            return;
        }
        if ($scope.terbuka == true && ($scope.selectedBidangUsaha.length > 0 || $scope.selectedRekanan.length > 0) ) {
            $.growl.error({title: "[PERINGATAN]", message: "Anda memilih pengumuman terbuka, Anda tidak perlu memilih bidang usaha atau memilih rekanan"});
            return;
        }
        $rootScope.loadLoadingModal("Silahkan Tunggu...");
        if ($scope.file === undefined) {
            $rootScope.authorize(update(item.pengumuman.url_dokumen));
        }
        else {
            var fileInput = $('.upload-file');
            var extFile = $('.upload-file').val().split('.').pop().toLowerCase();
            var maxSize = fileInput.data('max-size');
            if (fileInput.get(0).files.length) {
                var fileSize = fileInput.get(0).files[0].size;
                if (fileSize > maxSize) {
                    $.growl.error({title: "[WARNING]", message: "Ukuran file terlalu besar"});
                    $rootScope.unloadLoadingModal();
                    return;
                } else {
                    var restrictedExt = $rootScope.limitfiletype;
                    if ($.inArray(extFile, restrictedExt) == -1) {
                        $.growl.error({title: "[WARNING]", message: "Format file tidak valid"});
                        $rootScope.unloadLoadingModal();
                        return;
                    } else {
                        $rootScope.authorize(upload());
                    }
                }
            }
        }
    };

    function upload() {
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
        //alert($scope.pengumumanToEdit.isi_pengumuman_pengadaan);
        param.push($scope.pengumumanToEdit.isi_pengumuman_pengadaan);
        param.push(url);
        param.push(item.pengumuman.id_pengumuman_pengadaan);
        //itp.pengumumanPengadaan.update
        $http.post($rootScope.url_api + 'pengumumanpengadaan/update', {
            username: $rootScope.userLogin,
            param: param,
            flowpaket_id: flowpaket_id,
            paket_lelang_id: paket_id,
            selectedBidangUsaha: $scope.selectedBidangUsaha,
            selectedRekanan: $scope.selectedRekanan,
            kualifikasi: $scope.selectedKualifikasi.kualifikasi,
            terbuka: $scope.terbuka,
            email_bidang_usaha: $scope.pengumumanToEdit.emailFromCombo,
            email_tambahan: $scope.pengumumanToEdit.emailInput
        }).success(function(reply) {
            if (reply.status === 200) {
                if ($scope.pengumumanToEdit.emailFromCombo !== "" && $scope.pengumumanToEdit.emailInput !== "")
                    $scope.alamatTujuan = $scope.pengumumanToEdit.emailFromCombo + $scope.pengumumanToEdit.emailInput;
                else if ($scope.pengumumanToEdit.emailFromCombo === "")
                    $scope.alamatTujuan = $scope.pengumumanToEdit.emailInput;
                else if ($scope.pengumumanToEdit.emailInput === "")
                    $scope.alamatTujuan = $scope.pengumumanToEdit.emailFromCombo;
                $scope.selectedRekanan.forEach(function(r){
                    $scope.alamatTujuan += (',' + r.email);
                });
                if ($scope.alamatTujuan !== "") {
                    var alamat = $scope.alamatTujuan.split(',');
                    for (var i = 0; i < alamat.length; i++) {
                        $http.post($rootScope.url_api + 'email/send', {
                            recipients: [
                                {
                                    email : alamat[i],
                                    name : ''
                                }
                            ],
                            subject: $scope.subjectEmail,
                            content: $scope.pengumumanToEdit.isi_pengumuman_pengadaan,
                            attachments: []
                        }).success(function(reply) {
                        });
                    }
                    $.growl.notice({title: "[INFO]", message: "Berhasil membuat pengumuman"});
                    $rootScope.unloadLoadingModal();
                    $modalInstance.close();
                }
                else {
                    $rootScope.unloadLoadingModal();
                    $.growl.notice({title: "[INFO]", message: "Mengupdate pengumuman pengadaan tanpa mengirim email"});
                    $modalInstance.close();
                }
            }
            else {
                $rootScope.unloadLoadingModal();
                $.growl.error({title: "[PERINGATAN]", message: "Gagal mengupdate pengumuman pengadaan"});
            }
        });
    };

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };
};

var modalPilihBidangUsahaCtrl = function($scope, $modalInstance, data, $http, $rootScope, $cookieStore) {

    $scope.selectedBidangUsaha = data;
    $scope.bidangUsaha = [];
    $scope.count;
    $scope.pageNumber = 1;
    $scope.pageSize = 10;
    $scope.searchText = "";

    $scope.onBatalClick = function() {
        $modalInstance.dismiss('cancel');
    };

    $scope.onSearchSubmit = function(searchText) {
        $scope.searchText = searchText;
        $scope.loadBidangUsaha(1);
    };

    $scope.init = function() {
        $scope.loadBidangUsaha($scope.pageNumber);
    };

    $scope.loadBidangUsaha = function(page) {

        $scope.pageNumber = page;
        $rootScope.loadLoadingModal('Silahkan Tunggu...');
        $rootScope.authorize(
            //itp.bidangUsaha.select2
            $http.post($rootScope.url_api + 'bidusaha/select', {
                search: $scope.searchText,
                pageSize: $scope.pageSize,
                offset: ($scope.pageNumber - 1) * $scope.pageSize
            }).success(function(reply) {
                if (reply.status === 200) {
                    $scope.bidangUsaha = reply.result.data;
                    //itp.bidangUsaha.count2
                    $http.post($rootScope.url_api + 'bidusaha/count', {
                        search: $scope.searchText
                    }).success(function(reply) {
                        if (reply.status === 200) {
                            $scope.count = reply.result.data;
                        } else {
                            $.growl.error({title: "[PERINGATAN]", message: "Gagal mendapat data badan usaha"});
                        }
                        $rootScope.unloadLoadingModal();
                    });
                } else {
                    $.growl.error({title: "[PERINGATAN]", message: "Gagal mendapat data badan usaha"});
                }
                $rootScope.unloadLoadingModal();
            })
        );
    };

    $scope.onSelectClick = function(bu) {
        for (var i = 0; i < $scope.selectedBidangUsaha.length; i++) {
            if ($scope.selectedBidangUsaha[i].id_bidang_usaha === bu.id_bidang_usaha) {
                $.growl.error({title: "[PERINGATAN]", message: "Bidang Usaha ini sudah dipilih"});
                return;
            }
        }
        $modalInstance.close(bu);
    };
};

var modalPilihRekananCtrl = function($scope, $modalInstance, data, $http, $rootScope, $cookieStore) {

    $scope.selectedRekanan = data;
    $scope.rekanan = [];
    $scope.count;
    $scope.pageNumber = 1;
    $scope.pageSize = 10;
    $scope.searchText = "";

    $scope.onBatalClick = function() {
        $modalInstance.dismiss('cancel');
    };

    $scope.onSearchSubmit = function(searchText) {
        $scope.searchText = searchText;
        $scope.loadRekanan(1);
    };

    $scope.init = function() {
        $scope.loadRekanan($scope.pageNumber);
    };

    $scope.loadRekanan = function(page) {

        $scope.pageNumber = page;
        $rootScope.loadLoadingModal('Silahkan Tunggu...');
        $rootScope.authorize(
            //itp.pengumumanPengadaan.selectRekanan
            $http.post($rootScope.url_api + 'pengumumanpengadaan/selectrekanan', {
                search: $scope.searchText,
                pageSize: $scope.pageSize,
                offset: ($scope.pageNumber - 1) * $scope.pageSize
            }).success(function(reply) {
                if (reply.status === 200) {
                    $scope.rekanan = reply.result.data;
                    //itp.pengumumanPengadaan.countRekanan
                    $http.post($rootScope.url_api + 'pengumumanpengadaan/countrekanan', {
                        search: $scope.searchText
                    }).success(function(reply) {
                        if (reply.status === 200) {
                            $scope.count = reply.result.data;
                        } else {
                            $.growl.error({title: "[PERINGATAN]", message: "Gagal mendapat data rekanan"});
                        }
                        $rootScope.unloadLoadingModal();
                    });
                } else {
                    $.growl.error({title: "[PERINGATAN]", message: "Gagal mendapat data rekanan"});
                }
                $rootScope.unloadLoadingModal();
            })
        );
    };

    $scope.onSelectClick = function(r) {
        for (var i = 0; i < $scope.selectedRekanan.length; i++) {
            if ($scope.selectedRekanan[i].rekanan_id === r.rekanan_id) {
                $.growl.error({title: "[PERINGATAN]", message: "Rekanan ini sudah dipilih"});
                return;
            }
        }
        $modalInstance.close(r);
    };
};

function Pengumuman(isi_pengumuman_pengadaan, emailFromCombo, emailInput) {
    var self = this;
    self.isi_pengumuman_pengadaan = isi_pengumuman_pengadaan;
    self.emailFromCombo = emailFromCombo;
    self.emailInput = emailInput;
}