(function() {
    'use strict';

    angular.module("app")
            .controller("UploadFileConfigController", ctrl);

    ctrl.$inject = ['$http', '$translate', '$translatePartialLoader', '$location', 'SocketService', 'UploadFileConfigService', 'RoleService'];
    function ctrl($http, $translate, $translatePartialLoader, $location, SocketService, UploadFileConfigService, RoleService) {

        var vm = this;
        
        vm.totalItems = 0;
        vm.currentPage = 1;
        vm.maxSize = 10;

        vm.srcTextNamaHalaman = "";
        vm.srcTextFileType = "";
        vm.itemsByPage = 5;
        vm.userBisaMengatur;

        vm.itemFilterSisiHalaman = [];
        vm.itemNamaHalaman = [];
        vm.itemFileType = [];
        vm.sisiHalaman = {};

        vm.fileUploadConf = [];
        vm.page_id = 148;
        vm.inc;

        vm.sisiHalamanTercentang = false;
        vm.namaHalamanTercentang = false;
        vm.fileTypeTercentang = false;

        vm.init = init;
        function init() {
            $translatePartialLoader.addPart('master-konfig-fileupload');
            /*
            $rootScope.getSession().then(function(result){
                $rootScope.userSession = result.data.data;
                $rootScope.userLogin = $rootScope.userSession.session_data.username;
                userbisangatur();
                listfilter();
                loadFileUploadConfig();
            });
            */
            userbisangatur();
            listfilter();
            loadFileUploadConfig();
        };

        function userbisangatur() {
            RoleService.RoleService({
                    username: $rootScope.userLogin,  
                    page_id: vm.page_id, 
                    jenis_mengatur: 1
                }, function(reply){
                    if(reply.status === 200){
                        if(reply.result.data.length > 0){
                            var data = reply.result.data[0];
                            vm.userBisaMengatur = $rootScope.strtobool(data.bisa_mengatur);
                        }
                    }
                    else{
                        $.growl.error({ message: "Gagal mendapatkan data hak akses!" });
                    }
                }, function(err) {
                    /*
                        $http.post($rootScope.url_api + "logging", {
                            message: "Tidak berhasil akses API : " + JSON.stringify(err),
                            source: "pajak.js - rekanan/cekBisaMengubahData"
                        }) 
                        .then(function(response){

                        });
                    */
                    $.growl.error({ message: "Gagal Akses API >"+err });
                }
            );
        }

        function listfilter() {
            UploadFileConfigService.selectpagelayer(
                function(reply){
                    if(reply.status === 200){
                        if(reply.result.data.length > 0){
                            var data = reply.result.data;
                            vm.itemFilterSisiHalaman = data;
                        }
                    }
                    else{
                        $.growl.error({ message: "Gagal mendapatkan data page layer!" });
                    }
                }, function(err) {
                    /*
                    $http.post($rootScope.url_api + "logging", {
                                message: "Tidak berhasil akses API : " + JSON.stringify(err),
                                source: "pajak.js - rekanan/cekBisaMengubahData"
                          }) 
                          .then(function(response){
                              // do nothing
                              // don't have to feedback
                          });
                    */
                    $.growl.error({ message: "Gagal Akses API >"+err });
                }
            );

            UploadFileConfigService.selectpagename(
                function(reply){
                    if(reply.status === 200){
                        if(reply.result.data.length > 0){
                            var data = reply.result.data;
                            vm.itemFilterNamaHalaman = data;
                        }
                    }
                    else{
                        $.growl.error({ message: "Gagal mendapatkan data page layer!" });
                    }
                }, function(err) {
                    /*
                            message: "Tidak berhasil akses API : " + JSON.stringify(err),
                            source: "pajak.js - rekanan/cekBisaMengubahData"
                      }) 
                      .then(function(response){
                          // do nothing
                          // don't have to feedback
                      });
                    */
                    $.growl.error({ message: "Gagal Akses API >"+err });
                }
            );

            UploadFileConfigService.selecttypefile(
                function(reply){
                    if(reply.status === 200){
                        if(reply.result.data.length > 0){
                            var data = reply.result.data;
                            vm.itemFileType = data;
                        }
                    }
                    else{
                        $.growl.error({ message: "Gagal mendapatkan data page layer!" });
                    }
                }, function(err) {
                    /*
                        $http.post($rootScope.url_api + "logging", {
                            message: "Tidak berhasil akses API : " + JSON.stringify(err),
                            source: "pajak.js - rekanan/cekBisaMengubahData"
                      }) 
                      .then(function(response){
                          // do nothing
                          // don't have to feedback
                      });
                    */
                    $.growl.error({ message: "Gagal Akses API >"+err });
                }
            );
        }

        function loadFileUploadConfig(current) {
            $rootScope.loadLoading("Silahkan Tunggu...");
            vm.currentPage = 1;
            UploadFileConfigService.count({   
                    srcTextNamaHalaman: "%" + vm.srcTextNamaHalaman.page_name + "%",
                    srcTextFileType: "%" + vm.srcTextFileType.file_type_name + "%",
                    sisiHalamanTercentang: vm.sisiHalamanTercentang,
                    namaHalamanTercentang: vm.namaHalamanTercentang,
                    fileTypeTercentang: vm.fileTypeTercentang,
                    sisiHalaman: vm.sisiHalaman
                }, function(reply){
                    $rootScope.unloadLoading();
                    if(reply.status === 200){
                        var data = reply.result.data;
                        vm.totalItems = data;
                    }
                    else{
                        $.growl.error({ message: "Gagal mendapatkan jumlah data tipe file!!" });
                    }
                }, function(err) {
                    $.growl.error({ message: "Gagal Akses API >"+err });
                    $rootScope.unloadLoading();
                }
            );
            var offset;
            if(current === undefined){ offset = 0; }
            else{ offset = (current * 10) - 10;}
            var limit = 10;
            UploadFileConfigService.select({
                    srcTextNamaHalaman: "%" + vm.srcTextNamaHalaman.page_name + "%",
                    srcTextFileType: "%" + vm.srcTextFileType.file_type_name + "%",
                    sisiHalamanTercentang: vm.sisiHalamanTercentang,
                    namaHalamanTercentang: vm.namaHalamanTercentang,
                    fileTypeTercentang: vm.fileTypeTercentang,
                    sisiHalaman: vm.sisiHalaman,
                    offset: offset,
                    limit: limit
                }, function(reply){
                    $rootScope.unloadLoading();
                    if(reply.status === 200){
                        vm.fileUploadConf = reply.result.data;
                        if (vm.fileUploadConf.length > 0) {
                            for (var i = 0; i < vm.fileUploadConf.length; i++) {
                                vm.fileUploadConf[i].listFileType = [];
                            }
                            var arr1;
                            var arr2 = [];
                            for (var i = 0; i < vm.fileUploadConf.length; i++) {
                                arr1 = [];
                                arr1.push(vm.fileUploadConf[i].id_page_config);
                                arr2.push(arr1);
                            }
                            UploadFileConfigService.cektype({ 
                                param: arr2
                            }, function(reply2){
                                if(reply2.status === 200){
                                    vm.listFileType = reply2.result.data;
                                    for (var i = 0; i < vm.fileUploadConf.length; i++) {
                                        for (var j = 0; j < vm.listFileType.length; j++) {
                                            if (Number(vm.fileUploadConf[i].id_page_config) === Number(vm.listFileType[j].id_page_config)) {
                                                vm.fileUploadConf[i].listFileType.push(vm.listFileType[j]);
                                            }
                                        }
                                    }
                                }
                            }, function(err) {
                                $.growl.error({ message: "Gagal Akses API >"+err });
                                $rootScope.unloadLoading();
                            });
                        }
                    }
                    else{
                        $.growl.error({ message: "Gagal mendapatkan data tipe file!!" });
                    }
                }, function(err) {
                    $.growl.error({ message: "Gagal Akses API >"+err });
                    $rootScope.unloadLoading();
                }
            );
        }

        vm.jLoad = jLoad;
        function jLoad(current) {
            vm.currentPage = current;
            loadFileUploadConfig(current);

        };

        vm.set = set;
        function set(obj) {
            vm.sisiHalaman = obj;
        };

        vm.cariConfig = cariConfig;
        function cariConfig() {
            userbisangatur();
            loadFileUploadConfig();
            listfilter();
        };

        vm.tambah = tambah;
        function tambah() {
            var modalInstance = $modal.open({
                templateUrl: 'aturKonfigurasi.html',
                controller: aturKonfigurasiCtrl
            });
            modalInstance.result.then(function() {
                loadFileUploadConfig();
                listfilter();
            });
        };

        vm.ubah = ubah;
        function ubah(idConfig) {
            var modalInstance = $modal.open({
                templateUrl: 'ubahMaxSize.html',
                controller: ubahMaxSizeCtrl,
                resolve: {
                    item: function() {
                        return idConfig;
                    }
                }
            });
            modalInstance.result.then(function() {
                loadFileUploadConfig();
                listfilter();
            });
        };
    }
})();

