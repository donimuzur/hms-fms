(function () {
    'use strict';

    angular.module("app").controller("PostingAanwijzingCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'AanwijzingService', '$state', 'UIControlService', 'UploadFileConfigService',
        'UploaderService', 'GlobalConstantService', '$uibModal', '$stateParams', 'item', '$uibModalInstance', 'AuthService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService,
        AanwijzingService, $state, UIControlService, UploadFileConfigService,
        UploaderService, GlobalConstantService, $uibModal, $stateParams, item, $uibModalInstance, AuthService) {
        var vm = this;
        vm.IDAwj = item.IDAwj;
        vm.QuestionTitle = '';
        vm.Question = '';
        vm.fileUpload;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart("aanwijzing");
            getTypeSizeFile();
        }

        function getTypeSizeFile() {
            UploadFileConfigService.getByPageName("PAGE.VENDOR.TENDER.AANWIJZING", function (response) {
                UIControlService.unloadLoading();
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
                UIControlService.unloadLoading();
                return;
            });
        }

        vm.save = save;
        function save() {
            console.info("masuuuk");
            if (vm.fileUpload === undefined) {
                processsave('');
            } else {
                uploadFile();
            }
        }

        /*proses upload file*/
        function uploadFile() {
            AuthService.getUserLogin(function (reply) {
                vm.VendorLogin = reply.data.CurrentUsername;
                if (validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
                    upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes, vm.VendorLogin);
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
            });
        }

        function upload(file, config, filters, dates, callback) {
            var size = config.Size;
            var unit = config.SizeUnitName;
            if (unit == 'SIZE_UNIT_KB') {
                size *= 1024;
            }

            if (unit == 'SIZE_UNIT_MB') {
                size *= (1024 * 1024);
            }

            UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_FILE");
            UploaderService.uploadSingleFile(file, "UPLOAD_DIRECTORIES_VENDORDATA", size, filters, dates,
                function (response) {
                    console.info()
                    UIControlService.unloadLoading();
                    if (response.status == 200) {
                        var url = response.data.Url;
                        UIControlService.msg_growl("success", "FORM.MSG_SUC_UPLOAD");
                        processsave(url);

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
            }
            else {
                selectedFileType = selectedFileType;
            }
            //console.info("filenew:" + selectedFileType);
            //jika excel
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

        /* end proses upload*/
        
        function processsave(url) {
			if(url === null){
				url = " ";
			}
            var datasimpan = {
                AanwijzingStepID: vm.IDAwj,
                QuestionTitle: vm.QuestionTitle,
                Question: vm.Question,
                UploadURL: url
            }
            AanwijzingService.postingQuestionByVendor(datasimpan, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "Berhasil simpan pertanyaan aanwijzing");
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

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();