(function () {
    'use strict';

    angular.module("app").controller("uploadDokCtrl", ctrl);

    ctrl.$inject = ['$state', '$stateParams', 'item', '$http', '$filter','$uibModalInstance', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataKontrakService', 'UIControlService', 'GlobalConstantService', 'UploaderService', 'UploadFileConfigService'];
    /* @ngInject */
    function ctrl($state, $stateParams, item, $http, $filter, $uibModalInstance, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataKontrakService, UIControlService, GlobalConstantService, UploaderService, UploadFileConfigService) {

        var vm = this;
        vm.fileUpload;
        vm.datalempar = item.datalempar;
        var loadmsg = "MESSAGE.LOADING";
        vm.currentPage = 1;
        vm.DocName = "";
        vm.pageSize = 10;
        vm.totalItems = 0;
        vm.keyword = "";
        vm.column = 1;
        vm.maxSize = 10;
        vm.list = [];
        vm.init = init;
        vm.contractSignOff;

        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        var id = vm.datalempar.id;
        vm.id = vm.datalempar.id;

        function init() {
            $translatePartialLoader.addPart('upload-dokumen-pendukung');
            loadTypeSizeFile();
          //  loadPaket();
        };

        vm.loadPaket = loadPaket;
        function loadPaket() {
            UIControlService.loadLoading(loadmsg);
            DataKontrakService.DetailDok({
                Offset: vm.pageSize * (vm.currentPage - 1),
                Limit: vm.pageSize,
                Status: id
                
            },
            function (reply) {
                console.info("data:" + JSON.stringify(reply.data));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.contractSignOff = data;
                    vm.list = reply.data.List;
                    vm.totalItems = Number(data.Count);
                }
                else {
                    $.growl.error({ message: "MESSAGE.ERR_LOAD" });
                    UIControlService.unloadLoading();
                }
            },
            function (error) {
                UIControlService.unloadLoading();
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
            });
        }

        function loadTypeSizeFile() {
            UIControlService.loadLoading("MESSAGE.LOADING");
            //get tipe dan max.size file - 1
            UploadFileConfigService.getByPageName("PAGE.ADMIN.CONTRACTREQUISITION.DOCS", function (response) {
                UIControlService.unloadLoading();
                if (response.status == 200) {
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
            if (!vm.datalempar.DocName) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_LOAD_STEP");
                return;
            }
            if (!vm.datalempar.DocUrl && !vm.fileUpload) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
                return;
            }

            if (vm.fileUpload) {
                uploadFile();
            } else {
                saveDoc();
            }
        }

        /*proses upload file*/
        vm.uploadFile = uploadFile;
        function uploadFile() {
            var folder = "VaritionTender_" + vm.id + vm.datalempar.DocName;
            if (vm.fileUpload === undefined) {
                UIControlService.msg_growl("error", "MESSAGE.MSG_NOFILE");
                return;
            }

            if (UIControlService.validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
                upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes, folder);
            }
        }
        function upload(file, config, filters, folder, callback) {
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

            UIControlService.loadLoading("LOADING");
            UploaderService.uploadSingleFile(file, "UPLOAD_DIRECTORIES_ADMIN", size, filters, folder,
                function (response) {
                    UIControlService.unloadLoading();
                    if (response.status == 200) {
                        var url = response.data.Url;
                        vm.datalempar.DocUrl = url;
                        var contract = response.data.ContractRequisitionId;
                        vm.pathFile = vm.folderFile + url;
                       // console.info("sendata:" + JSON.stringify(fileName));

                        UIControlService.msg_growl("success", "MESSAGE.SUCCESS_UPLOAD");
                        saveProcess(url, size);

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
        /* end proses upload*/
        function saveProcess(docurl, docsize) {
            var senddata = {
                DocUrl: vm.datalempar.DocUrl,
                DocName: vm.datalempar.DocName,
                ContractSignOffId: id               

            }
            DataKontrakService.insertDok(senddata, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "MESSAGE.SUCCESS_SAVE");
                    $uibModalInstance.close();
                  //  loadPaket();
                }
                else {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                UIControlService.unloadLoadingModal();
            });
        }

        vm.cancel = cancel;
        function cancel() {
            $uibModalInstance.dismiss();
        };

        vm.convertDate = convertDate;
        function convertDate(date) {
            return UIControlService.convertDate(date);
        }
    }
})();
