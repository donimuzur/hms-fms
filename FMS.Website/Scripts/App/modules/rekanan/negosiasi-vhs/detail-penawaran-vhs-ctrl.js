(function () {
    'use strict';

    angular.module("app").controller("DPVHSVCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'NegosiasiVHSService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, NegosiasiVHSService,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService) {

        var vm = this;
        vm.VendorID = Number($stateParams.VendorID);
        //vm.TenderRefID = Number($stateParams.TenderRefID);
        //vm.ProcPackType = Number($stateParams.ProcPackType);
        vm.StepID = Number($stateParams.StepID);
        //vm.TenderStepDataID = Number($stateParams.StepID);
        // vm.TenderStepDataID = 0;
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.init = init;
        //vm.NegoId = 0;
        vm.jLoad = jLoad;
        vm.nego = [];

        function init() {
            // UIControlService.loadLoading("Silahkan Tunggu...");
            jLoad(1);
        }
        //tampil item all
        vm.jLoad = jLoad;
        function jLoad(current) {
            console.info("curr " + current)
            vm.nego = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            var tender = {
               // VendorID: vm.VendorID,
                TenderStepDataID: vm.StepID,
                Offset: offset,
                Limit: vm.pageSize
            }
            NegosiasiVHSService.itemallv(tender, function (reply) {
                console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var jum = reply.data;
                    vm.nego = reply.data;
                    vm.TenderRefID = vm.nego[0].tender.tender.TenderRefID;
                    vm.ProcPackType = vm.nego[0].tender.tender.ProcPackageType;
                    vm.Procentage1 = vm.nego[0].Procentage;
                    vm.idtender = vm.nego[0].tender.ID;
                    vm.generet = vm.nego[0].IsGenerate;
                    vm.totalItems = Number(jum.Count);
                    vm.maxSize = vm.totalItems;
                    vm.cost = 0; vm.cosEstimate = 0; vm.totalNego = 0;
                    for (var i = 0; i < vm.nego.length; i++) {
                        if (i === 0) {
                            vm.cost = vm.nego[i].TotalPriceVOE;
                            vm.totalNego = vm.nego[i].TotalPrice;

                        }
                            else {
                            vm.cost = +vm.cost + +vm.nego[i].TotalPriceVOE;
                            vm.totalNego = +vm.totalNego + +vm.nego[i].TotalPrice;
                        }
                    }
                    var flag = 0;
                    vm.isFlag = false;
                    vm.Procentage1 = "";
                    for (var j = 0; j < vm.nego.length; j++) {
                        if (j === 0) {
                            flag = vm.nego[j].Procentage;
                        }
                        else {
                            if( flag != vm.nego[j].Procentage){
                                j = vm.nego.length - 1;
                                flag = vm.nego[j].Procentage;
                                vm.isFlag = true;
                            }
                        }
                        if (j === vm.nego.length - 1 && vm.isFlag === false) {
                            vm.Procentage1 = vm.nego[j].Procentage
                        }
                    }
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data detail penawaran" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
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
                        if (vm.lv[i].VendorID === vm.VendorID) {
                            vm.generet = vm.lv[i].IsGenerate;
                            //vm.harga = vm.lv[i].NegotiationPrice;
                        }
                    }
                    //console.info("ini:" + JSON.stringify(vm.generet));
                    //console.info("ini:" + JSON.stringify(vm.harga));
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
        vm.editp = editp;
        function editp() {
            vm.list = [];
            vm.data = {
                Procentage: vm.Procentage1,
                //ID: vm.idtender
                tender: {
                    ID: vm.idtender
                }
            }
            NegosiasiVHSService.updatev(vm.data,
                                       function (reply) {
                                           UIControlService.loadLoading("Silahkan Tunggu...");
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
        vm.per_item = per_item;
        function per_item(i) {
            vm.list = [];
            vm.data = {
                ID: vm.nego[i].ID,
                IsOpen: vm.nego[i].IsOpen,
                UnitPriceVOE: vm.nego[i].UnitPriceVOE,
                VHSNegoId: vm.nego[i].VHSNegoId,
                Procentage: vm.nego[i].Procentage,
                Quantity: vm.nego[i].Quantity
            }
            NegosiasiVHSService.updateitem(vm.data,
                                       function (reply) {
                                           UIControlService.loadLoading("Silahkan Tunggu...");
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
        vm.back = back;
        function back() {
            $state.transitionTo('negosiasi-vhs-vendor', { VHSNegoId: vm.NegoId, StepID: vm.StepID, VendorID: vm.VendorID });
        }

        vm.save = save;
        function save() {
            vm.list = [];
            for (var i = 0; i < vm.nego.length; i++) {
                if (vm.nego[i].ID !== 0) {
                    var dta = {
                        ID: vm.nego[i].ID,
                        ItemPRId: vm.nego[i].ItemPRId,
                        VOEId: vm.nego[i].VOEId,
                        IsOpen: vm.nego[i].IsOpen,
                        UnitPrice: vm.nego[i].UnitPrice,
                        VHSNegoId: vm.nego[i].VHSNegoId,
                        Quantity: vm.nego[i].Quantity,
                        tender: {
                            ID: vm.nego[i].tender.ID
                        }
                    };
                }
                else {
                    var dta = {
                        ID: vm.nego[i].ID,
                        ItemPRId: vm.nego[i].ItemPRId,
                        VOEId: vm.nego[i].VOEId,
                        IsOpen: vm.nego[i].IsOpen,
                        UnitPrice: vm.nego[i].UnitPrice,
                        VHSNegoId: vm.nego[i].VHSNegoId,
                        Quantity: vm.nego[i].Quantity,
                        tender: {
                            ID: vm.nego[i].tender.ID
                        }
                    };
                }
                vm.list.push(dta);
            }
            NegosiasiVHSService.updatedetailv(vm.list,
               function (reply) {
                   UIControlService.unloadLoadingModal();
                   if (reply.status === 200) {
                       UIControlService.msg_growl("success", "Berhasil Simpan data");
                       init();
                       //   $uibModalInstance.close();
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
    }
    })();
    //TODO


