(function () {
	'use strict';

	angular.module("app").controller("PemasukkanPenawaranBarangCtrl", ctrl);

	ctrl.$inject = ['GoodOfferEntryService', '$state', 'UIControlService', 'UploaderService', 'GlobalConstantService', '$stateParams'];

	function ctrl(GOEService, $state, UIControlService, UploaderService, GlobalConstantService, $stateParams) {
		var vm = this;
		vm.IDTender = Number($stateParams.TenderRefID);
		vm.IDStepTender = Number($stateParams.StepID);
		vm.ProcPackType = Number($stateParams.ProcPackType);
		vm.IDDoc = Number($stateParams.DocID);
		vm.folderFile = GlobalConstantService.getConstant('api') + "/";
		vm.habisTanggal = false;
		vm.showterendah = true;

		vm.init = init;
		function init() {
			//console.info("AA");
			loadDataPenawaran();
		}

		function loadDataPenawaran() {
			UIControlService.loadLoading("Silahkan Tunggu");
			GOEService.getAll({ column: vm.IDStepTender }, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					console.info("re:" + JSON.stringify(reply));
					vm.detail = reply.data;

					GOEService.isNeedTenderStepApproval({ TenderStepID: vm.IDStepTender }, function (result) {
						vm.isNeedApproval = result.data;
						//console.info("jumlahData:" + vm.countRegister);
					}, function (err) {
						$.growl.error({ message: "Gagal mendapatkan data Approval" });
						UIControlService.unloadLoading();
					});

					if (vm.detail.length === 0) {
						GOEService.GetStep({
							Status: vm.IDTender,
							FilterType: vm.ProcPackType
						}, function (reply) {
							UIControlService.unloadLoading();
							if (reply.status === 200) {
								console.info("re:" + JSON.stringify(reply));
								var data = reply.data;
								vm.StartDate = data.StartDate;
								vm.EndDate = data.EndDate;
								vm.TenderName = data.tender.TenderName;
							}
						}, function (err) {
							UIControlService.msg_growl("error", "MESSAGE.API");
							UIControlService.unloadLoading();
						});
					} else {
						vm.StartDate = vm.detail[0].StartDateTen;
						vm.EndDate = vm.detail[0].EndDateTen;
						vm.TenderName = vm.detail[0].TenderName;
					}
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}


		vm.sendToApproval = sendToApproval;
		function sendToApproval() {
			UIControlService.loadLoading('MESSAGE.SENDING');
			GOEService.sendToApproval({ ID: vm.IDStepTender }, function (reply) {
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

		vm.simpan = simpan;
		function simpan() {

		}

		vm.backpengadaan = backpengadaan;
		function backpengadaan() {
			$state.transitionTo('data-pengadaan-tahapan', {
				TenderRefID: vm.IDTender,
				ProcPackType: vm.ProcPackType
			});
		}

	}
})();
