(function () {
    'use strict';

    angular.module("app")
    .controller("selectApproverModalTotalEvalCtrl", ctrl);
    
    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$uibModalInstance', 'item', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'TotalEvaluasiJasaService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $uibModalInstance, item, $translate, $translatePartialLoader, $location, SocketService, TotalEvaluasiJasaService, UIControlService) {

        var vm = this;
        var loadmsg = "";
        var currentData = item.currentData;

        vm.count;
        vm.pageNumber = 1;
        vm.pageSize = 10;
        vm.searchText = "";
        vm.pegawai = [];

        vm.onBatalClick = function () {
            $uibModalInstance.dismiss('cancel');
        };

        vm.onSearchSubmit = function (searchText) {
            vm.searchText = searchText;
            vm.loadData();
        };

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('verifikasi-tender');
            $translate.refresh().then(function () {
                loadmsg = $filter('translate')('MESSAGE.LOADING');
            });
            vm.loadData();
        };

        vm.loadData = loadData; 
        function loadData() {
            UIControlService.loadLoadingModal(loadmsg);
            TotalEvaluasiJasaService.SelectEmployee({
                Keyword: vm.searchText,
                Limit: vm.pageSize,
                Offset: (vm.pageNumber - 1) * vm.pageSize,
                column: 1,
            }, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    vm.pegawai = reply.data.List;
                    vm.count = Number(reply.data.Count);
                } else {
                    UIControlService.msg_growl('error', $filter('translate')('MESSAGE.ERR_LOAD_EMPLOYEE'));
                    UIControlService.unloadLoadingModal();
                }
            }, function (error) {
                UIControlService.msg_growl('error', $filter('translate')('MESSAGE.ERR_LOAD_EMPLOYEE'));
                UIControlService.unloadLoadingModal();
            });
        };

        vm.onSelectClick = onSelectClick;
        function onSelectClick(selected) {
            for (var i = 0; i < currentData.length; i++) {
                if (selected.EmployeeID === currentData[i].EmployeeID && currentData[i].IsActive == true) {
                    UIControlService.msg_growl('error', $filter('translate')('MESSAGE.ERR_DUPLICATE_EMPLOYEE'));
                    return;
                }
            }
            $uibModalInstance.close(selected);
        };
    }
})();