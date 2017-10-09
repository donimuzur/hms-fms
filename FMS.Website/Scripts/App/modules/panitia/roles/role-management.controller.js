angular.module('eprocAppPanitia')
    .controller('hakAksesCreateCtrl', function($scope, $http, $cookieStore, $rootScope, $modal) {
        $scope.roles = [];
        $scope.page_id = 21;
        $scope.userBisaMenambah = false;
        $scope.userBisaMengubah = false;

        $scope.init = function() {
            $rootScope.getSession().then(function(result){
                $rootScope.userSession = result.data.data;
                $rootScope.userLogin = $rootScope.userSession.session_data.username;
                $rootScope.authorize(loadRoles());
            });  

            $rootScope.addBreadcrumbItem({
                url: "hak-akses/create",
                label: "Master Hak Akses",
                icon: "fa-building",
                lastIndex: true
            });
        };

        function loadRoles() {
            //cek bisa menambah
            $http.post($rootScope.url_api+"roles/check_authority",
                {username: $rootScope.userLogin,  page_id: $scope.page_id, jenis_mengatur: 2})
            .success(function(reply){
                if(reply.status === 200){
                    var data = reply.result.data[0];
                    $scope.userBisaMenambah = data.bisa_mengatur;
                }
                else{
                    $.growl.error({ message: "Gagal mendapatkan data hak akses!" });
                    return;
                }
            })
            .error(function(err) {
                $.growl.error({ message: "Gagal Akses API >"+err });
                return;
            });
            //cek bisa mengubah
            $http.post($rootScope.url_api+"roles/check_authority",
                {username: $rootScope.userLogin,  page_id: $scope.page_id, jenis_mengatur: 3})
            .success(function(reply){
                if(reply.status === 200){
                    var data = reply.result.data[0];
                    $scope.userBisaMengubah = data.bisa_mengatur;
                }
                else{
                    $.growl.error({ message: "Gagal mendapatkan data hak akses!" });
                    return;
                }
            })
            .error(function(err) {
                $.growl.error({ message: "Gagal Akses API >"+err });
                return;
            });
            //itp.role.selectAll
            $rootScope.loadLoading('Silahkan Tunggu...');
            $http.get($rootScope.url_api+"roles/list")
            .success(function(reply){
                if(reply.status === 200){
                    var data = reply.result.data;
                    $scope.roles = data;
                    $rootScope.unloadLoading();
                }
                else{
                    $.growl.error({ message: "Gagal mendapatkan data hak akses!" });
                    $rootScope.unloadLoading();
                    return;
                }
            })
            .error(function(err) {
                $.growl.error({ message: "Gagal Akses API >"+err });
                return;
            });

        };

        $scope.tambahRole = function() {
            var modalInstance = $modal.open({
                templateUrl: 'tambahRole.html',
                controller: tambahRoleCtrl
            });
            modalInstance.result.then(function() {
                loadRoles();
            });
        };

        $scope.viewRoleDetail = function(role) {
            var modalInstance = $modal.open({
                templateUrl: 'viewRoleDetail.html',
                controller: viewRoleDetailCtrl,
                resolve: {
                    item: function() {
                        return role;
                    }
                }
            });
        };

        $scope.ubahRoleDetail = function(role) {
            var modalInstance = $modal.open({
                templateUrl: 'ubahRoleDetail.html',
                controller: ubahRoleDetailCtrl,
                resolve: {
                    item: function() {
                        return role;
                    }
                }
            });
            modalInstance.result.then(function() {
                loadRoles();
            });
        };
    });

