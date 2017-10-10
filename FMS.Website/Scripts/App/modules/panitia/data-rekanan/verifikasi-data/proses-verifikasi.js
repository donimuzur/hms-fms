(function () {
	'use strict';

	angular.module("app").controller("VerifiedProcessCtrl", ctrl);

	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'VerifiedSendService',
        'VerifikasiDataService', 'SrtPernyataanService', 'RoleService', 'UIControlService', '$uibModal', '$stateParams', '$state', 'GlobalConstantService'];
	function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, VerifiedSendService, VerifikasiDataService, SrtPernyataanService,
        RoleService, UIControlService, $uibModal, $stateParams, $state, GlobalConstantService) {

		var vm = this;
		vm.verified = [];
		vm.folderFile = GlobalConstantService.getConstant('api') + "/";
		vm.fullSize = 10;
		vm.id = Number($stateParams.id);
		vm.init = init;
		vm.Vendor = {};
		vm.totalItems = 0;
		vm.currentPage = 1;
		vm.flag = false;
		vm.keyword = '';
		vm.administrasi = [];
		vm.license = [];
		vm.DocType;
		vm.CertificatePengalaman = [];
		vm.CertificatePendidikan = [];
		vm.CertificateSertifikat = [];
		function init() {
			$translatePartialLoader.addPart('verifikasi-data');
			$translatePartialLoader.addPart('pengurus-perusahaan');
			$translatePartialLoader.addPart('akta-pendirian');
			$translatePartialLoader.addPart("data-izinusaha");
			$translatePartialLoader.addPart('data-administrasi');
			$translatePartialLoader.addPart('tenaga-ahli');
			$translatePartialLoader.addPart('surat-pernyataan');

			$translatePartialLoader.addPart('vendor-balance');
			jLoad(1);
			loadlicense();
			loadAkta();
			loadPengurus();
			loadTenagaAhli(1);
			loadBuilding(1);
			loadEquipmentVehicle(1);
			loadEquipmentTools(1);
			loadPengalaman();
			loadSaham();
			loadDokumen();
			loadNeraca();
			loadCommodity();
			otherdoc();
			loadBankDetail();
			loadUrlStatementLetter();
		}

		vm.jLoad = jLoad;
		function jLoad(current) {
			vm.addressBranch = '';
			vm.addressMain = '';
			VerifikasiDataService.select({
				VendorID: vm.id
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.verified = reply.data;
					for (var i = 0; i < vm.verified.length ; i++) {
						if (vm.verified[i].VendorContactType.Value === "VENDOR_CONTACT_TYPE_COMPANY") {
							vm.cityID = vm.verified[i].Contact.Address.State.Country.CountryID;
							vm.State = vm.verified[i].Contact.Address.State.Country.Code;
							if (vm.verified[i].Contact.Address.State.Country.Code === 'IDN') vm.cek = true;
							vm.TenderName = vm.verified[i].Vendor.TenderName;
							if (vm.verified[i].Contact.Fax !== null) {
								vm.ld = vm.verified[i].Contact.Fax.split(' ');
								vm.VendorTypeId = vm.verified[i].Vendor.VendorTypeID;
								console.info(vm.verified[i].Vendor.VendorTypeID);
							}
						}
						if (vm.verified[i].VendorContactType.Value === "VENDOR_OFFICE_TYPE_BRANCH") {
							vm.addressBranch = vm.verified[i].Contact.Address.AddressInfo + vm.verified[i].Contact.Address.AddressDetail; console.info(vm.addressBranch);

						}
						if (vm.verified[i].VendorContactType.Value === "VENDOR_OFFICE_TYPE_MAIN") {
							vm.addressMain = vm.verified[i].Contact.Address.AddressInfo + vm.verified[i].Contact.Address.AddressDetail;
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

		vm.addVerifikasi = addVerifikasi;
		function addVerifikasi(data, emailAddress) {
			if (data === true) {
				bootbox.confirm("Apakah anda yakin telah melakukan verifikasi data vendor tersebut?", function (res) {
					if (res) {
						simpan(data);
						sendMail('Kami telah berhasil melakukan verifikasi terhadap data perusahaan anda. Selanjutnya anda bisa mengikuti pengadaan yang kami selenggarakan di aplikasi eprocurement. Terimakasih', emailAddress);
						SocketService.emit("daftarRekanan");
					}
				});
			} else if (data === false) {
				bootbox.confirm("Apakah anda yakin ingin menolak hasil verifikasi data vendor tersebut?", function (res) {
					if (res) {
						simpan(data);
						sendMail('Kami telah melakukan verifikasi terhadap data perusahaan anda. Kami menemukan beberapa data yang tidak sesuai. Mohon segera dilakukan perbaikan dan dikirim kembalik untuk verifikasi. Terimakasih', emailAddress);
						SocketService.emit("daftarRekanan");
					}
				});
			}
		}

		function sendMail(mailContent, emailAddress) {
			var email = {
				subject: 'Vendor Verification Notification',
				mailContent: mailContent,
				isHtml: false,
				addresses: [emailAddress]
			};

			UIControlService.loadLoading("LOADERS.LOADING_SEND_EMAIL");
			VerifiedSendService.sendMail(email, function (response) {
				UIControlService.unloadLoading();
				if (response.status === 200) {
					UIControlService.msg_growl("notice", "Email Sent!")
				} else {
					UIControlService.handleRequestError(response.data);
				}
			}, function (response) {
				UIControlService.handleRequestError(response.data);
				UIControlService.unloadLoading();
				//$state.go('daftar_kuesioner');
			});
		}

		vm.simpan = simpan;
		function simpan(data) {

			if (data === true) {
				vm.Vendor = {
					VendorID: vm.id,
					Isverified: 1
				}
			} else if (data === false) {
				vm.Vendor = {
					VendorID: vm.id,
					Isverified: 0
				}
			}
			UIControlService.loadLoading("Silahkan Tunggu");
			//console.info("ada:"+JSON.stringify(data))
			VerifiedSendService.updateVerifikasi(vm.Vendor, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var msg = "";
					if (data === false) msg = " Tolak Verifikasi ";
					if (data === true) msg = "Verifikasi ";
					UIControlService.msg_growl("success", "Data Berhasil di " + msg);
					$state.transitionTo('verifikasi-data');
				} else {
					UIControlService.msg_growl("error", "Gagal memverifikasi data ");
					return;
				}
			}, function (err) {
				UIControlService.msg_growl("error", "Gagal Akses API ");
				UIControlService.unloadLoading();
			});
		}


		vm.loadUrlStatementLetter = loadUrlStatementLetter;
		function loadUrlStatementLetter() {
		    if (localStorage.getItem("currLang") === 'ID') {
		        vm.DocType = 4225;
		    }
		    else {
		        vm.DocType = 4232;
		    }
		    UIControlService.loadLoading("Silahkan Tunggu...");
		    VerifikasiDataService.DocConduct({
		        DocType: vm.DocType,
                VendorId: vm.id
		    }, function (reply) {
		        UIControlService.unloadLoading();
		        if (reply.status === 200) {
		            var data = reply.data;
		            vm.urlBusinessConduct = data[0].DocumentUrl;
		            loadAggreement();

		        } else {
		            $.growl.error({ message: "Gagal mendapatkan dokumen" });
		            UIControlService.unloadLoading();
		        }
		    }, function (err) {
		        console.info("error:" + JSON.stringify(err));
		        //$.growl.error({ message: "Gagal Akses API >" + err });
		        UIControlService.unloadLoading();
		    });
		}

		vm.loadAggreement = loadAggreement;
		function loadAggreement() {
		    UIControlService.loadLoading("Silahkan Tunggu...");
		    VerifikasiDataService.DocConduct({
		        DocType: 4226,
                VendorId: vm.id
		    }, function (reply) {
		        UIControlService.unloadLoading();
		        if (reply.status === 200) {
		            var data = reply.data;
		            vm.urlAggrement = data[0].DocumentUrl;

		        } else {
		            $.growl.error({ message: "Gagal mendapatkan dokumen" });
		            UIControlService.unloadLoading();
		        }
		    }, function (err) {
		        console.info("error:" + JSON.stringify(err));
		        //$.growl.error({ message: "Gagal Akses API >" + err });
		        UIControlService.unloadLoading();
		    });
		}

		vm.loadlicense = loadlicense;
		function loadlicense() {
			vm.currentPage = 1;
			var offset = (1 * 10) - 10;
			VerifikasiDataService.selectlicensi({
				Status: vm.id,
				Offset: offset,
				Limit: vm.fullSize
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.license = reply.data;
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

		vm.detail = detail;
		function detail(flag, datas) {
			console.info("masuk form add/edit");
			var data = {
				flag: flag,
				item: datas
			}
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/data-rekanan/verifikasi-data/modallicense.html',
				controller: 'ModalLicenseCtrl',
				controllerAs: 'ModalLicenseCtrl',
				resolve: {
					item: function () {
						return data;
					}
				}
			});
			modalInstance.result.then(function () {
				vm.jLoad(1);
			});
		}

		vm.loadAkta = loadAkta;
		function loadAkta() {
			VerifikasiDataService.GetByVendor({
				VendorID: vm.id
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.Akta = reply.data;
					vm.countPendirian = 0;
					vm.countPerubahan = 0;
					vm.countPengesahan = 0;
					for (var i = 0; i < vm.Akta.length; i++) {
						if (vm.Akta[i].DocumentType === 'LEGAL_DOC_PENDIRIAN') {
							vm.countPendirian = 1;
						} else if (vm.Akta[i].DocumentType === 'LEGAL_DOC_PERUBAHAN') {
							vm.countPerubahan = 1;
						} else if (vm.Akta[i].DocumentType === 'LEGAL_DOC_PENGESAHAN') {
							vm.countPengesahan = 1;
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

		vm.loadPengurus = loadPengurus;
		function loadPengurus() {
			VerifikasiDataService.GetByVendorComPer({
				VendorID: vm.id
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.compPersons = reply.data;
				} else {
					UIControlService.msg_growl('error', 'MESSAGE.ERR_LOAD');
					UIControlService.unloadLoading();
				}
			}, function (err) {
				UIControlService.msg_growl('error', 'MESSAGE.ERR_LOAD');
				UIControlService.unloadLoading();
			});
		}

		vm.loadTenagaAhli = loadTenagaAhli;
		function loadTenagaAhli(current) {
			vm.vendorexperts = [];
			vm.currentPage = current;
			var offset = (current * 10) - 10;
			VerifikasiDataService.allTenagaahli({
				Status: vm.id,
				Offset: offset,
				Limit: vm.fullSize,
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.vendorexperts = reply.data.List;
					//loadCertificate(vm.vendorexperts);
				} else {
					$.growl.error({ message: "Gagal mendapatkan data Tenaga Ahli Perusahaan" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				console.info("error:" + JSON.stringify(err));
				//$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}

		vm.loadCertificate = loadCertificate;
		function loadCertificate(data) {
			var offset = (1 * 10) - 10;
			VerifikasiDataService.selectCertificate({
				Offset: offset,
				Limit: vm.fullSize,
				Status: data.ID
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.vendorexpertsCertificate = reply.data;
					for (var i = 0; i < vm.vendorexpertsCertificate.length; i++) {
						if (vm.vendorexpertsCertificate[i].SysReference.RefID === 3128 && vm.vendorexpertsCertificate[i].IsActive === true) {
							vm.CertificatePengalaman.push(vm.vendorexpertsCertificate[i]);
						} else if (vm.vendorexpertsCertificate[i].SysReference.RefID === 3129 && vm.vendorexpertsCertificate[i].IsActive === true) {
							vm.CertificatePendidikan.push(vm.vendorexpertsCertificate[i]);
						} else if (vm.vendorexpertsCertificate[i].SysReference.RefID === 3130 && vm.vendorexpertsCertificate[i].IsActive === true) {
							vm.CertificateSertifikat.push(vm.vendorexpertsCertificate[i]);
						}
					}
				} else {
					$.growl.error({ message: "Gagal mendapatkan data Tenaga Ahli Perusahaan" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				console.info("error:" + JSON.stringify(err));
				//$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}

		vm.listBuilding = [];
		function loadBuilding(current) {
			var offset = (current * vm.fullSize) - vm.fullSize;
			UIControlService.loadLoading(vm.msgLoading);
			VerifikasiDataService.selectBuilding({
				Status: vm.id, Ofsset: offset, Limit: vm.fullSize
			}, function (reply) {
				UIControlService.unloadLoading();
				vm.listBuilding = reply.data.List;
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}

		vm.listVehicle = [];
		function loadEquipmentVehicle(current) {
			var offset = (current * vm.fullSize) - vm.fullSize;
			UIControlService.loadLoading(vm.msgLoading);
			VerifikasiDataService.selectVehicle({
				Status: vm.id, Ofsset: offset, Limit: vm.fullSize
			}, function (reply) {
				//console.info(">>"+JSON.stringify(reply));
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
		function loadEquipmentTools(current) {
			//console.info("mlebu");
			var offset = (current * vm.fullSize) - vm.fullSize;
			UIControlService.loadLoading(vm.msgLoading);
			VerifikasiDataService.selectEquipment({
				Status: vm.id, Ofsset: offset, Limit: vm.fullSize
			}, function (reply) {
				//console.info(">>>"+JSON.stringify(reply));
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

		vm.viewDetail = viewDetail;
		function viewDetail(data) {
			vm.namavendor = data.Name;
			vm.flag = true;
			vm.CertificatePengalaman = [];
			vm.CertificatePendidikan = [];
			vm.CertificateSertifikat = [];
			loadCertificate(data);
			vm.dataExpert = data;
		}

		vm.addFlag = addFlag;
		function addFlag() {
			vm.flag = false;
		}

		vm.detailForm = detailForm;
		function detailForm(type, datas, isAdd) {
			var data = {
				type: type,
				data: datas,
				isForm: isAdd
			}
			var ctrl;
			var ctrlAs;
			if (type === "building") {
				ctrl = "FormBuildingController";
				ctrlAs = "FormBuildingCtrl";
			} else {
				ctrl = "FormNonBuildingController";
				ctrlAs = "FormNonBuildingCtrl";
			}
			var modalInstance = $uibModal.open({
				templateUrl: "app/modules/panitia/data-rekanan/verifikasi-data/detailData.html",
				controller: ctrl,
				controllerAs: ctrlAs,
				resolve: {
					item: function () {
						return data;
					}
				}
			});
			modalInstance.result.then(function () {
				jLoad(1);
			});
		}

		vm.loadPengalaman = loadPengalaman;
		function loadPengalaman() {
			UIControlService.loadLoading('LOADING.VENDOREXPERIENCE.MESSAGE');
			VerifikasiDataService.selectExperience({
				Offset: (vm.currentPage - 1) * vm.fullSize,
				Limit: vm.fullSize,
				Keyword: vm.keyword,
				column: 1,
				Status: vm.id
			}, function (reply) {
				//console.info(JSON.stringify(reply));
				if (reply.status === 200) {
					vm.listFinishExp = reply.data.List;
					for (var i = 0; i < vm.listFinishExp.length; i++) {
						vm.listFinishExp[i].StartDate = UIControlService.getStrDate(vm.listFinishExp[i].StartDate);
					}
					vm.totalItems = reply.data.Count;
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
			VerifikasiDataService.selectExperience({
				Offset: (vm.currentPage - 1) * vm.fullSize,
				Limit: vm.fullSize,
				Keyword: vm.keyword,
				column: 2,
				Status: vm.id
			}, function (reply) {
				//console.info("current?:"+JSON.stringify(reply));
				if (reply.status === 200) {
					vm.listCurrentExp = reply.data.List;
					for (var i = 0; i < vm.listCurrentExp.length; i++) {
						vm.listCurrentExp[i].StartDate = UIControlService.getStrDate(vm.listCurrentExp[i].StartDate);
					}
					vm.totalItems = reply.data.Count;
					UIControlService.unloadLoading();
				} else {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("error", 'NOTIFICATION.VENDOREXPERIENCE.ERROR', "NOTIFICATION.VENDOREXPERIENCE.TITLE");
				}
			}, function (err) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", 'NOTIFICATION.VENDOREXPERIENCE.ERROR', "NOTIFICATION.VENDOREXPERIENCE.TITLE");
			});

		}

		vm.loadSaham = loadSaham;
		function loadSaham() {
			VerifikasiDataService.selectSaham({
				VendorID: vm.id
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.verifiedSaham = reply.data;
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

		vm.loadDokumen = loadDokumen;
		function loadDokumen() {
			VerifikasiDataService.GetByVendor({
				VendorID: vm.id
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.verifiedDokumen = reply.data;
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

		vm.loadNeraca = loadNeraca;
		function loadNeraca() {
		    loadBalanceUrl();
			vm.asset = 0;
			vm.hutang = 0;
			vm.modal = 0;
			VerifikasiDataService.selectNeraca({
				VendorID: vm.id
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.vendorbalance = reply.data;
					for (var i = 0; i < vm.vendorbalance.length; i++) {
						if (vm.vendorbalance[i].WealthType.Name === "WEALTH_TYPE_ASSET") {
							vm.listAsset = vm.vendorbalance[i];
						}
						if (vm.vendorbalance[i].WealthType.Name === "WEALTH_TYPE_DEBTH") {
							vm.listDebth = vm.vendorbalance[i];
						}
					}
					for (var i = 0; i < vm.vendorbalance.length; i++) {
						for (var j = 0; j < vm.vendorbalance[i].subWealth.length; j++) {
							if (vm.vendorbalance[i].subWealth[j].subCategory.length === 0) {
								if (vm.vendorbalance[i].WealthType.RefID === 3097 && vm.vendorbalance[i].subWealth[j].IsActive === true) {
									if (vm.asset === 0) {
										vm.asset = vm.vendorbalance[i].subWealth[j].nominal;
									} else
										vm.asset = +vm.asset + +vm.vendorbalance[i].subWealth[j].nominal;

								} else if (vm.vendorbalance[i].WealthType.RefID === 3099 && vm.vendorbalance[i].subWealth[j].IsActive === true) {
									if (vm.hutang === 0) {
										vm.hutang = vm.vendorbalance[i].subWealth[j].nominal;
									} else {
										vm.hutang = +vm.hutang + +vm.vendorbalance[i].subWealth[j].nominal;
									}


								}
							}
							for (var k = 0; k < vm.vendorbalance[i].subWealth[j].subCategory.length; k++) {

								if (vm.vendorbalance[i].subWealth[j].subCategory[k].WealthType === 3097 && vm.vendorbalance[i].subWealth[j].subCategory[k].IsActive === true) {
									if (vm.asset === 0) {
										vm.asset = vm.vendorbalance[i].subWealth[j].subCategory[k].Nominal;
									} else
										vm.asset = +vm.asset + +vm.vendorbalance[i].subWealth[j].subCategory[k].Nominal;

								} else if (vm.vendorbalance[i].subWealth[j].subCategory[k].WealthType === 3099 && vm.vendorbalance[i].subWealth[j].subCategory[k].IsActive === true) {
									if (vm.hutang === 0) {
										vm.hutang = vm.vendorbalance[i].subWealth[j].subCategory[k].Nominal;

									} else {
										vm.hutang = +vm.hutang + +vm.vendorbalance[i].subWealth[j].subCategory[k].Nominal;
									}


								}
							}
						}
					}
					vm.modal = +vm.asset - +vm.hutang;
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

		vm.loadBalanceUrl = loadBalanceUrl;
		function loadBalanceUrl() {
		    VerifikasiDataService.balanceDocUrl({VendorID: vm.id},function (reply) {
		        if (reply.status === 200) {
		            vm.balanceDocUrl = reply.data.DocUrl;
		        }
		    }, function (err) {
		        UIControlService.msg_growl("error", "MESSAGE.API");
		    });
		}

		vm.loadCompanyScale = loadCompanyScale;
		function loadCompanyScale(data) {
			VerifikasiDataService.getCompanyScale(function (reply) {
				if (reply.status === 200) {
					vm.listCompanyScale = reply.data.List;

					for (var j = 0; j < data.length; j++) {
						if (data[j].ScaleReff != 0) {
							for (var i = 0; i < vm.listCompanyScale.length; i++) {
								if (vm.listCompanyScale[i].RefID === vm.dataComm[j].ScaleReff) {
									vm.dataComm[j].ScaleReffS = vm.listCompanyScale[i];
								}
							}
						}
					}
				} else {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("error", 'NOTIFICATION.VENDOREXPERIENCE.ERROR', "NOTIFICATION.VENDOREXPERIENCE.TITLE");
				}
			}, function (err) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", 'NOTIFICATION.VENDOREXPERIENCE.ERROR', "NOTIFICATION.VENDOREXPERIENCE.TITLE");
			});
		}

		vm.cancel = cancel;
		function cancel() {
			$state.transitionTo('verifikasi-data');
		}

		vm.changeScale = changeScale;
		function changeScale(data) {
			var dataBalance = {
				ID: data.ID,
				ScaleReffS: data.ScaleReffS
			};
			VerifikasiDataService.UpdateScaleBalance(dataBalance, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					console.info("sukses");
				} else {
					$.growl.error({ message: "Gagal mendapatkan dokumen" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				console.info("error:" + JSON.stringify(err));
				//$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}

		function convertDate(date) {
			return UIControlService.convertDate(date);
		}

		vm.otherdoc = otherdoc;
		function otherdoc() {
			UIControlService.loadLoading("Silahkan Tunggu...");
			VerifikasiDataService.SelectVend({
				VendorID: vm.id
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.data;
					vm.document = data;
					vm.document.forEach(function (cr) {
						cr.ValidDateConverted = convertDate(cr.ValidDate);
					});
				} else {
					$.growl.error({ message: "Gagal mendapatkan dokumen" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				console.info("error:" + JSON.stringify(err));
				//$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}

		vm.openForm = openForm;
		function openForm(data, flag, isForm) {
			if (flag === 6 && isForm != true) {
				var data = {
					item: {
						BalanceID: isForm.BalanceID,
						Wealth: data,
						COA: isForm.COAType,
						Unit: isForm.Unit,
						Amount: isForm.Amount,
						DocUrl: isForm.DocUrl,
						Nominal: isForm.nominal
					}, flag: flag, isForm: true
				}
				var modalInstance = $uibModal.open({
					templateUrl: "app/modules/panitia/data-rekanan/verifikasi-data/form-izin-usaha.html",
					controller: 'FormIzinCtrl',
					controllerAs: 'FormIzinCtrl',
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
			else {
				var data = {
					item: data,
					flag: flag,
					isForm: isForm,
					city: vm.cityID
				}
				var modalInstance = $uibModal.open({
					templateUrl: "app/modules/panitia/data-rekanan/verifikasi-data/form-izin-usaha.html",
					controller: 'FormIzinCtrl',
					controllerAs: 'FormIzinCtrl',
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

		}

		vm.deleteObj = deleteObj;
		function deleteObj(data, flag) {
			vm.listdata = data;
			if (data.IsActive == true) vm.listdata.IsActive = false;
			else vm.listdata.IsActive = true;
			if (flag === 1) {
				VerifikasiDataService.DeleteVendorLicensi({
					LicenseID: vm.listdata.LicenseID,
					VendorID: vm.listdata.VendorID
				}, function (reply) {
					UIControlService.unloadLoading();
					if (reply.status === 200) {
						UIControlService.msg_growl("success", "FORM.MSG_SUC_SAVE");
						init();
						vm.flag = false;
					} else {
						UIControlService.msg_growl("error", "FORM.MSG_ERR_SAVE");
						return;
					}
				}, function (err) {
					UIControlService.msg_growl("error", "MESSAGE.ERR_API");
					UIControlService.unloadLoadingModal();
				});
			} else if (flag === 2) {
				VerifikasiDataService.DeleteVendorStock({
					IsActive: vm.listdata.IsActive,
					StockID: vm.listdata.StockID
				}, function (reply2) {
					UIControlService.unloadLoading();
					if (reply2.status === 200) {
						UIControlService.msg_growl('notice', 'MESSAGE.SUCC_DEL_VSTOCK');
						init();
					} else
						UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL_VSTOCK');
				}, function (error) {
					UIControlService.unloadLoading();
					UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL_VSTOCK');
				});
			} else if (flag === 3) {
				VerifikasiDataService.DeleteVendorLegal({
					IsActive: vm.listdata.IsActive,
					StockID: vm.listdata.ID
				}, function (reply2) {
					UIControlService.unloadLoading();
					if (reply2.status === 200) {
						UIControlService.msg_growl('notice', 'MESSAGE.SUCC_DEL');
						init();
					} else
						UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
				}, function (error) {
					UIControlService.unloadLoading();
					UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
				});
			} else if (flag === 6) {
				VerifikasiDataService.DeleteVendorBalance({
					IsActive: vm.listdata.IsActive,
					BalanceID: vm.listdata.BalanceID
				}, function (reply2) {
					UIControlService.unloadLoading();
					if (reply2.status === 200) {
						UIControlService.msg_growl('notice', 'MESSAGE.SUCC_DEL');
						init();
					} else
						UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
				}, function (error) {
					UIControlService.unloadLoading();
					UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
				});
			} else if (flag === 7) {
				VerifikasiDataService.DeletePengurus({
					ID: vm.listdata.ID
				}, function (reply2) {
					UIControlService.unloadLoading();
					if (reply2.status === 200) {
						UIControlService.msg_growl('notice', 'MESSAGE.SUCC_DEL');
						init();
					} else
						UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
				}, function (error) {
					UIControlService.unloadLoading();
					UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
				});
			} else if (flag === 8) {
				VerifikasiDataService.DeleteVendorExperts({
					IsActive: vm.listdata.IsActive,
					ID: vm.listdata.ID
				}, function (reply2) {
					UIControlService.unloadLoading();
					if (reply2.status === 200) {
						UIControlService.msg_growl('notice', 'MESSAGE.SUCC_DEL');
						init();
					} else
						UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
				}, function (error) {
					UIControlService.unloadLoading();
					UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
				});
			} else if (flag === 9 || flag === 10 || flag === 11) {
				VerifikasiDataService.DeleteVendorExpertCertificate({
					IsActive: vm.listdata.IsActive,
					ID: vm.listdata.ID
				}, function (reply2) {
					UIControlService.unloadLoading();
					if (reply2.status === 200) {
						UIControlService.msg_growl('notice', 'MESSAGE.SUCC_DEL');
						init();
						addFlag();
						viewDetail(vm.dataExpert);

					} else
						UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
				}, function (error) {
					UIControlService.unloadLoading();
					UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
				});
			} else if (flag === 12) {
				VerifikasiDataService.DeleteVendorBuilding({
					IsActive: vm.listdata.IsActive,
					ID: vm.listdata.ID
				}, function (reply2) {
					UIControlService.unloadLoading();
					if (reply2.status === 200) {
						UIControlService.msg_growl('notice', 'MESSAGE.SUCC_DEL');
						init();
					} else
						UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
				}, function (error) {
					UIControlService.unloadLoading();
					UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
				});
			} else if (flag === 13 || flag === 14) {
				VerifikasiDataService.DeleteVendorEquipment({
					IsActive: vm.listdata.IsActive,
					ID: vm.listdata.ID
				}, function (reply2) {
					UIControlService.unloadLoading();
					if (reply2.status === 200) {
						UIControlService.msg_growl('notice', 'MESSAGE.SUCC_DEL');
						init();
					} else
						UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
				}, function (error) {
					UIControlService.unloadLoading();
					UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
				});
			} else if (flag === 16) {
				VerifikasiDataService.DeleteOtherDoc({
					ID: vm.listdata.ID
				}, function (reply2) {
					UIControlService.unloadLoading();
					if (reply2.status === 200) {
						UIControlService.msg_growl('notice', 'MESSAGE.SUCC_DEL');
						init();
					} else
						UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
				}, function (error) {
					UIControlService.unloadLoading();
					UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
				});
			}
		}

		vm.openFormBuilding = openFormBuilding;
		function openFormBuilding(type, data, isAdd) {
			// console.info("cr:" + IsCR);
			var data = {
				type: type,
				data: data,
				isForm: isAdd
			}
			var temp;
			var ctrl;
			var ctrlAs;
			if (type === "building") {
				temp = "app/modules/panitia/data-rekanan/verifikasi-data/formBuilding.html";
				ctrl = "FormBuildingController";
				ctrlAs = "FormBuildingCtrl";
			} else {
				temp = "app/modules/panitia/data-rekanan/verifikasi-data/formNonBuilding.html";
				ctrl = "FormNonBuildingController";
				ctrlAs = "FormNonBuildingCtrl";
			}
			var modalInstance = $uibModal.open({
				templateUrl: temp,
				controller: ctrl,
				controllerAs: ctrlAs,
				resolve: {
					item: function () {
						return data;
					}
				}
			});
			modalInstance.result.then(function () {
				vm.init();
			});
		}

		vm.openFormTenagaAhli = openFormTenagaAhli;
		function openFormTenagaAhli(data, flag) {
			var data = {
				item: data,
				flag: flag
			}
			var modalInstance = $uibModal.open({
				templateUrl: "app/modules/panitia/data-rekanan/verifikasi-data/FormTenagaAhli.html",
				controller: 'formTenagaAhliCtrl',
				controllerAs: 'formTenagaAhliCtrl',
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

		vm.loadCommodity = loadCommodity;
		function loadCommodity() {
			VerifikasiDataService.VendorCommodity({
				VendorID: vm.id
			}, function (reply2) {
				UIControlService.unloadLoading();
				if (reply2.status === 200) {
					vm.dataComm = reply2.data;

					loadCompanyScale(vm.dataComm);

				} else
					UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
			}, function (error) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
			});
		}

		vm.loadBankDetail = loadBankDetail;
		function loadBankDetail() {
			vm.currentPage = 1;
			var offset = (1 * 10) - 10;
			vm.bankdetail = [];
			VerifikasiDataService.selectBankDetail({
				VendorID: vm.id
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.bankdetail = reply.data;
					console.info("bankDetail:" + JSON.stringify(vm.bankdetail));
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

		vm.updateBankDetail = updateBankDetail;
		function updateBankDetail(data) {
			//console.info("console edit dokumen");
			var data = {
				act: 0,
				item: data
			}
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/data-rekanan/verifikasi-data/bank-detail.modal.html',
				controller: "BankDetailModalCtrl",
				controllerAs: "BankDetModalCtrl",
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

		vm.deleteBankDetail = deleteBankDetail;
		function deleteBankDetail(doc) {
			bootbox.confirm('<h3 class="afta-font center-block">' + "Yakin ingin menghapus?" + '<h3>', function (reply) {
				if (reply) {
					//UIControlService.loadLoading(loadmsg);
					VerifikasiDataService.deleteBankDetail({ ID: doc.ID }, function (reply2) {
						UIControlService.unloadLoading();
						if (reply2.status === 200) {
							UIControlService.msg_growl('notice', 'data berhasil di hapus');
							init();
						} else
							UIControlService.msg_growl('error', 'gagal menghapus');
					}, function (error) {
						UIControlService.unloadLoading();
						UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
					});
				}
			});
		};

	}
})();
