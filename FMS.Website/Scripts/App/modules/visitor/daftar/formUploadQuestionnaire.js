(function () {
    'use strict';

    angular.module("app")
            .controller("FormQuestionnaireCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'VendorRegistrationService', 'UploadFileConfigService',
        'UIControlService', 'UploaderService', '$uibModalInstance', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, VendorRegistrationService, UploadFileConfigService,
        UIControlService, UploaderService, $uibModalInstance, GlobalConstantService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
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
            vm.VendorID =Number(localStorage.getItem('vendor_reg_id'));
            $translatePartialLoader.addPart('daftar');
            loadUploadFileConfig();
        }

        function loadUploadFileConfig() {
            UploadFileConfigService.getByPageName("PAGE.VENDOR.QUESTIONNAIRE", function (response) {
                if (response.status == 200) {
                    vm.idUploadConfigs = response.data;
                    vm.idFileTypes = generateFilterStrings(response.data);
                    vm.idFileSize = vm.idUploadConfigs[0];
                } else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_FILETYPE");
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_FILETYPE");
            });
        }

        function generateFilterStrings(allowedTypes) {
            var filetypes = "";
            for (var i = 0; i < allowedTypes.length; i++) {
                filetypes += "." + allowedTypes[i].Name + ",";
            }
            return filetypes.substring(0, filetypes.length - 1);
        }

        vm.selectUpload = selectUpload;
        function selectUpload() {
            console.info(vm.fileUpload);
        }

        vm.upload = upload;
        function upload() {
            if (vm.fileUpload) {
                uploadFile();
            } else {
                UIControlService.msg_growl('error', "NO_FILE");
                return;
            }
        }

        function validateFileType() {
            if (!vm.fileUpload || vm.fileUpload == 0) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
                return false;
            }
            return true;
        }

        function upload() {
            var size = vm.idFileSize.Size;
            var unit = vm.idFileSize.SizeUnitName;
            if (unit == 'SIZE_UNIT_KB') {
                size *= 1024;
            }
            if (unit == 'SIZE_UNIT_MB') {
                size *= (1024 * 1024);
            }
            UIControlService.loadLoadingModal(loadmsg);
            UploaderService.uploadSingleFileQuestionnaireVendor(vm.VendorID, vm.fileUpload, size, vm.idFileTypes,
            function (reply) {
                if (reply.status == 200) {
                    UIControlService.unloadLoadingModal();
                    var url = reply.data.Url;
                    vm.DocumentUrl = url;
                    saveUrl();
                } else {
                    UIControlService.unloadLoadingModal();
                    UIControlService.msg_growl("error", 'MESSAGE.ERR_UPLOAD');
                }
            }, function (err) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("error", "MESSAGE.ERR_UPLOAD");
            });
        };

        function saveUrl() {
            var questionnaire = {
                VendorID: vm.VendorID,
                QuestionnaireUrl: vm.DocumentUrl
            }

            UIControlService.loadLoadingModal(loadmsg);
            VendorRegistrationService.saveQuestionaireUrl(questionnaire, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status == 200) {
                    UIControlService.msg_growl("notice", 'KUESIONER.UPLOAD');
                    $uibModalInstance.close();
                } else {
                    UIControlService.msg_growl("error", 'MESSAGE.ERR_SAVE');
                }
            }, function (error) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_SAVE');
            });
        };

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };

    }
})();