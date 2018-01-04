(function () {
    'use strict';

    angular.module("app").controller("UploadDokModalCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$stateParams', '$location', 'SocketService', 'VerifiedSendService', 'UploadDokumenLainlainService',
        '$state', 'UIControlService', '$uibModal', '$uibModalInstance', 'GlobalConstantService', 'item', 'UploadFileConfigService', 'UploaderService'];
    function ctrl($http, $translate, $translatePartialLoader, $stateParams, $location, SocketService, VerifiedSendService, UploadDokumenLainlainService,
        $state, UIControlService, $uibModal, $uibModalInstance, GlobalConstantService, item, UploadFileConfigService, UploaderService) {
        var vm = this;
        //console.info("console modal upload");
        vm.detail = item.item;
        vm.VendorID;
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
        vm.Nama;
        vm.No;
        vm.ID;
        vm.idFileTypes;
        vm.idFileSize;
        vm.idUploadConfigs;
        vm.tgl = {};
        vm.DocUrl;
        vm.tglSekarang = UIControlService.getDateNow("");

        vm.init = init;
        function init() {
           
            loadVerifiedVendor();
            UIControlService.loadLoading("MESSAGE.LOADING");
            //get tipe dan max.size file - 1
            UploadFileConfigService.getByPageName("PAGE.VENDOR.UPLOADDL", function (response) {
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
            if (vm.isAdd === 1) {
                vm.action = "Tambah";
            }
            else{
                vm.action = "Ubah";
                vm.ID = item.item.ID;
                vm.Nama = item.item.DocumentName;
                vm.No = item.item.DocumentNo;
                vm.tgl.StartDate = item.item.ValidDate;
                //console.info(vm.tgl.StartDate);
            }


        }

        vm.simpan = simpan;
        function simpan() {
            if (vm.Nama === "") {
                alert("Nama Dokumen belum diisi!!");
                return;
            }
            else if (vm.No === "") {
                alert("Nomor Surat belum diisi!!");
                return;
            }
            else if (vm.tgl === "") {
                alert("tanggal belum dipilih!!");
                return;
            }
            else {
                uploadFile();
            }
        }
       

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
                //console.info("error:" + JSON.stringify(err));
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
        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        }

        vm.selectUpload = selectUpload;
        //vm.fileUpload;
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
                UIControlService.msg_growl("error", "File tidak ada!");
                return false;
            }
            return true;
        }

        vm.upload = upload;
        function upload(id, file, config, filters, callback) {
            convertToDate();

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
            UploaderService.uploadSingleFileUploadDocument(id, file, size, filters,
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
                        if (vm.flag == 0) {
                            vm.size = Math.floor(s);
                            
                        }
                        if (vm.flag == 1) {
                            vm.size = Math.floor(s / (1024));
                        }
                        if (vm.isAdd === 1) { 
                        UploadDokumenLainlainService.insert({
                            DocumentName: vm.Nama,
                            ValidDate : vm.tgl.StartDate,
                            DocumentNo: vm.No,
                            DocumentUrl: vm.DocUrl,
                            VendorID: vm.VendorID
                        },
                                function (reply) {
                                    //console.info("reply" + JSON.stringify(reply))
                                    UIControlService.unloadLoadingModal();
                                    if (reply.status === 200) {
                                        //console.info(vm.tgl.StartDate);
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
                            vm.tgl.StartDate = UIControlService.getStrDate(vm.tgl.StartDate);
                            UploadDokumenLainlainService.Update({
                                DocumentName: vm.Nama, DocumentUrl: vm.DocUrl, DocumentNo: vm.No, VendorID: vm.VendorID, ValidDate: vm.tgl.StartDate, ID : vm.ID
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
                                //console.info(vm.tgl.StartDate);
                                UIControlService.msg_growl("error", "Gagal Akses Api!!");
                                UIControlService.unloadLoadingModal();
                            });
                        }

                    } else {
                        UIControlService.msg_growl("error", "MESSAGE.ERR_UPLOAD");
                        return;
                    }
                },
                function (response) {
                    //console.info(response);
                    UIControlService.msg_growl("error", "MESSAGE.API")
                    UIControlService.unloadLoading();
                });



        }

        function convertAllDateToString() { // TIMEZONE (-)
                vm.tgl = UIControlService.getStrDate(vm.tgl);
        };

        function convertToDate() {
                vm.tgl.StartDate = UIControlService.getStrDate(vm.tgl.StartDate);
        }

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };
    }
})();
