(function () {
    'use strict';

    angular.module("app").controller("NegosiasiVHSChatVCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'NegosiasiVHSService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, NegosiasiVHSService,
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
            loadtender();

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
            NegosiasiVHSService.bychatv(tender, function (reply) {
                console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.nego = reply.data;
                    vm.VendorName = reply.data[0].VendorName;
                    vm.judul = reply.data[0].TenderName;
                    vm.VHSNegoId = reply.data[0].VHSNegoId;
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
            $state.transitionTo('detail-penawaran-vhs-vendor', { StepID: vm.StepID });
        }

        vm.loadtender = loadtender;
        function loadtender() {
            //console.info("curr " + current)
            vm.lt = [];
            //vm.currentPage = current;
            //var offset = (current * 10) - 10;
            var tend = {
                // VendorID: vm.VendorID,
                TenderStepDataID: vm.StepID
            }
            NegosiasiVHSService.itemallv(tend, function (reply) {
                console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.lt = reply.data;
                    vm.idt = vm.lt[0].tender.TenderID;
                    console.info("idt:" + JSON.stringify(vm.idt));
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data detail penawaran" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                //console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.back = back;
        function back() {
            $state.transitionTo('detail-tahapan-vendor', { TenderID: vm.idt });
        }
        //tulis, insertchat
        vm.tulis = tulis;
        function tulis() {
            var data = {
                //NegoId: vm.NegoId,
                VendorID: vm.VendorID,
                StepID: vm.StepID,
                Judul: vm.judul,
                VHSNegoId: vm.VHSNegoId
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/rekanan/negosiasi-vhs/write-chat-vhs.html',
                controller: 'WriteChatVHSVCtrl',
                controllerAs: 'WriteChatVHSVCtrl',
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


