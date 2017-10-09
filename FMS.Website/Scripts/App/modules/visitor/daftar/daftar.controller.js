(function () {
	'use strict';

	angular.module("app").controller("DaftarCtrl", ctrl);

	ctrl.$inject = ['$translatePartialLoader', '$state', '$translate', '$window', 'VendorRegistrationService', 'UIControlService', 'UploadFileConfigService', 'UploaderService', 'BlacklistService', 'MailerService', 'SocketService'];
	/* @ngInject */
	function ctrl($translatePartialLoader, $state, $translate, $window, VendorRegistrationService, UIControlService, UploadFileConfigService, UploaderService, BlacklistService, MailerService, SocketService) {
		// attributes
		var vm = this;
		var input;

		// register forms for autocomplete purpose only
		vm.regionalForm;
		vm.informationForm;
		vm.companyContact;
		vm.vendorContactForm;
		vm.vendorAddressForm;
		vm.stockForm;
		vm.tenderName = "";
		vm.contactPers = "";
		vm.flag = 0;
		// constants
		const MODULE_ID = 2;
		const STOCK_OWNER_ID_PAGE_NAME = 'PAGE.VENDOR.REGISTRATION.ID';
		const VENDOR_REGISTRATION_NO_PAGE_NAME = 'PAGE.VENDOR.REGISTRATION.NPWP';
		const VENDOR_LEGAL_NO_PAGE_NAME = 'PAGE.VENDOR.REGISTRATION.SIUP';
		const UPLOAD_PREFIX = 'UPLOAD_PREFIX';
		const UPLOAD_PREFIX_ID = 'UPLOAD_PREFIX_ID';
		const UPLOAD_PREFIX_NPWP = 'UPLOAD_PREFIX_NPWP';
		const UPLOAD_PREFIX_SIUP = 'UPLOAD_PREFIX_SIUP';
		const SIZE_UNIT_KB = 'SIZE_UNIT_KB';
		const SIZE_UNIT_MB = 'SIZE_UNIT_MB';
		const COUNTRY_INDONESIA = 'IDN';
		const STOCK_UNIT_CURRENCY = 'STOCK_UNIT_CURRENCY';
		const VENDOR_OFFICE_TYPE_MAIN = 'VENDOR_OFFICE_TYPE_MAIN';

		vm.passCaptcha = "";
		vm.checked = false;
		vm.enabledButton = true;
		vm.isCalendarOpened = false;
		vm.isDobCalendarOpened = false;
		vm.modulePath = "app/modules/visitor/daftar/";
		vm.formState = {
			regional: false
		};
		vm.vendor = {
			name: null,
			founded: null,
			currency: {},
			npwp: null,
			tendercodetemp: null,
			siup: null,
			contacts: [],
			addresses: [],
			stocks: []
		};

		vm.stock = {
			quantity: 0,
			unit: {},
			owner: {
				name: "",
				dob: "",
				id: "",
				doc_id: ""
			},
			Tmpblacklist: false
		};

		vm.address = {
			type: {},
			city: {},
			state: {},
			country: {}
		};

		vm.contact = {};

		vm.stocks = [];
		vm.contacts = [];
		vm.addresses = [];
		vm.formValid = false;
		vm.Tmpblacklist = false;

		// functions
		vm.initialize = initialize;
		vm.captcha = Captcha;
		vm.validCaptcha = validCaptcha;
		vm.loadRegions = loadRegions;
		vm.reloadRegions = reloadRegions;
		vm.loadCountries = loadCountries;
		vm.reloadCountries = reloadCountries;
		vm.loadStates = loadStates;
		vm.reloadStates = reloadStates;
		vm.loadCities = loadCities;
		vm.loadDistricts = loadDistricts;
		vm.loadBusiness = loadBusiness;
		vm.openCalendar = openCalendar;
		vm.addContact = addContact;
		vm.removeContact = removeContact;
		vm.openDobCalendar = openDobCalendar;
		vm.addStockData = addStockData;
		vm.removeStock = removeStock;
		vm.stockOwnerIDSelected = stockOwnerIDSelected;

		vm.npwpChanged = checkNpwp;
		vm.tenderChanged = checkTender;
		vm.vendorNameChanged = updateUsername;
		vm.npwpDocSelected = npwpDocSelected;
		vm.sktDocSelected = sktDocSelected;

		vm.reloadAddressCity = reloadAddressCity;
		vm.reloadAddressDistrict = reloadAddressDistrict;
		vm.addAddress = addAddress;
		vm.addCurrency = addCurrency;
		vm.removeAddress = removeAddress;
		vm.removeCurrency = removeCurrency;
		vm.checkLegal = checkLegal;
		vm.checkEmail = checkEmail;
		vm.setLang = setLang;
		vm.getCurrLang = getCurrLang;
		vm.getCurrCountry = getCurrCountry;
		vm.register = register;
		vm.cekOwnerID = cekOwnerID;

		// implementations
		function initialize() {
			$translatePartialLoader.addPart('daftar');
			Captcha();
			loadFilePrefix();
			loadCountries();
			reloadCountries();
			loadBusiness();
			loadCurrencies();
			loadStockUnits();
			loadConfig();
			loadPhoneCodes();
			loadOfficeType();
			loadLegal();
		}

		vm.calculateAge = calculateAge;
		function calculateAge(birthday) {
			if (birthday !== '') {
				var ageDifMs = Date.now() - birthday.getTime();
				var ageDate = new Date(ageDifMs); // miliseconds from epoch
				var age = Math.abs(ageDate.getUTCFullYear() - 1970);
				if (age < 17) {
				    vm.stock.owner.dob = undefined;
					UIControlService.msg_growl("error", "FORM.AGE");
					return;
				}
			}
		}

		function loadLegal(data) {
			UIControlService.loadLoading("LOADERS.LOADING");
			VendorRegistrationService.getLegal(
                function (response) {
                	vm.LegalList = response.data;
                	for (var i = 0; i < vm.LegalList.length; i++) {
                		if (data !== undefined) {
                			for (var j = 0; j < data.length; j++) {
                				if (data[j].LicenseID == vm.LegalList[i].ID) {
                					vm.vendor.legal = vm.LegalList[i];
                				}
                			}

                		}
                	}
                	UIControlService.unloadLoading();
                }, handleRequestError);
		}

		function checkLegal() {
			if (vm.vendor.legal === undefined) {
				UIControlService.msg_growl("error", "FORM.LEGAL");
				return;
			}
			else {
				UIControlService.loadLoading("LOADERS.LOADING");
				VendorRegistrationService.checkLegal({ Status: vm.vendor.legal.ID, Keyword: vm.vendor.siup },
                    function (response) {
                    	vm.legalAvailable = (response.data == false || response.data == 'false');
                    	if (vm.legalAvailable) {
                    		UIControlService.msg_growl('notice', 'FORM.VALIDATION_OK.LEGAL_AVAILABLE.MESSAGE', 'FORM.VALIDATION_OK.LEGAL_AVAILABLE.TITLE');
                    		UIControlService.unloadLoading();
                    	}
                    	else {
                    		UIControlService.msg_growl('error', 'FORM.VALIDATION_OK.LEGAL_NOT_AVAILABLE.MESSAGE', 'FORM.VALIDATION_OK.LEGAL_NOT_AVAILABLE.TITLE');
                    		UIControlService.unloadLoading();
                    		vm.vendor.legal = '';

                    	}
                    }, handleRequestError);
			}

		}

		function checkEmail(flag) {
			if (flag === true)
				var data = {
					Keyword: vm.vendor.email
				};
			else
				var data = {
					Keyword: vm.contact.email
				};
			VendorRegistrationService.checkEmail(data,
                function (response) {
                	vm.EmailAvailable = (response.data == false || response.data == 'false');
                	if (!vm.EmailAvailable) {
                		UIControlService.msg_growl('error', 'FORM.VALIDATION_ERRORS.EMAIL_AVAILABLE.MESSAGE', 'FORM.VALIDATION_ERRORS.EMAIL_AVAILABLE.TITLE');
                		if (flag == true) {
                			vm.vendor.email = '';
                		}
                		else {
                			vm.contact.email = '';
                		}
                	}
                }, handleRequestError);
		}

		function loadFilePrefix() {
			UIControlService.loadLoading("LOADERS.LOADING_PREFIX");
			VendorRegistrationService.getUploadPrefix(
                function (response) {
                	var prefixes = response.data;
                	vm.prefixes = {};
                	for (var i = 0; i < prefixes.length; i++) {
                		vm.prefixes[prefixes[i].Name] = prefixes[i];
                	}
                	console.log(vm.prefixes);
                	UIControlService.unloadLoading();
                }, handleRequestError);
		}

		function loadRegions(data) {
			UIControlService.loadLoading("LOADERS.LOADING_REGION");
			VendorRegistrationService.getRegions({ CountryID: data },
                function (response) {
                	vm.vendor.region = response.data;
                	UIControlService.unloadLoading();
                }, handleRequestError);
		}

		function reloadRegions(data) {
			UIControlService.loadLoading("LOADERS.LOADING_REGION");
			VendorRegistrationService.getRegions({ CountryID: data },
                function (response) {
                	vm.address.region = response.data;
                	UIControlService.unloadLoading();
                }, handleRequestError);
		}

		function loadCountries(data) {
			UIControlService.loadLoading("LOADERS.LOADING_COUNTRY");
			VendorRegistrationService.getCountries(
                function (response) {
                	vm.countryList = response.data;
                	vm.a = getCurrCountry();
                	vm.dataVen = getDataVendor();
                	if (vm.dataVen) loadVendor1(vm.dataVen);
                	for (var i = 0; i < vm.countryList.length; i++) {
                	    if (vm.a !== undefined) {
                			if (vm.a.CountryID === vm.countryList[i].CountryID) {
                				vm.vendor.country = vm.countryList[i];
                				UIControlService.loadLoading("LOADERS.LOADING_STATE");
                				loadRegions(vm.a.CountryID);
                				VendorRegistrationService.getStates(vm.vendor.country.CountryID,
									function (response) {
									    vm.stateList = response.data;
									    localStorage.removeItem('country');
									    vm.dataVenState = getDataVendorState();
									    vm.dataVenCity = getDataCity();
									    vm.dataVenDistric = getDataDistric();
									    for (var i = 0; i < vm.stateList.length; i++) {
									        if (vm.dataVenState !== "" && vm.dataVenState.StateID === vm.stateList[i].StateID) {
									            vm.vendor.state = vm.stateList[i];
									            localStorage.removeItem('state1');
									            localStorage.removeItem('city1');
									            localStorage.removeItem('distric1');
									            if (vm.vendor.state.Country.Code === 'IDN') {
									                loadCities(vm.vendor.state);
									                break;
									            }
									        }

									    }
										UIControlService.unloadLoading();
									}, handleRequestError);
                				break;
                			}
                		}
                	    if (data !== undefined) {
                	        console.info("sss");
                			if (data.CountryID === vm.countryList[i].CountryID) {
                				vm.vendor.country = vm.countryList[i];
                				UIControlService.loadLoading("LOADERS.LOADING_STATE");
                				loadRegions(vm.a.CountryID);
                				VendorRegistrationService.getStates(vm.vendor.country.CountryID,
									function (response) {
										vm.stateList = response.data;
										for (var i = 0; i < vm.stateList.length; i++) {
											if (vm.selectedState1 !== "" && vm.selectedState1.StateID === vm.stateList[i].StateID) {
												vm.vendor.state = vm.stateList[i];
												if (vm.vendor.state.Country.Code === 'IDN') {
													loadCities(vm.vendor.state);
													break;
												}
											}

										}
										UIControlService.unloadLoading();
									}, handleRequestError);
                				break;
                			}
                		}
                	}
                	UIControlService.unloadLoading();
                }, handleRequestError);
		}


		function reloadCountries() {
			UIControlService.loadLoading("LOADERS.LOADING_COUNTRY");
			VendorRegistrationService.getCountries(
                function (response) {
                	vm.addresscountryList = response.data;
                	UIControlService.unloadLoading();
                }, handleRequestError);
		}

		function loadStates(country) {
		    console.info(vm.dataVendor);
		    if (vm.dataVendor.length !== 0) {
		        localStorage.setItem('dataVendor', JSON.stringify(vm.dataVendor));
		        localStorage.setItem('state1', JSON.stringify(vm.selectedState1));
		        if (country.Code === 'IDN') {
		            localStorage.setItem('city1', JSON.stringify(vm.selectedCity1));
		            localStorage.setItem('distric1', JSON.stringify(vm.selectedDistrict1));
		        }
		    }

			if (!country) {
				country = vm.vendor.country;
				vm.vendor.state = "";
				vm.vendor.city = "";
				vm.vendor.district = "";
				vm.selectedState1 = "";
			}
			if (country.Code === 'IDN') {
			    loadRegions(country.CountryID); vm.dataVendor
				$translatePartialLoader.addPart('daftar');
				localStorage.setItem('currLang', 'ID');
				localStorage.setItem('country', JSON.stringify(country));
				$translate.preferredLanguage(getCurrLang());
				$window.location.reload();

			} else {
				loadRegions(country.CountryID);
				$translatePartialLoader.addPart('daftar');
				localStorage.setItem('currLang', 'EN');
				localStorage.setItem('country', JSON.stringify(country));
				$translate.preferredLanguage(getCurrLang());
				$window.location.reload();
			}
		}
		function getCurrCountry() {
			return JSON.parse(localStorage.getItem('country'));
		}
		
		function getDataVendor() {
		    return JSON.parse(localStorage.getItem('dataVendor'));
		}
		vm.getDataVendorState = getDataVendorState;
		function getDataVendorState() {
		    return JSON.parse(localStorage.getItem('state1'));
		}
		vm.getDataCity = getDataCity;
		function getDataCity() {
		    return JSON.parse(localStorage.getItem('city1'));
		}
		vm.getDataDistric = getDataDistric;
		function getDataDistric() {
		    return JSON.parse(localStorage.getItem('distric1'));
		}
		function reloadStates(country) {
			if (!country)
				country = vm.address.country;
			vm.address.state = "";
			vm.address.city = "";
			vm.address.district = "";
			reloadRegions(country.CountryID);
			UIControlService.loadLoading("LOADERS.LOADING_STATE");
			VendorRegistrationService.getStates(country.CountryID,
                function (response) {
                	vm.addressStateList = response.data;
                	UIControlService.unloadLoading();
                }, handleRequestError);
		}

		function loadCities(state) {
			if (!state) {
				state = vm.vendor.state;
				vm.vendor.city = "";
				vm.vendor.district = "";
				vm.selectedCity1 = "";
			}
			UIControlService.loadLoading("LOADERS.LOADING_CITY");
			VendorRegistrationService.getCities(state.StateID,
                function (response) {
                    vm.cityList = response.data;
                    if (vm.dataVenCity) {
                        for (var i = 0; i < vm.cityList.length; i++) {
                            if (vm.dataVenCity !== "" && vm.dataVenCity.CityID === vm.cityList[i].CityID) {
                                vm.vendor.city = vm.cityList[i];
                                if (state.Country.Code === 'IDN') {
                                    loadDistricts(vm.vendor.city);
                                    break;
                                }
                            }
                        }
                    }
                	UIControlService.unloadLoading();
                }, handleRequestError);
		}

		function loadDistricts(city) {
			if (!city) {
				city = vm.vendor.city;
				vm.vendor.district = "";
				vm.selectedDistrict1 = "";
			}
			UIControlService.loadLoading("LOADERS.LOADING_DISTRICT");
			VendorRegistrationService.getDistricts(city.CityID,
                function (response) {
                    vm.districtList = response.data;
                    if (vm.dataVenDistric) {
                        for (var i = 0; i < vm.districtList.length; i++) {
                            if (vm.dataVenDistric !== "" && vm.dataVenDistric.DistrictID === vm.districtList[i].DistrictID) {
                                vm.vendor.district = vm.districtList[i];
                                break;
                            }
                        }
                    }
                	UIControlService.unloadLoading();
                }, handleRequestError);
		}

		function loadPhoneCodes(data) {
			UIControlService.loadLoading("LOADERS.LOADING_PHONE");
			VendorRegistrationService.getCountries(
              function (response) {
                  vm.phoneCodeList = response.data;
                  if (data) {
                      for (var i = 0; i < vm.phoneCodeList.length; i++) {
                          if (vm.phoneCodeList[i].PhonePrefix === data) {
                              vm.phoneCode = vm.phoneCodeList[i];
                          }
                      }
                  }
                  
              	UIControlService.unloadLoading();
              }, handleRequestError);
		}

		function setLang(lang) {
			localStorage.setItem('currLang', lang);
			$translate.preferredLanguage(getCurrLang());
			//initialize();
		}

		function getCurrLang() {
			if (localStorage.getItem("currLang") === null || localStorage.getItem("currLang") === '')
				return '';
			return localStorage.getItem("currLang");

		}
		function loadBusiness(data) {
			UIControlService.loadLoading("LOADERS.LOADING_BUSINESS");
			VendorRegistrationService.getBusiness(
                function (response) {
                	vm.businessList = response.data;
                	for (var i = 0; i < vm.businessList.length; i++) {
                		if (data !== undefined && data === vm.businessList[i].BusinessID) {
                			vm.vendor.business = vm.businessList[i];
                			break;
                		}
                	}
                	UIControlService.unloadLoading();
                },
                handleRequestError);
		}

		function loadCurrencies() {
			UIControlService.loadLoading("LOADERS.LOADING_CURRENCY");
			VendorRegistrationService.getCurrencies(
                function (response) {
                	vm.currencyList = response.data;
                	UIControlService.unloadLoading();
                },
                handleRequestError);
		}

		function loadStockUnits(data) {
			UIControlService.loadLoading("LOADERS.LOADING_STOCK_TYPE");
			VendorRegistrationService.getStockTypes(
                function (response) {
                	vm.stockUnits = response.data;
                	if (data !== undefined) {
                		for (var i = 0; i < vm.stockUnits.length; i++) {
                			if (vm.stockUnits[i].RefID === data.Unit.RefID) {
                				vm.stockUnit = vm.stockUnits[i];
                				vm.flag = 1;
                			}

                		}

                	}

                	UIControlService.unloadLoading();
                },
                handleRequestError);
		}

		function loadOfficeType() {
			UIControlService.loadLoading("LOADERS.LOADING_OFFICE_TYPE");
			VendorRegistrationService.getOfficeType(
                function (response) {
                	if (response.status == 200) {
                		vm.officeTypes = response.data;
                	} else {
                		handleRequestError(response);
                	}
                },
                handleRequestError);
		}

		function reloadAddressCity(state) {
			if (!state)
				state = vm.address.state;
			vm.address.city = "";
			vm.address.district = "";
			UIControlService.loadLoading("LOADERS.LOADING_CITY");
			VendorRegistrationService.getCities(state.StateID,
                function (response) {
                	vm.addressCityList = response.data;
                	UIControlService.unloadLoading();
                }, handleRequestError);
		}

		function reloadAddressDistrict(city) {
			if (!city)
				city = vm.address.city;
			vm.address.district = "";
			UIControlService.loadLoading("LOADERS.LOADING_DISTRICT");
			VendorRegistrationService.getDistricts(city.CityID,
                function (response) {
                	vm.addressDistrictList = response.data;
                	UIControlService.unloadLoading();
                }, handleRequestError);
		}

		function handleRequestError(response) {
			UIControlService.log(response);
			UIControlService.handleRequestError(response.data, response.status);
			UIControlService.unloadLoading();
		}

		function openCalendar() {
			vm.isCalendarOpened = true;
		}

		function openDobCalendar() {
			vm.isDobCalendarOpened = true;
		}

		function loadConfig() {
			// load npwp filetype upload config

			UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_CONFIG");
			UploadFileConfigService.getByPageName(VENDOR_REGISTRATION_NO_PAGE_NAME, function (response) {
				if (response.status == 200) {
					vm.npwpUploadConfigs = response.data;
					vm.npwpFileTypes = generateFilterStrings(response.data);
					vm.npwpFileSize = vm.npwpUploadConfigs[0];
					UIControlService.unloadLoading();
				} else {
					handleRequestError(response);
				}
			},
            handleRequestError);

			// load siup filetype upload config

			UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_CONFIG");
			UploadFileConfigService.getByPageName(VENDOR_LEGAL_NO_PAGE_NAME, function (response) {
				if (response.status == 200) {
					vm.siupUploadConfigs = response.data;
					vm.siupFileTypes = generateFilterStrings(response.data);
					vm.siupFileSize = vm.siupUploadConfigs[0];
					UIControlService.unloadLoading();
				} else {
					handleRequestError(response);
				}
			},
            handleRequestError);

			// load id filetype upload config

			UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_CONFIG");
			UploadFileConfigService.getByPageName(STOCK_OWNER_ID_PAGE_NAME, function (response) {
				if (response.status == 200) {
					vm.idUploadConfigs = response.data;
					vm.idFileTypes = generateFilterStrings(response.data);
					vm.idFileSize = vm.idUploadConfigs[0];
					UIControlService.unloadLoading();
				} else {
					handleRequestError(response);
				}
			},
            handleRequestError);
		}

		function generateFilterStrings(allowedTypes) {
			var filetypes = "";
			for (var i = 0; i < allowedTypes.length; i++) {
				filetypes += "." + allowedTypes[i].Name + ",";
			}
			return filetypes.substring(0, filetypes.length - 1);
		}

		function validateFileType(file, allowedFileTypes) {
			if (!file || file.length == 0) {
				UIControlService.handleRequestError("ERRORS.INVALID_FILETYPE!");
				return false;
			}

			var selectedFileType = file[0].type;
			selectedFileType = selectedFileType.substring(selectedFileType.lastIndexOf('/') + 1);
			var allowed = false;
			for (var i = 0; i < allowedFileTypes.length; i++) {
				if (allowedFileTypes[i].Name == selectedFileType) {
					allowed = true;
					return allowed;
				}
			}

			if (!allowed) {
				UIControlService.handleRequestError("ERRORS.INVALID_FILETYPE!");
				return false;
			}
		}

		function stockOwnerIDSelected() {
			vm.stock.owner.id_doc = vm.stockOwnerID;
		}

		function npwpDocSelected() {
			vm.vendor.npwp_doc = vm.npwpDoc;
			//console.log(vm.npwpDoc);
		}

		function sktDocSelected() {
			vm.vendor.skt_doc = vm.sktDoc;
		}

		function upload(file, config, filters, callback) {
			//console.info(file);
			var size = config.Size;
			var unit = config.SizeUnitName;
			if (unit == SIZE_UNIT_KB) {
				size *= 1024;
			}

			if (unit == SIZE_UNIT_MB) {
				size *= (1024 * 1024);
			}

			UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_FILE");
			UploaderService.uploadRegistration(file, vm.vendor.npwp, config.Prefix, size, filters,
                function (response) {
                	if (response.status == 200) {
                		var url = response.data.Url;
                		UIControlService.unloadLoading();
                		callback(response.data);
                	} else {
                		UIControlService.handleRequestError("saham lebih");
                		UIControlService.unloadLoading();
                	}
                },
                function (response) {
                	UIControlService.handleRequestError("saham lebih");
                	UIControlService.unloadLoading();
                });

		}

		function addContact() {
			vm.contact.countryCode = vm.phoneCode;
			vm.contactPers = vm.contact.phone;
			checkTelp(false);
			if (!validateRequiredField(vm.contact.name)) return;
			//if (!validateRequiredField(vm.contact.phone)) return;
			//if (!validateRequiredField(vm.contact.countryCode)) return;

			vm.contacts.push(vm.contact);
			vm.contact = {};
			vm.vendorContactForm.$setPristine();
		}

		function removeContact(obj) {
			vm.contacts = remove(vm.contacts, obj);
		}

		function addStockData() {
			if (vm.stock.quantity <= 0) {
				UIControlService.msg_growl('error', "FORM.NOTNULL");
				return;
			}
			if (vm.stockUnit.Name === "STOCK_UNIT_PERCENTAGE") {
			    if (vm.stock.quantity > 100) {
			        UIControlService.msg_growl('error', "FORM.MAX_PERSEN");
			        return;
			    }
			    else {
			        var jumlah = 0;
			        for (var i = 0; i < vm.stocks.length; i++) {
			            jumlah += +vm.stocks[i].quantity;
			            if (i == (vm.stocks.length - 1)) {
			                if ((+vm.stock.quantity + +jumlah) > 100) {
			                    UIControlService.msg_growl('error', "FORM.MAX_PERSEN");
			                    return;
			                }

			            }
			}
			
        }
    }
			if (vm.stockForm.$invalid) return;

			cekOwnerID();

			VendorRegistrationService.isAnotherStockHolder({
				VendorID: 0,
				OwnerID: vm.stock.owner.id
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200 && reply.data === true) {
					UIControlService.msg_growl('error', "ERRORS.IS_ANOTHER_STOCKHOLDER");
				} else {
					if (validateFileType(vm.stockOwnerID, vm.idUploadConfigs)) {
						vm.idFileSize.Prefix = vm.prefixes.UPLOAD_PREFIX_ID.Value;
						upload(vm.stockOwnerID, vm.idFileSize, vm.idFileTypes, addStockToList);
					}
				}
			}, function (error) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl('error', "ERRORS.CANNOT_VERIFY_STOCKHOLDER");
				if (validateFileType(vm.stockOwnerID, vm.idUploadConfigs)) {
					vm.idFileSize.Prefix = vm.prefixes.UPLOAD_PREFIX_ID.Value;
					upload(vm.stockOwnerID, vm.idFileSize, vm.idFileTypes, addStockToList);
				}
			});
		}

		function addStockToList(obj) {
			vm.stock.owner.idDoc = obj.Url;
			vm.stock.owner.idFilename = obj.FileName;
			vm.stock.unit = vm.stockUnit;
			vm.stock.Currency = vm.stockUnitCurrency;
			if (vm.stockUnit.Value === 'STOCK_UNIT_CURRENCY')
			vm.stock.CurrencySymbol = vm.stockUnitCurrency.Symbol;
			vm.flag = 1;
			//vm.stock.currency = vm.stockUnitCurrency;
			vm.stock.Tmpblacklist = vm.Tmpblacklist;
			vm.stocks.push(vm.stock);
			vm.stock = {
				quantity: 0,
				unit: {},
				owner: {
					name: "",
					dob: "",
					id: "",
					doc_id: ""
				}
			};
			//vm.stockUnit = "";
			//vm.stockUnitCurrency = "";
			vm.stockForm.$setPristine();
		}

		function removeStock(obj) {
			vm.stocks = remove(vm.stocks, obj);
			if (vm.stocks.length === 0) {
				vm.stockUnit = {};
				vm.flag = 0;
			}
		}

		function remove(list, obj) {
			var index = list.indexOf(obj);
			if (index >= 0) {
				list.splice(index, 1);
			} else {
				UIControlService.msg_growl('error', "ERRORS.OBJECT_NOT_FOUND");
			}
			return list;
		}

		function addAddress() {
			var address = vm.address;

			if (!address.postalCode)
				address.postalCode = "";

			address.info = "";
			if (address.city) {
				address.info += " " + address.city.Name;
			}

			address.info += " " + address.state.Name + " " + address.state.Country.Name;

			if (address.type.Name == VENDOR_OFFICE_TYPE_MAIN) {
				if (hasMainOffice()) {
					UIControlService.msg_growl("error", "FORM.VALIDATION_ERRORS.MAIN_OFFICE_EXIST.MESSAGE", "FORM.VALIDATION_ERRORS.MAIN_OFFICE_EXIST.TITLE");
					return;
				}
			}
			vm.addresses.push(address);
			//console.info("alamats" + JSON.stringify(vm.addresses));
			vm.address = {};
			vm.vendorAddressForm.$setPristine();
		}

		vm.currencies = [];
		function addCurrency() {
			if (vm.vendor.currency.Symbol === undefined || vm.vendor.currency.Symbol === null) {
				UIControlService.msg_growl('error', "ERRORS.CURRENCY_NOT_FOUND");
				return;
			} else {
				var currency = vm.vendor.currency;
				vm.currencies.push(currency);
				vm.vendor.currency = {};
			}
		}

		function hasMainOffice() {
			for (var i = 0; i < vm.addresses.length; i++) {
				if (vm.addresses[i].type.Name == VENDOR_OFFICE_TYPE_MAIN) {
					return true;
				}
			}
			return false;
		}

		function removeAddress(address) {
			vm.addresses = remove(vm.addresses, address);
		}

		function removeCurrency(currency) {
			vm.currencies = remove(vm.currencies, currency);
		}

		function validateRequiredField(value) {
			if (!value) {
				UIControlService.msg_growl("error", "FORM.VALIDATION_ERRORS.REQUIRED.MESSAGE", "FORM.VALIDATION_ERRORS.REQUIRED.TITLE");
				UIControlService.unloadLoading();
				return false;
			}
			return true;
		}

		function checkNpwp() {
			console.log("npwp: " + vm.vendor.npwp);
			if (vm.vendor.npwp) {
				UIControlService.loadLoading("LOADERS.CHECKING_NPWP");
				VendorRegistrationService.checkNpwp(vm.vendor.npwp, function (response) {
					var data = response.data;
					if (data.IsCheckedNpwp == false) {
						UIControlService.msg_growl('notice', 'FORM.VALIDATION_OK.NPWP_AVAILABLE.MESSAGE', 'FORM.VALIDATION_OK.NPWP_AVAILABLE.TITLE');
						if (data.VendorID !== 0) {
							loadVendor(data.VendorID);
						}
						UIControlService.unloadLoading();
					} else {
						UIControlService.msg_growl('error', 'FORM.VALIDATION_OK.NPWP_NOT_AVAILABLE.MESSAGE', 'FORM.VALIDATION_OK.NPWP_NOT_AVAILABLE.TITLE');
						UIControlService.unloadLoading();
						vm.vendor.npwp = '';

					}
				}, handleRequestError);
			}
		}

		vm.checkCekCompany = checkCekCompany;
		function checkCekCompany() {
			if (vm.vendor.npwp) {
				UIControlService.loadLoading("LOADERS.CHECKING_NPWP");
				VendorRegistrationService.checkNpwp(vm.vendor.npwp, function (response) {
					var data = response.data;
					if (data.IsCheckedNpwp == false) {
						UIControlService.msg_growl('notice', 'FORM.VALIDATION_OK.NPWP_AVAILABLE.MESSAGE', 'FORM.VALIDATION_OK.NPWP_AVAILABLE.TITLE');
						UIControlService.unloadLoading();
					} else {
						UIControlService.msg_growl('error', 'FORM.VALIDATION_OK.NPWP_AVAILABLE.MESSAGE', 'FORM.VALIDATION_OK.NPWP_AVAILABLE.TITLE');
						UIControlService.unloadLoading();
						vm.vendor.npwp = '';

					}
				}, handleRequestError);
			}
		}

		function checkTender() {
			if (vm.vendor.tendercodetemp) {
				UIControlService.loadLoading("LOADERS.CHECKING_TENDER");
				VendorRegistrationService.checkTender(vm.vendor.tendercodetemp, function (response) {
					vm.tenderAvailable = response.data;
					if (vm.tenderAvailable == 2) {
						console.log(response);
						UIControlService.msg_growl('notice', 'FORM.TENDERVALID');
						UIControlService.unloadLoading();
						VendorRegistrationService.checkTenderName(vm.vendor.tendercodetemp, function (response) {
							vm.tenderName = response.data.TenderName;
						}, handleRequestError);
					} else if (vm.tenderAvailable == 1) {
						UIControlService.msg_growl('error', 'FORM.EXPIREDTENDER');
						UIControlService.unloadLoading();
						vm.vendor.tendercodetemp = '';
						vm.tenderName = "";
					} else if (vm.tenderAvailable == 0) {
						UIControlService.msg_growl('error', 'FORM.TENDERNOTVALID');
						UIControlService.unloadLoading();
						vm.vendor.tendercodetemp = '';
						vm.tenderName = "";
					}
				}, handleRequestError);
			}
		}

		function updateUsername() {
			vm.vendor.username = vm.vendor.name.replace(/ /gi, '_').toLowerCase();
		}

		function Captcha() {
			vm.passCaptcha = UIControlService.generateCaptcha('textCanvas', 'image');
		};

		function validCaptcha() {
			var valid = UIControlService.verifyCaptcha(vm.passCaptcha, 'txtInput');

			if (!valid) {
				Captcha();
			}
			return valid;
		};

		function uploadNpwp(callback, param) {
			var size = vm.npwpFileSize.Size;
			var unit = vm.npwpFileSize.SizeUnitName;
			if (unit == SIZE_UNIT_KB) {
				size *= 1024;
			}

			if (unit == SIZE_UNIT_MB) {
				size *= (1024 * 1024);
			}
			if (vm.vendor.npwp == vm.Npwp && vm.npwpDoc == vm.NpwpUrl) {
				vm.vendor.npwpUrl = vm.NpwpUrl;
				if (vm.vendor.country.Code == COUNTRY_INDONESIA) {
					uploadSiup();
				} else {
					commitUpdate();
				}
			} else {
				UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_FILE");
				UploaderService.uploadRegistration(vm.npwpDoc, vm.vendor.npwp, vm.prefixes.UPLOAD_PREFIX_NPWP.Value, size, vm.npwpFileTypes, function (response) {
					if (response.status == 200) {
						var url = response.data.Url;
						vm.vendor.npwpUrl = url;
						UIControlService.unloadLoading();
						if (vm.vendor.country.Code == COUNTRY_INDONESIA) {
							if (vm.vendor.siup == vm.Siup && vm.sktDoc == vm.siupUrl) {
								vm.vendor.siupUrl = vm.siupUrl;
								uploadSiup();
							}
							else callback.apply(this, param);
						} else {
							if (vm.vendor.npwp == vm.Npwp)
								commitUpdate();
							else
								commit();
						}
					} else {
						UIControlService.msg_growl('error', 'FORM.NOTCOMPLETENPWP');
						UIControlService.unloadLoading();
					}
				}, function (response) {
					UIControlService.msg_growl('error', 'FORM.NOTCOMPLETENPWP');
					UIControlService.unloadLoading();
				});
			}
		}

		function uploadSiup() {
			UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_FILE");
			var size = vm.siupFileSize.Size;
			var unit = vm.siupFileSize.SizeUnitName;
			if (unit == SIZE_UNIT_KB) {
				size *= 1024;
			}

			if (unit == SIZE_UNIT_MB) {
				size *= (1024 * 1024);
			}
			if (vm.vendor.siup == vm.Siup && vm.sktDoc == vm.siupUrl) {
				vm.vendor.siupUrl = vm.siupUrl;
				commitUpdate();
			} else {
				UploaderService.uploadRegistration(vm.sktDoc, vm.vendor.siup.toString(), vm.prefixes.UPLOAD_PREFIX_SIUP.Value, size, vm.siupFileTypes, function (response) {
					if (response.status == 200) {
						var url = response.data.Url;
						vm.vendor.siupUrl = url;
						UIControlService.unloadLoading();
						if (vm.vendor.npwp == vm.Npwp)
							commitUpdate();
						else
							commit();
					} else {
						UIControlService.handleRequestError("FORM.NOTCOMPLETESIUP");
						UIControlService.unloadLoading();
					}
				}, function (response) {
					UIControlService.handleRequestError("FORM.NOTCOMPLETESIUP");
					UIControlService.unloadLoading();
				});
			}
		}

		function commit() {
		    vm.vendor.phone = '(' + vm.phoneCode.PhonePrefix + ') ' + vm.vendor.phone;
			UIControlService.loadLoading("LOADERS.LOADING_REGISTER");
			VendorRegistrationService.register(vm.vendor,
			function (response) {
				UIControlService.unloadLoading();
				if (response.status == 200) {
					UIControlService.msg_growl("notice", "Registration complete!");
					SocketService.emit("daftarRekanan");
					localStorage.setItem('vendor_reg_id', response.data.VendorID);
					$state.go('daftar_kuesioner');
				} else {
					UIControlService.handleRequestError(response.data);
				}
			},
			function (response) {
				UIControlService.handleRequestError(response.data);
				UIControlService.unloadLoading();
			});
		}

		function commitUpdate() {

		    vm.vendor.phone = '(' + vm.phoneCode.PhonePrefix + ') ' + vm.vendor.phone;
			UIControlService.loadLoading("LOADERS.LOADING_REGISTER");
			VendorRegistrationService.registerUpdate(vm.vendor,
			function (response) {
				UIControlService.unloadLoading();
				if (response.status == 200) {
					UIControlService.msg_growl("notice", "Registration complete!");
					SocketService.emit("daftarRekanan");
					localStorage.setItem('vendor_reg_id', response.data.VendorID);
					$state.go('daftar_kuesioner');
				} else {
					UIControlService.handleRequestError(response.data);
				}
			},
			function (response) {
				UIControlService.handleRequestError(response.data);
				UIControlService.unloadLoading();
			});
		}

		function saveData() {
			vm.cek = 0;
			if (vm.vendor.country === undefined) {
				vm.cek = 1;
				UIControlService.msg_growl('error', 'ERRORS.COUNTRY');
				return;
			} else if (vm.vendor.state === '') {
				vm.cek = 1;
				UIControlService.msg_growl('error', 'ERRORS.STATE');
				return;
			} else if (vm.vendor.country.Code === "IDN") {
				if (vm.vendor.city === '') {
					vm.cek = 1;
					UIControlService.msg_growl('error', 'ERRORS.CITY');
					return;
				} else if (vm.vendor.district === '') {
					vm.cek = 1;
					UIControlService.msg_growl('error', 'ERRORS.DISTRICT');
					return;
				} else if (vm.vendor.business === undefined) {
					vm.cek = 1;
					UIControlService.msg_growl('error', 'ERRORS.BUSINESS');
					return;
				}
			}
			if (vm.vendor.name === null) {
				vm.cek = 1;
				UIControlService.msg_growl('error', 'ERRORS.NAME');
				return;
			} else if (vm.vendor.founded === null) {
				vm.cek = 1;
				UIControlService.msg_growl('error', 'ERRORS.FOUNDED');
				return;
			}
			else if (vm.vendor.npwp === null) {
				vm.cek = 1;
				UIControlService.msg_growl('error', 'ERRORS.NPWP');
				return;
			} else if (vm.npwpDoc === undefined) {
				vm.cek = 1;
				UIControlService.msg_growl('error', 'ERRORS.NPWPURL');
				return;
			} else if (vm.vendor.country.Code === "IDN") {
				if (vm.vendor.legal === undefined) {
					vm.cek = 1;
					UIControlService.msg_growl('error', 'ERRORS.LEGAL');
					return;
				} else if (vm.vendor.siup === null) {
					vm.cek = 1;
					UIControlService.msg_growl('error', 'ERRORS.LEGALNo');
					return;
				} else if (vm.sktDoc === undefined) {
					vm.cek = 1;
					UIControlService.msg_growl('error', 'ERRORS.SIUPURL');
					return;
				}
			}

			if (vm.currencies.length === 0) {
				vm.cek = 1;
				UIControlService.msg_growl('error', 'ERRORS.CURRENCY');
				return;
			} else if (vm.vendor.email === undefined) {
				vm.cek = 1;
				UIControlService.msg_growl('error', 'ERRORS.EMAIL');
				return;
			} else if (vm.vendor.password === undefined) {
				vm.cek = 1;
				UIControlService.msg_growl('error', 'ERRORS.PASSWORD');
				return;
			} else if (vm.vendor.confirmPassword === undefined) {
				vm.cek = 1;
				UIControlService.msg_growl('error', 'ERRORS.CONFIRMPASSWORD');
				return;
			} else if (vm.phoneCode === undefined) {
				vm.cek = 1;
				UIControlService.msg_growl('error', 'ERRORS.PHONECODE');
				return;
			} else if (vm.vendor.phone === undefined) {
				vm.cek = 1;
				UIControlService.msg_growl('error', 'ERRORS.PHONE');
				return;
			} else if (vm.contacts.length === 0) {
				vm.cek = 1;
				UIControlService.msg_growl('error', 'ERRORS.CONTACT');
				return;
			} else if (vm.addresses.length === 0) {
				vm.cek = 1;
				UIControlService.msg_growl('error', 'ERRORS.ADDRESS');
				return;
			} else if (vm.vendor.country.Code === "IDN") {
				if (vm.stocks.length === 0) {
					vm.cek = 1;
					UIControlService.msg_growl('error', 'ERRORS.STOCK');
					return;
				}
			}

			if (vm.cek === 0) {
				vm.countSize = 0;
				vm.vendor.addresses = vm.addresses;
				vm.vendor.contacts = vm.contacts;
				vm.vendor.stocks = vm.stocks;
				vm.vendor.currencies = vm.currencies;

				var passw = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20}$/;
				if (!vm.vendor.password.match(passw)) {
					UIControlService.msg_growl('error', 'ERRORS.MATCHPassword');
					return;
				} else if (vm.vendor.country.Code === 'IDN') {
					if (vm.vendor.business !== null) {
						if (vm.vendor.business.Name === "PT") {
							if (vm.stocks.length === 1) {
								UIControlService.msg_growl('error', 'ERRORS.STOCKHOLDER');
								return;
							}
						}
					}
					if (vm.stocks[0].unit.Name === "STOCK_UNIT_PERCENTAGE") {
						for (var i = 0; i < vm.stocks.length; i++) {
							vm.countSize += +vm.stocks[i].quantity;
							if (i === (vm.stocks.length - 1)) {
								if (vm.countSize < 100) {
									UIControlService.msg_growl('error', 'FORM.MIN_PERSEN');
									return;
								}
							}
						}
					}
				}
				uploadNpwp(uploadSiup);
			}

		}

		function register() {
			if (validCaptcha()) {
				saveData();
			}
		}

		

		function cekOwnerID() {
			BlacklistService.cek({
				Status: vm.stock.owner.id
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200 && reply.data.length > 0) {
					vm.Tmpblacklist = true;
				}
				else {
					vm.Tmpblacklist = false;

				}
			}, function (err) {

				UIControlService.msg_growl("error", "Gagal Akses API ");
				UIControlService.unloadLoading();
			});
		}

		vm.checkTelp = checkTelp;
		function checkTelp(flag) {
			if (flag == true) {
				if (vm.vendor.phone.length > 20) {
					UIControlService.msg_growl("error", "Panjang Telepon melebihi maksimal");
					return;
				}
			} else {
				if (vm.contactPers.length > 20) {
					vm.contactPers = "";
					UIControlService.msg_growl("error", "Panjang Telepon melebihi maksimal");
					return;
				}
			}
		}


		vm.loadVendor = loadVendor();
		function loadVendor(VendorID) {
			VendorRegistrationService.selectVendor({
				VendorID: VendorID
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
				    vm.contactVendor = reply.data;
				    vm.dataVendor = vm.contactVendor;
					vm.contacts = [];
					vm.addresses = [];
					loadStock(VendorID);
					loadLicense(VendorID);
					for (var i = 0; i < vm.contactVendor.length; i++) {
					    if (vm.contactVendor[i].VendorContactType.Name === 'VENDOR_CONTACT_TYPE_COMPANY') {
					        
					        vm.selectedState1 = vm.contactVendor[i].Contact.Address.State;
					        if (vm.contactVendor[i].Contact.Address.State.Country.Code === "IDN") {
					            vm.selectedCity1 = vm.contactVendor[i].Contact.Address.City;
					            vm.selectedDistrict1 = vm.contactVendor[i].Contact.Address.Distric;
					        }
					        loadStates(vm.contactVendor[i].Contact.Address.State.Country);
					    }
					}
				} else {
					$.growl.error({ message: "Gagal mendapatkan data Master Departemen" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				//$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}

		vm.loadVendor1 = loadVendor1();
		function loadVendor1(data) {
		    if (data != undefined) {
		        localStorage.removeItem('dataVendor');
		        vm.contactVendor = [];
		        vm.addresses = [];
		        loadStock(data[0].VendorID);
		        loadLicense(data[0].VendorID);
		        vm.contactVendor = data;
		        for (var i = 0; i < vm.contactVendor.length; i++) {
		            if ((vm.contactVendor[i].VendorContactType.Name === 'VENDOR_OFFICE_TYPE_MAIN' && vm.contactVendor[i].IsPrimary == null) || (vm.contactVendor[i].VendorContactType.Name === 'VENDOR_OFFICE_TYPE_BRANCH' && vm.contactVendor[i].IsPrimary == null)) {
		                vm.addressInfo = vm.contactVendor[i].Contact.Address.AddressInfo.split(' ');
		                for (var y = 0; y < (vm.addressInfo.length - 1) ; y++) {
		                    if (y == 0) vm.address1 = vm.addressInfo[y];
		                    else
		                        vm.address1 += (vm.addressInfo[y] + " ");
		                }
		                vm.postcalcode = vm.addressInfo[vm.addressInfo.length - 1];
		                vm.info = "";
		                if (vm.contactVendor[i].Contact.Address.City) {
		                    vm.info += " " + vm.contactVendor[i].Contact.Address.City.Name;
		                }
		                vm.info += " " + vm.contactVendor[i].Contact.Address.State.Name + " " + vm.contactVendor[i].Contact.Address.State.Country.Name;


		                var address = {
		                    ContactID: vm.contactVendor[i].Contact.ContactID,
		                    type: vm.contactVendor[i].VendorContactType,
		                    city: vm.contactVendor[i].Contact.Address.City,
		                    state: vm.contactVendor[i].Contact.Address.State,
		                    country: vm.contactVendor[i].Contact.Address.State.Country,
		                    district: vm.contactVendor[i].Contact.Address.District,
		                    region: vm.contactVendor[i].Contact.Address.State.Country.Continent,
		                    detail: vm.address1,
		                    postalCode: vm.postcalcode,
		                    info: vm.info
		                }
		                vm.addresses.push(address);

		            }
		            else if (vm.contactVendor[i].VendorContactType.Name === 'VENDOR_CONTACT_TYPE_PERSONAL') {
		                var Phone = vm.contactVendor[i].Contact.Phone;

		                var data = {
		                    VendorContactType: vm.contactVendor[i].VendorContactType,
		                    ContactID: vm.contactVendor[i].Contact.ContactID,
		                    name: vm.contactVendor[i].Contact.Name,
		                    phone: Phone,
		                    email: vm.contactVendor[i].Contact.Email
		                }
		                vm.contacts.push(data);


		            }
		            else if (vm.contactVendor[i].VendorContactType.Name === 'VENDOR_CONTACT_TYPE_COMPANY') {
		                vm.vendor.ContactID = vm.contactVendor[i].Contact.ContactID;
		                vm.Npwp = vm.contactVendor[i].Vendor.Npwp;
		                vm.vendor.npwp = vm.contactVendor[i].Vendor.Npwp;
		                vm.Siup = vm.contactVendor[i].Vendor.LegalNumber;
		                vm.NpwpUrl = vm.contactVendor[i].Vendor.NpwpUrl;
		                vm.siupUrl = vm.contactVendor[i].Vendor.LegalDocument;
		                vm.npwpDoc = vm.contactVendor[i].Vendor.NpwpUrl;
		                vm.sktDoc = vm.contactVendor[i].Vendor.LegalDocument;
		                vm.vendor.VendorID = vm.contactVendor[i].Vendor.VendorID;
		                vm.vendor.tendercodetemp = vm.contactVendor[i].Vendor.TenderCodeTemp;
		                vm.vendor.email = vm.contactVendor[i].Contact.Email;
		                vm.vendor.username = vm.contactVendor[i].Vendor.user.Username;
		                vm.vendor.website = vm.contactVendor[i].Contact.Website;
		                vm.phone = vm.contactVendor[i].Contact.Phone.split(' ');
		                vm.vendor.phone = vm.phone[1];
		                vm.phone = vm.phone[0].split(')');
		                vm.phone = vm.phone[0].split('(');
		                loadPhoneCodes(vm.phone[1]);
		                vm.vendor.fax = vm.contactVendor[i].Contact.Fax;
		                loadBusiness(vm.contactVendor[i].Vendor.BusinessID);
		                vm.vendor.name = vm.contactVendor[i].Vendor.VendorName;
		                vm.vendor.founded = new Date(Date.parse(vm.contactVendor[i].Vendor.FoundedDate));
		                vm.vendor.siup = vm.contactVendor[i].Vendor.LegalNumber;
		                loadCurrenciesData(vm.contactVendor[i].Vendor.VendorID);
		            }
		        }
		    }
		   
		}



		vm.loadCurrenciesData = loadCurrenciesData;
		function loadCurrenciesData(vendorID) {

			vm.currencies = [];
			VendorRegistrationService.selectVendorCurrencies({
				VendorID: vendorID
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.data;
					for (var i = 0; i < data.length; i++) {
						data[i].MstCurrency.Id = data[i].ID;
						vm.currencies.push(data[i].MstCurrency);
					}
				} else {
					$.growl.error({ message: "Gagal mendapatkan data Master Departemen" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				//$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}

		vm.loadStock = loadStock;
		function loadStock(vendorID) {
			vm.stocks = [];
			VendorRegistrationService.selectVendorStock({
				VendorID: vendorID
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.data;
					for (var i = 0; i < data.length; i++) {
						var stock = {
							StockID: data[i].StockID,
							quantity: data[i].Quantity,
							unit: data[i].Unit,
							owner: {
								name: data[i].OwnerName,
								dob: data[i].OwnerDOB,
								id: data[i].OwnerID,
								idDoc: data[i].OwnerIDUrl
							},
                            CurrencySymbol: data[i].CurrencySymbol
						};
						vm.stocks.push(stock);
					}
					console.info(vm.stocks);
					loadStockUnits(data[0]);
				} else {
					$.growl.error({ message: "Gagal mendapatkan data Master Departemen" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				//$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}

		vm.loadLicense = loadLicense;
		function loadLicense(vendorID) {
			vm.legals = [];
			VendorRegistrationService.selectVendorLegal({
				VendorID: vendorID
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.data;
					loadLegal(data);
				} else {
					$.growl.error({ message: "Gagal mendapatkan data Master Departemen" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				//$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}

	}
})();