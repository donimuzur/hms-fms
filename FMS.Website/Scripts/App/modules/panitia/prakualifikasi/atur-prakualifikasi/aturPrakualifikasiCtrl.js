angular.module('eprocAppPanitia')
        .controller('listAturPrakualifikasiCtrl', function($scope, $http, $rootScope, $modal, $state, $cookieStore) {

            $scope.totalItems = 0;
            $scope.currentPage = 1;
            $scope.maxSize = 10;
            $scope.kata = new Kata("");
            $scope.userBisaMengatur = false;
            $scope.userBisaMenambah = false;
            $scope.userBisaMengubah = false;
            $scope.userBisaMenghapus = false;
            $scope.searchBy = 0;
            $scope.metodeEvaluasi = [];
            $scope.menuhome = 0;
            var page_id = 140;
            $scope.page_id = 140;

            $scope.init = function() {
                $scope.menuhome = $rootScope.menuhome;
                $rootScope.getSession().then(function(result) {
                    $rootScope.userSession = result.data.data;
                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
                    $rootScope.authorize(new function() {
                        loadAwal();
                    });
                });
            };

            function loadAwal() {
                var param = [];
                param.push($rootScope.userlogged);
                param.push(page_id);

                $http.post($rootScope.url_api + "roles/check_authority",
                        {username: $rootScope.userLogin, page_id: $scope.page_id, jenis_mengatur: 2})
                        .success(function(reply) {
                            if (reply.status === 200) {
                                var data = reply.result.data[0];
                                $scope.userBisaMenambah = data.bisa_mengatur;
                            }
                            else {
                                $.growl.error({message: "Gagal mendapatkan data hak akses!"});
                                return;
                            }
                        })
                        .error(function(err) {
                            $.growl.error({message: "Gagal Akses API >" + err});
                            return;
                        });

                $http.post($rootScope.url_api + "roles/check_authority",
                        {username: $rootScope.userLogin, page_id: $scope.page_id, jenis_mengatur: 3})
                        .success(function(reply) {
                            if (reply.status === 200) {
                                var data = reply.result.data[0];
                                $scope.userBisaMengubah = data.bisa_mengatur;
                            }
                            else {
                                $.growl.error({message: "Gagal mendapatkan data hak akses!"});
                                return;
                            }
                        })
                        .error(function(err) {
                            $.growl.error({message: "Gagal Akses API >" + err});
                            return;
                        });

                $http.post($rootScope.url_api + "roles/check_authority",
                        {username: $rootScope.userLogin, page_id: $scope.page_id, jenis_mengatur: 4})
                        .success(function(reply) {
                            if (reply.status === 200) {
                                var data = reply.result.data[0];
                                $scope.userBisaMenghapus = data.bisa_mengatur;
                            }
                            else {
                                $.growl.error({message: "Gagal mendapatkan data hak akses!"});
                                return;
                            }
                        })
                        .error(function(err) {
                            $.growl.error({message: "Gagal Akses API >" + err});
                            return;
                        });

                $scope.cek_authorize($scope.loadMetodeEvaluasi());

            } // end loadAwal

            $scope.cek_authorize = function(action) {
                $rootScope.authorize(action);
            };

            $scope.loadMetodeEvaluasi = function() {
                //alert("mlebuu");
                $rootScope.loadLoading("Silahkan Tunggu...");

                $http.post($rootScope.url_api + "metodeEvaluasi/countByPage",
                        {keyword: "%" + $scope.kata.srcText + "%"})
                        .success(function(reply) {
                            if (reply.status === 200) {
                                var data = reply.result.data;
                                $scope.totalItems = data;
                            }
                            else {
                                $.growl.error({message: "Gagal mendapatkan data metode evaluasi!!"});
                                $rootScope.unloadLoading();
                                return;
                            }
                            $rootScope.unloadLoading();
                        })
                        .error(function(err) {
                            $.growl.error({message: "Gagal Akses API >" + err});
                            $rootScope.unloadLoading();
                            return;
                        });

                var offset = 0;
                $scope.metodeEvaluasi = [
                    {nama_metode: "Metode Prakualifikasi 01", tgl_awal:"01-01-216", tgl_akhir:"30-01-2016",flag_active:true}
                ];
                /*
                $http.post($rootScope.url_api + "metodeEvaluasi/selectByPage",
                        {offset: offset, limit: $scope.maxSize, keyword: "%" + $scope.kata.srcText + "%"})
                        .success(function(reply) {
                            if (reply.status === 200) {
                                var data = reply.result.data;
                                $scope.metodeEvaluasi = data;
                            }
                            else {
                                $.growl.error({message: "Gagal mendapatkan data metode evaluasi!!"});
                                $rootScope.unloadLoading();
                                return;
                            }
                            $rootScope.unloadLoading();
                        })
                        .error(function(err) {
                            $.growl.error({message: "Gagal Akses API >" + err});
                            $rootScope.unloadLoading();
                            return;
                        });
                */
            };

            $scope.jLoad = function(current) {
                $scope.metodeEvaluasi = [];
                $scope.currentPage = current;
                var offset = (current * 10) - 10;
                $http.post($rootScope.url_api + "metodeEvaluasi/selectByPage",
                        {offset: offset, limit: $scope.maxSize, keyword: "%" + $scope.kata.srcText + "%"})
                        .success(function(reply) {
                            if (reply.status === 200) {
                                var data = reply.result.data;
                                $scope.metodeEvaluasi = data;
                                for (var i = 0; i < $scope.metodeEvaluasi.length; i++) {
                                    $scope.metodeEvaluasi[i].flag_active = $rootScope.strtobool($scope.metodeEvaluasi[i].flag_active);
                                }
                            }
                            else {
                                $.growl.error({message: "Gagal mendapatkan data metode evaluasi!!"});
                                $rootScope.unloadLoading();
                                return;
                            }
                            $rootScope.unloadLoading();
                        })
                        .error(function(err) {
                            $.growl.error({message: "Gagal Akses API >" + err});
                            $rootScope.unloadLoading();
                            return;
                        });
            };

            $scope.ubah_aktif = function(id, nama, flag) {
                $rootScope.loadLoading("Silahkan Tunggu...");
                $http.post($rootScope.url_api + 'metodeEvaluasi/cekDipakaiDilelang', {
                    id: id,
                    nama: nama
                })
                        .success(function(reply1) {
                            if (reply1.status === 200 && reply1.result.data > 0) {
                                $.growl.error({title: "[WARNING]", message: "Metode Evaluasi ini tidak bisa dihapus karena sudah digunakan dalam Pengadaan"});
                                $rootScope.unloadLoading();
                            } else {
                                $http.post($rootScope.url_api + 'metodeEvaluasi/editActive', {
                                    active: !(flag),
                                    id: id,
                                    username: $rootScope.userLogin
                                }).success(function(reply) {
                                    if (reply.status === 200) {
                                        if (flag === true)
                                            $.growl.notice({title: "[INFO]", message: "Berhasil menonaktifkan metode evaluasi"});
                                        else
                                            $.growl.notice({title: "[INFO]", message: "Berhasil mengaktifkan metode evaluasi"});
                                        $rootScope.unloadLoading();
                                        $scope.jLoad($scope.currentPage);
                                    }
                                    else {
                                        if (flag === true)
                                            $.growl.error({title: "[WARNING]", message: "Gagal menonaktifkan metode evaluasi"});
                                        else
                                            $.growl.error({title: "[WARNING]", message: "Gagal mengaktifkan metode evaluasi"});
                                        $rootScope.unloadLoading();
                                    }
                                });
                            }
                        });
            };

            $scope.addAturPrakualifikasi = function() {
                $state.transitionTo('atur-prakualifikasi');
            };

            $scope.ubahDetail = function(metode_id) {
                //cek apakah metode evaluasi ini sudah digunakan untuk Pengadaan
                $http.post($rootScope.url_api + 'metodeEvaluasi/cekDipakaiDilelang', {
                    id: metode_id
                }).success(function(reply) {
                    if (reply.status === 200 && reply.result.data > 0) {
                        var modalInstance = $modal.open({
                            templateUrl: 'warningUbahMetodeEvaluasi.html',
                            controller: warningUbahMetodeEvaluasiCtrl
                        });
                        modalInstance.result.then(function() {
                            $state.transitionTo('detail-metode-evaluasi', {metode_id: metode_id});
                        });
                    }
                    else {
                        $state.transitionTo('detail-metode-evaluasi', {metode_id: metode_id});
                    }
                });

            };

            $scope.lihatDetail = function(metode_id) {
                var kirim = {
                    metode_evaluasi_id: metode_id
                };
                $modal.open({
                    templateUrl: 'detailMetodeEvaluasi.html',
                    controller: modalDetailMetodeEvaluasiCtrl,
                    resolve: {
                        item: function() {
                            return kirim;
                        }
                    }
                });
            };
            $scope.menujuPR = function() {
                $state.transitionTo('sync-purchase-requisition');
            };
            $scope.menujuPRConfirmed = function() {
                $state.transitionTo('pr-yg-belum-jadi-lelang');
            };
            $scope.menujuTahapanLelang = function() {
                $state.transitionTo('create-tahapanlelang');
            };
            $scope.menujuMetodeLelang = function() {
                $state.transitionTo('edit-metodelelang');
            };
            $scope.menujuKriteriaEvaluasi = function() {
                $state.transitionTo('kriteria-evaluasi-parent');
            };
            $scope.menujuMetodeEvaluasi = function() {
                $state.transitionTo('metode-evaluasi');
            };
            $scope.menujuSusunanPanitia = function() {
                $state.transitionTo('edit-susunanpanitia');
            };
        }) // end listMetodeEvaluasiCtrl


        .controller('tambahAturPrakualCtrl', function($scope, $http, $rootScope, $state, $cookieStore) {

            $scope.totalItems = 0;
            $scope.currentPage = 1;
            $scope.maxSize = 10;
            $scope.kriteria = [];
            $scope.userBisaMengatur = false;
            $scope.administrasiChecked = false;
            $scope.teknisChecked = false;
            $scope.hargaChecked = false;
            $scope.bobotTeknis = "";
            $scope.bobotHarga = "";
            $scope.namaMetode = "";
            $scope.menuhome = 0;
            var page_id = 140;
            $scope.page_id = 140;

//            eb.onopen = function() {
//                $rootScope.autorize();
//                $scope.loadAddMetodeEvaluasi();
//            };

            $scope.init = function() {
//                $rootScope.refreshWaktu();
                $scope.menuhome = $rootScope.menuhome;
//                $rootScope.isLogged = true;
                $rootScope.getSession().then(function(result) {
                    $rootScope.userSession = result.data.data;
                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
                    $rootScope.authorize(loadDataAdd());
                });
            };

            function loadDataAdd() {

                $http.post($rootScope.url_api + "roles/check_authority",
                        {username: $rootScope.userLogin, page_id: $scope.page_id, jenis_mengatur: 1})
                        .success(function(reply) {
                            if (reply.status === 200) {
                                var data = reply.result.data[0];
                                $scope.userBisaMengatur = data.bisa_mengatur;
                            }
                            else {
                                $.growl.error({message: "Gagal mendapatkan data hak akses!"});
                                $rootScope.unloadLoading();
                                return;
                            }
                            $rootScope.unloadLoading();
                        })
                        .error(function(err) {
                            $.growl.error({message: "Gagal Akses API >" + err});
                            $rootScope.unloadLoading();
                            return;
                        });
                $scope.kriteriaList = [
                    {kriteria_id : 1, kriteria_nama:"Kualifikasi"},{kriteria_id : 2, kriteria_nama:"K3L"},
                    {kriteria_id : 3, kriteria_nama:"Kinerja"}
                ];
                
                $scope.tahapanList = [
                    {id:0, nama_step: "Pengumuman "},
                    {id:1, nama_step: "Pendaftaran"},
                    {id:2, nama_step: "Pemasukkan Data"},
                    {id:3, nama_step: "Penentuan Hasil"},
                    {id:4, nama_step: "Penetapan Hasil"},
                    {id:5, nama_step: "Pengumuman Hasil"},
                    {id:6, nama_step: "Certificate"}
                ];

            }


            $scope.tambah = function() {
                if ($scope.namaMetode === "") {
                    $.growl.error({title: "[PERINGATAN]", message: "Nama metode belum dimasukkan"});
                    return;
                }
                if ($scope.administrasiChecked === false && $scope.teknisChecked === false && $scope.hargaChecked === false) {
                    $.growl.error({title: "[PERINGATAN]", message: "Belum ada yang dipilih"});
                    return;
                }
                if ($scope.teknisChecked && $scope.bobotTeknis === "") {
                    $.growl.error({title: "[PERINGATAN]", message: "Persentase teknis belum diisi"});
                    return;
                }
                if ($scope.hargaChecked && $scope.bobotHarga === "") {
                    $.growl.error({title: "[PERINGATAN]", message: "Persentase harga belum diisi"});
                    return;
                }
                var master = [];
                master.push($scope.namaMetode);
                var detail = [];
                var totalPersentase = 0;
                if ($scope.administrasiChecked) {
                    var hai = [];
                    hai.push('Administrasi');
                    hai.push(Number($scope.bobotAdministrasi));
                    totalPersentase = totalPersentase + Number($scope.bobotAdministrasi);
                    detail.push(hai);
                }
                if ($scope.teknisChecked) {
                    var hai = [];
                    hai.push('Teknis');
                    hai.push(Number($scope.bobotTeknis));
                    totalPersentase = totalPersentase + Number($scope.bobotTeknis);
                    detail.push(hai);
                }
                if ($scope.hargaChecked) {
                    var hai = [];
                    hai.push('Harga');
                    hai.push(Number($scope.bobotHarga));
                    totalPersentase = totalPersentase + Number($scope.bobotHarga);
                    detail.push(hai);
                }
                if (totalPersentase !== 100) {
                    $.growl.error({title: "[PERINGATAN]", message: "Total persentase tidak 100%"});
                    return;
                }

                $http.post($rootScope.url_api + 'metodeEvaluasi/insert', {
                    userid: $rootScope.userSession.session_data.pegawai_id,
                    username: $rootScope.userLogin,
                    master: master,
                    detail: detail
                }).success(function(reply) {
                    if (reply.status === 200) {
                        $.growl.notice({title: "[INFO]", message: "Berhasil membuat metode evaluasi"});
                        $state.transitionTo('metode-evaluasi');
                    }
                    else {
                        $.growl.error({title: "[PERINGATAN]", message: "Gagal membuat metode evaluasi"});
                    }
                });
            };

            $scope.back = function() {
                $state.transitionTo('prakual-metode-evaluasi');
            };

        })
        .controller('detailMetodeEvaluasiCtrl', function($scope, $http, $rootScope, $state, $stateParams, $cookieStore, $modal) {
            $scope.kategori = [];
            $scope.nama;
            $scope.userBisaMengatur = false;
            $scope.menuhome = 0;
            var metode_id = Number($stateParams.metode_id);
            var page_id = 140;
            $scope.page_id = 140;

//            eb.onopen = function() {
//                $rootScope.autorize();
//            };

            $scope.init = function() {
                //$rootScope.refreshWaktu();
                $scope.menuhome = $rootScope.menuhome;
                //$rootScope.isLogged = true;
                $rootScope.getSession().then(function(result) {
                    $rootScope.userSession = result.data.data;
                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
                    $rootScope.authorize(loadDataViewDetail());
                });
            };

            function loadDataViewDetail() {
                $http.post($rootScope.url_api + 'metodeEvaluasi/selectKategori', {
                    id: metode_id
                }).success(function(reply) {
                    if (reply.status === 200) {
                        $scope.kategori = reply.result.data;
                        $scope.nama = reply.result.data[0].metode_evaluasi_nm;

                    }
                });
            }

            $scope.keLevel1 = function(med_id) {
                $state.transitionTo('metode-evaluasi-level1', {med_id: med_id});
            };

            $scope.back = function() {
                $state.transitionTo('metode-evaluasi');
            };

            $scope.ubahDetailLevel0 = function() {
                var lempar = {
                    data: $scope.kategori,
                    namaMetode: $scope.nama,
                    metode_evaluasi_id: metode_id,
                    page_id: page_id
                };
                var modalInstance = $modal.open({
                    templateUrl: 'ubahDetailMetodeLevel0.html',
                    controller: ubahDetailMetodeLevel0Ctrl,
                    resolve: {
                        item: function() {
                            return lempar;
                        }
                    }
                });
                modalInstance.result.then(function() {
                    loadDataViewDetail();
                });
            };
        })
        .controller('metodeEvaluasiLevel1Ctrl', function($scope, $http, $rootScope, $stateParams, $state, $cookieStore, $modal) {
            $scope.sudahMengaturLevel1;
            $scope.userBisaMengatur = false;
            $scope.kriteria = [];
            $scope.namaMetode;
            $scope.menuhome = 0;
            var metode_evaluasi_id;
            $scope.metode_evaluasi_id;
            var page_id = 140;
            $scope.page_id = 140;
            var med_id = Number($stateParams.med_id);
            $scope.med_id = Number($stateParams.med_id);
            $scope.obj = {};

//            eb.onopen = function() {
//                $rootScope.autorize();
//                eb.send('itp.authManager.authorise', {
//                    sessionID: $cookieStore.get('sessId')
//                }, function(reply) {
//                    if (reply.status === 'ok') {
//                        $rootScope.userlogged = reply.username;
//                        $scope.init();
//                        $scope.$apply();
//                    }
//                });
//            };

            $scope.init = function() {
                $rootScope.getSession().then(function(result) {
                    $rootScope.userSession = result.data.data;
                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
                    $rootScope.authorize($scope.initialize());
                });
            };

            $scope.initialize = function() {
                //$rootScope.refreshWaktu();
                $scope.menuhome = $rootScope.menuhome;
                //$rootScope.isLogged = true;
                var param = [];
                param.push($rootScope.userlogged);
                param.push(page_id);
                $http.post($rootScope.url_api + "roles/check_authority",
                        {username: $rootScope.userLogin, page_id: $scope.page_id, jenis_mengatur: 1})
                        .success(function(reply) {
                            if (reply.status === 200) {
                                var data = reply.result.data[0];
                                $scope.userBisaMengatur = data.bisa_mengatur;
                                $scope.loadLevel1();
                            }
                            else {
                                $.growl.error({message: "Gagal mendapatkan data hak akses!"});
                                return;
                            }
                        })
                        .error(function(err) {
                            $.growl.error({message: "Gagal Akses API >" + err});
                            return;
                        });

                /*$http.post($rootScope.url_api+'itp.role.cekBisaMengatur', {
                 param: param,
                 page_id: page_id
                 }) .success(function(reply) {
                 if (reply.status === 'ok') {
                 if (reply.result.length > 0) {
                 $scope.userBisaMengatur = reply.result[0].bisa_mengatur;
                 }
                 $scope.loadLevel1();
                 $scope.$apply();
                 }
                 });*/
            };

            $scope.expanding_property = {
                field: "kriteria_nama",
                displayName: "Nama Kriteria Evaluasi",
                sortable: true,
                sortingType: "string",
                filterable: true
            };

            $scope.col_defs = [
                {
                    field: "bobot",
                    displayName: "Bobot (%)",
                    cellTemplate: '<a style="width: 10%"> {{row.branch[col.field]}}&nbsp; % </a> ',
                    sortable: true,
                    sortingType: "number",
                    filterable: true
                },
                {
                    field: "obj",
                    displayName: "  ",
                    cellTemplate: '<a ng-show="row.branch[col.field].bisaNgatur == true && row.branch[col.field].level < 3" class="btn btn-flat btn-xs btn-primary" ng-click="cellTemplateScope.click(row.branch[col.field])"><i class="fa fa-plus-circle"></i>&nbsp; Ubah Sub Kriteria</a>',
                    cellTemplateScope: {
                        click: function(data) {         // this works too: $scope.someMethod;
                            $scope.tambahDetailLevel1(data);
                        }
                    }
                }
//                {
//                    field: "obj",
//                    displayName: "  ",
//                    cellTemplate: '<a ng-show="row.branch[col.field].bisaNgatur" ng-click="cellTemplateScope.click(row.branch[col.field])" tittle="edit" class="btn btn-flat btn-xs btn-default"><i class="fa fa-edit"></i>&nbsp; Ubah</a>',
//                    cellTemplateScope: {
//                        click: function(data) {         // this works too: $scope.someMethod;
//                            $scope.ubahDetailLevel1(data);
//                        }
//                    }
//                }
            ];

            $scope.loadLevel1 = function() {
                $scope.kriteria = [];
                //console.info('masuk loadlevel1');
//                $('#level1').block({
//                    message: '<div style="text-align:center;"><h4>Silakan tunggu..</h4></div>',
//                    css: {border: '3px solid #a00'}
//                });
                $rootScope.loadLoading("Silahkan Tunggu...");
                $http.post($rootScope.url_api + 'metodeEvaluasi/getNamaMetodeBasedOnMed', {
                    med_id: med_id
                }).success(function(reply) {
                    $scope.namaMetode = reply.result.data.nama;
                    metode_evaluasi_id = reply.result.data.id;
                    $scope.metode_evaluasi_id = reply.result.data.id;
                });
                //cek sudah diatur apa belum
                $http.post($rootScope.url_api + 'metodeEvaluasi/sudahMengaturLevel', {
                    med_id: med_id,
                    level: 1,
                    parent: 0
                }).success(function(reply) {
                    if (reply.status === 200) {
                        $scope.sudahMengaturLevel1 = reply.result.data.hasil;
                        if ($scope.sudahMengaturLevel1 == false) {
                            $http.post($rootScope.url_api + 'kriteriaevaluasi/selectformetode', {
                                level: 1,
                                parent: 0
                            }).success(function(reply2) {
                                if (reply2.status === 200) {
                                    $scope.kriteria = reply2.result.data;
                                    for (var i = 0; i < $scope.kriteria.length; i++) {
                                        $scope.kriteria[i].checked = false;
                                        $scope.kriteria[i].bobot = "";
                                    }
                                }
                                $rootScope.unloadLoading();
                            });
                        }
                        else if ($scope.sudahMengaturLevel1 == true) {
                            $http.post($rootScope.url_api + 'medkriteria/select', {
                                med_id: med_id,
                                level: 1,
                                parent: 0
                            }).success(function(reply2) {
                                if (reply2.status === 200) {
                                    if (reply2.result.data.length > 0) {
//                                        $scope.obj = reply.result[0];
                                        reply2.result.data.forEach(function(obj) {
                                            obj.children = [];
                                            obj.bisaNgatur = $scope.userBisaMengatur;
                                            $scope.kriteria.push({
                                                "med_kriteria_id": obj.med_kriteria_id,
                                                "metode_evaluasi_id": obj.metode_evaluasi_id,
                                                "metode_evaluasi_nm": obj.metode_evaluasi_nm,
                                                "med_id": obj.med_id,
                                                "jenis_detail": obj.jenis_detail,
                                                "kriteria_id": obj.kriteria_id,
                                                "kriteria_nama": obj.kriteria_nama,
                                                "is_mandatory": obj.is_mandatory,
                                                "bobot": obj.bobot,
                                                "level": obj.level,
                                                "is_active": obj.is_active,
                                                "parent": obj.parent,
                                                "status_parent": obj.status_parent,
                                                "grandparent_id": obj.grandparent_id,
                                                "status_grandparent": obj.status_grandparent,
                                                "children": obj.children,
                                                "obj": obj,
                                                "bisaNgatur": obj.bisaNgatur
                                            });
                                            $scope.selectSubKriteria(obj);
                                        });
                                    }
                                }
                                $rootScope.unloadLoading();
                            });
                        }
                    }
                });
            };

            $scope.selectSubKriteria = function(object) {
                $http.post($rootScope.url_api + 'medkriteria/select', {
                    med_id: med_id,
                    level: Number(object.level) + 1,
                    parent: object.kriteria_id
                }).success(function(reply) {
                    if (reply.status === 200) {
                        if (reply.result.data.length > 0) {
                            reply.result.data.forEach(function(objChild) {
                                objChild.children = [];
                                objChild.bisaNgatur = $scope.userBisaMengatur;
                                $scope.kriteria.forEach(function(objParent) {
                                    if (objChild.parent == objParent.kriteria_id) {
                                        objChild.children = [];
                                        objChild.bisaNgatur = $scope.userBisaMengatur;
                                        objParent.children.push({
                                            "med_kriteria_id": objChild.med_kriteria_id,
                                            "metode_evaluasi_id": objChild.metode_evaluasi_id,
                                            "metode_evaluasi_nm": objChild.metode_evaluasi_nm,
                                            "med_id": objChild.med_id,
                                            "jenis_detail": objChild.jenis_detail,
                                            "kriteria_id": objChild.kriteria_id,
                                            "kriteria_nama": objChild.kriteria_nama,
                                            "is_mandatory": objChild.is_mandatory,
                                            "bobot": objChild.bobot,
                                            "level": objChild.level,
                                            "is_active": objChild.is_active,
                                            "parent": objChild.parent,
                                            "status_parent": objChild.status_parent,
                                            "grandparent_id": objChild.grandparent_id,
                                            "status_grandparent": objChild.status_grandparent,
                                            "children": objChild.children,
                                            "obj": objChild,
                                            "bisaNgatur": objChild.bisaNgatur
                                        });
                                    }
                                });
                                $http.post($rootScope.url_api + 'medkriteria/select', {
                                    sessionID: $cookieStore.get('sessId'),
                                    med_id: med_id,
                                    level: Number(objChild.level) + 1,
                                    parent: objChild.kriteria_id
                                }).success(function(reply2) {
                                    if (reply2.status === 200) {
                                        if (reply2.result.data.length > 0) {
                                            reply2.result.data.forEach(function(objGrndChild) {
                                                objGrndChild.children = [];
                                                objGrndChild.bisaNgatur = $scope.userBisaMengatur;
                                                for (var i = 0; i < $scope.kriteria.length; i++) {
                                                    if ($scope.kriteria[i].children.length > 0) {
                                                        for (var j = 0; j < $scope.kriteria[i].children.length; j++) {
                                                            if (objGrndChild.parent == $scope.kriteria[i].children[j].kriteria_id) {
                                                                $scope.kriteria[i].children[j].children.push({
                                                                    "med_kriteria_id": objGrndChild.med_kriteria_id,
                                                                    "metode_evaluasi_id": objGrndChild.metode_evaluasi_id,
                                                                    "metode_evaluasi_nm": objGrndChild.metode_evaluasi_nm,
                                                                    "med_id": objGrndChild.med_id,
                                                                    "jenis_detail": objGrndChild.jenis_detail,
                                                                    "kriteria_id": objGrndChild.kriteria_id,
                                                                    "kriteria_nama": objGrndChild.kriteria_nama,
                                                                    "is_mandatory": objGrndChild.is_mandatory,
                                                                    "bobot": objGrndChild.bobot,
                                                                    "level": objGrndChild.level,
                                                                    "is_active": objGrndChild.is_active,
                                                                    "parent": objGrndChild.parent,
                                                                    "status_parent": objGrndChild.status_parent,
                                                                    "grandparent_id": objGrndChild.grandparent_id,
                                                                    "status_grandparent": objGrndChild.status_grandparent,
                                                                    "children": objGrndChild.children,
                                                                    "obj": objGrndChild,
                                                                    "bisaNgatur": objGrndChild.bisaNgatur
                                                                });
                                                            }
                                                        }
                                                    }
                                                }
                                            });

                                        }
                                    }
                                });
                            });
                        }
                    }
                });
            };

            $scope.simpan = function() {
                if ($scope.kriteria.length === 0) {
                    $.growl.error({title: "[PERINGATAN]", message: "Tidak ada kriteria yang bisa dimasukkan"});
                    return;
                }
                for (var i = 0; i < $scope.kriteria.length; i++) {
                    if ($scope.kriteria[i].checked && $scope.kriteria[i].bobot === "") {
                        $.growl.error({title: "[PERINGATAN]", message: "Bobot harus diisi"});
                        return;
                    }
                }
                var totalPersentase = 0;
                for (var i = 0; i < $scope.kriteria.length; i++) {
                    if ($scope.kriteria[i].checked) {
                        totalPersentase = totalPersentase + Number($scope.kriteria[i].bobot);
                    }
                }
                if (totalPersentase !== 100) {
                    $.growl.error({title: "[PERINGATAN]", message: "Total persentase tidak 100%"});
                    return;
                }
                var detail = [];
                var temp;
                for (var i = 0; i < $scope.kriteria.length; i++) {
                    temp = [];
                    if ($scope.kriteria[i].checked) {
                        temp.push(med_id);
                        temp.push($scope.kriteria[i].kriteria_id);
                        temp.push($scope.kriteria[i].level);
                        temp.push($scope.kriteria[i].parent_id);
                        temp.push(Number($scope.kriteria[i].bobot));
                        //temp.push(Number($scope.kriteria[i].kriteria_nama));
                        detail.push(temp);
                    }
                }
                $rootScope.loadLoading("Silahkan Tunggu...");
                $http.post($rootScope.url_api + 'metodeEvaluasi/aturLevel', {
                    username: $rootScope.userLogin,
                    detail: detail
                }).success(function(reply) {
                    if (reply.status === 200) {
                        $scope.kriteria = [];
                        $scope.loadLevel1();
                        $.growl.notice({title: "[INFO]", message: "Berhasil mengatur level 1"});
                        $rootScope.unloadLoading();

                    }
                    else {
                        $.growl.error({title: "[WARNING]", message: "Gagal mengatur level 1"});
                        $rootScope.unloadLoading();
                    }
//                    $scope.loadLevel1();
                });
            };

            $scope.keLevel2 = function(parent) {
                $state.transitionTo('metode-evaluasi-level2', {med_id: med_id, parent: parent});
            };

            $scope.back = function() {
                $state.transitionTo('detail-metode-evaluasi', {metode_id: metode_evaluasi_id});
            };

            $scope.ubahDetailLevel1 = function() {
//                var data = [];
//                if (obj.level === 1) {
//                    data = $scope.kriteria;
//                } else if (obj.level === 2) {
//                    for (var i = 0; i < $scope.kriteria.length; i++) {
//                        if ($scope.kriteria[i].kriteria_id === obj.parent) {
//                            data = $scope.kriteria[i].children;
//                        }
//                    }
//                } else {
//                    for (var i = 0; i < $scope.kriteria.length; i++) {
//                        for (var j = 0; j < $scope.kriteria[i].children.length; j++) {
//                            if ($scope.kriteria[i].children[j].kriteria_id === obj.parent) {
//                                data = $scope.kriteria[i].children[j].children;
//                            }
//                        }
//                    }
//                }
                var lempar = {
                    data: $scope.kriteria,
                    med_id: med_id,
                    page_id: page_id,
                    level: 1,
                    parent: 0,
                    nama: ''
                };
                var modalInstance = $modal.open({
                    templateUrl: 'ubahDetailMetodeLevel1.html',
                    controller: ubahDetailMetodeLevel1Ctrl,
                    resolve: {
                        item: function() {
                            return lempar;
                        }
                    }
                });
                modalInstance.result.then(function() {
                    $scope.loadLevel1();
                });
            };

            $scope.tambahDetailLevel1 = function(obj) {
                var data = [];
                if (obj.level == 0) {
                    data = $scope.kriteria;
                } else if (obj.level == 1) {
                    for (var i = 0; i < $scope.kriteria.length; i++) {
                        if ($scope.kriteria[i].kriteria_id == obj.kriteria_id) {
                            data = $scope.kriteria[i].children;
                        }
                    }
                } else {
                    for (var i = 0; i < $scope.kriteria.length; i++) {
                        for (var j = 0; j < $scope.kriteria[i].children.length; j++) {
                            if ($scope.kriteria[i].children[j].kriteria_id == obj.kriteria_id) {
                                data = $scope.kriteria[i].children[j].children;
                            }
                        }
                    }
                }
                var lempar = {
                    data: data,
                    med_id: med_id,
                    page_id: page_id,
                    level: Number(obj.level) + 1,
                    parent: obj.kriteria_id,
                    nama: obj.kriteria_nama
                };
                var modalInstance = $modal.open({
                    templateUrl: 'ubahDetailMetodeLevel1.html',
                    controller: ubahDetailMetodeLevel1Ctrl,
                    resolve: {
                        item: function() {
                            return lempar;
                        }
                    }
                });
                modalInstance.result.then(function() {
                    $scope.loadLevel1();
                });
            };

        })
        .controller('metodeEvaluasiLevel2Ctrl', function($scope, $http, $rootScope, $state, $stateParams, $cookieStore, $modal) {
            $scope.userBisaMengatur = false;
            $scope.sudahMengaturLevel2;
            $scope.namaParent;
            $scope.menuhome = 0;
            var page_id = 140;
            $scope.page_id = 140;
            var med_id = Number($stateParams.med_id);
            var parent = Number($stateParams.parent);
            $scope.med_id = Number($stateParams.med_id);


//            eb.onopen = function() {
//                $rootScope.autorize();
//                $scope.init();
//            };

            $scope.init = function() {
                $rootScope.getSession().then(function(result) {
                    $rootScope.userSession = result.data.data;
                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
                    $rootScope.authorize($scope.initialize());
                });
            };

            $scope.initialize = function() {
                $rootScope.refreshWaktu();
                $scope.menuhome = $rootScope.menuhome;
                $rootScope.isLogged = true;
                var param = [];
                param.push($rootScope.userlogged);
                param.push(page_id);

                $http.post($rootScope.url_api + "roles/check_authority",
                        {username: $rootScope.userLogin, page_id: $scope.page_id, jenis_mengatur: 1})
                        .success(function(reply) {
                            if (reply.status === 200) {
                                var data = reply.result.data[0];
                                $scope.userBisaMengatur = data.bisa_mengatur;
                            }
                            else {
                                $.growl.error({message: "Gagal mendapatkan data hak akses!"});
                                return;
                            }
                        })
                        .error(function(err) {
                            $.growl.error({message: "Gagal Akses API >" + err});
                            return;
                        });
                $scope.loadLevel2();

                /* eb.send('itp.role.cekBisaMengatur', {
                 sessionID: $cookieStore.get('sessId'),
                 param: param,
                 page_id: page_id
                 }, function(reply) {
                 if (reply.status === 'ok') {
                 if (reply.result.length > 0) {
                 $scope.userBisaMengatur = reply.result[0].bisa_mengatur;
                 }
                 $scope.$apply();
                 }
                 });
                 $scope.loadLevel2();*/
            };

            $scope.loadLevel2 = function() {
                $rootScope.loadLoading("Silahkan Tunggu...");
                $http.post($rootScope.url_api + 'metodeEvaluasi/getNamaMetodeBasedOnMed', {
                    med_id: med_id
                }).success(function(reply) {
                    $scope.namaMetode = reply.nama;
                });
                $http.post($rootScope.url_api + 'kriteriaEvaluasi/getNamaParent', {
                    level: 2,
                    parent_id: parent
                }).success(function(reply) {
                    if (reply.status === 200) {
                        $scope.namaParent = reply.nama_parent;
                    }
                });
                //cek sudah diatur apa belum
                $http.post($rootScope.url_api + 'metodeEvaluasi/sudahMengaturLevel', {
                    med_id: med_id,
                    level: 2,
                    parent: parent
                }).success(function(reply) {
                    if (reply.status === 200) {
                        $scope.sudahMengaturLevel2 = reply.result.data.hasil;
                        if ($scope.sudahMengaturLevel2 == false) {
                            $http.post($rootScope.url_api + 'kriteriaevaluasi/selectformetode', {
                                level: 2,
                                parent: parent
                            }).success(function(reply2) {
                                if (reply2.status === 200) {
                                    $scope.kriteria = reply2.result.data;
                                    for (var i = 0; i < $scope.kriteria.length; i++) {
                                        $scope.kriteria[i].checked = false;
                                        $scope.kriteria[i].bobot = "";
                                    }
                                }
                                $rootScope.unloadLoading();
                            });
                        }
                        else if ($scope.sudahMengaturLevel2 == true) {
                            $http.post($rootScope.url_api + 'medkriteria/select', {
                                med_id: med_id,
                                level: 2,
                                parent: parent
                            }, function(reply2) {
                                if (reply2.status === 200) {
                                    $scope.kriteria = reply2.result.data;
                                }
                                $rootScope.unloadLoading();
                            });
                        }
                    }
                });
            };

            $scope.simpan = function() {
                if ($scope.kriteria.length === 0) {
                    $.growl.error({title: "[PERINGATAN]", message: "Tidak ada kriteria yang bisa dimasukkan"});
                    return;
                }
                for (var i = 0; i < $scope.kriteria.length; i++) {
                    if ($scope.kriteria[i].checked && $scope.kriteria[i].bobot === "") {
                        $.growl.error({title: "[PERINGATAN]", message: "Bobot harus diisi"});
                        return;
                    }
                }
                var totalPersentase = 0;
                for (var i = 0; i < $scope.kriteria.length; i++) {
                    if ($scope.kriteria[i].checked) {
                        totalPersentase = totalPersentase + Number($scope.kriteria[i].bobot);
                    }
                }
                if (totalPersentase !== 100) {
                    $.growl.error({title: "[PERINGATAN]", message: "Total persentase tidak 100%"});
                    return;
                }
                var detail = [];
                var temp;
                for (var i = 0; i < $scope.kriteria.length; i++) {
                    temp = [];
                    if ($scope.kriteria[i].checked) {
                        temp.push(med_id);
                        temp.push($scope.kriteria[i].kriteria_id);
                        temp.push($scope.kriteria[i].level);
                        temp.push($scope.kriteria[i].parent_id);
                        temp.push(Number($scope.kriteria[i].bobot));
                        //temp.push(Number($scope.kriteria[i].kriteria_nama));
                        detail.push(temp);
                    }
                }
                $rootScope.loadLoading("Silahkan Tunggu...");
                $http.post($rootScope.url_api + 'metodeEvaluasi/aturLevel', {
                    detail: detail,
                    page_id: page_id
                }).success(function(reply) {
                    if (reply.status === 200) {
                        $.growl.notice({title: "[INFO]", message: "Berhasil mengatur level 2"});
                        $rootScope.unloadLoading();
                        $scope.loadLevel2();
                    }
                    else {
                        $.growl.error({title: "[WARNING]", message: "Gagal mengatur level 2"});
                        $rootScope.unloadLoading();
                    }
                });
            };

            $scope.keLevel3 = function(parentIni) {
                $state.transitionTo('metode-evaluasi-level3', {med_id: med_id, parent: parentIni, parentSebelumnyaLagi: parent});
            };

            $scope.back = function() {
                $state.transitionTo('metode-evaluasi-level1', {med_id: med_id});
            };

            $scope.ubahDetailLevel2 = function() {
                var lempar = {
                    data: $scope.kriteria,
                    med_id: med_id,
                    page_id: page_id,
                    parent: parent
                };
                var modalInstance = $modal.open({
                    templateUrl: 'ubahDetailMetodeLevel2.html',
                    controller: ubahDetailMetodeLevel2Ctrl,
                    resolve: {
                        item: function() {
                            return lempar;
                        }
                    }
                });
                modalInstance.result.then(function() {
                    $scope.init();
                });
            };
        })
        .controller('metodeEvaluasiLevel3Ctrl', function($scope, $http, $rootScope, $state, $stateParams, $cookieStore, $modal) {
            $scope.userBisaMengatur = false;
            $scope.sudahMengaturLevel3;
            $scope.namaParent;
            $scope.menuhome = 0;
            var page_id = 140;
            $scope.page_id = 140;
            var med_id = Number($stateParams.med_id);
            var parent = Number($stateParams.parent);
            $scope.med_id = Number($stateParams.med_id);
            $scope.parent = Number($stateParams.parent);
            var parentSebelumnya = Number($stateParams.parentSebelumnyaLagi);

//            eb.onopen = function() {
//                $rootScope.autorize();
//                $scope.init();
//            };
            $scope.init = function() {
                $rootScope.getSession().then(function(result) {
                    $rootScope.userSession = result.data.data;
                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
                    $rootScope.authorize($scope.initialize());
                });
            };

            $scope.initialize = function() {
                $rootScope.refreshWaktu();
                $scope.menuhome = $rootScope.menuhome;
                $rootScope.isLogged = true;
                var param = [];
                param.push($rootScope.userlogged);
                param.push(page_id);

                $http.post($rootScope.url_api + "roles/check_authority",
                        {username: $rootScope.userLogin, page_id: $scope.page_id, jenis_mengatur: 1})
                        .success(function(reply) {
                            if (reply.status === 200) {
                                var data = reply.result.data[0];
                                $scope.userBisaMengatur = data.bisa_mengatur;
                            }
                            else {
                                $.growl.error({message: "Gagal mendapatkan data hak akses!"});
                                return;
                            }
                        })
                        .error(function(err) {
                            $.growl.error({message: "Gagal Akses API >" + err});
                            return;
                        });
                $scope.loadLevel3();


                /* $http.post($rootScope.url_api+'role/cekBisaMengatur', {
                 sessionID: $cookieStore.get('sessId'),
                 param: param,
                 page_id: page_id
                 }, function(reply) {
                 if (reply.status === 'ok') {
                 if (reply.result.length > 0) {
                 $scope.userBisaMengatur = reply.result[0].bisa_mengatur;
                 }
                 $scope.$apply();
                 }
                 });
                 $scope.loadLevel3();*/
            };

            $scope.loadLevel3 = function() {
                $rootScope.loadLoading("Silahkan Tunggu...");
                $http.post($rootScope.url_api + 'metodeEvaluasi/getNamaMetodeBasedOnMed', {
                    med_id: med_id
                }).success(function(reply) {
                    $scope.namaMetode = reply.nama;
                });
                $http.post($rootScope.url_api + 'kriteriaEvaluasi/getNamaParent', {
                    level: 3,
                    parent_id: parent
                }).success(function(reply) {
                    if (reply.status === 200) {
                        $scope.namaParent = reply.nama_parent;
                    }
                });
                //cek sudah diatur apa belum
                $http.post($rootScope.url_api + 'metodeEvaluasi/sudahMengaturLevel', {
                    med_id: med_id,
                    level: 3,
                    parent: parent
                }).success(function(reply) {
                    if (reply.status === 200) {
                        $scope.sudahMengaturLevel3 = reply.result.data.hasil;
                        if ($scope.sudahMengaturLevel3 == false) {
                            $http.post($rootScope.url_api + 'kriteriaevaluasi/selectformetode', {
                                level: 3,
                                parent: parent
                            }).success(function(reply2) {
                                if (reply2.status === 200) {
                                    $scope.kriteria = reply2.result.data;
                                    for (var i = 0; i < $scope.kriteria.length; i++) {
                                        $scope.kriteria[i].checked = false;
                                        $scope.kriteria[i].bobot = "";
                                    }
                                }
                                $rootScope.unloadLoading();
                            });
                        }
                        else if ($scope.sudahMengaturLevel3 == true) {
                            $http.post($rootScope.url_api + 'medkriteria/select', {
                                med_id: med_id,
                                level: 3,
                                parent: parent
                            }).success(function(reply2) {
                                if (reply2.status === 200) {
                                    $scope.kriteria = reply2.result.data;
                                }
                                $rootScope.unloadLoading();
                            });
                        }
                    }
                });
            };

            $scope.simpan = function() {
                if ($scope.kriteria.length === 0) {
                    $.growl.error({title: "[PERINGATAN]", message: "Tidak ada kriteria yang bisa dimasukkan"});
                    return;
                }
                for (var i = 0; i < $scope.kriteria.length; i++) {
                    if ($scope.kriteria[i].checked && $scope.kriteria[i].bobot === "") {
                        $.growl.error({title: "[PERINGATAN]", message: "Bobot harus diisi"});
                        return;
                    }
                }
                var totalPersentase = 0;
                for (var i = 0; i < $scope.kriteria.length; i++) {
                    if ($scope.kriteria[i].checked) {
                        totalPersentase = totalPersentase + Number($scope.kriteria[i].bobot);
                    }
                }
                if (totalPersentase !== 100) {
                    $.growl.error({title: "[PERINGATAN]", message: "Total persentase tidak 100%"});
                    return;
                }
                var detail = [];
                var temp;
                for (var i = 0; i < $scope.kriteria.length; i++) {
                    temp = [];
                    if ($scope.kriteria[i].checked) {
                        temp.push(med_id);
                        temp.push($scope.kriteria[i].kriteria_id);
                        temp.push($scope.kriteria[i].level);
                        temp.push($scope.kriteria[i].parent_id);
                        temp.push(Number($scope.kriteria[i].bobot));
                        //temp.push(Number($scope.kriteria[i].kriteria_nama));
                        detail.push(temp);
                    }
                }
                $rootScope.loadLoading("Silahkan Tunggu...");
                $http.post($rootScope.url_api + 'metodeEvaluasi/aturLevel', {
                    detail: detail
                }).success(function(reply) {
                    if (reply.status === 200) {
                        $.growl.notice({title: "[INFO]", message: "Berhasil mengatur level 3"});
                        $rootScope.unloadLoading();
                        $scope.loadLevel3();
                    }
                    else {
                        $.growl.error({title: "[WARNING]", message: "Gagal mengatur level 3"});
                        $rootScope.unloadLoading();
                    }
                });
            };

            $scope.back = function() {
                //console.info("parentSebelumnya = " + parentSebelumnya);
                $state.transitionTo('metode-evaluasi-level2', {med_id: med_id, parent: parentSebelumnya});
            };

            $scope.ubahDetailLevel3 = function() {
                var lempar = {
                    data: $scope.kriteria,
                    med_id: med_id,
                    page_id: page_id,
                    parent: parent
                };
                var modalInstance = $modal.open({
                    templateUrl: 'ubahDetailMetodeLevel3.html',
                    controller: ubahDetailMetodeLevel3Ctrl,
                    resolve: {
                        item: function() {
                            return lempar;
                        }
                    }
                });
                modalInstance.result.then(function() {
                    $scope.init();
                });
            };
        });

