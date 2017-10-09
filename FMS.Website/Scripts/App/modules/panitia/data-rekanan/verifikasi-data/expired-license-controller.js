(function () {
    'use strict';

    angular.module("app")
    .controller("ExpiredLicenseController", ctrl);

    ctrl.$inject = ['$http', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'VerifikasiDataService',
        'AuthService', 'UIControlService', '$uibModalInstance'];
    /* @ngInject */
    function ctrl($http, $uibModal, $translate, $translatePartialLoader, $location, SocketService, VerifikasiDataService, AuthService,
        UIControlService, $uibModalInstance) {
        var vm = this;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.listLicensi = [];

        vm.init = init;
        function init() {
            jLoad(1);
        }

        vm.jLoad = jLoad;
        function jLoad(current) {
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            UIControlService.loadLoading("MSG.LOADING");
            VerifikasiDataService.selectlicensi({
                Offset: offset,
                Limit: vm.pageSize
            }, function (response) {
                UIControlService.unloadLoading();
                if (response.status == 200) {
                    var list = response.data.List;
                    for (var i = 0; i < list.length; i++) {
                        if (!(list[i].IssuedDate === null)) {
                            list[i].IssuedDate = UIControlService.getStrDate(list[i].IssuedDate);
                        }
                        if (!(list[i].ExpiredDate === null)) {
                            list[i].ExpiredDate = UIControlService.getStrDate(list[i].ExpiredDate);
                        }
                    }
                    vm.listLicensi = list;
                } else {
                    UIControlService.msg_growl("error", ".MESSAGE.ERR_TYPEFILE");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
                return;
            });
        }

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };

    }
})();