(function () {
	'use strict';

	angular.module("app").controller("priceListOfferCtrl", ctrl);

	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', 'OfferEntryService', '$state', 'UIControlService', 'UploaderService', '$uibModal', 'GlobalConstantService', '$stateParams'];

	function ctrl($http, $translate, $translatePartialLoader, OEService, $state, UIControlService, UploaderService, $uibModal, GlobalConstantService, $stateParams) {
		var vm = this;

		vm.IDTender = Number($stateParams.TenderRefID);
		vm.IDStepTender = Number($stateParams.StepID);
		vm.ProcPackType = Number($stateParams.ProcPackType);
		vm.IDDoc = Number($stateParams.DocID);
		vm.folderFile = GlobalConstantService.getConstant('api') + "/";
		vm.habisTanggal = false;
		vm.showterendah = true;
		vm.subPekerjaan = [];
		vm.init = init;

		function init() {
			UIControlService.loadLoading("Silahkan Tunggu");
			OEService.getPricelist({
				TenderStepID: vm.IDStepTender
			}, function (reply) {
				if (reply.status === 200) {
					UIControlService.unloadLoading();
					vm.subPekerjaan = reply.data;
					vm.subPekerjaan.OfferTotalCostStr = reply.data.OfferTotalCost.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
					for (var i = 0; i < reply.data.ServiceOfferEntryPricelists.length; i++) {
						vm.subPekerjaan.ServiceOfferEntryPricelists[i].TotalPriceStr = reply.data.ServiceOfferEntryPricelists[i].TotalPrice.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
					}

					//{ nama: "SUB.A", total_line: 10900, 
					//	anak: [{
					//		nama: "Sub A1", cucu: [
					//			{ nama: "Administrasi dan Dokumentasi", qty: "1LS", unit: 100000, linecost: 100000 },
					//			{ nama: "Kabel", qty: "5 Meter", unit: 150000, linecost:150000 }]
					//	},
					//		{ nama: "Sub A2" }]
					//},
					//{ nama: "SUB.B", total_line:25000 }

					//console.info("tender::" + JSON.stringify(vm.dataTenderReal));
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});

			//UIControlService.loadLoading("Silahkan Tunggu");
			//OEService.getPricelist(vm.listQuest, function (reply) {
			//    UIControlService.unloadLoading();
			//    if (reply.status === 200) {
			//        var data = reply.data;
			//        UIControlService.msg_growl("error", "Berhasil Simpan Data Kuisioner");
			//        vm.init();
			//    }
			//}, function (err) {
			//    UIControlService.msg_growl("error", "MESSAGE.API");
			//    UIControlService.unloadLoading();
			//});
		}

		vm.priceChange = priceChange;
		function priceChange(parent, index) {
			var unitPrice = vm.subPekerjaan.ServiceOfferEntryPricelists[parent].ServiceOfferEntryPricelistDetails[index].UnitPrice;
			var qty = vm.subPekerjaan.ServiceOfferEntryPricelists[parent].ServiceOfferEntryPricelistDetails[index].Quantity;
			var count = vm.subPekerjaan.ServiceOfferEntryPricelists[parent].ServiceOfferEntryPricelistDetails;
			var countTotal = vm.subPekerjaan.ServiceOfferEntryPricelists
			var initVal = 0;
			var initTotal = 0;

			vm.subPekerjaan.ServiceOfferEntryPricelists[parent].ServiceOfferEntryPricelistDetails[index].TotalPrice = unitPrice * qty;

			for (var i = 0; i < count.length; i++) {
				initVal = initVal + count[i].TotalPrice;
			}

			vm.subPekerjaan.ServiceOfferEntryPricelists[parent].TotalPrice = initVal;
			vm.subPekerjaan.ServiceOfferEntryPricelists[parent].TotalPriceStr = initVal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");

			for (var i = 0; i < countTotal.length; i++) {
				initTotal = initTotal + countTotal[i].TotalPrice;
			}

			vm.subPekerjaan.OfferTotalCost = initTotal;
			vm.subPekerjaan.OfferTotalCostStr = initTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
		}

		vm.priceChangeChild = priceChangeChild;
		function priceChangeChild(parent, index) {
			var unitPrice = vm.subPekerjaan.ServiceOfferEntryPricelists[parent.$parent.$index].Childs[parent.$index].ServiceOfferEntryPricelistDetails[index].UnitPrice;
			var qty = vm.subPekerjaan.ServiceOfferEntryPricelists[parent.$parent.$index].Childs[parent.$index].ServiceOfferEntryPricelistDetails[index].Quantity;
			var count = vm.subPekerjaan.ServiceOfferEntryPricelists[parent.$parent.$index].Childs;
			var countTotal = vm.subPekerjaan.ServiceOfferEntryPricelists
			var initVal = 0;
			var initTotal = 0;

			vm.subPekerjaan.ServiceOfferEntryPricelists[parent.$parent.$index].Childs[parent.$index].ServiceOfferEntryPricelistDetails[index].TotalPrice = unitPrice * qty;

			for (var i = 0; i < count.length; i++) {
				for (var j = 0; j < count[i].ServiceOfferEntryPricelistDetails.length; j++) {
					initVal = initVal + count[i].ServiceOfferEntryPricelistDetails[j].TotalPrice;
				}
			}
			vm.subPekerjaan.ServiceOfferEntryPricelists[parent.$parent.$index].TotalPrice = initVal;
			vm.subPekerjaan.ServiceOfferEntryPricelists[parent.$parent.$index].TotalPriceStr = initVal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");

			for (var i = 0; i < countTotal.length; i++) {
				initTotal = initTotal + countTotal[i].TotalPrice;
			}
			vm.subPekerjaan.OfferTotalCost = initTotal;
			vm.subPekerjaan.OfferTotalCostStr = initTotal.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
		}

		vm.savePricelist = savePricelist;
		function savePricelist() {
			UIControlService.loadLoading("Silahkan Tunggu");
			OEService.savePricelist({
				ServiceOfferEntryPricelists: vm.subPekerjaan.ServiceOfferEntryPricelists,
				OfferTotalCost: 0,
				OfferEntryVendorID: vm.subPekerjaan.OfferEntryVendorID,
				ID: vm.subPekerjaan.ID
			}, function (reply) {
				if (reply.status === 200) {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("info", 'NOTIFICATION.UPDATE.SUCCESS.MESSAGE', "NOTIFICATION.UPDATE.SUCCESS.TITLE");
				}
			}, function (err) {
				UIControlService.msg_growl("error", "MESSAGE.API");
				UIControlService.unloadLoading();
			});

			//vm.subPekerjaan.OfferTotalCost = initTotal;
		}

		vm.back = back;
		function back() {
		    $state.transitionTo('kelengkapan-datakomersial-jasa-vendor', {
		        TenderRefID: vm.IDTender, StepID: vm.IDStepTender, ProcPackType: vm.ProcPackType, DocID: vm.IDDoc
		    });
		}
	}
})();