var warningUbahMetodeEvaluasiCtrl = function($scope, $modalInstance) {
    $scope.tetapUbah = function() {
        $modalInstance.close();
    };

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };
};

var ubahDetailMetodeLevel0Ctrl = function($scope, item, $modalInstance, $http, $cookieStore, $rootScope) {
    $scope.data = item.data;
    $scope.namaMetode = item.namaMetode;
    var metode_evaluasi_id = item.metode_evaluasi_id;
    var page_id = item.page_id;
    var master = [];
    var detail = [];
    var detailBaru = [];
    var detailLama = [];
    var totalPersentase = 0;
    var idAdministrasi;
    var idTeknis;
    var idHarga;
    $scope.administrasiChecked = false;
    $scope.bobotAdministrasi = 0;
    $scope.teknisChecked = false;
    $scope.bobotTeknis = 0;
    $scope.hargaChecked = false;
    $scope.bobotHarga = 0;

    $scope.init = function() {
        $rootScope.getSession().then(function(result) {
            $rootScope.userSession = result.data.data;
            $rootScope.userLogin = $rootScope.userSession.session_data.username;
            $rootScope.authorize($scope.initialize());
        });
    };

    $scope.initialize = function() {
        for (var i = 0; i < $scope.data.length; i++) {
            if ($scope.data[i].jenis_detail === 'Administrasi') {
                $scope.administrasiChecked = true;
                $scope.bobotAdministrasi = $scope.data[i].bobot;
                idAdministrasi = $scope.data[i].med_id;
            }
            else if ($scope.data[i].jenis_detail === 'Teknis') {
                $scope.teknisChecked = true;
                $scope.bobotTeknis = $scope.data[i].bobot;
                idTeknis = $scope.data[i].med_id;
            }
            else if ($scope.data[i].jenis_detail === 'Harga') {
                $scope.hargaChecked = true;
                $scope.bobotHarga = $scope.data[i].bobot;
                idHarga = $scope.data[i].med_id;
            }
        }
    };

    $scope.ubahAdministrasi = function(obj) {
        $scope.administrasiChecked = obj;
    };

    $scope.ubahTeknis = function(obj) {
        $scope.teknisChecked = obj;
    };

    $scope.ubahHarga = function(obj) {
        $scope.hargaChecked = obj;
    };

    $scope.ubahBobotAdministrasi = function(obj) {
        $scope.bobotAdministrasi = obj;
    };

    $scope.ubahBobotTeknis = function(obj) {
        $scope.bobotTeknis = obj;
    };

    $scope.ubahBobotHarga = function(obj) {
        $scope.bobotHarga = obj;
    };

    $scope.ubahNama = function(obj) {
        $scope.namaMetode = obj;
    };

    $scope.simpan = function() {
        totalPersentase = 0;
        if ($scope.namaMetode === "") {
            $.growl.error({title: "[PERINGATAN]", message: "Nama metode belum dimasukkan"});
            return;
        }
        if ($scope.administrasiChecked === false && $scope.teknisChecked === false && $scope.hargaChecked === false) {
            $.growl.error({title: "[PERINGATAN]", message: "Belum ada yang dipilih"});
            return;
        }
        if ($scope.teknisChecked && $scope.bobotTeknis === "") {
            $.growl.error({title: "[PERINGATAN]", message: "Persentase teknis belum diisi"});
            return;
        }
        if ($scope.hargaChecked && $scope.bobotHarga === "") {
            $.growl.error({title: "[PERINGATAN]", message: "Persentase harga belum diisi"});
            return;
        }
        if ($scope.administrasiChecked) {
            totalPersentase = totalPersentase + Number($scope.bobotAdministrasi);
        }
        if ($scope.teknisChecked) {
            totalPersentase = totalPersentase + Number($scope.bobotTeknis);
        }
        if ($scope.hargaChecked) {
            totalPersentase = totalPersentase + Number($scope.bobotHarga);
        }
        if (totalPersentase !== 100) {
            $.growl.error({title: "[PERINGATAN]", message: "Total persentase tidak 100%"});
            return;
        }
        master = [];
        master.push($scope.namaMetode);
        master.push(metode_evaluasi_id);
        
        detailLama = [];
        detailBaru = [];
        
        var hai = [];
        hai.push('Administrasi');
        if ($scope.administrasiChecked) {
            hai.push(Number($scope.bobotAdministrasi));
            hai.push(1);
        }
        else {
            hai.push(0);
            hai.push(0);
        }
        //cek lama atau baru
        var baru = true;
        for (var i = 0; i < $scope.data.length; i++) {
            if ($scope.data[i].jenis_detail === 'Administrasi') {
                baru = false;
                break;
            }
        }
        if (baru) {
            hai.push(metode_evaluasi_id);
            detailBaru.push(hai);
        }
        else {
            hai.push(idAdministrasi);
            detailLama.push(hai);
        }

        var hai = [];
        hai.push('Teknis');
        if ($scope.teknisChecked) {
            hai.push(Number($scope.bobotTeknis));
            hai.push(1);
        }
        else {
            hai.push(0);
            hai.push(0);
        }
        //cek lama atau baru
        baru = true;
        for (var i = 0; i < $scope.data.length; i++) {
            if ($scope.data[i].jenis_detail === 'Teknis') {
                baru = false;
                break;
            }
        }
        if (baru) {
            hai.push(metode_evaluasi_id);
            detailBaru.push(hai);
        }
        else {
            hai.push(idTeknis);
            detailLama.push(hai);
        }
        
        var hai = [];
        hai.push('Harga');
        if ($scope.hargaChecked) {
            hai.push(Number($scope.bobotHarga));
            hai.push(1);
        }
        else {
            hai.push(0);
            hai.push(0);
        }
        //cek lama atau baru
        baru = true;
        for (var i = 0; i < $scope.data.length; i++) {
            if ($scope.data[i].jenis_detail === 'Harga') {
                baru = false;
                break;
            }
        }
        if (baru) {
            hai.push(metode_evaluasi_id);
            detailBaru.push(hai);
        }
        else {
            hai.push(idHarga);
            detailLama.push(hai);
        }
        
        $http.post($rootScope.url_api + 'metodeEvaluasi/edit', {
            username: $rootScope.userLogin,
            userid: $rootScope.userSession.session_data.pegawai_id,
            master: master,
            detailLama: detailLama,
            detailBaru: detailBaru
        }).success(function(reply) {
            if (reply.status === 200) {
                $.growl.notice({title: "[INFO]", message: "Berhasil mengubah metode evaluasi"});
                $modalInstance.close();
            }
            else {
                $.growl.error({title: "[PERINGATAN]", message: "Gagal mengubah metode evaluasi"});
            }
        });
    };

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };
};

