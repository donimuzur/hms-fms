(function () {
    'use strict';

    angular.module("app")
    .controller("aturPaketPengadaanCtrl", ctrl);

    ctrl.$inject = ['$state', '$http', '$stateParams', '$filter', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'PaketPengadaanCEService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $http, $stateParams, $filter, $uibModal, $translate, $translatePartialLoader, $location, SocketService, PaketPengadaanCEService, UIControlService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";
        var contractRequisitionId = Number($stateParams.contractRequisitionId);

        vm.tenderPackageCR = {};

        vm.typeOptions = [];
        vm.schemaOptions = [];
        vm.evalOptions = [];
        vm.mstDocs = [];

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('paket-pengadaan-ce');
            loadOptions();
            loadContract();
        };

        function loadContract() {
            UIControlService.loadLoading(loadmsg);
            PaketPengadaanCEService.SelectTPByCRID({
                ContractRequisitionID: contractRequisitionId
            }, function (reply) {
                if (reply.data) {
                    UIControlService.unloadLoading();
                    vm.tenderPackageCR = reply.data;
                }
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
            });
        };

        function loadOptions() {
            PaketPengadaanCEService.GetAllOptions({
                ContractRequisitionId: contractRequisitionId
            }, function (reply) {
                vm.typeOptions = reply.data[0];
                vm.schemaOptions = reply.data[1];
                vm.evalOptions = reply.data[2];
                vm.tenderDocTypes = reply.data[3];
            }, function (error) {
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_OPTIONS');
            });
        }

        vm.loadTahapan = loadTahapan;
        function loadTahapan() {
            UIControlService.loadLoading(loadmsg);
            PaketPengadaanCEService.GetStepsByMethod({
                MethodID: vm.tenderPackageCR.ProcurementMethodID
            }, function (reply) {
                UIControlService.unloadLoading();
                vm.tenderPackageCR.TenderPackageCRSteps = reply.data;
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_STEPS');
            });
        }

        vm.save = save;
        function save() {
            if (!vm.tenderPackageCR.TenderType || !vm.tenderPackageCR.ProcurementMethodID || !vm.tenderPackageCR.EvalMethodID) {
                UIControlService.msg_growl("error", 'MESSAGE.ERR_INCOMPLETE_FIELD');
                return;
            }
            if (vm.tenderPackageCR.TenderPackageCRSteps.length == 0) {
                UIControlService.msg_growl("error", 'MESSAGE.ERR_NO_STEPS');
                return;
            }

            UIControlService.loadLoading(loadmsg);
            PaketPengadaanCEService.SaveTP(vm.tenderPackageCR, function (reply) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("notice", 'MESSAGE.SUCC_SAVE');
                loadContract();
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", "MESSAGE." + (error[0] ? error[0] : 'ERR_SAVE'));
            });
        }

        vm.confirm = confirm;
        function confirm() {
            UIControlService.loadLoading(loadmsg);
            PaketPengadaanCEService.ConfirmTP(vm.tenderPackageCR, function (reply) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("notice", 'MESSAGE.SUCC_CONFIRM');
                loadContract();
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", "MESSAGE." + (error[0] ? error[0] : 'ERR_CONFIRM'));
            });
        }

        vm.aturDokumen = aturDokumen;
        function aturDokumen(dt) {
            var item = {
                tenderPackageCRStepDocuments: dt.TenderPackageCRStepDocuments,
                tenderDocTypes: vm.tenderDocTypes,
                editable: vm.tenderPackageCR.IsPublished !== true
            };
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/proses-pengadaan/paket-pengadaan-ce/detailKelengkapanTender.modal.html',
                controller: 'detailKelengkapanTenderController',
                controllerAs: 'detKelTenderCtrl',
                resolve: { item: function () { return item; } }
            });
            modalInstance.result.then(function (result) {
                dt.TenderPackageCRStepDocuments = result
            });
        }

        vm.back = back;
        function back() {
            $state.transitionTo('paket-pengadaan-ce');
        }
    }
})();