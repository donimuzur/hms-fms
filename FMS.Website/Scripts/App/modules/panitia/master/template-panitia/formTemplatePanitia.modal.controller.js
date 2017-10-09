(function () {
	'use strict';

	angular.module("app").controller("formTemplatePanitiaCtrl", ctrl);

	ctrl.$inject = ['$http', 'item', '$uibModalInstance', '$uibModal', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'UIControlService', 'CommitteeTemplateService'];
	/* @ngInject */
	function ctrl($http, item, $uibModalInstance, $uibModal, $translate, $translatePartialLoader, $location, SocketService, UIControlService, CommitteeTemplateService) {
		var vm = this;
		
		vm.namaTemplate = "";
		vm.details = [];
		vm.posisiTerpilih = {};
        vm.pegawaiTerpilih = {};
        vm.positions = [];

        vm.init = init;
        function init() {
            UIControlService.loadLoadingModal("LOADING");
            CommitteeTemplateService.GetPositions(function (reply) {
                vm.positions = reply.data;
                if (item.ID) {
                    CommitteeTemplateService.GetById({
                        ID: item.ID
                    }, function (reply) {
                        vm.namaTemplate = reply.data.TemplateName;
                        vm.details = reply.data.MstCommitteeTemplateDetails;
                        UIControlService.unloadLoadingModal();
                    }, function (err) {
                        UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD');
                        UIControlService.unloadLoadingModal();
                    });
                } else {
                    UIControlService.unloadLoadingModal();
                }
            }, function (err) {
                UIControlService.msg_growl("error", 'MESSAGE.ERR_LOAD_POSITIONS');
                UIControlService.unloadLoadingModal();
            })
        }

		vm.pilihPegawai = pilihPegawai;
		function pilihPegawai() {
			var modalInstance = $uibModal.open({
				templateUrl: 'app/modules/panitia/master/template-panitia/formPilihPegawai.html',
				controller: 'selectEmployeeModal',
				controllerAs: 'selectEmployeeCtrl'
			});

			modalInstance.result.then(function (pgw) {
			    vm.pegawaiTerpilih = pgw;
			    vm.pegawaiTerpilih.EmployeeName = vm.pegawaiTerpilih.FullName + ' ' + vm.pegawaiTerpilih.SurName;
			});
		};

		vm.tambah = tambah;
		function tambah() {
		    if (!vm.posisiTerpilih.PositionID || !vm.pegawaiTerpilih.EmployeeID) {
		        UIControlService.msg_growl("error", 'MESSAGE.ERR_INCOMPLETE_FIELD');
		        return;
		    }
		    for(var i = 0;i < vm.details.length; i++){
		        if (vm.details[i].EmployeeID == vm.pegawaiTerpilih.EmployeeID) {
		            UIControlService.msg_growl("error", 'MESSAGE.ERR_DUPLICATE_EMPLOYEE');
		            return;
		        }
		    }
		    vm.details.push({
		        CommitteePositionID: vm.posisiTerpilih.PositionID,
		        EmployeeID: vm.pegawaiTerpilih.EmployeeID,
		        EmployeeName: vm.pegawaiTerpilih.EmployeeName,
		        CommitteePositionName: vm.posisiTerpilih.PositionName
		    });

		    vm.pegawaiTerpilih = {};
		    vm.posisiTerpilih = {};
		}

		vm.hapus = hapus;
		function hapus(index) {
		    vm.details.splice(index, 1);
		}

		vm.simpan = simpan;
		function simpan() {
		    if (!vm.namaTemplate) {
		        UIControlService.msg_growl("error", 'MESSAGE.ERR_NO_NAME');
		        return;
		    }
		    if (vm.details.length == 0) {
		        UIControlService.msg_growl("error", 'MESSAGE.ERR_NO_DETAILS');
		        return;
		    }
		    UIControlService.loadLoadingModal("LOADING");
		    CommitteeTemplateService.Save({
		        ID: item.ID,
		        TemplateName: vm.namaTemplate,
		        MstCommitteeTemplateDetails: vm.details
		    }, function (reply) {
		        UIControlService.msg_growl("notice", 'MESSAGE.SUCC_SAVE');
		        UIControlService.unloadLoadingModal();
		        $uibModalInstance.close();
		    }, function (err) {
		        UIControlService.msg_growl("error", 'MESSAGE.ERR_SAVE');
		        UIControlService.unloadLoadingModal();
		    });
		}

		vm.cancel = cancel;
		function cancel() {
			$uibModalInstance.dismiss('cancel');
		};

		
	}
})();