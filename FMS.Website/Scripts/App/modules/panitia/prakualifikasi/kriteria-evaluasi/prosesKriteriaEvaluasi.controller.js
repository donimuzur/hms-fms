(function () {
    'use strict';

    angular.module("app")
    .controller("formKriteriaEvaluasiCtrl", ctrl);
    
    ctrl.$inject = ['$http', '$uibModal', '$uibModalInstance', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'KriteriaEvaluasiService', 'item', 'UIControlService'];
    /* @ngInject */
    function ctrl($http, $uibModal, $uibModalInstance, $translate, $translatePartialLoader, $location, SocketService, KriteriaEvaluasiService, item, UIControlService) {
        var vm = this;
        vm.placeHolder = item.level === 1 ? 'MODAL.MASTER_KRITERIA' : 'MODAL.SUB_KRITERIA';
        vm.isEdit = false;
        vm.title = "MODAL.TAMBAH";
        vm.nama = "";
        
        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('prakualifikasi-kriteria-evaluasi');
            if (item.criteriaId) {
                vm.isEdit = true;
                vm.title = "MODAL.UBAH";
                vm.nama = item.criteriaName;
            }
        };

        vm.cancel = cancel;
        function cancel () {
            $uibModalInstance.dismiss('cancel');
        };
        
        vm.save = save;
        function save() {
            if (!vm.nama) {
                UIControlService.msg_growl("error", "Nama kriteria belum diisi");
                return;
            }

            UIControlService.loadLoadingModal("Silahkan Tunggu");
            if (vm.isEdit) {
                KriteriaEvaluasiService.updateCriteria({
                    criteriaId: item.criteriaId,
                    criteriaName: vm.nama
                }, function (reply) {
                    UIControlService.unloadLoadingModal();
                    if (reply.status === 200) {
                        UIControlService.msg_growl("notice", "Berhasil menyimpan kriteria");
                        $uibModalInstance.close();
                    } else {
                        UIControlService.msg_growl("error", "Gagal menyimpan kriteria");
                    }
                }, function (err) {
                    UIControlService.msg_growl("error", "Gagal mengakses API");
                    UIControlService.unloadLoadingModal();
                });
            } else {
                KriteriaEvaluasiService.insertCriteria({
                    criteriaName: vm.nama,
                    level: item.level,
                    parentId: item.parentId
                }, function (reply) {
                    UIControlService.unloadLoadingModal();
                    if (reply.status === 200) {
                        UIControlService.msg_growl("notice", "Berhasil menyimpan kriteria");
                        $uibModalInstance.close();
                    } else {
                        UIControlService.msg_growl("error", "Gagal menyimpan kriteria");
                    }
                }, function (err) {
                    UIControlService.msg_growl("error", "Gagal mengakses API");
                    UIControlService.unloadLoadingModal();
                });
            }
        }
    }
})();