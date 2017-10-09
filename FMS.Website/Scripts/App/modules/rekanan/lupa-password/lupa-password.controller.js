(function () {
    'use strict';

    angular.module("app")
    .controller("lupaPasswordRekananCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'LupaPasswordService', 'UIControlService'];
    /* @ngInject */
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, LupaPasswordService, UIControlService) {

        var vm = this;

        vm.input;
        vm.simpan = simpan;        
        function simpan() {
            UIControlService.loadLoading("MESSAGE.LOADING");
            LupaPasswordService.save({
                Keyword: vm.input
            }, function (response) {
                var result = response.data;
                UIControlService.msg_growl("notice", "MESSAGE.SUCC_RESET");
                UIControlService.unloadLoading();
            },function (error) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_RESET");
                UIControlService.unloadLoading();
            });
        }
    }
})();
