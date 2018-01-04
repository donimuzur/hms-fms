(function () {
	'use strict';

	angular.module("app").controller("TenderStepController", ctrl);

	ctrl.$inject = ['UIControlService', '$uibModal', '$translatePartialLoader', 'TenderStepService'];

	function ctrl(UIControlService, $uibModal, $translatePartialLoader, TenderStepService) {
		var vm = this;

		vm.findby = null;
		vm.findValue = null;
		vm.allowAdd = true;
		vm.allowEdit = true;
		vm.allowDelete = true;
		vm.tahapans = [];

		vm.totalItems = 0;
		vm.currentPage = 1;
		vm.maxSize = 10;
		vm.keyword = '';
		vm.searchBy = 0;

		vm.init = init;
		function init() {
			$translatePartialLoader.addPart('tahapan-tender');
			loadSteps(1);
		}

		vm.loadSteps = loadSteps;
		function loadSteps() {
			UIControlService.loadLoading('LOADING.GETSTEPS.MESSAGE');
			TenderStepService.select({
				Offset: (vm.currentPage - 1) * vm.maxSize,
				Limit: vm.maxSize,
				Keyword: vm.keyword,
				column: vm.searchBy
			}, function (reply) {
				if (reply.status === 200) {
					vm.tahapans = reply.data.List;
					vm.totalItems = reply.data.Count;
					UIControlService.unloadLoading();
				} else {
					UIControlService.unloadLoading();
					UIControlService.msg_growl("error", 'NOTIFICATION.GET.LOCATION.ERROR', "NOTIFICATION.GET.LOCATION.TITLE");
				}
			}, function (err) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", 'NOTIFICATION.GET.LOCATION.ERROR', "NOTIFICATION.GET.LOCATION.TITLE");
			});
		}

		vm.addstep = addStep;
		function addStep() {
			var modalInstance = $uibModal.open({
				templateUrl: 'addModalStep.html',
				controller: addStepCtrl,
				controllerAs: 'addStepCtrl'
			});

			modalInstance.result.then(function () {
				loadSteps();
			});
		}

		vm.editStep = editStep;
		function editStep(id) {
			UIControlService.loadLoading('LOADING.GETSTEP.MESSAGE');
			TenderStepService.isInUse({ TenderStepID: id }, function (reply) {
				if (reply.status === 200) {
					if (reply.data === true) {
						UIControlService.msg_growl("error", 'NOTIFICATION.ERROR.DATA_IS_IN_USE', "");
						return false;
					} else {
						var post = id;
						var modalInstance = $uibModal.open({
							templateUrl: 'editModalStep.html',
							controller: editStepCtrl,
							controllerAs: 'editStepCtrl',
							resolve: {
								item: function () {
									return post;
								}
							}
						});

						modalInstance.result.then(function () {
							loadSteps();
						});
					}
				}
			}, function (err) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", err, '');
			});
			UIControlService.unloadLoading();
		}

		vm.inactivate = inactivate;
		function inactivate(id) {
			UIControlService.loadLoading('LOADING.GETSTEP.MESSAGE');
			TenderStepService.isInUse({ TenderStepID: id }, function (reply) {
				if (reply.status === 200) {
					if (reply.data === true) {
						UIControlService.msg_growl("error", 'NOTIFICATION.ERROR.DATA_IS_IN_USE', "");
						return false;
					} else {
						var post = id;
						var modalInstance = $uibModal.open({
							templateUrl: 'inactivateTenderStep.html',
							controller: inactivateStepCtrl,
							controllerAs: 'inactivateStepCtrl',
							resolve: { item: function () { return post; } },
						});

						modalInstance.result.then(function () {
							loadSteps();
						});
					}
				}
			}, function (err) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", err, '');
			});
			UIControlService.unloadLoading();
		}

		vm.activate = activate;
		function activate(id) {
			UIControlService.loadLoading('LOADING.GETSTEP.MESSAGE');
			TenderStepService.isInUse({ TenderStepID: id }, function (reply) {
				if (reply.status === 200) {
					var post = id;
					var modalInstance = $uibModal.open({
						templateUrl: 'activateTenderStep.html',
						controller: activateStepCtrl,
						controllerAs: 'activateStepCtrl',
						resolve: { item: function () { return post; } }
					});

					modalInstance.result.then(function () {
						loadSteps();
					});
				}
			}, function (err) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", err, '');
			});
			UIControlService.unloadLoading();
		}

		vm.viewDetail = viewDetail;
		function viewDetail(id) {
			var post = id;
			var modalInstance = $uibModal.open({
				templateUrl: 'viewDetail.html',
				controller: viewDetailCtrl,
				controllerAs: 'viewDetailCtrl',
				resolve: { item: function () { return post; } }
			});
		}

		vm.pageChanged = pageChanged;
		function pageChanged() {
			loadSteps();
		}

		vm.viewStep = viewStep;
		function viewStep() { }
	}
})();

