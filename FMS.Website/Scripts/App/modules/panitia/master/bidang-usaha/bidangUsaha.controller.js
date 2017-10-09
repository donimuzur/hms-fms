(function () {
	'use strict';

	angular.module("app").controller("businessFieldCtrl", ctrl);

	ctrl.$inject = ['UIControlService', '$translatePartialLoader', '$uibModal', 'BusinessFieldService'];

	function ctrl(UIControlService, $translatePartialLoader, $uibModal, BusinessFieldService) {
		var vm = this;

		vm.totalItems = 0;
		vm.currentPage = 1;
		vm.maxSize = 10;
		vm.kata = "";
		vm.businessFields = [];

		vm.allowControl = true;
		vm.allowAdd = true;
		vm.allowEdit = true;
		vm.allowDelete = true;

		vm.loadBusinessFields = loadBusinessFields;
		function loadBusinessFields() {
			$translatePartialLoader.addPart('master-bidang-usaha');
			UIControlService.loadLoading('LOADING.GETBUSSFIELD.MESSAGE');
			jLoad(1);
		}

		vm.jLoad = jLoad;
		function jLoad(current) {
		    //console.info("curr "+current)
		    UIControlService.loadLoading("Silahkan Tunggu...");
		    vm.currentPage = current;
		    var offset = (current * 10) - 10;
		    BusinessFieldService.select({
		        Offset: offset,
		        Limit: vm.maxSize,
		        Keyword: vm.kata
		    }, function (reply) {
		        console.info("data:" + JSON.stringify(reply));
		        UIControlService.unloadLoading();
		        if (reply.status === 200) {
		            var data = reply.data;
		            vm.businessFields = data.List;
		            vm.totalItems = Number(data.Count);
		        } else {
		            $.growl.error({ message: "Gagal mendapatkan data Master Departemen" });
		            UIControlService.unloadLoading();
		        }
		    }, function (err) {
		        console.info("error:" + JSON.stringify(err));
		        //$.growl.error({ message: "Gagal Akses API >" + err });
		        UIControlService.unloadLoading();
		    });
		}

		vm.onSearchSubmit = onSearchSubmit;
		function onSearchSubmit() {
		    vm.jLoad(1);
		}

		vm.addBusinessField = addBusinessField;
		function addBusinessField() {
			var modalInstance = $uibModal.open({
				templateUrl: 'addBusinessField.html',
				controller: addBusinessFieldCtrl,
				controllerAs: 'addBusinessFieldCtrl'
			});
			modalInstance.result.then(function () {
				loadBusinessFields();
			});
		}

		vm.editBusinessField = editBusinessField;
		function editBusinessField(id) {
			var post = id;
			var modalInstance = $uibModal.open({
				templateUrl: 'editBusinessField.html',
				controller: editBusinessFieldCtrl,
				controllerAs: 'editBusinessFieldCtrl',
				resolve: { item: function () { return post; } }
			});

			modalInstance.result.then(function () {
				loadBusinessFields();
			});
		}

		vm.inactivateBusinessField = inactivateBusinessField;
		function inactivateBusinessField(id) {
			var post = id;
			var modalInstance = $uibModal.open({
				templateUrl: 'inactivateBusinessField.html',
				controller: inactivateBusinessFieldCtrl,
				controllerAs: 'inactivateBusinessFieldCtrl',
				resolve: { item: function () { return post; } }
			});

			modalInstance.result.then(function () {
				loadBusinessFields();
			});
		}

		vm.activateBusinessField = activateBusinessField;
		function activateBusinessField(id) {
			var post = id;
			var modalInstance = $uibModal.open({
				templateUrl: 'activateBusinessField.html',
				controller: inactivateBusinessFieldCtrl,
				controllerAs: 'inactivateBusinessFieldCtrl',
				resolve: { item: function () { return post; } }
			});

			modalInstance.result.then(function () {
				loadBusinessFields();
			});
		}
	}
})(); // end BidangUsahaCtrl