//TODO
var aturKonfigurasiCtrl = function($scope, $modalInstance, $http, $cookieStore, $rootScope) {
    var page_id = 148;
    $scope.webpage;
    $scope.type = [];
    $scope.filesType = [];
    $scope.webpages = [];
    $scope.side;
    $scope.posisiLayer;
    $scope.id_page_config = 1;
    $scope.pageList = false;
    $scope.typeList = false;

    $scope.init = function() {
        $scope.pageList = false;
        loadWebPages();
    };

    $scope.gantiLayer = function(obj) {
        $scope.webpage = undefined;
        $scope.side = obj;
        if (obj === '0') {
            $scope.posisiLayer = "RE";
            loadWebPages();
        }
        else if (obj === '1') {
            $scope.posisiLayer = "BE";
            loadWebPages();
        }

        if ($scope.webpage === undefined || $scope.webpage === '') {
            $.growl.warning({title: "[ATTENTION!]", message: "Silakan pilih salah satu halaman terlebih dahulu"});
            $scope.typeList = false;
            $scope.pageList = true;
        }
        else {
            $scope.typeList = true;
        }
    };

    function loadWebPages() {
        $rootScope.loadLoadingModal("Silahkan Tunggu...");
        var param = [];
        param.push($scope.posisiLayer);
        $http.post($rootScope.url_api+"fileconf/selectActiveFile",{param : param})
        .success(function(reply){
            $rootScope.unloadLoadingModal();
            if(reply.status === 200){
                $scope.webpages = reply.result.data;
            }
            else{
                $.growl.error({ message: "Gagal mendapatkan data hak akses!" });
                return;
            }
        })
        .error(function(err) {
            $rootScope.unloadLoadingModal();
            $.growl.error({ message: "Gagal Akses API >"+err });
            return;
        });        
    };

    $scope.gantiHalaman = function(obj) {
        $scope.webpage = obj;

        if ($scope.webpage !== undefined || $scope.webpage !== '') {
            $.growl.warning({title: "[ATTENTION!]", message: "Centang file type yang di inginkan"});
            $scope.typeList = true;
        }
        else {
            $scope.typeList = false;
        }
        loadFileType();
    };

    function loadFileType() {
        $rootScope.loadLoadingModal("Silahkan Tunggu...");
        var param = [];
        param.push($scope.webpage.id_page_config);
        $http.get($rootScope.url_api+"fileconf/selectAllFileType")
        .success(function(reply){
            $rootScope.unloadLoadingModal();
            //console.info("rep: "+JSON.stringify(reply));
            if(reply.status === 200){
                $scope.filesType = reply.result.data;
                $http.post($rootScope.url_api+"fileconf/selectAllFilePage",{ param: param})
                .success(function(reply2){
                    //console.info(JSON.stringify(reply2));
                    if(reply.status === 200){
                        $scope.type = reply2.result.data;
                        for (var i = 0; i < $scope.filesType.length; i++) {
                            $scope.filesType[i].flag_active = false;
                            for (var j = 0; j < $scope.type.length; j++) {
                                if (Number($scope.filesType[i].id_file_type) === Number($scope.type[j].id_file_type)) {
                                    $scope.filesType[i].flag_active = $rootScope.strtobool($scope.type[j].flag_active);
                                    break;
                                }
                            }
                        }
                    }
                    else{
                        $.growl.error({ message: "Gagal mendapatkan data hak akses!" });
                        return;
                    }
                })
                .error(function(err) {
                    $rootScope.unloadLoadingModal();
                    $.growl.error({ message: "Gagal Akses API >"+err });
                    return;
                });
            }
            else{
                $.growl.error({ message: "Gagal mendapatkan data hak akses!" });
                return;
            }
        })
        .error(function(err) {
            $rootScope.unloadLoadingModal();
            $.growl.error({ message: "Gagal Akses API >"+err });
            return;
        });
    };

    $scope.saveConfig = function() {
        simpanConfig();
    };

    function simpanConfig() {
        var inputElements = document.getElementsByTagName('input');
        var label;
        var arr;
        var arr2 = [];
        var paramForInsert;
        var paramsForInsert = [];
        for (var i = 0; inputElements[i]; ++i) {
            if (inputElements[i].className === "uacheck") {
                label = inputElements[i].value;
                var id_file_type = Number(label);
                var configBaruNih = true;
                for (var j = 0; j < $scope.type.length; j++) {
                    if (id_file_type === Number($scope.type[j].id_file_type)) {
                        configBaruNih = false;
                        break;
                    }
                }
                if (configBaruNih === false) {
                    arr = [];
                    arr.push(inputElements[i].checked);
                    arr.push(Number($scope.webpage.id_page_config));
                    arr.push(Number(id_file_type));
                    /*arr.push($scope.posisiLayer);*/
                    arr2.push(arr);
                }
                else {
                    paramForInsert = [];
                    paramForInsert.push(Number($scope.webpage.id_page_config));
                    paramForInsert.push(Number(id_file_type));
                    paramForInsert.push(inputElements[i].checked);
                    /*paramForInsert.push($scope.posisiLayer);*/
                    paramsForInsert.push(paramForInsert);

                }
            }
        }
        if (arr2.length > 0) {
            $rootScope.loadLoadingModal("Silahkan Tunggu...");
            $http.post($rootScope.url_api+"fileconf/updateFileConf",{username: $rootScope.userLogin, param: arr2,
                page_id: page_id})
            .success(function(reply){
                //console.info("update: "+JSON.stringify(arr2));
                $rootScope.unloadLoadingModal();
                if(reply.status === 200){
                    $.growl.notice({title: "[SELAMAT!!]", message: "Perubahan konfigurasi upload file berhasil"});
                    $modalInstance.close();
                }
            })
            .error(function(err) {
                $rootScope.unloadLoadingModal();
                $.growl.error({ message: "Gagal Akses API >"+err });
                return;
            });
        }

        if (paramsForInsert.length > 0) {
            $rootScope.loadLoadingModal("Silahkan Tunggu...");
            $http.post($rootScope.url_api+"fileconf/insertFileConf",{username: $rootScope.userLogin, param: paramsForInsert,
                page_id: page_id})
            .success(function(reply){
                //console.info("insert: "+JSON.stringify(paramForInsert));
                $rootScope.unloadLoadingModal();
                if(reply.status === 200){
                    $.growl.notice({title: "[SELAMAT!!]", message: "Penambahan konfigurasi upload file berhasil"});
                    $modalInstance.close();
                }
            })
            .error(function(err) {
                $rootScope.unloadLoadingModal();
                 $http.post($rootScope.url_api + "logging", {
                        message: "Tidak berhasil akses API : " + JSON.stringify(err),
                        source: "pajak.js - rekanan/cekBisaMengubahData"
                  }) 
                  .then(function(response){
                      // do nothing
                      // don't have to feedback
                  });
                $.growl.error({ message: "Gagal Akses API >"+err });
                return;
            });
        }
    }

    $scope.batal = function() {
        $modalInstance.dismiss('cancel');
    };
};