var addStepCtrl = function (UIControlService, $uibModalInstance, TenderStepService) {
	var vm = this;

	vm.goodsOrService;
	vm.selectedFormType;
	vm.stepName = '';
	vm.formTypes = [];
	vm.remark = '';

	TenderStepService.getFormTypes(function (reply) {
		UIControlService.loadLoadingModal('LOADING.VIEW.VENDOR');
		if (reply.status === 200) {
			vm.formTypes = reply.data;
			UIControlService.unloadLoadingModal();
		} else {
			UIControlService.unloadLoadingModal();
			UIControlService.msg_growl("error", 'NOTIFICATION.GET.FORMTYPE.ERROR', "NOTIFICATION.GET.LOCATION.TITLE");
		}
	}, function (err) {
		UIControlService.unloadLoadingModal();
		UIControlService.msg_growl("error", 'NOTIFICATION.GET.LOCATION.ERROR', "NOTIFICATION.GET.LOCATION.TITLE");
	});

	vm.createTenderStep = createTenderStep;
	function createTenderStep() {
		if (vm.goodsOrService === null || vm.selectedFormType === null || vm.stepName.trim() === '') {
			return false;
		}
		UIControlService.loadLoadingModal('LOADING.CREATE.MESSAGE');
		TenderStepService.create({
			FormType: vm.selectedFormType,
			GoodsOrService: vm.goodsOrService.toString(),
			TenderStepName: vm.stepName,
			Remark: vm.remark
		}, function (reply) {
			if (reply.status === 200) {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("success", 'NOTIFICATION.CREATE.SUCCESS.MESSAGE', "NOTIFICATION.CREATE.SUCCESS.TITLE");
				$uibModalInstance.close();
			} else {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("error", 'NOTIFICATION.CREATE.ERROR.MESSAGE', "NOTIFICATION.CREATE.ERROR.TITLE");
			}
		}, function (err) {
			UIControlService.unloadLoadingModal();
			UIControlService.msg_growl("error", err, '');
		});

		//$uibModalInstance.close();
	}

	vm.closeModal = closeModal;
	function closeModal() {
		$uibModalInstance.close();
	}
}

var editStepCtrl = function (UIControlService, item, $uibModalInstance, TenderStepService) {
	var vm = this;

	vm.goodsOrService;
	vm.selectedFormType;
	vm.stepName = '';
	vm.remark = '';
	vm.formTypes = [];

	TenderStepService.getFormTypes(function (reply) {
		UIControlService.loadLoadingModal('LOADING.GET.PROCMETHOD');
		if (reply.status === 200) {
			vm.formTypes = reply.data;
			UIControlService.unloadLoadingModal();
		} else {
			UIControlService.unloadLoadingModal();
			UIControlService.msg_growl("error", 'NOTIFICATION.GET.PROCMETHOD.ERROR', "NOTIFICATION.GET.PROCMETHOD.TITLE");
		}
	}, function (err) {
		UIControlService.unloadLoadingModal();
		UIControlService.msg_growl("error", 'NOTIFICATION.GET.PROCMETHOD.ERROR', "NOTIFICATION.GET.PROCMETHOD.TITLE");
	});

	TenderStepService.getByID({ TenderStepID: item }, function (reply) {
		UIControlService.loadLoadingModal('LOADING.GET.PROCMETHOD');
		if (reply.status === 200) {
			vm.goodsOrService = reply.data.GoodsOrService.toString();
			vm.selectedFormType = reply.data.FormType;
			vm.stepName = reply.data.TenderStepName;
			vm.remark = reply.data.Remark;
			UIControlService.unloadLoadingModal();
		} else {
			UIControlService.unloadLoadingModal();
			UIControlService.msg_growl("error", 'NOTIFICATION.GET.PROCMETHOD.ERROR', "NOTIFICATION.GET.PROCMETHOD.TITLE");
		}
	}, function (err) {
		UIControlService.unloadLoadingModal();
		UIControlService.msg_growl("error", 'NOTIFICATION.GET.PROCMETHOD.ERROR', "NOTIFICATION.GET.PROCMETHOD.TITLE");
	});

	vm.updateTenderStep = updateTenderStep;
	function updateTenderStep() {
		if (vm.goodsOrService === null || vm.selectedFormType === null || vm.stepName.trim() === '') {
			return false;
		}
		UIControlService.loadLoadingModal('LOADING.EDIT.MESSAGE');
		TenderStepService.update({
			FormType: vm.selectedFormType,
			GoodsOrService: vm.goodsOrService,
			TenderStepName: vm.stepName,
			TenderStepID: item,
			Remark: vm.remark,
		}, function (reply) {
			if (reply.status === 200) {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("success", 'NOTIFICATION.CREATE.SUCCESS.MESSAGE', "NOTIFICATION.CREATE.SUCCESS.TITLE");
				$uibModalInstance.close();
			} else {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("error", 'NOTIFICATION.EDIT.ERROR.MESSAGE', "NOTIFICATION.EDIT.ERROR.TITLE");
			}
		}, function (err) {
			UIControlService.unloadLoadingModal();
			UIControlService.msg_growl("error", 'NOTIFICATION.EDIT.ERROR.MESSAGE', "NOTIFICATION.EDIT.ERROR.TITLE");
		});
	}

	vm.closeModal = closeModal;
	function closeModal() {
		$uibModalInstance.close();
	}
}

