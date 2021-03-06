﻿(function () {
    'use strict';

    angular.module("app")
    .controller("detailEvaluasiPrakualModal", ctrl);
    
    ctrl.$inject = ['$http', '$state', '$stateParams', '$uibModalInstance', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'MetodeEvaluasiPrakualService', 'KriteriaEvaluasiService', 'UIControlService', 'item'];
    /* @ngInject */
    function ctrl($http, $state, $stateParams, $uibModalInstance, $translate, $translatePartialLoader, $location, SocketService, metodeEvaluasiPrakualService, KriteriaEvaluasiService, UIControlService, item) {

        var vm = this;
        var data = item.data;
        var med_id = item.med_id;
        var page_id = item.page_id;
        var detailLama = [];
        var detailBaru = [];
        vm.sudahDipakai = item.sudahDipakai;
        vm.hasChild = true;

        vm.init = init;
        function init() {
            //$rootScope.getSession().then(function (result) {
            //    $rootScope.userSession = result.data.data;
            //    $rootScope.userLogin = $rootScope.userSession.session_data.username;
            //    $rootScope.authorize(vm.initialize());
            //});
            vm.initialize()
        };

        vm.initialize = initialize;
        function initialize() {
            KriteriaEvaluasiService.selectCriteria({
                keyword: '',
                level: item.level,
                parentId: item.parent,
                offset: 0,
                limit: 0
            }, function (reply2) {
                if (reply2.status === 200) {
                    if (reply2.data.length > 0) {
                        vm.kriteria = reply2.data;
                        for (var i = 0; i < vm.kriteria.length; i++) {
                            vm.kriteria[i].EMDId = med_id
                            vm.kriteria[i].checked = false;
                            vm.kriteria[i].Weight = 0;
                            for (var j = 0; j < data.length; j++) {
                                if (vm.kriteria[i].CriteriaId === data[j].kriteria_id) {
                                    vm.kriteria[i].Id = data[j].med_kriteria_id;
                                    vm.kriteria[i].checked = true;
                                    vm.kriteria[i].Weight = data[j].bobot;
                                    break;
                                }
                            }
                        }
                    } else {
                        vm.hasChild = false;
                    }
                }
            });
        };

        vm.simpan = simpan;
        function simpan() {
            for (var i = 0; i < vm.kriteria.length; i++) {
                if (vm.kriteria[i].checked && !(vm.kriteria[i].Weight > 0)) {
                    UIControlService.msg_growl('error', "Bobot harus diisi antara 0 - 100");
                    return;
                }
            }
            var totalPersentase = 0;
            for (var i = 0; i < vm.kriteria.length; i++) {
                if (vm.kriteria[i].checked) {
                    totalPersentase = totalPersentase + Number(vm.kriteria[i].Weight);
                }
            }
            if (totalPersentase !== 100) {
                UIControlService.msg_growl('error', "Total persentase tidak 100%");
                return;
            }
            var detail = [];
            for (var i = 0; i < vm.kriteria.length; i++) {
                vm.kriteria[i].Parent = vm.kriteria[i].ParentId; //Beda Nama Kolom di DB
                vm.kriteria[i].IsActive = vm.kriteria[i].checked;
                vm.kriteria[i].Weight = vm.kriteria[i].checked ? vm.kriteria[i].Weight : 0;
                detail.push(vm.kriteria[i]);
            }
            UIControlService.loadLoading("Silahkan Tunggu...");
            metodeEvaluasiPrakualService.saveDetailCriteria(detail,
                function (reply) {
                    if (reply.status === 200) {
                        vm.kriteria = [];
                        UIControlService.msg_growl('notice', "Berhasil mengubah kriteria");
                        UIControlService.unloadLoading();
                        $uibModalInstance.close();
                    }
                    else {
                        UIControlService.msg_growl('error', "Gagal mengubah kriteria");
                        UIControlService.unloadLoading();
                    }
                }, function (err) {
                    UIControlService.msg_growl('error', 'Gagal Akses API');
                }
            );
        };

        vm.cancel = cancel;
        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();