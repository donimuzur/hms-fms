(function () {
	'use strict';

	angular.module("app").controller("ServiceOfferEntryCtrl", ctrl);

	ctrl.$inject = ['$translatePartialLoader', 'OfferEntryService', '$state', 'UIControlService', '$stateParams'];
	function ctrl($translatePartialLoader, OEService, $state, UIControlService, $stateParams) {
		var vm = this;
		vm.IDTender = Number($stateParams.TenderRefID);
		vm.IDStepTender = Number($stateParams.StepID);
		vm.ProcPackType = Number($stateParams.ProcPackType);
		vm.maxSize = 10;
		vm.currentPage = 1;
		vm.TenderName = '';
		vm.TenderCode = '';
		vm.StartDate = null;
		vm.EndDate = null;
		vm.listVendor = [];

		vm.init = init;
		function init() {
			$translatePartialLoader.addPart("pemasukkan-penawaran-jasa");
			if (vm.ProcPackType === 4189) {
				loadDataPenawaran();
			} else {
				UIControlService.msg_growl("warning", "Halaman Khusus Pemasukkan Penawaran Tender Jasa");
				return;
			}
		}

		function loadDataPenawaran() {
			UIControlService.loadLoading("Silahkan Tunggu");
			OEService.getAllDataOffer({ TenderStepID: vm.IDStepTender }, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					//console.info("re:" + JSON.stringify(reply));
					var data = reply.data;
					vm.listVendor = data.ServiceOfferEntryVendors;
					for (var i = 0; i < vm.listVendor.length; i++) {
						if (vm.listVendor[i].EntryDate != null) {
							vm.listVendor[i].EntryDate = UIControlService.getStrDate(vm.listVendor[i].EntryDate);
						}
					}
					vm.TenderName = data.TenderName;
					vm.StartDate = UIControlService.getStrDate(data.StartDate);
					vm.EndDate = UIControlService.getStrDate(data.EndDate);

					OEService.isNeedTenderStepApproval({ TenderStepID: vm.IDStepTender }, function (result) {
						vm.isNeedApproval = result.data;
						//console.info("jumlahData:" + vm.countRegister);
					}, function (err) {
						$.growl.error({ message: "Gagal mendapatkan data Approval" });
						UIControlService.unloadLoading();
					});

					//console.info("ve" + JSON.stringify(vm.listVendor));
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}

		vm.sendToApproval = sendToApproval;
		function sendToApproval() {
			UIControlService.loadLoading('MESSAGE.SENDING');
			OEService.sendToApproval({ ID: vm.IDStepTender }, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					UIControlService.msg_growl("success", 'NOTIFICATION.SEND.SUCCESS.MESSAGE', "NOTIFICATION.SEND.SUCCESS.TITLE");
					//UIControlService.msg_growl('notice', $filter('translate')('MESSAGE.SUCC_SEND_TO_APPRV'));
				} else {
					$.growl.error({ message: "Send Approval Failed." });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}

		vm.backDetailTahapan = backDetailTahapan;
		function backDetailTahapan() {
			$state.transitionTo('data-pengadaan-tahapan', { TenderRefID: vm.IDTender, ProcPackType: vm.ProcPackType });
		}

		vm.backTahapan = backTahapan;
		function backTahapan() {
			$state.transitionTo('pemasukkan-penawaran-jasa', { TenderRefID: vm.IDTender, ProcPackType: vm.ProcPackType, StepID: vm.IDStepTender });
		}

		vm.kelengkapanTender = kelengkapanTender;
		function kelengkapanTender() {
			$state.transitionTo('kelengkapan-tender-jasa', { TenderRefID: vm.IDTender, ProcPackType: vm.ProcPackType, StepID: vm.IDStepTender });
		}
	}
})();