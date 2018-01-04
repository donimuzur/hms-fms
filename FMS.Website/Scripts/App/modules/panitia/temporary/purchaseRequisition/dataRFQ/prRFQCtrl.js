angular.module('eprocAppPanitia').controller('MasterRequisitionPRRFQCtrl', function ($scope, $http, $rootScope, $modal, $cookieStore) {
    $scope.totalItems = 0;
    $scope.currentPage = 0;
    $scope.maxSize = 10;
    $scope.Pegawais = [];
    $scope.page_id = 35;
    $scope.userBisaMengatur;
    $scope.kata = new Kata("");
    $scope.menuhome = 0;
    $scope.userId = 0;
    $scope.rfqList = [
        {rfqName: 'Tender Barang A', rfqDate: '2016-01-01', status: 'Approved', remark: ''},
        {rfqName: 'Tender Barang B', rfqDate: '2016-01-02', status: 'On Process', remark: ''},
        {rfqName: 'Tender Barang C', rfqDate: '2016-01-03', status: 'Cancel', remark: ''},
        {rfqName: 'Tender Barang D', rfqDate: '2016-01-04', status: 'Draft', remark: ''},
        {rfqName: 'Tender Barang E', rfqDate: '2016-01-05', status: 'Draft', remark: ''},
        {rfqName: 'Tender Barang F', rfqDate: '2016-01-06', status: 'Draft', remark: ''},
        {rfqName: 'Tender Barang G', rfqDate: '2016-01-07', status: 'Draft', remark: ''},
        {rfqName: 'Tender Barang H', rfqDate: '2016-01-08', status: 'Draft', remark: ''},
        {rfqName: 'Tender Barang I', rfqDate: '2016-01-09', status: 'Draft', remark: ''},
        {rfqName: 'Tender Barang J', rfqDate: '2016-01-10', status: 'Draft', remark: ''}
    ];

    $scope.loadPegawai = function () {
        $rootScope.getSession().then(function (result) {
            $rootScope.userSession = result.data.data;
            //console.info($rootScope.userSession);
            $rootScope.userLogin = $rootScope.userSession.session_data.username;
            $scope.userId = Number($rootScope.userSession.session_data.pegawai_id);
            $rootScope.authorize(loadData());
        });
    };

    /* awal pencarian  edit by ani */
    $scope.cariPegawai = function () {
        $http.post($rootScope.url_api + "pegawai/pegawaicountsearch", {
            nama_pegawai: "%" + $scope.kata.srcText + "%", "offset": $scope.currentPage, "limit": $scope.maxSize
        }).success(function (reply) {
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.totalItems = data;
            } else {
                $.growl.error({message: "Gagal mendapatkan data jumlah pegawai!"});
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
        });

        $http.post($rootScope.url_api + "pegawai/pegawaisearch", {
            nama_pegawai: "%" + $scope.kata.srcText + "%", "offset": $scope.currentPage, "limit": $scope.maxSize
        }).success(function (reply) {
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.Pegawais = data;
            } else {
                $.growl.error({message: "Gagal mendapatkan data pegawai!"});
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
        });
    };

    function loadData() {
        var param = [];
        param.push($rootScope.userlogged);
        param.push($scope.page_id);

        $http.post($rootScope.url_api + "roles/check_authority", {
            username: $rootScope.userLogin, page_id: $scope.page_id, jenis_mengatur: 1
        }).success(function (reply) {
            if (reply.status === 200) {
                var data = reply.result.data[0];
                $scope.userBisaMengatur = $rootScope.strtobool(data.bisa_mengatur);
            } else {
                $.growl.error({message: "Gagal mendapatkan data jumlah pegawai!"});
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
        });

        $http.post($rootScope.url_api + "pegawai/pegawaicountsearch", {
            nama_pegawai: "%" + $scope.kata.srcText + "%", "offset": $scope.currentPage, "limit": $scope.maxSize
        }).success(function (reply) {
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.totalItems = data;
            } else {
                $.growl.error({message: "Gagal mendapatkan data jumlah pegawai!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
            return;
        });

        $http.post($rootScope.url_api + "pegawai/pegawaisearch", {
            nama_pegawai: "%" + $scope.kata.srcText + "%", "offset": 0, "limit": $scope.maxSize
        }).success(function (reply) {
            //console.info("reply: "+JSON.stringify(reply));
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.Pegawais = data;
            } else {
                $.growl.error({message: "Gagal mendapatkan data pegawai!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
            return;
        });
    }
    $scope.jLoad2 = function (current) {
        $scope.Pegawais = [];
        $scope.currentPage = current;
        $scope.offset = (current * 10) - 10;
        $rootScope.authorize($http.post($rootScope.url_api + "pegawai/pegawaisearch", {
            nama_pegawai: "%" + $scope.kata.srcText + "%", "offset": $scope.offset, "limit": $scope.maxSize
        }).success(function (reply) {
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.Pegawais = data;
            } else {
                $.growl.error({message: "Gagal mendapatkan data pegawai!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
            return;
        }));
    };

    $scope.viewPegawai = function (idPeg) {
        var item = {
            userId: $scope.userId,
            data: idPeg
        };
        var modalInstance = $modal.open({
            templateUrl: 'viewModalPegawai.html',
            controller: editModalPegawai,
            resolve: {item: function () {
                    return item;
                }}
        });
        modalInstance.result.then(function () {
            $rootScope.authorize(loadData());
        });
    };

    /*ini tidak dipakai di TPS*/
    $scope.addRFQ = function (size) {
        var item = {userId: $scope.userId};
        var modalInstance = $modal.open({
            templateUrl: 'addRFQ.html',
            controller: addModalRFQ,
            size: size,
            resolve: {item: function () {
                    return item;
                }}
        });
        modalInstance.result.then(function (newPeg) {
            $rootScope.authorize(loadData());
        });
    };
    
 var outstandingPRmodal = function ($scope, $modalInstance, $http, item, $rootScope, $modal) {
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    //$scope.
    //alert();
};   

    $scope.outstandingPR = function (size) {
        var item = {userId: $scope.userId};
        var modalInstance = $modal.open({
            templateUrl: 'outstandingPR.html',
            controller: outstandingPRmodal,
            size: size,
            resolve: {item: function () {
                    return item;
                }}
        });
        modalInstance.result.then(function (newPeg) {
            $rootScope.authorize(loadData());
        });
    };




    //fungsi edit n open modal form edit
    $scope.editPegawai = function (idPeg) {
        console.info("uu:" + $scope.userId);
        var item = {userId: $scope.userId, data: idPeg};
        var modalInstance = $modal.open({
            templateUrl: 'editModalPegawai.html',
            controller: editModalPegawai,
            resolve: {item: function () {
                    return item;
                }}
        });
        modalInstance.result.then(function () {
            $rootScope.authorize(loadData());
        });
    };

    //fungsi delete
    $scope.remove = function (idPeg) {
        var modalInstance = $modal.open({
            templateUrl: 'delModalPegawai.html',
            controller: delPegawai,
            resolve: {item: function () {
                    return idPeg;
                }}
        });
        modalInstance.result.then(function () {
            $rootScope.loadData();
        });
    };
}).controller('ResetPasswordPegawaiCtrl', function ($scope, $rootScope, $modal, $cookieStore, $http) {
    $scope.totalItems = 0;
    $scope.currentPage = 0;
    $scope.maxSize = 10;
    $scope.Pegawais = [];
    $scope.page_id = 36;
    $scope.userBisaMengatur = false;
    $scope.menuhome = 0;

    //fungsi untuk load data reset dari tabel
    $scope.loadResetPass = function () {
        $scope.menuhome = $rootScope.menuhome;
        $rootScope.getSession().then(function (result) {
            $rootScope.userSession = result.data.data;
            $rootScope.userLogin = $rootScope.userSession.session_data.username;
            $rootScope.authorize(loadDataReset());
        });
    };

    //fungsi untuk menampilkan data pegawai
    function loadDataReset() {
        $http.post($rootScope.url_api + "roles/check_authority", {
            username: $rootScope.userLogin, page_id: $scope.page_id, jenis_mengatur: 1
        }).success(function (reply) {
            if (reply.status === 200) {
                var data = reply.result.data[0];
                $scope.userBisaMengatur = data;
            } else {
                $.growl.error({message: "Gagal mendapatkan data hak akses!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
            return;
        });

        $scope.currentPage = 1;
        $http.get($rootScope.url_api + "pegawai/hitungreset", {
            username: $rootScope.userLogin, page_id: $scope.page_id
        }).success(function (reply) {
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.totalItems = data;
            } else {
                $.growl.error({message: "Gagal mendapatkan data hak akses!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
            return;
        });

        $scope.Pegawais = [];
        var offset = 0;
        var limit = 10;
        $http.post($rootScope.url_api + "pegawai/find", {
            offset: offset, limit: limit
        }).success(function (reply) {
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.Pegawais = data;
            } else {
                $.growl.error({message: "Gagal mendapatkan data hak akses!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
            return;
        });

    }

    //fungsi paging
    $scope.jLoad = function (current, urutan) {
        $scope.Pegawais = [];
        $scope.currentPage = current;
        $scope.offset = (current * 10) - 10;
        var limit = 10;

        $http.post($rootScope.url_api + "pegawai/find", {
            offset: $scope.offset, limit: limit
        }).success(function (reply) {
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.Pegawais = data;
            } else {
                $.growl.error({message: "Gagal mendapatkan data hak akses!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
            return;
        });
    };

    //fungsi lihat detail pegawai
    $scope.viewPegawai = function (idPeg) {
        var modalInstance = $modal.open({
            templateUrl: 'viewModalPegawai.html',
            controller: editModalPegawai,
            resolve: {item: function () {
                    return idPeg;
                }}
        });
        modalInstance.result.then(function () {
            loadData();
        });
    };

    //fungsi reset password
    $scope.reset = function (idPeg) {
        var modalInstance = $modal.open({
            templateUrl: 'resetModalPegawai.html',
            controller: resetPassword,
            resolve: {item: function () {
                    return idPeg;
                }}
        });
        modalInstance.result.then(function () {
            loadDataReset();
        });
    };

});

//controller editModal
var editModalPegawai = function ($scope, $http, $modalInstance, item, $cookieStore, $rootScope, $modal) {
    $scope.page_id = 35;
    var data = item.data;
    console.info("item: " + JSON.stringify(data));
    $scope.editPegawai = new Pegawai(data.pegawai_id, data.nama_pegawai, data.nik, data.email, data.telepon,
            data.username, data.password, data.created_by, data.created_date, data.updated_by, data.updated_date,
            data.flag_active, data.password, data.jabatan, data.bagian, data.atasan_id);
    $scope.nik_atasan = data.nik_atasan;
    $scope.nama_atasan = data.nama_atasan;
    $scope.departemen_nama = data.departemen_nama;
    $scope.editPass = new resetPass("", "", "");
    $scope.selectedOption;
    $scope.selectedDepartemen;
    $scope.departemen = [];
    $scope.authority = data.authority;
    $scope.userId = item.userId;
    $scope.selectedJabatan;
    $scope.jabatans = [];
    $scope.role_id = data.role_id;
    $scope.departemenId = data.departemen;
    $scope.jabatan = data.jabatan;
    $scope.atasanTerpilih = {pegawai_id: data.atasan_id, nama_pegawai: data.nama_atasan};
    $scope.txt_disabled = true;
    $scope.newJabatan = "";

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
                $.growl.error({message: "Gagal Mendapatkan Data Role!!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
            $http.post($rootScope.url_api + "logging", {
                message: "Tidak berhasil akses API : " + JSON.stringify(err),
                source: "pajak.js - rekanan/cekBisaMengubahData"
            }).then(function (response) {
                // do nothing
                // don't have to feedback
            });
            $.growl.error({message: "Gagal Akses API >" + err});
            return;
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
                $.growl.error({message: "Gagal Mendapatkan Data Departemen!!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
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
                var x = {jabatan: "Buat Baru"};
                for (var i = -1; i < data.length; i++) {
                    if (i === -1)
                        $scope.jabatans.push(x);
                    else
                        $scope.jabatans.push(data[i]);
                }
                for (var i = 0; i < $scope.jabatans.length; i++) {
                    if ($scope.jabatans[i].jabatan === $scope.jabatan) {
                        $scope.selectedJabatan = $scope.jabatans[i];
                        break;
                    }
                }
                //console.info("jabat:"+JSON.stringify($scope.jabatans));
            } else {
                $.growl.error({message: "Gagal Mendapatkan Data jabatan!!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
            $.growl.error({message: "Gagal Akses API >" + err});
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
            $.growl.error({title: "[PERINGATAN]", message: "Departemen belum dipilih"});
            return;
        }
        if ($scope.selectedOption === undefined) {
            $.growl.error({title: "[PERINGATAN]", message: "User role belum dipilih"});
            return;
        }

        if ($scope.editPass.newPass !== $scope.editPass.confirmPass) {
            $.growl.error({title: "[PERINGATAN]", message: "Gagal membulatkan harga"});
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
            where: {pegawai_id: $scope.editPegawai.Id}, data: arr
        }).success(function (reply) {
            if (reply.status === 200 && reply.result.data.inserted_id === true) {
                $.growl.notice({title: "[INFO]", message: "Berhasil mengubah data pegawai"});
                $rootScope.unloadLoadingModal();
                $modalInstance.close();
            } else {
                $.growl.error({message: "Gagal mendapatkan data pegawai!!"});
                $rootScope.unloadLoading();
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
            $rootScope.unloadLoading();
            return;
        }));
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};

//controlller delete data
var delPegawai = function ($scope, $http, $modalInstance, item, $cookieStore, $rootScope) {
    $scope.page_id = 35;
    $scope.selectedPeg = item;
    $scope.selectedId = $scope.selectedPeg.pegawai_id;

    $scope.delete = function () {
        $rootScope.authorize($http.post($rootScope.url_api + "pegawai/deletepegawai", {
            where: {
                pegawai_id: $scope.selectedPeg.pegawai_id
            }, data: {flag_active: false}
        }).success(function (reply) {
            if (reply.status === 200) {
                $.growl.notice({title: "[INFO]", message: "Berhasil Menonaktifkan Data Pegawai Tersebut!!"});
                $modalInstance.close();
            } else {
                $.growl.error({message: "Gagal mendapatkan data pegawai!!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
            return;
        }));
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};

//controller reset data
var resetPassword = function ($scope, $modalInstance, item, $cookieStore, $rootScope, $http) {
    $scope.page_id = 36;
    $scope.editPass = new resetPass(item.pegawai_id, "", "");
    $scope.username_pegawai = item.username;

    $scope.resetPass = function () {
        $scope.isSaving = true;

        $http.post($rootScope.url_api + "pegawai/resetpwd", {
            where: {
                pegawai_id: $scope.editPass.Id
            }, data: {password: $.md5("user12345")}
        }).success(function (reply) {
            if (reply.status === 200) {
                $http.post($rootScope.url_api + "pegawai/getemail", {
                    username: $scope.username_pegawai
                }).success(function (reply1) {
                    if (reply1.status === 200) {
                        var data1 = reply1.result.data[0];
                        $scope.email_pegawai = data1.email;
                        $.growl.notice({title: "[INFO]", message: "Reset password berhasil"});
                        /*send email*/
                        var variables = [];
                        var mailBody = "";
                        var mailSubject = "";

                        $http.post($rootScope.url_api + "mailconfig/getcontent", {
                            id_konten_email: 17, variables: variables
                        }).success(function (reply2) {
                            if (reply2.status === 200) {
                                mailBody = reply2.result.data[0].mailBody;
                                mailSubject = reply2.result.data[0].mailSubject;

                                /*ini belum ada API*/
                                //                                eb.send('itp.pegawai.sendNotification', {
                                //                                    from: $rootScope.email,
                                //                                    to: $scope.email_pegawai,
                                //                                    subject: mailSubject,
                                //                                    body: mailBody
                                //                                }, function(adm) {
                                //                                    if (adm.status === 'ok') {
                                //                                        $.growl.notice({title: "[INFO]", message: "Email notifikasi berhasil terkirim"});
                                //                                        $modalInstance.close();
                                //                                        $scope.$apply();
                                //                                    }
                                //                                    else {
                                //                                        $.growl.error({title: "[PERINGATAN]", message: "Email notifikasi gagal terkirim"});
                                //                                        $modalInstance.close();
                                //                                        $scope.$apply();
                                //                                    }
                                //                                });
                            } else {
                                $.growl.error({message: "Gagal mendapatkan data konten email!"});
                                return;
                            }
                        }).error(function (err2) {
                            $.growl.error({message: "Gagal Akses API Email Konten>" + err2});
                            return;
                        });
                    } else {
                        $.growl.error({message: "Gagal melakukan reset password!"});
                        return;
                    }
                }).error(function (err1) {
                    $.growl.error({message: "Gagal Akses API Email>" + err1});
                    return;
                });
            } else {
                $.growl.error({message: "Gagal melakukan reset password!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
            return;
        });
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
};



var detailTahapanModal = function ($scope, $modalInstance, $http, item, $rootScope, $modal) {
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.dokumenLain = function (size) {
        console.info("okeee");
        var modalInstance = $modal.open({
            templateUrl: 'dokumenLain.html',
            controller: dokumenModal,
            size: size,
            resolve: {item: function () {
                    return item;
                }}
        });
        modalInstance.result.then(function (newPeg) {
            $rootScope.authorize(loadData());
        });
    };
    //$scope.
    //alert();
};

var tambahVendorModal = function ($scope, $modalInstance, $http, item, $rootScope, $modal) {
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    //$scope.
    //alert();
};

var viewVendorModal = function ($scope, $modalInstance, $http, item, $rootScope, $modal) {
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    //$scope.
    //alert();
};

var modalpilihItemPR = function ($scope, $modalInstance, $http, item, $rootScope, $modal) {
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    //$scope.
    //alert();
};

var dokumenModal = function ($scope, $modalInstance, $http, item, $rootScope, $modal) {
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
    //$scope.
    //alert();
};

var addModalRFQ = function ($scope, $modalInstance, $http, item, $rootScope, $modal) {
    $scope.page_id = 35;
    $scope.newPegawai = new Pegawai("", "", "", "", "", "", "", 0, new Date(), 0, new Date(), true, "", "", "");
    $scope.roleUser = [];
    $scope.departemen = [];
    $scope.selectedOption;
    $scope.selectedDepartemen;
    $scope.selectedJabatan;
    $scope.jabatans = [];
    $scope.atasanTerpilih;
    $scope.txt_disabled = true;
    $scope.newJabatan = "";
    $scope.userId = item.userId;
    console.info("addModalRFQ ctrl");
    $scope.pilihItemPR = function (size) {
        var item = {userId: $scope.userId};
        var modalInstance = $modal.open({
            templateUrl: 'pilihItemPR.html',
            controller: modalpilihItemPR,
            size: size,
            resolve: {item: function () {
                    return item;
                }}

        });

        modalInstance.result.then(function (newPeg) {
            $rootScope.authorize(loadData());
        });
    };




    $scope.viewVendor = function (size) {
        var modalInstance = $modal.open({
            templateUrl: 'viewVendorRFQ.html',
            controller: viewVendorModal,
            size: size,
            resolve: {item: function () {
                    return item;
                }}
        });
        modalInstance.result.then(function (newPeg) {
            $rootScope.authorize(loadData());
        });
    };


    $scope.tambahVendor = function (size) {
        var modalInstance = $modal.open({
            templateUrl: 'tambahVendorRFQ.html',
            controller: tambahVendorModal,
            size: size,
            resolve: {item: function () {
                    return item;
                }}
        });
        modalInstance.result.then(function (newPeg) {
            $rootScope.authorize(loadData());
        });
    };

    $scope.detailTahapan = function (size) {
        var modalInstance = $modal.open({
            templateUrl: 'detailTahapan.html',
            controller: detailTahapanModal,
            size: size,
            resolve: {item: function () {
                    return item;
                }}
        });
        modalInstance.result.then(function (newPeg) {
            $rootScope.authorize(loadData());
        });
    };


    $scope.rfqList = [
        {stageName: 'Pengumuman Tender', startDate: '2016-01-01', endDate: '2016-01-01', duration: ''},
        {stageName: 'Pemasukan Penawaran', startDate: '2016-01-02', endDate: '2016-01-02', duration: ''},
        {stageName: 'Pembukaan Penawaran', startDate: '2016-01-03', endDate: '2016-01-03', duration: ''},
        {stageName: 'Evaluasi Penawaran', startDate: '2016-01-04', endDate: '2016-01-04', duration: ''},
        {stageName: 'Penetapan Calon Pemenang', startDate: '2016-01-05', endDate: '2016-01-05', duration: ''},
        {stageName: 'Klasifikasi dan Negosiasi', startDate: '2016-01-06', endDate: '2016-01-06', duration: ''},
        {stageName: 'Penunjukan Pemenang', startDate: '2016-01-07', endDate: '2016-01-07', duration: ''}
    ];
    $scope.vendors = [
        {name: 'PT. Karya Serindo Utama', emailAddr: 'info@serindo.com'},
        {name: 'PT. Sentral Laharindo Service', emailAddr: 'info@sentral.com'}
    ];
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
                $.growl.error({message: "Gagal Mendapatkan Data Role!!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
            return;
        });

        //isi combo departemen
        $http.get($rootScope.url_api + "departemen/selectActive").success(function (reply) {
            if (reply.status === 200) {
                var data = reply.result.data;
                $scope.departemen = data;
            } else {
                $.growl.error({message: "Gagal Mendapatkan data departemen!!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
            return;
        });

        $http.get($rootScope.url_api + "pegawai/selectjabatan").success(function (reply) {
            if (reply.status === 200) {
                var data = reply.result.data;
                var x = {jabatan: "Buat Baru"};
                for (var i = -1; i < data.length; i++) {
                    if (i === -1) {
                        $scope.jabatans.push(x);
                    } else {
                        $scope.jabatans.push(data[i]);
                    }
                }
                //console.info("jabat:"+JSON.stringify($scope.jabatans));
            } else {
                $.growl.error({message: "Gagal Mendapatkan Data jabatan!!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
            return;
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
            $.growl.error({title: "[PERINGATAN]", message: "Departemen belum dipilih"});
            return;
        }

        if (!ck_username.test(txt_username)) {
            $.growl.error({title: "[PERINGATAN]", message: "Username anda tidak valid atau mungkin terlalu panjang.\nMohon tidak menggunakan spasi, atau spesial character.\n Gunakan Username yang mudah diingat."});
            return;
        }
        if ($scope.newPegawai.Password !== $scope.newPegawai.ConfirmPassword) {
            $.growl.error({title: "[PERINGATAN]", message: "Password tidak sama"});
            return;
        }
        if ($scope.selectedOption === undefined) {
            $.growl.error({title: "[PERINGATAN]", message: "User role belum dipilih"});
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
                $.growl.notice({title: "[INFO]", message: "Berhasil menambah data pegawai"});
                $modalInstance.close();
            } else if (reply.status === 300 && reply.result.data === 'duplicate') {
                $.growl.warning({title: "[INFO]", message: "Username sudah ada"});
                return;
            } else {
                $.growl.error({message: "Gagal mendapatkan data pegawai!!"});
                $rootScope.unloadLoading();
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
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
                $.growl.error({message: "Gagal mendapatkan data jumlah pegawai!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
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
                $.growl.error({message: "Gagal mendapatkan data pegawai!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
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
                $.growl.error({message: "Gagal mendapatkan data pegawai!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
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
                $.growl.error({message: "Gagal mendapatkan data jumlah pegawai!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
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
                $.growl.error({message: "Gagal mendapatkan data pegawai!"});
                return;
            }
        }).error(function (err) {
            $.growl.error({message: "Gagal Akses API >" + err});
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
function resetPass(Id, newPass, confirmPass) {
    var self = this;
    self.Id = Id;
    self.newPass = newPass;
    self.confirmPass = confirmPass;
}
function Kata(srcText) {
    var self = this;
    self.srcText = srcText;
}