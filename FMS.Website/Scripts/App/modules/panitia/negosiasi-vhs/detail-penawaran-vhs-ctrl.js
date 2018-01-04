(function () {
    'use strict';

    angular.module("app").controller("DPVHSCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'NegosiasiVHSService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, NegosiasiVHSService,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService) {

        var vm = this;
        vm.VendorID = Number($stateParams.VendorID);
        vm.TenderRefID = Number($stateParams.TenderRefID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
        vm.StepID = Number($stateParams.StepID);
        /*
        if (vm.generet === undefined) {
            vm.generet = Number($stateParams.IsGenerate);
        }
        else {
            vm.IsGenerate = Number($stateParams.IsGenerate);
            if (vm.IsGenerate === true) {
                vm.generet === true;
            }
            else if (vm.IsGenerate === false) {
                vm.generate === false;
            }
            else if (vm.IsGenerate === 0) {
                vm.generet = false;
            }
            else if (vm.IsGenerate === 1){
                vm.generet = true;
            }
        }*/
        // vm.TenderStepDataID = 0;
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.init = init;
        //vm.NegoId = 0;
        vm.jLoad = jLoad;
        vm.nego = [];
        vm.allitem = [];

        function init() {
            //loadv();
            jLoad(1);
        }
        //tampil detail penawaran
        vm.jLoad = jLoad;
        function jLoad(current) {
           // console.info("curr "+current)
            vm.nego = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            console.info("offset jload" + offset);
            console.info("currentpage" + vm.currentPage);
           var tender = {
              VendorID: vm.VendorID,
              TenderStepDataID: vm.StepID,
              Offset: offset,
              Limit: vm.pageSize
           }
            NegosiasiVHSService.itemall(tender, function (reply) {
              //console.info("rep:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var jum = reply.data;
                    vm.nego = reply.data.List;
                    vm.generet = vm.nego[0].IsGenerate;
                    vm.totalItems = Number(jum.Count);
                    vm.maxSize = vm.totalItems;
                    showAll();
                    /*
                    if (vm.IsOpenAll === true) {
                        for (var i = 0; i < vm.nego.length; i++) {
                            vm.nego[i].IsOpen = true;
                        }
                    }
                    else if (vm.IsOpenAll === false) {
                        for (var i = 0; i < vm.nego.length; i++) {
                            vm.nego[i].IsOpen = false;
                        }
                    }*/
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


        vm.saveIsOpen = saveIsOpen;
        function saveIsOpen(data) {
            console.info("data:" + JSON.stringify(data));
            var param = {
                ID: data.ID,
                VOEId: data.VOEId,
                IsOpen: data.IsOpen,
                VHSNegoId: data.VHSNegoId
            };
            vm.savedata = [];
            vm.savedata.push(param);

            NegosiasiVHSService.updatedetail(vm.savedata,
               function (reply) {
                   UIControlService.unloadLoadingModal();
                   if (reply.status === 200) {
                       UIControlService.msg_growl("success", "Berhasil Simpan data");
                       //   $uibModalInstance.close();
                       //init();
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

        vm.showAll = showAll;
        function showAll() {
            var parameter = {
                VendorID: vm.VendorID,
                TenderStepDataID: vm.StepID,
                Offset: 0,
                Limit: vm.totalItems
            }
            NegosiasiVHSService.itemall(parameter, function (reply) {
                //console.info("replyshowAll:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.allitem = reply.data.List;
                    checkIsOpenData();
                    vm.cost = 0; vm.cosEstimate = 0; vm.totalNego = 0;
                    for (var i = 0; i < vm.allitem.length; i++) {
                        if (i === 0) {
                            vm.Id = vm.allitem[i].VHSNegoId;
                            vm.cost = vm.allitem[i].TotalPriceVOE;
                            vm.totalNego = vm.allitem[i].TotalPrice;

                        }
                        else {
                            vm.cost = +vm.cost + +vm.allitem[i].TotalPriceVOE;
                            vm.totalNego = +vm.totalNego + +vm.allitem[i].TotalPrice;
                        }
                    }
                    vm.selisih = vm.cost - vm.totalNego;
                    console.info("selisih" + vm.selisih);
                    vm.presentase = (vm.selisih / vm.cost) * 100;

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

        function checkIsOpenData() {
            vm.IsOpenAll = false;
            var isOpenTrue = 0;
            for (var i = 0; i < vm.allitem.length; i++) {
                if (vm.allitem[i].IsOpen === true) {
                    isOpenTrue = isOpenTrue + 1;
                }
            }
            if (isOpenTrue === vm.allitem.length) {
                vm.IsOpenAll = true;
            }
        }

        vm.loadOpenAll = loadOpenAll;
        function loadOpenAll() {
            console.info("openall?" + JSON.stringify(vm.IsOpenAll));
            var countPaging = vm.nego.length;
            var countItemInPage = vm.totalItems - countPaging;
            var startIndex = (vm.currentPage - 1) * 10;
            if (vm.IsOpenAll === true) {
                console.info("masuk");
                for (var i = 0; i < vm.totalItems; i++) {
                    if (startIndex >= 10) {
                        if (i >= startIndex) {
                            console.info("masuk startIndex");
                            vm.nego[i - startIndex].IsOpen = true;
                        }
                    }
                    else {
                        if (i < countPaging) {
                            vm.nego[i].IsOpen = true;
                        }
                    }
                }
            }
            else if (vm.IsOpenAll === false) {
                console.info("masuk");
                for (var i = 0; i < vm.totalItems; i++) {
                    if (startIndex >= 10) {
                        if (i >= startIndex) {
                            console.info("masuk startIndex");
                            vm.nego[i - startIndex].IsOpen = false;
                        }
                    }
                    else {
                        if (i < countPaging) {
                            vm.nego[i].IsOpen = false;
                        }
                    }
                }
            }
            /*
            var parameter = {
                VendorID: vm.VendorID,
                TenderStepDataID: vm.StepID,
                Offset: 0,
                Limit: vm.totalItems
            }
            //showAll(parameter);
            //console.info("loadAll:" + JSON.stringify(vm.allitem));*/
        }
        
        vm.loadv = loadv;
        function loadv() {
            //console.info("curr "+current)
            vm.lv = [];
            //vm.currentPage = current;
           // var offset = (current * 10) - 10;
            NegosiasiVHSService.select({
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
            console.info("isopenall?" + vm.IsOpenAll);
            if (vm.IsOpenAll === true) {
                for (var i = 0; i < vm.totalItems; i++) {
                    var param = {
                        ID: vm.allitem[i].ID,
                        VOEId: vm.allitem[i].VOEId,
                        IsOpen: true,
                        VHSNegoId: vm.allitem[i].VHSNegoId
                    };
                    vm.list.push(param);
                }
            }
            else if (vm.IsOpenAll === false) {
                for (var i = 0; i < vm.totalItems; i++) {
                    var param = {
                        ID: vm.allitem[i].ID,
                        VOEId: vm.allitem[i].VOEId,
                        IsOpen: false,
                        VHSNegoId: vm.allitem[i].VHSNegoId
                    };
                    vm.list.push(param);
                }
            }
           // console.info("allItem" + JSON.stringify(vm.list));
            NegosiasiVHSService.updatedetail(vm.list,
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
            /*
            for (var i = 0; i < vm.nego.length; i++) {
                if (vm.nego[i].ID !== 0) {
                    var dta = {
                        ID: vm.nego[i].ID,
                        VOEId: vm.nego[i].VOEId,
                        IsOpen: vm.nego[i].IsOpen,
                        VHSNegoId: vm.nego[i].VHSNegoId
                    };
                }
                else {
                    var dta = {
                        ID: 0,
                        VOEId: vm.nego[i].VOEId,
                        IsOpen: vm.nego[i].IsOpen,
                        VHSNegoId: vm.nego[i].VHSNegoId
                    };
                }
                vm.list.push(dta);
            }
            NegosiasiVHSService.updatedetail(vm.list,
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
          );*/
        }
        vm.back = back;
        function back() {
            $state.transitionTo('negosiasi-vhs-chat', { VHSNegoId: vm.Id, StepID: vm.StepID, VendorID: vm.VendorID, Id: vm.Id, ProcPackType: vm.ProcPackType, TenderRefID: vm.TenderRefID });
        }


        vm.generate = generate;
        function generate() {
            /*
            var isg = {
                ID: vm.Id,
                IsGenerate: vm.generet
            };*/
            NegosiasiVHSService.update({
                ID: vm.Id,
                IsGenerate: vm.generet
            },
            function (reply) {
                //UIControlService.loadLoading("Silahkan Tunggu...");
                console.info("gnrt:" + JSON.stringify(reply.status));
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "Berhasil Simpan data");
                    // vm.generet = isg.IsGenerate;
                    //console.info("gnrt:" + JSON.stringify(vm.generet));
                    init();
                    //jLoad(1);
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


