(function () {
	'use strict';

	angular.module("app").controller("BankDetailCtrl", ctrl);

	ctrl.$inject = ['$timeout','$http', '$uibModal', '$filter', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'BankDetailService', 'UploaderService', 'UIControlService', 'GlobalConstantService', 'UploadFileConfigService', 'VerifiedSendService'];
	/* @ngInject */
	function ctrl($timeout, $http, $uibModal, $filter, $translate, $translatePartialLoader, $location, SocketService, BankDetailService, UploaderService, UIControlService, GlobalConstantService, UploadFileConfigService, VerifiedSendService) {
		var vm = this;

		vm.totalItems = 0;
		vm.currentPage = 0;
		vm.maxSize = 10;
		vm.page_id = 35;
		vm.menuhome = 0;
		vm.folderFile = GlobalConstantService.getConstant('api') + "/";
		vm.userId = 0;
		vm.jLoad = jLoad;
		vm.bank = [];
		vm.ambilUrl;
		vm.ambilUrl2;
		vm.IsApprovedCR = false;

		//vm.Kata = "";
		vm.VendorID;

		vm.init = init;
		function init() {
			loadVerifiedVendor();

		}

		vm.loadVerifiedVendor = loadVerifiedVendor;
		function loadVerifiedVendor() {
			VerifiedSendService.selectVerifikasi(function (reply) {
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.verified = reply.data;
					//console.info("ver>" + JSON.stringify(vm.verified));
					if (vm.verified.VerifiedSendDate === null && vm.verified.VerifiedDate === null) {
						//vm.IsApprovedCR = true;
					}
					//vm.cekTemporary = vm.verified.IsTemporary;
					vm.VendorID = vm.verified.VendorID;
					jLoad(1);
				} else {
					$.growl.error({ message: "Gagal mendapatkan data bank" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				console.info("error:" + JSON.stringify(err));
				//$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}

		vm.jLoad = jLoad;
		function jLoad(current) {
			//console.info("curr "+current)
			UIControlService.loadLoading("Silahkan Tunggu...");
			vm.currentPage = current;
			var offset = (current * 10) - 10;
			BankDetailService.Select({ VendorID: vm.VendorID }, function (reply) {
				//console.info("data:" + JSON.stringify(reply));
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.data;
					vm.bank = data.List;

					BankDetailService.getCRbyVendor({ CRName: 'OC_VENDORBANKDETAIL' }, function (reply) {
						UIControlService.unloadLoading();
						console.info("CR:" + JSON.stringify(reply.status));
						if (reply.status === 200) {
							console.info("CR:" + JSON.stringify(reply.data));
							if (reply.data === true) {
								vm.IsApprovedCR = true;
							} else {
								vm.IsApprovedCR = false;
							}
						}
					}, function (err) {
						UIControlService.msg_growl("error", "MESSAGE.API");
						UIControlService.unloadLoading();
					});
				} else {
					$.growl.error({ message: "Gagal mendapatkan data" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				console.info("error:" + JSON.stringify(err));
				//$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}

		vm.tambah = tambah;
		function tambah(data) {
			var data = {
				act: 1,
				item: data
			}
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/rekanan/data-perusahaan/bank-detail/bank-detail.modal.html',
				controller: "BankDetailModalCtrl",
				controllerAs: "BankDetModalCtrl",
				resolve: {
					item: function () {
						return data;
					}
				}
			});
			modalInstance.result.then(function () {
				window.location.reload();
			});
		}

		vm.edit = edit;
		function edit(data) {
			//console.info("console edit dokumen");
			var data = {
				act: 0,
				item: data
			}
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/rekanan/data-perusahaan/bank-detail/bank-detail.modal.html',
				controller: "BankDetailModalCtrl",
				controllerAs: "BankDetModalCtrl",
				resolve: {
					item: function () {
						return data;
					}
				}
			});
			modalInstance.result.then(function () {
				jLoad(1);
			});
		}




		vm.remove = remove;
		function remove(doc) {
			bootbox.confirm('<h3 class="afta-font center-block">' + "Yakin ingin menghapus?" + '<h3>', function (reply) {
				if (reply) {
					//UIControlService.loadLoading(loadmsg);
					BankDetailService.remove({
						ID: doc.ID
					}, function (reply2) {
						UIControlService.unloadLoading();
						if (reply2.status === 200) {
							UIControlService.msg_growl('notice', 'data berhasil di hapus');
							window.location.reload();
						} else
							UIControlService.msg_growl('error', 'gagal menghapus');
					}, function (error) {
						UIControlService.unloadLoading();
						UIControlService.msg_growl('error', 'MESSAGE.ERR_DEL');
					});
				}
			});
		};
	}
})();