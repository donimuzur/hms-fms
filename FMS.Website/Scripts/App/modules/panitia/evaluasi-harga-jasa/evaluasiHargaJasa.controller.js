(function () {
    'use strict';

    angular.module("app")
    .controller("evaluasiHargaJasaController", ctrl);

    ctrl.$inject = ['$state', '$stateParams', '$http', '$filter', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'EvaluasiHargaJasaService', 'DataPengadaanService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $stateParams, $http, $filter, $uibModal, $translate, $translatePartialLoader, $location, SocketService, EvaluasiHargaJasaService, DataPengadaanService, UIControlService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";

        vm.stepID = Number($stateParams.StepID);
        vm.tenderRefID = Number($stateParams.TenderRefID);
        vm.procPackType = Number($stateParams.ProcPackType);

        vm.evaluasi = {};
        vm.tenderStepData = {};

        vm.isProcess = false;

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
                vm.isProcess = vm.tenderStepData.StatusName === "PROCUREMENT_TYPE_PROCESS";
                EvaluasiHargaJasaService.getByTenderStepData({
                    ID: vm.stepID
                }, function (reply) {
                    UIControlService.unloadLoading();
                    vm.evaluasi = reply.data;
                }, function (error) {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
                });
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
            });
        };

        vm.lakukanEvaluasi = lakukanEvaluasi;
        function lakukanEvaluasi() {

            var evaluationVendor = [];
            if (vm.evaluasi.ServiceOfferEntryEvaluationVendors) {
                vm.evaluasi.ServiceOfferEntryEvaluationVendors.forEach(function (det) {
                    evaluationVendor.push({
                        OfferEntryVendorID: det.OfferEntryVendorID,
                        VendorName: det.VendorName,
                        Score: det.Score,
                        Rank: det.Rank,
                        OfferTotalCost: det.OfferTotalCost
                    });
                });
            }
            var item = {
                evaluasiID: vm.evaluasi.ID,
                tenderRefID: vm.tenderRefID,
                procPackageType: vm.procPackType,
                tenderID: vm.tenderStepData.TenderID,
                evaluationVendor: evaluationVendor,
                tenderStepData: {
                    ID : vm.tenderStepData.ID,
                    Summary: vm.tenderStepData.Summary,
                    DocumentUrl: vm.tenderStepData.DocumentUrl,
                    DocumentDate: vm.tenderStepData.DocumentDate
                }
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/evaluasi-harga-jasa/evaluasiHargaJasa.modal.html',
                controller: 'evaluasiHargaJasaModalController',
                controllerAs: 'ehjmCtrl',
                resolve: { item: function () { return item; } }
            });
            modalInstance.result.then(function () {
                loadData();
            });
        }

        vm.lihatSummary = lihatSummary;
        function lihatSummary() {
            var item = {
                summary: vm.tenderStepData.Summary,
                documentUrl: vm.tenderStepData.DocumentUrl,
                documentDate: vm.tenderStepData.DocumentDate
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/evaluasi-harga-jasa/viewSummary.modal.html',
                controller: 'summaryPenawaranController',
                controllerAs: 'summaryCtrl',
                resolve: { item: function () { return item; } }
            });
            modalInstance.result.then(function () { });
        };

        vm.detail = detail;
        function detail(id) {
            $state.transitionTo('detail-penawaran-harga-jasa',
                { SOEEVID:id, TenderRefID: vm.tenderRefID, StepID: vm.stepID, ProcPackType: vm.procPackType });
        };

        vm.viewSummary = viewSummary;
        function viewSummary() {
        };
    }
})();