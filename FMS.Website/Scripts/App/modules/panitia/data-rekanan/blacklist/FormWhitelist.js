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
        vm.BlacklistID = item.item.BlacklistID;
        vm.VendorID = item.item.VendorID;
        vm.VendorName = item.item.Vendor.VendorName;
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
            }

        vm.getByBlacklistCompPers = getByBlacklistCompPers;
        function getByBlacklistCompPers(id) {
            BlacklistService.selectdetailCompPers({
                Status: id
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.listemployee = reply.data;
                    vm.employee = vm.listemployee.CompPers;
                    vm.vendorstock = vm.listemployee.VendorStock;
                    vm.listperson = vm.listemployee.DetailPerson;
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('DETAIL.MESSAGE.ERR_SELECT'));
                    UIControlService.unloadLoading();
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.API'));
                UIControlService.unloadLoading();
                return;
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

        vm.selectUpload = selectUpload;
        //vm.fileUpload;
        function selectUpload() {
            console.info(JSON.stringify(vm.fileUpload));
        }

        /*start upload */
        vm.uploadFile = uploadFile;
        function uploadFile(data) {
            if (validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
                upload(data, vm.fileUpload, vm.idFileSize, vm.idFileTypes, "");
            }
        }

        function validateFileType(file, allowedFileTypes) {
            if (!file || file.length == 0) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
                return false;
            }

            var selectedFileType = file[0].type;
            selectedFileType = selectedFileType.substring(selectedFileType.lastIndexOf('/') + 1);

            if (selectedFileType === "vnd.ms-excel") {
                selectedFileType = "xls";
            }
            else if (selectedFileType === "vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
                selectedFileType = "xlsx";
            } else if (selectedFileType === "application/msword") {
                selectedFileType = "doc";
            }
            else if (selectedFileType === "application/vnd.openxmlformats-officedocument.wordprocessingml.document" || selectedFileType == "vnd.openxmlformats-officedocument.wordprocessingml.document") {
                selectedFileType = "docx";
            }
            else {
                selectedFileType = selectedFileType;
            }
            vm.type = selectedFileType;
            console.info("filenew:" + selectedFileType);
            //jika excel
            if (selectedFileType === "vnd.ms-excel")
                var allowed = false;


            for (var i = 0; i < allowedFileTypes.length; i++) {

                if (allowedFileTypes[i].Name == selectedFileType) {
                    allowed = true;

                    return allowed;
                }
            }
            if (!allowed) {
                UIControlService.msg_growl("warning", "MESSAGE.ERR_INVALID_FILETYPE");
                return false;
            }
        }

        vm.upload = upload;
        function upload(data, file, config, filters, callback) {

            var size = config.Size;
            var unit = config.SizeUnitName;

            if (unit == 'SIZE_UNIT_KB') {
                size *= 1024;
                vm.flag = 0;
            }

            if (unit == 'SIZE_UNIT_MB') {
                size *= (1024 * 1024);
                vm.flag = 1;
            }


            UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_FILE");
            UploaderService.uploadSingleFileBlacklist(file, size, filters,
                function (response) {
                    UIControlService.unloadLoading();
                    if (response.status == 200) {
                        var url = response.data.Url;
                        vm.pathFile = url;
                        vm.name = response.data.FileName;
                        var s = response.data.FileLength;
                        if (vm.flag == 0) {

                            vm.size = Math.floor(s)
                        }

                        if (vm.flag == 1) {
                            vm.size = Math.floor(s / (1024));
                        }
                        openmodal();


                    } else {
                        UIControlService.msg_growl("error", "MESSAGE.ERR_UPLOAD");
                        return;
                    }
                },
                function (response) {
                    UIControlService.msg_growl("error", "MESSAGE.API")
                    UIControlService.unloadLoading();
                });
        }
        
        vm.cancel = cancel;
        function cancel() {
            $uibModalInstance.dismiss('cancel');
        }

        vm.batalkan = batalkan;
        function batalkan() {
            console.info(JSON.stringify(item.item.BlacklistTypeID));
            UIControlService.loadLoading("Silahkan Tunggu");
            BlacklistService.editBlacklist({
                BlacklistID: item.item.BlacklistID,
                BlacklistTypeID: item.item.BlacklistTypeID
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    //var msg = "";
                    //if (active === false) msg = " NonAktifkan ";
                    //if (active === true) msg = "Aktifkan ";
                    UIControlService.msg_growl("success", "Data Berhasil di Blacklist");
                    $uibModalInstance.close();
                    vm.jLoad(1);
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

        vm.openmodal = openmodal;
        function openmodal() {
            var data = {
                act: false,
                data: item.item,
                DocUrl: vm.pathFile,
                ReferenceNo: vm.ReferenceNo
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/data-rekanan/blacklist/formcancelblacklist.html',
                controller: 'FormCancelBlacklistCtrl',
                controllerAs: 'frmCancelBlacklistCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                $uibModalInstance.close();
            });
        }

    }
})();