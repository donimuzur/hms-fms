(function () {
    'use strict';

    angular.module("app").controller("DPCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'NegosiasiBarangService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, NegosiasiBarangService,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService) {

        var vm = this;
        vm.VendorID = Number($stateParams.VendorID);
        vm.TenderRefID = Number($stateParams.TenderRefID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
        vm.StepID = Number($stateParams.StepID);
        //vm.Id = Number($stateParams.Id);
        // vm.TenderStepDataID = 0;
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.init = init;
        //vm.NegoId = 0;
        vm.jLoad = jLoad;
        vm.nego = [];

        function init() {
           // loadv();
            jLoad(1);
        }
        //tampil detail penawaran
        vm.jLoad = jLoad;
        function jLoad(current) {
           // console.info("curr "+current)
            vm.nego = [];
            vm.currentPage = current;
           var offset = (current * 10) - 10;
           var tender = {
              VendorID: vm.VendorID,
              TenderStepDataID: vm.StepID
           }
            NegosiasiBarangService.itemall(tender, function (reply) {
              console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.nego = reply.data;
                    vm.Id = vm.nego[0].GoodsNegoId;
                    vm.TenderRefID = vm.nego[0].tender.tender.TenderRefID;
                    vm.ProcPackType = vm.nego[0].tender.tender.ProcPackageType;
                    vm.generet = vm.nego[0].IsGenerate;
                    vm.totalItems = vm.nego.length;
                    vm.IsOpenAll = false;
                    checkIsOpen();
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data detail penawaran" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                //console.info("error:" + JSON.stringify(err));
                $.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.checkIsOpen = checkIsOpen;
        function checkIsOpen() {
            vm.IsOpenAll = false;
            console.info("jml:" + JSON.stringify(vm.totalItems));
            var isOpenTrue = 0;
            for (var i = 0; i < vm.nego.length; i++) {
                if (vm.nego[i].IsOpen === true) {
                    isOpenTrue = isOpenTrue + 1;
                }
            }
            if (isOpenTrue === vm.totalItems) {
                vm.IsOpenAll = true;
            }
        }
        
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
                    //console.info("data:" + JSON.stringify(vm.nb));
                    //ambil data isgenerate
                    for (var i = 0; i < vm.lv.length; i++) {
                        if (vm.lv[i].ID === vm.Id && vm.lv[i].VendorID === vm.VendorID) {
                            vm.generet = vm.lv[i].IsGenerate;
                            vm.harga = vm.lv[i].NegotiationPrice;
                        }
                    }
                    console.info("ini:" + JSON.stringify(vm.generet));
                    console.info("ini:" + JSON.stringify(vm.harga));
                    //console.info("ini:" + JSON.stringify(vm.vendor));
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
        
        vm.save = save;
        function save() {
            vm.list = [];
            for (var i = 0; i < vm.nego.length; i++) {
                if (vm.nego[i].ID !== 0) {
                    var dta = {
                        IsOpen: vm.nego[i].IsOpen,
                        ID: vm.nego[i].ID,
                        ItemPRId: vm.nego[i].ItemPRId,
                        GOEId: vm.nego[i].GOEId,
                        UnitPrice: vm.nego[i].UnitPrice,
                        GoodsNegoId: vm.nego[i].GoodsNegoId,
                        Quantity: vm.nego[i].Quantity
                    };
                }
                else {
                    var dta = {
                        IsOpen: vm.nego[i].IsOpen,
                        ID: vm.nego[i].ID,
                        ItemPRId: vm.nego[i].ItemPRId,
                        GOEId: vm.nego[i].GOEId,
                        UnitPrice: vm.nego[i].UnitPrice,
                        GoodsNegoId: vm.nego[i].GoodsNegoId,
                        Quantity: vm.nego[i].Quantity
                    };
                }
                vm.list.push(dta);
            }
            NegosiasiBarangService.updatedetail(vm.list,
               function (reply) {
                   UIControlService.unloadLoadingModal();
                   if (reply.status === 200) {
                       UIControlService.msg_growl("success", "Berhasil Simpan data");
                       //   $uibModalInstance.close();
                       init();
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
        vm.back = back;
        function back() {
            $state.transitionTo('negosiasi-barang-chat', { StepID: vm.StepID, VendorID: vm.VendorID });
        }

        vm.saveIsOpen = saveIsOpen;
        function saveIsOpen(data) {
            console.info("data:" + JSON.stringify(data));
            var param = {
                IsOpen: data.IsOpen,
                ID: data.ID,
                ItemPRId: data.ItemPRId,
                GOEId: data.GOEId,
                UnitPrice: data.UnitPrice,
                GoodsNegoId: data.GoodsNegoId,
                Quantity: data.Quantity
            };
            vm.savedata = [];
            vm.savedata.push(param);

            NegosiasiBarangService.updatedetail(vm.savedata,
               function (reply) {
                   UIControlService.unloadLoadingModal();
                   if (reply.status === 200) {
                       UIControlService.msg_growl("success", "Berhasil Simpan data");
                       init();
                   }
                   else {
                       UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                       return;
                   }
               },
               function (err) {
                   UIControlService.msg_growl("error", "Gagal Akses Api!!");
               }
          );


        }

        vm.loadOpenAll = loadOpenAll;
        function loadOpenAll() {
            //tanpa paging
            console.info("openall?" + JSON.stringify(vm.IsOpenAll));
            var countPaging = vm.nego.length;
            var countItemInPage = vm.totalItems - countPaging;
            var startIndex = (vm.currentPage - 1) * 10;
            if (vm.IsOpenAll === true) {
                console.info("bnr");
                for (var i = 0; i < vm.totalItems; i++) {
                     vm.nego[i].IsOpen = true;
                }
            }
            else if (vm.IsOpenAll === false) {
                console.info("salah");
                for (var i = 0; i < vm.totalItems; i++) {
                    vm.nego[i].IsOpen = false;
                }
            }
        }


        vm.generate = generate;
        function generate() {
            NegosiasiBarangService.update({
                ID: vm.Id,
                IsGenerate: vm.generet
            },
            function (reply) {
                //UIControlService.loadLoading("Silahkan Tunggu...");
                console.info("gnrt:" + JSON.stringify(reply.status));
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "Berhasil Simpan data");
                    init();
                }
                else {
                    UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                    return;
                }
            },
            function (err) {
                UIControlService.msg_growl("error", "Gagal Akses Api!!");
            }
            );
            
            }
        
    }
})();
//TODO


