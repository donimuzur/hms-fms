(function () {
	'use strict';

	angular.module("app")
    .controller("dataContractReqDraftApprovalCtrl", ctrl);

	ctrl.$inject = ['$state', '$http', '$filter', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'DataContractRequisitionService', 'UIControlService'];
	/* @ngInject */
	function ctrl($state, $http, $filter, $uibModal, $translate, $translatePartialLoader, $location, SocketService, DataContractRequisitionService, UIControlService) {

		var vm = this;
		var loadmsg = "MESSAGE.LOADING";

		vm.currentPage = 1;
		vm.pageSize = 10;
		vm.totalItems = 0;
		vm.keyword = "";
		vm.column = 1;

		vm.contractRequisition = [];

		vm.statusLabels = [];
		vm.statusLabels["CR_PROCESS_2"] = 'STATUS.APPROVED';
		vm.statusLabels["CR_REJECT_1"] = 'STATUS.REJECTED';
		vm.statusLabels["CR_PROCESS_1"] = 'STATUS.ON_PROCESS';


		vm.init = init;
		function init() {
			$translatePartialLoader.addPart('data-contract-requisition');
			vm.loadContracts();
		};

		vm.onSearchClick = onSearchClick;
		function onSearchClick(keyword) {
			vm.keyword = keyword;
			vm.loadContracts();
			vm.currentPage = 1;
		}

		vm.onFilterTypeChange = onFilterTypeChange;
		function onFilterTypeChange(column) {
			vm.column = column;
		}

		vm.loadContracts = loadContracts;
		function loadContracts() {
			UIControlService.loadLoading(loadmsg);
			DataContractRequisitionService.SelectByApprover({
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

		vm.detailApproval = detailApproval;
		function detailApproval(dt) {
			var item = {
				contractRequisitionId: dt.ContractRequisitionId
			};
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/data-contract-requisition/draft-approval/detailApproval.modal.html',
				controller: 'detailDraftApprovalCtrl',
				controllerAs: 'detDAppCtrl',
				resolve: { item: function () { return item; } }
			});
			modalInstance.result.then(function () {
				loadContracts();
			});
		};

		vm.menujuDokumen = menujuDokumen;
		function menujuDokumen(dt) {
			$state.transitionTo('contract-requisition-docs-da', { contractRequisitionId: dt.ContractRequisitionId });
		};

		vm.detailContract = detailContract;
		function detailContract(dt) {
			console.info('tes');
			$state.transitionTo('detail-contract-requisition-da', { contractRequisitionId: dt.ContractRequisitionId });
		};

		vm.approve = approve;
		function approve(dt) {
			bootbox.confirm($filter('translate')('CONFIRM.APPROVE_CRDRAFT'), function (yes) {
				if (yes) {
					sendApproval(true, dt.ContractRequisitionId, null);
				}
			});
		}

		vm.reject = reject;
		function reject(dt) {
			var item = {
				contractRequisitionId: dt.ContractRequisitionId,
				projectTitle: dt.ProjectTitle
			};
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/data-contract-requisition/draft-approval/rejectForm.modal.html',
				controller: 'rejectFormCtrl',
				controllerAs: 'rejectFormCtrl',
				resolve: { item: function () { return item; } }
			});
			modalInstance.result.then(function (remark) {
				sendApproval(false, dt.ContractRequisitionId, remark);
			});
		}

		function sendApproval(approvalStatus, contractRequisitionId, remark) {
			UIControlService.loadLoading(loadmsg);
			DataContractRequisitionService.SetApprovalStatus({
				ContractRequisitionID: contractRequisitionId,
				ApprovalStatus: approvalStatus,
				Remark: remark
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.loadContracts();
					SocketService.emit("ContractRequisitionApproval");
				} else {
					UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SET_APPROVAL'));
				}
			}, function (error) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", $filter('translate')('MESSAGE.ERR_SET_APPROVAL'));
			});
		}

		vm.convertDate = convertDate;
		function convertDate(date) {
			return UIControlService.convertDate(date);
		};
	}
})();