(function () {
    'use strict';

    angular.module("app")
            .controller("formTenagaAhliCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'VerifikasiDataService',
        'UIControlService', 'item', '$uibModalInstance', 'GlobalConstantService', 'UploadFileConfigService', 'UploaderService', 'ProvinsiService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, VerifikasiDataService,
        UIControlService, item, $uibModalInstance, GlobalConstantService, UploadFileConfigService, UploaderService, ProvinsiService) {
        var loadmsg = 'MESSAGE.LOADING';
        var vm = this;
        vm.item = item.item;
        vm.data = item.item;
        vm.folderFile = GlobalConstantService.getConstant('api') + "/";
        vm.dataExp = item.item;
        var dataEdit = item.item;
        vm.flag = item.flag;
        vm.Gender = "";
        vm.tenagaahli = {};
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
        vm.radio = {
            tipeM: "M",
            tipeF: "F",
            StatusK: "CONTRACT",
            StatusI: "INTERNSHIP",
            StatusP: "PERMANENT",
        }
        vm.nationalities = ["Indonesia"];
        
        vm.init = init;
        function init() {
            console.info(vm.dataExp);
           // UIControlService.loadLoadingModal("Silahkan Tunggu");
            if (vm.flag === 8) {
            VerifikasiDataService.GetAllNationalities(function (reply) {
                UIControlService.unloadLoadingModal();
                if (reply.status === 200) {
                    vm.nationalities = reply.data;
                }
                else {
                    UIControlService.msg_growl("error", "Gagal mendapat daftar negara");
                }
            }, function (err) {
                UIControlService.msg_growl("error", "Gagal mendapat daftar negara");
                UIControlService.unloadLoadingModal();
            });
            vm.Name = vm.item.Name;
            vm.tenagaahli.BirthDate = vm.item.DateOfBirth;
            if (vm.item.Gender === "M") {
                vm.Gender = "M";
            }
            else if (vm.item.Gender === "F") {
                vm.Gender = "F";
            }
            vm.address = vm.item.address.AddressInfo;
            vm.Education = vm.item.Education;
            vm.Nationality = vm.item.country.Name;
            vm.Position = vm.item.Position;
            vm.YearOfExperience = vm.item.YearOfExperience;
            vm.Email = vm.item.Email;
            if (vm.item.Statusperson.Name === "CONTRACT") {
                vm.Status = "CONTRACT";
            }
            else if (vm.item.Statusperson.Name === "INTERNSHIP") {
                vm.Status = "INTERNSHIP";
            }
            else if (vm.item.Statusperson.Name === "PERMANENT") {
                vm.Status = "PERMANENT";
            }
            vm.Expertise = vm.item.Expertise;
            convertToDate();
        }
            else if (vm.flag === 7) {
                console.info(vm.data);
                vm.data.DateOfBirth = new Date(Date.parse(vm.data.DateOfBirth));
                    vm.data.ServiceEndDate = new Date(Date.parse(vm.data.ServiceEndDate));
                    vm.data.ServiceStartDate = new Date(Date.parse(vm.data.ServiceStartDate));
                    UploadFileConfigService.getByPageName("PAGE.VENDOR.COMPANYPERSON", function (response) {
                        if (response.status == 200) {
                            vm.idUploadConfigs = response.data;
                            vm.idFileTypes = generateFilterStrings(response.data);
                            vm.idFileSize = vm.idUploadConfigs[0];
                            VerifikasiDataService.GetPositionTypes(function (response) {
                                UIControlService.unloadLoadingModal();
                                if (response.status == 200) {
                                    vm.positionTypes = response.data;
                                    loadCountries(vm.data.Address.State);
                                    //loadCountries();getProvinsi();
                                } else {
                                    UIControlService.msg_growl("error", "MESSAGE.ERR_LOAD_POSITION");
                                }
                            }, function (err) {
                                UIControlService.unloadLoadingModal();
                                UIControlService.msg_growl("error", "MESSAGE.ERR_LOAD_POSITION");
                            });
                        } else {
                            UIControlService.unloadLoadingModal();
                            UIControlService.msg_growl("error", "MESSAGE.ERR_FILETYPE");
                        }
                    }, function (err) {
                        UIControlService.unloadLoadingModal();
                        UIControlService.msg_growl("error", "MESSAGE.ERR_FILETYPE");
                    });
                }
            else if (vm.flag === 9) {
                        vm.dataExp = new dataField(dataEdit.ContractName, dataEdit.Location, dataEdit.Address, dataEdit.Agency, dataEdit.AgencyTelpNo,
                            dataEdit.ContractNo, dataEdit.ContractValue, new Date(Date.parse(dataEdit.StartDate)),
                            new Date(Date.parse(dataEdit.EndDate)), new Date(Date.parse(dataEdit.UploadDate)), dataEdit.Field, dataEdit.FieldType,
                            dataEdit.Remark, dataEdit.ExperienceType, dataEdit.DocumentURL, dataEdit.StateLocation);
                        vm.regionID = dataEdit.StateLocationRef.Country.Continent.ContinentID;
                        vm.countryID = dataEdit.StateLocationRef.CountryID;
                        vm.datepelaksanaan = UIControlService.getStrDate(vm.dataExp.StartDate);
                        vm.dateselesai = UIControlService.getStrDate(vm.dataExp.EndDate);
                        if (vm.countryID === 360) {
                            vm.lokasidetail = dataEdit.CityLocation.Name + "," + dataEdit.StateLocationRef.Name + "," + dataEdit.StateLocationRef.Country.Name + "," + dataEdit.StateLocationRef.Country.Continent.Name;
                        } else {
                            vm.lokasidetail = dataEdit.StateLocationRef.Name + "," + dataEdit.StateLocationRef.Country.Name + "," + dataEdit.StateLocationRef.Country.Continent.Name;
                        }
                        loadCountries9(dataEdit.StateLocationRef);
            //getRegion();
            loadTypeTender();
            getTypeSizeFile();
            loadTypeExp();
            }
        }
        ////////////9
        vm.loadStates9 = loadStates9;
        function loadStates9(country) {
            if (!country) {
                country = vm.selectedCountry;
                vm.selectedState = "";
            }
            loadRegions9(country.CountryID);
            UIControlService.loadLoading("LOADERS.LOADING_STATE");
            ProvinsiService.getStates(country.CountryID,
                function (response) {
                    vm.stateList = response.data;
                    for (var i = 0; i < vm.stateList.length; i++) {
                        if (country.StateID === vm.stateList[i].StateID) {
                            vm.selectedState = vm.stateList[i];
                            if (vm.selectedState.Country.Code === 'IDN') {
                                changeState9();
                                break;
                            }
                        }
                    }
                    UIControlService.unloadLoading();
                });
        }

        vm.loadCountries9 = loadCountries9;
        function loadCountries9(data) {
            UIControlService.loadLoading("LOADERS.LOADING_COUNTRY");
            VerifikasiDataService.getCountries(
                function (response) {
                    vm.countryList = response.data;
                    for (var i = 0; i < vm.countryList.length; i++) {
                        if (data !== undefined) {
                            if (data.CountryID === vm.countryList[i].CountryID) {
                                vm.selectedCountry = vm.countryList[i];
                                loadStates9(data);
                                break;
                            }
                        }
                    }
                    UIControlService.unloadLoading();
                });
        }

        vm.loadRegions9 = loadRegions9;
        function loadRegions9(data) {
            UIControlService.loadLoading("LOADERS.LOADING_REGION");
            VerifikasiDataService.getRegions({ CountryID: data },
                function (response) {
                    console.info(response.data);
                    vm.selectedRegion = response.data;
                    UIControlService.unloadLoading();
                }
            );
        }

        vm.changeState9 = changeState9;
        vm.listCities = [];
        vm.selectedCities;
        function changeState9() {
            ProvinsiService.getCities(vm.selectedState.StateID,
               function (response) {
                   vm.listCities = response.data;
                   for (var i = 0; i < vm.listCities.length; i++) {
                       if (vm.dataExp.Location === vm.listCities[i].CityID) {
                           vm.selectedCities = vm.listCities[i];
                           changeCities9();

                           console.info("statezs");
                           break;
                       }
                   }
               },
           function (response) {
               UIControlService.msg_growl("error", "Gagal Akses API");
               return;
           });
        }

        vm.changeCities9 = changeCities9;
        function changeCities9() {
            vm.dataExp.StateLocation = vm.selectedState.StateID;
            if (vm.selectedCountry.CountryID === vm.IDN_id) {
                vm.dataExp.Location = vm.selectedCities.CityID;
                getDistrict9();
            }

        }
        function getDistrict9() {
            console.info("district");
            if (vm.data.Address.CityID) {
                UIControlService.loadLoadingModal(loadmsg);
                ProvinsiService.getDistrict(vm.data.Address.CityID, function (response) {
                    UIControlService.unloadLoadingModal();
                    vm.listKecamatan = response.data;
                }, function (response) {
                    UIControlService.unloadLoadingModal();
                    UIControlService.msg_growl("error", "MESSAGE.ERR_LOAD_DISTRICT");
                });
            }
        }





         /////////////////////// 7
        function loadCountries(data) {
            //UIControlService.loadLoading("LOADERS.LOADING_COUNTRY");
            VerifikasiDataService.getCountries(
                function (response) {
                    vm.countryList = response.data;

                    for (var i = 0; i < vm.countryList.length; i++) {
                        if (vm.countryList[i].CountryID === data.CountryID) {
                            vm.selectedCountry = vm.countryList[i];
                            vm.countryCode = data.Country.Code;
                            changeCountry1(false, data);
                        }
                    }
                }, function (response) {
                    UIControlService.unloadLoadingModal();
                    UIControlService.msg_growl("error", "MESSAGE.ERR_LOAD_STATES");;
                });
        }
        vm.changeCountry1 = changeCountry1;
        function changeCountry1(flag, data) {
            if (flag === true) {
                vm.countryCode = data.Code;
            }
            if (data.CountryID == undefined) vm.CountryID = data;
            else vm.CountryID = data.CountryID;
            getProvinsi1();
        }
        function getProvinsi1() {
            UIControlService.loadLoadingModal(loadmsg);
            ProvinsiService.getStates(vm.CountryID, function (response) {
                UIControlService.unloadLoadingModal();
                vm.listProvinsi = response.data;
                getCities1();
            }, function (response) {
                UIControlService.unloadLoadingModal();
                UIControlService.msg_growl("error", "MESSAGE.ERR_LOAD_STATES");;
            });
        }
        vm.changeProvince1 = changeProvince1;
        function changeProvince1() {
            vm.data.Address.CityID = null;
            vm.listKabupaten = [];
            vm.data.Address.DistrictID = null;
            vm.listKecamatan = [];
            getCities1();
        }
        function getCities1() {
            if (vm.data.Address.StateID) {
                UIControlService.loadLoadingModal(loadmsg);
                ProvinsiService.getCities(vm.data.Address.StateID, function (response) {
                    UIControlService.unloadLoadingModal();
                    vm.listKabupaten = response.data;
                    getDistrict1();
                }, function (response) {
                    UIControlService.unloadLoadingModal();
                    UIControlService.msg_growl("error", "MESSAGE.ERR_LOAD_CITIES");
                });
            }
        }
        vm.changeCities1 = changeCities1;
        function changeCities1() {
            vm.listKecamatan = [];
            getDistrict1();
        }
        function getDistrict1() {
            if (vm.data.Address.CityID) {
                UIControlService.loadLoadingModal(loadmsg);
                ProvinsiService.getDistrict(vm.data.Address.CityID, function (response) {
                    UIControlService.unloadLoadingModal();
                    vm.listKecamatan = response.data;
                }, function (response) {
                    UIControlService.unloadLoadingModal();
                    UIControlService.msg_growl("error", "MESSAGE.ERR_LOAD_DISTRICT");
                });
            }
        }


        vm.dataField = dataField;
        function dataField(ContractName, Location, Address, Agency, AgencyTelpNo, ContractNo, ContractValue, StartDate,
            EndDate, UploadDate, Field, FieldType, Remark, ExperienceType, DocumentURL, StateLocation) {
            var self = this;
            self.ContractName = ContractName;
            self.Location = Location;
            self.Address = Address;
            self.Agency = Agency;
            self.AgencyTelpNo = AgencyTelpNo;
            self.ContractNo = ContractNo;
            self.ContractValue = ContractValue;
            self.StartDate = StartDate;
            self.EndDate = EndDate;
            self.UploadDate = UploadDate;
            self.Field = Field;
            self.FieldType = FieldType;
            self.Remark = Remark;
            self.ExperienceType = ExperienceType;
            self.DocumentURL = DocumentURL;
            self.StateLocation = StateLocation;
        }

        vm.listTypeExp = [];
        vm.cekTypeExp = 3154;
        function loadTypeExp() {
            UIControlService.loadLoading("Loading Tipe Pengalaman");
            VerifikasiDataService.typeExperience(
            function (reply) {
                UIControlService.unloadLoading();
                vm.listTypeExp = reply.data.List;

            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API.TYPE_EXP");
                UIControlService.unloadLoading();
            });
        }

        function getTypeSizeFile() {
            UploadFileConfigService.getByPageName("PAGE.VENDOR.EXPERIENCE", function (response) {
                UIControlService.unloadLoading();
                if (response.status == 200) {
                    vm.idUploadConfigs = response.data;
                    vm.idFileTypes = UIControlService.generateFilterStrings(response.data);
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

        /*city dkk*/
        vm.listRegions = [];
        vm.selectedRegions;
        function getRegion() {
            ProvinsiService.getRegion(
                function (response) {
                    vm.listRegions = response.data;
                    if (vm.isAdd === false) {
                        for (var i = 0; i < vm.listRegions.length; i++) {
                            if (vm.regionID === vm.listRegions[i].ContinentID) {
                                vm.selectedRegions = vm.listRegions[i];
                                changeRegion();
                                break;
                            }
                        }
                    }
                },
            function (response) {
                UIControlService.msg_growl("error", "Gagal Akses API");
                return;
            });
        }

        vm.changeRegion = changeRegion;
        vm.listCountry = [];
        vm.selectedCountry;
        function changeRegion() {
            ProvinsiService.getCountries(vm.selectedRegions.ContinentID,
               function (response) {
                   //console.info("neg>" + JSON.stringify(response));
                   vm.listCountry = response.data;
                   if (vm.isAdd === false) {
                       for (var i = 0; i < vm.listCountry.length; i++) {
                           if (vm.countryID === vm.listCountry[i].CountryID) {
                               vm.selectedCountry = vm.listCountry[i];
                               changeCountry();
                               break;
                           }
                       }
                   }
               },
           function (response) {
               UIControlService.msg_growl("error", "Gagal Akses API");
               return;
           });
        }

        vm.changeCountry = changeCountry;
        vm.listState = [];
        vm.selectedState;
        function changeCountry() {
            ProvinsiService.getStates(vm.selectedCountry.CountryID,
               function (response) {
                   vm.listState = response.data;
                   //console.info(">> " + JSON.stringify(vm.listState));
                   if (vm.isAdd === false) {
                       for (var i = 0; i < vm.listState.length; i++) {
                           if (vm.dataExp.StateLocation === vm.listState[i].StateID) {
                               vm.selectedState = vm.listState[i];
                               changeState();
                               break;
                           }
                       }
                   }

               },
           function (response) {
               UIControlService.msg_growl("error", "Gagal Akses API");
               return;
           });
        }

        vm.changeState = changeState;
        vm.listCities = [];
        vm.selectedCities;
        function changeState() {
            if (!(vm.selectedCountry.CountryID === vm.IDN_id)) {
                vm.dataExp.Location = null;
                vm.dataExp.StateLocation = vm.selectedState.StateID;
            }
            ProvinsiService.getCities(vm.selectedState.StateID,
               function (response) {
                   vm.listCities = response.data;
                   //console.info(">> " + JSON.stringify(vm.listCities));
                   if (vm.isAdd === false) {
                       for (var i = 0; i < vm.listCities.length; i++) {
                           if (vm.dataExp.Location === vm.listCities[i].CityID) {
                               vm.selectedCities = vm.listCities[i];
                               changeCities();
                               break;
                           }
                       }
                   }
               },
           function (response) {
               UIControlService.msg_growl("error", "Gagal Akses API");
               return;
           });
        }

        vm.changeCities = changeCities;
        function changeCities() {
            vm.dataExp.StateLocation = vm.selectedState.StateID;
            if (vm.selectedCountry.CountryID === vm.IDN_id) {
                vm.dataExp.Location = vm.selectedCities.CityID;
            }

        }


        vm.selectedTypeTender;
        vm.listTypeTender;
        function loadTypeTender() {
            VerifikasiDataService.getTypeTender(function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    vm.listTypeTender = reply.data.List;
                    for (var i = 0; i < vm.listTypeTender.length; i++) {
                            if (vm.listTypeTender[i].RefID === vm.dataExp.FieldType) {
                                vm.selectedTypeTender = vm.listTypeTender[i];
                                loadBusinessField();
                                break;
                            }
                        }
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }

        vm.listBussinesDetailField = []
        vm.changeTypeTender = changeTypeTender;
        function changeTypeTender() {
            //console.info(JSON.stringify(vm.selectedTypeVendor));
            if (vm.selectedTypeTender === undefined) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_TYPEVENDOR");
                return;
            }
            vm.dataExp.FieldType = vm.selectedTypeTender.RefID;
            loadBusinessField();
            vm.listBussinesDetailField = [];
        }

        vm.loadBusinessField = loadBusinessField;
        vm.selectedBusinessField;
        vm.listBusinessField = [];
        function loadBusinessField() {
            VerifikasiDataService.SelectBusinessField({
                GoodsOrService: vm.dataExp.FieldType
            },
            function (response) {
                if (response.status === 200) {
                    vm.listBusinessField = response.data;
                    for (var i = 0; i < vm.listBusinessField.length; i++) {
                        if (vm.listBusinessField[i].ID === vm.dataExp.Field) {
                            vm.selectedBusinessField = vm.listBusinessField[i];
                            break;
                        }
                    }
                }
                else {
                    UIControlService.msg_growl("error", "Gagal mendapatkan list bidang usaha");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "Gagal Akses API");
                return;
            });
        }

        vm.changeBusinessField = changeBusinessField;
        function changeBusinessField() {
            //console.info("field:" + JSON.stringify(vm.selectedBusinessField));
            vm.dataExp.Field = vm.selectedBusinessField.ID;
        }
        /*end field*/


        function generateFilterStrings(allowedTypes) {
            var filetypes = "";
            for (var i = 0; i < allowedTypes.length; i++) {
                filetypes += "." + allowedTypes[i].Name + ",";
            }
            return filetypes.substring(0, filetypes.length - 1);
        }

        function handleRequestError(response) {
            UIControlService.log(response);
            UIControlService.handleRequestError(response.data, response.status);
            UIControlService.unloadLoading();
        }

        vm.savedata = savedata;
        function savedata() {
            if (vm.flag !== 1) {
                if (!(vm.fileUpload === undefined)) {
                    uploadFile();
                } else {
                    saveprocess();
                }
            }
            }


        vm.openCalendar = openCalendar;
        function openCalendar(index) {
            vm.isCalendarOpened[index] = true;
        };

        function convertAllDateToString() { // TIMEZONE (-)
            if (vm.tenagaahli.BirthDate) {
                vm.tenagaahli.BirthDate = UIControlService.getStrDate(vm.tenagaahli.BirthDate);
            }
        };

        //Supaya muncul di date picker saat awal load
        function convertToDate() {
            if (vm.tenagaahli.BirthDate) {
                vm.tenagaahli.BirthDate = new Date(Date.parse(vm.tenagaahli.BirthDate));
            }
        }

        vm.batal = batal;
        function batal() {
            $uibModalInstance.dismiss('cancel');
        };

        vm.update = update;
        vm.vendor = {};
        function update() {
            if (vm.flag === 8) {
                convertAllDateToString();
                UIControlService.loadLoadingModal("Silahkan Tunggu");
                vm.addresses.AddressInfo = vm.address;
                vm.countrys.Name = vm.Nationality;
                vm.statuss.Value = vm.Status;
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
                    Email: '',
                    Statusperson: vm.statuss,
                    Expertise: vm.Expertise
                };
                VerifikasiDataService.updateExperts(vm.vendor, function (reply) {
                    UIControlService.unloadLoadingModal();
                    if (reply.status === 200) {
                        UIControlService.msg_growl("success", "Data Tenaga Ahli Berhasil di update");
                        $uibModalInstance.close();
                    }
                    else {
                        UIControlService.msg_growl("error", "Gagal menonaktifkan data ");
                        return;
                    }
                }, function (err) {
                    UIControlService.msg_growl("error", "Data Tenaga Ahli gagal di update");
                    UIControlService.unloadLoadingModal();
                });
            }
            else if (vm.flag === 7 || vm.flag === 9) {
                if (!(vm.fileUpload === undefined)) {
                    uploadFile();
                } else {
                    saveprocess();
                }
                
            }

        }
        vm.uploadFile = uploadFile;
        function uploadFile() {
            if (validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
                upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes, "");
            }
        }

        function upload(file, config, filters, callback) {
            var size = config.Size;
            var unit = config.SizeUnitName;
            if (unit == 'SIZE_UNIT_KB') {
                size *= 1024;
            }

            if (unit == 'SIZE_UNIT_MB') {
                size *= (1024 * 1024);
            }
            UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_FILE");
            if (vm.flag === 7) {
                UploaderService.uploadCompanyPersonID(vm.data.VendorID, file, size, types,
                           function (reply) {
                               if (reply.status == 200) {
                                   UIControlService.unloadLoadingModal();
                                   vm.data.IDUrl = reply.data.Url;
                                   saveprocess();
                               } else {
                                   UIControlService.unloadLoadingModal();
                                   UIControlService.msg_growl("error", 'MESSAGE.ERR_UPLOAD');
                               }
                           }, function (err) {
                               UIControlService.unloadLoadingModal();
                               UIControlService.msg_growl("error", "MESSAGE.ERR_UPLOAD");
                           });
            }
            if (vm.flag === 9) {
                UploaderService.uploadSingleFile(file, "UPLOAD_DIRECTORIES_VENDORDATA", size, filters, dates,
                function (response) {
                    UIControlService.unloadLoading();
                    if (response.status == 200) {
                        var url = response.data.Url;
                        vm.dataExp.DocumentURL = url;
                        vm.pathFile = vm.folderFile + url;
                        UIControlService.msg_growl("success", "FORM.MSG_SUC_UPLOAD");
                        saveprocess();

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
        }

        function validateFileType(file, allowedFileTypes) {
            if (!file || file.length == 0) {
                UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
                return false;
            }

            var selectedFileType = file[0].type;
            selectedFileType = selectedFileType.substring(selectedFileType.lastIndexOf('/') + 1);
            //console.info("tipefile: " + selectedFileType);
            if (selectedFileType === "vnd.ms-excel") {
                selectedFileType = "xls";
            }
            else if (selectedFileType === "vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
                selectedFileType = "xlsx";
            }
            else {
                selectedFileType = selectedFileType;
            }
            //console.info("filenew:" + selectedFileType);
            //jika excel
            var allowed = false;
            for (var i = 0; i < allowedFileTypes.length; i++) {
                if (allowedFileTypes[i].Name == selectedFileType) {
                    allowed = true;
                    return allowed;
                }
            }

            if (!allowed) {
                UIControlService.msg_growl("warning", "MESSAGE.ERR_INVALID_FILETYPE");
                return false;
            }
        }


        vm.saveprocess = saveprocess;
        function saveprocess() {
            if (vm.flag == 7) {
                VerifikasiDataService.updateCompany(vm.data, function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        UIControlService.msg_growl("success", "FORM.MSG_SUC_SAVE");
                        $uibModalInstance.close();
                    }
                    else {
                        UIControlService.msg_growl("error", "FORM.MSG_ERR_SAVE");
                        return;
                    }
                }, function (err) {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                    UIControlService.unloadLoadingModal();
                });
            }
            else if (vm.flag == 9) {
                vm.dataExp.ID = vm.data.ID;
                vm.dataExp.VendorID = vm.data.VendorID;
                VerifikasiDataService.updateExperience(vm.dataExp, function (reply) {
                    UIControlService.unloadLoading();
                    if (reply.status === 200) {
                        UIControlService.msg_growl("success", "FORM.MSG_SUC_SAVE");
                        $uibModalInstance.close();
                    }
                    else {
                        UIControlService.msg_growl("error", "FORM.MSG_ERR_SAVE");
                        return;
                    }
                }, function (err) {
                    UIControlService.msg_growl("error", "MESSAGE.ERR_API");
                    UIControlService.unloadLoadingModal();
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
                Email: '',
                Statusperson: vm.statuss,
                Expertise: vm.Expertise
            };
        }

        vm.add = add;
        function add(data) {
        }
    }
})();