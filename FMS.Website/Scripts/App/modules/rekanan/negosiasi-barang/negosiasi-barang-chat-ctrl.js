(function () {
    'use strict';

    angular.module("app").controller("NegosiasiBarangChatVCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'NegosiasiBarangService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, NegosiasiBarangService,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService) {

        var vm = this;
        vm.StepID = Number($stateParams.StepID);
        //vm.VHSNegoId = Number($stateParams.VHSNegoId);
        //vm.VendorID = Number($stateParams.VendorID);;
        vm.TenderRefID = Number($stateParams.TenderRefID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.maxSize = 10;
        vm.init = init;
        vm.ProcPackType = 0;
        vm.jLoad = jLoad;
        vm.nb = [];

        function init() {
            //UIControlService.loadLoading("Silahkan Tunggu...");
            //loadTender();
            jLoad(1);
            //loadtender();

        }
        //tampil isi chat
       vm.jLoad = jLoad;
        function jLoad(current) {
            console.info("curr "+current)
            vm.nego = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            var tender = {
                //VendorID: vm.VendorID,
                TenderStepDataID: vm.StepID
            }
            NegosiasiBarangService.bychatv(tender, function (reply) {
                console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.nego = reply.data;
                    vm.VendorName = reply.data[0].VendorName;
                    vm.judul = reply.data[0].TenderName;
                    vm.idnb = vm.nego[0].GoodsNegoId
                    //vm.judul = vm.nego[0].TenderName; //buat modal
                    //vm.idtender = vm.nego[0].TenderStepID; //buat detail penawaran
                    //vm.idnb = vm.nego[0].VHSNegoId; //modal juga
                    //vm.VendorID = vm.nego[0].VendorID;
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
        //ke halaman itemall
        vm.detailpenawaran = detailpenawaran;
        function detailpenawaran(flag) {
            $state.transitionTo('detail-penawaran-vendor', { StepID: vm.StepID });
        }

        vm.back = back;
        function back() {
            $state.transitionTo('detail-tahapan-vendor', { TenderID: vm.TenderRefID });
        }
        //tulis, insertchat
        vm.tulis = tulis;
        function tulis() {
            var data = {
                StepID: vm.StepID,
                Judul: vm.judul,
                GoodsNegoId: vm.idnb
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/rekanan/negosiasi-barang/write-chat-barang.html',
                controller: 'WriteChatBarangVCtrl',
                controllerAs: 'WriteChatBarangVCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                init();
            });
        }

    }
})();
//TODO


