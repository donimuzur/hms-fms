(function () {
    'use strict';

    angular.module("app").controller("FormCommitteeTemplate2Ctrl", ctrl);
    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'ContractEngineerService', 'UIControlService', '$uibModalInstance', '$uibModal', 'GlobalConstantService'];
    /* @ngInject */
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService,
        ContractEngineerService, UIControlService, $uibModalInstance, $uibModal, GlobalConstantService) {
        var vm = this;
        vm.fullSize = 5;
        vm.currentPage = 1;
        vm.offset = (vm.currentPage * vm.fullSize) - vm.fullSize;
        vm.totalItems = 0;
        vm.flag = false;
        
        vm.searchText = '';
        vm.init = init();
        function init() {
            jLoad(1);
        };

        function jLoad(current) {
            vm.currentPage = current;
            var offset = (current * vm.fullSize) - vm.fullSize;
            ContractEngineerService.selecttemplate({
                Offset: offset,
                Limit: vm.fullSize,
                Keyword: vm.searchText
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.templates = reply.data.List;
                    vm.totalItems = reply.data.Count;
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Template" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                $.growl.error({ message: "Gagal mendapatkan data Template" });
                UIControlService.unloadLoading();
            });
        }

        vm.onSearchSubmit = onSearchSubmit;
        function onSearchSubmit(searchText) {
            if (searchText === undefined) vm.searchText = '';
            else vm.searchText = searchText;
            jLoad(vm.current)
        };
        
        vm.pilihTemplate = pilihTemplate;
        function pilihTemplate(data) {
            $uibModalInstance.close(data.MstCommitteeTemplateDetails);
        }

        vm.cancel = cancel;
        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }
    }
})();
