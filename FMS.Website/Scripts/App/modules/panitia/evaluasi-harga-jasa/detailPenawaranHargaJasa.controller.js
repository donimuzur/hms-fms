(function () {
    'use strict';

    angular.module("app")
    .controller("detailPenawaranController", ctrl);

    ctrl.$inject = ['$state', '$stateParams', '$http', '$filter', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'EvaluasiHargaJasaService', 'DataPengadaanService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $stateParams, $http, $filter, $uibModal, $translate, $translatePartialLoader, $location, SocketService, EvaluasiHargaJasaService, DataPengadaanService, UIControlService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";

        vm.stepID = Number($stateParams.StepID);
        vm.tenderRefID = Number($stateParams.TenderRefID);
        vm.procPackType = Number($stateParams.ProcPackType);
        vm.soeevID = Number($stateParams.SOEEVID);

        vm.tenderStepData = {};
        vm.ceSubOffers = [];
        vm.vendorName;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('evaluasi-harga-jasa');
            loadData();
        };

        function loadData() {
            UIControlService.loadLoading(loadmsg);
            DataPengadaanService.GetStepByID({
                ID: vm.stepID
            }, function (reply) {
                vm.tenderStepData = reply.data;
                EvaluasiHargaJasaService.getCESubOffer({
                    CRCESubID: 0,
                    SOEEvaluationVendorID: vm.soeevID
                }, function (reply) {
                    vm.ceSubOffers = reply.data;
                    vm.offerTotalCost = 0;
                    vm.ceSubTotalCost = 0;
                    vm.ceSubOffers.forEach(function (sub) {
                        vm.offerTotalCost += sub.OfferTotalCost;
                        vm.ceSubTotalCost += sub.TotalLineCost;
                    });
                    EvaluasiHargaJasaService.getOEEVInfo({
                        ID: vm.soeevID,
                    }, function (reply) {
                        vm.vendorName = reply.data.VendorName;
                        UIControlService.unloadLoading();
                    }, function (error) {
                        UIControlService.unloadLoading();
                        UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_OFFER');
                    });
                }, function (error) {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_OFFER');
                });
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
            });
        };       

        vm.detailSubPenawaran = detailSubPenawaran;
        function detailSubPenawaran(ceSub) {
            var item = {
                SOEEVID: vm.soeevID,
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
    }
})();