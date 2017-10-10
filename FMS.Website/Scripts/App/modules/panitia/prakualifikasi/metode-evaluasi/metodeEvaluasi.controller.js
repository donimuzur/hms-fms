(function () {
    'use strict';

    angular.module("app")
    .controller("listMetodeEvaluasiPrakualCtrl", ctrl);
    
    ctrl.$inject = ['$state', '$http', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'MetodeEvaluasiPrakualService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $http, $uibModal, $translate, $translatePartialLoader, $location, SocketService, metodeEvaluasiPrakualService, UIControlService) {
        var vm = this;
	    vm.totalItems = 0;
        vm.currentPage = 1;
        vm.maxSize = 10;
        vm.srcText = "";
        var srcText = "";
        var lang;
        //TODO
        vm.userBisaMengatur = true;
        vm.userBisaMenambah = true;
        vm.userBisaMengubah = true;
        vm.userBisaMenghapus = true;
        vm.searchBy = 0;
        vm.metodeEvaluasi = [];
        vm.menuhome = 0;
        vm.page_id = 140;
        
        //functions
        vm.init = init;
        vm.loadAwal = loadAwal;
        vm.cari = cari;
        vm.cek_authorize = cek_authorize;
        vm.loadMetodeEvaluasi = loadMetodeEvaluasi;
        vm.ubah_aktif = ubah_aktif;
        vm.jLoad = jLoad;
        vm.ubahDetail = ubahDetail;
        vm.lihatDetail = lihatDetail;
        vm.addMetodeEvaluasi = addMetodeEvaluasi;
        
        function init() {
//            vm.menuhome = $rootScope.menuhome;
//            $rootScope.getSession().then(function(result) {
//                $rootScope.userSession = result.data.data;
//                $rootScope.userLogin = $rootScope.userSession.session_data.username;
//                $rootScope.authorize(new function() {
//                    vm.loadAwal();
//                });
//            });
            vm.loadAwal();
            lang = $translate.use();
        };
        function cek_authorize(action) {
            $rootScope.authorize(action);
        };
            
        function loadAwal() {
            vm.loadMetodeEvaluasi();
        } // end loadAwal
        
        function cari() {
            srcText = vm.srcText;
            vm.currentPage = 1;
            vm.loadMetodeEvaluasi();
        };

        function loadMetodeEvaluasi() {
            UIControlService.loadLoading('Silahkan Tunggu...');
            metodeEvaluasiPrakualService.count({
                keyword: srcText
            }, function(reply) {
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.totalItems = data;
                    jLoad();
                }
                else {
                    UIControlService.msg_growl("error", "Gagal mendapatkan data metode evaluasi");
                    UIControlService.unloadLoading();
                }
            }, function(err) {
                UIControlService.msg_growl("error", "");
                UIControlService.unloadLoading();
            });
        };
        
        function jLoad() {
            var offset = (vm.currentPage - 1) * vm.maxSize;
            metodeEvaluasiPrakualService.select({
                offset: offset,
                limit: vm.maxSize,
                keyword: srcText
            }, function(reply) {
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.metodeEvaluasi = data;
                }
                else {
                    UIControlService.msg_growl("error", "Gagal mendapatkan data metode evaluasi!!");
                }
                UIControlService.unloadLoading();
            }, function(err) {
                UIControlService.msg_growl("error", "Gagal Akses API");
                UIControlService.unloadLoading();
            });
        };
        
        function addMetodeEvaluasi(){
            $state.transitionTo('tambah-metode-prakual', {id:0});
        }
        
        function ubah_aktif(metode) {
            UIControlService.loadLoading('Silahkan Tunggu...');
            metodeEvaluasiPrakualService.isUsed({
                EvaluationMethodId: metode.EvaluationMethodId
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.data === true) {
                    UIControlService.msg_growl('warning', 'Metode evaluasi ini sudah digunakan di pengadaan yang sudah berjalan');
                } else {
                    var pesan = "";

                    switch (lang) {
                        case 'id': pesan = 'Anda yakin untuk ' + (metode.IsActive ? 'menonaktifkan' : 'mengaktifkan') + ' Metode Evaluasi "' + metode.EvaluationMethodName + '"?'; break;
                        default: pesan = 'Are you sure want to ' + (metode.IsActive ? 'aktivate' : 'deactivate') + ' this Evaluation Method : "' + metode.EvaluationMethodName + '" ?'; break;
                    }

                    bootbox.confirm(pesan, function (yes) {
                        if (yes) {
                            UIControlService.loadLoading('Silahkan Tunggu...');
                            metodeEvaluasiPrakualService.switchActive({
                                EvaluationMethodId: metode.EvaluationMethodId,
                            }, function (reply) {
                                UIControlService.unloadLoading();
                                if (reply.status === 200) {
                                    UIControlService.msg_growl("notice", "Berhasil mengaktifkan/menonaktifkan Metode Evaluasi");
                                    loadMetodeEvaluasi();
                                } else {
                                    UIControlService.msg_growl("error", "Gagal mengaktifkan/menonaktifkan Metode Evaluasi");
                                }
                            }, function (err) {
                                UIControlService.unloadLoading();
                                UIControlService.msg_growl("error", "Gagal mengakses API");
                            });
                        }
                    })
                }
            });
        };
        
        function ubahDetail(metode_id) {
            //cek apakah metode evaluasi ini sudah digunakan untuk Pengadaan
            UIControlService.loadLoading('Silahkan Tunggu...');
            metodeEvaluasiPrakualService.isUsed({
                EvaluationMethodId: metode_id
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status == 200) {
                    if (reply.data === false) {
                        $state.transitionTo('tambah-metode-prakual', { id: metode_id });
                    } else {
                        UIControlService.msg_growl('warning', 'Metode evaluasi ini sudah digunakan di pengadaan yang sudah berjalan');
                        /*
                        var modalInstance = $uibModal.open({
                            templateUrl: 'warningUbahMetodeEvaluasi.html',
                            controller: warningUbahMetodeEvaluasiCtrl
                        });
                        modalInstance.result.then(function() {
                            $state.transitionTo('detail-metode-evaluasi', {metode_id: metode_id});
                        });
                        */
                    }
                }
            }, function (err) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl('error', 'Gagal Akses API');
            });
        };
        
        function lihatDetail(metode_id) {
            var kirim = {
                metode_evaluasi_id: metode_id
            };
            $uibModal.open({
                templateUrl: 'app/modules/panitia/prakualifikasi/metode-evaluasi/metodeEvaluasi.modal.html',
                controller: 'modalDetailMetodeEvaluasiCtrl',
                controllerAs: 'modalDMECtrl',
                resolve: {
                    item: function() {
                        return kirim;
                    }
                }
            });
        };
    }
    
    function Kata(srcText) {
        var self = this;
        self.srcText = srcText;
    }
})();

