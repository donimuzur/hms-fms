(function () {
    'use strict';

    angular.module("app").controller("UploadDokumenJasaCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 
        'OfferEntryService', '$state', 'UIControlService', 'UploadFileConfigService',
        'UploaderService', 'item', '$uibModalInstance', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, 
        OEService, $state, UIControlService, UploadFileConfigService,
        UploaderService, item, $uibModalInstance, GlobalConstantService) {
        var vm = this;
        //console.info("item:" + JSON.stringify(item));
        vm.fileUpload;
        vm.IDTender = item.IDTender;
        vm.data = item.dataDoc;
        vm.DocUrl = item.dataDoc.DocumentURL;
        vm.DocName = item.dataDoc.DocumentName;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.ischeck = item.ischeck;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart("purchase-requisition");
            loadTypeSizeFile();
            //console.info(JSON.stringify(vm.DocUrl));
        }

        function loadTypeSizeFile() {
            UIControlService.loadLoadingModal("MESSAGE.LOADING");
            //get tipe dan max.size file - 1
            UploadFileConfigService.getByPageName("PAGE.ADMIN.TENDER.SERVICEOFFER", function (response) {
                UIControlService.unloadLoadingModal();
                if (response.status == 200) {
                    vm.idUploadConfigs = response.data;
                    vm.idFileTypes = UIControlService.generateFilterStrings(response.data);
                    vm.idFileSize = vm.idUploadConfigs[0];

                } else {
                    UIControlService.msg_growl("error", ".MESSAGE.ERR_TYPEFILE");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoadingModal();
                return;
            });
        }
        /*proses upload file*/
        vm.uploadFile = uploadFile;
        function uploadFile() {
            var folder = "TENDERJASA_" + vm.IDTender;
            var tipefileupload = typefile(vm.fileUpload, vm.idUploadConfigs);
            if (validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
                upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes, folder, tipefileupload);
            }

        }

        function upload(file, config, filters, folder, tipefile, callback) {
            var size = config.Size;
            var unit = config.SizeUnitName;
            if (unit == 'SIZE_UNIT_KB') {
                size *= 1024;
            }

            if (unit == 'SIZE_UNIT_MB') {
                size *= (1024 * 1024);
            }

            //console.info(file[0].size + ":" + file[0].type);
            var tipesize_file = tipefile + " / " + Math.floor(file[0].size / 1024) + " KB";
            console.info(tipesize_file);
            
            UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_FILE");
            UploaderService.uploadSingleFile(file, "UPLOAD_DIRECTORIES_ADMIN", size, filters, folder,
                function (response) {
                    UIControlService.unloadLoading();
                    if (response.status == 200) {
                        var url = response.data.Url;
                        //vm.dataExp.DocumentURL = url;
                        //vm.pathFile = vm.folderFile + url;
                        UIControlService.msg_growl("success", "FORM.MSG_SUC_UPLOAD");
                        saveProcess(url, tipesize_file);

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

        function validateFileType(file, allowedFileTypes) {
            //console.info(JSON.stringify(allowedFileTypes));
            var selectedFileType = typefile(file, allowedFileTypes);
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

        function typefile(file, allowedFileTypes) {
            if (!file || file.length == 0) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
                return false;
            }

            var selectedFileType = file[0].type;
            selectedFileType = selectedFileType.substring(selectedFileType.lastIndexOf('/') + 1);
            //console.info("tipefile: " + selectedFileType);
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
            return selectedFileType;
            //console.info("file:" + selectedFileType);
        }
        /* end proses upload*/

        function saveProcess(docurl,tipesize) {
            var savedata = {
                ID: vm.data.ID,
                IsRequired: vm.data.IsRequired,
                DocumentURL: docurl,
                FileType: tipesize
            }
            if (vm.ischeck === false) {
                OEService.updateDocs(savedata, function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        UIControlService.msg_growl("success", "Berhasil Simpan Dokumen");
                        //loadData();
                        $uibModalInstance.close();
                    }
                    else {
                        UIControlService.msg_growl("error", "FORM.MSG_ERR_SAVE");
                        return;
                    }
                }, function (err) {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                    UIControlService.unloadLoadingModal();
                });
            } else {
                OEService.updateChecklist(savedata, function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        UIControlService.msg_growl("success", "Berhasil Simpan Dokumen");
                        //loadData();
                        $uibModalInstance.close();
                    }
                    else {
                        UIControlService.msg_growl("error", "FORM.MSG_ERR_SAVE");
                        return;
                    }
                }, function (err) {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                    UIControlService.unloadLoadingModal();
                });
            }
        }

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();