(function () {
    'use strict';

    angular.module("app")
    .controller("PendaftaranPengadaanCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'PengumumanPengadaanService', '$state', 'UIControlService', '$uibModal', '$uibModalInstance', 'GlobalConstantService', 'item'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, PengumumanPengadaanService,
        $state, UIControlService, $uibModal, $uibModalInstance, GlobalConstantService, item) {
        var vm = this;
        vm.data = item.item;

        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.TanggalHariIni = new Date();
        vm.textSearch = '';
        vm.listPengumuman = [];
        vm.currentPage = 1;
        vm.maxSize = 10;
        vm.statusMinat = false;

        vm.init = init;
        function init() {
            console.info(JSON.stringify(vm.data));
            loadDataVendor(1);
        }

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
            //$uibModalInstance.close();
        };


        function loadDataVendor(current) {
            var offset = (current * 10) - 10;
            PengumumanPengadaanService.selectDataVendor({
                Keyword: '', Offset: offset, Limit: 10
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.vendorID = reply.data[0].VendorID;
                    //console.info("vendor ID" + JSON.stringify(vm.vendorID));
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.changeStatusMinat = changeStatusMinat;
        function changeStatusMinat() {
            vm.statusMinat = true;
        }


        vm.minat = minat;
        function minat() {
            //console.info("minat" + JSON.stringify(vm.data.TenderAnnounID) +"x"+ JSON.stringify(vm.data.TenderStepData.ID) +"y"+JSON.stringify(vm.data.TenderStepData.TenderID));
            PengumumanPengadaanService.insertRegistration({
                //TenderAnnounID: vm.data.TenderAnnounID,
                VendorID: vm.vendorID,
                IsSurvive: 1, 
                IsResign: 0, 
                LastTenderStepID: vm.data.TenderStepData.ID, 
                TenderID: vm.data.TenderStepData.TenderID
            }, function (reply) {
                console.info(JSON.stringify(reply.status));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "Berhasil Mendaftar Tender");
                    $uibModalInstance.close();
                }
                else {
                    UIControlService.msg_growl("error", "Gagal menonaktifkan data ");
                    return;
                }
            }, function (err) {

                UIControlService.msg_growl("error", "Gagal Akses API ");
                UIControlService.unloadLoading();
            });

        }

        vm.cancel = cancel;
        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }
    }
})();
