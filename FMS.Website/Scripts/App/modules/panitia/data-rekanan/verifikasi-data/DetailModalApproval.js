(function () {
	'use strict';

	angular.module("app").controller("DetailModalApprovalCtrl", ctrl);
	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 
        'VerifikasiDataService', 'UIControlService', 'item', '$uibModalInstance', '$uibModal', 'GlobalConstantService'];
	/* @ngInject */
	function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, 
        VerifikasiDataService, UIControlService, item, $uibModalInstance, $uibModal, GlobalConstantService) {
		var vm = this;
		vm.isAdd = item.item;
		vm.act = item.act;
		vm.aktifasi = "";
		vm.pageSize = 10;
		vm.Keyword = "";
		vm.VendorName = item.item.VendorName;

		vm.initApproval = initApproval;
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
			VerifikasiDataService.editActive({
				IsActive: vm.act,
				VendorID: vm.isAdd.VendorID,
				Description: vm.isAdd.Description
			}, function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var msg = "";
					if (vm.act === false) msg = " NonAktifkan ";
					if (vm.act === true) msg = "Aktifkan ";
					UIControlService.msg_growl("success", "Data Berhasil di " + msg);
					$uibModalInstance.close();
				}
				else {
					UIControlService.msg_growl("error", "Gagal menonaktifkan data ");
					return;
				}
			}, function (err) {

				UIControlService.msg_growl("error", "Gagal Akses API ");
				UIControlService.unloadLoading();
			});
		}
	}
})();
