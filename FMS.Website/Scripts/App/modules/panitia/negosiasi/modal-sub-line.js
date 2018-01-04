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

        vm.init = init;
        function init() {
            console.info(vm.detail);
            checkIsOpen();
        }

        vm.checkIsOpen = checkIsOpen;
        function checkIsOpen() {
            vm.IsOpenAll = false;
            var isOpenTrue = 0;
            for (var i = 0; i < vm.detail.childern.length; i++) {
                if (vm.detail.childern[i].IsOpen === true) {
                    isOpenTrue = isOpenTrue + 1;
                }
            }
            if (isOpenTrue === vm.detail.childern.length) {
                vm.IsOpenAll = true;
            }
        }

        vm.saveIsOpen = saveIsOpen;
        function saveIsOpen(data) {
            console.info("data:" + JSON.stringify(data));
            var param = {
                IsOpen: data.IsOpen,
                ID: data.ID,
                NegoId: data.NegoId,
                CRCESubDetailId: data.CRCESubDetailId,
                SOEPDId: data.SOEPDId,
                UnitNegotiationPrice: data.UnitNegotiationPrice,
                quantity: data.quantity
            };
            vm.savedata = [];
            vm.savedata.push(param);

            NegosiasiService.InsertOpen(vm.savedata,
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

        vm.jLoad = jLoad;
        function jLoad(current) {
            vm.list = [];
            EvaluationTechnicalService.selectByEmployee({
                Keyword: vm.detail.EmployeeID
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.list = data;
                    console.info("data:" + JSON.stringify(vm.list));
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


        vm.loadOpenAll = loadOpenAll;
        function loadOpenAll() {
            //tanpa paging
            console.info("leng" + JSON.stringify(vm.detail.childern.length));
            if (vm.IsOpenAll === true) {
                console.info("bnr");
                for (var i = 0; i < vm.detail.childern.length; i++) {
                    vm.detail.childern[i].IsOpen = true;
                }
            }
            else if (vm.IsOpenAll === false) {
                console.info("salah");
                for (var i = 0; i < vm.detail.childern.length; i++) {
                    vm.detail.childern[i].IsOpen = false;
                }
            }
            save();
            
        }

        vm.batal = batal;
        function batal() {
            //$uibModalInstance.dismiss('cancel');
            $uibModalInstance.close();
        };

        vm.save = save;
        function save() {
            vm.list = [];
            for (var i = 0; i < vm.detail.childern.length; i++) {
                if (vm.detail.childern[i].ID !== 0) {
                    var dt = {
                        IsOpen: vm.detail.childern[i].IsOpen,
                        ID: vm.detail.childern[i].ID,
                        NegoId:vm.detail.childern[i].NegoId,
                        CRCESubDetailId: vm.detail.childern[i].CRCESubDetailId,
                        SOEPDId: vm.detail.childern[i].SOEPDId,
                        UnitNegotiationPrice: vm.detail.childern[i].UnitNegotiationPrice,
                        quantity: vm.detail.childern[i].quantity
                    };
                }
                else {
                    var dt = {
                        IsOpen: vm.detail.childern[i].IsOpen,
                        NegoId: vm.detail.childern[i].NegoId,
                        CRCESubDetailId: vm.detail.childern[i].CRCESubDetailId,
                        SOEPDId: vm.detail.childern[i].SOEPDId,
                        UnitNegotiationPrice: vm.detail.childern[i].UnitNegotiationPrice,
                        quantity: vm.detail.childern[i].quantity
                    };
                }
                vm.list.push(dt);
            }
            NegosiasiService.InsertOpen(vm.list,
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