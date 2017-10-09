(function () {
    'use strict';

    angular.module("app").controller("EvaluationQualificationCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'EvaluationQualificationService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, EvaluationQualificationService,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService) {
       
        var vm = this;
        var page_id = 141;
        vm.StepID = Number($stateParams.StepID);
        vm.TenderRefID = Number($stateParams.TenderRefID);
        vm.ProcPackType = Number($stateParams.ProcPackType);
        vm.departemen = [];
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.init = init;
        vm.jLoad = jLoad;

        function init() {
            jLoad(1);

        }
        vm.cariDepartemen = cariDepartemen;
        function cariDepartemen() {
            vm.jLoad(1);
        }

        vm.jLoad = jLoad;
        function jLoad(current) {
            //console.info("curr "+current)
            vm.listqualification = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            EvaluationQualificationService.select({
                Status: vm.TenderRefID,
                FilterType: vm.StepID,
                column: vm.ProcPackType,
                Offset: offset,
                Limit: vm.pageSize
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listqualification = data.List;
                    vm.totalItems = Number(data.Count);
                    console.info("data:"+JSON.stringify(data));
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Tahapan Evaluasi Kualifikasi" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.detail = detail;
        function detail(tender, proc, vendorID) {
            $state.transitionTo('equipment-evaluation-qualification', { TenderRefID: tender, ProcPackType: proc, VendorID: vendorID });
        }
    }
})();

