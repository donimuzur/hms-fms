(function () {
    'use strict';

    angular.module("app").controller("PostingInfoCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'AanwijzingService', '$state', 'UIControlService', 'UploadFileConfigService',
        'UploaderService', 'GlobalConstantService', '$uibModal', '$stateParams', 'item', '$uibModalInstance', ];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService,
        AanwijzingService, $state, UIControlService, UploadFileConfigService,
        UploaderService, GlobalConstantService, $uibModal, $stateParams, item, $uibModalInstance) {
        var vm = this;
        vm.title = item.Title;
        vm.post = item.Post;
        vm.pathfile = item.UploadURL;
        vm.fileUpload;
        //console.info(JSON.stringify(item));

        vm.folderFile = GlobalConstantService.getConstant('api') + "/";

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart("aanwijzing");
            getTypeSizeFile();
        }

        function getTypeSizeFile() {
            UploadFileConfigService.getByPageName("PAGE.ADMIN.TENDER.AANWIJZING", function (response) {
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
                UIControlService.msg_growl("error", "MESSAGE.ERR_TYPEFILE");
                UIControlService.unloadLoadingModal();
                return;
            });
        }
        vm.selectUpload = selectUpload;
        function selectUpload() {
            console.info(vm.fileUpload);
            //	//var test = vm.pathUpload;
        }
        /*proses upload file*/
        function uploadFile() {
            var folder = 'TENDER_';
            if (validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
                upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes, folder);
            }
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

            UIControlService.loadLoadingModal("LOADERS.LOADING_UPLOAD_FILE");
            UploaderService.uploadSingleFile(file, "UPLOAD_DIRECTORIES_ADMIN", size, filters, dates,
                function (response) {
                    console.info()
                    UIControlService.unloadLoadingModal();
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
                    UIControlService.msg_growl("error", "MESSAGE.ERR_UPLOAD")
                    UIControlService.unloadLoadingModal();
                });

        }

        function validateFileType(file, allowedFileTypes) {
            if (!file || file.length == 0) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
                return false;
            }

            return true;
        }

        /* end proses upload*/

        vm.save = save;
        function save() {		
            if (!vm.fileUpload && !vm.pathfile) {
                processsave('');
            }
            else if (vm.pathfile) {
                processsave(vm.pathfile);
            }
            else {
                uploadFile();
            }
        }

        function processsave(url) {
			if(url === null){		
				url = " ";		
			}		
            var datasimpan = {
                Title: vm.title,
                Post: vm.post,
                UploadURL: url,
                AanwijzingStepID: item.AanwijzingStepID
            }
            AanwijzingService.saveAdminPost(datasimpan, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "Berhasil melakukan posting informasi");
                    $uibModalInstance.close();
                }
                else {
                    UIControlService.msg_growl("error", "FORM.MSG_ERR_SAVE");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "FORM.MSG_ERR_SAVE");
                UIControlService.unloadLoadingModal();
            });
        }

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();