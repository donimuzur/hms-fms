(function () {
	'use strict';

	angular.module("app").controller("FormActiveVendorCtrl", ctrl);
	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 
        'VerifikasiDataService', 'UIControlService', 'item', '$uibModalInstance', '$uibModal', 'GlobalConstantService'];
	/* @ngInject */
	function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, 
        VerifikasiDataService, UIControlService, item, $uibModalInstance, $uibModal, GlobalConstantService) {
		var vm = this;
		vm.isAdd = item.item;
		vm.act = item.act;
		vm.init = init;
		vm.aktifasi = "";
		vm.pageSize = 10;
		vm.Keyword = "";
		vm.VendorName = item.item.VendorName;

		function init() {
			$translatePartialLoader.addPart('verifikasi-data');
			VerifikasiDataService.allcontact({
				VendorID: item.item.VendorID
			}, function (reply) {
				if (reply.status === 200) {
				    vm.detail = reply.data;
				    console.info(vm.detail);
				} else {
					$.growl.error({ message: "Gagal mendapatkan data Rekanan" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});

			if (vm.act == false) {
				vm.aktifasi = "Non Aktifkan";
			} else {
				vm.aktifasi = "Aktifkan";
			}
			// jLoad();


		};

		function initApproval() {
		    $translatePartialLoader.addPart('verifikasi-data');
		    jLoadApproval(1);


		};

		vm.jLoadApproval = jLoadApproval;
		function jLoadApproval(current) {
		    //console.info("curr "+current)
		    vm.listApproval = [];
		    vm.currentPage = current;
		    var offset = (current * 10) - 10;
		    VerifikasiDataService.selectApprovalByVendor({
		        Status: vm.isAdd.VendorID
		    }, function (reply) {
		        UIControlService.unloadLoading();
		        if (reply.status === 200) {
		            var data = reply.data;
		            vm.listApproval = data;
		        } else {
		            $.growl.error({ message: "Gagal mendapatkan data ApprovalVendor" });
		            UIControlService.unloadLoading();
		        }
		    }, function (err) {
		        console.info("error:" + JSON.stringify(err));
		        //$.growl.error({ message: "Gagal Akses API >" + err });
		        UIControlService.unloadLoading();
		    });
		}

		vm.cancel = cancel;
		function cancel() {
			$uibModalInstance.dismiss('cancel');
		}

		vm.save = save;
		function save() {
		    UIControlService.loadLoading("Silahkan Tunggu");
		    var data = {
		        act: vm.act,
		        VendorID: vm.isAdd.VendorID,
		        Description: vm.isAdd.Description
		    }
		    var modalInstance = $uibModal.open({
		        templateUrl: 'app/modules/panitia/data-rekanan/verifikasi-data/DetailSureActive.html',
		        controller: 'DetailSureActive',
		        controllerAs: 'DetailSureActive',
		        resolve: {
		            item: function () {
		                return data;
		            }
		        }
		    });
		    modalInstance.result.then(function () {
		        $uibModalInstance.close();
		    });
		}
	}
})();
