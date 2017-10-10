(function () {
    'use strict';

    angular.module("app")
    .controller("detailTahapanVendorController", ctrl);

    ctrl.$inject = ['$state', '$http', '$filter', '$stateParams', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DetailTahapanVendorService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $http, $filter, $stateParams, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DetailTahapanVendorService, UIControlService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";

        var TenderID = Number($stateParams.TenderID);

        vm.tenderReg;
        vm.step = [];
        vm.paket;
        vm.namaLelang;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('dashboard-vendor');
            loadSteps();
        };

        function loadSteps() {
            UIControlService.loadLoading(loadmsg);
            DetailTahapanVendorService.GetTenderReg({
                ID: TenderID
            }, function (reply) {
                vm.tenderReg = reply.data;
                console.info("tendReg" + JSON.stringify(vm.tenderReg));
                DetailTahapanVendorService.GetSteps({
                    ID: TenderID
                }, function (reply) {
                    vm.step = reply.data;
                    console.info("step" + JSON.stringify(vm.step));
                    if (vm.step.length > 0) {
                        vm.namaLelang = vm.step[0].tender.TenderName;
                    }
                    vm.step.forEach(function (fl) {
                        fl.StartDateConverted = convertTanggal(fl.StartDate);
                        fl.EndDateConverted = convertTanggal(fl.EndDate);
                    });
                    UIControlService.unloadLoading();
                }, function (error) {
                    UIControlService.unloadLoading();
                    UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_STEP');
                });
            }, function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_TENDER_REG');
            });            
        };       

        function convertTanggal(input) {
            return UIControlService.convertDateTime(input);
        };

        vm.kembali = kembali;
        function kembali() {
            $state.transitionTo('dashboard-vendor');
        };

        vm.menujuTahapan = menujuTahapan;
        function menujuTahapan(step) {
            //TenderRefID/:StepID/:ProcPackType
            console.info("masuk:"+JSON.stringify(step));
            $state.transitionTo(step.step.FormTypeURL + '-vendor', { TenderRefID: step.tender.TenderRefID, StepID: step.ID, ProcPackType: step.tender.ProcPackageType, TenderID: TenderID });
        };
    }
})();

