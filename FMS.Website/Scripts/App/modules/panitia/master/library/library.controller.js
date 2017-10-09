(function () {
    'use strict';

    angular.module("app").controller("LibraryCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'LibraryService',
	    'RoleService', 'UIControlService', '$uibModal', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, LibraryService,
        RoleService, UIControlService, $uibModal, GlobalConstantService) {

        var vm = this;
        vm.cariLibrary = cariLibrary;
        vm.library = [];
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.userBisaMengatur = false;
        vm.allowAdd = true;
        vm.allowEdit = true;
        vm.allowDelete = true;
        vm.kata = new Kata("");
        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('master-library');
            jLoad(1);
        };

        vm.cariLibrary = cariLibrary;
        function cariLibrary() {
            vm.jLoad(1);
        }

        
        
        vm.tambah = tambah;
        function tambah(data,act) {
            console.info("masuk form add/edit");
            var data = {
                act: act,
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/master/library/formLibrary.html',
                controller: "FormLibraryCtrl",
                controllerAs: "frmLibraryCtrl",
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                jLoad(1);
            });
        }


        vm.edit = edit;
        function edit(data, isAdd) {
            //console.info("modaala");
            var data = {
                act: isAdd,
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/master/library/formLibrary.html',
                controller: "FormLibraryCtrl",
                controllerAs: "frmLibraryCtrl",
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

        vm.jLoad = jLoad;
        function jLoad(current) {
            //console.info("curr "+current)
            vm.library = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            LibraryService.select({
                Offset: offset,
                Limit: vm.pageSize,
                Keyword: vm.kata.srcText
            }, function (reply) {
                //console.info("data:"+JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.library = data.List;
                    vm.totalItems = Number(data.Count);
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Master Departemen" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.ubah_aktif = ubah_aktif;
        function ubah_aktif(data, active) {
            UIControlService.loadLoading("Silahkan Tunggu");
            //console.info("ada:"+JSON.stringify(data))
            LibraryService.editActive({
                LibraryID: data.LibraryID, IsActive: active
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var msg = "";
                    if (active === false) msg = " NonAktifkan ";
                    if (active === true) msg = "Aktifkan ";
                    UIControlService.msg_growl("success", "Data Berhasil di " + msg);
                    jLoad(1);
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
       

    }
})();


function Kata(srcText) {
    var self = this;
    self.srcText = srcText;
}