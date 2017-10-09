(function () {
	'use strict';

	angular.module("app").controller("VerifiedSendCtrl", ctrl);

	ctrl.$inject = ['$translatePartialLoader', 'SocketService', 'VerifiedSendService', 'UIControlService'];
	function ctrl($translatePartialLoader, SocketService, VerifiedSendService, UIControlService) {
		var vm = this;
		vm.verified = [];
		vm.init = init;
		

		function init() {
			$translatePartialLoader.addPart('kirim-verifikasi');
			jLoad(1);
			role();
			console.info("a");
			check();
		}

		vm.jLoad = jLoad;
		function jLoad(current) {
			VerifiedSendService.selectVerifikasi(function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
				    vm.verified = reply.data;
				    vm.vendorID = vm.verified.VendorID;
				    console.info(vm.verified);
					//console.info(JSON.stringify(vm.vendorID));
				} else {
					$.growl.error({ message: "Gagal mendapatkan data verifikasi" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				console.info("error:" + JSON.stringify(err));
				//$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}
		vm.data= [];
		vm.role = role;
		function role(current) {
		    VerifiedSendService.role(function (reply) {
		        if (reply.status === 200) {
		            var data = reply.data[0].Children;
		            vm.data = data;
		            check();
		            console.info(vm.data);
		        } else {
		            $.growl.error({ message: "Gagal mendapatkan data verifikasi" });
		            UIControlService.unloadLoading();
		        }
		    }, function (err) {
		        console.info("error:" + JSON.stringify(err));
		        //$.growl.error({ message: "Gagal Akses API >" + err });
		        UIControlService.unloadLoading();
		    });
		}

		vm.loadContact = loadContact;
		function loadContact() {
		    VerifiedSendService.selectcontact({VendorID: vm.vendorID},function (reply) {
		        UIControlService.unloadLoading();
		        if (reply.status === 200) {
		            vm.contact = reply.data;
		            vm.listEmail = [];
		            for (var i = 0; i < vm.contact.length; i++) {
		                if (vm.contact[i].VendorContactType.Name === 'VENDOR_CONTACT_TYPE_COMPANY') {
		                    vm.listEmail.push(vm.contact[i].Contact.Email);
		                }
		            }
		            console.info("list email" + JSON.stringify(vm.listEmail));
		            sendMail(vm.listEmail);
		            //console.info(JSON.stringify(reply.data));
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

		vm.sendMail = sendMail;
		function sendMail(listEmail) {
		    var email = {
		        subject: 'Verifikasi Akun',
		        mailContent: 'Permintaan verifikasi akun Anda telah terkirim. Mohon tunggu sampai akun Anda diverifikasi oleh pihak administrator.<br>Terima kasih.',
		        isHtml: true,
		        addresses: listEmail
		    };

		    // UIControlService.loadLoading("LOADERS.LOADING_SEND_EMAIL");
		    VerifiedSendService.sendMail(email,
                function (response) {
                    // UIControlService.unloadLoading();
                    if (response.status == 200) {
                        UIControlService.msg_growl("notice", "Email Telah dikirim")
                    } else {
                        UIControlService.handleRequestError(response.data);
                    }
                },
                function (response) {
                    UIControlService.handleRequestError(response.data);
                    UIControlService.unloadLoading();
                });
		}

		vm.isChecked;
		vm.check = check;
		vm.flag = 1;
		function check() {
		    for (var i = 0; i < vm.data.length - 1; i++) {
		        if (vm.data[i].Label !== "MENUS.COMPANYDATA.SENDVERIFICATION"){
		            if(vm.data[i].Label !== "MENUS.COMPANYDATA.OTHERDOCUMENT") {
		                if (vm.data[i].IsChecked === null) {
		                    console.info(vm.data[i]);
		                    vm.flag = 0;
		                }
		            }
		        }
		     }
		 }


		vm.add = add;
		function add() {
		    UIControlService.loadLoading("Silahkan Tunggu");
			//console.info("ada:"+JSON.stringify(data))
			VerifiedSendService.update(function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
				    UIControlService.msg_growl("success", "Data verifikasi berhasil di kirim");
				    loadContact();
					SocketService.emit("daftarRekanan");
					jLoad(1);
				} else {
					UIControlService.msg_growl("error", "Proses verifikasi gagal.");
					return;
				}
			}, function (err) {
				UIControlService.msg_growl("error", "Gagal Akses API ");
				UIControlService.unloadLoading();
			});
		}
	}
})();
