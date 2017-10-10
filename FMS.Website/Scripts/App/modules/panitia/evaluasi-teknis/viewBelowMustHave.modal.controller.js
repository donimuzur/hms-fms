(function () {
    'use strict';

    angular.module("app")
    .controller("viewBelowMustHaveModal", ctrl);
    
    ctrl.$inject = ['$state', 'item', '$http', '$filter', '$uibModalInstance', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'EvaluationTechnicalService', 'UIControlService'];
    /* @ngInject */
    function ctrl($state, item, $http, $filter, $uibModalInstance, $translate, $translatePartialLoader, $location, SocketService, EvaluationTechnicalService, UIControlService) {

        var vm = this;
        var loadmsg = "";
        
        vm.kriteria = [];
        vm.vendorName = item.VendorName;

        vm.onBatalClick = function () {
            $uibModalInstance.dismiss('cancel');
        };

        vm.init = init;
        function init() {
            $translate.refresh().then(function () {
                loadmsg = $filter('translate')('MESSAGE.LOADING');
            });
            vm.loadData();
        };

        vm.loadData = loadData; 
        function loadData() {
            UIControlService.loadLoadingModal(loadmsg);
            EvaluationTechnicalService.getBelowMustHaveScores({
                VendorID: item.VendorID,
                TenderStepDataID: item.TenderStepDataID,
            }, function (reply) {
                if (reply.status === 200) {
                    vm.kriteria = reply.data;
                    UIControlService.unloadLoadingModal();
                } else {
                    UIControlService.msg_growl('error', $filter('translate')('Gagal mendapatkan kriteria'));
                    UIControlService.unloadLoadingModal();
                }
            }, function (error) {
                UIControlService.msg_growl('error', $filter('translate')('Gagal mendapatkan kriteria'));
                UIControlService.unloadLoadingModal();
            });
        };
    }
})();