////coding lama
//angular.module('eprocAppPanitia')
//        .controller('listMetodeEvaluasiPrakualCtrl', function(vm, $http, $rootScope, $modal, $state, $cookieStore) {
//            
//        }) // end listMetodeEvaluasiCtrl
//
//        .controller('tambahMetodePrakualCtrl', function(vm, $http, $rootScope, $state, $cookieStore) {
//            
//        })
//        .controller('detailMetodeEvaluasiCtrl', function(vm, $http, $rootScope, $state, $stateParams, $cookieStore, $modal) {
//
//            vm.ubahDetailLevel0 = function() {
//                
//            };
//        })
//        .controller('metodeEvaluasiLevel1Ctrl', function(vm, $http, $rootScope, $stateParams, $state, $cookieStore, $modal) {
//            vm.sudahMengaturLevel1;
//            vm.userBisaMengatur = false;
//            vm.kriteria = [];
//            vm.namaMetode;
//            vm.menuhome = 0;
//            var metode_evaluasi_id;
//            vm.metode_evaluasi_id;
//            var page_id = 140;
//            vm.page_id = 140;
//            var med_id = Number($stateParams.med_id);
//            vm.med_id = Number($stateParams.med_id);
//            vm.obj = {};
//
////            eb.onopen = function() {
////                $rootScope.autorize();
////                eb.send('itp.authManager.authorise', {
////                    sessionID: $cookieStore.get('sessId')
////                }, function(reply) {
////                    if (reply.status === 'ok') {
////                        $rootScope.userlogged = reply.username;
////                        vm.init();
////                        vm.$apply();
////                    }
////                });
////            };
//
//            vm.init = function() {
//                $rootScope.getSession().then(function(result) {
//                    $rootScope.userSession = result.data.data;
//                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
//                    $rootScope.authorize(vm.initialize());
//                });
//            };
//
//            vm.initialize = function() {
//                //$rootScope.refreshWaktu();
//                vm.menuhome = $rootScope.menuhome;
//                //$rootScope.isLogged = true;
//                var param = [];
//                param.push($rootScope.userlogged);
//                param.push(page_id);
//                $http.post($rootScope.url_api + "roles/check_authority",
//                        {username: $rootScope.userLogin, page_id: vm.page_id, jenis_mengatur: 1})
//                        .success(function(reply) {
//                            if (reply.status === 200) {
//                                var data = reply.result.data[0];
//                                vm.userBisaMengatur = data.bisa_mengatur;
//                                vm.loadLevel1();
//                            }
//                            else {
//                                $.growl.error({message: "Gagal mendapatkan data hak akses!"});
//                                return;
//                            }
//                        })
//                        .error(function(err) {
//                            $.growl.error({message: "Gagal Akses API >" + err});
//                            return;
//                        });
//
//                /*$http.post($rootScope.url_api+'itp.role.cekBisaMengatur', {
//                 param: param,
//                 page_id: page_id
//                 }) .success(function(reply) {
//                 if (reply.status === 'ok') {
//                 if (reply.result.length > 0) {
//                 vm.userBisaMengatur = reply.result[0].bisa_mengatur;
//                 }
//                 vm.loadLevel1();
//                 vm.$apply();
//                 }
//                 });*/
//            };
//
//            vm.expanding_property = {
//                field: "kriteria_nama",
//                displayName: "Nama Kriteria Evaluasi",
//                sortable: true,
//                sortingType: "string",
//                filterable: true
//            };
//
//            vm.col_defs = [
//                {
//                    field: "bobot",
//                    displayName: "Bobot (%)",
//                    cellTemplate: '<a style="width: 10%"> {{row.branch[col.field]}}&nbsp; % </a> ',
//                    sortable: true,
//                    sortingType: "number",
//                    filterable: true
//                },
//                {
//                    field: "obj",
//                    displayName: "  ",
//                    cellTemplate: '<a ng-show="row.branch[col.field].bisaNgatur == true && row.branch[col.field].level < 3" class="btn btn-flat btn-xs btn-primary" ng-click="cellTemplateScope.click(row.branch[col.field])"><i class="fa fa-plus-circle"></i>&nbsp; Ubah Sub Kriteria</a>',
//                    cellTemplateScope: {
//                        click: function(data) {         // this works too: vm.someMethod;
//                            vm.tambahDetailLevel1(data);
//                        }
//                    }
//                }
////                {
////                    field: "obj",
////                    displayName: "  ",
////                    cellTemplate: '<a ng-show="row.branch[col.field].bisaNgatur" ng-click="cellTemplateScope.click(row.branch[col.field])" tittle="edit" class="btn btn-flat btn-xs btn-default"><i class="fa fa-edit"></i>&nbsp; Ubah</a>',
////                    cellTemplateScope: {
////                        click: function(data) {         // this works too: vm.someMethod;
////                            vm.ubahDetailLevel1(data);
////                        }
////                    }
////                }
//            ];
//
//            vm.loadLevel1 = function() {
//                vm.kriteria = [];
//                //console.info('masuk loadlevel1');
////                $('#level1').block({
////                    message: '<div style="text-align:center;"><h4>Silakan tunggu..</h4></div>',
////                    css: {border: '3px solid #a00'}
////                });
//                $rootScope.loadLoading("Silahkan Tunggu...");
//                $http.post($rootScope.url_api + 'metodeEvaluasi/getNamaMetodeBasedOnMed', {
//                    med_id: med_id
//                }).success(function(reply) {
//                    vm.namaMetode = reply.result.data.nama;
//                    metode_evaluasi_id = reply.result.data.id;
//                    vm.metode_evaluasi_id = reply.result.data.id;
//                });
//                //cek sudah diatur apa belum
//                $http.post($rootScope.url_api + 'metodeEvaluasi/sudahMengaturLevel', {
//                    med_id: med_id,
//                    level: 1,
//                    parent: 0
//                }).success(function(reply) {
//                    if (reply.status === 200) {
//                        vm.sudahMengaturLevel1 = reply.result.data.hasil;
//                        if (vm.sudahMengaturLevel1 == false) {
//                            $http.post($rootScope.url_api + 'kriteriaevaluasi/selectformetode', {
//                                level: 1,
//                                parent: 0
//                            }).success(function(reply2) {
//                                if (reply2.status === 200) {
//                                    vm.kriteria = reply2.result.data;
//                                    for (var i = 0; i < vm.kriteria.length; i++) {
//                                        vm.kriteria[i].checked = false;
//                                        vm.kriteria[i].bobot = "";
//                                    }
//                                }
//                                $rootScope.unloadLoading();
//                            });
//                        }
//                        else if (vm.sudahMengaturLevel1 == true) {
//                            $http.post($rootScope.url_api + 'medkriteria/select', {
//                                med_id: med_id,
//                                level: 1,
//                                parent: 0
//                            }).success(function(reply2) {
//                                if (reply2.status === 200) {
//                                    if (reply2.result.data.length > 0) {
////                                        vm.obj = reply.result[0];
//                                        reply2.result.data.forEach(function(obj) {
//                                            obj.children = [];
//                                            obj.bisaNgatur = vm.userBisaMengatur;
//                                            vm.kriteria.push({
//                                                "med_kriteria_id": obj.med_kriteria_id,
//                                                "metode_evaluasi_id": obj.metode_evaluasi_id,
//                                                "metode_evaluasi_nm": obj.metode_evaluasi_nm,
//                                                "med_id": obj.med_id,
//                                                "jenis_detail": obj.jenis_detail,
//                                                "kriteria_id": obj.kriteria_id,
//                                                "kriteria_nama": obj.kriteria_nama,
//                                                "is_mandatory": obj.is_mandatory,
//                                                "bobot": obj.bobot,
//                                                "level": obj.level,
//                                                "is_active": obj.is_active,
//                                                "parent": obj.parent,
//                                                "status_parent": obj.status_parent,
//                                                "grandparent_id": obj.grandparent_id,
//                                                "status_grandparent": obj.status_grandparent,
//                                                "children": obj.children,
//                                                "obj": obj,
//                                                "bisaNgatur": obj.bisaNgatur
//                                            });
//                                            vm.selectSubKriteria(obj);
//                                        });
//                                    }
//                                }
//                                $rootScope.unloadLoading();
//                            });
//                        }
//                    }
//                });
//            };
//
//            vm.selectSubKriteria = function(object) {
//                $http.post($rootScope.url_api + 'medkriteria/select', {
//                    med_id: med_id,
//                    level: Number(object.level) + 1,
//                    parent: object.kriteria_id
//                }).success(function(reply) {
//                    if (reply.status === 200) {
//                        if (reply.result.data.length > 0) {
//                            reply.result.data.forEach(function(objChild) {
//                                objChild.children = [];
//                                objChild.bisaNgatur = vm.userBisaMengatur;
//                                vm.kriteria.forEach(function(objParent) {
//                                    if (objChild.parent == objParent.kriteria_id) {
//                                        objChild.children = [];
//                                        objChild.bisaNgatur = vm.userBisaMengatur;
//                                        objParent.children.push({
//                                            "med_kriteria_id": objChild.med_kriteria_id,
//                                            "metode_evaluasi_id": objChild.metode_evaluasi_id,
//                                            "metode_evaluasi_nm": objChild.metode_evaluasi_nm,
//                                            "med_id": objChild.med_id,
//                                            "jenis_detail": objChild.jenis_detail,
//                                            "kriteria_id": objChild.kriteria_id,
//                                            "kriteria_nama": objChild.kriteria_nama,
//                                            "is_mandatory": objChild.is_mandatory,
//                                            "bobot": objChild.bobot,
//                                            "level": objChild.level,
//                                            "is_active": objChild.is_active,
//                                            "parent": objChild.parent,
//                                            "status_parent": objChild.status_parent,
//                                            "grandparent_id": objChild.grandparent_id,
//                                            "status_grandparent": objChild.status_grandparent,
//                                            "children": objChild.children,
//                                            "obj": objChild,
//                                            "bisaNgatur": objChild.bisaNgatur
//                                        });
//                                    }
//                                });
//                                $http.post($rootScope.url_api + 'medkriteria/select', {
//                                    sessionID: $cookieStore.get('sessId'),
//                                    med_id: med_id,
//                                    level: Number(objChild.level) + 1,
//                                    parent: objChild.kriteria_id
//                                }).success(function(reply2) {
//                                    if (reply2.status === 200) {
//                                        if (reply2.result.data.length > 0) {
//                                            reply2.result.data.forEach(function(objGrndChild) {
//                                                objGrndChild.children = [];
//                                                objGrndChild.bisaNgatur = vm.userBisaMengatur;
//                                                for (var i = 0; i < vm.kriteria.length; i++) {
//                                                    if (vm.kriteria[i].children.length > 0) {
//                                                        for (var j = 0; j < vm.kriteria[i].children.length; j++) {
//                                                            if (objGrndChild.parent == vm.kriteria[i].children[j].kriteria_id) {
//                                                                vm.kriteria[i].children[j].children.push({
//                                                                    "med_kriteria_id": objGrndChild.med_kriteria_id,
//                                                                    "metode_evaluasi_id": objGrndChild.metode_evaluasi_id,
//                                                                    "metode_evaluasi_nm": objGrndChild.metode_evaluasi_nm,
//                                                                    "med_id": objGrndChild.med_id,
//                                                                    "jenis_detail": objGrndChild.jenis_detail,
//                                                                    "kriteria_id": objGrndChild.kriteria_id,
//                                                                    "kriteria_nama": objGrndChild.kriteria_nama,
//                                                                    "is_mandatory": objGrndChild.is_mandatory,
//                                                                    "bobot": objGrndChild.bobot,
//                                                                    "level": objGrndChild.level,
//                                                                    "is_active": objGrndChild.is_active,
//                                                                    "parent": objGrndChild.parent,
//                                                                    "status_parent": objGrndChild.status_parent,
//                                                                    "grandparent_id": objGrndChild.grandparent_id,
//                                                                    "status_grandparent": objGrndChild.status_grandparent,
//                                                                    "children": objGrndChild.children,
//                                                                    "obj": objGrndChild,
//                                                                    "bisaNgatur": objGrndChild.bisaNgatur
//                                                                });
//                                                            }
//                                                        }
//                                                    }
//                                                }
//                                            });
//
//                                        }
//                                    }
//                                });
//                            });
//                        }
//                    }
//                });
//            };
//
//            vm.simpan = function() {
//                if (vm.kriteria.length === 0) {
//                    $.growl.error({title: "[PERINGATAN]", message: "Tidak ada kriteria yang bisa dimasukkan"});
//                    return;
//                }
//                for (var i = 0; i < vm.kriteria.length; i++) {
//                    if (vm.kriteria[i].checked && vm.kriteria[i].bobot === "") {
//                        $.growl.error({title: "[PERINGATAN]", message: "Bobot harus diisi"});
//                        return;
//                    }
//                }
//                var totalPersentase = 0;
//                for (var i = 0; i < vm.kriteria.length; i++) {
//                    if (vm.kriteria[i].checked) {
//                        totalPersentase = totalPersentase + Number(vm.kriteria[i].bobot);
//                    }
//                }
//                if (totalPersentase !== 100) {
//                    $.growl.error({title: "[PERINGATAN]", message: "Total persentase tidak 100%"});
//                    return;
//                }
//                var detail = [];
//                var temp;
//                for (var i = 0; i < vm.kriteria.length; i++) {
//                    temp = [];
//                    if (vm.kriteria[i].checked) {
//                        temp.push(med_id);
//                        temp.push(vm.kriteria[i].kriteria_id);
//                        temp.push(vm.kriteria[i].level);
//                        temp.push(vm.kriteria[i].parent_id);
//                        temp.push(Number(vm.kriteria[i].bobot));
//                        //temp.push(Number(vm.kriteria[i].kriteria_nama));
//                        detail.push(temp);
//                    }
//                }
//                $rootScope.loadLoading("Silahkan Tunggu...");
//                $http.post($rootScope.url_api + 'metodeEvaluasi/aturLevel', {
//                    username: $rootScope.userLogin,
//                    detail: detail
//                }).success(function(reply) {
//                    if (reply.status === 200) {
//                        vm.kriteria = [];
//                        vm.loadLevel1();
//                        $.growl.notice({title: "[INFO]", message: "Berhasil mengatur level 1"});
//                        $rootScope.unloadLoading();
//
//                    }
//                    else {
//                        $.growl.error({title: "[WARNING]", message: "Gagal mengatur level 1"});
//                        $rootScope.unloadLoading();
//                    }
////                    vm.loadLevel1();
//                });
//            };
//
//            vm.keLevel2 = function(parent) {
//                $state.transitionTo('metode-evaluasi-level2', {med_id: med_id, parent: parent});
//            };
//
//            vm.back = function() {
//                $state.transitionTo('detail-metode-evaluasi', {metode_id: metode_evaluasi_id});
//            };
//
//            vm.ubahDetailLevel1 = function() {
////                var data = [];
////                if (obj.level === 1) {
////                    data = vm.kriteria;
////                } else if (obj.level === 2) {
////                    for (var i = 0; i < vm.kriteria.length; i++) {
////                        if (vm.kriteria[i].kriteria_id === obj.parent) {
////                            data = vm.kriteria[i].children;
////                        }
////                    }
////                } else {
////                    for (var i = 0; i < vm.kriteria.length; i++) {
////                        for (var j = 0; j < vm.kriteria[i].children.length; j++) {
////                            if (vm.kriteria[i].children[j].kriteria_id === obj.parent) {
////                                data = vm.kriteria[i].children[j].children;
////                            }
////                        }
////                    }
////                }
//                var lempar = {
//                    data: vm.kriteria,
//                    med_id: med_id,
//                    page_id: page_id,
//                    level: 1,
//                    parent: 0,
//                    nama: ''
//                };
//                var modalInstance = $modal.open({
//                    templateUrl: 'ubahDetailMetodeLevel1.html',
//                    controller: ubahDetailMetodeLevel1Ctrl,
//                    resolve: {
//                        item: function() {
//                            return lempar;
//                        }
//                    }
//                });
//                modalInstance.result.then(function() {
//                    vm.loadLevel1();
//                });
//            };
//
//            vm.tambahDetailLevel1 = function(obj) {
//                var data = [];
//                if (obj.level == 0) {
//                    data = vm.kriteria;
//                } else if (obj.level == 1) {
//                    for (var i = 0; i < vm.kriteria.length; i++) {
//                        if (vm.kriteria[i].kriteria_id == obj.kriteria_id) {
//                            data = vm.kriteria[i].children;
//                        }
//                    }
//                } else {
//                    for (var i = 0; i < vm.kriteria.length; i++) {
//                        for (var j = 0; j < vm.kriteria[i].children.length; j++) {
//                            if (vm.kriteria[i].children[j].kriteria_id == obj.kriteria_id) {
//                                data = vm.kriteria[i].children[j].children;
//                            }
//                        }
//                    }
//                }
//                var lempar = {
//                    data: data,
//                    med_id: med_id,
//                    page_id: page_id,
//                    level: Number(obj.level) + 1,
//                    parent: obj.kriteria_id,
//                    nama: obj.kriteria_nama
//                };
//                var modalInstance = $modal.open({
//                    templateUrl: 'ubahDetailMetodeLevel1.html',
//                    controller: ubahDetailMetodeLevel1Ctrl,
//                    resolve: {
//                        item: function() {
//                            return lempar;
//                        }
//                    }
//                });
//                modalInstance.result.then(function() {
//                    vm.loadLevel1();
//                });
//            };
//
//        })
//        .controller('metodeEvaluasiLevel2Ctrl', function(vm, $http, $rootScope, $state, $stateParams, $cookieStore, $modal) {
//            vm.userBisaMengatur = false;
//            vm.sudahMengaturLevel2;
//            vm.namaParent;
//            vm.menuhome = 0;
//            var page_id = 140;
//            vm.page_id = 140;
//            var med_id = Number($stateParams.med_id);
//            var parent = Number($stateParams.parent);
//            vm.med_id = Number($stateParams.med_id);
//
//
////            eb.onopen = function() {
////                $rootScope.autorize();
////                vm.init();
////            };
//
//            vm.init = function() {
//                $rootScope.getSession().then(function(result) {
//                    $rootScope.userSession = result.data.data;
//                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
//                    $rootScope.authorize(vm.initialize());
//                });
//            };
//
//            vm.initialize = function() {
//                $rootScope.refreshWaktu();
//                vm.menuhome = $rootScope.menuhome;
//                $rootScope.isLogged = true;
//                var param = [];
//                param.push($rootScope.userlogged);
//                param.push(page_id);
//
//                $http.post($rootScope.url_api + "roles/check_authority",
//                        {username: $rootScope.userLogin, page_id: vm.page_id, jenis_mengatur: 1})
//                        .success(function(reply) {
//                            if (reply.status === 200) {
//                                var data = reply.result.data[0];
//                                vm.userBisaMengatur = data.bisa_mengatur;
//                            }
//                            else {
//                                $.growl.error({message: "Gagal mendapatkan data hak akses!"});
//                                return;
//                            }
//                        })
//                        .error(function(err) {
//                            $.growl.error({message: "Gagal Akses API >" + err});
//                            return;
//                        });
//                vm.loadLevel2();
//
//                /* eb.send('itp.role.cekBisaMengatur', {
//                 sessionID: $cookieStore.get('sessId'),
//                 param: param,
//                 page_id: page_id
//                 }, function(reply) {
//                 if (reply.status === 'ok') {
//                 if (reply.result.length > 0) {
//                 vm.userBisaMengatur = reply.result[0].bisa_mengatur;
//                 }
//                 vm.$apply();
//                 }
//                 });
//                 vm.loadLevel2();*/
//            };
//
//            vm.loadLevel2 = function() {
//                $rootScope.loadLoading("Silahkan Tunggu...");
//                $http.post($rootScope.url_api + 'metodeEvaluasi/getNamaMetodeBasedOnMed', {
//                    med_id: med_id
//                }).success(function(reply) {
//                    vm.namaMetode = reply.nama;
//                });
//                $http.post($rootScope.url_api + 'kriteriaEvaluasi/getNamaParent', {
//                    level: 2,
//                    parent_id: parent
//                }).success(function(reply) {
//                    if (reply.status === 200) {
//                        vm.namaParent = reply.nama_parent;
//                    }
//                });
//                //cek sudah diatur apa belum
//                $http.post($rootScope.url_api + 'metodeEvaluasi/sudahMengaturLevel', {
//                    med_id: med_id,
//                    level: 2,
//                    parent: parent
//                }).success(function(reply) {
//                    if (reply.status === 200) {
//                        vm.sudahMengaturLevel2 = reply.result.data.hasil;
//                        if (vm.sudahMengaturLevel2 == false) {
//                            $http.post($rootScope.url_api + 'kriteriaevaluasi/selectformetode', {
//                                level: 2,
//                                parent: parent
//                            }).success(function(reply2) {
//                                if (reply2.status === 200) {
//                                    vm.kriteria = reply2.result.data;
//                                    for (var i = 0; i < vm.kriteria.length; i++) {
//                                        vm.kriteria[i].checked = false;
//                                        vm.kriteria[i].bobot = "";
//                                    }
//                                }
//                                $rootScope.unloadLoading();
//                            });
//                        }
//                        else if (vm.sudahMengaturLevel2 == true) {
//                            $http.post($rootScope.url_api + 'medkriteria/select', {
//                                med_id: med_id,
//                                level: 2,
//                                parent: parent
//                            }, function(reply2) {
//                                if (reply2.status === 200) {
//                                    vm.kriteria = reply2.result.data;
//                                }
//                                $rootScope.unloadLoading();
//                            });
//                        }
//                    }
//                });
//            };
//
//            vm.simpan = function() {
//                if (vm.kriteria.length === 0) {
//                    $.growl.error({title: "[PERINGATAN]", message: "Tidak ada kriteria yang bisa dimasukkan"});
//                    return;
//                }
//                for (var i = 0; i < vm.kriteria.length; i++) {
//                    if (vm.kriteria[i].checked && vm.kriteria[i].bobot === "") {
//                        $.growl.error({title: "[PERINGATAN]", message: "Bobot harus diisi"});
//                        return;
//                    }
//                }
//                var totalPersentase = 0;
//                for (var i = 0; i < vm.kriteria.length; i++) {
//                    if (vm.kriteria[i].checked) {
//                        totalPersentase = totalPersentase + Number(vm.kriteria[i].bobot);
//                    }
//                }
//                if (totalPersentase !== 100) {
//                    $.growl.error({title: "[PERINGATAN]", message: "Total persentase tidak 100%"});
//                    return;
//                }
//                var detail = [];
//                var temp;
//                for (var i = 0; i < vm.kriteria.length; i++) {
//                    temp = [];
//                    if (vm.kriteria[i].checked) {
//                        temp.push(med_id);
//                        temp.push(vm.kriteria[i].kriteria_id);
//                        temp.push(vm.kriteria[i].level);
//                        temp.push(vm.kriteria[i].parent_id);
//                        temp.push(Number(vm.kriteria[i].bobot));
//                        //temp.push(Number(vm.kriteria[i].kriteria_nama));
//                        detail.push(temp);
//                    }
//                }
//                $rootScope.loadLoading("Silahkan Tunggu...");
//                $http.post($rootScope.url_api + 'metodeEvaluasi/aturLevel', {
//                    detail: detail,
//                    page_id: page_id
//                }).success(function(reply) {
//                    if (reply.status === 200) {
//                        $.growl.notice({title: "[INFO]", message: "Berhasil mengatur level 2"});
//                        $rootScope.unloadLoading();
//                        vm.loadLevel2();
//                    }
//                    else {
//                        $.growl.error({title: "[WARNING]", message: "Gagal mengatur level 2"});
//                        $rootScope.unloadLoading();
//                    }
//                });
//            };
//
//            vm.keLevel3 = function(parentIni) {
//                $state.transitionTo('metode-evaluasi-level3', {med_id: med_id, parent: parentIni, parentSebelumnyaLagi: parent});
//            };
//
//            vm.back = function() {
//                $state.transitionTo('metode-evaluasi-level1', {med_id: med_id});
//            };
//
//            vm.ubahDetailLevel2 = function() {
//                var lempar = {
//                    data: vm.kriteria,
//                    med_id: med_id,
//                    page_id: page_id,
//                    parent: parent
//                };
//                var modalInstance = $modal.open({
//                    templateUrl: 'ubahDetailMetodeLevel2.html',
//                    controller: ubahDetailMetodeLevel2Ctrl,
//                    resolve: {
//                        item: function() {
//                            return lempar;
//                        }
//                    }
//                });
//                modalInstance.result.then(function() {
//                    vm.init();
//                });
//            };
//        })
//        .controller('metodeEvaluasiLevel3Ctrl', function(vm, $http, $rootScope, $state, $stateParams, $cookieStore, $modal) {
//            vm.userBisaMengatur = false;
//            vm.sudahMengaturLevel3;
//            vm.namaParent;
//            vm.menuhome = 0;
//            var page_id = 140;
//            vm.page_id = 140;
//            var med_id = Number($stateParams.med_id);
//            var parent = Number($stateParams.parent);
//            vm.med_id = Number($stateParams.med_id);
//            vm.parent = Number($stateParams.parent);
//            var parentSebelumnya = Number($stateParams.parentSebelumnyaLagi);
//
////            eb.onopen = function() {
////                $rootScope.autorize();
////                vm.init();
////            };
//            vm.init = function() {
//                $rootScope.getSession().then(function(result) {
//                    $rootScope.userSession = result.data.data;
//                    $rootScope.userLogin = $rootScope.userSession.session_data.username;
//                    $rootScope.authorize(vm.initialize());
//                });
//            };
//
//            vm.initialize = function() {
//                $rootScope.refreshWaktu();
//                vm.menuhome = $rootScope.menuhome;
//                $rootScope.isLogged = true;
//                var param = [];
//                param.push($rootScope.userlogged);
//                param.push(page_id);
//
//                $http.post($rootScope.url_api + "roles/check_authority",
//                        {username: $rootScope.userLogin, page_id: vm.page_id, jenis_mengatur: 1})
//                        .success(function(reply) {
//                            if (reply.status === 200) {
//                                var data = reply.result.data[0];
//                                vm.userBisaMengatur = data.bisa_mengatur;
//                            }
//                            else {
//                                $.growl.error({message: "Gagal mendapatkan data hak akses!"});
//                                return;
//                            }
//                        })
//                        .error(function(err) {
//                            $.growl.error({message: "Gagal Akses API >" + err});
//                            return;
//                        });
//                vm.loadLevel3();
//
//
//                /* $http.post($rootScope.url_api+'role/cekBisaMengatur', {
//                 sessionID: $cookieStore.get('sessId'),
//                 param: param,
//                 page_id: page_id
//                 }, function(reply) {
//                 if (reply.status === 'ok') {
//                 if (reply.result.length > 0) {
//                 vm.userBisaMengatur = reply.result[0].bisa_mengatur;
//                 }
//                 vm.$apply();
//                 }
//                 });
//                 vm.loadLevel3();*/
//            };
//
//            vm.loadLevel3 = function() {
//                $rootScope.loadLoading("Silahkan Tunggu...");
//                $http.post($rootScope.url_api + 'metodeEvaluasi/getNamaMetodeBasedOnMed', {
//                    med_id: med_id
//                }).success(function(reply) {
//                    vm.namaMetode = reply.nama;
//                });
//                $http.post($rootScope.url_api + 'kriteriaEvaluasi/getNamaParent', {
//                    level: 3,
//                    parent_id: parent
//                }).success(function(reply) {
//                    if (reply.status === 200) {
//                        vm.namaParent = reply.nama_parent;
//                    }
//                });
//                //cek sudah diatur apa belum
//                $http.post($rootScope.url_api + 'metodeEvaluasi/sudahMengaturLevel', {
//                    med_id: med_id,
//                    level: 3,
//                    parent: parent
//                }).success(function(reply) {
//                    if (reply.status === 200) {
//                        vm.sudahMengaturLevel3 = reply.result.data.hasil;
//                        if (vm.sudahMengaturLevel3 == false) {
//                            $http.post($rootScope.url_api + 'kriteriaevaluasi/selectformetode', {
//                                level: 3,
//                                parent: parent
//                            }).success(function(reply2) {
//                                if (reply2.status === 200) {
//                                    vm.kriteria = reply2.result.data;
//                                    for (var i = 0; i < vm.kriteria.length; i++) {
//                                        vm.kriteria[i].checked = false;
//                                        vm.kriteria[i].bobot = "";
//                                    }
//                                }
//                                $rootScope.unloadLoading();
//                            });
//                        }
//                        else if (vm.sudahMengaturLevel3 == true) {
//                            $http.post($rootScope.url_api + 'medkriteria/select', {
//                                med_id: med_id,
//                                level: 3,
//                                parent: parent
//                            }).success(function(reply2) {
//                                if (reply2.status === 200) {
//                                    vm.kriteria = reply2.result.data;
//                                }
//                                $rootScope.unloadLoading();
//                            });
//                        }
//                    }
//                });
//            };
//
//            vm.simpan = function() {
//                if (vm.kriteria.length === 0) {
//                    $.growl.error({title: "[PERINGATAN]", message: "Tidak ada kriteria yang bisa dimasukkan"});
//                    return;
//                }
//                for (var i = 0; i < vm.kriteria.length; i++) {
//                    if (vm.kriteria[i].checked && vm.kriteria[i].bobot === "") {
//                        $.growl.error({title: "[PERINGATAN]", message: "Bobot harus diisi"});
//                        return;
//                    }
//                }
//                var totalPersentase = 0;
//                for (var i = 0; i < vm.kriteria.length; i++) {
//                    if (vm.kriteria[i].checked) {
//                        totalPersentase = totalPersentase + Number(vm.kriteria[i].bobot);
//                    }
//                }
//                if (totalPersentase !== 100) {
//                    $.growl.error({title: "[PERINGATAN]", message: "Total persentase tidak 100%"});
//                    return;
//                }
//                var detail = [];
//                var temp;
//                for (var i = 0; i < vm.kriteria.length; i++) {
//                    temp = [];
//                    if (vm.kriteria[i].checked) {
//                        temp.push(med_id);
//                        temp.push(vm.kriteria[i].kriteria_id);
//                        temp.push(vm.kriteria[i].level);
//                        temp.push(vm.kriteria[i].parent_id);
//                        temp.push(Number(vm.kriteria[i].bobot));
//                        //temp.push(Number(vm.kriteria[i].kriteria_nama));
//                        detail.push(temp);
//                    }
//                }
//                $rootScope.loadLoading("Silahkan Tunggu...");
//                $http.post($rootScope.url_api + 'metodeEvaluasi/aturLevel', {
//                    detail: detail
//                }).success(function(reply) {
//                    if (reply.status === 200) {
//                        $.growl.notice({title: "[INFO]", message: "Berhasil mengatur level 3"});
//                        $rootScope.unloadLoading();
//                        vm.loadLevel3();
//                    }
//                    else {
//                        $.growl.error({title: "[WARNING]", message: "Gagal mengatur level 3"});
//                        $rootScope.unloadLoading();
//                    }
//                });
//            };
//
//            vm.back = function() {
//                //console.info("parentSebelumnya = " + parentSebelumnya);
//                $state.transitionTo('metode-evaluasi-level2', {med_id: med_id, parent: parentSebelumnya});
//            };
//
//            vm.ubahDetailLevel3 = function() {
//                var lempar = {
//                    data: vm.kriteria,
//                    med_id: med_id,
//                    page_id: page_id,
//                    parent: parent
//                };
//                var modalInstance = $modal.open({
//                    templateUrl: 'ubahDetailMetodeLevel3.html',
//                    controller: ubahDetailMetodeLevel3Ctrl,
//                    resolve: {
//                        item: function() {
//                            return lempar;
//                        }
//                    }
//                });
//                modalInstance.result.then(function() {
//                    vm.init();
//                });
//            };
//        });
//
//var warningUbahMetodeEvaluasiCtrl = function(vm, $modalInstance) {
//    vm.tetapUbah = function() {
//        $modalInstance.close();
//    };
//
//    vm.cancel = function() {
//        $modalInstance.dismiss('cancel');
//    };
//};
//
//var ubahDetailMetodeLevel0Ctrl = function(vm, item, $modalInstance, $http, $cookieStore, $rootScope) {
//    vm.data = item.data;
//    vm.namaMetode = item.namaMetode;
//    var metode_evaluasi_id = item.metode_evaluasi_id;
//    var page_id = item.page_id;
//    var master = [];
//    var detail = [];
//    var detailBaru = [];
//    var detailLama = [];
//    var totalPersentase = 0;
//    var idAdministrasi;
//    var idTeknis;
//    var idHarga;
//    vm.administrasiChecked = false;
//    vm.bobotAdministrasi = 0;
//    vm.teknisChecked = false;
//    vm.bobotTeknis = 0;
//    vm.hargaChecked = false;
//    vm.bobotHarga = 0;
//
//    vm.init = function() {
//        $rootScope.getSession().then(function(result) {
//            $rootScope.userSession = result.data.data;
//            $rootScope.userLogin = $rootScope.userSession.session_data.username;
//            $rootScope.authorize(vm.initialize());
//        });
//    };
//
//    vm.initialize = function() {
//        for (var i = 0; i < vm.data.length; i++) {
//            if (vm.data[i].jenis_detail === 'Administrasi') {
//                vm.administrasiChecked = true;
//                vm.bobotAdministrasi = vm.data[i].bobot;
//                idAdministrasi = vm.data[i].med_id;
//            }
//            else if (vm.data[i].jenis_detail === 'Teknis') {
//                vm.teknisChecked = true;
//                vm.bobotTeknis = vm.data[i].bobot;
//                idTeknis = vm.data[i].med_id;
//            }
//            else if (vm.data[i].jenis_detail === 'Harga') {
//                vm.hargaChecked = true;
//                vm.bobotHarga = vm.data[i].bobot;
//                idHarga = vm.data[i].med_id;
//            }
//        }
//    };
//
//    vm.ubahAdministrasi = function(obj) {
//        vm.administrasiChecked = obj;
//    };
//
//    vm.ubahTeknis = function(obj) {
//        vm.teknisChecked = obj;
//    };
//
//    vm.ubahHarga = function(obj) {
//        vm.hargaChecked = obj;
//    };
//
//    vm.ubahBobotAdministrasi = function(obj) {
//        vm.bobotAdministrasi = obj;
//    };
//
//    vm.ubahBobotTeknis = function(obj) {
//        vm.bobotTeknis = obj;
//    };
//
//    vm.ubahBobotHarga = function(obj) {
//        vm.bobotHarga = obj;
//    };
//
//    vm.ubahNama = function(obj) {
//        vm.namaMetode = obj;
//    };
//
//    vm.simpan = function() {
//        totalPersentase = 0;
//        if (vm.namaMetode === "") {
//            $.growl.error({title: "[PERINGATAN]", message: "Nama metode belum dimasukkan"});
//            return;
//        }
//        if (vm.administrasiChecked === false && vm.teknisChecked === false && vm.hargaChecked === false) {
//            $.growl.error({title: "[PERINGATAN]", message: "Belum ada yang dipilih"});
//            return;
//        }
//        if (vm.teknisChecked && vm.bobotTeknis === "") {
//            $.growl.error({title: "[PERINGATAN]", message: "Persentase teknis belum diisi"});
//            return;
//        }
//        if (vm.hargaChecked && vm.bobotHarga === "") {
//            $.growl.error({title: "[PERINGATAN]", message: "Persentase harga belum diisi"});
//            return;
//        }
//        if (vm.administrasiChecked) {
//            totalPersentase = totalPersentase + Number(vm.bobotAdministrasi);
//        }
//        if (vm.teknisChecked) {
//            totalPersentase = totalPersentase + Number(vm.bobotTeknis);
//        }
//        if (vm.hargaChecked) {
//            totalPersentase = totalPersentase + Number(vm.bobotHarga);
//        }
//        if (totalPersentase !== 100) {
//            $.growl.error({title: "[PERINGATAN]", message: "Total persentase tidak 100%"});
//            return;
//        }
//        master = [];
//        master.push(vm.namaMetode);
//        master.push(metode_evaluasi_id);
//        
//        detailLama = [];
//        detailBaru = [];
//        
//        var hai = [];
//        hai.push('Administrasi');
//        if (vm.administrasiChecked) {
//            hai.push(Number(vm.bobotAdministrasi));
//            hai.push(1);
//        }
//        else {
//            hai.push(0);
//            hai.push(0);
//        }
//        //cek lama atau baru
//        var baru = true;
//        for (var i = 0; i < vm.data.length; i++) {
//            if (vm.data[i].jenis_detail === 'Administrasi') {
//                baru = false;
//                break;
//            }
//        }
//        if (baru) {
//            hai.push(metode_evaluasi_id);
//            detailBaru.push(hai);
//        }
//        else {
//            hai.push(idAdministrasi);
//            detailLama.push(hai);
//        }
//
//        var hai = [];
//        hai.push('Teknis');
//        if (vm.teknisChecked) {
//            hai.push(Number(vm.bobotTeknis));
//            hai.push(1);
//        }
//        else {
//            hai.push(0);
//            hai.push(0);
//        }
//        //cek lama atau baru
//        baru = true;
//        for (var i = 0; i < vm.data.length; i++) {
//            if (vm.data[i].jenis_detail === 'Teknis') {
//                baru = false;
//                break;
//            }
//        }
//        if (baru) {
//            hai.push(metode_evaluasi_id);
//            detailBaru.push(hai);
//        }
//        else {
//            hai.push(idTeknis);
//            detailLama.push(hai);
//        }
//        
//        var hai = [];
//        hai.push('Harga');
//        if (vm.hargaChecked) {
//            hai.push(Number(vm.bobotHarga));
//            hai.push(1);
//        }
//        else {
//            hai.push(0);
//            hai.push(0);
//        }
//        //cek lama atau baru
//        baru = true;
//        for (var i = 0; i < vm.data.length; i++) {
//            if (vm.data[i].jenis_detail === 'Harga') {
//                baru = false;
//                break;
//            }
//        }
//        if (baru) {
//            hai.push(metode_evaluasi_id);
//            detailBaru.push(hai);
//        }
//        else {
//            hai.push(idHarga);
//            detailLama.push(hai);
//        }
//        
//        $http.post($rootScope.url_api + 'metodeEvaluasi/edit', {
//            username: $rootScope.userLogin,
//            userid: $rootScope.userSession.session_data.pegawai_id,
//            master: master,
//            detailLama: detailLama,
//            detailBaru: detailBaru
//        }).success(function(reply) {
//            if (reply.status === 200) {
//                $.growl.notice({title: "[INFO]", message: "Berhasil mengubah metode evaluasi"});
//                $modalInstance.close();
//            }
//            else {
//                $.growl.error({title: "[PERINGATAN]", message: "Gagal mengubah metode evaluasi"});
//            }
//        });
//    };
//
//    vm.cancel = function() {
//        $modalInstance.dismiss('cancel');
//    };
//};
//
//var ubahDetailMetodeLevel1Ctrl = function(vm, $modalInstance, $cookieStore, item, $http, $rootScope) {
//    var data = item.data;
//    var med_id = item.med_id;
//    var page_id = item.page_id;
//    var detailLama = [];
//    var detailBaru = [];
//    vm.hasChild = true;
//
//    vm.init = function() {
//        $rootScope.getSession().then(function(result) {
//            $rootScope.userSession = result.data.data;
//            $rootScope.userLogin = $rootScope.userSession.session_data.username;
//            $rootScope.authorize(vm.initialize());
//        });
//    };
//
//    vm.initialize = function() {
//        $http.post($rootScope.url_api + 'kriteriaevaluasi/selectformetode', {
//            username: $rootScope.userLogin,
//            level: item.level,
//            parent: item.parent
//        }).success(function(reply2) {
//            if (reply2.status === 200) {
//                if (reply2.result.data.length > 0) {
//                    vm.kriteria = reply2.result.data;
//                    for (var i = 0; i < vm.kriteria.length; i++) {
//                        vm.kriteria[i].checked = false;
//                        vm.kriteria[i].bobot = "";
//                        for (var j = 0; j < data.length; j++) {
//                            if (vm.kriteria[i].kriteria_id == data[j].kriteria_id) {
//                                vm.kriteria[i].checked = true;
//                                vm.kriteria[i].bobot = data[j].bobot;
//                                break;
//                            }
//                        }
//                    }
//                } else {
//                    vm.hasChild = false;
//                    if (item.nama != '') {
//                        $('#tableUbahDtl').block({
//                            message: '<div style="text-align:center;"><h4>Tidak ditemukan sub kriteria untuk ' + item.nama + '</h4></div>',
//                            css: {border: '3px solid #a00'}
//                        });
//                    } else {
//                        $('#tableUbahDtl').block({
//                            message: '<div style="text-align:center;"><h4>Tidak ditemukan sub kriteria </h4></div>',
//                            css: {border: '3px solid #a00'}
//                        });
//                    }
//                }
//            }
//        });
//    };
//
//    vm.simpan = function() {
//        detailBaru = [];
//        detailLama = [];
//        for (var i = 0; i < vm.kriteria.length; i++) {
//            if (vm.kriteria[i].checked && vm.kriteria[i].bobot == "") {
//                $.growl.error({title: "[PERINGATAN]", message: "Bobot harus diisi"});
//                return;
//            }
//        }
//        var totalPersentase = 0;
//        for (var i = 0; i < vm.kriteria.length; i++) {
//            if (vm.kriteria[i].checked) {
//                totalPersentase = totalPersentase + Number(vm.kriteria[i].bobot);
//            }
//        }
//        if (totalPersentase !== 100) {
//            $.growl.error({title: "[PERINGATAN]", message: "Total persentase tidak 100%"});
//            return;
//        }
//
//        var temp;
//        var baru;
//        for (var i = 0; i < vm.kriteria.length; i++) {
//            //console.info("ho = " + JSON.stringify(vm.kriteria[i]));
//            temp = [];
//            baru = true;
//            for (var j = 0; j < data.length; j++) {
//                if (vm.kriteria[i].kriteria_id == data[j].kriteria_id) {
//                    baru = false;
//                    if (vm.kriteria[i].checked == true) {
//                        temp.push(Number(vm.kriteria[i].bobot));
//                        temp.push(1);
//                    }
//                    else {
//                        temp.push(0);
//                        temp.push(0);
//                    }
//                    temp.push(data[j].med_kriteria_id);
//                    detailLama.push(temp);
//                    break;
//                }
//            }
//            if (baru) {
//                if (vm.kriteria[i].checked) {
//                    temp.push(med_id);
//                    temp.push(vm.kriteria[i].kriteria_id);
//                    temp.push(vm.kriteria[i].level);
//                    temp.push(vm.kriteria[i].parent_id);
//                    if (vm.kriteria[i].checked == true) {
//                        temp.push(Number(vm.kriteria[i].bobot));
//                    }
//                    else {
//                        temp.push(0);
//                    }
//                    detailBaru.push(temp);
//                }
//            }
//
//        }
//        $rootScope.loadLoadingModal("Silahkan Tunggu...");
//        if (detailLama.length === 0 && detailBaru.length > 0) {
//            $http.post($rootScope.url_api + 'metodeEvaluasi/aturLevel', {
//                username: $rootScope.userLogin,
//                detail: detailBaru
//            }).success(function(reply) {
//                if (reply.status === 200) {
//                    $.growl.notice({title: "[INFO]", message: "Berhasil mengatur kriteria"});
//                    $rootScope.unloadLoadingModal();
//                    $modalInstance.close();
//                }
//                else {
//                    $.growl.error({title: "[WARNING]", message: "Gagal mengatur kriteria"});
//                    $rootScope.unloadLoadingModal();
//                }
//            });
//        } else {
//            $http.post($rootScope.url_api + 'metodeEvaluasi/ubahLevel', {
//                username: $rootScope.userLogin,
//                detailLama: detailLama,
//                detailBaru: detailBaru
//            }).success(function(reply) {
//                if (reply.status === 200) {
//                    $.growl.notice({title: "[INFO]", message: "Berhasil mengubah kriteria"});
//                    $rootScope.unloadLoadingModal();
//                    $modalInstance.close();
//                }
//                else {
//                    $.growl.error({title: "[WARNING]", message: "Gagal mengubah kriteria"});
//                    $rootScope.unloadLoadingModal();
//                }
//            });
//        }
//    };
//
//    vm.cancel = function() {
//        $modalInstance.dismiss('cancel');
//    };
//};
//
//var ubahDetailMetodeLevel2Ctrl = function(vm, $modalInstance, item, $cookieStore, $http, $rootScope) {
//    var data = item.data;
//    var med_id = item.med_id;
//    var page_id = item.page_id;
//    var parent = item.parent;
//    var detailLama = [];
//    var detailBaru = [];
//
//    vm.init = function() {
//        $rootScope.getSession().then(function(result) {
//            $rootScope.userSession = result.data.data;
//            $rootScope.userLogin = $rootScope.userSession.session_data.username;
//            $rootScope.authorize(vm.initialize());
//        });
//    };
//
//    vm.initialize = function() {
//        $http.post($rootScope.url_api + 'kriteriaevaluasi/selectForMetode', {
//            username: $rootScope.userLogin,
//            level: 2,
//            parent: parent
//        }).success(function(reply2) {
//            if (reply2.status === 200) {
//                vm.kriteria = reply2.result.data;
//                for (var i = 0; i < vm.kriteria.length; i++) {
//                    vm.kriteria[i].checked = false;
//                    vm.kriteria[i].bobot = "";
//                    for (var j = 0; j < data.length; j++) {
//                        if (vm.kriteria[i].kriteria_id == data[j].kriteria_id) {
//                            vm.kriteria[i].checked = true;
//                            vm.kriteria[i].bobot = data[j].bobot;
//                            break;
//                        }
//                    }
//                }
//            }
//        });
//    };
//
//    vm.simpan = function() {
//        for (var i = 0; i < vm.kriteria.length; i++) {
//            if (vm.kriteria[i].checked && vm.kriteria[i].bobot === "") {
//                $.growl.error({title: "[PERINGATAN]", message: "Bobot harus diisi"});
//                return;
//            }
//        }
//        var totalPersentase = 0;
//        for (var i = 0; i < vm.kriteria.length; i++) {
//            if (vm.kriteria[i].checked) {
//                totalPersentase = totalPersentase + Number(vm.kriteria[i].bobot);
//            }
//        }
//        if (totalPersentase !== 100) {
//            $.growl.error({title: "[PERINGATAN]", message: "Total persentase tidak 100%"});
//            return;
//        }
//
//        var temp;
//        var baru;
//        for (var i = 0; i < vm.kriteria.length; i++) {
//            ////console.info("ho = " + JSON.stringify(vm.kriteria[i]));
//            temp = [];
//            baru = true;
//            for (var j = 0; j < data.length; j++) {
//                if (vm.kriteria[i].kriteria_id == data[j].kriteria_id) {
//                    baru = false;
//                    if (vm.kriteria[i].checked == true) {
//                        temp.push(Number(vm.kriteria[i].bobot));
//                        temp.push(1);
//                    }
//                    else {
//                        temp.push(0);
//                        temp.push(0);
//                    }
//                    temp.push(data[j].med_kriteria_id);
//                    detailLama.push(temp);
//                    break;
//                }
//            }
//            if (baru) {
//                if (vm.kriteria[i].checked) {
//                    temp.push(med_id);
//                    temp.push(vm.kriteria[i].kriteria_id);
//                    temp.push(vm.kriteria[i].level);
//                    temp.push(vm.kriteria[i].parent_id);
//                    if (vm.kriteria[i].checked == true) {
//                        temp.push(Number(vm.kriteria[i].bobot));
//                    }
//                    else {
//                        temp.push(0);
//                    }
//                    detailBaru.push(temp);
//                }
//            }
//
//        }
//        $rootScope.loadLoadingModal("Silahkan Tunggu...");
//        $http.post($rootScope.url_api + 'metodeEvaluasi/ubahLevel', {
//            username: $rootScope.userLogin,
//            detailLama: detailLama,
//            detailBaru: detailBaru
//        }).success(function(reply) {
//            if (reply.status === 200) {
//                $.growl.notice({title: "[INFO]", message: "Berhasil mengubah level 2"});
//                $rootScope.unloadLoadingModal();
//                $modalInstance.close();
//            }
//            else {
//                $.growl.error({title: "[WARNING]", message: "Gagal mengubah level 2"});
//                $rootScope.unloadLoadingModal();
//            }
//        });
//    };
//
//    vm.cancel = function() {
//        $modalInstance.dismiss('cancel');
//    };
//};
//
//var ubahDetailMetodeLevel3Ctrl = function(vm, $modalInstance, $http, item, $cookieStore, $rootScope) {
//    var data = item.data;
//    var med_id = item.med_id;
//    var page_id = item.page_id;
//    var parent = item.parent;
//    var detailLama = [];
//    var detailBaru = [];
//
//    vm.init = function() {
//        $rootScope.getSession().then(function(result) {
//            $rootScope.userSession = result.data.data;
//            $rootScope.userLogin = $rootScope.userSession.session_data.username;
//            $rootScope.authorize(vm.initialize());
//        });
//    };
//
//    vm.initialize = function() {
//        $http.post($rootScope.url_api + 'kriteriaevaluasi/selectformetode', {
//            username: $rootScope.userLogin,
//            level: 3,
//            parent: parent
//        }).success(function(reply2) {
//            if (reply2.status === 200) {
//                vm.kriteria = reply2.result.data;
//                for (var i = 0; i < vm.kriteria.length; i++) {
//                    vm.kriteria[i].checked = false;
//                    vm.kriteria[i].bobot = "";
//                    for (var j = 0; j < data.length; j++) {
//                        if (vm.kriteria[i].kriteria_id == data[j].kriteria_id) {
//                            vm.kriteria[i].checked = true;
//                            vm.kriteria[i].bobot = data[j].bobot;
//                            break;
//                        }
//                    }
//                }
//            }
//        });
//    };
//
//    vm.simpan = function() {
//        for (var i = 0; i < vm.kriteria.length; i++) {
//            if (vm.kriteria[i].checked && vm.kriteria[i].bobot == "") {
//                $.growl.error({title: "[PERINGATAN]", message: "Bobot harus diisi"});
//                return;
//            }
//        }
//        var totalPersentase = 0;
//        for (var i = 0; i < vm.kriteria.length; i++) {
//            if (vm.kriteria[i].checked) {
//                totalPersentase = totalPersentase + Number(vm.kriteria[i].bobot);
//            }
//        }
//        if (totalPersentase !== 100) {
//            $.growl.error({title: "[PERINGATAN]", message: "Total persentase tidak 100%"});
//            return;
//        }
//
//        var temp;
//        var baru;
//        for (var i = 0; i < vm.kriteria.length; i++) {
//            ////console.info("ho = " + JSON.stringify(vm.kriteria[i]));
//            temp = [];
//            baru = true;
//            for (var j = 0; j < data.length; j++) {
//                if (vm.kriteria[i].kriteria_id == data[j].kriteria_id) {
//                    baru = false;
//                    if (vm.kriteria[i].checked == true) {
//                        temp.push(Number(vm.kriteria[i].bobot));
//                        temp.push(1);
//                    }
//                    else {
//                        temp.push(0);
//                        temp.push(0);
//                    }
//                    temp.push(data[j].med_kriteria_id);
//                    detailLama.push(temp);
//                    break;
//                }
//            }
//            if (baru) {
//                if (vm.kriteria[i].checked) {
//                    temp.push(med_id);
//                    temp.push(vm.kriteria[i].kriteria_id);
//                    temp.push(vm.kriteria[i].level);
//                    temp.push(vm.kriteria[i].parent_id);
//                    if (vm.kriteria[i].checked == true) {
//                        temp.push(Number(vm.kriteria[i].bobot));
//                    }
//                    else {
//                        temp.push(0);
//                    }
//                    detailBaru.push(temp);
//                }
//            }
//        }
//        $rootScope.loadLoadingModal("Silahkan Tunggu...");
//        $http.post($rootScope.url_api + 'metodeEvaluasi/ubahLevel', {
//            username: $rootScope.userLogin,
//            detailLama: detailLama,
//            detailBaru: detailBaru
//        }).success(function(reply) {
//            if (reply.status === 200) {
//                $.growl.notice({title: "[INFO]", message: "Berhasil mengubah level 3"});
//                $rootScope.unloadLoadingModal();
//                $modalInstance.close();
//            }
//            else {
//                $.growl.error({title: "[WARNING]", message: "Gagal mengubah level 3"});
//                $rootScope.unloadLoadingModal();
//            }
//        });
//    };
//
//    vm.cancel = function() {
//        $modalInstance.dismiss('cancel');
//    };
//};
//
//var modalDetailMetodeEvaluasiCtrl = function(vm, $modalInstance, item, $cookieStore, $http, $rootScope) {
//    var metode_evaluasi_id = item.metode_evaluasi_id;
//    var Administrasi = [];
//    var AdministrasiLevel1 = [];
//    var AdministrasiLevel2 = [];
//    var AdministrasiLevel3 = [];
//    var Teknis = [];
//    var TeknisLevel1 = [];
//    var TeknisLevel2 = [];
//    var TeknisLevel3 = [];
//    var Harga = [];
//    vm.Administrasi;
//    vm.bobotAdministrasi;
//    vm.Teknis;
//    vm.bobotTeknis;
//    vm.Harga;
//    vm.bobotHarga;
//    vm.kategori;
//    vm.nama;
//
//    vm.init = function() {
//        $rootScope.getSession().then(function(result) {
//            $rootScope.userSession = result.data.data;
//            $rootScope.userLogin = $rootScope.userSession.session_data.username;
//            $rootScope.authorize(vm.initialize());
//        });
//    };
//
//    vm.initialize = function() {
//        $http.post($rootScope.url_api + 'metodeEvaluasi/selectKategori', {
//            id: metode_evaluasi_id
//        }).success(function(reply) {
//            if (reply.status === 200) {
//                vm.kategori = reply.result.data;
//                for (var i = 0; i < vm.kategori.length; i++) {
//                    if (vm.kategori[i].jenis_detail === 'Administrasi')
//                        vm.bobotAdministrasi = vm.kategori[i].bobot;
//                    else if (vm.kategori[i].jenis_detail === 'Teknis')
//                        vm.bobotTeknis = vm.kategori[i].bobot;
//                    else if (vm.kategori[i].jenis_detail === 'Harga')
//                        vm.bobotHarga = vm.kategori[i].bobot;
//                }
//                vm.nama = reply.result.data[0].metode_evaluasi_nm;
//            }
//        });
//
//        $http.post($rootScope.url_api + 'metodeEvaluasi/getDetail', {
//            id: metode_evaluasi_id
//        }).success(function(reply2) {
//            //console.info("reply2 = " + JSON.stringify(reply2));
//            if (reply2.status === 200) {
//                var hasil = reply2.result.data;
//                for (var i = 0; i < hasil.length; i++) {
//                    if (hasil[i].jenis_detail === 'Administrasi') {
//                        Administrasi.push(hasil[i]);
//                    }
//                    else if (hasil[i].jenis_detail === 'Teknis') {
//                        Teknis.push(hasil[i]);
//                    }
//                    else if (hasil[i].jenis_detail === 'Harga') {
//                        Harga.push(hasil[i]);
//                    }
//                }
//                for (var i = 0; i < Administrasi.length; i++) {
//                    if (Administrasi[i].level == 1) {
//                        AdministrasiLevel1.push(Administrasi[i]);
//                    }
//                    else if (Administrasi[i].level == 2) {
//                        AdministrasiLevel2.push(Administrasi[i]);
//                    }
//                    else if (Administrasi[i].level == 3) {
//                        AdministrasiLevel3.push(Administrasi[i]);
//                    }
//                }
//                for (var i = 0; i < AdministrasiLevel2.length; i++) {
//                    AdministrasiLevel2[i].sub = [];
//                    for (var j = 0; j < AdministrasiLevel3.length; j++) {
//                        if (AdministrasiLevel3[j].parent == AdministrasiLevel2[i].kriteria_id) {
//                            AdministrasiLevel2[i].sub.push(AdministrasiLevel3[j]);
//                        }
//                    }
//                }
//                for (var i = 0; i < AdministrasiLevel1.length; i++) {
//                    AdministrasiLevel1[i].sub = [];
//                    for (var j = 0; j < AdministrasiLevel2.length; j++) {
//                        if (AdministrasiLevel2[j].parent == AdministrasiLevel1[i].kriteria_id) {
//                            AdministrasiLevel1[i].sub.push(AdministrasiLevel2[j]);
//                        }
//                    }
//
//                }
//
//                for (var i = 0; i < Teknis.length; i++) {
//                    if (Teknis[i].level == 1) {
//                        TeknisLevel1.push(Teknis[i]);
//                    }
//                    else if (Teknis[i].level == 2) {
//                        TeknisLevel2.push(Teknis[i]);
//                    }
//                    else if (Teknis[i].level == 3) {
//                        TeknisLevel3.push(Teknis[i]);
//                    }
//                }
//                for (var i = 0; i < TeknisLevel2.length; i++) {
//                    TeknisLevel2[i].sub = [];
//                    for (var j = 0; j < TeknisLevel3.length; j++) {
//                        if (TeknisLevel3[j].parent == TeknisLevel2[i].kriteria_id) {
//                            TeknisLevel2[i].sub.push(TeknisLevel3[j]);
//                        }
//                    }
//                }
//                for (var i = 0; i < TeknisLevel1.length; i++) {
//                    TeknisLevel1[i].sub = [];
//                    for (var j = 0; j < TeknisLevel2.length; j++) {
//                        if (TeknisLevel2[j].parent == TeknisLevel1[i].kriteria_id) {
//                            TeknisLevel1[i].sub.push(TeknisLevel2[j]);
//                        }
//                    }
//                }
//                vm.Administrasi = AdministrasiLevel1;
//                vm.Teknis = TeknisLevel1;
//            }
//        });
//    };
//
//    vm.keluar = function() {
//        $modalInstance.dismiss('cancel');
//    };
//};
//
//function Kata(srcText) {
//    var self = this;
//    self.srcText = srcText;
//}