var ubahDetailMetodeLevel1Ctrl = function($scope, $modalInstance, $cookieStore, item, $http, $rootScope) {
    var data = item.data;
    var med_id = item.med_id;
    var page_id = item.page_id;
    var detailLama = [];
    var detailBaru = [];
    $scope.hasChild = true;

    $scope.init = function() {
        $rootScope.getSession().then(function(result) {
            $rootScope.userSession = result.data.data;
            $rootScope.userLogin = $rootScope.userSession.session_data.username;
            $rootScope.authorize($scope.initialize());
        });
    };

    $scope.initialize = function() {
        $http.post($rootScope.url_api + 'kriteriaevaluasi/selectformetode', {
            username: $rootScope.userLogin,
            level: item.level,
            parent: item.parent
        }).success(function(reply2) {
            if (reply2.status === 200) {
                if (reply2.result.data.length > 0) {
                    $scope.kriteria = reply2.result.data;
                    for (var i = 0; i < $scope.kriteria.length; i++) {
                        $scope.kriteria[i].checked = false;
                        $scope.kriteria[i].bobot = "";
                        for (var j = 0; j < data.length; j++) {
                            if ($scope.kriteria[i].kriteria_id == data[j].kriteria_id) {
                                $scope.kriteria[i].checked = true;
                                $scope.kriteria[i].bobot = data[j].bobot;
                                break;
                            }
                        }
                    }
                } else {
                    $scope.hasChild = false;
                    if (item.nama != '') {
                        $('#tableUbahDtl').block({
                            message: '<div style="text-align:center;"><h4>Tidak ditemukan sub kriteria untuk ' + item.nama + '</h4></div>',
                            css: {border: '3px solid #a00'}
                        });
                    } else {
                        $('#tableUbahDtl').block({
                            message: '<div style="text-align:center;"><h4>Tidak ditemukan sub kriteria </h4></div>',
                            css: {border: '3px solid #a00'}
                        });
                    }
                }
            }
        });
    };

    $scope.simpan = function() {
        detailBaru = [];
        detailLama = [];
        for (var i = 0; i < $scope.kriteria.length; i++) {
            if ($scope.kriteria[i].checked && $scope.kriteria[i].bobot == "") {
                $.growl.error({title: "[PERINGATAN]", message: "Bobot harus diisi"});
                return;
            }
        }
        var totalPersentase = 0;
        for (var i = 0; i < $scope.kriteria.length; i++) {
            if ($scope.kriteria[i].checked) {
                totalPersentase = totalPersentase + Number($scope.kriteria[i].bobot);
            }
        }
        if (totalPersentase !== 100) {
            $.growl.error({title: "[PERINGATAN]", message: "Total persentase tidak 100%"});
            return;
        }

        var temp;
        var baru;
        for (var i = 0; i < $scope.kriteria.length; i++) {
            //console.info("ho = " + JSON.stringify($scope.kriteria[i]));
            temp = [];
            baru = true;
            for (var j = 0; j < data.length; j++) {
                if ($scope.kriteria[i].kriteria_id == data[j].kriteria_id) {
                    baru = false;
                    if ($scope.kriteria[i].checked == true) {
                        temp.push(Number($scope.kriteria[i].bobot));
                        temp.push(1);
                    }
                    else {
                        temp.push(0);
                        temp.push(0);
                    }
                    temp.push(data[j].med_kriteria_id);
                    detailLama.push(temp);
                    break;
                }
            }
            if (baru) {
                if ($scope.kriteria[i].checked) {
                    temp.push(med_id);
                    temp.push($scope.kriteria[i].kriteria_id);
                    temp.push($scope.kriteria[i].level);
                    temp.push($scope.kriteria[i].parent_id);
                    if ($scope.kriteria[i].checked == true) {
                        temp.push(Number($scope.kriteria[i].bobot));
                    }
                    else {
                        temp.push(0);
                    }
                    detailBaru.push(temp);
                }
            }

        }
        $rootScope.loadLoadingModal("Silahkan Tunggu...");
        if (detailLama.length === 0 && detailBaru.length > 0) {
            $http.post($rootScope.url_api + 'metodeEvaluasi/aturLevel', {
                username: $rootScope.userLogin,
                detail: detailBaru
            }).success(function(reply) {
                if (reply.status === 200) {
                    $.growl.notice({title: "[INFO]", message: "Berhasil mengatur kriteria"});
                    $rootScope.unloadLoadingModal();
                    $modalInstance.close();
                }
                else {
                    $.growl.error({title: "[WARNING]", message: "Gagal mengatur kriteria"});
                    $rootScope.unloadLoadingModal();
                }
            });
        } else {
            $http.post($rootScope.url_api + 'metodeEvaluasi/ubahLevel', {
                username: $rootScope.userLogin,
                detailLama: detailLama,
                detailBaru: detailBaru
            }).success(function(reply) {
                if (reply.status === 200) {
                    $.growl.notice({title: "[INFO]", message: "Berhasil mengubah kriteria"});
                    $rootScope.unloadLoadingModal();
                    $modalInstance.close();
                }
                else {
                    $.growl.error({title: "[WARNING]", message: "Gagal mengubah kriteria"});
                    $rootScope.unloadLoadingModal();
                }
            });
        }
    };

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };
};

