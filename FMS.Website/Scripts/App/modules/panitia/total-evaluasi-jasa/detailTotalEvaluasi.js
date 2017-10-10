(function () {
    'use strict';

    angular.module("app")
    .controller("detTotalEvaluasiCtrl", ctrl);

    ctrl.$inject = ['$state', '$scope', '$http', '$filter', '$stateParams', '$uibModalInstance', 'item', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'UIControlService', 'GlobalConstantService', 'TotalEvaluasiJasaService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, $stateParams, $uibModalInstance, item, $translate, $translatePartialLoader, $location, SocketService, UIControlService, GlobalConstantService, TEJService) {

        var vm = this;
        vm.IDStepTender = item.TenderStepID;
        var loadmsg = "MESSAGE.LOADING";

        vm.crApps = [];
        vm.employeeFullName = "";
        vm.employeeID = 0;
        vm.information = "";

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('verifikasi-tender');
            //UIControlService.loadLoadingModal(loadmsg);
            loadDataTotalEval();
        }

        function loadDataTotalEval() {
            //UIControlService.unloadLoading();
            TEJService.getFinalTotalEval({
                TenderStepID: vm.IDStepTender
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listVendor = data;

                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }
        vm.cancel = cancel;
        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();