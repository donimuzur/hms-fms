(function () {
    'use strict';

    angular.module("app").controller("verKDCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'verKDService',
        '$state', 'UIControlService', 'UploadFileConfigService', 'UploaderService', 'GlobalConstantService',
        '$uibModal', '$stateParams'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, verKDService,
        $state, UIControlService, UploadFileConfigService, UploaderService, GlobalConstantService,
        $uibModal, $stateParams) {
        var vm = this;

        vm.IDTender = Number($stateParams.TenderRefID);
        vm.IDStepTender = Number($stateParams.StepID);
        vm.ProcPackType = Number($stateParams.ProcPackType);

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('verifikasi-kelengkapan-dokumen');
            loadData();
            loadStep();
        }

        function loadData() {
            verKDService.select({
                Status: vm.IDTender,
                FilterType: vm.ProcPackType,
                column: vm.IDStepTender
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.data = data;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }
        function loadStep() {
            verKDService.Step({
                ID: vm.IDStepTender
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.step = data;
                    console.info(vm.step);
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.pindah = pindah;
        function pindah(data) {
            $state.transitionTo('detail-kd', {VendorID: data.VendorID, TenderRefID: vm.IDTender, ProcPackType: vm.ProcPackType});
        }

        vm.backpengadaan = backpengadaan;
        function backpengadaan() {
            $state.transitionTo('data-pengadaan-tahapan', { TenderRefID: vm.IDTender, ProcPackType: vm.ProcPackType });
        }

    }
})();