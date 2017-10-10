(function () {
	'use strict';

	angular.module("app")
    .controller("formPPController", ctrl);

	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'PengumumanPengadaanService', '$state', 'UIControlService', 'item', '$uibModalInstance',
        'UploadFileConfigService', 'UploaderService', '$uibModal', 'GlobalConstantService'];
	function ctrl($http, $translate, $translatePartialLoader, $location, SocketService,
        PengumumanPengadaanService, $state, UIControlService, item, $uibModalInstance, UploadFileConfigService,
        UploaderService, $uibModal, GlobalConstantService) {
		console.info(JSON.stringify(item));
		var vm = this;
		vm.DataTender = item.DataTender;
		vm.Description = '';
		vm.fileUpload;
		vm.ProcPackType = vm.DataTender.IDProcPackType;
		vm.TypeTender = item.TypeTender;
		vm.TenderID = item.IDTender;
		vm.TenderStepDataID = item.IDStepTender;
		vm.folderFile = GlobalConstantService.getConstant('api') + "/";

		vm.isAdd = item.isAdd;
		//ng-model
		vm.TenderCode = ''; vm.TenderName = ''; vm.IsLocal = false; vm.IsNational = false; vm.IsInternational = false;
		vm.IsVendorEmails = false; vm.IsOpen = false; vm.Emails = ''; vm.CompScale = null; vm.CommodityID = null; vm.DocUrl = '';

		vm.init = init;
		function init() {
			loadTypeSizeFile();
			loadKlasifikasi();
			loadKomoditi();
			loadTechnical();
			vm.TenderCode = vm.DataTender.TenderCode;
			vm.TenderName = vm.DataTender.TenderName;
			if (vm.DataTender.IsLocal === true) {
				vm.IsLocal = vm.DataTender.IsLocal;
			}
			if (vm.DataTender.IsNational === true) {
				vm.IsNational = vm.DataTender.IsNational;
			}
			if (vm.DataTender.IsInternational === true) {
				vm.IsInternational = vm.DataTender.IsInternational;
			}
			if (vm.DataTender.IsOpen === true) {
				vm.IsOpen = vm.DataTender.IsOpen;
			}
			if (vm.DataTender.IsVendorEmails === true) {
				vm.IsVendorEmails = vm.DataTender.IsVendorEmails;
				//vm.Emails = vm.DataTender.Emails;
			}

			vm.CompScale = vm.DataTender.CompScale;
			vm.CommodityID = vm.DataTender.CommodityID;
			vm.Vendors = vm.DataTender.Vendors;
			if (vm.isAdd === false) {
				vm.Description = vm.DataTender.Description;
				vm.TechnicalID = vm.DataTender.TechnicalID;
				vm.DocUrl = vm.DataTender.DocUrl;
			}
			vm.Emails = vm.DataTender.Emails;
			vm.sendEmail = [];
			var batas = vm.Vendors.length - 1;
			if (vm.Emails === '' || vm.Emails === null) {
				for (var i = 0; i < vm.Vendors.length; i++) {
					if (i === batas) {
						vm.Emails += vm.Vendors[i].Email;
					} else {
						vm.Emails += vm.Vendors[i].Email + " , ";
					}
				}
			}
			if (!(vm.Emails === '' || vm.Emails === null)) {
				vm.IsVendorEmails = true;
			}

		}
		/*proses upload file*/
		function loadTypeSizeFile() {
			UIControlService.loadLoading("MESSAGE.LOADING");
			//get tipe dan max.size file - 1
			UploadFileConfigService.getByPageName("PAGE.ADMIN.TENDER.ANNOUNCEMENT", function (response) {
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

		vm.uploadFile = uploadFile;
		function uploadFile() {
			var folder = "TENDER" + vm.TypeTender + vm.TenderID;
			if (validateFileType(vm.fileUpload, vm.idUploadConfigs)) {
				upload(vm.fileUpload, vm.idFileSize, vm.idFileTypes, folder);
			}

		}

		function upload(file, config, filters, folder, callback) {
			var size = config.Size;
			var unit = config.SizeUnitName;
			if (unit == 'SIZE_UNIT_KB') {
				size *= 1024;
			}

			if (unit == 'SIZE_UNIT_MB') {
				size *= (1024 * 1024);
			}

			UIControlService.loadLoading("LOADERS.LOADING_UPLOAD_FILE");
			UploaderService.uploadSingleFile(file, "UPLOAD_DIRECTORIES_ADMIN", size, filters, folder,
                function (response) {
                	UIControlService.unloadLoading();
                	if (response.status == 200) {
                		var url = response.data.Url;
                		UIControlService.msg_growl("success", "Berhasil Upload File");
                		processSave(url);

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

			/* Berbeda antara Chrome dan Mozilla FF
            var selectedFileType = file[0].type;
            selectedFileType = selectedFileType.substring(selectedFileType.lastIndexOf('/') + 1);
            //console.info("tipefile: " + selectedFileType);
            if (selectedFileType === "vnd.ms-excel") {
                selectedFileType = "xls";
            }
            else if (selectedFileType === "vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
                selectedFileType = "xlsx";
            } else if (selectedFileType === "application/msword") {
                selectedFileType = "doc";
            }
            else if (selectedFileType === "application/vnd.openxmlformats-officedocument.wordprocessingml.document" || selectedFileType == "vnd.openxmlformats-officedocument.wordprocessingml.document") {
                selectedFileType = "docx";
            }
            else {
                selectedFileType = selectedFileType;
            }
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
            */
			return true;
		}
		/* end proses upload*/

		//list combo komoditas
		vm.selectedComodity;
		vm.listComodity = [];
		function loadKomoditi() {
			PengumumanPengadaanService.getCommodity({ type: vm.TypeTender }, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.listComodity = reply.data;
					if ((vm.CommodityID > 0 || vm.CommodityID != null)) {
						console.info(vm.CommodityID + "...");
						for (var i = 0; i < vm.listComodity.length; i++) {
							if (vm.CommodityID === vm.listComodity[i].ID) {
								vm.selectedComodity = vm.listComodity[i];
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

		vm.changeComodity = changeComodity;
		function changeComodity() {
			vm.CommodityID = vm.selectedComodity.ID;
		}

		//list combo klasifikasi
		vm.selectedClasification;
		vm.listClasification = [];
		function loadKlasifikasi() {
			PengumumanPengadaanService.getClasification(function (reply) {
				UIControlService.unloadLoading();
				vm.listClasification = reply.data.List;
				if ((vm.CompScale > 0 || vm.CompScale != null)) {
					for (var i = 0; i < vm.listClasification.length; i++) {
						if (vm.CompScale === vm.listClasification[i].RefID) {
							vm.selectedClasification = vm.listClasification[i];
							break;
						}
					}
				}
			}
            , function (err) {
            	UIControlService.msg_growl("error", "MESSAGE.API");
            	UIControlService.unloadLoading();
            });
		}

		vm.changeClasification = changeClasification;
		function changeClasification() {
			vm.CompScale = vm.selectedClasification.RefID;
		}

		//list combo technical
		vm.selectedTechnical;
		vm.listTechnical = [];
		function loadTechnical() {
			PengumumanPengadaanService.getTechnical(function (reply) {
				UIControlService.unloadLoading();
				vm.listTechnical = reply.data.List;
				if ((vm.TechnicalID > 0 || vm.TechnicalID != null)) {
					for (var i = 0; i < vm.listTechnical.length; i++) {
						if (vm.TechnicalID === vm.listTechnical[i].RefID) {
							vm.selectedTechnical = vm.listTechnical[i]
							break;
						}
					}
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}

		vm.changeTechnical = changeTechnical;
		function changeTechnical() {
			vm.TechnicalID = vm.selectedTechnical.RefID;
		}

		/* tambah vendor */
		vm.openDataVendor = openDataVendor;
		function openDataVendor() {
			var data = {};
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/pengumuman-pengadaan/DataVendor.html',
				controller: 'DataVendorCtrl',
				controllerAs: 'DataVendorCtrl',
				resolve: {
					item: function () {
						return data;
					}
				}
			});
			modalInstance.result.then(function (dataVendor) {
				//console.info("get:"+JSON.stringify(dataVendor));
				//cek vendor sudah ada belum
				var foundby = $.map(vm.Vendors, function (val) {
					return val.VendorID === dataVendor.VendorID ? val : null;
				});
				if (foundby.length > 0) {
					UIControlService.msg_growl("error", "Vendor " + dataVendor.VendorName + " sudah ditambahkan!!");
					return;
				} else {
					vm.Vendors.push(dataVendor);
				}
			});
		}

		//proses simpan
		vm.saveData = saveData;
		function saveData() {
			//console.info("com:" + vm.CommodityID + " clas:" + vm.CompScale + " tech" + vm.TechnicalID);
			if (vm.selectedTechnical === undefined) {
				vm.TechnicalID = null;
			}
			if (vm.fileUpload === undefined) {
				console.info("nofile");
				processSave('')
			} else {
				uploadFile();
			}

		}

		function processSave(urlDoc) {
			//console.info(">>>" + vm.TenderStepDataID);
			var detailVendor = [];
			for (var i = 0; i < vm.Vendors.length; i++) {
				var data = { VendorID: vm.Vendors[i].VendorID }
				detailVendor.push(data);
			}
			//set send email
			if (vm.Emails) {
				vm.sendEmail = vm.Emails.split(',');
			}
			//console.info(JSON.stringify(vm.sendEmail));

			var dataAnnouncement = {
				CommodityID: vm.CommodityID,
				CompanyScaleID: vm.CompScale,
				Description: vm.Description,
				DocUrl: urlDoc,
				Emails: vm.Emails,
				IsInternational: vm.IsInternational,
				IsLokal: vm.IsLocal,
				IsNational: vm.IsNational,
				IsOpen: vm.IsOpen,
				IsVendorEmail: vm.IsVendorEmails,
				TechnicalID: vm.TechnicalID,
				TenderStepDataID: vm.TenderStepDataID
			}
			UIControlService.unloadLoadingModal("Silahkan Tunggu");
			PengumumanPengadaanService.insertAnnouncement({
				TenderAnnouncement: dataAnnouncement, DetailVendor: detailVendor
			}, function (reply) {
				//console.info("insert"+JSON.stringify(reply));
				UIControlService.unloadLoadingModal();
				if (reply.status === 200) {
					UIControlService.msg_growl("success", "Berhasil Simpan Data Pengumuman");
					sendEmailVendors();
					sendEmail();
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoadingModal();
			});

		}

		function sendEmail() {
			var email = {
				subject: 'Pengumuman ' + vm.TenderName,
				mailContent: vm.Description + '<br />' + '<label>Tender code : ' + vm.DataTender.TenderCode + '</label>',
				isHtml: true,
				addresses: vm.sendEmail
			};

			UIControlService.loadLoadingModal("Loading");
			PengumumanPengadaanService.sendEmailToVendor(email, function (response) {
				UIControlService.unloadLoadingModal();
				if (response.status == 200) {
					UIControlService.msg_growl("notice", "Berhasil Kirim Email");
					$uibModalInstance.close();
				} else {
					UIControlService.msg_growl("error", "Gagal Kirim Email, Ada email tidak sesuai format");
					return;
				}
				//$state.go('daftar_kuesioner');
			}, function (response) {
				UIControlService.msg_growl("error", "Gagal Akses API");
				UIControlService.unloadLoadingModal();
				//$state.go('daftar_kuesioner');
			});
		}

		function sendEmailVendors() {
			var email = {
				subject: 'Pengumuman ' + vm.TenderName,
				mailContent: vm.Description,
				isHtml: true,
				Addresses: vm.DataTender.filteredEmails
			};

			UIControlService.loadLoadingModal("Loading");
			PengumumanPengadaanService.sendEmailToVendors(email, function (response) {
				UIControlService.unloadLoadingModal();
				if (response.status == 200) {
					//UIControlService.msg_growl("notice", "Berhasil Kirim Email");
					$uibModalInstance.close();
				} else {
					//UIControlService.msg_growl("error", "Gagal Kirim Email, Ada email tidak sesuai format");
					return;
				}
				//$state.go('daftar_kuesioner');
			}, function (response) {
				UIControlService.msg_growl("error", "Gagal Akses API");
				UIControlService.unloadLoadingModal();
				//$state.go('daftar_kuesioner');
			});
		}

		vm.batal = batal;
		function batal() {
			console.info("batal");
			$uibModalInstance.dismiss('cancel');
		};
	}
})();