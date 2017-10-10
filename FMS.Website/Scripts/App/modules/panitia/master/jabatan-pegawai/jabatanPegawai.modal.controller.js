(function () {
	'use strict';

	angular.module("app").controller("JabatanPegawaiModalCtrl", ctrl);

	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'JabatanPegawaiService', 'RoleService', 'UIControlService', 'item', '$uibModalInstance'];

	function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, JabatanPegawaiService,
		RoleService, UIControlService, item, $uibModalInstance) {
		var vm = this;
		//console.info("masuuk modal : " + JSON.stringify(item));
		vm.isAdd = item.act;
		vm.action = "";
		vm.IDJabatan= 0;
		vm.NamaJabatan = "";
		vm.KodeJabatan = "";
		
		vm.init = init;
		function init() {
		    if (vm.isAdd === 1) {
		        vm.action = "Tambah ";
		    }
		    else {
		        vm.action = "Ubah ";
		        vm.KodeJabatan = item.item.PositionCode;
		        vm.NamaJabatan = item.item.PositionName;
		        vm.IDJabatan = item.item.PositionID;
		    }
		}

		vm.batal = batal;
		function batal() {
		    $uibModalInstance.dismiss('cancel');
		};

		vm.simpan = simpan;
		function simpan() {
		    //console.info(vm.KodeJabatan + "-" + vm.NamaJabatan + "-" );
		    if (vm.KodeJabatan === "" || vm.KodeJabatan === null) {
		        UIControlService.msg_growl("warning", "Kode Jabatan belum diisi!!");
		        return;
		    }

		    if (vm.NamaJabatan === "" || vm.NamaJabatan === null) {
		        UIControlService.msg_growl("warning", "Nama Jabatan belum diisi!!");
		        return;
		    }
		    cekKodeNama();
		}

	    //proses simpan
		vm.prosesSimpan = prosesSimpan;
        function prosesSimpan(){
            UIControlService.loadLoadingModal("Silahkan Tunggu");
		    if (vm.isAdd === 1) {
		        JabatanPegawaiService.insert({
		            PositionCode: vm.KodeJabatan, PositionName: vm.NamaJabatan
		        }, function (reply) {
		            UIControlService.unloadLoadingModal();
		            if (reply.status === 200) {
		                UIControlService.msg_growl("success", "Berhasil Simpan Data Jabatan Pegawai!!");
		                $uibModalInstance.close();
		            }
		            else {
		                alert("Gagal menyimpan data!!");
		                UIControlService.msg_growl("error", "Gagal Menyimpan Fata!!");
		                return;
		            }
		        }, function (err) {
		            console.info("error:" + JSON.stringify(err));
		            alert("Gagal Akses Api!!");
		            UIControlService.unloadLoading();
		        });
		    }
		    else {
		        JabatanPegawaiService.update({
		            PositionCode: vm.KodeJabatan, PositionName: vm.NamaJabatan, PositionID: vm.IDJabatan
		        }, function (reply) {
		            UIControlService.unloadLoading();
		            if (reply.status === 200) {
		                alert("Berhasil Simpan Data Jabatan!!");
		                $uibModalInstance.close();
		            }
		            else {
		                alert("Gagal menyimpan data!!");
		                return;
		            }
		        }, function (err) {
		            console.info("error:" + JSON.stringify(err));
		            alert("Gagal Akses Api!!");
		            UIControlService.unloadLoading();
		        });
		    }
		}

		vm.cekKodeNama = cekKodeNama;
		function cekKodeNama() {
		    //pengecekkan kode atau nama sudah ada belum?
		    JabatanPegawaiService.cekData({
		        column: 1, Keyword: vm.KodeJabatan
		    }, function (reply) {
		        console.info("cek1:" + JSON.stringify(reply));
		        if (reply.status === 200 && reply.data.length > 0) {
		            alert("Kode sudah ada, silahkan masukkan kode yg lain!!");
		            return;
		        }
		        else if (reply.status === 200 && reply.data.length <= 0) {
		            JabatanPegawaiService.cekData({
		                column: 2, Keyword: vm.NamaJabatan
		            }, function (reply2) {
		                //console.info("cek2:" + JSON.stringify(reply2));
		                if (reply2.status === 200 && reply2.data.length > 0) {
		                    alert("Nama sudah ada, silahkan masukkan kode yg lain!!");
		                    return;
		                }
		                else if (reply2.status === 200 && reply2.data.length <= 0) {
		                    prosesSimpan();
		                }
		                else {
		                    alert("Gagal melakukan pengecekkan data");
		                    return;
		                }
		            }, function (err) {
		                console.info("error:" + JSON.stringify(err));
		                alert("Gagal Akses Api!!");
		                UIControlService.unloadLoading();
		            });
		        }
		        else {
		            alert("Gagal melakukan pengecekkan data");
		            return;
		        }
		    }, function (err) {
		        console.info("error:" + JSON.stringify(err));
		        alert("Gagal Akses Api!!");
		        UIControlService.unloadLoading();
		    });
		}

	}
})();