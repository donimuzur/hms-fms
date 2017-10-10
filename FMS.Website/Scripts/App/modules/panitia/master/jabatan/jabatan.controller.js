(function () {
	'use strict';

	angular.module("app").controller("JabatanCreateEditCtrl", ctrl);

	ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'JabatanService',
        'RoleService', 'UIControlService', '$uibModal'];
	function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, JabatanService,
        RoleService, UIControlService, $uibModal) {

		var vm = this;

		vm.jabatans = null;
		vm.totalItems = 0;
		vm.currentPage = 1;
		vm.maxSize = 10;
		vm.page_id = 9;
		vm.menuhome = 0;
		vm.userBisaMengatur;
		vm.userBisaMenambah = false;
		vm.userBisaMengubah = false;
		vm.userBisaMenghapus = false;
		vm.kata = new Kata("");

		vm.init = init;
		function init() {
			$translatePartialLoader.addPart('master-jabatan');
			loadJabatan();
		};

		vm.loadJabatan = loadJabatan;
		function loadJabatan() {
			//RoleService.checkAuthority({
			//	username: $rootScope.userLogin,
			//	page_id: vm.page_id,
			//	jenis_mengatur: 2
			//}, function (reply) {
			//	if (reply.status === 200) {
			//		var data = reply.result.data[0];
			//		vm.userBisaMenambah = data;
			//	} else {
			//		$.growl.error({ message: "Gagal mendapatkan data hak akses!" });
			//	}
			//}, function (err) {
			//	$.growl.error({ message: "Gagal Akses API >" + err });
			//});
			//RoleService.checkAuthority({
			//	username: $rootScope.userLogin,
			//	page_id: vm.page_id,
			//	jenis_mengatur: 3
			//}, function (reply) {
			//	if (reply.status === 200) {
			//		var data = reply.result.data[0];
			//		vm.userBisaMengubah = data;
			//	} else {
			//		$.growl.error({ message: "Gagal mendapatkan data hak akses!" });
			//	}
			//}, function (err) {
			//	$.growl.error({ message: "Gagal Akses API >" + err });
			//});
			//RoleService.checkAuthority({
			//	username: $rootScope.userLogin,
			//	page_id: vm.page_id,
			//	jenis_mengatur: 4
			//}, function (reply) {
			//	if (reply.status === 200) {
			//		var data = reply.result.data[0];
			//		vm.userBisaMenghapus = data;
			//	} else {
			//		$.growl.error({ message: "Gagal mendapatkan data hak akses!" });
			//	}
			//}, function (err) {
			//	$.growl.error({ message: "Gagal Akses API >" + err });
			//});
			jLoad(1);
		}; // end loadJabatan

		vm.cariJabatan = cariJabatan;
		function cariJabatan() {
			vm.jLoad(1);
		};
		/* akhir pencarian edit by ani*/

		vm.jLoad = jLoad;
		function jLoad(current) {
		    UIControlService.loadLoading("Silahkan Tunggu");
			vm.jabatans = [];
			vm.currentPage = current;
			vm.offset = (current * 10) - 10;

			JabatanService.select({
			    Offset: vm.offset, Limit: 10, Keyword: ""+ vm.kata.srcText +""
			}, function (reply) {
			    UIControlService.unloadLoading();
			    console.info("data:" + JSON.stringify(reply));
			    if (reply.status === 200) {
			        var data = reply.data;
			        vm.jabatans = data;
			        vm.totalItems = Number(data.length);
			    } else {
			       alert("Gagal mendapatkan data Master Departemen" );
			       return;
			    }
			}, function (err) {
			    alert("gagal akses API!! " + err);
			    //$.growl.error({ message: "Gagal Akses API >" + err });
			    UIControlService.unloadLoading();
			});

			//$http.post($rootScope.url_api + "jabatan/countallfind", {
			//	jabatan_nama: "%" + vm.kata.srcText + "%"
			//}).success(function (reply) {
			//	if (reply.status === 200) {
			//		var data = reply.result.data;
			//		vm.totalItems = data;
			//	} else {
			//		$.growl.error({ message: "Gagal mendapatkan jumlah data jabatan!!" });
			//		$rootScope.unloadLoading();
			//		return;
			//	}
			//}).error(function (err) {
			//	$.growl.error({ message: "Gagal Akses API >" + err });
			//	$rootScope.unloadLoading();
			//	return;
			//});

			//$http.post($rootScope.url_api + "jabatan/selectallfind", {
			//	jabatan_nama: "%" + vm.kata.srcText + "%", offset: vm.offset, limit: vm.maxSize
			//}).success(function (reply) {
			//	if (reply.status === 200) {
			//		var data = reply.result.data;
			//		vm.jabatans = data;
			//		for (var i = 0; i < vm.jabatans.length; i++) {
			//			vm.jabatans[i].flag_active = Number(vm.jabatans[i].flag_active);
			//		}
			//	} else {
			//		$.growl.error({ message: "Gagal mendapatkan data jabatan!!" });
			//		$rootScope.unloadLoading();
			//		return;
			//	}
			//}).error(function (err) {
			//	$.growl.error({ message: "Gagal Akses API >" + err });
			//	$rootScope.unloadLoading();
			//	return;
			//});
		};

		vm.nonaktifkan = nonaktifkan;
		function nonaktifkan(jabatan) {
			//vm.cek_authorize(
			JabatanService.editActive({
				jabatan_id: jabatan.jabatan_id,
				flag_active: false
			}, function (reply) {
				if (reply.status === 200) {
					$.growl.notice({ message: "Berhasil menonaktifkan data jabatan!" });
					vm.jLoad(0);
				} else {
					$.growl.error({ message: "Gagal menonaktifkan data jabatan!" });
				}
			}, function (err) {
				$.growl.error({ message: "Gagal menonaktifkan data jabatan!" });
			});
			//);
		};

		vm.aktifkan = aktifkan;
		function aktifkan(jabatan) {
			//vm.cek_authorize(
			JabatanService.editActive({
				jabatan_id: jabatan.jabatan_id,
				flag_active: true
			}, function (reply) {
				if (reply.status === 200) {
					$.growl.notice({ message: "Berhasil mengaktifkan data jabatan!" });
					vm.jLoad(0);
				} else {
					$.growl.error({ message: "Gagal mengaktifkan data jabatan!" });
				}
			}, function (err) {
				$.growl.error({ message: "Gagal mengaktifkan data jabatan!" });
			});
			//);
		};

		vm.tambahJabatan = tambahJabatan;
		function tambahJabatan() {
			var modalInstance = $modal.open({
				templateUrl: 'tambahJabatan.html',
				controller: tambahJabatanCtrl

			});
			modalInstance.result.then(function () {
				loadJabatan();
			});
		};

		vm.editJabatan = editJabatan;
		function editJabatan(jabatan) {
			var modalInstance = $modal.open({
				templateUrl: 'editJabatan.html',
				controller: editJabatanCtrl,
				resolve: {
					item: function () {
						return jabatan;
					}
				}
			});
			modalInstance.result.then(function () {
				loadJabatan();
			});
		};

		vm.detailJabatan = detailJabatan;
		function detailJabatan(jabatan) {
			var modalInstance = $modal.open({
				templateUrl: 'detailJabatan.html',
				controller: detailJabatanCtrl,
				resolve: {
					item: function () {
						return jabatan;
					}
				}
			});
			modalInstance.result.then(function () {
				loadJabatan();
			});
		};
	};
})();