var ubahDetailMetodeLevel2Ctrl = function($scope, $modalInstance, item, $cookieStore, $http, $rootScope) {
    var data = item.data;
    var med_id = item.med_id;
    var page_id = item.page_id;
    var parent = item.parent;
    var detailLama = [];
    var detailBaru = [];

    $scope.init = function() {
        $rootScope.getSession().then(function(result) {
            $rootScope.userSession = result.data.data;
            $rootScope.userLogin = $rootScope.userSession.session_data.username;
            $rootScope.authorize($scope.initialize());
        });
    };

    $scope.initialize = function() {
        $http.post($rootScope.url_api + 'kriteriaevaluasi/selectForMetode', {
            username: $rootScope.userLogin,
            level: 2,
            parent: parent
        }).success(function(reply2) {
            if (reply2.status === 200) {
                $scope.kriteria = reply2.result.data;
                for (var i = 0; i < $scope.kriteria.length; i++) {
                    $scope.kriteria[i].checked = false;
                    $scope.kriteria[i].bobot = "";
                    for (var j = 0; j < data.length; j++) {
                        if ($scope.kriteria[i].kriteria_id == data[j].kriteria_id) {
                            $scope.kriteria[i].checked = true;
                            $scope.kriteria[i].bobot = data[j].bobot;
                            break;
                        }
                    }
                }
            }
        });
    };

    $scope.simpan = function() {
        for (var i = 0; i < $scope.kriteria.length; i++) {
            if ($scope.kriteria[i].checked && $scope.kriteria[i].bobot === "") {
                $.growl.error({title: "[PERINGATAN]", message: "Bobot harus diisi"});
                return;
            }
        }
        var totalPersentase = 0;
        for (var i = 0; i < $scope.kriteria.length; i++) {
            if ($scope.kriteria[i].checked) {
                totalPersentase = totalPersentase + Number($scope.kriteria[i].bobot);
            }
        }
        if (totalPersentase !== 100) {
            $.growl.error({title: "[PERINGATAN]", message: "Total persentase tidak 100%"});
            return;
        }

        var temp;
        var baru;
        for (var i = 0; i < $scope.kriteria.length; i++) {
            ////console.info("ho = " + JSON.stringify($scope.kriteria[i]));
            temp = [];
            baru = true;
            for (var j = 0; j < data.length; j++) {
                if ($scope.kriteria[i].kriteria_id == data[j].kriteria_id) {
                    baru = false;
                    if ($scope.kriteria[i].checked == true) {
                        temp.push(Number($scope.kriteria[i].bobot));
                        temp.push(1);
                    }
                    else {
                        temp.push(0);
                        temp.push(0);
                    }
                    temp.push(data[j].med_kriteria_id);
                    detailLama.push(temp);
                    break;
                }
            }
            if (baru) {
                if ($scope.kriteria[i].checked) {
                    temp.push(med_id);
                    temp.push($scope.kriteria[i].kriteria_id);
                    temp.push($scope.kriteria[i].level);
                    temp.push($scope.kriteria[i].parent_id);
                    if ($scope.kriteria[i].checked == true) {
                        temp.push(Number($scope.kriteria[i].bobot));
                    }
                    else {
                        temp.push(0);
                    }
                    detailBaru.push(temp);
                }
            }

        }
        $rootScope.loadLoadingModal("Silahkan Tunggu...");
        $http.post($rootScope.url_api + 'metodeEvaluasi/ubahLevel', {
            username: $rootScope.userLogin,
            detailLama: detailLama,
            detailBaru: detailBaru
        }).success(function(reply) {
            if (reply.status === 200) {
                $.growl.notice({title: "[INFO]", message: "Berhasil mengubah level 2"});
                $rootScope.unloadLoadingModal();
                $modalInstance.close();
            }
            else {
                $.growl.error({title: "[WARNING]", message: "Gagal mengubah level 2"});
                $rootScope.unloadLoadingModal();
            }
        });
    };

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };
};

