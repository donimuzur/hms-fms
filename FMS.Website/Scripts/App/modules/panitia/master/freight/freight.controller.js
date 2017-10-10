(function () {
    'use strict';

    angular.module("app").controller("FreightCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'FreightService',
	    'RoleService', 'UIControlService', '$uibModal'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, FreightService,
        RoleService, UIControlService, $uibModal) {

        var vm = this;
        vm.freight_list = [];
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.maxSize = 10;
        vm.txtSearch = "";

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('master-freight');
            jLoad(1);
        };

        vm.jLoad = jLoad;
        function jLoad(current) {
            UIControlService.loadLoading("Silahkan Tunggu...");
            vm.currentPage = current;
            var offset = (current * 10) - 10;

            FreightService.select({
                Offset: offset,
                Limit: vm.maxSize,
                Keyword: vm.txtSearch
            }, function (reply) {
                //console.info("datane:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.freight_list = data.List;
                    vm.totalItems = Number(data.Count);
                } else {
                    UIControlService.msg_growl("error", "Gagal Mendapatkan Data Freight");
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                //console.info("error:" + JSON.stringify(err));
                UIControlService.msg_growl("error", "Gagal Mendapatkan Data Freight");
                UIControlService.unloadLoading();
            });
        }

        vm.searchData = searchData;
        function searchData() {
            jLoad(1);
        }

        vm.forminput = forminput;
        function forminput(data, isAdd) {
            //console.info("modaala");
            var data = {
                act: isAdd,
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/master/freight/formFreight.html',
                controller: 'FormFreightCtrl',
                controllerAs: 'formFreightCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            //console.info("okeee");
            modalInstance.result.then(function () {
                vm.jLoad(1);
            });
        }

        vm.ubah_aktif = ubah_aktif;
        function ubah_aktif(data, active) {
            //console.info("idneee:" + active);
            UIControlService.loadLoading("Silahkan Tunggu");
            FreightService.editActive({
                FreightCostID: data.FreightCostID, IsActive: active
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
                UIControlService.msg_growl("error", "Gagal menonaktifkan data");
                UIControlService.unloadLoading();
            });

        }
    }
})();