//TODO
/*fungsi addModalbidangUsaha*/
var addBusinessFieldCtrl = function (UIControlService, $uibModalInstance, BusinessFieldService) {
	var vm = this;

	vm.licenses = [];
	vm.commodities = [];
	vm.availLicenses = [];
	vm.availCommodities = [];
	vm.selectedLicense = null;
	vm.selectedAvailLicense = null;
	vm.selectedCommodity = null;
	vm.selectedAvailCommodity = null;
	vm.goodsOrService = null;
	vm.isCore = null;
	vm.name = '';

	BusinessFieldService.getCommodities({}, function (reply) {
		UIControlService.loadLoadingModal('LOADING.GET.COMMODITIES');
		if (reply.status === 200) {
			vm.availCommodities = reply.data.Commodities;
			UIControlService.unloadLoadingModal();
		} else {
			UIControlService.unloadLoadingModal();
			UIControlService.msg_growl("error", 'NOTIFICATION.GET.COMMODITIES.ERROR', "NOTIFICATION.GET.COMMODITIES.TITLE");
		}
	}, function (err) {
		UIControlService.unloadLoadingModal();
		UIControlService.msg_growl("error", 'NOTIFICATION.GET.COMMODITIES.ERROR', "NOTIFICATION.GET.COMMODITIES.TITLE");
	});

	BusinessFieldService.getLicenses({}, function (reply) {
		UIControlService.loadLoadingModal('LOADING.GET.LICENSES');
		if (reply.status === 200) {
			vm.availLicenses = reply.data.Licenses;
			UIControlService.unloadLoadingModal();
		} else {
			UIControlService.unloadLoadingModal();
			UIControlService.msg_growl("error", 'NOTIFICATION.GET.LICENSES.ERROR', "NOTIFICATION.GET.LICENSES.TITLE");
		}
	}, function (err) {
		UIControlService.unloadLoadingModal();
		UIControlService.msg_growl("error", 'NOTIFICATION.GET.LICENSES.ERROR', "NOTIFICATION.GET.LICENSES.TITLE");
	});

	vm.addLicense = addLicense;
	function addLicense() {
		if (vm.selectedAvailLicense === null)
			return false;

		vm.licenses.push(vm.selectedAvailLicense);
		vm.availLicenses = BusinessFieldService.remove(vm.availLicenses, vm.selectedAvailLicense);
	}

	vm.removeLicense = removeLicense;
	function removeLicense() {
		if (vm.selectedLicense === null) return false;

		vm.availLicenses.push(vm.selectedLicense);
		vm.licenses = BusinessFieldService.remove(vm.licenses, vm.selectedLicense);
	}

	vm.addCommodity = addCommodity;
	function addCommodity() {
		if (vm.selectedAvailCommodity === null) return false;

		vm.commodities.push(vm.selectedAvailCommodity);
		vm.availCommodities = BusinessFieldService.remove(vm.availCommodities, vm.selectedAvailCommodity);
	}

	vm.removeCommodity = removeCommodity;
	function removeCommodity() {
		if (vm.selectedCommodity === null) return false;

		vm.availCommodities.push(vm.selectedCommodity);
		vm.commodities = BusinessFieldService.remove(vm.commodities, vm.selectedCommodity);
	}

	vm.saveBusinessField = saveBusinessField;
	function saveBusinessField() {
		if (vm.goodsOrService === null || vm.isCore === null || vm.name.trim() === '') {
			return false;
		}

		UIControlService.loadLoadingModal('LOADING.CREATE.MESSAGE');
		BusinessFieldService.create({
			GoodsOrService: vm.goodsOrService,
			IsCore: vm.isCore,
			Name: vm.name,
			Licenses: vm.licenses,
			Commodities: vm.commodities
		}, function (reply) {
			if (reply.status === 200) {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("info", 'NOTIFICATION.CREATE.SUCCESS.MESSAGE', "NOTIFICATION.CREATE.SUCCESS.TITLE");
				$uibModalInstance.close();
			} else {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("error", 'NOTIFICATION.CREATE.ERROR.MESSAGE', "NOTIFICATION.CREATE.ERROR.TITLE");
			}
		}, function (err) {
			UIControlService.unloadLoadingModal();
			UIControlService.msg_growl("error", 'NOTIFICATION.CREATE.ERROR.MESSAGE', "NOTIFICATION.CREATE.ERROR.TITLE");
		});
	}

	vm.cancel = cancel;
	function cancel() {
		$uibModalInstance.close();
	}
};

