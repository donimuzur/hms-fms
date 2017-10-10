(function () {
	'use strict';

	angular.module("app").controller("FormActivedRejectedCtrl", ctrl);
	ctrl.$inject = ['$translatePartialLoader', 'VerifikasiDataService', 'UIControlService', 'item', '$uibModalInstance', 'MailerService', 'SocketService'];
	/* @ngInject */
	function ctrl($translatePartialLoader, VerifikasiDataService, UIControlService, item, $uibModalInstance, MailerService, SocketService) {
		var vm = this;
		vm.emailAddress = item.emailAddress;
		vm.isAdd = item.item;
		vm.act = item.act;
		vm.Description = "";
		//functions
		vm.init = init;
		//console.info("masuuk " + JSON.stringify(item));
		function init() {
			$translatePartialLoader.addPart('verifikasi-data');
		};

		vm.cancel = cancel;
		function cancel() {
			$uibModalInstance.dismiss('cancel');
		}

		vm.tolakactived = tolakactived;
		function tolakactived() {
			console.info("masuk form add/edit");
			var data = {
				item: item.item
			}
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/data-rekanan/verifikasi-data/ActivedRejected.html',
				controller: 'FormActivedRejectedCtrl',
				controllerAs: 'FrmActivedRejectedCtrl',
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

		vm.ditolak = ditolak;
		function ditolak(data, emailAddress) {
			UIControlService.loadLoading("Silahkan Tunggu");

			//sendMail('Terima kasih telah mendaftar di e-Procurement PT. Vale. Kami telah melakukan verifikasi terhadap data dan dokumen pendaftaran anda dan telah mengaktifkan akun anda.',emailAddress);

			VerifikasiDataService.InsertActivedRejected({
				IsActived: data,
				VendorID: item.item.VendorID,
				Description: vm.Description,
				UserID: item.item.UserID
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					if (data === false) { //rejected
						UIControlService.msg_growl("success", "Data Berhasil di Tolak Aktifasi");
						item.item.Notice = vm.Description;
						//sendMail('Terima kasih telah mendaftar di e-Procurement PT. Vale. Kami telah melakukan verifikasi terhadap data dan dokumen pendaftaran anda dan telah mengaktifkan akun anda.', emailAddress);
					} else { //accepted
						UIControlService.msg_growl("success", "Data Berhasil di Terima Aktifasi");
						//sendMail('Terima kasih telah mendaftar di e-Procurement PT. Vale. Kami telah melakukan verifikasi terhadap data dan dokumen pendaftaran anda dan namun kami belum dapat mengaktifkan akun anda.', emailAddress);
					}

					SocketService.emit("daftarRekanan");

					MailerService.getActMail({
						IsActived: data,
						VendorID: item.item.VendorID,
						Notice: item.item.Notice
					}, function (response) {
						if (response.status == 200) {
							var email = {
								subject: response.data.Subject,
								//mailContent: 'Terima kasih telah mendaftar di e-Procurement. Kami akan melakukan verifikasi terlebih dahulu terhadap data dan dokumen pendaftaran anda.',
								mailContent: response.data.MailContent,
								isHtml: true,
								addresses: new Array(emailAddress)
							};

							UIControlService.loadLoading("LOADERS.LOADING_SEND_EMAIL");
							MailerService.sendMail(email, function (response) {
								UIControlService.unloadLoading();
								if (response.status == 200) {
									UIControlService.msg_growl("notice", "Email Sent!");
								} else {
									UIControlService.msg_growl("error", "Failure sending mail.");
									//UIControlService.handleRequestError(response.data);
								}
								$state.go('daftar_kuesioner');
							}, function (response) {
								UIControlService.handleRequestError(response.data);
								UIControlService.unloadLoading();
								//$state.go('daftar_kuesioner');
							});
						} else {
							UIControlService.handleRequestError(response.data);
						}
					}, function (response) {
						UIControlService.handleRequestError(response.data);
						UIControlService.unloadLoading();
					});

					$uibModalInstance.close();
				} else {
					UIControlService.msg_growl("error", "Gagal menonaktifkan data ");
					return;
				}
			}, function (err) {
				UIControlService.msg_growl("error", "Gagal Akses API ");
				UIControlService.unloadLoading();
			});
		}

		function sendMail(mailContent, emailAddress) {
			MailerService.getMailContent({
				Email: 'Pendaftaran Rekanan Baru',
				Username: vm.vendor.username,
				Name: vm.vendor.name
			}, function (response) {
				if (response.status == 200) {
					var email = {
						subject: response.data.Subject,
						//mailContent: 'Terima kasih telah mendaftar di e-Procurement. Kami akan melakukan verifikasi terlebih dahulu terhadap data dan dokumen pendaftaran anda.',
						mailContent: response.data.MailContent,
						isHtml: true,
						addresses: [vm.vendor.email]
					};

					UIControlService.loadLoading("LOADERS.LOADING_SEND_EMAIL");
					VendorRegistrationService.sendMail(email, function (response) {
						UIControlService.unloadLoading();
						if (response.status == 200) {
							UIControlService.msg_growl("success", "Email Sent!");
						} else {
							UIControlService.msg_growl("error", "Failure sending mail.");
							//UIControlService.handleRequestError(response.data);
						}
						$state.go('daftar_kuesioner');
					}, function (response) {
						UIControlService.handleRequestError(response.data);
						UIControlService.unloadLoading();
						//$state.go('daftar_kuesioner');
					});
				} else {
					UIControlService.handleRequestError(response.data);
				}
			}, function (response) {
				UIControlService.handleRequestError(response.data);
				UIControlService.unloadLoading();
			});

			var email = {
				subject: 'Vendor registration notification',
				mailContent: mailContent,
				isHtml: false,
				addresses: [emailAddress]
			};

			UIControlService.loadLoading("LOADERS.LOADING_SEND_EMAIL");
			VerifikasiDataService.sendMail(email, function (response) {
				UIControlService.unloadLoading();
				if (response.status == 200) {
					UIControlService.msg_growl("notice", "Email Sent!");
				} else {
					UIControlService.msg_growl("error", "Failure sending mail.");
					//UIControlService.handleRequestError(response.data);
				}
			}, function (response) {
				UIControlService.handleRequestError(response.data);
				UIControlService.unloadLoading();
				//$state.go('daftar_kuesioner');
			});
		}
	}
})();
