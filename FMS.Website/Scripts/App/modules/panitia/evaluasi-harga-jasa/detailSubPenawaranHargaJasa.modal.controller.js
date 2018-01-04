(function () {
    'use strict';

    angular.module("app")
    .controller("detailSubPenawaranController", ctrl);

    ctrl.$inject = ['$state', 'item', '$http', '$filter', '$uibModal', '$uibModalInstance', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'EvaluasiHargaJasaService', 'DataPengadaanService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, item, $http, $filter, $uibModal, $uibModalInstance, $translate, $translatePartialLoader, $location, SocketService, EvaluasiHargaJasaService, DataPengadaanService, UIControlService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";

        var soeevID = item.SOEEVID;
        vm.ceSubName = item.CESubName;
        var ceSubID = item.CESubID;

        vm.tenderStepData = {};
        vm.ceSubOffers = [];
        

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('evaluasi-harga-jasa');
            loadData();
        };

        function loadData() {
            UIControlService.loadLoadingModal(loadmsg);
            EvaluasiHargaJasaService.getCESubOffer({
                CRCESubID: ceSubID,
                SOEEvaluationVendorID: soeevID
            }, function (reply) {
                vm.ceSubOffers = reply.data;
                vm.offerTotalCost = 0;
                vm.ceSubTotalCost = 0;
                vm.ceSubOffers.forEach(function (sub) {
                    vm.offerTotalCost += sub.OfferTotalCost;
                    vm.ceSubTotalCost += sub.TotalLineCost;
                });
                UIControlService.unloadLoadingModal();
            }, function (error) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_OFFER');
            });
        };

        vm.detailSubPenawaran = detailSubPenawaran;
        function detailSubPenawaran(ceSub) {
            var item = {
                SOEEVID: soeevID,
                CESubName: ceSub.Name,
                CESubID: ceSub.ContractRequisitionCESubID
            };
            if (ceSub.HasChildren) {
                var modalInstance = $uibModal.open({
                    templateUrl: 'app/modules/panitia/evaluasi-harga-jasa/detailSubPenawaranHargaJasa.modal.html',
                    controller: 'detailSubPenawaranController',
                    controllerAs: 'dspCtrl',
                    resolve: { item: function () { return item; } }
                });
                modalInstance.result.then(function () {
                    loadData();
                });
            } else {
                var modalInstance = $uibModal.open({
                    templateUrl: 'app/modules/panitia/evaluasi-harga-jasa/detailLinePenawaranHargaJasa.modal.html',
                    controller: 'detailLinePenawaranController',
                    controllerAs: 'dlpCtrl',
                    resolve: { item: function () { return item; } }
                });
                modalInstance.result.then(function () {
                    loadData();
                });
            }
        };

        vm.keluar = keluar;
        function keluar() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();