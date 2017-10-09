(function () {
    'use strict';

    angular.module("app").controller("BankDetailModalCtrl", ctrl);

    ctrl.$inject = ['$http', '$uibModalInstance', 'item', '$filter', '$translate', '$translatePartialLoader', '$location', 'VerifiedSendService', 'SocketService', 'VerifikasiDataService', 'UploaderService', 'UIControlService', 'GlobalConstantService', 'UploadFileConfigService'];
    /* @ngInject */
    function ctrl($http, $uibModalInstance, item, $filter, $translate, $translatePartialLoader, $location, VerifiedSendService, SocketService, VerifikasiDataService, UploaderService, UIControlService, GlobalConstantService, UploadFileConfigService) {
        var vm = this;

        //console.info("console modal tambah");
        vm.detail = item.item;
        vm.VendorID;
        vm.NamaVendor;
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
        vm.isCalendarOpened = [false, false, false];
        vm.NamaAkun;
        vm.NoRek;
        vm.NamaBank;
        vm.Cabang;
        vm.Swift;
        vm.ID;
        vm.idFileTypes;
        vm.idFileSize;
        vm.idUploadConfigs;
        vm.DocUrl;

        vm.init = init;
        function init() {
            loadVerifiedVendor();
            loadCurrency();
            UIControlService.loadLoading("MESSAGE.LOADING");
            //get tipe dan max.size file - 1
            UploadFileConfigService.getByPageName("PAGE.VENDOR.BANKDETAIL", function (response) {
                UIControlService.unloadLoading();
                if (response.status == 200) {
                    //console.info(response);
                    vm.name = response.data.name;
                    vm.idUploadConfigs = response.data;
                    vm.idFileTypes = generateFilterStrings(response.data);
                    vm.idFileSize = vm.idUploadConfigs[0];
                } else {
                    UIControlService.msg_growl("error", ".error!");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "gagal akses API!");
                UIControlService.unloadLoading();
                return;
            });
            vm.ID = item.item.ID;
            vm.Doc = item.item.UrlDoc;
            vm.NamaAkun = item.item.AccountName;
            vm.NoRek = item.item.AccountNo;
            vm.NamaBank = item.item.BankName;
            vm.Cabang = item.item.Branch;
            vm.VendorID = item.item.VendorID;
            vm.Swift = item.item.SwiftCode;
        }

        vm.loadCurrency = loadCurrency;
        function loadCurrency() {
            VerifiedSendService.getCurrencies(function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.currencyList = reply.data;
                    for (var i = 0; i < vm.currencyList.length; i++) {
                        if (vm.detail.CurrencyId == vm.currencyList[i].CurrencyID)
                            vm.currency = vm.currencyList[i];
                    }
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data bank." });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                //console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        //get VendorID
        vm.loadVerifiedVendor = loadVerifiedVendor;
        function loadVerifiedVendor() {
            VerifiedSendService.selectVerifikasi(function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.verified = reply.data;
                    vm.cekTemporary = vm.verified.IsTemporary;
                    vm.VendorID = vm.verified.VendorID;
                    vm.NamaVendor = vm.verified.VendorName;
                    //console.info(JSON.stringify(vm.NamaVendor));
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data bank" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                //console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
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
        //vm.fileUpload;
        function selectUpload() {
            //console.info(vm.fileUpload);
        }

        vm.uploadFile = uploadFile;
        function uploadFile() {
            if (vm.Doc) {
                if (vm.fileUpload) {
                    if (validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
                        upload(vm.VendorID, vm.fileUpload, vm.idFileSize, vm.idFileTypes);
                    }
                }
                else {
                    vm.pathFile = vm.Doc;
                    savedata();
                }
            }
           
        }

        function validateFileType(file, allowedFileTypes) {

            if (!file || file.length == 0) {
                UIControlService.msg_growl("error", "file tidak ada ");
                return false;
            }
            return true;
        }

        vm.upload = upload;
        function upload(id, file, config, filters, callback) {

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
            UploaderService.uploadSingleFileBankDetail(id, file, size, filters,
                function (response) {
                    UIControlService.unloadLoading();
                    //console.info("response:" + JSON.stringify(response));
                    if (response.status == 200) {
                        //console.info(response);
                        var url = response.data.Url;
                        vm.pathFile = url;
                        vm.name = response.data.FileName;
                        var s = response.data.FileLength;
                        vm.DocUrl = vm.pathFile;
                        //console.info(vm.DocUrl);
                        if (vm.flag == 0) {
                            vm.size = Math.floor(s);
                          //  console.info(vm.size);

                        }
                        if (vm.flag == 1) {
                            vm.size = Math.floor(s / (1024));
                        }
                        savedata();

                    } else {
                        UIControlService.msg_growl("error", "error");
                        return;
                    }
                },
                function (response) {
                    console.info(response);
                    UIControlService.msg_growl("error", "gagal akses api!")
                    UIControlService.unloadLoading();
                });



        }

        vm.savedata = savedata;
        function savedata() {
            VerifikasiDataService.updateBankDetail({
                ID: vm.ID,
                CurrencyId: vm.currency.CurrencyID,
                VendorID: vm.VendorID,
                AccountNo: vm.NoRek,
                AccountName: vm.NamaAkun,
                BankName: vm.NamaBank,
                Branch: vm.Cabang,
                SwiftCode: vm.Swift,
                UrlDoc: vm.pathFile
            }, function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    UIControlService.msg_growl("success", "Berhasil Simpan Dokumen!!");
                    $uibModalInstance.close();
                }
                else {
                    UIControlService.msg_growl("error", "Gagal menyimpan data!!");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "Gagal Akses Api!!");
                UIControlService.unloadLoadingModal();
            });
        }


        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();