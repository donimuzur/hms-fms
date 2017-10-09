(function () {
	'use strict';

	angular.module("app").controller("IzinUsahaController", ctrl);

	ctrl.$inject = ['$http', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'IzinUsahaService', 'AuthService', 'UIControlService'];
	/* @ngInject */
	function ctrl($http, $uibModal, $translate, $translatePartialLoader, $location, SocketService, IzinUsahaService, AuthService, UIControlService) {
		var vm = this;

		vm.listLicensi = [];
		vm.isChangeData = false;
		vm.IsApprovedCR = false;

		vm.init = init;
		function init() {
			jLoad();
			chekcIsVerified();
			//checkCR();
			//sendMail();
		}

		function chekcIsVerified() {
			IzinUsahaService.getCRbyVendor({ CRName: 'OC_VENDORLICENSI' }, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					//console.info("CR:" + JSON.stringify(reply.data));
					if (reply.data) {
						//if (reply.data === true) {
						vm.IsApprovedCR = true;
						// }
					} else {
						vm.IsApprovedCR = false;
					}
				}
				console.info(JSON.stringify(vm.IsApprovedCR));
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});

			//IzinUsahaService.isVerified(function (reply) {
			//    UIControlService.unloadLoading();
			//    if (reply.status === 200) {
			//        var data = reply.data;
			//        console.info("data" + JSON.stringify(data));
			//        if (data.VerifiedSendDate === null && data.VerifiedDate === null) {
			//            vm.IsApprovedCR = true;
			//        }
			//        else {
			//            checkCR();
			//        }
			//    }
			//}, function (err) {
			//    UIControlService.msg_growl("error", "MESSAGE.API");
			//    UIControlService.unloadLoading();
			//});
		}

		function checkCR() {
			UIControlService.loadLoading("Silahkan Tunggu");
			IzinUsahaService.getCRbyVendor({ CRName: 'OC_VENDORLICENSI' }, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					console.info("CR:" + JSON.stringify(reply.data));
					if (reply.data.length > 0) {
						//if (reply.data === true) {
						vm.IsApprovedCR = true;
						// }
						/*
                    else {
                        vm.isSentCR = false;
                    }*/
					}
					console.info(JSON.stringify(vm.IsApprovedCR));
				}

			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}

		vm.jLoad = jLoad;
		function jLoad() {
			UIControlService.loadLoading("MSG.LOADING");
			IzinUsahaService.selectLicensi(function (response) {
				UIControlService.unloadLoading();
				if (response.status == 200) {
					console.info(">>" + JSON.stringify(response.data));
					vm.VendorID = response.data[0].VendorID;
					var list = response.data;
					for (var i = 0; i < list.length; i++) {
						if (!(list[i].IssuedDate === null)) {
							list[i].IssuedDate = UIControlService.getStrDate(list[i].IssuedDate);
						}
						if (!(list[i].ExpiredDate === null)) {
							list[i].ExpiredDate = UIControlService.getStrDate(list[i].ExpiredDate);
						}
					}
					vm.listLicensi = list;
					checkExpiredDate(vm.listLicensi);
					loadCityCompany();
					console.info("list licensi" + JSON.stringify(vm.listLicensi));
					//loadEmailCompany(vm.VendorID);
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


		function loadCityCompany() {
			IzinUsahaService.selectcontact({ VendorID: vm.VendorID }, function (reply) {
				if (reply.status == 200) {
					vm.contactCompany = reply.data;
					for (var i = 0; i < vm.contactCompany.length; i++) {
						if (vm.contactCompany[i].VendorContactType.Name === 'VENDOR_CONTACT_TYPE_COMPANY') {
							console.info("kontak" + JSON.stringify(vm.contactCompany[i].Contact.Address.State.Country.CountryID));
							vm.cityID = vm.contactCompany[i].Contact.Address.State.Country.CountryID;
							break
						}
					}

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


		function checkExpiredDate(listLicensi) {
			console.info("masuk");
			var today = moment().format("YYYY-MM-DD");
			console.info(today);
			vm.dateA = []; vm.dateB = []; vm.dateC = [];
			for (var i = 0; i < listLicensi.length; i++) {
				vm.dateA[i] = moment(listLicensi[i].ExpiredDate).subtract(90, 'days').format("YYYY-MM-DD");
				vm.dateB[i] = moment(listLicensi[i].ExpiredDate).subtract(60, 'days').format("YYYY-MM-DD");
				vm.dateC[i] = moment(listLicensi[i].ExpiredDate).subtract(30, 'days').format("YYYY-MM-DD");
				//vm.coba = moment(listLicensi[0].ExpiredDate).subtract(30, 'days').format("YYYY-MM-DD");
				//console.info(JSON.stringify(vm.coba));
				if (vm.dateA[i] === today) {
					console.info("hariini");
					loadEmailCompany();
					vm.LicenseName = listLicensi[i].LicenseName;
					vm.days = 90;
				} else if (vm.dateB[i] === today) {
					loadEmailCompany();
					vm.LicenseName = listLicensi[i].LicenseName;
					vm.days = 60;
				} else if (vm.dateC[i] === today) {
					loadEmailCompany();
					vm.LicenseName = listLicensi[i].LicenseName;
					vm.days = 30;
				} else if (listLicensi[i].ExpiredDate === today) {
					loadEmailCompany();
					vm.LicenseName = listLicensi[i].LicenseName;
					vm.days = 0;
				}
			}
		}

		//load email vendor
		vm.loadEmailCompany = loadEmailCompany;
		function loadEmailCompany() {
			console.info("kirimemail");
			IzinUsahaService.selectcontact({ VendorID: vm.VendorID }, function (reply) {
				if (reply.status == 200) {
					vm.contact = reply.data;
					vm.listEmail = [];
					for (var i = 0; i < vm.contact.length; i++) {
						if (vm.contact[i].VendorContactType.Name === 'VENDOR_CONTACT_TYPE_COMPANY') {
							vm.listEmail.push(vm.contact[i].Contact.Email);
						}
					}
					console.info("list email" + JSON.stringify(vm.listEmail));
					sendMail(vm.listEmail);
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


		vm.sendMail = sendMail;
		function sendMail(listEmail) {
			console.info("kirimemail");
			var email = {
				subject: 'Notifikasi Ijin Usaha',
				mailContent: 'Kurang ' + vm.days + ' hari lagi ijin usaha ' + vm.LicenseName + ' akan kadaluarsa. Terima kasih.',
				isHtml: false,
				addresses: listEmail
			};
			console.info("kirimemail");
			// UIControlService.loadLoading("LOADERS.LOADING_SEND_EMAIL");
			IzinUsahaService.sendMail(email, function (response) {
				// UIControlService.unloadLoading();
				if (response.status == 200) {
					vm.days = ""; vm.LicenseName = "";
					UIControlService.msg_growl("notice", "Email Telah dikirim")
				} else {
					UIControlService.handleRequestError(response.data);
				}
			}, function (response) {
				UIControlService.handleRequestError(response.data);
				UIControlService.unloadLoading();
			});
		}


		//open form
		vm.openForm = openForm;
		function openForm(data, isForm) {
			var data = {
				item: data,
				isForm: isForm,
				cityID: vm.cityID
			}
			var temp;
			if (isForm === true) {
				temp = "app/modules/rekanan/data-perusahaan/izin-usaha/form-izin-usaha.html";
			} else {
				temp = "app/modules/rekanan/data-perusahaan/izin-usaha/detail-izin-usaha.html";
			}
			var modalInstance = $uibModal.open({
				templateUrl: temp,
				controller: 'FormIzinCtrl',
				controllerAs: 'FormIzinCtrl',
				resolve: {
					item: function () {
						return data;
					}
				}
			});
			modalInstance.result.then(function () {
				window.location.reload();
			});
		}
	}
})();