angular.module('eprocAppPanitia')
        .controller('paketlelangCreateCtrl', function($scope, $http, $rootScope, $state, $stateParams, $cookieStore, $modal, ngProgress) {
            $scope.namaLelang = "";
            $scope.jabatans = [];
            $scope.pegawai = [];
            $scope.jabatanTerpilih = null;
            $scope.pegawaiTerpilih = null;
            $scope.temp = [];
            $scope.page_id = 8;
            $scope.file;
            $scope.menuhome = 0;
            var pr_id = Number($stateParams.pr_id);
            var pr_id_upload = Number($stateParams.pr_id);
            $scope.fileDocument = undefined;
            $scope.filesToUpload = [];
            $scope.urls = [];
            $rootScope.fileuploadconfig(32);
            $scope.templatepanitia;
            $scope.user_id = 0;
            $scope.type_pengadaan = "";

            $scope.uploads = function() {
                var idx = -1;
                $.each($scope.filesToUpload, function(index, item) {
                    if (item.fileName === $scope.fName) {
                        idx = index;
                    }
                });
                if ($scope.fileDocument === '' || $scope.fileDocument === undefined) {
                    $.growl.error({title: "[PERINGATAN]", message: "File belum dipilih"});
                    return;
                } else if (idx > -1) {
                    $.growl.error({title: "[PERINGATAN]", message: "File sudah ada"});
                    return;
                } else {
                    /*
                     $scope.filesToUpload.push({
                     fileName: $scope.fName,
                     fileSize: $scope.fSize,
                     file: $scope.fileDocument
                     });
                     */
                    var fileInput = $('.upload-file');
                    var extFile = $('.upload-file').val().split('.').pop().toLowerCase();
                    var maxSize = fileInput.data('max-size');
                    if (fileInput.get(0).files.length) {
                        var fileSize = fileInput.get(0).files[0].size;
                        if (fileSize > maxSize) {
                            $rootScope.unloadLoadingModal();
                            $.growl.error({title: "[WARNING]", message: "Ukuran file terlalu besar"});
                            return;
                        } else {
                            var restrictedExt = $rootScope.limitfiletype;
                            if ($.inArray(extFile, restrictedExt) === -1) {
                                $rootScope.unloadLoadingModal();
                                $.growl.error({title: "[WARNING]", message: "Format file tidak valid"});
                                return;
                            } else {
                                $scope.filesToUpload.push({
                                    fileName: $scope.fName,
                                    fileSize: $scope.fSize,
                                    file: $scope.fileDocument
                                });
                            }
                        }
                    }
                }
            };
            $scope.fileMChanged = function(elm) {
                $scope.fileDocument = elm.files;
                if ($scope.fileDocument !== '' || $scope.fileDocument !== undefined) {
                    for (var i = 0; i < $scope.fileDocument.length; i++) {
                        $scope.fName = $scope.fileDocument[i].name;
                        $scope.fSize = ($scope.fileDocument[i].size / 1000).toFixed(1);
                        //console.log($scope.fileDocument[i]);
                        if ($scope.filesToUpload.length > 0) {
                            for (i = 0; i < $scope.filesToUpload.length; i++) {
                                if ($scope.fName === $scope.filesToUpload[i].fileName) {
                                    $.growl.error({title: "[PERINGATAN]", message: "File sudah ada "});
                                    document.getElementById('uploadFile').value = '';
                                    $scope.fileDocument = '';
                                    $scope.fName = '';
                                    $scope.fSize = '';
                                    return;
                                }
                            }
                        }
                    }
                }
            };
            $scope.removeFile = function(fileName) {
                var idx = -1;
                $.each($scope.filesToUpload, function(index, item) {
                    if (item.fileName === fileName) {
                        idx = index;
                    }
                });
                $scope.filesToUpload.splice(idx, 1);
            };

            $scope.loadPage = function() {
                $scope.menuhome = $rootScope.menuhome;
                $rootScope.getSession().then(function(result){
                    $rootScope.userSession = result.data.data;
                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
                    $scope.user_id = $rootScope.userSession.session_data.pegawai_id;
                    //console.info(JSON.stringify($scope.userSession));
                    $rootScope.authorize(loadall());
                });
            };
            function loadall() {
                if (pr_id !== 0) {
                    
                    $http.post($rootScope.url_api+"PR/detailPRLelang",{pr_id: pr_id })
                    .success(function(reply){
                        $rootScope.unloadLoadingModal();
                        if(reply.status === 200){
                            $scope.namaLelang = reply.result.data[0].pr_nama;
                        }
                        else{
                            $.growl.error({ message: "Gagal mendapatkan data PR!" });
                            return;
                        }
                    })
                    .error(function(err) {
                        $.growl.error({ message: "Gagal Akses API >"+err });
                        return;
                    });   
                }
                
            }


            $scope.pilihPegawai = function() {
                var modalInstance = $modal.open({
                    templateUrl: 'PilihPegawai.html',
                    controller: PilihPegawaiCtrl
                });
                modalInstance.result.then(function(pgw) {
                    $scope.pegawaiTerpilih = pgw;
                });
            };
            $scope.pilihTemplate = function() {
                var modalInstance = $modal.open({
                    templateUrl: 'PilihTemplate.html',
                    controller: pilihTemplateCtrl
                });
                modalInstance.result.then(function(pgw) {
                    $scope.templatepanitia = pgw;
                    for(var i = 0; i < $scope.templatepanitia.length; i++){
                        $scope.templatepanitia[i].id_pegawai = parseInt($scope.templatepanitia[i].id_pegawai);
                        $scope.templatepanitia[i].id_jabatan = parseInt($scope.templatepanitia[i].id_jabatan);
                        $scope.templatepanitia[i].approval = parseInt($scope.templatepanitia[i].approval);
                        $scope.templatepanitia[i].flag_detail = $rootScope.strtobool( $scope.templatepanitia[i].flag_detail);
                    }
                    $scope.type_pengadaan = $scope.templatepanitia[0].type;
                    console.info(JSON.stringify($scope.templatepanitia));
                });
            };
            
            /* //ini tidak dipakai jiwasraya, sudah generate template
            $scope.tambahPanitia = function() {
                if ($scope.jabatanTerpilih === "" || $scope.jabatanTerpilih === null) {
                    alert("Jabatan belum dipilih");
                    return;
                }
                if ($scope.pegawaiTerpilih === "" || $scope.pegawaiTerpilih === null) {
                    alert("Pegawai belum dipilih");
                    return;
                }
                for (var i = 0; i < $scope.temp.length; i++)
                {
                    if ($scope.temp[i].jabatan_id === $scope.jabatanTerpilih.jabatan_id && $scope.temp[i].jabatan_id !== 5 && $scope.temp[i].jabatan_id !== 6 && $scope.temp[i].jabatan_id !== 7)
                    {
                        alert("Jabatan tersebut tidak bisa diisi oleh lebih dari 1 orang");
                        return;
                    }
                }
                for (var i = 0; i < $scope.temp.length; i++)
                {
                    if ($scope.temp[i].pegawai_id === $scope.pegawaiTerpilih.pegawai_id)
                    {
                        alert("Pegawai sudah dipilih");
                        return;
                    }
                }

                $scope.temp.push({
                    jabatan_nama: $scope.jabatanTerpilih.jabatan_nama,
                    jabatan_id: $scope.jabatanTerpilih.jabatan_id,
                    nama_pegawai: $scope.pegawaiTerpilih.nama_pegawai,
                    pegawai_id: $scope.pegawaiTerpilih.pegawai_id,
                    departemen_nama: $scope.pegawaiTerpilih.departemen_nama
                });
            };
            */
            $scope.hapusPanitia = function(pegawai_id) {
                var idx = -1;
                for (var i = 0; i < $scope.templatepanitia.length; i++)
                {
                    if ($scope.templatepanitia[i].pegawai_id === pegawai_id)
                    {
                        idx = i;
                        break;
                    }
                }
                $scope.templatepanitia.splice(idx, 1);
            };
            $scope.submitInput = function() {
                if ($scope.namaLelang === "") {
                    $.growl.error({ message: "Nama paket belum dimasukkan" });
                    return;
                }
//                if ($scope.temp.length < 1) {
//                    $.growl.error({ message: "Panitia belum dipilih" });
//                    return;
//                } /*ini tidak dipakai di jiwasraya*/

                if ($scope.fileDocument === undefined) {
                    insert("");
                }
                else {
                    if ($scope.filesToUpload.length === 0) {
                        $.growl.error({title: "[PERINGATAN]", message: "Belum ada file yang ditambahkan"});
                        return;
                    }
                    ngProgress.color('cyan');
                    ngProgress.height('5px');
                    ngProgress.start();
                    $scope.panjang = $scope.filesToUpload.length;
                    upload(0);
                }
            };
            
            function upload(index) {
                $rootScope.loadLoading("Silahkan Tunggu...");
                //("files:"+JSON.stringify($scope.filesToUpload));
                var fd = new FormData();
                for (var i = 0; i < $scope.filesToUpload.length; i++) {
                    angular.forEach($scope.filesToUpload[i].file, function(item) {
                        fd.append("uploads[]", item);
                    });
                }
                $http.post($rootScope.url_api + "uploadZip/pr_id_" + pr_id_upload, fd, {
                    withCredentials: true,
                    transformRequest: angular.identity(),
                    headers: {'Content-Type': undefined}
                }).success(function(reply) {
                    $rootScope.unloadLoading();
                    //(JSON.stringify(reply));
                    if(reply.status === 200){
                        var filezip = reply.result.data.files[0].url;
                        insert(filezip);
                    }
                    else{
                        $.growl.error({ message: "Gagal mengupload file" });
                        return;
                    }
                });

            };
            function insert(url) {
                var arr = {nama_paket: $scope.namaLelang ,pakta_integritas: url,flag_active: true,
                            status_kelangsungan: 0, is_managed: 0, pr_id:  pr_id, type_pengadaan: $scope.type_pengadaan  };
                
                $rootScope.loadLoading("Silahkan Tunggu...");
                $http.post($rootScope.url_api+"panitia/insert",{
                    nama_paket: $scope.namaLelang,
                    master : arr,
                    detail : $scope.templatepanitia,
                    username: $rootScope.userLogin,
                    pr_id:pr_id,
                    created_by : $scope.user_id
                })
                .success(function(reply){
                    $rootScope.unloadLoading();
                    //console.info(">>"+JSON.stringify(reply));
                    if(reply.status === 200){
                        $.growl.notice({title: "[INFO]", message: "Berhasil menambah kepanitiaan"});
                        $state.transitionTo('edit-susunanpanitia');
                    }
                    else{
                        $.growl.error({ message: "Gagal menambah kepanitiaan!" });
                        return;
                    }
                })
                .error(function(err) {
                    $.growl.error({ message: "Gagal Akses API >"+err });
                    return;
                });                
            }
        })
        .controller('paketLelangViewCtrl', function($scope, eb, $rootScope, $state) {
            $scope.totalItems = 0;
            $scope.currentPage = 1;
            $scope.maxSize = 10;
            $scope.paket = [];
            $scope.menuhome = 0;
//            eb.onopen = function() {
//                $rootScope.autorize();
//                $scope.loadViewPaket();
//            };

            $scope.loadViewPaket = function() {
                $rootScope.refreshWaktu();
                $scope.menuhome = $rootScope.menuhome;
                $rootScope.isLogged = true;
                loadViewAll();
            };
            function loadViewAll() {
                $scope.currentPage = 1;
                var arr0 = [];
                arr0.push($rootScope.userlogged);
                eb.send('itp.paket.count', arr0, function(reply) {
                    $scope.totalItems = reply[0].count;
                    $scope.$apply();
                });
                $scope.paket = [];
                var offset = 0;
                var limit = 10;
                var arr = [];
                arr.push($rootScope.userlogged);
                arr.push(offset);
                arr.push(limit);
                eb.send('itp.paket.select', arr, function(reply) {
                    $scope.paket = reply;
                    $scope.$apply();
                });
            }

            $scope.viewTahapan = function(idPaket) {
                $state.transitionTo('paketlelang-viewtahapan', {idPaket: idPaket});
            };
        })
        .controller('prakualifikasiTahapanCtrl', function($scope, $http, $rootScope, $stateParams, $state, $cookieStore, $modal) {
            $scope.idPaket = Number($stateParams.idPrakual);
            $scope.flow = [];
            $scope.paket;
            $scope.namaLelang;
            $scope.userBisaMengatur = false;
            $scope.page_id = 37;
            $scope.selectedOption;
            $scope.menuhome = 0;
            $scope.ditolak = false;
            $scope.type_pengadaan = "";
            $scope.tahapanDitolak = {};

            $scope.view = function() {
                $scope.menuhome = $rootScope.menuhome;
                $rootScope.getSession().then(function(result) {
                    $rootScope.userSession = result.data.data;
                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
                    $rootScope.authorize(new function() {
                        viewAllSteps();
                    });
                });
            };
            function viewAllSteps() {
                $http.post($rootScope.url_api + 'approval/cektolak', {
                    paket_lelang_id: $scope.idPaket
                }).success(function(reply) {
                    //console.info("tolak: "+JSON.stringify(reply));
                    if (reply.status === 200) {
                        if(reply.result.data.length > 0){
                            $scope.ditolak = true;
                            $scope.tahapanDitolak = reply.result.data[0];
                        }
                    }
                    else {
                        $.growl.error({message: "Gagal mendapatkan status!"});
                        return;
                    }
                }).error(function(err) {
                    $.growl.error({message: "Gagal Akses API >" + err});
                    return;
                });
                
                var arr2 = [];
                arr2.push($scope.idPaket);
                arr2.push($rootScope.userLogin);
                arr2.push($scope.page_id);
                //itp.paket.cekBisaMengatur
                $http.post($rootScope.url_api + 'paket/cekmengatur', {
                    param: arr2,
                    page_id: $scope.page_id
                }).success(function(reply) {
                    if (reply.status === 200) {
                        if(reply.result.data.length > 0){
                            var data = reply.result.data[0].bisa_mengatur;
                            $scope.userBisaMengatur = data;
                        } else {
                            $scope.userBisaMengatur = false;
                        }
                    }
                    else {
                        $.growl.error({message: "Gagal mendapatkan data hak akses!"});
                        return;
                    }
                }).error(function(err) {
                    $.growl.error({message: "Gagal Akses API >" + err});
                    return;
                });
                getStatusLelang();
                var param = [];
                param.push($scope.idPaket);
                /*Begin Data Dummy*/
                $scope.flow = [
                    {urutan:0, nama_tahapan: "Pengumuman ", status: 1, flowpaket_id:809, jenis_form_url:"pengumuman-prakualifikasi"},
                    {urutan:1, nama_tahapan: "Pendaftaran", status: 1, flowpaket_id:810, jenis_form_url:"pendaftaran-prakualifikasi"},
                    {urutan:2, nama_tahapan: "Pemasukkan Data", status: 1, flowpaket_id:813, jenis_form_url:"pemasukan-dokumen-prakualifikasi"},
                    {urutan:3, nama_tahapan: "Penentuan Hasil", status: 1, flowpaket_id:813,  jenis_form_url:"pemasukan-dokumen-prakualifikasi"},
                    {urutan:4, nama_tahapan: "Penetapan Hasil", status: 1, flowpaket_id:813, jenis_form_url:"penetapan-hasil-prakualifikasi"},
                    {urutan:5, nama_tahapan: "Pengumuman Hasil", status: 1, flowpaket_id:813, jenis_form_url:"pengumuman-hasil-prakualifikasi"},
                    {urutan:6, nama_tahapan: "Certificate", status: 1, flowpaket_id:813, jenis_form_url:"certificate-prakualifikasi"}
                ];
                
                $rootScope.loadLoading("Silahkan Tunggu...");
                //itp.paketDetail.select
                $http.post($rootScope.url_api + 'paket/detail/select', {
                    param: param
                }).success(function(reply) {
                    console.info("select: "+JSON.stringify(reply));
                    if (reply.status === 200) {
//                        $scope.flow = reply.result.data;
//                        $scope.type_pengadaan = $scope.flow[0].type_pengadaan;
//                        for (var i = 0; i < $scope.flow.length; i++) {
//                            if (!($scope.flow[i].tgl_mulai === null) && !($scope.flow[i].tgl_mulai === ''))
//                                $scope.flow[i].tgl_mulai_formatted = convertTanggal($scope.flow[i].tgl_mulai);
//                            if (!($scope.flow[i].tgl_selesai === null) && !($scope.flow[i].tgl_selesai === ''))
//                                $scope.flow[i].tgl_selesai_formatted = convertTanggal($scope.flow[i].tgl_selesai);
//                        }
                    }
                    $rootScope.unloadLoading();
                }).error(function(err) {
                    $.growl.error({message: "Gagal Akses API > " + err});
                    return;
                });
               
            }
            ;
            function getStatusLelang() {
                var arr = [];
                arr.push($scope.idPaket);
                //itp.paket.selectById
                $http.post($rootScope.url_api + 'paket/byid', {
                    param: arr
                }).success(function(reply) {
                    if (reply.status === 200) {
                        $scope.paket = reply.result.data[0];
                        $scope.namaLelang = $scope.paket.nama_paket;
                        $scope.selectedOption = $scope.paket.status_kelangsungan;
                    }
                }).error(function(err) {
                    $.growl.error({message: "Gagal Akses API > " + err});
                    return;
                });
            }

            function convertTanggal(input) {
                return input ? $rootScope.convertTanggalWaktu(input) : '';
            }
            ;
            $scope.kembali = function() {
                //$state.transitionTo('homepanitia');
                var role_id = $rootScope.userSession.session_data.role_id;
                if (role_id == 18 || role_id == 26 || role_id == 29) { //harus pakai '=='
                    $state.transitionTo('daftar-lelang-admin'); // old: $state.transitionTo('homeadmin');
                }
                else if (role_id == 19) {
                    $state.transitionTo('daftar-lelang-panitia');
                }
                else {
                    // TODO: kick to login!
                }
            };
            $scope.ubahStatusLelang = function() {
                var kirim = {
                    page_id: $scope.page_id,
                    paket_lelang_id: $scope.idPaket,
                    nama_paket: $scope.namaLelang,
                    status_kelangsungan: Number($scope.selectedOption)
                };
                var modalInstance = $modal.open({
                    templateUrl: 'ubahStatusLelang.html',
                    controller: ubahStatusLelangCtrl,
                    resolve: {
                        item: function() {
                            return kirim;
                        }
                    }
                });
                modalInstance.result.then(function() {
                    getStatusLelang();
                });
            };
            $scope.aturTahapan = function(flow) {
                var modalInstance = $modal.open({
                    templateUrl: 'aturTahapan.html',
                    controller: aturTahapanModalCtrl,
                    resolve: {
                        item: function() {
                            return flow;
                        }
                    }
                });
                modalInstance.result.then(function() {
                    viewAllSteps();
                });
            };
            $scope.menujuTahapan = function(stateName, flowpaket_id) {
                $state.transitionTo(stateName, {flowpaket_id: flowpaket_id, paket_lelang_id: $scope.idPaket});
            };
        })


        .controller('paketLelangManageCtrl', function($scope, $http, $modal, $rootScope, $state, $cookieStore) {

            $scope.totalItems = 0;
            $scope.currentPage = 1;
            $scope.maxSize = 10;
            $scope.paket = [];
            $scope.page_id = 8;
            $scope.menuhome = 0;

            $scope.init = function() {
                $rootScope.getSession().then(function(result) {
                    $rootScope.userSession = result.data.data;
                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
                    $rootScope.authorize(loadPaket());

                });
            };
            function loadPaket() {

                $rootScope.loadLoading("Silahkan Tunggu...");
                $scope.currentPage = 1;
                $http.post($rootScope.url_api + "managepaket/count", {
                    page_id: $scope.page_id,
                    username: $scope.userLogin
                }).success(function(reply) {
                    if (reply.status === 200) {
                        $scope.totalItemsPekerjaan = reply.result.data;
                    }
                }).error(function(err) {
                    $.growl.error({message: "Gagal Akses API > " + err});
                    return;
                });

                $http.post($rootScope.url_api + "managepaket/select", {
                    offset: 0,
                    limit: 10,
                    page_id: $scope.page_id,
                    username: $rootScope.userLogin
                }).success(function(reply) {
                    if (reply.status == 200) {
                        $scope.paket = reply.result.data;
                        for (var i = 0; i < $scope.paket.length; i++)
                            $scope.paket[i].sudah_berjalan = false;
                            $http.get($rootScope.url_api + "paket/sudahberjalan", {
                            }).success(function(reply) {
                            if (reply.status === 200) {
                                for (var k = 0; k < $scope.paket.length; k++) {
                                    for (var l = 0; l < reply.result.length; l++) {
                                        if ($scope.paket[k].paket_id === reply.result[l].paket_lelang_id) {
                                            $scope.paket[k].sudah_berjalan = true;
                                        }
                                    }
                                }
                            } else {
                                $.growl.error({message: "Gagal mendapatkan data jenis form!"});
                                return;
                            }
                        }).error(function(err) {
                            $.growl.error({message: "Gagal Akses API > " + err});
                            return;
                        });

                    }
                    $rootScope.unloadLoading();


                }).error(function(err) {
                    $.growl.error({message: "Gagal Akses API > " + err});
                    return;
                });

            }
            // end loadPaket

            $scope.jLoad = function(current) {
                $scope.paket = [];
                $scope.currentPage = current;
                var offset = (current * 10) - 10;
                var limit = 10;
                $http.post($rootScope.url_api + "managepaket/select", {
                    sessionID: $cookieStore.get('sessId'),
                    page_id: $scope.page_id,
                    username: $rootScope.userLogin,
                    offset: offset,
                    limit: limit
                }).success(function(reply) {
                    if (reply.status == 200) {
                        $scope.paket = reply.result;
                        for (var j = 0; j < $scope.paket.length; j++) {
                            $scope.paket[j].sudah_berjalan = false;
                        }

                        $http.get($rootScope.url_api + "paket/sudahberjalan", 
                        {sessionID: $cookieStore.get('sessId')}
                        ).success(function(reply2){
                            if (reply2.status == 200) {
                                for (var k = 0; k < $scope.paket.length; k++) {
                                    for (var l = 0; l < reply2.result.length; l++) {
                                        if ($scope.paket[k].paket_id === reply2.result[l].paket_lelang_id) {
                                            $scope.paket[k].sudah_berjalan = true;
                                        }
                                    }
                                }
                                $scope.$apply();
                            }
                            
                        });
                        var arr2;
                        var arr3 = [];
                        for (var i = 0; i < $scope.paket.length; i++)
                        {
                            arr2 = [];
                            arr2.push($scope.paket[i].paket_id);
                            arr2.push($rootScope.userlogged);
                            arr2.push($scope.page_id);
                            arr3.push(arr2);
                        }

                        $http.post($rootScope.url_api + "paket/cekmengatur", 
                        {sessionID: $cookieStore.get('sessId'), 
                         param: arr3, 
                         page_id: $scope.page_id
                        }).success(function(reply2){
                            if (reply2.status == 200) {
                                for (var i = 0; i < reply2.result.length; i++)
                                    $scope.paket[i].bisa_mengatur = reply2.result[i].bisa_mengatur;
                            }
                            $scope.$apply();
                        });
                        $scope.$apply();
                    }
                });
            };
                $scope.managePaket = function(idPaket) {
                    $state.transitionTo('paketLelang-manageDetail', {idPaket: idPaket});
                };
                $scope.editPaket = function(idPaket) {
                    $rootScope.authorize(
                        $http.post($rootScope.url_api + "paket/ceksudahberjalan",{
                                id: idPaket
                        }).success(function(reply) {      
                                if (reply.status == 200) {
                                    var modalInstance = $modal.open(
                                        {
                                        templateUrl: 'warningUbahPaket.html',
                                        controller: warningUbahPaketCtrl
                                        }
                                    );
                                    modalInstance.result.then(function() {
                                        $state.transitionTo('paketLelang-edit', {idPaket: idPaket});
                                    });
                                }
                                else {                                    
                                      //$state.transitionTo('paketLelang-edit', {idPaket: idPaket});
                                      $.growl.error({message: "Gagal mendapatkan data jenis form!"});
                                      return; 
                                }
                            }).error(function(err) {
                                $.growl.error({message: "Gagal Akses API > " + err});
                                return;
                            })
                    );
//                        })
//                    );   
                };
                $scope.nonAktifkanPaket = function(idPaket, nama_paket) {
                var kirim = {
                        page_id: $scope.page_id,
                        paket_lelang_id: idPaket,
                        nama_paket: nama_paket
                };
                        var modalInstance = $modal.open({
                        templateUrl: 'hapusPaketLelang.html',
                                controller: hapusPaketLelangCtrl,
                                resolve: {
                                item: function() {
                                return kirim;
                                }
                                }
                        });
                        modalInstance.result.then(function() {
                        loadPaket();
                        });
                };
        }) // end paketLelangManageCtrl


        .controller('paketLelangManageDetailCtrl', function($scope, $modal, $http, $rootScope, $state, $stateParams, $cookieStore) {
            $scope.namaLelang;
            $scope.metode = [];
            $scope.metodeTerpilih;
            $scope.tahapan = [];
            $scope.metodeEvaluasi = [];
            $scope.metodeEvaluasiTerpilih;
            $scope.lokasiProyek;
            $scope.nilaiHPS;
            $scope.noSurat;
            $scope.idPaket = Number($stateParams.idPaket);
            $scope.page_id = 8;
            $scope.newLelang = new Lelang("", "", "");
            $scope.menuhome = 0;
            var Administrasi = [];
            var AdministrasiLevel1 = [];
            var AdministrasiLevel2 = [];
            var AdministrasiLevel3 = [];
            var Teknis = [];
            var TeknisLevel1 = [];
            var TeknisLevel2 = [];
            var TeknisLevel3 = [];
            var Harga = [];
//            eb.onopen = function() {
//                $rootScope.autorize();
//                $scope.init();
//            };

            $scope.init = function() {
                $scope.menuhome = $rootScope.menuhome;
                $rootScope.isLogged = true;
                $rootScope.authorize(manage());
            };
            function manage() {
                var arr = [];
                arr.push($scope.idPaket);
                $http.post($rootScope.url_api +'paket/selectwithnego', {
                    sessionID: $cookieStore.get('sessId'),
                    param: arr,
                    page_id: $scope.page_id
                }).success(function(reply) {
                    if (reply.status == 200) {
                        $scope.paket = reply.result.data;
                        $scope.namaLelang = $scope.paket[0].nama_paket;
                        $scope.newLelang.nilaiHPS = $scope.paket[0].total_hps;
                    }else{
                         $.growl.error({message: "Gagal mendapatkan data jenis form selectwithnego!"});
                         return; 
                    }
                    
                });
                $http.post($rootScope.url_api +'metode/selectAll', {
                    sessionID: $cookieStore.get('sessId'),
                    page_id: $scope.page_id
                }).success(function(reply) {
                    if (reply.status == 200) {
                        $scope.metode = reply.result.data;
                    }else{
                        $.growl.error({message: "Gagal mendapatkan data jenis form selectall!"});
                        return; 
                    }
                    
                });
                $http.get($rootScope.url_api +'metodeEvaluasi/selectActive', {
                    sessionID: $cookieStore.get('sessId'),
                    page_id: $scope.page_id
                }).success( function(reply) {
                    if (reply.status == 200) {
                        $scope.metodeEvaluasi = reply.result.data;
                    }else{
                         $.growl.error({message: "Gagal mendapatkan data metode evaluasi!"});
                         return; 
                    }
                    $scope.$apply();
                });
            }
            ;
            $scope.generateStep = function() {
                if ($scope.metodeTerpilih === undefined) {
                    $.growl.error({title: "[PERINGATAN]", message: "Metode Pengadaan belum dipilih"});
                    return;
                }
                if ($scope.metodeEvaluasiTerpilih === undefined) {
                    //$.growl.error({title: "[PERINGATAN]", message: "Metode evaluasi belum dipilih"});
                    //return;
                }
                var arr = [];
                arr.push($scope.metodeTerpilih.metode_id);
                Administrasi = [];
                AdministrasiLevel1 = [];
                AdministrasiLevel2 = [];
                AdministrasiLevel3 = [];
                Teknis = [];
                TeknisLevel1 = [];
                TeknisLevel2 = [];
                TeknisLevel3 = [];
//                $('#divManageDetail').block({
//                    message: '<div style="text-align:center;"><h4>Silakan tunggu..</h4></div>',
//                    css: {border: '3px solid #a00'}
//                });
                $rootScope.loadLoading("Silahkan Tunggu...");
                $http.post($rootScope.url_api+'metodeDetail/select', {
                    keyword:$scope.metodeTerpilih.metode_id,
                    page_id:$scope.page_id
                }).success(function(reply) {
                    if (reply.status == 200) {
                        $scope.tahapan = reply.result.data;
                        if ($scope.metodeEvaluasiTerpilih !== undefined) {
                            $http.post($rootScope.url_api+'metodeEvaluasi/getDetail', {
                                sessionID: $cookieStore.get('sessId'),
                                id: $scope.metodeEvaluasiTerpilih.metode_evaluasi_id
                            }).success(function(reply2) {
                                if (reply2.status == 200) {
                                    var hasil = reply2.result.data;
                                    for (var i = 0; i < hasil.length; i++) {
                                        if (hasil[i].jenis_detail === 'Administrasi') {
                                            Administrasi.push(hasil[i]);
                                        }
                                        else if (hasil[i].jenis_detail === 'Teknis') {
                                            Teknis.push(hasil[i]);
                                        }
                                        else if (hasil[i].jenis_detail === 'Harga') {
                                            Harga.push(hasil[i]);
                                        }
                                    }
                                    for (var i = 0; i < Administrasi.length; i++) {
                                        if (Administrasi[i].level == 1) {
                                            AdministrasiLevel1.push(Administrasi[i]);
                                        }
                                        else if (Administrasi[i].level == 2) {
                                            AdministrasiLevel2.push(Administrasi[i]);
                                        }
                                        else if (Administrasi[i].level == 3) {
                                            AdministrasiLevel3.push(Administrasi[i]);
                                        }
                                    }
                                    for (var i = 0; i < AdministrasiLevel2.length; i++) {
                                        AdministrasiLevel2[i].sub = [];
                                        for (var j = 0; j < AdministrasiLevel3.length; j++) {
                                            if (AdministrasiLevel3[j].parent == AdministrasiLevel2[i].kriteria_id) {
                                                AdministrasiLevel2[i].sub.push(AdministrasiLevel3[j]);
                                            }
                                        }
                                    }
                                    for (var i = 0; i < AdministrasiLevel1.length; i++) {
                                        AdministrasiLevel1[i].sub = [];
                                        for (var j = 0; j < AdministrasiLevel2.length; j++) {
                                            if (AdministrasiLevel2[j].parent == AdministrasiLevel1[i].kriteria_id) {
                                                AdministrasiLevel1[i].sub.push(AdministrasiLevel2[j]);
                                            }
                                        }

                                    }

                                    for (var i = 0; i < Teknis.length; i++) {
                                        if (Teknis[i].level == 1) {
                                            TeknisLevel1.push(Teknis[i]);
                                        }
                                        else if (Teknis[i].level == 2) {
                                            TeknisLevel2.push(Teknis[i]);
                                        }
                                        else if (Teknis[i].level == 3) {
                                            TeknisLevel3.push(Teknis[i]);
                                        }
                                    }
                                    for (var i = 0; i < TeknisLevel2.length; i++) {
                                        TeknisLevel2[i].sub = [];
                                        for (var j = 0; j < TeknisLevel3.length; j++) {
                                            if (TeknisLevel3[j].parent == TeknisLevel2[i].kriteria_id) {
                                                TeknisLevel2[i].sub.push(TeknisLevel3[j]);
                                            }
                                        }
                                    }
                                    for (var i = 0; i < TeknisLevel1.length; i++) {
                                        TeknisLevel1[i].sub = [];
                                        for (var j = 0; j < TeknisLevel2.length; j++) {
                                            if (TeknisLevel2[j].parent == TeknisLevel1[i].kriteria_id) {
                                                TeknisLevel1[i].sub.push(TeknisLevel2[j]);
                                            }
                                        }
                                    }
                                }else{
                                     $.growl.error({message: "Gagal mendapatkan data metode evaluasi!"});
                                     return; 
                                }
//                                $('#divManageDetail').unblock();
                                $rootScope.unloadLoading();
                                $scope.$apply();
                            });
                        }
                        else {
//                            $('#divManageDetail').unblock();
                            $rootScope.unloadLoading();
                            $scope.$apply();
                        }
                    }else{
                         $.growl.error({message: "Gagal mendapatkan data detail metode lelang!"});
                         return; 
                    }
                    $scope.$apply();
                });
            };
            $scope.simpanLelang = function() {
                if ($scope.metodeTerpilih === undefined) {
                    $.growl.error({title: "[PERINGATAN]", message: "Metode Pengadaan belum dipilih"});
                    return;
                }
                if ($scope.metodeEvaluasiTerpilih === undefined) {
                    //$.growl.error({title: "[PERINGATAN]", message: "Metode evaluasi belum dipilih"});
                    //return;
                    //$scope.metodeEvaluasiTerpilih.metode_evaluasi_id = 0;
                }
                if ($scope.newLelang.lokasiProyek === "") {
                    $.growl.error({title: "[PERINGATAN]", message: "Lokasi proyek belum diisi"});
                    return;
                }
                if ($scope.newLelang.nilaiHPS === "") {
                    $.growl.error({title: "[PERINGATAN]", message: "Nilai HPS masih kosong"});
                    return;
                }

                if ($scope.tahapan.length === 0) {
                    $.growl.error({title: "[PERINGATAN]", message: "Tahapan belum digenerate"});
                    return;
                }
                var metode_evaluasi_terpilih;
                if ($scope.metodeEvaluasiTerpilih === undefined) {
                    metode_evaluasi_terpilih = 0;
                }
                else {
                    metode_evaluasi_terpilih = $scope.metodeEvaluasiTerpilih.metode_evaluasi_id;
                }
                var master = {
                    paket_id:$scope.idPaket,
                    metode_id:$scope.metodeTerpilih.metode_id,
                    metode_evaluasi_id:metode_evaluasi_terpilih,
                    lokasi:$scope.newLelang.lokasiProyek,
                    nilai_hps:Number($scope.nilaiHPS),
                    no_surat:$scope.newLelang.noSurat
                };
                var temp;
                var detail = [];
                for (var i = 0; i < $scope.tahapan.length; i++)
                {
                    temp = [];
                    temp.push($scope.tahapan[i].metode_det_id);
                    temp.push(i + 1);
                    temp.push($scope.idPaket);
                    detail.push(temp);
                }
//                $('#divManageDetail').block({
//                    message: '<div style="text-align:center;"><h4>Silakan tunggu..</h4></div>',
//                    css: {border: '3px solid #a00'}
//                });
                $rootScope.loadLoading("Silahkan Tunggu...");
              
                        
                $http.post($rootScope.url_api+'paket/manage', {
                    sessionID: $cookieStore.get('sessId'),
                    master: master,
                    detail: detail,
                    nama_paket: $scope.namaLelang,
                    page_id: $scope.page_id,
                    simpan_confirm: 0,
                    idPaket: $scope.idPaket,
                    username: $rootScope.userLogin
                }).success(function(reply) {
                    if (reply.status == 200) {
                        $.growl.notice({title: "[INFO]", message: "Berhasil mengatur Paket Pengadaan"});
//                        $('#divManageDetail').unblock();
                        $rootScope.unloadLoading();
                        $scope.$apply();
                        $state.transitionTo('paketLelang-manage');
                    }
                    else {
                        $.growl.error({title: "[PERINGATAN]", message: "Gagal mengatur Paket Pengadaan"});
//                        $('#divManageDetail').unblock();
                        $rootScope.unloadLoading();
                    }
                });
            };
            $scope.kunciLelang = function() {
                if ($scope.metodeTerpilih === undefined) {
                    $.growl.error({title: "[PERINGATAN]", message: "Metode Pengadaan belum dipilih"});
                    return;
                }
                if ($scope.metodeEvaluasiTerpilih === undefined) {
                    //$.growl.error({title: "[PERINGATAN]", message: "Metode evaluasi belum dipilih"});
                    //return;
                }
                if ($scope.newLelang.lokasiProyek === "") {
                    $.growl.error({title: "[PERINGATAN]", message: "Lokasi proyek belum diisi"});
                    return;
                }
                if ($scope.newLelang.nilaiHPS === "") {
                    $.growl.error({title: "[PERINGATAN]", message: "Nilai HPS masih kosong"});
                    return;
                }
                if ($scope.newLelang.noSurat === "") {
                    $.growl.error({title: "[PERINGATAN]", message: "Nomor surat belum diisi"});
                    return;
                }
                if ($scope.tahapan.length === 0) {
                    $.growl.error({title: "[PERINGATAN]", message: "Tahapan belum digenerate"});
                    return;
                }
                var metode_evaluasi_terpilih;
                if ($scope.metodeEvaluasiTerpilih === undefined) {
                    metode_evaluasi_terpilih = 0;
                }
                else {
                    metode_evaluasi_terpilih = $scope.metodeEvaluasiTerpilih.metode_evaluasi_id;
                }
                var master = [];
                master.push($scope.metodeTerpilih.metode_id);
                //master.push($scope.metodeEvaluasiTerpilih.metode_evaluasi_id);
                master.push(metode_evaluasi_terpilih);
                master.push($scope.newLelang.lokasiProyek);
                master.push(Number($scope.newLelang.nilaiHPS));
                master.push($scope.newLelang.noSurat);
                master.push($scope.idPaket);
                var temp;
                var detail = [];
                for (var i = 0; i < $scope.tahapan.length; i++)
                {
                    temp = [];
                    temp.push($scope.tahapan[i].metode_det_id);
                    temp.push(i + 1);
                    temp.push($scope.idPaket);
                    detail.push(temp);
                }
//                $('#divManageDetail').block({
//                    message: '<div style="text-align:center;"><h4>Silakan tunggu..</h4></div>',
//                    css: {border: '3px solid #a00'}
//                });

//--Mulai Sini Konfirmasi--
//                $rootScope.loadLoading("Silahkan Tunggu...");
//                eb.send('itp.paket.manage', {
//                    sessionID: $cookieStore.get('sessId'),
//                    master: master,
//                    detail: detail,
//                    nama_paket: $scope.namaLelang,
//                    page_id: $scope.page_id,
//                    simpan_confirm: 1
//                }, function(reply) {
//                    if (reply.status === 'ok') {
//                        $.growl.notice({title: "[INFO]", message: "Berhasil mengatur Paket Pengadaan"});
////                        $('#divManageDetail').unblock();
//                        $rootScope.unloadLoading();
//                        $scope.$apply();
//                        $state.transitionTo('paketLelang-manage');
//                    }
//                    else {
//                        $.growl.error({title: "[PERINGATAN]", message: "Gagal mengatur Paket Pengadaan"});
////                        $('#divManageDetail').unblock();
//                        $rootScope.unloadLoading();
//                    }
//                });
//-- End Konfirmasi//
                var lempar = {master: master, detail: detail, nama_paket: $scope.namaLelang, page_id: $scope.page_id, simpan_confirm: 1};
                var modalInstance = $modal.open({
                    templateUrl: 'confirmPaketLelang.html',
                    controller: konfirmasiPaketLelangCtrl,
                    resolve: {
                        item: function() {
                            return lempar;
                        }
                    }
                });
                modalInstance.result.then(function() {
//                    $.growl.notice({title: "[INFO]", message: "Berhasil mengatur Paket Pengadaan"});
////                        $('#divManageDetail').unblock();
//                    $rootScope.unloadLoading();
//                    $scope.$apply();
//                    $state.transitionTo('paketLelang-manage');
//                    $scope.manage();
                });
            };
            $scope.cancel = function() {
                $state.transitionTo('paketLelang-manage');
            };
           $scope.lihatEvaluasi = function(jenis_form_id, nama_tahapan) {
               //console.info("crud:"+jenis_form_id+JSON.stringify(nama_tahapan));
                var lempar;
                if (Number(jenis_form_id) === 5) {
                    lempar = AdministrasiLevel1;
                }
                else if (Number(jenis_form_id) === 15) {
                    lempar = TeknisLevel1;
                }
                else if (Number(jenis_form_id) === 26) {
                    lempar = Harga;
                }
                
                var data = {
                    lempar: lempar,
                    nama_tahapan: nama_tahapan
                };
                $modal.open({
                    templateUrl: 'detailEvaluasi.html',
                    controller: lihatDetailEvaluasiCtrl,
                    resolve: {
                        item: function() {
                            return data;
                        }
                    }
                });
            };
        })
        .controller('paketLelangEditCtrl', function($scope, $http, $modal, $rootScope, $stateParams, $state, $cookieStore) {
            $scope.paket;
            $scope.namaLelang;
            $scope.metode = [];
            $scope.metodeTerpilih;
            $scope.metodeEvaluasi = [];
            $scope.metodeEvaluasiTerpilih;
            $scope.lokasiProyek;
            $scope.nilaiHPS;
            $scope.noSurat;
            $scope.tahapan = [];
            $scope.idPaket = Number($stateParams.idPaket);
            $scope.isLocked;
            $scope.menuhome = 0;
            var page_id = 8;
            var Administrasi = [];
            var AdministrasiLevel1 = [];
            var AdministrasiLevel2 = [];
            var AdministrasiLevel3 = [];
            var Teknis = [];
            var TeknisLevel1 = [];
            var TeknisLevel2 = [];
            var TeknisLevel3 = [];
            var Harga = [];
//            eb.onopen = function() {
//                $rootScope.autorize();
//                $scope.init();
//            };

            $scope.init = function() {
                $scope.menuhome = $rootScope;
                $rootScope.authorize(loadAwal());
            };
            function loadAwal() {
                var arr = [];
                arr.push($scope.idPaket);
//                $('#divEditLelang').block({
//                    message: '<div style="text-align:center;"><h4>Silakan tunggu..</h4></div>',
//                    css: {border: '3px solid #a00'}
//                });
                $rootScope.loadLoading("Silahkan Tunggu...");
                $http.post($rootScope.url_api +'paket/selecttoedit', {
                    sessionID: $cookieStore.get('sessId'),
                    param: arr,
                    page_id: page_id
                }).success(function(reply) {
                    //console.info('reply ' + JSON.stringify(reply));
                    if (reply.status == 200) {
                        $scope.paket = reply.result.data;
                        $scope.namaLelang = $scope.paket[0].nama_paket;
                        $scope.lokasiProyek = $scope.paket[0].lokasi;
                        $scope.nilaiHPS = $scope.paket[0].total_hps;
                        $scope.noSurat = $scope.paket[0].no_surat;
                        if ($scope.paket[0].is_managed == 1) {
                            $scope.isLocked = false;
                        }
                        else if ($scope.paket[0].is_managed == 2) {
                            $scope.isLocked = true;
                        }

                        $http.post($rootScope.url_api +'metode/selectAll', {
                            sessionID: $cookieStore.get('sessId'),
                            page_id: page_id
                        }).success(function(reply) {
                            if (reply.status == 200) {
                                $scope.metode = reply.result.data;
                                
                                for (var i = 0; i < $scope.metode.length; i++)
                                {
                                    if ($scope.metode[i].metode_id === $scope.paket[0].metode_lelang_id)
                                    {
                                        $scope.metodeTerpilih = $scope.metode[i];
                                        break;
                                    }
                                   
                                }
                            }else{
                                 $.growl.error({message: "Gagal mendapatkan data jenis form metode!"});
                                 return; 

                            }
                            
                        });
                        $http.get($rootScope.url_api+'metodeEvaluasi/selectActive', {   
                        }).success(function(reply) {
                            if (reply.status == 200) {
                                $scope.metodeEvaluasi = reply.result.data;
                                for (var i = 0; i < $scope.metodeEvaluasi.length; i++)
                                {
                                    
                                    if ($scope.metodeEvaluasi[i].metode_evaluasi_id === $scope.paket[0].metode_evaluasi_id)
                                    {
                                       
                                        $scope.metodeEvaluasiTerpilih = $scope.metodeEvaluasi[i];
                                        break;
                                    }
                                }
                            }else{
                                 $.growl.error({message: "Gagal mendapatkan data jenis form metode evaluasi!"});
                                 return; 
                            }
                            
                        });
                        var arr = [];
                        arr.push($scope.idPaket);
                        $http.post($rootScope.url_api+'paket/detail/select', {
                            sessionID: $cookieStore.get('sessId'),
                            param: arr,
                            page_id: page_id
                        }).success(function(reply) {
                            $rootScope.unloadLoading();
                            if (reply.status == 200) {
                                $scope.tahapan = reply.result.data;
                            }else{
                                 $.growl.error({message: "Gagal mendapatkan data jenis form detail paket!"});
                                 return; 
                            }
                            if ($scope.paket.metode_evaluasi_id === 0)
//                                $('#divEditLelang').unblock();
                                $rootScope.unloadLoading();
                                $scope.$apply();
                        });
                        if ($scope.paket.metode_evaluasi_id !== 0) {
                            $http.post($rootScope.url_api+'metodeEvaluasi/getDetail', {
                                id: $scope.paket[0].metode_evaluasi_id
                            }).success(function(reply2) {
                                if (reply2.status == 200) {
                                    var hasil = reply2.result.data;
                                    for (var i = 0; i < hasil.length; i++) {
                                        if (hasil[i].jenis_detail == 'Administrasi') {
                                            Administrasi.push(hasil[i]);
                                        }
                                        else if (hasil[i].jenis_detail == 'Teknis') {
                                            Teknis.push(hasil[i]);
                                        }
                                        else if (hasil[i].jenis_detail == 'Harga') {
                                            Harga.push(hasil[i]);
                                        }
                                    }
                                    for (var i = 0; i < Administrasi.length; i++) {
                                        if (Administrasi[i].level == 1) {
                                            AdministrasiLevel1.push(Administrasi[i]);
                                        }
                                        else if (Administrasi[i].level == 2) {
                                            AdministrasiLevel2.push(Administrasi[i]);
                                        }
                                        else if (Administrasi[i].level == 3) {
                                            AdministrasiLevel3.push(Administrasi[i]);
                                        }
                                    }
                                    for (var i = 0; i < AdministrasiLevel2.length; i++) {
                                        AdministrasiLevel2[i].sub = [];
                                        for (var j = 0; j < AdministrasiLevel3.length; j++) {
                                            if (AdministrasiLevel3[j].parent == AdministrasiLevel2[i].kriteria_id) {
                                                AdministrasiLevel2[i].sub.push(AdministrasiLevel3[j]);
                                            }
                                        }
                                    }
                                    for (var i = 0; i < AdministrasiLevel1.length; i++) {
                                        AdministrasiLevel1[i].sub = [];
                                        for (var j = 0; j < AdministrasiLevel2.length; j++) {
                                            if (AdministrasiLevel2[j].parent == AdministrasiLevel1[i].kriteria_id) {
                                                AdministrasiLevel1[i].sub.push(AdministrasiLevel2[j]);
                                            }
                                        }

                                    }

                                    for (var i = 0; i < Teknis.length; i++) {
                                        if (Teknis[i].level == 1) {
                                            TeknisLevel1.push(Teknis[i]);
                                        }
                                        else if (Teknis[i].level == 2) {
                                            TeknisLevel2.push(Teknis[i]);
                                        }
                                        else if (Teknis[i].level == 3) {
                                            TeknisLevel3.push(Teknis[i]);
                                        }
                                    }
                                    for (var i = 0; i < TeknisLevel2.length; i++) {
                                        TeknisLevel2[i].sub = [];
                                        for (var j = 0; j < TeknisLevel3.length; j++) {
                                            if (TeknisLevel3[j].parent == TeknisLevel2[i].kriteria_id) {
                                                TeknisLevel2[i].sub.push(TeknisLevel3[j]);
                                            }
                                        }
                                    }
                                    for (var i = 0; i < TeknisLevel1.length; i++) {
                                        TeknisLevel1[i].sub = [];
                                        for (var j = 0; j < TeknisLevel2.length; j++) {
                                            if (TeknisLevel2[j].parent == TeknisLevel1[i].kriteria_id) {
                                                TeknisLevel1[i].sub.push(TeknisLevel2[j]);
                                            }
                                        }
                                    }
                                    
                                }else{
                                     $.growl.error({message: "Gagal mendapatkan data jenis form!"});
                                    return; 
                                }
//                                $('#divEditLelang').unblock();
                                $rootScope.unloadLoading();
                                
                            });
                        }
                    }else{
                         $.growl.error({message: "Gagal mendapatkan data jenis form 1!"});
                         return; 
                    }
                });
            }

            $scope.generateStep = function() {
                if ($scope.metodeTerpilih == undefined) {
                    $.growl.error({title: "[PERINGATAN]", message: "Metode Pengadaan belum dipilih"});
                    return;
                }
                if ($scope.metodeEvaluasiTerpilih == undefined) {
                    //$.growl.error({title: "[PERINGATAN]", message: "Metode evaluasi belum dipilih"});
                    //return;
                }
                var arr = [];
                arr.push($scope.metodeTerpilih.metode_id);
                Administrasi = [];
                AdministrasiLevel1 = [];
                AdministrasiLevel2 = [];
                AdministrasiLevel3 = [];
                Teknis = [];
                TeknisLevel1 = [];
                TeknisLevel2 = [];
                TeknisLevel3 = [];
//                $('#divEditLelang').block({
//                    message: '<div style="text-align:center;"><h4>Silakan tunggu..</h4></div>',
//                    css: {border: '3px solid #a00'}
//                });
                $rootScope.loadLoading("Silahkan Tunggu...");
                $http.post($rootScope.url_api+'metodeDetail/select', {
                    keyword:$scope.metodeTerpilih.metode_id,
                    page_id: page_id
                }).success(function(reply) {
                    if (reply.status == 200) {
                        $scope.tahapan = reply.result.data;
                        if ($scope.metodeEvaluasiTerpilih !== undefined) {
                            $http.post($rootScope.url_api+'metodeEvaluasi/getDetail', {
                                sessionID: $cookieStore.get('sessId'),
                                id: $scope.metodeEvaluasiTerpilih.metode_evaluasi_id
                            }).success(function(reply2) {
                                if (reply2.status == 200) {
                                    var hasil = reply2.result.data;
                                    for (var i = 0; i < hasil.length; i++) {
                                        if (hasil[i].jenis_detail == 'Administrasi') {
                                            Administrasi.push(hasil[i]);
                                        }
                                        else if (hasil[i].jenis_detail == 'Teknis') {
                                            Teknis.push(hasil[i]);
                                        }
                                        else if (hasil[i].jenis_detail == 'Harga') {
                                            Harga.push(hasil[i]);
                                        }
                                    }
                                    for (var i = 0; i < Administrasi.length; i++) {
                                        if (Administrasi[i].level == 1) {
                                            AdministrasiLevel1.push(Administrasi[i]);
                                        }
                                        else if (Administrasi[i].level == 2) {
                                            AdministrasiLevel2.push(Administrasi[i]);
                                        }
                                        else if (Administrasi[i].level == 3) {
                                            AdministrasiLevel3.push(Administrasi[i]);
                                        }
                                    }
                                    for (var i = 0; i < AdministrasiLevel2.length; i++) {
                                        AdministrasiLevel2[i].sub = [];
                                        for (var j = 0; j < AdministrasiLevel3.length; j++) {
                                            if (AdministrasiLevel3[j].parent == AdministrasiLevel2[i].kriteria_id) {
                                                AdministrasiLevel2[i].sub.push(AdministrasiLevel3[j]);
                                            }
                                        }
                                    }
                                    for (var i = 0; i < AdministrasiLevel1.length; i++) {
                                        AdministrasiLevel1[i].sub = [];
                                        for (var j = 0; j < AdministrasiLevel2.length; j++) {
                                            if (AdministrasiLevel2[j].parent == AdministrasiLevel1[i].kriteria_id) {
                                                AdministrasiLevel1[i].sub.push(AdministrasiLevel2[j]);
                                            }
                                        }

                                    }

                                    for (var i = 0; i < Teknis.length; i++) {
                                        if (Teknis[i].level == 1) {
                                            TeknisLevel1.push(Teknis[i]);
                                        }
                                        else if (Teknis[i].level == 2) {
                                            TeknisLevel2.push(Teknis[i]);
                                        }
                                        else if (Teknis[i].level == 3) {
                                            TeknisLevel3.push(Teknis[i]);
                                        }
                                    }
                                    for (var i = 0; i < TeknisLevel2.length; i++) {
                                        TeknisLevel2[i].sub = [];
                                        for (var j = 0; j < TeknisLevel3.length; j++) {
                                            if (TeknisLevel3[j].parent == TeknisLevel2[i].kriteria_id) {
                                                TeknisLevel2[i].sub.push(TeknisLevel3[j]);
                                            }
                                        }
                                    }
                                    for (var i = 0; i < TeknisLevel1.length; i++) {
                                        TeknisLevel1[i].sub = [];
                                        for (var j = 0; j < TeknisLevel2.length; j++) {
                                            if (TeknisLevel2[j].parent == TeknisLevel1[i].kriteria_id) {
                                                TeknisLevel1[i].sub.push(TeknisLevel2[j]);
                                            }
                                        }
                                    }
                                }else{
                                    $.growl.error({message: "Gagal mendapatkan data metode evaluasi detail!"});
                                }
//                                $('#divEditLelang').unblock();
                                $rootScope.unloadLoading();
                                
                            });
                        }
                        else {
                            $rootScope.unloadLoading();
                        }
                    }else {
                        $.growl.error({message: "Gagal mendapatkan data metode evaluasi"});
                    }
                    $rootScope.unloadLoading();
                    //$scope.$apply();
                });
            };
            $scope.lihatEvaluasi = function(jenis_form_id, nama_tahapan) {
                //alert("what");
                var lempar;
                if (Number(jenis_form_id) == 5) {
                    lempar = AdministrasiLevel1;
                }
                else if (Number(jenis_form_id) == 15) {
                    lempar = TeknisLevel1;
                }
                else if (Number(jenis_form_id) == 26) {
                    lempar = Harga;
                }
                //console.info(jenis_form_id+":"+lempar+":"+nama_tahapan);
                var data = {
                    lempar: lempar,
                    nama_tahapan: nama_tahapan
                };
                $modal.open({
                    templateUrl: 'detailEvaluasi.html',
                    controller: lihatDetailEvaluasiCtrl,
                    resolve: {
                        item: function() {
                            return data;
                        }
                    }
                });
            };
            $scope.updateLelang = function(simpan_confirm) {
                if ($scope.metodeTerpilih === undefined) {
                    $.growl.error({title: "[PERINGATAN]", message: "Metode Pengadaan belum dipilih"});
                    return;
                }
                if ($scope.metodeEvaluasiTerpilih === undefined) {
                    //$.growl.error({title: "[PERINGATAN]", message: "Metode evaluasi belum dipilih"});
                    //return;
                }
                if ($scope.lokasiProyek === "") {
                    $.growl.error({title: "[PERINGATAN]", message: "Lokasi proyek belum diisi"});
                    return;
                }
                if ($scope.nilaiHPS === "") {
                    $.growl.error({title: "[PERINGATAN]", message: "Nilai HPS masih kosong"});
                    return;
                }

                var metode_evaluasi_terpilih;
                if ($scope.metodeEvaluasiTerpilih === undefined) {
                    metode_evaluasi_terpilih = 0;
                }
                else {
                    metode_evaluasi_terpilih = $scope.metodeEvaluasiTerpilih.metode_evaluasi_id;
                }
                var master = {
                    paket_id:$scope.paket[0].paket_id,
                    metode_id:$scope.metodeTerpilih.metode_id,
                    metode_evaluasi_id:metode_evaluasi_terpilih,
                    lokasi:$scope.lokasiProyek,
                    nilai_hps:$scope.nilaiHPS,
                    no_surat:$scope.noSurat
                };
                
                var temp;
                var detail = [];
                for (var i = 0; i < $scope.tahapan.length; i++)
                {
                    temp = [];
                    temp.push($scope.tahapan[i].metode_det_id);
                    temp.push(i + 1);
                    temp.push($scope.idPaket);
                    detail.push(temp);
                }
                if (simpan_confirm === 0) {
                    $('#divEditLelang').block({
                        message: '<div style="text-align:center;"><h4>Silakan tunggu..</h4></div>',
                        css: {border: '3px solid #a00'}
                    });
//--Simpan Konfirmasi--
                    $rootScope.loadLoading("Silahkan Tunggu...");
                    $http.post($rootScope.url_api+'paket/update', {
                        master: master,
                        detail: detail,
                        nama_paket: $scope.namaLelang,
                        page_id: page_id,
                        simpan_confirm: simpan_confirm,
                        idPaket: $scope.idPaket,
                        username: $rootScope.userLogin
                    }).success(function(reply) {
                        if (reply.status == 200) {
                            $.growl.notice({title: "[INFO]", message: "Berhasil menyimpan Paket Pengadaan"});
                            $rootScope.unloadLoading();
                            $state.transitionTo('paketLelang-manage');
                        }
                        else {
                            $.growl.error({title: "[PERINGATAN]", message: "Gagal menyimpan Paket Pengadaan"});
                            $rootScope.unloadLoading();
                        }
                    });
//-- end --
                } else {
                    var lempar = {master: master, detail: detail, nama_paket: $scope.namaLelang, page_id: $scope.page_id, simpan_confirm: simpan_confirm, idPaket: $scope.idPaket, username: $rootScope.userLogin};
                    var modalInstance = $modal.open({
                        templateUrl: 'confirmPaketLelang.html',
                        controller: konfirmasiEditPaketLelangCtrl,
                        resolve: {
                            item: function() {
                                return lempar;
                            }
                        }
                    });
                }
            };
            $scope.cancel = function() {
                $state.transitionTo('paketLelang-manage');
            };
        });
