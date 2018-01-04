(function () {
    'use strict';
    angular.module("app").controller("MSTFCtrl", ctrl);
    ctrl.$inject = ['$http', '$scope', '$translate', '$window', 'UIControlService', '$uibModal',
        'GlobalConstantService', '$state', '$stateParams', 'MstService',
    'UploadFileConfigService', 'ExcelReaderService', 'UploaderService'];

    function ctrl($http, $scope, $translate, $window, UIControlService, $uibModal,
        GlobalConstantService, $state, $stateParams, MstService,
        UploadFileConfigService, ExcelReaderService, UploaderService) {
        var vm = this;
        vm.coordinator = localStorage.getItem('username');
        vm.dataVendor = [];
        vm.totalRecords = 0;

        vm.init = init;
        function init() {
            UIControlService.loadLoading("loading");
            MstService.GetCoordinator(vm.coordinator, function (reply) {
                vm.id_coordinator = reply.data[0]['UserID'];
                console.info(vm.id_coordinator);
                console.log('coba')
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
            });
            vm.loadFleet();
        }

        vm.loadFleet = loadFleet;
        function loadFleet() {
            UIControlService.unloadLoading();
            MstService.SelectFleet(function (reply) {
                if (reply.status === 200) {
                    UIControlService.unloadLoading();
                    vm.dataVendor = reply.data;
                    vm.totalRecords = number(vm.dataVendor.length)
                    console.log("berhasil Gan")
                    Console.log(vm.dataVendor)
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data ePaf" });
                    UIControlService.unloadLoading();
                }
            }), function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.unloadLoading();
            };
            console.log("coba aja")
        }

        vm.insFleet = insFleet;
        function insFleet() {
            var modalInstance = $uibModal.open({
                templateUrl: 'insFleet.html',
                controller: function ($uibModalInstance, $scope) { }
            }
            )
        }
    }

})();