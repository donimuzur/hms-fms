(function () {
    'use strict';

    angular.module("app").controller("NegosiasiBarangCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'NegosiasiBarangService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, NegosiasiBarangService,
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
            UIControlService.loadLoading("Silahkan Tunggu...");
            jLoad(1);

        }
        //tampil data forum
        vm.jLoad = jLoad;
        function jLoad(current) {
            vm.list = [];
            //console.info("curr "+current)
            vm.nb = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            NegosiasiBarangService.select({
                column: vm.StepID,
                status: vm.TenderRefID,
                FilterType: vm.ProcPackType
            }, function (reply) {
               console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.nb = reply.data;
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
                    vm.TenderStepDataID = vm.StepID;
                    cek();
                    for (var i = 0; i < vm.nb.length; i++) {
                        if (vm.nb[i].ID === 0) {
                            vm.cek_summary = true;
                            i = vm.nb.length;
                        }
                    }
                   //console.info("data:" + JSON.stringify(vm.TenderStepDataID));
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
        vm.cek = cek;
        function cek() {
            NegosiasiBarangService.cek({
                TenderStepDataID: vm.TenderStepDataID
            }, function (reply) {
                console.info("data:"+JSON.stringify(reply));
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
                item : vm.list
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/negosiasi-barang/summary.html',
                controller: 'SummaryBarangCtrl',
                controllerAs: 'SummaryBarangCtrl',
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
        //detail chat
        vm.viewDetail = viewDetail;
        function viewDetail(data, flag) {
            if (flag === true) {
                var datainsert = {
                    VendorID: data.VendorID,
                    TenderStepDataID: vm.StepID
                }
               NegosiasiBarangService.insert(datainsert,
               function (reply) {
                   //console.info("data:"+JSON.stringify(reply));
                   if (reply.status === 200) {
                       vm.GoodsNegoId = data.ID;
                       //console.info("sini" + JSON.stringify(vm.GoodsNegoId));
                       $state.transitionTo('negosiasi-barang-chat', {StepID: data.TenderStepDataID, VendorID: data.VendorID });
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
                //console.info("situ" + JSON.stringify(data.ID));
                $state.transitionTo('negosiasi-barang-chat', {StepID: data.TenderStepDataID, VendorID: data.VendorID, ProcPackType: vm.ProcPackType, TenderRefID: vm.TenderRefID });
            }
           
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
            NegosiasiBarangService.editactive({
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