var PilihPegawaiCtrl = function($scope, $modalInstance, eb, $cookieStore) {
    $scope.pegawai = [];
    $scope.totalItems = 0;
    $scope.currentPage = 1;
    $scope.maxSize = 10;
    $scope.kata = new Kata("");
    $scope.page_id = 10;
    eb.send('itp.pegawai.countSearchPanitia', {
        sessionID: $cookieStore.get('sessId'),
        nama_pegawai: "%" + $scope.kata.srcText + "%",
        page_id: $scope.page_id
    }, function(reply) {
        if (reply.status === 'ok') {
            $scope.totalItems = reply.result[0].count;
            //console.info("hasil totalItems itp.pegawai.countSearchPanitia " + JSON.stringify($scope.totalItems));
        } else {
            alert("Tekan Tombol Refresh (F5)");
            //console.info("gagal mendapatkan jumlah pegawai");
        }
        $scope.$apply();
    });
    eb.send('itp.pegawai.selectSearchPanitia', {
        sessionID: $cookieStore.get('sessId'),
        nama_pegawai: "%" + $scope.kata.srcText + "%",
        offset: 0,
        limit: 10,
        page_id: $scope.page_id
    }, function(reply) {
        if (reply.status === 'ok') {
            $scope.pegawai = reply.result;
            //console.info("hasil pegawai itp.pegawai.selectSearchPanitia " + JSON.stringify($scope.pegawai));
        } else {
            alert("Tekan Tombol Refresh (F5)");
            //console.info("gagal mendapatkan list pegawai");
        }
        $scope.$apply();
    });
    $scope.jLoad = function(current) {
        $scope.pegawai = [];
        $scope.currentPage = current;
        $scope.offset = (current * 10) - 10;
        eb.send('itp.pegawai.selectSearchPanitia', {
            sessionID: $cookieStore.get('sessId'),
            nama_pegawai: "%" + $scope.kata.srcText + "%",
            offset: $scope.offset,
            limit: 10,
            page_id: $scope.page_id
        }, function(reply) {
            if (reply.status === 'ok') {
                $scope.pegawai = reply.result;
            }
            $scope.$apply();
        });
    };
    $scope.cariPegawai = function() {
        eb.send('itp.pegawai.countSearchPanitia', {
            sessionID: $cookieStore.get('sessId'),
            nama_pegawai: "%" + $scope.kata.srcText + "%",
            page_id: $scope.page_id
        }, function(reply) {
            if (reply.status === 'ok') {
                $scope.totalItems = reply.result[0].count;
            }
            $scope.$apply();
        });
        eb.send('itp.pegawai.selectSearchPanitia', {
            sessionID: $cookieStore.get('sessId'),
            nama_pegawai: "%" + $scope.kata.srcText + "%",
            offset: 0,
            limit: 10,
            page_id: $scope.page_id
        }, function(reply) {
            if (reply.status === 'ok') {
                $scope.pegawai = reply.result;
            }
            $scope.$apply();
        });
    };
    $scope.pilihPegawai = function(pgw) {
        $modalInstance.close(pgw);
    };
    $scope.keluar = function() {
        $modalInstance.dismiss('cancel');
    };
};