var ubahDetailMetodeLevel3Ctrl = function($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
    var data = item.data;
    var med_id = item.med_id;
    var page_id = item.page_id;
    var parent = item.parent;
    var detailLama = [];
    var detailBaru = [];

    $scope.init = function() {
        $rootScope.getSession().then(function(result) {
            $rootScope.userSession = result.data.data;
            $rootScope.userLogin = $rootScope.userSession.session_data.username;
            $rootScope.authorize($scope.initialize());
        });
    };

    $scope.initialize = function() {
        $http.post($rootScope.url_api + 'kriteriaevaluasi/selectformetode', {
            username: $rootScope.userLogin,
            level: 3,
            parent: parent
        }).success(function(reply2) {
            if (reply2.status === 200) {
                $scope.kriteria = reply2.result.data;
                for (var i = 0; i < $scope.kriteria.length; i++) {
                    $scope.kriteria[i].checked = false;
                    $scope.kriteria[i].bobot = "";
                    for (var j = 0; j < data.length; j++) {
                        if ($scope.kriteria[i].kriteria_id == data[j].kriteria_id) {
                            $scope.kriteria[i].checked = true;
                            $scope.kriteria[i].bobot = data[j].bobot;
                            break;
                        }
                    }
                }
            }
        });
    };

    $scope.simpan = function() {
        for (var i = 0; i < $scope.kriteria.length; i++) {
            if ($scope.kriteria[i].checked && $scope.kriteria[i].bobot == "") {
                $.growl.error({title: "[PERINGATAN]", message: "Bobot harus diisi"});
                return;
            }
        }
        var totalPersentase = 0;
        for (var i = 0; i < $scope.kriteria.length; i++) {
            if ($scope.kriteria[i].checked) {
                totalPersentase = totalPersentase + Number($scope.kriteria[i].bobot);
            }
        }
        if (totalPersentase !== 100) {
            $.growl.error({title: "[PERINGATAN]", message: "Total persentase tidak 100%"});
            return;
        }

        var temp;
        var baru;
        for (var i = 0; i < $scope.kriteria.length; i++) {
            ////console.info("ho = " + JSON.stringify($scope.kriteria[i]));
            temp = [];
            baru = true;
            for (var j = 0; j < data.length; j++) {
                if ($scope.kriteria[i].kriteria_id == data[j].kriteria_id) {
                    baru = false;
                    if ($scope.kriteria[i].checked == true) {
                        temp.push(Number($scope.kriteria[i].bobot));
                        temp.push(1);
                    }
                    else {
                        temp.push(0);
                        temp.push(0);
                    }
                    temp.push(data[j].med_kriteria_id);
                    detailLama.push(temp);
                    break;
                }
            }
            if (baru) {
                if ($scope.kriteria[i].checked) {
                    temp.push(med_id);
                    temp.push($scope.kriteria[i].kriteria_id);
                    temp.push($scope.kriteria[i].level);
                    temp.push($scope.kriteria[i].parent_id);
                    if ($scope.kriteria[i].checked == true) {
                        temp.push(Number($scope.kriteria[i].bobot));
                    }
                    else {
                        temp.push(0);
                    }
                    detailBaru.push(temp);
                }
            }
        }
        $rootScope.loadLoadingModal("Silahkan Tunggu...");
        $http.post($rootScope.url_api + 'metodeEvaluasi/ubahLevel', {
            username: $rootScope.userLogin,
            detailLama: detailLama,
            detailBaru: detailBaru
        }).success(function(reply) {
            if (reply.status === 200) {
                $.growl.notice({title: "[INFO]", message: "Berhasil mengubah level 3"});
                $rootScope.unloadLoadingModal();
                $modalInstance.close();
            }
            else {
                $.growl.error({title: "[WARNING]", message: "Gagal mengubah level 3"});
                $rootScope.unloadLoadingModal();
            }
        });
    };

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };
};

