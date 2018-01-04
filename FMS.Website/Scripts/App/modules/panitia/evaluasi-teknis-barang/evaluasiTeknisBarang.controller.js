(function () {
	'use strict';

	angular.module("app").controller("evaluasiTeknisBarangCtrl", ctrl);

	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'EvaluasiTeknisBarangService', 'DataPengadaanService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
	function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, EvaluasiTeknisBarangService,
        DataPengadaanService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService) {

	    var vm = this;
	    var loadmsg = "MESSAGE.LOADING";
		vm.StepID = Number($stateParams.StepID);
		vm.TenderRefID = Number($stateParams.TenderRefID);
		vm.ProcPackType = Number($stateParams.ProcPackType);
		vm.evaluations = [];

		vm.init = init;
		function init() {
		    $translatePartialLoader.addPart('evaluasi-teknis-barang');
		    UIControlService.loadLoading(loadmsg);
			loadData();
		}

		function loadData() {
		    UIControlService.loadLoading(loadmsg);
		    DataPengadaanService.GetStepByID({
		        ID: vm.StepID
		    }, function (reply) {
		        vm.tenderStepData = reply.data;
		        vm.isProcess = vm.tenderStepData.StatusName === "PROCUREMENT_TYPE_PROCESS";
		        EvaluasiTeknisBarangService.selectOfferEntries({
		            ID: vm.StepID,
		            TenderID: vm.tenderStepData.TenderID
		        }, function (reply) {
		            UIControlService.unloadLoading();
		            vm.evaluations = reply.data;
                    
		        }, function (error) {
		            UIControlService.unloadLoading();
		            UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
		        });
		    }, function (error) {
		        UIControlService.unloadLoading();
		        UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
		    });
		};

		vm.detail = detail;
		function detail(vendorID) {
            $state.transitionTo('detail-evaluasi-teknis-barang', { TenderRefID: vm.TenderRefID, StepID: vm.StepID, ProcPackType: vm.ProcPackType, VendorID: vendorID });
		}

		vm.kembali = kembali;
		function kembali() {
			$state.transitionTo('data-pengadaan-tahapan', { TenderRefID: vm.TenderRefID, ProcPackType: vm.ProcPackType });
		}
	}
})();

