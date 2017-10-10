(function () {
    'use strict';

    angular.module("app").controller("NegosiasiVHSCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'NegosiasiVHSService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, NegosiasiVHSService,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService) {

        var vm = this;
        vm.StepID = Number($stateParams.StepID);
        vm.TenderRefID = Number($stateParams.TenderRefID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.init = init;
        vm.jLoad = jLoad;
        vm.nb = [];
        vm.GoodsNegoId = 0;
        vm.generate = 7;

        function init() {
           // UIControlService.loadLoading("Silahkan Tunggu...");
            jLoad(1);

        }
        //tampil data forum
        vm.jLoad = jLoad;
        function jLoad(current) {
            //console.info("curr "+current)
            vm.nb = [];
            vm.list = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            NegosiasiVHSService.select({
                column: vm.StepID,
                status: vm.TenderRefID,
                FilterType: vm.ProcPackType
            }, function (reply) {
               console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.nb = reply.data;
                    console.info("data:" + JSON.stringify(vm.nb));
                    vm.flagSum = true;
                    for (var i = 0; i < vm.nb.length; i++) {
                        if (vm.nb[i].ID !== 0) {
                            if (vm.nb[i].IsDeal !== null) {
                                console.info(vm.nb[i].IsDeal);
                                vm.flagSum = false;
                            }
                            else {
                                console.info(vm.nb[i].IsDeal);
                                vm.flagSum = true;
                            }
                            vm.list.push(vm.nb[i]);
                        }
                    }
                    vm.TenderStepDataID = vm.nb[0].TenderStepDataID;
                    cek();
                    for (var i = 0; i < vm.nb.length; i++) {
                        if (vm.nb[i].ID === 0) {
                            vm.cek_summary = true;
                            i = vm.nb.length;
                        }
                    }
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
        //btn kembali
        vm.backpengadaan = backpengadaan;
        function backpengadaan() {
            $state.transitionTo('data-pengadaan-tahapan', { TenderRefID: vm.TenderRefID, ProcPackType: vm.ProcPackType });
        }
        //detail chat
        vm.viewDetail = viewDetail;
        function viewDetail(data, flag) {
            if (flag === true) {
                var datainsert = {
                    VendorID: data.VendorID,
                    TenderStepDataID: data.TenderStepDataID
                }
               NegosiasiVHSService.insert(datainsert,
               function (reply) {
                   if (reply.status === 200) {
                       vm.VHSNegoId = reply.data.ID;
                       console.info("sini" + JSON.stringify(data.ID));
                       $state.transitionTo('negosiasi-vhs-chat', { VHSNegoId: vm.VHSNegoId, StepID: data.TenderStepDataID, VendorID: data.VendorID, Id: vm.VHSNegoId, ProcPackType: vm.ProcPackType, TenderRefID: vm.TenderRefID });
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
            else {
                console.info("situ" + JSON.stringify(data.ID));
                $state.transitionTo('negosiasi-vhs-chat', { VHSNegoId: data.ID, StepID: data.TenderStepDataID, VendorID: data.VendorID, Id: data.ID, ProcPackType: vm.ProcPackType, TenderRefID: vm.TenderRefID });
            }
           
        }

        vm.cek = cek;
        function cek() {
            NegosiasiVHSService.cek({
                TenderStepDataID: vm.TenderStepDataID
            }, function (reply) {
                console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.hasil_cek = reply.data;
                    //console.info("data:" + JSON.stringify(vm.hasil_cek));
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

        //modal summary
        vm.do_summary = do_summary;
        function do_summary() {
            var data = {
                item: vm.list
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/negosiasi-vhs/summary.html',
                controller: 'SummaryVHSCtrl',
                controllerAs: 'SummaryVHSCtrl',
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

        //btn end nego
        /*
        vm.endnego = endnego;
        function endnego(data) {
            var tnd = {
                VendorID: 2060,
                TenderStepDataID: 70
            }
            NegosiasiBarangService.editactive(tnd, function (reply) {
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
                 // console.info("data:" + JSON.stringify(TenderStepID));
                 // console.info("data:" + JSON.stringify(vm.StepID));
                  console.info("error:" + JSON.stringify(err));
                  UIControlService.msg_growl("error", "Gagal Akses Api!!");
              }
         );
        }
        */
        vm.endnego = endnego;
        function endnego(data) {
            NegosiasiVHSService.editactive({
                ID: data.ID,
                IsActive: false
            },
              function (reply) {
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


