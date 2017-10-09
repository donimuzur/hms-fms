(function () {
	'use strict';

	angular.module("app").controller("FormIzinCtrl", ctrl);

	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', 'DataAdministrasiService','$location', 'SocketService',
		'UIControlService', 'item', '$uibModal', 'VerifikasiDataService', '$uibModalInstance', 'UploadFileConfigService', 'UploaderService',
		'AuthService', '$filter', 'ProvinsiService', 'GlobalConstantService'];

	function ctrl($http, $translate, $translatePartialLoader, DataAdministrasiService, $location, SocketService,
		 UIControlService, item, $uibModal, VerifikasiDataService, $uibModalInstance, UploadFileConfigService, UploaderService, AuthService,
            $filter, ProvinsiService, GlobalConstantService)
	    {
	    var vm = this;
	    vm.data = item.item;
	    vm.flag = item.flag;
	    vm.administrasiDate = {};
	    vm.folderFile = GlobalConstantService.getConstant('api') + "/";
	    const STOCK_OWNER_ID_PAGE_NAME = 'PAGE.VENDOR.REGISTRATION.ID';
		vm.isCalendarOpened = [false, false, false, false];
		vm.pathFile;
		vm.getIDstate;
		vm.VendorLogin;
		vm.folderFile = GlobalConstantService.getConstant('api') + "/";
		vm.mindate = new Date();
		vm.city = item.city;
		vm.init = init;
		function init() {
		    $translatePartialLoader.addPart('proses-verifikasi');
		    $translatePartialLoader.addPart('pengurus-perusahaan');
		    $translatePartialLoader.addPart('akta-pendirian');
		    $translatePartialLoader.addPart("data-izinusaha");
		    $translatePartialLoader.addPart('data-administrasi');
		    $translatePartialLoader.addPart('tenaga-ahli');
		    $translatePartialLoader.addPart('surat-pernyataan');
		    $translatePartialLoader.addPart('vendor-balance');
		    UIControlService.loadLoading("MESSAGE.LOADING");
		    
		    if (vm.flag === 0) {
		        loadConfigSPPKP();
		        VerifikasiDataService.selectcontact({
		            VendorID: item.item
		        }, function (reply) {
		            UIControlService.unloadLoading();
		            if (reply.status === 200) {
		                vm.listPersonal = [];
		                vm.contact = reply.data;
		                vm.flagPrimary = 0;
		                for (var i = 0; i < vm.contact.length; i++) {
		                    if (vm.contact[i].IsPrimary === 2) { vm.flagPrimary = 1; }
		                    if (i == (vm.contact.length - 1)) {
		                        if (vm.flagPrimary == 0) {
		                            loadCountryAlternatif();
		                        }
		                    }
		                }
		                for (var i = 0; i < vm.contact.length; i++) {
		                    if (vm.contact[i].VendorContactType.Name === 'VENDOR_CONTACT_TYPE_COMPANY') {
		                        vm.user = vm.contact[i].Vendor;
		                        vm.StartDate = vm.contact[i].Vendor.FoundedDate;
		                        vm.PKPNumber = vm.contact[i].Vendor.PKPNumber;
		                        loadTypeVendor(vm.contact[i].Vendor);
		                        loadAssociation(vm.contact[i].Vendor);
		                        loadVendorCommodity(vm.contact[i].Vendor.VendorID);
		                        vm.VendorContactTypeCompany = vm.contact[i].VendorContactType;
		                        vm.ContactID = vm.contact[i].Contact.ContactID;
		                        vm.Phone = vm.contact[i].Contact.Phone;
		                        vm.Email = vm.contact[i].Contact.Email;
		                        vm.Website = vm.contact[i].Contact.Website;
		                        vm.Fax = vm.contact[i].Contact.Fax;
		                        vm.User = vm.contact[i].Vendor.user.Username;
		                        vm.Region = vm.contact[i].Contact.Address.State.Country.Continent.Name;
		                        if (vm.contact[i].IsPrimary === 1 || vm.contact[i].IsPrimary === null) {
		                            vm.CountryCode = vm.contact[i].Contact.Address.State.Country.Code;
		                            loadCurrencies(vm.contact[i].Vendor);
		                            if (vm.CountryCode == 'IDN') {
		                                loadBusinessEntity(vm.contact[i].Vendor);
		                            }
		                        }
		                        //console.info("isprim" + JSON.stringify(vm.contact[i].IsPrimary));
		                        vm.contactpersonal = vm.contact[i];
		                    } else if (vm.contact[i].VendorContactType.Name === 'VENDOR_CONTACT_TYPE_PERSONAL') {
		                        vm.listPersonal.push(vm.contact[i]);
		                    } else if (vm.contact[i].VendorContactType.Name === 'VENDOR_OFFICE_TYPE_MAIN' && vm.contact[i].IsPrimary === null) {
		                        vm.addressFlag = 1;
		                        vm.ContactOfficeId = vm.contact[i].Contact.ContactID;
		                        vm.ContactName = "Kantor Pusat";
		                        vm.Name = vm.contact[i].Contact.Name;
		                        vm.AddressId = vm.contact[i].Contact.AddressID;
		                        vm.VendorContactType = vm.contact[i].VendorContactType;

		                        vm.addressInfo = vm.contact[i].Contact.Address.AddressInfo.split(' ');
		                        for (var y = 0; y < (vm.addressInfo.length - 1) ; y++) {
		                            if (y == 0) vm.address1 = vm.addressInfo[y]+ " ";
		                            else vm.address1 += (vm.addressInfo[y] + " ");
		                        }
		                        console.info(vm.addressInfo);
		                        vm.postcalcode = vm.addressInfo[vm.addressInfo.length - 1];
		                        loadCountry(vm.contact[i].Contact.Address.State);//loadRegion(vm.contact[i].Contact.Address.State.Country.CountryID);
		                        vm.selectedState1 = vm.contact[i].Contact.Address.State;
		                        if (vm.contact[i].Contact.Address.State.Country.Code === "IDN") {
		                            vm.selectedCity1 = vm.contact[i].Contact.Address.City;
		                            vm.selectedDistrict1 = vm.contact[i].Contact.Address.Distric;

		                        }
		                    } else if (vm.contact[i].VendorContactType.Name === 'VENDOR_OFFICE_TYPE_BRANCH' && vm.contact[i].IsPrimary === null) {
		                        if (vm.addressFlag == 0) {
		                            vm.AddressId = vm.contact[i].Contact.AddressID;
		                            vm.ContactOfficeId = vm.contact[i].Contact.ContactID;
		                            vm.VendorContactType = vm.contact[i].VendorContactType;
		                            vm.ContactName = "Kantor Cabang";
		                            vm.Name = vm.contact[i].Contact.Name;
		                            vm.addressInfo = vm.contact[i].Contact.Address.AddressInfo.split(' ');
		                            for (var y = 0; y < (vm.addressInfo.length - 1) ; y++) {
		                                if (y == 0) vm.address1 = vm.addressInfo[y] + " ";
		                                else vm.address1 += (vm.addressInfo[y] + " ");
		                            }

		                            vm.postcalcode = vm.addressInfo[vm.addressInfo.length - 1];
		                            loadCountry(vm.contact[i].Contact.Address.State);//loadRegion(vm.contact[i].Contact.Address.State.Country.CountryID);
		                            vm.selectedState1 = vm.contact[i].Contact.Address.State;
		                            if (vm.contact[i].Contact.Address.State.Country.Code === "IDN") {
		                                vm.selectedCity1 = vm.contact[i].Contact.Address.City;
		                                vm.selectedDistrict1 = vm.contact[i].Contact.Address.Distric;

		                            }
		                        }


		                    } else if (vm.contact[i].VendorContactType.Name === 'VENDOR_OFFICE_TYPE_MAIN' && vm.contact[i].IsPrimary === 2) {
		                        vm.addressAlterFlag = 1;
		                        vm.ContactOfficeAlterId = vm.contact[i].Contact.ContactID;
		                        vm.AddressAlterId = vm.contact[i].Contact.AddressID;
		                        vm.VendorContactTypeAlter = vm.contact[i].VendorContactType;
		                        vm.addressInfo = vm.contact[i].Contact.Address.AddressInfo.split(' ');
		                        for (var y = 0; y < (vm.addressInfo.length - 1) ; y++) {
		                            if (y == 0) vm.addressinfo = vm.addressInfo[y] + " ";
		                            else vm.addressinfo += (vm.addressInfo[y] + " ");
		                        }
		                        vm.PostalCodeAlternatif = vm.addressInfo[vm.addressInfo.length - 1];
		                        loadCountryAlternatif(vm.contact[i].Contact.Address.State);//loadRegion(vm.contact[i].Contact.Address.State.Country.CountryID);
		                        vm.selectedStateAlternatif1 = vm.contact[i].Contact.Address.State;
		                        if (vm.contact[i].Contact.Address.State.Country.Code === "IDN") {
		                            vm.selectedCityAlternatif1 = vm.contact[i].Contact.Address.City;
		                            vm.selectedDistrictAlternatif1 = vm.contact[i].Contact.Address.Distric;

		                        }

		                    } else if (vm.contact[i].VendorContactType.Name === 'VENDOR_OFFICE_TYPE_BRANCH' && vm.contact[i].IsPrimary === 2) {
		                        if (vm.addressAlterFlag == 0) {
		                            vm.ContactOfficeAlterId = vm.contact[i].Contact.ContactID;
		                            vm.AddressAlterId = vm.contact[i].Contact.AddressID;
		                            vm.VendorContactTypeAlter = vm.contact[i].VendorContactType;
		                            vm.addressInfo = vm.contact[i].Contact.Address.AddressInfo.split(' ');
		                            for (var y = 0; y < (vm.addressInfo.length - 1) ; y++) {
		                                if (y == 0) vm.addressinfo = vm.addressInfo[y] + " ";
		                                else vm.addressinfo += (vm.addressInfo[y] + " ");
		                            }
		                            vm.PostalCodeAlternatif = vm.addressInfo[vm.addressInfo.length - 1];
		                            loadCountryAlternatif(vm.contact[i].Contact.Address.State);//loadRegion(vm.contact[i].Contact.Address.State.Country.CountryID);
		                            vm.selectedStateAlternatif1 = vm.contact[i].Contact.Address.State;
		                            if (vm.contact[i].Contact.Address.State.Country.Code === "IDN") {
		                                vm.selectedCityAlternatif1 = vm.contact[i].Contact.Address.City;
		                                vm.selectedDistrictAlternatif1 = vm.contact[i].Contact.Address.Distric;

		                            }
		                        }
		                    }

		                }

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
		    else if (vm.flag === 1) {
		        loadKlasifikasi();
		        getTypeSizeFile();
		        if (item.isForm === true) {
		            vm.data.IssuedDate = new Date(Date.parse(vm.data.IssuedDate));
		            vm.data.ExpiredDate = new Date(Date.parse(vm.data.ExpiredDate));
		        } else {
		            vm.data.IssuedDate = UIControlService.getStrDate(vm.data.IssuedDate);
		            vm.data.ExpiredDate = UIControlService.getStrDate(vm.data.ExpiredDate);
		        }
		        if (vm.data.license == null) {

		            vm.licensiname = vm.data.LicenseName;
		        }
		        else {
		            vm.licensiname = vm.data.license.Name;
		        }
		        if (vm.data.IssuedLocation !== null) {
		            getCityByID(vm.data.IssuedLocation);
		        } else {
		            changeCountry(vm.city);
		        }

		    }
		    else if (vm.flag === 2) {
		        console.info(vm.data);
		        loadConfigStock();
		        loadStockUnits(vm.data);
		        vm.nameStock = vm.data.OwnerName;
		        vm.data.OwnerDOB = new Date(Date.parse(vm.data.OwnerDOB));
		        vm.OwnerId = vm.data.OwnerID;
		        vm.Quantity = vm.data.Quantity;
		    }
		    else if (vm.flag === 3) {
		        vm.data.DocumentDate = new Date(Date.parse(vm.data.DocumentDate));
		        loadDetailVendor();
		    }
		    else if (vm.flag === 4 || vm.flag === 5) {
		        vm.data.DocumentDate = new Date(Date.parse(vm.data.DocumentDate));
		        loadConfigAkta();
		    }
		    else if (vm.flag === 6) {
		        loadAsset();
		        loadUnit();
		        loadConfigNeraca();
		    }
		    else if (vm.flag === 8) {
		        //loadNational();
		        //loadAsset();
		        //loadUnit();
		        //loadConfigNeraca();
		        //vm.addresses = {
		        //    AddressInfo: ""
		        //};
		        //vm.countrys = {
		        //    Name: ""
		        //};
		        //vm.statuss = {
		        //    Name: ""
		        //};
		        //vm.radio = {
		        //    tipeM: "M",
		        //    tipeF: "F",
		        //    StatusK: "CONTRACT",
		        //    StatusI: "INTERNSHIP",
		        //    StatusP: "PERMANENT",
		        //}
		        //vm.nationalities = ["Indonesia"];
		        //vm.data.DateOfBirth = new Date(Date.parse(vm.data.DateOfBirth));
		       
            }
		    else if (vm.flag === 7) {
		        vm.data.DateOfBirth = new Date(Date.parse(vm.data.DateOfBirth));
		        vm.data.ServiceEndDate = new Date(Date.parse(vm.data.ServiceEndDate));
		        vm.dataServiceStartDate = new Date(Date.parse(vm.data.ServiceStartDate));
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
		    else if (vm.flag === 9 || vm.flag === 10 || vm.flag === 11) {
		        loadConfigExpertCertificate();
		        vm.data.StartDate = new Date(Date.parse(vm.data.StartDate));
		        vm.data.EndDate = new Date(Date.parse(vm.data.EndDate));
		        
		    }
		    else if (vm.flag === 16) {
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
		        vm.data.ValidDate = new Date(Date.parse(vm.data.ValidDate));
		    }
		        
		}

		vm.loadNational = loadNational;
		function loadNational() {
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
}
		vm.loadConfigSPPKP = loadConfigSPPKP;
		function loadConfigSPPKP() {
		    UploadFileConfigService.getByPageName("PAGE.VENDOR.ADMINISTRATION.PKP", function (response) {
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

		vm.loadConfigNeraca = loadConfigNeraca;
		function loadConfigNeraca() {
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

		vm.loadConfigExpertCertificate = loadConfigExpertCertificate;
		function loadConfigExpertCertificate() {
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
		}

		vm.selectedTypeVendor;
		vm.listTypeVendor;
		function loadTypeVendor(data) {
		    DataAdministrasiService.getTypeVendor(function (reply) {
		        UIControlService.unloadLoading();
		        if (reply.status === 200) {
		            vm.listTypeVendor = reply.data.List;
		            if (data !== undefined) {
		                for (var i = 0; i < vm.listTypeVendor.length; i++) {
		                    if (data.VendorTypeID === vm.listTypeVendor[i].RefID) {
		                        vm.selectedTypeVendor = vm.listTypeVendor[i];
		                        console.info("load");
		                        changeTypeVendor(data);
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

		vm.selectedSupplier;
		vm.listSupplier;
		function loadSupplier(data) {
		    DataAdministrasiService.getSupplier(function (reply) {
		        UIControlService.unloadLoading();
		        //console.info("PMS:"+JSON.stringify(reply));
		        if (reply.status === 200) {
		            vm.listSupplier = reply.data.List;
		            if (data) {
		                for (var i = 0; i < vm.listSupplier.length; i++) {
		                    if (data.SupplierID === vm.listSupplier[i].RefID) {
		                        vm.selectedSupplier = vm.listSupplier[i];
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


		vm.listBussinesDetailField = []
		vm.changeTypeVendor = changeTypeVendor;
		function changeTypeVendor(data) {
		    if (vm.selectedTypeVendor !== undefined) {
		        if (vm.selectedTypeVendor.Value === "VENDOR_TYPE_SERVICE") {
		            vm.disablePemasok = true;
		            vm.listSupplier = {};
		            UIControlService.msg_growl("warning", "Tidak ada jenis pemasok dalam pilihan ini");
		            //console.info("dpm:" + JSON.stringify(vm.disablePemasok));
		        }
		        if (vm.selectedTypeVendor.Value !== "VENDOR_TYPE_SERVICE") {
		            vm.disablePemasok = false;
		            if (data) {

		                loadSupplier(data);
		            }
		            else {

		                loadSupplier();
		            }
		            //console.info("dpm:" + JSON.stringify(vm.disablePemasok));
		        }

		        vm.GoodsOrService = vm.selectedTypeVendor.RefID;
		        loadBusinessField();
		        vm.listBussinesDetailField = [];
		        vm.listComodity = [];
		    }

		}

		vm.loadVendorCommodity = loadVendorCommodity;
		function loadVendorCommodity(data) {
		    DataAdministrasiService.SelectVendorCommodity({ VendorID: data }, function (reply) {
		        UIControlService.unloadLoading();
		        //console.info("PMS:"+JSON.stringify(reply));
		        if (reply.status === 200) {
		            vm.listBussinesDetailField = reply.data;
		        }
		    }, function (err) {
		        UIControlService.msg_growl("error", "MESSAGE.API");
		        UIControlService.unloadLoading();
		    });
		}

		vm.loadBusinessField = loadBusinessField;
		vm.selectedBusinessField;
		vm.listBusinessField = [];
		function loadBusinessField() {
		    DataAdministrasiService.SelectBusinessField({
		        GoodsOrService: vm.GoodsOrService
		    },
               function (response) {
                   if (response.status === 200) {
                       vm.listBusinessField = response.data;
                       //console.info("bfield" + JSON.stringify(vm.listBusinessField[0].GoodsOrService));
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

		vm.changeBussinesField = changeBussinesField;
		function changeBussinesField() {
		    //console.info("bfield" + JSON.stringify(vm.listBusinessField[0].GoodsOrService));
		    if (vm.selectedBusinessField === undefined) {
		        UIControlService.msg_growl("warning", "Jenis Bidang Usaha Belum di Pilih!");
		        return;
		    }
		    else {
		        vm.loadComodity();
		    }
		}


		vm.loadComodity = loadComodity;
		vm.selectedComodity;
		vm.listComodity = [];
		function loadComodity() {
		    //console.info("bidang usaha goodsorservice" + JSON.stringify(vm.selectedBusinessField.GoodsOrService));
		    if (vm.selectedBusinessField.GoodsOrService === 3091) {
		        UIControlService.msg_growl("success", "Tidak ada komoditas dalam bidang ini");
		        vm.listComodity = [];
		    }
		    else {
		        DataAdministrasiService.SelectComodity({ ID: vm.selectedBusinessField.ID },
                   function (response) {
                       //console.info("xx"+JSON.stringify(response));
                       if (response.status === 200 && response.data.length > 0) {
                           vm.listComodity = response.data;
                       }
                       else if (response.status === 200 && response.data.length < 1) {
                           UIControlService.msg_growl("success", "Tidak ada komoditas dalam bidang ini");
                       }
                       else {
                           UIControlService.msg_growl("error", "Gagal mendapatkan list komoditas");
                           return;
                       }
                   }, function (err) {
                       UIControlService.msg_growl("error", "Gagal Akses API");
                       return;
                   });
		    }
		}


		vm.addDetailBussinesField = addDetailBussinesField;
		function addDetailBussinesField() {
		    if (vm.selectedBusinessField === undefined) {
		        UIControlService.msg_growl("warning", "Bidang Usaha Belum di Pilih");
		        return;
		    }

		    var comID;
		    if (vm.selectedComodity === undefined) {
		        //UIControlService.msg_growl("warning", "Komoditas Belum di Pilih");
		        //return;
		        comID = null;
		    } else if (!(vm.selectedComodity === undefined)) {
		        comID = vm.selectedComodity.ID;
		    }

		    /*jika barang maka max. 3 komoditas*/
		    if (vm.selectedTypeVendor.RefID === 3090) {
		        for (var a = 1; a < vm.listBussinesDetailField.length; a++) {

		        }
		    }

		    var dataDetail = {
		        VendorID: item.item,
		        CommodityID: comID,
		        BusinessFieldID: vm.selectedBusinessField.ID,
		        Commodity: vm.selectedComodity,
		        BusinessField: vm.selectedBusinessField
		    }
		    //console.info(JSON.stringify(dataDetail));
		    vm.listBussinesDetailField.push(dataDetail);
		}

		vm.deleteRow = deleteRow;
		function deleteRow(index) {
		    var idx = index - 1;
		    var _length = vm.listBussinesDetailField.length; // panjangSemula
		    vm.listBussinesDetailField.splice(idx, 1);
		};

		vm.loadSelectedBusinessEntity = loadSelectedBusinessEntity;
		function loadSelectedBusinessEntity(selectedBE) {
		    vm.selectedBE = vm.selectedBusinessEntity.Description;
		    if (vm.selectedBE === "CV") {
		        for (var i = 0; i < vm.listTypeVendor.length; i++) {
		            if (vm.listTypeVendor[i].Value === "VENDOR_TYPE_GOODS") {
		                vm.listTV = vm.listTypeVendor[i];
		                i = vm.listTypeVendor.length;
		            }
		        }
		        vm.listTypeVendor = {};
		        vm.listTypeVendor[0] = vm.listTV;
		        changeTypeVendor();
		        //console.info("lteeee" + JSON.stringify(vm.listTypeVendor));
		    }
		    else if (vm.selectedBE !== "CV") {
		        loadTypeVendor();
		    }
		}

		function showSelectedTypeVendor(selectedBE) {
		    DataAdministrasiService.getTypeVendor(function (reply) {
		        UIControlService.unloadLoading();
		        if (reply.status === 200) {
		            vm.listTypeVendor = reply.data.List;
		            for (var i = 0; i < vm.listTypeVendor.length; i++) {
		                if (vm.user.VendorTypeID === vm.listTypeVendor[i].VendorTypeID) {
		                    vm.selectedTypeVendor = vm.listTypeVendor[i];
		                    changeTypeVendor();
		                    break;
		                }
		            }
		        }
		    }, function (err) {
		        UIControlService.msg_growl("error", "MESSAGE.API");
		        UIControlService.unloadLoading();
		    });
		}

		vm.loadBusinessEntity = loadBusinessEntity;
		vm.selectedBusinessEntity;
		vm.listBusinessEntity = [];
		function loadBusinessEntity(data1) {
		    DataAdministrasiService.SelectBusinessEntity(
               function (response) {
                   if (response.status === 200) {
                       vm.listBusinessEntity = response.data;
                       for (var i = 0; i < vm.listBusinessEntity.length; i++) {
                           if (data1.business.BusinessID === vm.listBusinessEntity[i].BusinessID) {
                               vm.selectedBusinessEntity = vm.listBusinessEntity[i];
                               break;
                           }
                       }
                   }
                   else {
                       UIControlService.msg_growl("error", "Gagal mendapatkan list jenis perusahaan");
                       return;
                   }
               }, function (err) {
                   UIControlService.msg_growl("error", "Gagal Akses API");
                   return;
               });
		}

		vm.loadCurrencies = loadCurrencies;
		vm.selectedCurrencies = [];
		vm.listCurrencies = [];
		var CurrencyID;
		function loadCurrencies(data) {
		    VerifikasiDataService.getCurrencies({VendorID: vm.data},
            function (response) {
                if (response.status === 200) {
                    vm.listCurrencies = response.data;
                    //console.info("selectedcurrencies:" + JSON.stringify(vm.selectedCurrencies));

                }
                else {
                    UIControlService.msg_growl("error", "Gagal mendapatkan list jenis perusahaan");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "Gagal Akses API");
                return;
            });
		}

		vm.loadAssociation = loadAssociation;
		vm.selectedAssociation;
		vm.listAssociation = [];
		function loadAssociation(data) {
		    DataAdministrasiService.getAssociation({
		        Offset: 0,
		        Limit: 0,
		        Keyword: ""
		    },
            function (response) {
                if (response.status === 200) {
                    vm.listAssociation = response.data.List;
                    console.info(JSON.stringify(data.AssociationID));
                    for (var i = 0; i < vm.listAssociation.length; i++) {
                        if (data.AssociationID === vm.listAssociation[i].AssosiationID) {
                            vm.selectedAssociation = vm.listAssociation[i];
                            break;
                        }
                    }
                }
                else {
                    UIControlService.msg_growl("error", "Gagal mendapatkan list jenis perusahaan");
                    return;
                }
            }, function (err) {
                UIControlService.msg_growl("error", "Gagal Akses API");
                return;
            });
		}

		vm.loadRegion = loadRegion;
		vm.selectedRegion;
		vm.listRegion = [];
		function loadRegion(countryID) {
		    DataAdministrasiService.SelectRegion({ CountryID: countryID },
           function (response) {
               vm.listRegion = response.data;
           }, function (err) {
               UIControlService.msg_growl("error", "Gagal Akses API");
               return;
           });
		}

		vm.loadCountry = loadCountry;
		vm.selectedCountry;
		vm.listCountry = [];
		function loadCountry(data) {
		    DataAdministrasiService.SelectCountry(
           function (response) {
               vm.listCountry = response.data;
               for (var i = 0; i < vm.listCountry.length; i++) {
                   if (data.CountryID === vm.listCountry[i].CountryID) {
                       vm.selectedCountry = vm.listCountry[i];
                       loadState(data);
                       break;
                   }
               }


           }, function (err) {
               UIControlService.msg_growl("error", "Gagal Akses API");
               return;
           });
		}

		vm.loadState = loadState;
		vm.selectedState;
		vm.listState = [];
		function loadState(data) {
		    if (!data) {
		        data = vm.selectedCountry;
		        vm.selectedState = "";
		        vm.selectedCity = "";
		        vm.selectedDistrict = "";
		        vm.selectedState1 = "";
		    }
		    loadRegion(data.CountryID);

		    DataAdministrasiService.SelectState(data.CountryID,
           function (response) {
               vm.listState = response.data;
               for (var i = 0; i < vm.listState.length; i++) {
                   if (vm.selectedState1 !== "" && vm.selectedState1.StateID === vm.listState[i].StateID) {
                       vm.selectedState = vm.listState[i];
                       if (vm.selectedState.Country.Code === 'IDN') {
                           loadCity(vm.selectedState);
                           break;
                       }
                   }
               }


           }, function (err) {
               UIControlService.msg_growl("error", "Gagal Akses API");
               return;
           });
		}

		vm.loadCity = loadCity;
		vm.selectedCity;
		vm.listCity = [];
		function loadCity(data) {
		    if (!data) {

		        data = vm.selectedState;
		        vm.selectedCity = "";
		        vm.selectedCity1 = "";
		        vm.selectedDistrict = "";
		    }
		    DataAdministrasiService.SelectCity(data.StateID,
           function (response) {
               vm.listCity = response.data;
               for (var i = 0; i < vm.listCity.length; i++) {
                   if (vm.selectedCity1 !== "" && vm.selectedCity1.CityID === vm.listCity[i].CityID) {
                       vm.selectedCity = vm.listCity[i];
                       if (vm.selectedState.Country.Code === 'IDN') {
                           loadDistrict(vm.selectedCity);
                           break;
                       }
                   }
               }
           }, function (err) {
               UIControlService.msg_growl("error", "Gagal Akses API");
               return;
           });
		}

		vm.loadDistrict = loadDistrict;
		vm.selectedDistrict;
		vm.listDistrict = [];
		function loadDistrict(city) {
		    if (!city) {
		        city = vm.selectedCity;
		        vm.selectedDistrict = "";
		        vm.selectedDistrict1 = "";

		    }
		    DataAdministrasiService.SelectDistrict(city.CityID,
           function (response) {
               vm.listDistrict = response.data;
               for (var i = 0; i < vm.listDistrict.length; i++) {
                   if (vm.selectedDistrict1 !== "" && vm.selectedDistrict1.DistrictID === vm.listDistrict[i].DistrictID) {
                       vm.selectedDistrict = vm.listDistrict[i];
                       break;
                   }
               }
           }, function (err) {
               UIControlService.msg_growl("error", "Gagal Akses API");
               return;
           });
		}

		vm.loadRegionAlternatif = loadRegionAlternatif;
		vm.selectedRegionAlternatif;
		vm.listRegionAlternatif = [];
		function loadRegionAlternatif(countryID) {
		    DataAdministrasiService.SelectRegion({ CountryID: countryID },
           function (response) {
               vm.listRegionAlternatif = response.data;
           }, function (err) {
               UIControlService.msg_growl("error", "Gagal Akses API");
               return;
           });
		}

		vm.loadCountryAlternatif = loadCountryAlternatif;
		vm.selectedCountryAlternatif;
		vm.listCountryAlternatif = [];
		function loadCountryAlternatif(data) {
		    DataAdministrasiService.SelectCountry(
           function (response) {
               vm.listCountryAlternatif = response.data;
               for (var i = 0; i < vm.listCountryAlternatif.length; i++) {
                   if (data !== undefined) {
                       if (data.CountryID === vm.listCountryAlternatif[i].CountryID) {
                           vm.selectedCountryAlternatif = vm.listCountryAlternatif[i];
                           loadStateAlternatif(data);
                           break;
                       }

                   }
               }


           }, function (err) {
               UIControlService.msg_growl("error", "Gagal Akses API");
               return;
           });
		}

		vm.loadStateAlternatif = loadStateAlternatif;
		vm.selectedStateAlternatif;
		vm.listStateAlternatif = [];
		function loadStateAlternatif(data) {
		    if (!data) {
		        data = vm.selectedCountryAlternatif;
		        vm.selectedStateAlternatif = "";
		        vm.selectedCityAlternatif = "";
		        vm.selectedDistrictAlternatif = "";
		        vm.selectedStateAlternatif1 = "";
		    }
		    loadRegionAlternatif(data.CountryID);

		    DataAdministrasiService.SelectState(data.CountryID,
           function (response) {
               vm.listStateAlternatif = response.data;
               for (var i = 0; i < vm.listStateAlternatif.length; i++) {
                   if (vm.selectedStateAlternatif1 !== "" && vm.selectedStateAlternatif1.StateID === vm.listStateAlternatif[i].StateID) {
                       vm.selectedStateAlternatif = vm.listStateAlternatif[i];
                       if (vm.selectedStateAlternatif.Country.Code === 'IDN') {
                           loadCityAlternatif(vm.selectedStateAlternatif);
                           break;
                       }
                   }
               }


           }, function (err) {
               UIControlService.msg_growl("error", "Gagal Akses API");
               return;
           });
		}

		vm.loadCityAlternatif = loadCityAlternatif;
		vm.selectedCityAlternatif;
		vm.listCityAlternatif = [];
		function loadCityAlternatif(data) {
		    if (!data) {

		        data = vm.selectedStateAlternatif;
		        vm.selectedCityAlternatif = "";
		        vm.selectedCityAlternatif1 = "";
		        vm.selectedDistrictAlternatif = "";
		    }
		    DataAdministrasiService.SelectCity(data.StateID,
           function (response) {
               vm.listCityAlternatif = response.data;
               for (var i = 0; i < vm.listCityAlternatif.length; i++) {
                   if (vm.selectedCityAlternatif1 !== "" && vm.selectedCityAlternatif1.CityID === vm.listCityAlternatif[i].CityID) {
                       vm.selectedCityAlternatif = vm.listCityAlternatif[i];
                       if (vm.selectedStateAlternatif.Country.Code === 'IDN') {
                           loadDistrictAlternatif(vm.selectedCityAlternatif);
                           break;
                       }
                   }
               }
           }, function (err) {
               UIControlService.msg_growl("error", "Gagal Akses API");
               return;
           });
		}

		vm.loadDistrictAlternatif = loadDistrictAlternatif;
		vm.selectedDistrictAlternatif;
		vm.listDistrictAlternatif = [];
		function loadDistrictAlternatif(city) {
		    if (!city) {
		        city = vm.selectedCityAlternatif;
		        vm.selectedDistrictAlternatif = "";
		        vm.selectedDistrictAlternatif1 = "";

		    }
		    DataAdministrasiService.SelectDistrict(city.CityID,
           function (response) {
               vm.listDistrictAlternatif = response.data;
               for (var i = 0; i < vm.listDistrictAlternatif.length; i++) {
                   if (vm.selectedDistrictAlternatif1 !== "" && vm.selectedDistrictAlternatif1.DistrictID === vm.listDistrictAlternatif[i].DistrictID) {
                       vm.selectedDistrictAlternatif = vm.listDistrictAlternatif[i];
                       break;
                   }
               }
           }, function (err) {
               UIControlService.msg_growl("error", "Gagal Akses API");
               return;
           });
		}

		vm.changeSubCOA = changeSubCOA;
		function changeSubCOA(param) {
		    VerifikasiDataService.getUnit(function (reply) {
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

		vm.selectedAsset;
		vm.listAsset;
		function loadAsset() {
		    VerifikasiDataService.getAsset(function (reply) {
		        UIControlService.unloadLoading();
		        if (reply.status === 200) {
		            vm.listAsset = reply.data.List;
		            for (var i = 0; i < vm.listAsset.length; i++) {
		                if (vm.data.Wealth.RefID === vm.listAsset[i].RefID) {
		                    vm.selectedAsset = vm.listAsset[i];
		                    loadCOA(vm.selectedAsset);
		                    break;
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
		    vm.param = "";
		    if (data.RefID === 3097) {
		        vm.param = "COA_TYPE_ASSET"
		    }
		    else if (data.RefID === 3099) {
		        vm.param = "COA_TYPE_DEBTH"
		    }

		    VerifikasiDataService.getCOA({
		        Keyword: vm.param
		    }, function (reply) {
		        UIControlService.unloadLoading();
		        if (reply.status === 200) {
		            vm.listCOA = reply.data.List;
		            for (var i = 0; i < vm.listCOA.length; i++) {
		                if (vm.data.COA.RefID === vm.listCOA[i].RefID) {
		                    vm.selectedCOA = vm.listCOA[i];
		                    loadSubCOA(vm.selectedCOA);
		                    break;
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
		    vm.param = "";
		    if (data.RefID === 3100) {
		        vm.param = "SUB_COA_CASH"
		    }
		    else if (data.RefID === 3101) {
		        vm.param = "SUB_COA_DEBTHSTOCK"
		    }
		    VerifikasiDataService.getSubCOA({
		        Keyword: vm.param
		    }, function (reply) {
		        UIControlService.unloadLoading();
		        if (reply.status === 200) {
		            vm.listSubCOA = reply.data.List;
		            var param = vm.listSubCOA.length;
		            if (param === 0) {
		                changeSubCOA(param);
		            }
		            for (var i = 0; i < vm.listSubCOA.length; i++) {
		                if (vm.data.SubCOA.RefID === vm.listSubCOA[i].RefID) {
		                    vm.selectedSubCOA = vm.listSubCOA[i];
		                    break;
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
		    VerifikasiDataService.getUnit(function (reply) {
		        UIControlService.unloadLoading();
		        if (reply.status === 200) {
		            vm.listUnit = reply.data.List;
		            for (var i = 0; i < vm.listUnit.length; i++) {
		                if (vm.data.Unit.RefID === vm.listUnit[i].RefID) {
		                    vm.selectedUnit = vm.listUnit[i];
		                    break;
		                }
		            }
		        }
		    }, function (err) {
		        UIControlService.msg_growl("error", "MESSAGE.API");
		        UIControlService.unloadLoading();
		    });
		}

		vm.loadDetailVendor = loadDetailVendor;
		function loadDetailVendor() {
		    VerifikasiDataService.GetDetailVendor({VendorID: vm.data.VendorID},function (reply) {
		        if (reply.status === 200) {
		            if (!(reply.data === null)) {
		                vm.vendorName = reply.data.Name;
		                vm.vendorID = reply.data.VendorID;
		                vm.businessID = reply.data.BusinessID;
		                vm.businessName = reply.data.BusinessName;
		                vm.vendorNpwp = reply.data.Npwp;
		                vm.vendorAddress = reply.data.AddressInfo + " , " + reply.data.AddressDetail;
		            }
		        } else {
		            UIControlService.unloadLoading();
		            UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_USER');
		        }
		    }, function (error) {
		        UIControlService.unloadLoading();
		        UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_USER');
		    });

		    VerifikasiDataService.GetCities(function (reply) {
		        if (reply.status === 200) {
		            vm.listKotaKab = reply.data;
		            for (var i = 0; i < vm.listKotaKab.length; i++) {
		                if (vm.data.NotaryLocation === vm.listKotaKab[i].CityID) {
		                    vm.selectedNotaryLocation = vm.listKotaKab[i];
		                }
		            }
		        } else {
		            UIControlService.msg_growl('error', 'MESSAGE.ERR_LOAD_CITIES');
		        }
		    }, function (err) {
		        UIControlService.msg_growl('error', 'MESSAGE.ERR_LOAD_CITIES');
		    });
		    loadConfigAkta();
		}

		vm.changeNotarisPlace = changeNotarisPlace;
		function changeNotarisPlace() {
		    vm.data.NotaryLocation = vm.selectedNotaryLocation.CityID;
		}
		vm.loadConfigAkta = loadConfigAkta;
		function loadConfigAkta(){
		    UploadFileConfigService.getByPageName("PAGE.VENDOR.LEGALDOCS", function (response) {
		        if (response.status == 200) {
		            vm.idUploadConfigs = response.data;
		            vm.idFileTypes = generateFilterStrings(response.data);
		            vm.idFileSize = vm.idUploadConfigs[0];
		        } else {
		            UIControlService.msg_growl("error", "MESSAGE.ERR_FILETYPE");
		        }
		    }, function (err) {
		        UIControlService.msg_growl("error", "MESSAGE.ERR_FILETYPE");
		    });
		}

		vm.loadConfigStock = loadConfigStock;
		function loadConfigStock() {
		    UIControlService.loadLoading();
		    UploadFileConfigService.getByPageName("PAGE.VENDOR.REGISTRATION.ID", function (response) {
		        if (response.status == 200) {
		            vm.idUploadConfigs = response.data;
		            vm.idFileTypes = generateFilterStrings(response.data);
		            vm.idFileSize = vm.idUploadConfigs[0];
		            VerifikasiDataService.GetCurrencies(
                        function (response) {
                            vm.currencyList = response.data;
                            VerifikasiDataService.getUploadPrefix(
                                function (response) {
                                    var prefixes = response.data;
                                    vm.prefixes = {};
                                    for (var i = 0; i < prefixes.length; i++) {
                                        vm.prefixes[prefixes[i].Name] = prefixes[i];
                                    }
                                    UIControlService.unloadLoadingModal();
                                }, function (error) {
                                    UIControlService.unloadLoadingModal();
                                    UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_PREFIXES')
                                }
                            );
                        },
                        function (error) {
                            UIControlService.unloadLoadingModal();
                            UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_CURRENCY')
                        }
                    );
		        } else {
		            UIControlService.unloadLoadingModal();
		            UIControlService.msg_growl("error", "MESSAGE.ERR_FILETYPE");
		        }
		    }, function (err) {
		        UIControlService.unloadLoadingModal();
		        UIControlService.msg_growl("error", "MESSAGE.ERR_FILETYPE");
		    });
		}

		vm.onUnitChange = onUnitChange;
		function onUnitChange() {
		    vm.showCurrencyField = false;
		    for (var i = 0; i < vm.stockUnits.length; i++) {
		        if (vm.stockUnits[i].RefID === vm.stock.UnitID) {
		            vm.showCurrencyField = vm.stockUnits[i].Name === 'STOCK_UNIT_CURRENCY';
		            break;
		        }
		    }
		}

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

		vm.loadStockUnits = loadStockUnits;
		function loadStockUnits(data) {
		    VerifikasiDataService.getStockTypes(
                function (response) {
                    vm.stockUnits = response.data;
                    if (data !== undefined) {
                        for (var i = 0; i < vm.stockUnits.length; i++) {
                            if (vm.stockUnits[i].RefID === data.Unit.RefID) {
                                vm.data.Unit = vm.stockUnits[i];
                            }

                        }

                    }
                    UIControlService.unloadLoading();
                },
                handleRequestError);
		}

        /*get username*/
		function getUsLogin() {
			AuthService.getUserLogin(function (reply) {
			    vm.VendorLogin = reply.data.CurrentUsername;
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
			});
		}

		vm.selectedIssuedDate = selectedIssuedDate;
		function selectedIssuedDate(sid) {
		    //console.info("selected issued date:" + JSON.stringify(sid));
		    vm.sid = sid;
		}

		vm.selectedExpiredDate = selectedExpiredDate;
		function selectedExpiredDate(sed) {
		    //console.info("selected expired date:" + JSON.stringify(sed));

		    if (sed > vm.sid) {
		    }
		    else if (sed < vm.sid) {
		        vm.data.ExpiredDate = moment();
		    }
		}

	    /*get city by id*/
		function getCityByID(id) {

		    ProvinsiService.getCityByID({ column: id }, function (reply) {
		        UIControlService.unloadLoading();
		        var data = reply.data.List[0];
                console.info(data);
		        vm.selectedCities = data;
		        vm.selectedState = data.State;
		        changeState(data.State.StateID);
		    }, function (err) {
		        UIControlService.msg_growl("error", "MESSAGE.API");
		        UIControlService.unloadLoading();
		    });
		}

		/*open form date*/
		vm.openCalendar = openCalendar;
		function openCalendar(index) {
			vm.isCalendarOpened[index] = true;
		};

		/*get combo klasifikasi*/
		vm.listClasification = [];
		vm.selectedClasification;
		function loadKlasifikasi() {
			//alert("load");
		    VerifikasiDataService.getClasification(function (reply) {
				UIControlService.unloadLoading();
				vm.listClasification = reply.data.List;
				if (!(vm.data.LicenseNo === null)) {
				    for (var i = 0; i < vm.listClasification.length; i++) {
				        if (vm.listClasification[i].RefID === vm.data.CompanyScale) {
				            vm.selectedClasification = vm.listClasification[i];
				            break;
				        }
				    }
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}
	    /* combo country, state, city */

		vm.changeCountry = changeCountry;
        vm.listState =[];
        vm.selectedState;
        function changeCountry(idstate) {
            console.info(idstate);
            ProvinsiService.getStates(idstate,
               function (response) {
                   vm.listState = response.data;
                   if (vm.data.IssuedLocation !== null) {
                       for (var i = 0; i < vm.listState.length; i++) {
                           if (vm.selectedState.StateID === vm.listState[i].StateID) {
                               vm.selectedState = vm.listState[i];
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
        vm.listCities =[];
        vm.selectedCities;
        function changeState() {
            ProvinsiService.getCities(vm.selectedState.StateID,
            function (response) {
            vm.listCities = response.data;
            if (!(vm.data.LicenseNo === null)) {
                for (var i = 0; i < vm.listCities.length; i++) {
                    if (vm.data.IssuedLocation === vm.listCities[i].CityID) {
                        vm.selectedCities = vm.listCities[i];
                        changeCountry(vm.selectedState.CountryID);
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
		    vm.data.IssuedLocation = vm.selectedCities.CityID;
        }
        /* end combo country, state, city*/

	    /*get type n size file upload*/
        vm.selectUpload = selectUpload;
        vm.fileUpload;
        function selectUpload() {
            console.info(">"+vm.fileUpload);
            //vm.fileUpload = vm.fileUpload;
        }

		function getTypeSizeFile() {
			UploadFileConfigService.getByPageName("PAGE.VENDOR.LICENSI", function (response) {
				UIControlService.unloadLoading();
				if (response.status == 200) {
					vm.idUploadConfigs = response.data;
					vm.idFileTypes = UIControlService.generateFilterStrings(response.data);
					vm.idFileSize = vm.idUploadConfigs[0];
					//console.info("file:" + JSON.stringify(response));

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

		//simpan
		vm.savedata = savedata;
		function savedata() {
		    if (vm.flag === 1) {
		        if (vm.selectedClasification === undefined && vm.licensiname === 'SIUP') {
		            UIControlService.msg_growl("warning", "MESSAGE.CMB_CLASIFICATION");
		            return;
		        }
		        if (vm.data.ExpiredDate === null) {
		            UIControlService.msg_growl("warning", "MESSAGE.EXPIRED_DATE");
		            return;
		        }

		        vm.datasimpan = vm.data;
		        var cscale;
		        if (vm.selectedClasification === undefined) {
		            cscale = null;
		        } else {
		            cscale = vm.selectedClasification.RefID;
		        }
		        vm.datasimpan.ExpiredDate = UIControlService.getStrDate(vm.datasimpan.ExpiredDate);
		        vm.datasimpan['CompanyScale'] = cscale;
		        if (!(vm.fileUpload === undefined)) {
		            uploadFile();
		        } else {
		            //console.info("1-"+JSON.stringify(vm.datasimpan));
		            saveprocess();
		        }

		    }
		        //vm.flag === 2 || vm.flag === 3 || vm.flag === 4 || vm.flag === 5 || vm.flag === 6
		    else if (vm.flag !== 1) {
		        if (!(vm.fileUpload === undefined)) {
		            uploadFile();
		        } else {
		            saveprocess();
		        }
		    }
		}

		/*proses upload file*/
		function uploadFile() {
		    AuthService.getUserLogin(function (reply) {
		        console.info(JSON.stringify(reply));
		        vm.VendorLogin = reply.data.CurrentUsername;
		        if (validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
		            upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes, vm.VendorLogin);
		        }
		    }, function (err) {
		        UIControlService.msg_growl("error", "MESSAGE.API");
		    });
			
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
			if (vm.flag === 0) {
			    UploaderService.uploadSingleFile(file, "UPLOAD_DIRECTORIES_VENDORDATA", size, filters, item.item,
                    function (response) {
                        console.info()
                        UIControlService.unloadLoading();
                        if (response.status == 200) {
                            var url = response.data.Url;
                            vm.PKPUrl = url;
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
			else if (vm.flag === 1) {
			    UploaderService.uploadSingleFile(file, "UPLOAD_DIRECTORIES_VENDORDATA", size, filters, vm.datasimpan.LicenseID,
                    function (response) {
                        console.info()
                        UIControlService.unloadLoading();
                        if (response.status == 200) {
                            var url = response.data.Url;
                            vm.datasimpan['DocumentURL'] = url;
                            vm.pathFile = url;
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
			else if (vm.flag === 2) {
			    UploaderService.uploadRegistration(file, vm.data.Vendor.Npwp, vm.prefixes['UPLOAD_PREFIX_ID'].Value, size, filters,

                    function (response) {
                        console.info()
                        UIControlService.unloadLoading();
                        if (response.status == 200) {
                            var url = response.data.Url;
                            vm.data.OwnerIDUrl = url;
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

			else if (vm.flag === 3 || vm.flag === 4 || vm.flag === 5) {
			    UploaderService.uploadSingleFileLegalDocuments(vm.data.VendorID, file, size, filters,
           function (reply) {
               if (reply.status == 200) {
                   UIControlService.unloadLoadingModal();
                       vm.data.DocumentURL = reply.data.Url;
                       vm.data.FileSize = reply.data.FileLength;
                       vm.data.FileName = reply.data.FileName;
                   UIControlService.msg_growl("success", "FORM.MSG_SUC_UPLOAD");
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

			else if (vm.flag === 7) {
			    UploaderService.uploadCompanyPersonID(vm.data.VendorID, file, size, types,
                           function (reply) {
                               if (reply.status == 200) {
                                   UIControlService.unloadLoadingModal();
                                   vm.data.IDUrl = reply.data.Url;
                               } else {
                                   UIControlService.unloadLoadingModal();
                                   UIControlService.msg_growl("error", 'MESSAGE.ERR_UPLOAD');
                               }
                           }, function (err) {
                               UIControlService.unloadLoadingModal();
                               UIControlService.msg_growl("error", "MESSAGE.ERR_UPLOAD");
                           });
			}

			else if (vm.flag === 6) {
			    UploaderService.uploadSingleFileBalance(vm.data.BalanceID, file, size, filters,
                function (response) {
                    UIControlService.unloadLoading();
                    if (response.status == 200) {
                        UIControlService.unloadLoadingModal();
                        vm.data.DocUrl = response.data.Url;
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

            else if (vm.flag === 9 || vm.flag === 10 || vm.flag === 11) {
			    UploaderService.uploadSingleFileCertificate(vm.data.ID, file, size, filters,
                function (response) {
                    UIControlService.unloadLoading();
                    if (response.status == 200) {
                        UIControlService.unloadLoadingModal();
                        vm.data.DocUrl = response.data.Url;
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
            else if (vm.flag == 16) {
                UploaderService.uploadSingleFileUploadDocument(file, size, filters,
               function (response) {
                   UIControlService.unloadLoading();
                   console.info("response:" + JSON.stringify(response));
                   if (response.status == 200) {
                       console.info(response);
                       var url = response.data.Url;
                       vm.pathFile = url;
                       vm.name = response.data.FileName;
                       var s = response.data.FileLength;
                       vm.data.DocUrl = vm.pathFile;
                       saveprocess();
                   } else {
                       UIControlService.msg_growl("error", "MESSAGE.ERR_UPLOAD");
                       return;
                   }
               },
               function (response) {
                   console.info(response);
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

		/* end proses upload*/
        vm.saveprocess = saveprocess;
        function saveprocess() {
            if (vm.flag === 0) {
                addtolist();
                VerifikasiDataService.insert(vm.vendor, function (reply) {
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
            else if (vm.flag === 1) {
                VerifikasiDataService.updateLicensi(vm.data, function (reply) {
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
            else if (vm.flag === 2) {
                vm.data.UnitID = vm.data.Unit.RefID;
                if (vm.stockUnitCurrency !== undefined)
                    vm.data.UnitCurrencyID = vm.stockUnitCurrency.CurrencyID;
                VerifikasiDataService.updateStock(vm.data, function (reply) {
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
            else if (vm.flag === 3 || vm.flag === 4 || vm.flag === 5) {
                VerifikasiDataService.updateLegal(vm.data, function (reply) {
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
            else if (vm.flag === 6) {
                VerifikasiDataService.updateBalance(vm.data, function (reply) {
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
            else if (vm.flag === 7) {
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
            else if (vm.flag === 8) {
                VerifikasiDataService.updateExperts(vm.data, function (reply) {
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
            else if (vm.flag === 9 || vm.flag === 10 || vm.flag === 11) {
                VerifikasiDataService.updateExpertCertificate(vm.data, function (reply) {
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
            else if (vm.flag === 16) {
                VerifikasiDataService.updateDoc(vm.data, function (reply) {
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

		vm.batal = batal;
		function batal() {
		    //$uibModalInstance.dismiss('cancel');
		    $uibModalInstance.close();
		};

		vm.addtolist = addtolist;
		vm.vendor = {};
		vm.listcontact = [];
		function addtolist() {
		    for (var i = 0; i < vm.listPersonal.length; i++) {
		        vm.listcontact.push(vm.listPersonal[i]);
		    }
		    var contactdt = {
		        VendorContactType: vm.VendorContactTypeCompany,
		        Contact: {
		            ContactID: vm.ContactID,
		            Email: vm.Email,
		            Phone: vm.Phone,
		            Website: vm.Website,
                    Fax: vm.Fax
		        }
		    }
		    vm.listcontact.push(contactdt);

		    if (vm.selectedCity == undefined && vm.selectedDistrict == undefined) {
		        if (vm.postcalcode == undefined) var addressInfo = vm.address1;
		        else var addressInfo = vm.address1 + " " + vm.postcalcode;

		        vm.address = {
		            AddressID: vm.AddressId,
		            AddressInfo: addressInfo,
		            PostalCode: vm.postcalcode,
		            StateID: vm.selectedState.StateID
		        }
		    } else {
		        if (vm.postcalcode == undefined) var addressInfo = vm.address1;
		        else var addressInfo = vm.address1 + " " + vm.postcalcode;

		        vm.address = {
		            AddressID: vm.AddressId,
		            AddressInfo: addressInfo,
		            PostalCode: vm.postcalcode,
		            StateID: vm.selectedState.StateID,
		            CityID: vm.selectedCity.CityID,
		            DistrictID: vm.selectedDistrict.DistrictID
		        }
		    }
		    var contact = {
		        ContactID: vm.ContactOfficeId,
		        Address: vm.address
		    }
		    var contactdt = {
		        VendorContactType: vm.VendorContactType,
		        Contact: contact
		    }
		    vm.listcontact.push(contactdt);

		    if (vm.selectedCountryAlternatif !== undefined) {
		        if (!vm.selectedCityAlternatif && !vm.selectedDistrictAlternatif) {
		            if (vm.PostalCodeAlternatif == undefined) var addressInfo = vm.addressinfo;
		            else var addressInfoAlter = vm.addressinfo + " " + vm.PostalCodeAlternatif;
		            vm.address2 = {
		                AddressID: vm.AddressAlterId,
		                AddressInfo: addressInfoAlter,
		                PostalCode: vm.PostalCodeAlternatif,
		                StateID: vm.selectedStateAlternatif ? vm.selectedStateAlternatif.StateID : null

		            }
		        } else {
		            if (vm.PostalCodeAlternatif == undefined) var addressInfo = vm.addressinfo;
		            else var addressInfoAlter = vm.addressinfo + " " + vm.PostalCodeAlternatif;

		            vm.address2 = {
		                AddressID: vm.AddressAlterId,
		                AddressInfo: addressInfoAlter,
		                PostalCode: vm.PostalCodeAlternatif,
		                StateID: vm.selectedStateAlternatif ? vm.selectedStateAlternatif.StateID : null,
		                CityID: vm.selectedCityAlternatif.CityID,
		                DistrictID: vm.selectedDistrictAlternatif.DistrictID
		            }
		        }
		        if (vm.AddressAlterId == 0) {
		            var contact = {
		                Name: vm.Name,
		                ModifiedBy: vm.user.user.Username,
		                Address: vm.address2
		            }
		        } else {
		            var contact = {
		                ContactID: vm.ContactOfficeAlterId,
		                ModifiedBy: vm.user.user.Username,
		                Address: vm.address2
		            }
		        }
		        if (vm.VendorContactTypeAlter == undefined) {
		            var contactdt = {
		                VendorContactType: vm.VendorContactType,
		                Contact: contact,
		                IsPrimary: 2
		            }
		        } else {
		            var contactdt = {
		                VendorContactType: vm.VendorContactTypeAlter,
		                Contact: contact,
		                IsPrimary: 2
		            }
		        }

		        vm.listcontact.push(contactdt);
		        vm.listcontact.push(vm.contactpersonal);
		    }

		    var asoc;
		    if (vm.selectedAssociation === undefined) {
		        asoc = null;
		    } else {
		        asoc = vm.selectedAssociation.AssosiationID
		    }

		    //console.info("coba" + JSON.stringify(vm.selectedSupplier));
		    
		    if (vm.CountryCode !== 'IDN') {
		        vm.selectedBusinessEntity = {
		            BusinessID: null
		        };
		    }
		    vm.vendor = {
		        
		        VendorID: item.item,
		        Npwp: vm.contact[0].Vendor.Npwp,
		        BusinessID: vm.selectedBusinessEntity.BusinessID,
		        PKPNumber: vm.PKPNumber,
		        PKPUrl: vm.PKPUrl,
		        ModifiedBy: vm.User,
		        FoundedDate: UIControlService.getStrDate(vm.StartDate),
		        vendorName: vm.contact[0].Vendor.VendorName,
		        AssociationID: asoc,
		        Contacts: vm.listcontact,
		        commodity: vm.listBussinesDetailField,
		        
		    }
		    console.info(vm.selectedSupplier);
		    if (vm.listSupplier) {
		        if (vm.selectedSupplier === undefined || vm.selectedSupplier === null) {
		            vm.vendor.SupplierID = null;
		        }

		        else vm.vendor.SupplierID = vm.selectedSupplier.RefID;
		    }
		    //console.info(vm.addressinfo + ">>" + JSON.stringify(vm.selectedAssociation));
		    if (vm.selectedTypeVendor === undefined) {
		        vm.vendor.VendorTypeID = null;
		    }
		    else vm.vendor.VendorTypeID= vm.selectedTypeVendor.RefID;


		}

		vm.editcontact = editcontact;
		function editcontact(data) {
		    var data = {
		        item: data
		    }
		    var modalInstance = $uibModal.open({
		        templateUrl: 'app/modules/panitia/data-rekanan/verifikasi-data/DetailContact.html',
		        controller: 'DetailContactCtrl',
		        controllerAs: 'DetailContactCtrl',
		        resolve: {
		            item: function () {
		                return data;
		            }
		        }
		    });
		    modalInstance.result.then(function (data) {
		        console.info(data);
		        vm.listPers = [];
		        vm.listPers = vm.listPersonal;
		        vm.listPersonal = [];
		        for (var i = 0; i < vm.listPers.length; i++) {
		            if (vm.listPers[i].ContactID === data.ContactID) {
		                var aish = {
		                    VendorContactType: vm.listPers[i].VendorContactType,
		                    ContactID: vm.listPers[i].ContactID,
		                    VendorID: vm.listPers[i].VendorID,
		                    Contact: {
		                        Name: data.Name,
		                        Email: data.Email,
		                        Phone: data.Phone
		                    }
		                }
		                vm.listPersonal.push(aish);
		            }
		            else {
		                vm.listPersonal.push(vm.listPers[i]);
		            }
		        }
		    });
		}

		vm.cekNpwp = cekNpwp;
		function cekNpwp(data) {
		    var datacek = {
		        Keyword: data,
		        column: item.item
		    }
		    VerifikasiDataService.cekNpwp(datacek, function (response) {
		        var data = response.data;
		        if (data.IsCheckedNpwp == true){
		            UIControlService.msg_growl('error', 'FORM.VALIDATION_OK.NPWP_NOT_AVAILABLE.MESSAGE', 'FORM.VALIDATION_OK.NPWP_NOT_AVAILABLE.TITLE');
		            UIControlService.unloadLoading();
		            vm.contact[0].Vendor.Npwp = null;

		        }
		    }, handleRequestError);
		}


	}
})();