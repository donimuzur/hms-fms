(function () {
    'use strict';

    angular.module("app")
    .controller("evaluasiPenawaranVHSController", ctrl);

    ctrl.$inject = ['$state', '$stateParams', '$http', '$filter', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'EvaluasiPenawaranVHSService', 'DataPengadaanService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $stateParams, $http, $filter, $uibModal, $translate, $translatePartialLoader, $location, SocketService, EvaluasiPenawaranVHSService, DataPengadaanService, UIControlService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";

        vm.stepID = Number($stateParams.StepID);
        vm.tenderRefID = Number($stateParams.TenderRefID);
        vm.procPackType = Number($stateParams.ProcPackType);

        vm.evaluasi = {};
        vm.tenderStepData = {};

        vm.isProcess = false;
        vm.noEvaluation = true;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('evaluasi-penawaran-barang');
            loadData();
        };

        function loadData() {
            UIControlService.loadLoading(loadmsg);
            DataPengadaanService.GetStepByID({
                ID: vm.stepID
            }, function (reply) {
                vm.tenderStepData = reply.data;
                vm.isProcess = vm.tenderStepData.StatusName === "PROCUREMENT_TYPE_PROCESS";
                EvaluasiPenawaranVHSService.getByTenderStepData({
                    ID: vm.stepID,
                    TenderID: vm.tenderStepData.TenderID
                }, function (reply) {
                    UIControlService.unloadLoading();
                    vm.evaluasi = reply.data;
                    vm.noEvaluation = !vm.evaluasi.VHSOfferEvaluationVendors || vm.evaluasi.VHSOfferEvaluationVendors.length === 0;
                    if (!vm.noEvaluation) {
                        vm.evaluasi.VHSOfferEvaluationVendors.sort(function (a, b) {
                            return b.Score - a.Score;
                        });
                    }
                }, function (error) {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
                });
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
            });
        };

        vm.buatSummary = buatSummary;
        function buatSummary() {

            var evaluationVendor = [];
            if (vm.evaluasi.VHSOfferEvaluationVendors) {
                vm.evaluasi.VHSOfferEvaluationVendors.forEach(function (det) {
                    evaluationVendor.push({
                        VHSOfferEntryID: det.VHSOfferEntryID,
                        VendorName: det.VendorName,
                        Score: det.Score,
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
                templateUrl: 'app/modules/panitia/evaluasi-penawaran-vhs/evaluasiPenawaranVHS.modal.html',
                controller: 'evaluasiPenawaranVHSModalController',
                controllerAs: 'epvmCtrl',
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
                templateUrl: 'app/modules/panitia/evaluasi-penawaran-vhs/viewSummary.modal.html',
                controller: 'summaryPenawaranController',
                controllerAs: 'summaryCtrl',
                resolve: { item: function () { return item; } }
            });
            modalInstance.result.then(function () { });
        };

        vm.lakukanEvaluasi = lakukanEvaluasi;
        function lakukanEvaluasi() {
            $state.transitionTo('detail-evaluasi-penawaran-vhs',
                { TenderRefID: vm.tenderRefID, StepID: vm.stepID, ProcPackType: vm.procPackType });
        };
    }
})();