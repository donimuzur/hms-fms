(function () {
    'use strict';

    angular.module("app").controller("CPRCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'CPRService',
        '$state', 'UIControlService', 'UploadFileConfigService', 'UploaderService', 'GlobalConstantService',
        '$uibModal', '$stateParams'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, CPRService,
        $state, UIControlService, UploadFileConfigService, UploaderService, GlobalConstantService,
        $uibModal, $stateParams) {
        var vm = this;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart("cpr");
            vm.cpr = [
                { nama_vendor: "0193109 - Marine Support", depart: "CS-Service", cp: "Igna", jenis: "Plan", tgl: "19-Maret-2016" },
                { nama_vendor: "0193109 - Process Plan", depart: "Process Plan", cp: "Agni", jenis: "No Plan", tgl: "19-Maret-2016" }
            ];
        };

        vm.tambahform = tambahform;
        function tambahform() {
            $state.transitionTo('form-cpr-vendor');
        }
    }
})();