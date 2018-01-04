(function () {
	'use strict';

	angular.module("app").controller("BankDetailModalCtrl", ctrl);

	ctrl.$inject = ['$http', '$uibModalInstance', 'item', '$filter', '$translate', '$translatePartialLoader', '$location', 'VerifiedSendService', 'SocketService', 'BankDetailService', 'UploaderService', 'UIControlService', 'GlobalConstantService', 'UploadFileConfigService'];
	/* @ngInject */
	function ctrl($http, $uibModalInstance, item, $filter, $translate, $translatePartialLoader, $location, VerifiedSendService, SocketService, BankDetailService, UploaderService, UIControlService, GlobalConstantService, UploadFileConfigService) {
		var vm = this;

		//console.info("console modal tambah");
		vm.detail = item.item;
		vm.VendorID;
		vm.NamaVendor;
		vm.folderFile = GlobalConstantService.getConstant('api') + "/";
		vm.isAdd = item.act;
		vm.action = "";
		vm.pathFile;
		vm.Description;
		vm.fileUpload;
		vm.size;
		vm.name;
		vm.type;
		vm.flag;
		vm.selectedForm;
		vm.isCalendarOpened = [false, false, false];
		vm.NamaAkun;
		vm.NoRek;
		vm.NamaBank;
		vm.Cabang;
		vm.Swift;
		vm.ID;
		vm.idFileTypes;
		vm.idFileSize;
		vm.idUploadConfigs;
		vm.DocUrl;

		vm.init = init;
		function init() {
			loadVerifiedVendor();
			loadCurrency();
			UIControlService.loadLoading("MESSAGE.LOADING");
			//get tipe dansa max.size file - 1
			UploadFileConfigService.getByPageName("PAGE.VENDOR.BANKDETAIL", function (response) {
				UIControlService.unloadLoading();
				if (response.status == 200) {
					//console.info(response);
					vm.name = response.data.name;
					vm.idUploadConfigs = response.data;
					vm.idFileTypes = generateFilterStrings(response.data);
					vm.idFileSize = vm.idUploadConfigs[0];
				} else {
					UIControlService.msg_growl("error", ".error!");
					return;
				}
			}, function (err) {
				UIControlService.msg_growl("error", "gagal akses API.");
				UIControlService.unloadLoading();
				return;
			});
			if (vm.isAdd === 1) {
				vm.action = "Tambah";
			} else {
				vm.action = "Ubah";
				vm.ID = item.item.ID;
				vm.NamaAkun = item.item.AccountName;
				vm.NoRek = item.item.AccountNo;
				vm.NamaBank = item.item.BankName;
				vm.Cabang = item.item.Branch;
				vm.Swift = item.item.SwiftCode;
				//vm.fileUpload = item.item.UrlDoc;
			}
		}

		//get VendorID
		vm.loadVerifiedVendor = loadVerifiedVendor;
		function loadVerifiedVendor() {
			VerifiedSendService.selectVerifikasi(function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.verified = reply.data;
					vm.cekTemporary = vm.verified.IsTemporary;
					vm.VendorID = vm.verified.VendorID;
					vm.NamaVendor = vm.verified.VendorName;
					//console.info(JSON.stringify(vm.NamaVendor));
				} else {
					$.growl.error({ message: "Gagal mendapatkan data bank." });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				//console.info("error:" + JSON.stringify(err));
				//$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}

		function generateFilterStrings(allowedTypes) {
			var filetypes = "";
			for (var i = 0; i < allowedTypes.length; i++) {
				filetypes += "." + allowedTypes[i].Name + ",";
			}
			return filetypes.substring(0, filetypes.length - 1);
		}

		vm.selectUpload = selectUpload;
		//vm.fileUpload;
		function selectUpload() {
			//console.info(vm.fileUpload);
		}

		vm.uploadFile = uploadFile;
		function uploadFile() {
			if (!(vm.action === 'Ubah' && (vm.fileUpload == null || vm.fileUpload == ''))) {
				if (validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
					upload(vm.VendorID, vm.fileUpload, vm.idFileSize, vm.idFileTypes);
				} else {
					return false;
				}
			} else {
				saveProcess();
			}
		}

		vm.loadCurrency = loadCurrency;
		function loadCurrency() {
		    BankDetailService.getCurrencies(function (reply) {
		        UIControlService.unloadLoading();
		        if (reply.status === 200) {
		            vm.currencyList = reply.data;
		            for (var i = 0; i < vm.currencyList.length; i++) {
		                if (vm.detail.CurrencyId == vm.currencyList[i].CurrencyID)
		                    vm.currency = vm.currencyList[i];
		            }
		        } else {
		            $.growl.error({ message: "Gagal mendapatkan data bank." });
		            UIControlService.unloadLoading();
		        }
		    }, function (err) {
		        //console.info("error:" + JSON.stringify(err));
		        //$.growl.error({ message: "Gagal Akses API >" + err });
		        UIControlService.unloadLoading();
		    });
		}

		function saveProcess() {
			if (vm.isAdd === 1) {
			    BankDetailService.insert({
			        CurrencyId: vm.currency.CurrencyID,
					VendorID: vm.VendorID,
					AccountNo: vm.NoRek,
					AccountName: vm.NamaAkun,
					BankName: vm.NamaBank,
					Branch: vm.Cabang,
					SwiftCode: vm.Swift,
					UrlDoc: vm.pathFile
				}, function (reply) {
					//console.info("reply" + JSON.stringify(reply))
					UIControlService.unloadLoadingModal();
					if (reply.status === 200) {
						UIControlService.msg_growl("success", "Berhasil Simpan Data");
						$uibModalInstance.close();
					} else {
						UIControlService.msg_growl("error", "Gagal menyimpan data.");
						return;
					}
				}, function (err) {
					console.info(err);
					UIControlService.msg_growl("error", "Gagal Akses Api.");
					UIControlService.unloadLoadingModal();
				});
			} else {
				if (vm.pathFile == null || vm.pathFile == '') {
					vm.pathFile = item.item.UrlDoc;
				}

				BankDetailService.Update({
					ID: vm.ID,
					VendorID: vm.VendorID,
					AccountNo: vm.NoRek,
					AccountName: vm.NamaAkun,
					BankName: vm.NamaBank,
					Branch: vm.Cabang,
					SwiftCode: vm.Swift,
					UrlDoc: vm.pathFile,
				    CurrencyId: vm.currency.CurrencyID
				}, function (reply) {
					UIControlService.unloadLoadingModal();
					if (reply.status === 200) {
						UIControlService.msg_growl("success", "Berhasil Simpan Dokumen.");
						$uibModalInstance.close();
					} else {
						UIControlService.msg_growl("error", "Gagal menyimpan data.");
						return;
					}
				}, function (err) {
					UIControlService.msg_growl("error", "Gagal Akses Api!!");
					UIControlService.unloadLoadingModal();
				});
			}
		}

		function validateFileType(file, allowedFileTypes) {
			if (!file || file.length == 0) {
				UIControlService.msg_growl("error", "file tidak ada ");
				return false;
			}

			var selectedFileType = file[0].type;
			selectedFileType = selectedFileType.substring(selectedFileType.lastIndexOf('/') + 1);

			if (selectedFileType === "jpeg") {
				selectedFileType = "jpeg";
			} else if (selectedFileType == "pdf") {
				selectedFileType = "pdf";
			} else {
				selectedFileType = selectedFileType;
			}
			vm.type = selectedFileType;
			//console.info("filenew:" + selectedFileType);
			//jika excel
			if (selectedFileType === "vnd.ms-excel")
				var allowed = false;
			for (var i = 0; i < allowedFileTypes.length; i++) {
				if (allowedFileTypes[i].Name == selectedFileType) {
					allowed = true;
					return allowed;
				}
			}
			if (allowed === false) {
				UIControlService.msg_growl("warning", "file tidak ada");
				return false;
			}
		}

		vm.upload = upload;
		function upload(id, file, config, filters, callback) {
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
			UploaderService.uploadSingleFileBankDetail(id, file, size, filters, function (response) {
				UIControlService.unloadLoading();
				//console.info("response:" + JSON.stringify(response));
				if (response.status == 200) {
					//console.info(response);
					var url = response.data.Url;
					vm.pathFile = url;
					vm.name = response.data.FileName;
					var s = response.data.FileLength;
					vm.DocUrl = vm.pathFile;
					//console.info(vm.DocUrl);
					if (vm.flag == 0) {
						vm.size = Math.floor(s);
						//  console.info(vm.size);
					}

					if (vm.flag == 1) {
						vm.size = Math.floor(s / (1024));
					}

					saveProcess();
				} else {
					UIControlService.msg_growl("error", "error");
					return;
				}
			}, function (response) {
				console.info(response);
				UIControlService.msg_growl("error", "gagal akses api!")
				UIControlService.unloadLoading();
			});


		}

		vm.batal = batal;
		function batal() {
			$uibModalInstance.dismiss('cancel');
		};
	}
})();