(function () {
    'use strict';

    angular.module("app")
    .controller("tambahMetodeEvaluasiPrakual", ctrl);
    
    ctrl.$inject = ['$http', '$state', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'MetodeEvaluasiPrakualService', 'UIControlService'];
    /* @ngInject */
    function ctrl($http, $state, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, metodeEvaluasiPrakualService, UIControlService) {

        var evaluationMethodId = Number($stateParams.id);
        var vm = this;
        var evaluationMethodModel;
        
        vm.isEdit = evaluationMethodId > 0;
        vm.namaMetode = "";
        vm.kriteria = [];
        vm.menuhome = 0;
        vm.page_id = 140;
        
        //functions
        vm.init = init;
        vm.loadDataAdd = loadDataAdd;

        function init() {
//            vm.menuhome = $rootScope.menuhome;
//            $rootScope.getSession().then(function(result) {
//                $rootScope.userSession = result.data.data;
//                $rootScope.userLogin = $rootScope.userSession.session_data.username;
//                $rootScope.authorize(vm.loadDataAdd());
//            });
            vm.loadDataAdd();
        };

        function loadDataAdd() {

//            $http.post($rootScope.url_api + "roles/check_authority",
//                {username: $rootScope.userLogin, page_id: vm.page_id, jenis_mengatur: 1})
//                .success(function(reply) {
//                    if (reply.status === 200) {
//                        var data = reply.result.data[0];
//                        vm.userBisaMengatur = data.bisa_mengatur;
//                    }
//                    else {
//                        $.growl.error({message: "Gagal mendapatkan data hak akses!"});
//                        $rootScope.unloadLoading();
//                        return;
//                    }
//                    $rootScope.unloadLoading();
//                })
//                .error(function(err) {
//                    $.growl.error({message: "Gagal Akses API >" + err});
//                    $rootScope.unloadLoading();
//                    return;
//                });
            vm.kriteriaList = [
                { kriteria_nama: "Administrasi", checked: false, bobot: 0 },
                { kriteria_nama: "Teknis", checked: false, bobot: 0 },
                { kriteria_nama: "Harga", checked: false, bobot: 0 }
            ];

            if (vm.isEdit) {
                UIControlService.loadLoading("Silahkan Tunggu...");
                metodeEvaluasiPrakualService.selectById({
                    EvaluationMethodId : evaluationMethodId
                }, function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        evaluationMethodModel = reply.data;
                        vm.namaMetode = evaluationMethodModel.EvaluationMethodName;
                        if (evaluationMethodModel.EvaluationMethodDetails.length > 0) {
                            vm.kriteriaList.forEach(function (kl) {
                                evaluationMethodModel.EvaluationMethodDetails.forEach(function (emm) {
                                    if (kl.kriteria_nama === emm.DetailType) {
                                        kl.id = emm.EMDId;
                                        kl.checked = emm.IsActive;
                                        kl.bobot = emm.Weight;
                                    }
                                });
                            });
                        }

                    } else {
                        UIControlService.msg_growl("error", "Gagal mendapat data metode evaluasi");
                    }
                }, function (err) {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", "Gagal akses API");
                });
            }
        }


        vm.tambah = tambah;
        function tambah() {
            if (vm.namaMetode === "") {
                UIControlService.msg_growl("error","Nama metode belum dimasukkan" );
                return;
            }

            var metodeEvaluasi = {
                EvaluationMethodName: vm.namaMetode,
                EvaluationMethodDetails: []
            }
            if (vm.isEdit) {
                metodeEvaluasi.EvaluationMethodId = evaluationMethodModel.EvaluationMethodId;
            }

            var totalPersen = 0;
            var details = [];
            vm.kriteriaList.forEach(function (k) {
                var detail = {
                    DetailType: k.kriteria_nama,
                    Weight: k.checked ? k.bobot : 0,
                    IsActive: k.checked
                }
                if (k.id > 0) {
                    detail.EMDId = k.id;
                }
                totalPersen += detail.Weight;
                details.push(detail);
            });

            if (details.length === 0) {
                UIControlService.msg_growl("error", "Belum ada yang dipilih");
                return;
            }
            if (totalPersen !== 100) {
                UIControlService.msg_growl("error", "Total persentase tidak 100%");
                return;
            }

            metodeEvaluasi.EvaluationMethodDetails = details;            

            if (vm.isEdit) {
                UIControlService.loadLoading("Silahkan Tunggu...");
                metodeEvaluasiPrakualService.update(metodeEvaluasi,
                    function (reply) {
                        UIControlService.unloadLoading();
                        if (reply.status === 200) {
                            UIControlService.msg_growl("notice", "Berhasil mengubah metode evaluasi");
                            $state.transitionTo('prakual-metode-evaluasi');
                        }
                        else {
                            UIControlService.msg_growl("error", "Gagal mengubah metode evaluasi");
                        }
                    }, function (err) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", "Gagal akses API");
                    }
                );
            } else {
                UIControlService.loadLoading("Silahkan Tunggu...");
                metodeEvaluasiPrakualService.insert(metodeEvaluasi,
                    function (reply) {
                        UIControlService.unloadLoading();
                        if (reply.status === 200) {
                            UIControlService.msg_growl("notice", "Berhasil membuat metode evaluasi");
                            $state.transitionTo('prakual-metode-evaluasi');
                        }
                        else {
                            UIControlService.msg_growl("error", "Gagal membuat metode evaluasi");
                        }
                    }, function (err) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", "Gagal akses API");
                    }
                );
            }
        };

        vm.detail = detail;
        function detail(id) {
            $state.transitionTo('detail-metode-prakual', {id: id});
        }

        vm.back = back;
        function back() {
            $state.transitionTo('prakual-metode-evaluasi');
        };
    }
})();