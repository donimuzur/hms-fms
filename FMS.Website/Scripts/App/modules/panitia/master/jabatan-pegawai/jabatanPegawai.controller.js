(function () {
	'use strict';

	angular.module("app").controller("JabatanPegawaiCtrl", ctrl);

	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'JabatanPegawaiService', 'RoleService', 'UIControlService', '$uibModal'];
	function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, JabatanPegawaiService,
        RoleService, UIControlService, $uibModal) {

		var vm = this;
		var page_id = 141;

		vm.jabatanPegawai = [];
		vm.totalItems = 0;
		vm.currentPage = 1;
		vm.maxSize = 10;
		vm.userBisaMengatur = false;
		vm.allowAdd = true;
		vm.allowEdit = true;
		vm.allowDelete = true;
		vm.kata = new Kata("");
		vm.init = init;

		vm.cariJabatan = cariJabatan;
		vm.jLoad = jLoad;
		//vm.loadAll = loadAll;
		vm.ubah_aktif = ubah_aktif;
		vm.tambah = tambah;
		vm.edit = edit;

		function init() {
			$translatePartialLoader.addPart('jabatan-pegawai');
			UIControlService.loadLoading("Silahkan Tunggu...");
			//loadJabatan();
			jLoad(1);
		}

		
		vm.cariJabatan = cariJabatan;
		function cariJabatan() {
		    //console.info("masuuuk " + JSON.stringify(vm.kata.srcText));
			jLoad(1);
		}

		vm.jLoad = jLoad;
		function jLoad(current) {
		    UIControlService.loadLoading("Silahkan Tunggu...");
			vm.currentPage = current;
			var offset = (current * 10) - 10;
			JabatanPegawaiService.select({
				Offset: offset,
				Limit: vm.maxSize,
				Keyword: vm.kata.srcText
			}, function (reply) {
			    //console.info("datajabatan:"+JSON.stringify(reply));
			    UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.data;
					vm.jabatanPegawai = data.List;
					vm.totalItems = Number(data.Count);
				} else {
				    UIControlService.msg_growl("error", "Gagal mendapatkan data jabatan pegawai!!");
					UIControlService.unloadLoading();
				}
			}, function (err) {
			    //console.info("error:" + JSON.stringify(err));
			    UIControlService.msg_growl("error", "Gagal mengakses API");
				UIControlService.unloadLoading();
			});
		}
        
		vm.ubah_aktif = ubah_aktif;
		function ubah_aktif(data, active) {
		    UIControlService.loadLoading("Silahkan Tunggu");  
            //console.info("ada:"+JSON.stringify(data))
			JabatanPegawaiService.editActive({
			    PositionID: data.PositionID, PositionCode: data.PositionCode, PositionName: data.PositionName, IsActive: active
			}, function (reply) {
			    //console.info("ada:" + JSON.stringify(reply))
			    UIControlService.unloadLoading();
			    if (reply.status === 200) {
			        var msg = "";
			        if(active === false) msg = " NonAktifkan ";
			        if (active === true) msg = "Aktifkan ";
			        UIControlService.msg_growl("success", "Data Berhasil di " + msg);
			        jLoad(1);
			    }
			    else {
			        UIControlService.msg_growl("error", "Gagal menonaktifkan data");
			        return;
			    }
			}, function (err) {
				//$.growl.error({ message: "Gagal Akses API >" + err });
			    UIControlService.msg_growl("error", "Gagal mengakses API");
				UIControlService.unloadLoading();
			});
            
		}

		vm.tambah = tambah;
		function tambah(data) {
		    //console.info("masuk form add/edit");
		    var data = {
		        act: 1,
		        item: data
		    }
		    var modalInstance = $uibModal.open({
			    templateUrl: 'app/modules/panitia/master/jabatan-pegawai/formJabatanPegawai.html',
			    controller: 'JabatanPegawaiModalCtrl',
			    controllerAs: 'jabatanPegModCtrl',
				resolve: {
					item: function () {
						return data;
					}
				}
			});
			modalInstance.result.then(function () {
				vm.jLoad(1);
			});
		}

		vm.edit = edit;
		function edit(data) {
		    //console.info("masuk form add/edit");
		    var data = {
		        act: 0,
		        item: data
		    }
		    var modalInstance = $uibModal.open({
		        templateUrl: 'app/modules/panitia/master/jabatan-pegawai/formJabatanPegawai.html',
		        controller: 'JabatanPegawaiModalCtrl',
		        controllerAs: 'jabatanPegModCtrl',
		        resolve: {
		            item: function () {
		                return data;
		            }
		        }
		    });
		    modalInstance.result.then(function () {
		        vm.jLoad(1);
		    });
		}
	}
})();
function Kata(srcText) {
	var self = this;
	self.srcText = srcText;
}