var viewRoleDetailCtrl = function($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
    $scope.accessrole = [];
    $scope.role = item;
    $scope.page_id = 21;

    $scope.init = function() {
        
        $http.get($rootScope.url_api+"roles/detail/"+item.role_id)
        .success(function(reply){
            //console.info(item.role_id+" rep:"+JSON.stringify(reply));
            if(reply.status === 200){
                $scope.tampung = reply.result.data;
                var submenu;
                var submenus = [];
                var subsubmenus = [];
                var masuk = [];
                for (var p = 0; p < $scope.tampung.length; p++) {
                    masuk[p] = false;
                }
                for (var i = 0; i < $scope.tampung.length; i++) {
                    if (!($scope.tampung[i].menu_parent === '0') || masuk[i] === 'true')
                        continue;
                    for (var j = 0; j < $scope.tampung.length; j++) {
                        if (j === i || masuk[j] === 'true')
                            continue;
                        if ($scope.tampung[j].menu_parent === $scope.tampung[i].menu_id) {
                            submenu = $scope.tampung[j];
                            for (var k = 0; k < $scope.tampung.length; k++) {
                                if (k === i || k === j || masuk[k] === 'true')
                                    continue;
                                if ($scope.tampung[k].menu_parent === $scope.tampung[j].menu_id) {
                                    subsubmenus.push($scope.tampung[k]);
                                    masuk[k] = true;
                                }
                            }
                            submenu.childs = subsubmenus;
                            subsubmenus = [];
                            submenus.push(submenu);
                            masuk[j] = true;
                        }
                    }
                    $scope.tampung[i].childs = submenus;
                    submenus = [];
                    $scope.accessrole.push($scope.tampung[i]);
                    masuk[i] = true;
                }
            }
            else{
                $.growl.error({ message: "Gagal mendapatkan data detail roles" });
                return;
            }
        })
        .error(function(err) {
            $.growl.error({ message: "Gagal Akses API >"+err });
            return;
        });
    };

    $scope.keluar = function() {
        $modalInstance.dismiss('cancel');
    };
};

var tambahRoleCtrl = function($scope, $modalInstance, $http, $cookieStore, $rootScope) {
    $scope.tampung = [];
    $scope.accessrole = [];
    $scope.newRole = new Role("");
    $scope.jenisRole = "";
    $scope.page_id = 21;

    $scope.ubahJenisRole = function(obj) {
        $scope.jenisRole = obj;
    };

    $scope.init = function() {        
        
        $rootScope.authorize(
            //itp.menu.select
            $http.post($rootScope.url_api + 'menu/select', {

            }).success(function(reply) {
                if (reply.status === 200) {
                    $scope.tampung = reply.result.data;
                    for (var p = 0; p < $scope.tampung.length; p++) {
                        $scope.tampung[p].ispermitted = false;
                        $scope.tampung[p].bisa_mengatur = "0";
                        $scope.tampung[p].bisa_tambah = "0";
                        $scope.tampung[p].bisa_ubah = "0";
                        $scope.tampung[p].bisa_hapus = "0";
                    }
                    var submenu;
                    var submenus = [];
                    var subsubmenus = [];
                    var masuk = [];
                    for (var p = 0; p < $scope.tampung.length; p++) {
                        masuk[p] = false;
                    }
                    for (var i = 0; i < $scope.tampung.length; i++) {
                        if (!($scope.tampung[i].menu_parent == 0) || masuk[i] === true)
                            continue;
                        for (var j = 0; j < $scope.tampung.length; j++) {
                            if (j === i || masuk[j] === true)
                                continue;
                            if ($scope.tampung[j].menu_parent == $scope.tampung[i].menu_id) {
                                submenu = $scope.tampung[j];
                                for (var k = 0; k < $scope.tampung.length; k++) {
                                    if (k === i || k === j || masuk[k] === true)
                                        continue;
                                    if ($scope.tampung[k].menu_parent == $scope.tampung[j].menu_id) {
                                        subsubmenus.push($scope.tampung[k]);
                                        masuk[k] = true;
                                    }
                                }
                                submenu.childs = subsubmenus;
                                subsubmenus = [];
                                submenus.push(submenu);
                                masuk[j] = true;
                            }
                        }
                        $scope.tampung[i].childs = submenus;
                        submenus = [];
                        $scope.accessrole.push($scope.tampung[i]);
                        masuk[i] = true;
                    }
                }
            }).error(function(err) {
                $.growl.error({ message: "Gagal Akses API >"+err });
                return;
            })                
        );  
    };

    $scope.addRole = function() {
        if ($scope.newRole.authority === "") {
            $.growl.warning({ message: "Nama Role belum diisi!!" });
            return;
        }
        if ($scope.jenisRole === "") {
            $.growl.warning({ message: "Jenis Role belum dipilih!!" });
            return;
        }
        var master = {};
        master.authority = $scope.newRole.authority;
        master.jenis_role = $scope.jenisRole;

        var inputElements = document.getElementsByTagName('input');
        var label;
        var arr;
        var arr2 = [];

        for (var i = 0; inputElements[i]; ++i) {
            if (inputElements[i].className === "uacheck") {
                label = inputElements[i].value;
                arr = {};
                arr.ispermitted = inputElements[i].checked;
                arr.page_id = Number(label);//label=page_id
                for (var j = 0; j < $scope.tampung.length; j++) {
                    if ($scope.tampung[j].page_id == label) {
                        arr.bisa_mengatur = ($scope.tampung[j].bisa_mengatur === '1');
                        arr.bisa_tambah = ($scope.tampung[j].bisa_tambah === '1');
                        arr.bisa_ubah = ($scope.tampung[j].bisa_ubah === '1');
                        arr.bisa_hapus = ($scope.tampung[j].bisa_hapus === '1');
                        break;
                    }
                }
                arr2.push(arr);
            }
        }
        
        $rootScope.authorize(
            //itp.role.insert
            $http.post($rootScope.url_api + 'roles/create', {
                master: master, 
                details: arr2,
                logging: {
                    username : $rootScope.userLogin,
                    user_activity_id : 13
                }
            }).success(function(reply) {
                if (reply.status === 200) {
                    alert("Berhasil membuat role");
                    $modalInstance.close();
                }
                else
                    alert("Gagal membuat role");
            }).error(function(err) {
                $.growl.error({ message: "Gagal Akses API >"+err });
                return;
            })
        );
    };

    $scope.keluar = function() {
        $modalInstance.dismiss('cancel');
    };
};

