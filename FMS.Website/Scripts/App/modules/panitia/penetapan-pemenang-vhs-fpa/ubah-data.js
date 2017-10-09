(function () {
    'use strict';

    angular.module("app").controller("UbahPPGVHSCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'PenetapanPemenangVHSservice', '$state', 'UIControlService', '$uibModal', '$uibModalInstance', 'GlobalConstantService', 'item', 'UploadFileConfigService', 'UploaderService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, PenetapanPemenangVHSservice,
        $state, UIControlService, $uibModal, $uibModalInstance, GlobalConstantService, item, UploadFileConfigService, UploaderService) {
        var vm = this;
        vm.detail = item.item;
        vm.Step= item.StepID;
        vm.reff = item.reff;
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

        vm.tglSekarang = UIControlService.getDateNow("");

        vm.init = init;
        function init() {
            console.info(item);
            UIControlService.loadLoading("MESSAGE.LOADING");
            //get tipe dan max.size file - 1
            UploadFileConfigService.getByPageName("PAGE.ADMIN.VHSAWARD", function (response) {
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

            }


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
            upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes, vm.tglSekarang);
        }
        }

        function validateFileType(file, allowedFileTypes) {
            if (!file || file.length == 0) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
                return false;
            }

            var selectedFileType = file[0].type;
            selectedFileType = selectedFileType.substring(selectedFileType.lastIndexOf('/') + 1);

            if (selectedFileType === "vnd.openxmlformats-officedocument.wordprocessingml.document") {
                selectedFileType = "docx";
            }
            else if (selectedFileType == "msword") {
                selectedFileType = "doc";
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
        function upload(file, config, filters, dates, callback) {

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
            UploaderService.uploadSingleFileVHSOfferEntry(dates, file, size, filters,
                function (response) {
                    UIControlService.unloadLoading();
                    console.info("response:" + JSON.stringify(response));
                    if (response.status == 200) {
                        var url = response.data.Url;
                        vm.pathFile = url;
                        vm.name = response.data.FileName;
                        var s = response.data.FileLength;


                        if (vm.flag == 0) {
                            vm.size = Math.floor(s);
                        }
                        else if (vm.flag == 1) {
                            vm.size = Math.floor(s / (1024));
                        } if (vm.detail.ID == 0) {
                            var dataSimpan = {
                                ID: vm.detail.ID,
                                DocumentUrl: vm.pathFile,
                                TenderStepID: vm.Step,
                                VendorID: vm.detail.VendorID,
                                StartContractDate: vm.detail.StartContractDate,
                                Duration: vm.detail.Duration,
                                FinishContractDate: vm.detail.FinishContractDate,
                                SAPContractNo: vm.detail.SAPContractNo,
                                FinishContractPVIDate: vm.detail.FinishContractPVIDate,
                                RFQVHSId: vm.reff,
                                RFQType: vm.detail.RFQType
                            };
                        }
                        else{
                            var dataSimpan = {
                                ID: vm.detail.ID,
                                DocumentUrl: vm.pathFile
                            };
                        }
                        PenetapanPemenangVHSservice.uploadDoc(
                            dataSimpan,
                                function (reply) {
                                    console.info("reply" + JSON.stringify(reply))
                                    UIControlService.unloadLoadingModal();
                                    if (reply.status === 200) {
                                        UIControlService.msg_growl("success", "Berhasil Simpan Upload");
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