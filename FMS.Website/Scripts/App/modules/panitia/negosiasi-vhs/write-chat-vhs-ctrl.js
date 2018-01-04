(function () {
    'use strict';

    angular.module("app").controller("WriteChatVHSCtrl", ctrl);

    ctrl.$inject = ['item', '$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'NegosiasiVHSService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService', '$uibModalInstance'];
    function ctrl(item, $http, $translate, $translatePartialLoader, $location, SocketService, NegosiasiVHSService,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService, $uibModalInstance) {

        var vm = this;
        vm.vendor = item.VendorID;
        vm.step = item.StepID;
        vm.NegoId = item.NegoId;
        vm.judule = item.Judul;
        vm.vni = item.VHSNegoId;
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.init = init;
        vm.tender = {};

       // vm.jLoad = jLoad;

        function init() {
            UIControlService.loadLoading("Silahkan Tunggu...");
           jLoad(1);

        }
        
        vm.jLoad = jLoad;
        function jLoad(current) {
            console.info("curr " + current)
            vm.nego = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            var tend = {
                VendorID: vm.VendorID,
                TenderStepDataID: vm.StepID
            }
            NegosiasiVHSService.bychat(tend, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.nego = reply.data;
                    //vm.judule = vm.nego[0].TenderName;
                    //vm.idtender = vm.nego[0].TenderStepID;
                   // console.info("judul:" + JSON.stringify(vm.idtender));
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Chatting" });
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
            var dt = {
                VHSNegoId: vm.vni,
                VendorID: vm.vendor,
                Description: vm.isi
            };
            NegosiasiVHSService.insertchat(dt,
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
                   console.info("error:" + JSON.stringify(err));
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