//TODO             
var tambahJabatanCtrl = function ($scope, $modalInstance, $http, $cookieStore, $rootScope, item) {
	$scope.newJabatan = new Jabatan("");
	$scope.roles = [];
	$scope.selectedOption;
	$scope.page_id = 9;
	$scope.tipe_pengadaan;
	$scope.selectedTipe;
	$scope.approvalke = [];
	$scope.selectedApprovalke = 0;

	$scope.change = function (obj) {
		$scope.selectedOption = obj;
	};

	$scope.changeTipe = function (obj) {
		$scope.selectedTipe = obj;
		if (!($scope.selectedTipe.ref_id === '8')) { $scope.selectedApprovalke = 0; }
	};

	$scope.changeApprov = function (obj) {
		$scope.selectedApprovalke = obj;
	};

	$scope.init = function () {
		var param = [];
		param.push("KEPANITIAAN");
		$http.post($rootScope.url_api + "roles/role_by_type", { jenis_role: "KEPANITIAAN" })
		.success(function (reply) {
			if (reply.status === 200) {
				var datakode = reply.result.data;
				$scope.roles = datakode;
			}
			else {
				$.growl.error({ title: "[ERROR]", message: "gagal mendapatkan data role" });
				return;
			}
		})
		.error(function (err) {
			$.growl.error({ title: "[ERROR]", message: "gagal mendapatkan data role" });
			return;
		});

		$http.post($rootScope.url_api + "reference/listbycode", { code: "tipe_pengadaan" })
		.success(function (reply) {
			if (reply.status === 200) {
				$scope.tipe_pengadaan = reply.result.data;
			}
			else {
				$.growl.error({ title: "[ERROR]", message: "gagal data tipe" });
				return;
			}
		})
		.error(function (err) {
			$.growl.error({ title: "[ERROR]", message: "gagal mendapatkan data tipe" });
			return;
		});

		for (var i = 0; i <= 2; i++) {
			$scope.approvalke.push(i);
		}
	};

	$scope.insert = function () {
		if ($scope.newJabatan.jabatan_nama === "") {
			$.growl.error({ title: "[ERROR]", message: "Nama jabatan belum diisi" });
			return;
		}
		if ($scope.selectedOption === null || $scope.selectedOption === undefined) {
			$.growl.error({ title: "[ERROR]", message: "Role belum dipilih" });
			return;
		}
		if ($scope.selectedTipe === null || $scope.selectedTipe === undefined) {
			$.growl.error({ title: "[ERROR]", message: "Tipe Pengadaan  belum dipilih" });
			return;
		}
		//console.info(JSON.stringify($scope.selectedApprovalke)+" :: "+JSON.stringify($scope.selectedTipe));

		$rootScope.loadLoadingModal("Proses Simpan");
		$rootScope.authorize(
            $http.post($rootScope.url_api + "jabatan/insertjabatan", {
            	jabatan_nama: $scope.newJabatan.jabatan_nama,
            	flag_active: true, role_id: $scope.selectedOption.role_id, approval: $scope.selectedApprovalke,
            	type: $scope.selectedTipe.ref_value
            })
            .success(function (reply) {
            	$rootScope.unloadLoadingModal();
            	if (reply.status === 200 && reply.result.data.inserted_id === true) {
            		$.growl.notice({ title: "[INFO]", message: "Data Jabatan Berhasil Disimpan" });
            		$modalInstance.close();
            	}
            	else if (reply.status === 200 && reply.result.data === 'duplicate') {
            		$.growl.warning({ title: "[INFO]", message: "Nama jabatan sudah pernah diinputkan" });
            		return;
            	}
            	else {
            		$.growl.error({ title: "[ERROR]", message: "Data Jabatan Gagal Disimpan" });
            		return;
            	}
            })
            .error(function (err) {
            	$rootScope.unloadLoadingModal();
            	$.growl.error({ title: "[ERROR]", message: "Gagal mengakses API!" });
            	return;
            })
        );

	};
	$scope.batal = function () {
		$modalInstance.dismiss('cancel');
	};
};

