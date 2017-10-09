(function() {
    'use strict';

    angular.module("app")
    .controller("BadanUsahaCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'BadanUsahaService',
         'RoleService', 'UIControlService', '$uibModal', 'growl'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, BadanUsahaService,
        RoleService, UIControlService, $uibModal, growl) {

        var vm = this;
        vm.badanList = [];
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.maxSize = 10;
        vm.userBisaMengatur = true;
        vm.userBisaMenambah = true;
        vm.userBisaMengubah = true;
        vm.userBisaMenghapus = true;
        vm.txtSearch = "";
        var page_id = 136;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('master-badan-usaha');
            loadBadanUsaha(1);
        };

        vm.loadBadanUsaha = loadBadanUsaha;
        function loadBadanUsaha(current){
            UIControlService.loadLoading("Silahkan Tunggu...");
            vm.currentPage = current;
            var offset = (current * 10) - 10;

            BadanUsahaService.select({
                Offset: offset,
                Limit: vm.maxSize,
                Keyword: vm.txtSearch
            }, function (reply) {
                //console.info("datane:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.badanList = data.List;
                    vm.totalItems = Number(data.Count);
                } else {
                    UIControlService.msg_growl("error", "Gagal mendapatkan data Master Badan Usaha");
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                //console.info("error:" + JSON.stringify(err));
                UIControlService.msg_growl("error", "Gagal akses API");
                UIControlService.unloadLoading();
            });
        };

        vm.cariBadanUsaha = cariBadanUsaha;
        function cariBadanUsaha() {
            vm.loadBadanUsaha(1);
        }

        vm.jLoad = jLoad;
        function jLoad(curr) {
            loadBadanUsaha(curr);
        }

        vm.ubah_aktif = ubah_aktif;
        function ubah_aktif(data, active) {
            UIControlService.loadLoading("Silahkan Tunggu");
            //console.info("ada:"+JSON.stringify(data))
            BadanUsahaService.editActive({
                BusinessID: data.BusinessID, IsActive: active
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var msg = "";
                    if (active === false) msg = " NonAktifkan ";
                    if (active === true) msg = "Aktifkan ";
                    UIControlService.msg_growl("notice", "Data Berhasil di " + msg);
                    loadBadanUsaha(1);
                }
                else {
                    UIControlService.msg_growl("error", "Gagal menonaktifkan data");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "Gagal Akses API");
                UIControlService.unloadLoading();
            });
        };

        vm.forminput = forminput;
        function forminput(data, isAdd) {
            console.info("modaala");
            var data = {
                act: isAdd,
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/master/badan-usaha/badanUsahaModal.html',
                controller: 'BadanUsahaModalCtrl',
                controllerAs: 'BUModalCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            //console.info("okeee");
            modalInstance.result.then(function () {
                vm.loadBadanUsaha();
            });
        };
    };
})();
