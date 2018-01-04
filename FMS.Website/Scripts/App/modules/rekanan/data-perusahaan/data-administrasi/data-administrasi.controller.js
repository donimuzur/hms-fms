(function () {
	'use strict';

	angular.module("app").controller("DataAdministrasiCtrl", ctrl);

	ctrl.$inject = ['$http', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'VerifiedSendService', 'DataAdministrasiService', 'UploaderService', 'UIControlService', 'GlobalConstantService', 'UploadFileConfigService', 'VendorRegistrationService'];
	/* @ngInject */
	function ctrl($http, $uibModal, $translate, $translatePartialLoader, $location, SocketService,
        VerifiedSendService, DataAdministrasiService, UploaderService, UIControlService, GlobalConstantService, UploadFileConfigService, VendorRegistrationService) {
		var vm = this;
		var flag;

		vm.vendorContactForm;
		vm.address1;
		vm.pageSize = 10;
		vm.folderFile = GlobalConstantService.getConstant('api') + "/";
		vm.bisaMengubahData;
		vm.dataAdmin = {};
		vm.arrProvince = [];
		vm.arrTown = [];
		vm.administrasi = [];
		vm.contact = [];
		vm.id_page_config = 1;
		vm.businessID;
		vm.administrasiDate = {};
		vm.isCalendarOpened = [false, false, false, false];
		vm.pathFile;
		vm.fileUpload;
		vm.size;
		vm.name;
		vm.type;
		vm.flag;
		vm.selectedForm;
		vm.selectedTypeVendor = {};
		vm.listCurrFalse = [];
		vm.listPersFalse = [];
		vm.address = {
			AddressID: 0,
			AddressInfo: "",
			PostalCode: "",
			StateID: 0,
			CityID: 0,
			DistrictID: 0
		};
		vm.flag = false;
		vm.addressFlag = 0;
		vm.addressAlterFlag = 0;
		vm.AddressAlterId = 0;
		vm.address2 = {
			AddressID: 0,
			AddressInfo: "",
			PostalCode: "",
			StateID: 0,
			CityID: 0,
			DistrictID: 0
		};
		vm.addresses = [];
		vm.contact1 = {
			ContactID: 0,
			Name: "",
			Website: "",
			Phone: "",
			MobilePhone: "",
			Email: "",
			email3: "",
			Address: {}
		};
		vm.selectedBE = null;
		vm.Contact = [];
		vm.contactpersonal = {};
		vm.isApprovedCRAdm = false;
		vm.isApprovedCRBF = false;
		vm.isCalendarOpened = [false, false, false, false];
		vm.IsEdit = false;
		vm.IsEditAlter = false;
		vm.initialize = initialize;
		function initialize() {
		    $translatePartialLoader.addPart('data-administrasi');
		    UIControlService.loadLoading("MESSAGE.LOADING");
		    //get tipe dan max.size file - 1
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
		    loadCurrency();
		    loadVerifiedVendor();
		    jLoad(1);
		    loadCountryAlternatif();
		    loadCheckCR();

		}

		vm.loadPhoneCodes = loadPhoneCodes;
		function loadPhoneCodes(data) {
		    UIControlService.loadLoading("Loading");
		    VendorRegistrationService.getCountries(
              function (response) {
                  vm.phoneCodeList = response.data;
                  for (var i = 0; i < vm.phoneCodeList.length; i++) {
                      if (vm.phoneCodeList[i].PhonePrefix === data) {
                          vm.phoneCode = vm.phoneCodeList[i];
                      }
                  }
                  UIControlService.unloadLoading();
              }, function (err) {
                  $.growl.error({ message: "Gagal Akses API >" + err });
                  UIControlService.unloadLoading();
              });
		}

		function loadCheckCR() {
			UIControlService.loadLoading("Silahkan Tunggu");
			DataAdministrasiService.getCRbyVendor({ CRName: 'OC_ADM_LEGAL' }, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
				    if (reply.data === true) {
				        vm.isApprovedCRAdm = true;
				    }
                    else{
					    if (reply.data === false) {
					        vm.isApprovedCRAdm = false;
					    }
				    }
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
			DataAdministrasiService.getCRbyVendor({ CRName: 'OC_VENDORBUSINESSFIELD' }, function (reply) {
			    UIControlService.unloadLoading();
			    if (reply.status === 200) {
			        if (reply.data === true) {
			            vm.isApprovedCRBF = true;
			        }
			        else {
			            if (reply.data === false) {
			                vm.isApprovedCRBF = false;
			            }
			        }
			    }
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}

		vm.loadVerifiedVendor = loadVerifiedVendor;
		function loadVerifiedVendor() {
			VerifiedSendService.selectVerifikasi(function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
				    vm.verified = reply.data;
				    vm.isVerified = vm.verified.Isverified;
				    loadCheckCR();
					vm.cekTemporary = vm.verified.IsTemporary;
					//console.info("verified"+JSON.stringify(vm.verified.Isverified));
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

		vm.jLoad = jLoad;
		function jLoad(current) {
			vm.currentPage = current;
			var offset = (current * 10) - 10;
			DataAdministrasiService.select({
				Offset: offset,
				Limit: vm.pageSize,
				Keyword: vm.Username
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.list = {};
					vm.administrasi = reply.data[0];
					vm.administrasiDate.StartDate = vm.administrasi.FoundedDate;
					vm.Username = vm.administrasi.user.Username;
					vm.PKPNumber = vm.administrasi.PKPNumber;
					convertToDate();
					loadTypeVendor(vm.administrasi);
					loadAssociation(vm.administrasi);
					loadVendorCommodity(vm.administrasi.VendorID);
					DataAdministrasiService.selectcontact({
						VendorID: vm.administrasi.VendorID
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
									vm.VendorContactTypeCompany = vm.contact[i].VendorContactType;
									if (vm.contact[i].Contact.Fax !== null) {
									    vm.fax = vm.contact[i].Contact.Fax;
									}
									console.info(vm.contact[i].Contact);
									vm.ContactID = vm.contact[i].Contact.ContactID;
									vm.phone = vm.contact[i].Contact.Phone.split(' ');
									vm.Phone = Number(vm.phone[1]);
									vm.phone = vm.phone[0].split(')');
									vm.phone = vm.phone[0].split('(');
									loadPhoneCodes(vm.phone[1]);
									vm.Email = vm.contact[i].Contact.Email;
									vm.Website = vm.contact[i].Contact.Website;
									vm.addressIdComp = vm.contact[i].Contact.AddressID;
									loadCountryAdmin(vm.contact[i].Contact.Address.State);
									vm.CityCompany = vm.contact[i].Contact.Address.City;
									vm.DistrictCompany = vm.contact[i].Contact.Address.Distric;
									vm.Region = vm.contact[i].Contact.Address.State.Country.Continent.Name;
									if (vm.contact[i].IsPrimary === 1 || vm.contact[i].IsPrimary === null) {
									    vm.CountryCode = vm.contact[i].Contact.Address.State.Country.Code;
									    loadCurrencies(vm.administrasi);
									    if (vm.CountryCode == 'IDN') {
							                loadBusinessEntity(vm.administrasi);
									    }
									}
									//console.info("isprim" + JSON.stringify(vm.contact[i].IsPrimary));
									vm.contactpersonal = vm.contact[i];
								} else if (vm.contact[i].VendorContactType.Name === 'VENDOR_CONTACT_TYPE_PERSONAL') {
								    vm.listPersonal.push(vm.contact[i]);
								    vm.VendorContactTypePers = vm.contact[i].VendorContactType;
								} else if (vm.contact[i].VendorContactType.Name === 'VENDOR_OFFICE_TYPE_MAIN' && vm.contact[i].IsPrimary === null) {
								    vm.addressFlag = 1;
								    vm.ContactOfficeId = vm.contact[i].Contact.ContactID;
									vm.ContactName = "Kantor Pusat";
									vm.Name = vm.contact[i].Contact.Name;
									vm.AddressId = vm.contact[i].Contact.AddressID;
									vm.VendorContactType = vm.contact[i].VendorContactType;

									vm.addressInfo = vm.contact[i].Contact.Address.AddressInfo.split(' ');
									for (var y = 0; y < (vm.addressInfo.length - 1) ; y++) {
										if (y == 0) vm.address1 = (vm.addressInfo[y] + ' ');
										else vm.address1 += (vm.addressInfo[y] + " ");
									}
									vm.cekAddress = vm.address1;
									vm.postcalcode = vm.addressInfo[vm.addressInfo.length - 1];
									vm.cekPostCode = vm.postcalcode;
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
										    if (y == 0) vm.address1 = vm.addressInfo[y] + ' ';
											else vm.address1 += (vm.addressInfo[y] + " ");
										}

										vm.cekAddress = vm.address1;
										vm.postcalcode = vm.addressInfo[vm.addressInfo.length - 1];
										vm.cekPostCode = vm.postcalcode;
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
									    if (y == 0) vm.addressinfo = vm.addressInfo[y] + ' ';
										else vm.addressinfo += (vm.addressInfo[y] + " ");
									}

									vm.cekAddress1 = vm.addressinfo;
									vm.PostalCodeAlternatif = vm.addressInfo[vm.addressInfo.length - 1];
									vm.cekPostCode1 = vm.PostalCodeAlternatif;
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
										    if (y == 0) vm.addressinfo = vm.addressInfo[y] + ' ';
											else vm.addressinfo += (vm.addressInfo[y] + " ");
										}

										vm.cekAddress1 = vm.addressinfo;
										vm.PostalCodeAlternatif = vm.addressInfo[vm.addressInfo.length - 1];
										vm.cekPostCode1 = vm.PostalCodeAlternatif;
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

		vm.CheckAddress = CheckAddress;
		function CheckAddress(flag) {
		    if (flag == true) {
		        if (vm.address1 !== vm.cekAddress) vm.IsEdit = true;
		    }
		    else {
		        if (vm.addressinfo !== vm.cekAddress1) vm.IsEditAlter = true;
		    }
		}
		vm.CheckPostcode = CheckPostcode;
		function CheckPostcode(flag) {
		    if (flag == true) {
		        if (vm.postcalcode !== vm.cekPostCode) vm.IsEdit = true;
		    }
		    else {
		        if (vm.PostalCodeAlternatif !== vm.cekPostCode1) vm.IsEditAlter = true;
		    }
		}

	    /*isi combo jenis pemasok*/
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

		/*isi combo jenis Vendor*/
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
				                changeTypeVendor(vm.administrasi);
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
		    if (vm.selectedTypeVendor !== undefined)
		    {
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
			   		//console.info("bfield" + JSON.stringify(vm.listBusinessField));
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
			} else {
				vm.loadComodity();
			}
		}

		vm.editcontact = editcontact;
		function editcontact(data) {
		    var data = {
		        item: data
		    }
		    var modalInstance = $uibModal.open({
		        templateUrl: 'app/modules/rekanan/data-perusahaan/data-administrasi/DetailContact.html',
		        controller: 'DetailContactAdministrasiCtrl',
		        controllerAs: 'DetailContactAdministrasiCtrl',
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
                                ContactID: data.ContactID,
		                        Name: data.Name,
		                        Email: data.Email,
		                        Phone: data.Phone
		                    },
		                    IsActive: true,
                            IsEdit: true
		                }
		                vm.listPersonal.push(aish);
		            }
		            else {
		                vm.listPersonal.push(vm.listPers[i]);
		            }
		        }
		    });
		}

		function handleRequestError(response) {
		    UIControlService.log(response);
		    UIControlService.handleRequestError(response.data, response.status);
		    UIControlService.unloadLoading();
		}

		vm.CheckEmail = CheckEmail;
		function CheckEmail() {
		    if (vm.EmailPers !== '') {
		        var data = {
		            Keyword: vm.EmailPers
		        };
		        VendorRegistrationService.checkEmail(data,
                        function (response) {
                            vm.EmailAvailable = (response.data == false || response.data == 'false');
                            if (!vm.EmailAvailable) {
                                UIControlService.msg_growl('error', 'FORM.VALIDATION_ERRORS.EMAIL_AVAILABLE.MESSAGE', 'FORM.VALIDATION_ERRORS.EMAIL_AVAILABLE.TITLE');
                                vm.EmailPers = '';
                            }
                        }, handleRequestError);
		    }
        }

		
		vm.CheckUsername = CheckUsername;
		function CheckUsername() {
		    if (vm.Username !== vm.administrasi.user.Username) {
		        DataAdministrasiService.checkUsername({ Keyword: vm.Username },
                        function (response) {
                            vm.UsernameAvailable = (response.data == false || response.data == 'false');
                            if (!vm.UsernameAvailable) {
                                UIControlService.msg_growl('error', 'Username not valid');
                                vm.Username = vm.administrasi.user.Username;
                            }
                        }, handleRequestError);
		    }
		    if (vm.Username === '') {
		        vm.Username = vm.administrasi.user.Username;
		    }
		}
		vm.addCurrency = addCurrency;
		function addCurrency() {
		    vm.flagCurr = false;
		    console.info(vm.Currency);
		    for (var i = 0; i < vm.listCurrencies.length; i++) {
		        if (vm.Currency.CurrencyID == vm.listCurrencies[i].MstCurrency.CurrencyID && vm.listCurrencies[i].IsActive == true) { vm.flagCurr = true; }
		    }
		    if (vm.flagCurr == false) {
		        vm.listCurrencies.push({
		            ID: 0,
		            CurrencyID: vm.Currency.CurrencyID,
                    VendorID: vm.administrasi.VendorID,
                    MstCurrency: vm.Currency,
                    IsActive: true
		        });
		    }
		    else {
		        UIControlService.msg_growl('error', 'Username not valid');
		    }
		}


		vm.addContactPers = addContactPers;
		function addContactPers() {
		    vm.listPersonal.push({
                
		        Contact: {
                    ContactID: 0,
		            Name: vm.NamePers,
		            Phone: vm.PhonePers,
                    Email: vm.EmailPers
		        },
                IsActive: true
		    });
		}

		vm.loadCurrency = loadCurrency;
		function loadCurrency() {
		    console.info("ss");
		    UIControlService.loadLoading("LOADERS.LOADING_CURRENCY");
		    VendorRegistrationService.getCurrencies(
                function (response) {
                    vm.currencyList = response.data;
                    UIControlService.unloadLoading();
                },
                handleRequestError);
		}

		vm.loadComodity = loadComodity;
		vm.selectedComodity;
		vm.listComodity = [];
		function loadComodity() {
			//console.info("bidang usaha goodsorservice" + JSON.stringify(vm.selectedBusinessField.GoodsOrService));
			if (vm.selectedBusinessField.GoodsOrService === 3091) {
				UIControlService.msg_growl("success", "Tidak ada komoditas dalam bidang ini");
				vm.listComodity = [];
			} else {
				DataAdministrasiService.SelectComodity({ ID: vm.selectedBusinessField.ID },
				   function (response) {
				   	//console.info("xx"+JSON.stringify(response));
				   	if (response.status === 200 && response.data.length > 0) {
				   		vm.listComodity = response.data;
				   	} else if (response.status === 200 && response.data.length < 1) {
				   	    UIControlService.msg_growl("success", "Tidak ada komoditas dalam bidang ini");
				   	    vm.listComodity = [];
				   	} else {
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
            
			var countDetailBusinessField = vm.listBussinesDetailField.length;
			var addPermission = false; var sameItem = true;
			//console.info("countDetail" + JSON.stringify(countDetailBusinessField));
			var dataDetail = {
				VendorID: vm.administrasi.VendorID,
				CommodityID: comID,
				BusinessFieldID: vm.selectedBusinessField.ID,
				Commodity: vm.selectedComodity,
				BusinessField: vm.selectedBusinessField
			}
			if (countDetailBusinessField <= 6) {
			    var countGoodsDetail = 0; var countServiceDetail = 0;
			    for (var a = 0; a < vm.listBussinesDetailField.length; a++) {
			        if (countDetailBusinessField > 0) {
			            if (dataDetail.BusinessField.GoodsOrService === 3091) {
			                if (vm.listBussinesDetailField[a].BusinessField.GoodsOrService === 3091) {
			                    if (vm.listBussinesDetailField[a].BusinessFieldID !== dataDetail.BusinessFieldID) {
			                        countServiceDetail = +countServiceDetail + 1;
			                    }
			                    else {
			                        UIControlService.msg_growl("warning", "Bidang usaha jasa telah dipilih");
			                        sameItem = false;
			                    }
			                }
			            }
			            else if (dataDetail.BusinessField.GoodsOrService === 3090) {
			                if (vm.listBussinesDetailField[a].BusinessField.GoodsOrService === 3090) {
			                    if (vm.listBussinesDetailField[a].CommodityID !== dataDetail.CommodityID) {
			                        countGoodsDetail = +countGoodsDetail + 1;
			                    }
			                    else {
			                        UIControlService.msg_growl("warning", "Komoditas telah dipilih");
			                        sameItem = false;
			                    }
			                }
			            }
			        }
			    }
                //barang & jasa
			    if (vm.GoodsOrService === 3092) {
			        if (dataDetail.BusinessField.GoodsOrService===3090){
			            if (countGoodsDetail < 3) {
			                addPermission = true;
			            }
			            else {
			                UIControlService.msg_growl("warning", "Untuk tipe vendor barang dan jasa, maksimal bidang usaha barang adalah 3.");
			            }
			        }
                    else if (dataDetail.BusinessField.GoodsOrService===3091){
                        if (countServiceDetail<3){
                            addPermission = true;
                        }
                        else {
                            UIControlService.msg_growl("warning", "Untuk tipe vendor barang dan jasa, maksimal bidang usaha jasa adalah 3");
                        }
                    }

			        if (countDetailBusinessField === 6) {
			            UIControlService.msg_growl("warning", "Untuk tipe vendor barang dan jasa, maksimal total bidang usaha adalah 6");
			        }
			    }
			    //barang
			    else if (vm.GoodsOrService === 3090) {
			        if (dataDetail.BusinessField.GoodsOrService === 3090) {
			            if (countGoodsDetail < 3) {
			                addPermission = true;
			            }
			            else {
			                UIControlService.msg_growl("warning", "Untuk tipe vendor barang, maksimal bidang usaha barang adalah 3");
			            }
			        }
			    }

			    //jasa
			    else if (vm.GoodsOrService === 3091) {
			        if (dataDetail.BusinessField.GoodsOrService === 3091) {
			            if (countServiceDetail < 5) {
			                addPermission = true;
			            }
			            else {
			                UIControlService.msg_growl("warning", "Untuk tipe vendor jasa, maksimal bidang usaha jasa adalah 5");
			            }
			        }
			    }


			}
			if (addPermission === true && sameItem===true) {
			    vm.listBussinesDetailField.push(dataDetail);
			}

		}

		vm.deleteRow = deleteRow;
		function deleteRow(index) {
			var idx = index - 1;
			var _length = vm.listBussinesDetailField.length; // panjangSemula
			vm.listBussinesDetailField.splice(idx, 1);
		};
		vm.deleteRowCurr = deleteRowCurr;
		function deleteRowCurr(index, data) {
		    if (data.ID != undefined){
		        data.IsActive = false;
		        vm.listCurrFalse.push(data);
		    }
		    var idx = index;
		    var _length = vm.listCurrencies.length; // panjangSemula
		    vm.listCurrencies.splice(idx, 1);
		};
		vm.deleteRowPers = deleteRowPers;
		function deleteRowPers(index, data) {
		    console.info(data);
		    if (data.ContactID != 0){
		        data.IsActive = false;
		        vm.listPersFalse.push(data);
		    }
		    var idx = index;
		    var _length = vm.listPersonal.length; // panjangSemula
		    vm.listPersonal.splice(idx, 1);
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
				//loadTypeVendor();
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
						if (vm.administrasi.VendorTypeID === vm.listTypeVendor[i].VendorTypeID) {
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
			DataAdministrasiService.SelectBusinessEntity(function (response) {
				if (response.status === 200) {
					vm.listBusinessEntity = response.data;
					for (var i = 0; i < vm.listBusinessEntity.length; i++) {
						if (data1.business.BusinessID === vm.listBusinessEntity[i].BusinessID) {
							vm.selectedBusinessEntity = vm.listBusinessEntity[i];
							break;
						}
					}
				} else {
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
			DataAdministrasiService.getCurrencies(function (response) {
				if (response.status === 200) {
					vm.listCurrencies = response.data;

				} else {
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
					for (var i = 0; i < vm.listAssociation.length; i++) {
						if (data.AssociationID === vm.listAssociation[i].AssosiationID) {
							vm.selectedAssociation = vm.listAssociation[i];
							break;
						}
					}
				} else {
					UIControlService.msg_growl("error", "Gagal mendapatkan list jenis perusahaan");
					return;
				}
			}, function (err) {
				UIControlService.msg_growl("error", "Gagal Akses API");
				return;
			});
		}

		vm.openCalendar = openCalendar;
		function openCalendar(index) {
			vm.isCalendarOpened[index] = true;
		};

		function convertAllDateToString() { // TIMEZONE (-)
			if (vm.administrasiDate.StartDate) {
				vm.administrasiDate.StartDate = UIControlService.getStrDate(vm.administrasiDate.StartDate);
			}
		};

		//Supaya muncul di date picker saat awal load
		function convertToDate() {
			if (vm.administrasiDate.StartDate) {
				vm.administrasiDate.StartDate = new Date(Date.parse(vm.administrasiDate.StartDate));
			}
		}

		function generateFilterStrings(allowedTypes) {
			var filetypes = "";
			for (var i = 0; i < allowedTypes.length; i++) {
				filetypes += "." + allowedTypes[i].Name + ",";
			}
			return filetypes.substring(0, filetypes.length - 1);
		}

		vm.loadRegion = loadRegion;
		vm.selectedRegion;
		vm.listRegion = [];
		function loadRegion(countryID) {
			DataAdministrasiService.SelectRegion({
				CountryID: countryID
			}, function (response) {
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
			DataAdministrasiService.SelectCountry(function (response) {
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

			DataAdministrasiService.SelectState(data.CountryID, function (response) {
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
			DataAdministrasiService.SelectCity(data.StateID, function (response) {
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
			DataAdministrasiService.SelectDistrict(city.CityID, function (response) {
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
			DataAdministrasiService.SelectRegion({ CountryID: countryID }, function (response) {
				vm.listRegionAlternatif = response.data;
			}, function (err) {
				UIControlService.msg_growl("error", "Gagal Akses API");
				return;
			});
		}

		vm.loadRegionAdmin = loadRegionAdmin;
		vm.selectedRegionAdmin;
		vm.listRegionAdmin = [];
		function loadRegionAdmin(countryID) {
		    console.info(countryID);
		    DataAdministrasiService.SelectRegion({ CountryID: countryID }, function (response) {
		        vm.listRegionAdmin = response.data;
		        console.info(vm.listRegionAdmin);
		    }, function (err) {
		        UIControlService.msg_growl("error", "Gagal Akses API");
		        return;
		    });
		}

		vm.loadCountryAlternatif = loadCountryAlternatif;
		vm.selectedCountryAlternatif;
		vm.listCountryAlternatif = [];
		function loadCountryAlternatif(data) {
			DataAdministrasiService.SelectCountry(function (response) {
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

		vm.loadCountryAdmin = loadCountryAdmin;
		vm.selectedCountryAdmin;
		vm.listCountry = [];
		function loadCountryAdmin(data) {
		    DataAdministrasiService.SelectCountry(function (response) {
		        vm.listCountryAdmin = response.data;
		        for (var i = 0; i < vm.listCountryAdmin.length; i++) {
		            if (data !== undefined) {
		                if (data.CountryID === vm.listCountryAdmin[i].CountryID) {
		                    vm.selectedCountryAdmin = vm.listCountryAdmin[i];
		                    loadStateAdmin(data);
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

			DataAdministrasiService.SelectState(data.CountryID, function (response) {
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

		vm.loadStateAdmin = loadStateAdmin;
		vm.selectedStateAdmin;
		vm.listStateAdmin = [];
		function loadStateAdmin(data) {
		    if (!data) {
		        data = vm.selectedCountryAdmin;
		        vm.selectedStateAdmin = "";
		        vm.selectedCityAdmin = "";
		        vm.selectedDistrictAdmin = "";
		        vm.selectedStateAdmin1 = "";
		    }
		    loadRegionAdmin(data.CountryID);

		    DataAdministrasiService.SelectState(data.CountryID, function (response) {
		        vm.listStateAdmin = response.data;
		        for (var i = 0; i < vm.listStateAdmin.length; i++) {
		            if (data !== undefined) {
		                if (data.StateID === vm.listStateAdmin[i].StateID) {
		                    vm.selectedStateAdmin = vm.listStateAdmin[i];
		                    if (vm.selectedStateAdmin.Country.Code === 'IDN') {
		                        loadCityAdmin(vm.selectedStateAdmin);
		                        break;
		                    }
		                }
		            }
		        }


		    }, function (err) {
		        UIControlService.msg_growl("error", "Gagal Akses API");
		        return;
		    });
		}
		vm.loadCityAdmin = loadCityAdmin;
		vm.selectedCityAdmin;
		vm.listCityAdmin = [];
		function loadCityAdmin(data) {
		    if (!data) {

		        data = vm.selectedStateAdmin;
		        vm.selectedCityAdmin = "";
		        vm.selectedCityAdmin1 = "";
		        vm.selectedDistrictAdmin = "";
		    }
		    DataAdministrasiService.SelectCity(data.StateID, function (response) {
		        vm.listCityAdmin = response.data;
		        for (var i = 0; i < vm.listCityAdmin.length; i++) {
		            if (data !== undefined) {
		                if (vm.CityCompany.CityID === vm.listCityAdmin[i].CityID) {
		                    vm.selectedCityAdmin = vm.listCityAdmin[i];
		                    if (vm.selectedStateAdmin.Country.Code === 'IDN') {
		                        loadDistrictAdmin(vm.selectedCityAdmin);
		                        break;
		                    }
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
			DataAdministrasiService.SelectCity(data.StateID, function (response) {
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

		vm.loadDistrictAdmin = loadDistrictAdmin;
		vm.selectedDistrictAdmin;
		vm.listDistrictAdmin = [];
		function loadDistrictAdmin(city) {
		    if (!city) {
		        city = vm.selectedCityAdmin;
		        vm.selectedDistrictAdmin = "";
		        vm.selectedDistrictAdmin1 = "";

		    }
		    DataAdministrasiService.SelectDistrict(city.CityID, function (response) {
		        vm.listDistrictAdmin = response.data;
		        for (var i = 0; i < vm.listDistrictAdmin.length; i++) {
		            if (city !== undefined) {
		                if (vm.DistrictCompany.DistrictID === vm.listDistrictAdmin[i].DistrictID) {
		                    vm.selectedDistrictAdmin = vm.listDistrictAdmin[i];
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
			DataAdministrasiService.SelectDistrict(city.CityID, function (response) {
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


		vm.uploadFile = uploadFile;
		function uploadFile() {
			if (vm.fileUpload === undefined) {
				vm.PKPUrl = vm.administrasi.PKPUrl;
				savedata();
			} else {
			    //console.info("masuk");
			    if (validateFileType(vm.administrasi, vm.fileUpload, vm.idUploadConfigs)) {
			        //console.info("ok"+JSON.stringify(validateFileType(vm.administrasi, vm.fileUpload, vm.idUploadConfigs)));
					upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes, "");
				}
			}
		}

		vm.validateFileType = validateFileType;
		function validateFileType(administrasi, file, allowedFileTypes) {
			if (!file && administrasi.PKPUrl === "") {
				UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
				return false;
			}
			return true;
		}

		vm.upload = upload;
		function upload(file, config, filters, callback) {
		    //console.info("masuk"+JSON.stringify(vm.administrasi.PKPUrl));
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
		    //if (vm.administrasi.PKPUrl === null) {
				UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_FILE");
				UploaderService.uploadSingleFileLibrary(file, size, filters, function (response) {
					UIControlService.unloadLoading();
					if (response.status == 200) {
						var url = response.data.Url;
						vm.pathFile = url;
						vm.name = response.data.FileName;
						var s = response.data.FileLength;
						if (vm.flag == 0) {
							vm.size = Math.floor(s)
						}
						if (vm.flag == 1) {
							vm.size = Math.floor(s / (1024));
						}
						vm.PKPUrl = vm.pathFile;
						savedata();
					} else {
						UIControlService.msg_growl("error", "MESSAGE.ERR_UPLOAD");
						return;
					}
				}, function (response) {
					UIControlService.msg_growl("error", "MESSAGE.API")
					UIControlService.unloadLoading();
				});
			//} end if
		}

		function savedata() {
		    if (vm.selectedTypeVendor === undefined) {
		        UIControlService.msg_growl("warning", "Tipe Vendor Belum di pilih"); return;
		    }
		    else {
		        vm.VendorTypeID = vm.selectedTypeVendor.RefID;
		        if (vm.selectedTypeVendor.Name === "VENDOR_TYPE_SERVICE") {
		            vm.SupplierID = null;
		        }
		        else {
		            if (vm.selectedSupplier === undefined) {
		                UIControlService.msg_growl("warning", "Jenis Pemasok Belum di pilih"); return;
		            }
		            else vm.SupplierID = vm.selectedSupplier.RefID;
		        }
		    }
		    if (vm.listCurrencies.length === 0) {
		        UIControlService.msg_growl("warning", "Currency tidak boleh");
		        return;
		    }
		    else if (vm.listPersonal.length === 0) {
		        UIControlService.msg_growl("warning", "Kontak tidak boleh");
		        return;
		    }
		    //if (vm.listSupplier.length !== undefined) {
		    //    if (vm.selectedSupplier === undefined) {
		    //        UIControlService.msg_growl("warning", "Jenis Pemasok Belum di pilih"); return;
		    //    }
		        
		    //}
		    ////console.info(vm.addressinfo + ">>" + JSON.stringify(vm.selectedAssociation));
		    //else if (vm.selectedTypeVendor === undefined) {
		    //    UIControlService.msg_growl("warning", "Tipe Vendor Belum di pilih"); return;
		    //}
		    //else {
		    addtolist(vm.VendorTypeID, vm.SupplierID);
		 //   }
		}

		vm.addtolist = addtolist;
		vm.vendor = {};
		vm.listcontact = [];
		function addtolist(data1, data2) {
		    if (!vm.selectedCityAdmin && !vm.selectedDistrictAdmin) {
		        var addressComp = {
		            AddressID: vm.addressIdComp,
		            StateID: vm.selectedStateAdmin.StateID
		        }
		    }
		    else {
		        var addressComp = {
		            AddressID: vm.addressIdComp,
		            StateID: vm.selectedStateAdmin.StateID,
		            CityID: vm.selectedCityAdmin.CityID,
		            DistrictID: vm.selectedDistrictAdmin.DistrictID
		        }
		    }
			var contactdt = {
				VendorContactType: vm.VendorContactTypeCompany,
				Contact: {
					ContactID: vm.ContactID,
					Email: vm.Email,
					Phone: '(' + vm.phoneCode.PhonePrefix + ') ' + vm.Phone,
					Website: vm.Website,
					Fax: vm.fax,
					Address: addressComp
				}
			}
			vm.listcontact.push(contactdt);

			if (vm.selectedCity == undefined && vm.selectedDistrict == undefined) {
			    if (vm.postcalcode == undefined) var addressInfo = vm.address1;
			    else var addressInfo = vm.address1 + vm.postcalcode;
				vm.address = {
					AddressID: vm.AddressId,
					AddressInfo: addressInfo,
					PostalCode: vm.postcalcode,
					StateID: vm.selectedState.StateID
				}
			} else {
			    if (vm.postcalcode == undefined) var addressInfo = vm.address1;
			    else var addressInfo = vm.address1+  vm.postcalcode;

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
				Contact: contact,
                IsEdit: vm.IsEdit
			}
			vm.listcontact.push(contactdt);

			if (vm.selectedCountryAlternatif !== undefined) {
			    if (!vm.selectedCityAlternatif && !vm.selectedDistrictAlternatif) {

			        if (vm.PostalCodeAlternatif == undefined) vm.AddressInfo = vm.addressinfo;
			        else vm.AddressInfo = vm.addressinfo + ' ' + vm.PostalCodeAlternatif;
			        vm.address2 = {
			            AddressID: vm.AddressAlterId,
			            AddressInfo: vm.AddressInfo,
			            PostalCode: vm.PostalCodeAlternatif,
			            StateID: vm.selectedStateAlternatif ? vm.selectedStateAlternatif.StateID : null

			        }
			    } else {
			        if (vm.PostalCodeAlternatif == undefined) vm.AddressInfo = vm.addressinfo;
			        else vm.AddressInfo = vm.addressinfo +' '+ vm.PostalCodeAlternatif;

			        vm.address2 = {
			            AddressID: vm.AddressAlterId,
			            AddressInfo: vm.AddressInfo,
			            PostalCode: vm.PostalCodeAlternatif,
			            StateID: vm.selectedStateAlternatif ? vm.selectedStateAlternatif.StateID : null,
			            CityID: vm.selectedCityAlternatif.CityID,
			            DistrictID: vm.selectedDistrictAlternatif.DistrictID
			        }
			    }
			    if (vm.AddressAlterId == 0) {
			        var contact = {
			            Name: vm.Name,
			            ModifiedBy: vm.administrasi.user.Username,
			            Address: vm.address2
			        }
			    } else {
			        var contact = {
			            ContactID: vm.ContactOfficeAlterId,
			            ModifiedBy: vm.administrasi.user.Username,
			            Address: vm.address2
			        }
			    }
			    if (vm.VendorContactTypeAlter == undefined) {
			        var contactdt = {
			            VendorContactType: vm.VendorContactType,
			            Contact: contact,
			            IsPrimary: 2,
			            IsEdit: vm.IsEditAlter
			        }
			    } else {
			        var contactdt = {
			            VendorContactType: vm.VendorContactTypeAlter,
			            Contact: contact,
			            IsPrimary: 2,
                        IsEdit: vm.IsEditAlter
			        }
			    }

			    vm.listcontact.push(contactdt);
			    vm.listcontact.push(vm.contactpersonal);
			}
			for (var i = 0; i < vm.listPersonal.length; i++) {
			    var contactdt = {
			        VendorContactType: vm.VendorContactTypePers,
			        Contact: {
			            ContactID: vm.listPersonal[i].Contact.ContactID,
			            Email: vm.listPersonal[i].Contact.Email,
			            Phone: vm.listPersonal[i].Contact.Phone,
			            Name: vm.listPersonal[i].Contact.Name
			        },
			        IsActive: true,
                    IsEdit: vm.listPersonal[i].IsEdit
			    }
			    vm.listcontact.push(contactdt);
			}
			for (var i = 0; i < vm.listPersFalse.length; i++) {
			    var contactdt = {
			        VendorContactType: vm.VendorContactTypePers,
			        Contact: {
			            ContactID: vm.listPersFalse[i].Contact.ContactID,
			            Email: vm.listPersFalse[i].Contact.Email,
			            Phone: vm.listPersFalse[i].Contact.Phone,
			            Name: vm.listPersFalse[i].Contact.Name
			        },
			        IsActive: false
			    }
			    vm.listcontact.push(contactdt);
			}
			for (var i = 0; i < vm.listCurrFalse.length; i++) {
			    vm.listCurrencies.push(vm.listCurrFalse[i]);
			}

			var asoc;
			if (vm.selectedAssociation === undefined) {
				asoc = null;
			} else {
				asoc = vm.selectedAssociation.AssosiationID
			}
			
		    //console.info("coba" + JSON.stringify(vm.selectedSupplier));
			if (vm.selectedSupplier === null) {
			    vm.selectedSupplier = {
			        RefID: null
			    };
			}
			if (vm.CountryCode !== 'IDN') {
			    vm.selectedBusinessEntity = {
			        BusinessID: null
			    };
			}
			vm.vendor = {
			    SupplierID: data2,
			    VendorName: vm.administrasi.VendorName,
			    VendorID: vm.administrasi.VendorID,
			    user:{
                    Username: vm.Username
			    },
			    FoundedDate: UIControlService.getStrDate(vm.administrasiDate.StartDate),
				BusinessID: vm.selectedBusinessEntity.BusinessID,
				PKPNumber: vm.PKPNumber,
				PKPUrl: vm.PKPUrl,
				ModifiedBy: vm.administrasi.user.Username,
				AssociationID: asoc,
				Contacts: vm.listcontact,
				commodity: vm.listBussinesDetailField,
				VendorTypeID: data1,
				Currency: vm.listCurrencies

			}
			console.info(JSON.stringify(vm.vendor));
			DataAdministrasiService.insert(vm.vendor, function (reply) {
			    //console.info("reply" + JSON.stringify(reply))
			    UIControlService.unloadLoadingModal();
			    if (reply.status === 200) {
			        UIControlService.msg_growl("success", "Berhasil Simpan Data Administrasi !!");
			        window.location.reload();
			    } else {
			        UIControlService.msg_growl("error", "Gagal menyimpan data!!");
			        return;
			    }
			}, function (err) {
			    UIControlService.msg_growl("error", "Gagal Akses Api!!");
			    UIControlService.unloadLoadingModal();
			});

			

		}


	}
})();

