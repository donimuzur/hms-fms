(function () {
    'use strict';

    angular.module("app")
    .controller("selectVendorModal", ctrl);

    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$uibModalInstance', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService', 'item'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $uibModalInstance, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService, item) {

        var vm = this;
        var loadmsg = "";

        vm.count;
        vm.pageNumber = 1;
        vm.pageSize = 10;
        vm.searchText = "";
        vm.vendor = [];
        vm.currentData = item.currentData;

        vm.onBatalClick = function () {
            $uibModalInstance.dismiss('cancel');
        };

        vm.onSearchSubmit = function (searchText) {
            vm.searchText = searchText;
            vm.loadData();
        };

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('data-contract-requisition');
            $translate.refresh().then(function () {
                loadmsg = $filter('translate')('MESSAGE.LOADING');
            });
            vm.loadData();
        };

        vm.loadData = loadData;
        function loadData() {
            UIControlService.loadLoadingModal(loadmsg);
            DataContractRequisitionService.SelectVendor({
                Keyword: vm.searchText,
                Limit: vm.pageSize,
                Offset: (vm.pageNumber - 1) * vm.pageSize,
                column: 2,
            }, function (reply) {
                if (reply.status === 200) {
                    vm.vendor = reply.data;
                    DataContractRequisitionService.CountVendor({
                        Keyword: vm.searchText,
                        column: 1
                    }, function (reply) {
                        if (reply.status === 200) {
                            vm.count = reply.data;
                        } else {
                            UIControlService.msg_growl('error', $filter('translate')('MESSAGE.ERR_LOAD_VENDOR'));
                        }
                        UIControlService.unloadLoadingModal();
                    }, function (error) {
                        UIControlService.msg_growl('error', $filter('translate')('MESSAGE.ERR_LOAD_VENDOR'));
                        UIControlService.unloadLoadingModal();
                    });
                } else {
                    UIControlService.msg_growl('error', $filter('translate')('MESSAGE.ERR_LOAD_VENDOR'));
                    UIControlService.unloadLoadingModal();
                }
            }, function (error) {
                UIControlService.msg_growl('error', $filter('translate')('MESSAGE.ERR_LOAD_VENDOR'));
                UIControlService.unloadLoadingModal();
            });
        };

        vm.onSelectClick = onSelectClick;
        function onSelectClick(vendor) {
            for (var i = 0; i < vm.currentData.length; i++) {
                if (vendor.VendorID === vm.currentData[i].VendorID) {
                    UIControlService.msg_growl('error', $filter('translate')('MESSAGE.ERR_DUPLICATE_VENDOR'));
                    return;
                }
            }
            $uibModalInstance.close(vendor);
        };
    }
})();