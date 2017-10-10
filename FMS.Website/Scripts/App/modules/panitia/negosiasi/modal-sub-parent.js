(function () {
    'use strict';

    angular.module("app")
    .controller("DetailSubParentCtrl", ctrl);

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
            for (var i = 0; i < vm.detail.childern.length; i++) {
                console.info("sss");
                if (i === 0) {
                    vm.cost = vm.detail.childern[i].TotalCost;
                    vm.cosEstimate = vm.detail.childern[i].TotalCostEstimate;
                    vm.totalNego = vm.detail.childern[i].TotalNegotiation;
                    console.info(vm.cost);
                    console.info(vm.cosEstimate);
                    console.info(vm.totalNego);

                }
                else {
                    vm.cost = +vm.cost + +vm.detail.childern[i].TotalCost;
                    vm.cosEstimate = +vm.cosEstimate + +vm.detail.childern[i].TotalCostEstimate;
                    vm.totalNego = +vm.totalNego + +vm.detail.childern[i].TotalNegotiation;
                }
                if (i === (vm.detail.childern.length - 1)) {
                    vm.total = (((vm.cosEstimate - vm.cost) / vm.cosEstimate) * 100).toFixed(2);

                }
            }
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

        vm.batal = batal;
        function batal() {
            //$uibModalInstance.dismiss('cancel');
            $uibModalInstance.close();
        };

        vm.save = save;
        function save() {
            if (vm.detail.ID !== 0) {
                var dt = {
                    ID: vm.detail.ID,
                    NegoId: vm.detail.NegoId,
                    CRCESubDetailId: vm.detail.CRCESubDetailId,
                    SOEPDId: vm.detail.SOEPDId,
                    UnitNegotiationPrice: vm.detail.UnitNegotiationPrice,
                    quantity: vm.detail.quantity
                };
            }
            else {
                var dt = {
                    NegoId: vm.detail.NegoId,
                    CRCESubDetailId: vm.detail.CRCESubDetailId,
                    SOEPDId: vm.detail.SOEPDId,
                    UnitNegotiationPrice: vm.detail.UnitNegotiationPrice,
                    quantity: vm.detail.quantity
                };
            }
            NegosiasiService.InsertDetail(dt,
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

        vm.detailLine = detailLine;
        function detailLine(data){
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
}
)();