//TODO
var editJabatanCtrl = function ($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
	$scope.jabatanToEdit = new Jabatan(item.jabatan_nama);
	$scope.roles = [];
	$scope.selectedOption;
	$scope.page_id = 9;
	$scope.tipe_pengadaan;
	$scope.selectedTipe;
	$scope.approvalke = [];
	$scope.selectedApprovalke = 0;
	$scope.nama_jabatanold = item.jabatan_nama;

	$scope.change = function (obj) {
		$scope.selectedOption = obj;
	};

	$scope.changeTipe = function (obj) {
		$scope.selectedTipe = obj;
		if (!($scope.selectedTipe.ref_id === '8')) { $scope.selectedApprovalke = 0; }
	};

	$scope.changeApprov = function (obj) {
		$scope.selectedApprovalke = obj;
	};

	$scope.init = function () {
		$http.post($rootScope.url_api + "roles/role_by_type", { jenis_role: "KEPANITIAAN" })
        .success(function (reply) {
        	if (reply.status === 200) {
        		var datakode = reply.result.data;
        		$scope.roles = datakode;
        		for (var i = 0; i < $scope.roles.length; i++) {
        			if ($scope.roles[i].role_id === item.role_id) {
        				$scope.selectedOption = $scope.roles[i];
        				break;
        			}
        		}
        	}
        	else {
        		$.growl.error({ title: "[ERROR]", message: "gagal mendapatkan data role" });
        		return;
        	}
        })
        .error(function (err) {
        	$.growl.error({ title: "[ERROR]", message: "gagal mendapatkan data role" });
        	return;
        });

		$http.post($rootScope.url_api + "reference/listbycode", { code: "tipe_pengadaan" })
        .success(function (reply) {
        	if (reply.status === 200) {
        		$scope.tipe_pengadaan = reply.result.data;
        		for (var i = 0; i < $scope.tipe_pengadaan.length; i++) {
        			if ($scope.tipe_pengadaan[i].ref_value === item.type) {
        				$scope.selectedTipe = $scope.tipe_pengadaan[i];
        			}
        		}
        	}
        	else {
        		$.growl.error({ title: "[ERROR]", message: "gagal data tipe" });
        		return;
        	}
        })
        .error(function (err) {
        	$.growl.error({ title: "[ERROR]", message: "gagal mendapatkan data tipe" });
        	return;
        });

		for (var i = 0; i <= 2; i++) {
			$scope.approvalke.push(i);
			if (i === Number(item.approval)) {
				$scope.selectedApprovalke = i;
			}
		}
	};

	$scope.update = function () {
		if ($scope.jabatanToEdit.jabatan_nama === "") {
			$.growl.error({ title: "[ERROR]", message: "Nama jabatan belum diisi" });
			return;
		}
		if ($scope.selectedOption === null || $scope.selectedOption === undefined) {
			$.growl.error({ title: "[ERROR]", message: "Role belum dipilih" });
			return;
		}
		if ($scope.selectedTipe === null || $scope.selectedTipe === undefined) {
			$.growl.error({ title: "[ERROR]", message: "Tipe Pengadaan  belum dipilih" });
			return;
		}
		var cek;
		if ($scope.jabatanToEdit.jabatan_nama === $scope.nama_jabatanold) {
			cek = 0;
		} else { cek = 1; }
		//console.info(JSON.stringify($scope.selectedApprovalke)+" :: "+JSON.stringify($scope.selectedTipe)); 
		$rootScope.loadLoadingModal("Proses Simpan");
		$rootScope.authorize(
            $http.post($rootScope.url_api + "jabatan/updatejabatan", {
            	jabatan_id: item.jabatan_id,
            	jabatan_nama: $scope.jabatanToEdit.jabatan_nama, role_id: $scope.selectedOption.role_id,
            	approval: $scope.selectedApprovalke, type: $scope.selectedTipe.ref_value, cek: cek
            })
            .success(function (reply) {
            	console.info(JSON.stringify(reply));
            	$rootScope.unloadLoadingModal();
            	if (reply.status === 200 && reply.result.data.inserted_id === true) {
            		$.growl.notice({ title: "[INFO]", message: "Data Jabatan Berhasil Disimpan" });
            		$modalInstance.close();
            	}
            	else if (reply.status === 200 && reply.result.data === 'duplicate') {
            		$.growl.warning({ title: "[INFO]", message: "Nama jabatan sudah pernah diinputkan" });
            		return;
            	}
            	else {
            		$.growl.error({ title: "[ERROR]", message: "Data Jabatan Gagal Disimpan" });
            		return;
            	}
            })
            .error(function (err) {
            	$rootScope.unloadLoadingModal();
            	$.growl.error({ title: "[ERROR]", message: "gagal akses API" });
            	return;
            })
        );

	};

	$scope.batal = function () {
		$modalInstance.dismiss('cancel');
	};
};