var modalDetailMetodeEvaluasiCtrl = function($scope, $modalInstance, item, $cookieStore, $http, $rootScope) {
    var metode_evaluasi_id = item.metode_evaluasi_id;
    var Administrasi = [];
    var AdministrasiLevel1 = [];
    var AdministrasiLevel2 = [];
    var AdministrasiLevel3 = [];
    var Teknis = [];
    var TeknisLevel1 = [];
    var TeknisLevel2 = [];
    var TeknisLevel3 = [];
    var Harga = [];
    $scope.Administrasi;
    $scope.bobotAdministrasi;
    $scope.Teknis;
    $scope.bobotTeknis;
    $scope.Harga;
    $scope.bobotHarga;
    $scope.kategori;
    $scope.nama;

    $scope.init = function() {
        $rootScope.getSession().then(function(result) {
            $rootScope.userSession = result.data.data;
            $rootScope.userLogin = $rootScope.userSession.session_data.username;
            $rootScope.authorize($scope.initialize());
        });
    };

    $scope.initialize = function() {
        $http.post($rootScope.url_api + 'metodeEvaluasi/selectKategori', {
            id: metode_evaluasi_id
        }).success(function(reply) {
            if (reply.status === 200) {
                $scope.kategori = reply.result.data;
                for (var i = 0; i < $scope.kategori.length; i++) {
                    if ($scope.kategori[i].jenis_detail === 'Administrasi')
                        $scope.bobotAdministrasi = $scope.kategori[i].bobot;
                    else if ($scope.kategori[i].jenis_detail === 'Teknis')
                        $scope.bobotTeknis = $scope.kategori[i].bobot;
                    else if ($scope.kategori[i].jenis_detail === 'Harga')
                        $scope.bobotHarga = $scope.kategori[i].bobot;
                }
                $scope.nama = reply.result.data[0].metode_evaluasi_nm;
            }
        });

        $http.post($rootScope.url_api + 'metodeEvaluasi/getDetail', {
            id: metode_evaluasi_id
        }).success(function(reply2) {
            //console.info("reply2 = " + JSON.stringify(reply2));
            if (reply2.status === 200) {
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
                $scope.Administrasi = AdministrasiLevel1;
                $scope.Teknis = TeknisLevel1;
            }
        });
    };

    $scope.keluar = function() {
        $modalInstance.dismiss('cancel');
    };
};

function Kata(srcText) {
    var self = this;
    self.srcText = srcText;
}