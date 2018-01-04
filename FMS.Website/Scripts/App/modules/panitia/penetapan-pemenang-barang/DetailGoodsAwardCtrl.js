(function () {
    'use strict';

    angular.module("app")
    .controller("DetailGoodsAwardCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'GoodsAwardService', '$state', 'UIControlService', '$uibModal', '$uibModalInstance', 'GlobalConstantService', 'item'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, GoodsAwardService,
        $state, UIControlService, $uibModal, $uibModalInstance, GlobalConstantService, item) {
        var vm = this;
        vm.detail = item.item;

        vm.isCalendarOpened = [false, false, false, false];
        vm.init = init;
        function init() {
            if (vm.detail.NoPO !== null) {
                vm.NoPO = vm.detail.NoPO;
                vm.PODate = vm.detail.PODate;
            }
            convertToDate();
        }


        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        };

        function convertAllDateToString() { // TIMEZONE (-)
            if (vm.PODate) {
                vm.PODate = UIControlService.getStrDate(vm.PODate);
            }
        };

        //Supaya muncul di date picker saat awal load
        function convertToDate() {
            if (vm.PODate) {
                vm.PODate = new Date(Date.parse(vm.PODate));
            }
        }

        vm.simpan = simpan;
        function simpan() {
            UIControlService.loadLoadingModal("Silahkan Tunggu...");
                convertAllDateToString();
                GoodsAwardService.update({
                    ID: vm.detail.ID,
                    TenderStepID: vm.detail.TenderStepID,
                    VendorID: vm.detail.VendorID,
                    TotalNego: vm.detail.TotalNego,
                    NoPO: vm.NoPO,
                    PODate: vm.PODate

                }, function (reply) {
                    UIControlService.unloadLoadingModal();
                    if (reply.status === 200) {
                        UIControlService.msg_growl("success", "Berhasil Simpan Data PO!!");
                        $uibModalInstance.close();
                    }
                    else {
                        UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                        return;
                    }
                }, function (err) {
                    UIControlService.msg_growl("error", "Gagal Akses Api!!");
                    UIControlService.unloadLoadingModal();
                });
        }


        vm.batal = batal;
        function batal() {
            $uibModalInstance.close();
        };


    }
})();;