//TODO
var detailJabatanCtrl = function ($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
	$scope.namaJabatan = item.jabatan_nama;
	$scope.idJabatan = item.jabatan_id;
	$scope.pegawai = [];
	$scope.selectedOption;
	$scope.page_id = 9;
	$scope.currentPage = 1;
	$scope.totalItems;
	$scope.maxSize = 10;

	function jumlahdata() {
		$http.post($rootScope.url_api + "jabatan/countpegawai",
            { jabatan_id: $scope.idJabatan })
        .success(function (reply) {
        	if (reply.status === 200) {
        		var data = reply.result.data;
        		$scope.totalItems = data;
        	}
        	else {
        		$.growl.error({ message: "Gagal mendapatkan data jumlah pegawai!" });
        	}
        })
        .error(function (err) {
        	$.growl.error({ message: "Gagal Akses API >" + err });
        });
	}

	$scope.init = function () {
		$rootScope.authorize(jumlahdata());
		$scope.jLoad(1);
	};

	$scope.jLoad = function (current) {
		$scope.pegawai = [];
		$scope.currentPage = current;
		$scope.offset = (current * 10) - 10;
		$rootScope.authorize(
            $http.post($rootScope.url_api + "jabatan/selectpegawai",
                { jabatan_id: $scope.idJabatan, offset: $scope.offset, limit: $scope.maxSize })
            .success(function (reply) {
            	//console.info($scope.offset+":"+$scope.maxSize+":"+$scope.idJabatan+" rep:"+JSON.stringify(reply));
            	if (reply.status === 200) {
            		var data = reply.data;
            		$scope.pegawai = data;
            	}
            	else {
            		$.growl.error({ message: "Gagal mendapatkan data pegawai!" });
            	}
            })
            .error(function (err) {
            	$.growl.error({ message: "Gagal Akses API >" + err });
            })
        );
	};

	$scope.batal = function () {
		$modalInstance.dismiss('cancel');
	};
};

function Jabatan(jabatan_nama) {
	var self = this;
	self.jabatan_nama = jabatan_nama;
}
function Kata(srcText) {
	var self = this;
	self.srcText = srcText;
}