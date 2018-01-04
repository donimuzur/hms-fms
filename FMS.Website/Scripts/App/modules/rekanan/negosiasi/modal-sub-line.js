(function () {
    'use strict';

    angular.module("app")
    .controller("DetailSubLineCtrl", ctrl);

    ctrl.$inject = ['item', '$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'NegosiasiService', '$state', 'UIControlService', '$uibModal', 'GlobalConstantService', '$stateParams', '$uibModalInstance'];
    function ctrl(item, $http, $translate, $translatePartialLoader, $location, SocketService, NegosiasiService,
        $state, UIControlService, $uibModal, GlobalConstantService, $stateParams, $uibModalInstance) {
        var vm = this;
        vm.detail = item.item;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.textSearch = '';
        vm.listPengumuman = [];
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.TenderRefID = item.TenderRefID;
        vm.ProcPackType = item.ProcPackType;

        vm.init = init;
        function init() {
            console.info(JSON.stringify(vm.detail));
            jLoad(1);
        }
        vm.jLoad = jLoad;
        function jLoad(current) {
            vm.list = [];
            NegosiasiService.selectByVendor({
                Keyword: vm.detail.EmployeeID
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.list = data;
                    console.info("data subline:" + JSON.stringify(vm.list));
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Penilai" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.batal = batal;
        function batal() {
            //$uibModalInstance.dismiss('cancel');
            $uibModalInstance.close();
        };

        vm.per_item = per_item;
        function per_item(i) {
            vm.list = [];
            vm.data = {
                ID: vm.detail.childern[i].ID,
                IsOpen: vm.detail.childern[i].IsOpen,
                UnitVendorPrice: vm.detail.childern[i].UnitVendorPrice,
                NegoId: vm.detail.childern[i].NegoId,
                Procentage: vm.detail.childern[i].Procentage,
                quantity: vm.detail.childern[i].quantity
            }
            NegosiasiService.updateitem(vm.data,
                                       function (reply) {
                                           UIControlService.loadLoading("Silahkan Tunggu...");
                                           if (reply.status === 200) {
                                               UIControlService.msg_growl("success", "Berhasil Simpan data");
                                               NegosiasiService.CekDetail({
                                                   ID: vm.detail.childern[i].ID
                                               }, function (reply) {
                                                   UIControlService.unloadLoading();
                                                   if (reply.status === 200) {
                                                       var data = reply.data;
                                                       vm.detail.childern[i].UnitNegotiationPrice = data.UnitNegotiationPrice;
                                                       vm.detail.childern[i].TotalPriceNego = data.TotalPriceNego;
                                                   } else {
                                                       $.growl.error({ message: "Gagal mendapatkan data Penilai" });
                                                       UIControlService.unloadLoading();
                                                   }
                                               }, function (err) {
                                                   console.info("error:" + JSON.stringify(err));
                                                   //$.growl.error({ message: "Gagal Akses API >" + err });
                                                   UIControlService.unloadLoading();
                                               });
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

        vm.save = save;
        function save() {
            NegosiasiService.InsertDetailVendor(vm.detail,
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
                   UIControlService.unloadLoadingModal();
               }
          );
        }

    }
}
)();