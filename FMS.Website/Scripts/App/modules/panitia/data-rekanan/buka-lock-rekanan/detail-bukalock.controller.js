(function () {
	'use strict';

	angular.module("app").controller("DetailOpenLockCtrl", ctrl);

	ctrl.$inject = ['$translatePartialLoader', 'PermintaanUbahDataService', '$state', 'UIControlService', '$uibModal', '$stateParams', 'GlobalConstantService'];

	function ctrl($translatePartialLoader, PUbahDataService, $state, UIControlService, $uibModal, $stateParams, GlobalConstantService) {
		var vm = this;
		vm.currentMainPage = Number($stateParams.currentMainPage);
		vm.CRID = Number($stateParams.CRID);
		vm.folderFile = GlobalConstantService.getConstant('api') + "/";
		vm.listCR = [];
		vm.FilterColumn = 0;
		vm.textSearch = '';
		vm.maxSize = 10;
		vm.currentPage = 0;
		vm.VendorName = '';
		vm.CRDate = '';
		vm.VendorID;
		vm.init = init;

		function init() {
			$translatePartialLoader.addPart("permintaan-ubah-data");
			$translatePartialLoader.addPart('data-administrasi');
			$translatePartialLoader.addPart('vendor-balance');
			loadData();
		};

		vm.openCalendar = openCalendar;
		vm.isCalendarOpened = [false, false, false, false];
		function openCalendar(index) {
			vm.isCalendarOpened[index] = true;
		};

		function loadData() {
			UIControlService.loadLoading("Silahkan Tunggu");
			PUbahDataService.getDetailDataCR({
				ChangeRequestID: vm.CRID
			}, function (reply) {
				console.info("data:" + JSON.stringify(reply));
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.data;
					vm.VendorName = data[0].VendorName;
					vm.VendorID = data[0].VendorID;
					vm.OpsiCode = data[0].OpsiCode;
					if (!(data[0].CRDate === null)) {
						vm.CRDate = UIControlService.getStrDate(data[0].CRDate);
					}
					if (data[0].IsEditedByVendor == null) {
						vm.isEdited = false;
					} else {
						vm.isEdited = data[0].IsEditedByVendor;
					}

					vm.listCR = data;
					console.info(data[0].OpsiCode);
					if (data[0].OpsiCode === 'OC_COMPANYPERSON') {
						vm.loadCompanyPerson(1);
					} else if (data[0].OpsiCode === 'OC_VENDORBUSINESSFIELD') {
						vm.loadBusinessField(1);
					} else if (data[0].OpsiCode === 'OC_VENDORLICENSI') {
						vm.loadVendorLicense(1);
					} else if (data[0].OpsiCode === 'OC_VENDOREXPERIENCE') {
						vm.loadVendorExperience(1);
					} else if (data[0].OpsiCode === 'OC_VENDORSTOCK') {
						vm.loadVendorStock(1);
					} else if (data[0].OpsiCode === 'OC_VENDOREQUIPMENT') {
						vm.loadDataBuilding(1);
						vm.loadEquipmentVehicle(1);
						vm.loadEquipmentTools(1);
					} else if (data[0].OpsiCode === 'OC_VENDOREXPERTS') {
						vm.loadVendorExperts(1);
					} else if (data[0].OpsiCode === 'OC_ADM_LEGAL') {
						vm.LoadDataAdmin();
					} else if (data[0].OpsiCode === 'OC_VENDORBALANCE') {
						vm.loadNeraca();
					} else if (data[0].OpsiCode === 'OC_VENDORBANKDETAIL') {
						vm.loadBankDetail(1);
					}

					//vm.loadDataBuilding(1);
					//vm.loadEquipmentVehicle(1);
					//vm.loadEquipmentTools(1);
					//vm.loadVendorExperience(1);
					//vm.LoadDataAdmin();
					//vm.loadlicense();
				} else {
					UIControlService.msg_growl("error", "Gagal Mendapatkan Detai Data");
					UIControlService.unloadLoading();
				}
			}, function (err) {
				//console.info("error:" + JSON.stringify(err));
				UIControlService.msg_growl("error", "Gagal akses API");
				UIControlService.unloadLoading();
			});
		}

		vm.loadBankDetail = loadBankDetail;
		vm.listBankDetail = [];
		function loadBankDetail(current) {
			var offset = (current * 10) - 10;
			UIControlService.loadLoading(vm.msgLoading);
			PUbahDataService.selectBankDetail({
				Parameter: vm.VendorID,
				Offset: offset,
				Limit: 10
			}, function (reply) {
				UIControlService.unloadLoading();
				vm.changedData = reply.data.List;
				console.info("bankDet:" + JSON.stringify(vm.changedData));
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}

		//start load data perlengkapan
		vm.loadDataBuilding = loadDataBuilding;
		vm.listBuilding = [];
		vm.maxSizeBuild = 10;
		function loadDataBuilding(current) {
			var offset = (current * vm.maxSizeBuild) - vm.maxSizeBuild;
			UIControlService.loadLoading(vm.msgLoading);
			PUbahDataService.selectBuilding({
				Parameter: vm.VendorID,
				Offset: offset,
				Limit: 10
			}, function (reply) {
				console.info("bangunan:" + JSON.stringify(reply));
				UIControlService.unloadLoading();
				vm.listBuilding = reply.data.List;
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}

		vm.listVehicle = [];
		vm.loadEquipmentVehicle = loadEquipmentVehicle;
		function loadEquipmentVehicle(current) {
			var maxsize = 10;
			var offset = (current * vm.maxsize) - vm.maxsize;
			UIControlService.loadLoading(vm.msgLoading);
			PUbahDataService.selectVehicle({
				Parameter: vm.VendorID,
				Offset: offset,
				Limit: 10
			}, function (reply) {
				console.info("kendaraan:" + JSON.stringify(reply));
				UIControlService.unloadLoading();
				vm.listVehicle = reply.data.List;
				for (var i = 0; i < vm.listVehicle.length; i++) {
					vm.listVehicle[i].MfgDate = UIControlService.getStrDate(vm.listVehicle[i].MfgDate);
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}

		vm.listEquipmentTools = [];
		vm.loadEquipmentTools = loadEquipmentTools;
		function loadEquipmentTools(current) {
			//console.info("mlebu");
			var maxSize = 10;
			var offset = (current * maxSize) - maxSize;
			UIControlService.loadLoading(vm.msgLoading);
			PUbahDataService.selectEquipment({
				Parameter: vm.VendorID,
				Offset: offset,
				Limit: 10
			}, function (reply) {
				console.info("peralatan:" + JSON.stringify(reply));
				UIControlService.unloadLoading();
				vm.listEquipmentTools = reply.data.List;
				for (var i = 0; i < vm.listEquipmentTools.length; i++) {
					vm.listEquipmentTools[i].MfgDate = UIControlService.getStrDate(vm.listEquipmentTools[i].MfgDate);
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}
		//end

		//start load Company person data
		vm.loadCompanyPerson = loadCompanyPerson;
		function loadCompanyPerson(current) {
			vm.currentPage = current;
			var offset = (current * 10) - 10;
			PUbahDataService.selectCompanyPerson({
				Parameter: vm.VendorID,
				Offset: offset,
				Limit: 10
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.verified = reply.data.List;
					vm.totalData = reply.data.Count;
					vm.changedData = [];
					vm.modifiedData = [];
					//console.info("compers:" + JSON.stringify(vm.verified));
					for (var i = 0; i < vm.totalData; i++) {
						if (vm.verified[i].Action === 1 && vm.verified[i].IsActive === false) {
							vm.modifiedData.push(vm.verified[i]);
						}
					}
					//console.info("modifiedData:" + JSON.stringify(vm.modifiedData));

					for (var i = 0; i < vm.totalData; i++) {
						if (vm.verified[i].Action !== 1) {
							for (var j = 0; j < vm.modifiedData.length; j++) {
								if (vm.modifiedData[j].RefVendorId === vm.verified[i].ID) {
									vm.verified[i] = null;
									j = vm.modifiedData.length;
								}
							}
						}
					}
					console.info("ver:" + JSON.stringify(vm.verified));
					for (var i = 0; i < vm.totalData; i++) {
						if (vm.verified[i] !== null) {
							vm.changedData.push(vm.verified[i]);
						}
					}
					UIControlService.unloadLoading();
					console.info("changedData:" + JSON.stringify(vm.changedData));

				} else {
					$.growl.error({ message: "Gagal mendapatkan data" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				console.info("error:" + JSON.stringify(err));
				UIControlService.unloadLoading();
			});
		}
		//end

		//start load saham
		vm.loadVendorStock = loadVendorStock;
		function loadVendorStock(current) {
			vm.currentPage = current;
			var offset = (current * 10) - 10;
			PUbahDataService.selectVendorStock({
				Parameter: vm.VendorID,
				Offset: offset,
				Limit: 10
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.verified = reply.data.List;
					console.info("stock:" + JSON.stringify(vm.verified));
					vm.changedData = [];
					vm.totalData = reply.data.Count;
					vm.modifiedData = [];
					//console.info("compers:" + JSON.stringify(vm.verified));
					for (var i = 0; i < vm.totalData; i++) {
						if (vm.verified[i].Vendor.Action === 1 && vm.verified[i].Vendor.IsActive === false) {
							vm.modifiedData.push(vm.verified[i]);
						}
					}
					//console.info("modifiedData:" + JSON.stringify(vm.modifiedData));

					for (var i = 0; i < vm.totalData; i++) {
						if (vm.verified[i].Vendor.Action !== 1) {
							for (var j = 0; j < vm.modifiedData.length; j++) {
								if (vm.modifiedData[j].RefVendorId === vm.verified[i].ID) {
									vm.verified[i] = null;
									j = vm.modifiedData.length;
								}
							}
						}
					}
					console.info("ver:" + JSON.stringify(vm.verified));
					for (var i = 0; i < vm.totalData; i++) {
						if (vm.verified[i] !== null) {
							vm.changedData.push(vm.verified[i]);
						}
					}
				} else {
					$.growl.error({ message: "Gagal mendapatkan data" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				console.info("error:" + JSON.stringify(err));
				UIControlService.unloadLoading();
			});
		}
		//end

		//start load business field
		vm.loadBusinessField = loadBusinessField;
		function loadBusinessField(current) {
			vm.currentPage = current;
			var offset = (current * 10) - 10;
			PUbahDataService.selectBusinessField({
				Parameter: vm.VendorID,
				Offset: offset,
				Limit: 10
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.verified = reply.data.List;
					console.info("rep businessfield" + JSON.stringify(reply));
					vm.changedData = [];
					vm.totalData = reply.data.Count;
					console.info("totalData businessField:" + vm.totalData);
					for (var i = 0; i < vm.totalData; i++) {
						if (vm.verified[i].IsActive===true) {
							vm.changedData.push(vm.verified[i]);
						}
					}
					//console.info("changedData businessField:" + JSON.stringify(vm.changedData));
				} else {
					$.growl.error({ message: "Gagal mendapatkan data" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				console.info("error:" + JSON.stringify(err));
				UIControlService.unloadLoading();
			});
		}
		//end

		//start load Vendor License data
		vm.loadVendorLicense = loadVendorLicense;
		function loadVendorLicense(current) {
			vm.currentPage = current;
			var offset = (current * 10) - 10;
			PUbahDataService.selectVendorLicense({
				Parameter: vm.VendorID,
				Offset: offset,
				Limit: 10
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.verified = reply.data.List;
					vm.countLicense = reply.data.Count;
				} else {
					$.growl.error({ message: "Gagal mendapatkan data" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				console.info("error:" + JSON.stringify(err));
				UIControlService.unloadLoading();
			});
		}
		//end 

		//start load Vendor Experts data
		vm.loadVendorExperts = loadVendorExperts;
		function loadVendorExperts(current) {
			vm.currentPage = current;
			var offset = (current * 10) - 10;
			PUbahDataService.selectVendorExperts({
				Parameter: vm.VendorID,
				Offset: offset,
				Limit: 10
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					console.info("expert:" + JSON.stringify(reply));
					vm.vendorexperts = reply.data.List;
					vm.totalData = reply.data.Count;
				} else {
					$.growl.error({ message: "Gagal mendapatkan data" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				console.info("error:" + JSON.stringify(err));
				UIControlService.unloadLoading();
			});
		}

		//start load data pengalaman vendor
		vm.loadVendorExperience = loadVendorExperience;
		function loadVendorExperience(current) {
			var offset = (current * 10) - 10;
			UIControlService.loadLoading('LOADING.VENDOREXPERIENCE.MESSAGE');
			PUbahDataService.selectVendorExperience({
				Parameter: vm.VendorID,
				Offset: offset,
				Limit: 10,
				column: 1
			}, function (reply) {
				console.info("currExp: " + JSON.stringify(reply));
				if (reply.status === 200) {
					vm.listCurrExp = reply.data.List;
					vm.totalDataCurrExp = reply.data.Count;
					vm.changedDataCurrExp = [];
					vm.modifiedDataCurrExp = [];
					//console.info("totalData currExp:" + vm.totalDataCurrExp);
					for (var i = 0; i < vm.totalDataCurrExp; i++) {
						if (vm.listCurrExp[i].Action === 1 && vm.listCurrExp[i].IsActive === false) {
							vm.modifiedDataCurrExp.push(vm.listCurrExp[i]);
						}
					}
					//console.info("modifiedData currExp:" + JSON.stringify(vm.modifiedDataCurrExp));
					for (var i = 0; i < vm.totalDataCurrExp; i++) {
						if (vm.listCurrExp[i].Action !== 1) {
							for (var j = 0; j < vm.modifiedDataCurrExp.length; j++) {
								if (vm.modifiedDataCurrExp[j].RefVendorId === vm.listCurrExp[i].ID) {
									vm.listCurrExp[i] = null;
									j = vm.modifiedDataCurrExp.length;
								}
							}
						}
					}
					for (var i = 0; i < vm.totalDataCurrExp; i++) {
						if (vm.listCurrExp[i] !== null) {
							vm.changedDataCurrExp.push(vm.listCurrExp[i]);
						}
					}
					UIControlService.unloadLoading();
				} else {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("error", 'NOTIFICATION.VENDOREXPERIENCE.ERROR', "NOTIFICATION.VENDOREXPERIENCE.TITLE");
				}
			}, function (err) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", 'NOTIFICATION.VENDOREXPERIENCE.ERROR', "NOTIFICATION.VENDOREXPERIENCE.TITLE");
			});

			UIControlService.loadLoading('LOADING.VENDOREXPERIENCE.MESSAGE');
			PUbahDataService.selectVendorExperience({
				Parameter: vm.VendorID,
				Offset: offset,
				Limit: 10,
				column: 2
			}, function (reply) {
				console.info("finExp:" + JSON.stringify(reply));
				if (reply.status === 200) {
					UIControlService.unloadLoading();
					vm.listFinExp = reply.data.List;
					vm.totalDataFinExp = reply.data.Count;
					vm.changedDataFinExp = [];
					vm.modifiedDataFinExp = [];
					//console.info("totalData currExp:" + vm.totalDataCurrExp);
					for (var i = 0; i < vm.totalDataFinExp; i++) {
						if (vm.listFinExp[i].Action === 1 && vm.listFinExp[i].IsActive === false) {
							vm.modifiedDataFinExp.push(vm.listFinExp[i]);
						}
					}
					//console.info("modifiedData currExp:" + JSON.stringify(vm.modifiedDataCurrExp));
					for (var i = 0; i < vm.totalDataFinExp; i++) {
						if (vm.listFinExp[i].Action !== 1) {
							for (var j = 0; j < vm.modifiedDataFinExp.length; j++) {
								if (vm.modifiedDataFinExp[j].RefVendorId === vm.listFinExp[i].ID) {
									vm.listFinExp[i] = null;
									j = vm.modifiedDataFinExp.length;
								}
							}
						}
					}
					for (var i = 0; i < vm.totalDataFinExp; i++) {
						if (vm.listFinExp[i] !== null) {
							vm.changedDataFinExp.push(vm.listFinExp[i]);
						}
					}
					UIControlService.unloadLoading();
					/*
					console.info("totalData finExp:" + vm.totalDataFinExp);
					for (var i = 0; i < vm.listFinExp.length; i++) {
						if (vm.listFinExp[i].Action === 0 || vm.listFinExp[i].RefVendorId !== null) {
							if (vm.listFinExp[i].Action === 1) {
								var modAdded = false;
								for (var j = 0; j < vm.listFinExp.length; j++) {
									if (vm.listFinExp[i].RefVendorId === vm.listFinExp[j].ID) {
										//console.info("ketemu");
										modAdded = true;
									}
								}
								if (modAdded == false) {
									vm.changedDataFinExp.push(vm.listFinExp[i]);
								}
							} else if (vm.listFinExp[i].Action === 0) {
								vm.changedDataFinExp.push(vm.listFinExp[i]);
							}
						}
					}
					//console.info("changedData finExp:" + JSON.stringify(vm.changedDataFinExp));

					for (var i = 0; i < vm.changedDataFinExp.length; i++) {
						vm.changedDataFinExp[i].StartDate = UIControlService.getStrDate(vm.changedDataFinExp[i].StartDate);
					}
                    */
					//vm.totalItems = reply.data.Count;
				} else {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("error", 'NOTIFICATION.VENDOREXPERIENCE.ERROR', "NOTIFICATION.VENDOREXPERIENCE.TITLE");
				}
			}, function (err) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", 'NOTIFICATION.VENDOREXPERIENCE.ERROR', "NOTIFICATION.VENDOREXPERIENCE.TITLE");
			});

		}
		//end


		//start load data administrasi
		vm.LoadDataAdmin = LoadDataAdmin;
		function LoadDataAdmin() {
			PUbahDataService.selectDataAdministrasi({
				Parameter: vm.VendorID
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.verified = reply.data.List[0];
					console.info("adm:" + JSON.stringify(vm.verified));
					vm.LoadDataCurrency();
					vm.LoadDataContact();
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

		vm.LoadDataCurrency = LoadDataCurrency;
		function LoadDataCurrency() {
			vm.Currency = [];
			PUbahDataService.selectDataAdministrasiCurr({
				Parameter: vm.VendorID
			}, function (reply) {
				console.info(reply.data.List);
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.listcurr = reply.data.List;
					for (var i = 0; vm.listcurr.length; i++) {
						if (vm.listcurr[i].IsTemporary == true) vm.Currency.push(vm.listcurr[i]);
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

		vm.LoadDataContact = LoadDataContact;
		function LoadDataContact() {
			vm.ListPersonal = [];
			PUbahDataService.selectDataAdministrasiContact({
				Parameter: vm.VendorID
			}, function (reply) {
				console.info("contact" + JSON.stringify(reply));
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.contact = reply.data.List;
					console.info(vm.contact);
					for (var i = 0; i < vm.contact.length; i++) {
						if (vm.contact[i].VendorContactType.Name === 'VENDOR_CONTACT_TYPE_COMPANY' && vm.contact[i].IsTemporary === true && vm.contact[i].IsActive === true) {
							vm.refContactId = vm.contact[i].RefContactID;
							if (vm.contact[i].Contact.Fax !== "") {
								vm.FaxTemp = vm.contact[i].Contact.Fax;
							}
							vm.ContactID = vm.contact[i].Contact.ContactID;
							vm.phone = vm.contact[i].Contact.Phone.split(' ');
							vm.PhoneTemp = Number(vm.phone[1]);
							vm.EmailTemp = vm.contact[i].Contact.Email;
							vm.WebsiteTemp = vm.contact[i].Contact.Website;
							vm.AddressCompanyTemp = vm.contact[i].Contact.Address;
							for (var j = 0; j < vm.contact.length; j++) {
								if (vm.contact[j].VendorContactType.Name === 'VENDOR_CONTACT_TYPE_COMPANY' && vm.contact[j].ContactID === vm.refContactId) {
									if (vm.contact[j].Contact.Fax !== "") {
										vm.Fax = vm.contact[j].Contact.Fax;
									}
									vm.phone = vm.contact[j].Contact.Phone.split(' ');
									vm.Phone = Number(vm.phone[1]);
									vm.Email = vm.contact[j].Contact.Email;
									vm.Website = vm.contact[j].Contact.Website;
									vm.AddressCompany = vm.contact[j].Contact.Address;
									//vm.addressInfo = vm.contact[j].Contact.Address.AddressInfo.split(' ');
									//for (var y = 0; y < (vm.addressInfo.length) ; y++) {
									//    if (y == 0) vm.address = (vm.addressInfo[y] + ' ');
									//    else {
									//        if (vm.addressInfo[y] !== " ") {
									//            vm.address += (vm.addressInfo[y] + " ");
									//        }
									//    }
									//}
									//vm.address += (vm.contact[i].Contact.Address.AddressDetail);
								}

							}
						} else if (vm.contact[i].VendorContactType.Name === 'VENDOR_CONTACT_TYPE_PERSONAL' && vm.contact[i].IsTemporary === true && vm.contact[i].IsActive === true) {
							vm.ListPersonal.push(vm.contact[i]);
						}

						if (vm.contact[i].VendorContactType.Name === 'VENDOR_OFFICE_TYPE_MAIN' && vm.contact[i].IsPrimary === null && vm.contact[i].IsTemporary === true && vm.contact[i].IsActive === true) {
							vm.addressFlag = 1;
							vm.ContactName = "Kantor Pusat";
							vm.refContactId = vm.contact[i].ContactID;
							vm.addressInfo = vm.contact[i].Contact.Address.AddressInfo.split(' ');
							for (var y = 0; y < (vm.addressInfo.length) ; y++) {
								if (y == 0)
									vm.addressTemp = (vm.addressInfo[y] + ' ');
								else {
									if (vm.addressInfo[y] !== " ") {
										vm.addressTemp += (vm.addressInfo[y] + " ");
									}
								}
							}
							vm.addressTemp += (vm.contact[i].Contact.Address.AddressDetail);
							console.info(vm.addressTemp);
							for (var j = 0; j < vm.contact.length; j++) {
								if (vm.contact[j].VendorContactType.Name === 'VENDOR_OFFICE_TYPE_MAIN' && vm.contact[j].ContactID === vm.refContactId) {
									vm.addressInfo = vm.contact[j].Contact.Address.AddressInfo.split(' ');
									for (var y = 0; y < (vm.addressInfo.length) ; y++) {
										if (y == 0)
											vm.address1 = (vm.addressInfo[y] + ' ');
										else {
											if (vm.addressInfo[y] !== " ") {
												vm.address1 += (vm.addressInfo[y] + " ");
											}
										}
									}
									vm.address1 += (vm.contact[j].Contact.Address.AddressDetail);
									console.info(vm.address1);
								}

							}
						} else if (vm.contact[i].VendorContactType.Name === 'VENDOR_OFFICE_TYPE_MAIN' && vm.contact[i].IsPrimary === null) {
							vm.addressFlag = 1;
							vm.ContactName = "Kantor Pusat";
							vm.refContactId = vm.contact[i].ContactID;
							vm.addressInfo = vm.contact[i].Contact.Address.AddressInfo.split(' ');
							for (var y = 0; y < (vm.addressInfo.length) ; y++) {
								if (y == 0)
									vm.addressTemp = (vm.addressInfo[y] + ' ');
								else {
									if (vm.addressInfo[y] !== " ") {
										vm.addressTemp += (vm.addressInfo[y] + " ");
									}
								}
							}
							if (vm.contact[i].Contact.Address.DistrictID != null && vm.contact[i].Contact.Address.CityID != null)
							vm.addressTemp += ', ' + (vm.contact[i].Contact.Address.District.Name) +', ' + (vm.contact[i].Contact.Address.City.Name) + ', ' + (vm.contact[i].Contact.Address.State.Name);
                            else
							vm.addressTemp += ', '+ (vm.contact[i].Contact.Address.State.Name);
                            vm.address1 = vm.addressTemp;
						}

						if (vm.contact[i].VendorContactType.Name === 'VENDOR_OFFICE_TYPE_BRANCH' && vm.contact[i].IsPrimary === null && vm.contact[i].IsTemporary === true && vm.contact[i].IsActive === true) {
							if (vm.addressFlag == 0) {
								vm.refContactId = vm.contact[i].ContactID;
								vm.ContactName = "Kantor Cabang";
								vm.addressInfo = vm.contact[i].Contact.Address.AddressInfo.split(' ');
								for (var y = 0; y < (vm.addressInfo.length) ; y++) {
									if (y == 0)
										vm.addressTemp = (vm.addressInfo[y] + ' ');
									else {
										if (vm.addressInfo[y] !== " ") {
											vm.addressTemp += (vm.addressInfo[y] + " ");
										}
									}
								}
								vm.addressTemp += (vm.contact[i].Contact.Address.AddressDetail);
								console.info(vm.addressTemp);
								for (var j = 0; j < vm.contact.length; j++) {
									if (vm.contact[j].VendorContactType.Name === 'VENDOR_OFFICE_TYPE_BRANCH' && vm.contact[j].ContactID === vm.refContactId) {
										vm.addressInfo = vm.contact[j].Contact.Address.AddressInfo.split(' ');
										for (var y = 0; y < (vm.addressInfo.length) ; y++) {
											if (y == 0)
												vm.address1 = (vm.addressInfo[y] + ' ');
											else {
												if (vm.addressInfo[y] !== " ") {
													vm.address1 += (vm.addressInfo[y] + " ");
												}
											}
										}
                                        if (vm.contact[j].Contact.Address.DistrictID != null && vm.contact[j].Contact.Address.CityID != null)
							                vm.address1 += ', ' +(vm.contact[j].Contact.Address.District.Name) +', ' +(vm.contact[j].Contact.Address.City.Name) + ', ' +(vm.contact[j].Contact.Address.State.Name);
							            else
							                vm.address1 += ', ' +(vm.contact[j].Contact.Address.State.Name);
										
										console.info(vm.address1);
									}

								}
							}
						} else if (vm.contact[i].VendorContactType.Name === 'VENDOR_OFFICE_TYPE_BRANCH' && vm.contact[i].IsPrimary === null) {
							if (vm.addressFlag == 0) {
								vm.refContactId = vm.contact[i].ContactID;
								vm.ContactName = "Kantor Cabang";
								vm.addressInfo = vm.contact[i].Contact.Address.AddressInfo.split(' ');
								for (var y = 0; y < (vm.addressInfo.length) ; y++) {
									if (y == 0)
										vm.addressTemp = (vm.addressInfo[y] + ' ');
									else {
										if (vm.addressInfo[y] !== " ") {
											vm.addressTemp += (vm.addressInfo[y] + " ");
										}
									}
								}
                                if (vm.contact[i].Contact.Address.DistrictID != null && vm.contact[i].Contact.Address.CityID != null)
							        vm.addressTemp += ', ' +(vm.contact[i].Contact.Address.District.Name) + ', ' +(vm.contact[i].Contact.Address.City.Name) + ', ' +(vm.contact[i].Contact.Address.State.Name);
                                else
							        vm.addressTemp += ', '+(vm.contact[i].Contact.Address.State.Name);
								vm.address1 = vm.addressTemp;
							}
						}

						if (vm.contact[i].VendorContactType.Name === 'VENDOR_OFFICE_TYPE_MAIN' && vm.contact[i].IsPrimary !== null && vm.contact[i].IsTemporary === true && vm.contact[i].IsActive === true) {
							vm.addressFlagAlter = 1;
							vm.ContactNameAlter = "Kantor Pusat";
							vm.refContactId = vm.contact[i].ContactID;
							vm.addressInfo = vm.contact[i].Contact.Address.AddressInfo.split(' ');
							for (var y = 0; y < (vm.addressInfo.length) ; y++) {
								if (y == 0)
									vm.addressTempAlter = (vm.addressInfo[y] + ' ');
								else {
									if (vm.addressInfo[y] !== " ") {
										vm.addressTempAlter += (vm.addressInfo[y] + " ");
									}
								}
							}
							vm.addressTempAlter += (vm.contact[i].Contact.Address.AddressDetail);
							for (var j = 0; j < vm.contact.length; j++) {
								if (vm.contact[j].VendorContactType.Name === 'VENDOR_OFFICE_TYPE_MAIN' && vm.contact[j].ContactID === vm.refContactId) {
									vm.addressInfo = vm.contact[j].Contact.Address.AddressInfo.split(' ');
									for (var y = 0; y < (vm.addressInfo.length) ; y++) {
										if (y == 0)
											vm.address1Alter = (vm.addressInfo[y] + ' ');
										else {
											if (vm.addressInfo[y] !== " ") {
												vm.address1Alter += (vm.addressInfo[y] + " ");
											}
										}
									}
                                    if (vm.contact[j].Contact.Address.DistrictID != null && vm.contact[j].Contact.Address.CityID != null)
                                        vm.address1Alter += ', ' +(vm.contact[j].Contact.Address.District.Name) + ', ' +(vm.contact[j].Contact.Address.City.Name) + ', ' +(vm.contact[j].Contact.Address.State.Name);
                                    else
                                        vm.address1Alter += ', ' +(vm.contact[j].Contact.Address.State.Name);
								}

							}
						} else if (vm.contact[i].VendorContactType.Name === 'VENDOR_OFFICE_TYPE_MAIN' && vm.contact[i].IsPrimary !== null) {
							vm.addressFlagAlter = 1;
							vm.ContactNameAlter = "Kantor Pusat";
							vm.refContactId = vm.contact[i].ContactID;
							vm.addressInfo = vm.contact[i].Contact.Address.AddressInfo.split(' ');
							for (var y = 0; y < (vm.addressInfo.length) ; y++) {
								if (y == 0)
									vm.addressTempAlter = (vm.addressInfo[y] + ' ');
								else {
									if (vm.addressInfo[y] !== " ") {
										vm.addressTempAlter += (vm.addressInfo[y] + " ");
									}
								}
							}
                            if (vm.contact[i].Contact.Address.DistrictID != null && vm.contact[i].Contact.Address.CityID != null)
                                vm.addressTempAlter += ', ' +(vm.contact[i].Contact.Address.District.Name) + ', ' +(vm.contact[i].Contact.Address.City.Name) + ', ' +(vm.contact[i].Contact.Address.State.Name);
                            else
                                vm.addressTempAlter += ', ' +(vm.contact[i].Contact.Address.State.Name);
							vm.address1Alter = vm.addressTempAlter;
						}

						if (vm.contact[i].VendorContactType.Name === 'VENDOR_OFFICE_TYPE_BRANCH' && vm.contact[i].IsPrimary === null && vm.contact[i].IsTemporary === true && vm.contact[i].IsActive === true) {
							if (vm.addressFlagAlter == 0) {
								vm.refContactId = vm.contact[i].ContactID;
								vm.ContactName = "Kantor Cabang";
								vm.addressInfo = vm.contact[i].Contact.Address.AddressInfo.split(' ');
								for (var y = 0; y < (vm.addressInfo.length) ; y++) {
									if (y == 0)
										vm.addressTempAlter = (vm.addressInfo[y] + ' ');
									else {
										if (vm.addressInfo[y] !== " ") {
											vm.addressTempAlter += (vm.addressInfo[y] + " ");
										}
									}
								}
								vm.addressTempAlter += (vm.contact[i].Contact.Address.AddressDetail);
								for (var j = 0; j < vm.contact.length; j++) {
									if (vm.contact[j].VendorContactType.Name === 'VENDOR_OFFICE_TYPE_BRANCH' && vm.contact[j].ContactID === vm.refContactId) {
										vm.addressInfo = vm.contact[j].Contact.Address.AddressInfo.split(' ');
										for (var y = 0; y < (vm.addressInfo.length) ; y++) {
											if (y == 0)
												vm.address1Alter = (vm.addressInfo[y] + ' ');
											else {
												if (vm.addressInfo[y] !== " ") {
													vm.address1Alter += (vm.addressInfo[y] + " ");
												}
											}
										}
                                    if (vm.contact[j].Contact.Address.DistrictID != null && vm.contact[j].Contact.Address.CityID != null)
                                        vm.address1Alter += ', ' +(vm.contact[j].Contact.Address.District.Name) + ', ' +(vm.contact[j].Contact.Address.City.Name) + ', ' +(vm.contact[j].Contact.Address.State.Name);
                                    else
                                        vm.address1Alter += ', ' + (vm.contact[j].Contact.Address.State.Name);
									}
								}
							}
						} else if (vm.contact[i].VendorContactType.Name === 'VENDOR_OFFICE_TYPE_BRANCH' && vm.contact[i].IsPrimary === null) {
							if (vm.addressFlagAlter == 0) {
								vm.refContactId = vm.contact[i].ContactID;
								vm.ContactName = "Kantor Cabang";
								vm.addressInfo = vm.contact[i].Contact.Address.AddressInfo.split(' ');
								for (var y = 0; y < (vm.addressInfo.length) ; y++) {
									if (y == 0)
										vm.addressTempAlter = (vm.addressInfo[y] + ' ');
									else {
										if (vm.addressInfo[y] !== " ") {
											vm.addressTempAlter += (vm.addressInfo[y] + " ");
										}
									}
								}
                                if (vm.contact[i].Contact.Address.DistrictID != null && vm.contact[i].Contact.Address.CityID != null)
                                    vm.addressTempAlter += ', ' +(vm.contact[i].Contact.Address.District.Name) + ', ' +(vm.contact[i].Contact.Address.City.Name) + ', ' +(vm.contact[i].Contact.Address.State.Name);
                                        else
                                    vm.addressTempAlter += ', ' +(vm.contact[i].Contact.Address.State.Name);
								vm.address1Alter = vm.addressTempAlter;
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
		//end load data admnistrasi

		//start load data licensi
		vm.loadlicense = loadlicense;
		function loadlicense() {
			vm.currentPage = 1;
			var offset = (1 * 10) - 10;
			PUbahDataService.selectLicensi({
				Status: vm.VendorID,
				Offset: offset,
				Limit: vm.maxSize
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.license = reply.data;
					console.info("licensi:" + JSON.stringify(vm.license));
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
		//end load data licensi


		//start load neraca
		vm.loadNeraca = loadNeraca;
		function loadNeraca() {
			vm.asset = 0;
			vm.hutang = 0;
			vm.modal = 0;
			//console.info("curr "+current)
			vm.vendorbalance = [];
			PUbahDataService.selectNeraca({ VendorID: vm.VendorID }, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.vendorbalance = reply.data;
					console.info("neraca: " + JSON.stringify(vm.vendorbalance));
					for (var i = 0; i < vm.vendorbalance.length; i++) {
						if (vm.vendorbalance[i].WealthType.Name == "WEALTH_TYPE_ASSET") {
							vm.listAsset = vm.vendorbalance[i];
						}
						if (vm.vendorbalance[i].WealthType.Name == "WEALTH_TYPE_DEBTH") {
							vm.listDebth = vm.vendorbalance[i];
						}
					}
					for (var i = 0; i < vm.vendorbalance.length; i++) {
						for (var j = 0; j < vm.vendorbalance[i].subWealth.length; j++) {
							if (vm.vendorbalance[i].subWealth[j].subCategory.length === 0) {
								if (vm.vendorbalance[i].WealthType.RefID === 3097 && vm.vendorbalance[i].subWealth[j].IsActive === true) {
									if (vm.asset === 0) {
										vm.asset = vm.vendorbalance[i].subWealth[j].nominal;
										console.info(vm.asset);
									} else {
										vm.asset = +vm.asset + +vm.vendorbalance[i].subWealth[j].nominal;
										console.info(vm.asset);
									}
								} else if (vm.vendorbalance[i].WealthType.RefID === 3099 && vm.vendorbalance[i].subWealth[j].IsActive === true) {
									if (vm.hutang === 0) {
										vm.hutang = vm.vendorbalance[i].subWealth[j].nominal;
										console.info(vm.hutang);
									} else {
										vm.hutang = +vm.hutang + +vm.vendorbalance[i].subWealth[j].nominal;
										console.info(vm.hutang);
									}
								}
							} else {
								for (var k = 0; k < vm.vendorbalance[i].subWealth[j].subCategory.length; k++) {
									console.info(vm.vendorbalance[i].subWealth[j].subCategory[k]);
									if (vm.vendorbalance[i].subWealth[j].subCategory[k].Wealth.RefID === 3097 && vm.vendorbalance[i].subWealth[j].subCategory[k].IsActive === true) {
										if (vm.asset === 0) {
											vm.asset = vm.vendorbalance[i].subWealth[j].subCategory[k].Nominal;
											console.info(vm.asset);
										} else {
											vm.asset = +vm.asset + +vm.vendorbalance[i].subWealth[j].subCategory[k].Nominal;
											console.info(vm.asset);
										}
									} else if (vm.vendorbalance[i].subWealth[j].subCategory[k].WealthType === 3099 && vm.vendorbalance[i].subWealth[j].subCategory[k].IsActive === true) {
										if (vm.hutang === 0) {
											vm.hutang = vm.vendorbalance[i].subWealth[j].subCategory[k].Nominal;
											console.info(vm.hutang);
										} else {
											vm.hutang = +vm.hutang + +vm.vendorbalance[i].subWealth[j].subCategory[k].Nominal;
											console.info(vm.hutang);
										}
									}
								}
							}
						}
					}
					vm.modal = +vm.asset - +vm.hutang;
					console.info(JSON.stringify(vm.modal));
				} else {
					$.growl.error({ message: "Gagal mendapatkan data Neraca Perusahaan" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				console.info("error:" + JSON.stringify(err));
				//$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}
		//end

		//modal detail data
		vm.detailChangedData = detailChangedData;
		function detailChangedData(param, obj) {
			console.info("param:" + param);
			console.info("dataModal:" + JSON.stringify(obj));

			if (param == 9) { //Experience
				if (obj.ExperienceType === 3153) {
					console.info("finExp");
					vm.totalData = vm.totalDataFinExp;
				} else if (obj.ExperienceType === 3154) {
					console.info("currExp");
					vm.totalData = vm.totalDataCurrExp;
				}
			} else if (param == 6) { //Neraca
				//if (obj.ExperienceType === 3153) {
				//	console.info("finExp");
				//	vm.totalData = vm.totalDataFinExp;
				//} else if (obj.ExperienceType === 3154) {
				//	console.info("currExp");
				//	vm.totalData = vm.totalDataCurrExp;
				//}
			}

			var data = {
				item: obj,
				param: param,
				vendorID: vm.VendorID,
				totalData: vm.totalData
			}
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/data-rekanan/buka-lock-rekanan/changedDataDetail.modal.html',
				controller: 'ChangedDataDetailCtrl',
				controllerAs: 'ChangedDataDetailCtrl',
				resolve: {
					item: function () {
						return data;
					}
				}
			});
			modalInstance.result.then(function () {
				init();
			});
		}


		vm.approve = approve;
		function approve() {
			PUbahDataService.approve({ ChangeRequestID: vm.CRID }, function (reply) {
				if (reply.status === 200) {
					UIControlService.unloadLoading();
					UIControlService.msg_growl('notice', 'MESSAGE.SUCC_APPROVE');
					init();
				} else {
					UIControlService.unloadLoading();
					UIControlService.msg_growl('error', 'Gagal Akses API');
				}
			}, function (err) {
				UIControlService.unloadLoading();
				console.info("error:" + JSON.stringify(err));
				UIControlService.msg_growl('error', 'Gagal Akses API');
			});
		}

		vm.reject = reject;
		function reject() {
			PUbahDataService.reject({ ChangeRequestID: vm.CRID }, function (reply) {
				if (reply.status === 200) {
					UIControlService.unloadLoading();
					UIControlService.msg_growl('notice', 'MESSAGE.SUCC_REJECT');
				} else {
					UIControlService.unloadLoading();
					UIControlService.msg_growl('error', 'Gagal Akses API');
				}
			}, function (err) {
				UIControlService.unloadLoading();
				console.info("error:" + JSON.stringify(err));
				UIControlService.msg_growl('error', 'Gagal Akses API');
			});
		}

		vm.back = back;
		function back() {
		    console.info("curMainpage detail OL:" + vm.currentMainPage);
		    $state.transitionTo('open-lock-vendor', { currentMainPage: vm.currentMainPage });
		}
	}
})();