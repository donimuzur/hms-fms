(function () {
    'use strict';

    angular.module("app").controller("NegosiasiBarangChatCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'NegosiasiBarangService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, NegosiasiBarangService,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService) {

        var vm = this;
        vm.StepID = Number($stateParams.StepID);
        //vm.GoodsNegoId = Number($stateParams.GoodsNegoId);
        vm.VendorID = Number($stateParams.VendorID);
        //vm.IsGenerate = Number($stateParams.IsGenerate);
        vm.Id = Number($stateParams.Id);
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.maxSize = 10;
        vm.init = init;
        //vm.TenderRefID = 70;
        //vm.ProcPackType = 0;
        vm.jLoad = jLoad;
        vm.nb = [];


        function init() {
            //UIControlService.loadLoading("Silahkan Tunggu...");
            //loadTender();
            jLoad(1);
            //loadv();
            loadTender();
        }
        //tampil isi chat
       vm.jLoad = jLoad;
        function jLoad(current) {
            //console.info("curr "+current)
            vm.nego = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            var tender = {
                VendorID: vm.VendorID,
                TenderStepDataID: vm.StepID
            }
            NegosiasiBarangService.bychat(tender, function (reply) {
                console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.nego = reply.data;
                    vm.VendorName = reply.data[0].VendorName;
                    vm.judul = reply.data[0].TenderName;
                    vm.idnb = vm.nego[0].GoodsNegoId
                    /*
                    vm.judul = vm.nego[0].TenderName; //buat modal
                    vm.idtender = vm.nego[0].TenderStepID; //buat detail penawaran
                    vm.idnb = vm.nego[0].GoodsNegoId; //modal juga
                   console.info("idnee:"+JSON.stringify(vm.TenderRefID));
                   */
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Chatting" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                //console.info("error:" + JSON.stringify(err));
                $.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }


        vm.loadTender = loadTender;
        function loadTender() {
            NegosiasiBarangService.selectStep({ ID: vm.StepID }, function (reply) {
                console.info("dataTender:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.step = reply.data;
                    vm.TenderRefID = vm.step.tender.TenderRefID;
                    vm.ProcPackType = vm.step.tender.ProcPackageType;
                    console.info(JSON.stringify(vm.step));
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

        //ambil data vendor
        vm.loadv = loadv;
        function loadv() {
            //console.info("curr "+current)
            vm.lv = [];
            //vm.currentPage = current;
            // var offset = (current * 10) - 10;
            NegosiasiBarangService.select({
                column: vm.StepID,
                status: vm.TenderRefID,
                FilterType: vm.ProcPackType
            }, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.lv = reply.data;
                    console.info("data:" + JSON.stringify(vm.nb));
                    //ambil data isgenerate
                    for (var i = 0; i < vm.lv.length; i++) {
                        if (vm.lv[i].ID === vm.Id) {
                            vm.VendorName = vm.lv[i].VendorName;
                            vm.judul = vm.lv[i].TenderName;
                            vm.idnb = vm.lv[i].ID;
                        }
                    }
                    console.info("ini:" + JSON.stringify(vm.VendorName));
                    console.info("ini:" + JSON.stringify(vm.judul));
                    console.info("ini:" + JSON.stringify(vm.idnb));
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
        //ke halaman detail penawaran
        vm.detailpenawaran = detailpenawaran;
        function detailpenawaran(flag) {
            $state.transitionTo('detail-penawaran', { VendorID: vm.VendorID, StepID: vm.StepID });
        }

        vm.back = back;
        function back() {
            $state.transitionTo('negosiasi-barang', { TenderRefID: vm.TenderRefID, ProcPackType: vm.ProcPackType, StepID: vm.StepID });
        }
        //tulis, insertchat
        vm.tulis = tulis;
        function tulis() {
            var data = {
                NegoId: vm.NegoId,
                VendorID: vm.VendorID,
                StepID: vm.StepID,
                Judul: vm.judul,
                GoodsNegoId: vm.idnb
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/negosiasi-barang/write-chat-barang.html',
                controller: 'WriteChatBarangCtrl',
                controllerAs: 'WriteChatBarangCtrl',
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


