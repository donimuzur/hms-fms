(function () {
    'use strict';

    angular.module("app").controller("FormWhitelistCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'BlacklistService', 'UploadFileConfigService', 'RoleService', 'UIControlService', 'UploaderService', 'item', '$uibModalInstance', '$state', 'GlobalConstantService', '$uibModal'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, BlacklistService, UploadFileConfigService,
        RoleService, UIControlService, UploaderService, item, $uibModalInstance, $state, GlobalConstantService, $uibModal) {

        var vm = this;
        var page_id = 141;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.Area;
        vm.flag = item.act;
        vm.BlacklistID = item.BlacklistId;
        vm.VendorID = item.VendorID;
        vm.VendorName = item.VendorName;
        vm.Description = "";
        vm.action = "";
        vm.DetailPerson = "";
        vm.actionblacklst = "";
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.pageSize = 10;
        vm.init = init;

        vm.isCalendarOpened = [false, false, false, false];

        vm.pathFile;
        vm.fileUpload = "";
        vm.size;
        vm.name;
        vm.type;
        vm.flag;
        vm.employee = [];

        vm.vendorstock = [];
        vm.listperson = [];
        vm.selectedMasaBlacklist = "";
        vm.blacklistdate = {};
        vm.cek1 = false;

        function init() {
            console.info(JSON.stringify(item.item));
            $translatePartialLoader.addPart("blacklist-data");
            UIControlService.loadLoading("MESSAGE.LOADING");
            //get tipe dan max.size file - 1
            UploadFileConfigService.getByPageName("PAGE.ADMIN.BLACKLIST", function (response) {
                UIControlService.unloadLoading();
                if (response.status == 200) {
                    vm.name = response.data.name;
                    vm.idUploadConfigs = response.data;
                    vm.idFileTypes = generateFilterStrings(response.data);
                    vm.idFileSize = vm.idUploadConfigs[0];

                } else {
                    UIControlService.msg_growl("error", ".MESSAGE.ERR_TYPEFILE");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
                return;
            });
            BlacklistService.BlacklistById({
                BlacklistID: vm.BlacklistID
            }, function (reply) {
                if (reply.status === 200) {
                    vm.detailBlacklist = reply.data;
                    console.info(vm.detailBlacklist);
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Rekanan" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                $.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        //get tipe dan max.size file - 2
        function generateFilterStrings(allowedTypes) {
            var filetypes = "";
            for (var i = 0; i < allowedTypes.length; i++) {
                filetypes += "." + allowedTypes[i].Name + ",";
            }
            return filetypes.substring(0, filetypes.length - 1);
        }

        vm.cancel = cancel;
        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        vm.save = save;
        function save(dt) {
            UIControlService.loadLoading("Silahkan Tunggu");
            BlacklistService.editApproval({
                ID: item.ID,
                flagUpdate: dt,
                LType: item.LType,
                BlacklistId: vm.BlacklistID,
                BlacklistType: item.BlacklistType,
                Description: vm.Description
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "Data Berhasil di Reject ");
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


    }
})();