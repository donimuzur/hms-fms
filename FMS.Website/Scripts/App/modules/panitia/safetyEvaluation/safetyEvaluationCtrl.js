(function () {
	'use strict';

	angular.module("app").controller("EvaluationSafetyCtrl", ctrl);

	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'EvalSafetyService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
	function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, EvalSafetyService,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService) {

		var vm = this;
		var page_id = 141;
		vm.StepID = Number($stateParams.StepID);
		vm.TenderRefID = Number($stateParams.TenderRefID);
		vm.ProcPackType = Number($stateParams.ProcPackType);
		vm.evalsafety = [];
		vm.totalItems = 0;
		vm.currentPage = 1;
		vm.pageSize = 10;
		vm.userBisaMengatur = false;
		vm.allowAdd = true;
		vm.allowEdit = true;
		vm.allowDelete = true;
		vm.kata = new Kata("");
		vm.init = init;

		vm.folderFile = GlobalConstantService.getConstant('api') + "/";

		//vm.loadDepartemen = loadDepartemen;
		vm.cariEvalSafety = cariEvalSafety;
		vm.jLoad = jLoad;
		//vm.loadAll = loadAll;
		//vm.ubah_aktif = ubah_aktif;
		//vm.tambah = tambah;
		//vm.edit = edit;

		function init() {
			UIControlService.loadLoading("Silahkan Tunggu...");
			jLoad(1);

		}
		vm.cariEvalSafety = cariEvalSafety;
		function cariEvalSafety() {
			vm.jLoad(1);
		}

		vm.jLoad = jLoad;
		function jLoad(current) {
			//console.info("curr "+current)
			vm.evalsafety = [];
			vm.currentPage = current;
			var offset = (current * 10) - 10;
			var tender = {
				ID: vm.StepID,
				tender: {
					TenderRefID: vm.TenderRefID,
					ProcPackageType: vm.ProcPackType
				}
			}
			EvalSafetyService.select(tender, function (reply) {
				//console.info("data:"+JSON.stringify(reply));
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.evalsafety = reply.data;
					console.info("data:" + JSON.stringify(vm.evalsafety));

					EvalSafetyService.isNeedTenderStepApproval(tender, function (result) {
						vm.isNeedApproval = result.data;
						//console.info("jumlahData:" + vm.countRegister);
					}, function (err) {
						$.growl.error({ message: "Gagal mendapatkan data Approval" });
						UIControlService.unloadLoading();
					});

				} else {
					$.growl.error({ message: "Gagal mendapatkan data Evalasi Safety" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				console.info("error:" + JSON.stringify(err));
				//$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}

		vm.detail = detail;
		function detail(id, vendorID, flag) {
			if (flag === true) {
				$state.transitionTo('detail-evaluasi-safety', { StepID: vm.StepID, VendorID: vendorID });
			}
		}

		vm.sendToApproval = sendToApproval;
		function sendToApproval() {
			UIControlService.loadLoading('MESSAGE.SENDING');
			EvalSafetyService.sendToApproval({ ID: vm.StepID }, function (reply) {
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


		vm.Approval = Approval;
		function Approval() {
			var data = {
				item: vm.TenderRefID
			}
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/safetyEvaluation/DetailApproval.html',
				controller: 'DetailApprovalCtrl',
				controllerAs: 'DetailApprovalCtrl',
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

		vm.kembali = kembali;
		function kembali() {
			$state.transitionTo('data-pengadaan-tahapan', { TenderRefID: vm.TenderRefID, ProcPackType: vm.ProcPackType });
		}
	}
})();
//TODO

function Kata(srcText) {
	var self = this;
	self.srcText = srcText;
}

