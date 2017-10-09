(function () {
    'use strict';

    angular.module("app").controller("formVendornotactiveCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'VendorNotActiveService', 'RoleService', 'UIControlService', 'item', '$uibModalInstance'];

    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, VendorNotActiveService,
		RoleService, UIControlService, item, $uibModalInstance) {
        var vm = this;
        vm.aktifasi = "";
        vm.Keyword = "";
        vm.pageSize = 10;
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.IdVendor = item.item.VendorID;
        vm.act = item.act;
        vm.init = init;
        vm.VendorName = item.item.VendorName;
        function init() {
            if (vm.act === true)
                vm.aktifasi = "Aktifkan";
            else vm.aktifasi = "NonAktifkan";
            // $translatePartialLoader.addPart('blacklist-data');

        };

        vm.save = save;
        function save() {
            UIControlService.loadLoading("Silahkan Tunggu");
            VendorNotActiveService.editActive({
                IsActive: vm.act,
                VendorID: vm.IdVendor,
                Description: vm.Description
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var msg = "";
                    if (vm.act === false) msg = " NonAktifkan ";
                    if (vm.act === true) msg = "Aktifkan ";
                    UIControlService.msg_growl("success", "Data Berhasil di " + msg);
                    $uibModalInstance.close();
                }
                else {
                    UIControlService.msg_growl("error", "Gagal menonaktifkan data ");
                    return;
                }
            }, function (err) {

                UIControlService.msg_growl("error", "Gagal Akses API ");
                UIControlService.unloadLoading();
            });
        }

        vm.cancel = cancel;
        function cancel() {
            $uibModalInstance.close();
        }
    }
})();
