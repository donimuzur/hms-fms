(function () {
	'use strict';

	angular.module("app").controller("KelengkapanTenderJasaVendorCtrl", ctrl);

	ctrl.$inject = ['PengumumanPengadaanService', '$state', 'UIControlService', '$uibModal', '$stateParams'];
	function ctrl(PengumumanPengadaanService, $state, UIControlService, $uibModal, $stateParams) {
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
		    //loadDataTender();
		    loadDocChecklist();
		}
		function loadDocChecklist() {
			UIControlService.loadLoading("Silahkan Tunggu");
			PengumumanPengadaanService.getDataChecklistVendor({
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
				var dt = {
					DocumentName: data.DocumentName,
					FileType: data.FileType,
					ApproveDate: data.ApproveDate,
					DocumentURL: data.DocumentURL,
					OfferEntryDocumentID: data.OfferEntryChecklistID,
					OfferEntryVendorID: data.OfferEntryVendorID,
					IsCheck: true
				}
				var modalInstance = $uibModal.open({
				    templateUrl: 'detail-dok-penawaran.html',
				    controller: 'DetDokPenawaranCtrl',
				    controllerAs: 'DetDokPenawaranCtrl',
				    resolve: {
				        item: function () {
				            return dt;
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