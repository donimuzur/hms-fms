(function () {
    'use strict';

    angular.module("app")
    .controller("uploadSOWDModalCtrl", ctrl);

    ctrl.$inject = ['$state', '$scope', '$http', '$filter', 'item', '$uibModalInstance', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService', 'UploaderService', 'UploadFileConfigService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, item, $uibModalInstance, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService, UploaderService, UploadFileConfigService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";

        //vm.tglSekarang = UIControlService.getDateNow("");
        vm.sowDoc = item.sowDoc;
        vm.fileUpload;
        vm.title = vm.sowDoc.ContractRequisitionSOADocId > 0 ? 'UBAH' : 'TAMBAH';

        vm.init = init;
        function init() {
            UIControlService.loadLoadingModal(loadmsg);
            $translatePartialLoader.addPart('data-contract-requisition');
            UploadFileConfigService.getByPageName("PAGE.ADMIN.CONTRACTREQUISITION.SOADOCS", function (response) {
                UIControlService.unloadLoadingModal();
                if (response.status == 200) {
                    vm.idUploadConfigs = response.data;
                    vm.idFileTypes = generateFilterStrings(response.data);
                    vm.idFileSize = vm.idUploadConfigs[0];
                } else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_FILETYPE");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                UIControlService.unloadLoadingModal();l
                return;
            });
        };

        function generateFilterStrings(allowedTypes) {
            var filetypes = "";
            for (var i = 0; i < allowedTypes.length; i++) {
                filetypes += "." + allowedTypes[i].Name + ",";
            }
            return filetypes.substring(0, filetypes.length - 1);
        }

        vm.selectUpload = selectUpload;
        function selectUpload(fileUpload) {
            vm.fileUpload = fileUpload;
        }

        vm.save = save;
        function save() {
            if (!vm.sowDoc.DocName) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_NOSOWDOCNAME");
                return;
            }
            if (!vm.sowDoc.DocUrl && !vm.fileUpload) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
                return;
            }

            if (vm.fileUpload) {
                uploadFile();
            } else {
                saveSOWDoc();
            }
        }

        function uploadFile() {
            if (validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
                upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes);
            }
        }

        function validateFileType(file) {
            if (!file || file.length == 0) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
                return false;
            }

            return true;
        }

        function upload(file, config, types) {

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

            UIControlService.loadLoadingModal(loadmsg);
            UploaderService.uploadSingleFileContractReqSOW(vm.sowDoc.ContractRequisitionId, file, size, types,
            function (reply) {
                if (reply.status == 200) {
                    UIControlService.unloadLoadingModal();
                    var url = reply.data.Url;
                    var size = reply.data.FileLength;
                    vm.sowDoc.DocUrl = url;
                    vm.sowDoc.Size = size;
                    saveSOWDoc();
                } else {
                    UIControlService.unloadLoadingModal();
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE_SOW_DOC'));
                }
            }, function (err) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        }

        function saveSOWDoc() {
            UIControlService.loadLoadingModal(loadmsg);
            DataContractRequisitionService.SaveSOWDoc(vm.sowDoc,
            function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status == 200) {
                    UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SAVE_SOW_DOC'));
                    $uibModalInstance.close();
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE_SOW_DOC'));
                }
            }, function (err) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        }

        vm.cancel = cancel;
        function cancel() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();