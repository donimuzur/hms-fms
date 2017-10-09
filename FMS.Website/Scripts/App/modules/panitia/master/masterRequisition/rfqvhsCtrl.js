(function () {
	'use strict';

	angular.module("app").controller("RFQVHSController", ctrl);

	ctrl.$inject = ['UIControlService', '$uibModal', '$translatePartialLoader', 'RFQVHSService', '$state'];

	function ctrl(UIControlService, $uibModal, $translatePartialLoader, RFQVHSService, $state) {
		var vm = this;

		vm.totalItems = 0;
		vm.totalItemsApprvls = 0;
		vm.currentPage = 1;
		vm.maxSize = 10;
		vm.keyword = '';
		vm.column = 1;

		vm.findValue = null;
		vm.allowAdd = true;
		vm.allowEdit = true;
		vm.allowDelete = true;
		vm.rfqvhses = [];
		vm.rfqvhsApprvls = [];
		vm.ID = 0;

		vm.getApprovalData = getApprovalData;
		function getApprovalData() {
			$translatePartialLoader.addPart('vhs-requisition');
			//UIControlService.loadLoading('LOADING.GETRFQ.MESSAGE');
			RFQVHSService.getApprovalData({
				Offset: (vm.currentPage - 1) * vm.maxSize,
				Limit: vm.maxSize,
				Keyword: vm.keyword,
				Column: vm.column
			}, function (reply) {
				if (reply.status === 200) {
					vm.rfqvhsApprvls = reply.data.List;
					vm.totalItemsApprvls = reply.data.Count;
					UIControlService.unloadLoading();
				} else {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("error", 'NOTIFICATION.GETRFQ.ERROR', "NOTIFICATION.GETRFQ.TITLE");
				}
			}, function (err) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", 'NOTIFICATION.GETRFQ.ERROR', "NOTIFICATION.GETRFQ.TITLE");
			});
		}

		vm.menujuApproval = menujuApproval;
		function menujuApproval() {
			$state.transitionTo('rfqvhs-draft-approval');
		}

		vm.loadRFQVHS = loadRFQVHS;
		function loadRFQVHS() {
			$translatePartialLoader.addPart('vhs-requisition');
			UIControlService.loadLoading('LOADING.GETRFQ.MESSAGE');
			RFQVHSService.selectRFQ({
				Offset: (vm.currentPage - 1) * vm.maxSize,
				Limit: vm.maxSize,
				Keyword: vm.keyword,
				Column: vm.column
			}, function (reply) {
				if (reply.status === 200) {
					vm.rfqvhses = reply.data.List;
					vm.totalItems = reply.data.Count;
					UIControlService.unloadLoading();
				} else {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("error", 'NOTIFICATION.GETRFQ.ERROR', "NOTIFICATION.GETRFQ.TITLE");
				}
			}, function (err) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", 'NOTIFICATION.GETRFQ.ERROR', "NOTIFICATION.GETRFQ.TITLE");
			});
		}

		vm.addRFQ = addRFQ;
		function addRFQ() {
			var item = {
				ID: 0,
				allowEdit: true
			}
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/master/masterRequisition/addRFQ.html',
				controller: 'addRFQCtrl',
				controllerAs: 'addRFQCtrl',
				resolve: { item: function () { return item; } }
			});

			modalInstance.result.then(function () {
				loadRFQVHS();
			});
		}

		vm.detailApproval = detailApproval;
		function detailApproval(rfqvhsid) {
			$translatePartialLoader.addPart('data-contract-requisition');
			var item = rfqvhsid

			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/master/masterRequisition/detailApproval.html',
				controller: "RFQVHSApprvController",
				controllerAs: "RFQVHSApprvCtrl",
				resolve: { item: function () { vm.ID = item; return item; } }
			});

			modalInstance.result.then(function () {
				loadRFQVHS();
			});
		}

		vm.editRFQ = editRFQ;
		function editRFQ(id, allowEdit) {
			var item = {
				ID: id,
				allowEdit: allowEdit
			}
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/master/masterRequisition/addRFQ.html',
				controller: 'addRFQCtrl',
				controllerAs: 'addRFQCtrl',
				resolve: { item: function () { return item; } }
			});

			modalInstance.result.then(function () {
				loadRFQVHS();
			});
		}

		vm.openFormDokumen = openFormDokumen;
		function openFormDokumen(data) {
			var senddata = { data: data };
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/master/masterRequisition/uploadDokumen.modal.html',
				controller: 'UploadDokumenCtrl',
				controllerAs: 'uploadDokCtrl',
				resolve: {
					item: function () {
						return senddata;
					}
				}
			});
			modalInstance.result.then(function (dataVendor) {
				vm.loadRFQVHS();
			});
		}

		vm.pageChanged = pageChanged;
		function pageChanged() {
			loadRFQVHS();
		}

		vm.sendForApproval = sendForApproval;
		function sendForApproval(data) {
			bootbox.confirm("Apakah anda yakin akan mengirimkan approval?", function (res) {
				if (res) {
					UIControlService.loadLoading('LOADING.SENDAPPROVAL.MESSAGE');
					RFQVHSService.sendForApproval({
						ID: data.ID,
						RFQName: data.RFQName,
						RFQCode: data.RFQCode ? data.RFQCode : "####", //Bypass model validation check
						RFQType: data.RFQType,
						DeliveryTerms: data.DeliveryTerms,
						EvalMethod: data.EvalMethod,
						ProcMethod: data.ProcMethod
					}, function (reply) {
						if (reply.status === 200) {
							UIControlService.unloadLoading();
							UIControlService.msg_growl("notice", 'NOTIFICATION.SENDAPPROVAL.SUCCESS', "NOTIFICATION.SENDAPPROVAL.TITLE");
							loadRFQVHS();
						} else {
							UIControlService.unloadLoading();
							UIControlService.msg_growl("error", 'NOTIFICATION.SENDAPPROVAL.ERROR', "NOTIFICATION.SENDAPPROVAL.TITLE");
						}
					}, function (err) {
						UIControlService.unloadLoading();
						UIControlService.msg_growl("error", 'NOTIFICATION.SENDAPPROVAL.ERROR', "NOTIFICATION.SENDAPPROVAL.TITLE");
					});
				}
			});
		}

		vm.approve = approve;
		function approve(data) {
			bootbox.confirm("Apakah anda yakin akan melakukan approval?", function (res) {
				if (res) {
					UIControlService.loadLoading('LOADING.APPROVE.MESSAGE');
					RFQVHSService.approve({
						RFQVHSId: data.ID,
						RFQName: data.RFQName,
						RFQCode: data.RFQCode ? data.RFQCode : "####", //Bypass model validation check
						RFQType: data.RFQType,
						DeliveryTerms: data.DeliveryTerms,
						EvalMethod: data.EvalMethod,
						ProcMethod: data.ProcMethod
					}, function (reply) {
						if (reply.status === 200) {
							UIControlService.unloadLoading();
							UIControlService.msg_growl("notice", 'NOTIFICATION.APPROVE.SUCCESS', "NOTIFICATION.APPROVE.TITLE");
							getApprovalData();
						} else {
							UIControlService.unloadLoading();
							UIControlService.msg_growl("error", 'NOTIFICATION.APPROVE.FAILED', "NOTIFICATION.APPROVE.TITLE");
						}
					}, function (err) {
						UIControlService.unloadLoading();
						UIControlService.msg_growl("error", 'NOTIFICATION.APPROVE.FAILED', "NOTIFICATION.APPROVE.TITLE");
					});
				}
			});
		}

		vm.reject = reject;
		function reject(data) {
			var item = {
				RFQVHSId: data.ID,
				RFQName: data.RFQName,
				RFQCode: data.RFQCode ? data.RFQCode : "####", //Bypass model validation check
				RFQType: data.RFQType,
				DeliveryTerms: data.DeliveryTerms,
				EvalMethod: data.EvalMethod,
				ProcMethod: data.ProcMethod
			};
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/master/masterRequisition/rejectForm.modal.html',
				controller: 'rejectRFQFormCtrl',
				controllerAs: 'rejectRFQFormCtrl',
				resolve: { item: function () { return item; } }
			});
			modalInstance.result.then(function (remark) {
				rejectRFQ(data, remark);
			});
		}

		function rejectRFQ(data, remark) {
			UIControlService.loadLoading('LOADING.REJECT.MESSAGE');
			RFQVHSService.reject({
				RFQVHSId: data.ID,
				RFQName: data.RFQName,
				RFQCode: data.RFQCode ? data.RFQCode : "####", //Bypass model validation check
				RFQType: data.RFQType,
				DeliveryTerms: data.DeliveryTerms,
				EvalMethod: data.EvalMethod,
				ProcMethod: data.ProcMethod,
				Comment: remark
			}, function (reply) {
				if (reply.status === 200) {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("notice", 'NOTIFICATION.REJECT.SUCCESS', "NOTIFICATION.REJECT.TITLE");
					getApprovalData();
				} else {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("error", 'NOTIFICATION.REJECT.ERROR', "NOTIFICATION.REJECT.TITLE");
				}
			}, function (err) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", 'NOTIFICATION.REJECT.ERROR', "NOTIFICATION.REJECT.TITLE");
			});
		}

		vm.publish = publish;
		function publish(data) {
			UIControlService.loadLoading('LOADING.PUBLISH.MESSAGE');
			RFQVHSService.publish({
				ID: data.ID,
				RFQName: data.RFQName,
				RFQCode: data.RFQCode ? data.RFQCode : "####", //Bypass model validation check
				RFQType: data.RFQType,
				DeliveryTerms: data.DeliveryTerms,
				EvalMethod: data.EvalMethod,
				ProcMethod: data.ProcMethod
			}, function (reply) {
				if (reply.status === 200) {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("notice", 'NOTIFICATION.PUBLISH.SUCCESS', "NOTIFICATION.PUBLISH.TITLE");
					loadRFQVHS();
				} else {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("error", 'NOTIFICATION.PUBLISH.ERROR', "NOTIFICATION.PUBLISH.TITLE");
				}
			}, function (err) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", 'NOTIFICATION.PUBLISH.ERROR', "NOTIFICATION.PUBLISH.TITLE");
			});
		}
	}
})();