var pilihTemplateCtrl = function($scope, $modalInstance, $http, $cookieStore, $rootScope){
    $scope.template = []; 
    $scope.offset = 0;
    $scope.limit = 10;
    $scope.currentPage = 0;
    $scope.textcari = "";
    $scope.temp_detail;
    
    $scope.loadData = function(){
        $http.get($rootScope.url_api+"template/countpanitia")
            .success(function(reply){
                $rootScope.unloadLoading();
                if(reply.status === 200){
                    var data = reply.result.data;
                    $scope.totalItems = data;
                }
                else{
                    $.growl.error({ message: "Gagal mendapatkan jumlah data paket!" });
                    return;
                }
            })
            .error(function(err) {
                $.growl.error({ message: "Gagal Akses API >"+err });
                return;
            });
            $scope.jLoad(1);
    };
    
    $scope.keluar = function() {
        $modalInstance.dismiss('cancel');
    };
    
    $scope.jLoad = function(current){
        $rootScope.loadLoadingModal("Silahkan Tunggu...");
        $scope.currentPage = current;
        $scope.offset = (current * 10) - 10;
        //console.info("of "+$scope.offset+" ,li: "+$scope.limit);
        $http.post($rootScope.url_api+"template/selectpanitia",{
            keyword: $scope.textcari, offset:$scope.offset, limit: $scope.limit, action:"generate" })
            .success(function(reply){
                //console.info(JSON.stringify("all:::"+reply));
                $rootScope.unloadLoadingModal();
                if(reply.status === 200){
                    var data = reply.result.data;
                    $scope.template =  data;
                    var arr1;
                    var arr2 = [];
                    for (var i = 0; i < $scope.template.length; i++) {
                        $scope.template[i].detail = [];
                        arr1 = [];
                        arr1.push($scope.template[i].id_template_panitia);
                        arr2.push(arr1);
                    }
                    $http.post($rootScope.url_api+"template/detailTempPanitia",{ param: arr2})
                    .success(function(reply2){
                        //console.info(JSON.stringify("alldet:::"+reply2));
                        if(reply2.status === 200){
                            $scope.temp_detail = reply2.result.data;
                                for (var i = 0; i < $scope.template.length; i++) {
                                    for (var j = 0; j < $scope.temp_detail.length; j++) {
                                        if (Number($scope.template[i].id_template_panitia) === Number($scope.temp_detail[j].id_template_panitia)) {
                                            $scope.template[i].detail.push($scope.temp_detail[j]);
                                        }
                                    }
                                }
                        }
                    })
                    .error(function(err) {
                        $.growl.error({ message: "Gagal Akses API >"+err });
                        $rootScope.unloadLoadingModal();
                        return;
                    });
                    //console.info("data:"+JSON.stringify($scope.template));
                    /*
                    for(var i = 0; i < data.length; i++){
                        if($scope.template.length > 0){
                            var foundactive = $.map($scope.template, function (val) {
                                return (val.id_template_panitia === data[i].id_template_panitia) ? val : null;
                            });
                            if(foundactive.length < 1){
                                var found = $.map(data , function (val) {
                                    return (val.id_template_panitia === data[i].id_template_panitia) ? val : null;
                                });
                                var hasil = {
                                    id_template_panitia : data[i].id_template_panitia, 
                                    nama_template : data[i].nama_template,
                                    detail : found
                                };
                                $scope.template.push(hasil);
                            }
                        }
                        else{
                            var foundactive = $.map(data , function (val) {
                                return (val.id_template_panitia === data[i].id_template_panitia) ? val : null;
                            });
                            var hasil = {
                                id_template_panitia : data[i].id_template_panitia, 
                                nama_template : data[i].nama_template,
                                detail : foundactive
                            };
                            $scope.template.push(hasil);
                        }
                    }
                    */
                }
                else{
                    $.growl.error({ message: "Gagal mendapatkan data template!" });
                    return;
                }
            })
            .error(function(err) {
                $.growl.error({ message: "Gagal Akses API >"+err });
                return;
            });
            
    };
    
    $scope.caritemplate = function(){
        $scope.template = [];
        $scope.jLoad(0);  
    };
    
    $scope.pilihTemplate = function(pgw) {
        $modalInstance.close(pgw);
    };
};