//TODO
var ubahMaxSizeCtrl = function($scope, $modalInstance, $http, item, $rootScope) {
    $scope.maxFileSize = item.limit_size;
    $scope.id_page_config = item.id_page_config;

    $scope.data_config = new config_file_filter(item.id_page_config, item.limit_size);

    $scope.updateFileSize = function() {
        $rootScope.loadLoadingModal("Silahkan Tunggu...");
            $http.post($rootScope.url_api+"fileconf/updateLimitSize",{username: $rootScope.userLogin, 
                limit_size: $scope.data_config.limit_size, id_page_config: $scope.id_page_config})
            .success(function(reply){
                $rootScope.unloadLoadingModal();
                if(reply.status === 200){
                    $.growl.notice({title: "[INFO]", message: "Berhasil mengubah ukuran file maximal!!"});
                    $modalInstance.close();
                }
            })
            .error(function(err) {
                $rootScope.unloadLoadingModal();
                $http.post($rootScope.url_api + "logging", {
                        message: "Tidak berhasil akses API : " + JSON.stringify(err),
                        source: "pajak.js - rekanan/cekBisaMengubahData"
                  }) 
                  .then(function(response){
                      // do nothing
                      // don't have to feedback
                  });
                $.growl.error({ message: "Gagal Akses API >"+err });
                return;
            });
    };

    $scope.cancel = function() {
        $modalInstance.dismiss('cancel');
    };
};

function config_file_filter(id_page_config, limit_size) {
    var self = this;
    self.id_page_config = id_page_config;
    self.limit_size = limit_size;
}
;