//TODO
/* controller editModal BidangUsaha*/
var editBusinessFieldCtrl = function (UIControlService, $uibModalInstance, item, BusinessFieldService) {
	var vm = this;

	vm.licenses = [];
	vm.commodities = [];
	vm.availLicenses = [];
	vm.availCommodities = [];
	vm.selectedLicense = null;
	vm.selectedAvailLicense = null;
	vm.selectedCommodity = null;
	vm.selectedAvailCommodity = null;
	vm.goodsOrService = null;
	vm.isCore = null;
	vm.name = '';

	BusinessFieldService.getByID({ ID: item }, function (reply) {
		UIControlService.loadLoadingModal('LOADING.GET.BUSSFIELD');
		if (reply.status === 200) {
			vm.goodsOrService = reply.data[0].GoodsOrService.toString();
			vm.name = reply.data[0].Name;
			vm.isCore = reply.data[0].IsCore.toString();
			vm.commodities = reply.data[0].Commodities;
			vm.licenses = reply.data[0].Licenses;
			UIControlService.unloadLoadingModal();
		} else {
			UIControlService.unloadLoadingModal();
			UIControlService.msg_growl("error", 'NOTIFICATION.GET.BUSSFIELD.ERROR', "NOTIFICATION.GET.BUSSFIELD.TITLE");
		}
	}, function (err) {
		UIControlService.unloadLoadingModal();
		UIControlService.msg_growl("error", 'NOTIFICATION.GET.BUSSFIELD.ERROR', "NOTIFICATION.GET.BUSSFIELD.TITLE");
	});

	BusinessFieldService.getLicenses({ ID: item }, function (reply) {
		UIControlService.loadLoadingModal('LOADING.GET.LICENSES');
		if (reply.status === 200) {
			vm.availLicenses = reply.data.Licenses;
			UIControlService.unloadLoadingModal();
		} else {
			UIControlService.unloadLoadingModal();
			UIControlService.msg_growl("error", 'NOTIFICATION.GET.LICENSES.ERROR', "NOTIFICATION.GET.LICENSES.TITLE");
		}
	}, function (err) {
		UIControlService.unloadLoadingModal();
		UIControlService.msg_growl("error", 'NOTIFICATION.GET.LICENSES.ERROR', "NOTIFICATION.GET.LICENSES.TITLE");
	});

	BusinessFieldService.getCommodities({ ID: item }, function (reply) {
		UIControlService.loadLoadingModal('LOADING.GET.COMMODITIES');
		if (reply.status === 200) {
			vm.availCommodities = reply.data.Commodities;
			UIControlService.unloadLoadingModal();
		} else {
			UIControlService.unloadLoadingModal();
			UIControlService.msg_growl("error", 'NOTIFICATION.GET.COMMODITIES.ERROR', "NOTIFICATION.GET.COMMODITIES.TITLE");
		}
	}, function (err) {
		UIControlService.unloadLoadingModal();
		UIControlService.msg_growl("error", 'NOTIFICATION.GET.COMMODITIES.ERROR', "NOTIFICATION.GET.COMMODITIES.TITLE");
	});

	vm.addLicense = addLicense;
	function addLicense() {
		if (vm.selectedAvailLicense === null)
			return false;

		vm.licenses.push(vm.selectedAvailLicense);
		vm.availLicenses = BusinessFieldService.remove(vm.availLicenses, vm.selectedAvailLicense);
	}

	vm.removeLicense = removeLicense;
	function removeLicense() {
		if (vm.selectedLicense === null)
			return false;

		vm.availLicenses.push(vm.selectedLicense);
		vm.licenses = BusinessFieldService.remove(vm.licenses, vm.selectedLicense);
	}

	vm.addCommodity = addCommodity;
	function addCommodity() {
		if (vm.selectedAvailCommodity === null)
			return false;

		vm.commodities.push(vm.selectedAvailCommodity);
		vm.availCommodities = BusinessFieldService.remove(vm.availCommodities, vm.selectedAvailCommodity);
	}

	vm.removeCommodity = removeCommodity;
	function removeCommodity() {
		if (vm.selectedCommodity === null)
			return false;

		vm.availCommodities.push(vm.selectedCommodity);
		vm.commodities = BusinessFieldService.remove(vm.commodities, vm.selectedCommodity);
	}

	vm.editBusinessField = editBusinessField;
	function editBusinessField() {
		if (vm.goodsOrService === null || vm.isCore === null || vm.name.trim() === '') {
			return false;
		}
		UIControlService.loadLoadingModal('LOADING.EDIT.MESSAGE');
		BusinessFieldService.update({
			ID: item,
			GoodsOrService: vm.goodsOrService,
			IsCore: vm.isCore,
			Name: vm.name,
			Commodities: vm.commodities,
			Licenses: vm.licenses
		}, function (reply) {
			if (reply.status === 200) {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("info", 'NOTIFICATION.EDIT.SUCCESS.MESSAGE', "NOTIFICATION.EDIT.SUCCESS.TITLE");
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

	vm.cancel = cancel;
	function cancel() {
		$uibModalInstance.close();
	}
};

//TODO
var inactivateBusinessFieldCtrl = function (UIControlService, item, $uibModalInstance, BusinessFieldService) {
	var vm = this;

	vm.inactivate = inactivate;
	function inactivate() {
		BusinessFieldService.inactivate({ ID: item }, function (reply) {
			UIControlService.loadLoadingModal('LOADING.INACTIVATE.MESSAGE');
			if (reply.status === 200) {
				UIControlService.unloadLoadingModal();
				UIControlService.msg_growl("info", 'NOTIFICATION.INACTIVATE.SUCCESS.MESSAGE', "NOTIFICATION.INACTIVATE.SUCCESS.TITLE");
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
};
