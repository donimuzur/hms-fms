(function () {
    'use strict';

    angular.module("app").controller("SuratPernyataanModalUploadCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$stateParams', '$location', 'SocketService', 'VerifiedSendService', 'SrtPernyataanService',
        '$state', 'UIControlService', '$uibModal', '$uibModalInstance', 'GlobalConstantService', 'item', 'UploadFileConfigService', 'UploaderService'];
    function ctrl($http, $translate, $translatePartialLoader, $stateParams, $location, SocketService, VerifiedSendService, SrtPernyataanService,
        $state, UIControlService, $uibModal, $uibModalInstance, GlobalConstantService, item, UploadFileConfigService, UploaderService) {
        var vm = this;

        vm.cek2 = item.act;
        vm.VendorID;
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
        vm.idFileTypes;
        vm.idFileSize;
        vm.idUploadConfigs;
        vm.DocUrl;
        vm.tglSekarang = UIControlService.getDateNow2("-");
        vm.DocType = item.DocType;
        vm.ID;

        vm.init = init;
        function init() {
            //console.info(vm.cek);
            $translatePartialLoader.addPart('surat-pernyataan');
            loadVerifiedVendor();
            UploadFileConfigService.getByPageName("PAGE.VENDOR.SURATPERNYATAAN", function (response) {
                UIControlService.unloadLoading();
                if (response.status == 200) {
                    vm.name = response.data.name;
                    vm.idUploadConfigs = response.data;
                    vm.idFileTypes = generateFilterStrings(response.data);
                    vm.idFileSize = vm.idUploadConfigs[0];
                    //console.info(vm.idFileTypes);
                } else {
                    UIControlService.msg_growl("error", ".MESSAGE.ERR_TYPEFILE");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
                return;
            });
        };

        //ambil VendorID
        vm.loadVerifiedVendor = loadVerifiedVendor;
        function loadVerifiedVendor() {
            VerifiedSendService.selectVerifikasi(function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.verified = reply.data;
                    vm.cekTemporary = vm.verified.IsTemporary;
                    vm.VendorID = vm.verified.VendorID;
                    //console.info(JSON.stringify(vm.verified.VendorID));
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Perusahaan" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }

        //get tipe dan max.size file - 2
        function generateFilterStrings(allowedTypes) {
            var filetypes = "";
            for (var i = 0; i < allowedTypes.length; i++) {
                filetypes += "." + allowedTypes[i].Name + ",";
            }
            return filetypes.substring(0, filetypes.length - 1);
        }

        vm.selectUpload = selectUpload;
        vm.fileUpload;
        function selectUpload() {
            //console.info(vm.fileUpload);
        }

        /*start upload */
        vm.uploadFile = uploadFile;
        function uploadFile() {
            if (validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
                upload(vm.VendorID, vm.fileUpload, vm.idFileSize, vm.idFileTypes);
              }
        }

        function validateFileType(file, allowedFileTypes) {

            if (!file || file.length == 0) {
                UIControlService.msg_growl("error", "No file selected!");
                return false;
            }
            return true;
        }

        vm.upload = upload;
        function upload(id, file, config, filters, callback) {

            //console.info(id);
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
            if (vm.cek2 === 1) {
                UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_FILE");
                UploaderService.uploadSingleFileBusinessConduct(id, file, size, filters,
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
                                //console.info(vm.size);
                            }
                            if (vm.flag == 1) {
                                vm.size = Math.floor(s / (1024));
                            }
                            SrtPernyataanService.insertDoc({
                                VendorId: vm.VendorID,
                                DocType: vm.DocType,
                                DocumentUrl: vm.DocUrl,
                                UploadDate: vm.tglSekarang,
                                IsActive: 1

                            },
            function (reply) {
                //console.info("reply" + JSON.stringify(reply))
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
                    console.info(err);
                    UIControlService.msg_growl("error", "Gagal Akses Api!!");
                    UIControlService.unloadLoadingModal();
                }
            );
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
            else {
                UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_FILE");
                UploaderService.uploadSingleFileAgreement(id, file, size, filters,
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
                                //console.info(vm.size);
                            }
                            if (vm.flag == 1) {
                                vm.size = Math.floor(s / (1024));
                            }

                            SrtPernyataanService.insertDoc({
                                VendorId: vm.VendorID,
                                DocType: vm.DocType,
                                DocumentUrl: vm.DocUrl,
                                UploadDate: vm.tglSekarang,
                                IsActive: 1

                            },
        function (reply) {
            //console.info("reply" + JSON.stringify(reply))
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
                console.info(err);
                UIControlService.msg_growl("error", "Gagal Akses Api!!");
                UIControlService.unloadLoadingModal();
            }
        );
                            
                           

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
        }
        

        vm.cek = cek;
        function cek() {
            VerifiedSendService.selectVerifikasi(function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.verified = reply.data;
                    vm.cekTemporary = vm.verified.IsTemporary;
                    vm.VendorID = vm.verified.VendorID;
                    vm.vendorName = vm.verified.VendorName;
                    vm.url = vm.pathFile;
                    //loadCompanyPerson(1);
                    //loadLegalDoc(1);
                    //console.info(vm.url);
                    Cekcek(1);
                    //console.info(JSON.stringify(vm.verified.VendorID));
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data Perusahaan" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }
        vm.Cekcek = Cekcek;
        function Cekcek(current) {
            //console.info("curr "+current)
            UIControlService.loadLoading("Silahkan Tunggu...");
            //vm.currentPage = current;
            //var offset = (current * 10) - 10;
            SrtPernyataanService.selectCek2({
                VendorId: vm.VendorID
            }, function (reply) {
                //console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200 && reply.data.length > 0) {
                    var data = reply.data;
                    //vm.Nama = data;
                    vm.ID = data[0].ID;
                    vm.AgreementDate = data[0].AgreementDate;
                    vm.IsAgree = data[0].IsAgree;
                    //console.info(vm.IsAgree);
                    vm.IsActive = data[0].IsActive;
                    vm.url = vm.pathFile;
                    SrtPernyataanService.updateDoc({
                        ID: vm.ID,
                        VendorId: vm.VendorID,
                        DocType: vm.DocType,
                        DocumentUrl: vm.url,
                        UploadDate: vm.tglSekarang,
                        IsAgree: vm.IsAgree,
                        AgreementDate: vm.AgreementDate,
                        IsActive: vm.IsActive

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
                else if (reply.status === 200 && reply.data.length <= 0) {
                    vm.url = vm.pathFile;
                    //console.info(vm.url);
                    SrtPernyataanService.insertDoc({
                        VendorId: vm.VendorID,
                        DocType: vm.DocType,
                        DocumentUrl: vm.url,
                        UploadDate: vm.tglSekarang,
                        IsActive: 1

                    },
            function (reply) {
                //console.info("reply" + JSON.stringify(reply))
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
                    console.info(err);
                    UIControlService.msg_growl("error", "Gagal Akses Api!!");
                    UIControlService.unloadLoadingModal();
                }
            );


                }
                else {
                    $.growl.error({ message: "Gagal mendapatkan dokumen" });
                    UIControlService.unloadLoading();
                }
            }, function (err) {
                console.info("error:" + JSON.stringify(err));
                //$.growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
            });
        }







        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };

    }
})();