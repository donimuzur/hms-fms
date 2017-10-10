(function () {
    'use strict';

    angular.module("app")
    .controller("DetailEquipmentQualificationCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'EvaluationQualificationService', '$state', 'UIControlService', '$uibModal', '$uibModalInstance', 'GlobalConstantService', 'item'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, EvaluationQualificationService,
        $state, UIControlService, $uibModal, $uibModalInstance, GlobalConstantService, item) {
        var vm = this;
        vm.detail = item.item;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";

        vm.init = init;
        function init() {
        }

        vm.batal = batal;
        function batal() {
            //$uibModalInstance.dismiss('cancel');
            $uibModalInstance.close();
        };


    }
})();;