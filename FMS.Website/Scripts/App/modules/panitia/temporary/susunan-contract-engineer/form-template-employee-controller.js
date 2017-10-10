(function () {
    'use strict';

    angular.module("app").controller("FormCommitteeTemplateCtrl", ctrl);
    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'ContractEngineerService', 'UIControlService', '$uibModalInstance', '$uibModal', 'GlobalConstantService'];
    /* @ngInject */
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService,
        ContractEngineerService, UIControlService, $uibModalInstance, $uibModal, GlobalConstantService) {
        var vm = this;
        vm.currentPage = 1;
        vm.offset = (vm.currentPage * vm.fullSize) - vm.fullSize;
        vm.totalItems = 0;
        vm.flag = false;
        vm.fullSize = 5;
        
        vm.searchText = '';
        vm.init = init();
        function init() {
            // $translatePartialLoader.addPart('verifikasi-data');
            jLoad(1);
            //loadPosition();


        };
        //comite
        function jLoad(current) {
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            ContractEngineerService.selectemployeeAll({
                Offset: offset,
                Limit: vm.fullSize,
                Keyword: vm.searchText
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.detailEmployee = reply.data.List;
                    //console.info(JSON.stringify(vm.detailEmployee));
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Pegawai" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                $.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.onSearchSubmit = onSearchSubmit;
        function onSearchSubmit(searchText) {
            //console.info(searchText);
            if (searchText === undefined) vm.searchText = '';
            else vm.searchText = searchText;
            jLoad(vm.current)
        };

        vm.loadPosition = loadPosition;
        function loadPosition() {
            ContractEngineerService.selectposition(function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.detailPosition = reply.data;
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Rekanan" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                $.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.pilihTemplate = pilihTemplate;
        function pilihTemplate(data) {
            $uibModalInstance.close(data.list);
        }

        vm.cancel = cancel;
        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }
    }
})();
