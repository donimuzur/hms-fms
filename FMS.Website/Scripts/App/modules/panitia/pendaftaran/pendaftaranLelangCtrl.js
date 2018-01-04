(function () {
	'use strict';

	angular.module("app").controller("PendaftaranLelangCtrl", ctrl);

	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'PendaftaranLelangService', 'RoleService', 'UIControlService', '$uibModal', '$stateParams', 'GlobalConstantService'];
	function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, PendaftaranLelangService,
        RoleService, UIControlService, $uibModal, $stateParams, GlobalConstantService) {
		var vm = this;
		vm.TenderRefID = Number($stateParams.TenderRefID);
		vm.StepID = Number($stateParams.StepID);
		vm.ProcPackType = Number($stateParams.ProcPackType);
		vm.pendaftaran = [];
		vm.userBisaNgatur = false;
		vm.page_id = 103;
		vm.nama_paket = "";
		vm.nama_tahapan = "";
		vm.is_created = false;
		vm.status = -1;
		vm.peserta = [];
		vm.menuhome = 0;
		vm.labelcurr;

		vm.init = init;
		function init() {
			$translatePartialLoader.addPart('pendaftaran-lelang');
			// UIControlService.loadLoading("Silahkan Tunggu...");
			//  getBlacklist();
			console.info(vm.StepID + ">>");
			jLoad(1);
			loadDataTender();
		}

		vm.sendToApproval = sendToApproval;
		function sendToApproval() {
			UIControlService.loadLoading('MESSAGE.SENDING');
			PendaftaranLelangService.sendToApproval({ ID: vm.StepID }, function (reply) {
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

		function loadDataTender() {
			PendaftaranLelangService.getDataStepTender({
				ID: vm.StepID
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.data;
					vm.TenderName = data.tender.TenderName;
					vm.StartDate = UIControlService.getStrDate(data.StartDate);
					vm.EndDate = UIControlService.getStrDate(data.EndDate);
					vm.nama_tahapan = data.step.TenderStepName;
					vm.TenderID = data.TenderID;
					vm.DocumentUrl = data.DocumentUrl;
					console.info("tender::" + JSON.stringify(data));
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}

		vm.jLoad = jLoad;
		function jLoad(current) {
			vm.register = [];
			PendaftaranLelangService.SelectTender({
				StepID: vm.StepID,
				ProcPackageType: vm.ProcPackType,
				TenderRefID: vm.TenderRefID
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.register = reply.data;
					//console.info(JSON.stringify(vm.register));

					PendaftaranLelangService.isNeedTenderStepApproval({
						TenderRefID: vm.TenderRefID,
						TenderStepDataID: vm.StepID,
						ProcPackageType: vm.ProcPackType
					}, function (result) {
						vm.isNeedApproval = result.data;
						//console.info("jumlahData:" + vm.countRegister);
					}, function (err) {
						$.growl.error({ message: "Gagal mendapatkan data Approval" });
						UIControlService.unloadLoading();
					});
				} else {
					$.growl.error({ message: "Gagal mendapatkan data Pendaftaran Lelang" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}

		vm.lihatPendaftaranLelang = lihatPendaftaranLelang;
		function lihatPendaftaranLelang() {
			var data = {
				TenderName: vm.TenderName,
				DocumentUrl: vm.DocumentUrl
			}
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/pendaftaran/DetailPendaftaranLelang.html',
				controller: 'DetailPendaftaranLelangCtrl',
				controllerAs: 'DetailPendaftaranLelangCtrl',
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

		vm.uploadDokumen = uploadDokumen;
		function uploadDokumen(tenderID, tenderStepID) {
			var data = {
				TenderID: tenderID, TenderStepID: tenderStepID
			}
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/pendaftaran/uploadDokumen.html',
				controller: 'FormUploadPendaftaranLelangCtrl',
				controllerAs: 'FormUploadPendaftaranLelangCtrl',
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

		vm.batal = batal;
		function batal() {
			$uibModalInstance.dismiss('cancel');
		}
	}
})();