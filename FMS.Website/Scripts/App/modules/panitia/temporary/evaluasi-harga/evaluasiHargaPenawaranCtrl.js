angular.module('eprocAppPanitia')

        /*angular.module('evaluasi-harga-penawaran', ['ui.router', 'ngAnimate', 'ui.tinymce', 'ngSanitize', 'ngCookies',
         'ui.bootstrap'])
         
         
         .config(function($stateProvider) {
         var moduleStates = [
         {
         name: 'evaluasi-harga',
         url: '/evaluasi-harga-penawaran/:flowpaket_id/:paket_lelang_id',
         templateUrl: '/application/modules/evaluasiHargaPenawaran/evaluasiHargaPenawaran.html',
         pageTitle: 'Evaluasi Harga',
         controller: 'evaluasiHargaPenawaranCtrl',
         authenticate: true
         },
         {
         name: 'detail-penawaran-harga',
         url: '/evaluasi-harga-penawaran/detail-penawaran-harga/:flow_paket_id/:paket_lelang_id/:rekanan_id',
         templateUrl: '/application/modules/evaluasiHargaPenawaran/detailPenawaranHarga.html',
         pageTitle: 'Detail Penawaran Harga',
         controller: 'detailHargaPenawaranCtrl',
         authenticate: true
         },
         {
         name: 'sub-dalam-subPekerjaan',
         url: '/evaluasi-harga-penawaran/sub-dalam-sub/:flow_paket_id/:paket_lelang_id/:rekanan_id/:parent',
         templateUrl: '/application/modules/evaluasiHargaPenawaran/subDalamSubPekerjaan.html',
         pageTitle: 'Sub dalam Sub Pekerjaan',
         controller: 'subDalamSubPekerjaanCtrl',
         authenticate: true
         },
         {
         name: 'detail-sub-penawaran-harga',
         url: '/evaluasi-harga-penawaran/detail-penawaran-harga/sub-detail-penawaran/:flow_paket_id/:paket_lelang_id/:rekanan_id/:pr_subpekerjaan_id',
         templateUrl: '/application/modules/evaluasiHargaPenawaran/subDetailPenawaranHarga.html',
         pageTitle: 'Sub-Detail Penawaran Harga',
         controller: 'subDetailPenawaranCtrl',
         authenticate: true
         },
         {
         name: 'summary-evaluasi-penawaran-harga',
         url: '/summary/:flow_paket_id',
         templateUrl: '/application/modules/evaluasiHargaPenawaran/lihatSummary.html',
         pageTitle: 'Summary Evaluasi Penawaran',
         controller: 'summaryEvaluasiHargaCtrl',
         authenticate: true
         },
         {
         name: 'beritaacara-evaluasi-harga',
         url: '/evaluasi-harga-penawaran/berita-acara/:flowpaket_id/:paket_lelang_id',
         templateUrl: '/application/modules/evaluasiHargaPenawaran/beritaAcaraEvaluasiHarga.html',
         pageTitle: 'Berita Acara Evaluasi Harga',
         controller: 'BAevaluasiHargaCtrl',
         authenticate: true
         }
         ];
         for (i = 0; i < moduleStates.length; i++) {
         $stateProvider.state(moduleStates[i].name, moduleStates[i]);
         }
         })
         
         
         .state('evaluasi-harga',{
         name: 'evaluasi-harga',
         url: '/evaluasi-harga-penawaran/:flowpaket_id/:paket_lelang_id',
         templateUrl: folderTemplate+'evaluasiHargaPenawaran/evaluasiHargaPenawaran.html',
         pageTitle: 'Evaluasi Harga',
         controller: 'evaluasiHargaPenawaranCtrl',
         resolve: {
         loadMyFiles:function($ocLazyLoad) {
         return $ocLazyLoad.load({
         name: nameAngularModule ,
         files:[
         folderTemplate+'evaluasiHargaPenawaran/evaluasiHargaPenawaranCtrl.js'
         ]
         });
         }
         }
         })
         
         */

        .controller('evaluasiHargaPenawaranCtrl', function ($state, $scope, $http, $rootScope, $modal, $cookieStore, $stateParams) {
            var flow_paket_id = Number($stateParams.flowpaket_id);
            var paket_lelang_id = Number($stateParams.paket_lelang_id);
            $scope.paket_lelang_id = Number($stateParams.paket_lelang_id);
            $scope.userBisaNgatur = false;
            var page_id = 145;
            $scope.currentPeserta = [];
            $scope.nama_paket = "";
            $scope.nama_tahapan = "";
            $scope.is_created = false;
            var sampul;
            $scope.peserta = [];
            $scope.dokumen = [];
            $scope.evaluasiHargaPenawaran;
            $scope.status = -1;
            $scope.menuhome = 0;
            var urutan = 0;
            var metode_evaluasi_id;
            var pr_id;
            var nilai_hps;
            var paket;
            var tahapan;
            $scope.paket = [];

            $scope.labelcurr;
//            eb.onopen = function() {
//                $rootScope.autorize();
//                $scope.init();
//            };

            $scope.init = function () {
                $scope.menuhome = $rootScope.menuhome;
                $rootScope.getSession().then(function (result) {
                    $rootScope.userSession = result.data.data;
                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
                    $rootScope.authorize(loadAwal());
                });

            };

            function loadAwal() {
                var arr = [];
                arr.push($rootScope.userLogin);
                arr.push(paket_lelang_id);
                arr.push(page_id);
                $http.post($rootScope.url_api + 'panitia/cekbisamengatur', {
                    param: arr,
                    page_id: $scope.page_id
                }).success(function (reply) {
                    if (reply.status === 200 && reply.result.data.length > 0) {
                        $scope.userBisaNgatur = reply.result.data[0].bisa_mengatur === "1";
//                        console.info("userBisaNgatur " + $scope.userBisaNgatur)
                        $rootScope.unloadLoading();
                    }
                }).error(function (err) {
                    $.growl.error({message: "Gagal Akses API >" + err});
                    return;
                });
//                $('#divEvaluasiHarga').block({
//                    message: '<div style="text-align:center;"><h4>Silakan tunggu..</h4></div>',
//                    css: {border: '3px solid #a00'}
//                });
                $rootScope.loadLoading("Silahkan Tunggu...");
                $http.post($rootScope.url_api + 'paket/detail/info', {
                    sessionID: $cookieStore.get('sessId'),
                    paket_lelang_id: paket_lelang_id,
                    flowpaket_id: flow_paket_id
                }).success(function (reply) {
                    if (reply.status == 200) {
                        $scope.paket = reply.result.data.result;
                        urutan = $scope.paket[0].urutan;
                        $scope.nama_paket = $scope.paket[0].nama_paket;
                        $scope.nama_tahapan = $scope.paket[0].nama_tahapan;
                        if( $scope.paket[0].nilai_hps == null){
                            nilai_hps = 0;
                        }else{
                            nilai_hps = $scope.paket[0].nilai_hps;
                        }
                        $scope.labelcurr = $scope.paket[0].label;
                        $scope.status = $scope.paket[0].status;
                        if ($scope.paket[0].tgl_mulai !== null) {
                            $scope.tgl_mulai = $rootScope.convertTanggal($scope.paket[0].tgl_mulai);
                        }
                        if ($scope.paket[0].tgl_selesai !== null) {
                            $scope.tgl_selesai = $rootScope.convertTanggal($scope.paket[0].tgl_selesai);
                        }
                        $scope.is_created = $scope.paket[0].is_created;

                        sampul = $scope.paket[0].sampul;
                        urutan = $scope.paket[0].urutan;
                        metode_evaluasi_id = $scope.paket[0].metode_evaluasi_id;
                        pr_id = $scope.paket[0].pr_id;
                        if ($scope.is_created == false) {
                            $http.post($rootScope.url_api + 'evaluasiharga/gethargasemua', {
                                sessionID: $cookieStore.get('sessId'),
                                paket_lelang_id: paket_lelang_id,
                                urutan: urutan,
                                metode_evaluasi_id: metode_evaluasi_id,
                                pr_id: pr_id
                            }).success(function (reply2) {
                                if (reply2.status == 200) {
                                    $scope.peserta = reply2.result.data.result;
                                    $rootScope.unloadLoading();
                                } else {
                                    $.growl.error({message: "Gagal mendapatkan data peserta!"});
                                }
                            });
                            
                        } else {
                           
                            $http.post($rootScope.url_api + 'evaluasipenawaran/select', {
                                sessionID: $cookieStore.get('sessId'),
                                paket_lelang_id: paket_lelang_id,
                                flow_paket_id: flow_paket_id
                            }).success(function (reply2) {
                                if (reply2.result.data.length > 0) {
                                    $scope.temp = reply2.result.data;
                                    console.info("temp:"+$scope.temp);
                                    $http.post($rootScope.url_api + 'evaluasiharga/gethargasemua', {
                                        sessionID: $cookieStore.get('sessId'),
                                        paket_lelang_id: paket_lelang_id,
                                        urutan: urutan,
                                        metode_evaluasi_id: metode_evaluasi_id,
                                        pr_id: pr_id
                                    }).success(function (reply3) {
                                        if (reply3.result.data.result.length > 0) {
                                            $scope.peserta = reply3.result.data.result;
                                            for (var i = 0; i < $scope.peserta.length; i++) {
                                                for (var j = 0; j < $scope.temp.length; j++) {
                                                    if ($scope.peserta[i].rekanan_id === $scope.temp[j].rekanan_id) {
                                                        $scope.peserta[i].lolos = $scope.temp[j].lolos;
                                                        $scope.peserta[i].skor = $scope.temp[j].skor;
                                                        break;
                                                    }
                                                }
                                            }
//                                            $('#divEvaluasiHarga').unblock();
                                            $rootScope.unloadLoading();
                                        } else {
                                            $.growl.error({message: "Gagal mendapatkan data jenis form evaluasi harga!"});
                                        }
                                    });

                                    $rootScope.unloadLoading();
                                } else {
                                    $.growl.error({message: "Gagal mendapatkan data jenis form evaluasi penawaran!"});
                                }
                            });
                        }

                        $rootScope.unloadLoading();

                    } else {
                        $.growl.error({message: "Gagal mendapatkan data jenis form "});
                    }
                });
            }

            $scope.lakukan = function () {
                var lempar = {
                    flow_paket_id: flow_paket_id,
                    paket_lelang_id: paket_lelang_id,
                    nama_paket: $scope.nama_paket,
                    nama_tahapan: $scope.nama_tahapan,
                    urutan: urutan,
                    peserta: $scope.peserta,
                    nilai_hps: nilai_hps
                };
                var modalInstance = $modal.open({
                    templateUrl: 'lakukanEvaluasiPenawaran.html',
                    controller: lakukanEvaluasiPenawaranHargaCtrl,
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

            $scope.edit = function () {
                var lempar = {
                    flow_paket_id: flow_paket_id,
                    paket_lelang_id: paket_lelang_id,
                    nama_paket: $scope.nama_paket,
                    nama_tahapan: $scope.nama_tahapan,
                    urutan: urutan,
                    listSkor: $scope.peserta,
                    nilai_hps: nilai_hps,
                    username: $rootScope.userLogin
                };
                var modalInstance = $modal.open({
                    templateUrl: 'editEvaluasiPenawaran.html',
                    controller: editEvaluasiPenawaranHargaCtrl,
                    resolve: {
                        item: function () {
                            return lempar;
                        }
                    }
                });
                modalInstance.result.then(function () {
                    loadAwal();
                });
            };

            $scope.detailPenawaran = function (rekanan_id) {
                $state.transitionTo('detail-penawaran-harga', {flow_paket_id: flow_paket_id, paket_lelang_id: paket_lelang_id, rekanan_id: rekanan_id});
            };

            $scope.lihatSummary = function () {
                $state.transitionTo('summary-evaluasi-penawaran-harga', {flow_paket_id: flow_paket_id});
            };
        })

        .controller('detailHargaPenawaranCtrl', function ($state, $scope, $http, $rootScope, $cookieStore, $stateParams, $modal) {
            var flow_paket_id = Number($stateParams.flow_paket_id);
            var paket_lelang_id = Number($stateParams.paket_lelang_id);
            $scope.flow_paket_id = Number($stateParams.flow_paket_id);
            $scope.paket_lelang_id = Number($stateParams.paket_lelang_id);
            var rekanan_id = Number($stateParams.rekanan_id);
            var pr_id;
            var metode_evaluasi_id;
            $scope.subPekerjaan = [];
            $scope.subTotal;
            $scope.subTotal_HPS;
            $scope.PPN;
            $scope.PPN_HPS;
            $scope.total;
            $scope.total_HPS;
            $scope.bobot;
            $scope.hargaPembulatan;
            $scope.sudahDibulatkan;
            $scope.namaLelang = "";
            $scope.namaRekanan = "";
            $scope.pembulatanTerendah;
            $scope.skor;
            $scope.menuhome = 0;
            var urutan;
            $scope.labelcurr;
            $scope.sudahSubmit;


            $scope.init = function () {
                $rootScope.getSession().then(function (result) {
                    $rootScope.userSession = result.data.data;
                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
                    $scope.menuhome = $rootScope.menuhome;
                    $rootScope.loadLoading("Silahkan Tunggu...");
                    $rootScope.authorize(loadawal());
                });
//                $('#tabelHarga').block({
//                    message: '<div style="text-align:center;"><h4>Silakan tunggu..</h4></div>',
//                    css: {border: '3px solid #a00'}
//                });
            };

            function loadawal() {
                var arr = [];
                arr.push(paket_lelang_id);
                $http.post($rootScope.url_api + 'paket/byid', {
                    sessionID: $cookieStore.get('sessId'),
                    param: arr
                }).success(function (reply) {
                    if (reply.status == 200) {
                        $scope.namaLelang = reply.result.data[0].nama_paket;
                    } else {
                        $.growl.error({message: "Gagal mendapatkan Nama Lelang!!"});
                        $rootScope.unloadLoading();
                        return;
                    }
                }).error(function (err) {
                    $.growl.error({message: "Gagal Akses API >" + err});
                    $rootScope.unloadLoading();
                    return;
                });
                $http.post($rootScope.url_api + 'rekanan/getNama', {
                    sessionID: $cookieStore.get('sessId'),
                    rekanan_id: rekanan_id
                }).success(function (reply) {
                    if (reply.status === 200) {
                        $scope.namaRekanan = reply.result.data[0].nama_perusahaan;
//                        console.info($scope.namaRekanan);
                    } else {
                        $.growl.error({message: "Gagal mendapatkan Nama Rekanan!!"});
                        $rootScope.unloadLoading();
                        return;
                    }
                }).error(function (err) {
                    $.growl.error({message: "Gagal Akses API >" + err});
                    $rootScope.unloadLoading();
                    return;
                });
                $http.post($rootScope.url_api + 'paket/detail/info', {
                    sessionID: $cookieStore.get('sessId'),
                    paket_lelang_id: paket_lelang_id,
                    flowpaket_id: flow_paket_id
                }).success(function (reply) {
                    if (reply.status === 200) {
                        pr_id = reply.result.data.result[0].pr_id;
                        $scope.labelcurr = reply.result.data.result[0].label;
                        metode_evaluasi_id = reply.result.data.result[0].metode_evaluasi_id;
                        urutan = reply.result.data.result[0].urutan;
                        $http.post($rootScope.url_api + 'metodeEvaluasi/bobotHarga', {
                            sessionID: $cookieStore.get('sessId'),
                            metode_evaluasi_id: metode_evaluasi_id
                        }).success(function (reply2) {
                            if (reply2.result.data.length > 0) {
                                $scope.bobot = reply2.result.data[0].bobot;
                                $http.post($rootScope.url_api + 'evaluasiharga/getharga', {
                                    sessionID: $cookieStore.get('sessId'),
                                    flow_paket_id: flow_paket_id,
                                    paket_lelang_id: paket_lelang_id,
                                    rekanan_id: rekanan_id,
                                    pr_id: pr_id,
                                    urutan: urutan
                                }).success(function (reply3) {
                                    //////console.info("reply2 = " + JSON.stringify(reply2));
                                    if (reply3.status === 200) {
                                        $scope.subPekerjaan = reply3.result.data.result;
                                        $scope.subTotal = reply3.result.data.subTotal;
                                        $scope.subTotal_HPS = reply3.result.data.subTotal_HPS;
                                        $scope.PPN = reply3.result.data.PPN;
                                        $scope.PPN_HPS = reply3.result.data.PPN_HPS;
                                        $scope.total = reply3.result.data.total;
                                        $scope.total_HPS = reply3.result.data.total_HPS;
                                        $scope.pembulatanTerendah = reply3.result.data.pembulatanTerendah;
                                        $scope.hargaPembulatan = reply3.result.data.pembulatan;
                                        $scope.sudahDibulatkan = reply3.result.data.sudahDibulatkan;
                                        //Tidak digunakan di Jiwasraya -> Set Manual 0-100
                                        /*
                                        $scope.skor = ($scope.total_HPS / $scope.total) * $scope.bobot;
                                        $scope.skor = Math.round($scope.skor) / 100;
                                        */
                                        $scope.sudahSubmit = reply3.result.data.sudahSubmit;
                                        $rootScope.unloadLoading();
                                    } else {
                                        $.growl.error({message: "Gagal mendapatkan harga "});
                                        return;
                                        $rootScope.unloadLoading();
                                    }
                                });
//                                
                            } else {
                                $.growl.error({message: "Gagal mendapatkan bobot harga "});
                                return;
                                $rootScope.unloadLoading();
                            }
                        }).error(function (err) {
                            $.growl.error({message: "Gagal Akses API >" + err});
                            return;
                            $rootScope.unloadLoading();
                        });
                        //getPembulatan();

//                        
                    } else {
                        $.growl.error({message: "Gagal mendapatkan data info detail paket "});
                        return;
                        $rootScope.unloadLoading();
                    }
                });
            }
            /*function getPembulatan() {
             eb.send('itp.evaluasiHarga.getPembulatan', {
             sessionID: $cookieStore.get('sessId'),
             paket_lelang_id: paket_lelang_id,
             rekanan_id: rekanan_id
             }, function(reply2) {
             ////////console.info("get pembulatan = " + JSON.stringify(reply2));
             if (reply2.result.length > 0) {
             $scope.hargaPembulatan = reply2.result[0].harga;
             $scope.sudahDibulatkan = true;
             
             }
             else {
             $scope.hargaPembulatan = 0;
             $scope.sudahDibulatkan = false;
             }
             
             });
             }*/

            $scope.detailSubPenawaran = function (pr_subpekerjaan_id, jumlah_anak) {
                console.info("jumlah anak = " + jumlah_anak);
                if (jumlah_anak === 0) {
                    $state.transitionTo('detail-sub-penawaran-harga', {flow_paket_id: flow_paket_id, paket_lelang_id: paket_lelang_id, rekanan_id: rekanan_id, pr_subpekerjaan_id: pr_subpekerjaan_id});
                } else if (jumlah_anak > 0) {
                    $state.transitionTo('sub-dalam-subPekerjaan', {flow_paket_id: flow_paket_id, paket_lelang_id: paket_lelang_id, rekanan_id: rekanan_id, parent: pr_subpekerjaan_id});
                }
            };

            $scope.bulatkan = function () {
//                $('#tabelHarga').block({
//                    message: '<div style="text-align:center;"><h4>Silakan tunggu..</h4></div>',
//                    css: {border: '3px solid #a00'}
//                });
                $rootScope.loadLoading("Silahkan Tunggu...");
                console.info($scope.sudahDibulatkan);
                //////console.info("$scope.sudahDibulatkan = "+$scope.sudahDibulatkan);
                if ($scope.sudahDibulatkan == false) {
                    $http.post($rootScope.url_api + 'evaluasiharga/pembulatan/insert', {
                        sessionID: $cookieStore.get('sessId'),
                        paket_lelang_id: paket_lelang_id,
                        rekanan_id: rekanan_id,
                        harga: $scope.hargaPembulatan,
                        username: $rootScope.userLogin
                    }).success(function (reply) {
//                        $('#tabelHarga').unblock();
                        $rootScope.unloadLoading();
                        if (reply.status === 200) {
                            $.growl.notice({title: "[INFO]", message: "Berhasil membulatkan harga"});
                            //getPembulatan();
                            $scope.init();
//                            
                        } else {
                            $.growl.error({title: "[PERINGATAN]", message: "Gagal membulatkan harga"});
                        }
                    });
                } else if ($scope.sudahDibulatkan === true) {
                    $http.post($rootScope.url_api + 'evaluasiharga/pembulatan/update', {
                        sessionID: $cookieStore.get('sessId'),
                        paket_lelang_id: paket_lelang_id,
                        rekanan_id: rekanan_id,
                        harga: $scope.hargaPembulatan,
                        username: $rootScope.userLogin
                    }).success(function (reply) {
//                        $('#tabelHarga').unblock();
                        $rootScope.unloadLoading();
                        if (reply.status === 200) {
                            $.growl.notice({title: "[INFO]", message: "Berhasil mengupdate pembulatan harga"});
                            //getPembulatan();
                            $scope.init();

                        } else {
                            $.growl.error({title: "[PERINGATAN]", message: "Gagal mengupdate pembulatan harga"});
                        }
                    });
                }
            };
        })

        .controller('subDalamSubPekerjaanCtrl', function ($state, $scope, $http, $rootScope, $cookieStore, $stateParams) {
            var flow_paket_id = Number($stateParams.flow_paket_id);
            var paket_lelang_id = Number($stateParams.paket_lelang_id);
            var rekanan_id = Number($stateParams.rekanan_id);
            var parent = Number($stateParams.parent);
            $scope.flow_paket_id = Number($stateParams.flow_paket_id);
            $scope.paket_lelang_id = Number($stateParams.paket_lelang_id);
            $scope.rekanan_id = Number($stateParams.rekanan_id);
            $scope.parent = Number($stateParams.parent);
            //var pr_id;
            $scope.subPekerjaan = [];
            $scope.subTotal;
            $scope.subTotal_HPS;
            //$scope.PPN;
            //$scope.PPN_HPS;
            //$scope.total;
            //$scope.total_HPS;
            //$scope.bobot;
            //$scope.hargaPembulatan;
            //$scope.sudahDibulatkan;
            $scope.namaLelang = "";
            $scope.namaRekanan = "";
            $scope.namaParent = "";
            $scope.labelcurr;
//            eb.onopen = function() {
//                $rootScope.autorize();
//                $scope.init();
//            };

            $scope.init = function () {
                $rootScope.getSession().then(function (result) {
                    $rootScope.userSession = result.data.data;
                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
                    $scope.menuhome = $rootScope.menuhome;
                    $rootScope.loadLoading("Silahkan Tunggu...");
                    $rootScope.authorize(loadawal());
                });
            };

            function loadawal() {
                var arr = [];
                arr.push(paket_lelang_id);
                $http.post($rootScope.url_api + 'paket/byid', {
                    sessionID: $cookieStore.get('sessId'),
                    param: arr
                }).success(function (reply) {
                    if (reply.status == 200) {
                        $scope.namaLelang = reply.result.data[0].nama_paket;
                        $rootScope.unloadLoading();
                    } else {
                        $.growl.error({message: "Gagal mendapatkan Nama Lelang!!"});
                        $rootScope.unloadLoading();
                        return;
                    }
                }).error(function (err) {
                    $.growl.error({message: "Gagal Akses API >" + err});
                    $rootScope.unloadLoading();
                    return;
                });

                $http.post($rootScope.url_api + 'rekanan/getdatarekanan', {
                    sessionID: $cookieStore.get('sessId'),
                    rekanan_id: rekanan_id
                }).success(function (reply) {
                    if (reply.status == 200) {
                        $scope.namaRekanan = reply.result.data[0].nama_perusahaan;
                        $rootScope.unloadLoading();
                    } else {
                        $.growl.error({message: "Gagal mendapatkan Nama Perusahaan!!"});
                        $rootScope.unloadLoading();
                        return;
                    }
                }).error(function (err) {
                    $.growl.error({message: "Gagal Akses API >" + err});
                    $rootScope.unloadLoading();
                    return;
                });
                $http.post($rootScope.url_api + 'EHC/SubPekerjaanName', {
                    sessionID: $cookieStore.get('sessId'),
                    pr_subpekerjaan_id: parent
                }).success(function (reply) {
                    if (reply.status === 200) {
                        $scope.namaParent = reply.result.data.nama;
                        $rootScope.unloadLoading();
                    } else {
                        $.growl.error({message: "Gagal mendapatkan Nama Sub Pekerjaan!!"});
                        $rootScope.unloadLoading();
                        return;
                    }
                }).error(function (err) {
                    $.growl.error({message: "Gagal Akses API >" + err});
                    $rootScope.unloadLoading();
                    return;
                });

                //getPembulatan();
                //itp.evaluasiHarga.getSubDalamSub
                $http.post($rootScope.url_api + 'evaluasiharga/subdalamsub', {
                    sessionID: $cookieStore.get('sessId'),
                    flow_paket_id: flow_paket_id,
                    paket_lelang_id: paket_lelang_id,
                    rekanan_id: rekanan_id,
                    parent: parent
                }).success(function (reply2) {
                    ////////console.info("reply2 = "+JSON.stringify(reply2));
                    if (reply2.status == 200) {
                        $scope.subPekerjaan = reply2.result.data.result;
                        $scope.subTotal = reply2.result.data.subTotal;
                        $scope.subTotal_HPS = reply2.result.data.subTotal_HPS;
                        //$scope.PPN = reply2.PPN;
                        //$scope.PPN_HPS = reply2.PPN_HPS;
                        //$scope.total = reply2.total;
                        //$scope.total_HPS = reply2.total_HPS;

//                        $('#tabelHarga').unblock();
                        $rootScope.unloadLoading();
                    }
                });
                $http.post($rootScope.url_api + 'paket/detail/info', {
                    sessionID: $cookieStore.get('sessId'),
                    paket_lelang_id: paket_lelang_id,
                    flowpaket_id: flow_paket_id
                }, function (reply) {
                    if (reply.status == 200) {
                        $scope.labelcurr = reply.result.data.result[0].label;

                    }
                });
            }
            /*function getPembulatan() {
             eb.send('itp.evaluasiHarga.getPembulatan', {
             sessionID: $cookieStore.get('sessId'),
             paket_lelang_id: paket_lelang_id,
             rekanan_id: rekanan_id
             }, function(reply2) {
             //////console.info("get pembulatan = " + JSON.stringify(reply2));
             if (reply2.result.length > 0) {
             $scope.hargaPembulatan = reply2.result[0].harga;
             $scope.sudahDibulatkan = true;
             
             }
             else {
             $scope.hargaPembulatan = 0;
             $scope.sudahDibulatkan = false;
             }
             
             });
             }*/

            $scope.detailSubPenawaran = function (pr_subpekerjaan_id) {
                $state.transitionTo('detail-sub-penawaran-harga', {flow_paket_id: flow_paket_id, paket_lelang_id: paket_lelang_id, rekanan_id: rekanan_id, pr_subpekerjaan_id: pr_subpekerjaan_id});
            };

            /*$scope.bulatkan = function() {
             $('#tabelHarga').block({
             message: '<div style="text-align:center;"><h4>Silakan tunggu..</h4></div>',
             css: {border: '3px solid #a00'}
             });
             if ($scope.sudahDibulatkan === false) {
             eb.send('itp.evaluasiHarga.insertPembulatan', {
             sessionID: $cookieStore.get('sessId'),
             paket_lelang_id: paket_lelang_id,
             rekanan_id: rekanan_id,
             harga: $scope.hargaPembulatan
             }, function(reply) {
             if (reply.status === 'berhasil') {
             $.growl.notice({title: "[INFO]", message: "Berhasil membulatkan harga"});
             getPembulatan();
             
             }
             else {
             $.growl.error({title: "[PERINGATAN]", message: "Gagal membulatkan harga"});
             }
             $('#tabelHarga').unblock();
             });
             }
             else if ($scope.sudahDibulatkan === true) {
             eb.send('itp.evaluasiHarga.updatePembulatan', {
             sessionID: $cookieStore.get('sessId'),
             paket_lelang_id: paket_lelang_id,
             rekanan_id: rekanan_id,
             harga: $scope.hargaPembulatan
             }, function(reply) {
             if (reply.status === 'berhasil') {
             $.growl.notice({title: "[INFO]", message: "Berhasil mengupdate pembulatan harga"});
             getPembulatan();
             
             }
             else {
             $.growl.error({title: "[PERINGATAN]", message: "Gagal mengupdate pembulatan harga"});
             }
             $('#tabelHarga').unblock();
             });
             }
             };*/
        })

        .controller('subDetailPenawaranCtrl', function ($scope, $http, $rootScope, $cookieStore, $stateParams) {
            var flow_paket_id = Number($stateParams.flow_paket_id);
            var paket_lelang_id = Number($stateParams.paket_lelang_id);
            var rekanan_id = Number($stateParams.rekanan_id);
            var pr_subpekerjaan_id = Number($stateParams.pr_subpekerjaan_id);
            $scope.flow_paket_id = Number($stateParams.flow_paket_id);
            $scope.paket_lelang_id = Number($stateParams.paket_lelang_id);
            $scope.rekanan_id = Number($stateParams.rekanan_id);
            $scope.pr_subpekerjaan_id = Number($stateParams.pr_subpekerjaan_id);
            $scope.detail;
            $scope.HPS;
            $scope.total;
            $scope.labelcurr;
            $scope.namaLelang = '';
            $scope.namaRekanan = '';
            $scope.namaParent = '';
//            eb.onopen = function() {
//                $rootScope.autorize();
//                $scope.init();
//            };

            $scope.init = function () {
                $rootScope.getSession().then(function (result) {
                    $rootScope.userSession = result.data.data;
                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
                    $scope.menuhome = $rootScope.menuhome;
                    $rootScope.loadLoading("Silahkan Tunggu...");
                    $rootScope.authorize(loadawal());
                });
            };

            function loadawal() {
                var arr = [];
                arr.push(paket_lelang_id);
                $http.post($rootScope.url_api + 'paket/byid', {
                    sessionID: $cookieStore.get('sessId'),
                    param: arr
                }).success(function (reply) {
                    if (reply.status === 200) {
                        $scope.namaLelang = reply.result.data[0].nama_paket;
                        $rootScope.unloadLoading();
                    } else {
                        $.growl.error({message: "Gagal mendapatkan Nama Lelang!!"});
                        $rootScope.unloadLoading();
                        return;
                    }
                }).error(function (err) {
                    $.growl.error({message: "Gagal Akses API >" + err});
                    $rootScope.unloadLoading();
                    return;
                });
                $http.post($rootScope.url_api + 'rekanan/getNama', {
                    sessionID: $cookieStore.get('sessId'),
                    rekanan_id: rekanan_id
                }).success(function (reply) {
                    if (reply.status === 200) {
                        $scope.namaRekanan = reply.result.data[0].nama_perusahaan;
                        $rootScope.unloadLoading();
                    } else {
                        $.growl.error({message: "Gagal mendapatkan Nama Rekanan!!"});
                        $rootScope.unloadLoading();
                        return;
                    }
                }).error(function (err) {
                    $.growl.error({message: "Gagal Akses API >" + err});
                    $rootScope.unloadLoading();
                    return;
                });

                $http.post($rootScope.url_api + 'EHC/SubPekerjaanName', {
                    sessionID: $cookieStore.get('sessId'),
                    pr_subpekerjaan_id: pr_subpekerjaan_id
                }).success(function (reply) {
                    if (reply.status === 200) {
                        $scope.namaParent = reply.result.data.nama;
                        $rootScope.unloadLoading();
                    } else {
                        $.growl.error({message: "Gagal mendapatkan Nama Sub Pekerjaan!!"});
                        $rootScope.unloadLoading();
                        return;
                    }
                }).error(function (err) {
                    $.growl.error({message: "Gagal Akses API >" + err});
                    $rootScope.unloadLoading();
                    return;
                });

                //itp.evaluasiharga.getHargaPerSub
                $http.post($rootScope.url_api + 'evaluasiharga/hargapersub', {
                    sessionID: $cookieStore.get('sessId'),
                    paket_lelang_id: paket_lelang_id,
                    flowpaket_id: flow_paket_id,
                    rekanan_id: rekanan_id,
                    pr_subpekerjaan_id: pr_subpekerjaan_id
                }).success(function (reply) {
                    if (reply.status == 200) {
                        $scope.detail = reply.result.data.result;
                        $scope.HPS = reply.result.data.HPS;
                        $scope.total = reply.result.data.total;
//                        $('#tabelHarga').unblock();
                        $rootScope.unloadLoading();
                    } else {
                        $.growl.error({message: "Gagal mendapatkan Data Harga per Sub!!"});
                        $rootScope.unloadLoading();
                        return;
                    }
                }).error(function (err) {
                    $.growl.error({message: "Gagal Akses API >" + err});
                    $rootScope.unloadLoading();
                    return;
                });
                //itp.paketDetail.getInfo
                $http.post($rootScope.url_api + 'paket/detail/info', {
                    sessionID: $cookieStore.get('sessId'),
                    paket_lelang_id: paket_lelang_id,
                    flowpaket_id: flow_paket_id
                }).success(function (reply) {
                    if (reply.status === 200) {
                        $scope.labelcurr = reply.result.data.result[0].label;
                        $rootScope.unloadLoading();
                    } else {
                        $.growl.error({message: "Gagal mendapatkan Data Detail Info!!"});
                        $rootScope.unloadLoading();
                        return;
                    }
                }).error(function (err) {
                    $.growl.error({message: "Gagal Akses API >" + err});
                    $rootScope.unloadLoading();
                    return;
                });
            }
        })
        .controller('summaryEvaluasiHargaCtrl', function ($scope, $http, $rootScope, $cookieStore, $stateParams) {
            var flow_paket_id = Number($stateParams.flow_paket_id);
            $scope.evaluasi = {};

//            eb.onopen = function() {
//                $rootScope.autorize();
//                $scope.init();
//            };

            $scope.init = function () {
                loadAwal();
            };

            function loadAwal() {
                $http.post($rootScope.url_api + 'getsummary', {
                    sessionID: $cookieStore.get('sessId'),
                    flow_paket_id: flow_paket_id
                }).success(function (reply2) {
                    if (reply2.result.data.length > 0) {
                        $scope.evaluasi = reply2.result.data[0];
                        if ($scope.evaluasi.tgl_upload !== null || $scope.evaluasi.tgl_upload !== "") {
                            $scope.evaluasi.tgl_upload_conv = $rootScope.convertTanggal($scope.evaluasi.tgl_upload);
                        } else {
                            $scope.evaluasi.tgl_upload_conv = "";
                        }
                        if ($scope.evaluasi.tgl_evaluasi !== null || $scope.evaluasi.tgl_evaluasi !== "") {
                            $scope.evaluasi.tgl_evaluasi_conv = $rootScope.convertTanggal($scope.evaluasi.tgl_evaluasi);
                        } else {
                            $scope.evaluasi.tgl_evaluasi_conv = "";
                        }
                        console.info('evaluasi ' + JSON.stringify($scope.evaluasi.dokumen_file));
                    }
                });
            }
        })
        .controller('BAevaluasiHargaCtrl', function ($state, $scope, $rootScope, $modal, $cookieStore, $stateParams) {
            var flow_paket_id = Number($stateParams.flowpaket_id);
            var paket_lelang_id = Number($stateParams.paket_lelang_id);
            $scope.userBisaNgatur = false;
            var page_id = 145;
            $scope.currentPeserta = [];
            $scope.nama_paket = "";
            $scope.nama_tahapan = "";
            $scope.is_created = false;
            var sampul;
            $scope.peserta = [];
            $scope.dokumen = [];
            $scope.evaluasiHargaPenawaran;
            $scope.status = -1;
            var urutan = 0;

//            eb.onopen = function() {
//                $rootScope.autorize();
//                $scope.init();
//            };

            $scope.init = function () {
                loadAwal();
            };

            function loadAwal() {

            }

            $scope.cetakBA = function () {
                $('#printable').jqprint();
            };
        });

var lakukanEvaluasiPenawaranHargaCtrl = function ($scope, $rootScope, $modalInstance, $http, $cookieStore, item) {
    var paket_lelang_id = item.paket_lelang_id;
    var flow_paket_id = item.flow_paket_id;
    var nama_paket = item.nama_paket;
    var nama_tahapan = item.nama_tahapan;
    var urutan = item.urutan;
    var peserta = item.peserta;
    var sortedHarga = [];
    var hargaPembulatan = 0;
    var dtlTemplate = '';
    var alamatRekanan = '';
    var template = '';
    var nama_rekanan = '';
    var nilai_hps = item.nilai_hps;
    $scope.isLolos = [{value: true, label: 'Lolos'}, {value: false, label: 'Tidak Lolos'}];
    $scope.evaluasiHargaPenawaran = new evaluasiHargaPenawaran($rootScope.currentDate, "", "", "", "", "");
    $scope.customTinymce = {
        theme: "modern",
        plugins: [
            "advlist autolink lists link image charmap print preview hr anchor pagebreak",
            "searchreplace wordcount visualblocks visualchars fullscreen",
            "insertdatetime media nonbreaking save table contextmenu directionality",
            "emoticons template paste textcolor"
        ],
        toolbar1: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
        toolbar2: "print preview media | forecolor backcolor",
        image_advtab: true,
        height: "200px",
        width: "auto"
    };
    $scope.file;
    $scope.filesTChanged = function (elm) {
        $scope.file = elm.files;
    };
    $rootScope.fileuploadconfig(24);
    $scope.currentPeserta = [];
    $scope.dataGrid = [];
    $scope.selectedRekanan;

    $scope.init = function () {
//        eb.send('itp.pendaftaranLelang.getPesertaByUrutanDanBertahan', {
//            sessionID: $cookieStore.get('sessId'),
//            paket_lelang_id: paket_lelang_id,
//            urutan: urutan
//        }, function(reply) {
//            if (reply.status === 'ok') {
//                //console.info('current peserta ' + JSON.stringify(reply.result));
//                $scope.currentPeserta = reply.result;
//                for (var i = 0; i < $scope.currentPeserta.length; i++) {
//                    for (var j = 0; j < listSkor.length; j++) {
//                        if ($scope.currentPeserta[i].rekanan_id === listSkor[j].rekanan_id) {
//                            $scope.currentPeserta[i].skor = listSkor[j].skor;
//                        }
//                    }
//                }
//                
//            }
//        });
       // console.info("nama_tahapan --" + nama_tahapan + "----");
        //console.info("peserta" + JSON.stringify(item.peserta));
        $scope.temp = peserta;

    };

    Number.prototype.format = function (n, x, s, c) {
        var re = '\\d(?=(\\d{' + (x || 3) + '})+' + (n > 0 ? '\\D' : '$') + ')',
                num = this.toFixed(Math.max(0, ~~n));

        return (c ? num.replace('.', c) : num).replace(new RegExp(re, 'g'), '$&' + (s || ','));
    };

    function generate() {
        //console.info('temp ' + JSON.stringify($scope.temp));
        var list = [];
        for (var i = 0; i < peserta.length; i++) {
            for (var j = 0; j < $scope.temp.length; j++) {
                if ((peserta[i].rekanan_id == $scope.temp[j].rekanan_id) && ($scope.temp[j].lolos)) {
                    list.push(peserta[i]);
                }
            }
        }
        for (var i = 0; i < list.length; i++) {
            var harga_total_paket = 0;
            var obj = {};
            for (var j = 0; j < list[i].hasil.length; j++) {
//                ////console.info('harga total ' + parseFloat(listSkor[i].hasil[j].total));
                harga_total_paket += parseFloat(list[i].hasil[j].total);
            }
            obj = {
                rekanan_id: list[i].rekanan_id,
                nama_perusahaan: list[i].nama_perusahaan,
                harga_total_paket: harga_total_paket
            };
            sortedHarga.push(obj);
        }

        sortedHarga.sort(function (a, b) {
            return parseFloat(a.harga_total_paket) - parseFloat(b.harga_total_paket);
        });


        for (var i = 0; i < sortedHarga.length; i++) {
            if (i == (sortedHarga.length - 1)) {
                nama_rekanan += sortedHarga[i].nama_perusahaan;
            } else {
                nama_rekanan += sortedHarga[i].nama_perusahaan + ', ';
            }
        }
        //itp.template.select
        $http.post($rootScope.url_api + 'template/select', {
            sessionID: $cookieStore.get('sessId'),
            type: "evaluasi_harga"
        }).success(function (reply) {
            if (reply.status == 200) {
                template = reply.result.data[0].isi_template;
                $http.post($rootScope.url_api + 'template/select', {
                    sessionID: $cookieStore.get('sessId'),
                    type: "detail_evaluasi_harga"
                }).success(function (replya) {
                    if (replya.status == 200) {
                        getData(replya, 0);

//                        for (var i = 0; i < sortedHarga.length; i++) {
////                            var rekanan_id = sortedHarga[i].rekanan_id;
//                            var sorted = sortedHarga[i];
//                            ////console.info(romanize(i + 1) + '. sorted => ' + JSON.stringify(sorted));
//                            var dtlTemp = '';
//                            if (i != (sortedHarga.length - 1)) {
//                                eb.send('itp.rekanan.getBiodata', {
//                                    sessionID: $cookieStore.get('sessId'),
//                                    rekanan_id: sorted.rekanan_id
//                                }, function(reply2) {
//                                    ////console.info('reply2 ' + JSON.stringify(reply2));
//                                    if (reply2.status === 'ok') {
//                                        alamatRekanan = reply2.result[0].alamat;
//                                        eb.send('itp.evaluasiHarga.getPembulatan', {
//                                            sessionID: $cookieStore.get('sessId'),
//                                            paket_lelang_id: paket_lelang_id,
//                                            rekanan_id: sorted.rekanan_id
//                                        }, function(reply3) {
//                                            ////console.info('reply3 ' + JSON.stringify(reply3));
//                                            if (reply3.result.length > 0) {
//                                                hargaPembulatan = reply3.result[0].harga;
//                                            } else {
//                                                hargaPembulatan = 0;
//                                            }
//                                            dtlTemp = replya.result[0].isi_template;
//                                            dtlTemp = dtlTemp
//                                                    .replace('#penawar_ke', romanize(i + 1))
//                                                    .replace('#rekanan_tawar_terendah', sorted.nama_perusahaan)
//                                                    .replace('#alamat_rekanan_tawar_terendah', alamatRekanan)
//                                                    .replace('#harga_tawar_terendah', sorted.harga_total_paket.format(2, 3, '.', ','))
//                                                    .replace('#harga_pembulatan', hargaPembulatan.format(2, 3, '.', ','))
//                                                    .replace('#nomor_ba', $scope.evaluasiHargaPenawaran.nomor_ba)
//                                                    .replace('#tanggal_kata_bulan', (stu[2] + ' ' + wMonths[parseInt(stu[1]) - 1] + ' ' + stu[0]));
//
//                                            dtlTemplate += dtlTemp;
//                                        });
//                                    }
//                                });
//                            } else {
//                                eb.send('itp.rekanan.getBiodata', {
//                                    sessionID: $cookieStore.get('sessId'),
//                                    rekanan_id: sorted.rekanan_id
//                                }, function(reply4) {
//                                    if (reply4.status === 'ok') {
//                                        alamatRekanan = reply4.result[0].alamat;
//                                        eb.send('itp.evaluasiHarga.getPembulatan', {
//                                            sessionID: $cookieStore.get('sessId'),
//                                            paket_lelang_id: paket_lelang_id,
//                                            rekanan_id: sorted.rekanan_id
//                                        }, function(reply5) {
//                                            if (reply5.result.length > 0) {
//                                                hargaPembulatan = reply5.result[0].harga;
//                                            } else {
//                                                hargaPembulatan = 0;
//                                            }
//                                            dtlTemp = replya.result[0].isi_template;
//                                            dtlTemp = dtlTemp
//                                                    .replace('#penawar_ke', romanize(i + 1))
//                                                    .replace('#rekanan_tawar_terendah', sorted.nama_perusahaan)
//                                                    .replace('#alamat_rekanan_tawar_terendah', alamatRekanan)
//                                                    .replace('#harga_tawar_terendah', sorted.harga_total_paket.format(2, 3, '.', ','))
//                                                    .replace('#harga_pembulatan', hargaPembulatan.format(2, 3, '.', ','))
//                                                    .replace('#nomor_ba', $scope.evaluasiHargaPenawaran.nomor_ba)
//                                                    .replace('#tanggal_kata_bulan', (stu[2] + ' ' + wMonths[parseInt(stu[1]) - 1] + ' ' + stu[0]));
//
//                                            dtlTemplate += dtlTemp;
//
//                        template = reply.result[0].isi_template;
//
//                        template = template
//                                .replace('#upper_tahapan', nama_tahapan.toUpperCase())
//                                .replace(/#nomor_ba/g, $scope.evaluasiHargaPenawaran.nomor_ba)
//                                .replace('#tahapan', nama_tahapan)
//                                .replace('#hari', getDayName(stu[0] + '-' + stu[1] + '-' + stu[2]))
//                                .replace('#tanggal_kata', toWords(stu[2]) + " " + wMonths[parseInt(stu[1]) - 1] + " " + toWords(stu[0]))
//                                .replace(/#pekerjaan/g, nama_paket)
//                                .replace('#tanggal', (stu[2] + '-' + stu[1] + '-' + stu[0]))
//                                .replace('#jml_lolos_kata', toWords(sortedHarga.length))
//                                .replace('#jml_lolos', sortedHarga.length)
//                                .replace('#rekanan_lolos', nama_rekanan)
//                                .replace('#nilai_hps', nilai_hps.format(2, 3, '.', ','))
//                                .replace('#detail_evaluasi_harga', getData(replya, 0));
////                    $scope.berita_acara = new berita_acara(template, "", $scope.berita_acara.nomor_ba);
//                        $scope.evaluasiHargaPenawaran.summary = template;
//                        tinymce.get('berita_acara_content').setContent(template);
//                        $scope.$apply;
//                                        });
//                                    }
//                                });
//                            }
//                        }
                    }
                });

            }
        });

    }
    ;

    function getData(replya, i) {
        ////console.info((i + 1) + '. romawinya adlah ' + romanize(i + 1));
        var roman = '';
        roman = romanize(i + 1);
        var sorted = '';
        sorted = sortedHarga[i];
        console.info(' sorted ' + JSON.stringify(sorted));
        var sta = $scope.evaluasiHargaPenawaran.tanggal_evaluasi.split(/(\s+)/);
        var stu = sta[0].split('-');
        var wMonths = ['Januari', 'Februari', 'Maret', 'April', 'Mei', 'Juni', 'Juli', 'Agustus', 'September', 'Oktober', 'November', 'Desember'];

        var dtlTemp = '';
        $http.post($rootScope.url_api + 'rekanan/getdatarekanan', {
            sessionID: $cookieStore.get('sessId'),
            rekanan_id: sorted.rekanan_id
        }).success(function (reply2) {
            ////console.info('reply2 ' + JSON.stringify(reply2));
            if (reply2.status == 200) {
                alamatRekanan = reply2.result.data[0].alamat;
                //itp.evaluasiHarga.getPembulatan
                $http.post($rootScope.url_api + 'evaluasiharga/getpembulatan', {
                    sessionID: $cookieStore.get('sessId'),
                    paket_lelang_id: paket_lelang_id,
                    rekanan_id: sorted.rekanan_id
                }).success(function (reply3) {
                    ////console.info('reply3 ' + JSON.stringify(reply3));
                    if (reply3.result.length > 0) {
                        hargaPembulatan = reply3.result.data[0].harga;
                    } else {
                        hargaPembulatan = 0;
                    }
                    dtlTemp = replya.result.data[0].isi_template;
                    dtlTemp = dtlTemp
                            .replace('#penawar_ke', roman)
                            .replace('#rekanan_tawar_terendah', sorted.nama_perusahaan)
                            .replace('#alamat_rekanan_tawar_terendah', alamatRekanan)
                            .replace('#harga_tawar_terendah', sorted.harga_total_paket.format(2, 3, '.', ',')).replace('#harga_pembulatan', hargaPembulatan.format(2, 3, '.', ',')).replace('#nomor_ba', $scope.evaluasiHargaPenawaran.nomor_ba)
                            .replace('#tanggal_kata_bulan', (stu[2] + ' ' + wMonths[parseInt(stu[1]) - 1] + ' ' + stu[0]));
                    dtlTemplate += dtlTemp;
                    i++;
                    console.info("oke tes");
                    if (i < sortedHarga.length) {
                        getData(replya, i);
                    } else {
                        setTemplate();
                    }
                });
            }
        });
//        ////console.info(dtlTemplate);
//        return dtlTemplate;
    }

    function setTemplate() {
        var sta = $scope.evaluasiHargaPenawaran.tanggal_evaluasi.split(/(\s+)/);
        var stu = sta[0].split('-');
        var wMonths = ['Januari', 'Februari', 'Maret', 'April', 'Mei', 'Juni', 'Juli', 'Agustus', 'September', 'Oktober', 'November', 'Desember'];
//        console.info(nilai_hps);
        template = template.replace('#upper_tahapan', nama_tahapan.toUpperCase())
                .replace(/#nomor_ba/g, $scope.evaluasiHargaPenawaran.nomor_ba)
                .replace('#tahapan', nama_tahapan)
                .replace('#hari', getDayName(stu[0] + '-' + stu[1] + '-' + stu[2])).replace('#tanggal_kata', toWords(stu[2]) + " " + wMonths[parseInt(stu[1]) - 1] + " " + toWords(stu[0]))
                .replace(/#pekerjaan/g, nama_paket)
                .replace('#tanggal', (stu[2] + '-' + stu[1] + '-' + stu[0])).replace('#jml_lolos_kata', toWords(sortedHarga.length))
                .replace('#jml_lolos', sortedHarga.length)
                .replace('#rekanan_lolos', nama_rekanan)
                .replace('#nilai_hps', Number(nilai_hps).format(2, 3, '.', ',')).replace('#detail_evaluasi_harga', dtlTemplate)
                .replace('#rekanan_tawar_terendah', sortedHarga[0].nama_perusahaan);
//                    $scope.berita_acara = new berita_acara(template, "", $scope.berita_acara.nomor_ba);
        $scope.evaluasiHargaPenawaran.summary = template;
        tinymce.get('berita_acara_content').setContent(template);
//        console.info("temp"+template);
    }


    $scope.generateBeritaAcara = function () {
        if ($scope.temp.length === 0) {
            $.growl.error({title: "[PERINGATAN]", message: 'pilih rekanan terlebih dahulu'});
            return;
        }
        if ($scope.evaluasiHargaPenawaran.nomor_ba === '') {
            $.growl.error({title: "[PERINGATAN]", message: 'input nomor berita acara terlebih dahulu'});
            return;
        }
        if ($scope.evaluasiHargaPenawaran.tanggal_evaluasi === '') {
            $scope.evaluasiHargaPenawaran.tanggal_evaluasi = $rootScope.currentDate; //DARI HENDRA
            /*$.growl.error({title: "[PERINGATAN]", message: 'input tanggal evaluasi terlebih dahulu'});
             return;*/
        }
        //        if ($scope.berita_acara.nomor_ba !== '' && $scope.berita_acara.tanggal_berita_acara !== '' && $scope.listPengurusTerpilih.length >= 0 && $scope.berita_acara.tgl_mulai !== '') {
//            ////console.info('tanggal ' + $scope.berita_acara.tanggal_berita_acara);
        generate();
//        }
    };

    $scope.change = function (obj) {
        $scope.selectedRekanan = obj;
        for (var i = 0; i < peserta.length; i++) {
            if (obj.rekanan_id == peserta[i].rekanan_id) {
                $scope.evaluasiHargaPenawaran.skor = peserta[i].skor;
                break;
            }
        }
    };

    $scope.stringSelectedHasilEvaluasi = "";
    $scope.lolos = false;
    $scope.changeLolos = function (obj) {
//        if (obj.lolos === '0') {
//            obj.lolos = false;
//        } else {
//            obj.lolos = true;
//        }
//        //console.info('obj ' + JSON.stringify(obj));
//        $scope.stringSelectedHasilEvaluasi = obj;

        $scope.temp.sort(function (a, b) {
            if (a.lolos && !b.lolos)
                return -1;
            else if (!a.lolos && b.lolos)
                return 1;
            else {
                return b.skor - a.skor;
            }
        });
//        $scope.$apply;

//        if (!obj.lolos) {
////            $scope.lolos = false;
//            for (i = 0; i < $scope.temp.length; i++) {
//                var switch = $scope.temp[i];
//                
//            }
//            $scope.evaluasiHargaPenawaran.peringkat = "";
//            $scope.evaluasiHargaPenawaran.skor = "";
//        }
//        else if (obj === '1')
//            $scope.lolos = true;
    };

    $scope.temp = [];
    $scope.tambahPerusahaan = function () {
        if ($scope.selectedRekanan === "" || $scope.selectedRekanan === null || $scope.selectedRekanan === undefined) {
            $.growl.error({title: "[PERINGATAN]", message: "Rekanan belum dipilih"});
            return;
        }
        if ($scope.stringSelectedHasilEvaluasi === '') {
            $.growl.error({title: "[PERINGATAN]", message: "Lolos tidaknya belum ditentukan"});
            return;
        }
        if ($scope.lolos === true && $scope.evaluasiHargaPenawaran.skor === '') {
            $.growl.error({title: "[PERINGATAN]", message: "Skor belum dimasukkan"});
            return;
        }
        if ($scope.lolos === true && $scope.evaluasiHargaPenawaran.peringkat === '') {
            $.growl.error({title: "[PERINGATAN]", message: "Peringkat belum dimasukkan"});
            return;
        }

        var sudahAda = false;
        for (var i = 0; i < $scope.temp.length; i++)
        {
            if ($scope.temp[i].rekanan_id === $scope.selectedRekanan.rekanan_id)
            {
                sudahAda = true;
                break;
            }
        }
        if (sudahAda) {
            $.growl.error({title: "[PERINGATAN]", message: "Rekanan sudah dipilih"});
            return;
        }
        var peringkatSudahAda = false;
        for (var i = 0; i < $scope.temp.length; i++)
        {
            if ($scope.lolos === true && $scope.temp[i].peringkat === $scope.evaluasiHargaPenawaran.peringkat)
            {
                peringkatSudahAda = true;
                break;
            }
        }
        if (peringkatSudahAda) {
            $.growl.error({title: "[PERINGATAN]", message: "Peringkat sudah ada"});
            return;
        } else {
            $scope.temp.push({
                rekanan_id: $scope.selectedRekanan.rekanan_id,
                nama_perusahaan: $scope.selectedRekanan.nama_perusahaan,
                lolos: $scope.lolos,
                peringkat: $scope.evaluasiHargaPenawaran.peringkat,
                skor: $scope.evaluasiHargaPenawaran.skor
            });
        }
    };

    $scope.hapusData = function (rekanan_id) {
        var idx = -1;
        for (var i = 0; i < $scope.temp.length; i++)
        {
            if ($scope.temp[i].rekanan_id === rekanan_id)
            {
                idx = i;
                break;
            }
        }
        $scope.temp.splice(idx, 1);
    };

    $scope.simpan = function () {
        if ($scope.evaluasiHargaPenawaran.tanggal_evaluasi === '') {
            $.growl.error({title: "[PERINGATAN]", message: "Tanggal evaluasi belum dimasukkan"});
            return;
        }
        //if($scope.evaluasiHargaPenawaran.tanggal_upload === ''){
        //    alert("tanggal upload belum dimasukkan");
        //    return;
        //}
        if ($scope.evaluasiHargaPenawaran.nomor_ba === '') {
            $.growl.error({title: "[PERINGATAN]", message: "Nomor BA belum dimasukkan"});
            return;
        }
        if ($scope.evaluasiHargaPenawaran.summary === '') {
            $.growl.error({title: "[PERINGATAN]", message: "Summary masih kosong"});
            return;
        }
        if ($scope.temp.length === 0) {
            $.growl.error({title: "[PERINGATAN]", message: "Belum ada rekanan yang dievaluasi"});
            return;
        }
        if ($scope.temp.length < $scope.currentPeserta.length) {
            $.growl.error({title: "[PERINGATAN]", message: "Ada rekanan yang belum dievaluasi"});
            return;
        }
        var skorValid = true;
        $scope.temp.forEach(function(t){
            if (t.lolos == true && !t.skor){
                skorValid = false;
            }
        });
        if(!skorValid){
            $.growl.error({title: "[PERINGATAN]", message: "Nilai Skor tidak valid"});
            return;
        }
//        $('#lakukanEvaluasi').block({
//            message: '<div style="text-align:center;"><h4>Silakan tunggu..</h4></div>',
//            css: {border: '3px solid #a00'}
//        });
        $rootScope.loadLoadingModal("Silahkan Tunggu...");
        for (var i = 0; i < $scope.temp.length; i++) {
            if ($scope.temp[i].lolos) {
                $scope.temp[i].peringkat = i + 1;
            } else {
                $scope.temp[i].peringkat = '';
                $scope.temp[i].skor = '';
            }
        }
        if ($scope.file === undefined) {
            insert("");
        } else {
            $scope.evaluasiHargaPenawaran.tanggal_upload = $rootScope.currentDate;
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
                    if ($.inArray(extFile, restrictedExt) == -1) {
                        $rootScope.unloadLoadingModal();
                        $.growl.error({title: "[WARNING]", message: "Format file tidak valid"});
                        return;
                    } else {
                        upload();
                    }
                }
            }
        }
    };

    function upload() {
        var fd = new FormData();
        angular.forEach($scope.file, function (file) {
            fd.append("uploads", file);
        });
        $http.post($rootScope.url_api + "/upload/pengadaan_"+ paket_lelang_id +"/", fd, {
            withCredentials: true,
            transformRequest: angular.identity(),
            headers: {'Content-Type': undefined}
        }).success(function (reply) {
            if (reply.status === 200) {
                var result = reply.result.data;
                insert(result.files[0].url);
//                        insertBlacklist(result.files[0].url);
            } else {
                $.growl.error({message: "Gagal mendapatkan data hak akses"});
                return;
            }
        })
                .error(function (err) {
                    $.growl.error({message: "Gagal Akses API >" + err});
                    return;
                });
    }
    ;


    function insert(url) {
        var master = [];
        master.push($scope.evaluasiHargaPenawaran.tanggal_evaluasi);
        master.push($scope.evaluasiHargaPenawaran.tanggal_upload);
        master.push($scope.evaluasiHargaPenawaran.nomor_ba);
        master.push(url);
        master.push($scope.evaluasiHargaPenawaran.summary);
        master.push(flow_paket_id);
        //master.push(paket_lelang_id);
        var details = [];
        var detail;
        for (var i = 0; i < $scope.temp.length; i++) {
            detail = [];
            //detail.push($scope.temp[i].lolos);
            if ($scope.temp[i].lolos === true) {
                detail.push(1);
            } else {
                detail.push(0);
                //detail.push(urutan);
            }
            detail.push($scope.temp[i].rekanan_id);
            detail.push(paket_lelang_id);
            detail.push(Number($scope.temp[i].peringkat));
            detail.push(parseFloat($scope.temp[i].skor));
            detail.push(flow_paket_id);
            details.push(detail);
        }
        //itp.evaluasiHarga.insert
        $http.post($rootScope.url_api + 'evaluasiharga/insert', {
            sessionID: $cookieStore.get('sessId'),
            paket_lelang_id: paket_lelang_id,
            flow_paket_id: flow_paket_id,
            master: master,
            detail: details,
            nama_paket: nama_paket,
            nama_tahapan: nama_tahapan,
            urutan: urutan,
            username: $rootScope.userLogin
        }).success(function (reply) {
            if (reply.status == 200) {
//                $('#lakukanEvaluasi').unblock();
                $rootScope.unloadLoadingModal();
                $.growl.notice({title: "[INFO]", message: "Berhasil menyimpan data evaluasi penawaran"});
                $modalInstance.close();
            } else {
//                $('#lakukanEvaluasi').unblock();
                $rootScope.unloadLoadingModal();
                $.growl.notice({title: "[INFO]", message: "Gagal menyimpan data evaluasi penawaran"});
            }
        });
    }
    ;
    function getDayName(date) {
        var d = new Date(date);
        var weekday = new Array(7);
        weekday[0] = "Minggu";
        weekday[1] = "Senin";
        weekday[2] = "Selasa";
        weekday[3] = "Rabu";
        weekday[4] = "Kamis";
        weekday[5] = "Jum'at";
        weekday[6] = "Sabtu";
        return weekday[d.getDay()];
    }

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


    function toWords(s) {
        // Convert numbers to words
// copyright 25th July 2006, by Stephen Chapman http://javascript.about.com
// permission to use this Javascript on your web page is granted
// provided that all of the code (including this copyright notice) is
// used exactly as shown (you can change the numbering system if you wish)

// American Numbering System
        var th = ['', 'Ribu', 'juta', 'milyar', 'triliun'];
// uncomment this line for English Number System
// var th = ['','thousand','million', 'milliard','billion'];

        var dg = ['nol', 'satu', 'dua', 'tiga', 'empat', 'lima', 'enam', 'tujuh', 'delapan', 'sembilan'];
        var tn = ['sepuluh', 'sebelas', 'dua belas', 'tiga belas', 'empat belas', 'lima belas', 'enam belas', 'tujuh belas', 'delapan belas', 'sembilan belas'];
        var tw = ['dua puluh', 'tiga puluh', 'empat puluh', 'lima puluh', 'enam puluh', 'tujuh puluh', 'delapan puluh', 'sembilan puluh'];
        s = s.toString();
        s = s.replace(/[\, ]/g, '');
        if (s != parseFloat(s))
            return 'not a number';
        var x = s.indexOf('.');
        if (x == -1)
            x = s.length;
        if (x > 15)
            return 'too big';
        var n = s.split('');
        var str = '';
        var sk = 0;
        for (var i = 0; i < x; i++) {
            if ((x - i) % 3 == 2) {
                if (n[i] == '1') {
                    str += tn[Number(n[i + 1])] + ' ';
                    i++;
                    sk = 1;
                } else if (n[i] != 0) {
                    str += tw[n[i] - 2] + ' ';
                    sk = 1;
                }
            } else if (n[i] != 0) {
                str += dg[n[i]] + ' ';
                if ((x - i) % 3 == 0)
                    str += 'ratus ';
                if (n[i] == '1')
                    str += '';
                sk = 1;
            }
            if ((x - i) % 3 == 1) {
                if (sk)
                    str += th[(x - i - 1) / 3] + ' ';
                sk = 0;
            }
        }
        if (x != s.length) {
            var y = s.length;
            str += 'point ';
            for (var i = x + 1; i < y; i++)
                str += dg[n[i]] + ' ';
        }
        return str.replace(/\s+/g, ' ');
    }

    function days_between(date1, date2) {
        // The number of milliseconds in one day
        var ONE_DAY = 1000 * 60 * 60 * 24;
        // Convert both dates to milliseconds
        var getDate1 = new Date(date1);
        var getDate2 = new Date(date2);
        var date1_ms = getDate1.getTime();
        var date2_ms = getDate2.getTime();

        // Calculate the difference in milliseconds
        var difference_ms = Math.abs(date1_ms - date2_ms);

        // Convert back to days and return
        return Math.round(difference_ms / ONE_DAY);
    }

    $scope.batal = function () {
        $modalInstance.dismiss('cancel');
    };
};

var editEvaluasiPenawaranHargaCtrl = function ($scope, $modalInstance, $http, $cookieStore, item, $rootScope) {
    var paket_lelang_id = item.paket_lelang_id;
    var flow_paket_id = item.flow_paket_id;
    var nama_paket = item.nama_paket;
    var nama_tahapan = item.nama_tahapan;
    var urutan = item.urutan;
    var listSkor = item.listSkor;
    var sortedHarga = [];
    var nama_rekanan = '';
    var template = '';
    var alamatRekanan = '';
    var template = '';
    var hargaPembulatan = 0;
    var dtlTemplate = '';
    var nilai_hps = item.nilai_hps;
    var username = item.username;
    $scope.isLolos = [{value: true, label: 'Lolos'}, {value: false, label: 'Tidak Lolos'}];
    $scope.evaluasiHargaPenawaran;
    $scope.customTinymce = {
        theme: "modern",
        plugins: [
            "advlist autolink lists link image charmap print preview hr anchor pagebreak",
            "searchreplace wordcount visualblocks visualchars fullscreen",
            "insertdatetime media nonbreaking save table contextmenu directionality",
            "emoticons template paste textcolor"
        ],
        toolbar1: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",
        toolbar2: "print preview media | forecolor backcolor",
        image_advtab: true,
        height: "200px",
        width: "auto"
    };
    $scope.file;
    $scope.filesTChanged = function (elm) {
        $scope.file = elm.files;
    };
    $rootScope.fileuploadconfig(24);
    $scope.currentPeserta = [];
    $scope.evaluasi;
    $scope.selectedRekanan;


    $scope.init = function () {
        $http.post($rootScope.url_api + 'getsummary', {
            sessionID: $cookieStore.get('sessId'),
            flow_paket_id: item.flow_paket_id
        }).success(function (reply2) {
            if (reply2.result.data.length > 0) {
                $scope.evaluasi = reply2.result.data[0];
                //tanggal_evaluasi, tanggal_upload, summary, skor, peringkat, nomor_ba
                $scope.evaluasiHargaPenawaran = new evaluasiHargaPenawaran($scope.evaluasi.tgl_evaluasi, $scope.evaluasi.tgl_upload, $scope.evaluasi.summary, "", "", $scope.evaluasi.nomor_ba);
                tinymce.get('berita_acara_content').setContent($scope.evaluasi.summary);
            }
        });
        $http.post($rootScope.url_api + 'evaluasipenawaran/select', {
            sessionID: $cookieStore.get('sessId'),
            flow_paket_id: item.flow_paket_id,
            paket_lelang_id: item.paket_lelang_id
        }).success(function (reply) {
            $scope.temp = reply.result.data;
//            console.info(JSON.stringify($scope.temp));
//            for (var i = 0; i < $scope.temp.length; i++) {
//                $scope.currentPeserta.push($scope.temp[i]);
//            }
            for (var i = 0; i < $scope.temp.length; i++) {
                $scope.temp[i].peringkat = $scope.temp[i].peringkat + "";
                $scope.temp[i].skor = Number($scope.temp[i].skor);
                $scope.temp[i].lolos = $scope.temp[i].lolos === "1";
            }
        });
    };
    $scope.change = function (obj) {
        $scope.selectedRekanan = obj;
        for (var i = 0; i < listSkor.length; i++) {
            if (obj.rekanan_id == listSkor[i].rekanan_id) {
                $scope.evaluasiHargaPenawaran.skor = listSkor[i].skor;
                break;
            }
        }
    };

    $scope.stringSelectedHasilEvaluasi = "";
    $scope.lolos = false;
    $scope.changeLolos = function (obj, idx) {
        $scope.temp.sort(function (a, b) {
            if (a.lolos && !b.lolos)
                return -1;
            else if (!a.lolos && b.lolos)
                return 1;
            else {
                return b.skor - a.skor;
            }
        });
        if (obj.lolos) {
            for (var i = 0; i < listSkor.length; i++) {
                if (obj.rekanan_id === listSkor[i].rekanan_id) {
                    $scope.temp[idx].skor = listSkor[i].skor;
                    break;
                }
            }
        }
//        $scope.$apply;
//        $scope.stringSelectedHasilEvaluasi = obj;
//        if (obj === '0') {
//            $scope.lolos = false;
//            $scope.evaluasiHargaPenawaran.peringkat = "";
//            $scope.evaluasiHargaPenawaran.skor = "";
//        }
//        else if (obj === '1')
//            $scope.lolos = true;
    };

    Number.prototype.format = function (n, x, s, c) {
        var re = '\\d(?=(\\d{' + (x || 3) + '})+' + (n > 0 ? '\\D' : '$') + ')',
                num = this.toFixed(Math.max(0, ~~n));

        return (c ? num.replace('.', c) : num).replace(new RegExp(re, 'g'), '$&' + (s || ','));
    };

    function generate() {
        var list = [];
        for (var i = 0; i < listSkor.length; i++) {
            for (var j = 0; j < $scope.temp.length; j++) {
                if ((listSkor[i].rekanan_id == $scope.temp[j].rekanan_id) && ($scope.temp[j].lolos)) {
                    list.push(listSkor[i]);
                }
            }
        }         ////console.info('list.length ' + list.length);
        for (var i = 0; i < list.length; i++) {
            var harga_total_paket = 0;
            var obj = {};
            for (var j = 0; j < list[i].hasil.length; j++) {
                //                ////console.info('harga total ' + parseFloat(listSkor[i].hasil[j].total));
                harga_total_paket += parseFloat(list[i].hasil[j].total);
            }
            obj = {rekanan_id: list[i].rekanan_id,
                nama_perusahaan: list[i].nama_perusahaan,
                harga_total_paket: harga_total_paket};
            sortedHarga.push(obj);
        }

        sortedHarga.sort(function (a, b) {
            return parseFloat(a.harga_total_paket) - parseFloat(b.harga_total_paket);
        });


        for (var i = 0; i < sortedHarga.length; i++) {
            if (i == (sortedHarga.length - 1)) {
                nama_rekanan += sortedHarga[i].nama_perusahaan;
            } else {
                nama_rekanan += sortedHarga[i].nama_perusahaan + ', ';
            }
        }

        $http.post($rootScope.url_api + 'template/select', {
            sessionID: $cookieStore.get('sessId'),
            type: "evaluasi_harga"
        }).success(function (reply) {
            if (reply.status == 200) {
                template = reply.result[0].isi_template;
                $http.post($rootScope.url_api + 'template/select', {
                    sessionID: $cookieStore.get('sessId'),
                    type: "detail_evaluasi_harga"
                }).success(function (replya) {
                    if (replya.status == 200) {
                        getData(replya, 0);

                    }
                });

            }
        });

    }

    function getData(replya, i) {
        ////console.info((i + 1) + '. romawinya adlah ' + romanize(i + 1));
        var roman = '';
        roman = romanize(i + 1);
        var sorted = '';
        sorted = sortedHarga[i];
        var sta = $scope.evaluasiHargaPenawaran.tanggal_evaluasi.split(/(\s+)/);
        var stu = sta[0].split('-');
        var wMonths = ['Januari', 'Februari', 'Maret', 'April', 'Mei', 'Juni', 'Juli', 'Agustus', 'September', 'Oktober', 'November', 'Desember'];

        var dtlTemp = '';
        //itp.rekanan.getBiodata
        $http.post($rootScope.url_api + 'rekanan/getdatarekanan', {
            sessionID: $cookieStore.get('sessId'),
            rekanan_id: sorted.rekanan_id
        }).success(function (reply2) {
            ////console.info('reply2 ' + JSON.stringify(reply2));
            if (reply2.status == 200) {
                alamatRekanan = reply2.result.data[0].alamat;
                $http.post($rootScope.url_api + 'evaluasiharga/getpembulatan', {
                    sessionID: $cookieStore.get('sessId'),
                    paket_lelang_id: paket_lelang_id,
                    rekanan_id: sorted.rekanan_id
                }).success(function (reply3) {
                    ////console.info('reply3 ' + JSON.stringify(reply3));
                    if (reply3.result.length > 0) {
                        hargaPembulatan = reply3.result[0].harga;
                    } else {
                        hargaPembulatan = 0;
                    }
                    dtlTemp = replya.result[0].isi_template;
                    dtlTemp = dtlTemp
                            .replace('#penawar_ke', roman)
                            .replace('#rekanan_tawar_terendah', sorted.nama_perusahaan)
                            .replace('#alamat_rekanan_tawar_terendah', alamatRekanan)
                            .replace('#harga_tawar_terendah', sorted.harga_total_paket.format(2, 3, '.', ','))
                            .replace('#harga_pembulatan', hargaPembulatan.format(2, 3, '.', ','))
                            .replace('#nomor_ba', $scope.evaluasiHargaPenawaran.nomor_ba)
                            .replace('#tanggal_kata_bulan', (stu[2] + ' ' + wMonths[parseInt(stu[1]) - 1] + ' ' + stu[0]));

                    dtlTemplate += dtlTemp;
                    i++;
                    if (i < sortedHarga.length) {
                        getData(replya, i);
                    } else {
                        setTemplate();
                    }
                });
            }
        });
        //        ////console.info(dtlTemplate);
        //        return dtlTemplate;
    }

    function setTemplate() {
        var sta = $scope.evaluasiHargaPenawaran.tanggal_evaluasi.split(/(\s+)/);
        var stu = sta[0].split('-');
        var wMonths = ['Januari', 'Februari', 'Maret', 'April', 'Mei', 'Juni', 'Juli', 'Agustus', 'September', 'Oktober', 'November', 'Desember'];


        console.info("hps:"+nilai_hps);
        template = template
                .replace('#upper_tahapan', nama_tahapan.toUpperCase())
                .replace(/#nomor_ba/g, $scope.evaluasiHargaPenawaran.nomor_ba)
                .replace('#tahapan', nama_tahapan)
                .replace('#hari', getDayName(stu[0] + '-' + stu[1] + '-' + stu[2]))
                .replace('#tanggal_kata', toWords(stu[2]) + " " + wMonths[parseInt(stu[1]) - 1] + " " + toWords(stu[0]))
                .replace(/#pekerjaan/g, nama_paket)
                .replace('#tanggal', (stu[2] + '-' + stu[1] + '-' + stu[0]))
                .replace('#jml_lolos_kata', toWords(sortedHarga.length))
                .replace('#jml_lolos', sortedHarga.length)
                .replace('#rekanan_lolos', nama_rekanan)
                .replace('#nilai_hps', nilai_hps.format(2, 3, '.', ','))
                .replace('#detail_evaluasi_harga', dtlTemplate)
                .replace('#rekanan_tawar_terendah', sortedHarga[0].nama_perusahaan);
        //                    $scope.berita_acara = new berita_acara(template, "", $scope.berita_acara.nomor_ba);         $scope.evaluasiHargaPenawaran.summary = template;
        tinymce.get('berita_acara_content').setContent(template);
        
    }


    $scope.generateBeritaAcara = function () {
        if ($scope.temp.length === 0) {
            $.growl.error({title: "[PERINGATAN]", message: 'pilih rekanan terlebih dahulu'});
            return;
        }
        if ($scope.evaluasiHargaPenawaran.nomor_ba === '') {
            $.growl.error({title: "[PERINGATAN]", message: 'input nomor berita acara terlebih dahulu'});
            return;
        }
        if ($scope.evaluasiHargaPenawaran.tanggal_evaluasi === '') {
            $scope.evaluasiHargaPenawaran.tanggal_evaluasi = $rootScope.currentDate; //DARI HENDRA
            /*$.growl.error({title: "[PERINGATAN]", message: 'input tanggal evaluasi terlebih dahulu'});
             return;*/
        }
//        if ($scope.berita_acara.nomor_ba !== '' && $scope.berita_acara.tanggal_berita_acara !== '' && $scope.listPengurusTerpilih.length >= 0 && $scope.berita_acara.tgl_mulai !== '') {
//            ////console.info('tanggal ' + $scope.berita_acara.tanggal_berita_acara);
        generate();
//        }
    };

    $scope.temp = [];
    $scope.tambahPerusahaan = function () {
        if ($scope.selectedRekanan === "" || $scope.selectedRekanan === null || $scope.selectedRekanan === undefined) {
            $.growl.error({title: "[PERINGATAN]", message: "Rekanan belum dipilih"});
            return;
        }
        if ($scope.stringSelectedHasilEvaluasi === '') {
            $.growl.error({title: "[PERINGATAN]", message: "Lolos tidaknya belum ditentukan"});
            return;
        }
        if ($scope.lolos === true && $scope.evaluasiHargaPenawaran.skor === '') {
            $.growl.error({title: "[PERINGATAN]", message: "Skor belum dimasukkan"});
            return;
        }
        if ($scope.lolos === true && $scope.evaluasiHargaPenawaran.peringkat === '') {
            $.growl.error({title: "[PERINGATAN]", message: "Peringkat belum dimasukkan"});
            return;
        }

        var sudahAda = false;
        for (var i = 0; i < $scope.temp.length; i++)
        {
            if ($scope.temp[i].rekanan_id === $scope.selectedRekanan.rekanan_id)
            {
                sudahAda = true;
                break;
            }
        }
        if (sudahAda) {
            $.growl.error({title: "[PERINGATAN]", message: "Rekanan sudah dipilih"});
            return;
        }
        var peringkatSudahAda = false;
        for (var i = 0; i < $scope.temp.length; i++)
        {
            if ($scope.lolos == true && $scope.temp[i].peringkat == $scope.evaluasiHargaPenawaran.peringkat)
            {
                peringkatSudahAda = true;
                break;
            }
        }
        if (peringkatSudahAda) {
            $.growl.error({title: "[PERINGATAN]", message: "Peringkat sudah ada"});
            return;
        } else {
            $scope.temp.push({
                rekanan_id: $scope.selectedRekanan.rekanan_id,
                nama_perusahaan: $scope.selectedRekanan.nama_perusahaan,
                lolos: $scope.lolos,
                peringkat: $scope.evaluasiHargaPenawaran.peringkat,
                skor: $scope.evaluasiHargaPenawaran.skor,
                evaluasi_penawaran_id: $scope.selectedRekanan.evaluasi_penawaran_id
            });
        }
    };

    $scope.hapusData = function (rekanan_id) {
        var idx = -1;
        for (var i = 0; i < $scope.temp.length; i++)
        {
            if ($scope.temp[i].rekanan_id === rekanan_id)
            {
                idx = i;
                break;
            }
        }
        $scope.temp.splice(idx, 1);
    };

    $scope.simpan = function () {
        if ($scope.evaluasiHargaPenawaran.tanggal_evaluasi === '') {
            $.growl.error({title: "[PERINGATAN]", message: "Tanggal evaluasi belum dimasukkan"});
            return;
        }
        if ($scope.evaluasiHargaPenawaran.nomor_ba === '') {
            $.growl.error({title: "[PERINGATAN]", message: "Nomor BA belum dimasukkan"});
            return;
        }
        if ($scope.evaluasiHargaPenawaran.summary === '') {
            $.growl.error({title: "[PERINGATAN]", message: "Summary masih kosong"});
            return;
        }
        if ($scope.temp.length === 0) {
            $.growl.error({title: "[PERINGATAN]", message: "Belum ada rekanan yang dievaluasi"});
            return;
        }
        if ($scope.temp.length < $scope.currentPeserta.length) {
            $.growl.error({title: "[PERINGATAN]", message: "Ada rekanan yang belum dievaluasi"});
            return;
        }
        var skorValid = true;
        $scope.temp.forEach(function(t){
            if (t.lolos == true && !t.skor){
                skorValid = false;
            }
        });
        if(!skorValid){
            $.growl.error({title: "[PERINGATAN]", message: "Nilai Skor tidak valid"});
            return;
        }
//        for (var i = 0; i < $scope.temp.length; i++) {
//            if ($scope.temp[i].lolos == null) {
//                $.growl.error({title: "[PERINGATAN]", message: "Hasil evaluasi rekanan belum diisi"});
//                return;
//            }
//        }
        if ($scope.file === undefined) {
            update($scope.evaluasi.dokumen_file);
        } else {
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
                    if ($.inArray(extFile, restrictedExt) == -1) {
                        $rootScope.unloadLoadingModal();
                        $.growl.error({title: "[WARNING]", message: "Format file tidak valid"});
                        return;
                    } else {
                        upload();
                    }
                }
            }
        }
    };

//    function upload() {
//        uploaderSvc.uploadFile($scope.file, paket_lelang_id).then(function(url) {
//            if (!(url === 'failed')) {
//                update(url);
//            }
//            else {
//                $.growl.error({title: "[PERINGATAN]", message: "Upload gagal"});
//            }
//        });
//
//    }
//    ;
    function upload() {
        var fd = new FormData();
        angular.forEach($scope.file, function (file) {
            fd.append("uploads", file);
        });
        $http.post($rootScope.url_api + "/upload/pengadaan_" + paket_lelang_id + "/", fd, {
            withCredentials: true,
            transformRequest: angular.identity(),
            headers: {'Content-Type': undefined}
        })
                .success(function (reply) {
                    if (reply.status === 200) {
                        var result = reply.result.data;
                        update(result.files[0].url);
//                        insertBlacklist(result.files[0].url);
                    } else {
                        $.growl.error({message: "Gagal mendapatkan data hak akses"});
                        return;
                    }
                })
                .error(function (err) {
                    $.growl.error({message: "Gagal Akses API >" + err});
                    return;
                });
    }
    ;


    function update(url) {
        var master = [];
        master.push($scope.evaluasiHargaPenawaran.tanggal_evaluasi);
        master.push($scope.evaluasiHargaPenawaran.tanggal_upload);
        master.push($scope.evaluasiHargaPenawaran.nomor_ba);
        master.push(url);
        master.push($scope.evaluasiHargaPenawaran.summary);
        master.push(flow_paket_id);
        //master.push(paket_lelang_id);
        var details = [];
        var detail;
        for (var i = 0; i < $scope.temp.length; i++) {
            detail = [];
            //detail.push($scope.temp[i].lolos);
            if ($scope.temp[i].lolos === true) {
                detail.push(1);
            } else {
                detail.push(0);
                //detail.push(urutan);
            }
            //detail.push(paket_lelang_id);
            detail.push(Number($scope.temp[i].peringkat));
            detail.push(parseFloat($scope.temp[i].skor));
            //detail.push(flow_paket_id);
            detail.push($scope.temp[i].evaluasi_penawaran_id);
            detail.push($scope.temp[i].rekanan_id);
            details.push(detail);
        }
//        $('#ubahEvaluasi').block({
//            message: '<div style="text-align:center;"><h4>Silakan tunggu..</h4></div>',
//            css: {border: '3px solid #a00'}
//        });
        $rootScope.loadLoadingModal("Silahkan Tunggu...");
        $http.post($rootScope.url_api + 'evaluasiharga/update', {
            sessionID: $cookieStore.get('sessId'),
            paket_lelang_id: paket_lelang_id,
            flow_paket_id: flow_paket_id,
            master: master,
            detail: details,
            nama_paket: nama_paket,
            nama_tahapan: nama_tahapan,
            urutan: urutan,
            username: username
        }).success(function (reply) {
            if (reply.status == 200) {
//                $('#ubahEvaluasi').unblock();
                $rootScope.unloadLoadingModal();
                $.growl.notice({title: "[INFO]", message: "Berhasil mengubah data evaluasi penawaran"});
                $modalInstance.close();
            } else {
                $('#ubahEvaluasi').unblock();
                $rootScope.unloadLoadingModal();
                $.growl.error({title: "[PERINGATAN]", message: "Gagal mengubah data evaluasi penawaran"});
            }
        });
    }
    ;

    function getDayName(date) {
        var d = new Date(date);
        var weekday = new Array(7);
        weekday[0] = "Minggu";
        weekday[1] = "Senin";
        weekday[2] = "Selasa";
        weekday[3] = "Rabu";
        weekday[4] = "Kamis";
        weekday[5] = "Jum'at";
        weekday[6] = "Sabtu";

        return weekday[d.getDay()];
    }

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


    function toWords(s) {
        // Convert numbers to words
// copyright 25th July 2006, by Stephen Chapman http://javascript.about.com
// permission to use this Javascript on your web page is granted
// provided that all of the code (including this copyright notice) is
// used exactly as shown (you can change the numbering system if you wish)

// American Numbering System
        var th = ['', 'Ribu', 'juta', 'milyar', 'triliun'];
// uncomment this line for English Number System
// var th = ['','thousand','million', 'milliard','billion'];

        var dg = ['nol', 'satu', 'dua', 'tiga', 'empat', 'lima', 'enam', 'tujuh', 'delapan', 'sembilan'];
        var tn = ['sepuluh', 'sebelas', 'dua belas', 'tiga belas', 'empat belas', 'lima belas', 'enam belas', 'tujuh belas', 'delapan belas', 'sembilan belas'];
        var tw = ['dua puluh', 'tiga puluh', 'empat puluh', 'lima puluh', 'enam puluh', 'tujuh puluh', 'delapan puluh', 'sembilan puluh'];
        s = s.toString();
        s = s.replace(/[\, ]/g, '');
        if (s != parseFloat(s))
            return 'not a number';
        var x = s.indexOf('.');
        if (x == -1)
            x = s.length;
        if (x > 15)
            return 'too big';
        var n = s.split('');
        var str = '';
        var sk = 0;
        for (var i = 0; i < x; i++) {
            if ((x - i) % 3 == 2) {
                if (n[i] == '1') {
                    str += tn[Number(n[i + 1])] + ' ';
                    i++;
                    sk = 1;
                } else if (n[i] != 0) {
                    str += tw[n[i] - 2] + ' ';
                    sk = 1;
                }
            } else if (n[i] != 0) {
                str += dg[n[i]] + ' ';
                if ((x - i) % 3 == 0)
                    str += 'ratus ';
                if (n[i] == '1')
                    str += '';
                sk = 1;
            }
            if ((x - i) % 3 == 1) {
                if (sk)
                    str += th[(x - i - 1) / 3] + ' ';
                sk = 0;
            }
        }
        if (x != s.length) {
            var y = s.length;
            str += 'point ';
            for (var i = x + 1; i < y; i++)
                str += dg[n[i]] + ' ';
        }
        return str.replace(/\s+/g, ' ');
    }

    $scope.batal = function () {
        $modalInstance.dismiss('cancel');
    };
};

function evaluasiHargaPenawaran(tanggal_evaluasi, tanggal_upload, summary, skor, peringkat, nomor_ba) {
    var self = this;
    self.tanggal_evaluasi = tanggal_evaluasi;
    self.tanggal_upload = tanggal_upload;
    self.summary = summary;
    self.skor = skor;
    self.peringkat = peringkat;
    self.nomor_ba = nomor_ba;
}