var aturTahapanModalCtrl = function($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {

    $scope.flowInfo = [];
    $scope.page_id = 2;
    $scope.tampilkanAlert = false;
    $scope.tampilkanOpsiPublish;
    $scope.tampilkanOpsiTerendah;
    $scope.listPanitia = [];
    $scope.panitiaTerpilih;
    $scope.evaluator = [];
    var evaluatorToSend = [];
    $scope.listCalonApprover = [];
    $scope.approval = [];
    var evaluatorCekMasuk = [];
    $scope.approvalCekMasuk = [];
    var approvalToSend = [];
    $scope.approvalTerpilih;
    var approvalEmail = '';
    var evaluatorEmail = '';
    var approvalRemoveEmail = '';
    var evaluatorRemoveEmail = '';
    $scope.init = function() {
        $rootScope.authorize(initialize());
    };
    function initialize() {
        var param = [];
        param.push(Number(item.flowpaket_id));
        //itp.paketDetail.getInfoForSettingDate
        $http.post($rootScope.url_api + 'paket/detail/info4settingdate', {
            param: param
        }).success(function(reply) {
            if (reply.status === 200) {
                $scope.flowInfo = reply.result.data[0];
                if (!$scope.flowInfo.tgl_mulai)
                    $scope.flowInfo.tgl_mulai = '';
                if (!$scope.flowInfo.tgl_selesai)
                    $scope.flowInfo.tgl_selesai = '';
                if ($scope.flowInfo.jenis_form_id == 1 || $scope.flowInfo.jenis_form_id == 2 || $scope.flowInfo.jenis_form_id == 3 || $scope.flowInfo.jenis_form_id == 5 ||
                        $scope.flowInfo.jenis_form_id == 6 || $scope.flowInfo.jenis_form_id == 7 || $scope.flowInfo.jenis_form_id == 15 || $scope.flowInfo.jenis_form_id == 29 ||
                        $scope.flowInfo.jenis_form_id == 16 || $scope.flowInfo.jenis_form_id == 17 || $scope.flowInfo.jenis_form_id == 19 || $scope.flowInfo.jenis_form_id == 26 ||
                        $scope.flowInfo.jenis_form_id == 30 || $scope.flowInfo.jenis_form_id == 31) {
                    $scope.tampilkanOpsiPublish = false;
                }
                else {
                    $scope.tampilkanOpsiPublish = true;
                }
                if($scope.flowInfo.jenis_form_id == 27){
                    $scope.tampilkanOpsiTerendah = true;
                }else{
                    $scope.tampilkanOpsiTerendah = false;
                }
                if($scope.flowInfo.jenis_form_id == 19){
                    $scope.tampilkanOpsiTanpaApproval = true;
                }else{
                    $scope.tampilkanOpsiTanpaApproval = false;
                }
                $scope.flowInfo.publish = ($scope.flowInfo.publish === 1 || $scope.flowInfo.publish === '1' || $scope.flowInfo.publish === 'true');
                $scope.flowInfo.show_terendah = ($scope.flowInfo.show_terendah === 1 || $scope.flowInfo.show_terendah === '1' || $scope.flowInfo.show_terendah === 'true');
                $scope.flowInfo.tanpa_approval = ($scope.flowInfo.tanpa_approval === 1 || $scope.flowInfo.tanpa_approval === '1' || $scope.flowInfo.tanpa_approval === 'true');
                var param2 = [];
                param2.push(item.paket_lelang_id);
                if ($scope.flowInfo.jenis_form_id == 19 || $scope.flowInfo.jenis_form_id == 20 || $scope.flowInfo.jenis_form_id == 21 || $scope.flowInfo.jenis_form_id == 23) {
                    $rootScope.loadLoadingModal("Silahkan Tunggu...");
                    //itp.panitia.select
                    $http.post($rootScope.url_api + 'panitia/select', {
                        param: param2
                    }).success(function(reply) {
                        if (reply.status === 200) {
                            $scope.listCalonApprover = reply.result.data;
                            $scope.listCalonApprover.forEach(function(a){
                                a.level = 0;
                            });
                            //itp.approver.select
                            $http.post($rootScope.url_api + 'approver/select', {
                                flow_paket_id: item.flowpaket_id
                            }).success(function(reply) {
                                if (reply.status === 200) {
                                    $scope.approval = reply.result.data;
                                    if($scope.approval.length > 0){
                                        $scope.approval.forEach(function(obj) {
                                            $scope.approvalCekMasuk.push({
                                                panitia_id: obj.panitia_id,
                                                pegawai_id: obj.pegawai_id,
                                                nama_pegawai: obj.nama_pegawai,
                                                jabatan_nama: obj.jabatan_nama,
                                                level: obj.level,
                                                email: obj.email
                                            });
                                            if (obj.status_approval_id != 1){
                                                $scope.sudahAdaApproval = true;
                                            }
                                        });
                                    } else {
                                        //Ambil susunan default dari panitia
                                        $scope.listCalonApprover.forEach(function(obj) {
                                            if(Number(obj.approver_ke) > 0){
                                                $scope.approval.push({
                                                    panitia_id: obj.panitia_id,
                                                    pegawai_id: obj.pegawai_id,
                                                    nama_pegawai: obj.nama_pegawai,
                                                    jabatan_nama: obj.jabatan_nama,
                                                    level: obj.approver_ke,
                                                    email: obj.email,
                                                    flag_active: 1,
                                                    status_approval_id: 1,
                                                    status: 'Pending'
                                                });
                                            }
                                        });
                                    }
                                }
                                $rootScope.unloadLoadingModal();
                            }).error(function(err) {
                                $.growl.error({message: "Gagal Akses API >" + err});
                                return;
                            });
                        }
                        $rootScope.unloadLoadingModal();
                    }).error(function(err) {
                        $.growl.error({message: "Gagal Akses API >" + err});
                        return;
                    });
                    $rootScope.loadLoadingModal("Silahkan Tunggu...");                    
                }
                if ($scope.flowInfo.jenis_form_id == 5 || $scope.flowInfo.jenis_form_id == 15) {
                    $rootScope.loadLoadingModal("Silahkan Tunggu...");
                    //itp.panitia.select
                    $http.post($rootScope.url_api + 'panitia/select', {
                        param: param2
                    }).success(function(reply) {
                        if (reply.status === 200) {
                            $scope.listPanitia = reply.result.data;
                        }
                        $rootScope.unloadLoadingModal();
                    }).error(function(err) {
                        $.growl.error({message: "Gagal Akses API >" + err});
                        return;
                    });
                    $rootScope.loadLoadingModal("Silahkan Tunggu...");
                    //itp.evaluator.select
                    $http.post($rootScope.url_api + 'evaluator/select', {
                        flow_paket_id: item.flowpaket_id
                    }).success(function(reply) {
                        if (reply.status === 200) {
                            $scope.evaluator = reply.result.data;
                            $scope.evaluator.forEach(function(obj) {
                                evaluatorCekMasuk.push({
                                    panitia_id: obj.panitia_id,
                                    pegawai_id: obj.pegawai_id,
                                    nama_pegawai: obj.nama_pegawai,
                                    jabatan_nama: obj.jabatan_nama,
                                    email: obj.email
                                });
                            });
                        }
                        $rootScope.unloadLoadingModal();
                    }).error(function(err) {
                        $.growl.error({message: "Gagal Akses API >" + err});
                        return;
                    });
                }
            }
        }).error(function(err) {
            $.growl.error({message: "Gagal Akses API >" + err});
            return;
        });
    }
    ;
    $scope.ubahRentang = function(obj) {
        $scope.flowInfo.rentang = obj;
    };
    $scope.hilangkanAlert = function() {
        $scope.tampilkanAlert = false;
    };
    $scope.changeEvaluator = function(obj) {
        $scope.panitiaTerpilih = obj;
    };
    $scope.tambahEvaluator = function() {
        if ($scope.panitiaTerpilih === undefined) {
            $.growl.error({title: "[PERINGATAN]", message: "Pegawai belum dipilih"});
            return;
        }
        for (var i = 0; i < $scope.evaluator.length; i++)
        {
            if ($scope.evaluator[i].panitia_id === $scope.panitiaTerpilih.panitia_id)
            {
                $.growl.error({title: "[PERINGATAN]", message: "Panitia sudah dipilih, tidak bisa dipilih lagi"});
                return;
            }
        }

        $scope.evaluator.push({
            panitia_id: $scope.panitiaTerpilih.panitia_id,
            pegawai_id: $scope.panitiaTerpilih.pegawai_id,
            nama_pegawai: $scope.panitiaTerpilih.nama_pegawai,
            jabatan_nama: $scope.panitiaTerpilih.jabatan_nama,
            flag_active: 1,
            email: $scope.panitiaTerpilih.email
        });
    };
    $scope.hapusEvaluator = function(panitia_id) {
        var idx = -1;
        for (var i = 0; i < $scope.evaluator.length; i++)
        {
            if ($scope.evaluator[i].panitia_id === panitia_id) {
                idx = i;
                break;
            }
        }
        $scope.evaluator.splice(idx, 1);
    };
    $scope.changeApproval = function(obj) {
        $scope.approvalTerpilih = obj;
    };
    $scope.changeLevel = function(obj) {
        $scope.approvalLevel = obj;
    };
    $scope.tambahApproval = function() {
        if ($scope.approvalTerpilih === undefined) {
            $.growl.error({title: "[PERINGATAN]", message: "Pegawai belum dipilih"});
            return;
        }
        if (!$scope.approvalLevel) {
            $.growl.error({title: "[PERINGATAN]", message: "Urutan approval dipilih"});
            return;
        }
        for (var i = 0; i < $scope.approval.length; i++)
        {
            if ($scope.approval[i].panitia_id === $scope.approvalTerpilih.panitia_id)
            {
                $.growl.error({title: "[PERINGATAN]", message: "Panitia sudah dipilih, tidak bisa dipilih lagi"});
                return;
            }
        }

        $scope.approval.push({
            panitia_id: $scope.approvalTerpilih.panitia_id,
            pegawai_id: $scope.approvalTerpilih.pegawai_id,
            nama_pegawai: $scope.approvalTerpilih.nama_pegawai,
            jabatan_nama: $scope.approvalTerpilih.jabatan_nama,
            level: $scope.approvalLevel,
            flag_active: 1,
            email: $scope.approvalTerpilih.email,
            status_approval_id: 1
        });
    };
    $scope.hapusApproval = function(panitia_id) {
        var idx = -1;
        for (var i = 0; i < $scope.approval.length; i++)
        {
            if ($scope.approval[i].panitia_id === panitia_id) {
                idx = i;
                break;
            }
        }
        $scope.approval.splice(idx, 1);
    };
    function contains(a, obj) {
        for (var i = 0; i < a.length; i++) {
            if (a[i] === obj) {
                return true;
                break;
            }
        }
        return false;
    }

    $scope.cektanggal = function() {
        var sel = parseInt(Date.parse($scope.flowInfo.tgl_selesai)) - parseInt(Date.parse($scope.flowInfo.tgl_mulai));
        //console.info("tgl>> "+sel);
        if ($scope.flowInfo.tgl_selesai < $scope.flowInfo.tgl_mulai) {
            $.growl.error({title: "[PERINGATAN]", message: "Waktu Selesai yang Anda Masukan Lebih Kecil Dari Tanggal Mulai"});
            return;
        }
        if (sel < 3600000) {
            $.growl.error({title: "[PERINGATAN]", message: "Rentang Waktu Mulai dan Selesai Harus Minimal 1 jam"});
            return;
        }
    };
    $scope.simpanTahapan = function() {

        var sel = parseInt(Date.parse($scope.flowInfo.tgl_selesai)) - parseInt(Date.parse($scope.flowInfo.tgl_mulai));
        if (($scope.flowInfo.tgl_selesai < $scope.flowInfo.tgl_mulai) && sel < 3600000) {
            $.growl.error({title: "[PERINGATAN]", message: "Rentang Waktu Tahapan yang Anda Pilih melebihi batas minimum rentang waktu per tahapan"});
            return;
        }
        if (sel < 3600000) {
            $.growl.error({title: "[PERINGATAN]", message: "Rentang Waktu Mulai dan Selesai Harus Minimal 1 jam"});
            return;
        }
        if ($scope.flowInfo.jenis_form_id == 19 || $scope.flowInfo.jenis_form_id == 20 || $scope.flowInfo.jenis_form_id == 21 || $scope.flowInfo.jenis_form_id == 23){
            if ($scope.approval.length === 0){
                $.growl.error({title: "[PERINGATAN]", message: "Daftar approval masih kosong"});
                return;
            }
        }

        if (($scope.flowInfo.rentang === 1 || $scope.flowInfo.rentang === '1') && $scope.flowInfo.tgl_selesai === '') {
            $scope.tampilkanAlert = true;
            return;
        }

        var param = [];
        param.push($scope.flowInfo.tgl_mulai);
        param.push($scope.flowInfo.tgl_selesai);
        param.push(Number($scope.flowInfo.status));
        param.push(Number($scope.flowInfo.rentang));
        var inputElements = document.getElementsByTagName('input');
        for (var i = 0; inputElements[i]; ++i) {
            if (inputElements[i].className === "uacheck") {
                if (inputElements[i].checked === true)
                    param.push(true);
                else
                    param.push(false);
            }
        }
        for (var i = 0; inputElements[i]; ++i) {
            if (inputElements[i].className === "uacheckrendah") {
                if (inputElements[i].checked === true)
                    param.push(true);
                else
                    param.push(false);
            }
        }
        param.push($scope.flowInfo.tanpa_approval);
        param.push(Number($scope.flowInfo.offline_online));
        param.push(Number(item.flowpaket_id));
        if($scope.flowInfo.tanpa_approval == true){
            $scope.approval = [];
        }
        for (var i = 0; i < $scope.approvalCekMasuk.length; i++) {
            var approvalMasuk = false;
            for (var j = 0; j < $scope.approval.length; j++) {
                if ($scope.approvalCekMasuk[i].panitia_id === $scope.approval[j].panitia_id) {
                    approvalMasuk = true;
                    break;
                }
            }
            if (!approvalMasuk) {
                if (approvalRemoveEmail === '') {
                    approvalRemoveEmail += $scope.approvalCekMasuk[i].email;
                } else {
                    approvalRemoveEmail += ',' + $scope.approvalCekMasuk[i].email;
                }
            }
        }

        for (var i = 0; i < $scope.approval.length; i++) {
            var a = false;
            for (var j = 0; j < $scope.approvalCekMasuk.length; j++) {
                if ($scope.approval[i].panitia_id === $scope.approvalCekMasuk[j].panitia_id) {
                    a = true;
                    break;
                }
            }
            if (!a) {
                if (approvalEmail === '') {
                    approvalEmail += $scope.approval[i].email;
                } else {
                    approvalEmail += ',' + $scope.approval[i].email;
                }
            }

            approvalToSend.push($scope.approval[i]);
        }
        for (var i = 0; i < $scope.listCalonApprover.length; i++) {
            var masuk = false;
            for (var j = 0; j < $scope.approval.length; j++) {
                if ($scope.listCalonApprover[i].panitia_id === $scope.approval[j].panitia_id) {
                    masuk = true;
                    break;
                }
            }
            if (!masuk) {
                approvalToSend.push({
                    panitia_id: $scope.listCalonApprover[i].panitia_id,
                    pegawai_id: $scope.listCalonApprover[i].pegawai_id,
                    nama_pegawai: $scope.listCalonApprover[i].nama_pegawai,
                    level: 0,
                    flag_active: 0
                });
            }
        }

        for (var i = 0; i < evaluatorCekMasuk.length; i++) {
            var evaluatorMasuk = false;
            for (var j = 0; j < $scope.evaluator.length; j++) {
                if (evaluatorCekMasuk[i].panitia_id === $scope.evaluator[j].panitia_id) {
                    evaluatorMasuk = true;
                    break;
                }
            }
            if (!evaluatorMasuk) {
                if (evaluatorRemoveEmail === '') {
                    evaluatorRemoveEmail += evaluatorCekMasuk[i].email;
                } else {
                    evaluatorRemoveEmail += ',' + evaluatorCekMasuk[i].email;
                }
            }
        }

        for (var i = 0; i < $scope.evaluator.length; i++) {
            var b = false;
            for (var j = 0; j < evaluatorCekMasuk.length; j++) {
                if ($scope.evaluator[i].panitia_id === evaluatorCekMasuk[j].panitia_id) {
                    b = true;
                    break;
                }
            }
            if (!b) {
                if (evaluatorEmail === '') {
                    evaluatorEmail += $scope.evaluator[i].email;
                } else {
                    evaluatorEmail += ',' + $scope.evaluator[i].email;
                }
            }
            evaluatorToSend.push($scope.evaluator[i]);
        }
        for (var i = 0; i < $scope.listPanitia.length; i++) {
            var sudahMasuk = false;
            for (var j = 0; j < $scope.evaluator.length; j++) {
                if ($scope.listPanitia[i].panitia_id === $scope.evaluator[j].panitia_id) {
                    sudahMasuk = true;
                    break;
                }
            }
            if (!sudahMasuk) {
                evaluatorToSend.push({
                    panitia_id: $scope.listPanitia[i].panitia_id,
                    pegawai_id: $scope.listPanitia[i].pegawai_id,
                    nama_pegawai: $scope.listPanitia[i].nama_pegawai,
                    flag_active: 0
                });
            }
        }

        $rootScope.loadLoadingModal("Silahkan Tunggu...");
        //itp.paketDetail.update
        $http.post($rootScope.url_api + 'paket/detail/update', {
            username: $rootScope.userLogin,
            param: param,
            page_id: $scope.page_id,
            nama_paket: item.nama_paket,
            nama_tahapan: item.nama_tahapan,
            approval: approvalToSend,
            evaluator: evaluatorToSend,
            flow_paket_id: item.flowpaket_id
        }).success(function(reply) {
            if (reply.status === 200) {
                if ($scope.flowInfo.jenis_form_id === 24) {
                    //itp.paket.getCurrentPeserta
                    $http.post($rootScope.url_api + 'paket/currpeserta', {
                        paket_lelang_id: item.paket_lelang_id
                    }).success(function(reply2) {
                        if (reply2.status === 200) {
                            var mailReceiver = "";
                            reply2.result.data.forEach(function(res) {
                                if (res.email)
                                    mailReceiver += "," + res.email;
                                if (res.email_pribadi)
                                    mailReceiver += "," + res.email_pribadi;
                            });
                            mailReceiver = mailReceiver.substring(1);
                            var variables = [];
                            variables.push({key: "tglawal", value: $scope.flowInfo.tgl_mulai});
                            variables.push({key: "tglakhir", value: $scope.flowInfo.tgl_selesai});
                            variables.push({key: "paket", value: item.nama_paket});
                            //itp.mailconfig.getMailContent
                            $http.post($rootScope.url_api + "mailconfig/getcontent", {
                                id_konten_email: 19,
                                variables: variables
                            }).success(function(reply3) {
                                var mailBody = "";
                                var mailSubject = "";
                                if (reply3.status === 200) {
                                    mailBody = reply3.result.data.mailBody;
                                    mailSubject = reply3.result.data.mailSubject;
                                }
                                //itp.panitia.kirimNotif
                                $http.post($rootScope.url_api + 'panitia/notif', {
                                    username: $rootScope.userLogin,
                                    alamat: mailReceiver,
                                    subject: mailSubject,
                                    type: "Notifikasi Auction",
                                    body: mailBody
                                }).success(function(reply4) {

                                });
                            });
                        }
                    });
                }

                var statusKirimEmail = [];
                if (approvalEmail !== '') {
                    var variables = [];
                    variables.push({key: "paket", value: item.nama_paket});
                    variables.push({key: "tahapan", value: item.nama_tahapan});
                    //itp.mailconfig.getMailContent
                    $http.post($rootScope.url_api + "mailconfig/getcontent", {
                        id_konten_email: 12,
                        variables: variables
                    }).success(function(reply6) {
                        var mailBody = "";
                        var mailSubject = "";
                        if (reply6.status === 200) {
                            mailBody = reply6.result.data.mailBody;
                            mailSubject = reply6.result.data.mailSubject;
                        }
                        //itp.panitia.kirimNotif
                        $http.post($rootScope.url_api + 'panitia/notif', {
                            username: $rootScope.userLogin,
                            alamat: approvalEmail,
                            subject: mailSubject,
                            type: "Notifikasi Approval",
                            body: mailBody
                        }).success(function(reply2) {
                            if (reply2.status === 200) {
                                statusKirimEmail.push(true);
                            } else {
                                statusKirimEmail.push(false);
                            }
                        });
                    });
                }
                if (evaluatorEmail !== '') {
                    var variables = [];
                    variables.push({key: "paket", value: item.nama_paket});
                    variables.push({key: "tahapan", value: item.nama_tahapan});
                    //itp.mailconfig.getMailContent
                    $http.post($rootScope.url_api + "mailconfig/getcontent", {
                        id_konten_email: 13,
                        variables: variables
                    }).success(function(reply6) {
                        var mailBody = "";
                        var mailSubject = "";
                        if (reply6.status === 200) {
                            mailBody = reply6.result.data.mailBody;
                            mailSubject = reply6.result.data.mailSubject;
                        }
                        //itp.panitia.kirimNotif
                        $http.post($rootScope.url_api + 'panitia/notif', {
                            username: $rootScope.userLogin,
                            alamat: evaluatorEmail,
                            subject: mailSubject,
                            type: "Notifikasi Evaluator",
                            body: mailBody
                        }).success(function(reply2) {
                            if (reply2.status === 200) {
                                statusKirimEmail.push(true);
                            } else {
                                statusKirimEmail.push(false);
                            }
                        });
                    });
                }

                if (approvalRemoveEmail !== '') {
                    var variables = [];
                    variables.push({key: "paket", value: item.nama_paket});
                    variables.push({key: "tahapan", value: item.nama_tahapan});
                    //itp.mailconfig.getMailContent
                    $http.post($rootScope.url_api + "mailconfig/getcontent", {
                        id_konten_email: 14,
                        variables: variables
                    }).success(function(reply6) {
                        var mailBody = "";
                        var mailSubject = "";
                        if (reply6.status === 200) {
                            mailBody = reply6.result.data.mailBody;
                            mailSubject = reply6.result.data.mailSubject;
                        }
                        //itp.panitia.kirimNotif
                        $http.post($rootScope.url_api + 'panitia/notif', {
                            username: $rootScope.userLogin,
                            alamat: approvalRemoveEmail,
                            subject: mailSubject,
                            type: "Notifikasi Approval",
                            body: mailBody
                        }).success(function(reply2) {
                            if (reply2.status === 200) {
                                statusKirimEmail.push(true);
                            } else {
                                statusKirimEmail.push(false);
                            }
                        });
                    });
                }

                if (evaluatorRemoveEmail !== '') {
                    var variables = [];
                    variables.push({key: "paket", value: item.nama_paket});
                    variables.push({key: "tahapan", value: item.nama_tahapan});
                    //itp.mailconfig.getMailContent
                    $http.post($rootScope.url_api + "mailconfig/getcontent", {
                        id_konten_email: 15,
                        variables: variables
                    }).success(function(reply6) {
                        var mailBody = "";
                        var mailSubject = "";
                        if (reply6.status === 200) {
                            mailBody = reply6.result.data.mailBody;
                            mailSubject = reply6.result.data.mailSubject;
                        }
                        //itp.panitia.kirimNotif
                        $http.post($rootScope.url_api + 'panitia/notif', {
                            username: $rootScope.userLogin,
                            alamat: evaluatorRemoveEmail,
                            subject: mailSubject,
                            type: "Notifikasi Evaluator",
                            body: mailBody
                        }).success(function(reply2) {
                            if (reply2.status === 200)
                                statusKirimEmail.push(true);
                            else
                                statusKirimEmail.push(false);
                        });
                    });
                }

                $.growl.notice({title: "[INFO]", message: "Berhasil mengubah tahapan"});
                $rootScope.unloadLoadingModal();
                $modalInstance.close();
            }
            else {
                $.growl.error({title: "[PERINGATAN]", message: "Gagal mengubah tahapan"});
                $rootScope.unloadLoadingModal();
            }
        }).error(function(err) {
            $.growl.error({message: "Gagal Akses API >" + err});
            return;
        });
    };
    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };
};
var hapusPaketLelangCtrl = function($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
    $scope.delete = function() {
        //itp.paket.cekongoing
        $http.post($rootScope.url_api+'paket/cekongoing', {
            sessionID: $cookieStore.get('sessId'),
            page_id: item.page_id,
            paket_id: item.paket_lelang_id
        }).success(function(replyLangsung) {
            console.info(JSON.stringify(replyLangsung.result.data))
            if (replyLangsung.result.data == false) {
                //itp.paket.nonaktifkan
                $http.post($rootScope.url_api+'paket/nonaktifkan', {
                    sessionID: $cookieStore.get('sessId'),
                    page_id: item.page_id,
                    paket_id: item.paket_lelang_id,
                    nama_paket: item.nama_paket
                }).success(function(reply){
                    if (reply.status == 200) {
                        $.growl.notice({title: "[INFO]", message: "Berhasil menghapus Paket Pengadaan"});
                        $modalInstance.close();
                    }
                    else {
                        $.growl.error({title: "[WARNING]", message: "Terjadi kesalahan"});
                        $modalInstance.close();
                    }
                    $scope.$apply();
                })
            }
            else {
                $.growl.error({title: "[WARNING]", message: "Paket Pengadaan tidak bisa dihapus karena telah berlangsung"});
            }
            $scope.$apply();
        });
    };
    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };
};
var ubahStatusLelangCtrl = function($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
    $scope.loadAwal = function() {
        $scope.namapaketlelang = item.nama_paket;
    };
    $scope.ubahStatus = function() {
        //itp.paket.ubahStatus
        $http.post($rootScope.url_api + 'paket/ubahstatus', {
            paket_lelang_id: item.paket_lelang_id,
            nama_paket: item.nama_paket,
            status_kelangsungan: item.status_kelangsungan,
            username: $rootScope.userLogin
        }).success(function(reply) {
            if (reply.status === 200) {
                $.growl.notice({title: "[INFO]", message: "Berhasil mengubah Status Pengadaan"});
                $modalInstance.close();
            }
        }).error(function(err) {
            $.growl.error({message: "Gagal Akses API > " + err});
            return;
        });
    };
    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };
};
var lihatDetailEvaluasiCtrl = function($scope, $modalInstance, item) {
    //console.info(JSON.stringify(item));
    $scope.kriteria = item.lempar;
    $scope.tahapan = item.nama_tahapan;
    $scope.keluar = function() {
        $modalInstance.dismiss('cancel');
    };
};
var warningUbahPaketCtrl = function($scope, $modalInstance) {
    $scope.tetapUbah = function() {
        $modalInstance.close();
    };
    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };
};
function Kata(srcText) {
    var self = this;
    self.srcText = srcText;
}

