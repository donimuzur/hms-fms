(function () {
	'use strict';

	angular.module("app").controller("dataContractReqCtrl", ctrl);

	ctrl.$inject = ['$state', '$filter', '$uibModal', '$translatePartialLoader', 'SocketService', 'DataContractRequisitionService', 'UIControlService'];
	/* @ngInject */
	function ctrl($state, $filter, $uibModal, $translatePartialLoader, SocketService, DataContractRequisitionService, UIControlService) {

		var vm = this;
		var loadmsg = "MESSAGE.LOADING";

		vm.currentPage = 1;
		vm.pageSize = 10;
		vm.totalItems = 0;
		vm.keyword = "";
		vm.column = 1;

		vm.contractRequisition = [];
		vm.statusLabels = [];
		vm.statusLabels["CR_DRAFT"] = 'STATUS.DRAFT';
		vm.statusLabels["CR_PROCESS_1"] = 'STATUS.ON_PROCESS';
		vm.statusLabels["CR_PROCESS_2"] = 'STATUS.APPROVED';
		vm.statusLabels["CR_PROCESS_3"] = 'STATUS.APPROVED';
		vm.statusLabels["CR_REJECT_1"] = 'STATUS.DRAFT';
		vm.statusLabels["CR_REJECT_2"] = 'STATUS.RE_APPROVED';
		vm.statusLabels["CR_REJECT_3"] = 'STATUS.PENDING';
		vm.statusLabels["CR_APPROVED"] = 'STATUS.APPROVED';
		vm.rejectStatusNames = ["CR_REJECT_1", "CR_REJECT_2", "CR_REJECT_3"];

		vm.init = init;
		function init() {
			$translatePartialLoader.addPart('data-contract-requisition');
			DataContractRequisitionService.EvaluateApprovalStatuses(function (reply) {
				vm.loadContracts();
			}, function (error) {
				UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_API'));
				vm.loadContracts();
			});
			vm.currentPage = 1;
		};

		vm.onSearchClick = onSearchClick;
		function onSearchClick(keyword) {
			vm.keyword = keyword;
			vm.loadContracts();
		}

		vm.onFilterTypeChange = onFilterTypeChange;
		function onFilterTypeChange(column) {
			vm.column = column;
		}

		vm.loadContracts = loadContracts;
		function loadContracts() {
			UIControlService.loadLoading(loadmsg);
			DataContractRequisitionService.Select({
				Keyword: vm.keyword,
				Offset: vm.pageSize * (vm.currentPage - 1),
				Limit: vm.pageSize,
				Column: vm.column
			}, function (reply) {
				if (reply.status === 200) {
					UIControlService.unloadLoading();
					vm.contractRequisition = reply.data.List;
					vm.totalItems = reply.data.Count;
				} else {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD'));
				}
			}, function (error) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_LOAD'));
			});
		};

		vm.tambah = tambah;
		function tambah() {
			$state.transitionTo('csms-decision', { contractRequisitionId: 0 });
		};

		vm.sendToApproval = sendToApproval;
		function sendToApproval(dt) {
			UIControlService.loadLoading(loadmsg);
			DataContractRequisitionService.IsCRDataComplete({
				contractRequisitionId: dt.ContractRequisitionId
			}, function (reply) {
				if (reply.data === true) {
					DataContractRequisitionService.IsCostEstimateLineUsedAll({
						contractRequisitionId: dt.ContractRequisitionId
					}, function (reply) {
						if (reply.data === true) {
							if (reply.data === true) {
								DataContractRequisitionService.IsCRDetailSaved({
									contractRequisitionId: dt.ContractRequisitionId
								}, function (reply) {
									if (reply.data === true) {
										UIControlService.unloadLoading();
										sendapproval(dt);
									} else {
										UIControlService.unloadLoading();
										UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_CR_DETAIL_INCOMPLETE'));
									}
								}, function (error) {
									UIControlService.unloadLoading();
									UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_CHECK_CR_DETAIL'));
								});
							} else {
								UIControlService.unloadLoading();
								UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_CR_DATA_INCOMPLETE'));
							}
						} else {
							UIControlService.unloadLoading();
							UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_CE_LINE_INCOMPLETE'));
						}
					}, function (error) {
						UIControlService.unloadLoading();
						UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_CHECK_CE_LINE'));
					});
				} else {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_CR_DATA_INCOMPLETE'));
				}
			}, function (error) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_CHECK_CR'));
			});
		};

		function sendapproval(dt) {
			bootbox.confirm($filter('translate')('CONFIRM.SEND_FOR_APPROVAL'), function (yes) {
				if (yes) {
					UIControlService.loadLoading(loadmsg);
					DataContractRequisitionService.SendToApproval({ contractRequisitionId: dt.ContractRequisitionId }, function (reply) {
						if (reply.status === 200) {
							UIControlService.unloadLoading();
							UIControlService.msg_growl("notice", $filter('translate')('MESSAGE.SUCC_SEND_TO_APPRV'));
							vm.loadContracts();

							SocketService.emit("ContractRequisitionApproval");
						} else {
							UIControlService.unloadLoading();
							UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SEND_TO_APPRV'));
						}
					}, function (error) {
						UIControlService.unloadLoading();
						UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SEND_TO_APPRV'));
					});
				}
			});
		}

		vm.menujuDokumen = menujuDokumen;
		function menujuDokumen(dt) {
			$state.transitionTo('contract-requisition-docs', { contractRequisitionId: dt.ContractRequisitionId });
		};

		vm.menujuApproval = menujuApproval;
		function menujuApproval() {
			$state.transitionTo('contract-requisition-draft-approval');
		};

		vm.detailContract = detailContract;
		function detailContract(dt) {
			$state.transitionTo('detail-contract-requisition', { contractRequisitionId: dt.ContractRequisitionId });
		};

		vm.detailApproval = detailApproval;
		function detailApproval(dt) {
			var item = { contractRequisitionId: dt.ContractRequisitionId };
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/data-contract-requisition/detailApproval.modal.html',
				controller: 'viewDraftApprovalCtrl',
				controllerAs: 'viewDAppCtrl',
				resolve: { item: function () { return item; } }
			});
		};

		vm.buatVariasi = buatVariasi;
		function buatVariasi(dt) {
			$state.transitionTo('contract-variation', { contractRequisitionId: dt.ContractRequisitionId, contractRequisitionVariationId: 0 });
		};

		vm.convertDate = convertDate;
		function convertDate(date) {
			return UIControlService.convertDate(date);
		}
	}
})();