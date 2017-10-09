(function () {
    'use strict';

    angular.module("app").controller("questionnaireServiceTenderCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'OfferEntryService',
        '$state', 'UIControlService', 'UploaderService', '$uibModal', 'GlobalConstantService', '$stateParams'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, OEService, $state,
        UIControlService, UploaderService, $uibModal, GlobalConstantService, $stateParams) {
        var vm = this;
        vm.listQuest = [];
        vm.IDTender = Number($stateParams.TenderRefID);
        vm.IDStepTender = Number($stateParams.StepID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
        vm.IDDoc = Number($stateParams.DocID);

        vm.back = back;
        function back() {
            $state.transitionTo('pemasukkan-penawaran-jasa-vendor', {
                TenderRefID: vm.IDTender, StepID: vm.IDStepTender, ProcPackType: vm.ProcPackType, DocID: vm.IDDoc
            });
        }

        vm.init = init;
        function init() {
            UIControlService.loadLoading("Silahkan Tunggu");
            OEService.getQuestionaireByVendor({
                TenderStepID: vm.IDStepTender
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listQuest = data.VendorQuestionaires;
                    console.info(JSON.stringify(vm.listQuest));
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.saveQuest = saveQuest;
        function saveQuest() {
            UIControlService.loadLoading("Silahkan Tunggu");
            OEService.approveQuestionaire(vm.listQuest, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    UIControlService.msg_growl("success", "Berhasil Simpan Data Kuisioner");
                    vm.init();
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.back = back;
        function back() {
            $state.transitionTo('pemasukkan-penawaran-jasa-vendor', {
                TenderRefID: vm.IDTender, StepID: vm.IDStepTender, ProcPackType: vm.ProcPackType, DocID: vm.IDDoc
            });
        }
    }
})();
