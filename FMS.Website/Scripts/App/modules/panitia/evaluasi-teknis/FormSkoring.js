(function () {
    'use strict';

    angular.module("app")
    .controller("FormScoringTechnicalCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'EvaluationTechnicalService', '$state', 'UIControlService', '$uibModal', '$uibModalInstance', 'GlobalConstantService', 'item'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, EvaluationTechnicalService,
        $state, UIControlService, $uibModal, $uibModalInstance, GlobalConstantService, item) {
        var vm = this;
        vm.detail = item.item;

        vm.init = init;
        function init() {
        }

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
            //$uibModalInstance.close();
        };

        vm.save = save;
        function save() {
            if (vm.detail.Score > 5 || vm.detail.Score < 1 || !vm.detail.Score) {
                UIControlService.msg_growl("error", "Nilai skor harus berada di dalam rentang yang ditentukan...");
                return;
            }
            vm.data = [];
            console.info(JSON.stringify(vm.detail));
            var data = {
                ID: vm.detail.ID,
                EvalMDCriteria: vm.detail.EvalMDCriteria,
                Weight: vm.detail.Weight,
                Score: vm.detail.Score,
                EvalTechID: vm.detail.EvalTechID,
                VendorID: vm.detail.VendorID,
                TenderStepDataID: vm.detail.TenderStepDataID
            }
            vm.data.push(data);
            UIControlService.loadLoadingModal('MESSAGE.LOADING');
            EvaluationTechnicalService.Insert(vm.data,
               function (reply) {
                   UIControlService.unloadLoadingModal();
                   if (reply.status === 200) {
                       UIControlService.msg_growl("success", "Berhasil Simpan data");
                       $uibModalInstance.close();
                   }
                   else {
                       UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                       return;
                   }
               },
               function (err) {
                   UIControlService.msg_growl("error", "Gagal Akses Api!!");
                   UIControlService.unloadLoadingModal();
               }
          );
        }


    }
})();;