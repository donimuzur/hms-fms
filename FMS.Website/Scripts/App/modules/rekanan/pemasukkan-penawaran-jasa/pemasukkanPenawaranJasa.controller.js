(function () {
	'use strict';

	angular.module("app").controller("ServiceOfferEntryVendorCtrl", ctrl);

	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'OfferEntryService', '$state', 'UIControlService', '$uibModal', '$stateParams'];
	function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, OEService,
        $state, UIControlService, $uibModal, $stateParams) {
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
		vm.listKelengkapan = [];

		vm.init = init;
		function init() {
			$translatePartialLoader.addPart("pemasukkan-penawaran-jasa");
			if (vm.ProcPackType === 4189) {
				loadDataPenawaran();
				loadDataTender();
			} else {
				UIControlService.msg_growl("warning", "Halaman Khusus Pemasukkan Penawaran Tender Jasa");
				return;
			}
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

		function loadDataPenawaran() {
			UIControlService.loadLoading("Silahkan Tunggu");
			OEService.getKelengkapanDocVendor({
				TenderStepID: vm.IDStepTender
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.data;
					console.info("penawaran:" + JSON.stringify(data));
					vm.listKelengkapan = data.VendorDocuments;
					for (var i = 0; i < vm.listKelengkapan.length; i++) {
						if (!(vm.listKelengkapan[i].ApproveDate === null)) {
							vm.listKelengkapan[i].ApproveDate = UIControlService.getStrDate(vm.listKelengkapan[i].ApproveDate);
						}
					}

				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}

		vm.detailDokumen = detailDokumen;
		function detailDokumen(data) {
			console.info(JSON.stringify(data));
			if (!(data.DocumentType === 'FORM_DOCUMENT')) {
				var datax = {
					DocumentName: data.DocumentName,
					FileType: data.FileType,
					ApproveDate: data.ApproveDate,
					DocumentURL: data.DocumentURL,
					OfferEntryDocumentID: data.OfferEntryDocumentID,
					OfferEntryVendorID: data.OfferEntryVendorID,
					IsCheck: false
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
			} else {
				$state.transitionTo('kelengkapan-datakomersial-jasa-vendor', {
					TenderRefID: vm.IDTender, StepID: vm.IDStepTender, ProcPackType: vm.ProcPackType, DocID: data.OfferEntryDocumentID
				});
			}
		}

		vm.backDetailTahapan = backDetailTahapan;
		function backDetailTahapan() {
			$state.transitionTo('data-pengadaan-tahapan', { TenderRefID: vm.IDTender, ProcPackType: vm.ProcPackType });
		}

		vm.backTahapan = backTahapan;
		function backTahapan() {
			$state.transitionTo('pemasukkan-penawaran-jasa', { TenderRefID: vm.IDTender, ProcPackType: vm.ProcPackType, StepID: vm.IDStepTender });
		}

		vm.batal = batal;
		function batal() {
			$state.transitionTo('dashboard-vendor');
		}

		vm.complete = complete;
		function complete() {
			OEService.complete({
				TenderStepID: vm.IDStepTender
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					UIControlService.msg_growl("succcess", "Berhasil kirim penawaran jasa.");
					UIControlService.msg_growl("success", 'NOTIFICATION.UPDATE.SUCCESS.MESSAGE', "NOTIFICATION.UPDATE.SUCCESS.TITLE");
					batal();
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});
		}

		vm.kelengkapanTender = kelengkapanTender;
		function kelengkapanTender() {
			$state.transitionTo('kelengkapan-tender-jasa', { TenderRefID: vm.IDTender, ProcPackType: vm.ProcPackType, StepID: vm.IDStepTender });
		}

		vm.updateChecklist = updateChecklist;
		function updateChecklist(data) {
			//console.info("data:" + JSON.stringify(data));
			UIControlService.loadLoadingModal("Silahkan Tunggu");
			OEService.approveDocByVendor({
				OfferEntryDocumentID: data.OfferEntryDocumentID,
				OfferEntryVendorID: data.OfferEntryVendorID,
				IsApproved: data.IsApproved
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
	}
})();