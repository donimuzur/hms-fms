(function () {
    'use strict';

    angular.module("app").controller("ListSubNego", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'NegosiasiService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, NegosiasiService,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService) {

        var vm = this;
        vm.VendorID = Number($stateParams.VendorID);
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
            vm.nego = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            var tender = {
                Status: vm.TenderRefID,
                FilterType: vm.ProcPackType,
                column: vm.VendorID
            }
            NegosiasiService.selectVendor(tender, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.nego = reply.data;
                    console.info("data:" + JSON.stringify(vm.nego));
                    vm.Generate = vm.nego[0].IsGenerate;
                    console.info(vm.nego[0].IsGenerate);
                    vm.cost = 0; vm.cosEstimate = 0; vm.totalNego = 0;
                    for (var i = 0; i < vm.nego.length; i++) {
                        if (i === 0) {
                            vm.cost = vm.nego[i].TotalCost;
                            vm.cosEstimate = vm.nego[i].TotalCostEstimate;
                            vm.totalNego = vm.nego[i].TotalNegotiation;

                        }
                        else {
                            vm.cost = +vm.cost + +vm.nego[i].TotalCost;
                            vm.cosEstimate = +vm.cosEstimate + +vm.nego[i].TotalCostEstimate;
                            vm.totalNego = +vm.totalNego + +vm.nego[i].TotalNegotiation;
                        }
                        if (i === (vm.nego.length - 1)) {
                            vm.total = (((vm.cost - vm.cosEstimate) / vm.cost) * 100).toFixed(2);
                            if (vm.total < 0) vm.total *= -1;
                            
                        }
                    }
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Evaluasi Teknis" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.detail = detail;
        function detail(data) {
            if (data.Parent == 0)
            {
                var data = {
                    item: data
                }
                var modalInstance = $uibModal.open({
                    templateUrl: 'app/modules/panitia/negosiasi/modal-sub-parent.html',
                    controller: 'DetailSubParentCtrl',
                    controllerAs: 'DetailSubParentCtrl',
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
            else {
                var data = {
                    item: data
                }
                var modalInstance = $uibModal.open({
                    templateUrl: 'app/modules/panitia/negosiasi/modal-sub-line.html',
                    controller: 'DetailSubLineCtrl',
                    controllerAs: 'DetailSubLineCtrl',
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
                    VendorID: data.VendorID,
                    TenderStepDataID: data.TenderStepDataID
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

        }

        vm.Approval = Approval;
        function Approval() {
            var data = {
                item: vm.TenderRefID
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/temporary/safetyEvaluation/DetailApproval.html',
                controller: 'DetailApprovalCtrl',
                controllerAs: 'DetailApprovalCtrl',
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

        vm.back = back;
        function back() {
            $state.transitionTo('negosiasi-chat', { NegoId:vm.nego[0].NegoId, VendorID: vm.nego[0].VendorID, StepID: vm.nego[0].TenderStepDataID });
        }

        vm.generate = generate;
        function generate() {
                vm.Insert = {
                    IsGenerate :vm.Generate,
                    SOEPDId: vm.nego[0].SOEPDId,
                    TenderStepDataID: vm.nego[0].TenderStepDataID,
                    VendorID: vm.nego[0].VendorID
                };
            NegosiasiService.InsertByPersen(vm.Insert,
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
        
    }
})();
//TODO


