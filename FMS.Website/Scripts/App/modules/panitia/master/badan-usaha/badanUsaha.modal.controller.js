(function () {
	'use strict';

	angular.module("app").controller("BadanUsahaModalCtrl", ctrl);

	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'BadanUsahaService', 'RoleService', 'UIControlService', 'item', '$uibModalInstance'];

	function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, BadanUsahaService,
		RoleService, UIControlService, item, $uibModalInstance) {
		var vm = this;
		//console.info("masuuk modal : " + JSON.stringify(item));
		vm.isAdd = item.act;
		vm.action = "";
		vm.id_depart = 0;
		vm.Name = "";
		vm.Description = "";
		vm.BusinessID = 0;
		
		vm.init = init;
		function init() {
		    if (vm.isAdd === true) {
		        vm.action = "Tambah ";
		    }
		    else {
		        vm.action = "Ubah ";
		        vm.Name = item.item.Name;
		        vm.Description = item.item.Description;
		        vm.BusinessID = item.item.BusinessID;
		    }
		}

		vm.batal = batal;
		function batal() {
		    $uibModalInstance.dismiss('cancel');
		};

		vm.simpan = simpan;
		function simpan() {
		    //console.info(vm.kode_depart + "-" + vm.nama_depart + "-" + vm.ket_depart);
		    if (vm.Name === "" || vm.Name === null) {
		        UIControlService.msg_growl("warning", "Kode Badan Usaha belum diisi!!");
		        return;
		    }
		    cekKodeNama();
		}

	    //proses simpan
		vm.prosesSimpan = prosesSimpan;
        function prosesSimpan(){
            UIControlService.loadLoadingModal("Silahkan Tunggu...");
		    if (vm.isAdd === true) {
		        BadanUsahaService.insert({
		            Name: vm.Name, Description: vm.Description
		        }, function (reply) {
		            UIControlService.unloadLoadingModal();
		            if (reply.status === 200) {
		                UIControlService.msg_growl("success", "Berhasil Simpan Data Departemen");
		                $uibModalInstance.close();
		            }
		            else {
		                UIControlService.msg_growl("error", "Gagal menyimpan data");
		                return;
		            }
		        }, function (err) {
		            UIControlService.msg_growl("error", "Gagal Mengakses API");
		            UIControlService.unloadLoadingModal();
		        });
		    }
		    else {
		        BadanUsahaService.update({
		            BusinessID: vm.BusinessID, Name: vm.Name, Description: vm.Description
		        }, function (reply) {
		            UIControlService.unloadLoadingModal();
		            if (reply.status === 200) {
		                UIControlService.msg_growl("success", "Berhasil Simpan Data Departemen");
		                $uibModalInstance.close();
		            }
		            else {
		                UIControlService.msg_growl("error", "Gagal menyimpan data");
		                return;
		            }
		        }, function (err) {
		            UIControlService.msg_growl("error", "Gagal Akses Api!!");
		            UIControlService.unloadLoadingModal();
		        });
		    }
		}

		vm.cekKodeNama = cekKodeNama;
		function cekKodeNama() {
		    //pengecekkan kode atau nama sudah ada belum?
		    BadanUsahaService.cekData({
		        column: 1, Keyword: vm.Name
		    }, function (reply) {
		        //console.info("cek1:" + JSON.stringify(reply));
		        if (reply.status === 200 && reply.data.length > 0) {
		            UIControlService.msg_growl("warning", "Nama Sudah Ada, silahkan masukkan nama badan usaha yang lain");
		            return;
		        }
		        else if (reply.status === 200 && reply.data.length <= 0) {
		            prosesSimpan();
		        }
		        else {
		            UIControlService.msg_growl("error", "Gagal melakukan pengecekkan data");
		            return;
		        }
		    }, function (err) {
		        UIControlService.msg_growl("error", "Gagal Akses Api!!");
		        UIControlService.unloadLoading();
		    });
		}

	}
})();