(function() {
    'use strict';

    angular.module("app")
            .controller("AsosiasiCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'AsosiasiService',
         'UIControlService', '$uibModal', 'RoleService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, AsosiasiService,
         UIControlService, $uibModal, RoleService) {

        var vm = this;
        vm.listAllData = [];
        vm.txtSearch = "";
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.maxSize = 10;
        
        vm.init = init;
        function init(){
            $translatePartialLoader.addPart('master-asosiasi');
            jLoad(1);
        };

        vm.jLoad = jLoad;
        function jLoad(current) {
            UIControlService.loadLoading("Silahkan Tunggu...");
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            AsosiasiService.select({
                Offset: offset,
                Limit: vm.maxSize,
                Keyword: vm.txtSearch
            }, function (reply) {
                //console.info("data_asoc:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.listAllData = data.List;
                    vm.totalItems = Number(data.Count);
                } else {
                    UIControlService.msg_growl("error", "Gagal Mendapatkan Data Asosiasi!!");
                    return;
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                UIControlService.msg_growl("error", "Gagal Akses API");
                UIControlService.unloadLoading();
            });
        }

        vm.searchData = searchData;
        function searchData() {
            jLoad(1);
        }

        vm.forminput = forminput;
        function forminput(data, isAdd) {
            var data = {
                act: isAdd,
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/master/asosiasi/formAsosiasi.html',
                controller: 'FormAsosiasiCtrl',
                controllerAs: 'formAsosiasiCtrl',
                resolve: {
                    item: function() {
                        return data;
                    }
                }
            });
            //console.info("okeee");
            modalInstance.result.then(function() {
                vm.jLoad();
            });
        };

        vm.ubah_aktif = ubah_aktif;
        function ubah_aktif(data, active) {
            //console.info("idneee:" + active);
            UIControlService.loadLoading("Silahkan Tunggu");
            AsosiasiService.editActive({
                AssosiationID: data.AssosiationID, IsActive: active
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var msg = "";
                    if (active === false) msg = " NonAktifkan ";
                    if (active === true) msg = "Aktifkan ";
                    UIControlService.msg_growl("notice", "Data Berhasil di " + msg);
                    jLoad(1);
                }
                else {
                    UIControlService.msg_growl("error", "Gagal menonaktifkan data");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "Gagal Akses API!!");
                UIControlService.unloadLoading();
            });

        }
    }
})();
