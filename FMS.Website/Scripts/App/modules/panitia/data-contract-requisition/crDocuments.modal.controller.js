(function () {
    'use strict';

    angular.module("app")
    .controller("crDocsModalCtrl", ctrl);

    ctrl.$inject = ['$state', '$scope', '$http', '$filter', 'item', '$uibModalInstance', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService', 'UploaderService', 'UploadFileConfigService'];
    /* @ngInject */
    function ctrl($state, $scope, $http, $filter, item, $uibModalInstance, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService, UploaderService, UploadFileConfigService) {

        var vm = this;
        var loadmsg = "MESSAGE.LOADING";

        //vm.tglSekarang = UIControlService.getDateNow("");
        vm.doc = item.doc;
        vm.fileUpload;
        vm.title = vm.doc.ContractRequisitionDocumentId > 0 ? 'UBAH' : 'TAMBAH';

        vm.init = init;
        function init() {
            UIControlService.loadLoadingModal(loadmsg);
            $translatePartialLoader.addPart('data-contract-requisition');
            UploadFileConfigService.getByPageName("PAGE.ADMIN.CONTRACTREQUISITION.DOCS", function (response) {
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
            if (!vm.doc.DocName) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_NODOCNAME");
                return;
            }
            if (!vm.doc.DocUrl && !vm.fileUpload) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
                return;
            }

            if (vm.fileUpload) {
                uploadFile();
            } else {
                saveDoc();
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
            UploaderService.uploadSingleFileContractReqDocument(vm.doc.ContractRequisitionId, file, size, types,
            function (reply) {
                if (reply.status == 200) {
                    UIControlService.unloadLoadingModal();
                    var url = reply.data.Url;
                    var size = reply.data.FileLength;
                    vm.doc.DocUrl = url;
                    vm.doc.Size = size;
                    saveDoc();
                } else {
                    UIControlService.unloadLoadingModal();
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE_DOC'));
                }
            }, function (err) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
            });
        }

        function saveDoc() {
            UIControlService.loadLoadingModal(loadmsg);
            DataContractRequisitionService.SaveDoc(vm.doc,
            function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status == 200) {
                    UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SAVE_DOC'));
                    $uibModalInstance.close();
                } else {
                    UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SAVE_DOC'));
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