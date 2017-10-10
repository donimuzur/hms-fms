(function () {
	'use strict';

	angular.module("app").controller("KelengkapanDataKomersialCtrl", ctrl);

	ctrl.$inject = ['OfferEntryService', 'UIControlService', 'item', '$uibModalInstance', '$uibModal'];
	function ctrl(OEService, UIControlService, item, $uibModalInstance, $uibModal) {
		var vm = this;
		vm.data = item.data;
		console.info(JSON.stringify(item));
		vm.TenderName = item.TenderName;
		vm.StartDate = UIControlService.getStrDate(item.StartDate);
		vm.EndDate = UIControlService.getStrDate(item.EndDate);
		vm.listKomersial = item.listCheck;

		vm.init = init;
		function init() {
			if (vm.listKomersial.length > 0) {
				for (var i = 0; i < vm.listKomersial.length; i++) {
					if (vm.listKomersial[i].DocumentURL === null) {
						vm.listKomersial[i].UploadDate = null;
					} else {
						vm.listKomersial[i].UploadDate = UIControlService.getStrDate(vm.listKomersial[i].UploadDate);
					}
				}
			}
		}

		vm.batal = batal;
		function batal() {
			$uibModalInstance.dismiss('cancel');
		};

		vm.detailKelengkapan = detailKelengkapan;
		function detailKelengkapan(data) {
			var datasend = {
				dataDoc: data
			}
			var modalInstance = $uibModal.open({
				templateUrl: 'upload-dokumen.html',
				controller: 'UploadDokumenJasaCtrl',
				controllerAs: 'uploadDokJasaCtrl',
				resolve: {
					item: function () {
						return datasend;
					}
				}
			});
			modalInstance.result.then(function () {
				init();
			});
		}

		vm.updateChecklist = updateChecklist;
		function updateChecklist(data) {
			var savedata = {
				ID: data.ID,
				IsRequired: data.IsRequired,
				DocumentURL: data.DocumentURL
			}
			OEService.updateChecklist(savedata, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					UIControlService.msg_growl("success", "Berhasil Update Data");
					vm.init();
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