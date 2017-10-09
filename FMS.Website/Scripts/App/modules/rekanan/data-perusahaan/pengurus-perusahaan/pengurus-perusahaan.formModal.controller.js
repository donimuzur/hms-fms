
(function () {
	'use strict';

	angular.module("app").controller("formPengurusPerusahaanCtrl", ctrl);

	ctrl.$inject = ['$uibModalInstance', 'item', '$location', 'SocketService', 'PengurusPerusahaanService', 'UploaderService', 'UIControlService', 'GlobalConstantService', 'UploadFileConfigService', 'ProvinsiService'];
	/* @ngInject */
	function ctrl($uibModalInstance, item, $location, SocketService, PengurusPerusahaanService, UploaderService, UIControlService, GlobalConstantService, UploadFileConfigService, ProvinsiService) {
		var loadmsg = 'MESSAGE.LOADING';
		var vm = this;

		vm.folderFile = GlobalConstantService.getConstant('api') + "/";
		vm.compPerson = item.compPerson;
		vm.file;
		vm.isCalendarOpened = [false, false, false];
		vm.positionTypes = [];
		//vm.IsCR = item.compPerson.IsCR;
		vm.vendorLocation = item.compPerson.Location;
		vm.action = item.action;

		//console.info("cr:" + JSON.stringify(item.compPerson.Address.StateID));
		vm.StateID = item.compPerson.Address.StateID;

		vm.init = init;
		function init() {
			console.info(item);
			UIControlService.loadLoadingModal(loadmsg);
			console.info("action:" + JSON.stringify(vm.action));
			//Konfigurasi upload disamakan dengan yang ada di halaman pendaftaran

			UploadFileConfigService.getByPageName("PAGE.VENDOR.COMPANYPERSON", function (response) {
				if (response.status == 200) {
					vm.idUploadConfigs = response.data;
					vm.idFileTypes = generateFilterStrings(response.data);
					vm.idFileSize = vm.idUploadConfigs[0];
					PengurusPerusahaanService.GetPositionTypes(function (response) {
						UIControlService.unloadLoadingModal();
						if (response.status == 200) {
							vm.positionTypes = response.data;
							if (vm.action === 'add') {
								loadCountries();
							} else if (vm.action === 'edit') {
								loadCountries(vm.compPerson.Address.State);
							}
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

		function generateFilterStrings(allowedTypes) {
			var filetypes = "";
			for (var i = 0; i < allowedTypes.length; i++) {
				filetypes += "." + allowedTypes[i].Name + ",";
			}
			return filetypes.substring(0, filetypes.length - 1);
		}

		/* begin provinsi, kabupaten, kecamatan */

		function loadCountries(data) {
			//UIControlService.loadLoading("LOADERS.LOADING_COUNTRY");
			PengurusPerusahaanService.getCountries(
                function (response) {
                	vm.countryList = response.data;
                	//console.info("negara"+JSON.stringify(vm.countryList));
                	if (vm.action === 'edit') {
                		for (var i = 0; i < vm.countryList.length; i++) {
                			if (vm.countryList[i].CountryID === data.CountryID) {
                				vm.selectedCountry = vm.countryList[i];
                				vm.countryCode = data.Country.Code;
                				changeCountry(false, data);
                			}
                		}
                	}
                }, function (response) {
                	UIControlService.unloadLoadingModal();
                	UIControlService.msg_growl("error", "MESSAGE.ERR_LOAD_STATES");;
                });
		}

		function getProvinsi() {
			UIControlService.loadLoadingModal(loadmsg);
			ProvinsiService.getStates(vm.CountryID, function (response) {
				UIControlService.unloadLoadingModal();
				vm.listProvinsi = response.data;
				getCities();
			}, function (response) {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("error", "MESSAGE.ERR_LOAD_STATES");;
			});
		}

		vm.changeProvince = changeProvince;
		function changeProvince() {
			vm.compPerson.Address.CityID = null;
			vm.listKabupaten = [];
			vm.compPerson.Address.DistrictID = null;
			vm.listKecamatan = [];
			getCities();
		}

		vm.changeCountry = changeCountry;
		function changeCountry(flag, data) {
			if (flag === true) {
				vm.countryCode = data.Code;
			}
			vm.CountryID = data.CountryID;
			getProvinsi();
		}

		function getCities() {
			if (vm.compPerson.Address.StateID) {
				UIControlService.loadLoadingModal(loadmsg);
				ProvinsiService.getCities(vm.compPerson.Address.StateID, function (response) {
					UIControlService.unloadLoadingModal();
					vm.listKabupaten = response.data;
					getDistrict();
				}, function (response) {
					UIControlService.unloadLoadingModal();
					UIControlService.msg_growl("error", "MESSAGE.ERR_LOAD_CITIES");
				});
			}
		}

		vm.validateAge = validateAge;
		function validateAge(inputDate) {
			//var convertedDate = moment(inputDate).format("DD-MM-YYYY");
			var validatedAge = false;
			var birthDate = moment(inputDate).format("DD-MM");;
			var dateNow = moment().format("DD-MM");
			var birthYear = moment(inputDate).format("YYYY");;
			var yearNow = moment().format("YYYY");
			var yearAge = yearNow - birthYear;
			if (yearAge > 17) {
				var validatedAge = true;
			} else if (yearAge === 17) {
				if (birthDate < dateNow || birthDate === dateNow) {
					var validatedAge = true;
				}
			}

			if (validatedAge === false) {
				UIControlService.msg_growl('error', "ERRORS.AGE_UNDER17");
				vm.compPerson.DateOfBirth = null;
			}

			console.info("umur" + JSON.stringify(yearAge));
			console.info("validasi" + JSON.stringify(validatedAge));
		}

		vm.changeCities = changeCities;
		function changeCities() {
			vm.compPerson.Address.DistrictID = null;
			vm.listKecamatan = [];
			getDistrict();
		}

		function getDistrict() {
			if (vm.compPerson.Address.CityID) {
				UIControlService.loadLoadingModal(loadmsg);
				ProvinsiService.getDistrict(vm.compPerson.Address.CityID, function (response) {
					UIControlService.unloadLoadingModal();
					vm.listKecamatan = response.data;
				}, function (response) {
					UIControlService.unloadLoadingModal();
					UIControlService.msg_growl("error", "MESSAGE.ERR_LOAD_DISTRICT");
				});
			}
		}
		/* end provinsi, kabupaten, kecamatan */

		vm.openCalendar = openCalendar;
		function openCalendar(index) {
			vm.isCalendarOpened[index] = true;
		}

		vm.fileSelect = fileSelect;
		function fileSelect(file) {
			vm.file = file;
		}

		vm.save = save;
		function save() {
			if (validateField() === false) {
				UIControlService.msg_growl("error", "MESSAGE.ERR_INCOMPLETE_FIELD");
				return;
			}

			if (!vm.file && !vm.compPerson.IDUrl) {
				UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
				return;
			}
			if (vm.file) {
				uploadFile(vm.file);
			} else {
				saveCompPerson(vm.compPerson.IDUrl);
			}
		};

		function validateField() {
			console.info("validate" + JSON.stringify(vm.vendorLocation));
			if (vm.vendorLocation === 'IDN') {
				if (!vm.compPerson.PersonName || !vm.compPerson.DateOfBirth || !vm.compPerson.NoID || !vm.compPerson.ServiceStartDate || !vm.compPerson.Address.AddressInfo) {
					console.info("vendor lokal");
					return false;
				}
			} else if (vm.vendorLocation !== 'IDN') {
				if (!vm.compPerson.PersonName || !vm.compPerson.DateOfBirth || !vm.compPerson.ServiceStartDate || !vm.compPerson.Address.AddressInfo) {
					console.info("vendor internasional");
					return false;
				}
			}
			return true;
		};

		function uploadFile(file) {
			if (validateFileType(file, vm.idUploadConfigs)) {
				upload(file, vm.idFileSize, vm.idFileTypes);
			}
		}

		function validateFileType(file, idUploadConfigs) {
			if (!file || file.length == 0) {
				UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
				return false;
			}
			return true;
		}

		function upload(file, config, types) {
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
			UIControlService.loadLoadingModal(loadmsg);
			UploaderService.uploadCompanyPersonID(vm.compPerson.VendorID, file, size, types,
            function (reply) {
            	if (reply.status == 200) {
            		UIControlService.unloadLoadingModal();
            		var url = reply.data.Url;
            		saveCompPerson(url);
            	} else {
            		UIControlService.unloadLoadingModal();
            		UIControlService.msg_growl("error", 'MESSAGE.ERR_UPLOAD');
            	}
            }, function (err) {
            	UIControlService.unloadLoadingModal();
            	UIControlService.msg_growl("error", "MESSAGE.ERR_UPLOAD");
            });
		};

		function saveCompPerson(url) {
			vm.compPerson.IDUrl = url;
			vm.compPerson.CompanyPosition;
			vm.compPerson.DateOfBirth = UIControlService.getStrDate(vm.compPerson.DateOfBirth);
			vm.compPerson.ServiceStartDate = UIControlService.getStrDate(vm.compPerson.ServiceStartDate);
			vm.compPerson.ServiceEndDate = UIControlService.getStrDate(vm.compPerson.ServiceEndDate);
			/*start handle change request*/
			console.info(JSON.stringify(vm.compPerson.NoID));
			if (vm.compPerson.ID === undefined) {
				//console.info("Add");
				//vm.compPerson['IsTemporary'] = true;
				//vm.compPerson['RefVendorId'] = null;
				//vm.compPerson['Action'] = 0;
			} else if (!vm.compPerson.ID === undefined) {
				//vm.compPerson['IsTemporary'] = true;
				//vm.compPerson['RefVendorId'] = vm.compPerson.ID;
				//vm.compPerson['Action'] = 1;
				//console.info("edit");
			}
			/*end handle change request*/
			//console.info(vm.IsCR);
			var saveSingle = vm.compPerson.ID ? PengurusPerusahaanService.EditSingle : PengurusPerusahaanService.CreateSingle;
			if (vm.vendorLocation !== 'IDN' && vm.compPerson.NoID === undefined) {
				vm.compPerson.NoID = "";
			}
			UIControlService.loadLoadingModal(loadmsg);
			saveSingle(vm.compPerson, function (reply) {
				UIControlService.unloadLoadingModal();
				if (reply.status === 200) {
					UIControlService.msg_growl("notice", "MESSAGE.SUCC_SAVE");
					$uibModalInstance.close();
				} else {
					UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
				}
			}, function (error) {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
			});
		};

		vm.close = close;
		function close() {
			$uibModalInstance.dismiss('cancel');
		};
	}
})();