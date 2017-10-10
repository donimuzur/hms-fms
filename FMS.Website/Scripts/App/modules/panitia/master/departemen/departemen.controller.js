(function () {
	'use strict';

	angular.module("app").controller("DepartemenCtrl", ctrl);

	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService',
        'DepartemenService', 'RoleService', 'UIControlService', '$uibModal' ];
	function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, DepartemenService,
        RoleService, UIControlService, $uibModal) {

		var vm = this;
		var page_id = 141;

		vm.departemen = [];
		vm.totalItems = 0;
		vm.currentPage = 1;
		vm.pageSize = 10;
		vm.userBisaMengatur = false;
		vm.allowAdd = true;
		vm.allowEdit = true;
		vm.allowDelete = true;
		vm.kata = new Kata("");
		vm.init = init;

		//vm.loadDepartemen = loadDepartemen;
		vm.cariDepartemen = cariDepartemen;
		vm.jLoad = jLoad;
		//vm.loadAll = loadAll;
		vm.ubah_aktif = ubah_aktif;
		vm.tambah = tambah;
		vm.edit = edit;

		function init() {
			$translatePartialLoader.addPart('master-departemen');
			UIControlService.loadLoading("Silahkan Tunggu...");
			//loadDepartemen();
			jLoad(1);
			//vm.loadAll();
			//vm.getDepartments(vm.currentPage, vm.pageSize);

		}
        /*
		function loadDepartemen() {
			//coding ref > itp.role.cekBisaMenambah
			RoleService.checkAuthority({
				username: $rootScope.userLogin,
				page_id: vm.page_id,
				jenis_mengatur: 2
			}, function (reply) {
				if (reply.status === 200) {
					var data = reply.result.data[0];
					vm.userBisaMenambah = data.bisa_mengatur;
				} else {
					$.growl.error({ message: "Gagal mendapatkan data hak akses!" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
			    $.growl.error({ message: "Gagal Akses API >" + err });
			    console.info("error: "+JSON.stringify(err));
				UIControlService.unloadLoading();
			});
			//coding ref > itp.role.cekBisaMengubah
			RoleService.checkAuthority({
				username: $rootScope.userLogin,
				page_id: vm.page_id,
				jenis_mengatur: 3
			}, function (reply) {
				if (reply.status === 200) {
					var data = reply.result.data[0];
					vm.userBisaMengubah = data.bisa_mengatur;
				} else {
					$.growl.error({ message: "Gagal mendapatkan data hak akses!" });
					UIControlService.unloadLoading();
				}
			}, function (err) {
				$.growl.error({ message: "Gagal Akses API >" + err });
				console.info("error: " + JSON.stringify(err));
				UIControlService.unloadLoading();
			});
			//coding ref > itp.role.cekBisaMenghapus 
			RoleService.checkAuthority({
				username: $rootScope.userLogin,
				page_id: vm.page_id,
				jenis_mengatur: 4
			}, function (reply) {
			    UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.result.data[0];
					vm.userBisaMenghapus = data.bisa_mengatur;
				} else {
					$.growl.error({ message: "Gagal mendapatkan data hak akses" });
				}
			}, function (err) {
				$.growl.error({ message: "Gagal Akses API >" + err });
				UIControlService.unloadLoading();
			});
		}
        */
		vm.cariDepartemen = cariDepartemen;
		function cariDepartemen() {
			vm.jLoad(1);
		}

		vm.jLoad = jLoad;
		function jLoad(current) {
            //console.info("curr "+current)
			vm.departemen = [];
			vm.currentPage = current;
			var offset = (current * 10) - 10;
			DepartemenService.select({
				Offset: offset,
				Limit: vm.pageSize,
				Keyword: vm.kata.srcText
			}, function (reply) {
			    //console.info("data:"+JSON.stringify(reply));
			    UIControlService.unloadLoading();
				if (reply.status === 200) {
					var data = reply.data;
					vm.departemen = data.List;
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
        
		vm.ubah_aktif = ubah_aktif;
		function ubah_aktif(data, active) {
		    UIControlService.loadLoading("Silahkan Tunggu");  
            //console.info("ada:"+JSON.stringify(data))
			DepartemenService.editActive({
			    DepartmentID: data.DepartmentID, DepartmentCode: data.DepartmentCode, DepartmentName: data.DepartmentName, IsActive: active
			}, function (reply) {
			    UIControlService.unloadLoading();
			    if (reply.status === 200) {
			        var msg = "";
			        if(active === false) msg = " NonAktifkan ";
			        if (active === true) msg = "Aktifkan ";
			        UIControlService.msg_growl("success", "Data Berhasil di " + msg);
			        jLoad(1);
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

		vm.tambah = tambah;
		function tambah(data) {
		    console.info("masuk form add/edit");
		    var data = {
		        act: 1,
		        item: data
		    }
		    var modalInstance = $uibModal.open({
			    templateUrl: 'app/modules/panitia/master/departemen/formDepartemen.html',
			    controller: 'departemenModalCtrl',
			    controllerAs: 'formDepartCtrl',
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
		    console.info("masuk form add/edit");
		    var data = {
		        act: 0,
		        item: data
		    }
		    var modalInstance = $uibModal.open({
		        templateUrl: 'app/modules/panitia/master/departemen/formDepartemen.html',
		        controller: 'departemenModalCtrl',
		        controllerAs: 'formDepartCtrl',
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
//TODO

function Kata(srcText) {
	var self = this;
	self.srcText = srcText;
}

