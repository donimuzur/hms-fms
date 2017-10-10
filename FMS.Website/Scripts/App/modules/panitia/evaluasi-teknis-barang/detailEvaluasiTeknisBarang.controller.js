(function () {
	'use strict';

	angular.module("app").controller("detailEvaluasiTeknisBarangCtrl", ctrl);

	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'EvaluasiTeknisBarangService', 'DataPengadaanService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
	function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, EvaluasiTeknisBarangService,
        DataPengadaanService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService) {

	    var vm = this;
	    var loadmsg = "MESSAGE.LOADING";
		vm.StepID = Number($stateParams.StepID);
		vm.TenderRefID = Number($stateParams.TenderRefID);
		vm.ProcPackType = Number($stateParams.ProcPackType);
		vm.VendorID = Number($stateParams.VendorID);
		vm.folderFile = GlobalConstantService.getConstant('api') + "/";

		vm.keyword = "";
		vm.column = 1;
		vm.pageNumber = 1;
		vm.pageSize = 10;
		vm.count = 0;

		vm.evaluationScore = {};
		vm.evaluations = [];

		vm.init = init;
		function init() {
		    $translatePartialLoader.addPart('evaluasi-teknis-barang');
		    UIControlService.loadLoading(loadmsg);
		    EvaluasiTeknisBarangService.isEvaluator({
		        TenderRefID: vm.TenderRefID
		    }, function (reply) {
		        if (reply.data === true) {
		            DataPengadaanService.GetStepByID({
		                ID: vm.StepID
		            }, function (reply) {
		                UIControlService.unloadLoading();
		                vm.tenderStepData = reply.data;
		                vm.isProcess = vm.tenderStepData.StatusName === "PROCUREMENT_TYPE_PROCESS";
		                loadScore();
		            }, function (error) {
		                UIControlService.unloadLoading();
		                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
		            });
		        } else {
		            UIControlService.unloadLoading();
		            UIControlService.msg_growl('error', "MESSAGE.ERR_NOT_EVALUATOR");
		            vm.kembali();
		        }
		    }, function (err) {
		        UIControlService.msg_growl('error', "MESSAGE.ERR_NOT_EVALUATOR");
		        UIControlService.unloadLoading();
		    });
		}

		function loadScore() {
		    UIControlService.loadLoading(loadmsg);
		    EvaluasiTeknisBarangService.getEvaluationScore({
		        tenderStepDataID: vm.StepID,
		        VendorID: vm.VendorID
		    }, function (reply) {
		        UIControlService.unloadLoading();
		        vm.evaluationScore = reply.data;
		        loadEvaluations();
		    }, function (error) {
		        UIControlService.unloadLoading();
		        UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
		    });
		};

		vm.loadEvaluations = loadEvaluations;
		function loadEvaluations() {
		    UIControlService.loadLoading(loadmsg);
		    EvaluasiTeknisBarangService.selectEvaluations({
		        Keyword2: vm.VendorID,
		        Keyword3: vm.tenderStepData.tender.ID,
		        Parameter: vm.tenderStepData.TenderID,
		        Keyword: vm.keyword,
		        column: vm.column,
		        Offset: (vm.pageNumber - 1) * vm.pageSize,
		        Limit: vm.pageSize
		    }, function (reply) {
		        UIControlService.unloadLoading();
		        vm.evaluations = reply.data.List;
		        vm.Count = reply.data.Count;
		    }, function (error) {
		        UIControlService.unloadLoading();
		        UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
		    });
		}

		vm.saveEvaluation = saveEvaluation;
		function saveEvaluation(item) {
		    UIControlService.loadLoading(loadmsg);
		    EvaluasiTeknisBarangService.setEvaluationResult({
		        GOEDId: item.GOEDId,
		        IsPassed: item.IsPassed,
		        TenderStepDataID: vm.StepID
		    }, function (reply) {
		        UIControlService.unloadLoading();
		        UIControlService.msg_growl("success", 'MESSAGE.SUCC_SAVE_EVALUATION');
		        loadScore();
		    }, function (error) {
		        UIControlService.unloadLoading();
		        UIControlService.msg_growl("error", 'MESSAGE.ERR_SAVE_EVALUATION');
		        loadScore();
		    });
		}

		vm.saveScore = saveScore;
		function saveScore() {
		    UIControlService.loadLoading(loadmsg);
		    EvaluasiTeknisBarangService.setEvaluationScore({
		        VendorID: vm.VendorID,
		        Score: vm.evaluationScore.Score,
		        TenderStepDataID: vm.StepID
		    }, function (reply) {
		        UIControlService.unloadLoading();
		        UIControlService.msg_growl("success", 'MESSAGE.SUCC_SAVE_SCORE');
		        loadScore();
		    }, function (error) {
		        UIControlService.unloadLoading();
		        UIControlService.msg_growl("error", 'MESSAGE.ERR_SAVE_SCORE');
		        loadScore();
		    });
		}

		vm.kembali = kembali;
		function kembali() {
		    $state.transitionTo('evaluasi-teknis-barang', { TenderRefID: vm.TenderRefID, StepID: vm.StepID, ProcPackType: vm.ProcPackType });
		}
	}
})();

