(function () {
    'use strict';

    angular.module("app")
            .controller("formDetailTenagaAhliCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'TenagaAhliService',
        'UIControlService', 'item', 'UploaderService', '$uibModalInstance', 'GlobalConstantService','UploadFileConfigService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, TenagaAhliService,
        UIControlService,item, UploaderService, $uibModalInstance, GlobalConstantService, UploadFileConfigService) {

        var vm = this;
        var type = item.type;
        vm.act = item.act;
        vm.item = item.item;
        vm.SysReference = { Value: "" };
        vm.Gender = 0;
        vm.tenagaahli = {};
        vm.fileUpload ="";
        vm.isCalendarOpened = [false, false, false, false];
        vm.addresses = {
            AddressInfo: ""
        };
        vm.countrys = {
            Name:""
        };
        vm.statuss = {
            Name: ""
        };
        vm.vendor = {};
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        
        vm.init = init;
        function init() {
            console.info(type);
            if (type == 1) {
                vm.type = "Job Experience";
            }
            else if (type === 2) {
                vm.type = "DIPLOMA";
            }
            else if (type === 3) {
                vm.type = "Sertifikat";
            }
            if (vm.act == true) vm.action = "Tambah";
            else vm.action = "Ubah";

            $translatePartialLoader.addPart('tenaga-ahli');
            console.info(vm.item);
            UIControlService.loadLoading("MESSAGE.LOADING");
            //get tipe dan max.size file - 1
            UploadFileConfigService.getByPageName("PAGE.VENDOR.EXPERTSCERTIFICATE", function (response) {
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
            if (vm.act === false) {
                vm.tenagaahli.StartDate = vm.item.StartDate;
                vm.tenagaahli.EndDate = vm.item.EndDate;
                vm.fileUpload = vm.item.DocUrl;
                vm.Description = vm.item.Description;
                convertToDate();
            }
        }

        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        };

        function convertAllDateToString() { // TIMEZONE (-)
            if (vm.tenagaahli.StartDate) {
                vm.tenagaahli.StartDate = UIControlService.getStrDate(vm.tenagaahli.StartDate);
            }
            if (vm.tenagaahli.EndDate) {
                vm.tenagaahli.EndDate = UIControlService.getStrDate(vm.tenagaahli.EndDate);
            }
        };

        //Supaya muncul di date picker saat awal load
        function convertToDate() {
            if (vm.tenagaahli.StartDate) {
                vm.tenagaahli.StartDate = new Date(Date.parse(vm.tenagaahli.StartDate));
            }
            if (vm.tenagaahli.EndDate) {
                vm.tenagaahli.EndDate = new Date(Date.parse(vm.tenagaahli.EndDate));
            }
        }

        function generateFilterStrings(allowedTypes) {
            console.info(allowedTypes);
            var filetypes = "";
            for (var i = 0; i < allowedTypes.length; i++) {
                filetypes += "." + allowedTypes[i].Name + ",";
            }
            return filetypes.substring(0, filetypes.length - 1);
        }

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };

        vm.uploadFile = uploadFile;
        function uploadFile() {

            if (vm.fileUpload !== undefined && vm.act === false) {
                vm.DocUrl = vm.item.DocUrl;
                addToSave();
            }
            else if (vm.act === true) {
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
        
        vm.addToSave = addToSave;
        vm.vendor = {};
        function addToSave() {

            if (type == 1) {
                vm.SysReference.Value = "JOB_EXPERIENCE";
            }
            else if (type === 2) {
                vm.SysReference.Value = "DIPLOMA";
            }
            else if (type === 3) {
                vm.SysReference.Value = "CERTIFICATE";
            }
            if (vm.act === true) {
                vm.vendor = {
                    VendorExpertsID: vm.item.VendorExpertsID,
                    StartDate: vm.tenagaahli.StartDate,
                    EndDate: vm.tenagaahli.EndDate,
                    Description: vm.Description,
                    DocUrl: vm.DocUrl,
                    SysReference: vm.SysReference
                };
            }
            if (vm.act === false) {
                vm.vendor = {
                    ID: vm.item.ID,
                    VendorExpertsID: vm.item.VendorExpertsID,
                    StartDate: vm.tenagaahli.StartDate,
                    EndDate: vm.tenagaahli.EndDate,
                    Description: vm.Description,
                    DocUrl: vm.DocUrl,
                    SysReference: vm.SysReference
                };
            }
            console.info(JSON.stringify(vm.vendor));
            if (vm.act === true) {
                TenagaAhliService.insertExpertCertificate(vm.vendor,
                    function (reply) {
                        console.info("reply" + JSON.stringify(reply))
                        UIControlService.unloadLoadingModal();
                        if (reply.status === 200) {
                            if (type === 1) {
                                UIControlService.msg_growl("success", "Berhasil Menambahkan Data Pengalaman Kerja !!");

                            }
                            else if (type === 2) {
                                UIControlService.msg_growl("success", "Berhasil Menambahkan Data Pendidikan !!");

                            }
                            else if (type === 3) {
                                UIControlService.msg_growl("success", "Berhasil Menambahkan Data Sertifikat !!");

                            }
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
             if (vm.act === false) {
                 TenagaAhliService.updateExpertCertificate(vm.vendor,
                    function (reply) {
                        UIControlService.unloadLoadingModal();
                        if (reply.status === 200) {
                            if (type === 1) {
                                UIControlService.msg_growl("success", "Berhasil Update Data Pengalaman Kerja !!");

                            }
                            else if (type === 2) {
                                UIControlService.msg_growl("success", "Berhasil Update Data Pendidikan !!");

                            }
                            else if (type === 3) {
                                UIControlService.msg_growl("success", "Berhasil Update Data Sertifikat !!");

                            }
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





        vm.update = update;
        vm.vendor = {};
        function update() {
            convertAllDateToString();
            UIControlService.loadLoading("Silahkan Tunggu");
            if (vm.act === false) {
                vm.addresses.AddressInfo = vm.address;
                vm.countrys.Name = vm.Nationality;
                vm.statuss.Name = vm.Status;
                vm.vendor = {
                    ID: vm.item.ID,
                    Name: vm.Name,
                    DateOfBirth: vm.tenagaahli.BirthDate,
                    Gender: vm.Gender,
                    address: vm.addresses,
                    Education: vm.Education,
                    country: vm.countrys,
                    Position: vm.Position,
                    YearOfExperience: vm.YearOfExperience,
                    Email: vm.Email,
                    Statusperson: vm.statuss,
                    Expertise: vm.Expertise
                };
                TenagaAhliService.update(vm.vendor, function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        UIControlService.msg_growl("success", "Data Tenaga Ahli Berhasil di update");
                        jLoad(1);
                    }
                    else {
                        UIControlService.msg_growl("error", "Gagal menonaktifkan data ");
                        return;
                    }
                }, function (err) {

                    UIControlService.msg_growl("error", "Gagal Akses API ");
                    UIControlService.unloadLoading();
                });
            }
            else if (vm.act === true) {
                vm.addresses.AddressInfo = vm.address;
                vm.countrys.Name = vm.Nationality;
                vm.statuss.Name = vm.Status;
                vm.vendor = {
                    Name: vm.Name,
                    DateOfBirth: vm.tenagaahli.BirthDate,
                    Gender: vm.Gender,
                    address: vm.addresses,
                    Education: vm.Education,
                    country: vm.countrys,
                    Position: vm.Position,
                    Email: vm.Email,
                    Statusperson: vm.statuss,
                    Expertise: vm.Expertise
                };
                TenagaAhliService.insert(vm.vendor, function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        UIControlService.msg_growl("success", "Data Tenaga Ahli Berhasil di update");
                        jLoad(1);
                    }
                    else {
                        UIControlService.msg_growl("error", "Gagal menonaktifkan data ");
                        return;
                    }
                }, function (err) {

                    UIControlService.msg_growl("error", "Gagal Akses API ");
                    UIControlService.unloadLoading();
                });
            }
            

        }

        vm.addToList = addToList;
        function addToList() {
            vm.addresses.AddressInfo = vm.address;
            vm.countrys.Name = vm.Nationality;
            vm.statuss.Name = vm.Status;
            vm.vendor = {
                Name: vm.Name,
                DateOfBirth: vm.tenagaahli.BirthDate,
                Gender: vm.Gender,
                address: vm.addresses,
                Education: vm.Education,
                country: vm.countrys,
                Position: vm.Position,
                YearOfExperience: vm.YearOfExperience,
                Email: vm.Email,
                Statusperson: vm.statuss,
                Expertise: vm.Expertise
            };

            console.info(JSON.stringify(vm.vendor));
        }

        vm.add = add;
        function add(data) {
            console.info(JSON.stringify(data));
        }
    }
})();