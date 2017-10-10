(function () {
    'use strict';

    angular.module("app").controller("NegosiasiCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'NegosiasiService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, NegosiasiService,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService) {

        var vm = this;
        vm.StepID = Number($stateParams.StepID);
        vm.TenderRefID = Number($stateParams.TenderRefID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.init = init;
        vm.NegoId = 0;
        vm.jLoad = jLoad;

        function init() {
            UIControlService.loadLoading("Silahkan Tunggu...");
            jLoad(1);

        }

        vm.jLoad = jLoad;
        function jLoad(current) {
            //console.info("curr "+current)
            vm.list = [];
            vm.nego = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            var tender = {
                Status: vm.TenderRefID,
                FilterType: vm.ProcPackType,
                column: vm.StepID
            }
            NegosiasiService.selectFirst(tender, function (reply) {
                console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.nego = reply.data;
                    //console.info("data:" + JSON.stringify(vm.nego));
                    vm.flagSum = true;
                    for (var i = 0; i < vm.nego.length; i++) {
                        if (vm.nego[i].ID !== 0) {
                            if (vm.nego[i].IsDeal !== null) {
                                console.info(vm.nego[i].IsDeal);
                                vm.flagSum = false;
                            }
                            else {
                                console.info(vm.nego[i].IsDeal);
                                vm.flagSum = true;
                            }
                            vm.list.push(vm.nego[i]);
                        }
                    }
                    vm.TenderStepDataID = vm.nego[0].TenderStepDataID;
                    cek();
                    for (var i = 0; i < vm.nego.length; i++) {
                        if (vm.nego[i].ID === 0) {
                            vm.cek_summary = true;
                            i = vm.nego.length;
                        }
                    }
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Negosiasi" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.cek = cek;
        function cek() {
            NegosiasiService.cek({
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
                templateUrl: 'app/modules/panitia/negosiasi/summary.html',
                controller: 'SummaryCtrl',
                controllerAs: 'SummaryCtrl',
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

        vm.detail = detail;
        function detail(flag) {
            if (flag === 1) {
                var data = {
                    item: vm.evaltechnical,
                    act: true
                }
                var modalInstance = $uibModal.open({
                    templateUrl: 'app/modules/panitia/evaluasi-teknis/FormEvaluator.html',
                    controller: 'FormEvaluator',
                    controllerAs: 'FormEvaluator',
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
            else if (flag === 2) {
                $state.transitionTo('evaluasi-teknis-tim', { TenderRefID: vm.TenderRefID, StepID: vm.StepID, ProcPackType: vm.ProcPackType });
            }
            else if (flag === 3) {
                $state.transitionTo('data-evaluator', { TenderRefID: vm.TenderRefID, ProcPackType: vm.ProcPackType });
            }
            else if (flag === 4) {
                var data = {
                    item: vm.evaltechnical,
                    act: false
                }
                var modalInstance = $uibModal.open({
                    templateUrl: 'app/modules/panitia/evaluasi-teknis/FormEvaluator.html',
                    controller: 'FormEvaluator',
                    controllerAs: 'FormEvaluator',
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

        vm.viewDetail = viewDetail;
        function viewDetail(data, flag) {
            if (flag === true) {
                var data = {
                    SOEPDId: data.SOEPDId,
                    VendorID: data.VendorID,
                    TenderStepDataID: vm.StepID
                }
                NegosiasiService.Insert(data,
               function (reply) {
                   if (reply.status === 200) {
                       vm.NegoId = reply.data.ID;
                       $state.transitionTo('negosiasi-chat', { NegoId: vm.NegoId, StepID: data.TenderStepDataID, VendorID: data.VendorID });
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
                $state.transitionTo('negosiasi-chat', { NegoId: data.NegoId, StepID: data.TenderStepDataID, VendorID: data.VendorID });
            }
           
        }

        vm.backpengadaan = backpengadaan;
        function backpengadaan() {
            $state.transitionTo('data-pengadaan-tahapan', { TenderRefID: vm.TenderRefID, ProcPackType: vm.ProcPackType });
        }

        vm.EndNego = EndNego;
        function EndNego(data) {
            NegosiasiService.InsertActive({
                ID: data.NegoId,
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


