(function () {
	'use strict';

	angular.module("app").controller("ProcurementMethodController", ctrl);

	ctrl.$inject = ['$uibModal', '$translatePartialLoader', 'ProcurementMethodService', 'UIControlService'];

	function ctrl($uibModal, $translatePartialLoader, ProcurementMethodService, UIControlService) {
		var vm = this;

		vm.methods = {};
		vm.allowAdd = true;
		vm.allowEdit = true;
		vm.allowDelete = true;
		vm.currentPage = 1;
		vm.maxSize = 10;
		vm.keyword = '';
		vm.totalItems = 0;
		vm.goodsOrService = 0;

		vm.loadMethods = loadMethods;
		function loadMethods() {
			$translatePartialLoader.addPart('metode-lelang');
			ProcurementMethodService.select({
				Offset: (vm.currentPage - 1) * vm.maxSize,
				Limit: vm.maxSize,
				Keyword: vm.keyword
			}, function (reply) {
			    if (reply.status === 200) {
					vm.methods = reply.data.List;
					vm.totalItems = reply.data.Count;
					vm.goodsOrService = reply.data.GoodsOrService;
				} else {
					//$rootScope.unloadLoading();
					return;
				}
				//$rootScope.unloadLoading();
			}, function (err) {
				//$.growl.error({ message: "Gagal Akses API >" + err });
				//$rootScope.unloadLoading();
			});
		}

		vm.addMethod = addMethod;
		function addMethod() {
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/metodelelang/metodeLelangCreate.html',
				controller: addMethodCtrl,
				controllerAs: 'addMethodCtrl'
			});

			modalInstance.result.then(function () {
				loadMethods();
			});
		}

		vm.editMethod = editMethod;
		function editMethod(id) {
			UIControlService.loadLoading('LOADING.GETSTEP.MESSAGE');
			ProcurementMethodService.isInUse({ MethodID: id }, function (reply) {
				if (reply.status === 200) {
					if (reply.data === true) {
						UIControlService.msg_growl("error", 'NOTIFICATION.ERROR.DATA_IS_IN_USE', "");
						return false;
					} else {
						var post = id;
						var modalInstance = $uibModal.open({
							templateUrl: 'editMethod.html',
							controller: editMethodCtrl,
							controllerAs: 'editMethodCtrl',
							resolve: { item: function () { return post; } }
						});

						modalInstance.result.then(function () {
							loadMethods();
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
			ProcurementMethodService.isInUse({ MethodID: id }, function (reply) {
				if (reply.status === 200) {
				    var post = id;
				    var modalInstance = $uibModal.open({
				        templateUrl: 'inactivate.html',
				        controller: inactivateCtrl,
				        controllerAs: 'inactivateCtrl',
				        resolve: { item: function () { return post; } }
				    });

				    modalInstance.result.then(function (data) {
				        if (data == 1) window.location.reload();
				    });
				}
			}, function (err) {
				UIControlService.unloadLoading();
				UIControlService.msg_growl("error", err, '');
			});
			UIControlService.unloadLoading();
		}

		vm.activate = activate;
		function activate(id) {
			var post = id;
			var modalInstance = $uibModal.open({
				templateUrl: 'activate.html',
				controller: activateCtrl,
				controllerAs: 'activateCtrl',
				resolve: { item: function () { return post; } }
			});

			modalInstance.result.then(function (data) {
			    if (data == 1) window.location.reload();
			});
		}

		vm.viewDetail = viewDetail;
		function viewDetail(id) {
			var post = id;
			var modalInstance = $uibModal.open({
				templateUrl: 'viewMethodDetail.html',
				controller: methodDetailCtrl,
				controllerAs: 'methodDetailCtrl',
				resolve: { item: function () { return post; } }
			});
		}

		vm.pageChanged = pageChanged;
		function pageChanged() {
			loadMethods();
		}
	}
})();

var addMethodCtrl = function ($uibModalInstance, TenderStepService, ProcurementMethodService) {
	var vm = this;

	vm.steps = [];
	vm.formTypes = {};
	vm.tenderSteps = {};
	vm.stepSeq = 0;
	vm.selectedFormType = '';
	vm.selectedStep = 0;
	vm.goodsOrService = '';
	vm.methodName = '';
	vm.lockGoodsService = false;

	//TenderStepService.getFormTypes(function (reply) {
	//	if (reply.status === 200) {
	//		vm.formTypes = reply.data;
	//	} else {
	//	}
	//}, function (err) {
	//	$.growl.error({ message: "Gagal Akses API >" + err });
	//	//$rootScope.unloadLoading();
	//});

	vm.changeGoodsOrSvc = changeGoodsOrSvc;
	function changeGoodsOrSvc() {
		vm.selectedStep = 0;

		TenderStepService.getFormTypes({
			GoodsOrService: vm.goodsOrService
		}, function (reply) {
			if (reply.status === 200) {
				vm.formTypes = reply.data;
			} else {
			}
		}, function (err) {
			$.growl.error({ message: "Gagal Akses API >" + err });
			//$rootScope.unloadLoading();
		});

		ProcurementMethodService.getTenderSteps({
			FormType: vm.selectedFormType,
			GoodsOrService: vm.goodsOrService
		}, function (reply) {
			if (reply.status === 200) {
				vm.tenderSteps = reply.data;
			} else {
			}
		}, function (err) {
			$.growl.error({ message: "Gagal Akses API >" + err });
			//$rootScope.unloadLoading();
		});
	}

	vm.addStep = addStep;
	function addStep() {
		if (vm.selectedFormType === '' || vm.goodsOrService === '' || vm.selectedStep === 0) {
			return false;
		}

		vm.lockGoodsService = true;

		vm.steps.push({
			TenderStepID: vm.selectedStep.TenderStepID,
			TenderStepName: vm.selectedStep.TenderStepName,
			Seq: vm.stepSeq,
			Envelope: 0
		});

		vm.stepSeq++;
	}

	vm.removeStep = removeStep;
	function removeStep(indexNo) {
		vm.steps.splice(indexNo, 1);
	}

	vm.moveUp = moveUp;
	function moveUp(indexNo) {
		var temp1 = vm.steps[indexNo];
		var temp2 = vm.steps[indexNo - 1];

		vm.steps[indexNo] = temp2;
		vm.steps[indexNo - 1] = temp1;
	}

	vm.moveDown = moveDown;
	function moveDown(indexNo) {
		var temp1 = vm.steps[indexNo];
		var temp2 = vm.steps[indexNo + 1];

		vm.steps[indexNo] = temp2;
		vm.steps[indexNo + 1] = temp1;
	}

	vm.createMethod = createMethod;
	function createMethod() {
		if (vm.methodName.trim() == '' || vm.steps.length == 0) {
			return false;
		}

		ProcurementMethodService.createMethod({
			MethodName: vm.methodName,
			MethodDetails: vm.steps
		}, function (reply) {
			if (reply.status === 200) {
				alert('Create Procurement Method Succeed.');
				$uibModalInstance.close();
			} else {
			}
		}, function (err) {
			$.growl.error({ message: "Gagal Akses API >" + err });
			//$rootScope.unloadLoading();
		});
	}

	vm.closeModal = closeModal;
	function closeModal() {
	    $uibModalInstance.close(0);
	}
};

var editMethodCtrl = function (item, $uibModalInstance, TenderStepService, ProcurementMethodService) {
	var vm = this;

	vm.steps = [];
	vm.formTypes = {};
	vm.tenderSteps = {};
	vm.stepSeq = 0;
	vm.selectedFormType = '';
	vm.selectedStep = 0;
	vm.goodsOrService = '';
	vm.methodName = '';
	vm.lockGoodsService = true;

	//TenderStepService.getFormTypes(function (reply) {
	//	if (reply.status === 200) {
	//		vm.formTypes = reply.data;
	//	} else {
	//	}
	//}, function (err) {
	//	$.growl.error({ message: "Gagal Akses API >" + err });
	//	//$rootScope.unloadLoading();
	//});

	ProcurementMethodService.getProcMethodByID({ MethodID: item }, function (reply) {
		if (reply.status === 200) {
			vm.steps = reply.data.MethodDetails;
			vm.methodName = reply.data.MethodName;
			vm.stepSeq = reply.data.MethodDetails.length;
			vm.goodsOrService = reply.data.GoodsOrService.toString();

			TenderStepService.getFormTypes({ GoodsOrService: Number(vm.goodsOrService) }, function (reply) {
				if (reply.status === 200) {
					vm.formTypes = reply.data;
				} else {
				}
			}, function (err) {
				$.growl.error({ message: "Gagal Akses API >" + err });
				//$rootScope.unloadLoading();
			});
		} else {
		}
	}, function (err) {
		$.growl.error({ message: "Gagal Akses API >" + err });
		//$rootScope.unloadLoading();
	});

	vm.changeGoodsOrSvc = changeGoodsOrSvc;
	function changeGoodsOrSvc() {
		vm.selectedStep = 0;

		TenderStepService.getFormTypes({ GoodsOrService: vm.goodsOrService }, function (reply) {
			if (reply.status === 200) {
				vm.formTypes = reply.data;
			} else {
			}
		}, function (err) {
			$.growl.error({ message: "Gagal Akses API >" + err });
			//$rootScope.unloadLoading();
		});

		ProcurementMethodService.getTenderSteps({
			FormType: vm.selectedFormType,
			GoodsOrService: vm.goodsOrService
		}, function (reply) {
			if (reply.status === 200) {
				vm.tenderSteps = reply.data;
			} else {
			}
		}, function (err) {
			$.growl.error({ message: "Gagal Akses API >" + err });
			//$rootScope.unloadLoading();
		});
	}

	vm.addStep = addStep;
	function addStep() {
		if (vm.selectedFormType === '' || vm.goodsOrService === '' || vm.selectedStep === 0) {
			return false;
		}

		vm.steps.push({
			TenderStepID: vm.selectedStep.TenderStepID,
			TenderStepName: vm.selectedStep.TenderStepName,
			Sequence: vm.stepSeq,
			Envelope: 0
		});

		vm.stepSeq++;
	}

	vm.moveUp = moveUp;
	function moveUp(indexNo) {
		var temp1 = vm.steps[indexNo];
		var temp2 = vm.steps[indexNo - 1];

		vm.steps[indexNo] = temp2;
		vm.steps[indexNo - 1] = temp1;
	}

	vm.moveDown = moveDown;
	function moveDown(indexNo) {
		var temp1 = vm.steps[indexNo];
		var temp2 = vm.steps[indexNo + 1];

		vm.steps[indexNo] = temp2;
		vm.steps[indexNo + 1] = temp1;
	}

	vm.removeStep = removeStep;
	function removeStep(indexNo) {
		vm.steps.splice(indexNo, 1);
	}

	vm.editMethod = editMethod;
	function editMethod() {
		if (vm.methodName.trim() == '' || vm.steps.length == 0) {
			return false;
		}

		ProcurementMethodService.editMethod({
			MethodID: item,
			MethodName: vm.methodName,
			MethodDetails: vm.steps
		}, function (reply) {
			if (reply.status === 200) {
				alert('Edit Procurement Method Succeed.');
			} else {
			}
		}, function (err) {
			$.growl.error({ message: "Gagal Akses API >" + err });
			//$rootScope.unloadLoading();
		});

		$uibModalInstance.close();
	}

	vm.closeModal = closeModal;
	function closeModal() {
		$uibModalInstance.close();
	}
};

var inactivateCtrl = function (item, $uibModalInstance, ProcurementMethodService) {
	var vm = this;

	vm.inactivate = inactivate;
	function inactivate() {
		ProcurementMethodService.activateInactivate({ MethodID: item }, function (reply) {
			if (reply.status === 200) {
			    alert('Data Inactivated.');
			} else {
			}
		}, function (err) {
			$.growl.error({ message: "Gagal Akses API >" + err });
			//$rootScope.unloadLoading();
		});

		$uibModalInstance.close(1);
	}

	vm.closeModal = closeModal;
	function closeModal() {
		$uibModalInstance.close();
	}
};

var activateCtrl = function (item, $uibModalInstance, ProcurementMethodService) {
	var vm = this;

	vm.activate = activate;
	function activate() {
		ProcurementMethodService.activateInactivate({ MethodID: item }, function (reply) {
			if (reply.status === 200) {
				alert('Data Activated.');
			} else {
			}
		}, function (err) {
			$.growl.error({ message: "Gagal Akses API >" + err });
			//$rootScope.unloadLoading();
		});

		$uibModalInstance.close(1);
	}

	vm.closeModal = closeModal;
	function closeModal() {
		$uibModalInstance.close();
	}
};

var methodDetailCtrl = function (item, $uibModalInstance, ProcurementMethodService) {
	var vm = this;

	vm.methodName = '';
	vm.stages = [];

	ProcurementMethodService.getProcMethodByID({
		MethodID: item
	}, function (reply) {
		if (reply.status === 200) {
			vm.methodName = reply.data.MethodName;
			vm.stages = reply.data.MethodDetails;
		} else {
		}
	}, function (err) {
		$.growl.error({ message: "Gagal Akses API >" + err });
		//$rootScope.unloadLoading();
	});

	vm.closeModal = closeModal;
	function closeModal() {
		$uibModalInstance.close();
	}
}