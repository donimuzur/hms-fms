(function () {
	'use strict';

	angular.module("app").controller("FormIzinCtrl", ctrl);

	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'UIControlService', 'item', '$uibModal', 'IzinUsahaService', '$uibModalInstance', 'UploadFileConfigService', 'UploaderService', 'AuthService', '$filter', 'ProvinsiService', 'GlobalConstantService'];

	function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, UIControlService, item, $uibModal, IzinUsahaService, $uibModalInstance, UploadFileConfigService, UploaderService, AuthService, $filter, ProvinsiService, GlobalConstantService) {
		var vm = this;
		//console.info(JSON.stringify(item));
		vm.dataLicensi = item.item;
		if (item.isForm === true) {
			if (vm.dataLicensi.IssuedDate !== null) {
				vm.dataLicensi.IssuedDate = new Date(Date.parse(vm.dataLicensi.IssuedDate));
			}
			if (vm.dataLicensi.ExpiredDate !== null) {
				vm.dataLicensi.ExpiredDate = new Date(Date.parse(vm.dataLicensi.ExpiredDate));
			}
			console.info(JSON.stringify(vm.dataLicensi));
		} else {
			if (vm.dataLicensi.IssuedDate !== null) {
				vm.dataLicensi.IssuedDate = UIControlService.getStrDate(vm.dataLicensi.IssuedDate);
				// vm.dataLicensi.ExpiredDate = UIControlService.getStrDate(vm.dataLicensi.ExpiredDate);
			}
			if (vm.dataLicensi.ExpiredDate !== null) {
				vm.dataLicensi.ExpiredDate = UIControlService.getStrDate(vm.dataLicensi.ExpiredDate);
			}
		}

		vm.licensiname = vm.dataLicensi.LicenseName;
		vm.isCalendarOpened = [false, false, false, false];
		vm.pathFile;
		vm.getIDstate;
		vm.VendorLogin;
		vm.folderFile = GlobalConstantService.getConstant('api') + "/";
		vm.mindate = new Date();
		vm.cityID = item.cityID;
		vm.isNominal = vm.dataLicensi.IsNominal;
		vm.tglSekarang = new Date();

		vm.init = init;
		function init() {
			$translatePartialLoader.addPart("data-izinusaha");
			loadKlasifikasi();
			getTypeSizeFile();
			//console.info(vm.isNominal);
			//console.info(JSON.stringify(vm.cityID));
			if (vm.dataLicensi.IssuedLocation !== null) {
			    getCityByID(vm.dataLicensi.IssuedLocation);
			} else {
			    changeCountry(vm.cityID);
			}

			/*
		    if (!(vm.dataLicensi.LicenseNo === null)) {
		        //console.info("sini"+JSON.stringify(vm.dataLicensi.IssuedLocation));
		        getCityByID(vm.cityID);
		    } else {
		        changeCountry(0);
		    }*/
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
			vm.dataLicensi.IssuedDate = sid;
		}

		vm.selectedExpiredDate = selectedExpiredDate;
		function selectedExpiredDate(sed) {
			//console.info("selected expired date:" + JSON.stringify(sed));

			if (sed > vm.dataLicensi.IssuedDate) {
			} else if (sed < vm.dataLicensi.IssuedDate) {
				UIControlService.msg_growl("warning", "Tanggal berakhir harus sesudah tanggal mulai");
				vm.dataLicensi.ExpiredDate = "";
				//console.info("masuk");
			}
		}

		vm.selectedExpiredDate2 = selectedExpiredDate2;
		function selectedExpiredDate2(sed) {
			//console.info("selected expired date:" + JSON.stringify(sed));

			if (sed > vm.tglSekarang) {
			} else if (sed >= vm.dataLicensi.IssuedDate && sed <= vm.tglSekarang) {
				UIControlService.msg_growl("warning", "Masa berlaku  izin usaha sudah habis");
				vm.dataLicensi.ExpiredDate = "";
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
			IzinUsahaService.getClasification(function (reply) {
				UIControlService.unloadLoading();
				vm.listClasification = reply.data.List;
				if (!(vm.dataLicensi.LicenseNo === null)) {
					for (var i = 0; i < vm.listClasification.length; i++) {
						if (vm.listClasification[i].RefID === vm.dataLicensi.CompanyScale) {
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
		vm.listState = [];
		vm.selectedState;
		function changeCountry(idstate) {
		    ProvinsiService.getStates(idstate,
               function (response) {
                   vm.listState = response.data;
                   if (vm.dataLicensi.IssuedLocation !== null) {
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
		vm.listCities = [];
		vm.selectedCities;
		function changeState() {
		    ProvinsiService.getCities(vm.selectedState.StateID,
            function (response) {
                vm.listCities = response.data;
                if (!(vm.dataLicensi.LicenseNo === null)) {
                    for (var i = 0; i < vm.listCities.length; i++) {
                        if (vm.dataLicensi.IssuedLocation === vm.listCities[i].CityID) {
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
			vm.dataLicensi.IssuedLocation = vm.selectedCities.CityID;
		}
		/* end combo country, state, city*/

		/*get type n size file upload*/
		vm.selectUpload = selectUpload;
		vm.fileUpload;
		function selectUpload() {
			console.info(">" + vm.fileUpload);
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
		    console.info(vm.dataLicensi.CapitalAmount);
		    if (vm.licensiname === 'SIUP') {
		        if (vm.selectedClasification === undefined && vm.dataLicensi.CapitalAmount === " ") {
		            UIControlService.msg_growl("warning", "Klasifikasi dan Nominal harus terisi (minimal salah satu)");
		            return;
		        }
				//UIControlService.msg_growl("warning", "MESSAGE.CMB_CLASIFICATION");
				//return;
		    }

		    if (vm.dataLicensi.IssuedDate === null) {
		        UIControlService.msg_growl("warning", "MESSAGE.ISSUED_DATE");
		        return;
		    }   
			else if (vm.dataLicensi.ExpiredDate === null) {
				UIControlService.msg_growl("warning", "MESSAGE.EXPIRED_DATE");
				return;
			}
			if (vm.dataLicensi.LicenseNo == null || vm.dataLicensi.IssuedBy == null || vm.selectedState == null ||
                vm.selectedCities == null || (vm.fileUpload == null && vm.dataLicensi.DocumentURL === null)) {
			    UIControlService.msg_growl("warning", "Data tidak lengkap");
			    return;
			}
			else if (!(vm.dataLicensi.LicenseNo == null || vm.dataLicensi.IssuedBy == null || vm.selectedState == null ||
                vm.selectedCities == null || (vm.fileUpload == null && vm.dataLicensi.DocumentURL === null))) {
			    vm.datasimpan = vm.dataLicensi;
			    //console.info("lha"+JSON.stringify(vm.datasimpan));
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
			        vm.datasimpan['DocumentURL'] = vm.dataLicensi.DocumentURL;
			        //console.info("1-"+JSON.stringify(vm.datasimpan));
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

		function upload(file, config, filters, dates, callback) {
			console.info(dates);
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
                		vm.datasimpan['DocumentURL'] = url;
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

		function validateFileType(file, allowedFileTypes) {
			if (!file || file.length == 0) {
				UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
				return false;
			}
			return true;
		}

		/* end proses upload*/

		function saveprocess() {
			console.info("data simpan" + JSON.stringify(vm.datasimpan));
			IzinUsahaService.updateLicensi(vm.datasimpan, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					UIControlService.msg_growl("success", "FORM.MSG_SUC_SAVE");
					$uibModalInstance.close();
				} else {
					UIControlService.msg_growl("error", "FORM.MSG_ERR_SAVE");
					return;
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.ERR_API");
				UIControlService.unloadLoadingModal();
			});
		}

		vm.batal = batal;
		function batal() {
			$uibModalInstance.close();
		};
	}
})();