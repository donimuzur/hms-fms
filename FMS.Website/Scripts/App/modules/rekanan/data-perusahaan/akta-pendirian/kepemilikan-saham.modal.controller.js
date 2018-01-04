(function () {
	'use strict';

	angular.module("app").controller("formKepemilikanSahamCtrl", ctrl);

	ctrl.$inject = ['$http', '$uibModalInstance', 'item', '$filter', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'AktaPendirianService', 'CommonEngineService', 'UploaderService', 'UIControlService', 'GlobalConstantService', 'UploadFileConfigService', 'VendorRegistrationService'];
	/* @ngInject */
	function ctrl($http, $uibModalInstance, item, $filter, $translate, $translatePartialLoader, $location, SocketService, AktaPendirianService, CommonEngineService, UploaderService, UIControlService, GlobalConstantService, UploadFileConfigService, VendorRegistrationService) {

		var loadmsg = 'MESSAGE.LOADING';
		var vm = this;

		vm.folderFile = GlobalConstantService.getConstant('api') + "/";

		vm.currencyList = [];
		vm.stockUnits = [];
		vm.stock = {};
		vm.file;
		vm.isDobCalendarOpened = false;

		vm.init = init;
		function init() {
			UIControlService.loadLoadingModal(loadmsg);
			//Konfigurasi upload disamakan dengan yang ada di halaman pendaftaran

			UploadFileConfigService.getByPageName("PAGE.VENDOR.REGISTRATION.ID", function (response) {
				if (response.status == 200) {
					vm.idUploadConfigs = response.data;
					vm.idFileTypes = generateFilterStrings(response.data);
					vm.idFileSize = vm.idUploadConfigs[0];
					AktaPendirianService.GetCurrencies(
                        function (response) {
                        	vm.currencyList = response.data;
                        	AktaPendirianService.GetStockTypes(
                                function (response) {
                                	vm.stockUnits = response.data;
                                	vm.stock = item.stock;
                                	vm.onUnitChange();
                                	VendorRegistrationService.getUploadPrefix(
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
                                }, function (error) {
                                	UIControlService.unloadLoadingModal();
                                	UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_TYPES')
                                }
                            );
                        }, function (error) {
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

		function generateFilterStrings(allowedTypes) {
			var filetypes = "";
			for (var i = 0; i < allowedTypes.length; i++) {
				filetypes += "." + allowedTypes[i].Name + ",";
			}
			return filetypes.substring(0, filetypes.length - 1);
		}

		vm.openDobCalendar = openDobCalendar;
		function openDobCalendar() {
			vm.isDobCalendarOpened = true;
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
				vm.stock.OwnerDOBDate = null;
			}

			console.info("umur" + JSON.stringify(yearAge));
			console.info("validasi" + JSON.stringify(validatedAge));
		}


		vm.save = save;
		function save() {
			//if (vm.stock.quantity === 0) {
			//	UIControlService.msg_growl('error', "FORM.NOTNULL");
			//	return;
			//}
			//if (vm.stocks.length === 0 || vm.stock.quantity > 100) {
			//	if (vm.stockUnit.Name === "STOCK_UNIT_PERCENTAGE") {
			//		if (vm.stock.quantity > 100) {
			//			UIControlService.msg_growl('error', "FORM.MAX_PERSEN");
			//			return;
			//		}
			//	}
			//} else {
			//	var jumlah = 0;
			//	for (var i = 0; i < vm.stocks.length; i++) {
			//		jumlah += +vm.stocks[i].quantity;
			//		if (i == (vm.stocks.length - 1)) {
			//			if ((+vm.stock.quantity + +jumlah) > 100) {
			//				UIControlService.msg_growl('error', "FORM.MAX_PERSEN");
			//				return;
			//			}

			//		}
			//	}
			//}


			UIControlService.loadLoadingModal(loadmsg);
			VendorRegistrationService.isAnotherStockHolder({
				VendorID: vm.stock.VendorID,
				OwnerID: vm.stock.OwnerID
			}, function (reply) {
				UIControlService.unloadLoadingModal();
				if (reply.status === 200 && reply.data === true) {
					UIControlService.msg_growl('error', "ERRORS.IS_ANOTHER_STOCKHOLDER");
				} else {
					uploadAndSave();
				}
			}, function (error) {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl('error', "ERRORS.CANNOT_VERIFY_STOCKHOLDER");
				uploadAndSave();
			});
		};

		function uploadAndSave() {
			if (!vm.file && !vm.stock.OwnerIDUrl) {
				UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
				return;
			}
			if (vm.file) {
				uploadFile(vm.file);
			} else {
				saveVendorStock(vm.stock.OwnerIDUrl);
			}
		}

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
			UploaderService.uploadRegistration(file, vm.stock.Npwp, vm.prefixes['UPLOAD_PREFIX_ID'].Value, size, types,
            function (reply) {
            	if (reply.status == 200) {
            		UIControlService.unloadLoadingModal();
            		var url = reply.data.Url;
            		saveVendorStock(url);
            	} else {
            		UIControlService.unloadLoadingModal();
            		UIControlService.msg_growl("error", 'MESSAGE.ERR_UPLOAD');
            	}
            }, function (err) {
            	UIControlService.unloadLoadingModal();
            	UIControlService.msg_growl("error", "MESSAGE.ERR_UPLOAD");
            });
		};

		function saveVendorStock(url) {
			vm.stock.OwnerIDUrl = url;
			vm.stock.Position;
			vm.stock.OwnerDOB = UIControlService.getStrDate(vm.stock.OwnerDOBDate);

			var saveVendorStock = vm.stock.StockID ? AktaPendirianService.EditVendorStock : AktaPendirianService.CreateVendorStock;

			UIControlService.loadLoadingModal(loadmsg);
			saveVendorStock(vm.stock, function (reply) {
				UIControlService.unloadLoadingModal();
				if (reply.status === 200) {
					UIControlService.msg_growl("notice", "MESSAGE.SUCC_SAVE_VSTOCK");
					$uibModalInstance.close();
				} else {
					UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE_VSTOCK");
				}
			}, function (error) {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE_VSTOCK");
			});
		};

		vm.close = close;
		function close() {
			$uibModalInstance.dismiss('cancel');
		};
	}
})();