function Lelang(lokasiProyek, nilaiHPS, noSurat) {
    var self = this;
    self.lokasiProyek = lokasiProyek;
    self.nilaiHPS = nilaiHPS;
    self.noSurat = noSurat;
}

var konfirmasiPaketLelangCtrl = function($scope, $modalInstance, item, $cookieStore, $http, $rootScope, $state) {
    var page_id = 8;
    $scope.confirm = function() {
        $rootScope.loadLoading("Silahkan Tunggu...");
        $http.post($rootScope.url_api +'paket/manage', {
            sessionID: $cookieStore.get('sessId'),
            master: item.master,
            detail: item.detail,
            nama_paket: item.namaLelang,
            page_id: item.page_id,
            simpan_confirm: item.simpan_confirm
        }).success(function(reply) {
            if (reply.status == 200) {
                $.growl.notice({title: "[INFO]", message: "Berhasil mengatur Paket Pengadaan"});
                $rootScope.unloadLoading();
                $scope.$apply();
                $state.transitionTo('paketLelang-manage');
                $modalInstance.close();
            }
            else {
                $.growl.error({title: "[PERINGATAN]", message: "Gagal mengatur Paket Pengadaan"});
                $rootScope.unloadLoading();
                $modalInstance.close();
            }
        });
    };
    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };
};
var konfirmasiEditPaketLelangCtrl = function($scope, $modalInstance, item, $cookieStore, $http, $rootScope, $state) {
    var page_id = 8;
    $scope.confirm = function() {
        $rootScope.loadLoading("Silahkan Tunggu...");
        $http.post($rootScope.url_api+'paket/update', {
            sessionID: $cookieStore.get('sessId'),
            master: item.master,
            detail: item.detail,
            nama_paket: item.nama_paket,
            page_id: page_id,
            simpan_confirm: item.simpan_confirm,
            idPaket: item.idPaket,
            username: item.username
        }).success(function(reply) {
            if (reply.status == 200) {
                $.growl.notice({title: "[INFO]", message: "Berhasil menyimpan Paket Pengadaan"});
                $rootScope.unloadLoading();
                $modalInstance.close();
                $scope.$apply();
                $state.transitionTo('paketLelang-manage');
            }
            else {
                $.growl.error({title: "[PERINGATAN]", message: "Gagal menyimpan Paket Pengadaan"});
                $modalInstance.close();
                $rootScope.unloadLoading();
            }
        });
    };
    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };
};