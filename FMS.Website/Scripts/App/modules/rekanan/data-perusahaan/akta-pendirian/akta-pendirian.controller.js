(function () {
	'use strict';

	angular.module("app").controller("aktaPendirianCtrl", ctrl);

	ctrl.$inject = ['$http', '$uibModal', '$filter', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'AktaPendirianService', 'CommonEngineService', 'UploaderService', 'UIControlService', 'GlobalConstantService', 'UploadFileConfigService'];
	/* @ngInject */
	function ctrl($http, $uibModal, $filter, $translate, $translatePartialLoader, $location, SocketService, AktaPendirianService, CommonEngineService, UploaderService, UIControlService, GlobalConstantService, UploadFileConfigService) {

		var vm = this;
		vm.folderFile = GlobalConstantService.getConstant('api') + "/";

		vm.bisaMengubahData;
		vm.data = {};
		vm.file1;
		vm.file2;
		vm.file3;
		vm.files1 = [];
		vm.files2 = [];
		vm.files3 = [];
		vm.stocks = [];
		vm.listKotaKab = [];
		vm.isEditPendirian = false; // isEditPendirian_1
		vm.isEditPerubahan = false; // isEditPerubahan_1
		vm.isEditKemenkumham = false; // isEditKemenkumham_1
		vm.isCalendarOpened = [false, false, false];
		vm.url_result = "";
		vm.id_page_config = 3;
		vm.isChangeData = false;
		var loadmsg = "MESSAGE.LOADING";
		vm.isApprovedCRStock = false;

		vm.init = init;
		function init() {
			$translatePartialLoader.addPart('akta-pendirian');
			UIControlService.loadLoading(loadmsg);
			AktaPendirianService.GetDetailVendor(function (reply) {
				if (reply.status === 200) {
					console.info("ven: " + JSON.stringify(reply.data));
					if (!(reply.data === null)) {
						vm.vendorName = reply.data.Name;
						vm.vendorID = reply.data.VendorID;
						vm.businessID = reply.data.BusinessID;
						vm.businessName = reply.data.BusinessName;
						vm.vendorNpwp = reply.data.Npwp;
						if (reply.data.AddressInfo !== null) {
							vm.vendorAddress = reply.data.AddressInfo + ", " + reply.data.AddressDetail;
						}
						else {
							vm.vendorAddress = reply.data.AddressDetail;
						}
						vm.init_NoAuth();
					}
				} else {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_USER');
				}
			}, function (error) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_USER');
			});

			chekcIsVerified();
			loadCheckCRStock();
		};

		function chekcIsVerified() {
			AktaPendirianService.isVerified(function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.data;
					if (!(data.Isverified === null)) {
						vm.isChangeData = true;
					}
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}

		function loadCheckCRStock() {
			UIControlService.loadLoading("Silahkan Tunggu");
			AktaPendirianService.getCRbyVendor({ CRName: 'OC_VENDORSTOCK' }, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					if (reply.data === true) {
						vm.isApprovedCRStock = true;
					} else {
						if (reply.data === false) {
							vm.isApprovedCRStock = false;
						}
					}
				}

			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}

		vm.init_NoAuth = init_NoAuth;
		function init_NoAuth() {
			vm.bisaMengubahData;
			vm.file1 = null;
			vm.file2 = null;
			vm.file3 = null;
			vm.files1 = [];
			vm.files2 = [];
			vm.files3 = [];

			//TODO : cek permintaan ubah data
			vm.bisaMengubahData = true;
			/*
            $http.post($rootScope.url_api + "rekanan/cekBisaMengubahData", {
                rekananId: [$rootScope.rekananid]
            }).success(function (reply) {
                if (reply.status === 200) {
                    vm.data = reply.data;
                    vm.bisaMengubahData = data[0].bisa_mengubah_data == "1";
                    UIControlService.unloadLoading();
                } else {
                    UIControlService.msg_growl.error({ message: "Gagal mendapatkan Hak Bisa Mengubah Data!!" });
                    UIControlService.unloadLoading();
                    return;
                }
                UIControlService.unloadLoading();
            }).error(function (err) {
                UIControlService.msg_growl.error({ message: "Gagal Akses API >" + err });
                UIControlService.unloadLoading();
                return;
            });
            */

			AktaPendirianService.GetByVendor({
				VendorID: vm.vendorID
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					//console.info(">>"+JSON.stringify(reply));
					if (vm.businessID == 3 || vm.businessID == 4) {
						//$rootScope.insertStatusIsiData($rootScope.rekananid, 'ap', 1);
					}
					if (reply.data.length > 0) {
						for (var i = 0; i < reply.data.length; i++) {
							if (reply.data[i].DocumentType === 'LEGAL_DOC_PENDIRIAN') {
								vm.files1.push(reply.data[i]);
							} else if (reply.data[i].DocumentType === 'LEGAL_DOC_PERUBAHAN') {
								vm.files2.push(reply.data[i]);
							} else if (reply.data[i].DocumentType === 'LEGAL_DOC_PENGESAHAN') {
								vm.files3.push(reply.data[i]);
							}
							reply.data[i].DocumentDateConverted = UIControlService.convertDate(reply.data[i].DocumentDate);
							reply.data[i].FilesizeKB = reply.data[i].Filesize / 1024;
							reply.data[i].FilesizeKB = reply.data[i].FilesizeKB.toFixed(1);
						}
					}
					vm.data = {};
					if (!(vm.businessID == 3 || vm.businessID == 4)) {
						//$rootScope.insertStatusIsiData($rootScope.rekananid, 'ap', vm.files1.length > 0 ? 1 : 0);
					}

					getStock();
				} else {
					UIControlService.msg_growl('error', 'MESSAGE.ERR_LOAD');
					UIControlService.unloadLoading();
				}
			}, function (err) {
				UIControlService.msg_growl('error', 'MESSAGE.ERR_LOAD');
				UIControlService.unloadLoading();
			});

			AktaPendirianService.GetCities(function (reply) {
				if (reply.status === 200) {
					vm.listKotaKab = reply.data;
				} else {
					UIControlService.msg_growl('error', 'MESSAGE.ERR_LOAD_CITIES');
				}
			}, function (err) {
				UIControlService.msg_growl('error', 'MESSAGE.ERR_LOAD_CITIES');
			});

			UploadFileConfigService.getByPageName("PAGE.VENDOR.LEGALDOCS", function (response) {
				if (response.status == 200) {
					vm.idUploadConfigs = response.data;
					vm.idFileTypes = generateFilterStrings(response.data);
					vm.idFileSize = vm.idUploadConfigs[0];
				} else {
					UIControlService.msg_growl("error", "MESSAGE.ERR_FILETYPE");
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.ERR_FILETYPE");
			});
		} // end init_NoAuth

		function generateFilterStrings(allowedTypes) {
			var filetypes = "";
			for (var i = 0; i < allowedTypes.length; i++) {
				filetypes += "." + allowedTypes[i].Name + ",";
			}
			return filetypes.substring(0, filetypes.length - 1);
		}

		function getStock() {
			UIControlService.loadLoading(loadmsg);
			AktaPendirianService.GetVendorStocks({
				VendorID: vm.vendorID
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.stocks = reply.data;
					vm.stocks.forEach(function (s) {
						s.OwnerDOBConverted = UIControlService.convertDate(s.OwnerDOB);
					});
				} else {
					UIControlService.msg_growl('error', 'MESSAGE.ERR_LOAD_STOCKS');
				}
			}, function (error) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl('error', 'MESSAGE.ERR_LOAD_STOCKS');
			});
		};

		vm.getDoc = getDoc;
		function getDoc() {
			vm.files1 = [];
			vm.files2 = [];
			vm.files3 = [];
			AktaPendirianService.GetByVendor({
				VendorID: vm.vendorID
			}, function (reply) {
				if (reply.status === 200) {
					if (reply.data[0] === undefined) {
						vm.data = {};
						UIControlService.unloadLoading();
					} else {
						if (reply.data.length > 0) {
							for (var i = 0; i < reply.data.length; i++) {
								if (reply.data[i].DocumentType === 'LEGAL_DOC_PENDIRIAN') {
									vm.files1.push(reply.data[i]);
								} else if (reply.data[i].DocumentType === 'LEGAL_DOC_PERUBAHAN') {
									vm.files2.push(reply.data[i]);
								} else if (reply.data[i].DocumentType === 'LEGAL_DOC_PENGESAHAN') {
									vm.files3.push(reply.data[i]);
								}
								reply.data[i].DocumentDateConverted = UIControlService.convertDate(reply.data[i].DocumentDate);
								reply.data[i].FilesizeKB = reply.data[i].Filesize / 1024;
								reply.data[i].FilesizeKB = reply.data[i].FilesizeKB.toFixed(1);
							}
						}
						vm.data = {};
						UIControlService.unloadLoading();
					}
					if (!(vm.businessID == 3 || vm.businessID == 4)) {
						//$rootScope.insertStatusIsiData($rootScope.rekananid, 'ap', vm.files1.length > 0 ? 1 : 0);
					}
				} else {
					UIControlService.unloadLoading();
				}
			});
		}

		vm.openCalendar = openCalendar;
		function openCalendar(index) {
			vm.isCalendarOpened[index] = true;
		}

		vm.file1Change = file1Change;
		function file1Change(fileUpload) {
			vm.file1 = fileUpload;
		};

		vm.file2Change = file2Change;
		function file2Change(fileUpload) {
			vm.file2 = fileUpload;
		};

		vm.file3Change = file3Change;
		function file3Change(fileUpload) {
			vm.file3 = fileUpload;
		};

		vm.viewLegalStock = viewLegalStock;
		function viewLegalStock(obj) {
			var lempar = {
				stocks: obj.VendorStocks,
				documentNo: obj.DocumentNo
			};
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/rekanan/data-perusahaan/akta-pendirian/view-akta-saham.modal.html',
				controller: 'viewAktaSahamCtrl',
				controllerAs: 'viewAktaSahamCtrl',
				resolve: {
					item: function () {
						return lempar;
					}
				}
			});
		}

		vm.editLegalStock = editLegalStock;
		function editLegalStock(obj) {
			var lempar = {
				stocks: obj.VendorStocks,
				documentNo: obj.DocumentNo,
				allStocks: vm.stocks,
				legalDocId: obj.ID
			};
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/rekanan/data-perusahaan/akta-pendirian/ubah-akta-saham.modal.html',
				controller: 'ubahAktaSahamCtrl',
				controllerAs: 'ubahAktaSahamCtrl',
				resolve: {
					item: function () {
						return lempar;
					}
				}
			});
			modalInstance.result.then(function () {
				getDoc();
			});
		}

		vm.removeFile = removeFile;
		function removeFile(obj, code) {
			vm.idx = -1;
			bootbox.confirm('<h3 class="afta-font center-block">' + $filter('translate')('MESSAGE.CONFIRM_DEL') + '<h3>', function (reply) {
				if (reply) {
					UIControlService.loadLoading(loadmsg);
					AktaPendirianService.Delete({
						ID: obj.ID,
						VendorID: obj.VendorID
					}, function (reply2) {
						UIControlService.unloadLoading();
						if (reply2.status === 200) {
							UIControlService.msg_growl('notice', 'MESSAGE.SUCC_DEL');
							getDoc();
						} else
							UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
					}, function (error) {
						UIControlService.unloadLoading();
						UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
					});
				}
			}); // end bootbox
		}

		vm.removeFile1 = removeFile1;
		function removeFile1(obj) {
			removeFile(obj, 1);
		};

		vm.removeFile2 = removeFile2;
		function removeFile2(obj) {
			removeFile(obj, 2);
		};

		vm.removeFile3 = removeFile3;
		function removeFile3(obj) {
			removeFile(obj, 3);
		};

		vm.editDocument = editDocument;
		function editDocument(id, obj) {
			vm.resetDoc1_update();
			vm.resetDoc2_update();
			vm.resetDoc3_update();

			$.each(vm.files1, function (index, item) {
				item.isEdit = false;
			});
			$.each(vm.files2, function (index, item) {
				item.isEdit = false;
			});
			$.each(vm.files3, function (index, item) {
				item.isEdit = false;
			});
			obj.isEdit = true;

			if (id === 1) {
				vm.data.no_aktapendirian = obj.DocumentNo;
				vm.data.tgl_suratpendirian = new Date(Date.parse(obj.DocumentDate));
				vm.data.notaris_pendirian = obj.NotaryName;
				vm.data.tempat_notaris = obj.NotaryLocation;
				vm.data.dokumen_akta_id1 = obj.ID;
				vm.fName1 = obj.Filename;
				vm.fSize1 = obj.Filesize;
				vm.fUrl1 = obj.DocumentURL;
				vm.file1 = null;
				vm.isEditPendirian = true; // isEditPendirian_2

				document.getElementById("nomorAktaPendirian").focus();
			} else if (id === 2) {
				vm.data.no_perubahanakhir = obj.DocumentNo;
				vm.data.tgl_suratperubahan = new Date(Date.parse(obj.DocumentDate));
				vm.data.notaris_perubahan = obj.NotaryName;
				vm.data.dokumen_akta_id2 = obj.ID;
				vm.fName2 = obj.Filename;
				vm.fSize2 = obj.Filesize;
				vm.fUrl2 = obj.DocumentURL;
				vm.file2 = null;
				vm.isEditPerubahan = true; // isEditPerubahan_2                

				document.getElementById("nomorAktaPerubahan").focus();
			} else {
				vm.data.no_kemenkumham = obj.DocumentNo;
				vm.data.tgl_kemenkumham = new Date(Date.parse(obj.DocumentDate));
				vm.data.dokumen_akta_id3 = obj.ID;
				vm.fName3 = obj.Filename;
				vm.fSize3 = obj.Filesize;
				vm.fUrl3 = obj.DocumentURL;
				vm.file3 = null;
				vm.isEditKemenkumham = true; // isEditKemenkumham_2

				document.getElementById("nomorAktaPengesahan").focus();
			}
		}; // end editDocument

		vm.cancelUpdate = cancelUpdate;
		function cancelUpdate(id) {
			vm.data = {};
			if (id === 1) {
				vm.fName1 = '';
				vm.fSize1 = '';
				vm.fUrl1 = '';
				vm.isEditPendirian = false; // isEditPendirian_3
			} else if (id === 2) {
				vm.fName2 = '';
				vm.fSize2 = '';
				vm.fUrl2 = '';
				vm.isEditPerubahan = false; // isEditPerubahan_3
			} else {
				vm.fName3 = '';
				vm.fSize3 = '';
				vm.fUrl3 = '';
				vm.isEditKemenkumham = false; // isEditKemenkumham_3
			}
			getDoc();
		};

		vm.resetDoc1 = resetDoc1;
		function resetDoc1() {
			vm.file1 = null;
			vm.data.no_aktapendirian = '';
			vm.data.tgl_suratpendirian = '';
			vm.data.notaris_pendirian = '';
			vm.data.tempat_notaris = '';
		}

		vm.resetDoc1_update = resetDoc1_update;
		function resetDoc1_update() {
			resetDoc1();
			vm.data.dokumen_akta_id1 = '';
			vm.isEditPendirian = false;
		}

		vm.resetDoc2 = resetDoc2;
		function resetDoc2() {
			vm.file2 = null;
			vm.data.no_perubahanakhir = '';
			vm.data.tgl_suratperubahan = '';
			vm.data.notaris_perubahan = '';
		}

		vm.resetDoc2_update = resetDoc2_update;
		function resetDoc2_update() {
			resetDoc2();
			vm.data.dokumen_akta_id2 = '';
			vm.isEditPerubahan = false;
		}

		vm.resetDoc3 = resetDoc3;
		function resetDoc3() {
			vm.file3 = null;
			vm.data.no_kemenkumham = '';
			vm.data.tgl_kemenkumham = '';

		}

		vm.resetDoc3_update = resetDoc3_update;
		function resetDoc3_update() {
			resetDoc3();
			vm.data.dokumen_akta_id3 = '';
			vm.isEditKemenkumham = false;
		}

		vm.updateDoc = updateDoc;
		function updateDoc(code, fileName, fileSize, docUrl, docType, docNo, docDate, notary, notaryPlace, aktaId) {
			UIControlService.loadLoading(loadmsg);
			AktaPendirianService.Edit({
				Filename: fileName,
				Filesize: fileSize,
				DocumentURL: docUrl,
				DocumentType: docType,
				VendorID: vm.vendorID,
				DocumentNo: docNo,
				DocumentDate: docDate,
				NotaryName: notary,
				NotaryLocation: notaryPlace,
				ID: aktaId
			}, function (reply) {
				if (reply.status === 200) {
					UIControlService.msg_growl("notice", "MESSAGE.SUCC_SAVE");
					if (code === 1)
						resetDoc1_update();
					else if (code === 2)
						resetDoc2_update();
					else if (code === 3)
						resetDoc3_update();
					vm.getDoc();
				} else {
					UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
				}
				UIControlService.unloadLoading();
			}, function (error) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
			});
		} // end updateDoc

		vm.insertDoc = insertDoc;
		function insertDoc(code, fileName, fileSize, docUrl, docType, docNo, docDate, notary, notaryPlace) {
			UIControlService.loadLoading(loadmsg);
			AktaPendirianService.Create({
				Filename: fileName,
				Filesize: fileSize,
				DocumentURL: docUrl,
				DocumentType: docType,
				VendorID: vm.vendorID,
				DocumentNo: docNo,
				DocumentDate: docDate,
				NotaryName: notary,
				NotaryLocation: notaryPlace,
			}, function (reply) {
				if (reply.status === 200) {
					UIControlService.msg_growl("notice", "MESSAGE.SUCC_SAVE");
					window.location.reload();
					if (code === 1)
						resetDoc1();
					else if (code === 2)
						resetDoc2();
					else if (code === 3)
						resetDoc3();
					vm.getDoc();
				} else {
					UIControlService.msg_growl("error", "MESSAGE.ERR_SAVE");
				}
				UIControlService.unloadLoading();
			}, function (error) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", "MESSAGE." + (error[0] ? error[0] : 'ERR_SAVE'));
			});
		};

		vm.addStock = addStock;
		function addStock() {
			var lempar = {
				stock: {
					VendorID: vm.vendorID,
					Npwp: vm.vendorNpwp
				}
			};
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/rekanan/data-perusahaan/akta-pendirian/kepemilikan-saham.modal.html',
				controller: 'formKepemilikanSahamCtrl',
				controllerAs: 'sahamCtrl',
				resolve: { item: function () { return lempar; } }
			});
			modalInstance.result.then(function () {
				window.location.reload();
			});
		};

		vm.editStock = editStock;
		function editStock(dt) {
			var lempar = {
				stock: {
					StockID: dt.StockID,
					VendorID: vm.vendorID,
					OwnerName: dt.OwnerName,
					OwnerID: Number(dt.OwnerID),
					OwnerIDUrl: dt.OwnerIDUrl,
					OwnerDOBDate: new Date(Date.parse(dt.OwnerDOB)),
					Quantity: dt.Quantity,
					UnitID: dt.UnitID,
					UnitCurrencyID: dt.UnitCurrencyID,
					Npwp: vm.vendorNpwp,
					Position: dt.Position
				}
			};
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/rekanan/data-perusahaan/akta-pendirian/kepemilikan-saham.modal.html',
				controller: 'formKepemilikanSahamCtrl',
				controllerAs: 'sahamCtrl',
				resolve: {
					item: function () {
						return lempar;
					}
				}
			});
			modalInstance.result.then(function () {
				getStock();
			});
		};

		vm.removeStock = removeStock;
		function removeStock(dt) {
			bootbox.confirm('<h3 class="afta-font center-block">' + $filter('translate')('MESSAGE.CONFIRM_DEL_VSTOCK') + '<h3>', function (reply) {
				if (reply) {
					UIControlService.loadLoading(loadmsg);
					AktaPendirianService.DeleteVendorStock({
						StockID: dt.StockID
					}, function (reply2) {
						UIControlService.unloadLoading();
						if (reply2.status === 200) {
							UIControlService.msg_growl('notice', 'MESSAGE.SUCC_DEL_VSTOCK');
							getStock();
							getDoc();
						} else
							UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL_VSTOCK');
					}, function (error) {
						UIControlService.unloadLoading();
						UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL_VSTOCK');
					});
				}
			});
		}

		vm.addDocument = addDocument;
		function addDocument(code) {
			if (validateField(code) === false) {
				UIControlService.msg_growl("error", "MESSAGE.ERR_INCOMPLETE_FIELD");
				return;
			}
			if (validateUploadField(code) === false) {
				UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
				return;
			}
			if (code === 1 && vm.file1 || code === 2 && vm.file2 || code === 3 && vm.file3) {
				if (code === 1) {
					uploadFile(code, vm.file1);
				} else if (code === 2) {
					uploadFile(code, vm.file2);
				} else if (code === 3) {
					uploadFile(code, vm.file3);
				}
			} else {
				saveLegalDoc(code, '', '', '');
			}
		};

		function uploadFile(code, file) {
			if (validateFileType(file, vm.idUploadConfigs)) {
				upload(code, file, vm.idFileSize, vm.idFileTypes);
			}
		}

		function validateFileType(file, idUploadConfigs) {
			if (!file || file.length == 0) {
				UIControlService.msg_growl("error", "MESSAGE.ERR_NOFILE");
				return false;
			}
			return true;
		}

		function upload(code, file, config, types) {
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
			UploaderService.uploadSingleFileLegalDocuments(vm.vendorID, file, size, types,
            function (reply) {
            	if (reply.status == 200) {
            		UIControlService.unloadLoadingModal();
            		var url = reply.data.Url;
            		var size = reply.data.FileLength;
            		var name = reply.data.FileName;
            		saveLegalDoc(code, url, size, name);
            	} else {
            		UIControlService.unloadLoadingModal();
            		UIControlService.msg_growl("error", 'MESSAGE.ERR_UPLOAD');
            	}
            }, function (err) {
            	UIControlService.unloadLoadingModal();
            	UIControlService.msg_growl("error", "MESSAGE.ERR_UPLOAD");
            });
		};

		function saveLegalDoc(code, url, size, name) {
			if (code === 1) {
				vm.fUrl1 = url ? url : vm.fUrl1;
				vm.fSize1 = size ? size : vm.fSize1;
				vm.fName1 = name ? name : vm.fName1;
				if (vm.isEditPendirian) {
					updateDoc(1, vm.fName1, vm.fSize1, vm.fUrl1, 'LEGAL_DOC_PENDIRIAN', vm.data.no_aktapendirian, UIControlService.getStrDate(vm.data.tgl_suratpendirian), vm.data.notaris_pendirian, vm.data.tempat_notaris.kota_nama ? vm.data.tempat_notaris.kota_nama : vm.data.tempat_notaris, vm.data.dokumen_akta_id1);
				} else {
					insertDoc(1, vm.fName1, vm.fSize1, vm.fUrl1, 'LEGAL_DOC_PENDIRIAN', vm.data.no_aktapendirian, UIControlService.getStrDate(vm.data.tgl_suratpendirian), vm.data.notaris_pendirian, vm.data.tempat_notaris.kota_nama ? vm.data.tempat_notaris.kota_nama : vm.data.tempat_notaris);
				}
			} else if (code === 2) {
				vm.fUrl2 = url ? url : vm.fUrl2;
				vm.fSize2 = size ? size : vm.fSize2;
				vm.fName2 = name ? name : vm.fName2;
				if (vm.isEditPerubahan) {
					updateDoc(2, vm.fName2, vm.fSize2, vm.fUrl2, 'LEGAL_DOC_PERUBAHAN', vm.data.no_perubahanakhir, UIControlService.getStrDate(vm.data.tgl_suratperubahan), vm.data.notaris_perubahan, '', vm.data.dokumen_akta_id2);
				} else {
					insertDoc(2, vm.fName2, vm.fSize2, vm.fUrl2, 'LEGAL_DOC_PERUBAHAN', vm.data.no_perubahanakhir, UIControlService.getStrDate(vm.data.tgl_suratperubahan), vm.data.notaris_perubahan, '');
				}
			} else if (code === 3) {
				vm.fUrl3 = url ? url : vm.fUrl3;
				vm.fSize3 = size ? size : vm.fSize3;
				vm.fName3 = name ? name : vm.fName3;
				if (vm.isEditKemenkumham) {
					updateDoc(3, vm.fName3, vm.fSize3, vm.fUrl3, 'LEGAL_DOC_PENGESAHAN', vm.data.no_kemenkumham, UIControlService.getStrDate(vm.data.tgl_kemenkumham), '', '', vm.data.dokumen_akta_id3);
				} else {
					insertDoc(3, vm.fName3, vm.fSize3, vm.fUrl3, 'LEGAL_DOC_PENGESAHAN', vm.data.no_kemenkumham, UIControlService.getStrDate(vm.data.tgl_kemenkumham), '', '');
				}
			}
		};

		function validateField(code) {
			if (code === 1) {
				if (!vm.data.no_aktapendirian || !vm.data.tgl_suratpendirian || !vm.data.notaris_pendirian || !vm.data.tempat_notaris) {
					return false;
				}
				return true;
			} else if (code === 2) {
				if (!vm.data.no_perubahanakhir || !vm.data.tgl_suratperubahan || !vm.data.notaris_perubahan) {
					return false;
				}
				return true;
			} else if (code === 3) {
				if (!vm.data.no_kemenkumham || !vm.data.tgl_kemenkumham) {
					return false;
				}
				return true;
			}
		};

		function validateUploadField(code) {
			if (code === 1) {
				if (!vm.fUrl1 && !vm.file1) {
					return false;
				}
				return true;
			} else if (code === 2) {
				if (!vm.fUrl2 && !vm.file2) {
					return false;
				}
				return true;
			} else if (code === 3) {
				if (!vm.fUrl3 && !vm.file3) {
					return false;
				}
				return true;
			}
		};

	}
})();