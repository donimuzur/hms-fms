(function () {
	'use strict';

	angular.module("app")
    .controller("formExpCtrl", ctrl);

	ctrl.$inject = ['$translatePartialLoader', 'VendorExperienceService', 'UIControlService', 'GlobalConstantService', 'UploadFileConfigService', 'UploaderService', 'item', 'ProvinsiService', 'AuthService', '$uibModalInstance'];
	/* @ngInject */
	function ctrl($translatePartialLoader, VendorExperienceService, UIControlService, GlobalConstantService, UploadFileConfigService, UploaderService, item, ProvinsiService, AuthService, $uibModalInstance) {
		var vm = this;
		//cek hanya bisa input pengalaman 5 tahun terakhir
		console.info(JSON.stringify(item));
		vm.isAdd = item.isAdd;
		vm.dataExp = {};
		var dataEdit = item.item;
		vm.fileUpload;
		vm.folderFile = GlobalConstantService.getConstant('api') + "/";
		vm.isCalendarOpened = [false, false, false, false];
		vm.IDN_id = 360;
		vm.regionID;
		vm.countryID;
		vm.folderFile = GlobalConstantService.getConstant('api') + "/";
		vm.datepelaksanaan;
		vm.dateselesai;
		vm.lokasidetail;
		vm.IsCR = item.isCR;
		//console.info(JSON.stringify(item));
		vm.selectedCurrencies = [];
		vm.listCurrencies = [];

		//if (vm.isAdd === false) {
		//	vm.cekTemporary = dataEdit.IsTemporary;
		//}

		vm.init = init;
		function init() {
			$translatePartialLoader.addPart("vendor-experience");
			loadTypeExp();
			if (vm.isAdd === true) {
				vm.dataExp = new dataField('', null, '', '', '', '', 0, null, null, new Date(), 0, 0, '', 0, '', null, 0);
				loadCountries();
			} else {
				vm.dataExp = new dataField(dataEdit.ContractName, dataEdit.Location, dataEdit.Address, dataEdit.Agency, dataEdit.AgencyTelpNo,
                    dataEdit.ContractNo, dataEdit.ContractValue, new Date(Date.parse(dataEdit.StartDate)),
                    new Date(Date.parse(dataEdit.EndDate)), new Date(Date.parse(dataEdit.UploadDate)), dataEdit.Field, dataEdit.FieldType,
                    dataEdit.Remark, dataEdit.ExperienceType, dataEdit.DocumentURL, dataEdit.StateLocation, dataEdit.ExpCurrID);

				vm.regionID = dataEdit.StateLocationRef.Country.Continent.ContinentID;
				vm.countryID = dataEdit.StateLocationRef.CountryID;
				vm.datepelaksanaan = UIControlService.getStrDate(vm.dataExp.StartDate);
				vm.dateselesai = UIControlService.getStrDate(vm.dataExp.EndDate);
				if (vm.countryID === 360) {
					vm.lokasidetail = dataEdit.CityLocation.Name + "," + dataEdit.StateLocationRef.Name + "," + dataEdit.StateLocationRef.Country.Name + "," + dataEdit.StateLocationRef.Country.Continent.Name;
				} else {
					vm.lokasidetail = dataEdit.StateLocationRef.Name + "," + dataEdit.StateLocationRef.Country.Name + "," + dataEdit.StateLocationRef.Country.Continent.Name;
				}
				loadCountries(dataEdit.StateLocationRef);
			}
			//loadCountries(dataEdit.StateLocationRef);
			loadTypeTender();
			getTypeSizeFile();
			loadCurrency(dataEdit);
		}

		vm.checkStartDate = checkStartDate;
		function checkStartDate(selectedStartDate) {
			var today = new Date();
			if (today < selectedStartDate) {
				UIControlService.msg_growl("warning", "Tanggal pelaksanaan tidak boleh setelah hari ini");
				vm.dataExp.StartDate = " ";
			}
		}

		vm.checkEndDate = checkEndDate;
		function checkEndDate(selectedEndDate, selectedStartDate, selectedExpType) {
			console.info(selectedExpType);
			if (selectedEndDate < selectedStartDate) {
				UIControlService.msg_growl("warning", "Tanggal selesai kontrak tidak boleh sebelum tanggal pelaksanaan");
				vm.dataExp.EndDate = " ";
			}
			else if (selectedExpType == 3153) {
				var convertedEndDate = moment(selectedEndDate).format('YYYY-MM-DD');

				console.info(convertedEndDate);
				var datenow = new Date();
				var convertedDateNow = moment(datenow).format('YYYY-MM-DD');
				console.info(convertedDateNow);
				if (datenow < selectedEndDate) {
					UIControlService.msg_growl("warning", "Tanggal selesai kontrak tidak boleh setelah hari ini karena jenis proses pekerjaan yang dipilih: sudah selesai");
				}
				else {
					console.info("masuk");
				}
			}
		}

		vm.listTypeExp = [];
		vm.cekTypeExp = 3154;
		function loadTypeExp() {
			UIControlService.loadLoading("Loading Tipe Pengalaman");
			VendorExperienceService.typeExperience(
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

		vm.loadStates = loadStates;
		function loadStates(country) {
			if (!country) {
				country = vm.selectedCountry;
				vm.selectedState = "";
			}
			loadRegions(country.CountryID);
			UIControlService.loadLoading("LOADERS.LOADING_STATE");
			VendorExperienceService.getStates(country.CountryID,
                function (response) {
                	vm.stateList = response.data;
                	for (var i = 0; i < vm.stateList.length; i++) {
                		if (country.StateID === vm.stateList[i].StateID) {
                			vm.selectedState = vm.stateList[i];
                			console.info("...." + JSON.stringify(vm.selectedState.Country.Code));
                			if (vm.selectedState.Country.Code === 'IDN') {
                				changeState();
                				break;
                			}
                		}
                	}
                	UIControlService.unloadLoading();
                });
		}

		vm.loadCountries = loadCountries;
		function loadCountries(data) {
			UIControlService.loadLoading("LOADERS.LOADING_COUNTRY");
			console.info("loadCountries");
			VendorExperienceService.getCountries(
                function (response) {
                	vm.countryList = response.data;
                	for (var i = 0; i < vm.countryList.length; i++) {
                		if (data !== undefined) {
                			if (data.CountryID === vm.countryList[i].CountryID) {
                				vm.selectedCountry = vm.countryList[i];
                				loadStates(data);
                				break;
                			}
                		}
                	}
                	UIControlService.unloadLoading();
                });
		}

		vm.loadRegions = loadRegions;
		function loadRegions(data) {
			UIControlService.loadLoading("LOADERS.LOADING_REGION");
			VendorExperienceService.getRegions({ CountryID: data },
                function (response) {
                	vm.selectedRegion = response.data;
                	UIControlService.unloadLoading();
                }
            );
		}

		/*city dkk
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
            console.info(JSON.stringify( vm.selectedRegions));
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
            console.info(JSON.stringify(vm.selectedCountry));
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
        }*/

		vm.changeState = changeState;
		vm.listCities = [];
		vm.selectedCities;
		function changeState() {
			console.info("masuk");
			if (!(vm.selectedState.CountryID === vm.IDN_id)) {
				vm.dataExp.Location = null;
				vm.dataExp.StateLocation = vm.selectedState.StateID;
			}
			ProvinsiService.getCities(vm.selectedState.StateID,
               function (response) {
               	vm.listCities = response.data;
               	console.info(">> " + JSON.stringify(vm.listCities));
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
			console.info(JSON.stringify(vm.selectedCities));
			vm.dataExp.StateLocation = vm.selectedState.StateID;
			if (vm.selectedCountry.CountryID === vm.IDN_id) {
				vm.dataExp.Location = vm.selectedCities.CityID;
			}

		}
		//end city dkk

		vm.openCalendar = openCalendar;
		function openCalendar(index) {
			vm.isCalendarOpened[index] = true;
		};

		/*field*/
		vm.selectedTypeTender;
		vm.listTypeTender;
		function loadTypeTender() {
			VendorExperienceService.getTypeTender(function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.listTypeTender = reply.data.List;
					if (vm.isAdd === false) {
						for (var i = 0; i < vm.listTypeTender.length; i++) {
							if (vm.listTypeTender[i].RefID === vm.dataExp.FieldType) {
								vm.selectedTypeTender = vm.listTypeTender[i];
								loadBusinessField();
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

		vm.loadCurrency = loadCurrency;
		vm.selectedCurrencies = [];
		function loadCurrency(data) {
			VendorExperienceService.getCurrencies(function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.listCurrencies = reply.data;
					if (vm.isAdd === false) {
						console.info(data.ExpCurrID);
						for (var i = 0; i < vm.listCurrencies.length; i++) {
							if (data.ExpCurrID === vm.listCurrencies[i].CurrencyID) {
								vm.selectedCurrencies = vm.listCurrencies[i];
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

		vm.changeCurr = changeCurr;
		function changeCurr(data) {
			console.info(data);
			vm.dataExp.ExpCurrID = data.CurrencyID;
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
			console.info(vm.dataExp.Field);
			VendorExperienceService.SelectBusinessField({
				GoodsOrService: vm.dataExp.FieldType
			},
            function (response) {
            	if (response.status === 200) {
            		vm.listBusinessField = response.data;
            		if (vm.isAdd === false) {
            			for (var i = 0; i < vm.listBusinessField.length; i++) {
            				if (vm.listBusinessField[i].ID === vm.dataExp.Field) {
            					vm.selectedBusinessField = vm.listBusinessField[i];
            					break;
            				}
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

		/*proses upload file*/
		function uploadFile() {
			AuthService.getUserLogin(function (reply) {
				console.info(JSON.stringify(reply));
				vm.dataExpLogin = reply.data.CurrentUsername + "_expe";
				if (validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
					upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes, vm.dataExpLogin);
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
			});

		}

		function upload(file, config, filters, dates, callback) {
			var size = config.Size;
			var unit = config.SizeUnitName;
			if (unit == 'SIZE_UNIT_KB') {
				size *= 1024;
			}

			if (unit == 'SIZE_UNIT_MB') {
				size *= (1024 * 1024);
			}

			UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_FILE");
			UploaderService.uploadSingleFile(file, "UPLOAD_DIRECTORIES_VENDORDATA", size, filters, dates,
                function (response) {
                	console.info()
                	UIControlService.unloadLoading();
                	if (response.status == 200) {
                		var url = response.data.Url;
                		vm.dataExp.DocumentURL = url;
                		vm.pathFile = vm.folderFile + url;
                		UIControlService.msg_growl("success", "FORM.MSG_SUC_UPLOAD");
                		saveProcess();

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

		function validateFileType(file, allowedFileTypes) {
			//console.info(JSON.stringify(allowedFileTypes));
			if (!file || file.length == 0) {
				UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
				return false;
			}
			return true;
		}

		/* end proses upload*/

		/*proses simpan*/
		vm.saveData = saveData;
		function saveData() {
			/*handle change request*/
			//if (vm.IsCR === false) vm.IsTemporary = false;
			//else if (vm.IsCR === true) vm.IsTemporary = true;

			//if (vm.isAdd === true) { vm.Action = 0; }
			//if (vm.isAdd === false) { vm.Action = 1; }
			//vm.dataExp['IsTemporary'] = vm.IsTemporary;
			//vm.dataExp['Action'] = vm.Action;
			//if (vm.isAdd === true && vm.IsCR === true) {
			//vm.dataExp['RefVendorId'] = null;
			//}
			/*end handle change request*/
			if (vm.fileUpload === undefined && vm.isAdd === true) {
				vm.dataExp.DocumentURL = '';
				saveProcess();
			} else if (!(vm.fileUpload === undefined)) {
				uploadFile();
			} else if (vm.fileUpload === undefined && vm.isAdd === false) {
				saveProcess();
			}
			vm.dataExp.ExperienceType = Number(vm.dataExp.ExperienceType);
		}

		function saveProcess() {
			//console.info(vm.IsCR);
			//console.info(vm.isAdd);
			//console.info(vm.cekTemporary);
			//if (vm.isAdd == true || ((vm.IsCR === true) && (vm.cekTemporary === false))) {
			if (vm.isAdd == true) {
				vm.dataExp['ExpCurrID'] = vm.dataExp.ExpCurrID;
				vm.dataExp['StateLocation'] = vm.selectedState.StateID;
				console.info(JSON.stringify(vm.dataExp));
				VendorExperienceService.Insert(vm.dataExp, function (reply) {
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
				//else if (vm.isAdd === false || ((vm.IsCR === true) && (vm.cekTemporary === true))) {
			else if (vm.isAdd === false) {
				vm.dataExp['ID'] = dataEdit.ID;
				//vm.dataExp['RefVendorId'] = dataEdit.ID;
				//console.info("edit:"+JSON.stringify(vm.dataExp));
				VendorExperienceService.Update(vm.dataExp, function (reply) {
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

		/*end proses simpan*/

		vm.batal = batal;
		function batal() {
			$uibModalInstance.dismiss('cancel');
		}

		function dataField(ContractName, Location, Address, Agency, AgencyTelpNo, ContractNo, ContractValue, StartDate,
            EndDate, UploadDate, Field, FieldType, Remark, ExperienceType, DocumentURL, StateLocation, ExpCurrID) {
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
			self.ExpCurrID = ExpCurrID;
		}
	}
})();

