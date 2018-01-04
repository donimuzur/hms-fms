(function () {
	'use strict';

	angular.module("app").controller("PermintaanPerubahanDataCtrl", ctrl);

	ctrl.$inject = ['$translatePartialLoader', 'PermintaanUbahDataService', 'UIControlService', 'SocketService'];
	function ctrl($translatePartialLoader, PUbahDataService, UIControlService, SocketService) {
		var vm = this;
		vm.init = init;
		vm.listCR = [];
		vm.Remark = '';
		vm.isVerified;
		vm.IsApprovedBy;

		function init() {
			$translatePartialLoader.addPart('permintaan-ubah-data');
			loadListOpsiChangeRequest();
			loadCheckCR();
			chekcIsVerified();
		}

		function chekcIsVerified() {
			PUbahDataService.isVerified(function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					console.info("ver>>" + JSON.stringify(reply));
					var data = reply.data;
					if (!(data.Isverified === null)) {
						vm.isVerified = true;
					} else {
						vm.isVerified = false;
					}
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}

		function loadCheckCR() {
			UIControlService.loadLoading("Silahkan Tunggu");
			PUbahDataService.getCRbyVendor(function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					for (var i = 0; i < reply.data.length; i++) {
						for (var j = 0; j < vm.listCR.length; j++) {
							if (reply.data[i].ChangeRequestDataDetails[0].ChangeRequestRefID === vm.listCR[j].ChangeRequestRefID) {
								vm.listCR.splice(j, 1);
								break;
							}
						}
					}

					vm.isSentCR = false;
					//console.info("listCR:" + JSON.stringify(vm.listCR));
					//console.info("CR:" + JSON.stringify(reply));
					//if (!(reply.data === null) && reply.data.ApproveBy === null) {
					//	vm.isSentCR = true;
					//	vm.IsApprovedBy = reply.data.ApproveBy;
					//}
					//if (!(reply.data === null) && reply.data.ApproveBy === 1) {
					//	vm.isSentCR = true;
					//	vm.IsApprovedBy = reply.data.ApproveBy;
					//} else {
					//	vm.isSentCR = false;
					//}
				}

			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}

		function loadListOpsiChangeRequest() {
			//alert("load");
			vm.listCR = [];
			PUbahDataService.getOpsiChangeReq(function (reply) {
				UIControlService.unloadLoading();
				var data = reply.data.List;
				for (var i = 0; i < data.length; i++) {
					var newmap = {
						ChangeRequestRefID: data[i].RefID,
						CRName: data[i].Value,
						Description: "",
						IsApproved: false
					}
					vm.listCR.push(newmap);
				}
				console.info("list cr" + JSON.stringify(vm.listCR));
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}

		vm.sendData = sendData;
		function sendData() {
			//mapping data
			var newlist = [];
			for (var i = 0; i < vm.listCR.length; i++) {
				if (vm.listCR[i].IsApproved === true) {
					var newmap = {
						ChangeRequestRefID: vm.listCR[i].ChangeRequestRefID,
						Description: vm.listCR[i].Description,
						IsApproved: 0
					}
					newlist.push(newmap);
				}
			}
			//console.info(JSON.stringify(newlist));
			var datasimpan = {
				Remark: vm.Remark,
				ChangeRequestDataDetails: newlist
			}
			PUbahDataService.insertChangeReq(datasimpan, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					UIControlService.msg_growl("success", "Berhasil Kirim Data Permintaan Ubah Data");
					SocketService.emit("PermintaanUbahData");
					init();
				} else {
					UIControlService.msg_growl("error", "FORM.MSG_ERR_SAVE");
					return;
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.ERR_API");
				UIControlService.unloadLoadingModal();
			});
		}
	}
})();