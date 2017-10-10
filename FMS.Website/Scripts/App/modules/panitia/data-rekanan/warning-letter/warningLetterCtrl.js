(function () {
    'use strict';

    angular.module("app").controller("WarningLetterCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'WarningLetterService', 'RoleService', 'UIControlService', '$uibModal'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, WarningLetterService,
        RoleService, UIControlService, $uibModal) {
        var vm = this;
        var page_id = 141;
        vm.lettername = "";
        vm.warningletter = [];
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.userBisaMengatur = false;
        vm.allowAdd = true;
        vm.allowEdit = true;
        vm.allowDelete = true;

        vm.kata = new Kata("");
        vm.init = init;

        vm.cariwarningLetter = cariwarningLetter;
        vm.jLoad = jLoad;
        //vm.loadAll = loadAll;
        vm.ubah_aktif = ubah_aktif;
        vm.tambah = tambah;
        vm.edit = edit;

        function init() {
            $translatePartialLoader.addPart('master-warningLetter');
            UIControlService.loadLoading("Silahkan Tunggu...");
            jLoad(1);

        }

        vm.cariwarningLetter = cariwarningLetter;
        function cariwarningLetter() {
            init();
        }

        vm.jLoad = jLoad;
        function jLoad(current) {
            //console.info("curr "+current)
            vm.warningLetter = [];
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            WarningLetterService.select({
                Offset: offset,
                Limit: vm.pageSize,
                Keyword: vm.lettername
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.warningletter = data.List;
                    vm.totalItems = Number(data.Count);
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Master Departemen" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                $.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        vm.ubah_aktif = ubah_aktif;
        function ubah_aktif(data, active) {
            UIControlService.loadLoading("Silahkan Tunggu");
            //console.info("ada:"+JSON.stringify(data))
            DepartemenService.editActive({
                DepartmentID: data.DepartmentID, DepartmentCode: data.DepartmentCode, DepartmentName: data.DepartmentName, IsActive: active
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

        vm.tambah = tambah;
        function tambah(data, act) {
            console.info("masuk form add/edit");
            var data = {
                act: act,
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-rekanan/warning-letter/FormWarningLetter.html',
                controller: 'FormWarningLetterCtrl',
                controllerAs: 'frmWarningLetterCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.jLoad(1);
            });
        }

        vm.edit = edit;
        function edit(data,act) {
            console.info("masuk form add/edit");
            var data = {
                act: act,
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-rekanan/warning-letter/FormWarningLetter.html',
                controller: 'FormWarningLetterCtrl',
                controllerAs: 'frmWarningLetterCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.jLoad(1);
            });
        }

        vm.ubah_aktif = ubah_aktif;
        function ubah_aktif(data, active) {
            UIControlService.loadLoading("Silahkan Tunggu");
            WarningLetterService.editActive({
                LetterID: data.LetterID, 
                IsActive: active
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

        vm.lihatDetail = lihatDetail;
        function lihatDetail(data) {
            console.info("masuk form add/edit");
            var data = {
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-rekanan/warning-letter/DetailWarningLetter.html',
                controller: 'DetailWarningLetterCtrl',
                controllerAs: 'DetailwarningLetterCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.jLoad(1);
            });
        }


    }
})();
//TODO

function Kata(srcText) {
    var self = this;
    self.srcText = srcText;
}

