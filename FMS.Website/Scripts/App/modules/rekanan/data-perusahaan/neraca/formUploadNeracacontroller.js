(function () {
    'use strict';

    angular.module("app")
            .controller("frmUploadNeracaCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'BalanceVendorService', 'UploadFileConfigService',
        'UIControlService', 'UploaderService', 'item', '$uibModalInstance', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, BalanceVendorService, UploadFileConfigService,
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
        var allowed = false;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart("vendor-balance");
            UIControlService.loadLoading("MESSAGE.LOADING");
            //get tipe dan max.size file - 1
            UploadFileConfigService.getByPageName("PAGE.VENDOR.BALANCE", function (response) {
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

        //get tipe dan max.size file - 2
        function generateFilterStrings(allowedTypes) {
            console.info(allowedTypes);
            var filetypes = "";
            for (var i = 0; i < allowedTypes.length; i++) {
                filetypes += "." + allowedTypes[i].Name + ",";
            }
            return filetypes.substring(0, filetypes.length - 1);
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
            return true;
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
                        BalanceVendorService.insert({
                            DocUrl: vm.pathFile
                        },
                  function (reply) {
                      console.info("reply" + JSON.stringify(reply))
                      UIControlService.unloadLoadingModal();
                      if (reply.status === 200) {
                          UIControlService.msg_growl("success", "Berhasil Upload Berkas Neraca !!");
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