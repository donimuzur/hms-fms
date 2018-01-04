(function () {
    'use strict';

    angular.module("app").controller("SummaryVHSCtrl", ctrl);

    ctrl.$inject = ['item', '$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'NegosiasiVHSService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService', '$uibModalInstance'];
    function ctrl(item, $http, $translate, $translatePartialLoader, $location, SocketService, NegosiasiVHSService,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService, $uibModalInstance) {

        var vm = this;
        vm.StepID = item.StepID;
        vm.TenderRefID = item.TenderRefID;
        vm.ProcPackType = item.ProcPackType;
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.init = init;
        vm.tender = {};
        vm.nb = item.item;
       // vm.jLoad = jLoad;

        function init() {
            console.info(item);
            UIControlService.loadLoading("Silahkan Tunggu...");
         //  jLoad(1);

        }
        
        vm.jLoad = jLoad;
        function jLoad(current) {
            //console.info("curr "+current)
            vm.nb = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            NegosiasiVHSService.select({
                column: vm.StepID,
                status: vm.TenderRefID,
                FilterType: vm.ProcPackType
            }, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.nb = reply.data;
                    //console.info("data:" + JSON.stringify(vm.TenderStepDataID));
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                // console.info("error:" + JSON.stringify(err));
                $.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }
        
        vm.simpan = simpan;
        function simpan() {
            vm.list = [];
            for (var i = 0; i < vm.nb.length; i++) {
                if (vm.nb[i].IsDeal === null) {
                    vm.nb[i].IsDeal = false;
                }
                if (vm.nb[i].ID !== 0) {
                    var dta = {
                        ID: vm.nb[i].ID,
                        IsDeal: vm.nb[i].IsDeal
                    };
                }
                else {
                    var dta = {
                        ID: vm.nb[i].ID,
                        IsDeal: vm.nb[i].IsDeal
                    };
                }
                vm.list.push(dta);
            }
            NegosiasiVHSService.updatedeal(vm.list,
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
                   // UIControlService.unloadLoadingModal();
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


