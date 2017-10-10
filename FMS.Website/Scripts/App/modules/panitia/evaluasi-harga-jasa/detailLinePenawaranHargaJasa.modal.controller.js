(function () {
    'use strict';

    angular.module("app")
    .controller("detailLinePenawaranController", ctrl);

    ctrl.$inject = ['$state', 'item', '$http', '$filter', '$uibModalInstance', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'EvaluasiHargaJasaService', 'DataPengadaanService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, item, $http, $filter, $uibModalInstance, $translate, $translatePartialLoader, $location, SocketService, EvaluasiHargaJasaService, DataPengadaanService, UIControlService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";

        var soeevID = item.SOEEVID;
        vm.ceSubName = item.CESubName;
        var ceSubID = item.CESubID;

        vm.ceLineOffers = [];        

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('evaluasi-harga-jasa');
            loadData();
        };

        function loadData() {
            UIControlService.loadLoadingModal(loadmsg);
            EvaluasiHargaJasaService.getCELineOffer({
                CRCESubID: ceSubID,
                SOEEvaluationVendorID: soeevID
            }, function (reply) {
                vm.ceLineOffers = reply.data;
                vm.offerTotalCost = 0;
                vm.ceSubTotalCost = 0;
                vm.ceLineOffers.forEach(function (sub) {
                    vm.offerTotalCost += sub.LineOfferCost;
                    vm.ceSubTotalCost += sub.LineCost;
                });
                UIControlService.unloadLoadingModal();
            }, function (error) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_OFFER');
            });
        };

        vm.keluar = keluar;
        function keluar() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();