var inactivateStepCtrl = function (UIControlService, item, $uibModalInstance, TenderStepService) {
	var vm = this;

	vm.inactivateTenderStep = inactivateTenderStep;
	function inactivateTenderStep() {
		UIControlService.loadLoadingModal('LOADING.INACTIVATE.MESSAGE');
		TenderStepService.inactivate({ TenderStepID: item }, function (reply) {
			if (reply.status === 200) {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("success", 'NOTIFICATION.INACTIVATE.SUCCESS.MESSAGE', "NOTIFICATION.INACTIVATE.SUCCESS.TITLE");
				$uibModalInstance.close();
			} else {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("error", 'NOTIFICATION.INACTIVATE.ERROR.MESSAGE', "NOTIFICATION.INACTIVATE.ERROR.TITLE");
			}
		}, function (err) {
			UIControlService.unloadLoadingModal();
			UIControlService.msg_growl("error", 'NOTIFICATION.INACTIVATE.ERROR.MESSAGE', "NOTIFICATION.INACTIVATE.ERROR.TITLE");
		});
	}

	vm.closeModal = closeModal;
	function closeModal() {
		$uibModalInstance.close();
	}
}

var activateStepCtrl = function (UIControlService, item, $uibModalInstance, TenderStepService) {
	var vm = this;

	vm.activateTenderStep = activateTenderStep;
	function activateTenderStep() {
		UIControlService.loadLoadingModal('LOADING.ACTIVATE.MESSAGE');
		TenderStepService.activate({ TenderStepID: item }, function (reply) {
			if (reply.status === 200) {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("success", 'NOTIFICATION.ACTIVATE.SUCCESS.MESSAGE', "NOTIFICATION.ACTIVATE.SUCCESS.TITLE");
				$uibModalInstance.close();
			} else {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("error", 'NOTIFICATION.ACTIVATE.ERROR.MESSAGE', "NOTIFICATION.ACTIVATE.ERROR.TITLE");
			}
		}, function (err) {
			UIControlService.unloadLoadingModal();
			UIControlService.msg_growl("error", 'NOTIFICATION.ACTIVATE.ERROR.MESSAGE', "NOTIFICATION.ACTIVATE.ERROR.TITLE");
		});
	}

	vm.closeModal = closeModal;
	function closeModal() {
		$uibModalInstance.close();
	}
}

var viewDetailCtrl = function (UIControlService, item, $uibModalInstance, TenderStepService) {
	var vm = this;

	vm.tenderStepName = '';
	vm.remark = '';

	UIControlService.loadLoadingModal('LOADING.VIEW.MESSAGE');
	TenderStepService.getByID({ TenderStepID: item }, function (reply) {
		if (reply.status === 200) {
			vm.formTypeName = reply.data.FormTypeName;
			vm.tenderStepName = reply.data.TenderStepName;
			vm.remark = reply.data.Remark;
			UIControlService.unloadLoadingModal();
		} else {
			UIControlService.unloadLoadingModal();
			UIControlService.msg_growl("error", 'NOTIFICATION.VIEW.ERROR.MESSAGE', "NOTIFICATION.VIEW.ERROR.TITLE");
		}
	}, function (err) {
		UIControlService.unloadLoadingModal();
		UIControlService.msg_growl("error", 'NOTIFICATION.VIEW.ERROR.MESSAGE', "NOTIFICATION.VIEW.ERROR.TITLE");
	});

	vm.closeModal = closeModal;
	function closeModal() {
		$uibModalInstance.close();
	}
}