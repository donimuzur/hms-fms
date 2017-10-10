(function () {
    'use strict';

    angular.module("app").controller("WriteChatVendorCtrl", ctrl);

    ctrl.$inject = ['item', '$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'NegosiasiService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService', '$uibModalInstance'];
    function ctrl(item, $http, $translate, $translatePartialLoader, $location, SocketService, NegosiasiService,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService, $uibModalInstance) {

        var vm = this;
        vm.step = item.StepID;
        vm.detail = item.item;
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.init = init;

        vm.jLoad = jLoad;

        function init() {
            UIControlService.loadLoading("Silahkan Tunggu...");
            jLoad(1);

        }

        vm.jLoad = jLoad;
        function jLoad(current) {
            //console.info("curr "+current)
            vm.Tenderstep = {};
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            var tender = {
                Status: vm.step
            }
            NegosiasiService.selectTender(tender, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.Tenderstep = reply.data;
                    vm.judul = vm.Tenderstep.TenderName;
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Evaluasi Teknis" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }
        
        vm.simpan = simpan;
        function simpan() {
            var data = {
                NegoId: vm.detail.NegoId,
                EvaluatorID: vm.detail.EvaluatorID,
                Description: vm.isi
            };
            NegosiasiService.InsertChatVendor(data,
               function (reply) {
                   UIControlService.unloadLoadingModal();
                   if (reply.status === 200) {
                       UIControlService.msg_growl("success", "Berhasil Simpan Data!!");
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

        vm.backpengadaan = backpengadaan;
        function backpengadaan() {
            $state.transitionTo('data-pengadaan-tahapan', { TenderRefID: vm.TenderRefID, ProcPackType: vm.ProcPackType });
        }
        vm.batal = batal;
        function batal() {
            $uibModalInstance.close();
        }

    }
})();
//TODO