var ubahRoleDetailCtrl = function($scope, $modalInstance, $http, item, $cookieStore, $rootScope) {
    $scope.accessrole = [];
    $scope.role = item;
    $scope.tampung2 = [];
    var page_id = 21;
    $scope.jenisRole = item.jenis_role;
    
    $scope.ubahJenisRole = function(obj) {
        $scope.jenisRole = obj;
    };

    $scope.init = function() {
        $rootScope.authorize($scope.initialize());
    };

    $scope.initialize = function() {
        $rootScope.loadLoadingModal("Silahkan Tunggu...");
        //itp.menu.select
        $http.post($rootScope.url_api + 'menu/select', {
            
        }).success(function(reply) {
            if (reply.status === 200) {
                $scope.tampung = reply.result.data;
                var param = [];
                param.push(item.role_id);
                //itp.role.getDetail
                $http.get($rootScope.url_api + 'roles/detail/' + item.role_id)
                .success(function(reply2) {
                    if (reply2.status === 200) {
                        $scope.tampung2 = reply2.result.data;
                        for (var p = 0; p < $scope.tampung.length; p++) {
                            $scope.tampung[p].ispermitted = false;
                            $scope.tampung[p].bisa_mengatur = "0";
                            $scope.tampung[p].bisa_tambah = "0";
                            $scope.tampung[p].bisa_ubah = "0";
                            $scope.tampung[p].bisa_hapus = "0";
                            for (var q = 0; q < $scope.tampung2.length; q++) {
                                if ($scope.tampung[p].menu_id == $scope.tampung2[q].menu_id) {
                                    $scope.tampung[p].ispermitted = $scope.tampung2[q].ispermitted;
                                    if ($scope.tampung2[q].bisa_mengatur == true)
                                        $scope.tampung[p].bisa_mengatur = "1";
                                    else
                                        $scope.tampung[p].bisa_mengatur = "0";
                                    if ($scope.tampung2[q].bisa_tambah == true)
                                        $scope.tampung[p].bisa_tambah = "1";
                                    else
                                        $scope.tampung[p].bisa_tambah = "0";
                                    if ($scope.tampung2[q].bisa_ubah == true)
                                        $scope.tampung[p].bisa_ubah = "1";
                                    else
                                        $scope.tampung[p].bisa_ubah = "0";
                                    if ($scope.tampung2[q].bisa_hapus == true)
                                        $scope.tampung[p].bisa_hapus = "1";
                                    else
                                        $scope.tampung[p].bisa_hapus = "0";
                                    break;
                                }
                            }
                        }
                        var submenu;
                        var submenus = [];
                        var subsubmenus = [];
                        var masuk = [];
                        for (var p = 0; p < $scope.tampung.length; p++) {
                            masuk[p] = false;
                        }
                        for (var i = 0; i < $scope.tampung.length; i++) {
                            if (!($scope.tampung[i].menu_parent == 0) || masuk[i] === true)
                                continue;
                            for (var j = 0; j < $scope.tampung.length; j++) {
                                if (j === i || masuk[j] === true)
                                    continue;
                                if ($scope.tampung[j].menu_parent == $scope.tampung[i].menu_id) {
                                    submenu = $scope.tampung[j];
                                    for (var k = 0; k < $scope.tampung.length; k++) {
                                        if (k === i || k === j || masuk[k] === true)
                                            continue;
                                        if ($scope.tampung[k].menu_parent == $scope.tampung[j].menu_id) {
                                            subsubmenus.push($scope.tampung[k]);
                                            masuk[k] = true;
                                        }
                                    }
                                    submenu.childs = subsubmenus;
                                    subsubmenus = [];
                                    submenus.push(submenu);
                                    masuk[j] = true;
                                }
                            }
                            $scope.tampung[i].childs = submenus;
                            submenus = [];
                            $scope.accessrole.push($scope.tampung[i]);
                            masuk[i] = true;
                        }
                        $rootScope.unloadLoadingModal();
                    }
                }).error(function(err) {
                    $.growl.error({ message: "Gagal Akses API >"+err });
                    return;
                }); 
            }
        }).error(function(err) {
            $.growl.error({ message: "Gagal Akses API >"+err });
            return;
        }); 
    };

    $scope.updateRoleDetail = function() {
        var inputElements = document.getElementsByTagName('input');
        var label;
        var arr;
        var arr2 = [];
        var paramForInsert;
        var paramsForInsert = [];

        for (var i = 0; inputElements[i]; ++i) {
            if (inputElements[i].className === "uacheck") {
                label = inputElements[i].value;
                var page_id = Number(label);
                var accessBaruNih = true;
                for (var j = 0; j < $scope.tampung2.length; j++) {
                    if (page_id == $scope.tampung2[j].menu_id) {
                        accessBaruNih = false;
                        break;
                    }
                }
                if (accessBaruNih === false) {
                    arr = {};
                    arr.ispermitted = inputElements[i].checked;
                    arr.page_id = page_id;
                    for (var j = 0; j < $scope.tampung.length; j++) {
                        if ($scope.tampung[j].page_id == label) {
                            arr.bisa_mengatur = ($scope.tampung[j].bisa_mengatur === '1');
                            arr.bisa_tambah = ($scope.tampung[j].bisa_tambah === '1');
                            arr.bisa_ubah = ($scope.tampung[j].bisa_ubah === '1');
                            arr.bisa_hapus = ($scope.tampung[j].bisa_hapus === '1');
                            break;
                        }
                    }
                    arr2.push(arr);
                }
                else {
                    paramForInsert = {};
                    paramForInsert.ispermitted = inputElements[i].checked;
                    paramForInsert.page_id = page_id;//label=page_id
                    for (var j = 0; j < $scope.tampung.length; j++) {
                        if ($scope.tampung[j].page_id == label) {
                            paramForInsert.bisa_mengatur = ($scope.tampung[j].bisa_mengatur === '1');
                            paramForInsert.bisa_tambah = ($scope.tampung[j].bisa_tambah === '1');
                            paramForInsert.bisa_ubah = ($scope.tampung[j].bisa_ubah === '1');
                            paramForInsert.bisa_hapus = ($scope.tampung[j].bisa_hapus === '1');
                            break;
                        }
                    }
                    paramsForInsert.push(paramForInsert);
                }
            }
        }
        
        $rootScope.loadLoadingModal("Silahkan Tunggu...");
        $rootScope.authorize(
            //itp.role.update
            $http.post($rootScope.url_api + 'roles/update', {
                role: {
                    jenis_role: $scope.jenisRole,
                    role_id: item.role_id,
                    authority: item.authority
                },
                paramsUpdate: arr2,
                paramsInsert: paramsForInsert,
                logging: {
                    username : $rootScope.userLogin,
                    user_activity_id : 12
                }
            }).success(function(reply) {
                $rootScope.unloadLoadingModal();
                if (reply.status === 200) {
                    $.growl.notice({title: "[INFO]", message: "Update role berhasil"});
                    $modalInstance.close();
                }
                else
                    $.growl.error({title: "[PERINGATAN]", message: "Gagal update role"});
            }).error(function(err) {
                $.growl.error({ message: "Gagal Akses API >"+err });
                return;
            })
        );
    };

    $scope.keluar = function() {
        $modalInstance.dismiss('cancel');
    };
};

function Role(authority) {
    var self = this;
    self.authority = authority;
}