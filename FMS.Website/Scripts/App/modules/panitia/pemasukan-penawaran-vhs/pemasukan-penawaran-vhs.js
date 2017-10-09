(function () {
	'use strict';

	angular.module("app").controller("PPVHSCtrl", ctrl);

	ctrl.$inject = ['SocketService', 'PPVHSAdminService', 'UIControlService', '$state', '$stateParams'];

	function ctrl(SocketService, PPVHSAdminService, UIControlService, $state, $stateParams) {
		var vm = this;
		vm.data = [];
		vm.totalItems = 0;
		vm.currentPage = 1;
		vm.pageSize = 10;
		vm.init = init;
		vm.jLoad = jLoad;
		vm.IDTender = Number($stateParams.TenderRefID);
		vm.IDStepTender = Number($stateParams.StepID);
		vm.ProcPackType = Number($stateParams.ProcPackType);

		function init() {
			//UIControlService.loadLoading("Silahkan Tunggu...");
			//loadTemplate();
			//if (vm.ProcPackType === 3168) {
			//    loadDataTender();
			//    jLoad(1);
			// }
			jLoad(1);
		}
		/*
        function loadDataTender() {
            PPVHSAdminService.selectAll({
                ID: vm.IDStepTender
            }, function (reply) {
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    console.info("data:" + JSON.stringify(reply));
                    vm.TenderName = data.tender.TenderName;
                    vm.StartDate = UIControlService.getStrDate(data.StartDate);
                    vm.EndDate = UIControlService.getStrDate(data.EndDate);
                    console.info("tender::" + JSON.stringify(vm.dataTenderReal));
                }
            }, function (err) {
                UIControlService.msg_growl("error", "MESSAGE.API");
                UIControlService.unloadLoading();
            });
        }*/

		vm.jLoad = jLoad;
		function jLoad(current) {
			//console.info("curr "+current)
			vm.data = [];
			vm.currentPage = current;
			var offset = (current * 10) - 10;
			PPVHSAdminService.selectAll({
				ID: vm.IDStepTender
			}, function (reply) {
				//console.info("data:"+JSON.stringify(reply));
				UIControlService.unloadLoading();
				if (reply.status === 200) {
					vm.data = reply.data;
					//console.info("data:" + JSON.stringify(reply.data));
					vm.totalItems = Number(vm.data.Count);
					//console.info("data:" + JSON.stringify(vm.data));

					PPVHSAdminService.isNeedTenderStepApproval({ ID: vm.IDStepTender }, function (result) {
						vm.isNeedApproval = result.data;
						//console.info("jumlahData:" + vm.countRegister);
					}, function (err) {
						$.growl.error({ message: "Gagal mendapatkan data Approval" });
						UIControlService.unloadLoading();
					});

				} else {
					$.growl.error({ message: "Gagal mendapatkan data" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				console.info("error:" + JSON.stringify(err));
				//$.growl.error({ message: "Gagal Akses API" + err });
				UIControlService.unloadLoading();
			});
		}

		vm.sendToApproval = sendToApproval;
		function sendToApproval() {
			UIControlService.loadLoading('MESSAGE.SENDING');
			PPVHSAdminServicee.sendToApproval({ ID: vm.IDStepTender }, function (reply) {
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

		vm.backpengadaan = backpengadaan;
		function backpengadaan() {
			$state.transitionTo('data-pengadaan-tahapan', { TenderRefID: vm.IDTender, ProcPackType: vm.ProcPackType });
		}
	}
})();


