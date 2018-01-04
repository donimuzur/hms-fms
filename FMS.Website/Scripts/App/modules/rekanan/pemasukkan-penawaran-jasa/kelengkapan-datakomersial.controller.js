(function () {
	'use strict';

	angular.module("app").controller("KelengkapanDataKomersialVendorCtrl", ctrl);

	ctrl.$inject = ['OfferEntryService', '$state', 'UIControlService', '$uibModal', '$stateParams'];
	function ctrl(OEService, $state, UIControlService, $uibModal, $stateParams) {
		var vm = this;
		vm.IDTender = Number($stateParams.TenderRefID);
		vm.IDStepTender = Number($stateParams.StepID);
		vm.ProcPackType = Number($stateParams.ProcPackType);
		vm.IDDoc = Number($stateParams.DocID);
		vm.listDocChecks = [];
		vm.TenderName = '';
		vm.TenderCode = '';
		vm.StartDate = null;
		vm.EndDate = null;

		vm.init = init;
		function init() {
			loadDocChecklist();
			loadDataTender();
		}
		function loadDataTender() {
			OEService.getDataStepTender({
				ID: vm.IDStepTender
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.data;
					vm.TenderName = data.tender.TenderName;
					vm.StartDate = UIControlService.getStrDate(data.StartDate);
					vm.EndDate = UIControlService.getStrDate(data.EndDate);
					//console.info("tender::" + JSON.stringify(vm.dataTenderReal));
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}
		function loadDocChecklist() {
			UIControlService.loadLoading("Silahkan Tunggu");
			OEService.getDataChecklistVendor({
				TenderStepID: vm.IDStepTender
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.data;
					vm.listDocChecks = data.VendorChecklists;
					for (var i = 0; i < vm.listDocChecks.length; i++) {
						if (!(vm.listDocChecks[i].ApproveDate === null)) {
							vm.listDocChecks[i].ApproveDate = UIControlService.getStrDate(vm.listDocChecks[i].ApproveDate);
						}
					}
					//console.info(JSON.stringify(vm.listDocChecks));
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}

		vm.batal = batal;
		function batal() {
			$uibModalInstance.dismiss('cancel');
		};

		vm.detailDokumen = detailDokumen;
		function detailDokumen(data) {
			//console.info("det: " + JSON.stringify(data));
			if (!(data.DocumentType === 'FORM_DOCUMENT')) {
				var datax = {
					DocumentName: data.DocumentName,
					FileType: data.FileType,
					ApproveDate: data.ApproveDate,
					DocumentURL: data.DocumentURL,
					OfferEntryDocumentID: data.OfferEntryChecklistID,
					OfferEntryVendorID: data.OfferEntryVendorID,
					IsCheck: true
				}
				var modalInstance = $uibModal.open({
					templateUrl: 'app/modules/rekanan/pemasukkan-penawaran-jasa/detailDokumen.html',
					controller: 'detailDokumenJasaCtrl',
					controllerAs: 'detDokCtrl',
					resolve: {
						item: function () {
							return datax;
						}
					}
				});
				modalInstance.result.then(function () {
					vm.init();
				});
			} else if (data.DocumentType == 'FORM_DOCUMENT' && data.DocumentName == 'Tender Questionaire') {
				$state.transitionTo('questionnaire-tenderjasa-vendor', {
					TenderRefID: vm.IDTender, StepID: vm.IDStepTender, ProcPackType: vm.ProcPackType, DocID: data.OfferEntryChecklistID
				});
			} else if (data.DocumentType == 'FORM_DOCUMENT' && data.DocumentName == 'Pricing') {
				$state.transitionTo('pricelist-serviceoffer-vendor', {
					TenderRefID: vm.IDTender, StepID: vm.IDStepTender, ProcPackType: vm.ProcPackType, DocID: data.OfferEntryChecklistID
				});
			}
		}

		vm.updateChecklist = updateChecklist;
		function updateChecklist(data) {
			//console.info("data:" + JSON.stringify(data));
			UIControlService.loadLoadingModal("Silahkan Tunggu");
			OEService.approveChecklistByVendor({
				OfferEntryChecklistID: data.OfferEntryChecklistID, OfferEntryVendorID: data.OfferEntryVendorID
			}, function (reply) {
				UIControlService.unloadLoadingModal();
				if (reply.status === 200) {
					var data = reply.data;
					UIControlService.msg_growl("succcess", "Berhasil Simpan Data Persetujuan");
					vm.init();
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoadingModal();
			});
		}

		vm.back = back;
		function back() {
			$state.transitionTo('pemasukkan-penawaran-jasa-vendor', {
				TenderRefID: vm.IDTender, StepID: vm.IDStepTender, ProcPackType: vm.ProcPackType
			});
		}

	}
})();