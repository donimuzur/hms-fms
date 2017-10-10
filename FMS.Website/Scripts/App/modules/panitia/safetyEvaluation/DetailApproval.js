(function () {
    'use strict';

    angular.module("app")
    .controller("DetailApprovalCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'EvalSafetyService', '$state', 'UIControlService', '$uibModal', '$uibModalInstance', 'GlobalConstantService', 'item'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, EvalSafetyService,
        $state, UIControlService, $uibModal, $uibModalInstance, GlobalConstantService, item) {
        var vm = this;
        vm.id = item.item;
        vm.init = init;
        function init() {
            jLoad();
        }

        vm.jLoad = jLoad;
        function jLoad() {
            vm.evalsafety = [];
            EvalSafetyService.selectApproval({Status: vm.id}, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.evalsafety = reply.data;
                    console.info(JSON.stringify(vm.evalsafety));
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Evalasi Safety" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }


        vm.batal = batal;
        function batal() {
            //$uibModalInstance.dismiss('cancel');
            $uibModalInstance.close();
        };


    }
})();;