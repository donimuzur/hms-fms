(function () {
	'use strict';

	angular.module("app").controller("EvaluationTechnicalCtrl", ctrl);

	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'EvaluationTechnicalService', 'DataPengadaanService', 'RoleService', 'UIControlService', '$uibModal', '$state', '$stateParams', 'GlobalConstantService'];
	function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, EvaluationTechnicalService, DataPengadaanService,
        RoleService, UIControlService, $uibModal, $state, $stateParams, GlobalConstantService) {

		var vm = this;
		var page_id = 141;
		vm.StepID = Number($stateParams.StepID);
		vm.TenderRefID = Number($stateParams.TenderRefID);
		vm.ProcPackType = Number($stateParams.ProcPackType);
		vm.evalsafety = [];
		vm.userBisaMengatur = false;
		vm.allowAdd = true;
		vm.allowEdit = true;
		vm.allowDelete = true;
		vm.init = init;

		vm.jLoad = jLoad;
		//vm.loadAll = loadAll;
		//vm.ubah_aktif = ubah_aktif;
		//vm.tambah = tambah;
		//vm.edit = edit;

		function init() {
			UIControlService.loadLoading("Silahkan Tunggu...");
			jLoad(1);
		}

		vm.jLoad = jLoad;
		function jLoad(current) {
			//console.info("curr "+current)
			vm.evaltechnical = [];
			vm.tenderStepData = {};
			vm.currentPage = current;
			var offset = (current * 10) - 10;
			var tender = {
				ID: vm.StepID,
				tender: {
					TenderRefID: vm.TenderRefID,
					ProcPackageType: vm.ProcPackType
				}
			}
			DataPengadaanService.GetStepByID({ ID: vm.StepID }, function (reply) {
				vm.tenderStepData = reply.data;
				vm.isProcess = vm.tenderStepData.StatusName === "PROCUREMENT_TYPE_PROCESS";
				EvaluationTechnicalService.getSummaryScore({ ID: vm.StepID }, function (reply) {
					UIControlService.unloadLoading();
					if (reply.status === 200) {
						vm.evaltechnical = reply.data;

						EvaluationTechnicalService.isNeedTenderStepApproval({ ID: vm.StepID }, function (result) {
							vm.isNeedApproval = result.data;
							//console.info("jumlahData:" + vm.countRegister);
						}, function (err) {
							$.growl.error({ message: "Gagal mendapatkan data Approval" });
							UIControlService.unloadLoading();
						});

					} else {
						UIControlService.msg_growl("error", "Gagal mendapatkan data Evaluasi Teknis");
						UIControlService.unloadLoading();
					}
				}, function (err) {
					UIControlService.msg_growl("error", "Gagal mendapatkan data Evaluasi Teknis");
					UIControlService.unloadLoading();
				});
			}, function (error) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", "Gagal mendapatkan data Evaluasi Teknis");
			});
		}

		vm.sendToApproval = sendToApproval;
		function sendToApproval() {
			UIControlService.loadLoading('MESSAGE.SENDING');
			EvaluationTechnicalService.sendToApproval({ ID: vm.StepID }, function (reply) {
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

		vm.detail = detail;
		function detail(flag) {
			if (flag === 2) {
				$state.transitionTo('evaluasi-teknis-tim', { TenderRefID: vm.TenderRefID, StepID: vm.StepID, ProcPackType: vm.ProcPackType });
			}
				/*
				else if (flag === 3) {
					$state.transitionTo('data-evaluator', { TenderRefID: vm.TenderRefID, ProcPackType: vm.ProcPackType });
				}
				*/
			else if (flag === 4) {
				var data = {
					tenderStepData: vm.tenderStepData,
					isProcess: vm.isProcess,
					item: vm.evaltechnical
				}
				var modalInstance = $uibModal.open({
					templateUrl: 'app/modules/panitia/evaluasi-teknis/FormEvaluator.html',
					controller: 'FormEvaluator',
					controllerAs: 'FormEvaluator',
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
		}

		vm.view = view;
		function view(data) {
			$state.transitionTo('equipment-evaluation-qualf-teknis', { TenderRefID: vm.TenderRefID, VendorID: data.VendorID, ProcPackType: vm.ProcPackType });
		}

		/*
        vm.Approval = Approval;
        function Approval() {
            var data = {
                item: vm.TenderRefID
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/temporary/safetyEvaluation/DetailApproval.html',
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
        */

		vm.backpengadaan = backpengadaan;
		function backpengadaan() {
			$state.transitionTo('data-pengadaan-tahapan', { TenderRefID: vm.TenderRefID, ProcPackType: vm.ProcPackType });
		}
	}
})();

