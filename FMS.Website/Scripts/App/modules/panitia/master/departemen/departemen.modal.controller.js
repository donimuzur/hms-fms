(function () {
	'use strict';

	angular.module("app").controller("departemenModalCtrl", ctrl);

	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'DepartemenService', 'RoleService', 'UIControlService', 'item', '$uibModalInstance'];

	function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, DepartemenService,
		RoleService, UIControlService, item, $uibModalInstance) {
		var vm = this;
		//console.info("masuuk modal : " + JSON.stringify(item));
		vm.isAdd = item.act;
		vm.action = "";
		vm.regionID = 0;
		vm.countryID = "";
		vm.provinceID = "";
		
		vm.init = init;
		function init() {
		    if (vm.isAdd === true) {
		        vm.action = "Tambah ";
		    }
		    else {
		        vm.action = "Ubah ";
		        vm.kode_depart = item.item.DepartmentCode;
		        vm.nama_depart = item.item.DepartmentName;
		        vm.id_depart = item.item.DepartmentID;
		    }
		}

		vm.batal = batal;
		function batal() {
		    $uibModalInstance.dismiss('cancel');
		};

		vm.simpan = simpan;
		function simpan() {
		    //console.info(vm.kode_depart + "-" + vm.nama_depart + "-" + vm.ket_depart);
		    if (vm.kode_depart === "" || vm.kode_depart === null) {
		        alert("Kode Departemen belum diisi!!");
		        return;
		    }

		    if (vm.nama_depart === "" || vm.nama_depart === null) {
		        alert("Nama Departemen belum diisi!!");
		        return;
		    }
		    cekKodeNama();
		}

	    //proses simpan
		vm.prosesSimpan = prosesSimpan;
        function prosesSimpan(){
            UIControlService.loadLoadingModal("Silahkan Tunggu...");
		    if (vm.isAdd === 1) {
		        DepartemenService.insert({
		            DepartmentCode: vm.kode_depart, DepartmentName: vm.nama_depart
		        }, function (reply) {
		            UIControlService.unloadLoadingModal();
		            if (reply.status === 200) {
		                UIControlService.msg_growl("success", "Berhasil Simpan Data Departemen!!");
		                $uibModalInstance.close();
		            }
		            else {
		                UIControlService.msg_growl("error", "Gagal menyimpan data!!");
		                return;
		            }
		        }, function (err) {
		            UIControlService.msg_growl("error", "Gagal Akses Api!!");
		            UIControlService.unloadLoadingModal();
		        });
		    }
		    else {
		        DepartemenService.update({
		            DepartmentCode: vm.kode_depart, DepartmentName: vm.nama_depart, DepartmentID: vm.id_depart
		        }, function (reply) {
		            UIControlService.unloadLoadingModal();
		            if (reply.status === 200) {
		                UIControlService.msg_growl("success", "Berhasil Simpan Data Departemen!!");
		                $uibModalInstance.close();
		            }
		            else {
		                UIControlService.msg_growl("error", "Gagal menyimpan data!!");
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
		    DepartemenService.cekData({
		        column: 1, Keyword: vm.kode_depart
		    }, function (reply) {
		        //console.info("cek1:" + JSON.stringify(reply));
		        if (reply.status === 200 && reply.data.length > 0) {
		            UIControlService.msg_growl("warning", "Kode sudah ada, silahkan masukkan kode yg lain!!");
		            return;
		        }
		        else if (reply.status === 200 && reply.data.length <= 0) {
		            DepartemenService.cekData({
		                column: 2, Keyword: vm.nama_depart
		            }, function (reply2) {
		                //console.info("cek2:" + JSON.stringify(reply2));
		                if (reply2.status === 200 && reply2.data.length > 0) {
		                    UIControlService.msg_growl("warning", "Nama sudah ada, silahkan masukkan kode yg lain!!");
		                    return;
		                }
		                else if (reply2.status === 200 && reply2.data.length <= 0) {
		                    prosesSimpan();
		                }
		                else {
		                    UIControlService.msg_growl("error", "Gagal melakukan pengecekkan data");
		                    return;
		                }
		            }, function (err) {
		                UIControlService.msg_growl("error", "Gagal Akses API!!");
		                UIControlService.unloadLoading();
		            });
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