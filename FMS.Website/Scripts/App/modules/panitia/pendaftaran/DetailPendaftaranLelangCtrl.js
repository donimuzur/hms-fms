(function () {
    'use strict';

    angular.module("app").controller("DetailPendaftaranLelangCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'PendaftaranLelangService', 'RoleService', 'UIControlService', '$uibModal', 'GlobalConstantService', 'item', '$uibModalInstance'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, PendaftaranLelangService,
        RoleService, UIControlService, $uibModal,  GlobalConstantService, item, $uibModalInstance) {
        var vm = this;
        vm.data = item;
        vm.pendaftaran = [];
        vm.userBisaNgatur = false;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.page_id = 103;
        vm.nama_paket = "";
        vm.nama_tahapan = "";
        vm.is_created = false;
        vm.status = -1;
        vm.peserta = [];
        vm.menuhome = 0;
        vm.labelcurr;

        vm.init = init;
        function init() {
            console.info(JSON.stringify(vm.data));
            //$translatePartialLoader.addPart('pendaftaran-lelang');
            // UIControlService.loadLoading("Silahkan Tunggu...");
            //  getBlacklist();
            
        }

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        }
    }
})();