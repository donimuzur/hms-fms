(function () {
    'use strict';

    angular.module("app")
            .controller("FormLibraryCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'LibraryService', 'UploadFileConfigService',
        'UIControlService', 'UploaderService', 'item', '$uibModalInstance', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, LibraryService, UploadFileConfigService,
        UIControlService, UploaderService, item, $uibModalInstance, GlobalConstantService) {

        var vm = this;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.isAdd = item.act;
        vm.action = "";
        vm.pathFile;
        vm.Description;
        vm.fileUpload;
        vm.size;
        vm.name;
        vm.type;
        vm.flag;
        vm.selectedForm;
        

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart("master-library");
            UIControlService.loadLoading("MESSAGE.LOADING");
            //get tipe dan max.size file - 1
            UploadFileConfigService.getByPageName("PAGE.ADMIN.MASTER.LIBRARY", function (response) {
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
            if (vm.isAdd === true) {
                vm.action = "Tambah";
                
            }
            else {
                vm.action = "Ubah ";
                vm.idLibrary = item.item.LibraryID;
                vm.name = item.item.DocName;
                vm.selectedDocumentType = item.item.reference.RefID;
                vm.type = item.item.FileType;
                vm.size = item.item.Size;
                vm.pathFile = item.item.DocUrl;
                vm.Description = item.item.Description;
                vm.idFileTypes = vm.name + vm.type;

            }
            getTipeWilayah();
           

        }

        vm.getTipeWilayah = getTipeWilayah;
        vm.selectedDocumentType;
        vm.listDocumentType = [];
        function getTipeWilayah() {
            LibraryService.SelectTypeForm(
               function (response) {
                   console.info("respon1:" + JSON.stringify(response));
                   if (response.status === 200) {
                       vm.listDocumentType = response.data;
                       if (vm.isAdd === false) {
                           for (var i = 0; i < vm.listDocumentType.length; i++) {
                               if (vm.selectedDocumentType === vm.listDocumentType[i].RefID) {
                                   vm.selectedDocumentType = vm.listDocumentType[i];
                                   
                                   break;
                               }
                           }
                       }
                   }
                   else {
                       UIControlService.msg_growl("error", "Gagal mendapatkan list tipe wilayah");
                       return;
                   }
               }, function (err) {
                   UIControlService.msg_growl("error", "Gagal Akses API");
                   return;
               });
        }

        vm.selected = selected;
        function selected() {
            console.info("respon1:" + JSON.stringify(vm.selectedDocumentType));
        }

        //get tipe dan max.size file - 2
        function generateFilterStrings(allowedTypes) {
            console.info(allowedTypes);
            var filetypes = "";
            for (var i = 0; i < allowedTypes.length; i++) {
                filetypes += "." + allowedTypes[i].Name + ",";
            }
            return filetypes.substring(0, filetypes.length - 1);
        }

        vm.selectUpload = selectUpload;
        //vm.fileUpload;
        function selectUpload() {
            console.info((vm.fileUpload));
        }
        /*start upload */
        vm.uploadFile = uploadFile;
        function uploadFile() {

            if (validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
                upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes, "");
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
        function upload(file, config, filters, callback) {
            
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
            UploaderService.uploadSingleFileLibrary(file, size, filters,
                function (response) {
                    UIControlService.unloadLoading();
                    console.info("response:" + JSON.stringify(response));
                    if (response.status == 200) {
                        var url = response.data.Url;
                        vm.pathFile = url;
                        vm.name = response.data.FileName;
                        var s = response.data.FileLength;
                        if (vm.flag == 0) {
                            
                            vm.size = Math.floor(s)
                        }

                        if (vm.flag == 1) {
                            vm.size = Math.floor(s/(1024));
                        }
                        if (vm.action === "Tambah") {
                            
                                LibraryService.insert({
                                    DocName: vm.name,
                                    DocType: vm.selectedDocumentType.RefID,
                                    FileType: vm.type,
                                    Size: vm.size,
                                    DocUrl: vm.pathFile,
                                    Description: vm.Description
                                },
                                    function (reply) {
                                        console.info("reply" + JSON.stringify(reply))
                                        UIControlService.unloadLoadingModal();
                                        if (reply.status === 200) {
                                            UIControlService.msg_growl("success", "Berhasil Simpan Data Library !!");
                                            $uibModalInstance.close();

                                        }
                                        else {
                                            UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                                            return;
                                        }
                                    },
                                    function (err) {
                                        UIControlService.msg_growl("error", "Gagal Akses Api!!");
                                        UIControlService.unloadLoadingModal();
                                    }
                                );
                            }
                        else if (vm.action === "Ubah ") {
                            LibraryService.update({
                                LibraryID: vm.idLibrary,
                                DocName: vm.name,
                                DocType: vm.selectedDocumentType.RefID,
                                FileType: vm.type,
                                Size: vm.size,
                                DocUrl: vm.pathFile,
                                Description: vm.Description
                            },
                                function (reply) {
                                    console.info("reply" + JSON.stringify(reply))
                                    UIControlService.unloadLoadingModal();
                                    if (reply.status === 200) {
                                        UIControlService.msg_growl("success", "Berhasil Update Data Library !!");
                                        $uibModalInstance.close();
                                    }
                                    else {
                                        UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                                        return;
                                    }
                                },
                                function (err) {
                                    UIControlService.msg_growl("error", "Gagal Akses Api!!");
                                    UIControlService.unloadLoadingModal();
                                }
                            );
                        }
                         
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

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };

    }
})();