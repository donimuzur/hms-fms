(function () {
	'use strict';

	angular.module("app").controller("detailDokumenJasaCtrl", ctrl);

	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'OfferEntryService',
        '$state', 'UploadFileConfigService', 'UIControlService', 'UploaderService', 'item', '$uibModalInstance',
        'GlobalConstantService'];
	function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, OEService, $state, UploadFileConfigService,
        UIControlService, UploaderService, item, $uibModalInstance, GlobalConstantService) {
		var vm = this;
		vm.docurl = '';
		vm.DocumentName = item.DocumentName;
		vm.folderFile = GlobalConstantService.getConstant('api') + "/";
		vm.TypeSizeDoc = item.FileType;
		vm.ApproveDate = item.ApproveDate;
		vm.FileDoc = item.DocumentURL;
		vm.Remarks;
		vm.OfferEntryDocumentID = item.OfferEntryDocumentID;
		vm.OfferEntryVendorID = item.OfferEntryVendorID
		console.info("dok:" + JSON.stringify(item));
		vm.IsCheck = item.IsCheck;

		vm.init = init;
		function init() {
			//$translatePartialLoader.addPart("purchase-requisition");
			loadTypeSizeFile();
			//console.info(JSON.stringify(vm.DocUrl));
		}

		vm.batal = batal;
		function batal() {
			$uibModalInstance.dismiss('cancel');
		};

		vm.agreement = agreement;
		function agreement(data) {
			if (vm.IsCheck === false) {
				UIControlService.loadLoadingModal("Silahkan Tunggu");
				OEService.approveDocByVendor({
					OfferEntryDocumentID: vm.OfferEntryDocumentID,
					OfferEntryVendorID: vm.OfferEntryVendorID,
					UploadURL:vm.docurl
				}, function (reply) {
					UIControlService.unloadLoadingModal();
					if (reply.status === 200) {
						var data = reply.data;
						UIControlService.msg_growl("success", "Berhasil Simpan Data Persetujuan");
						$uibModalInstance.close();
					}
				}, function (err) {
					UIControlService.msg_growl("error", "MESSAGE.API");
					UIControlService.unloadLoadingModal();
				});
			} else {
				UIControlService.loadLoadingModal("Silahkan Tunggu");
				OEService.approveChecklistByVendor({
					OfferEntryChecklistID: vm.OfferEntryDocumentID,
					OfferEntryVendorID: vm.OfferEntryVendorID
				}, function (reply) {
					UIControlService.unloadLoadingModal();
					if (reply.status === 200) {
						var data = reply.data;
						UIControlService.msg_growl("success", "Berhasil Simpan Data Persetujuan");
						$uibModalInstance.close();
					}
				}, function (err) {
					UIControlService.msg_growl("error", "MESSAGE.API");
					UIControlService.unloadLoadingModal();
				});
			}
		}

		function loadTypeSizeFile() {
			UIControlService.loadLoadingModal("MESSAGE.LOADING");
			//get tipe dan max.size file - 1
			UploadFileConfigService.getByPageName("PAGE.ADMIN.TENDER.SERVICEOFFER", function (response) {
				UIControlService.unloadLoadingModal();
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
				UIControlService.unloadLoadingModal();
				return;
			});
		}

		vm.uploadFile = uploadFile;
		function uploadFile(data) {
			var folder = "TENDERJASA_" + vm.OfferEntryVendorID.toString() + '-' + vm.OfferEntryDocumentID.toString();
			var tipefileupload = typefile(vm.fileUpload, vm.idUploadConfigs);
			if (validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
				upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes, folder, tipefileupload,data);
			}
		}

		function typefile(file, allowedFileTypes) {
			if (!file || file.length == 0) {
				UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
				return false;
			}

			var selectedFileType = file[0].type;
			selectedFileType = selectedFileType.substring(selectedFileType.lastIndexOf('/') + 1);
			//console.info("tipefile: " + selectedFileType);
			if (selectedFileType === "vnd.ms-excel") {
				selectedFileType = "xls";
			} else if (selectedFileType === "vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
				selectedFileType = "xlsx";
			} else if (selectedFileType === "application/msword") {
				selectedFileType = "doc";
			} else if (selectedFileType === "application/vnd.openxmlformats-officedocument.wordprocessingml.document" || selectedFileType == "vnd.openxmlformats-officedocument.wordprocessingml.document") {
				selectedFileType = "docx";
			} else {
				selectedFileType = selectedFileType;
			}
			return selectedFileType;
			//console.info("file:" + selectedFileType);
		}

		function validateFileType(file, allowedFileTypes) {
			//console.info(JSON.stringify(allowedFileTypes));
			var selectedFileType = typefile(file, allowedFileTypes);
			var allowed = false;
			return true;
		}

		function upload(file, config, filters, folder, tipefile, data) {
			var size = config.Size;
			var unit = config.SizeUnitName;
			if (unit == 'SIZE_UNIT_KB') {
				size *= 1024;
			}

			if (unit == 'SIZE_UNIT_MB') {
				size *= (1024 * 1024);
			}

			//console.info(file[0].size + ":" + file[0].type);
			var tipesize_file = tipefile + " / " + Math.floor(file[0].size / 1024) + " KB";
			console.info(tipesize_file);

			UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_FILE");
			UploaderService.uploadSingleFile(file, "UPLOAD_DIRECTORIES_VENDORDATA", size, filters, folder, function (response) {
				UIControlService.unloadLoading();
				if (response.status == 200) {
					var url = response.data.Url;
					vm.docurl = url;
					//vm.dataExp.DocumentURL = url;
					//vm.pathFile = vm.folderFile + url;
					UIControlService.msg_growl("success", "FORM.MSG_SUC_UPLOAD");
					//saveProcess(url, tipesize_file);
					agreement(data);
				} else {
					UIControlService.msg_growl("error", "MESSAGE.ERR_UPLOAD");
					return;
				}
			}, function (response) {
				UIControlService.msg_growl("error", "MESSAGE.API")
				UIControlService.unloadLoading();
			});
		}

		function saveProcess(docurl, tipesize) {
			var savedata = {
				ID: vm.data.ID,
				IsRequired: vm.data.IsRequired,
				DocumentURL: docurl,
				FileType: tipesize
			}
			if (vm.ischeck === false) {
				OEService.updateDocs(savedata, function (reply) {
					UIControlService.unloadLoading();
					if (reply.status === 200) {
						UIControlService.msg_growl("success", "Berhasil Simpan Dokumen");
						//loadData();
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
			} else {
				OEService.updateChecklist(savedata, function (reply) {
					UIControlService.unloadLoading();
					if (reply.status === 200) {
						UIControlService.msg_growl("success", "Berhasil Simpan Dokumen");
						//loadData();
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

	}
})();
