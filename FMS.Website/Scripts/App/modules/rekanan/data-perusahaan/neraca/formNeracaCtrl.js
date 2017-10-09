(function () {
    'use strict';

    angular.module("app")
            .controller("formNeracaCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'BalanceVendorService', 'UploadFileConfigService',
        'UIControlService', 'UploaderService', 'item', '$uibModalInstance', 'GlobalConstantService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, BalanceVendorService, UploadFileConfigService,
        UIControlService, UploaderService, item, $uibModalInstance, GlobalConstantService) {

        var vm = this;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.isAdd = item.act;
        vm.item = item.item;
        vm.action = "";
        vm.fileUpload;
        vm.Nominal;
        vm.Amount;
        vm.size;
        vm.name;
        vm.type;
        vm.flag;
        vm.DocUrl;
        vm.isApprovedCR = false;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('vendor-balance');
            //get tipe dan max.size file - 1
            UploadFileConfigService.getByPageName("PAGE.VENDOR.BALANCE", function (response) {
                UIControlService.unloadLoading();
                if (response.status == 200) {
                    vm.name = response.data.name;
                    vm.idUploadConfigs = response.data;
                    vm.idFileTypes = generateFilterStrings(response.data);
                    vm.idFileSize = vm.idUploadConfigs[0];

                   console.info(JSON.stringify(response));

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
                vm.Amount = vm.item.Amount;
                vm.Nominal = vm.item.Nominal;
                vm.fileUpload = vm.item.DocUrl;
            }
            loadAsset();
            loadUnit();
            loadCheckCR();

        }
        vm.changeSubCOA = changeSubCOA;
        function changeSubCOA(param) {
            //console.info("param:"+JSON.stringify(param));
                BalanceVendorService.getUnit(function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        vm.listUnit = reply.data.List;
                        if (param.Name === 'SUB_CASH_TYPE2') {
                            vm.selectedUnit = vm.listUnit[0];
                        }
                        else if (param.Name === 'SUB_CASH_TYPE3') {
                            vm.selectedUnit = vm.listUnit[2];
                        }
                        else if (param === 0) {
                            vm.selectedUnit = vm.listUnit[1];
                        }
                    }
                }, function (err) {
                    UIControlService.msg_growl("error", "MESSAGE.API");
                    UIControlService.unloadLoading();
                });
        }

        function loadCheckCR() {
            UIControlService.loadLoading("Silahkan Tunggu");
            BalanceVendorService.getCRbyVendor(function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    if (!(reply.data === null) && reply.data.ApproveBy === 1) {
                        vm.isApprovedCR = true;
                    }
                    else {
                        vm.isSentCR = false;
                    }
                }

            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }
        vm.selectedAsset;
        vm.listAsset;
        function loadAsset() {
            BalanceVendorService.getAsset(function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.listAsset = reply.data.List;
                    if (vm.isAdd === false) {
                        for (var i = 0; i < vm.listAsset.length; i++) {
                            if (item.item.Wealth.RefID === vm.listAsset[i].RefID) {
                                vm.selectedAsset = vm.listAsset[i];
                                loadCOA(vm.selectedAsset);
                                break;
                            }
                        }
                    }

                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.loadCOA = loadCOA;
        vm.selectedCOA;
        vm.disableLU;
        vm.listCOA;
        function loadCOA(data) {
            console.info(JSON.stringify(data.RefID));
            vm.param = "";
            if (data.RefID === 3097)
            {
                vm.param = "COA_TYPE_ASSET"
            }
            else if (data.RefID === 3099) {
                vm.param = "COA_TYPE_DEBTH"
            }

            BalanceVendorService.getCOA({
                Keyword: vm.param
            },function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.listCOA = reply.data.List;
                    console.info("coa"+JSON.stringify(vm.listCOA));
                    if (vm.isAdd === false) {
                        for (var i = 0; i < vm.listCOA.length; i++) {
                            if (item.item.COA.RefID === vm.listCOA[i].RefID) {
                                vm.selectedCOA = vm.listCOA[i];
                                loadSubCOA(vm.selectedCOA);
                                break;
                            }
                        }
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }


        vm.loadSubCOA = loadSubCOA;
        vm.selectedSubCOA;
        vm.listSubCOA;
        function loadSubCOA(data) {
            console.info("coss"+JSON.stringify(data.RefID));
            vm.param = "";
            if (data.RefID === 3100) {
                vm.param = "SUB_COA_CASH"
            }
            else if (data.RefID === 3101) {
                vm.param = "SUB_COA_DEBTHSTOCK"
            }
            console.info(JSON.stringify(vm.param));
            BalanceVendorService.getSubCOA({
                Keyword: vm.param
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.listSubCOA = reply.data.List;
                    console.info("listsub" + JSON.stringify(vm.listSubCOA.length));
                    var param = vm.listSubCOA.length;
                    if (param === 0) {
                        changeSubCOA(param);
                    }
                    if (vm.isAdd === false) {
                        for (var i = 0; i < vm.listSubCOA.length; i++) {
                            if (item.item.SubCOA.RefID === vm.listSubCOA[i].RefID) {
                                vm.selectedSubCOA = vm.listSubCOA[i];
                                break;
                            }
                        }
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.selectedUnit;
        vm.listUnit;
        function loadUnit() {
            BalanceVendorService.getUnit(function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.listUnit = reply.data.List;
                    console.info("list unit:"+JSON.stringify(vm.listUnit))
                    if (vm.isAdd === false) {
                        for (var i = 0; i < vm.listUnit.length; i++) {
                            if (item.item.Unit.RefID === vm.listUnit[i].RefID) {
                                vm.selectedUnit = vm.listUnit[i];
                                break;
                            }
                        }
                    }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
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

        vm.uploadFile = uploadFile;
        function uploadFile() {
            if (vm.fileUpload !== undefined && vm.isAdd === false) {
                vm.DocUrl = item.item.DocUrl;
                addToSave();
            }
            else if (vm.isAdd === true) {
                if (validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
                    upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes, "");
                }
            }
        }

        function validateFileType(file, allowedFileTypes) {
            //console.info(JSON.stringify(allowedFileTypes));
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
                        vm.DocUrl = url;
                        vm.name = response.data.FileName;
                        var s = response.data.FileLength;
                        if (vm.flag == 0) {

                            vm.size = Math.floor(s)
                        }

                        if (vm.flag == 1) {
                            vm.size = Math.floor(s / (1024));
                        }
                        addToSave();
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

        vm.addToSave = addToSave;
        vm.vendor = {};
        function addToSave() {
            if (vm.selectedAsset === undefined) {
                UIControlService.msg_growl("warning", "Jenis Kekayaan Belum di pilih"); return;
            }
            if (vm.selectedCOA === undefined) {
                UIControlService.msg_growl("warning", "Jenis COA Belum di pilih"); return;
            }
            if (vm.selectedSubCOA === undefined && vm.listSubCOA.length !== 0) {
                UIControlService.msg_growl("warning", "Jenis Sub COA Belum di pilih"); return;
            }
            if (vm.isAdd === true) {
                if (vm.listSubCOA.length === 0) {
                    vm.vendor = {
                        isApprovedCR: vm.isApprovedCR,
                        DocUrl: vm.DocUrl,
                        Nominal: vm.Nominal,
                        Amount: vm.Amount,
                        UnitType: vm.selectedUnit.RefID,
                        COAType: vm.selectedCOA.RefID,
                        WealthType: vm.selectedAsset.RefID
                    };
                }
               else{
                    vm.vendor = {
                        isApprovedCR: vm.isApprovedCR,
                        DocUrl: vm.DocUrl,
                        Nominal: vm.Nominal,
                        Amount: vm.Amount,
                        UnitType: vm.selectedUnit.RefID,
                        SubCOAType: vm.selectedSubCOA.RefID,
                        COAType: vm.selectedCOA.RefID,
                        WealthType: vm.selectedAsset.RefID
                    };
                }
                
            }
            else if (vm.isAdd === false) {
                if (vm.listSubCOA.length === 0) {
                    vm.vendor = {
                        isApprovedCR: vm.isApprovedCR,
                        BalanceID: item.item.BalanceID,
                        DocUrl: vm.DocUrl,
                        Nominal: vm.Nominal,
                        Amount: vm.Amount,
                        UnitType: vm.selectedUnit.RefID,
                        COAType: vm.selectedCOA.RefID,
                        WealthType: vm.selectedAsset.RefID
                    };
                }
                else {
                    vm.vendor = {
                        isApprovedCR: vm.isApprovedCR,
                        BalanceID: item.item.BalanceID,
                        DocUrl: vm.DocUrl,
                        Nominal: vm.Nominal,
                        Amount: vm.Amount,
                        UnitType: vm.selectedUnit.RefID,
                        SubCOAType: vm.selectedSubCOA.RefID,
                        COAType: vm.selectedCOA.RefID,
                        WealthType: vm.selectedAsset.RefID
                    };
                }
            }
            console.info(JSON.stringify(vm.isAdd));
            if (vm.isAdd === true) {
                BalanceVendorService.insert(vm.vendor,
                    function (reply) {
                        console.info("reply" + JSON.stringify(reply))
                        UIControlService.unloadLoadingModal();
                        if (reply.status === 200) {
                            UIControlService.msg_growl("success", "Berhasil Simpan Data Neraca !!");
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
            else if (vm.isAdd === false) {
                BalanceVendorService.update(vm.vendor,
                    function (reply) {
                        console.info("reply" + JSON.stringify(reply))
                        UIControlService.unloadLoadingModal();
                        if (reply.status === 200) {
                            UIControlService.msg_growl("success", "Berhasil Simpan Data Neraca !!");
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
        }

    }
})();