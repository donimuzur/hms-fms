(function () {
    'use strict';

    angular.module("app").controller("PegawaiCtrl", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'PegawaiService', 'RoleService', 'UIControlService', '$uibModal'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, PegawaiService, RoleService, UIControlService, $uibModal) {

        var vm = this;
        vm.totalItems = 0;
        vm.currentPage = 0;
        vm.maxSize = 10;
        vm.Pegawais = [];
        vm.page_id = 35;
        vm.userBisaMengatur;
        vm.kata = new Kata("");
        vm.menuhome = 0;
        vm.userId = 0;
        vm.jLoad2 = jLoad2;
        vm.cariPegawai = cariPegawai;

        vm.edit = edit;


        vm.loadPegawai = loadPegawai;
        function loadPegawai() {
            $translatePartialLoader.addPart('master-pegawai');
            //UIControlService.loadLoading("Silahkan Tunggu...");
            //$rootScope.getSession().then(function (result) {
            //	$rootScope.userSession = result.data.data;
            //	//console.info($rootScope.userSession);
            //	$rootScope.userLogin = $rootScope.userSession.session_data.username;
            //	vm.userId = Number($rootScope.userSession.session_data.pegawai_id);
            //	$rootScope.authorize(loadData());
            //});
            jLoad2(1);
            //jLoad3(1);
            //loadData();
        };

        function loadData() {
            var param = [];
            //param.push($rootScope.userlogged);
            param.push(vm.page_id);

            //RoleService.checkAuthority({
            //	username: $rootScope.userLogin,
            //	page_id: vm.page_id,
            //	jenis_mengatur: 1
            //}, function (reply) {
            //	if (reply.status === 200) {
            //		var data = reply.result.data[0];
            //		vm.userBisaMengatur = $rootScope.strtobool(data.bisa_mengatur);
            //	} else {
            //		$.growl.error({ message: "Gagal mendapatkan data jumlah pegawai!" });
            //	}
            //}, function (err) {
            //	$.growl.error({ message: "Gagal Akses API >" + err });
            //});

            //EmployeeService.count({
            //	nama_pegawai: "%" + vm.kata.srcText + "%",
            //	offset: vm.currentPage,
            //	limit: vm.maxSize
            //}, function (reply) {
            //	if (reply.status === 200) {
            //		var data = reply.result.data;
            //		vm.totalItems = data;
            //	} else {
            //		$.growl.error({ message: "Gagal mendapatkan data jumlah pegawai!" });
            //	}
            //}, function (err) {
            //	$.growl.error({ message: "Gagal Akses API >" + err });
            //});

            PegawaiService.all(function (reply) {
                console.info("reply: " + JSON.stringify(reply));
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.Pegawais = data;
                    vm.totalItems = data.length;
                } else {
                    $.growl.error({ message: "Gagal mendapatkan data pegawai!" });
                    return;
                }
            }, function (err) {
                $.growl.error({ message: "Gagal Akses API >" + err });
                return;
            });

            //EmployeeService.select({
            //	nama_pegawai: "%" + vm.kata.srcText + "%",
            //	offset: 0,
            //	limit: vm.maxSize
            //}, function (reply) {
            //	//console.info("reply: "+JSON.stringify(reply));
            //	if (reply.status === 200) {
            //		var data = reply.result.data;
            //		vm.Pegawais = data;
            //	} else {
            //		$.growl.error({ message: "Gagal mendapatkan data pegawai!" });
            //		return;
            //	}
            //}, function (err) {
            //	$.growl.error({ message: "Gagal Akses API >" + err });
            //	return;
            //});
        }

        







        vm.cariPegawai = cariPegawai;
        function cariPegawai() {
            vm.jLoad2(1);
        }

        vm.jLoad2 = jLoad2;
        function jLoad2(current) {
            //console.info("curr "+current)
            UIControlService.loadLoading("Silahkan Tunggu...");
            vm.currentPage = current;
            var offset = (current * 10) - 10;
            PegawaiService.Select({
                Offset: offset,
                Limit: vm.maxSize,
                Keyword: vm.kata.srcText
            }, function (reply) {
                console.info("data:" + JSON.stringify(reply));
                UIControlService.unloadLoading();
                if (reply.status === 200) {
                    var data = reply.data;
                    vm.Pegawais = data.List;
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
        vm.edit = edit;
        function edit(data) {
            console.info("masuk form edit");
            var data = {
                act: 0,
                item: data
            }
            var modalInstance = $uibModal.open({
                templateUrl: 'app/modules/panitia/master/pegawai/editPegawai.html',
                controller: 'pegawaiModalCtrl',
                controllerAs: 'editPegawaiCtrl',
                resolve: {
                    item: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(function () {
                vm.jLoad2(1);
            });
        }

























        vm.viewPegawai = viewPegawai;
        function viewPegawai(idPeg) {
            var item = { userId: vm.userId, data: idPeg };
            var modalInstance = $modal.open({
                templateUrl: 'viewModalPegawai.html',
                controller: editModalPegawai,
                resolve: {
                    item: function () { return item; }
                }
            });
            modalInstance.result.then(function () {
                $rootScope.authorize(jLoad2());
            });
        };

        /*ini tidak dipakai di TPS*/
        vm.addPegawai = addPegawai;
        function addPegawai(size) {
            var item = { userId: vm.userId };
            var modalInstance = $modal.open({
                templateUrl: 'addModalPegawai.html',
                controller: addModalPegawai,
                size: size,
                resolve: {
                    item: function () { return item; }
                }
            });
            modalInstance.result.then(function (newPeg) {
                $rootScope.authorize(loadData());
            });
        };

        //fungsi edit n open modal form edit
        vm.editPegawai = editPegawai;
        function editPegawai(idPeg) {
            var item = { userId: vm.userId, data: idPeg };
            var modalInstance = $modal.open({
                templateUrl: 'editModalPegawai.html',
                controller: editModalPegawai,
                resolve: {
                    item: function () { return item; }
                }
            });
            modalInstance.result.then(function () {
                $rootScope.authorize(loadData());
            });
        };

        //fungsi delete
        vm.remove = remove;
        function remove(idPeg) {
            var modalInstance = $modal.open({
                templateUrl: 'delModalPegawai.html',
                controller: delPegawai,
                resolve: {
                    item: function () { return idPeg; }
                }
            });
            modalInstance.result.then(function () {
                $rootScope.loadData();
            });
        };
    }
})();

//TODO
//controller editModal
var editModalPegawai = function ($scope, $http, $modalInstance, item, $cookieStore, $rootScope, $modal) {
    $scope.page_id = 35;
    var data = item.data;
    console.info("item: " + JSON.stringify(data));
    $scope.editPegawai = new Pegawai(data.pegawai_id, data.nama_pegawai, data.nik, data.email, data.telepon,
    data.username, data.password, data.created_by, data.created_date, data.updated_by, data.updated_date,
    data.flag_active, data.password, data.jabatan, data.bagian, data.atasan_id);
    $scope.nik_atasan = data.nik_atasan; $scope.nama_atasan = data.nama_atasan;
    $scope.departemen_nama = data.departemen_nama;
    $scope.editPass = new resetPass("", "", "");
    $scope.selectedOption;
    $scope.selectedDepartemen;
    $scope.departemen = [];
    $scope.authority = data.authority;
    $scope.userId = item.userId;
    $scope.selectedJabatan;
    $scope.jabatans = [];
    $scope.role_id = data.role_id; $scope.departemenId = data.departemen; $scope.jabatan = data.jabatan;
    $scope.atasanTerpilih = { pegawai_id: data.atasan_id, nama_pegawai: data.nama_atasan };
    $scope.txt_disabled = true; $scope.newJabatan = "";

    console.info(">" + JSON.stringify(data.role_id));
    function loadData() {
        $http.post($rootScope.url_api + "roles/get_role_user", {
            jenis_role: 'USER', is_active: 1
        }).success(function (reply) {
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.roleUser = data;

                for (var i = 0; i < $scope.roleUser.length; i++) {
                    if ($scope.roleUser[i].role_id === $scope.role_id) {
                        $scope.selectedOption = $scope.roleUser[i];
                        break;
                    }
                }
            } else {
                $.growl.error({ message: "Gagal Mendapatkan Data Role!!" });
                return;
            }
        }).error(function (err) {
            $.growl.error({ message: "Gagal Akses API >" + err });
            $http.post($rootScope.url_api + "logging", {
                message: "Tidak berhasil akses API : " + JSON.stringify(err),
                source: "pajak.js - rekanan/cekBisaMengubahData"
            }).then(function (response) {
                // do nothing
                // don't have to feedback
            });
            $.growl.error({ message: "Gagal Akses API >" + err }); return;
        });
        //isi combo departemen
        $http.get($rootScope.url_api + "departemen/selectActive").success(function (reply) {
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.departemen = data;
                console.info("de" + JSON.stringify($scope.editPegawai));
                for (var i = 0; i < $scope.departemen.length; i++) {
                    if ($scope.departemen[i].departemen_id === $scope.departemenId) {
                        $scope.selectedDepartemen = $scope.departemen[i];
                        break;
                    }
                }
            } else {
                $.growl.error({ message: "Gagal Mendapatkan Data Departemen!!" });
                return;
            }
        }).error(function (err) {
            $.growl.error({ message: "Gagal Akses API >" + err });
            $http.post($rootScope.url_api + "logging", {
                message: "Tidak berhasil akses API : " + JSON.stringify(err),
                source: "pajak.js - rekanan/cekBisaMengubahData"
            }).then(function (response) {
                // do nothing
                // don't have to feedback
            });
            return;
        });

        $http.get($rootScope.url_api + "pegawai/selectjabatan").success(function (reply) {
            if (reply.status === 200) {
                var data = reply.result.data;
                var x = { jabatan: "Buat Baru" };
                for (var i = -1; i < data.length; i++) {
                    if (i === -1) $scope.jabatans.push(x);
                    else $scope.jabatans.push(data[i]);
                }
                for (var i = 0; i < $scope.jabatans.length; i++) {
                    if ($scope.jabatans[i].jabatan === $scope.jabatan) {
                        $scope.selectedJabatan = $scope.jabatans[i];
                        break;
                    }
                }
                //console.info("jabat:"+JSON.stringify($scope.jabatans));
            } else {
                $.growl.error({ message: "Gagal Mendapatkan Data jabatan!!" });
                return;
            }
        }).error(function (err) {
            $.growl.error({ message: "Gagal Akses API >" + err });
            $.growl.error({ message: "Gagal Akses API >" + err });
            $http.post($rootScope.url_api + "logging", {
                message: "Tidak berhasil akses API : " + JSON.stringify(err),
                source: "pajak.js - rekanan/cekBisaMengubahData"
            }).then(function (response) {
                // do nothing
                // don't have to feedback
            });
            return;
        });
    }

    $scope.init = function () {
        $rootScope.authorize(loadData());
    };

    $scope.change = function (obj) {
        $scope.selectedOption = obj;
    };
    $scope.changeDepartemen = function (obj) {
        $scope.selectedDepartemen = obj;
    };

    $scope.changeJabatan = function (obj) {
        $scope.selectedJabatan = obj;
        //console.info("jb:"+jbt);
        if ($scope.selectedJabatan.jabatan === "Buat Baru") {
            $scope.txt_disabled = false;
            $scope.editPegawai.Jabatan = $scope.newJabatan;
        } else {
            $scope.txt_disabled = true;
            $scope.newJabatan = "";
        }
    };

    $scope.pilihAtasan = function () {
        var modalInstance = $modal.open({
            templateUrl: 'PilihAtasan.html',
            controller: PilihAtasanCtrl
        });
        modalInstance.result.then(function (pgw) {
            $scope.atasanTerpilih = pgw;
            $scope.editPegawai.atasan_id = $scope.atasanTerpilih.pegawai_id;
        });
    };

    //update data pegawai
    $scope.updatePegawai = function () {
        if ($scope.selectedDepartemen === undefined) {
            $.growl.error({ title: "[PERINGATAN]", message: "Departemen belum dipilih" });
            return;
        }
        if ($scope.selectedOption === undefined) {
            $.growl.error({ title: "[PERINGATAN]", message: "User role belum dipilih" });
            return;
        }
        if ($scope.editPass.newPass !== $scope.editPass.confirmPass) {
            $.growl.error({ title: "[PERINGATAN]", message: "Gagal membulatkan harga" });
            return;
        }
        if ($scope.atasanTerpilih === undefined) {
            $scope.editPegawai.atasan_id = 0;
        } else {
            $scope.editPegawai.atasan_id = $scope.atasanTerpilih.pegawai_id;
        }
        if ($scope.selectedJabatan.jabatan !== "Buat Baru" && $scope.newJabatan === "") {
            $scope.newJabatan = $scope.selectedJabatan.jabatan;
        }

        $rootScope.loadLoadingModal("Silahkan Tunggu...");
        var pwd = $scope.editPass.newPass;
        var new_pwd;
        if (pwd) {
            new_pwd = $.md5($scope.editPass.newPass);
        } else {
            new_pwd = $scope.editPegawai.Password;
        }
        var arr = {
            nik: $scope.editPegawai.Nik,
            nama_pegawai: $scope.editPegawai.Nama,
            email: $scope.editPegawai.Email,
            telepon: $scope.editPegawai.Telepon,
            username: $scope.editPegawai.Username,
            password: new_pwd,
            flag_active: true,
            jabatan: $scope.newJabatan,
            departemen: $scope.selectedDepartemen.departemen_id,
            role_id: $scope.selectedOption.role_id,
            atasan_id: $scope.editPegawai.atasan_id,
            updated_by: $scope.userId
        };

        $rootScope.authorize($http.post($rootScope.url_api + "pegawai/editpegawai", {
            where: { pegawai_id: $scope.editPegawai.Id },
            data: arr
        }).success(function (reply) {
            if (reply.status === 200 && reply.result.data.inserted_id === true) {
                $.growl.notice({ title: "[INFO]", message: "Berhasil mengubah data pegawai" });
                $rootScope.unloadLoadingModal();
                $modalInstance.close();
            } else {
                $.growl.error({ message: "Gagal mendapatkan data pegawai!!" });
                $rootScope.unloadLoading();
                return;
            }
        }).error(function (err) {
            $.growl.error({ message: "Gagal Akses API >" + err });
            $rootScope.unloadLoading();
            return;
        }));
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};

//TODO
//controlller delete data
var delPegawai = function ($scope, $http, $modalInstance, item, $cookieStore, $rootScope) {
    $scope.page_id = 35;
    $scope.selectedPeg = item;
    $scope.selectedId = $scope.selectedPeg.pegawai_id;

    $scope.delete = function () {
        $rootScope.authorize($http.post($rootScope.url_api + "pegawai/deletepegawai", {
            where: {
                pegawai_id: $scope.selectedPeg.pegawai_id
            }, data: {
                flag_active: false
            }
        }).success(function (reply) {
            if (reply.status === 200) {
                $.growl.notice({
                    title: "[INFO]",
                    message: "Berhasil Menonaktifkan Data Pegawai Tersebut!!"
                });
                $modalInstance.close();
            } else {
                $.growl.error({ message: "Gagal mendapatkan data pegawai!!" });
                return;
            }
        }).error(function (err) {
            $.growl.error({ message: "Gagal Akses API >" + err });
            return;
        }));
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};

//TODO
var addModalPegawai = function ($scope, $modalInstance, $http, item, $rootScope, $modal) {
    $scope.page_id = 35;
    $scope.newPegawai = new Pegawai("", "", "", "", "", "", "", 0, new Date(), 0, new Date(), true, "", "", "");
    $scope.roleUser = [];
    $scope.departemen = [];
    $scope.selectedOption;
    $scope.selectedDepartemen;
    $scope.selectedJabatan;
    $scope.jabatans = [];
    $scope.atasanTerpilih; $scope.txt_disabled = true; $scope.newJabatan = "";
    $scope.userId = item.userId;
    //alert($scope.userId);
    $scope.init = function () {
        //console.info("pegL:"+JSON.stringify($scope.pegawaiTerpilih));
        $http.post($rootScope.url_api + "roles/get_role_user", {
            jenis_role: 'USER', is_active: 1
        }).success(function (reply) {
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.roleUser = data;
            } else {
                $.growl.error({ message: "Gagal Mendapatkan Data Role!!" });
                return;
            }
        }).error(function (err) {
            $.growl.error({ message: "Gagal Akses API >" + err }); return;
        });

        //isi combo departemen
        $http.get($rootScope.url_api + "departemen/selectActive").success(function (reply) {
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.departemen = data;

            } else {
                $.growl.error({ message: "Gagal Mendapatkan data departemen!!" });
                return;
            }
        }).error(function (err) {
            $.growl.error({ message: "Gagal Akses API >" + err });
            return;
        });

        $http.get($rootScope.url_api + "pegawai/selectjabatan").success(function (reply) {
            if (reply.status === 200) {
                var data = reply.result.data;
                var x = { jabatan: "Buat Baru" };
                for (var i = -1; i < data.length; i++) {
                    if (i === -1) $scope.jabatans.push(x);
                    else $scope.jabatans.push(data[i]);
                }
                //console.info("jabat:"+JSON.stringify($scope.jabatans));
            } else {
                $.growl.error({ message: "Gagal Mendapatkan Data jabatan!!" });
                return;
            }
        }).error(function (err) {
            $.growl.error({ message: "Gagal Akses API >" + err }); return;
        });
    };

    $scope.change = function (obj) {
        $scope.selectedOption = obj;
    };

    $scope.changeDepartemen = function (obj) {
        $scope.selectedDepartemen = obj;
    };

    $scope.changeJabatan = function (obj) {
        $scope.selectedJabatan = obj;
        //console.info("jb:"+jbt);
        if ($scope.selectedJabatan.jabatan === "Buat Baru") {
            $scope.txt_disabled = false;
            $scope.newPegawai.Jabatan = $scope.newJabatan;
        } else {
            $scope.txt_disabled = true;
            $scope.newJabatan = "";
        }
    };

    $scope.pilihAtasan = function () {
        var modalInstance = $modal.open({
            templateUrl: 'PilihAtasan.html',
            controller: PilihAtasanCtrl
        });
        modalInstance.result.then(function (pgw) {
            $scope.atasanTerpilih = pgw;
            $scope.newPegawai.atasan_id = $scope.atasanTerpilih.pegawai_id;
        });
    };

    $scope.savePegawai = function () {
        //console.info(JSON.stringify($scope.atasanTerpilih)+":"+$scope.newJabatan);
        var ck_username = /^[A-Za-z0-9_]{1,20}$/;
        var txt_username = $scope.newPegawai.Username;

        if ($scope.selectedDepartemen === undefined) {
            $.growl.error({ title: "[PERINGATAN]", message: "Departemen belum dipilih" });
            return;
        }

        if (!ck_username.test(txt_username)) {
            $.growl.error({ title: "[PERINGATAN]", message: "Username anda tidak valid atau mungkin terlalu panjang.\nMohon tidak menggunakan spasi, atau spesial character.\n Gunakan Username yang mudah diingat." });
            return;
        }
        if ($scope.newPegawai.Password !== $scope.newPegawai.ConfirmPassword) {
            $.growl.error({ title: "[PERINGATAN]", message: "Password tidak sama" });
            return;
        }
        if ($scope.selectedOption === undefined) {
            $.growl.error({ title: "[PERINGATAN]", message: "User role belum dipilih" });
            return;
        }
        if ($scope.atasanTerpilih === undefined) {
            $scope.newPegawai.atasan_id = 0;
        } else {
            $scope.newPegawai.atasan_id = $scope.atasanTerpilih.pegawai_id;
        }
        if ($scope.selectedJabatan.jabatan !== "Buat Baru" && $scope.newJabatan === "") {
            $scope.newJabatan = $scope.selectedJabatan.jabatan;
        }

        $rootScope.authorize($http.post($rootScope.url_api + "pegawai/insertpegawai", {
            nik: $scope.newPegawai.Nik,
            nama_pegawai: $scope.newPegawai.Nama,
            email: $scope.newPegawai.Email,
            telepon: $scope.newPegawai.Telepon,
            username: $scope.newPegawai.Username,
            password: $.md5($scope.newPegawai.Password),
            flag_active: true,
            jabatan: $scope.newJabatan,
            departemen: $scope.selectedDepartemen.departemen_id,
            role_id: $scope.selectedOption.role_id,
            atasan_id: $scope.newPegawai.atasan_id,
            created_by: $scope.userId
        }).success(function (reply) {
            $rootScope.unloadLoadingModal();
            console.info("insert: " + JSON.stringify(reply));
            if (reply.status === 200 && reply.result.data.inserted_id === true) {
                $.growl.notice({ title: "[INFO]", message: "Berhasil menambah data pegawai" });
                $modalInstance.close();
            } else if (reply.status === 300 && reply.result.data === 'duplicate') {
                $.growl.warning({ title: "[INFO]", message: "Username sudah ada" });
                return;
            } else {
                $.growl.error({ message: "Gagal mendapatkan data pegawai!!" });
                $rootScope.unloadLoading();
                return;
            }
        }).error(function (err) {
            $.growl.error({ message: "Gagal Akses API >" + err });
            $http.post($rootScope.url_api + "logging", {
                message: "Tidak berhasil akses API : " + JSON.stringify(err),
                source: "pajak.js - rekanan/cekBisaMengubahData"
            }).then(function (response) {
                // do nothing
                // don't have to feedback
            });
            $rootScope.unloadLoading();
            return;
        }));
    };
    //end fungsi save        
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};

//TODO
var PilihAtasanCtrl = function ($scope, $modalInstance, $http, $cookieStore, $rootScope) {
    $scope.pegawai = [];
    $scope.totalItems = 0;
    $scope.currentPage = 1;
    $scope.maxSize = 10;
    $scope.kata = new Kata("");
    $scope.page_id = 10;

    $scope.initialize = function () {
        $rootScope.authorize(loadAwal());
    };

    function loadAwal() {

        $rootScope.loadLoadingModal("Silahkan Tunggu...");
        $http.post($rootScope.url_api + "pegawai/countsearchpanitia", {
            nama_pegawai: "%" + $scope.kata.srcText + "%"
        }).success(function (reply) {
            $rootScope.unloadLoadingModal();
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.totalItems = data;
            } else {
                $.growl.error({ message: "Gagal mendapatkan data jumlah pegawai!" });
                return;
            }
        }).error(function (err) {
            $.growl.error({ message: "Gagal Akses API >" + err });
            return;
        });

        $http.post($rootScope.url_api + "pegawai/selectsearchpanitia", {
            nama_pegawai: "%" + $scope.kata.srcText + "%", offset: 0, limit: 10
        }).success(function (reply) {
            $rootScope.unloadLoadingModal();
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.pegawai = data;
            } else {
                $.growl.error({ message: "Gagal mendapatkan data pegawai!" });
                return;
            }
        }).error(function (err) {
            $.growl.error({ message: "Gagal Akses API >" + err });
            return;
        });
    }

    $scope.jLoad = function (current) {

        $rootScope.loadLoadingModal("Silahkan Tunggu...");
        $scope.pegawai = [];
        $scope.currentPage = current;
        $scope.offset = (current * 10) - 10;


        $http.post($rootScope.url_api + "pegawai/selectsearchpanitia", {
            nama_pegawai: "%" + $scope.kata.srcText + "%", offset: $scope.offset, limit: 10
        }).success(function (reply) {
            $rootScope.unloadLoadingModal();
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.pegawai = data;
            } else {
                $.growl.error({ message: "Gagal mendapatkan data pegawai!" });
                return;
            }
        }).error(function (err) {
            $.growl.error({ message: "Gagal Akses API >" + err });
            return;
        });
    };

    $scope.cariPegawai = function () {
        $rootScope.loadLoadingModal("Silahkan Tunggu...");
        $http.post($rootScope.url_api + "pegawai/countsearchpanitia", {
            nama_pegawai: "%" + $scope.kata.srcText + "%"
        }).success(function (reply) {
            $rootScope.unloadLoadingModal();
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.totalItems = data;
            } else {
                $.growl.error({ message: "Gagal mendapatkan data jumlah pegawai!" });
                return;
            }
        }).error(function (err) {
            $.growl.error({ message: "Gagal Akses API >" + err });
            return;
        });

        $http.post($rootScope.url_api + "pegawai/selectsearchpanitia", {
            nama_pegawai: "%" + $scope.kata.srcText + "%", offset: 0, limit: 10
        }).success(function (reply) {
            $rootScope.unloadLoadingModal();
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.pegawai = data;
            } else {
                $.growl.error({ message: "Gagal mendapatkan data pegawai!" });
                return;
            }
        }).error(function (err) {
            $.growl.error({ message: "Gagal Akses API >" + err });
            return;
        });
    };

    $scope.pilihAtasan = function (pgw) {
        $modalInstance.close(pgw);
    };


    $scope.keluar = function () {
        $modalInstance.dismiss('cancel');
    };
};

//fungsi field
function Pegawai(Id, Nama, Nik, Email, Telepon, Username, Password, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, FlagActive, ConfirmPassword, Jabatan, Bagian, Tgl_reset, atasan_id) {
    var self = this;
    self.Id = Id;
    self.Nama = Nama;
    self.Nik = Nik;
    self.Email = Email;
    self.Telepon = Telepon;
    self.Username = Username;
    self.Password = Password;
    self.Jabatan = Jabatan;
    self.Bagian = Bagian;
    self.Createdby = CreatedBy;
    self.CreatedDate = CreatedDate;
    self.UpdatedBy = UpdatedBy;
    self.UpdatedDate = UpdatedDate;
    self.FlagActive = FlagActive;
    self.ConfirmPassword = ConfirmPassword;
    self.Tgl_reset = Tgl_reset;
    self.atasan_id = atasan_id;
}

function Kata(srcText) {
    var self = this;
    self.srcText = srcText;
}